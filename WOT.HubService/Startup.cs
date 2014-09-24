using Microsoft.Owin.Cors;
using Owin;

namespace WOT.HubService
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}