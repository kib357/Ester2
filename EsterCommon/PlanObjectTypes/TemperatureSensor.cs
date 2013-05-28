using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using EsterCommon.Enums;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Data;

namespace EsterCommon.PlanObjectTypes
{
	public class TemperatureSensor : Subsystem
	{
		private PropertyValue _temperature;
		private PropertyValue _temperatureSetpoint;
		private BoolPropertyValue _allowManual;

		public TemperatureSensor()
		{
			_temperature = new PropertyValue();
			_temperatureSetpoint = new PropertyValue();
			_allowManual = new BoolPropertyValue();
		}

		public PropertyValue Temperature
		{
			get { return _temperature; }
			set { _temperature = value; RaisePropertyChanged("Temperature"); }
		}
		public PropertyValue TemperatureSetpoint
		{
			get { return _temperatureSetpoint; }
			set
			{
				if (value.Equals(_temperatureSetpoint)) return;
				_temperatureSetpoint = value;
				RaisePropertyChanged("TemperatureSetpoint");
			}
		}

		public BoolPropertyValue AllowManual
		{
			get { return _allowManual; }
			set
			{
				if (value.Equals(_allowManual)) return;
				_allowManual = value;
				RaisePropertyChanged("AllowManual");
			}
		}

		public override void Update(BaseObject newObject)
		{
			base.Update(newObject);
			Temperature = ((TemperatureSensor)newObject).Temperature;
			TemperatureSetpoint = ((TemperatureSensor)newObject).TemperatureSetpoint;
		}

		public override List<Tuple<PropertyTypes, string>> GetChanges(BaseObject newObj)
		{
			var values = newObj as TemperatureSensor;
			if (values == null)
				throw new ArgumentException("Аргумент не соответствует типу");

			var result = new List<Tuple<PropertyTypes, string>>();
			if (TemperatureSetpoint.Value != values.TemperatureSetpoint.Value)
				result.Add(new Tuple<PropertyTypes, string>(PropertyTypes.Thermometer_Setpoint, values.TemperatureSetpoint.Value.ToString()));

			if (AllowManual.BoolValue != values.AllowManual.BoolValue)
				result.Add(new Tuple<PropertyTypes, string>(PropertyTypes.Thermometer_AllowManual, values.AllowManual.Value.ToString()));

			return result;
		}
	}
}