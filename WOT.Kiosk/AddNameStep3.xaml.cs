using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNet.SignalR.Client;
using WOT.Kiosk.Models;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    public partial class AddNameStep3 : Page
    {
        private readonly Person _person;
        public string KioskName = Settings.Default.KioskName;
        private readonly AppContext db = new AppContext();

        public AddNameStep3(Person person)
        {
            InitializeComponent();
            _person = person;
            tbPersonName.Text = _person.ToString();
        }

        private void btnAgree_Click(object sender, RoutedEventArgs e)
        {
            var isSaved = false;
            var message = string.Empty;
            try
            {
                db.Persons.Add(_person);
                db.SaveChanges();
                isSaved = true;
            }
            catch (Exception exception)
            {
                //TODO: Log error of DB save operation and display message
                Debug.WriteLine(exception);
            }
            switch (ConnectionManager.Connection.State)
            {
                case ConnectionState.Connected:
                    ConnectionManager.HubProxy.Invoke("Send", KioskName, _person.ToString());
                    break;
                case ConnectionState.Disconnected:
                    //TODO: Log error and display message of name not sent to display
                    Debug.WriteLine("Connection is disconnected.");
                    if (isSaved)
                    {
                        message +=
                            "Your name has been saved, but currently unable to display your name on the Wall due to the connection is unavailable. " +
                            "You can try later to find your name from the Welcome page and choose to display it the Wall.";
                    }
                    else
                    {
                        message +=
                            "Your name has NOT been saved, and is unable to display your name on the Wall due to the connection is unavailable. " +
                            "You can try later to enter your name again from the Welcome page.";
                    }
                    break;
            }
            NavigationService.Navigate(new FinishPage(message));
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
