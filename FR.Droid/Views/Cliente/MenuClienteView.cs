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

namespace FR.Droid.Views.Cliente
{
    [Activity(Label = "FR - Procesos del Sistema", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation|Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MenuClienteView : MvxBindingActivityView<MenuClienteViewModel>
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MenuCliente);

            FindViewById<ImageButton>(Resource.Id.btnMenuAnular).Click += delegate
            {
                this.Title = "FR - Procesos del Cliente-Anular";
            };

            FindViewById<ImageButton>(Resource.Id.btnMenuConsultar).Click += delegate
            {
                this.Title = "FR - Procesos del Cliente-Consultar";
            };            
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

        protected override void OnViewModelSet()
        {                        
        }

        public override void OnBackPressed()
        {
            this.Title = "FR - Procesos del Cliente";
            ViewModel.ComandoRegresar.Execute(null);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            ViewModel.Resumir();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //ViewModel.Resumir();
        }

    }
}