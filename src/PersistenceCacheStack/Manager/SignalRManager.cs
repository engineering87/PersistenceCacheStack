using System;
using Microsoft.Owin.Hosting;
using PersistenceCacheStack.SignalR;

namespace PersistenceCacheStack.Manager
{
    public class SignalRManager
    {
        private readonly IDisposable m_WebApp = null;

        public SignalRManager()
        {
            var address = string.Concat("");
            m_WebApp = WebApp.Start<Startup>(address);
        }


    }
}
