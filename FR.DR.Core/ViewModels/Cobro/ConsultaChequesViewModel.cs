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

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaChequesViewModel : ListViewModel
    {
#pragma warning disable 414

        #region Constructores

        /// <summary>
        /// Crea un nuevo formulario de cheques para consultar.
        /// </summary>
        /// <param name="chequesAMostrar">Cheques que se desea que se muestren al cargar el formulario</param>
        public ConsultaChequesViewModel()
        {
            this.chequesIngresados = ConsultaCobroViewModel.ReciboSeleccionado.ChequesAsociados;
            this.Cheques = new SimpleObservableCollection<Cheque>(this.chequesIngresados);
            this.esConsulta = true;
            this.TotalCheques = ConsultaCobroViewModel.ReciboSeleccionado.TotalCheques;
            this.moneda = ConsultaCobroViewModel.ReciboSeleccionado.Moneda;
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

        public Recibo Recibo { get { return ConsultaCobroViewModel.ReciboSeleccionado; } }

        //public IObservableCollection<Cheque> Cheques { get { return new SimpleObservableCollection<Cheque>(Recibo.ChequesAsociados); } }
        public IObservableCollection<Cheque> Cheques { get; set; }

        public List<Cheque> chequesIngresados = new List<Cheque>();

        private bool esConsulta;

        private decimal totalCheques;
        public decimal TotalCheques
        {
            get { return totalCheques; }
            set
            {
                totalCheques = value;
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