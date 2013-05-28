using System.Windows;
using System.Windows.Media.Animation;
using Ester.ViewModel;

namespace Ester
{
	/// <summary>
	/// Interaction logic for Shell.xaml
	/// </summary>
	public partial class Shell : Window
	{
		public Shell()
		{
			InitializeComponent();
		}

		private void MinimizeApplication(object sender, RoutedEventArgs e)
		{
			Application.Current.MainWindow.WindowState = WindowState.Minimized;
		}

		private void CloseApplication(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
		{
			(DataContext as ShellViewModel).AppCloseCommand.Execute();
		}
	}
}
