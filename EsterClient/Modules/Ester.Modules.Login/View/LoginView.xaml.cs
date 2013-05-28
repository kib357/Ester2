using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Ester.Modules.Login.ViewModel;
using Microsoft.Practices.Prism.Events;

namespace Ester.Modules.Login
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class LoginView : UserControl
	{
		public LoginView()
		{
			InitializeComponent();
		}

		private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			passwordTextBox.Visibility = Visibility.Collapsed;
			Keyboard.Focus(passwordBox);
		}

		private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(passwordBox.Password))
				passwordTextBox.Visibility = Visibility.Visible;
		}

		private void HideGrid()
		{
			if (LoginGrid.Visibility == Visibility.Collapsed)
				return;

			var hideGridAnimation = new ThicknessAnimation();
			hideGridAnimation.From = new Thickness(0);
			hideGridAnimation.To = new Thickness(0, FullScreenGrid.ActualHeight * -2, 0, 0);
			hideGridAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));
			hideGridAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
			var sb = new Storyboard();
			sb.Children.Add(hideGridAnimation);
			Storyboard.SetTarget(hideGridAnimation, LoginGrid);
			Storyboard.SetTargetProperty(hideGridAnimation, new PropertyPath(MarginProperty));
			sb.Completed += HideGridCompleted;
			sb.Begin();
		}

		private void HideGridCompleted(object sender, EventArgs e)
		{
			LoginGrid.Visibility = Visibility.Collapsed;
		}

		private void ShowGrid()
		{
			if (LoginGrid.Visibility == Visibility.Visible)
				return;

			LoginGrid.Visibility = Visibility.Visible;

			var hideGridAnimation = new ThicknessAnimation();
			hideGridAnimation.From = new Thickness(0, FullScreenGrid.ActualHeight * -2, 0, 0);
			hideGridAnimation.To = new Thickness(0);
			hideGridAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));
			hideGridAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
			var sb = new Storyboard();
			sb.Children.Add(hideGridAnimation);
			Storyboard.SetTarget(hideGridAnimation, LoginGrid);
			Storyboard.SetTargetProperty(hideGridAnimation, new PropertyPath(MarginProperty));
			sb.Completed += ShowGridCompleted;
			sb.Begin();
		}

		private void ShowGridCompleted(object sender, EventArgs e)
		{
		}

		private void LoginViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var dc = e.NewValue as LoginViewModel;
			if (dc == null) return;

			dc.SuccessLoginEvent += HideGrid;
			dc.FailedLoginEvent += ShakeTextBoxes;
		}

		private void ShakeTextBoxes()
		{
			loginTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x99, 0x99));
			loginTextBox.BorderThickness = new Thickness(2);
			passwordTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x99, 0x99));
			passwordTextBox.BorderThickness = new Thickness(2);
			passwordBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x99, 0x99));
			passwordBox.BorderThickness = new Thickness(2);

			var loginTextBoxAnimation = new ThicknessAnimation();
			loginTextBoxAnimation.From = new Thickness(0, 0, 0, 0);
			loginTextBoxAnimation.To = new Thickness(10, 0, 0, 0);
			loginTextBoxAnimation.AutoReverse = true;
			loginTextBoxAnimation.RepeatBehavior = new RepeatBehavior(4);
			loginTextBoxAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
			var passwordTextBoxAnimation = new ThicknessAnimation();
			passwordTextBoxAnimation.From = new Thickness(0, 0, 0, 0);
			passwordTextBoxAnimation.To = new Thickness(10, 0, 0, 0);
			passwordTextBoxAnimation.AutoReverse = true;
			passwordTextBoxAnimation.RepeatBehavior = new RepeatBehavior(4);
			passwordTextBoxAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
			var passwordBoxAnimation = new ThicknessAnimation();
			passwordBoxAnimation.From = new Thickness(0, 0, 0, 0);
			passwordBoxAnimation.To = new Thickness(10, 0, 0, 0);
			passwordBoxAnimation.AutoReverse = true;
			passwordBoxAnimation.RepeatBehavior = new RepeatBehavior(4);
			passwordBoxAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));

			var sb = new Storyboard();
			sb.Children.Add(loginTextBoxAnimation);
			sb.Children.Add(passwordTextBoxAnimation);
			sb.Children.Add(passwordBoxAnimation);
			Storyboard.SetTarget(loginTextBoxAnimation, loginTextBox);
			Storyboard.SetTargetProperty(loginTextBoxAnimation, new PropertyPath(MarginProperty));
			Storyboard.SetTarget(passwordTextBoxAnimation, passwordTextBox);
			Storyboard.SetTargetProperty(passwordTextBoxAnimation, new PropertyPath(MarginProperty));
			Storyboard.SetTarget(passwordBoxAnimation, passwordBox);
			Storyboard.SetTargetProperty(passwordBoxAnimation, new PropertyPath(MarginProperty));
			//sb.Completed += AnimationCompleted;
			sb.Begin();
		}

		private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
		{
			ResetTextBoxesBorder();
			passwordTextBox.Visibility = Visibility.Collapsed;
			Keyboard.Focus(passwordBox);
		}

		private void loginTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			ResetTextBoxesBorder();
		}

		private void ResetTextBoxesBorder()
		{
			loginTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
			loginTextBox.BorderThickness = new Thickness(1);
			passwordTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
			passwordTextBox.BorderThickness = new Thickness(1);
			passwordBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));
			passwordBox.BorderThickness = new Thickness(1);
		}

		private void LoginViewLoaded(object sender, RoutedEventArgs e)
		{
			MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		private void AppShutdown(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}