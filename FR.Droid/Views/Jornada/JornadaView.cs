using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading.Tasks;
using System.Threading;
using Softland.ERP.FR.Mobile.Cls;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;

namespace FR.Droid.Views.Cliente
{
    [Activity(Label = "FR - Rubros de Jornada", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class JornadaView : MvxBindingActivityView<RubrosJornadaViewModel>
    {

        protected override void OnViewModelSet()
        {            
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.RubrosJornada);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
        }

        public override void OnBackPressed()
        {
           //Do Nothing
        }
    }
}