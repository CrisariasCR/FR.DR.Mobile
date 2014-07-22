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
using System.Net;
using System.IO;
using Android.Graphics;

namespace FR.Droid.Views.Cliente
{
    [Activity(Label = "FR - Registro de Visita", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class VisitaView : MvxBindingActivityView<VisitaViewModel>, ILocationListener
    {
        MvxBindableSpinner cmbTipoVisita;
        MvxBindableSpinner cmbRazonVisita;
        ImageButton btnAceptar;
        TextView txtUbicacion;
        LocationManager _locMgr;
        public bool locationOff = false;
        private SincroDesatendidaService _loginService;


        protected override void OnViewModelSet()
        {
            
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Visita);
            this.cmbTipoVisita = FindViewById<MvxBindableSpinner>(Resource.Id.cmbTipoVisita);
            this.cmbRazonVisita = FindViewById<MvxBindableSpinner>(Resource.Id.cmbRazonVisita);
            this.txtUbicacion = FindViewById<TextView>(Resource.Id.txtUbicacion);

            this.btnAceptar = FindViewById<ImageButton>(Resource.Id.btnAceptar);

            cmbTipoVisita.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbRazonVisita.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;

           btnAceptar.Click += btnAceptar_Click;

        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Abar, menu);          
            return true;
        }
        

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            var menucli = FindViewById(Resource.Id.menucli);
            if (menucli.Id == item.ItemId)
            {
                Toast tx = Toast.MakeText(this, ViewModel.NameCliente, ToastLength.Short);
                tx.SetGravity(GravityFlags.Center, 0, 0);
                tx.Show();
            }
            return base.OnMenuItemSelected(featureId, item);
        }
     

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        protected override void OnStart()
        {
            _locMgr = GetSystemService(Context.LocationService) as LocationManager;
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);            
            this.OnStartSer();
            
        }

        protected override void OnResume()
        {
            base.OnResume();                        
        }

        private void OnStartSer() 
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
                    }
                    else
                    {
                        locationOff = true;
                        ViewModel.gpsOff = true;
                        Toast.MakeText(this, "Servicio de ubicación no disponible", ToastLength.Short).Show();
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
                }
            }


            if (!locationOff)
            {
                if (FRmConfig.GuardarUbicacionVisita)
                {
                    try
                    {
                        _locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Error Obteniendo ubicación." + ex.Message, ToastLength.Short).Show();
                    }
                }
            }
        }

        protected override void OnPause()
        {
            if (FRmConfig.GuardarUbicacionVisita && !locationOff)
            {

                try
                {
                    _locMgr.RemoveUpdates(this);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error deteniendo servicio ubicación." + ex.Message, ToastLength.Short).Show();
                }
            }
            base.OnPause();
        }

        public async void OnLocationChanged(Location location)
        {
            if (FRmConfig.GuardarUbicacionVisita && !locationOff)
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
                        txtUbicacion.Text = "Decifrando la ubicación mediante latidud/longitud...";

                        IList<Address> addresses = await getAddressTask;

                        if (addresses.Count > 0)
                        {
                            Address addr = addresses[0];
                            //addressTextView.Text = FormatAddress(addr); 
                            txtUbicacion.Text = addr.CountryName + "," + addr.AdminArea + "," + addr.SubAdminArea + "," + addr.SubLocality;
                        }
                        else
                        {
                            Toast.MakeText(this, "No se pudo decifrar la ubicación", ToastLength.Short).Show();
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

        async void btnAceptar_Click(object sender, EventArgs e)
        {
            if (Softland.ERP.FR.Mobile.App.Prefijo == "(SYNC) ")
            {
                ViewModel.RegistrarAdvertencia();
            }
            else
            {
                if (FRdConfig.PermiteSincroDesatendida)
                {
                    Softland.ERP.FR.Mobile.App.Prefijo = "(SYNC) ";
                    _loginService = new SincroDesatendidaService();
                    ViewModel.Registrar();
                    if (await _loginService.LoginAsync(this))
                    {
                        //onSuccessfulLogin();
                        _loginService.onSuccess();
                    }
                    else
                    {
                        _loginService.onSuccess();
                    }
                }
                else
                {
                    ViewModel.Registrar();
                }
            }
             //when the Task<int> returns, the value is available and we can display on the UI
            

             //effectively returns void          
        }

        ////void btnAceptar_Click(object sender, EventArgs e)
        ////{
        ////    ViewModel.Registrar();       
        ////}
    }

    
    public class SincroDesatendidaService
    {

        private SincroDesatendida sincro;

        //public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        //{
        //    Log.Debug("DemoService", "DemoService started");

        //    StartServiceInForeground();

        //    DoWork();

        //    return StartCommandResult.NotSticky;
        //}

        public async Task<bool> LoginAsync(Android.Content.Context ctx,CancellationToken cancellationToken = default(CancellationToken))
        {
            bool x = await Task.Factory.StartNew(() =>
            {                
                Thread.Sleep(5000);
                sincro = new SincroDesatendida(ctx);
                sincro.DescargaDatos();
                return true;
            }, cancellationToken);
            return x;
           // return result;
            //return Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(10000);
            //    return true;
            //}, cancellationToken);
        }

        public void onSuccess()
        {
            //Softland.ERP.FR.Mobile.App.getCurrentActivity().FindViewById<TextView>(Resource.Id.txtPrueba).Text = "Finalizado desde la clase";                        
            //Softland.ERP.FR.Mobile.App.getCurrentActivity().Title="...."+Softland.ERP.FR.Mobile.App.getCurrentActivity().Title;            
            Softland.ERP.FR.Mobile.App.Prefijo = string.Empty;
        }
    }


}