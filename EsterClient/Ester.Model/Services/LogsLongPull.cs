﻿using System;
﻿using System.ComponentModel;
﻿using System.Threading;
﻿using System.Threading.Tasks;
﻿using System.Windows;
﻿using Ester.Model.Enums;
﻿using Ester.Model.Events;
using Ester.Model.Interfaces;
﻿using EsterCommon.BaseClasses;
﻿using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace Ester.Model.Services
{
   //лонг-пулл сервис логов

    public delegate void LogItemRecievedHandler(LogItem item); 

    public static class LogsLongPull
    {
        private const string LogsNotificationsQuery = Urls.Logs + "/longpull";

        private static readonly IDataTransport DataTransport;
        private static readonly IEventAggregator EventAggregator;

        public static event LogItemRecievedHandler LogItemRecivedEvent;

        public static bool IsStarted { get; private set; }

        static LogsLongPull()
        {
            DataTransport = CommonInstances.UnityContainer.Resolve<IDataTransport>();
            EventAggregator = CommonInstances.UnityContainer.Resolve<IEventAggregator>();
        }
        
        public static void Start()
        {
            if (IsStarted) return;
            IsStarted = true;
            Task.Factory.StartNew(PullLogs, TaskCreationOptions.LongRunning);
        }

        public static void Stop()
        {
            IsStarted = false;
        }

        private static void PullLogs()
        {
            try
            {
                var item = DataTransport.GetRequest<LogItem>(LogsNotificationsQuery, true, 120000);
                if (!item.Equals(new LogItem()))
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => LogItemRecivedEvent(item)));
                if (IsStarted) PullLogs();
            }
            catch (Exception ex)
            {
                EventAggregator.GetEvent<ShowErrorEvent>().Publish(new Exception("Ошибка во время выполнения подписки на события.", ex));
                Thread.Sleep(60000);
                if (IsStarted) PullLogs();
            }   
        }
    }
}
