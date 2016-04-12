using System;
using System.Threading;
using System.Threading.Tasks;

using Android.Content;
using Android.Util;

using App4.Services;

namespace App4
{
    public class App
    {
        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };

        protected readonly string logTag = "App";
        protected LocationServiceConnection locationServiceConnection;

        public static App Current
        {
            get { return current; }
        }
        private static App current;

        public LocationService LocationService
        {
            get
            {
                if (this.locationServiceConnection.Binder == null)
                    throw new Exception("Service not bound yet");
                return this.locationServiceConnection.Binder.Service;
            }
        }

        #region Application context

        static App()
        {
            current = new App();
        }
        protected App()
        {
            new Task(() => {
                Log.Debug(logTag, "Calling StartService");
                Android.App.Application.Context.StartService(new Intent(Android.App.Application.Context, typeof(LocationService)));
                this.locationServiceConnection = new LocationServiceConnection(null);
                this.locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
                    Log.Debug(logTag, "Service Connected");
                    this.LocationServiceConnected(this, e);
                };
                Intent locationServiceIntent = new Intent(Android.App.Application.Context, typeof(LocationService));
                Log.Debug(logTag, "Calling service binding");
                Android.App.Application.Context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);

            }).Start();
        }

        #endregion

    }
}


