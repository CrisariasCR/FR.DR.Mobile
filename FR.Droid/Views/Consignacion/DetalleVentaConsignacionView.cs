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
using Cirrious.MvvmCross.Droid.Interfaces;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Binding.Droid.Interfaces.Views;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Helper;



namespace FR.Droid.Views.Consignacion
{
    [Activity(Label = "FR - Detalle Consignación", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class DetalleVentaConsignacionView : MvxBindingActivityView<DetalleVentaConsignacionViewModel>
    {
        
        MvxBindableSpinner cmbCriterios;
        MvxBindableSpinner cmbCompanias;
        MvxBindableListView header;
        MvxBindableListView lista;
        EditText txtBusqueda;
        ImageButton btnRefrescar;
        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DetalleVentaConsignacion);
            scanner = new MobileBarcodeScanner(this);

            this.cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriosdvc);
            btnRefrescar = FindViewById<ImageButton>(Resource.Id.btnRefrescar);
            this.cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniasdvc);
            this.header = (MvxBindableListView)this.FindViewById(Resource.Id.header);
            this.lista = (MvxBindableListView)this.FindViewById(Resource.Id.items);
            this.txtBusqueda = (EditText)this.FindViewById(Resource.Id.txtBusqueda);

            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            txtBusqueda.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtBusqueda_AfterTextChanged);            
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

        protected override void OnViewModelSet()
        {            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
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

        private void cmbCriterios_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                CriterioArticulo selected = this.ViewModel.Criterios[args.Position];

                // no está haciendo el binding (el set hacia el modelview), entonces se fuerza
                if (this.ViewModel.CriterioActual != this.ViewModel.Criterios[args.Position])
                {
                    this.ViewModel.CriterioActual = this.ViewModel.Criterios[args.Position];
                }

                ChangeTemplates(selected);
                this.ViewModel.EjecutarBusquedaDetalles();
            }
        }

        private void ChangeTemplates(CriterioArticulo selected)
        {
            lista.ItemTemplateId = ResourceHelper.GetID(typeof(Resource.Layout), selected.ItemTemplate(NombreView));
            header.ItemTemplateId = ResourceHelper.GetID(typeof(Resource.Layout), selected.HeaderTemplate(NombreView));
        }

        /// <summary>
        /// Nombre del View en los recursos (el que se llama BusquedaArticulos.axml en Resources/Layout!)
        /// </summary>
        private static string NombreView
        {
            get { return "DetalleVentaConsignacion"; }
        }

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }

    }
}