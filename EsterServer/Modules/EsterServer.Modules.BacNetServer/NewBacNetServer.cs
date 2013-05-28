using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Timers;
using System.Web;
using BacNetApi;
using BacNetApi.Extensions;
using BACsharp;
using EsterCommon.Exceptions;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Interfaces;
using EsterServer.Model.Aspects;
using EsterServer.Model.Data;
using EsterServer.Model.Ioc;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Nini.Config;
using NLog;
using PlansDb;
using Address = PlansDb.Address;
using Timer = System.Timers.Timer;

namespace EsterServer.Modules.BacNetServer
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[UnityServiceBehavior]
	public class NewBacNetServer : IBacNetServer, IEsterServerModule
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private readonly XmlConfigSource _configSource =
			new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };

		public static BacNet Network;
		public BacNetServerStates State { get; set; }
		public string StateDescription { get; set; }

		private readonly List<BaseObject> _rootObjects;
		private readonly Dictionary<int, BaseObject> _planObjects;
		private readonly Dictionary<string, List<Address>> _addresses;
		private readonly List<BaseObject> _changedValues;

		private readonly Timer _pushValuesToClientTimer;
		private readonly PlansDc _plansDb;
		private readonly IUnityContainer _container;

		public NewBacNetServer(IUnityContainer container)
		{
			_container = container;

			_addresses = new Dictionary<string, List<Address>>();
			_planObjects = new Dictionary<int, BaseObject>();
			_changedValues = new List<BaseObject>();

			_pushValuesToClientTimer = new Timer(5000);
			_pushValuesToClientTimer.Elapsed += PushValuesToClientTimerTick;

			_plansDb = new PlansDc(@"Data Source=192.168.0.160\SQLEXPRESS;Initial Catalog=Ester;Persist Security Info=True;User ID=sa;Password=6*!vb9%q");

			State = BacNetServerStates.Initializing;

			_rootObjects = _plansDb.PlanObjects.Where(po => po.Parent == null).Select(p => BaseObject.FromDbObject(p)).ToList();
			FillPlanObjects(_planObjects, _rootObjects);

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

			State = BacNetServerStates.Normal;

			foreach (var group in _plansDb.Addresses.GroupBy(g => g.Path))
			{
				if (!group.Key.Contains('.'))
					continue;

				var objAddr = group.Key.Split('.')[1].Trim();
				uint instance;
				instance = uint.TryParse(group.Key.Split('.')[0].Trim(), out instance) ? instance : 0;

				if (instance != 0 && !string.IsNullOrEmpty(objAddr))
					Network[instance].Objects[objAddr].ValueChangedEvent += OnBacnetValueChanged;

				_addresses.Add(group.Key, group.ToList());
			}
			_pushValuesToClientTimer.Start();
		}

		private void FillPlanObjects(Dictionary<int, BaseObject> planObjects, IEnumerable<BaseObject> rootObjects)
		{
			foreach (var rootObject in rootObjects)
			{
				planObjects.Add(rootObject.Id, rootObject);
				if (rootObject is IContainerObject)
					FillPlanObjects(planObjects, ((IContainerObject)rootObject).Children);
			}
		}

		#region Client requests

		#region Get values

		[Log("Получение объектов планов")]
		[ServerExceptionAspect]
		public Stream GetPlanObjects()
		{
			var myResponseBody = JsonConvert.SerializeObject(_rootObjects, Formatting.Indented);
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}
		
		#region OLD

		[ServerExceptionAspect]
		public Stream GetDevices()
		{
			var res = Network.SubscribedDevices.Select
			(
				d => new
				{
					d.Title,
					d.Id,
					d.LastUpdated,
					Status = d.Status.GetStringValue(),
					SubscribedObjects = d.Objects.ToList(),
					Objects = d.ObjectList
				}
			);

			var myResponseBody = JsonConvert.SerializeObject(res);
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}

		[Log("Чтение значений всех опрашиваемых объектов BACnet")]
		[ServerExceptionAspect]
		public Stream GetAllSensorsValues()
		{
			var myResponseBody = "nya";//JsonConvert.SerializeObject(_allValues);
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

		#endregion

		#region Set value

		public void SetBacnetvalue(BaseObject data)
		{

		}


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

		private void PushValuesToClients()
		{
			ValuesPusherComet.SetEvent(new List<BaseObject>(_changedValues));
			_changedValues.Clear();
		}

		private void OnBacnetValueChanged(string address, string value)
		{
			// log event
			var myEvent = new LogEventInfo(LogLevel.Info, "BAChistory", "");
			myEvent.Properties.Add("address", address);
			myEvent.Properties.Add("value", value);
			Logger.Log(myEvent);

			if (_addresses.ContainsKey(address))
				foreach (var dbAddress in _addresses[address])
				{
					if (!_planObjects.ContainsKey(dbAddress.ObjectId))
						continue;

					var doubleValue = double.Parse(value);

					switch (dbAddress.AddressTypeId)
					{
						case 2:
							((TemperatureSensor)_planObjects[dbAddress.ObjectId]).Temperature = doubleValue;
							break;
						default:
							return;
					}

					if (!_changedValues.Contains(_planObjects[dbAddress.ObjectId]))
						_changedValues.Add(_planObjects[dbAddress.ObjectId]);
				}

			if (_changedValues.Count >= 100)
				PushValuesToClients();
		}
	}
}
