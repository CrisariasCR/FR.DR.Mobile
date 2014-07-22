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
using Softland.ERP.FR.Mobile.UI;
using FR.DR.Core.Helper;

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Aplicar Pago", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class AplicarPagoView : MvxBindingActivityView<AplicarPagoViewModel>
    {
        RadioButton rdLocal;
        RadioButton rdDolar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AplicarPago);
            rdLocal = (RadioButton)this.FindViewById(Resource.Id.rbLocal);
            rdDolar = (RadioButton)this.FindViewById(Resource.Id.rbDolar);

            rdLocal.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdLocal_CheckedChange);
            rdDolar.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdDolar_CheckedChange);
            
        }

        protected override void OnViewModelSet()
        {            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            if (ViewModel.bcargarInfoCobro)
            {
                ViewModel.CargaInfoCobro();
                ViewModel.bcargarInfoCobro = false;
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

        public override void OnBackPressed()
        {
            ViewModel.Cancelar();
        }

        private void rdLocal_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.RbLocal = true;
            }
            else
            {
                ViewModel.RbLocal = false;
            }
            ViewModel.CargaInfoCobro();
        }

        private void rdDolar_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.RbDolar = true;
            }
            else
            {
                ViewModel.RbDolar = false;
            }
            ViewModel.CargaInfoCobro();
        }

    }
}