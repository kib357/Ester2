using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Ester.Model.Services;

namespace Ester.Model.Converters
{
    public class ByteArrayToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] array = value as byte[];
            if (array == null) return null;
            return WorkWithImages.BytesToBitmapSource(array,500);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage img = value as BitmapImage;
            if (img == null) return null;
            return WorkWithImages.ImageSourceToBytes(img,"jpg");
        }
    }
}
