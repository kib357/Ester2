using System.DirectoryServices;

namespace EsterServer.Modules.Users.ActiveDirectory
{
    class ActiveDirectoryWatcher
    {
        private DirectoryEntry _dirEntry;

        public int Interval { get; set; }


        private string _domain;
        public string Domain
        {
            get { return _domain; }
            set
            {
                _domain = value;
                UpdateDirectoryEntry();
            }
        }

        private string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                UpdateDirectoryEntry();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                UpdateDirectoryEntry();
            }
        }

        private void UpdateDirectoryEntry()
        {
            _dirEntry = new DirectoryEntry(@"LDAP://" + Domain, Login, Password);
        }

        public ActiveDirectoryWatcher(string domain, string login, string password, int interval)
        {
            Domain = domain;
            Login = login;
            Password = password;
            Interval = interval;
            UpdateDirectoryEntry();
        }


    }
}
