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

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "FR - Parámetros del Sistema", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ParametrosSistemaView : MvxBindingActivityView<ParametrosSistemaViewModel>
    {
        MvxBindableSpinner cmbCompanias, cmbRutas;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ParametrosSistema);
            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniasps);
            cmbRutas = FindViewById<MvxBindableSpinner>(Resource.Id.cmbRutasps);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbRutas.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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
    }
}