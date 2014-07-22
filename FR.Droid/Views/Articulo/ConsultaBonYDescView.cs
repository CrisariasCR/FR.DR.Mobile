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
    [Activity(Label = "FR - Detalles Descuentos", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConsultaBonYDescView : MvxBindingActivityView<ConsultaBonYDescViewModel>
    {
        MvxBindableListView header;
        MvxBindableListView lista;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConsultaBonYDesc);

            this.header = (MvxBindableListView)this.FindViewById(Resource.Id.HeaderLista);
            this.lista = (MvxBindableListView)this.FindViewById(Resource.Id.ListaBonYDescs);
         
            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            if (this.ViewModel.EsDescuento)
            {
                this.Title = "FR - Detalle Descuento Escalonado";
                header.ItemTemplateId = Resource.Layout.ConsultaBonYDescHeaderDesc;
                lista.ItemTemplateId = Resource.Layout.ConsultaBonYDescItemDesc;
            }
            else
            {
                this.Title = "FR - Detalle Bonificación";
                header.ItemTemplateId = Resource.Layout.ConsultaBonYDescHeaderBon;
                lista.ItemTemplateId = Resource.Layout.ConsultaBonYDescItemBon;
            }
        }

        protected override void OnViewModelSet()
        {
            
        }

    }
}