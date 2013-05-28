using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Ester.Model.BaseClasses;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Ester.Model.Events;

namespace Ester.Model.UserControls
{
	public delegate void ChangeZoomAllowedEventHandler(bool allow);
	/// <summary>
	/// Interaction logic for ZoomPanel.xaml
	/// </summary>
	public partial class ZoomPanel : UserControl
	{
		private readonly IEventAggregator _eventAggregator;
		private bool _animating;
		private bool _canZoom = true;
		Point? _lastCenterPositionOnTarget;
		Point? _lastMousePositionOnTarget;
		private Point? _lastDragPoint;
		private Point _lastMouseDownPoint;

		public event ChangeZoomAllowedEventHandler ChangeZoomAllowedEvent;

		public void OnChangeZoomAllowed(bool allow)
		{
			ChangeZoomAllowedEventHandler handler = ChangeZoomAllowedEvent;
			if (handler != null) handler(allow);
		}

		public ZoomPanel()
		{
			InitializeComponent();

			_eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
			_eventAggregator.GetEvent<ZoomInto>().Subscribe(OnZoomInto);
			_eventAggregator.GetEvent<ShowFullPlan>().Subscribe(ShowAll);
			_eventAggregator.GetEvent<PlanControlDragEvent>().Subscribe(CanDragChanged);
			_eventAggregator.GetEvent<ChangeZoomAllowedEvent>().Subscribe(CanZoomChanged);


			PlanScrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
			PlanScrollViewer.MouseLeave += OnMouseLeave;
			PlanScrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
			PlanScrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
			PlanScrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
			PlanScrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			PlanScrollViewer.MouseMove += OnMouseMove;
			slider.ValueChanged += OnSliderValueChanged;
		}

		private void CanZoomChanged(bool allowed)
		{
			_canZoom = allowed;
		}

		private void CanDragChanged(bool value)
		{
			_canDrag = value;
		}

		private void PlanScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			PlanGrid.Width = PlanScrollViewer.ActualWidth - 25;
		}

		#region Zoom
		private void OnZoomInto(SensorBase obj)
		{
			Point k = obj.TranslatePoint(new Point(obj.ActualWidth / 2, obj.ActualHeight / 2), PlanGrid);
			double scale = obj.ActualHeight > obj.ActualWidth
						? PlanScrollViewer.ActualHeight / obj.ActualHeight * 0.6
						: PlanScrollViewer.ActualWidth / obj.ActualWidth * 0.6;
			_lastCenterPositionOnTarget = null;

			ScrollToPosition(k.X * scale - PlanScrollViewer.ActualWidth / 2, k.Y * scale - PlanScrollViewer.ActualHeight / 2, scale);
		}

		private void ShowAll(double speed = 800)
		{
			ScrollToPosition(0, 0, 1, speed);
		}

		private double newScale;
		private void ScrollToPosition(double x, double y, double scale, double speed = 800)
		{
			if (_animating) return;
			_animating = true;
			newScale = scale;
			var scaleAnim = new DoubleAnimation();
			scaleAnim.From = slider.Value;
			scaleAnim.To = scale;
			scaleAnim.DecelerationRatio = .2;
			scaleAnim.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
			scaleAnim.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
			var vertAnim = new DoubleAnimation();
			vertAnim.From = PlanScrollViewer.VerticalOffset;
			vertAnim.To = y;
			vertAnim.DecelerationRatio = .2;
			vertAnim.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
			vertAnim.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
			var horzAnim = new DoubleAnimation();
			horzAnim.From = PlanScrollViewer.HorizontalOffset;
			horzAnim.To = x;
			horzAnim.DecelerationRatio = .2;
			horzAnim.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
			horzAnim.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
			var sb = new Storyboard();
			sb.FillBehavior = FillBehavior.Stop;
			sb.Children.Add(scaleAnim);
			sb.Children.Add(vertAnim);
			sb.Children.Add(horzAnim);
			Storyboard.SetTarget(scaleAnim, this);
			Storyboard.SetTargetProperty(scaleAnim, new PropertyPath(ScaleProperty));
			Storyboard.SetTarget(vertAnim, PlanScrollViewer);
			Storyboard.SetTargetProperty(vertAnim, new PropertyPath(ExtScrollViewer.CurrentVerticalOffsetProperty));
			Storyboard.SetTarget(horzAnim, PlanScrollViewer);
			Storyboard.SetTargetProperty(horzAnim, new PropertyPath(ExtScrollViewer.CurrentHorizontalOffsetProperty));
			sb.Completed += AnimationCompleted;
			sb.Begin();
		}

		private void AnimationCompleted(object sender, EventArgs e)
		{
			slider.Value = newScale;
			_animating = false;
		}

		void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!_canZoom) return;
			if (_animating) return;

			//_eventAggregator.GetEvent<BuildingPartClearedEvent>().Publish(new object());
			if (e.Delta > 0) ZoomIn(); else ZoomOut();
			e.Handled = false;
		}

		private void ZoomIn()
		{
			if (slider.Value <= 49)
			{
				slider.Value += 1;
				_lastMousePositionOnTarget = Mouse.GetPosition(PlanGrid);
			}
			else
				slider.Value = 50;
		}

		private void ZoomOut()
		{
			if (slider.Value >= 2)
			{
				slider.Value -= 1;
				_lastMousePositionOnTarget = Mouse.GetPosition(PlanGrid);
			}
			else
				slider.Value = 1;
		}

		private void ZoomOut_Click(object sender, RoutedEventArgs e)
		{
			ZoomOut();
		}

		private void ZoomIn_Click(object sender, RoutedEventArgs e)
		{
			ZoomIn();
		}

		void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var centerOfViewport = new Point(PlanScrollViewer.ViewportWidth / 2, PlanScrollViewer.ViewportHeight / 2);
			_lastCenterPositionOnTarget = PlanScrollViewer.TranslatePoint(centerOfViewport, PlanGrid);
			ShowAllButton.Visibility = e.NewValue == 1 ? Visibility.Hidden : Visibility.Visible;
		}

		void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (_animating || (e.ExtentHeightChange == 0 && e.ExtentWidthChange == 0)) return;

			Point? targetBefore = null;
			Point? targetNow = null;

			if (!_lastMousePositionOnTarget.HasValue)
			{
				if (_lastCenterPositionOnTarget.HasValue)
				{
					var centerOfViewport = new Point(PlanScrollViewer.ViewportWidth / 2, PlanScrollViewer.ViewportHeight / 2);
					Point centerOfTargetNow = PlanScrollViewer.TranslatePoint(centerOfViewport, PlanGrid);

					targetBefore = _lastCenterPositionOnTarget;
					targetNow = centerOfTargetNow;
				}
			}
			else
			{
				targetBefore = _lastMousePositionOnTarget;
				targetNow = Mouse.GetPosition(PlanGrid);

				_lastMousePositionOnTarget = null;
			}

			if (targetBefore.HasValue)
			{
				double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
				double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

				double multiplicatorX = e.ExtentWidth / PlanGrid.ActualWidth;
				double multiplicatorY = e.ExtentHeight / PlanGrid.ActualHeight;

				double newOffsetX = PlanScrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
				double newOffsetY = PlanScrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

				if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
				{
					return;
				}

				PlanScrollViewer.ScrollToHorizontalOffset(newOffsetX);
				PlanScrollViewer.ScrollToVerticalOffset(newOffsetY);
			}
		}
		#endregion Zoom

		#region Drag
		void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_lastDragPoint.HasValue)
			{
				Point posNow = e.GetPosition(PlanScrollViewer);

				double dX = posNow.X - _lastDragPoint.Value.X;
				double dY = posNow.Y - _lastDragPoint.Value.Y;

				_lastDragPoint = posNow;

				PlanScrollViewer.ScrollToHorizontalOffset(PlanScrollViewer.HorizontalOffset - dX);
				PlanScrollViewer.ScrollToVerticalOffset(PlanScrollViewer.VerticalOffset - dY);
			}
		}

		private bool _canDrag = true;
		void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_canDrag)
			{
				var mousePos = e.GetPosition(PlanScrollViewer);
				if (mousePos.X <= PlanScrollViewer.ViewportWidth && mousePos.Y <
					PlanScrollViewer.ViewportHeight) //make sure we still can use the scrollbars
				{
					PlanScrollViewer.Cursor = Cursors.SizeAll;
					_lastDragPoint = mousePos;
					_lastMouseDownPoint = mousePos;
				}
			}
		}

		void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			PlanScrollViewer.Cursor = Cursors.Arrow;
			PlanScrollViewer.ReleaseMouseCapture();
			//if (!_animating && _lastMouseDownPoint != e.GetPosition(PlanScrollViewer))
			//_eventAggregator.GetEvent<BuildingPartClearedEvent>().Publish(new object());
			_lastDragPoint = null;
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			PlanScrollViewer.Cursor = Cursors.Arrow;
			PlanScrollViewer.ReleaseMouseCapture();
			_lastDragPoint = null;
		}
		#endregion

		#region Dependency properties
		public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
			"Scale", typeof(double), typeof(ZoomPanel), new PropertyMetadata(1.0));

		public double Scale
		{
			get { return (double)GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}

		public static readonly DependencyProperty PanelContentProperty = DependencyProperty.Register(
	"PanelContent", typeof(object), typeof(ZoomPanel), new PropertyMetadata(OnPanelContentChanged));

		private static void OnPanelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ZoomPanel sender = d as ZoomPanel;
			if (sender == null) return;

			//sender.p1.Content = e.NewValue;
		}

		public object PanelContent
		{
			get { return (object)GetValue(PanelContentProperty); }
			set { SetValue(PanelContentProperty, value); }
		}
		#endregion

		public void ShowPlan(object content)
		{
			if (_changingPlan)
			{
				_newContent = content;
				return;
			}
			_changingPlan = true;

			if (Content1Grid.Visibility == Visibility.Collapsed)
			{
				Content1.Content = content;
				FadeInAnimation(Content1Grid, Content2Grid);
			}
			else
			{
				Content2.Content = content;
				FadeInAnimation(Content2Grid, Content1Grid);
			}
		}

		private void FadeInAnimation(FrameworkElement elementToShow, FrameworkElement elementToHide, int speed = 500)
		{
			_elementToHide = elementToHide;
			elementToShow.Opacity = 0;
			elementToShow.Margin = new Thickness(0);
			elementToShow.Visibility = Visibility.Visible;

			var fadeInAnimation = new DoubleAnimation();
			fadeInAnimation.From = 0;
			fadeInAnimation.To = 1;
			fadeInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(speed / 2));
			fadeInAnimation.BeginTime = TimeSpan.FromMilliseconds(speed / 2);
			var fadeOutAnimation = new DoubleAnimation();
			fadeOutAnimation.From = 1;
			fadeOutAnimation.To = 0;
			fadeOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(speed / 2));
			var scaleAnim = new DoubleAnimation();
			scaleAnim.From = slider.Value;
			scaleAnim.To = 1;
			scaleAnim.Duration = new Duration(TimeSpan.FromMilliseconds(0));
			scaleAnim.BeginTime = TimeSpan.FromMilliseconds(speed / 2);
			var sb = new Storyboard();
			sb.Children.Add(fadeInAnimation);
			sb.Children.Add(fadeOutAnimation);
			sb.Children.Add(scaleAnim);
			Storyboard.SetTarget(fadeInAnimation, elementToShow);
			Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(OpacityProperty));
			Storyboard.SetTarget(fadeOutAnimation, elementToHide);
			Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(OpacityProperty));
			Storyboard.SetTarget(scaleAnim, this);
			Storyboard.SetTargetProperty(scaleAnim, new PropertyPath(ScaleProperty));
			sb.FillBehavior = FillBehavior.Stop;
			sb.Completed += OnPlanChangeCompleted;
			sb.Begin();
		}

		public void ShowFromTop(object content)
		{
			SlidePage(content, true);
		}

		public void ShowFromBottom(object content)
		{
			SlidePage(content, false);
		}

		private object _newContent;
		private bool _changingPlan;
		private async void SlidePage(object content, bool fromTop)
		{
			if (_changingPlan)
			{
				_newContent = content;
				return;
			}
			_changingPlan = true;

			if (Scale != 1)
			{
				ShowAll(250);
				await Task.Delay(250);
			}

			if (Content1Grid.Visibility == Visibility.Collapsed)
			{
				Content1.Content = content;
				SlideAnimation(Content1Grid, Content2Grid, fromTop);
			}
			else
			{
				Content2.Content = content;
				SlideAnimation(Content2Grid, Content1Grid, fromTop);
			}
		}

		private FrameworkElement _elementToHide;


		private void SlideAnimation(FrameworkElement elementToShow, FrameworkElement elementToHide, bool fromTop, double speed = 500)
		{
			_elementToHide = elementToHide;
			Thickness slideInSourceMargin = fromTop
								   ? new Thickness(0, -PlanGrid.ActualHeight * 2, 0, 0)
								   : new Thickness(0, 0, 0, -PlanGrid.ActualHeight * 2);
			Thickness slideOutDestMargin = fromTop
								 ? new Thickness(0, 0, 0, -PlanGrid.ActualHeight * 2)
								 : new Thickness(0, -PlanGrid.ActualHeight * 2, 0, 0);

			elementToShow.Margin = slideInSourceMargin;
			elementToShow.Visibility = Visibility.Visible;

			var slideInAnimation = new ThicknessAnimation();
			slideInAnimation.From = slideInSourceMargin;
			slideInAnimation.To = new Thickness(0, 0, 0, 0);
			slideInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
			slideInAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 3 };
			var slideOutAnimation = new ThicknessAnimation();
			slideOutAnimation.From = new Thickness(0);
			slideOutAnimation.To = slideOutDestMargin;
			slideOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
			slideOutAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 3 };

			var sb = new Storyboard();
			sb.Children.Add(slideInAnimation);
			sb.Children.Add(slideOutAnimation);
			Storyboard.SetTarget(slideInAnimation, elementToShow);
			Storyboard.SetTargetProperty(slideInAnimation, new PropertyPath(MarginProperty));
			Storyboard.SetTarget(slideOutAnimation, elementToHide);
			Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath(MarginProperty));
			sb.FillBehavior = FillBehavior.Stop;
			sb.Completed += OnPlanChangeCompleted;
			sb.Begin();
		}

		private void OnPlanChangeCompleted(object sender, EventArgs e)
		{
			_elementToHide.Visibility = Visibility.Collapsed;
			_elementToHide = null;

			Content1Grid.Opacity = 1;
			Content2Grid.Opacity = 1;
			Content1Grid.Margin = new Thickness(0);
			Content2Grid.Margin = new Thickness(0);
			//slider.Value = 1;

			if (_newContent != null)
			{
				if (Content2Grid.Visibility == Visibility.Collapsed)
				{
					Content2.Content = null;
					Content1.Content = _newContent;
				}
				if (Content1Grid.Visibility == Visibility.Collapsed)
				{
					Content1.Content = null;
					Content2.Content = _newContent;
				}
				_newContent = null;
			}
			_changingPlan = false;
		}

		private void ShowAll_Click(object sender, RoutedEventArgs e)
		{
			ShowAll(400);
		}
	}
}
