using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Ester.Model.Services;

namespace Ester.Model.Converters
{
    public class UserTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                switch (value as string)
                {
                    case "Расписания":
                        return SessionInfo.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
                    default:
                        return Visibility.Visible;
                }
            }
            return SessionInfo.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
