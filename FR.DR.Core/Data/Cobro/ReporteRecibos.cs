using System;
using System.Collections;
using System.Data;
using EMF.Printing;
using System.Collections.Generic;
using System.Windows.Forms;

using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
	/// <summary>
	/// Reporte para Recibos.
	/// </summary>
    public class ReporteRecibos : IPrintable
	{
		#region Variables de instancia
		
		private List<Recibo> recibosCobro = new List<Recibo>();
        /// <summary>
        /// Lista de recibos de cobro
        /// </summary>
        public List<Recibo> RecibosCobro
        {
          get { return recibosCobro; }
          set { recibosCobro = value; }
        }

		private decimal totalRecibos = 0;
        /// <summary>
        /// Total de recibos
        /// </summary>
        public decimal TotalRecibos
        {
          get { return totalRecibos; }
          set { totalRecibos = value; }
        }

        private ClienteBase cliente;
        /// <summary>
        /// Cliente asociado al reporte
        /// </summary>
        public ClienteBase Cliente
        {
          get { return cliente; }
          set { cliente = value; }
        }


		#endregion

		#region Constructor

        /// <summary>
        /// Constructor de reporte 
        /// </summary>
        /// <param name="recibos">lista de recibos asociados</param>
        /// <param name="cliente">cliente asociadao</param>
		public ReporteRecibos(List<Recibo> recibos, ClienteBase cliente)
		{
            this.cliente = cliente;
			this.recibosCobro = recibos;
		}
		/// <summary>
		/// Constructor de reporte
		/// </summary>
        /// <param name="recibos">lista de recibos asociados</param>
		/// <param name="totalRecibos">total por concepto de los recibos de cobro</param>
		public ReporteRecibos(List<Recibo> recibos,decimal totalRecibos)
		{
			this.recibosCobro = recibos;
			this.totalRecibos = totalRecibos;
		}

        public ReporteRecibos()
        { }

		#endregion

        #region Impresion del resumen de los recibos

        /// <summary>
        /// Forma la colilla que se imprime indicando las posiciones 
        /// en las que va la información y los componentes que formen la 
        /// colilla.
        /// </summary>
        /// <param name="recibos">la lista de recibos a imprimir</param>
        /// <param name="cliente">el cliente asociado</param>
        public void ImprimeResumenRecibos(List<Recibo> recibos, ClienteBase cliente, BaseViewModel viewModel)
        {
            Report resumenRecibos = new Report(ReportHelper.CrearRutaReporte(Rdl.ResumenRecibos), Impresora.ObtenerDriver());

            resumenRecibos.AddObject(new ReporteRecibos(recibos,cliente));

            PreguntarDeNuevoResumenRecibo(resumenRecibos,viewModel);            
                
        }

        public void PreguntarDeNuevoResumenRecibo(Report reporteRecibo,BaseViewModel viewModel) 
        {
            reporteRecibo.Print();

            if (reporteRecibo.ErrorLog != string.Empty)
            {

                viewModel.mostrarAlerta("Ocurrió un error durante la impresión del recibo: " + reporteRecibo.ErrorLog);
            }
            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",res=>
            {
                if (res == DialogResult.Yes)
                {
                    PreguntarDeNuevoResumenRecibo(reporteRecibo,viewModel);
                }
                else
                {
                    return;
                }
            });
        }

        #endregion


        #region Reporte Cierre Cobro

        /// <summary>
        /// Metodo encargado de pintar y definir toda la interfaz grafica 
        /// del reporte de cierre. Se indican los puntos donde se quiere que 
        /// se imprima desde las tablas hasta la información a desplegar
        /// </summary>
        public void ImprimeReporteCierre(BaseViewModel viewModel)
        {
            Report reporteCierre = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteCierre), Impresora.ObtenerDriver());
            reporteCierre.AddObject(this);
            ImprimeReporteCierre(viewModel, reporteCierre);
        }

        private void ImprimeReporteCierre(BaseViewModel viewModel, Report reporteCierre)
        {
            reporteCierre.Print();
            if (reporteCierre.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteCierre.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        ImprimeReporteCierre(viewModel, reporteCierre);
                    }
                });
        }

        /// <summary>
        /// Carga los montos cobrados y los montos de notas de crédito
        /// que se le cobraron y aplicaron respectivamente a los clientes a los que se 
        /// les realizo un cobro
        /// </summary>
        public void CargaReporteCierre()
        {
            recibosCobro.Clear();
            recibosCobro = Recibo.CargaRecibosGenerados();

            foreach (Recibo recibo in recibosCobro)
            {
                try
                {
                    recibo.CargaDetalles();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error cargando detalles del recibo '" + recibo.Numero + "'. " + ex.Message);
                }
            }
        }

        #endregion       


        #region Impresion del detalle de los recibos desde la consulta

        //Caso 25452 LDS 30/10/2007
        /// <summary>
        /// Imprime el detalle de los recibos que hayan sido seleccionados.
        /// </summary>
        /// <param name="cantidadCopias">Indica la cantidad de copias que se requiere imprimir de los recibos. </param>        
        public void ImprimeDetalleRecibos(int cantidadCopias)
        {
            Report reporteRecibo;
            string imprimirTodo = string.Empty;

            //Imprime el original
            bool imprimirOriginal = false;

            foreach (Recibo recibo in recibosCobro)
                if (recibo.LeyendaOriginal && !recibo.Impreso)
                {
                    imprimirOriginal = true;
                    break;
                }
            //Contiene los recibos que ya han sido impresos
            List<Recibo> recibosImpresos = new List<Recibo>();

            if (imprimirOriginal)
            {
                foreach (Recibo recibo in recibosCobro)
                    if (recibo.Impreso)
                        recibosImpresos.Add(recibo);
                

                reporteRecibo = new Report(ReportHelper.CrearRutaReporte(Rdl.DetalleRecibos), Impresora.ObtenerDriver());
                reporteRecibo.AddObject(this);
                //reporteRecibo.Print();
                reporteRecibo.PrintAll(ref imprimirTodo);
                imprimirTodo += "\n\n";
                if (reporteRecibo.ErrorLog != string.Empty)
                    throw new Exception("Ocurrió un error durante la impresión de los recibos: " + reporteRecibo.ErrorLog);

                reporteRecibo = null;

                foreach (Recibo recibo in recibosCobro)
                    if (!recibo.Impreso && recibo.LeyendaOriginal)
                    {
                        recibo.Impreso = true;
                        recibo.LeyendaOriginal = false;
                        recibo.ActualizarImpresion();
                    }

                for (int indice = recibosImpresos.Count; indice > 0; indice--)
                    recibosCobro.Remove((Recibo)recibosImpresos[indice - 1]);
            }

            //Imprime las copias con la leyenda de copia
            if (cantidadCopias > 0)
            {
                foreach (Recibo recibo in recibosImpresos)
                    recibosCobro.Add(recibo);

                reporteRecibo = new Report(ReportHelper.CrearRutaReporte(Rdl.DetalleRecibos), Impresora.ObtenerDriver());
                reporteRecibo.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteRecibo.Print();
                    reporteRecibo.PrintAll(ref imprimirTodo);
                    imprimirTodo += "\n\n";
                    if (reporteRecibo.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de los recibos: " + reporteRecibo.ErrorLog);
                }
                //Imprime todo a la vez
                reporteRecibo.PrintText(imprimirTodo);
                reporteRecibo = null;
            }
        }

        #endregion

		#region IPrintable Members

		public string GetObjectName()
		{		
			return "RECIBOS";
		}

		public object GetField(string name)
		{
            switch (name)
            {
			    case "TOTAL":return  this.totalRecibos;
			    case "CLIENTE_NOMBRE":return  cliente.Nombre;
			    case "CLIENTE_CODIGO":return  cliente.Codigo;
			    case "FECHA":return DateTime.Now;
			    case "LISTA_RECIBOS":return new ArrayList(RecibosCobro);
                default: return null;
            }
		}

		#endregion
	}
}
