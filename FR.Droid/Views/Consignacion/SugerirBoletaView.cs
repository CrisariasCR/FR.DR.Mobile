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
    [Activity(Label = "FR - Sugerencia Venta Consignación", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SugerirBoletaView : MvxBindingActivityView<SugerirBoletaViewModel>
    {
        RadioButton rdAnterior;
        RadioButton rdSoloSaldo;
        RadioButton rdNinguno;   
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SugerirBoleta);
            rdAnterior = (RadioButton)this.FindViewById(Resource.Id.rdAnterior);
            rdSoloSaldo = (RadioButton)this.FindViewById(Resource.Id.rdSoloSaldo);
            rdNinguno = (RadioButton)this.FindViewById(Resource.Id.rdNinguno);

            rdAnterior.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdAnterior_CheckedChange);
            rdSoloSaldo.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdSoloSaldo_CheckedChange);
            rdNinguno.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdNinguno_CheckedChange);            
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


        private void rdAnterior_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.BAnterior = true;
            }
            else
            {
                ViewModel.BAnterior = false;
            }
        }

        private void rdSoloSaldo_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.BsoloSaldos = true;
            }
            else
            {
                ViewModel.BsoloSaldos = false;
            }
        }

        private void rdNinguno_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.BNinguno = true;
            }
            else
            {
                ViewModel.BNinguno = false;
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.DoNothing();
        }
    
    }
}