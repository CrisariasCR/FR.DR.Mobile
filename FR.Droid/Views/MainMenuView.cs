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

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Menú Principal", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainMenuView : MvxBindingActivityView<MenuPrincipalViewModel>
    {
        //public static List<MvxBindingActivityView<BaseViewModel>> VentanasAbiertas = new List<MvxBindingActivityView<BaseViewModel>>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainMenu);
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
            var MenuItem1 = menu.Add(0, 1, 1, "Cerrar Sesión");
            //var MenuItem2 = menu.Add(0, 2, 2, "Salir");

            MenuItem1.SetIcon(Resource.Drawable.ic_login);
            //MenuItem2.SetIcon(Resource.Drawable.ic_cancelar);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 1:
                    ViewModel.CerrarSesion();
                    return true;
                default:
                    return true;
            }
        }

        public override void OnBackPressed()
        {
            //ViewModel.CerrarSesion();
            this.MoveTaskToBack(true);
        }

        protected override void OnPause()
        {            
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
                
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("CERO", 0);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);

            decimal numero = savedInstanceState.GetInt("CERO");
        }
        
    }
}