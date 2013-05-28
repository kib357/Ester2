using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ester.Model.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        private bool _inverted = false;

        public bool Inverted
        {
            get { return _inverted; }
            set { _inverted = value; }
        }

        private object DoubleToString(object value)
        {
            if (!(value is double))
                return DependencyProperty.UnsetValue;           
            return value.ToString();
        }

        private object StringToDouble(object value)
        {
            if (!(value is string))
                return DependencyProperty.UnsetValue;
            double newValue;
            double.TryParse(value.ToString(), out newValue);
            return newValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? StringToDouble(value) : DoubleToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? DoubleToString(value) : StringToDouble(value);
        }
    }
}
