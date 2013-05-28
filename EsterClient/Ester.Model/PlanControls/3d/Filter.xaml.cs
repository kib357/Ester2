using System.Collections.Generic;
using System.Windows;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Filter
    {
        public static readonly DependencyProperty OpenPercentageProperty = DependencyProperty.Register(
    "OpenPercentage", typeof(double), typeof(Filter), new PropertyMetadata(0.0));

        public double OpenPercentage
        {
            get { return (double)GetValue(OpenPercentageProperty); }
            set { SetValue(OpenPercentageProperty, value); }
        }

        public static readonly DependencyProperty MaxPDProperty = DependencyProperty.Register(
    "MaxPDPercentage", typeof(double), typeof(Filter), new PropertyMetadata(double.MaxValue));

        public double MaxPD
        {
            get { return (double)GetValue(MaxPDProperty); }
            set { SetValue(MaxPDProperty, value); }
        }

        public Filter()
        {
            InitializeComponent();

            /*int pressure = new Random().Next(40, 60);
            OpenPercentage = (pressure - 39) / 60;
            PDText.Text = pressure + " Па";*/

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
                PDTextBorder.Visibility = Visibility.Visible;
        }

        private void Filter_Loaded(object sender, RoutedEventArgs e)
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
                                PDText.Text = ConvertValueByUnits(sensor.Value, !string.IsNullOrWhiteSpace(UnitsList[0]) ? UnitsList[0] : "Па");
                                OpenPercentage = tmp / MaxPD > 1 ? 1 : tmp / MaxPD;                                
                                break;
                            }
                            PDText.Text = "?";
                            break;
                    }
            }
        }
    }
}
