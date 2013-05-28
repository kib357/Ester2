using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Ester.Model.PlanControls
{
    /// <summary>
    /// Interaction logic for Radiator.xaml
    /// </summary>
    public partial class WaterFilter
    {
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
    "RotationAngle", typeof(int), typeof(WaterFilter), new PropertyMetadata(0));

        public int RotationAngle
        {
            get { return (int)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty ZRotationAngleProperty = DependencyProperty.Register(
    "ZRotationAngle", typeof(int), typeof(WaterFilter), new PropertyMetadata(0));

        public int ZRotationAngle
        {
            get { return (int)GetValue(ZRotationAngleProperty); }
            set { SetValue(ZRotationAngleProperty, value); }
        }

        public static readonly DependencyProperty InTempProperty = DependencyProperty.Register(
    "InTemp", typeof(double), typeof(WaterFilter), new PropertyMetadata(0.0, OnInTempChanged));

        private static void OnInTempChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Radiator sender = d as Radiator;
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

        public WaterFilter()
        {
            InitializeComponent();
        }
    }
}
