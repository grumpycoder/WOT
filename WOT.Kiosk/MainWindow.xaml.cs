using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string KioskName = Settings.Default.KioskName;
        public IHubProxy HubProxy { get; set; }
        private string ServerURI = Settings.Default.ServerURI;
        private string HubName = Settings.Default.HubName; 
        public HubConnection Connection { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Height = Settings.Default.AppHeight;
            mainWindow.Width = Settings.Default.AppWidth;
            ConnectAsync();
        }



        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", KioskName, tbFirstName.Text);
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);

            HubProxy = Connection.CreateHubProxy(HubName);
            HubProxy.On<string, string>("sendName",
                (name, message) =>
                    this.Dispatcher.Invoke(
                        () => StatusText.Content = String.Format("{0}: {1}\r", name, message)));

            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException e)
            {
                StatusText.Content = "Unable to connect to server: Start server before connecting clients";
                StatusText.Content = e.InnerException.ToString();
                return;
            }
            StatusText.Content = "Connected to server at " + ServerURI;
        }

    }
}
