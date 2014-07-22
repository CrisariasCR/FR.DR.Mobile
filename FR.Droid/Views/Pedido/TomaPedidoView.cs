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
using Android.Graphics;
using ZXing;
using ZXing.Mobile;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Android.Text;



namespace FR.Droid.Views.Pedido
{
    [Activity(Label = "FR - Toma Pedido", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class TomaPedidoView : MvxBindingActivityView<TomaPedidoViewModel>
    {
        MvxBindableSpinner cmbCriterios, cmbFamilias, cmbCompanias, cmbFamiliasB, cmbSubFamilias;
        EditText txtCantDet, txtCantAlm, txtBusquedatp;
        ImageButton btnRefrescar;		
        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TomaPedido);

            scanner = new MobileBarcodeScanner(this);

            cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriostp);
            btnRefrescar = FindViewById<ImageButton>(Resource.Id.btnRefrescar);
            cmbFamilias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbFamiliastp);
            cmbFamiliasB = FindViewById<MvxBindableSpinner>(Resource.Id.cmbFamiliastpB);
            cmbSubFamilias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbSubFamiliastp);
            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniastp);
            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            cmbFamilias.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbFamilias_ItemSelected);
            cmbFamiliasB.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbFamilias_ItemSelectedB); 
            txtCantAlm = FindViewById<EditText>(Resource.Id.txtCantAlm);
            txtCantDet = FindViewById<EditText>(Resource.Id.txtCantDet);
            txtBusquedatp = FindViewById<EditText>(Resource.Id.txtBusquedatp);
            txtBusquedatp.AfterTextChanged += new EventHandler<AfterTextChangedEventArgs>(txtBusquedatp_AfterTextChanged);                       

            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbFamilias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;

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

        protected override void OnViewModelSet()
        {
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            ViewModel.OnResume();
            txtBusquedatp.RequestFocus();
        }

        async void btnRefrescar_Click(object senderr, EventArgs ee)
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
            this.RunOnUiThread(() => CapturaBarrasCamara( msg));
        }

        private void CapturaBarrasCamara(string msg)
        {
            TomaPedidoViewModel.ventanaInactiva = false;
            ViewModel.TextoBusqueda = msg;
            ViewModel.TextoBusqueda = string.Empty;
        }

        void txtBusquedatp_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
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
                ViewModel.CambioComboCriterios(selected);
            }
        }

        private void cmbFamilias_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                string selected = this.ViewModel.Familias[args.Position];
                ViewModel.FamiliaActual = selected;
            }
        }

        private void cmbFamilias_ItemSelectedB(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                string selected = this.ViewModel.Familias[args.Position];
                ViewModel.FamiliaActual = selected;
            }
        }

        private void cmbSubFamilias_ItemSelectedB(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                CriterioArticulo selected = this.ViewModel.SubFamilias[args.Position];
                ViewModel.SubFamiliaActual = selected;
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
            //base.OnBackPressed();
        }

        protected override void OnResume()
        {        
            base.OnResume();
        }

        protected override void OnStop()
        {
            txtBusquedatp.RequestFocus();
            TomaPedidoViewModel.ventanaInactiva = true;
            base.OnStop();
        }        
        
    }
}