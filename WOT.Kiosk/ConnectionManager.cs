using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
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

        //public static async void ConnectAsync()
        public static void ConnectAsync()
        {
            Connection.StateChanged += ConnectionOnStateChanged;
            
            HubProxy = Connection.CreateHubProxy(HubName);
            try
            {
                //await Connection.Start();
                Connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted != false) return;
                    if (Connection.State != ConnectionState.Connected) return;

                    Debug.WriteLine("Connected");
                    HubProxy.Invoke("Join", "Kiosk1").ContinueWith(task_join =>
                    {
                        if (task_join.IsFaulted)
                        {
                            Debug.WriteLine("Error during joining the server!");
                        }
                        else
                        {
                            var sub = HubProxy.Subscribe("addMessage");
                        }
                    });
                }).Wait();

            }
            catch (HttpRequestException e)
            {
                //TODO: Log message of error
            }
            catch (HttpClientException clientException)
            {
                //TODO: Log message of error
            }
        }

        private static void ConnectionOnStateChanged(StateChange stateChange)
        {
            Debug.WriteLine("Connection State: " + Connection.State);
        }

        public static void SendMessage(string message)
        {
            HubProxy.Invoke("SendMessage", message).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.WriteLine("Error during sending the message!");
                }
                else
                {
                    Debug.WriteLine("Message sent successfully");
                }
            });
        }
       
    }
}