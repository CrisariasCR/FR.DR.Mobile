using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Net;
//using System.Windows;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Cobro;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaNotasCreditoViewModel : ListViewModel
    {

        #region Propiedades

        #region Items e ItemActual
        private NotaCredito itemActual;
        public  NotaCredito ItemActual
        {
            get { return itemActual; }
            set
            {
               itemActual = value;
               SeleccionarNota(value);
               RaisePropertyChanged("ItemActual");
                
            }
        }

        private List<NotaCredito> ItemsSeleccionados
        {
            get
            {
                return new List<NotaCredito>(this.Items.Where<NotaCredito>(x => x.Selected));
            }

        }

        /// <summary>
        /// retorma la coleccion de Indices seleccionados
        /// </summary>
        public List<int> SelectedIndex
        {
            get
            {
                List<int> result = new List<int>();
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Selected)
                        result.Add(i);
                }
                return result;
            }
        }

        private IObservableCollection<NotaCredito> items;
        public IObservableCollection<NotaCredito> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        } 

        #endregion Items e ItemActual

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set
            {
                if (value != nombreCliente)
                {
                    nombreCliente = value;
                    RaisePropertyChanged("NombreCliente");
                }
            }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private DateTime fechaCreacionNC;
        public DateTime FechaCreacionNC
        {
            get { return fechaCreacionNC; }
            set
            {
                if (value != fechaCreacionNC)
                {
                    fechaCreacionNC = value;
                    RaisePropertyChanged("FechaCreacionNC");
                }
            }
        }

        private decimal montoNC;
        public decimal MontoNC
        {
            get { return montoNC; }
            set
            {
                if (value != montoNC)
                {
                    montoNC = value;
                    RaisePropertyChanged("MontoNC");
                }
            }
        }

        private decimal totalNotasCredito;
        public decimal TotalNotasCredito
        {
            get { return totalNotasCredito; }
            set
            {
                if (value != totalNotasCredito)
                {
                    totalNotasCredito = value;
                    RaisePropertyChanged("TotalNotaCredito");
                }
            }
        }

        private decimal totalSaldoFacturas;
        public decimal TotalSaldoFacturas
        {
            get { return totalSaldoFacturas; }
            set
            {
                if (value != totalSaldoFacturas)
                {
                    totalSaldoFacturas = value;
                    RaisePropertyChanged("TotalSaldoFacturas");
                }
            }
        }

        
        #endregion Propiedades

        public ConsultaNotasCreditoViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            try
            {
                List<NotaCredito> lista = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).NotasCredito;
                this.Items = new SimpleObservableCollection<NotaCredito>(lista);
                RaisePropertyChanged("Items");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando notas del cliente. " + ex.Message);
            }

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
            {
                this.TotalNotasCredito = Cobros.MontoNotasCreditoLocal;
                this.TotalSaldoFacturas = Cobros.MontoFacturasLocal;
            }
            else
            {
                this.TotalNotasCredito = Cobros.MontoNotasCreditoDolar;
                this.TotalSaldoFacturas = Cobros.MontoFacturasDolar;
            }
        }

        #endregion CargaInicial

        #region Comandos

        #endregion Comandos

        #region Acciones

        /// <summary>
        /// Funcion que se ejecuta con el cambio de indice del
        /// listview muestra la informacion de la nota de credito seleccionada
        /// </summary>
        private void SeleccionarNota(NotaCredito nota)
        {            
            //Verifica que exista una linea valida seleccionada
            if (nota != null)
            {                
                // binding fechaObtenerFechaString
                this.FechaCreacionNC = nota.FechaRealizacion;

                // binding fechaObtenerFechaString
                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    this.MontoNC = nota.MontoDocLocal;
                else
                    this.MontoNC = nota.MontoDocDolar;
            }
        }

        public void Regresar()
        {
            this.DoClose();
            //this.RequestNavigate<AplicarPagoViewModel>();
        }

        #endregion Accioness
	

    }
}
