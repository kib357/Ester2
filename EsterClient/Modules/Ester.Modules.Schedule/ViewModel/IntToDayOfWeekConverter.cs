using System;
using System.Globalization;
using System.Windows.Data;

namespace Ester.Modules.Schedule.ViewModel
{
	public class IntToDayOfWeekConverter : IValueConverter
	{
		static CultureInfo russian = CultureInfo.GetCultureInfo("ru-RU");

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return russian.DateTimeFormat.AbbreviatedDayNames[((byte)value + 1) % 7];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
