using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ester.Model.Converters
{
    public class DateTimeToDateDDMMYYYY : IValueConverter
    {
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                var val = (DateTime)value;
                return val.Date.Day + "/" + val.Date.Month + "/" + val.Date.Year;
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                DateTime tmp;
                if (DateTime.TryParse((string) value, out tmp)) return tmp; else return null;
            }
            return null;
        }
    }
}
