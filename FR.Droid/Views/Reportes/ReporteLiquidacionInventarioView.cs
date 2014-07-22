using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Mobile;
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

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Reporte Liquidación Inventario", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ReporteLiquidacionInventarioView : MvxBindingActivityView<ReporteLiquidacionInventarioViewModel>
    {
        MvxBindableSpinner cmbCompaniasrli; 
        MvxBindableSpinner cmbBodegarli;
        EditText txtBusqueda;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ReporteLiquidacionInventario);
            this.cmbCompaniasrli = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniasrli);
            this.cmbBodegarli = FindViewById<MvxBindableSpinner>(Resource.Id.cmbBodegarli);
            this.txtBusqueda = (EditText)this.FindViewById(Resource.Id.txtBusqueda);
            cmbCompaniasrli.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbBodegarli.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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