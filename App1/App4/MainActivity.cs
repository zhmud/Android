using System;
using System.Threading.Tasks;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Locations;
using App4.Services;
using Android.Content;

namespace App4
{
    [Activity(Label = "Background Location", MainLauncher = true)]
    public class MainActivity : Activity
    {
        readonly string logTag = "MainActivity";

        // make our labels
        TextView latText;
        TextView longText;
        TextView altText;
        TextView speedText;
        TextView bearText;
        TextView accText;
        EditText editLogin;
        EditText editpassword;
        Button btnLogin;
        Button btnSos;
        string login;
        string password;
        bool send = false;
        double lat;
        double lon;
        double acc;

        readonly Client client = new Client("46.37.197.32", 8080);

        #region Lifecycle

        //Lifecycle stages
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
                Log.Debug(logTag, "ServiceConnected Event Raised");
                App.Current.LocationService.LocationChanged += HandleLocationChanged;
                App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                App.Current.LocationService.StatusChanged += HandleStatusChanged;
            };

            latText = FindViewById<TextView>(Resource.Id.lat);
            longText = FindViewById<TextView>(Resource.Id.longx);
            altText = FindViewById<TextView>(Resource.Id.alt);
            speedText = FindViewById<TextView>(Resource.Id.speed);
            bearText = FindViewById<TextView>(Resource.Id.bear);
            accText = FindViewById<TextView>(Resource.Id.acc);
            editLogin = FindViewById<EditText>(Resource.Id.login);
            editpassword = FindViewById<EditText>(Resource.Id.password);
            btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnSos = FindViewById<Button>(Resource.Id.btnSos);

            btnLogin.Click += delegate {
                login = editLogin.Text;
                password = editpassword.Text;
                editLogin.Enabled = false;
                editpassword.Enabled = false;
                btnLogin.Enabled = false;
                client.Authorization(login, password);
                send = true;
            };

            btnSos.Click += delegate {
                client.Send(Android.OS.Build.Serial, lon, lat, (int)acc, GetBattery(), DateTime.Now, true);
            };
        }

        protected override void OnPause()
        {
            Log.Debug(logTag, "Location app is moving to background");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Log.Debug(logTag, "Location app is moving into foreground");
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            Log.Debug(logTag, "Location app is becoming inactive");
            base.OnDestroy();
        }

        #endregion

        #region Android Location Service methods

        ///<summary>
        /// Updates UI with location data
        /// </summary>
        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Android.Locations.Location location = e.Location;
            Log.Debug(logTag, "Foreground updating");

            RunOnUiThread(() => {
                latText.Text = String.Format("Latitude: {0}", location.Latitude);
                longText.Text = String.Format("Longitude: {0}", location.Longitude);
                altText.Text = String.Format("Altitude: {0}", location.Altitude);
                speedText.Text = String.Format("Speed: {0}", location.Speed);
                accText.Text = String.Format("Accuracy: {0}", location.Accuracy);
                bearText.Text = String.Format("Bearing: {0}", location.Bearing);
                lat = location.Latitude;
                lon = location.Longitude;
                acc = location.Accuracy;
                if (send)
                {
                    client.Send(Android.OS.Build.Serial, location.Longitude, location.Latitude, (int)location.Accuracy, GetBattery(), DateTime.Now, false);
                }
            });

        }

        public int GetBattery()
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);
            int BPercetage = (int)System.Math.Floor(level * 100D / scale);
            return BPercetage;
        }


        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider disabled event raised");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider enabled event raised");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(logTag, "Location status changed, event raised");
        }

        #endregion

    }
}


