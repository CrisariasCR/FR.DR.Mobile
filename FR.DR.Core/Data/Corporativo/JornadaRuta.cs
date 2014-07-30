using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    public class JornadaRuta
    {
        #region Constantes
        public const string RUTA = "RUTA";
        public const string FECHA = "FECHA";
        public const string FECHA_INICIO = "FECHA_HORA_INICIO";
        public const string FECHA_FIN = "FECHA_HORA_FIN";
        public const string PEDIDOS_LOCAL = "PEDIDOS_LOCAL";
        public const string PEDIDOS_DOLAR = "PEDIDOS_DOLAR";
        public const string MONTO_PEDIDOS_LOCAL = "MONTO_PEDIDOS_LOCAL";
        public const string MONTO_PEDIDOS_DOLAR = "MONTO_PEDIDOS_DOLAR";
        public const string FACTURAS_LOCAL = "FACTURAS_LOCAL";
        public const string FACTURAS_LOCAL_CRE = "FACTURAS_LOCAL_CRE";
        public const string FACTURAS_LOCAL_CONT = "FACTURAS_LOCAL_CRE";
        public const string GARANTIAS_LOCAL = "GARANTIAS_LOCAL";
        public const string GARANTIAS_LOCAL_CRE = "GARANTIAS_LOCAL_CRE";
        public const string GARANTIAS_LOCAL_CONT = "GARANTIAS_LOCAL_CRE";
        public const string FACTURAS_DOLAR = "FACTURAS_DOLAR";
        public const string FACTURAS_DOLAR_CRE = "FACTURAS_DOLAR_CRE";
        public const string FACTURAS_DOLAR_CONT = "FACTURAS_DOLAR_CRE";
        public const string GARANTIAS_DOLAR = "GARANTIAS_DOLAR";
        public const string GARANTIAS_DOLAR_CRE = "GARANTIAS_DOLAR_CRE";
        public const string GARANTIAS_DOLAR_CONT = "GARANTIAS_DOLAR_CRE";
        public const string MONTO_FACTURAS_LOCAL = "MONTO_FACTURAS_LOCAL";
        public const string MONTO_FACTURAS_LOCAL_CRE = "MONTO_FACTURAS_LOCAL_CRE";
        public const string MONTO_FACTURAS_LOCAL_CONT = "MONTO_FACTURAS_LOCAL_CONT";
        public const string MONTO_GARANTIAS_LOCAL = "MONTO_GARANTIAS_LOCAL";
        public const string MONTO_GARANTIAS_LOCAL_CRE = "MONTO_GARANTIAS_LOCAL_CRE";
        public const string MONTO_GARANTIAS_LOCAL_CONT = "MONTO_GARANTIAS_LOCAL_CONT";
        public const string MONTO_FACTURAS_DOLAR = "MONTO_FACTURAS_DOLAR";
        public const string MONTO_FACTURAS_DOLAR_CRE = "MONTO_FACTURAS_DOLAR_CRE";
        public const string MONTO_FACTURAS_DOLAR_CONT = "MONTO_FACTURAS_DOLAR_CONT";
        public const string MONTO_GARANTIAS_DOLAR = "MONTO_GARANTIAS_DOLAR";
        public const string MONTO_GARANTIAS_DOLAR_CRE = "MONTO_GARANTIAS_DOLAR_CRE";
        public const string MONTO_GARANTIAS_DOLAR_CONT = "MONTO_GARANTIAS_DOLAR_CONT";
        public const string FACTURASTF_LOCAL = "FACTURASTF_LOCAL";
        public const string FACTURASTF_LOCAL_CRE = "FACTURASTF_LOCAL_CRE";
        public const string FACTURASTF_LOCAL_CONT = "FACTURASTF_LOCAL_CRE";
        public const string FACTURASTF_DOLAR = "FACTURASTF_DOLAR";
        public const string FACTURASTF_DOLAR_CRE = "FACTURASTF_DOLAR_CRE";
        public const string FACTURASTF_DOLAR_CONT = "FACTURASTF_DOLAR_CONT";
        public const string MONTO_FACTURASTF_LOCAL = "MONTO_FACTURASTF_LOCAL";
        public const string MONTO_FACTURASTF_LOCAL_CRE = "MONTO_FACTURASTF_LOCAL_CRE";
        public const string MONTO_FACTURASTF_LOCAL_CONT = "MONTO_FACTURASTF_LOCAL_CONT";
        public const string MONTO_FACTURASTF_DOLAR = "MONTO_FACTURASTF_DOLAR";
        public const string MONTO_FACTURASTF_DOLAR_CRE = "MONTO_FACTURASTF_DOLAR_CRE";
        public const string MONTO_FACTURASTF_DOLAR_CONT = "MONTO_FACTURASTF_DOLAR_CONT";
        public const string DEVOLUCIONES_LOCAL = "DEVOLUCIONES_LOCAL";
        public const string DEVOLUCIONES_DOLAR = "DEVOLUCIONES_DOLAR";
        public const string MONTO_DEVOLUCIONES_LOCAL = "MONTO_DEVOLUCIONES_LOCAL";
        public const string MONTO_DEVOLUCIONES_DOLAR = "MONTO_DEVOLUCIONES_DOLAR";
        public const string COBROS_LOCAL = "COBROS_LOCAL";
        public const string COBROS_DOLAR = "COBROS_DOLAR";
        public const string MONTO_COBROS_LOCAL = "MONTO_COBROS_LOCAL";
        public const string MONTO_COBROS_DOLAR = "MONTO_COBROS_DOLAR";
        public const string MONTO_COBROS_EFECTIVO_LOCAL = "MONTO_COBROS_EFC_LOCAL";
        public const string MONTO_COBROS_EFECTIVO_DOLAR = "MONTO_COBROS_EFC_DOLAR";
        public const string MONTO_COBROS_CHEQUE_LOCAL = "MONTO_COBROS_CHQ_LOCAL";
        public const string MONTO_COBROS_CHEQUE_DOLAR = "MONTO_COBROS_CHQ_DOLAR";
        public const string MONTO_COBROS_NOTA_CREDITO_LOCAL = "MONTO_COBROS_NC_LOCAL";
        public const string MONTO_COBROS_NOTA_CREDITO_DOLAR = "MONTO_COBROS_NC_DOLAR";
        public const string DEPOSITOS_LOCAL = "DEPOSITOS_LOCAL";
        public const string DEPOSITOS_DOLAR = "DEPOSITOS_DOLAR";
        public const string MONTO_DEPOSITOS_LOCAL = "MONTO_DEPOSITOS_LOCAL";
        public const string MONTO_DEPOSITOS_DOLAR = "MONTO_DEPOSITOS_DOLAR";
        

        public const string DEVOLUCIONES_EFC_LOCAL = "DEVOLUCIONES_EFC_LOCAL";
        public const string DEVOLUCIONES_EFC_DOLAR = "DEVOLUCIONES_EFC_DOLAR";

        public const string MONTO_DEVOLUCION_EFC_LOCAL = "MONTO_DEVOLUCION_EFC_LOCAL";
        public const string MONTO_DEVOLUCION_EFC_DOLAR = "MONTO_DEVOLUCION_EFC_DOLAR";

        public const string MONTO_COBROS_TOTAL_LOCAL = "MONTO_COBROS_TOTAL_LOCAL";
        public const string MONTO_CONTADO_TOTAL_LOCAL = "MONTO_CONTADO_TOTAL_LOCAL";
        public const string MONTO_CREDITO_TOTAL_LOCAL = "MONTO_CREDITO_TOTAL_LOCAL";

        public const string MONTO_COBROS_TOTAL_DOLAR = "MONTO_COBROS_TOTAL_DOLAR";
        public const string MONTO_CONTADO_TOTAL_DOLAR = "MONTO_CONTADO_TOTAL_DOLAR";
        public const string MONTO_CREDITO_TOTAL_DOLAR = "MONTO_CREDITO_TOTAL_DOLAR";

        public const string MONTO_DIFERENCIA_TOTAL_LOCAL = "MONTO_DIFERENCIA_TOTAL_LOCAL";
        public const string MONTO_DIFERENCIA_TOTAL_DOLAR = "MONTO_DIFERENCIA_TOTAL_DOLAR";

        public const string SINCRONIZADO = "SINCRONIZADO";

        public const string JORNADA_RUTAS_SERVIDOR = "ERPADMIN.JORNADA_RUTAS";

        /// <summary>
        /// Constante para separar las sentencias de un conjunto de sentencias a ejecutar.
        /// </summary>
        private const string SEPARADOR = "#|#";

        #endregion
        #region Atributos
        private string ruta;
        private DateTime fecha;   
        private DateTime? fechaHoraInicio;
        private DateTime? fechaHoraFin;
        private decimal? pedidosLocal;
        private decimal? montoPedidosLocal;
        private decimal? facturasLocal;
        private decimal? facturasLocalCre;
        private decimal? facturasLocalCont;
        private decimal? garantiasLocal;
        private decimal? garantiasLocalCre;
        private decimal? garantiasLocalCont;
        private decimal? facturasTomaFisicaLocal;
        private decimal? facturasTomaFisicaLocalCre;
        private decimal? facturasTomaFisicaLocalCont;
        private decimal? montoFacturasLocal;
        private decimal? montoFacturasLocalCre;
        private decimal? montoFacturasLocalCont;
        private decimal? montoGarantiasLocal;
        private decimal? montoGarantiasLocalCre;
        private decimal? montoGarantiasLocalCont;
        private decimal? montoFacturasTomaFisicaLocal;
        private decimal? montoFacturasTomaFisicaLocalCre;
        private decimal? montoFacturasTomaFisicaLocalCont;
        private decimal? devolucionesLocal;
        private decimal? montoDevolucionesLocal;
        private decimal? cobrosLocal;
        private decimal? montoCobrosLocal;
        private decimal? montoCobrosChequeLocal;
        private decimal? montoCobrosEfectivoLocal;
        private decimal? montoCobrosNotaCreditoLocal;
        private decimal? depositosLocal;
        private decimal? montoDepositosLocal;
        private decimal? pedidosDolar;
        private decimal? montoPedidosDolar;
        private decimal? facturasDolar;
        private decimal? facturasDolarCre;
        private decimal? facturasDolarCont;
        private decimal? garantiasDolar;
        private decimal? garantiasDolarCre;
        private decimal? garantiasDolarCont;
        private decimal? facturasTomaFisicaDolar;
        private decimal? facturasTomaFisicaDolarCre;
        private decimal? facturasTomaFisicaDolarCont;
        private decimal? montoFacturasDolar;
        private decimal? montoFacturasDolarCre;
        private decimal? montoFacturasDolarCont;
        private decimal? montoGarantiasDolar;
        private decimal? montoGarantiasDolarCre;
        private decimal? montoGarantiasDolarCont;
        private decimal? montoFacturasTomaFisicaDolar;
        private decimal? montoFacturasTomaFisicaDolarCre;
        private decimal? montoFacturasTomaFisicaDolarCont;
        private decimal? devolucionesDolar;
        private decimal? montoDevolucionesDolar;
        private decimal? cobrosDolar;
        private decimal? montoCobrosDolar;
        private decimal? montoCobrosChequeDolar;
        private decimal? montoCobrosEfectivoDolar;
        private decimal? montoCobrosNotaCreditoDolar;
        private decimal? depositosDolar;
        private decimal? montoDepositosDolar;
        private bool sincronizado;

        #region  Facturas de contado y recibos en FR - KFC

        private decimal? devolucionesEfectivoLocal;
        private decimal? devolucionesEfectivoDolar;
        private decimal? montoDevolucionesEfectivoLocal;
        private decimal? montoDevolucionesEfectivoDolar;

        #endregion

        /// <summary>
        /// Arreglo de columnas para los montos y cantidades.
        /// </summary>
        private static String[] columnas = new String[]{
            PEDIDOS_LOCAL, MONTO_PEDIDOS_LOCAL
            , FACTURAS_LOCAL, MONTO_FACTURAS_LOCAL
            , DEVOLUCIONES_LOCAL, MONTO_DEVOLUCIONES_LOCAL
            , COBROS_LOCAL , MONTO_COBROS_LOCAL
            , MONTO_COBROS_CHEQUE_LOCAL, MONTO_COBROS_EFECTIVO_LOCAL , MONTO_COBROS_NOTA_CREDITO_LOCAL
            , DEPOSITOS_LOCAL, MONTO_DEPOSITOS_LOCAL
            , PEDIDOS_DOLAR, MONTO_PEDIDOS_DOLAR
            , FACTURAS_DOLAR, MONTO_FACTURAS_DOLAR
            , DEVOLUCIONES_DOLAR, MONTO_DEVOLUCIONES_DOLAR
            , COBROS_DOLAR , MONTO_COBROS_DOLAR
            , MONTO_COBROS_CHEQUE_DOLAR, MONTO_COBROS_EFECTIVO_DOLAR , MONTO_COBROS_NOTA_CREDITO_DOLAR
            , DEPOSITOS_DOLAR
            , MONTO_DEPOSITOS_DOLAR
            , MONTO_DEVOLUCION_EFC_DOLAR, MONTO_DEVOLUCION_EFC_LOCAL
            , DEVOLUCIONES_EFC_DOLAR, DEVOLUCIONES_EFC_LOCAL,GARANTIAS_LOCAL,MONTO_GARANTIAS_LOCAL,GARANTIAS_DOLAR,MONTO_GARANTIAS_DOLAR,
            FACTURASTF_LOCAL,MONTO_FACTURASTF_LOCAL,FACTURASTF_DOLAR,MONTO_FACTURASTF_DOLAR,
            FACTURAS_LOCAL_CRE,MONTO_FACTURAS_LOCAL_CRE,FACTURAS_DOLAR_CRE,MONTO_FACTURAS_DOLAR_CRE,
            GARANTIAS_LOCAL_CRE,MONTO_GARANTIAS_LOCAL_CRE,GARANTIAS_DOLAR_CRE,MONTO_GARANTIAS_DOLAR_CRE,
            FACTURASTF_LOCAL_CRE,MONTO_FACTURASTF_LOCAL_CRE,FACTURASTF_DOLAR_CRE,MONTO_FACTURASTF_DOLAR_CRE,
            FACTURAS_LOCAL_CONT,MONTO_FACTURAS_LOCAL_CONT,FACTURAS_DOLAR_CONT,MONTO_FACTURAS_DOLAR_CONT,
            GARANTIAS_LOCAL_CONT,MONTO_GARANTIAS_LOCAL_CONT,GARANTIAS_DOLAR_CONT,MONTO_GARANTIAS_DOLAR_CONT,
            FACTURASTF_LOCAL_CONT,MONTO_FACTURASTF_LOCAL_CONT,FACTURASTF_DOLAR_CONT,MONTO_FACTURASTF_DOLAR_CONT
        };
        #endregion
        #region Propiedades
        public string Ruta1
        {
            get { return ruta; }
            set { ruta = value; }
        }
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
        public DateTime? FechaHoraInicio
        {
            get { return fechaHoraInicio; }
            set { fechaHoraInicio = value; }
        }
        public DateTime? FechaHoraFin
        {
            get { return fechaHoraFin; }
            set { fechaHoraFin = value; }
        }
        //
        public decimal? PedidosLocal
        {
            get { return pedidosLocal; }
            set { pedidosLocal = value; }
        }
        public decimal? MontoPedidosLocal
        {
            get { return montoPedidosLocal; }
            set { montoPedidosLocal = value; }
        }
        public decimal? FacturasLocal
        {
            get { return facturasLocal; }
            set { facturasLocal = value; }
        }
        public decimal? FacturasTomaFisicaLocal
        {
            get { return facturasTomaFisicaLocal; }
            set { facturasTomaFisicaLocal = value; }
        }
        public decimal? GarantiasLocal
        {
            get { return garantiasLocal; }
            set { garantiasLocal = value; }
        }
        public decimal? MontoFacturasLocal
        {
            get { return montoFacturasLocal; }
            set { montoFacturasLocal = value; }
        }
        public decimal? MontoFacturasTomaFisicaLocal
        {
            get { return montoFacturasTomaFisicaLocal; }
            set { montoFacturasTomaFisicaLocal = value; }
        }
        public decimal? MontoGarantiasLocal
        {
            get { return montoGarantiasLocal; }
            set { montoGarantiasLocal = value; }
        }
        public decimal? FacturasLocalCre
        {
            get { return facturasLocalCre; }
            set { facturasLocalCre = value; }
        }
        public decimal? FacturasTomaFisicaLocalCre
        {
            get { return facturasTomaFisicaLocalCre; }
            set { facturasTomaFisicaLocalCre = value; }
        }
        public decimal? GarantiasLocalCre
        {
            get { return garantiasLocalCre; }
            set { garantiasLocalCre = value; }
        }
        public decimal? MontoFacturasLocalCre
        {
            get { return montoFacturasLocalCre; }
            set { montoFacturasLocalCre = value; }
        }
        public decimal? MontoFacturasTomaFisicaLocalCre
        {
            get { return montoFacturasTomaFisicaLocalCre; }
            set { montoFacturasTomaFisicaLocalCre = value; }
        }
        public decimal? MontoGarantiasLocalCre
        {
            get { return montoGarantiasLocalCre; }
            set { montoGarantiasLocalCre = value; }
        }
        public decimal? FacturasLocalCont
        {
            get { return facturasLocalCont; }
            set { facturasLocalCont = value; }
        }
        public decimal? FacturasTomaFisicaLocalCont
        {
            get { return facturasTomaFisicaLocalCont; }
            set { facturasTomaFisicaLocalCont = value; }
        }
        public decimal? GarantiasLocalCont
        {
            get { return garantiasLocalCont; }
            set { garantiasLocalCont = value; }
        }
        public decimal? MontoFacturasLocalCont
        {
            get { return montoFacturasLocalCont; }
            set { montoFacturasLocalCont = value; }
        }
        public decimal? MontoFacturasTomaFisicaLocalCont
        {
            get { return montoFacturasTomaFisicaLocalCont; }
            set { montoFacturasTomaFisicaLocalCont = value; }
        }
        public decimal? MontoGarantiasLocalCont
        {
            get { return montoGarantiasLocalCont; }
            set { montoGarantiasLocalCont = value; }
        }
        public decimal? DevolucionesLocal
        {
            get { return devolucionesLocal; }
            set { devolucionesLocal = value; }
        }
        public decimal? MontoDevolucionesLocal
        {
            get { return montoDevolucionesLocal; }
            set { montoDevolucionesLocal = value; }
        }
        public decimal? CobrosLocal
        {
            get { return cobrosLocal; }
            set { cobrosLocal = value; }
        }

        public decimal? MontoCobrosChequeLocal
        {
            get { return montoCobrosChequeLocal; }
            set { montoCobrosChequeLocal = value; }
        }
        public decimal? MontoCobrosEfectivoLocal
        {
            get { return montoCobrosEfectivoLocal; }
            set { montoCobrosEfectivoLocal = value; }
        }
        public decimal? MontoCobrosNotaCreditoLocal
        {
            get { return montoCobrosNotaCreditoLocal; }
            set { montoCobrosNotaCreditoLocal = value; }
        }
        public decimal? DepositosLocal
        {
            get { return depositosLocal; }
            set { depositosLocal = value; }
        }
        public decimal? MontoDepositosLocal
        {
            get { return montoDepositosLocal; }
            set { montoDepositosLocal = value; }
        }
        public decimal? MontoCobrosLocal
        {
            get { return montoCobrosLocal; }
            set { montoCobrosLocal = value; }
        }
        //
        public decimal? PedidosDolar
        {
            get { return pedidosDolar; }
            set { pedidosDolar = value; }
        }
        public decimal? MontoPedidosDolar
        {
            get { return montoPedidosDolar; }
            set { montoPedidosDolar = value; }
        }
        public decimal? FacturasDolar
        {
            get { return facturasDolar; }
            set { facturasDolar = value; }
        }
        public decimal? FacturasTomaFisicaDolar
        {
            get { return facturasTomaFisicaDolar; }
            set { facturasTomaFisicaDolar = value; }
        }
        public decimal? GarantiasDolar
        {
            get { return garantiasDolar; }
            set { garantiasDolar = value; }
        }
        public decimal? MontoFacturasDolar
        {
            get { return montoFacturasDolar; }
            set { montoFacturasDolar = value; }
        }
        public decimal? MontoFacturasTomaFisicaDolar
        {
            get { return montoFacturasTomaFisicaDolar; }
            set { montoFacturasTomaFisicaDolar = value; }
        }
        public decimal? MontoGarantiasDolar
        {
            get { return montoGarantiasDolar; }
            set { montoGarantiasDolar = value; }
        }
        public decimal? FacturasDolarCre
        {
            get { return facturasDolarCre; }
            set { facturasDolarCre = value; }
        }
        public decimal? FacturasTomaFisicaDolarCre
        {
            get { return facturasTomaFisicaDolarCre; }
            set { facturasTomaFisicaDolarCre = value; }
        }
        public decimal? GarantiasDolarCre
        {
            get { return garantiasDolarCre; }
            set { garantiasDolarCre = value; }
        }
        public decimal? MontoFacturasDolarCre
        {
            get { return montoFacturasDolarCre; }
            set { montoFacturasDolarCre = value; }
        }
        public decimal? MontoFacturasTomaFisicaDolarCre
        {
            get { return montoFacturasTomaFisicaDolarCre; }
            set { montoFacturasTomaFisicaDolarCre = value; }
        }
        public decimal? MontoGarantiasDolarCre
        {
            get { return montoGarantiasDolarCre; }
            set { montoGarantiasDolarCre = value; }
        }
        public decimal? FacturasDolarCont
        {
            get { return facturasDolarCont; }
            set { facturasDolarCont = value; }
        }
        public decimal? FacturasTomaFisicaDolarCont
        {
            get { return facturasTomaFisicaDolarCont; }
            set { facturasTomaFisicaDolarCont = value; }
        }
        public decimal? GarantiasDolarCont
        {
            get { return garantiasDolarCont; }
            set { garantiasDolarCont = value; }
        }
        public decimal? MontoFacturasDolarCont
        {
            get { return montoFacturasDolarCont; }
            set { montoFacturasDolarCont = value; }
        }
        public decimal? MontoFacturasTomaFisicaDolarCont
        {
            get { return montoFacturasTomaFisicaDolarCont; }
            set { montoFacturasTomaFisicaDolarCont = value; }
        }
        public decimal? MontoGarantiasDolarCont
        {
            get { return montoGarantiasDolarCont; }
            set { montoGarantiasDolarCont = value; }
        }
        public decimal? DevolucionesDolar
        {
            get { return devolucionesDolar; }
            set { devolucionesDolar = value; }
        }
        public decimal? MontoDevolucionesDolar
        {
            get { return montoDevolucionesDolar; }
            set { montoDevolucionesDolar = value; }
        }
        public decimal? CobrosDolar
        {
            get { return cobrosDolar; }
            set { cobrosDolar = value; }
        }

        public decimal? MontoCobrosChequeDolar
        {
            get { return montoCobrosChequeDolar; }
            set { montoCobrosChequeDolar = value; }
        }
        public decimal? MontoCobrosEfectivoDolar
        {
            get { return montoCobrosEfectivoDolar; }
            set { montoCobrosEfectivoDolar = value; }
        }
        public decimal? MontoCobrosNotaCreditoDolar
        {
            get { return montoCobrosNotaCreditoDolar; }
            set { montoCobrosNotaCreditoDolar = value; }
        }
        public decimal? DepositosDolar
        {
            get { return depositosDolar; }
            set { depositosDolar = value; }
        }
        public decimal? MontoDepositosDolar
        {
            get { return montoDepositosDolar; }
            set { montoDepositosDolar = value; }
        }
        public decimal? MontoCobrosDolar
        {
            get { return montoCobrosDolar; }
            set { montoCobrosDolar = value; }
        }
        public bool Sincronizado
        {
            get { return sincronizado; }
            set { sincronizado = value; }
        }


        #region  Facturas de contado y recibos en FR - KFC

        public decimal? DevolucionesEfectivoLocal
        {
            get { return devolucionesEfectivoLocal; }
            set { devolucionesEfectivoLocal = value; }
        }
        public decimal? DevolucionesEfectivoDolar
        {
            get { return devolucionesEfectivoDolar; }
            set { devolucionesEfectivoDolar = value; }
        }
        public decimal? MontoDevolucionesEfectivoLocal
        {
            get { return montoDevolucionesEfectivoLocal; }
            set { montoDevolucionesEfectivoLocal = value; }
        }
        public decimal? MontoDevolucionesEfectivoDolar
        {
            get { return montoDevolucionesEfectivoDolar; }
            set { montoDevolucionesEfectivoDolar = value; }
        }
        #endregion

        
        #endregion
        #region Constructores
        public JornadaRuta()
        {
        }
        public JornadaRuta(string ruta, DateTime fecha)
        {
            this.ruta = ruta;
            this.fecha = fecha;
        }
        
        #endregion
        #region Metodos
        /// <summary>
        /// Abre la distintas jornadas según las rutas asocidads al dispositivo
        /// </summary>
        /// <returns></returns>
        public static bool AbrirJornada()
        {
            bool resultado = false;
            try
            {
                Purgar();
                List<Ruta> rutas = Ruta.ObtenerRutas();
                foreach (Ruta ruta in rutas)
                {
                    AbrirJornada(ruta.Codigo, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultado;
        }

        /// <summary>
        /// Abre la joranada de trabajo, asigna los valores de las columnas
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="fechaInicio"></param>
        /// <returns></returns>
        private static bool AbrirJornada(string ruta, DateTime fechaInicio)
        {
            bool resultado = false;

            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();

            sentencia.AppendLine(String.Format(" INSERT INTO {0} ({1}, {2}, {3}) VALUES (@RUTA, date('now','localtime'), @FECHA_INICIO) "
                , Table.ERPADMIN_JORNADA_RUTAS, RUTA, FECHA , FECHA_INICIO ));

            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@RUTA", ruta)
                //, new SQLiteParameter("@FECHA", fechaInicio.Date)
                , new SQLiteParameter("@FECHA_INICIO", fechaInicio)});
            try
            {
                int registrosAfectados = GestorDatos.EjecutarComando(sentencia.ToString(), parametros);
                resultado = registrosAfectados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se puede crear la jornada para la ruta '{0}' y fecha '{1}'.  {2}", ruta, fechaInicio, ex.Message));
            }

            return resultado;
        }

        /// <summary>
        /// Cierra las distintas jornadas según las rutas asocidads al dispositivo
        /// </summary>
        /// <returns></returns>
        public static bool CerrarJornada()
        {
            bool resultado = false;
            try
            {
                List<Ruta> rutas = Ruta.ObtenerRutas();
                foreach (Ruta ruta in rutas)
                {
                    CerrarJornada(ruta.Codigo, DateTime.Now.Date, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultado;
        }

        /// <summary>
        /// Cierre la jornada en la fecha dada.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="fechaCierre"></param>
        /// <returns></returns>
        private static bool CerrarJornada(string ruta, DateTime fecha, DateTime fechaCierre)
        {
            bool resultado = false;

            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();

            sentencia.AppendLine(String.Format(" UPDATE {0} SET {1} = @FECHA_FIN"
                , Table.ERPADMIN_JORNADA_RUTAS, FECHA_FIN));
            sentencia.AppendLine(" WHERE RUTA = @RUTA AND julianday(date(FECHA)) = julianday(date('now','localtime')) ");

            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@RUTA", ruta)
                //, new SQLiteParameter("@FECHA", fecha.Date)
                , new SQLiteParameter("@FECHA_FIN", fechaCierre)});
            try
            {
                int registrosAfectados = GestorDatos.EjecutarComando(sentencia.ToString(), parametros);
                resultado = registrosAfectados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se puede crear la jornada para la ruta '{0}' y fecha '{1}'.  {2}", ruta, fecha, ex.Message));
            }

            return resultado;
        }

        /// <summary>
        /// Actualiza el valor de la columna para el registro de la jornada.
        /// </summary>
        /// <param name="columna"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static bool ActualizarRegistro(string ruta, string columna, decimal valor)
        {
            bool resultado = false;
            
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();

            sentencia.AppendLine(String.Format(" UPDATE {0} SET {1} = {1} + @VALOR", Table.ERPADMIN_JORNADA_RUTAS, columna, valor));
            sentencia.AppendLine(" WHERE RUTA = @RUTA AND julianday(date(FECHA)) = julianday(date('now','localtime')) ");

            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@RUTA", ruta),new SQLiteParameter("@VALOR", valor)
                // mvega: se usa la funcion date('now','localtime') de SQLite
                //, new SQLiteParameter("@FECHA", DateTime.Now.Date)
            });
            try
            {
                int registrosAfectados = GestorDatos.EjecutarComando(sentencia.ToString(), parametros);
                resultado = registrosAfectados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se puede actualizar el valor de la columna {0} de la jornada para la ruta '{1}' para la fecha '{2}' . {3}", columna, ruta, DateTime.Now.Date, ex.Message));
            }

            return resultado;
        }

        #region codigo comentado pues no se utiliza
        ///// <summary>
        ///// Obtiene las jornadas de una ruta.
        ///// </summary>
        ///// <param name="ruta"></param>
        ///// <returns></returns>
        //public static List<JornadaRuta> ObtenerJornadas(string ruta)
        //{
        //    List<JornadaRuta> resultado = new List<JornadaRuta>();
        //    SQLiteParameterList parametros;
        //    StringBuilder sentencia = new StringBuilder();



        //    sentencia.AppendLine(" SELECT RUTA,FECHA,FECHA_HORA_INICIO,FECHA_HORA_FIN,PEDIDOS_LOCAL,MONTO_PEDIDOS_LOCAL,FACTURAS_LOCAL,MONTO_FACTURAS_LOCAL,DEVOLUCIONES_LOCAL,MONTO_DEVOLUCIONES_LOCAL,COBROS_LOCAL,MONTO_COBROS_LOCAL,MONTO_COBROS_CHQ_LOCAL,MONTO_COBROS_EFC_LOCAL,MONTO_COBROS_NC_LOCAL,DEPOSITOS_LOCAL,MONTO_DEPOSITOS_LOCAL ");
        //    sentencia.AppendLine(" ,PEDIDOS_DOLAR,MONTO_PEDIDOS_DOLAR,FACTURAS_DOLAR,MONTO_FACTURAS_DOLAR,DEVOLUCIONES_DOLAR,MONTO_DEVOLUCIONES_DOLAR,COBROS_DOLAR,MONTO_COBROS_DOLAR,MONTO_COBROS_CHQ_DOLAR,MONTO_COBROS_EFC_DOLAR,MONTO_COBROS_NC_DOLAR,DEPOSITOS_DOLAR,MONTO_DEPOSITOS_DOLAR ");
        //    sentencia.AppendLine(" ,SINCRONIZADO ");
        //    sentencia.AppendLine(", MONTO_DEVOLUCION_EFC_LOCAL,MONTO_DEVOLUCION_EFC_DOLAR,DEVOLUCIONES_EFC_LOCAL,DEVOLUCIONES_EFC_DOLAR ");
        //    sentencia.AppendLine(String.Format(" FROM {0}", Table.ERPADMIN_JORNADA_RUTAS));
        //    sentencia.AppendLine(" WHERE RUTA = @RUTA");
            
        //    parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@RUTA", ruta) });

        //    SQLiteDataReader reader = null;

        //    try
        //    {
        //        reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

        //        while (reader.Read())
        //        {
        //            JornadaRuta jornada = ObtenerJornada(reader);

        //            resultado.Add(jornada);
        //        }

        //        return resultado;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(String.Format("No se puede obtener las jornadas para la ruta '{0}'. {1}", ruta, ex.Message));
        //    }
        //    finally
        //    {
        //        if (reader != null)
        //            reader.Close();
        //    }

        //}
        #endregion codigo comentado pues no se utiliza

        /// <summary>
        /// Obtiene la jornada para la ruta y la fecha dada.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public static JornadaRuta ObtenerJornada(string ruta, DateTime fecha)
        {
            JornadaRuta resultado = null;
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();

            sentencia.AppendLine(" SELECT RUTA,FECHA,FECHA_HORA_INICIO,FECHA_HORA_FIN,PEDIDOS_LOCAL,MONTO_PEDIDOS_LOCAL,FACTURAS_LOCAL,MONTO_FACTURAS_LOCAL,DEVOLUCIONES_LOCAL,MONTO_DEVOLUCIONES_LOCAL,COBROS_LOCAL,MONTO_COBROS_LOCAL,MONTO_COBROS_CHQ_LOCAL,MONTO_COBROS_EFC_LOCAL,MONTO_COBROS_NC_LOCAL,DEPOSITOS_LOCAL,MONTO_DEPOSITOS_LOCAL ");
            sentencia.AppendLine(" ,PEDIDOS_DOLAR,MONTO_PEDIDOS_DOLAR,FACTURAS_DOLAR,MONTO_FACTURAS_DOLAR,DEVOLUCIONES_DOLAR,MONTO_DEVOLUCIONES_DOLAR,COBROS_DOLAR,MONTO_COBROS_DOLAR,MONTO_COBROS_CHQ_DOLAR,MONTO_COBROS_EFC_DOLAR,MONTO_COBROS_NC_DOLAR,DEPOSITOS_DOLAR,MONTO_DEPOSITOS_DOLAR ");
            
            sentencia.AppendLine(" , SINCRONIZADO ");
            sentencia.AppendLine(" , MONTO_DEVOLUCION_EFC_LOCAL, MONTO_DEVOLUCION_EFC_DOLAR, DEVOLUCIONES_EFC_LOCAL, DEVOLUCIONES_EFC_DOLAR ");
            sentencia.AppendLine(" , FACTURASTF_LOCAL, FACTURASTF_DOLAR, MONTO_FACTURASTF_LOCAL,MONTO_FACTURASTF_DOLAR,GARANTIAS_LOCAL,GARANTIAS_DOLAR,MONTO_GARANTIAS_LOCAL,MONTO_GARANTIAS_DOLAR ");
            sentencia.AppendLine(" , FACTURAS_LOCAL_CRE,MONTO_FACTURAS_LOCAL_CRE,FACTURAS_DOLAR_CRE,MONTO_FACTURAS_DOLAR_CRE");
            sentencia.AppendLine(" , GARANTIAS_LOCAL_CRE,MONTO_GARANTIAS_LOCAL_CRE,GARANTIAS_DOLAR_CRE,MONTO_GARANTIAS_DOLAR_CRE");
            sentencia.AppendLine(" , FACTURASTF_LOCAL_CRE,MONTO_FACTURASTF_LOCAL_CRE,FACTURASTF_DOLAR_CRE,MONTO_FACTURASTF_DOLAR_CRE");
            sentencia.AppendLine(" , FACTURAS_LOCAL_CONT,MONTO_FACTURAS_LOCAL_CONT,FACTURAS_DOLAR_CONT,MONTO_FACTURAS_DOLAR_CONT");
            sentencia.AppendLine(" , GARANTIAS_LOCAL_CONT,MONTO_GARANTIAS_LOCAL_CONT,GARANTIAS_DOLAR_CONT,MONTO_GARANTIAS_DOLAR_CONT");
            sentencia.AppendLine(" , FACTURASTF_LOCAL_CONT,MONTO_FACTURASTF_LOCAL_CONT,FACTURASTF_DOLAR_CONT,MONTO_FACTURASTF_DOLAR_CONT");
            sentencia.AppendLine(String.Format(" FROM {0}", Table.ERPADMIN_JORNADA_RUTAS));
            sentencia.AppendLine(" WHERE RUTA = @RUTA AND julianday(date(FECHA)) = julianday(date('now','localtime'))");

            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@RUTA", ruta)});

            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                if (reader.Read())
                {
                    resultado = ObtenerJornada(reader);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se puede obtener las jornada para la ruta '{0}' y la fecha '{1}'. {2}", ruta, fecha, ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        /// <summary>
        /// Obtiene la jornada de un SQL reader.
        /// </summary>
        /// <param name="reader">Reader donde se encuentra almacenada la jornada</param>
        /// <returns></returns>
        private static JornadaRuta ObtenerJornada(SQLiteDataReader reader)
        {
            JornadaRuta resultado = new JornadaRuta();
            resultado.ruta = reader.GetString(0);
            resultado.fecha = reader.GetDateTime(1);
            resultado.fechaHoraInicio = GetNullableDateTime(reader, 2);
            resultado.fechaHoraFin = GetNullableDateTime(reader, 3);
            resultado.pedidosLocal = GetNullableDecimal(reader, 4);
            resultado.montoPedidosLocal = GetNullableDecimal(reader, 5);
            resultado.facturasLocal = GetNullableDecimal(reader, 6);
            resultado.montoFacturasLocal = GetNullableDecimal(reader, 7);
            resultado.devolucionesLocal = GetNullableDecimal(reader, 8);
            resultado.montoDevolucionesLocal = GetNullableDecimal(reader, 9);
            resultado.cobrosLocal = GetNullableDecimal(reader, 10);
            resultado.montoCobrosLocal = GetNullableDecimal(reader, 11);
            resultado.montoCobrosChequeLocal = GetNullableDecimal(reader, 12);
            resultado.montoCobrosEfectivoLocal = GetNullableDecimal(reader, 13);
            resultado.montoCobrosNotaCreditoLocal = GetNullableDecimal(reader, 14);
            resultado.depositosLocal = GetNullableDecimal(reader, 15);
            resultado.montoDepositosLocal = GetNullableDecimal(reader, 16);

            resultado.pedidosDolar = GetNullableDecimal(reader, 17);
            resultado.montoPedidosDolar = GetNullableDecimal(reader, 18);
            resultado.facturasDolar = GetNullableDecimal(reader, 19);
            resultado.montoFacturasDolar = GetNullableDecimal(reader, 20);
            resultado.devolucionesDolar = GetNullableDecimal(reader, 21);
            resultado.montoDevolucionesDolar = GetNullableDecimal(reader, 22);
            resultado.cobrosDolar = GetNullableDecimal(reader, 23);
            resultado.montoCobrosDolar = GetNullableDecimal(reader, 24);
            resultado.montoCobrosChequeDolar = GetNullableDecimal(reader, 25);
            resultado.montoCobrosEfectivoDolar = GetNullableDecimal(reader, 26);
            resultado.montoCobrosNotaCreditoDolar = GetNullableDecimal(reader, 27);
            resultado.depositosDolar = GetNullableDecimal(reader, 28);
            resultado.montoDepositosDolar = GetNullableDecimal(reader, 29);

            resultado.sincronizado = reader.GetString(30) == "S";

            #region  Facturas de contado y recibos en FR - KFC

            resultado.montoDevolucionesEfectivoLocal = GetNullableDecimal(reader, 31);
            resultado.montoDevolucionesEfectivoDolar = GetNullableDecimal(reader, 32);
            resultado.DevolucionesEfectivoLocal = GetNullableDecimal(reader, 33);
            resultado.DevolucionesEfectivoDolar = GetNullableDecimal(reader, 34);
            #endregion

            resultado.FacturasTomaFisicaLocal = GetNullableDecimal(reader, 35);
            resultado.FacturasTomaFisicaDolar = GetNullableDecimal(reader, 36);
            resultado.MontoFacturasTomaFisicaLocal = GetNullableDecimal(reader, 37);
            resultado.montoFacturasTomaFisicaDolar = GetNullableDecimal(reader, 38);

            resultado.GarantiasLocal = GetNullableDecimal(reader, 39);
            resultado.GarantiasDolar = GetNullableDecimal(reader, 40);
            resultado.MontoGarantiasLocal = GetNullableDecimal(reader, 41);
            resultado.MontoGarantiasDolar = GetNullableDecimal(reader, 42);

            //Proyecto Gas Z separación de Crédito y Contado

            resultado.FacturasLocalCre = GetNullableDecimal(reader, 43);
            resultado.MontoFacturasLocalCre = GetNullableDecimal(reader, 44);
            resultado.FacturasDolarCre = GetNullableDecimal(reader, 45);
            resultado.MontoFacturasDolarCre = GetNullableDecimal(reader, 46);

            resultado.GarantiasLocalCre = GetNullableDecimal(reader, 47);
            resultado.MontoGarantiasLocalCre = GetNullableDecimal(reader, 48);
            resultado.GarantiasDolarCre = GetNullableDecimal(reader, 49);
            resultado.MontoGarantiasDolarCre = GetNullableDecimal(reader, 50);

            resultado.FacturasTomaFisicaLocalCre = GetNullableDecimal(reader, 51);
            resultado.MontoFacturasTomaFisicaLocalCre = GetNullableDecimal(reader, 52);
            resultado.FacturasTomaFisicaDolarCre = GetNullableDecimal(reader, 53);
            resultado.MontoFacturasTomaFisicaDolarCre = GetNullableDecimal(reader, 54);

            resultado.FacturasLocalCont = GetNullableDecimal(reader, 55);
            resultado.MontoFacturasLocalCont = GetNullableDecimal(reader, 56);
            resultado.FacturasDolarCont = GetNullableDecimal(reader, 57);
            resultado.MontoFacturasDolarCont = GetNullableDecimal(reader, 58);

            resultado.GarantiasLocalCont = GetNullableDecimal(reader, 59);
            resultado.MontoGarantiasLocalCont = GetNullableDecimal(reader, 60);
            resultado.GarantiasDolarCont = GetNullableDecimal(reader, 61);
            resultado.MontoGarantiasDolarCont = GetNullableDecimal(reader, 62);

            resultado.FacturasTomaFisicaLocalCont = GetNullableDecimal(reader, 63);
            resultado.MontoFacturasTomaFisicaLocalCont = GetNullableDecimal(reader, 64);
            resultado.FacturasTomaFisicaDolarCont = GetNullableDecimal(reader, 65);
            resultado.MontoFacturasTomaFisicaDolarCont = GetNullableDecimal(reader, 66);

            return resultado;
        }


        public static bool VerificarJornadaAbierta()
        {
            bool resultado = false;
            List<Ruta> rutas = Ruta.ObtenerRutas();
            SQLiteParameterList parametros;
            StringBuilder sentencia;

            if (rutas.Count > 0)
            {
                sentencia = new StringBuilder();
                sentencia.AppendLine(" SELECT COUNT(RUTA) ");
                sentencia.AppendLine(String.Format(" FROM {0} ", Table.ERPADMIN_JORNADA_RUTAS));
                sentencia.AppendLine(String.Format(" WHERE RUTA = @RUTA AND julianday(date(FECHA)) = julianday(date('now','localtime'))"));
                sentencia.AppendLine(String.Format(" AND FECHA_HORA_INICIO is not NULL "));

                SQLiteDataReader reader = null;

                parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                    new SQLiteParameter("@RUTA", rutas[0].Codigo)
                    // mvega: se usa la funcion date('now','localtime') de SQLite
                    //,new SQLiteParameter("@FECHA", DateTime.Now.Date) 
                });

                try
                {
                    reader = GestorDatos.EjecutarConsulta(sentencia.ToString(),parametros);

                    if (reader.Read())
                    {
                        int count = reader.GetInt32(0);
                        resultado = count > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Error verificando si la jornada está abierta. {0}", ex.Message));
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            return resultado;  
        }

        public static bool VerificarJornadaCerrada()
        {
            bool resultado = false;
            List<Ruta> rutas = Ruta.ObtenerRutas();
            SQLiteParameterList parametros;
            StringBuilder sentencia;

            if (rutas.Count > 0)
            {
                sentencia = new StringBuilder();
                sentencia.AppendLine(" SELECT COUNT(RUTA) ");
                sentencia.AppendLine(String.Format(" FROM {0} ", Table.ERPADMIN_JORNADA_RUTAS));
                sentencia.AppendLine(String.Format(" WHERE RUTA = @RUTA AND julianday(date(FECHA)) = julianday(date('now','localtime')) "));
                sentencia.AppendLine(String.Format(" AND FECHA_HORA_FIN is not NULL "));

                SQLiteDataReader reader = null;

                parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                    new SQLiteParameter("@RUTA", rutas[0].Codigo)
                    // mvega: se usa la funcion date('now','localtime') de SQLite
                    //,new SQLiteParameter("@FECHA", DateTime.Now.Date) 
                });

                try
                {
                    reader = GestorDatos.EjecutarConsulta(sentencia.ToString(),parametros);

                    if (reader.Read())
                    {
                        int count = reader.GetInt32(0);
                        resultado = count > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Error verificando si la jornada está cerrada. {0}", ex.Message));
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            return resultado;
        }

        public static bool VerificarTomaFísica(bool verificar)
        {            
            bool resultado = false;
            SQLiteParameterList parametros;
            StringBuilder sentencia;

            if (verificar)
            {
                sentencia = new StringBuilder();
                sentencia.AppendLine(" SELECT COUNT(CONSECUTIVO) ");
                sentencia.AppendLine(String.Format(" FROM {0} ", Table.ERPADMIN_TOMA_FISICA_INV));
                sentencia.AppendLine(String.Format(" WHERE HANDHELD = @HH AND julianday(date(FECHA)) = julianday(date('now','localtime')) "));

                SQLiteDataReader reader = null;

                parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                    new SQLiteParameter("@HH",Ruta.NombreDispositivo())
                    // mvega: se usa la funcion date('now','localtime') de SQLite
                    //,new SQLiteParameter("@FECHA", DateTime.Now.Date) 
                });

                try
                {
                    reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                    if (reader.Read())
                    {
                        int count = reader.GetInt32(0);
                        resultado = count > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Error verificando si la jornada está cerrada. {0}", ex.Message));
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene la sentencia de actualización o insercción dependido si el registro ya se encuentra actualizado.
        /// </summary>
        /// <returns></returns>
        public static String ObtenerSentenciaActualizacionJornada()
        {
            String resultado = String.Empty;
            StringBuilder sentencia = new StringBuilder();
            try
            {
                List<Ruta> rutas = Ruta.ObtenerRutas();
                for (int i = 0; i < rutas.Count; i++)
                {
                    Ruta ruta = rutas[i];
                    String separador = i < rutas.Count - 1 ? SEPARADOR : String.Empty;
                    /*
                     * Se manejaran los mismo numeros para remplazos en ambas sentencias:
                     * {0} = Nombre tabla
                     * {1} = PEDIDO, {2} MONTO_PEDIDOS, {3} FACTURAS, {4} MONTO_FACTURAS, {5} DEVOLUCIONES, {6} MONTO_DEVOLCUIONES
                     *  , {7} COBROS, {8} MONTO_COBROS, {9} MONTO_COBROS_CHQ, {10} MONTO_COBROS_EFC, {11} MONTO_COBROS_NC
                     *  , {12} DEPOSITOS, {13}MONTO_DEPOSITOS 
                     *  , {14} RUTA, {15} FECHA, {16} FECHA_HORA_INICIO, {17} FECHA_HORA_FIN
                     * 
                     * 
                     *  RUTA,FECHA,FECHA_HORA_INICIO,FECHA_HORA_FIN,PEDIDOS,MONTO_PEDIDOS,FACTURAS,MONTO_FACTURAS,
            DEVOLUCIONES,MONTO_DEVOLUCIONES,COBROS,MONTO_COBROS,MONTO_COBROS_CHQ,MONTO_COBROS_EFC,
            MONTO_COBROS_NC,DEPOSITOS,MONTO_DEPOSITOS
                     */
                    
                    JornadaRuta jornada = ObtenerJornada(ruta.Codigo, DateTime.Now.Date);
                    if (jornada != null)
                    {
                        if (jornada.sincronizado)
                        {
                            sentencia.AppendLine(ObtenerSentenciaUpdate(jornada));
                        }
                        else
                        {
                            sentencia.AppendLine(ObtenerSentenciaInsert(jornada));
                            sentencia.Append(SEPARADOR);
                            sentencia.AppendLine(ObtenerSentenciaUpdate(jornada));
                        }

                        //sentencia.Append(separador);
                        sentencia.Append(SEPARADOR);

                        resultado = String.Format(sentencia.ToString()
                            , jornada.pedidosLocal, jornada.montoPedidosLocal
                            , jornada.facturasLocal, jornada.montoFacturasLocal
                            , jornada.devolucionesLocal, jornada.montoDevolucionesLocal
                            , jornada.cobrosLocal, jornada.montoCobrosLocal
                            , jornada.montoCobrosChequeLocal, jornada.montoCobrosEfectivoLocal, jornada.montoCobrosNotaCreditoLocal
                            , jornada.depositosLocal, jornada.montoDepositosLocal
                            , jornada.pedidosDolar, jornada.montoPedidosDolar
                            , jornada.facturasDolar, jornada.montoFacturasDolar
                            , jornada.devolucionesDolar, jornada.montoDevolucionesDolar
                            , jornada.cobrosDolar, jornada.montoCobrosDolar
                            , jornada.montoCobrosChequeDolar, jornada.montoCobrosEfectivoDolar, jornada.montoCobrosNotaCreditoDolar
                            , jornada.depositosDolar, jornada.montoDepositosDolar
                            , jornada.montoDevolucionesEfectivoDolar, jornada.montoDevolucionesEfectivoLocal
                            , jornada.devolucionesEfectivoDolar,jornada.devolucionesEfectivoLocal
                            , jornada.garantiasLocal,jornada.montoGarantiasLocal
                            ,jornada.garantiasDolar,jornada.montoGarantiasDolar
                            ,jornada.facturasTomaFisicaLocal,jornada.montoFacturasTomaFisicaLocal
                            , jornada.facturasTomaFisicaDolar, jornada.montoFacturasTomaFisicaDolar,
                            //Proyecto Gas Z
                            jornada.facturasLocalCre,jornada.montoFacturasLocalCre,
                            jornada.facturasDolarCre,jornada.montoFacturasLocalCre,
                            jornada.garantiasLocalCre,jornada.montoGarantiasLocalCre,
                            jornada.garantiasDolarCre, jornada.montoGarantiasDolarCre,
                            jornada.FacturasTomaFisicaLocalCre,jornada.montoFacturasTomaFisicaLocalCre,
                            jornada.FacturasTomaFisicaDolarCre,jornada.montoFacturasTomaFisicaDolarCre,

                            jornada.facturasLocalCont,jornada.montoFacturasLocalCont,
                            jornada.facturasDolarCont,jornada.montoFacturasLocalCont,
                            jornada.garantiasLocalCont,jornada.montoGarantiasLocalCont,
                            jornada.garantiasDolarCont, jornada.montoGarantiasDolarCont,
                            jornada.FacturasTomaFisicaLocalCont,jornada.montoFacturasTomaFisicaLocalCont,
                            jornada.FacturasTomaFisicaDolarCont,jornada.montoFacturasTomaFisicaDolarCont
                            );                       
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return resultado;
        }

        /// <summary>
        /// Obtiene la sentencia UPDATE para la actualización del servidor. 
        /// Forma la sentencia según el arreglo de columnas. Inicia desde elemento número 4 debido a que son las columnas
        /// de los valores de los documentos. De la columna 0 a la 3 son las columnas "Informativas/"
        /// </summary>
        /// <returns></returns>
        private static String ObtenerSentenciaUpdate(JornadaRuta jornada)
        {
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(String.Format(" UPDATE {0} SET  ", JORNADA_RUTAS_SERVIDOR));
            if (jornada.FechaHoraFin != null)
            {
                sentencia.Append(String.Format(" FECHA_HORA_FIN = ERPADMIN.CONVIERTA_FECHA('{0:yyyy/MM/dd HH:mm:ss}'),", jornada.FechaHoraFin));
                
            }
            for (int i = 0; i < columnas.Length; i++)
            {
                sentencia.Append(String.Format("{0} = {{{1}}}{2}", columnas[i], i, i != columnas.Length - 1 ? "," : String.Empty));
            }
            sentencia.Append(String.Format(" WHERE RUTA = '{0}' AND FECHA = ERPADMIN.CONVIERTA_FECHA('{1:yyyy/MM/dd}') ", jornada.Ruta1, jornada.Fecha));
            return sentencia.ToString();
        }

        /// <summary>
        /// Obtiene la sentencia INSERT para guardar el registro de la jornada en el servidor.
        /// Construye "dinamicamente" la sentencia basado en el arreglo de columnas
        /// </summary>
        /// <returns></returns>
        private static String ObtenerSentenciaInsert(JornadaRuta jornada)
        {
            StringBuilder sentencia = new StringBuilder();
            StringBuilder select = new StringBuilder();
            
            sentencia.Append(String.Format(" INSERT INTO {0} (RUTA,FECHA,FECHA_HORA_INICIO", JORNADA_RUTAS_SERVIDOR));
            select.Append(String.Format("SELECT '{0}', ERPADMIN.CONVIERTA_FECHA('{1:yyyy/MM/dd}'), ERPADMIN.CONVIERTA_FECHA('{2:yyyy/MM/dd HH:mm:ss}') "
                , jornada.Ruta1, jornada.Fecha, jornada.FechaHoraInicio));

            if (jornada.FechaHoraFin != null)
            {
                sentencia.Append(",FECHA_HORA_FIN");
                select.Append(String.Format(",'{0:yyyy/MM/dd HH:mm:ss}' ", jornada.FechaHoraFin));
            }
            select.Append(String.Format(" FROM {0} ", "ERPADMIN.RUTA_CFG"));
            select.Append(String.Format(" WHERE RUTA = '{0}' AND 0 = (SELECT COUNT(*) FROM {1} WHERE RUTA = '{0}' AND FECHA = ERPADMIN.CONVIERTA_FECHA('{2:yyyy/MM/dd}') )"
                , jornada.ruta, JORNADA_RUTAS_SERVIDOR, jornada.Fecha.Date));

            sentencia.Append(String.Format(") {0}", select.ToString()));
            return sentencia.ToString();
        }

        /// <summary>
        /// Purga los datos de todas las jornadas.
        /// </summary>
        /// <returns></returns>
        public static bool Purgar()
        {
            bool resultado = false;
            StringBuilder sentencia = new StringBuilder();

            sentencia.AppendLine(String.Format(" DELETE FROM {0} ", Table.ERPADMIN_JORNADA_RUTAS));
            try
            {
                int registrosAfectados = GestorDatos.EjecutarComando(sentencia.ToString());
                resultado = registrosAfectados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se puede purgar los datos de las jornadas . {0}", DateTime.Now.Date, ex.Message));
            }

            return resultado;
        }
        /// <summary>
        /// Actualiza el estado de las jornadas de hoy a SINCRONIZADO.
        /// </summary>
        /// <returns></returns>
        public static bool EstablecerSincronizado()
        {
            bool resultado = false;
            StringBuilder sentencia = new StringBuilder();

            sentencia.AppendLine(String.Format(" UPDATE {0} SET {1} = 'S' ", Table.ERPADMIN_JORNADA_RUTAS, SINCRONIZADO));
            sentencia.AppendLine(" WHERE julianday(date(FECHA)) = julianday(date('now','localtime')) ");

            // mvega: se usa la funcion date('now','localtime') de SQLite
            //parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@FECHA", DateTime.Now.Date) });
            try
            {
                int registrosAfectados = GestorDatos.EjecutarComando(sentencia.ToString());
                resultado = registrosAfectados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("No se puede actualizar el estado del registro de jornada para la fecha '{0}' . {1}", DateTime.Now.Date, ex.Message));
            }

            return resultado;
        }

        #region Util
        /// <summary>
        /// Obtiene el valor de la columna dada. Verifica que el resultado no sea null.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        private static decimal GetNullableDecimal(SQLiteDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }
        /// <summary>
        /// Obtiene el valor de la columna dada. Verifica que el resultado no sea null.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        private static DateTime? GetNullableDateTime(SQLiteDataReader reader, int ordinal)
        {
            DateTime? resultado = null;
            if(!reader.IsDBNull(ordinal))
            {
                resultado = reader.GetDateTime(ordinal);
            }
            return resultado;
        }

        #endregion
        #endregion
    }
}
