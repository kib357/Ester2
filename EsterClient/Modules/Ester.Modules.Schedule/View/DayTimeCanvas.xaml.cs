using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ester.Modules.Schedule.View
{
    /// <summary>
    /// Interaction logic for DayTimeCanvas.xaml
    /// </summary>
    public partial class DayTimeCanvas : UserControl
    {
	    private const int _padding = 30;
        public DayTimeCanvas()
        {
            InitializeComponent();
            
        }

        private void DayTimeGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
	        double width = DayTimeGrid.ActualWidth - _padding;

            DayTimeGrid.Children.Clear();
            var start = 2;
            var line = new Line();
            line.X1 = start;
            line.Y1 = 30;
			line.X2 = width;
            line.Y2 = 30;
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.StrokeThickness = 1;
            DayTimeGrid.Children.Add(line);
			double step = width / 1440;
            for (int i = 0; i <= 48; i++)
            {
                line = new Line();
                line.X1 = i * 30 * step + start;
                line.Y1 = i % 2 == 0 ? 15 : 20;
                line.X2 = line.X1;
                line.Y2 = 30;
                line.Stroke = System.Windows.Media.Brushes.Black;
                line.StrokeThickness = 1;
                DayTimeGrid.Children.Add(line);
                if (i % 2 == 0)
                {
                    var text = new TextBlock();
                    var time = new DateTime(1900, 1, 1, 0, 0, 0).AddMinutes(i*30);
                    text.Text = String.Format("{0:HH}:{0:mm}", time);
                    text.Margin = new Thickness(i*30*step + start - 30*step/2, 0, 0, 0);
                    DayTimeGrid.Children.Add(text);
                }
            }
        }
    }
}
