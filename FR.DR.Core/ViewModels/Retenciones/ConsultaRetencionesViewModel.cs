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
using Softland.ERP.FR.Mobile.UI;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls;
using FR.DR.Core.Data.Documentos.Retenciones;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaRetencionesViewModel : ListViewModel
    {
#pragma warning disable 414

        #region Constructores

        /// <summary>
        /// Crea un nuevo formulario de retenciones para consultar.
        /// </summary>        
        public ConsultaRetencionesViewModel()
        {
            this.Retenciones = new SimpleObservableCollection<fclsRetencion>( Gestor.Pedidos.Gestionados[0].iArregloRetenciones);
            this.esConsulta = true;
            this.TotalRetenciones = Gestor.Pedidos.Gestionados[0].inTotalRetenciones;
            if (Gestor.Pedidos.Gestionados[0].Moneda.Equals(TipoMoneda.LOCAL))
                this.moneda = TipoMoneda.LOCAL;
            else
                this.moneda = TipoMoneda.DOLAR;
            this.LoadPantalla();
        }

        #endregion

        #region Propiedades

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

        public IObservableCollection<fclsRetencion> retenciones;

        //public IObservableCollection<Cheque> Cheques { get { return new SimpleObservableCollection<Cheque>(Recibo.ChequesAsociados); } }
        public IObservableCollection<fclsRetencion> Retenciones
        {
            get { return retenciones; }
            set {retenciones=value;RaisePropertyChanged("Retenciones");}
        }

        private bool esConsulta;

        private decimal totalRetenciones;
        public decimal TotalRetenciones
        {
            get { return totalRetenciones; }
            set
            {
                totalRetenciones = value;
                RaisePropertyChanged("TotalCheques");
            }
        }

        private TipoMoneda moneda;

        #endregion


        #region Metodos de la logica de Negocio

      
        /// <summary>
		/// Funcion que carga los datos iniciales.
		/// </summary>
		private void LoadPantalla()
		{            
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNomClt.Visible = false;			
		}		


        #endregion

       
    }
}