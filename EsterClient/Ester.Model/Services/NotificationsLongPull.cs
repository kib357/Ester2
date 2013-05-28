﻿using System;
﻿using System.ComponentModel;
﻿using System.Threading;
﻿using System.Threading.Tasks;
﻿using System.Windows;
﻿using Ester.Model.Enums;
﻿using Ester.Model.Events;
using Ester.Model.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace Ester.Model.Services
{
   //лонг-пулл сервис нотификаций

    public delegate void NotificationRecievedHandler(string message); 

    public static class NotificationsLongPull
    {
        private const string PullNotificationsQuery = Urls.Notifications + "/subscribe/";

        private static readonly IDataTransport DataTransport;
        private static readonly IEventAggregator EventAggregator;

        public static event NotificationRecievedHandler NotificationRecivedEvent;

        public static bool IsStarted { get; private set; }

        static NotificationsLongPull()
        {
            DataTransport = CommonInstances.UnityContainer.Resolve<IDataTransport>();
            EventAggregator = CommonInstances.UnityContainer.Resolve<IEventAggregator>();  
        }
        
        public static void Start()
        {
            if (IsStarted) return;
            IsStarted = true;
            Task.Factory.StartNew(PullNotifications, TaskCreationOptions.LongRunning);
        }

        public static void Stop()
        {
            IsStarted = false;
        }

        private static void PullNotifications()
        {
            try
            {
                var message = DataTransport.GetRequest<string>(PullNotificationsQuery + Guid.NewGuid(), true, 120000);
                if (!message.Contains("Timeout"))
                    if (NotificationRecivedEvent != null)
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => NotificationRecivedEvent(message)));
                if (IsStarted) PullNotifications();
            }
            catch (Exception ex)
            {
                EventAggregator.GetEvent<ShowErrorEvent>().Publish(new Exception("Ошибка при попытке получения уведомлений.", ex));
                Thread.Sleep(60000);
                if (IsStarted) PullNotifications();
            }

            
        }
    }
}
