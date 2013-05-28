//===============================================================================================
// Класс предоставляет интерфейс клиентской части к серверам на базе 
// транспорта ITV(посредством COM компонента iidk.ocx)
//===============================================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using VideoServer.Helpers;
using VideoServer.Interfaces;


namespace VideoServer.Model
{
    /// <summary>
    /// Класс соединитель с ядром :)
    /// </summary>
    public class IntellectConnector : IIntellectConnector, ISynchronizeInvoke, IDisposable
    {
        //то что коннектится к серверу
        private AxIIDK_COMLib.AxIIDK_COM axIIDK_connector;

        //Вся инициализация в конструкторе. Если что то не так то throw
        public IntellectConnector()
        {
            
            axIIDK_connector = new AxIIDK_COMLib.AxIIDK_COM();
            axIIDK_connector.CreateControl();
            axIIDK_connector.OnConnect += _iidk_OnConnected;
            axIIDK_connector.OnDisconnect += _iidk_OnDisconnected;
            axIIDK_connector.OnMessage += _iidk_OnMessage;
            axIIDK_connector.OnVideoFrame += _iidk_OnVideoFrame;
        }

        ~IntellectConnector()
        {
            axIIDK_connector.OnConnect -= _iidk_OnConnected;
            axIIDK_connector.OnDisconnect -= _iidk_OnDisconnected;
            axIIDK_connector.OnMessage -= _iidk_OnMessage;
            axIIDK_connector.OnVideoFrame -= _iidk_OnVideoFrame;
            Dispose(false);
        }

        public bool IsConnected()
        {
            return (axIIDK_connector.IsConnected() == 0);
        }

        //Освободить ресурсы
        public virtual void Dispose()
        {
            //GC.SuppressFinalize(this);
            //Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                axIIDK_connector.Disconnect();
                axIIDK_connector.Dispose();
            }
        }

        private bool _receiveBitmap;
        public bool ReceiveBitmap
        {
            set
            {
                _receiveBitmap = value;
                axIIDK_connector.Options = _receiveBitmap ? 1 : 0;
            }
            get
            {
                return _receiveBitmap;
            }
        }

        #region IIntellectConnectorEvent Members

        public virtual int Connect(string ip, int port, string id)
        {
            return axIIDK_connector.Connect(ip,port,id);
        }

        public void AsyncConnect(string ip, int port, string id)
        {
            axIIDK_connector.AsyncConnect(ip, port, id);
        }

        public void AsyncConnectUnique(string ip, int port)
        {
            axIIDK_connector.AsyncConnectUnique(ip, port);
        }

        public virtual int Disconnect()
        {
            return axIIDK_connector.Disconnect();
        }

        public virtual int Send(string message)
        {
            return axIIDK_connector.SendMsg(message);
        }

        public virtual int DoReact(string message)
        {
            return axIIDK_connector.DoReact(message);
        }

        #endregion
        #region IIDK Events
        public event EventHandler<EventArgs> OnConnected;

        public event EventHandler<EventArgs> OnDisconnected;

        public event EventHandler<ItvMessageEventArgs> OnMessage;

        public event EventHandler<ItvVideoFrameEventArgs> OnVideoFrame;

        //----------------------------------------------------------------------------------------
        private void _iidk_OnConnected(object sender, EventArgs e)
        {
            if (OnConnected != null)
            {
                OnConnected(this, e);
            }
        }
        //----------------------------------------------------------------------------------------
        private void _iidk_OnDisconnected(object sender, EventArgs e)
        {
            if (OnDisconnected != null)
            {
                OnDisconnected(this, e);
            }
        }
        //----------------------------------------------------------------------------------------
        private void _iidk_OnMessage(object sender, AxIIDK_COMLib._DIIDK_COMEvents_OnMessageEvent e)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new ItvMessageEventArgs(e.msg));
            }
        }
        //----------------------------------------------------------------------------------------
        private void _iidk_OnVideoFrame(object sender, AxIIDK_COMLib._DIIDK_COMEvents_OnVideoFrameEvent e)
        {
            if (OnVideoFrame != null)
            {
                DateTime timestamp = IIDKHelpers.GetDateTime(e.sysTime);
                Bitmap bmp = IIDKHelpers.GetBitmap(e.pImage);
                OnVideoFrame(this, new ItvVideoFrameEventArgs(e.subtitle, timestamp, bmp));
            }
        }
        //----------------------------------------------------------------------------------------
        #endregion    
        #region ISynchronizeInvoke Members

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return axIIDK_connector.BeginInvoke(method, args);
        }

        public object EndInvoke(IAsyncResult result)
        {
            return axIIDK_connector.EndInvoke(result);
        }

        public object Invoke(Delegate method, object[] args)
        {
            return axIIDK_connector.Invoke(method, args);
        }

        public bool InvokeRequired
        {
            get 
            {
                return axIIDK_connector.InvokeRequired;
            }
        }

        #endregion
    }
}
