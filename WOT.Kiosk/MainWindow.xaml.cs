using System;
using System.Net.Http;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{

    public partial class MainWindow : Window
    {
        public string KioskName = Settings.Default.KioskName;
        public IHubProxy HubProxy { get; set; }
        private readonly string ServerURI = Settings.Default.ServerURI;
        private readonly string HubName = Settings.Default.HubName;
        public HubConnection Connection { get; set; }
        
        public MainWindow()
        {

            InitializeComponent();

            mainWindow.Height = Settings.Default.AppHeight;
            mainWindow.Width = Settings.Default.AppWidth;
            ConnectAsync();

            //TODO: Try reconnect when server back online
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", KioskName, tbFirstName.Text);
            //TODO: Save name to persistent storage
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            Connection.Closed += ConnectionOnClosed;
            Connection.StateChanged += ConnectionOnStateChanged;
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
                //TODO: Log message of error
                return;
            }
        }

        private void ConnectionOnStateChanged(StateChange stateChange)
        {
            if (Connection.State == ConnectionState.Disconnected)
            {
                this.Dispatcher.Invoke(() => StatusText.Content = "Disconnected");
            }
            if (Connection.State == ConnectionState.Connecting)
            {
                this.Dispatcher.Invoke(() => StatusText.Content = "Connecting");
            }
            if (Connection.State == ConnectionState.Connected)
            {
                this.Dispatcher.Invoke(() => StatusText.Content = "Connected");
            }
            this.Dispatcher.Invoke(() => StatusText.Content = Connection.State);
        }

        private void ConnectionOnClosed()
        {
            this.Dispatcher.Invoke(()=> StatusText.Content = "Disconnected");
        }
    }
}
