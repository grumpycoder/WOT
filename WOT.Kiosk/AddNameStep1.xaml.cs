using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WOT.Kiosk.Models;

namespace WOT.Kiosk
{
    public partial class AddNameStep1 : Page
    {
        private Person _person; 
        public AddNameStep1()
        {
            InitializeComponent();
            tbFirstname.Focus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            _person = new Person()
            {
                Firstname = tbFirstname.Text,
                Lastname = tbLastname.Text,
                EmailAddress = tbEmailAddress.Text
            };
            NavigationService.Navigate(new AddNameStep2(_person));
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

        private void textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox) sender).Background = Brushes.Yellow;
        }

        private void textbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox) sender).Background = Brushes.White;
        }

    }
}
