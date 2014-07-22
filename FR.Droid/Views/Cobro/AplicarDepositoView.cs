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

namespace FR.Droid.Views.Cobro
{
    [Activity(Label = "FR - Finalizar Depósito", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class AplicarDepositoView : MvxBindingActivityView<AplicarDepositoViewModel>
    {
        MvxBindableSpinner cmbCuentas;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AplicarDeposito);
            cmbCuentas = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCuentasad);            
            cmbCuentas.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        
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