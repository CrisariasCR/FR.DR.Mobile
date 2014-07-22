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
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Helper;
using Android.Text;
using Softland.ERP.FR.Mobile.Cls;

namespace FR.Droid.Views.TomaFisica
{
    [Activity(Label = "FR - Datos de Toma Física", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class TomaFisicaInventarioView : MvxBindingActivityView<TomaFisicaInventarioViewModel>
    {
        MvxBindableSpinner cmbLotes;
        EditText txtDispAlmacen;
        EditText txtDispDetalle;
        EditText txtBusqueda;
        ImageButton btnRefrescar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TomaFisicaInventario);
            cmbLotes = (MvxBindableSpinner)this.FindViewById(Resource.Id.cmbLotestf);
            txtDispAlmacen = (EditText)this.FindViewById(Resource.Id.txtDispAlmacen);
            txtDispDetalle = (EditText)this.FindViewById(Resource.Id.txtDispDetalle);
            txtBusqueda = (EditText)this.FindViewById(Resource.Id.txtBusqueda);
            this.btnRefrescar = (ImageButton)this.FindViewById(Resource.Id.btnRefrescar);

            cmbLotes.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbLotes_ItemSelected);
            txtDispAlmacen.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtDispAlmacen_TextChanged);
            txtDispDetalle.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtDispDetalle_TextChanged);
            txtBusqueda.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>(txtBusqueda_TextChanged);
            btnRefrescar.Click += new EventHandler(btnRefrescar_Click);            
            cmbLotes.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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

        private void cmbLotes_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {            
            if (ViewModel.LoteSelectedItem())
            {
                txtDispAlmacen.RequestFocus();
            }
        }

        private void btnRefrescar_Click(object sender, EventArgs args)
        {
            if (ViewModel.ObtenerDatosArticulo(true))
            {
                txtDispAlmacen.RequestFocus();
            }
            else 
            {
                txtBusqueda.RequestFocus();
            }
        }

        private void txtDispAlmacen_TextChanged(object sender, Android.Text.AfterTextChangedEventArgs args)
        {
            if (!ViewModel.txtDisponibleAlmacen_Validating(((EditText)sender).Text))
            {
                txtDispAlmacen.RequestFocus();                
            }
        }

        private void txtDispDetalle_TextChanged(object sender, Android.Text.AfterTextChangedEventArgs args)
        {
            if (!ViewModel.txtDisponibleDetalle_Validating(((EditText)sender).Text))
            {
                txtDispDetalle.RequestFocus();
            }
        }

        private void txtBusqueda_TextChanged(object sender, Android.Text.AfterTextChangedEventArgs args)
        {
            if (ViewModel.txtBusquedaTextChanged(((EditText)sender).Text)) 
            {
                txtDispAlmacen.RequestFocus();
            }
        }
    
    }
}