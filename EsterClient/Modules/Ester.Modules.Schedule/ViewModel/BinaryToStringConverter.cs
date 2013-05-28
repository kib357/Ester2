using System;
using System.Globalization;
using System.Windows.Data;

namespace Ester.Modules.Schedule.ViewModel
{
	public class BinaryToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value == 1.0 ? "Вкл" : "Выкл";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (string)value == "Вкл" ? 1.0 : 0.0;
		}
	}
}
