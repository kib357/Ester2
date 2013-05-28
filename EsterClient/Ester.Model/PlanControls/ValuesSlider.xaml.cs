using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Ester.Model.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for TextSensor.xaml
    /// </summary>
    public partial class ValuesSlider
    {
        private bool _changedByServer;

        public ValuesSlider()
        {
            InitializeComponent();
			if(!DesignerProperties.GetIsInDesignMode(this))
			{
				EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
			}
        }

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            var formatInfo = new NumberFormatInfo { NumberDecimalSeparator = "," };
            if (Address != sensor.Key) return;
            double tmp;
            if (double.TryParse(sensor.Value, NumberStyles.Any, formatInfo, out tmp))
            {
                ValueLabel.Text = ConvertValueByUnits(sensor.Value, Units);
                if (CurrentValue != tmp)
                {
                    _changedByServer = true;
                    CurrentValue = tmp;
                }
            }
        }

        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register(
            "CurrentValue", typeof(double), typeof(ValuesSlider), new PropertyMetadata(OnCurrentValueChanged));

        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        private static async void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ValuesSlider;
            if (sender == null) return;                      

            double value;
            if (double.TryParse(e.NewValue.ToString(), out value))
            {
                sender.Vs.IsEnabled = (value >= sender.MinValue && value <= sender.MaxValue);
            }
            else
                sender.Vs.IsEnabled = false;

            if (sender._changedByServer)
                sender._changedByServer = false;
            else
                if (!await sender.TryPushValueToServerAsync(sender.Address, sender.CurrentValue.ToString(CultureInfo.InvariantCulture)))
                {
                    sender._changedByServer = true;
                    if (e.OldValue is double)
                        sender.CurrentValue = (double)e.OldValue;
                }
        }   

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(ValuesSlider), new PropertyMetadata((double)1));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(ValuesSlider), new PropertyMetadata((double)0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty ValuesCountProperty = DependencyProperty.Register(
            "ValuesCount", typeof(int), typeof(ValuesSlider), new PropertyMetadata(0, OnValuesCountChanged));

        private static void OnValuesCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ValuesSlider;
            if (sender == null) return;

            double diff = sender.MaxValue - sender.MinValue;
            double value;
            if (double.TryParse(e.NewValue.ToString(), out value) && diff > 0)
            {
                sender.Vs.TickFrequency = (int) (diff/value);
            }
        }

        public int ValuesCount
        {
            get { return (int)GetValue(ValuesCountProperty); }
            set { SetValue(ValuesCountProperty, value); }
        }        

        private void VsMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EventAggregator.GetEvent<PlanControlDragEvent>().Publish(false);
        }

        private void VsMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EventAggregator.GetEvent<PlanControlDragEvent>().Publish(true);
        }
    }
}
