using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EsterCommon.PlanObjectTypes;

namespace Ester.Modules.Building
{
	public class SubsystemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate TemperatureSensorTemplate { get; set; }
		public DataTemplate LightSensorTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
		{
			if (item is TemperatureSensor)
				return TemperatureSensorTemplate;
			if (item is LightSensor)
				return LightSensorTemplate;
			return base.SelectTemplate(item, container);
		}
	}
}
