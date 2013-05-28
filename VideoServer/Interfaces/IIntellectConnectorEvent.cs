using System;
using VideoServer.Model;

namespace VideoServer.Interfaces
{
    /// <summary>
    /// Интерфейс событий от коннектора интеллекта.
    /// </summary>
    public interface IIntellectConnectorEvent
    {
        /// <summary>
        /// Соединение с интеллектом. ( только в случае асинхронной установки свзязи)
        /// </summary>
        /// <param name="sender">Инициализатор события</param>
        /// <param name="args">Пустой аргумент</param>
        void OnConnected(object sender, EventArgs args);
        /// <summary>
        /// Разединение с интеллектом. ( только в случае асинхронной установки свзязи)
        /// </summary>
        /// <param name="sender">Инициализатор события</param>
        /// <param name="args">Пустой аргумент</param>

        void OnDisconnected(object sender, EventArgs args);
        /// <summary>
        /// Получено сообщение от ядра.
        /// </summary>
        /// <param name="sender">Инифиализатор события</param>
        /// <param name="args">Аргумент сообщения. см ItvMessageEventArgs.</param>
        void OnMessage(object sender, ItvMessageEventArgs args);

        /// <summary>
        /// Получение кадров от видеоподсистемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnVideoFrame(object sender, ItvVideoFrameEventArgs args);
    }
}
