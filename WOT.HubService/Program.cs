using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Hosting;
using WOT.HubService.Properties;
using Microsoft.Owin.Host.HttpListener;

namespace WOT.HubService
{
      
    class Program
    {
        public IDisposable SignalR;
        public static string ServerURI = Settings.Default.ServerURI;


        private const string HubName = "MessageHub";
        private static readonly HubConnection _connection = new HubConnection(ServerURI);
        private static IHubProxy _hubProxy;

        public static HubConnection Connection { get { return _connection; } }

        public async static void ConnectAsync()
        {
            _hubProxy = Connection.CreateHubProxy(HubName);
            try
            {
                await Connection.Start();
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

        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(ServerURI))
            {
                Console.WriteLine("Server started: {0}", ServerURI);
                ConnectAsync();

                var quitNow = false;
                while (!quitNow)
                {
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "/help":
                            Console.WriteLine("/users - Get list of currently connected users.");
                            Console.WriteLine("/quit - Quits Hub application.");
                            break;

                        case "/version":
                            Console.WriteLine("This should be version.");
                            break;

                        case "/users":
                            _hubProxy.Invoke<List<String>>("GetUsers").ContinueWith(task =>
                            {
                                if (task.IsFaulted) return;

                                if (task.Result.Count == 0)
                                {
                                    Console.WriteLine("No connections");
                                    return;
                                }
                                foreach (var user in task.Result)
                                {
                                    Console.WriteLine(user);
                                }
                            });
                            break;
                        case "/quit":
                            quitNow = true;
                            break;

                        default:
                            Console.WriteLine("Unknown Command " + command);
                            break;
                    }
                }
            }




        }
     
    }
}
