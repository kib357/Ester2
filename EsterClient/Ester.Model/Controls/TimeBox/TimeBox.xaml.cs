using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ester.Model.Controls.TimeBox
{
    /// <summary>
    /// Interaction logic for TimeBox.xaml
    /// </summary>
    public partial class TimeBox : UserControl
    {
        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register("Hours", typeof(string), typeof(TimeBox));

        public string Hours
        {
            get { return (string)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register("Minutes", typeof(string), typeof(TimeBox));

        public string Minutes
        {
            get { return (string)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(string), typeof(TimeBox));

        public string Seconds
        {
            get { return (string)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public static readonly DependencyProperty SecondsVisibleProperty = DependencyProperty.Register("SecondsVisible", typeof(Visibility), typeof(TimeBox));

        public Visibility SecondsVisible
        {
            get { return (Visibility)GetValue(SecondsVisibleProperty); }
            set { SetValue(SecondsVisibleProperty, value); }
        }

        public static readonly DependencyProperty LengthInputStringProperty = DependencyProperty.Register("LengthInputString", typeof(int), typeof(TimeBox));

        public int LengthInputString 
        {
            get { return (int)GetValue(LengthInputStringProperty); }
            set { SetValue(LengthInputStringProperty, value); }
        }

        public static readonly DependencyProperty DefaultHourValueProperty = DependencyProperty.Register("DefaultHourValue", typeof(string), typeof(TimeBox), new PropertyMetadata("00"));

        public string DefaultHourValue 
        {
            get { return (string)GetValue(DefaultHourValueProperty); }
            set { SetValue(DefaultHourValueProperty, value); }
        }

        public static readonly DependencyProperty DefaultMinuteValueProperty = DependencyProperty.Register("DefaultMinuteValue", typeof(string), typeof(TimeBox), new PropertyMetadata("00"));

        public string DefaultMinuteValue
        {
            get { return (string)GetValue(DefaultMinuteValueProperty); }
            set { SetValue(DefaultMinuteValueProperty, value); }
        }

        public static readonly byte ZERO_NUMPAD_ASCII_POSITION = 96;
        public static readonly byte NINE_NUMPAD_ASCII_POSITION = 105;
        public static readonly byte NUMBERS_OFFSET = 48;
        public static readonly byte LENGTH_INPUT_STRING = 2;

        public TimeBox()
        {
            InitializeComponent();

            LengthInputString = LENGTH_INPUT_STRING;
        }

        public void Clear()
        {
            Hours = string.Empty;
            Minutes = string.Empty;
            Seconds = string.Empty;
        }

        private bool _IsNumber(Key key)
        {
            return key >= Key.D0 && key <= Key.D9 ||
                key >= Key.NumPad0 && key <= Key.NumPad9;
        }

        private char _TransformKeyToChar(Key keyInput) 
        {
            int keyvalue = KeyInterop.VirtualKeyFromKey(keyInput);

            return keyvalue >= ZERO_NUMPAD_ASCII_POSITION && keyvalue <= NINE_NUMPAD_ASCII_POSITION
                ? (char)(keyvalue - NUMBERS_OFFSET)
                : (char)keyvalue;
        }

        private bool _IsValidTime(string inputText, TimeInputType timeInputType) 
        {
            //ertfertertert

            bool valudationResult = false;

            switch (timeInputType)
            {
                case TimeInputType.Hours:
                    valudationResult = TimeInputValidation.IsValidHours(inputText);
                    break;
                case TimeInputType.Minutes:
                    valudationResult = TimeInputValidation.IsValidMinutes(inputText);
                    break;
                case TimeInputType.Seconds:
                    valudationResult = TimeInputValidation.IsValidSeconds(inputText);
                    break;
            }

            return valudationResult;
        }

        private void box_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox == null) return;

            if (_IsNumber(e.Key)) 
            {
                char inputSymb = _TransformKeyToChar(e.Key);
                TimeInputType timeInputType = (TimeInputType)int.Parse(tbox.Tag.ToString());

                if (tbox.Text.Length == LENGTH_INPUT_STRING)
                {
                    if (tbox.SelectionStart == LENGTH_INPUT_STRING &&
                        (
                            (TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Hours || 
                            ((TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Minutes) && SecondsVisible == System.Windows.Visibility.Visible)
                        )
                    {
                        tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                        if ((TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Hours)
                        {
                            minutes.Text = inputSymb.ToString();
                            minutes.SelectionStart = minutes.Text.Length;
                        }

                        if ((TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Minutes)
                        {
                            seconds.Text = inputSymb.ToString();
                            seconds.SelectionStart = seconds.Text.Length;
                        }
                    }

                    e.Handled = true;
                    return;
                }

                string resultInput = tbox.SelectionStart == 0
                    ? inputSymb + tbox.Text
                    : tbox.Text + inputSymb;

                e.Handled = !_IsValidTime(resultInput, timeInputType);
                return;
            }

            if (e.Key == Key.Back && 
                (TimeInputType)int.Parse(tbox.Tag.ToString()) != TimeInputType.Hours &&
                tbox.SelectionStart == 0)
            {
                tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
            }

            if (e.Key == Key.Right &&
                ((TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Hours || 
                ((TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Minutes) && SecondsVisible == System.Windows.Visibility.Visible) &&
                tbox.SelectionStart == LENGTH_INPUT_STRING)
            {
                tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
            }

            if (e.Key == Key.Left &&
                ((TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Minutes || (TimeInputType)int.Parse(tbox.Tag.ToString()) == TimeInputType.Seconds) &&
                tbox.SelectionStart == 0)
            {
                tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
            }

            //if (e.Key == Key.Back && 
            //    tbox.SelectionStart == 0 &&
            //    (string)tbox.Tag != "0" &&
            //    tbox.SelectionLength != 2) 
            //{
            //    tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
            //}

            //if (e.Key == Key.Right &&
            //    tbox.SelectionStart == 2 &&
            //    ((string)tbox.Tag == "0" || ((string)tbox.Tag == "1") && SecondsVisible == System.Windows.Visibility.Visible)) 
            //{
            //    tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
            //}

            //if (e.Key == Key.Left &&
            //    tbox.SelectionStart == 0 &&
            //    ((string)tbox.Tag == "1" || (string)tbox.Tag == "2")) 
            //{
            //    tbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
            //}

            if (!(e.Key == Key.Back ||
                e.Key == Key.Delete ||
                e.Key == Key.Right ||
                e.Key == Key.Left))
            {
                e.Handled = true;
            }
        }

        private void timebox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox == null) return;

            _lastTime = tbox.Text;
            tbox.Text = string.Empty;
            tbox.Focus();
        }

        private void timebox_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox == null) return;

            if (string.IsNullOrEmpty(tbox.Text)) 
            {
                tbox.Text = _lastTime;
            }
        }

        private void timebox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox == null) return;

            if (string.IsNullOrEmpty(tbox.Text))
            {
                TimeInputType timeInputType = (TimeInputType)int.Parse(tbox.Tag.ToString());

                switch (timeInputType) 
                {
                    case TimeInputType.Hours:
                        {
                            tbox.Text = DefaultHourValue;
                            break;
                        }
                    case TimeInputType.Minutes:
                        {
                            tbox.Text = DefaultMinuteValue;
                            break;
                        }
                }
            }
        }

        private void timebox_GotFocus(object sender, RoutedEventArgs e)
        {
            //timebox_MouseEnter(sender, null);
        }

        private string _lastTime;
    }
}
