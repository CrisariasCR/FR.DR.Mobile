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

namespace FR.Droid.Views.Pedido
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation|Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConsultaPedidosView : MvxBindingActivityView<ConsultaPedidosViewModel>
    {
        MvxBindableSpinner cmbCriterios;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConsultaPedidos);
            cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriterioscp);
            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);            

            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        }

        protected override void OnViewModelSet()
        {         
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            string titulo = ViewModel.Anulando ? "FR - Anulación de " : "FR - Consulta de ";
            string entidad = ViewModel.TipoPedido == TipoPedido.Factura ? "Facturas" : "Pedidos";
            this.Title = titulo + entidad;
        }

        private void cmbCriterios_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //TipoConsulta selected = EnumHelper.GetValue<TipoConsulta>((args.View as TextView).Text);
                TipoConsulta selected = this.ViewModel.Estados[args.Position];
                if (ViewModel.EstadoSeleccionado != selected)
                {
                    ViewModel.EstadoSeleccionado = selected;
                }
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

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }
    }
}