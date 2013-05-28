using EsterCommon.BaseClasses;
using EsterCommon.Data;
using EsterCommon.Enums;
using EsterCommon.Events;
using EsterCommon.Exceptions;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;
using EsterCommon.Services;
using EsterServer.Model.Extensions;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Nini.Config;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Timers;
using System.Web;
using Timer = System.Timers.Timer;

namespace EsterServer.Modules.Data
{
	public delegate void DataReceivedFromNetworkEventHandler(Dictionary<string, string> values);
	public delegate void DataSavedToNetworkEventHandler(Dictionary<string, string> values, bool success);
	public delegate void DataReceivedFromClientEventHandler(Dictionary<string, string> values);


	/// <summary>
	/// Класс отвечающий за обмен данными с контроллерами и исполняющими устройствами
	/// </summary>
	// ReSharper disable ClassNeverInstantiated.Global
	public class DataManager
	// ReSharper restore ClassNeverInstantiated.Global
	{
		/// <summary>
		/// Интервал между отправкой значений подписчикам, в миллисекундах
		/// </summary>
		private const double PushValuesInterval = 5000;

		/// <summary>
		/// Строка подключения к бд
		/// </summary>
		private readonly string _esterConnectionString = ConfigurationManager.ConnectionStrings["Ester"].ConnectionString;

		/// <summary>
		/// Корневые контейнеры планов
		/// </summary>
		private List<BaseObject> _rootObjects;

		/// <summary>
		/// Полный список объектов на планах
		/// </summary>
		private readonly Dictionary<int, BaseObject> _planObjects;

		/// <summary>
		/// Словарь адресов и свойств, на них завязанных
		/// </summary>
		private readonly Dictionary<string, List<Property>> _addresses;

		/// <summary>
		/// Свойства, по которым пришло уведомление об изменении
		/// </summary>
		private readonly List<BaseObject> _changedValues;

		/// <summary>
		/// Таймер для периодической отправки измененных объектов подписчикам
		/// </summary>
		private readonly Timer _pushValuesToClientTimer;

		/// <summary>
		/// Провайдеры данных
		/// </summary>
		private readonly List<IDataProvider> _providers;

		/// <summary>
		/// Корневые объекты на планах
		/// </summary>
		public List<BaseObject> RootObjects { get { return _rootObjects; } }

		public DataManager(IUnityContainer container)
		{
			// подписываемся на событие обновления планов через редактор
            var eventAggregator = container.Resolve<EventAggregator>();
			var eve = eventAggregator.GetEvent<PlansModifiedEvent>();
			eve.Subscribe(OnPlansModified);

			// инициализируем переменные
			_addresses = new Dictionary<string, List<Property>>();
			_planObjects = new Dictionary<int, BaseObject>();
			_providers = new List<IDataProvider>();
			_changedValues = new List<BaseObject>();

			_pushValuesToClientTimer = new Timer(PushValuesInterval) { AutoReset = true };
			_pushValuesToClientTimer.Elapsed += PushValuesToClientTimerOnElapsed;

			using (var plansDb = new PlansDc(_esterConnectionString))
			{
				// получаем необходимые значения из бд
				_rootObjects = plansDb.PlanObjects.Where(po => po.Parent == null).Select(p => ServerExtensions.FromDbObject(p)).ToList();
				foreach (var group in plansDb.Properties.GroupBy(g => g.Path))
					_addresses.Add(group.Key, group.ToList());
			}

			FillPlanObjects(_planObjects, _rootObjects);

			// Регистрируем все имеющиеся провайдеры данных
			RegisterDataProviders();

			// формируем конфигурацию для провайдеров данных
			var config = new Dictionary<string, object>
			{
				{
					"ConfigSource",
					new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true }
				},
				{
					"Addresses",
					_addresses.Keys.ToList()
				}
			};

			// инициализируем провайдеры
			foreach (var dataProvider in _providers)
			{
				dataProvider.DataProviderInitializedEvent += OnDataProviderInitializedEvent;
				dataProvider.Initialize(config);
			}

			// запускаем таймер
			_pushValuesToClientTimer.Start();
		}

		/// <summary>
		/// Процедура для обновления планов из БД
		/// </summary>
		private void OnPlansModified(BaseObject obj)
		{
			// прекращаем отправлять значения
			_pushValuesToClientTimer.Stop();

			// очищаем адреса и изменившиеся объекты
			_addresses.Clear();
			_changedValues.Clear();
			_planObjects.Clear();

			using (var plansDb = new PlansDc(_esterConnectionString))
			{
				// обновляем информцию из бд
				_rootObjects = plansDb.PlanObjects.Where(po => po.Parent == null).Select(p => ServerExtensions.FromDbObject(p)).ToList();
				foreach (var group in plansDb.Properties.GroupBy(g => g.Path)) _addresses.Add(group.Key, group.ToList());
			}

			FillPlanObjects(_planObjects, _rootObjects);

			// обновляем подписку на всех работающих провайдерах данных
			foreach (var dataProvider in _providers.Where(p => p.State == DataProviderState.Working))
				dataProvider.UpdateSubscription(_addresses.Keys.ToList());

			// возобновляем ход времени :)
			_pushValuesToClientTimer.Start();

			// уведомляем подписчиков о необходимости перечитать дерево целиком
			PushValuesToClients(true);
		}


		/// <summary>
		/// Периодическая отправка значений клиентам
		/// </summary>
		private void PushValuesToClientTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (_changedValues.Count > 0)
				PushValuesToClients();
		}

		/// <summary>
		/// Функция для регистрации провайдеров данных
		/// </summary>
		private void RegisterDataProviders()
		{
			// Bacnet-провайдер
			_providers.Add(new BacnetDataProvider());
			_providers.Add(new FakeDataProvider());
		}

		/// <summary>
		/// Отправка значений подписчикам
		/// </summary>
		/// <param name="updateTree">True, если клиенту необходимо обновить структуру дерева, False в противном случае</param>
		private void PushValuesToClients(bool updateTree = false)
		{
			ValuesPusherComet.SetEvent(new UpdateInfo { ChangedValues = new List<BaseObject>(_changedValues), UpdateObjectTree = updateTree });
			_changedValues.Clear();
		}

		private void OnDataProviderInitializedEvent(IDataProvider sender)
		{
			if (sender.State != DataProviderState.Working) return;

			// отписываемся от инициализации
			sender.DataProviderInitializedEvent -= OnDataProviderInitializedEvent;

			// подписываемся на события
			sender.DataRecievedEvent += NetworkOnDataRecievedEvent;
			sender.DataWrittenEvent += OnDataSavedToNetworkEvent;
		}

		/// <summary>
		/// Обработка поступивших от провайдеров значений
		/// </summary>
		/// <param name="values">Ключ - адрес в сети, значение - значение по адресу</param>
		private void NetworkOnDataRecievedEvent(Dictionary<string, string> values)
		{
			foreach (var value in values)
			{
				// log event
				var myEvent = new LogEventInfo(LogLevel.Info, "BAChistory", "");
				myEvent.Properties.Add("address", value.Key);
				myEvent.Properties.Add("value", value);
				LogManager.GetCurrentClassLogger().Log(myEvent);

				if (_addresses.ContainsKey(value.Key))
					// обновляем значения на всех объектах, для которых прописан данный адрес
					foreach (var dbAddress in _addresses[value.Key])
					{
						if (!_planObjects.ContainsKey(dbAddress.ObjectId)) continue;

						var doubleValue = double.Parse(value.Value);
						_planObjects[dbAddress.ObjectId].Update((PropertyTypes)dbAddress.AddressTypeId, doubleValue);

						// если список обновленных значений еще не содержит этот объект, добавляем его
						if (!_changedValues.Contains(_planObjects[dbAddress.ObjectId]))
							_changedValues.Add(_planObjects[dbAddress.ObjectId]);
					}
			}
			if (_changedValues.Count >= 100)
				// скопилось достаточно много данных, отправим-ка их клиенту
				PushValuesToClients();
		}

		/// <summary>
		///  Строит одноуровневый список объектов по лесу объетов
		/// </summary>
		/// <param name="planObjects">Словарь, в который добавляются объекты</param>
		/// <param name="rootObjects">Список объектов, от которых начинать построение</param>
		private static void FillPlanObjects(IDictionary<int, BaseObject> planObjects, IEnumerable<BaseObject> rootObjects)
		{
			foreach (var rootObject in rootObjects)
			{
				planObjects.Add(rootObject.Id, rootObject);
				var containerObject = rootObject as IContainerObject;
				if (containerObject != null)
					FillPlanObjects(planObjects, containerObject.Children);
			}
		}

		/// <summary>
		/// Возвращает список устройств из сети
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NetworkDevice> GetDevices()
		{
			return _providers.SelectMany(p => p.GetDevices());
		}


		#region Users request
		public Stream GetAllSchedules()
		{

			var myResponseBody = JsonConvert.SerializeObject(GetSchedulesFromDataBase().Where(s => !s.DeleteOnSync));
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}

		private List<ScheduleClass> GetSchedulesFromDataBase()
		{
			using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
				return context.Schedules.Select(dbSchedule => ConvertDbScheduleToScheduleClass(dbSchedule)).ToList();
		}

		private ScheduleClass ConvertDbScheduleToScheduleClass(Schedule dbSchedule)
		{
			var schedule = new ScheduleClass
			{
				Name = dbSchedule.Title,
				Id = dbSchedule.ObjectId,
				OverrideController = dbSchedule.OverrideController ?? false,
				DeleteOnSync = dbSchedule.DeleteOnSync ?? false,
				ControlledObjects = dbSchedule.SchedulesControlledProperties.Select(c => c.Property).ToList()
			};

			var days = new List<List<ValueTimeRange>>();
			for (var i = 0; i < 7; i++)
			{
				var daySchedule = dbSchedule.SchedulesContents.Where(sc => sc.Day == i).
					Select(ds => new ValueTimeRange(ds.Time, ds.Value)).ToList();
				days.Add(daySchedule);
			}
			schedule.Days = days.ToArray();
			return schedule;
		}

		private Schedule ConvertScheduleClassToDbSchedule(ScheduleClass schedule)
		{
			var dbSchedule = new Schedule
			{
				Title = schedule.Name,
				ObjectId = schedule.Id,
				OverrideController = schedule.OverrideController,
				DeleteOnSync = schedule.DeleteOnSync
			};

			foreach (var controlledObject in schedule.ControlledObjects)
			{
				dbSchedule.SchedulesControlledProperties.Add(new SchedulesControlledProperty() { });
			}

			for (int index = 0; index < schedule.Days.Length; index++)
			{
				var day = schedule.Days[index];
				dbSchedule.SchedulesContents.AddRange(
					day.Select(s => new SchedulesContent { Day = index, Time = s.Start, Value = s.Value }).ToList());
			}
			return dbSchedule;
		}

		//[ServerExceptionAspect]
		//public Stream AddSchedule(Stream stream)
		//{
		//	var schedule = GetScheduleClassFromStream(stream);
		//	schedule.Id = GetFreeIdForType(schedule.Type);
		//	schedule.OverrideController = true;
		//	AddScheduleToDataBase(schedule);
		//	var myResponseBody = JsonConvert.SerializeObject(schedule);
		//	if (WebOperationContext.Current != null)
		//		WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
		//	return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		//}

		//[ServerExceptionAspect]
		//public Stream EditSchedule(string id, Stream stream)
		//{
		//	var schedule = GetScheduleClassFromStream(stream);
		//	try
		//	{
		//		int intId = int.Parse(id);
		//		schedule.Id = intId;
		//		schedule.OverrideController = true;
		//		EditScheduleInDataBase(schedule);
		//	}
		//	catch
		//	{
		//		throw new BadRequestException();
		//	}
		//	var myResponseBody = JsonConvert.SerializeObject(schedule);
		//	if (WebOperationContext.Current != null)
		//		WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
		//	return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		//}

		public void DeleteSchedule(string id)
		{
			try
			{
				int intId = int.Parse(id);
				using (var context = new PlansDc(ConfigurationManager.ConnectionStrings["Ester"].ConnectionString))
				{
					var dbSchedule = context.Schedules.Single(s => s.ObjectId == intId);
					dbSchedule.DeleteOnSync = true;
					context.SubmitChanges();
				}
			}
			catch
			{
				throw new BadRequestException();
			}
		}

		//[ServerExceptionAspect]
		//public Stream GetScheduleById(string id)
		//{
		//	try
		//	{
		//		int intId = int.Parse(id);
		//		using (var context = new PlansDc())
		//		{
		//			var dbSchedule = context.Schedules.Single(s => s.ObjectId == intId);
		//			var schedule = ConvertDbScheduleToScheduleClass(dbSchedule);
		//			var myResponseBody = JsonConvert.SerializeObject(schedule);
		//			if (WebOperationContext.Current != null)
		//				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
		//			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		//		}
		//	}
		//	catch
		//	{
		//		throw new BadRequestException();
		//	}
		//}

		public void SetControlledObjects(Stream stream)
		{
			throw new NotImplementedException();
		}

		#endregion

		private static ScheduleClass GetScheduleClassFromStream(Stream stream)
		{
			var reader = new StreamReader(stream);
			var data = reader.ReadToEnd();
			var res = JsonConvert.DeserializeObject<ScheduleClass>(data);
			return res;
		}

		#region Получение данных из сети и передача веб-сервису

		public event DataReceivedFromNetworkEventHandler DataReceivedFromNetworkEvent;

		protected virtual void OnDataReceivedFromNetworkEvent(Dictionary<string, string> values)
		{
			var handler = DataReceivedFromNetworkEvent;
			if (handler != null) handler(values);
		}

		public void DataReceivedFromNetwork(Dictionary<string, string> values)
		{
			OnDataReceivedFromNetworkEvent(values);
		}

		#endregion

		#region Получение данных от веб-сервиса и запись в сеть

		public event DataReceivedFromClientEventHandler DataReceivedFromClientEvent;

		protected virtual void OnDataReceivedFromClientEvent(Dictionary<string, string> values)
		{
			var handler = DataReceivedFromClientEvent;
			if (handler != null) handler(values);
		}

		public event DataSavedToNetworkEventHandler DataSavedToNetworkEvent;

		protected virtual void OnDataSavedToNetworkEvent(Dictionary<string, string> values, bool success)
		{
			var handler = DataSavedToNetworkEvent;
			if (handler != null) handler(values, success);
		}

		public void SaveDataToNetwork(Dictionary<string, string> values)
		{
			OnDataReceivedFromClientEvent(values);
		}

		public void DataSavedToNetwork(Dictionary<string, string> values, bool success)
		{
			OnDataSavedToNetworkEvent(values, success);
		}

		#endregion


		/// <summary>
		/// Обновляет значения в сетях контроллеров
		/// </summary>
		/// <param name="obj">Объект для сохранения</param>
		internal object UpdateObjectProperties(BaseObject obj)
		{
			var currentState = _planObjects[obj.Id];
			if (currentState == null)
				return false;

			var updateValues = currentState.GetChanges(obj);
			foreach (var updateValue in updateValues)
			{
				var save = _addresses
					.SelectMany(a => a.Value)
					.Where(a => a.ObjectId == obj.Id && a.AddressTypeId == (int)updateValue.Item1)
					.ToDictionary(d => d.Path, d => updateValue.Item2);
				SaveToNetwork(save);
			}
			return true;
		}

		/// <summary>
		/// Записать значения в сеть контроллеров
		/// </summary>
		/// <param name="values">Ключ - адрес в сети, значение - значение по адресу</param>
		private void SaveToNetwork(Dictionary<string, string> values)
		{
			foreach (var dataProvider in _providers.Where(p => p.State == DataProviderState.Working))
			{
				dataProvider.SaveValuesToNetwork(values);
			}
		}
	}
}
