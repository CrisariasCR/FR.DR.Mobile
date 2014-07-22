using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario
{
    /// <summary>
    /// Reportes de los inventarios realizados en la ruta
    /// </summary>
    public class ReporteInventario : IPrintable
    {
		#region Variables de instancia

        /// <summary>
        /// instancia del viewModel que invocó el reporte, necesaria para mostrar los mensajes/alertas
        /// </summary>
        private BaseViewModel viewModel;

        private string bodega = string.Empty;
        /// <summary>
        /// Corresponde al código de la bodega de donde se realiza el inventario.
        /// </summary>
        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }
		
        private List<DetallesInventario> detallesReporte = new List<DetallesInventario>();
        /// <summary>
        /// Lista de detalles de inventario de las compañías.
        /// </summary>
        public List<DetallesInventario> DetallesReporte
        {
            get { return detallesReporte; }
            set { detallesReporte = value; }
        }

        private int totalLineas = 0;
        /// <summary>
        /// Contiene el total de las líneas del inventario.
        /// </summary>
        public int TotalLineas
        {
            get { return totalLineas; }
            set { totalLineas = value; }
        }

		#endregion

		#region Constructor
		/// <summary>
		/// Proporciona la interfaz para realizar el reporte de inventario.
		/// </summary>
		/// <param name="bodega">
		/// Corresponde al código de la bodega de donde se desea realizar el inventario.
		/// </param>
		/// <param name="companias">
		/// Corresponde al conjunto de compañías a las que corresponden los detalles del inventario.
		/// </param>
		/// <param name="detallesInventario">
		/// Corresponde a un arreglo con los detalles del inventario, donde cada item del arreglo es de tipo DetalleInventario.
		/// </param>	
        public ReporteInventario(BaseViewModel viewModel, string bodega, ICollection<string> companias, ICollection<DetalleInventario> detallesInventario)
		{
            this.viewModel = viewModel;
			this.Bodega = bodega;	
			this.CrearDetalles(companias,detallesInventario);
			this.TotalLineas = detallesInventario.Count;
		}

		#endregion

		#region Metodos de la instancia
	
		/// <summary>
		/// Crea el inventario de la(s) compañía(s) que se enviara al reporte de inventario. 
		/// Forma un arreglo con los detalles del inventario para cada compañía, 
		/// donde cada item del arreglo es de tipo DetallesInventario.
		/// </summary>
		/// <param name="companias">
		/// Corresponde al conjunto de compañías a las que corresponden los detalles del inventario.
		/// </param>
		/// <param name="detallesInventario">
		/// Corresponde a un arreglo con los detalles del inventario, donde cada item del arreglo es de tipo DetalleInventario.
		/// </param>		
        private void CrearDetalles(ICollection<string> companias, ICollection<DetalleInventario> detalles)
		{
            detallesReporte.Clear();

            DetallesInventario detallesEnCia;
			
            foreach(string cia in companias)
			{
                detallesEnCia = new DetallesInventario(cia);
                detallesEnCia.CargarEnCompania(detalles);
				this.detallesReporte.Add(detallesEnCia);
			}
		}

		#endregion

		#region IPrintable Members

		public string GetObjectName()
		{		
			return "INVENTARIOS";
		}

		public object GetField(string name)
		{
            switch (name)
            {
			    case "FECHA":return DateTime.Now;
			    case "BODEGA":return this.Bodega;	
			    case "COMPANIAS":return new ArrayList( this.detallesReporte);
			    case "TOTAL_LINEAS":return this.TotalLineas;
                default: return null;
            }
		}

		#endregion

        //Caso 29069 LDS 14/09/2007
        #region ReporteInventario

        public void Imprime()
        {
            Imprime(false);
        }

        public void Imprime(bool tomafisica)
        {
            Report reporteInventario;

            if (!tomafisica)
                reporteInventario = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteInventario), Impresora.ObtenerDriver());
            else
                reporteInventario = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteInventarioTomaFisica), Impresora.ObtenerDriver());

             reporteInventario.AddObject(this);

            Imprime(reporteInventario, tomafisica);
        }


        private void Imprime(Report reporteInventario, bool tomafisica)
        {
            reporteInventario.Print();
            if (reporteInventario.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteInventario.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        Imprime(reporteInventario, tomafisica);
                    }
                    else
                    {
                        reporteInventario = null;
                    }
                });
        }
        #endregion


	}
}
