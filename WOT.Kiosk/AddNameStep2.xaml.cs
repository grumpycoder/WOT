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
    /// Interaction logic for AddNameStep2.xaml
    /// </summary>
    public partial class AddNameStep2 : Page
    {
        private Person _person; 
        public AddNameStep2(Person person)
        {
            InitializeComponent();
            _person = person;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            _person.ZipCode = tbZipCode.Password;
            NavigationService.Navigate(new AddNameStep3(_person));
        }

        private void key_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)e.OriginalSource;
            var s = btn.Content.ToString();
            tbZipCode.Password += s;
        }
    }
}
