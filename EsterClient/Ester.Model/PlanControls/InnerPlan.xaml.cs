using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ester.Model.BaseClasses;
using Ester.Model.Events;
using Ester.Model.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Room.xaml
    /// </summary>
    public partial class InnerPlan
    {
        private Point _lastDragPoint;
        private readonly Color _defaultBackground = Color.FromRgb(0xFC, 0xFF, 0xF5);
        private readonly Color _selectedBackground = Color.FromRgb(0xC3, 0xD8, 0xE3);

        public InnerPlan()
        {
            InitializeComponent();
            EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }     

        #region Mouse events

        private void InnerPlanMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastDragPoint = e.GetPosition(this);
        }

        private void InnerPlanMouseUp(object sender, MouseButtonEventArgs e)
        {
            var content = Content as Shape;
            if (content == null) return;
            SelectPlan(e);
        }

        private void SelectPlan(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var newPoint = e.GetPosition(this);
                if (newPoint == _lastDragPoint)
                {
                    EventAggregator.GetEvent<ShowPlanEvent>().Publish(PlanUid);
                }
            }
        }

        public static readonly DependencyProperty PlanUidProperty = DependencyProperty.Register(
            "PlanUid", typeof(string), typeof(object), new PropertyMetadata(""));

        public string PlanUid
        {
            get { return (string)GetValue(PlanUidProperty); }
            set { SetValue(PlanUidProperty, value); }
        }

        #endregion        
    }
}
