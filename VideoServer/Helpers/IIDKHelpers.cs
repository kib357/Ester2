// Вспомогательные методы для извлечения кратинки, получаемой из iidk.ocx

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace VideoServer.Helpers
{
    public class IIDKHelpers
    {
        // Declares a class member for each structure element.
        [StructLayout(LayoutKind.Sequential)]
        public class SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

            public DateTime ToDateTime()
            {
                return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public uint bfOffBits;
        }

        private const ushort BFTYPE_BMP = 19778;

        private static BITMAPFILEHEADER createFileHeader(ref BITMAPINFOHEADER info)
        {
            BITMAPFILEHEADER header = new BITMAPFILEHEADER();
            header.bfType = BFTYPE_BMP;
            int headerSize = Marshal.SizeOf(header) + Marshal.SizeOf(info);
            header.bfSize = (uint) headerSize + info.biSizeImage;
            header.bfOffBits = (uint) headerSize;
            return header;
        }

        public static Bitmap GetBitmap(int pImage)
        {
            IntPtr pImagePtr = new IntPtr(pImage);
            BITMAPINFOHEADER info = (BITMAPINFOHEADER) Marshal.PtrToStructure(pImagePtr, typeof (BITMAPINFOHEADER));
            BITMAPFILEHEADER header = createFileHeader(ref info);

            int headerSize = Marshal.SizeOf(header);
            byte[] data = new byte[header.bfSize];

            IntPtr pHeader = Marshal.AllocHGlobal(headerSize);
            Marshal.StructureToPtr(header, pHeader, false);
            Marshal.Copy(pHeader, data, 0, headerSize);
            Marshal.FreeHGlobal(pHeader);
            Marshal.Copy(pImagePtr, data, headerSize, (int) header.bfSize - headerSize);

            using (MemoryStream ms = new MemoryStream(data))
            {
                return new Bitmap(ms);
            }
        }

        public static DateTime GetDateTime(int sysTime)
        {
            IntPtr pSysTime = new IntPtr(sysTime);

            SystemTime sTime = new SystemTime();
            sTime = (SystemTime) Marshal.PtrToStructure(pSysTime, typeof (SystemTime));

            return sTime.ToDateTime();
        }
    }
}
