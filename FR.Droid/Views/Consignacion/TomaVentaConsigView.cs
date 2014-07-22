using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Mobile;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Helper;

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Cantidades", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class TomaVentaConsigView : MvxBindingActivityView<TomaVentaConsigViewModel>
    {
        MvxBindableSpinner cmbCompanias; 
        MvxBindableSpinner cmbCriterios;
        //MvxBindableListView header;
        EditText txtBusqueda;        
        EditText txtUnidadDetalle;
        EditText txtUnidadAlmacen;
        ImageButton btnDetalle;
        ImageButton btnAgregar;
        ImageButton btnRefrescar;
        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TomaVentaConsig);
            scanner = new MobileBarcodeScanner(this);
            this.txtBusqueda = FindViewById<EditText>(Resource.Id.txtBusqueda);
            this.cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriostc);
            this.cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniastc);  
            this.txtUnidadDetalle = (EditText)this.FindViewById(Resource.Id.txtUnidadDetalle);
            this.txtUnidadAlmacen = (EditText)this.FindViewById(Resource.Id.txtUnidadAlmacen);
            btnRefrescar = FindViewById<ImageButton>(Resource.Id.btnRefrescar);
            this.btnDetalle = (ImageButton)this.FindViewById(Resource.Id.btnDetalle);
            this.btnAgregar = (ImageButton)this.FindViewById(Resource.Id.btnAgregar);

            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            txtBusqueda.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtBusqueda_AfterTextChanged);
            txtUnidadDetalle.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadDetalle_TextChanged);
            txtUnidadAlmacen.TextChanged += new EventHandler<Android.Text.TextChangedEventArgs>(txtUnidadAlmacen_TextChanged);

            btnDetalle.Click += new EventHandler(btnDetalle_Click);
            btnAgregar.Click += new EventHandler(btnAgregar_Click);            
            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            this.txtBusqueda.RequestFocus();

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

        private void CapturaBarrasCamara(string msg)
        {
            TomaVentaConsigViewModel.ventanaInactiva = false;
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

        private void btnDetalle_Click(object sender, EventArgs args)
        {
            this.txtBusqueda.RequestFocus();
        }

        private void txtUnidadDetalle_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            this.ViewModel.CambioUnidadesDetalle();
           // if (!this.ViewModel.Cargando) this.ViewModel.CalculaUnidades();
        }

        private void txtUnidadAlmacen_TextChanged(object sender, Android.Text.TextChangedEventArgs args)
        {
            this.ViewModel.CambioUnidadesAlmacen();
            // if (!this.ViewModel.Cargando) this.ViewModel.CalculaUnidades();
        }

        public override void OnBackPressed()
        {
            ViewModel.DoNothing();
        }

        protected override void OnResume()
        {          
            base.OnResume();
        }

        protected override void OnStop()
        {
            txtBusqueda.RequestFocus();
            TomaVentaConsigViewModel.ventanaInactiva = true;
            base.OnStop();
        }
        
    }
}