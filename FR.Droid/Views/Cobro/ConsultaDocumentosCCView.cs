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
using Softland.ERP.FR.Mobile.UI;
using FR.DR.Core.Helper;

using Softland.ERP.FR.Mobile.Cls;

namespace FR.Droid.Views
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation|Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConsultaDocumentosCCView : MvxBindingActivityView<ConsultaDocumentosCCViewModel>
    {

        RadioButton radioLocal, radioDolar;
        MvxBindableListView listaDocumentos;
        MvxBindableSpinner cmbCompanias;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConsultaDocumentosCC);
            radioLocal = FindViewById<RadioButton>(Resource.Id.radioLocal);
            radioDolar = FindViewById<RadioButton>(Resource.Id.radioDolar);

            radioLocal.Click += new EventHandler(onRadioButtonClicked);
            radioDolar.Click += new EventHandler(onRadioButtonClicked);
            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniascdcc);
            listaDocumentos = FindViewById<MvxBindableListView>(Resource.Id.ListaDocumentos);            
            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        }

        protected override void OnViewModelSet()
        {            
            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            this.Title = ViewModel.TipoDocumento == TipoDocumento.Factura ? "FR - Documentos Pendientes" : "FR - Notas de Crédito";
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

        void onRadioButtonClicked(object sender, EventArgs e)
        {
            View view = (View)sender;
            switch (view.Id)
            {
                case Resource.Id.radioLocal: 
                    listaDocumentos.ItemTemplateId = Resource.Layout.ConsultaDocumentosCCItemLocal;
                    ViewModel.TipoMoneda = TipoMoneda.LOCAL;
                    break;
                case Resource.Id.radioDolar: 
                    listaDocumentos.ItemTemplateId = Resource.Layout.ConsultaDocumentosCCItemDolar;
                    ViewModel.TipoMoneda = TipoMoneda.DOLAR;
                    break;
            }
        }
       

        public override void OnBackPressed()
        {
            ViewModel.DoClose();
            
        }
    }
}