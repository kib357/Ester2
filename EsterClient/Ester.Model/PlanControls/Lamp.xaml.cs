using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Ester.Model.Events;
using Ester.Model.Interfaces;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Lamp.xaml
    /// </summary>
    public partial class Lamp
    {
        private static readonly SolidColorBrush LampOnColor = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0x00));
        private static readonly SolidColorBrush LampOffColor = new SolidColorBrush(Color.FromRgb(0x77, 0x77, 0x77));
        
        public Lamp()
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
            if (Address != sensor.Key)
                return;
            float onPercentageFloat;
            float.TryParse(sensor.Value, out onPercentageFloat);
            if (Units == "On/Off")
                OnPercentage = onPercentageFloat != 0 ? "100" : "0";
            else
                OnPercentage = Math.Round(onPercentageFloat).ToString(CultureInfo.InvariantCulture);
        }

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Lamp;
            if (sender == null) return;

            if (sender.OnPercentage == "0" || string.IsNullOrWhiteSpace(sender.OnPercentage))
                sender.LampColor = LampOffColor;
            else
                sender.LampColor = LampOnColor;
        }

        public static readonly DependencyProperty OnPercentageProperty = DependencyProperty.Register(
            "OnPercentage", typeof(string), typeof(Lamp), new PropertyMetadata("?", OnPercentageChanged));

        public string OnPercentage
        {
            get { return (string)GetValue(OnPercentageProperty); }
            set { SetValue(OnPercentageProperty, value); }
        }

        public static readonly DependencyProperty LampColorProperty = DependencyProperty.Register(
            "LampColor", typeof(SolidColorBrush), typeof(Lamp), new PropertyMetadata(LampOffColor));

        public SolidColorBrush LampColor
        {
            get { return (SolidColorBrush)GetValue(LampColorProperty); }
            set { SetValue(LampColorProperty, value); }
        }

        public static readonly DependencyProperty GroupColorProperty = DependencyProperty.Register(
            "GroupColor", typeof(SolidColorBrush), typeof(Lamp));

        public SolidColorBrush GroupColor
        {
            get { return (SolidColorBrush)GetValue(GroupColorProperty); }
            set { SetValue(GroupColorProperty, value); }
        }        

        public static readonly DependencyProperty LampFailureVisibilityProperty = DependencyProperty.Register(
            "LampFailureVisibility", typeof(Visibility), typeof(Lamp), new PropertyMetadata(Visibility.Hidden));        

        public Visibility LampFailureVisibility
        {
            get { return (Visibility)GetValue(LampFailureVisibilityProperty); }
            set { SetValue(LampFailureVisibilityProperty, value); }
        }

        public static readonly DependencyProperty LampWidthProperty = DependencyProperty.Register(
            "LampWidth", typeof(string), typeof(Lamp), new PropertyMetadata("40"));

        public string LampWidth
        {
            get { return (string)GetValue(LampWidthProperty); }
            set { SetValue(LampWidthProperty, value); }
        }

        public static readonly DependencyProperty LampHeightProperty = DependencyProperty.Register(
            "LampHeight", typeof(string), typeof(Lamp), new PropertyMetadata("20"));

        public string LampHeight
        {
            get { return (string)GetValue(LampHeightProperty); }
            set { SetValue(LampHeightProperty, value); }
        }        
    }
}
