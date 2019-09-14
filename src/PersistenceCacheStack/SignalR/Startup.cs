using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using PersistenceCacheStack.SignalR;

[assembly: OwinStartup(typeof(Startup))]
namespace PersistenceCacheStack.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                // Setup the cors middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                var hubConfiguration = new HubConfiguration
                {
                    EnableJSONP = false,
                    EnableDetailedErrors = true
                };

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
