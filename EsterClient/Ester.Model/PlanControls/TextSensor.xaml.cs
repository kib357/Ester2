using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for TextSensor.xaml
    /// </summary>
    public partial class TextSensor
    {
        //private readonly Logger _logger = LogManager.GetLogger("InternalErrors");

        public static readonly DependencyProperty TextValueProperty = DependencyProperty.Register(
            "TextValue", typeof(string), typeof(TextSensor), new PropertyMetadata(OnTextValueChanged));

        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        private static void OnTextValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as TextSensor;
            if (sender == null) return;

            string[] units = sender.Units.Split(',');
            string valueUnits = units.Length > 0 ? units[0] : sender.Units;
            sender.ValueLabel.Content = sender.ConvertValueByUnits(sender.TextValue, valueUnits);

            if (sender.MinValue > sender.MaxValue) return;

            double value;

            if (Double.TryParse(sender.TextValue, out value))
            {
                if (value < sender.MinValue || value > sender.MaxValue)
                {
                    var sb = (Storyboard) sender.ValueLabel.FindResource("ShowAlarm");
                    sb.Begin();
                }
                else
                {
                    var sb = (Storyboard) sender.ValueLabel.FindResource("ShowAlarm");
                    sb.Stop();
                }
            }
        }

        public static readonly DependencyProperty ErrorValueProperty = DependencyProperty.Register(
            "ErrorValue", typeof(string), typeof(TextSensor), new PropertyMetadata(OnErrorValueChanged));

        public string ErrorValue
        {
            get { return (string)GetValue(ErrorValueProperty); }
            set { SetValue(ErrorValueProperty, value); }
        }

        private static void OnErrorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as TextSensor;
            if (sender == null) return;

            Storyboard sb = (Storyboard)sender.ValueLabel.FindResource("ShowAlarm");
            if (e.NewValue.ToString() == "ON")
            {
                sb.Begin();
            }
            else
            {
                sb.Stop();                
            }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(TextSensor), new PropertyMetadata(Double.MinValue));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(TextSensor), new PropertyMetadata(Double.MaxValue));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty SensorStyleProperty = DependencyProperty.Register(
            "SensorStyle", typeof(Style), typeof(TextSensor));

        public Style SensorStyle
        {
            get { return (Style)GetValue(SensorStyleProperty); }
            set { SetValue(SensorStyleProperty, value); }
        }

        public TextSensor()
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
            /*_tmpTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 5) };
            _tmpTimer.Tick += OnTmpTimerTick;
            _tmpTimer.Start();*/
        }

        /*private void OnTmpTimerTick(object sender, EventArgs e)
        {
            int tmp = new Random().Next((int)MinValue, (int)MaxValue);
            double tmp2 = new Random().NextDouble();
            _resTmp = tmp + tmp2;
            if (Address == "712.AI104")
            {
                _resTmp = 1.14 + (new Random().NextDouble() % 0.1 * (new Random().Next(0, 10) > 5 ? 1 : -1));
            }
            if (Address == "712.AI103")
            {
                
                _resTmp = 1.43 + (new Random().NextDouble() % 0.1 * (new Random().Next(0,10) > 5 ? 1 : -1));
            }
            
            ValueLabel.Content = _resTmp.ToString().Substring(0,4) + Units;
            Thread.Sleep(50);
        }

        private double _resTmp;
        private DispatcherTimer _tmpTimer;*/

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            for (int i = 0; i < AddressList.Length; i++)
            {
                if (AddressList[i] == sensor.Key)
                    switch (i)
                    {
                        case 0:
                            TextValue = sensor.Value;
                            break;
                        case 1:
                            ErrorValue = sensor.Value;
                            break;
                    }
            }
        }
    }
}
