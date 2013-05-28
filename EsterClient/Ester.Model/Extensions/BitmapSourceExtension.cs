using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using IO = System.IO;
namespace Ester.Model.Extensions
{
    public  static  class BitmapSourceExtension
    {
        public static void Save(this BitmapSource bitmapSource, IO.Stream stream, BitmapEncoder encoder)
        {
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
            stream.Flush();
        }
    }
}
