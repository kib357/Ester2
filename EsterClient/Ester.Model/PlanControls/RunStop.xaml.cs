using System.Collections.Generic;
using System.Windows;
using Ester.Model.BaseClasses;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for RunStop.xaml
    /// </summary>
    public partial class RunStop : SensorBase
    {
        public RunStop()
        {
            InitializeComponent();
        }

        protected override void SetValues(KeyValuePair<string, string> sensor)
        {
            if (Address != sensor.Key) return;
            bool tmp;
            if (TryParseBool(sensor.Value, out tmp) && IsRunning != tmp)
            {
                _changedByServer = true;
                IsRunning = tmp;
            }
        }

        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
            "IsRunning", typeof(bool?), typeof(RunStop), new PropertyMetadata(OnIsRunningChanged));

        private bool _changedByServer;
        private static async void OnIsRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as RunStop;
            if (sender == null) return;
            if (sender._changedByServer)
                sender._changedByServer = false;
            else
                if(!await sender.TryPushValueToServerAsync(sender.Address, sender.IsRunning.ToString()))
                {
                    sender._changedByServer = true;
                    sender.IsRunning = e.OldValue as bool?;
                }
        }

        public bool? IsRunning
        {
            get { return (bool?)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }        
    }
}
