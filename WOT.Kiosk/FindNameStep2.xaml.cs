using System.Windows;
using System.Windows.Controls;

namespace WOT.Kiosk
{
    public partial class FindNameStep2 : Page
    {
        public FindNameStep2(string searchTerm)
        {
            InitializeComponent();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FinishPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
