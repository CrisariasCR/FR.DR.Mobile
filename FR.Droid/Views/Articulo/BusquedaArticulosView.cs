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
    [Activity(Label = "FR - Búsqueda Artículos", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class BusquedaArticulosView : MvxBindingActivityView<BusquedaArticulosViewModel>
    {
        MvxBindableSpinner cmbCompanias; 
        MvxBindableSpinner cmbCriterios;
        MvxBindableListView header;
        MvxBindableListView lista;
        EditText txtBusqueda;
        MobileBarcodeScanner scanner;        
        ImageButton btnRefrescar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.BusquedaArticulos);
            scanner = new MobileBarcodeScanner(this);
            this.cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompanias);
            this.cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriterios);
            this.header = (MvxBindableListView)this.FindViewById(Resource.Id.HeaderLista);
            this.lista = (MvxBindableListView)this.FindViewById(Resource.Id.listaArticulos);
            this.btnRefrescar = (ImageButton)this.FindViewById(Resource.Id.btnRefrescar);
            this.txtBusqueda = (EditText)this.FindViewById(Resource.Id.txtBusquedaba);

            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            
            txtBusqueda.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtBusqueda_AfterTextChanged);
            
            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            
            btnRefrescar.Click += btnRefrescar_Click;

            //Button flashButton;
            //View zxingOverlay;
            //btnRefrescar.Click += async delegate
            //{
            //    if (ViewModel.CriterioActual != CriterioArticulo.Barras)
            //    {
            //        this.ViewModel.Refrescar();
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

        async void btnRefrescar_Click(object senderq, EventArgs eq)
        {
            Button flashButton;
            View zxingOverlay;
             if (ViewModel.CriterioActual != CriterioArticulo.Barras)
             {
                 this.ViewModel.Refrescar();
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
                
                //CriterioArticulo selected = EnumHelper.GetValue<CriterioArticulo>((args.View as TextView).Text);
                CriterioArticulo selected = this.ViewModel.Criterios[args.Position];
                // no está haciendo el binding (el set hacia el modelview), entonces se fuerza
                if (this.ViewModel.CriterioActual != selected)
                {
                    this.ViewModel.CriterioActual = selected;
                }

                ChangeTemplates(selected);
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
            get { return "BusquedaArticulos"; }
        }

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }
    }
}