using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZXing;
using ZXing.Mobile;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Helper;

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Cantidades", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation|Android.Content.PM.ConfigChanges.ScreenSize)]
    public class DesgloseVentaConsignacionView : MvxBindingActivityView<DesgloseVentaConsignacionViewModel>
    {
        MvxBindableSpinner cmbCompanias; 
        MvxBindableSpinner cmbCriterios;
        //MvxBindableListView header;
        EditText txtBusqueda;
        EditText txtUnidadAlmacenVendido;
        EditText txtUnidadDetalleVendido;
        EditText txtUnidadAlmacenBueno;
        EditText txtUnidadDetalleBueno;
        EditText txtUnidadAlmacenMalo;
        EditText txtUnidadDetalleMalo;
        ImageButton btnAgregar;
        ImageButton btnRefrescar;
        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DesgloceVentaConsignacion);
            scanner = new MobileBarcodeScanner(this);            
            this.cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriosdsc);
            this.cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniasdsc);   
            this.txtBusqueda = (EditText)this.FindViewById(Resource.Id.txtBusqueda);
            btnRefrescar = FindViewById<ImageButton>(Resource.Id.btnRefrescar);
            this.txtUnidadAlmacenVendido = (EditText)this.FindViewById(Resource.Id.txtUnidadAlmacenVendido);
            this.txtUnidadDetalleVendido = (EditText)this.FindViewById(Resource.Id.txtUnidadDetalleVendido);
            this.txtUnidadAlmacenBueno = (EditText)this.FindViewById(Resource.Id.txtUnidadAlmacenBueno);
            this.txtUnidadDetalleBueno = (EditText)this.FindViewById(Resource.Id.txtUnidadDetalleBueno);
            this.txtUnidadAlmacenMalo = (EditText)this.FindViewById(Resource.Id.txtUnidadAlmacenMalo);
            this.txtUnidadDetalleMalo = (EditText)this.FindViewById(Resource.Id.txtUnidadDetalleMalo);           
            this.btnAgregar = (ImageButton)this.FindViewById(Resource.Id.btnAgregar);

            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            txtUnidadAlmacenVendido.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadAlmacenVendido_TextChanged);
            txtUnidadDetalleVendido.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadDetalleVendido_TextChanged);
            txtUnidadAlmacenBueno.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadAlmacenBueno_TextChanged);
            txtUnidadDetalleBueno.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadDetalleBueno_TextChanged);
            txtUnidadAlmacenMalo.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadAlmacenMalo_TextChanged);
            txtUnidadDetalleMalo.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadDetalleMalo_TextChanged);
            txtBusqueda.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtBusqueda_AfterTextChanged);

            
            btnAgregar.Click += new EventHandler(btnAgregar_Click);            
            //btnAgregar.Click += delegate { this.ViewModel.ComandoRefrescar.Execute(null); };
            this.txtBusqueda.RequestFocus();
            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;           

            btnRefrescar.Click += btnRefrescar_Click;

            //Button flashButton;
            //View zxingOverlay;

            //btnRefrescar.Click += async delegate
            //{
            //    if (ViewModel.CriterioActual != CriterioArticulo.Barras)
            //    {
            //        this.ViewModel.RealizarBusqueda();
            //    }
            //    else
            //    {
            //        if (Android.Hardware.Camera.NumberOfCameras > 0)
            //        {
            //            //Tell our scanner we want to use a custom overlay instead of the default
            //            scanner.UseCustomOverlay = true;

            //            //Inflate our custom overlay from a resource layout
            //            zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.ZxingOverlay, null);

            //            //Find the button from our resource layout and wire up the click event
            //            flashButton = zxingOverlay.FindViewById<Button>(Resource.Id.buttonZxingFlash);
            //            flashButton.Click += (sender, e) => scanner.ToggleTorch();

            //            //Set our custom overlay
            //            scanner.CustomOverlay = zxingOverlay;

            //            //Start scanning!
            //            var result = await scanner.Scan();

            //            HandleScanResult(result);
            //        }
            //        else
            //        {
            //            ViewModel.mostrarAlerta("Este dipositivo no soporta escaneo por cámara");
            //        }
            //    }

            //};
        }

        protected override void OnViewModelSet()
        {
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            txtBusqueda.RequestFocus();
            ViewModel.OnResume();
        }

        async void btnRefrescar_Click(object senderr, EventArgs ee)
        {
            Button flashButton;
            View zxingOverlay;

            if (ViewModel.CriterioActual != CriterioArticulo.Barras)
            {
                this.ViewModel.RealizarBusqueda();
            }
            else
            {
                if (Android.Hardware.Camera.NumberOfCameras > 0)
                {
                    //Tell our scanner we want to use a custom overlay instead of the default
                    scanner.UseCustomOverlay = true;

                    //Inflate our custom overlay from a resource layout
                    zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.ZxingOverlay, null);

                    //Find the button from our resource layout and wire up the click event
                    flashButton = zxingOverlay.FindViewById<Button>(Resource.Id.buttonZxingFlash);
                    flashButton.Click += (sender, e) => scanner.ToggleTorch();

                    //Set our custom overlay
                    scanner.CustomOverlay = zxingOverlay;

                    //Start scanning!
                    var result = await scanner.Scan();

                    HandleScanResult(result);
                }
                else
                {
                    ViewModel.mostrarAlerta("Este dipositivo no soporta escaneo por cámara");
                }
            }
        }

        

        void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
                msg = result.Text;
            //this.RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Short).Show());
            this.RunOnUiThread(() => CapturaBarrasCamara(msg));
        }

        private void CapturaBarrasCamara(string msg)
        {
            DesgloseVentaConsignacionViewModel.ventanaInactiva = false;
            ViewModel.TextoBusqueda = msg;
            ViewModel.TextoBusqueda = string.Empty;
        }

        void txtBusqueda_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            if (ViewModel.EsBarras)
            {
                ViewModel.EsBarras = false;
                ((EditText)sender).Text = string.Empty;

            }
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

        
        private void cmbCriterios_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                CriterioArticulo selected = this.ViewModel.Criterios[args.Position];

                // no está haciendo el binding (el set hacia el modelview), entonces se fuerza
                if (this.ViewModel.CriterioActual != selected)
                {
                    this.ViewModel.CriterioActual = selected;
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs args)
        {           
            //Caso:33124 ABC 01/07/2008 Setear el foco en el filtro de busqueda para inventario por si la búsqueda es con lector de códigos de barra.
            this.txtBusqueda.RequestFocus();
        }

        private void txtUnidadAlmacenVendido_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            ViewModel.CambioUnidadesAlmacenVendidas();
        }

        private void txtUnidadDetalleVendido_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            ViewModel.CambioUnidadesDetalleVendidas();
        }

        private void txtUnidadAlmacenBueno_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            ViewModel.CambioUnidadesAlmacenBuenEstado();
        }

        private void txtUnidadDetalleBueno_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            ViewModel.CambioUnidadesDetalleBuenEstado();
        }

        private void txtUnidadAlmacenMalo_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            ViewModel.CambioUnidadesAlmacenMalEstado(); ;
        }

        private void txtUnidadDetalleMalo_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            ViewModel.CambioUnidadesDetalleMalEstado();
        }

        public override void OnBackPressed()
        {
            ViewModel.CancelarDesglose();
        }

        protected override void OnResume()
        {            
            base.OnResume();
        }

        protected override void OnStop()
        {
            txtBusqueda.RequestFocus();
            DesgloseVentaConsignacionViewModel.ventanaInactiva = true;
            base.OnStop();
        }
        
    }
}