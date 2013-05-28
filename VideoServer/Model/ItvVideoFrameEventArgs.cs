using System;
using System.Drawing;

namespace VideoServer.Model
{
    /// <summary>
    /// Класс, содержащий готовый кадр
    /// </summary>
    public class ItvVideoFrameEventArgs : EventArgs
    {
        private string _subtitle = String.Empty;
        private DateTime _dt;
        private Bitmap _bmp;

        public ItvVideoFrameEventArgs(string subtitle, DateTime dt, Bitmap bmp)
            : base()
        {
            _subtitle = subtitle;
            _dt = dt;
            _bmp = bmp;
        }

        public string Subtitle
        {
            get
            {
                return _subtitle;
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return _dt;
            }
        }

        public Bitmap ImageBitmap
        {
            get
            {
                return _bmp;
            }
        }
    }
}
