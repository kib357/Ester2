using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Ester.Model.UserControls
{
	/// <summary>
	/// Interaction logic for FlipPanel.xaml
	/// </summary>
	public partial class FlipPanel : UserControl
	{
		public FlipPanel()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty FrontTemplateProperty =
			DependencyProperty.Register("FrontTemplate", typeof(DataTemplate), typeof(FlipPanel), new FrameworkPropertyMetadata());

		public DataTemplate FrontTemplate
		{
			get
			{
				return (DataTemplate)GetValue(FrontTemplateProperty);
			}
			set
			{
				SetValue(FrontTemplateProperty, value);
			}
		}

		public static readonly DependencyProperty BackTemplateProperty =
			DependencyProperty.Register("BackTemplate", typeof(DataTemplate), typeof(FlipPanel), new FrameworkPropertyMetadata());

		public DataTemplate BackTemplate
		{
			get
			{
				return (DataTemplate)GetValue(BackTemplateProperty);
			}
			set
			{
				SetValue(BackTemplateProperty, value);
			}
		}
	}
}
