using System;
using System.Globalization;
using System.Windows.Data;

namespace Ester.Model.Converters
{
    public class ObjectToStringConverter : IValueConverter
    {
        private bool _inverted = false;

        public bool Inverted
        {
            get { return _inverted; }
            set { _inverted = value; }
        }

        private object StringToObject(object value)
        {           
            return value.ToString();
        }

        private object ObjectToString(object value)
        {
            return value.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? ObjectToString(value) : StringToObject(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? StringToObject(value) : ObjectToString(value);
        }
    }
}
