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

using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Java.Util;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Android.Text;

namespace FR.Droid.Views.Pedido
{
    [Activity(Label = "FR - Aplicar Pedido", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class AplicarPedidoView : MvxBindingActivityView<AplicarPedidoViewModel>
    {

        ImageButton btnCambiarFecha;
        private DateTime date;
        int DATE_DIALOG_ID = 0;
        EditText porcDescuento1, porcDescuento2;
        MvxBindableSpinner cmbDirecciones, cmbCompanias;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AplicarPedido);
            btnCambiarFecha = this.FindViewById<ImageButton>(Resource.Id.btnCambiarFecha);
            btnCambiarFecha.Click += new EventHandler(btnCambiarFecha_Click);
            cmbDirecciones = FindViewById<MvxBindableSpinner>(Resource.Id.cmbDireccionesap);
            cmbDirecciones.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbDirecciones_ItemSelected);

            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniasap);
            cmbCompanias.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCompanias_ItemSelected);          
            

            porcDescuento1 = FindViewById<EditText>(Resource.Id.txtDescuento1);
            porcDescuento1.TextChanged += new EventHandler<TextChangedEventArgs>(porcDescuento1_TextChanged);

            porcDescuento2 = FindViewById<EditText>(Resource.Id.txtDescuento2);
            porcDescuento2.TextChanged += new EventHandler<TextChangedEventArgs>(porcDescuento2_TextChanged);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbDirecciones.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        }

        protected override void OnViewModelSet()
        {            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            if (Pedidos.FacturarPedido)
            {
                Title = "FR - Aplicar Factura";
            }
            else
            {
                Title = "FR - Aplicar Pedido";
            }

            date = DateTime.Today;
            UpdateDisplay();
            ViewModel.OnResume();
        }

        void porcDescuento1_TextChanged(object sender,TextChangedEventArgs e)
        {
            if(!AplicarPedidoViewModel.ventanaInactiva)
            {
                if (!string.IsNullOrEmpty(porcDescuento1.Text))
                {
                    ViewModel.PorcDescuento1 = decimal.Parse(porcDescuento1.Text);
                }
                else
                {
                    ViewModel.PorcDescuento1 = decimal.Zero;
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

        void porcDescuento2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(porcDescuento2.Text))
            {
                ViewModel.PorcDescuento2 = decimal.Parse(porcDescuento2.Text);
            }
            else
            {
                ViewModel.PorcDescuento2 = decimal.Zero;
            }
        }

        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.date = e.Date;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            ViewModel.FechaEntrega = date;
        }

        protected override Dialog OnCreateDialog(int id)
        {
            return new DatePickerDialog(this, OnDateSet, date.Year, date.Month - 1, date.Day);
        }

        void btnCambiarFecha_Click(object sender, EventArgs e)
        {
            ShowDialog(DATE_DIALOG_ID);
        }        

        private void cmbDirecciones_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //ViewModel.SeleccionarDireccion((args.View as TextView).Text);
                ViewModel.SeleccionarDireccion(this.ViewModel.Direcciones[args.Position].ToString());
            }
        }

        private void cmbCompanias_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            var cia = this.ViewModel.Companias[args.Position];

            if (ViewModel.CompaniaActual != cia)
            {
                ViewModel.CompaniaActual = cia;
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
            porcDescuento1.RequestFocus();
            AplicarPedidoViewModel.ventanaInactiva = true;
            base.OnStop();
        }

    }
}