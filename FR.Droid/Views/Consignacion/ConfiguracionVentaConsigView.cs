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

namespace FR.Droid.Views.Consignacion
{
    [Activity(Label = "FR - Configuración de Consignación", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConfiguracionVentaConsigView : MvxBindingActivityView<ConfiguracionVentaConsigViewModel>
    {

        MvxBindableSpinner paises;
        MvxBindableSpinner condiciones;
        MvxBindableSpinner niveles;
        MvxBindableSpinner companias;
        Android.Widget.RadioGroup radioGroup;
        RadioButton rdNormal;
        RadioButton rdCredito;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConfiguracionVentaConsig);
            this.paises = FindViewById<MvxBindableSpinner>(Resource.Id.cmbPaisescfc);
            this.condiciones = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCondicionescfc);
            this.niveles = FindViewById<MvxBindableSpinner>(Resource.Id.cmbNivelescfc);
            this.companias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniascfc);
            this.radioGroup = FindViewById<Android.Widget.RadioGroup>(Resource.Id.rgClase);

            rdNormal = (RadioButton)this.FindViewById(Resource.Id.rdNormal);
            rdCredito = (RadioButton)this.FindViewById(Resource.Id.rdCredito);

            rdNormal.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdNormal_CheckedChange);
            rdCredito.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdCredito_CheckedChange);            
            paises.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            condiciones.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            niveles.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            companias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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

        private void rdNormal_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.RbNormal = true;
            }
            else
            {
                ViewModel.RbNormal = false;
            }
        }

        private void rdCredito_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.RbCredito = true;
            }
            else
            {
                ViewModel.RbCredito = false;
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.Cancelar();
        }
    
    }
}