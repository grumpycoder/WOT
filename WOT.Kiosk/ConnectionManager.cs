using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNet.SignalR.Client;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    public class ConnectionManager
    {
        private static readonly string ServerURI = Settings.Default.ServerURI;
        private static readonly string HubName = Settings.Default.HubName;
        private static readonly HubConnection _connection = new HubConnection(ServerURI);
        public static IHubProxy HubProxy; 

        public static HubConnection Connection { get { return _connection; } }

        public static async void ConnectAsync()
        {
            //Connection = new HubConnection(ServerURI);
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

        private static void ConnectionOnStateChanged(StateChange stateChange)
        {
            Debug.WriteLine("Connection State: " + stateChange);
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