using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Ester.Model.BaseClasses;

namespace Ester.Model.Converters
{
    public class TemplatesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var templates = value as ObservableCollection<TemplateObject>;
            var result = "";
            if (templates!= null)
                result = templates.Where(template => template.IsSelected).Aggregate("", (current, template) =>
                                                                                        current + (template.Name + ", "));
            return result.Trim().TrimEnd(',');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
