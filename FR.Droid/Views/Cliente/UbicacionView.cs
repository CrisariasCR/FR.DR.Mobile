using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading.Tasks;
using System.Threading;
using Softland.ERP.FR.Mobile.Cls;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;

namespace FR.Droid.Views.Cliente
{
    [Activity(Label = "FR - Registro de Ubicacion", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class UbicacionView : MvxBindingActivityView<UbicacionClienteViewModel>, ILocationListener
    {
        TextView txtUbicacion;
        LocationManager _locMgr;
        public bool locationOff = false;

        protected override void OnViewModelSet()
        {
            
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UbicacionCliente);
            this.txtUbicacion = FindViewById<TextView>(Resource.Id.txtUbicacion);            
        }

        protected override void OnStart()
        {
            _locMgr = GetSystemService(Context.LocationService) as LocationManager;
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            this.OnStartServ();
        }

        public override void OnBackPressed()
        {
            ViewModel.regresar();
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        protected override void OnResume()
        {            
            base.OnResume();
        }

        private void OnStartServ() 
        {
            //Determina si el GPS esta encendido
            string locationProvider = LocationManager.NetworkProvider;
            if (isOnline())
            {
                if (!_locMgr.IsProviderEnabled(locationProvider))
                {
                    locationProvider = LocationManager.GpsProvider;
                    if (!_locMgr.IsProviderEnabled(locationProvider))
                    {
                        locationOff = true;
                        ViewModel.gpsOff = true;
                        Toast.MakeText(this, "Servicio de GPS no disponible", ToastLength.Short).Show();
                        txtUbicacion.Text = "Servicio de GPS no disponible";
                    }
                    else
                    {
                        locationOff = true;
                        ViewModel.gpsOff = true;
                        Toast.MakeText(this, "Servicio de ubicación no disponible", ToastLength.Short).Show();
                        txtUbicacion.Text = "Servicio de ubicación no disponible";
                    }
                }

            }
            else
            {
                locationProvider = LocationManager.GpsProvider;
                if (!_locMgr.IsProviderEnabled(locationProvider))
                {
                    locationOff = true;
                    ViewModel.gpsOff = true;
                    Toast.MakeText(this, "Servicio de GPS no disponible", ToastLength.Short).Show();
                    txtUbicacion.Text = "Servicio de GPS no disponible";
                }
            }


            if (!locationOff)
            {
                try
                {
                    _locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error Obteniendo ubicación." + ex.Message, ToastLength.Short).Show();
                    txtUbicacion.Text = "Error Obteniendo ubicación.";
                }

            }
        }

        protected override void OnPause()
        {
            if (!locationOff)
            {

                try
                {
                    _locMgr.RemoveUpdates(this);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error deteniendo servicio ubicación." + ex.Message, ToastLength.Short).Show();
                    txtUbicacion.Text = "Error deteniendo servicio ubicación.";
                }
            }
            base.OnPause();
        }

        public async void OnLocationChanged(Location location)
        {
            if (!locationOff)
            {
                try
                {
                    ViewModel.latidud = location.Latitude;
                    ViewModel.longitud = location.Longitude;
                    ViewModel.altitud = location.Altitude;
                    long unixDate = location.Time;
                    DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    ViewModel.gpsTime = start.AddMilliseconds(unixDate).ToLocalTime();
                    ViewModel.gpsOff = false;
                    try
                    {
                        Geocoder geocdr = new Geocoder(this);

                        Task<IList<Address>> getAddressTask = geocdr.GetFromLocationAsync(location.Latitude, location.Longitude, 5);

                        IList<Address> addresses = await getAddressTask;

                        if (addresses.Count > 0)
                        {
                            Address addr = addresses[0];
                            //addressTextView.Text = FormatAddress(addr); 
                            txtUbicacion.Text = addr.CountryName + "," + addr.AdminArea + "," + addr.SubAdminArea + "," + addr.SubLocality;
                            ViewModel.GuardarUbicacionCliente();
                        }
                        else
                        {
                            Toast.MakeText(this, "No se pudo decifrar la ubicación", ToastLength.Short).Show();
                            txtUbicacion.Text = "No se pudo decifrar la ubicación";
                        }
                    }
                    catch (Exception ex)
                    {
                        //Toast.MakeText(this, "Error Decifrando ubicación." + ex.Message, ToastLength.Short).Show();
                    }

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error asignando ubicación." + ex.Message, ToastLength.Short).Show();
                    txtUbicacion.Text = "Error asignando ubicación";
                    ViewModel.gpsOff = true;
                }
            }
        }

        public bool isOnline()
        {
            ConnectivityManager cm =
                 (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo netInfo = cm.ActiveNetworkInfo;
            if (netInfo != null && netInfo.IsConnected)
            {
                return true;
            }
            return false;
        }
    }
}