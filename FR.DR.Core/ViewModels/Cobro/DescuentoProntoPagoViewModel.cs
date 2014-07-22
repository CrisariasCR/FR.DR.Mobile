using System;
//using System.Net;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls; // FrmConfig
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI; //GlobalUI
using FR.DR.Core.Helper;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DescuentoProntoPagoViewModel : BaseViewModel
    {

        #region Constructor
        public DescuentoProntoPagoViewModel() 
        {
            Cargar(Gestor.DescuentosFacturas);
        }
        #endregion

        #region Propiedades

        private IObservableCollection<FacturaDescuento> items;
        public IObservableCollection<FacturaDescuento> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }

        private decimal monto;
        public decimal Monto
        {
            get { return monto; }
            set
            {
                monto = value;
                RaisePropertyChanged("Monto");
            }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #endregion

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Salir); }
        }


        #endregion

        #region mobile
        #region Metodos

        public void Salir() 
        {
            this.DoClose();
        }

        /// <summary>
        /// Carga el ListView con los descuentos en la lista.
        /// </summary>
        /// <param name="descuentos"></param>
        private void Cargar(List<FacturaDescuento> descuentos)
        {
            Items = new SimpleObservableCollection<FacturaDescuento>(descuentos);
            //foreach (FacturaDescuento descuento in descuentos)
            //{
            //    ListViewItem item = new ListViewItem(
            //        new string[]{descuento.TipoDocumento.ToString()
            //            ,descuento.Documento
            //            ,String.Format("{0}%",descuento.Porcentage)
            //            ,GestorUtilitario.FormatNumero(descuento.Monto)});
                
            //    montoTotal += descuento.Monto;
            //    lsvDescuentos.Items.Add(item);
            //}
            Monto = descuentos.Sum(x => x.Monto);
        }
        #endregion

        #endregion

    }
}
