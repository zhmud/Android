using System;

using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using Android.Locations;

namespace App4.Services
{
    [Service]
    public class LocationService : Service, ILocationListener
    {
        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        

        public LocationService()
        {
        }

        protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService("location") as LocationManager;

        readonly string logTag = "LocationService";
        IBinder binder;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(logTag, "OnCreate called in the Location Service");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(logTag, "LocationService started");

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(logTag, "Client now bound to service");

            binder = new LocationServiceBinder(this);
            return binder;
        }

        public void StartLocationUpdates()
        {
            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
            Log.Debug(logTag, string.Format("You are about to get location updates via {0}", locationProvider));

            LocMgr.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 0, this);

            Log.Debug(logTag, "Now sending location updates");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(logTag, "Service has been terminated");
        }

        #region ILocationListener implementation

        public void OnLocationChanged(Android.Locations.Location location)
        {
            this.LocationChanged(this, new LocationChangedEventArgs(location));

            Log.Debug(logTag, String.Format("Latitude is {0}", location.Latitude));
            Log.Debug(logTag, String.Format("Longitude is {0}", location.Longitude));
            Log.Debug(logTag, String.Format("Altitude is {0}", location.Altitude));
            Log.Debug(logTag, String.Format("Speed is {0}", location.Speed));
            Log.Debug(logTag, String.Format("Accuracy is {0}", location.Accuracy));
            Log.Debug(logTag, String.Format("Bearing is {0}", location.Bearing));

        }

        public void OnProviderDisabled(string provider)
        {
            this.ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            this.ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            this.StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
        }

        #endregion

    }
}

