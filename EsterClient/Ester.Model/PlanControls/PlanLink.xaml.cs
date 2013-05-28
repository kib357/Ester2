using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ester.Model.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for PlanLink.xaml
    /// </summary>
    public partial class PlanLink
    {
        private Point _lastDragPoint;
        private IEventAggregator _eventAggregator;

        public PlanLink()
        {
            InitializeComponent();

            try
            {
                _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            }
            catch
            {
                return;
            }
        }        

        #region Mouse events

        private void PlanLinkMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastDragPoint = e.GetPosition(this);
        }

        private void PlanLinkMouseUp(object sender, MouseButtonEventArgs e)
        {
            _eventAggregator.GetEvent<ShowPlanEvent>().Publish(PlanUid);
        }        

        #endregion

        public static readonly DependencyProperty PlanUidProperty = DependencyProperty.Register(
    "PlanUid", typeof(string), typeof(PlanLink), new PropertyMetadata(string.Empty));

        public string PlanUid
        {
            get { return (string)GetValue(PlanUidProperty); }
            set { SetValue(PlanUidProperty, value); }
        }
    }
}
