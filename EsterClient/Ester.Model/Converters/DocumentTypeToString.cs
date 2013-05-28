using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Ester.Model.Enums;
using EsterCommon.Enums;

namespace Ester.Model.Converters
{
    public class DocumentTypeToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var type = (DocumentTypes)value;

            switch (type)
            {
                case DocumentTypes.RussianPassport:
                    return "Паспорт РФ";
                case DocumentTypes.DriverLicence:
                    return "Водительское удостоверение";
                case DocumentTypes.ForeignPassport:
                    return "Загранпаспорт";
                case DocumentTypes.ForeignRusPassport:
                    return "Загранпаспорт РФ";
                default:
                    return "Неизвестный документ";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string str = (string) value;
            switch (str.ToLower())
            {
                case "паспорт рф":
                    return DocumentTypes.RussianPassport;
                case "водительское удостоверение":
                    return DocumentTypes.DriverLicence;
                case "загранпаспорт":
                    return DocumentTypes.ForeignPassport;
                case "загранпаспорт рф":
                    return DocumentTypes.ForeignRusPassport;
                default:
                    return DocumentTypes.Unknown;
            }
        }
    }

}
