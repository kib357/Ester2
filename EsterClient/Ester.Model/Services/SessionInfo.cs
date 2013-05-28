using System;
using Ester.Model.Enums;
using Ester.Model.Interfaces;

namespace Ester.Model.Services
{
    //информация о текущей сессии

    public class SessionInfo : ISessionInfo
    {
        public Guid ApiKey { get; set; }

        private AuthStates _authState;
        public AuthStates AuthState
        {
            get { return _authState; }
            set { _authState = value;}
        }

        public static bool IsAdmin { get; set; }

        public SessionInfo()
        {
            ApiKey = Guid.Empty;
            _authState = AuthStates.NonAuthentiticated;
        }
    }
}
