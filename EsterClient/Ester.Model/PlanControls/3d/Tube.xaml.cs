using System;
using System.Windows;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Tube.xaml
    /// </summary>
    public partial class Tube
    {
        public static readonly DependencyProperty PipeWidthProperty = DependencyProperty.Register(
    "PipeWidth", typeof(Double), typeof(Tube), new PropertyMetadata(7.0));

        public Double PipeWidth
        {
            get { return (Double)GetValue(PipeWidthProperty); }
            set { SetValue(PipeWidthProperty, value); }
        }

        public Tube()
        {
            InitializeComponent();
        }
    }
}
