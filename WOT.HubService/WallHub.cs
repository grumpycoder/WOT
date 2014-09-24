using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace WOT.HubService
{
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