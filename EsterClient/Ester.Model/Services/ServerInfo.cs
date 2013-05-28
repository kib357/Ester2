using System;
using Ester.Model.BaseClasses;
using Ester.Model.Interfaces;
using Nini.Config;

namespace Ester.Model.Services
{
    public class ServerInfo : IServerInfo
    {
        public static XmlConfigSource ConfigSource { get; set; }

        public void Initialize()
        {
            ConfigSource = new XmlConfigSource("Config.xml") { AutoSave = true };
        }

        public String CommonLoginQuery
        {
            get { return ConfigSource.Configs["Common"].Get("LoginQuery"); }
            set { ConfigSource.Configs["Common"].Set("LoginQuery", value); }
        }

        public String CommonServerAddress
        {
            get { return ConfigSource.Configs["Common"].Get("ServerAddress").TrimEnd('/'); }
            set { ConfigSource.Configs["Common"].Set("ServerAddress", value); }
        }

        public String CommonWinAuthAddress
        {
            get { return ConfigSource.Configs["Common"].Get("WinAuthAddress").TrimEnd('/'); }
            set { ConfigSource.Configs["Common"].Set("WinAuthAddress", value); }
        }

        public bool CommonPinContext
        {
            get { return bool.Parse(ConfigSource.Configs["Common"].Get("PinContext")); }
            set { ConfigSource.Configs["Common"].Set("PinContext", value); }
        }

        public int BacnetInterval
        {
            get { return int.Parse(ConfigSource.Configs["BacNet"].Get("Interval")); }
            set { ConfigSource.Configs["BacNet"].Set("Interval", value); }
        }

        public int BacnetAlarmsInterval
        {
            get { return int.Parse(ConfigSource.Configs["BacNet"].Get("AlarmsInterval")); }
            set { ConfigSource.Configs["BacNet"].Set("AlarmsInterval", value); }
        }

        public int UpdateInterval
        {
            get { return int.Parse(ConfigSource.Configs["Update"].Get("Interval")); }
            set { ConfigSource.Configs["Update"].Set("Interval", value); }
        }

        public double UpdateVersion
        {
            get { return double.Parse(ConfigSource.Configs["Update"].Get("Version")); }
            set { ConfigSource.Configs["Update"].Set("Version", value); }
        }

        public bool UpdateNeed
        {
            get { return bool.Parse(ConfigSource.Configs["Update"].Get("NeedUpdate")); }
            set { ConfigSource.Configs["Update"].Set("NeedUpdate", value); }
        }

        public string AbbyyPath
        {
            get { return ConfigSource.Configs["Abbyy"].Get("Path"); }
            set { ConfigSource.Configs["Abbyy"].Set("Path", value); }
        }

        public string AccessCardReaderPort
        {
            get { return ConfigSource.Configs["AccessCardReader"].Get("Port"); }
            set { ConfigSource.Configs["AccessCardReader"].Set("Port", value); }
        }

    }
}
