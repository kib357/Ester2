using System;
using System.Globalization;
using System.Windows.Data;

namespace Ester.Model.Converters
{
	public class TemperatureConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "Нет";
			else
				return ((double)value).ToString("0.0").Replace('.', ',');
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (value as string)
			{
				case "Нет":
					return null;
				default:
					return double.Parse(value as string);
			}
		}
	}
}