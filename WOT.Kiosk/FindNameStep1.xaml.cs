using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WOT.Kiosk
{
    /// <summary>
    /// Interaction logic for FindNameStep1.xaml
    /// </summary>
    public partial class FindNameStep1 : Page
    {
        public FindNameStep1()
        {
            InitializeComponent();
            tbSearchName.Focus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FindNameStep2(tbSearchName.Text));
        }

        private void textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox) sender).Background = Brushes.Yellow;
        }

        private void textbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox) sender).Background = Brushes.White;
        }

        private void key_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)e.OriginalSource;
            var s = btn.Content.ToString();
            var control = Keyboard.FocusedElement as UIElement;

            if (control != null && control.GetType() != typeof(TextBox))
            {
                control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

            if (control != null && control.GetType() == typeof(TextBox))
            {
                var textbox = control as TextBox;
                var cIndex = textbox.CaretIndex;

                switch (s)
                {
                    case "TAB":
                        control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        break;
                    case "DEL":
                        if (cIndex > 0)
                        {
                            textbox.Text = textbox.Text.Remove(cIndex - 1, 1);
                            textbox.CaretIndex = cIndex - 1;
                        }
                        break;
                    default:
                        textbox.Text = textbox.Text.Insert(cIndex, s);
                        textbox.CaretIndex = cIndex + 1;
                        break;
                }
            }
        }
    }
}
