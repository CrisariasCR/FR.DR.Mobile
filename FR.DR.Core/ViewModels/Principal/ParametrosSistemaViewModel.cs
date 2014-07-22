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

using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using FR.Core.Model;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ParametrosSistemaViewModel : BaseViewModel
    {
        public ParametrosSistemaViewModel()
        {   
			companiaActual = string.Empty;
            rutaActual = string.Empty;
            CargarDatos();
        }


        protected bool CargarDatos()
        {
            bool procesoExitoso = true;

            try
            {

                
                Companias = new SimpleObservableCollection<string>(Util.CargarCias());
                if (Companias.Count > 0)
                {
                    CompaniaActual = Companias[0];
                }
                Rutas = new SimpleObservableCollection<string>(Util.CargarRutas());
                if (Rutas.Count > 0)
                {
                    RutaActual = Rutas[0];
                }
                //string compania = cmbCompania.Text;
                RefrescarParametros();
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(String.Format("Problemas cargando la información de los NCF's.", ex.Message));
                
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        private void RefrescarParametros()
        {
            foreach (ParametroSistema elParametro in ParametroSistema.Consecutivos)
            {
                if (elParametro.Compania.Equals(CompaniaActual) && elParametro.Zona.Equals(RutaActual))
                {
                    this.pedido = elParametro.NumeroPedido;
                    this.recibo = elParametro.NumeroRecibo;
                    this.inventario = elParametro.NumeroInventario;
                    this.devolucion = elParametro.NumeroDevolucion;
                    this.factura = elParametro.NumeroFactura;
                    this.pedidoDesc = elParametro.NumeroPedidoDescuento;
                    this.notaCredito = elParametro.NumeroNotaCredito;
                    break;
                }
            }
        }

        #region Propiedades

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                if (value != companiaActual)
                {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                    RefrescarParametros();
                }
            }
        }

        private string rutaActual;
        public string RutaActual
        {
            get { return rutaActual; }
            set
            {
                if (value != rutaActual)
                {
                    rutaActual = value;
                    RaisePropertyChanged("RutaActual");
                    RefrescarParametros();
                }
            }
        }

        public IObservableCollection<string> Companias { get; set; }
        public IObservableCollection<string> Rutas { get; set; }


        private string pedido;
        public string Pedido
        {
            get { return pedido; }
            set { pedido = value; RaisePropertyChanged("Pedido"); }
        }

        private string pedidoDesc;
        public string PedidoDesc
        {
            get { return pedidoDesc; }
            set { pedidoDesc = value; RaisePropertyChanged("PedidoDesc"); }
        }

        private string recibo;
        public string Recibo
        {
            get { return recibo; }
            set { recibo = value; RaisePropertyChanged("Recibo"); }
        }

        private string inventario;
        public string Inventario
        {
            get { return inventario; }
            set { inventario = value; RaisePropertyChanged("Inventario"); }
        }

        private string devolucion;
        public string Devolucion
        {
            get { return devolucion; }
            set { devolucion = value; RaisePropertyChanged("Devolucion"); }
        }

        private string factura;
        public string Factura
        {
            get { return factura; }
            set { factura = value; RaisePropertyChanged("Factura"); }
        }

        private string notaCredito;
        public string NotaCredito
        {
            get { return notaCredito; }
            set { notaCredito = value; RaisePropertyChanged("NotaCredito"); }
        }

        #endregion
    }
}