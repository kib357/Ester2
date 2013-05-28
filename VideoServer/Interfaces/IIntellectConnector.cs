using System;
using VideoServer.Model;

namespace VideoServer.Interfaces
{
    /// <summary>
    /// ��������� ������� ����������.
    /// </summary>
    public interface IIntellectConnector
    {
        /// <summary>
        /// ���������� � �����.
        /// </summary>
        event EventHandler<EventArgs> OnConnected;
        /// <summary>
        /// ������������ �� ����.
        /// </summary>
        event EventHandler<EventArgs> OnDisconnected;
        /// <summary>
        /// ������ ��������� �� ����.
        /// </summary>
        event EventHandler<ItvMessageEventArgs> OnMessage;

        /// <summary>
        /// ��������� ������ �� ���������������
        /// </summary>
        event EventHandler<ItvVideoFrameEventArgs> OnVideoFrame;

        /// <summary>
        /// ����������� � �����
        /// </summary>
        /// <param name="ip">IP ����� ����. ������ �����: 255.255.255.255</param>
        /// <param name="port">���� ���� �� 1 �� 65535</param>
        /// <param name="id">������������.</param>
        /// <returns>���������� 0 ���� ��� ������, ����� ������ �������.</returns>
        int Connect(string ip, int port, string id);

        /// <summary>
        /// ������ ������� ��������� ������
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="id"></param>
        void AsyncConnect(string ip, int port, string id);

        /// <summary>
        /// ������ ������� ���������  ������ � �������������� ���������� ���������������
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        void AsyncConnectUnique(string ip, int port);

        /// <summary>
        /// �������� ��������� � ����.
        /// </summary>
        /// <param name="message">��������� ��������� � ������� ���������.</param>
        /// <returns>?</returns>
        int Send(string message);

        /// <summary>
        /// �������� ������� � ����.
        /// </summary>
        /// <param name="message">��������� ��������� � ������� ���������.</param>
        /// <returns>?</returns>
        int DoReact(string message);

        /// <summary>
        /// ����������� �� �������.
        /// </summary>
        /// <returns>���������� 0 ���� ������ ����� �������� ������.</returns>
        int Disconnect();


        /// <summary>
        /// ���������� ������� ����� � ��������
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// ����������, �������� ������� ����������� ��� ���
        /// </summary>
        bool ReceiveBitmap { set; get; }

    }
}
