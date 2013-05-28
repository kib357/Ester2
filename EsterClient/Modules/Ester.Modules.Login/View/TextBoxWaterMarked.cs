using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Ester.Modules.Login.View
{
    public class TextBoxWatermarked : TextBox
    {
        public string Watermark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }
        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(TextBoxWatermarked), new PropertyMetadata(new PropertyChangedCallback(OnWatermarkChanged)));

        private bool _isWatermarked = false;
        private Binding _textBinding = null;

        public TextBoxWatermarked()
        {
            Loaded += (s, ea) => ShowWatermark();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            HideWatermark();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            ShowWatermark();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (_isWatermarked && Text != Watermark)
            {
                TextChange tc = e.Changes.LastOrDefault();
                if (tc == null) return;

                var newText = (tc.AddedLength > 0) ? Text.Substring(tc.Offset, tc.AddedLength) : "";
                HideWatermark(newText);
            }
        }

        private static void OnWatermarkChanged(DependencyObject sender, DependencyPropertyChangedEventArgs ea)
        {
            var tbw = sender as TextBoxWatermarked;
            if (tbw == null || !tbw.IsLoaded) return; //needed to check IsLoaded so that we didn't dive into the ShowWatermark() routine before initial Bindings had been made
            tbw.ShowWatermark();
        }

        private void ShowWatermark()
        {
            if (String.IsNullOrEmpty(Text) && !String.IsNullOrEmpty(Watermark))
            {
                _isWatermarked = true;

                //save the existing binding so it can be restored
                _textBinding = BindingOperations.GetBinding(this, TextProperty);

                //blank out the existing binding so we can throw in our Watermark
                BindingOperations.ClearBinding(this, TextProperty);

                //set the signature watermark gray
                Foreground = new SolidColorBrush(Colors.Gray);

                //display our watermark text
                Text = Watermark;
            }
        }

        private void HideWatermark(string value = "")
        {
            if (_isWatermarked)
            {
                _isWatermarked = false;
                ClearValue(ForegroundProperty);
                if (_textBinding != null) SetBinding(TextProperty, _textBinding);
                Text = value;
                CaretIndex = Text.Length;
            }
        }

    }
}

