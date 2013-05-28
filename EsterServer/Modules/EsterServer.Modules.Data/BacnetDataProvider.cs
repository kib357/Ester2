using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BacNetApi;
using BacNetApi.Data;
using BacNetApi.Extensions;
using BACsharp;
using Nini.Config;

namespace EsterServer.Modules.Data
{
	internal static class BacnetExtesions
	{
		/// <summary>
		/// Возвращает объект bacnet-сети по его адресу
		/// </summary>
		public static PrimitiveObject Object(this BacNet network, string address)
		{
			if (!address.Contains('.'))
				throw new ArgumentException("Адрес не содержит точек", "address");

			var objAddr = address.Split('.')[1].Trim();
			uint instance;
			instance = uint.TryParse(address.Split('.')[0].Trim(), out instance) ? instance : 0;

			return network[instance].Objects[objAddr];
		}
	}

	public class BacnetDataProvider : IDataProvider
	{
		/// <summary>
		/// Бакнет-сеть
		/// </summary>
		private BacNet _network;

		/// <summary>
		/// Состояние провайдера
		/// </summary>
		private DataProviderState _state;

		/// <summary>
		/// Регулярное выражение для проверки адресов на принадлежность к бакнет сети
		/// </summary>
		private readonly Regex _backnetAddressRegex = new Regex(
			@"^(\d{1,5})\.(AI|AO|AV|BI|BO|BV|CAL|CMD|DEV|EE|FI|LE|GR|LOOP|MI|MO|NC|PG|SCH|AR|MV|TL|LSP|LSZ|AC|PC|EL|GG|TLM|LC|SV|CU|AG|DC|DG)(\d{1,7})$",
			RegexOptions.Singleline
		);

		public event DataProviderInitializedEventHandler DataProviderInitializedEvent;
		public event DataRecievedEventHandler DataRecievedEvent;
		public event DataWrittenEventHandler DataWrittenEvent;

		/// <summary>
		/// Список адресов на которые запрашивали подписку
		/// </summary>
		private readonly List<string> _subscribedAddresses;

		/// <summary>
		/// Состояние провайдера данных
		/// </summary>
		public DataProviderState State
		{
			get { return _state; }
		}

		public BacnetDataProvider()
		{
			_state = DataProviderState.Uninitialized;
			_subscribedAddresses = new List<string>();
		}

		/// <summary>
		/// Инициализация провайдера
		/// </summary>
		/// <param name="configuration"></param>
		public void Initialize(Dictionary<string, object> configuration)
		{
			_state = DataProviderState.Initializing;
			try
			{
				// читаем конфиг
				var config = (XmlConfigSource)configuration["ConfigSource"];
				_network = new BacNet(config.Configs["BacNet"].Get("Ip"));

				// выгребаем бакнет-адреса
				var addresses = ((List<string>)configuration["Addresses"]).Where(a => _backnetAddressRegex.IsMatch(a)).ToList();

				// подписываемся
				addresses.ForEach(Subscribe);

				// считаем, что работатем
				_state = DataProviderState.Working;

				// уведомляем внимательных слушателей о инициализации данного провайдера
				if (DataProviderInitializedEvent != null)
					DataProviderInitializedEvent(this);
			}
			catch (Exception)
			{
				// бида-пичаль
				_state = DataProviderState.Fault;
				_subscribedAddresses.Clear();
			}
		}

		/// <summary>
		/// Обработка обновления значения по адресу на сети
		/// </summary>
		/// <param name="address">Адрес</param>
		/// <param name="value">Значение</param>
		private void OnValueChangedEvent(string address, string value)
		{
			if (DataRecievedEvent != null)
				// пробрасываем собтие дальше
				DataRecievedEvent(new Dictionary<string, string> { { address, value } });
		}

		/// <summary>
		/// Сохранение значений в бакнет-сеть
		/// </summary>
		public bool SaveValuesToNetwork(Dictionary<string, string> values)
		{
			values = values.Where(d => _backnetAddressRegex.IsMatch(d.Key)).ToDictionary(dk => dk.Key, dk => dk.Value);

			// много значений
			if (values.Count > 1)
			{
				var result = true;
				var devWithObjValues = new List<DeviceWithObjValues>();
				foreach (var val in values)
				{
					var obj = val.Key.Split('.');
					var isContains = false;
					foreach (var dev in devWithObjValues.Where(dev => dev.DeviceId == obj[0]))
					{
						var newValues = new Dictionary<BacnetPropertyId, object> { { BacnetPropertyId.PresentValue, val.Value } };
						dev.ObjValues.Add(obj[1], newValues);
						isContains = true;
					}
					if (isContains) continue;
					var propWithValues = new Dictionary<BacnetPropertyId, object> { { BacnetPropertyId.PresentValue, val.Value } };
					var devWithValues = new DeviceWithObjValues
					{
						DeviceId = obj[0],
						ObjValues = new Dictionary<string, Dictionary<BacnetPropertyId, object>> { { obj[1], propWithValues } }
					};
					devWithObjValues.Add(devWithValues);
				}
				foreach (var deviceWithObjValues in devWithObjValues)
				{
					result &= WriteSeveralValues(deviceWithObjValues.DeviceId, deviceWithObjValues.ObjValues);
					Thread.Sleep(100);
				}
				return result;
			}

			// одно значение
			if (values.Count == 1)
			{
				var pair = values.ElementAt(0);
				var addr = pair.Key.Split('.');

				return WriteValue(addr[0], addr[1], pair.Value);
			}
			return true;
		}


		/// <summary>
		/// Запись значения в бакнет сеть
		/// </summary>
		/// <param name="deviceAddress">Номер контроллера</param>
		/// <param name="objectAddress">Адрес объекта</param>
		/// <param name="value">Значение</param>
		/// <param name="propertyId">Идентификтор свойства</param>
		private bool WriteValue(string deviceAddress, string objectAddress, string value, string propertyId = "")
		{
			var property = BacnetPropertyId.PresentValue;

			if (!string.IsNullOrEmpty(propertyId))
				Enum.TryParse(propertyId, out property);

			uint instance;
			if (!uint.TryParse(deviceAddress, out instance))
				return false;

			return _network[instance].Objects[objectAddress].Set(value, property);
		}

		/// <summary>
		/// Запись значения в бакнет сеть
		/// </summary>
		/// <param name="deviceAddress">Номер контроллера</param>
		/// <param name="values">Значения для записи</param>
		private bool WriteSeveralValues(string deviceAddress, Dictionary<string, Dictionary<BacnetPropertyId, object>> values)
		{
			uint instance;
			if (!uint.TryParse(deviceAddress, out instance))
				return false;

			return _network[instance].WritePropertyMultiple(values);
		}

		/// <summary>
		/// Функция возвращает все зарегистрированные в сети устройства
		/// </summary>
		public IEnumerable<NetworkDevice> GetDevices()
		{
			if (_network == null)
				return null;

			return _network.OnlineDevices.Select
			(
				d => new NetworkDevice
				{
					Title = d.Title,
					Id = d.Id,
					LastUpdated = d.LastUpdated,
					Status = d.Status.GetStringValue(),
					SubscribedObjects = d.Objects.ToList(),
					Objects = d.ObjectList
				}
			);
		}

		/// <summary>
		/// Прекратить
		/// </summary>
		public void Stop()
		{
			_network.Stop();
		}


		/// <summary>
		/// Обновление подписки.
		/// </summary>
		/// <param name="list">Список адресов, с которых необходимо получать значения</param>
		public void UpdateSubscription(List<string> list)
		{
			// отписаться от отсутсвующих в новом списке адресов
			_subscribedAddresses.Where(s => list.All(n => n != s)).ToList().ForEach(Unsubscribe);

			// подписаться на новые адреса
			list.Where(s => _subscribedAddresses.All(n => n != s)).ToList().ForEach(Subscribe);
		}

		/// <summary>
		/// Подписывается на объект в сети
		/// </summary>
		/// <param name="address">Адрес объекта в бакнет сети</param>
		private void Subscribe(string address)
		{
			if (!_backnetAddressRegex.IsMatch(address))
				return;

			if (State == DataProviderState.Fault || State == DataProviderState.Uninitialized) return;

			if (_subscribedAddresses.Contains(address)) return;

			_network.Object(address).ValueChangedEvent += OnValueChangedEvent;

			if (!_subscribedAddresses.Contains(address))
				_subscribedAddresses.Add(address);
		}

		/// <summary>
		/// Отписывается от объекта в бакнет сети
		/// </summary>
		/// <param name="address"></param>
		private void Unsubscribe(string address)
		{
			if (State == DataProviderState.Fault || State == DataProviderState.Uninitialized)
				return;

			if (!_subscribedAddresses.Contains(address))
				return;

			_network.Object(address).ValueChangedEvent -= OnValueChangedEvent;
			_subscribedAddresses.Remove(address);
		}
	}
}
