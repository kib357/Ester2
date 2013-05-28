using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ester.Model.Converters
{
	public class ItemsCountToVisibilityConverter : IValueConverter
	{
		public ItemsCountToVisibilityConverter()
		{
			IsReverse = false;
		}

		public bool IsReverse { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
			{
				if (value is int && ((int)value) > 0)
					return IsReverse ? Visibility.Visible : Visibility.Collapsed;
				else
					return IsReverse ? Visibility.Collapsed : Visibility.Visible;
			}
			else
			{
				int i;
				if (int.TryParse(parameter as string, out i))
					if (value is int && (int)value > i)
						return IsReverse ? Visibility.Visible : Visibility.Collapsed;
					else
						return IsReverse ? Visibility.Collapsed : Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
