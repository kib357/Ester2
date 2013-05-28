using System;
using System.Collections.Generic;

namespace EsterServer.Modules.Data.Notifications
{
    public class Comet
    {
        private static readonly Dictionary<string, DataPusher> ClientList = new Dictionary<string, DataPusher>();

        public static TimeSpan TimeOut = new TimeSpan(0, 0, 60);

        internal static void RegisterCometInstance(string clientId, DataPusher cometSvc)
        {
            lock (typeof(Comet))
            {
                ClientList[clientId] = cometSvc;
            }
        }

        internal static void UnregisterCometInstance(string clientId)
        {
            lock (typeof(Comet))
            {
                if (ClientList.ContainsKey(clientId))
                    ClientList.Remove(clientId);
            }
        }

        public static void SetEvent(string clientId, string message)
        {
            DataPusher cometSvc;

            lock (typeof(Comet))
            {
                ClientList.TryGetValue(clientId, out cometSvc);
            }

            if (cometSvc != null)
                cometSvc.SetEvent(message);
        }

        public static void SetEvent(string message)
        {
            lock (typeof(Comet))
            {
                foreach (var dataPusher in ClientList)
                {
                    dataPusher.Value.SetEvent(message);
                }
            }
        }
    }
}
