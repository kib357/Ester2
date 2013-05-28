using System;

namespace VideoServer.Model
{
    /// <summary>
    /// Класс аргумента данных сообщения.
    /// </summary>
    public class ItvMessageEventArgs : EventArgs
    {
        private string _msg = String.Empty;
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение в формате Интеллект.</param>
        public ItvMessageEventArgs(string message)
            : base()
        {
            _msg = message;
        }
        /// <summary>
        /// Получить сообщение в формате интеллект.
        /// </summary>
        public string Message
        {
            get { return _msg; }
        }
    }
}
