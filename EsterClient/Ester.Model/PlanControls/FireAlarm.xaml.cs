using System.Windows;
using Microsoft.Practices.Prism.Events;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for FireAlarm.xaml
    /// </summary>
    public partial class FireAlarm
    {
        //private readonly Logger _logger = LogManager.GetLogger("InternalErrors");

        public static readonly DependencyProperty AlarmProperty = DependencyProperty.Register(
            "Alarm", typeof(string), typeof(FireAlarm), new PropertyMetadata(OnAlarmChanged));

        public string Alarm
        {
            get { return (string)GetValue(AlarmProperty); }
            set { SetValue(AlarmProperty, value); }
        }

        private static void OnAlarmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as FireAlarm;
            if (sender == null) return;
            /*if (e.NewValue.ToString() == "0")
            {
                Storyboard sb = (Storyboard)sender.AlarmRectangle.FindResource("ShowAlarm");
                sb.Begin();
                sender.EventAggregator.GetEvent<AddNotifyEvent>().Publish(new NotifyBase { ID = sender.Address + "Alarm", Text = "Пожар " + sender.Tag, TimeToShow = 0 });

                //Убираем возможные данные о неисправности в случае пожара
                sb = (Storyboard)sender.FaultRectangle.FindResource("ShowFault");
                sb.Stop();
                sender.EventAggregator.GetEvent<RemoveNotifyEvent>().Publish(new NotifyBase { ID = sender.Address + "Fault", Text = "Неисправность " + sender.Tag, TimeToShow = 0 });

                if (sender.ChangeTab)
                {
                    sender.EventAggregator.GetEvent<SelectedTabChangedEvent>().Publish("DU");
                }
            }
            else
            {
                Storyboard sb = (Storyboard)sender.AlarmRectangle.FindResource("ShowAlarm");
                sb.Stop();
                sender.AlarmRectangle.Opacity = 1;
                sender.EventAggregator.GetEvent<RemoveNotifyEvent>().Publish(new NotifyBase { ID = sender.Address + "Alarm", Text = "Пожар " + sender.Tag, TimeToShow = 0 });
            }*/
        }

        public static readonly DependencyProperty ChangeTabProperty = DependencyProperty.Register(
    "ChangeTab", typeof(bool), typeof(FireAlarm), new PropertyMetadata(false));

        public bool ChangeTab
        {
            get { return (bool)GetValue(ChangeTabProperty); }
            set { SetValue(ChangeTabProperty, value); }
        }

        public static readonly DependencyProperty FaultProperty = DependencyProperty.Register(
            "Fault", typeof(string), typeof(FireAlarm), new PropertyMetadata(OnFaultChanged));

        public string Fault
        {
            get { return (string)GetValue(FaultProperty); }
            set { SetValue(FaultProperty, value); }
        }

        private static void OnFaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as FireAlarm;
            if (sender == null) return;
            /*if (e.NewValue.ToString() == "0" && sender.Alarm != "0")
            {
                Storyboard sb = (Storyboard)sender.FaultRectangle.FindResource("ShowFault");
                sb.Begin();
                sender.EventAggregator.GetEvent<AddNotifyEvent>().Publish(new NotifyBase { ID = sender.Address + "Fault", Text = "Неисправность " + sender.Tag, TimeToShow = 0 });
            }
            else
            {
                Storyboard sb = (Storyboard)sender.FaultRectangle.FindResource("ShowFault");
                sb.Stop();
                sender.FaultRectangle.Opacity = 1;
                sender.EventAggregator.GetEvent<RemoveNotifyEvent>().Publish(new NotifyBase { ID = sender.Address + "Fault", Text = "Неисправность " + sender.Tag, TimeToShow = 0 });
            }*/
        }

        public FireAlarm()
        {
            InitializeComponent();
            EventAggregator = new EventAggregator();
        }

        /*protected override void SetValues(Sensor sensor)
        {
            for (int i = 0; i < AddressList.Length; i++)
            {
                if (AddressList[i] == sensor.Address)
                    switch (i)
                    {
                        case 0:
                            Alarm = sensor.Value;
                            break;
                        case 1:
                            Fault = sensor.Value;
                            break;
                    }
            }
        }*/
    }
}
