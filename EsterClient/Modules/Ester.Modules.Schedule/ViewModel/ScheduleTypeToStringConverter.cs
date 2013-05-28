using System;
using System.Globalization;
using System.Windows.Data;
using EsterCommon.Enums;

namespace Ester.Modules.Schedule.ViewModel
{
	public class ScheduleTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((ScheduleTypes)value)
			{
				case ScheduleTypes.SKUD:
					return "СКУД";
				case ScheduleTypes.Light:
					return "Освещение";
				case ScheduleTypes.Heat:
					return "Отопление";
				case ScheduleTypes.Ventilation:
					return "Вентиляция";
				case ScheduleTypes.AC:
					return "Кондиционирование";
				default:
					throw new ArgumentOutOfRangeException("value");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
