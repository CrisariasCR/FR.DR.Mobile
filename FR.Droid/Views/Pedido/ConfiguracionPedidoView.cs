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

namespace FR.Droid.Views.Pedido
{
    [Activity(Label = " Configuración de Pedido", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConfiguracionPedidoView : MvxBindingActivityView<ConfiguracionPedidoViewModel> 
    {
        MvxBindableSpinner cmbCompanias, cmbPais, cmbCondicion, cmbNivelPrecio, cmbDiv1cfp, cmbDiv2cfp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConfiguracionPedido);
            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniascfp);
            cmbPais = FindViewById<MvxBindableSpinner>(Resource.Id.cmbPaiscfp);
            cmbDiv1cfp = FindViewById<MvxBindableSpinner>(Resource.Id.cmbDiv1cfp);
            cmbDiv2cfp = FindViewById<MvxBindableSpinner>(Resource.Id.cmbDiv2cfp);
            cmbCondicion = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCondicioncfp);
            cmbNivelPrecio = FindViewById<MvxBindableSpinner>(Resource.Id.cmbNivelPreciocfp);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbPais.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbDiv1cfp.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbDiv2cfp.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCondicion.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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