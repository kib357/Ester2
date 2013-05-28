using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ester.Model.UserControls
{
    public class StartEventArgs : System.EventArgs
    {
        public int StartPoint { get; set; }
    }
    /// <summary>
    /// Interaction logic for Loader.xaml
    /// </summary>
    public partial class Loader : UserControl
    {
        private readonly List<Storyboard> _storyList = new List<Storyboard>();

        private const int NextEllipseAnimationDelay = 250;
        private const int StartDelay = 250;
        private const int FadeOutDelay = 500;
        private const int MoveOutTime = 1200;//1000;
        private const int MoveInTime = 2000;//1400;
        private const int FadeOutTime = 600;//500;
        private const int FadeInTime = 250;
        private const int EndPointPosition = 390;
        private bool _stopped;

        public Loader()
        {
            InitializeComponent();
        }

        private async void StartAnimation()
        {
            EllipseFadeOut(Ellipse5, new StartEventArgs {StartPoint = 250});
            await Task.Delay(NextEllipseAnimationDelay);
            if (_stopped) return;
            EllipseFadeOut(Ellipse4, new StartEventArgs {StartPoint = 220});
            await Task.Delay(NextEllipseAnimationDelay);
            if (_stopped) return;
            EllipseFadeOut(Ellipse3, new StartEventArgs {StartPoint = 190});
            await Task.Delay(NextEllipseAnimationDelay);
            if (_stopped) return;
            EllipseFadeOut(Ellipse2, new StartEventArgs {StartPoint = 160});
            await Task.Delay(NextEllipseAnimationDelay);
            if (_stopped) return;
            EllipseFadeOut(Ellipse1, new StartEventArgs {StartPoint = 130});
        }

        private async void EllipseFadeOut(object sender, StartEventArgs e)
        {
            await Task.Delay(StartDelay);
            if (_stopped) return;

            var moveOutAnimation = new DoubleAnimation();
            moveOutAnimation.From = e.StartPoint;
            moveOutAnimation.To = EndPointPosition;
            moveOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(MoveOutTime));
            //moveOutAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseIn, Exponent = 2 };

            var sbMoveOut = new Storyboard();
            _storyList.Add(sbMoveOut);
            sbMoveOut.Children.Add(moveOutAnimation);
            Storyboard.SetTarget(moveOutAnimation, (Ellipse)sender);
            Storyboard.SetTargetProperty(moveOutAnimation, new PropertyPath(Canvas.LeftProperty));
            sbMoveOut.Completed += (sender1, args) => EllipseFadeIn(sender, e);

            var fadeOutAnimation = new DoubleAnimation();
            fadeOutAnimation.From = 1;
            fadeOutAnimation.To = 0;
            fadeOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FadeOutTime));

            var sbFadeOut = new Storyboard();
            _storyList.Add(sbFadeOut);
            sbFadeOut.Children.Add(fadeOutAnimation);
            Storyboard.SetTarget(fadeOutAnimation, (Ellipse)sender);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(OpacityProperty));

            sbMoveOut.Begin();

            await Task.Delay(FadeOutDelay);
            if (_stopped) return;
            sbFadeOut.Begin();
        }

        private void EllipseFadeIn(object sender, StartEventArgs e)
        {
            var moveInAnimation = new DoubleAnimation();
            moveInAnimation.From = 0;
            moveInAnimation.To = e.StartPoint;
            moveInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(MoveInTime));
            moveInAnimation.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 7 };

            var fadeInAnimation = new DoubleAnimation();
            fadeInAnimation.From = 0;
            fadeInAnimation.To = 1;
            fadeInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FadeInTime));

            var sb = new Storyboard();
            _storyList.Add(sb);
            sb.Children.Add(moveInAnimation);
            sb.Children.Add(fadeInAnimation);
            Storyboard.SetTarget(moveInAnimation, (Ellipse)sender);
            Storyboard.SetTargetProperty(moveInAnimation, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTarget(fadeInAnimation, (Ellipse)sender);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(OpacityProperty));
            sb.Completed += (sender1, args) => EllipseFadeOut(sender, e);

            sb.Begin();
        }

        private void UserControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _stopped = false;
                StartAnimation();
            }
            else
            {
                _stopped = true;
                foreach (var storyboard in _storyList)
                    storyboard.Stop();
                _storyList.Clear();
                Ellipse1.SetValue(Canvas.LeftProperty, (double)130);
                Ellipse2.SetValue(Canvas.LeftProperty, (double)160);
                Ellipse3.SetValue(Canvas.LeftProperty, (double)190);
                Ellipse4.SetValue(Canvas.LeftProperty, (double)220);
                Ellipse5.SetValue(Canvas.LeftProperty, (double)250);
            }
        }


        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register(
            "FillColor", typeof(Brush), typeof(Loader), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xFF,0xFF,0xFF))));

        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }
    }
}
