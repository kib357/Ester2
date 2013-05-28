using System;

namespace EsterCommon.BaseClasses
{
    public class LogItem
    {
        public LogItem()
        {
            TimeStamp = DateTime.MinValue;
        }
        public string Logger { get; set; }
        public string LogLevel { get; set; }
        public DateTime TimeStamp { get; set; }
        public string EsterUser { get; set; }
        public string Address { get; set; }
        public string Value { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}