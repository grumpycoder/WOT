using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
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

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class WallHub : Hub
    {
        public void Send(string kiosk, string message)
        {
            Clients.All.sendName(kiosk, message);
            Console.WriteLine("{0} sent {1}", kiosk, message);
        }

        public override Task OnConnected()
        {
            Console.WriteLine("{0} Connected", Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("{0} Disconnected", Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}
