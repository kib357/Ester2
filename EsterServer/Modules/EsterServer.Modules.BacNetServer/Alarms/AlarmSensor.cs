using System;
using NLog;

namespace EsterServer.Modules.BacNetServer.Alarms
{
    class AlarmSensor
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string Address { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string ErrorValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public int ErrorCode { get; set; }
        public int Priority { get; set; }

        public AlarmSensor()
        {
            BacNetServer.ValuesChanged += OnValuesChanged;
            AlarmChecker.ReloadDictionaries += OnReloadDictionaries;
        }

        private void OnReloadDictionaries(bool reloading)
        {
            if (reloading)
                BacNetServer.ValuesChanged -= OnValuesChanged;
            else
                BacNetServer.ValuesChanged += OnValuesChanged;
        }

        private void OnValuesChanged(string address, string oldvalue, string newvalue)
        {
            if(Address.Trim() != address) return;

            if (ErrorValue != string.Empty && ErrorValue == newvalue)
            {
                CreateAlarm();
                return;
            }

            double newDoubleValue, oldDoubleValue;
            if (double.TryParse(newvalue, out newDoubleValue))
            {
                if (newDoubleValue > MaxValue)
                {
                    if (double.TryParse(oldvalue, out oldDoubleValue) && (oldDoubleValue < MinValue || (oldDoubleValue != newDoubleValue && oldDoubleValue > MaxValue)))
                    {
                        CreateAlarm(1, newDoubleValue, true);
                        return;
                    }
                    CreateAlarm(1, newDoubleValue);
                    return;
                }

                if (newDoubleValue < MinValue)
                {
                    if (double.TryParse(oldvalue, out oldDoubleValue) && (oldDoubleValue > MaxValue || (oldDoubleValue != newDoubleValue && oldDoubleValue < MinValue)))
                    {
                        CreateAlarm(2, newDoubleValue, true);
                        return;
                    }
                    CreateAlarm(2, newDoubleValue);
                    return;
                }
            }

            if (AlarmChecker.CurrentAlarmList.ContainsKey(Address))
            {
                AlarmChecker.CurrentAlarmList.Remove(Address);

                var alarmEvent = new LogEventInfo(LogLevel.Info, _logger.Name, "Аварийная ситуация устранена: " + Description);
                alarmEvent.Properties.Add("address", Address);
                alarmEvent.Properties.Add("value", Value);
                _logger.Log(alarmEvent);
            }
        }

        private void CreateAlarm(byte showValues = 0, double newValue = 0, bool force = false)
        {
            string fullDescription = Description;
            if (showValues == 1)
                fullDescription += (Description[Description.Length - 1] == '.' ? "" : ".") + " Текущее значение: " + Math.Round(newValue, 2) + ", Максимальное: " + Math.Round(MaxValue, 2);
            if (showValues == 2)
                fullDescription += (Description[Description.Length - 1] == '.' ? "" : ".") + " Текущее значение: " + Math.Round(newValue, 2) + ", Минимальное: " + Math.Round(MinValue, 2);

            if (force && AlarmChecker.CurrentAlarmList.ContainsKey(Address))
                AlarmChecker.CurrentAlarmList.Remove(Address);

            if (!AlarmChecker.CurrentAlarmList.ContainsKey(Address))
            {
                AlarmChecker.CurrentAlarmList.Add(Address, fullDescription + "|" + Priority);

                var alarmEvent = new LogEventInfo(LogLevel.Warn, _logger.Name, fullDescription);
                alarmEvent.Properties.Add("address", Address);
                alarmEvent.Properties.Add("value", Value);
                _logger.Log(alarmEvent);
            }
        }
    }
}
