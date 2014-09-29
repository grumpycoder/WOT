using System;
using System.Net.Http;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    public partial class MainWindow : Window
    {
        private readonly string HubName = Settings.Default.HubName;
        private readonly string ServerURI = Settings.Default.ServerURI;
        public string KioskName = Settings.Default.KioskName;

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Height = Settings.Default.AppHeight;
            mainWindow.Width = Settings.Default.AppWidth;
            ConnectAsync();
            _mainFrame.Navigate(new WelcomePage());

            //TODO: Try reconnect when server back online
        }

        public IHubProxy HubProxy { get; set; }
        public HubConnection Connection { get; set; }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            //HubProxy.Invoke("Send", KioskName, tbFirstName.Text);
            //TODO: Save name to persistent storage
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            Connection.StateChanged += ConnectionOnStateChanged;
            HubProxy = Connection.CreateHubProxy(HubName);
            //HubProxy.On<string, string>("sendName",
            //    (name, message) =>
            //        Dispatcher.Invoke(
            //            () => StatusText.Content = String.Format("{0}: {1}\r", name, message)));

            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException e)
            {
                //TODO: Log message of error
            }
        }

        private void ConnectionOnStateChanged(StateChange stateChange)
        {
            if (Connection.State == ConnectionState.Disconnected)
            {
                //Dispatcher.Invoke(() => StatusText.Content = "Disconnected");
            }
            if (Connection.State == ConnectionState.Connecting)
            {
                //Dispatcher.Invoke(() => StatusText.Content = "Connecting");
            }
            if (Connection.State == ConnectionState.Connected)
            {
                //Dispatcher.Invoke(() => StatusText.Content = "Connected");
            }
            //Dispatcher.Invoke(() => StatusText.Content = Connection.State);
        }
    }
}