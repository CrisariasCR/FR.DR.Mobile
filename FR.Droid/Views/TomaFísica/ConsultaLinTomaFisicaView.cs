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
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.UI;
using FR.DR.Core.Helper;

namespace FR.Droid.Views.TomaFisica
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation|Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConsultaLinTomaFisicaView : MvxBindingActivityView<ConsultaLinTomaFisicaViewModel>
    {        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConsultaLinTomaFisica);                     
        }

        protected override void OnViewModelSet()
        {                 
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            string titulo = string.Empty;
            this.Title = "Líneas Inventario Físico";
        }
    }
}