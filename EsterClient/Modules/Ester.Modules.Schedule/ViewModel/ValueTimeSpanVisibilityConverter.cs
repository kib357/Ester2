using System;
using System.Windows;
using System.Windows.Data;

namespace Ester.Modules.Schedule.ViewModel
{
	public class ValueTimeSpanVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((DateTime)value) == DateTime.MinValue ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
