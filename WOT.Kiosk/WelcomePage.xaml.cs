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
using WOT.Kiosk.Models;

namespace WOT.Kiosk
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
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
