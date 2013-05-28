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
    /// Interaction logic for Radiator.xaml
    /// </summary>
    public partial class RadiatorTop
    {
        public static readonly DependencyProperty PipeColorProperty = DependencyProperty.Register(
    "PipeColor", typeof(Color), typeof(RadiatorTop), new PropertyMetadata());

        public Color PipeColor
        {
            get { return (Color)GetValue(PipeColorProperty); }
            set { SetValue(PipeColorProperty, value); }
        }

        public static readonly DependencyProperty InTempProperty = DependencyProperty.Register(
    "InTemp", typeof(double), typeof(RadiatorTop), new PropertyMetadata(0.0, OnInTempChanged));

        private static void OnInTempChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadiatorTop sender = d as RadiatorTop;
            if (sender == null) return;
            double value = (double)e.NewValue;

            if (value >= 0 || value <= 120)
            {
                if (value <= 25)
                    sender.PipeColor = Color.FromArgb(0xff, (byte)(value * 7.2), (byte)(value * 3.96 + 115), 255);
                else
                    sender.PipeColor = Color.FromArgb(0xff, 255, (byte)(190 - (value - 25) * 2), (byte)(190 - (value - 25) * 2));
            }
            if (value <= 0)
                sender.PipeColor = Color.FromArgb(0xff, 0, 115, 255);
            if (value > 120)
                sender.PipeColor = Color.FromArgb(0xff, 255, 0, 0);

            /*if ((double)e.NewValue < sender.MinTemp)
            {
                sender.EventAggregator.GetEvent<AddNotifyEvent>().Publish(new NotifyBase
                {
                    ID = sender.Address + "ValueError",
                    Text = "Температура на радиаторе меньше допустимой. " + (sender.Tag ?? "") + " Допустимая: " + sender.MinTemp + ". Текущая: " + sender.TemperatureText.Text,
                    TimeToShow = 0
                });
            }
            if ((double)e.OldValue < sender.MinTemp && (double)e.NewValue >= sender.MinTemp)
            {
                sender.EventAggregator.GetEvent<RemoveNotifyEvent>().Publish(new NotifyBase { ID = sender.Address + "ValueError" });
            }*/
        }

        public double InTemp
        {
            get { return (double)GetValue(InTempProperty); }
            set { SetValue(InTempProperty, value); }
        }

        public static readonly DependencyProperty MinTempProperty = DependencyProperty.Register(
    "MinTemp", typeof(double), typeof(RadiatorTop), new PropertyMetadata(double.MinValue));

        public double MinTemp
        {
            get { return (double)GetValue(MinTempProperty); }
            set { SetValue(MinTempProperty, value); }
        }

        public RadiatorTop()
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

        protected override void InitializeView()
        {
            if (AddressList.Length > 0 && !string.IsNullOrWhiteSpace(AddressList[0]))
                TemperatureTextBorder.Visibility = Visibility.Visible;
        }

        private void RadiatorTop_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeView();
        }

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            for (int i = 0; i < AddressList.Length; i++)
            {
                if (AddressList[i] == sensor.Key)
                    switch (i)
                    {
                        case 0:
                            double tmp;
                            if (double.TryParse(sensor.Value, out tmp))
                            {
                                TemperatureText.Text = ConvertValueByUnits(sensor.Value, !string.IsNullOrWhiteSpace(UnitsList[0]) ? UnitsList[0] : "°C");
                                InTemp = (int)tmp;
                                break;
                            }
                            TemperatureText.Text = "?";
                            break;
                    }
            }
        }
    }
}
