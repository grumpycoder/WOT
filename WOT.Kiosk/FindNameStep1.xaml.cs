using System;
using System.Collections.Generic;
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
            (sender as TextBox).Background = Brushes.Yellow;
        }

        private void textbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).Background = Brushes.White;
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
