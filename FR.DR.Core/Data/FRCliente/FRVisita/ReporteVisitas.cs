using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita
{
	/// <summary>
	/// Lista de visitas que se van a imprimir.
	/// </summary>
    public class ReporteVisitas : IPrintable
    {
		#region Variables de instancia
        /// <summary>
        /// instancia del viewModel que invocó el reporte, necesaria para mostrar los mensajes/alertas
        /// </summary>
        private BaseViewModel viewModel;
        /// <summary>
        /// Visitas realizadas en ruta
        /// </summary>
        private ICollection<Visita> visitas;

		#endregion

		#region Constructor
        /// <summary>
        /// Constructor de la impresion del reporte
        /// </summary>
        /// <param name="visitas">visitas para la ruta</param>
        public ReporteVisitas(BaseViewModel viewModel, ICollection<Visita> visitas)
		{
            this.viewModel = viewModel;
			this.visitas = visitas;
		}

		#endregion

        #region Impresion de reporte de visita

        /// <summary>
        /// Metodo encargado de pintar el reporte de visitas 
        /// en este metodo se indica donde se va a imprimir cada 
        /// parte del reporte desde las tablas en la que se guarda la información
        /// hasta la misma información se le debe indicar el punto donde se 
        /// quiere que se imprima.
        /// </summary>
        public void Imprime()
        {
            Report reporte = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteVisita), Impresora.ObtenerDriver());

                reporte.AddObject(this);
                Imprime(reporte);
        }

        public void Imprime(Report reporte)
        {
            reporte.Print();
            if (reporte.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporte.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        Imprime(reporte);
                    }
                });
        }
        #endregion

		#region IPrintable Members

		public string GetObjectName()
		{			
			return "VISITAS";
		}

		public object GetField(string name)
		{
			if(name=="LISTA_VISITAS")
				return new ArrayList(this.visitas as ICollection);

			if (name=="FECHA")
				return DateTime.Now;

			return null;
		}

		#endregion
	}
}