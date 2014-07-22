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
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Cobro;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ReporteDevueltoViewModel : ListViewModel
    {
        #region Propiedades

        #region Items
        public IObservableCollection<Cliente> Items { get; set; }
        #endregion Items

        private decimal montoTotal;
        public decimal MontoTotal
        {
            get { return montoTotal; }
            set
            {
                if (value != montoTotal)
                {
                    montoTotal = value;
                    RaisePropertyChanged("MontoTotal");
                }
            }
        }

        private ReporteMontos reporte;
        #endregion Propiedades

        public ReporteDevueltoViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            try
            {                
                List<Cliente> lista = Cliente.CargaReporteMontosDevueltos();                
                MontoTotal = lista.Sum(r => r.Montos.MontoDevuelto);
                reporte = new ReporteMontos(this, lista, MontoTotal);
                //binding
                //  Cliente,
                //  Montos.MontoDevueltoSinDoc FormatNumero(, TipoMoneda.DOLAR),
                //  Montos.MontoDevueltoConDoc FormatNumero(TipoMoneda.DOLAR),
                // ComandoImprimir
                // MontoTotal
                
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
            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el reporte de cierre",
                result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        if (this.Items.Count != 0)
                        {
                            //LAS. CR0-03491-N897 Se deja usar la propiedad Text del campo,
                            //se utiliza la propiedad value                            
                            reporte.ImprimeDevuelto();
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
