using System;
using VideoServer.Model;

namespace VideoServer.Interfaces
{
    /// <summary>
    /// Интерфейс событий соединения.
    /// </summary>
    public interface IIntellectConnector
    {
        /// <summary>
        /// Соединение с ядром.
        /// </summary>
        event EventHandler<EventArgs> OnConnected;
        /// <summary>
        /// Отсоединения от ядра.
        /// </summary>
        event EventHandler<EventArgs> OnDisconnected;
        /// <summary>
        /// Пришло сообщение от ядра.
        /// </summary>
        event EventHandler<ItvMessageEventArgs> OnMessage;

        /// <summary>
        /// Получение кадров от видеоподсистемы
        /// </summary>
        event EventHandler<ItvVideoFrameEventArgs> OnVideoFrame;

        /// <summary>
        /// Соединиться с ядром
        /// </summary>
        /// <param name="ip">IP адрес ядра. Формат маски: 255.255.255.255</param>
        /// <param name="port">порт ядра от 1 до 65535</param>
        /// <param name="id">Идетификатор.</param>
        /// <returns>Возвращает 0 если все удачно, иначе ошибки видновс.</returns>
        int Connect(string ip, int port, string id);

        /// <summary>
        /// Начать процесс установки свзязи
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="id"></param>
        void AsyncConnect(string ip, int port, string id);

        /// <summary>
        /// Начать процесс установки  свзязи с гарантированно уникальным идентификатором
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        void AsyncConnectUnique(string ip, int port);

        /// <summary>
        /// Отослать сообщение в ядро.
        /// </summary>
        /// <param name="message">Текстовое сообщение в формате интеллект.</param>
        /// <returns>?</returns>
        int Send(string message);

        /// <summary>
        /// Отослать реакцию в ядро.
        /// </summary>
        /// <param name="message">Текстовое сообщение в формате интеллект.</param>
        /// <returns>?</returns>
        int DoReact(string message);

        /// <summary>
        /// Отключиться от сервера.
        /// </summary>
        /// <returns>Возвращает 0 если удачно иначе виндовые ошибки.</returns>
        int Disconnect();


        /// <summary>
        /// Определяет наличие сзязи с сервером
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// Определяет, получать готовое изображение или нет
        /// </summary>
        bool ReceiveBitmap { set; get; }

    }
}
