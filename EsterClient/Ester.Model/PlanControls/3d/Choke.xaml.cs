using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Choke.xaml
    /// </summary>
    public partial class Choke
    {
        public static readonly DependencyProperty AxisAngleProperty = DependencyProperty.Register(
    "AxisAngle", typeof(AxisAngleRotation3D), typeof(Choke), new PropertyMetadata(new AxisAngleRotation3D(new Vector3D(-20, 0, 0), 20)));

        public AxisAngleRotation3D AxisAngle
        {
            get { return (AxisAngleRotation3D)GetValue(AxisAngleProperty); }
            set { SetValue(AxisAngleProperty, value); }
        }

        public static readonly DependencyProperty OpenPercentageProperty = DependencyProperty.Register(
    "OpenPercentage", typeof(int), typeof(Choke), new PropertyMetadata(-1, OnOpenPercentageChanged));

        private static void OnOpenPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Choke sender = d as Choke;
            if (sender == null) return;

            if ((int)e.NewValue >= 0 && (int)e.NewValue <= 100)
            {
                sender.PercentageText.Text = sender.ConvertValueByUnits(sender.OpenPercentage.ToString(), "%");
                sender.AxisAngle = new AxisAngleRotation3D(new Vector3D(-sender.OpenPercentage * 0.9, 0, 0), sender.OpenPercentage * 0.9);
            }
            if (sender.ChangeColor)
            {
                if ((int) e.NewValue == 0)
                {
                    sender.FrameColor = Color.FromArgb(0x77, 0xcc, 0x43, 0x53);
                }
                if ((int) e.NewValue != 0 && (int) e.NewValue != 100)
                {
                    sender.FrameColor = Color.FromArgb(0x77, 0x00, 0x63, 0xBA);
                }
                if ((int) e.NewValue == 100)
                {
                    sender.FrameColor = Color.FromArgb(0x77, 0x00, 0xcc, 0x53);
                }
            }
        }

        public int OpenPercentage
        {
            get { return (int)GetValue(OpenPercentageProperty); }
            set { SetValue(OpenPercentageProperty, value); }
        }

        public static readonly DependencyProperty FrameColorProperty = DependencyProperty.Register(
            "FrameColor", typeof(Color), typeof(Choke), new PropertyMetadata(Color.FromArgb(0x77, 0x00, 0x63, 0xBA)));

        public Color FrameColor
        {
            get { return (Color)GetValue(FrameColorProperty); }
            set { SetValue(FrameColorProperty, value); }
        }

        public static readonly DependencyProperty ChangeColorProperty = DependencyProperty.Register(
    "ChangeColor", typeof(bool), typeof(Choke), new PropertyMetadata(false));

        public bool ChangeColor
        {
            get { return (bool)GetValue(ChangeColorProperty); }
            set { SetValue(ChangeColorProperty, value); }
        }

        public static readonly DependencyProperty OpenValueProperty = DependencyProperty.Register(
           "OpenValue", typeof(bool?), typeof(Choke), new PropertyMetadata(OnOpenValueChanged));

        public bool? OpenValue
        {
            get { return (bool?)GetValue(OpenValueProperty); }
            set { SetValue(OpenValueProperty, value); }
        }

        private static void OnOpenValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Choke;
            if (sender != null)
            {
                SetUi(sender);
            }
        }

        public static readonly DependencyProperty CloseValueProperty = DependencyProperty.Register(
            "CloseValue", typeof(bool?), typeof(Choke), new PropertyMetadata(OnCloseValueChanged));

        public bool? CloseValue
        {
            get { return (bool?)GetValue(CloseValueProperty); }
            set { SetValue(CloseValueProperty, value); }
        }

        private static void OnCloseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Choke;
            if (sender != null)
            {
                SetUi(sender);
            }
        }

        private static void SetUi(Choke sender)
        {
            if (sender.OpenValue == null | sender.CloseValue == null)
            {
                sender.PercentageText.Text = "?";
                sender.OpenPercentage = 50;
                return;
            }
            if (sender.OpenValue == true && sender.CloseValue != true)
            {
                sender.PercentageText.Text = sender.ConvertValueByUnits("100", "%");
                sender.OpenPercentage = 100;
                return;
            }
            if (sender.CloseValue == true && sender.OpenValue != true)
            {
                sender.PercentageText.Text = sender.ConvertValueByUnits("0", "%");
                sender.OpenPercentage = 0;
                return;
            }
            if (sender.CloseValue != true && sender.OpenValue != true)
            {
                sender.PercentageText.Text = "?";
                sender.OpenPercentage = 50;
            }
        }

        public Choke()
        {
            InitializeComponent();
            //MyTransfrom.Rotation = new AxisAngleRotation3D(new Vector3D(-80,0,0), 80);

            try
            {
                EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            }
            catch
            {
                return;
            }
        }

        protected override void InitializeView()
        {
            if (AddressList.Length > 0 && !string.IsNullOrWhiteSpace(AddressList[0]))
                PercentageTextBorder.Visibility = Visibility.Visible;
        }

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            for (int i = 0; i < AddressList.Length; i++)
            {
                if (AddressList[i] == sensor.Key)
                    switch (i)
                    {
                        case 0:
                            if (UnitsList[0] == "%")
                            {
                                int tmp;
                                if (int.TryParse(sensor.Value, out tmp))
                                {
                                    if (tmp >= 0 && tmp <= 100)
                                    {
                                        OpenPercentage = tmp;
                                    }
                                    else
                                        PercentageText.Text = "Ошибка значения";
                                }
                                else
                                    PercentageText.Text = "?";
                            }
                            if (UnitsList[0] == "On")
                            {
                                if (sensor.Value == "1")
                                {
                                    OpenPercentage = 100;
                                }
                                if (sensor.Value == "0")
                                {
                                    OpenPercentage = 0;
                                }
                            }
                            if (UnitsList[0] == "OnOff")
                            {
                                if (sensor.Value == "1")
                                    OpenValue = true;
                                if (sensor.Value == "0")
                                    OpenValue = false;
                            }
                            break;
                        case 1:
                            if (UnitsList[0] == "OnOff")
                            {
                                if (sensor.Value == "1")
                                    CloseValue = true;
                                if (sensor.Value == "0")
                                    CloseValue = false;
                            }
                            break;
                    }
            }
        }

        private void Choke_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeView();
        }
    }
}
