using System;
using System.Windows;
using System.Windows.Data;

namespace Ester.Model.Converters
{
	public class NullableToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null || (string) parameter == bool.TrueString)
			{
				//if (value is string)
				//{
				//	return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
				//}
				return value == null ? Visibility.Collapsed : Visibility.Visible;
			}
			else
			{
				if (value is string && parameter as string == "NullOrEmpty")
				{
					return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
				}

				return value == null ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
