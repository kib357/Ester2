using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace Ester.Model.Services
{
    public static class WorkWithImages
    {
        public static void Rotate(int angle,ref BitmapSource img)
        {
            if (img != null)
            {
                var tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = img;
                var transform = new RotateTransform(angle);
                tb.Transform = transform;
                tb.EndInit();
                img = BitmapSourceToBitmapImage(tb);
            }
        }

        public static BitmapImage BitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            var encoder = new JpegBitmapEncoder();
            var memoryStream = new MemoryStream();
            var bImg = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);

            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(memoryStream.ToArray());
            bImg.EndInit();

            memoryStream.Close();

            return bImg;
        }

        public static byte[] ImageSourceToBytes(ImageSource image, string preferredFormat)
        {
            byte[] result = null;
            BitmapEncoder encoder = null;
            switch (preferredFormat.ToLower())
            {
                case "jpg":
                case "jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case "bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case "png":
                    encoder = new PngBitmapEncoder();
                    break;

                case "tif":
                case "tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case "gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case "wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
            }

            if (image is BitmapSource)
            {
                MemoryStream stream = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);
                br.Read(result, 0, (int)stream.Length);
                br.Close();
                stream.Close();
            }
            return result;
        }

        public static BitmapSource BytesToBitmapSource(byte[] imageData, int decodePixelWidth=0, int decodePixelHeight=0)
        {
            if (imageData == null || imageData.Length==0) return null;

            BitmapImage result = new BitmapImage();
            result.BeginInit();
            if (decodePixelWidth > 0)
            {
                result.DecodePixelWidth = decodePixelWidth;
            }
            if (decodePixelHeight > 0)
            {
                result.DecodePixelHeight = decodePixelHeight;
            }
            result.StreamSource = new MemoryStream(imageData);
            result.CreateOptions = BitmapCreateOptions.None;
            result.CacheOption = BitmapCacheOption.Default;
            result.EndInit();
            return result;
        }

        public static byte[] ResizeImageToBytes(ImageSource source, int width, int height)
        {
            byte[] imageBytes = ImageSourceToBytes(source,"jpg");

            BitmapSource imageSource = BytesToBitmapSource(imageBytes, width, height);

            return ImageSourceToBytes(imageSource, "jpg");
           
        }

        public static ImageSource ResizeImageToImage(ImageSource source, int width, int height)
        {
            byte[] imageBytes = ImageSourceToBytes(source, "jpg");

            BitmapSource imageSource = BytesToBitmapSource(imageBytes, width, height);

            return imageSource;

        }


        public static BitmapImage ConvertToOtherPixelFormat(BitmapSource source, PixelFormat format)
        {
            var newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = source;
            newFormatedBitmapSource.DestinationFormat = format;
            newFormatedBitmapSource.EndInit();
            return BitmapSourceToBitmapImage(newFormatedBitmapSource.Source);
        }

    }
    
}
