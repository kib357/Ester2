using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for AirChoke.xaml
    /// </summary>
    public partial class FireChoke
    {
        public static readonly DependencyProperty OpenValueProperty = DependencyProperty.Register(
            "OpenValue", typeof(bool?), typeof(FireChoke), new PropertyMetadata(OnOpenValueChanged));

        public bool? OpenValue
        {
            get { return (bool?)GetValue(OpenValueProperty); }
            set { SetValue(OpenValueProperty, value); }
        }

        private static void OnOpenValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as FireChoke;
            if (sender != null)
            {
                SetUi(sender);
            }
        }      

        public static readonly DependencyProperty CloseValueProperty = DependencyProperty.Register(
            "CloseValue", typeof(bool?), typeof(FireChoke), new PropertyMetadata(OnCloseValueChanged));

        public bool? CloseValue
        {
            get { return (bool?)GetValue(CloseValueProperty); }
            set { SetValue(CloseValueProperty, value); }
        }

        private static void OnCloseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as FireChoke;
            if (sender != null)
            {
                SetUi(sender);
            }
        }

        private static void SetUi(FireChoke sender)
        {
            if (sender.OpenValue == null | sender.CloseValue == null)
            {
                sender.Choke.Visibility = Visibility.Hidden;
                return;
            }
            if (sender.OpenValue == true && sender.CloseValue != true)
            {
                sender.Choke.Visibility = Visibility.Visible;
                sender.RotationAngle = 90;
                sender.PipeColor = new SolidColorBrush(Color.FromRgb(0x00, 0xcc, 0x53));
                return;
            }
            if (sender.CloseValue == true && sender.OpenValue != true)
            {
                sender.Choke.Visibility = Visibility.Visible;
                sender.RotationAngle = 0;
                sender.PipeColor = new SolidColorBrush(Color.FromRgb(0xcc, 0x43, 0x53));
                return;
            }
            if (sender.CloseValue != true && sender.OpenValue != true)
            {
                sender.Choke.Visibility = Visibility.Visible;
                sender.RotationAngle = 45;
                sender.PipeColor = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
            }
        }

        public static readonly DependencyProperty ConnectionsProperty = DependencyProperty.Register(
            "Connections", typeof(string), typeof(FireChoke), new PropertyMetadata(OnConnectionsChanged));

        public string Connections
        {
            get { return (string)GetValue(ConnectionsProperty); }
            set { SetValue(ConnectionsProperty, value); }
        }

        private static void OnConnectionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as FireChoke;
            if (sender != null)
            {
                if (sender.Connections == null)
                {
                    sender.LeftBorder.Visibility = Visibility.Collapsed;
                    sender.RightBorder.Visibility = Visibility.Collapsed;
                    return;
                }

                string[] tmpValues = sender.Connections.Split(',');

                if (tmpValues.Length == 2)
                {
                    int left, right;
                    int.TryParse(tmpValues[0], out left);
                    int.TryParse(tmpValues[1], out right);
                    if (left == 0)
                    {
                        sender.LeftBorder.Visibility = Visibility.Collapsed;
                    }
                    if (left == 1)
                    {
                        sender.LeftBorder.Visibility = Visibility.Visible;
                        sender.LeftBorder.VerticalAlignment = VerticalAlignment.Top;
                    }
                    if (left == 2)
                    {
                        sender.LeftBorder.Visibility = Visibility.Visible;
                        sender.LeftBorder.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    if (left == 3)
                    {
                        sender.LeftBorder.Visibility = Visibility.Visible;
                        sender.LeftBorder.Height = 190;
                    }
                    if (right == 0)
                    {
                        sender.RightBorder.Visibility = Visibility.Collapsed;
                    }
                    if (right == 1)
                    {
                        sender.RightBorder.Visibility = Visibility.Visible;
                        sender.RightBorder.VerticalAlignment = VerticalAlignment.Top;
                    }
                    if (right == 2)
                    {
                        sender.RightBorder.Visibility = Visibility.Visible;
                        sender.RightBorder.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    if (right == 3)
                    {
                        sender.RightBorder.Visibility = Visibility.Visible;
                        sender.RightBorder.Height = 190;
                    }
                }
            }
        }    

        public static readonly DependencyProperty PipeWidthProperty = DependencyProperty.Register(
            "PipeWidth", typeof(int), typeof(FireChoke), new PropertyMetadata(0));

        public int PipeWidth
        {
            get { return (int)GetValue(PipeWidthProperty); }
            set { SetValue(PipeWidthProperty, value); }
        }

        public static readonly DependencyProperty PipeColorProperty = DependencyProperty.Register(
            "PipeColor", typeof(SolidColorBrush), typeof(FireChoke), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xff, 0x77, 0x77, 0x77))));

        public SolidColorBrush PipeColor
        {
            get { return (SolidColorBrush)GetValue(PipeColorProperty); }
            set { SetValue(PipeColorProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
            "RotationAngle", typeof(int), typeof(FireChoke), new PropertyMetadata(0));

        public int RotationAngle
        {
            get { return (int)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public FireChoke()
        {
            InitializeComponent();

            try
            {
                EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            }
            catch
            {
                return;
            }
        }

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            for (int i = 0; i < AddressList.Length; i++)
            {
                if (AddressList[i] == sensor.Key)
                    switch (i)
                    {
                        case 0:
                            if (sensor.Value == "1")
                                OpenValue = true;
                            if (sensor.Value == "0")
                                OpenValue = false;
                            break;
                        case 1:
                            if (sensor.Value == "1")
                                CloseValue = true;
                            if (sensor.Value == "0")
                                CloseValue = false;
                            break;
                    }
            }
        }
    }
}
