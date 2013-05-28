using System;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ester.Model.Events;
using Ester.Model.Interfaces;
using Ester.Model.Services;
using Ester.Modules.Login.WindowsAuth;
using EsterCommon.Exceptions;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Ester.Modules.Login.ViewModel
{
    public delegate void SuccessLoginEventHandler();
    public delegate void FailedLoginEventHandler();

    public class LoginViewModel : NotificationObject
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IDataTransport _dataTransport;
        private readonly IServerInfo _serverInfo;
        private readonly EsterBootstrapper _esterBootstrapper;
        
        private string _status;
        private string _userName;
        private string _language;        
        private Visibility _loaderVisible = Visibility.Collapsed;

        #endregion

        #region Properties and Events

        public event SuccessLoginEventHandler SuccessLoginEvent;
        public void OnSuccessLoginEvent()
        {
            SuccessLoginEventHandler handler = SuccessLoginEvent;
            if (handler != null) handler();
        }

        public event FailedLoginEventHandler FailedLoginEvent;
        public void OnFailedLoginEvent()
        {
            FailedLoginEventHandler handler = FailedLoginEvent;
            if (handler != null) handler();
        }

        public DelegateCommand<object> LoginCommand { get; private set; }
        public DelegateCommand<object> LoginAdUserCommand { get; private set; }

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    RaisePropertyChanged("Language");
                }
            }
        }

        private bool _isViewEnabled = true;
        public bool IsViewEnabled
        {
            get { return _isViewEnabled; }
            set
            {
                if (_isViewEnabled != value)
                {
                    _isViewEnabled = value;
                    LoaderVisible = _isViewEnabled ? Visibility.Collapsed : Visibility.Visible;
                    RaisePropertyChanged("IsViewEnabled");
                }
            }
        }
        
        public Visibility LoaderVisible
        {
            get { return _loaderVisible; }
            set
            {
                if (_loaderVisible != value)
                {
                    _loaderVisible = value;
                    RaisePropertyChanged("LoaderVisible");
                }
            }
        }

        public string CurrentUser
        {
            get
            {
                var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
                if (windowsIdentity != null)
                    return "войти как " + windowsIdentity.Name;
                return "";
            }
        }

        private ObservableCollection<string> _bootLog = new ObservableCollection<string>();
        public ObservableCollection<string> BootLog
        {
            get { return _bootLog; }
            set
            {
                if (_bootLog == value) return;
                _bootLog = value;
                RaisePropertyChanged("BootLog");
            }
        }

        #endregion

        public LoginViewModel(IEventAggregator eventAggregator, IDataTransport dataTransport, IServerInfo serverInfo, EsterBootstrapper esterBootstrapper)
        {
            _esterBootstrapper = esterBootstrapper;
            _esterBootstrapper.BootProgressChangedEvent += OnBootProgressChanged;
            _eventAggregator = eventAggregator;
            _dataTransport = dataTransport;
            _serverInfo = serverInfo;

            LoginCommand = new DelegateCommand<object>(Login);
            LoginAdUserCommand = new DelegateCommand<object>(LoginAdUser);

            Language = InputLanguageManager.Current.CurrentInputLanguage.TwoLetterISOLanguageName.ToUpper();
            InputLanguageManager.Current.InputLanguageChanged += InputLanguageChanged;

            _eventAggregator.GetEvent<ApplicationLoadedEvent>().Subscribe(OnApplicationLoaded, ThreadOption.UIThread);

            ////todo: убрать на продакшн
            UserName = "Admin";
        }

        private void OnBootProgressChanged(string message)
        {
            BootLog.Insert(0, message);
        }

        private void OnApplicationLoaded(object obj)
        {
            IsViewEnabled = true;
            OnSuccessLoginEvent();
        }

        private void InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
            Language = e.NewLanguage.TwoLetterISOLanguageName.ToUpper();
        }                

        private void Login(object pbox)
        {
            IsViewEnabled = false;            

            string password = ((PasswordBox)pbox).Password;
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(UserName))
            {
                OnFailedLoginEvent();
                IsViewEnabled = true;
                return;
            }

            SessionInfo.IsAdmin = UserName.ToLower() == "admin";
            
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            string requestString = string.Format(
                "/{0}?user={1}&pass={2}",
                _serverInfo.CommonLoginQuery,
                HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(UserName)),
                HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(password)));

            LoginRequest(requestString);            
        }
        
        private void LoginAdUser(object obj)
        {
            Status = string.Empty;
            try
            {
                var client = new WindowsAuthServiceClient("Endpoint",new EndpointAddress(new Uri(_serverInfo.CommonWinAuthAddress)));
                var apiKey = client.GetApiKey();
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    Guid key;
                    if (Guid.TryParse(apiKey, out key))
                    {
                        _esterBootstrapper.BeginLoad(key);
                    }
                }
                else
                {
                    IsViewEnabled = true;
                    Status = "Произошла ошибка на сервере. Попробуйте позднее или обратитесь к администратору.";
                }
            }
            catch (Exception)
            {
                IsViewEnabled = true;
                Status = "Не удалось установить связь с сервером. Попробуйте позднее или обратитесь к администратору.";
            }
        }

        private async void LoginRequest(string requestString)
        {
            Status = string.Empty;
            try
            {
                var apiKey = await _dataTransport.GetRequestAsync<string>(requestString, false);
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    Guid key;
                    if (Guid.TryParse(apiKey, out key))
                    {
                        _esterBootstrapper.BeginLoad(key);
                    }
                    else
                    {
                        Status = "Ошибка. Неверный ApiKey";
                    }
                }
                else
                {
                    IsViewEnabled = true;
                    Status = "Произошла ошибка на сервере. Попробуйте позднее или обратитесь к администратору.";
                }
            }
            catch (BadRequestException)
            {
                IsViewEnabled = true;
                Status = "Неверный логин или пароль";
            }
            catch (Exception)
            {
                IsViewEnabled = true;
                Status = "Не удалось установить связь с сервером. Попробуйте позднее или обратитесь к администратору.";
            }            
        }
    }
}
