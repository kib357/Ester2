using System;
using System.Windows.Data;

namespace Ester.Modules.Schedule.ViewModel
{
	public class TimeSpanToDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is TimeSpan)
			{
				return ((TimeSpan)value).TotalMinutes;
			}
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType == typeof(TimeSpan))
			{
				int val = (int)value;
				return new TimeSpan(val / 60, (val % 60), 0);
			}
			throw new NotImplementedException();
		}
	}
}
