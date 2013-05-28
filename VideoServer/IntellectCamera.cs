using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using VideoServer.Enums;
using VideoServer.Interfaces;
using VideoServer.Model;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Timers;

namespace VideoServer
{

    public class IntellectCamera : NotificationObject, ICamera
    {
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set { _isConnected = value; RaisePropertyChanged("IsConnected"); }
        }
        private IntellectConnector _intellectConnector;
        private string _guid;
        private VideoGetterTasks _currentTask;
        private string _message;
        private AutoResetEvent _autoEvent = new AutoResetEvent(false);
        
        private DateTime _fileStart;
        private DateTime _fileEnd;
        private DateTime _timeStamp;
        private System.Timers.Timer _timer;
        private bool _getingOneFrame;
        public string Address { get; set; }
        public int Port { get; set; }
        public string Id { get; private set; }
        public string Name { get; private set; }
        private BitmapImage _frame;
        public BitmapImage Frame
        {
            get { return _frame; }
            set { _frame = value; RaisePropertyChanged("Frame"); }
        }

        private string _subTitles;
        public string SubTitles
        {
            get { return _subTitles; }
            set { _subTitles = value; RaisePropertyChanged("SubTitles"); }
        }

        public bool IsPlayingVideo;

        public Dictionary<string, List<string>> History { get; private set; }

        public List<string> Messages { get; set; }

        public IntellectCamera(string address, int port, string id)
        {
            Address = address;
            Port = port;
            Id = id;
            Name = "";
            IsConnected = false;
            _guid = Guid.NewGuid().ToString();
            _currentTask = VideoGetterTasks.None;
            History = null;
            Messages = new List<string>();
            Frame = new BitmapImage();
            _getingOneFrame = false;
            SubTitles = String.Empty;
            _timer = new System.Timers.Timer { Interval = 100 };
            _timer.Elapsed += TimerElapsedEvent;
            IsPlayingVideo = false;
            _intellectConnector = new IntellectConnector() { ReceiveBitmap = true };
            _intellectConnector.OnVideoFrame += OnIntellectVideoFrame;
            _intellectConnector.OnMessage += OnIntellectMessage;
            _intellectConnector.OnConnected += OnIntellectConnected;
        }

        private void OnIntellectConnected(object sender, EventArgs e)
        {
            IsConnected = true;
            GetFrame();
        }

        private void OnIntellectMessage(object sender, ItvMessageEventArgs e)
        {
            if (!IsConnected) return;
            if (String.IsNullOrEmpty(Name) && e.Message.Contains("name<"))
            {
                var start = e.Message.IndexOf("name<") + 5;
                Name = e.Message.Substring(start, e.Message.IndexOf('>', start) - start);
            }
            Messages.Add(e.Message);
            if (e.Message.Contains("SET_INTERVALSREC"))
            {
                _message = e.Message;
                _autoEvent.Set();
            }
        }

        private void OnIntellectVideoFrame(object sender, ItvVideoFrameEventArgs e)
        {
            if (!_isConnected) return;
            using (var memory = new MemoryStream())
            {
                var bitmap = new Bitmap(e.ImageBitmap);
                bitmap.Save(memory, ImageFormat.Png);
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();
                Frame = bitmapimage;
                _timeStamp = e.Timestamp;
                SubTitles = e.Subtitle + " " + e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (_getingOneFrame)
                {
                    _getingOneFrame = false;
                    Stop();
                }
            }
        }

        public void Connect()
        {
            if (IsConnected)
            {
                Stop();
                Disconnect();
            }
            _intellectConnector.Connect(Address, Port, _guid);
            _currentTask = VideoGetterTasks.None;
        }

        public void Disconnect()
        {
            Stop();
            _intellectConnector.Disconnect();
        }

        public void Pause()
        {
            if (_currentTask != VideoGetterTasks.GettingArchive || !_timer.Enabled) return;
            _timer.Stop();
        }

        public void PrevFrame()
        {
            Pause();
            _currentTask = VideoGetterTasks.GettingArchive;
            _intellectConnector.Send(String.Format("CAM|{0}|ARCH_FRAME_PREV", Id));
            IsPlayingVideo = false;
        }

        public void NextFrame()
        {
            Pause();
            _currentTask = VideoGetterTasks.GettingArchive;
            _intellectConnector.Send(String.Format("CAM|{0}|ARCH_FRAME_NEXT", Id));
            IsPlayingVideo = false;
        }

        public void Stop()
        {
            switch (_currentTask)
            {
                case VideoGetterTasks.GettingArchive:
                    if (_timer.Enabled)
                    {
                        _timer.Stop();
                        _intellectConnector.Send(String.Format("CAM|{0}|ARCH_FRAME_TIME|time<{1}>,exact<0>", Id, _fileStart.ToString("dd-MM-yy HH:mm:ss.fff")));
                    }
                    break;
                case VideoGetterTasks.LiveVideo:
                    _intellectConnector.Send(String.Format("CAM|{0}|STOP_VIDEO", Id));
                    break;
            }
            IsPlayingVideo = false;
            _currentTask = VideoGetterTasks.None;
        }

        public void GetFrame()
        {
            _getingOneFrame = true;
            Play();
        }

        public void Play()
        {
            if (!IsConnected) return;
            Stop();
            _currentTask = VideoGetterTasks.LiveVideo;
            _intellectConnector.Send(String.Format("CAM|{0}|START_VIDEO|compress<3>", Id));

        }

        public void GetFirstFrame(string date, string time)
        {
            if (!IsConnected) return;
            if (_currentTask == VideoGetterTasks.LiveVideo || IsPlayingVideo)
                Stop();
            _currentTask = VideoGetterTasks.GettingArchive;
            _fileStart = DateTime.Parse(date + " " + time.Substring(0, time.IndexOf(' ')));
            _fileEnd = DateTime.Parse(date + " " + time.Substring(time.IndexOf(' ')));
            _intellectConnector.Send(String.Format("CAM|{0}|ARCH_FRAME_TIME|time<{1}>", Id, (_fileStart + TimeSpan.FromSeconds(1)).ToString("dd-MM-yy HH:mm:ss.fff")));
        }

        public void Play(string date, string time, int speed)
        {
            if (!IsConnected) return;
            if (_currentTask == VideoGetterTasks.LiveVideo)
                Stop();
            if (_currentTask == VideoGetterTasks.GettingArchive)
            {
                _timer.Interval = 80 / speed;
                IsPlayingVideo = true;
                _timer.Start();
                return;
            }
            _currentTask = VideoGetterTasks.GettingArchive;
            _fileStart = DateTime.Parse(date + " " + time.Substring(0, time.IndexOf(' ')));
            _fileEnd = DateTime.Parse(date + " " + time.Substring(time.IndexOf(' ')));
            _intellectConnector.Send(String.Format("CAM|{0}|ARCH_FRAME_TIME|time<{1}>", Id, _fileStart.ToString("dd-MM-yy HH:mm:ss.fff")));
            _timeStamp = DateTime.Parse("01-01-0001 00:00:00");
            _currentTask = VideoGetterTasks.GettingArchive;
            _timer.Interval = 80 / speed;
            _timer.Start();
            IsPlayingVideo = true;
        }

        private void TimerElapsedEvent(object sender, ElapsedEventArgs e)
        {
            _intellectConnector.Send(String.Format("CAM|{0}|ARCH_FRAME_NEXT", Id));
            if (_timeStamp >= _fileEnd)
            {
                Stop();
            }
        }


        private List<string> GetIntervals(string source, bool days)
        {
            if (!IsConnected) return null;
            var intervals = source;
            var start = intervals.IndexOf("intervals<", System.StringComparison.Ordinal) + 10;
            var daysString = intervals.Substring(start, intervals.IndexOf(">", start, StringComparison.Ordinal) - start);
            var intervalsList = daysString.Split('\n').ToList().Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
            return days ? intervalsList.OrderBy(DateTime.Parse).ToList() : intervalsList.OrderBy(s => DateTime.Parse(s.Substring(0, s.IndexOf(' ') - 1))).ToList();
        }

        public Dictionary<string, List<string>> GetHistory(bool all = true)
        {
            if (History == null)
            {
                var res = new Dictionary<string, List<string>>();
                if (!IsConnected) return res;
                _message = String.Empty;
                _intellectConnector.Send(String.Format("CAM|{0}|ARCH_GET_INTERVALSREC", Id));
                _autoEvent.WaitOne();
                var daylist = GetIntervals(_message, true);
                _message = String.Empty;
                foreach (var day in daylist)
                {
                    if (all)
                    {
                        _intellectConnector.Send(String.Format("CAM|{0}|ARCH_GET_INTERVALSREC|date<{1}>", Id, day));
                        _autoEvent.WaitOne();
                        var timeList = GetIntervals(_message, false);
                        res.Add(day, timeList);
                    }
                    else
                        res.Add(day, null);
                }
                History = res;
            }
            else
            {
                if(all)
                {
                    foreach (var pair in History.Where(s => s.Value == null))
                    {
                        _intellectConnector.Send(String.Format("CAM|{0}|ARCH_GET_INTERVALSREC|date<{1}>", Id, pair.Key));
                        _autoEvent.WaitOne();
                        var timeList = GetIntervals(_message, false);
                        History[pair.Key] = timeList;
                    }
                }  
            }
            return History;
        }

        public List<string> GetHistory(string date)
        {
            if (History[date] == null)
            {
                var result = new List<string>();
                if (!_isConnected) return result;
                _intellectConnector.Send(String.Format("CAM|{0}|ARCH_GET_INTERVALSREC|date<{1}>", Id, date));
                _autoEvent.WaitOne();
                result = GetIntervals(_message, false);
                History[date] = result;
            }
            return History[date];
        }

        public Dictionary<string, List<string>> GetEvents()
        {
            throw new NotImplementedException();
        }
    }
}
