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
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

namespace FR.Droid.Views.Devoluciones
{
    [Activity(Label = "FR - Detalle Devoluciones", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class DetalleDevolucionesView : MvxBindingActivityView<DetalleDevolucionesViewModel>
    {
        MvxBindableSpinner cmbCriterios;
        MvxBindableSpinner cmbCompanias;
        MvxBindableListView header, lista;
        EditText txtBusqueda;
        ImageButton btnRefrescar;
        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DetalleDevoluciones);
            scanner = new MobileBarcodeScanner(this);
            btnRefrescar = FindViewById<ImageButton>(Resource.Id.btnRefrescar);
            this.cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriosdd);
            this.cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniasdd);
            this.header = (MvxBindableListView)this.FindViewById(Resource.Id.HeaderLista);
            this.txtBusqueda = (EditText)this.FindViewById(Resource.Id.txtBusquedadd);
            this.lista = (MvxBindableListView)this.FindViewById(Resource.Id.ListaDevoluciones);

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
            //        this.ViewModel.EjecutarBusquedaDetalles();
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
                this.ViewModel.EjecutarBusquedaDetalles();
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

                ChangeTemplates(selected);
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.ComandoRegresar.Execute(null);
        }

        private void ChangeTemplates(CriterioArticulo selected)
        {
            lista.ItemTemplateId = ResourceHelper.GetID(typeof(Resource.Layout), selected.ItemTemplate(NombreView));
            header.ItemTemplateId = ResourceHelper.GetID(typeof(Resource.Layout), selected.HeaderTemplate(NombreView));
        }

        private static string NombreView
        {
            get { return "DetalleDevoluciones"; }
        }
    }
}