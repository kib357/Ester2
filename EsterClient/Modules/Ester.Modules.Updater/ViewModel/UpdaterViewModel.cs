using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ester.Model.Events;
using Ester.Model.Interfaces;
using Ionic.Zip;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Ester.Modules.Updater.ViewModel
{
    public class UpdaterViewModel : NotificationObject, IEsterViewModel
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDataTransport _dataTransport;
        private IEventAggregator _eventAggregator;

        readonly string _dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private readonly string _updateDir =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Update";
        readonly string _zipFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Updates.zip";


        private Visibility _isVisible;
        private int _lastTime;
        const int AutoRestartTime = 60;

        private readonly DispatcherTimer _checkTimer;
        private readonly DispatcherTimer _restartTimer;
        private IServerInfo _serverInfo;
        private ISessionInfo _sessionInfo;

        public DelegateCommand<object> RestartAppCommand { get; private set; }
        public DelegateCommand<object> WaitForUpdateCommand { get; private set; }

        public UpdaterViewModel(IEventAggregator eventAggregator, IDataTransport dataTransport, IServerInfo serverInfo, ISessionInfo sessionInfo)
        {
            _eventAggregator = eventAggregator;

            _serverInfo = serverInfo;
            _sessionInfo = sessionInfo;

            // _isVisible = Visibility.Visible;
            _isVisible = Visibility.Collapsed;
            LastTime = AutoRestartTime;

            _dataTransport = dataTransport;

            RestartAppCommand = new DelegateCommand<object>(RestartApp);
            WaitForUpdateCommand = new DelegateCommand<object>(UpdateAbortedByUser);

            _checkTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, _serverInfo.UpdateInterval, 0) };
            _checkTimer.Tick += StartCheckUpdatesThread;
            _restartTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
            _restartTimer.Tick += RestartTimerTick;
        }

        public event ViewModelConfiguredEventHandler ViewModelConfiguredEvent;

        public void OnViewModelConfiguredEvent(IEsterViewModel sender)
        {
            ViewModelConfiguredEventHandler handler = ViewModelConfiguredEvent;
            if (handler != null) handler(sender);
        }

        private bool _isReady;
        public bool IsReady
        {
            get { return _isReady; }
        }
        public string Title { get { return "обновления программы"; } }

        /// <summary>
        /// При запуске приложения эта процедура проверят поле NeedUpdate в файле UpdaterSettings.xml. 
        /// Если это поле имеет значение true, то открывается апдейтер и закрывается приложение.
        /// Если значение - false, то стартует таймер, по срабатыванию таймера программа начинает проверять обновления.
        /// </summary>
        public async void Configure()
        {
            if (_serverInfo.UpdateNeed & File.Exists(_dir + "\\Updates.zip"))
            {
                StartUpdater();
            }
            else
            {
                _isReady = await CheckUpdatesAsync();
            }
            OnViewModelConfiguredEvent(this);
        }                

        private void StartCheckUpdatesThread(object sender, EventArgs e)
        {
            CheckUpdatesAsync();
        }

        private async Task<bool> CheckUpdatesAsync()
        {
            return await Task.Run(() => CheckUpdates());
        }

        private bool CheckUpdates()
        {
            if (_serverInfo.UpdateNeed)
            {
                UpdatesChecked(true);
                return false;
            }

            var queryUrl = "/updates/?client-version=" + _serverInfo.UpdateVersion;
            string updateUrl;
            try
            {
                updateUrl = _dataTransport.GetRequest<string>(queryUrl, false);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<ShowErrorEvent>().Publish(
                    new Exception("Ошибка при проверке обновлений.", ex)));
                _logger.ErrorException("Ошибка при проверке обновлений", ex);
                UpdatesChecked(false);
                return true;
            }
            if (!string.IsNullOrEmpty(updateUrl))
            {
                Stream responseStream;
                try
                {
                    WebRequest request = WebRequest.Create(_serverInfo.CommonServerAddress + updateUrl + "?apikey=" + _sessionInfo.ApiKey);
                    WebResponse response = request.GetResponse();
                    responseStream = response.GetResponseStream();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => _eventAggregator.GetEvent<ShowErrorEvent>().Publish(
                        new Exception("Ошибка при загрузке обновлений.", ex)));
                    _logger.ErrorException("Ошибка при загрузке обновления", ex);
                    UpdatesChecked(false);
                    return true;
                }
                if (responseStream == null) return true;
                var unzipped = UnZipUpdate(responseStream);
                UpdatesChecked(unzipped);
                return !unzipped;
            }
            return true;
        }

        private bool UnZipUpdate(Stream responseStream)
        {
            try
            {
                var fileStream = new FileStream(_dir + @"\Updates.zip", FileMode.Create, FileAccess.ReadWrite);
                responseStream.CopyTo(fileStream);
                fileStream.Close();

                UnZipFile();
                if (File.Exists(_updateDir + @"\EsterUpdater.exe"))
                {
                    File.Copy(_updateDir + @"\EsterUpdater.exe", _dir + @"\EsterUpdater.exe", true);
                    File.Delete(_updateDir + @"\EsterUpdater.exe");
                    return true;
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() => _eventAggregator.GetEvent<ShowErrorEvent>().Publish(
                        new Exception("В новой версии отсутствует программа обновленния, эта версия не будет установлена", new Exception()))));
                _logger.Warn("В новой версии отсутствует программа обновления, эта версия не будет установлена");
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        void UnZipFile()
        {
            if (Directory.Exists(_updateDir))
                Directory.Delete(_updateDir, true);
            Directory.CreateDirectory(_updateDir);
            using (var zipFile = ZipFile.Read(_zipFile, new ReadOptions() { Encoding = Encoding.GetEncoding(866) }))
            {
                foreach (ZipEntry e in zipFile)
                {
                    try
                    {
                        e.Extract(_updateDir, ExtractExistingFileAction.OverwriteSilently);
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => _eventAggregator.GetEvent<ShowErrorEvent>().Publish(
                    new Exception("Ошибка при рааспаковке файла обновлений", ex))));
                        _logger.WarnException("Ошибка при распаковке файла обновлений", ex);
                    }
                }
            }
            File.Delete(_zipFile);
        }

        private void UpdatesChecked(bool checkResult)
        {
            if (checkResult)
            {
                _serverInfo.UpdateNeed = true;
                IsVisible = Visibility.Visible;
                LastTime = AutoRestartTime;
                _checkTimer.Stop();
                _restartTimer.Start();
            }
            else
            {
                _checkTimer.Start();
            }
        }

        private void RestartTimerTick(object sender, EventArgs e)
        {
            if (LastTime != 0)
            {
                LastTime = LastTime - 1;
            }
            else
            {
                _restartTimer.Stop();
                StartUpdater();
            }
        }

        void StartUpdater()
        {
            if (File.Exists(_dir + "\\EsterUpdater.exe"))
            {
                Process.Start(_dir + "\\EsterUpdater.exe");
                Application.Current.Shutdown();
            }
        }

        private void RestartApp(object obj)
        {
            StartUpdater();
        }

        private void UpdateAbortedByUser(object obj)
        {
            _checkTimer.Start();
            _restartTimer.Stop();
            LastTime = AutoRestartTime;
            IsVisible = Visibility.Collapsed;
        }

        public int LastTime
        {
            get { return _lastTime; }
            set
            {
                if (_lastTime != value)
                {
                    _lastTime = value;
                    RaisePropertyChanged("LastTime");
                }
            }
        }

        public Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }        
    }
}
