using System;
using System.Windows;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Tube.xaml
    /// </summary>
    public partial class VTube
    {
        public static readonly DependencyProperty PipeHeightProperty = DependencyProperty.Register(
    "PipeHeight", typeof(Double), typeof(Tube), new PropertyMetadata(2.0));

        public Double PipeHeight
        {
            get { return (Double)GetValue(PipeHeightProperty); }
            set { SetValue(PipeHeightProperty, value); }
        }

        public VTube()
        {
            InitializeComponent();
        }
    }
}
