using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ester.Model.Services;

namespace Ester.Model.Interfaces
{
    public interface IAccessCardReader
    {
        String Port { get; set; }
        bool IsOpened { get; }
        void Open();
        void Close();
        OnReceived Received { get; set; }
    }
}
