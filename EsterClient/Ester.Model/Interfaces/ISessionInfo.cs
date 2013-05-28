using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ester.Model.Enums;
using Ester.Model.Services;

namespace Ester.Model.Interfaces
{
    public interface ISessionInfo
    {
        Guid ApiKey { get; set; }
        AuthStates AuthState { get; set; }
    }
}
