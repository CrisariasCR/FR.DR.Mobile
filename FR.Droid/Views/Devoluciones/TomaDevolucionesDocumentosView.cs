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

namespace FR.Droid.Views.Devoluciones
{
    [Activity(Label = " Ingreso de Cantidades", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class TomaDevolucionesDocumentosView : MvxBindingActivityView<TomaDevolucionesDocumentosViewModel>
    {
        RadioButton rdrCredito, rdrEfectivo;
        MvxBindableSpinner cmbCompanias, cmbCriterios, cmbEstadoArticulo;
        EditText txtBusqueda;
        ImageButton btnRefrescar;
        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TomaDevolucionesDocumentos);
            scanner = new MobileBarcodeScanner(this);
            rdrCredito = FindViewById<RadioButton>(Resource.Id.radioCredito);
            rdrEfectivo = FindViewById<RadioButton>(Resource.Id.radioEfectivo);
            btnRefrescar = FindViewById<ImageButton>(Resource.Id.btnRefrescar);
            txtBusqueda = FindViewById<EditText>(Resource.Id.txtBusquedatdd);

            rdrCredito.Click += new EventHandler(onRadioButtonClicked);
            rdrEfectivo.Click += new EventHandler(onRadioButtonClicked);

            this.cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniastdd);
            this.cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriostdd);
            this.cmbEstadoArticulo = FindViewById<MvxBindableSpinner>(Resource.Id.cmbEstadoArticulotdd);
            txtBusqueda.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtBusqueda_AfterTextChanged);

            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            cmbCompanias.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCompanias_ItemSelected);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbEstadoArticulo.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;

            txtBusqueda.RequestFocus();

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

        void cmbCompanias_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //var companiaSeleccionada = (e.View as TextView).Text;
            var companiaSeleccionada = this.ViewModel.Companias[e.Position];
            if (ViewModel.CompaniaSeleccionada != companiaSeleccionada)
            {
                ViewModel.CompaniaSeleccionada = companiaSeleccionada;
            }
        }        

        public override void OnBackPressed()
        {
            ViewModel.ComandoCancelar.Execute(null);
        }

        void onRadioButtonClicked(object sender, EventArgs e)
        {
            View view = (View)sender;
            if (view.Id == Resource.Id.radioEfectivo)
            {
                ViewModel.TipoPagoDevolucion = "E";
            }
            else
            {
                ViewModel.TipoPagoDevolucion = "C";
            }
        }

        private void cmbCriterios_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //CriterioArticulo selected = this.ViewModel.Criterios[args.Position];
                CriterioArticulo selected = this.ViewModel.Criterios[args.Position];
                // no está haciendo el binding (el set hacia el modelview), entonces se fuerza
                if (this.ViewModel.CriterioActual != selected)
                {
                    this.ViewModel.CriterioActual = selected;
                }
            }
        }
    }
}