using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNet.SignalR.Client;
using WOT.Kiosk.Models;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    public partial class FindNameStep2 : Page
    {
        public string KioskName = Settings.Default.KioskName;

        public FindNameStep2(string searchTerm)
        {
            InitializeComponent();
            var db = new AppContext();

            var firstname = searchTerm.Split(' ')[0];
            var lastname = searchTerm.Split(' ').Count() > 1 ? searchTerm.Split(' ')[1] : "";

            var list = lastname != null ?
                db.Persons.Where(x => x.Firstname.Contains(firstname) && x.Lastname.Contains(lastname)).ToList() :
                db.Persons.Where(x => x.Firstname.Contains(firstname) || x.Lastname.Contains(lastname)).ToList();

            personList.ItemsSource = list;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            var item = personList.SelectedItem;
            var message = " ";

            switch (ConnectionManager.Connection.State)
            {
                case ConnectionState.Connected:
                    ConnectionManager.HubProxy.Invoke("Send", KioskName, item.ToString());
                    break;
                case ConnectionState.Disconnected:
                    //TODO: Log error and display message of name not sent to display
                    Debug.WriteLine("Connection is disconnected.");
                    message +=
                        "Currently unable to display your name on the Wall due to the connection is unavailable. " +
                        "You can try later to find your name from the Welcome page and choose to display it the Wall.";
                    break;
            }
            NavigationService.Navigate(new FinishPage(message));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
