using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ester.Model.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public StringToVisibilityConverter()
        {
            Inverted = true;
        }

        public bool Inverted { get; set; }

        private object VisibilityToString(object value)
        {
            if (!(value is Visibility))
                return DependencyProperty.UnsetValue;           
            return value.ToString();
        }

        private object StringToVisibility(object value)
        {
            if (!(value is string))
                return DependencyProperty.UnsetValue;            
            return value.ToString().Length > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? StringToVisibility(value) : VisibilityToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? VisibilityToString(value) : StringToVisibility(value);
        }
    }
}
