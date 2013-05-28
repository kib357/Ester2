using System.IO;
using System.Linq;
using System.Web;
using BACsharp;
using BACsharp.AppService.UnconfirmedServices;
using EsterServer.Model;
using EsterServer.Model.Data;
using NLog;
using Nini.Config;

namespace EsterServer.Modules.BacNetServer.Notifications
{
    public class NotificationListener
    {
        private readonly XmlConfigSource _configSource = new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };
        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

        public NotificationListener()
        {
            //BacNetServer.Network.NotificationEvent += OnNotificationReceived;
        }

        private void OnNotificationReceived(UnconfirmedEventNotificationRequest notification)
        {
            //if(notification.ObjectId.ObjectType == (decimal) BacnetObjectType.Door)
                //OnDoorNotificationReceived(notification);
            if(notification.ObjectId.ObjectType == (decimal)BacnetObjectType.AccessControlEventLog)
                OnCELNotificationReceived(notification);
        }

        private void OnCELNotificationReceived(UnconfirmedEventNotificationRequest notification)
        {
            string loggedMessage = string.Empty;
            string message = notification.MessageText.ToString();
            var splittedMessage = message.Split(':');
            if (splittedMessage.Length != 2) return;

            string messageType = splittedMessage[0].Remove(splittedMessage[0].LastIndexOf('(')).ToLower();
            var messageParams = splittedMessage[1].Split(',');
            if (messageParams.Length < 1) return;           

            if (messageType.Contains("request to exit"))
                loggedMessage = messageParams[0] + ";Запрос выхода";

            if (messageType.Contains("valid access") && messageParams.Length > 1)
                loggedMessage = messageParams[0] + ";Разрешён доступ;" + messageParams[1];

            if (messageType.Contains("invalid zone access") && messageParams.Length > 1)
                loggedMessage = messageParams[0] + ";Попытка доступа в неразрешенную зону;" + messageParams[1];

            if (messageType.Contains("unrecognized card") && messageParams.Length > 2)
                loggedMessage = messageParams[0] + ";Неизвесная карта;" + messageParams[2];

            if (messageType.Contains("disabled card") && messageParams.Length > 2)
                loggedMessage = messageParams[0] + ";Попытка прохода по заблокированной карте;" + 
                               messageParams[1] + " - " + messageParams[2];

            if (messageType.Contains("lost card") && messageParams.Length > 2)
                loggedMessage = messageParams[0] + ";Попытка прохода по утерянной карте;" +
                               messageParams[1] + " - " + messageParams[2];

            if (messageType.Contains("manual unlocked") && messageParams.Length > 2)
                loggedMessage = messageParams[0] + ";Ручное открытие;" +
                               messageParams[1] + " - " + messageParams[2];

            if (string.IsNullOrWhiteSpace(loggedMessage)) return;
            Comet.SetEvent(loggedMessage);

            var notify = loggedMessage.Split(';');
            int id;
            if (notify.Length == 3 && notify[2].Contains("CU") && int.TryParse(notify[2].Remove(0, 2), out id))
            {
                /*var person = dataContext.Persons.FirstOrDefault(s => s.ID == id);
                if (person != null)
                    notify[2] = person.LastName + " " + person.FirstName;
                loggedMessage = string.Join(";", notify);*/
            }
            var myEvent = new LogEventInfo(LogLevel.Info, NLogger.Name, loggedMessage);
                myEvent.Properties.Add("address", messageParams[0]);
            NLogger.Log(myEvent);
        }

        private void OnDoorNotificationReceived(UnconfirmedEventNotificationRequest notification)
        {
            string message = notification.MessageText.ToString().ToLower();
            string door = notification.DeviceId + ".DC" + notification.ObjectId;

            if (message.Contains("access granted"))
                AccessGrantedEvent(door, message);

            if (message.Contains("request to exit"))
                RequestToExitEvent(door);

            if (message.Contains("unrecognized card"))
                UnrecognizedCardEvent(door, message);

            if (message.Contains("disabled card"))
                DisabledCardEvent(door, message);

            if (message.Contains("lost card"))
                LostCardEvent(door, message);

            if (message.Contains("invalid zone access"))
                InvalidZoneAccessEvent(door, message);
            //message Text: ANSI X3.4 '200.DC201(200.DC201)..Disabled Card for <.............(CU1) Site=181 Card=19511'
            //message Text: ANSI X3.4 '200.DC201(200.DC201)..Invalid Zone Access <.............(CU1)'
        }

        private void InvalidZoneAccessEvent(string door, string message)
        {
            int index = message.LastIndexOf("(CU", System.StringComparison.Ordinal);
            if (index > 0)
            {
                string user = message.Substring(index + 3).Remove(message.Substring(index + 3).LastIndexOf(')'));
                Comet.SetEvent(door + ";Попытка доступа в неразрешенную зону;" + user);
            }
        }

        private void LostCardEvent(string door, string message)
        {
            Comet.SetEvent(door + ";Попытка прохода по утерянной карте;" + message.Substring(message.LastIndexOf(')') + 2));
        }

        private void DisabledCardEvent(string door, string message)
        {
            int index = message.LastIndexOf("(CU", System.StringComparison.Ordinal);
            if (index > 0)
            {
                string user = message.Substring(index + 3).Remove(message.Substring(index + 3).LastIndexOf(')'));
                Comet.SetEvent(door + ";Попытка прохода по заблокированной карте;" + user + ";" + message.Substring(message.LastIndexOf("site=", System.StringComparison.Ordinal)));
            }
        }

        private void UnrecognizedCardEvent(string door, string message)
        {
            Comet.SetEvent(door + ";Неизвесная карта;" + message.Substring(message.LastIndexOf("site=", System.StringComparison.Ordinal)));
        }

        private void RequestToExitEvent(string door)
        {
            Comet.SetEvent(door + ";Запрос выхода");
        }

        private void AccessGrantedEvent(string door, string message)
        {
            int index = message.LastIndexOf("(CU", System.StringComparison.Ordinal);
            if (index > 0)
            {
                string user = message.Substring(index + 3).Remove(message.Substring(index + 3).LastIndexOf(')'));
                Comet.SetEvent(door + ";Разрешён доступ;" + user);
            }
        }
    }
}
