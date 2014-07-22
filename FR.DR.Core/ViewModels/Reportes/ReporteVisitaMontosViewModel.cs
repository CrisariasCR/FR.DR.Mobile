using System;
//using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ReporteVisitaMontosViewModel : ListViewModel
    {
        #region Propiedades

        #region Items
        private IObservableCollection<Cliente> items;
        public IObservableCollection<Cliente> Items
        {
            get { return items; }
            set
            {
                items = value;
                RaisePropertyChanged("Items");

            }
        }
        #endregion Items e ItemActual

        private decimal montoCobrado;
        public decimal MontoCobrado
        {
            get { return montoCobrado; }
            set
            {
                if (value != montoCobrado)
                {
                    montoCobrado = value;
                    RaisePropertyChanged("MontoCobrado");
                }
            }
        }
        // Caso: 27409 LDS 20/03/2007
        /// <summary>
        /// Es el monto total vendido en preventa.
        /// </summary>
        private decimal montoPreventa;
        public decimal MontoPreventa
        {
            get { return montoPreventa; }
            set
            {
                if (value != montoPreventa)
                {
                    montoPreventa = value;
                    RaisePropertyChanged("MontoPreventa");
                }
            }
        }
        // Caso: 27409 LDS 20/03/2007
        /// <summary>
        /// Es el monto total vendido facturado.
        /// </summary>
        private decimal montoFacturas;
        public decimal MontoFacturas
        {
            get { return montoFacturas; }
            set
            {
                if (value != montoFacturas)
                {
                    montoFacturas = value;
                    RaisePropertyChanged("MontoFacturas");
                }
            }
        }
        // Caso: 27409 LDS 20/03/2007
        /// <summary>
        /// Es el monto total vendido.
        /// </summary>
        /// <remarks>
        /// montoVendido = montoPreventa + montoFacturas.
        /// </remarks>		 
        private decimal montoVendido { get; set; }
        public decimal MontoVendido
        {
            get { return montoVendido; }
            set
            {
                if (value != montoVendido)
                {
                    montoVendido = value;
                    RaisePropertyChanged("MontoVendido");
                }
            }
        }
        #endregion Propiedades

        public ReporteVisitaMontosViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            try
            {
                
                //Caso:ASF LDS 31/05/2007
                //Quitamos del arreglo los clientes con montos cero
                
                //Seleccionar los clientes del reporte cuyos montos sean mayores a cero
                List<Cliente> lista = Cliente.CargaReporteMontos().Where(c => c.Montos.MontoPagado > 0 || c.Montos.MontoNotasAplicadas > 0 || c.Montos.MontoComprado > 0).ToList();

                MontoCobrado = lista.Sum(c => decimal.Round(c.Montos.MontoPagado, 2));
                MontoPreventa = lista.Sum(c => decimal.Round(c.Montos.MontoPreventa, 2)); // Caso: 27409 LDS 20/03/2007
                MontoFacturas = lista.Sum(c => decimal.Round(c.Montos.MontoFacturas, 2)); // Caso: 27409 LDS 20/03/2007
                MontoVendido = lista.Sum(c => decimal.Round(c.Montos.MontoComprado, 2));  // Caso: 27409 LDS 20/03/2007

                // binding:
                // empty
                // Nombre,
                // Montos.MontoPagado con convertidor: FormatNumero
                // Montos.MontoNotasAplicadas
				// Montos.MontoComprado
				// Montos.MontoPreventa
				// Montos.MontoFacturas 
                // ComandoImprimir

                this.Items = new SimpleObservableCollection<Cliente>(lista);
                RaisePropertyChanged("Items");
            }
            catch (Exception exc)
            {
                this.mostrarAlerta("Error en la carga del reporte. " + exc.Message);
            }
        }

        #endregion CargaInicial

        #region Comandos y Acciones

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }


        /// Permite realizar la impresion del reporte siempre y cuando
        /// haya información para imprimirlo
        /// </summary>
        private void Imprime()
        {
            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el reporte de montos",
                result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        ReporteMontos trans = new ReporteMontos(
                            this,
                            this.Items,
                            this.montoCobrado,
                            this.montoVendido,
                            this.montoPreventa,
                            this.montoFacturas);

                        if (this.Items.Count != 0)
                        {
                            trans.ImprimeVendidoCobrado();
                        }
                        else
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "El reporte no tiene información.");
                        }
                    }
                });
        }

        #endregion Accioness
    }
}
