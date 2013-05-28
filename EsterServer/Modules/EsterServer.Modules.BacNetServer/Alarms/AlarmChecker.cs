using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;
using EsterServer.Model.Ioc;
using NLog;
using Nini.Config;

namespace EsterServer.Modules.BacNetServer.Alarms
{
    public delegate void ReloadDictionariesEventHandler(bool reloading);

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [UnityServiceBehavior]
    public class AlarmChecker
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly XmlConfigSource _configSource = new XmlConfigSource(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\ServerConfig.xml")) { AutoSave = true };

        public static event ReloadDictionariesEventHandler ReloadDictionaries;
        private readonly FileSystemWatcher _watcher;
        private List<AlarmSensor> _alarmSensors;
        public static Dictionary<string, string> CurrentAlarmList = new Dictionary<string, string>();

        public AlarmChecker()
        {
            _watcher = new FileSystemWatcher(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\AlarmDictionary"));
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.IncludeSubdirectories = true;
            _watcher.Created += WatcherEvent;
            _watcher.Changed += WatcherEvent;
            _watcher.Deleted += WatcherEvent;

            _alarmSensors = LoadAlarmSensors();
            BacNetServer.ValuesChanged += OnValuesChanged;

            _watcher.EnableRaisingEvents = true;
        }

        private void WatcherEvent(object sender, FileSystemEventArgs e)
        {
            BacNetServer.ValuesChanged -= OnValuesChanged;
            if (ReloadDictionaries != null)
                ReloadDictionaries(true);
            _alarmSensors = LoadAlarmSensors();
            if (ReloadDictionaries != null)
                ReloadDictionaries(false);
            BacNetServer.ValuesChanged += OnValuesChanged;
        }

        private void OnValuesChanged(string address, string oldvalue, string newvalue)
        {
            //а тут мы обрабатываем событие
        }

        private List<AlarmSensor> LoadAlarmSensors()
        {
            List<AlarmSensor> res = new List<AlarmSensor>();
            try
            {
                XDocument sensorConfig = XDocument.Load(Path.Combine(HttpRuntime.AppDomainAppPath, _configSource.Configs["BacNet"].Get("ErrorFileUrl")));
                if (sensorConfig.Root != null)
                    foreach (XElement error in sensorConfig.Root.Elements())
                    {
                        AlarmSensor sensor = new AlarmSensor();
                        if (error.Attribute("Address") != null)
                        {
                            sensor.Address = (string) error.Attribute("Address");
                            if (error.Attribute("Value") != null)
                                sensor.Value = (string) error.Attribute("Value");
                            if (error.Attribute("Description") != null)
                                sensor.Description = (string) error.Attribute("Description");
                            if (error.Attribute("ErrorValue") != null)
                                sensor.ErrorValue = (string) error.Attribute("ErrorValue");
                            if (error.Attribute("MaxValue") != null)
                                sensor.MaxValue = (double)error.Attribute("MaxValue");
                            else
                                sensor.MaxValue = double.MaxValue;
                            if (error.Attribute("MinValue") != null)
                                sensor.MinValue = (double) error.Attribute("MinValue");
                            else
                                sensor.MinValue = double.MinValue;
                            if (error.Attribute("ErrorCode") != null)
                                sensor.ErrorCode = (int) error.Attribute("ErrorCode");
                            if (error.Attribute("Priority") != null)
                                sensor.Priority = (int)error.Attribute("Priority");
                        }
                        if (error.Attribute("ErrorValue") != null || error.Attribute("MaxValue") != null || error.Attribute("MinValue") != null)
                        {
                            res.Add(sensor);
                            BacNetServer.SubscribeToBacnetCOV(sensor.Address);
                        }
                    }
            }
            catch (Exception)
            {
                //_logger.ErrorException("Не удалось загрузить cписок датчиков аварийных ситуаций. Сообщение:", ex);
            }
            return res;
        }

        [WebGet(UriTemplate = "", ResponseFormat = WebMessageFormat.Json)]//, BodyStyle = WebMessageBodyStyle.Bare)]
        private Dictionary<string, string> GetErrorsList()
        {
            return CurrentAlarmList;
        }
    }
}
