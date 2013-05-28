using System;
using VideoServer.Model;

namespace VideoServer.Interfaces
{
    /// <summary>
    /// ��������� ������� �� ���������� ����������.
    /// </summary>
    public interface IIntellectConnectorEvent
    {
        /// <summary>
        /// ���������� � �����������. ( ������ � ������ ����������� ��������� ������)
        /// </summary>
        /// <param name="sender">������������� �������</param>
        /// <param name="args">������ ��������</param>
        void OnConnected(object sender, EventArgs args);
        /// <summary>
        /// ����������� � �����������. ( ������ � ������ ����������� ��������� ������)
        /// </summary>
        /// <param name="sender">������������� �������</param>
        /// <param name="args">������ ��������</param>

        void OnDisconnected(object sender, EventArgs args);
        /// <summary>
        /// �������� ��������� �� ����.
        /// </summary>
        /// <param name="sender">������������� �������</param>
        /// <param name="args">�������� ���������. �� ItvMessageEventArgs.</param>
        void OnMessage(object sender, ItvMessageEventArgs args);

        /// <summary>
        /// ��������� ������ �� ���������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnVideoFrame(object sender, ItvVideoFrameEventArgs args);
    }
}
