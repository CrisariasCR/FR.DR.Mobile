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
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;

namespace FR.Droid.Views.Garantia
{
    [Activity(Label = " Configuraci�n de Garant�a", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConfiguracionGarantiaView : MvxBindingActivityView<ConfiguracionGarantiaViewModel> 
    {
        MvxBindableSpinner cmbCompanias, cmbPais,cmbNivelPrecio;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConfiguracionGarantia);
            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniascfp);
            cmbPais = FindViewById<MvxBindableSpinner>(Resource.Id.cmbPaiscfp);
            cmbNivelPrecio = FindViewById<MvxBindableSpinner>(Resource.Id.cmbNivelPreciocfp);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbPais.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbNivelPrecio.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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

        public override void OnBackPressed()
        {
            ViewModel.ComandoCancelar.Execute(null);
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
    }
}