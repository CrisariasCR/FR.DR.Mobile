using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using Android.App;
//using Android.Content;
using Android.OS;
//using Android.Runtime;
//using Android.Views;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace FR.Droid.Views
{

    [Activity(Label = " Selección Cliente", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SeleccionClienteView : MvxBindingActivityView<SeleccionClienteViewModel>
    {
        MvxBindableSpinner cmbRutas, cmbDias, cmbEstadosVisita, cmbCriterios;
        EditText txtBusqueda;
        //MvxBindableListView list;
   
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SeleccionClientes);

            cmbRutas = FindViewById<MvxBindableSpinner>(Resource.Id.cmbRutasc);
            cmbDias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbDiasc);
            txtBusqueda = FindViewById<EditText>(Resource.Id.txtBusqueda);
            //list = FindViewById<MvxBindableListView>(Resource.Id.listaclientes);
            cmbEstadosVisita = FindViewById<MvxBindableSpinner>(Resource.Id.cmbVisitadosc);
            cmbCriterios = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCriteriossc);           
            

            cmbRutas.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbDias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbEstadosVisita.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbCriterios.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            //list.ChoiceMode = ChoiceMode.Single;            
            //list.DescendantFocusability = DescendantFocusability.AfterDescendants;
        
            
        }

        protected override void OnViewModelSet()
        {
            
            
        }

        protected override void OnPause()
        {
            cmbCriterios.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            cmbEstadosVisita.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbEstadosVisita_ItemSelected);
            cmbDias.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbDias_ItemSelected);
            cmbRutas.ItemSelected -= new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbRutas_ItemSelected);
            base.OnPause();
        }

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);            
        }

        protected override void OnResume()
        {
            cmbCriterios.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbCriterios_ItemSelected);
            cmbEstadosVisita.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbEstadosVisita_ItemSelected);
            cmbDias.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbDias_ItemSelected);
            cmbRutas.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(cmbRutas_ItemSelected);
            ViewModel.OnResume();
            base.OnResume();
        }

        protected override void OnStop()
        {
            txtBusqueda.RequestFocus();
            SeleccionClienteViewModel.ventanaInactiva = true;
            base.OnStop();
        }     


        #region Eventos Combos

        private void cmbCriterios_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //CriterioCliente selected = EnumHelper.GetValue<CriterioCliente>((args.View as TextView).Text);
                CriterioCliente selected = this.ViewModel.CriteriosBusqueda[args.Position];
                if (ViewModel.CriterioSeleccionado != selected)
                {
                    ViewModel.CriterioSeleccionado = selected;
                }
            }
        }

        private void cmbEstadosVisita_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //EstadoVisita selected = EnumHelper.GetValue<EstadoVisita>((args.View as TextView).Text);
                EstadoVisita selected = this.ViewModel.ItemsComboEstadoVisita[args.Position];
                if (ViewModel.EstadoVisitaActual != selected)
                {
                    ViewModel.EstadoVisitaActual = selected;
                }
            }
        }

        private void cmbDias_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null)
            {
                //DiaSemana selected = EnumHelper.GetValue<DiaSemana>((args.View as TextView).Text);
                DiaSemana selected = this.ViewModel.ItemsComboDia[args.Position];
                if (ViewModel.DiaActual != selected)
                {
                    ViewModel.DiaActual = selected;
                }
            }
        }

        private void cmbRutas_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            if (args.View != null && ViewModel.RutaActual != ViewModel.Rutas[args.Position])
            {
                ViewModel.RutaActual = ViewModel.Rutas[args.Position];
            }
        }        

        #endregion
    }
}