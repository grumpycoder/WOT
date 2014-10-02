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
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using WOT.Kiosk.Models;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    /// <summary>
    /// Interaction logic for AddNameStep3.xaml
    /// </summary>
    public partial class AddNameStep3 : Page
    {
        private Person _person;
        public string KioskName = Settings.Default.KioskName;
        private AppContext db = new AppContext();

        public AddNameStep3(Person person)
        {
            InitializeComponent();
            _person = person;
            tbPersonName.Text = _person.ToString();
        }

        private void btnAgree_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add person to database and send to big screen app
            db.Persons.Add(_person);
            db.SaveChanges();
            ConnectionManager.HubProxy.Invoke("Send", KioskName, _person.ToString());
            NavigationService.Navigate(new FinishPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WelcomePage());
        }
    }
}
