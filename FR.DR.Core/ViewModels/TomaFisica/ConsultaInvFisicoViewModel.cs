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

using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaInvFisicoViewModel : ListViewModel
    {
        public ConsultaInvFisicoViewModel()
        {
            
        }

        #region Propiedades            

        private IObservableCollection<VentaConsignacion> items;
        public IObservableCollection<VentaConsignacion> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }               

        #endregion

        #region Comandos

        //public ICommand ComandoConsultar
        //{
        //    get { return new MvxRelayCommand(ConsultarConsignacion); }
        //}

        //public ICommand ComandoEliminar
        //{
        //    get { return new MvxRelayCommand(EliminarConsignacion); }
        //}

        //public ICommand ComandoImprimir
        //{
        //    get { return new MvxRelayCommand(EliminarConsignacion); }
        //}

        #endregion

        #region Acciones



        #endregion

        
    }
}