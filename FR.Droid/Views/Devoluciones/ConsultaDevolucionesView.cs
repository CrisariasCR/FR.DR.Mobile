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
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.UI;

namespace FR.Droid.Views.Devoluciones
{
    [Activity(Label = "FR - Consulta de Devoluciones", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConsultaDevolucionesView : MvxBindingActivityView<ConsultaDevolucionesViewModel>
    {
        MvxBindableSpinner cmbCriterios;
        protected override void OnViewModelSet()
        {            
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConsultaDevoluciones);            

            cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriterioscd);
            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);            
            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            if (ViewModel.Anulando)
            {
                Title = "FR - Anulación de Devoluciones";
            }
            else
            {
                Title = "FR - Consulta de Devoluciones";
            }
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

        void cmbCriterios_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //TipoConsulta selected = EnumHelper.GetValue<TipoConsulta>((args.View as TextView).Text); 
                TipoConsulta selected = this.ViewModel.Criterios[args.Position]; ; 
                if (ViewModel.TipoConsultaSeleccionado != selected)
                {
                    ViewModel.TipoConsultaSeleccionado = selected;
                }
            }
        }

        public override void OnBackPressed()
        {
            ViewModel.regresar();
        }
    }
}