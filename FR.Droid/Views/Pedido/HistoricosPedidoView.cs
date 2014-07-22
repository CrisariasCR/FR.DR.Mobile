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

namespace FR.Droid.Views.Pedido
{
    [Activity(Label = "FR - Históricos Pedido", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class HistoricosPedidoView : MvxBindingActivityView<HistoricoPedidosViewModel>
    {
        MvxBindableSpinner cmbEstados;
        MvxBindableListView lista, header;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.HistoricosPedido);
            cmbEstados = FindViewById<MvxBindableSpinner>(Resource.Id.cmbEstadoshp);
            cmbEstados.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbEstados_ItemSelected);

            lista = FindViewById<MvxBindableListView>(Resource.Id.ListaPedidos);
            header = FindViewById<MvxBindableListView>(Resource.Id.HeaderLista);            

            cmbEstados.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
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

        void cmbEstados_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.View != null)
            {
                //string selected = (e.View as TextView).Text; 
                string selected = this.ViewModel.Estados[e.Position];
                if (selected == "Todos")
                {
                    lista.ItemTemplateId = Resource.Layout.HistoricosPedidosItemTodos;
                    header.ItemTemplateId = Resource.Layout.HistoricosPedidosHeaderTodos;
                }
                else
                {
                    lista.ItemTemplateId = Resource.Layout.HistoricosPedidosItem;
                    header.ItemTemplateId = Resource.Layout.HistoricosPedidosHeader;
                }
                if (ViewModel.EstadoSeleccionado != selected && selected != "Todos")
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