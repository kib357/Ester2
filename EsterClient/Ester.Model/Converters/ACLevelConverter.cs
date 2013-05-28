using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ester.Model.Converters
{
	public class ACLevelConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			dynamic val = value;
			switch ((int?)val)
			{
				case null:
					return "Нет";
				case 0:
					return "Выкл";
				case 1:
					return "1";
				case 2:
					return "2";
				case 3:
					return "3";
				case 4:
					return "4";
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch (value as string)
			{
				case "Выкл":
					return 0;
				case "1":
					return 1;
				case "2":
					return 2;
				case "3":
					return 3;
				case "4":
					return 4;
				default:
					return null;
			}
		}
	}
}
