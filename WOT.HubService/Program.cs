using System;
using Microsoft.Owin.Hosting;
using WOT.HubService.Properties;

namespace WOT.HubService
{
      
    class Program
    {
        public IDisposable SignalR;
        public static string ServerURI = Settings.Default.ServerURI;

        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(ServerURI))
            {
                Console.WriteLine("Server started: {0}", ServerURI);
                Console.ReadLine();
            }
        }
     
    }
}
