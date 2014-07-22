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

namespace FR.Droid.Views.Devoluciones
{
    [Activity(Label = " Tipo de Devolución", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class TipoDevolucionView : MvxBindingActivityView<TipoDevolucionViewModel>
    {
        RadioButton rdConDoc, rdSinDoc;
        RadioButton rdrCredito, rdrEfectivo;
        MvxBindableSpinner cmbTipo, cmbCompanias,cmbPais,cmbDiv1cd, cmbDiv2cd;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TipoDevolucion);
            rdConDoc = FindViewById<RadioButton>(Resource.Id.rdConDoc);
            rdSinDoc = FindViewById<RadioButton>(Resource.Id.rdSinDoc);
            cmbCompanias= FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniastd);
            cmbTipo = FindViewById<MvxBindableSpinner>(Resource.Id.cmbTipotd);
            cmbPais = FindViewById<MvxBindableSpinner>(Resource.Id.cmbPaiscd);
            cmbDiv1cd = FindViewById<MvxBindableSpinner>(Resource.Id.cmbDiv1cd);
            cmbDiv2cd = FindViewById<MvxBindableSpinner>(Resource.Id.cmbDiv2cd);

            rdConDoc.Click += new EventHandler(onRadioButtonClicked);
            rdSinDoc.Click += new EventHandler(onRadioButtonClicked);

            rdrCredito = FindViewById<RadioButton>(Resource.Id.radioCredito);
            rdrEfectivo = FindViewById<RadioButton>(Resource.Id.radioEfectivo);

            rdrCredito.Click += new EventHandler(onRadioButtonClicked2);
            rdrEfectivo.Click += new EventHandler(onRadioButtonClicked2);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbTipo.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbPais.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbDiv1cd.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbDiv2cd.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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

        void onRadioButtonClicked2(object sender, EventArgs e)
        {
            View view = (View)sender;
            if (view.Id == Resource.Id.radioEfectivo)
            {
                ViewModel.TipoPagoDevolucion = "E";
            }
            else
            {
                ViewModel.TipoPagoDevolucion = "C";
            }
        }

        void onRadioButtonClicked(object sender, EventArgs e)
        {
            View view = (View)sender;
            switch (view.Id)
            {
                case Resource.Id.rdConDoc:
                    ViewModel.ConDocumentoChecked = true;
                    break;
                default:
                    ViewModel.ConDocumentoChecked = false;
                    break;
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.ComandoCancelar.Execute(null);
        }
    }
}