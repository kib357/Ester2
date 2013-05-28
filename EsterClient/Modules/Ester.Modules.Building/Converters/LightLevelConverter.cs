using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ester.Modules.Building.ViewModel
{
	public class LightLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (value as int?)
			{
				case null:
					return "Нет";
				default:
					return value.ToString();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (value as string)
			{
				case "Нет":
					return null;
				default:
					return int.Parse(value as string);
			}
		}
	}
}
