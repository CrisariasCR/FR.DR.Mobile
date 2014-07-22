//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using Android.App;
//using Android.Content;
using Android.OS;
//using Android.Runtime;
using Android.Views;
//using Android.Widget;

using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
//using FR.DR.Core.Helper;

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Actualizar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ActualizarView2 : MvxBindingActivityView<ActualizarCustomViewModel>
    {      

        protected override void OnCreate(Bundle bundle)
        {
            ActualizarCustomViewModel.contexto = this;
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Actualizar2);                                  
        }

        protected override void OnViewModelSet()
        {
            
        }

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            this.Title = "FR - " + this.ViewModel.ObtenerTitulo();
            if (ViewModel.NohayArchivos)
                ViewModel.MostrarMensajeSinArchivos();
        }

        protected override void OnResume()
        {            
            base.OnResume();            
        }

    }
}