using System;
using System.Collections;
using System.Data;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using System.Collections.Generic;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.Reporte
{
	/// <summary>
	/// Reporte de montos.
	/// </summary>
	public class ReporteMontos:IPrintable
	{
        /// <summary>
        /// instancia del viewModel que invocó el reporte, necesaria para mostrar los mensajes/alertas
        /// </summary>
        private BaseViewModel viewModel;
        //mvega: se usa ICollection en lugar de List, pues es más genérico, y entonces podemos usar un SimpleObservableCollection
        private ICollection<Cliente> clientes;
		private decimal MontoCobro;
		private decimal MontoVendido;
		private decimal MontoPreventa;
		private decimal MontoFacturado;
        private decimal MontoDevuelto;

		public ReporteMontos(
            BaseViewModel viewModel,
            ICollection<Cliente> clientes,
			decimal montoCobro,
			decimal montoVendido, 
			decimal montoPreventa,
			decimal montoFacturado)
		{
            this.viewModel = viewModel;
            this.clientes = clientes;
			this.MontoCobro = montoCobro;
			this.MontoVendido = montoVendido;
			this.MontoPreventa = montoPreventa;
			this.MontoFacturado = montoFacturado;
		}

        public ReporteMontos(
            BaseViewModel viewModel,
            ICollection<Cliente> clientes,
            decimal montoDevuelto)
        {
            this.viewModel = viewModel;
            this.clientes = clientes;
            this.MontoCobro = this.MontoVendido = this.MontoPreventa = this.MontoFacturado = 0;
            this.MontoDevuelto = montoDevuelto;
        }

        #region Impresion del reporte de montos vendidos y cobrados

        /// <summary>
        /// Metodo encargado de definir como se va a ver el reporte una vez impreso
        /// en este metodo se indica la posicion de todo lo que vaya en el reporte 
        /// </summary>
        public void ImprimeVendidoCobrado()
        {
            Report reporteRecibo = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteMontos), Impresora.ObtenerDriver());

            reporteRecibo.AddObject(this);

            ImprimeVendidoCobrado(reporteRecibo);

        }

        /// <summary>
        /// imprime y pregunta por reimpresión
        /// </summary>
        /// <param name="reporteRecibo"></param>
        private void ImprimeVendidoCobrado(Report reporteRecibo)
        {
            reporteRecibo.Print();
            if (reporteRecibo.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteRecibo.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        ImprimeVendidoCobrado(reporteRecibo);
                    }
                });
        } 

        //ABC Caso 35771
        /// <summary>
        /// Metodo encargado de definir como se va a ver el reporte una vez impreso
        /// en este metodo se indica la posicion de todo lo que vaya en el reporte 
        /// </summary>
        public void ImprimeDevuelto()
        {
            Report reporteRecibo = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteDevoluciones), Impresora.ObtenerDriver());

            reporteRecibo.AddObject(this);

            ImprimeDevuelto(reporteRecibo);
        }

        /// <summary>
        /// imprime y pregunta por reimpresión
        /// </summary>
        /// <param name="reporteRecibo"></param>
        private void ImprimeDevuelto(Report reporteRecibo)
        {
            reporteRecibo.Print();
            if (reporteRecibo.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteRecibo.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        ImprimeDevuelto(reporteRecibo);
                    }
                });
        }

        #endregion

		#region IPrintable Members

		public string GetObjectName()
		{			
			return "TRANSACCIONES";
		}

		public object GetField(string name)
		{
            switch (name)
            {
                //ABC 35771
                case "TOTAL_DEVUELTO": return this.MontoDevuelto;
                case "TOTAL_COBRADO": return this.MontoCobro;
                case "TOTAL_VENDIDO": return this.MontoVendido;
                case "TOTAL_PEDIDOS": return this.MontoPreventa;
                case "TOTAL_FACTURAS": return this.MontoFacturado;
                case "FECHA": return DateTime.Now;
                case "CLIENTES": return new ArrayList(this.clientes as ICollection);
                default: return string.Empty;
            }
		}

		#endregion
	}
}
