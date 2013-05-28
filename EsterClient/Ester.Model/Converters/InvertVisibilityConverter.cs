using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ester.Model.Converters
{
    public class InvertVisibilityConverter : IValueConverter
    {
        private object InvertVisibility(object value)
        {
            if (!(value is Visibility))
                return DependencyProperty.UnsetValue;
            return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return InvertVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return InvertVisibility(value);
        }
    }
}
