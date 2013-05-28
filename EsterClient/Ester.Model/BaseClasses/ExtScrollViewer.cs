using System.Windows;
using System.Windows.Controls;

namespace Ester.Model.BaseClasses
{
    public class ExtScrollViewer : ScrollViewer
    {
        public static DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
            "CurrentVerticalOffset", typeof(double), typeof(ExtScrollViewer), new PropertyMetadata(OnVerticalChanged));
        public static DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register(
            "CurrentHorizontalOffsetOffset", typeof(double), typeof(ExtScrollViewer), new PropertyMetadata(OnHorizontalChanged));

        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as ExtScrollViewer;
            if (viewer == null) return;
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as ExtScrollViewer;
            if (viewer == null) return;
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        public double CurrentHorizontalOffset
        {
            get { return (double)this.GetValue(CurrentHorizontalOffsetProperty); }
            set { this.SetValue(CurrentHorizontalOffsetProperty, value); }
        }

        public double CurrentVerticalOffset
        {
            get { return (double)this.GetValue(CurrentVerticalOffsetProperty); }
            set { this.SetValue(CurrentVerticalOffsetProperty, value); }
        }
    }
}
