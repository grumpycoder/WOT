using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace WOT.HubService
{
    public class MessageHub : Hub
    {
        public Task SendMessage(string message)
        {
            var formatedMessage = String.Format("{0}:{1}: {2}", DateTime.Now, LocalCache.Users[Context.ConnectionId], message);
            Console.WriteLine(formatedMessage);
            return Clients.All.addMessage(formatedMessage);
        }

        public Boolean Join(string username)
        {
            LocalCache.Users.Add(Context.ConnectionId, username);
            Clients.Others.addMessage(username + " joined.");
            Console.WriteLine("{0}:{1} ({2}) connected.", DateTime.Now, Context.ConnectionId, username);
            return true;
        }

        public List<String> GetUsers()
        {
            return LocalCache.Users.Values.ToList();
        }


        public void Send(string kiosk, string message)
        {
            Clients.All.sendName(kiosk, message);
            Clients.All.reportStatus(kiosk, message);
            Console.WriteLine("{0}:{1} sent {2}", DateTime.Now, kiosk, message);
        }

        public override Task OnConnected()
        {
            Console.WriteLine("{0}:{1} Connected", DateTime.Now, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            Console.WriteLine("{0}:{1} Connected", DateTime.Now, Context.ConnectionId);
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var username = LocalCache.Users[Context.ConnectionId];
            Console.WriteLine("{0}:{1} ({2}) Disconnected", DateTime.Now, Context.ConnectionId, username);
            return base.OnDisconnected(stopCalled);
        }
    }
}