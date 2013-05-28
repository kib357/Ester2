using System.IO;
using System.Windows.Media.Imaging;

namespace EsterServer.Model.Services
{
    public static class WorkWithImage
    {
        public static byte[] BitmapImageToByteArray(BitmapImage image)
        {
            byte[] data;
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
    }
}
