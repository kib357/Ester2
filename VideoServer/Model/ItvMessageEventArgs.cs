using System;

namespace VideoServer.Model
{
    /// <summary>
    /// ����� ��������� ������ ���������.
    /// </summary>
    public class ItvMessageEventArgs : EventArgs
    {
        private string _msg = String.Empty;
        /// <summary>
        /// ����������� ������.
        /// </summary>
        /// <param name="message">��������� � ������� ���������.</param>
        public ItvMessageEventArgs(string message)
            : base()
        {
            _msg = message;
        }
        /// <summary>
        /// �������� ��������� � ������� ���������.
        /// </summary>
        public string Message
        {
            get { return _msg; }
        }
    }
}
