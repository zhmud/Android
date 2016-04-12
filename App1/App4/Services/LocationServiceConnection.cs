using System;

using Android.Content;
using Android.OS;
using Android.Util;

namespace App4.Services
{
    public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate { };

        public LocationServiceBinder Binder
        {
            get { return this.binder; }
            set { this.binder = value; }
        }
        protected LocationServiceBinder binder;

        public LocationServiceConnection(LocationServiceBinder binder)
        {
            if (binder != null)
            {
                this.binder = binder;
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            LocationServiceBinder serviceBinder = service as LocationServiceBinder;
            if (serviceBinder != null)
            {
                this.binder = serviceBinder;
                this.binder.IsBound = true;
                Log.Debug("ServiceConnection", "OnServiceConnected Called");
                this.ServiceConnected(this, new ServiceConnectedEventArgs() { Binder = service });

                serviceBinder.Service.StartLocationUpdates();
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this.binder.IsBound = false;
            Log.Debug("ServiceConnection", "Service unbound");
        }
    }
}

