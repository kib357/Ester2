using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ester.Model.Enums;
using Ester.Model.Interfaces;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Ester.Model.BaseClasses
{
	/// <summary>
	///   ������� ����� ������ ������� ��� �������� ���������� ��������� ��������������� "EnigneMon"
	///   � ��� �� "�������", 2011
	/// </summary>
	public class SensorBase : UserControl
	{
		protected IDataTransport _dataTransport;

		public SensorBase()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				//ValuesLongPull.ValuesChangedEvent += SetValues;
				var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
				_dataTransport = container.Resolve<IDataTransport>();
			}
		}

		public override string ToString()
		{
			return string.IsNullOrWhiteSpace(Title) ? "SensorBase" : Title;
		}

		public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
			"Id", typeof(string), typeof(SensorBase), new PropertyMetadata(""));

		public string Id
		{
			get { return (string)GetValue(IdProperty); }
			set { SetValue(IdProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title", typeof(string), typeof(SensorBase), new PropertyMetadata(""));

		
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
			"Description", typeof(string), typeof(SensorBase), new PropertyMetadata(""));

		
		
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}

		public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
			"Address", typeof(string), typeof(SensorBase), new PropertyMetadata(""));

		
		public string Address
		{
			get { return (string)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}

		public string[] AddressList
		{
			get
			{
				if (string.IsNullOrEmpty(Address))
					return new string[0];

				var res = Address.Split(',');
				for (int i = 0; i < res.Length; i++)
					res[i] = res[i].Trim();
				return res;
			}
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			"Value", typeof(string), typeof(SensorBase));

		
		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register(
			"Units", typeof(string), typeof(SensorBase), new PropertyMetadata(string.Empty));

		
		public string Units
		{
			get { return (string)GetValue(UnitsProperty); }
			set { SetValue(UnitsProperty, value); }
		}

		public string[] UnitsList
		{
			get
			{
				int length = AddressList.Length;
				if (string.IsNullOrEmpty(Units))
					return new string[length];

				var unitsList = Units.Split(',');
				string[] res = new string[length];
				for (int i = 0; i < res.Length && i < unitsList.Length; i++)
					res[i] = unitsList[i].Trim();
				return res;
			}
		}

		protected string ConvertValueByUnits(string value, string units)
		{
			if (value == null)
				return "?";
			string stringValue = value;
			double percentage;
			if (stringValue == "Active" || stringValue == "1")
				stringValue = "ON";
			if (stringValue == "Inactive" || stringValue == "0")
				stringValue = "OFF";
			var cultureInfo = new CultureInfo("ru-RU", true);
			switch (units)
			{
				case "�C":
					stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage, 1) + " �C" : "? �C";
					break;
				case "���":
					stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage, 1) + " ���" : "? ���";
					break;
				case "��":
					stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage, 1) + " ��" : "? ��";
					break;
				case "���":
					stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage, 1) + " ���" : "? ���";
					break;
				case "%":
					stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage) + " %" : "? %";
					break;
				case "��%":
					stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage * 2) + " %" : "? %";
					break;
				case "NState":
					switch (stringValue)
					{
						case "1":
							stringValue = "�����";
							break;
						case "2":
							stringValue = "����";
							break;
						case "3":
							stringValue = "���";
							break;
						case "4":
							stringValue = "����";
							break;
						default:
							stringValue = "?";
							break;
					}
					break;
				case "RS":
					if (stringValue == "ON")
						stringValue = true.ToString();
					else if (stringValue == "OFF")
						stringValue = false.ToString();
					break;
				case "State":
				case "PumpState":
				case "KoState":
					switch (stringValue)
					{
						case "OFF":
							stringValue = "����";
							break;
						case "ON":
							stringValue = "�����";
							break;
					}
					break;
				case "PumpMode":
					switch (stringValue)
					{
						case "OFF":
							stringValue = "������";
							break;
						case "ON":
							stringValue = "����";
							break;
					}
					break;
				case "kWtenth":
					{
						stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage / 10.0, 2) + " ���" : "? ���";
						break;
					}
				case "�Ftenth":
					{
						stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(((percentage / 10.0) - 32.0) * 5.0 / 9.0, 2) + " �C" : "? �C";
						break;
					}
				case "ConOverallStatus":
					{
						switch (stringValue)
						{
							case "OFF": stringValue = "����������"; break;
							case "2": stringValue = "�����������"; break;
							case "4": stringValue = "��������������"; break;
							case "8": stringValue = "�����������"; break;
						}
						break;
					}
				case "A":
					{
						stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage, 4) + " A" : "? A";
						break;
					}
				case "V":
					{
						stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage, 2) + " �" : "? �";
						break;
					}
				case "W":
					{
						stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage / 1000.0, 4) + " ���" : "? ���";
						break;
					}
				case "Hz":
					{
						stringValue = Double.TryParse(stringValue, NumberStyles.Any, cultureInfo.NumberFormat, out percentage) ? Math.Round(percentage / 10.0, 4) + " ��" : "? ��";
						break;
					}
			}
			return stringValue;
		}

		protected IEventAggregator EventAggregator;

		protected virtual void InitializeView()
		{
		}

		protected virtual void SetValues(KeyValuePair<string, string> sensor)
		{
			for (int i = 0; i < AddressList.Length; i++)
			{
				if (AddressList[i] == sensor.Key)
					switch (i)
					{
						case 0:
							Value = sensor.Value;
							break;
					}
			}
		}

		protected bool TryParseBool(string value, out bool result)
		{
			value = value.Replace("0", "false").Replace("1", "true").ToLower();
			return bool.TryParse(value, out result);
		}

        [Obsolete("����� ����� �� ������������, ���������������� ����������������")]
		protected async Task<bool> TryPushValueToServerAsync(string address, string value)
		{
			//return await Task.Run(() => TryPushValuesToServer(address, value));
		    return false;
		}

        [Obsolete("����� ����� �� ������������, ���������������� ����������������")]
		protected bool TryPushValuesToServer(string address, string value)
		{
			var addressList = address.Split('.');
			if (_dataTransport == null || addressList.Length != 2) return false;
			try
			{
				//_dataTransport.PostRequest<object>(value, Urls.Bacnet + "/" + addressList[0] + "/" + addressList[1], true, 10000);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
