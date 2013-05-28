using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using Ester.Model.BaseClasses;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for AirPump.xaml
    /// </summary>
    public partial class AirPump
    {
        public static readonly DependencyProperty PowerPercentageProperty = DependencyProperty.Register(
    "PowerPercentage", typeof(int), typeof(AirPump), new PropertyMetadata(0, OnPowerPercentageChanged));

        public int PowerPercentage
        {
            get { return (int)GetValue(PowerPercentageProperty); }
            set { SetValue(PowerPercentageProperty, value); }
        }

        private static void OnPowerPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as AirPump;
            if (sender != null)
            {
                double persentage = Convert.ToDouble(e.NewValue);
                var sb = (Storyboard)sender.MainView.FindResource("spin");
                
                if (persentage > 100)
                {
                    sb.SetSpeedRatio(0);
                }
                else
                {
                    sb.SetSpeedRatio(persentage * 0.1);
                }
            }
        }

        public static readonly DependencyProperty MaxPDProperty = DependencyProperty.Register(
    "MaxPD", typeof(double), typeof(AirPump), new PropertyMetadata(Double.MaxValue));

        public double MaxPD
        {
            get { return (double)GetValue(MaxPDProperty); }
            set { SetValue(MaxPDProperty, value); }
        }

        public static readonly DependencyProperty PDProperty = DependencyProperty.Register(
    "PD", typeof(double), typeof(AirPump), new PropertyMetadata(OnPDChanged));

        public double PD
        {
            get { return (double)GetValue(PDProperty); }
            set { SetValue(PDProperty, value); }
        }

        private static void OnPDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AirPump sender = d as AirPump;
            if (sender == null) return;            
        }

        public AirPump()
        {
            InitializeComponent();
            var sb = (Storyboard)MainView.FindResource("spin");
            sb.Begin();
            sb.SetSpeedRatio(0);

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
                PowerTextBorder.Visibility = Visibility.Visible;
            if (AddressList.Length > 1 && !string.IsNullOrWhiteSpace(AddressList[1]))
                PDTextBorder.Visibility = Visibility.Visible;
        }

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            for (int i = 0; i < AddressList.Length; i++)
            {
                if (AddressList[i] == sensor.Key)
                {
                    double tmp;
                    switch (i)
                    {
                        case 0:
                            if (UnitsList[0] != "On" && double.TryParse(sensor.Value, out tmp))
                            {
                                if (UnitsList[0] == "Гц%")
                                    PowerPercentage = (int)tmp * 2;
                                else
                                {
                                    PowerPercentage = (int)tmp;
                                    Units = "%";
                                }
                                PowerText.Text = ConvertValueByUnits(sensor.Value, Units);
                            }
                            if (UnitsList[0] == "On")
                            {
                                if (sensor.Value == "1")
                                {
                                    PowerPercentage = 100;
                                    PowerText.Text = "On";
                                }
                                if (sensor.Value == "0")
                                {   
                                    PowerPercentage = 0;
                                    PowerText.Text = "Off";
                                }
                            }
                            break;
                        case 1:
                            if (double.TryParse(sensor.Value, out tmp))
                            {
                                PDText.Text = ConvertValueByUnits(sensor.Value, !string.IsNullOrWhiteSpace(UnitsList[1]) ? UnitsList[1] : "Па");
                                PD = tmp;
                            }
                            break;
                    }
                }
            }
        }

        private void AirPumpLoaded(object sender, RoutedEventArgs e)
        {
            InitializeView();
        }
    }
}
