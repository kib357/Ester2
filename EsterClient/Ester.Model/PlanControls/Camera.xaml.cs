using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using VideoServer;
using VideoServer.Interfaces;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class Camera
    {
        public Camera()
            : base()
        {
            try
            {
                EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            }
            catch (Exception)
            {

                return;
            }
            InitializeComponent();
        }

        public static readonly DependencyProperty OutputModesProperty = DependencyProperty.Register(
"OutputModes", typeof(List<string>), typeof(Camera));

        public List<string> OutputModes
        {
            get { return (List<string>)GetValue(OutputModesProperty); }
            set { SetValue(OutputModesProperty, value); }
        }

        public static readonly DependencyProperty ArchiveVisibilityProperty = DependencyProperty.Register(
"ArchiveVisibility", typeof(Visibility), typeof(Camera));

        public Visibility ArchiveVisibility
        {
            get { return (Visibility)GetValue(ArchiveVisibilityProperty); }
            set { SetValue(ArchiveVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CurrentOutputModeProperty = DependencyProperty.Register(
"CurrentOutputMode", typeof(string), typeof(Camera), new PropertyMetadata(OnOutputModeChanged));

        private static void OnOutputModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Camera;
            if (sender == null) return;
            switch (sender.CurrentOutputMode)
            {
                case "Живое видео":
                    sender.ArchiveVisibility = Visibility.Collapsed;
                    sender.IntCamera.Stop();
                    sender.IntCamera.Play();
                    break;
                case "Просмотр архива":
                    sender.ArchiveVisibility = Visibility.Visible;
                    sender.IntCamera.Stop();
                    if (sender.ArchRecords == null)
                        sender.ArchRecords = sender.IntCamera.GetHistory(false);
                    sender.IntCamera.GetFirstFrame(sender.CurrentArchDay, sender.CurrentArchTime);
                    break;
            }
        }

        public string CurrentOutputMode
        {
            get { return (string)GetValue(CurrentOutputModeProperty); }
            set { SetValue(CurrentOutputModeProperty, value); }
        }

        public static readonly DependencyProperty ArchRecordsProperty = DependencyProperty.Register(
"ArchRecords", typeof(Dictionary<string, List<string>>), typeof(Camera), new PropertyMetadata(OnArchRecordsPropertyChanged));

        private static void OnArchRecordsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Camera;
            if (sender == null) return;
            sender.CurrentArchDay = sender.ArchRecords.Keys.FirstOrDefault();
        }

        public Dictionary<string, List<string>> ArchRecords
        {
            get { return (Dictionary<string, List<string>>)GetValue(ArchRecordsProperty); }
            set { SetValue(ArchRecordsProperty, value); }
        }

        public static readonly DependencyProperty CurrentArchDayProperty = DependencyProperty.Register(
"CurrentArchDay", typeof(string), typeof(Camera), new PropertyMetadata(OnCurrentArchDayPropertyChanged));

        private static void OnCurrentArchDayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Camera;
            if (sender == null || sender.CurrentArchDay == null) return;
            var timeList = sender.ArchRecords[sender.CurrentArchDay];
            if (timeList == null)
                sender.ArchRecords[sender.CurrentArchDay] = sender.IntCamera.GetHistory(sender.CurrentArchDay);
            sender.ArchTimeList = sender.ArchRecords[sender.CurrentArchDay];
                
        }

        public string CurrentArchDay
        {
            get { return (string)GetValue(CurrentArchDayProperty); }
            set { SetValue(CurrentArchDayProperty, value); }
        }

        public static readonly DependencyProperty ArchTimeListProperty = DependencyProperty.Register(
"ArchTimeList", typeof(List<string>), typeof(Camera), new PropertyMetadata(OnArchTimeListPropertyChanged));

        private static void OnArchTimeListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Camera;
            if (sender == null) return;
            sender.CurrentArchTime = sender.ArchTimeList[0];
            //sender.IntCamera.GetFirstFrame(sender.CurrentArchDay, sender.CurrentArchTime);
        }

        public List<string> ArchTimeList
        {
            get { return (List<string>) GetValue(ArchTimeListProperty); }
            set { SetValue(ArchTimeListProperty, value); }
        }

        public static readonly DependencyProperty CurrentArchTimeProperty = DependencyProperty.Register(
"CurrentArchTime", typeof(string), typeof(Camera), new PropertyMetadata(OnCurrentArchTimePropertyChanged));

        private static void OnCurrentArchTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Camera;
            if (sender == null) return;
            sender.IntCamera.GetFirstFrame(sender.CurrentArchDay, sender.CurrentArchTime);
        }

        public string CurrentArchTime
        {
            get { return (string)GetValue(CurrentArchTimeProperty); }
            set { SetValue(CurrentArchTimeProperty, value); }
        }

        public static readonly DependencyProperty IntCameraProperty = DependencyProperty.Register(
"IntCamera", typeof(IntellectCamera), typeof(Camera));

        public IntellectCamera IntCamera
        {
            get { return (IntellectCamera)GetValue(IntCameraProperty); }
            set { SetValue(IntCameraProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
    "RotationAngle", typeof(int), typeof(Camera), new PropertyMetadata(0));

        public int RotationAngle
        {
            get { return (int)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        private void IconMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AddressList.Length != 2) return;
            if (IntCamera == null)
            {                
                IntCamera = new IntellectCamera(AddressList[0], 900, AddressList[1]);
            }
            IntCamera.Connect();
            IntCamera.Play();
            OutputModes = new List<string> { "Живое видео", "Просмотр архива" };
            CurrentOutputMode = "Живое видео";
            FramePopup.IsOpen = true;
        }
        
        private void FramePopupClosed(object sender, EventArgs e)
        {
            IntCamera.Stop();
            IntCamera.Disconnect();
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            FramePopup.IsOpen = false;
        }

        private void GetPrevHistoryFrame(object sender, RoutedEventArgs e)
        {
            IntCamera.PrevFrame();
        }

        private void PlayArchiveFile(object sender, RoutedEventArgs e)
        {
            IntCamera.Play(CurrentArchDay, CurrentArchTime, 1);
        }

        private void GetNextHistoryFrame(object sender, RoutedEventArgs e)
        {
            IntCamera.NextFrame();
        }

        private void PauseArchiveFile(object sender, RoutedEventArgs e)
        {
            IntCamera.Pause();
        }

        private void StopArchiveFile(object sender, RoutedEventArgs e)
        {
            IntCamera.Stop();
        }

        private void PopupResize(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double yadjust = FramePopup.Height + e.VerticalChange;
            double xadjust = FramePopup.Width + e.VerticalChange;
            if (yadjust >= 350 && xadjust >= 400)
            {
                FramePopup.Width = xadjust;
                FramePopup.Height = yadjust;
            }
        }

        private void PopupDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            FramePopup.HorizontalOffset += e.HorizontalChange;
            FramePopup.VerticalOffset += e.VerticalChange;
        }
    }
}
