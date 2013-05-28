using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Ester.Model.Enums;

namespace Ester.Model.BaseClasses
{
    public class User : NotificationObject
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("Id"); }
        }

        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; RaisePropertyChanged("Login"); }
        }

        private string _passHash;
        public string PasswordHash
        {
            get { return _passHash; }
            set { _passHash = value; RaisePropertyChanged("PasswordHash"); }
        }

        private string _passSalt;
        public string PasswordSalt
        {
            get { return _passSalt; }
            set
            {
                _passSalt = value;
                RaisePropertyChanged("PasswordSalt");
            }
        }

        private string _sid;
        public string Sid
        {
            get { return _sid; }
            set
            {
                _sid = value;
                RaisePropertyChanged("Sid");
            }
        }

        private UserRoles _role;
        public UserRoles Role
        {
            get { return _role; }
            set
            {
                _role = value;
                RaisePropertyChanged("Role");
            }
        }

        private string _domain;
        public string Domain
        {
            get { return _domain; }
            set
            {
                _domain = value;
                RaisePropertyChanged("Domain");
            }
        }

        //тру если пользователь из актив директории
        private bool _isAdUser;
        public bool IsActiveDirectoryUser
        {
            get { return _isAdUser; }
            set 
            { 
                _isAdUser = value;
                RaisePropertyChanged("IsActiveDirectoryUser");
            }
        }

        public string ToLongUserString()
        {
            if (Domain+Login=="") return "Новый пользователь";
            return (IsActiveDirectoryUser)
                           ? Role.ToString() + ":" + Domain + "\\" + Login
                           : Role.ToString() + ":" + Login;
        }

        public override string ToString()
        {
            return (IsActiveDirectoryUser) ? Domain + "\\" + Login : Login;
        }

        public User()
        {
            InitializeFields();
        }

        private void InitializeFields()
        {
            Login = "";
            PasswordHash = "";
            PasswordSalt = "";
            Sid = "";
            Domain = "";
            Role = UserRoles.None;
            IsActiveDirectoryUser = false;
        }
    }
}
