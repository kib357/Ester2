using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Radiator.xaml
    /// </summary>
    public partial class EHeater
    {
        public static readonly DependencyProperty PipeColorProperty = DependencyProperty.Register(
    "PipeColor", typeof(Color), typeof(EHeater), new PropertyMetadata());

        public Color PipeColor
        {
            get { return (Color)GetValue(PipeColorProperty); }
            set { SetValue(PipeColorProperty, value); }
        }

        public static readonly DependencyProperty InTempProperty = DependencyProperty.Register(
    "InTemp", typeof(double), typeof(EHeater), new PropertyMetadata(80.0, OnInTempChanged));

        private static void OnInTempChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EHeater sender = d as EHeater;
            if (sender == null) return;
            double value = (double)e.NewValue;
            MaterialGroup mg = (MaterialGroup)sender.MainView.FindResource("M_Pipe");

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
        }

        public double InTemp
        {
            get { return (double)GetValue(InTempProperty); }
            set { SetValue(InTempProperty, value); }
        }

        public EHeater()
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
                StateTextBorder.Visibility = Visibility.Visible;
        }

        private void EHeater_Loaded(object sender, RoutedEventArgs e)
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
                            if (sensor.Value == "1")
                            {
                                InTemp = 120;
                                StateText.Text = "On";
                                break;
                            }
                            if (sensor.Value == "0")
                            {
                                InTemp = -1;
                                StateText.Text = "Off";
                                break;
                            }
                            StateText.Text = "?";
                            InTemp = 30;
                            break;
                    }
            }
        }
    }
}
