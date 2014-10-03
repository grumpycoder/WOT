using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WOT.Kiosk
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void btnFindName_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FindNameStep1());
        }

        private void btnAddName_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddNameStep1());
        }
    }
}
