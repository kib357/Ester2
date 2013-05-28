using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;

namespace EsterServer.Modules.BacNetServer.Notifications
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataPusher
    {
        private readonly AutoResetEvent _ev = new AutoResetEvent(false);
        private string _message = string.Empty;

        //∆дем врем€, указанное в Comet.TimeOut, если в течении этого времени не был вызван метод Comet.SetEvent(clientId), то выдаем ошибку по тайм ауту
        [OperationContract]
        [WebGet(UriTemplate = "/subscribe/{clientId}", ResponseFormat = WebMessageFormat.Json)]
        public string Notification(string clientId)
        {
            string arg;

            Comet.RegisterCometInstance(clientId, this);

            if (_ev.WaitOne(Comet.TimeOut))
            {
                lock (typeof(Comet))
                {
                    arg = _message;
                }
            }
            else
                arg = string.Format("Timeout Elapsed {0}", clientId);

            Comet.UnregisterCometInstance(clientId);
            return arg;
        }

        internal void SetEvent(string message)
        {
            _message = message;
            _ev.Set();
        }
    }
}
