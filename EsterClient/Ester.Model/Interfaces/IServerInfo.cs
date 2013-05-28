using System;
using Ester.Model.BaseClasses;

namespace Ester.Model.Interfaces
{
    public interface IServerInfo
    {
        void Initialize();
        String CommonLoginQuery { get; set; }
        String CommonServerAddress { get; set; }
        String CommonWinAuthAddress { get; set; }
        bool CommonPinContext { get; set; }
        int BacnetInterval { get; set; }
        int BacnetAlarmsInterval { get; set; }
        int UpdateInterval { get; set; }
        double UpdateVersion { get; set; }
        bool UpdateNeed { get; set; }
        string AbbyyPath { get; set; }
        string AccessCardReaderPort { get; set; }
    }
}