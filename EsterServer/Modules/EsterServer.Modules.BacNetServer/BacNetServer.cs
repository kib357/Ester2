using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web;
using BACsharp;
using BacNetApi;
using BacNetApi.Extensions;
using EsterCommon.Exceptions;
using EsterServer.Model.Aspects;
using EsterServer.Model.Data;
using EsterServer.Model.Ioc;
using EsterServer.Modules.BacNetServer.Alarms;
using EsterServer.Modules.BacNetServer.Notifications;
using EsterServer.Modules.BacNetServer.Schedules;
using Microsoft.Practices.Unity;
using NLog;
using Newtonsoft.Json;
using Nini.Config;
using Timer = System.Timers.Timer;

namespace EsterServer.Modules.BacNetServer
{
	public delegate void ValuesChangedEventHandler(string address, string oldValue, string newValue);

	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[UnityServiceBehavior]
	public class BacNetServer : IBacNetServer, IEsterServerModule
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private readonly XmlConfigSource _configSource =
			new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };

		public static BacNet Network;
		public static event ValuesChangedEventHandler ValuesChanged;
		public BacNetServerStates State { get; set; }
		public string StateDescription { get; set; }

		private static Dictionary<string, string> _allValues;
		private static Dictionary<string, string> _changedValues;
		private readonly Timer _pushValuesToClientTimer;
		private readonly EsterClassesDataContext _dataContext;
		private readonly IUnityContainer _container;

		public Stream GetPlanObjects()
		{
			return null;
		}

		public BacNetServer(IUnityContainer container)
		{
			State = BacNetServerStates.NotStarted;
			_container = container;
			_dataContext = new EsterClassesDataContext();
			_pushValuesToClientTimer = new Timer(5000);
			_pushValuesToClientTimer.Elapsed += PushValuesToClientTimerTick;
			InitializeModule();
		}

		private void InitializeModule()
		{
			State = BacNetServerStates.Initializing;
			_allValues = new Dictionary<string, string>();
			_changedValues = new Dictionary<string, string>();

			try
			{
				Network = new BacNet(_configSource.Configs["BacNet"].Get("Ip"));
			}
			catch (Exception)
			{
				State = BacNetServerStates.Fault;
				StateDescription = "Cannot initialize BACnet provider, check ip address configuration.";
				return;
			}

			SubscribeToDatabaseObjects();
			_pushValuesToClientTimer.Start();

			//_container.RegisterInstance(_container.Resolve<AlarmChecker>());
			_container.RegisterInstance(_container.Resolve<NotificationListener>());
			_container.RegisterInstance(_container.Resolve<SchedulesService>());
			State = BacNetServerStates.Normal;
		}

		#region Client requests

		#region Get values

		[ServerExceptionAspect]
		public Stream GetDevices()
		{
			var res = Network.SubscribedDevices.Select(
					d => new
					{
						d.Title,
						d.Id,
						d.LastUpdated,
						Status = d.Status.GetStringValue(),
						SubscribedObjects = d.Objects.ToList(),
						Objects = d.ObjectList
					});
			var myResponseBody = JsonConvert.SerializeObject(res);
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}

		[Log("Чтение значений всех опрашиваемых объектов BACnet")]
		[ServerExceptionAspect]
		public Stream GetAllSensorsValues()
		{
			var myResponseBody = JsonConvert.SerializeObject(_allValues);
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}

		[Log("Чтение текущего значения объекта BACnet")]
		[ServerExceptionAspect]
		public string GetPresentValueProperty(string deviceAddress, string objectAddress)
		{
			uint instance;
			if (uint.TryParse(deviceAddress, out instance))
			{
				var value = Network[instance].Objects[objectAddress].Get();
				if (value != null)
					return value.ToString();
			}
			throw new BadRequestException();
		}

		[Log("Чтение истории объекта BACnet")]
		[ServerExceptionAspect]
		public Stream GetPresentValueHistory(string deviceAddress, string objectAddress)
		{
			return GetPresentValueHistoryInternal(deviceAddress, objectAddress);
		}

		[ServerExceptionAspect]
		public Stream GetPresentValueHistoryWithEqualPeriods(string deviceAddress, string objectAddress, string frequency)
		{
			return GetPresentValueHistoryInternal(deviceAddress, objectAddress, "", "", frequency);
		}

		[ServerExceptionAspect]
		public Stream GetPresentValueHistoryWithCalculatedEqualPeriods(string deviceAddress, string objectAddress)
		{
			return GetPresentValueHistoryInternal(deviceAddress, objectAddress, "", "", "auto");
		}

		[ServerExceptionAspect]
		public Stream GetPresentValueHistoryFilteredByDate(string deviceAddress, string objectAddress, string startDate, string endDate)
		{
			return GetPresentValueHistoryInternal(deviceAddress, objectAddress, startDate, endDate);
		}

		[ServerExceptionAspect]
		public Stream GetPresentValueHistoryWithEqualPeriodsFilteredByDate(string deviceAddress, string objectAddress, string frequency, string startDate, string endDate)
		{
			return GetPresentValueHistoryInternal(deviceAddress, objectAddress, startDate, endDate, frequency);
		}

		[ServerExceptionAspect]
		public Stream GetPresentValueHistoryWithCalculatedEqualPeriodsFilteredByDate(string deviceAddress, string objectAddress, string startDate, string endDate)
		{
			return GetPresentValueHistoryInternal(deviceAddress, objectAddress, startDate, endDate, "auto");
		}

		private static Stream GetPresentValueHistoryInternal(string deviceAddress, string objectAddress,
																string startDate = "", string endDate = "",
																string frequency = "")
		{
			var address = deviceAddress + "." + objectAddress;
			using (var context = new EsterClassesDataContext())
			{
				var data = context.Histories.Where(h => h.Address == address);
				if (startDate != "")
				{
					data = data.Where(d => d.TimeStamp > DateTime.Parse(startDate));
				}
				if (endDate != "")
				{
					data = data.Where(d => d.TimeStamp <= EndOfDay(DateTime.Parse(endDate)));
				}
				var res = new Dictionary<DateTime, string>();

				if (data.Count() > 0)
				{
					foreach (var historyItem in data)
					{
						if (!res.ContainsKey(historyItem.TimeStamp))
							res.Add(historyItem.TimeStamp, historyItem.Value);
					}
					if (frequency != "")
					{
						TimeSpan timeSpan;

						if (frequency == "auto")
						{
							var minValue = res.Keys.Min();
							var maxValue = res.Keys.Max();
							var daysDifference = (maxValue - minValue).Days;

							if (daysDifference <= 1)
							{
								timeSpan = new TimeSpan(0, 0, 30, 0);
							}
							else if (daysDifference <= 8)
							{
								timeSpan = new TimeSpan(0, 1, 0, 0);
							}
							else if (daysDifference > 8)
							{
								timeSpan = new TimeSpan(0, 12, 0, 0);
							}
							else
							{
								timeSpan = new TimeSpan(0, 0, 30, 0);
							}
						}
						else
						{
							timeSpan = new TimeSpan(0, 0, int.Parse(frequency), 0);
						}

						var t = JsonConvert.SerializeObject(timeSpan);
						t = HttpUtility.UrlEncode(t);

						res = MakeEqualTimePeriods(res, timeSpan);
					}
				}

				var myResponseBody = JsonConvert.SerializeObject(res);
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
				return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
			}
		}

		private static DateTime EndOfDay(DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
		}

		private static Dictionary<DateTime, string> MakeEqualTimePeriods(Dictionary<DateTime, string> dictionary,
																		 TimeSpan frequency)
		{
			if (frequency.Days == 0 && frequency.Hours == 0 && frequency.Minutes == 0 && frequency.Seconds == 0)
			{
				return dictionary;
			}

			var result = new Dictionary<DateTime, string>();

			var from = dictionary.Keys.Min();
			var to = from + frequency;

			var periodValues = 0d;
			var periodParts = 0;
			foreach (var entry in dictionary)
			{
				if (entry.Key <= to)
				{
					periodValues += double.Parse(entry.Value);
					periodParts++;
				}
				else
				{
					if (periodParts == 0)
					{
						result.Add(from, entry.Value);
					}
					else
					{
						var averageValue = periodValues / periodParts;
						result.Add(from, averageValue.ToString(CultureInfo.InvariantCulture));
					}

					periodValues = 0d;
					periodParts = 0;
					from = to;
					while (to < entry.Key)
					{
						to += frequency;
					}
				}
			}

			return result;
		}

		[Log("Чтение свойства объекта BACnet")]
		[ServerExceptionAspect]
		public string GetCustomPropertyValue(string deviceAddress, string objectAddress, string propertyName)
		{
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotImplemented;
			return string.Empty;
		}

		#endregion

		#region Set value

		[Log("Запись текущего значения объекта BACnet")]
		[ServerExceptionAspect]
		public void SetPresentValueProperty(string deviceAddress, string objectAddress, Stream stream)
		{
			var reader = new StreamReader(stream);
			string data = reader.ReadToEnd();
			var value = JsonConvert.DeserializeObject<string>(data);

			WriteValue(deviceAddress, objectAddress, value);
		}

		[Log("Запись свойства объекта BACnet")]
		[ServerExceptionAspect]
		public void SetCustomPropertyValue(string deviceAddress, string objectAddress, string propertyName, Stream stream)
		{
			var reader = new StreamReader(stream);
			var data = reader.ReadToEnd();
			var value = JsonConvert.DeserializeObject<string>(data);

			WriteValue(deviceAddress, objectAddress, value, propertyName);
		}

		[Log("Запись значений нескольких объектов BACnet")]
		[ServerExceptionAspect]
		public void SetSeveral(Stream stream)
		{
			//var reader = new StreamReader(stream);
			//var data = reader.ReadToEnd();
			//var value = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

			//var devWithObjValues = new List<DeviceWithObjValues>();
			//foreach (var val in value)
			//{
			//	var obj = val.Key.Split('.');
			//	var isContains = false;
			//	foreach (var dev in devWithObjValues.Where(dev => dev.DeviceId == obj[0]))
			//	{
			//		var values = new Dictionary<BacnetPropertyId, object> { { BacnetPropertyId.PresentValue, val.Value } };
			//		dev.ObjValues.Add(obj[1], values);
			//		isContains = true;
			//	}
			//	if (isContains) continue;
			//	var propWithValues = new Dictionary<BacnetPropertyId, object> { { BacnetPropertyId.PresentValue, val.Value } };
			//	var devWithValues = new DeviceWithObjValues
			//							{
			//								DeviceId = obj[0],
			//								ObjValues =
			//									new Dictionary<string, Dictionary<BacnetPropertyId, object>> { { obj[1], propWithValues } }
			//							};
			//	devWithObjValues.Add(devWithValues);
			//}
			//foreach (var deviceWithObjValues in devWithObjValues)
			//{
			//	WriteSeveralValues(deviceWithObjValues.DeviceId, deviceWithObjValues.ObjValues);
			//	Thread.Sleep(100);
			//}
		}

		#endregion

		#endregion

		private void WriteValue(string deviceAddress, string objectAddress, string value, string propertyId = "")
		{
			var property = BacnetPropertyId.PresentValue;
			if (!string.IsNullOrEmpty(propertyId))
				Enum.TryParse(propertyId, out property);
			uint instance;
			if (!uint.TryParse(deviceAddress, out instance)) return;
			Network[instance].Objects[objectAddress].Set(value, property);
		}

		private void WriteSeveralValues(string deviceAddress, Dictionary<string, Dictionary<BacnetPropertyId, object>> values)
		{
			uint instance;
			if (!uint.TryParse(deviceAddress, out instance)) return;
			Network[instance].WritePropertyMultiple(values);
		}

		private void PushValuesToClientTimerTick(object sender, ElapsedEventArgs e)
		{
			if (_changedValues.Count > 0)
				PushValuesToClients();
			_pushValuesToClientTimer.Start();
		}

		private static void PushValuesToClients()
		{
			//ValuesPusherComet.SetEvent(_changedValues);
			//_changedValues = new Dictionary<string, string>();
		}

		private static void OnBacnetValueChanged(string address, string value)
		{
			var myEvent = new LogEventInfo(LogLevel.Info, "BAChistory", "");
			myEvent.Properties.Add("address", address);
			myEvent.Properties.Add("value", value);
			Logger.Log(myEvent);
			if (_allValues.ContainsKey(address))
			{
				if (ValuesChanged != null)
					ValuesChanged(address, _allValues[address], value);
				_allValues[address] = value;
			}
			else
			{
				if (ValuesChanged != null)
					ValuesChanged(address, null, value);
				_allValues.Add(address, value);
			}
			if (_changedValues.ContainsKey(address))
				_changedValues[address] = value;
			else
				_changedValues.Add(address, value);
			if (_changedValues.Count >= 100)
				PushValuesToClients();
		}

		#region Get bacnet adresses list
		private void SubscribeToDatabaseObjects()
		{
			foreach (var dictionary in _dataContext.Dictionaries.Select(d => d.Data))
			{
				foreach (var descendant in dictionary.Descendants())
				{
					foreach (var xAttribute in descendant.Attributes().Where(x => x.Name.LocalName.ToLower().Contains("address")))
					{
						var addrList = xAttribute.Value.Split(',');
						foreach (var address in addrList)
						{
							SubscribeToBacnetCOV(address);
						}
					}
				}
			}
		}

		public static void SubscribeToBacnetCOV(string address)
		{
			if (string.IsNullOrWhiteSpace(address) || !address.Contains('.')) return;

			uint instance;
			if (!uint.TryParse(address.Split('.')[0].Trim(), out instance)) return;
			var objAddress = address.Split('.')[1].Trim();
			if (!_allValues.ContainsKey(address))
			{
				Network[instance].Objects[objAddress].ValueChangedEvent += OnBacnetValueChanged;
			}
		}
		#endregion
	}

	
}
