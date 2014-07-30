using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    public class ReporteJornada : IPrintable 
    {

        private JornadaRuta jornada;

        #region Constructores

        public ReporteJornada()
        {        }

        /// <summary>
        /// Constructor definir clase de impresion Jornada
        /// </summary>
        /// <param name="ruta">Ruta a imprimir reporte</param>
        public ReporteJornada(JornadaRuta jornadaRuta)
        {
            jornada = jornadaRuta;
        }

        #endregion Constructores

        #region Metodos de Clase

        public void Imprime()
        {
            Report reporteJornada = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteJornada), Impresora.ObtenerDriver());

            reporteJornada.AddObject(this);
            //do
            {
                reporteJornada.Print();
                if (reporteJornada.ErrorLog != string.Empty)
                {
                    // TODO: hay que tener un ViewModel o View para poder desplegar mensajes
                    //Mensaje.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteJornada.ErrorLog);
                }

            } 
            // TODO: hay que tener un ViewModel o View para poder desplegar mensajes
            //while (Mensaje.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo") == System.Windows.Forms.DialogResult.Yes);
        }

        public void ImprimeCierreCaja()
        {
            Report reporteJornada = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteCierreCaja), Impresora.ObtenerDriver());

            reporteJornada.AddObject(this);
            //do
            {
                reporteJornada.Print();
                if (reporteJornada.ErrorLog != string.Empty)
                {
                    // TODO: hay que tener un ViewModel o View para poder desplegar mensajes
                    //Mensaje.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteJornada.ErrorLog);
                }

            }
            // TODO: hay que tener un ViewModel o View para poder desplegar mensajes
            //while (Mensaje.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo") == System.Windows.Forms.DialogResult.Yes);
        }

        #endregion Metodos de Clase

        #region IPrintable Members

        public object GetField(string name)
        {
            switch (name)
            {
                case JornadaRuta.RUTA: return jornada.Ruta1;
                case JornadaRuta.FECHA: return jornada.Fecha;
                case JornadaRuta.FECHA_INICIO: return jornada.FechaHoraInicio;

                case JornadaRuta.FECHA_FIN:
                    if (jornada.FechaHoraFin != null)
                        return jornada.FechaHoraFin;
                    else
                        return "**Jornada Abierta**";

                case JornadaRuta.PEDIDOS_LOCAL: return jornada.PedidosLocal ?? decimal.Zero;
                case JornadaRuta.PEDIDOS_DOLAR: return jornada.PedidosDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_PEDIDOS_LOCAL: return jornada.MontoPedidosLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_PEDIDOS_DOLAR: return jornada.MontoPedidosDolar ?? decimal.Zero;
                case JornadaRuta.FACTURAS_LOCAL: return jornada.FacturasLocal ?? decimal.Zero;
                case JornadaRuta.FACTURASTF_LOCAL: return jornada.FacturasTomaFisicaLocal ?? decimal.Zero;
                case JornadaRuta.FACTURAS_DOLAR: return jornada.FacturasDolar ?? decimal.Zero;
                case JornadaRuta.FACTURASTF_DOLAR: return jornada.FacturasTomaFisicaDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURAS_LOCAL: return jornada.MontoFacturasLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURASTF_LOCAL: return jornada.MontoFacturasTomaFisicaLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURAS_DOLAR: return jornada.MontoFacturasDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURASTF_DOLAR: return jornada.MontoFacturasTomaFisicaDolar ?? decimal.Zero;
                case JornadaRuta.DEVOLUCIONES_LOCAL: return jornada.DevolucionesLocal ?? decimal.Zero;
                case JornadaRuta.DEVOLUCIONES_DOLAR: return jornada.DevolucionesDolar ?? decimal.Zero;
                case JornadaRuta.DEVOLUCIONES_EFC_LOCAL: return jornada.DevolucionesEfectivoLocal ?? decimal.Zero;
                case JornadaRuta.DEVOLUCIONES_EFC_DOLAR: return jornada.DevolucionesEfectivoDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_DEVOLUCIONES_LOCAL: return jornada.MontoDevolucionesLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_DEVOLUCIONES_DOLAR: return jornada.MontoDevolucionesDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_DEVOLUCION_EFC_LOCAL: return jornada.MontoDevolucionesEfectivoLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_DEVOLUCION_EFC_DOLAR: return jornada.MontoDevolucionesEfectivoDolar ?? decimal.Zero;                
                case JornadaRuta.COBROS_LOCAL: return jornada.CobrosLocal ?? decimal.Zero;
                case JornadaRuta.COBROS_DOLAR: return jornada.CobrosDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_CHEQUE_LOCAL: return jornada.MontoCobrosChequeLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_CHEQUE_DOLAR: return jornada.MontoCobrosChequeDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_EFECTIVO_LOCAL: return (jornada.MontoCobrosEfectivoLocal - jornada.MontoDevolucionesEfectivoLocal) ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_EFECTIVO_DOLAR: return (jornada.MontoCobrosEfectivoDolar - jornada.MontoDevolucionesEfectivoDolar) ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_NOTA_CREDITO_LOCAL: return jornada.MontoCobrosNotaCreditoLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_NOTA_CREDITO_DOLAR: return jornada.MontoCobrosNotaCreditoDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_LOCAL: return jornada.MontoCobrosLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_DOLAR: return jornada.MontoCobrosDolar ?? decimal.Zero;
                case JornadaRuta.DEPOSITOS_LOCAL: return jornada.DepositosLocal ?? decimal.Zero;
                case JornadaRuta.DEPOSITOS_DOLAR: return jornada.DepositosDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_DEPOSITOS_LOCAL: return jornada.MontoDepositosLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_DEPOSITOS_DOLAR: return jornada.MontoDepositosDolar ?? decimal.Zero;
                case JornadaRuta.GARANTIAS_LOCAL: return jornada.GarantiasLocal ?? decimal.Zero;
                case JornadaRuta.GARANTIAS_DOLAR: return jornada.GarantiasDolar ?? decimal.Zero;
                case JornadaRuta.MONTO_GARANTIAS_LOCAL: return jornada.MontoGarantiasLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_GARANTIAS_DOLAR: return jornada.MontoGarantiasDolar ?? decimal.Zero;

                //Proyecto Gas Z - Cierre Caja

                case JornadaRuta.MONTO_FACTURAS_LOCAL_CRE: return jornada.MontoFacturasLocalCre ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURAS_DOLAR_CRE: return jornada.MontoFacturasDolarCre ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURASTF_LOCAL_CRE: return jornada.MontoFacturasTomaFisicaLocalCre ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURASTF_DOLAR_CRE: return jornada.MontoFacturasTomaFisicaDolarCre ?? decimal.Zero;
                case JornadaRuta.MONTO_GARANTIAS_LOCAL_CRE: return jornada.MontoGarantiasLocalCre ?? decimal.Zero;
                case JornadaRuta.MONTO_GARANTIAS_DOLAR_CRE: return jornada.MontoGarantiasDolarCre ?? decimal.Zero;

                case JornadaRuta.MONTO_FACTURAS_LOCAL_CONT: return jornada.MontoFacturasLocalCont ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURAS_DOLAR_CONT: return jornada.MontoFacturasDolarCont ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURASTF_LOCAL_CONT: return jornada.MontoFacturasTomaFisicaLocalCont ?? decimal.Zero;
                case JornadaRuta.MONTO_FACTURASTF_DOLAR_CONT: return jornada.MontoFacturasTomaFisicaDolarCont ?? decimal.Zero;
                case JornadaRuta.MONTO_GARANTIAS_LOCAL_CONT: return jornada.MontoGarantiasLocalCont ?? decimal.Zero;
                case JornadaRuta.MONTO_GARANTIAS_DOLAR_CONT: return jornada.MontoGarantiasDolarCont ?? decimal.Zero;

                case JornadaRuta.MONTO_CONTADO_TOTAL_LOCAL: return jornada.MontoFacturasLocalCont + jornada.MontoFacturasTomaFisicaLocalCont + jornada.MontoGarantiasLocalCont ?? decimal.Zero;
                case JornadaRuta.MONTO_CONTADO_TOTAL_DOLAR: return jornada.MontoFacturasDolarCont + jornada.MontoFacturasTomaFisicaDolarCont + jornada.MontoGarantiasDolarCont ?? decimal.Zero;

                case JornadaRuta.MONTO_CREDITO_TOTAL_LOCAL: return jornada.MontoFacturasLocalCre + jornada.MontoFacturasTomaFisicaLocalCre + jornada.MontoGarantiasLocalCre ?? decimal.Zero;
                case JornadaRuta.MONTO_CREDITO_TOTAL_DOLAR: return jornada.MontoFacturasDolarCre + jornada.MontoFacturasTomaFisicaDolarCre + jornada.MontoGarantiasDolarCre ?? decimal.Zero;

                case JornadaRuta.MONTO_COBROS_TOTAL_LOCAL: return jornada.MontoCobrosLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_COBROS_TOTAL_DOLAR: return jornada.MontoCobrosDolar ?? decimal.Zero;

                case JornadaRuta.MONTO_DIFERENCIA_TOTAL_LOCAL: return jornada.MontoCobrosLocal - (jornada.MontoFacturasLocalCont + jornada.MontoFacturasTomaFisicaLocalCont + jornada.MontoGarantiasLocalCont)-jornada.MontoDepositosLocal-jornada.MontoDevolucionesEfectivoLocal ?? decimal.Zero;
                case JornadaRuta.MONTO_DIFERENCIA_TOTAL_DOLAR: return jornada.MontoCobrosDolar - (jornada.MontoFacturasDolarCont + jornada.MontoFacturasTomaFisicaDolarCont + jornada.MontoGarantiasDolarCont) - jornada.MontoDepositosDolar - jornada.MontoDevolucionesEfectivoDolar ?? decimal.Zero;





                default: return null;
            }
        }

        public string GetObjectName()
        {
            return "JORNADAS";
        }

        #endregion IPrintable Members
    }
}
