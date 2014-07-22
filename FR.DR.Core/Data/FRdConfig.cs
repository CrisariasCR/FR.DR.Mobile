using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls
{
	[Android.Runtime.Preserve(AllMembers=true)]
    public static class FRdConfig
    {
        public const string NoDefinido = "ND";

        private static bool permitirInfoDescuentosVencida;
        /// <summary>
        /// Indica si el rutero puede utilizar el sistema con información
        /// de descuentos vencida.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool PermitirInfoDescuentosVencida
        {
            get { return FRdConfig.permitirInfoDescuentosVencida; }
            set { FRdConfig.permitirInfoDescuentosVencida = value; }
        }

        private static bool prontoPagoTotales;
        /// <summary>
        /// Indica si se aplica el descuento por pronto pago a pagos parciales.
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool ProntoPagoTotales
        {
            get { return FRdConfig.prontoPagoTotales; }
            set { FRdConfig.prontoPagoTotales = value; }
        }

        private static bool permiteSincroDesatendida;
        /// <summary>
        /// Indica si se permite la sincronización desatendida.
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool PermiteSincroDesatendida
        {
            get { return FRdConfig.permiteSincroDesatendida; }
            set { FRdConfig.permiteSincroDesatendida = value; }
        }

        private static bool aplicaRecargo;

        public static bool AplicaRecargo
        {
            get { return FRdConfig.aplicaRecargo; }
            set { FRdConfig.aplicaRecargo = value; }
        }

        private static decimal porcentajeRecargo;

        public static decimal PorcentajeRecargo
        {
            get { return FRdConfig.porcentajeRecargo; }
            set { FRdConfig.porcentajeRecargo = value; }
        }

        private static decimal porcentajeUsoResoluciones;

        public static decimal PorcentajeUsoResoluciones
        {
            get { return FRdConfig.porcentajeUsoResoluciones; }
            set { FRdConfig.porcentajeUsoResoluciones = value; }
        }

        #region Cambios Motor Precios 7.0 - KFC
        /* Cambios Motor Precios 7.0 - KFC        
         * private static FRArticulo.EsquemaDescuento esquemaDescuento;

        public static FRArticulo.EsquemaDescuento EsquemaDescuento
        {
            get { return esquemaDescuento; }
            set { esquemaDescuento = value; }
        */

        #endregion

        private static bool usaConsignacion;

        public static bool UsaConsignacion
        {
            get { return FRdConfig.usaConsignacion; }
            set { FRdConfig.usaConsignacion = value; }
        }
        private static bool cobrarMalEstado;

        public static bool CobrarMalEstado
        {
            get { return FRdConfig.cobrarMalEstado; }
            set { FRdConfig.cobrarMalEstado = value; }
        }

       // Facturas de contado y recibos en FR - KFC
        private static bool reciboFacturasContado;

        public static bool ReciboFacturasContado
        {
            get { return FRdConfig.reciboFacturasContado; }
            set { FRdConfig.reciboFacturasContado = value; }
        }

        // Modificaciones en funcionalidad de generacion de recibos de contado - KFC
        private static string formaGenerarRecibo;

        public static string FormaGenerarRecibo
        {
            get { return FRdConfig.formaGenerarRecibo; }
            set { FRdConfig.formaGenerarRecibo = value; }
        }

        private static bool permiteCambiarNivelPrecio;
        /// <summary>
        /// Indica si se permite cambiar la lista de precios del cliente en el pedido.
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool PermiteCambiarNivelPrecio
        {
            get { return FRdConfig.permiteCambiarNivelPrecio; }
            set { FRdConfig.permiteCambiarNivelPrecio = value; }
        }
        private static bool permiteCambiarPais;
        /// <summary>
        /// Indica si se permite cambiar el pais del cliente en el pedido.
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool PermiteCambiarPais
        {
            get { return FRdConfig.permiteCambiarPais; }
            set { FRdConfig.permiteCambiarPais = value; }
        }
        private static bool permiteCambiarClase;
        /// <summary>
        /// Indica si se permite cambiar la clase del pedido (normal o credito fiscal).
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool PermiteCambiarClase
        {
            get { return FRdConfig.permiteCambiarClase; }
            set { FRdConfig.permiteCambiarClase = value; }
        }
        private static bool permiteCambiarCondicionPago;
        /// <summary>
        /// Indica si se permite cambiar la condicion de pago del cliente en el pedido.
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool PermiteCambiarCondicionPago
        {
            get { return FRdConfig.permiteCambiarCondicionPago; }
            set { FRdConfig.permiteCambiarCondicionPago = value; }
        }
        private static bool usaFacturacion;
        /// <summary>
        /// Indica que se puede realizar facturación en ruta. 
        /// Variable configurada en Config.xml.
        /// </summary>
        public static bool UsaFacturacion
        {
            get { return FRdConfig.usaFacturacion; }
            set { FRdConfig.usaFacturacion = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static bool usaSugeridoVenta;
        /// <summary>
        /// Indica si por defecto se van a usar sugeridos de venta.
        /// Variable cargada desde los parametros de FRd.
        /// </summary>
        public static bool UsaSugeridoVenta
        {
            get { return FRdConfig.usaSugeridoVenta; }
            set { FRdConfig.usaSugeridoVenta = value; }
        }

        private static bool usaJornadaLaboral;
        /// <summary>
        /// Indica si se va a usar la opcion de jornada laboral.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static bool UsaJornadaLaboral
        {
            get { return FRdConfig.usaJornadaLaboral; }
            set { FRdConfig.usaJornadaLaboral = value; }
        }

        private static bool usaRubrosJornada;
        /// <summary>
        /// Indica si se va a usar la opcion de rubros jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static bool UsaRubrosJornada
        {
            get { return FRdConfig.usaRubrosJornada; }
            set { FRdConfig.usaRubrosJornada = value; }
        }

        private static bool usaEnvases;
        /// <summary>
        /// Indica si se va a usar la opcion de rubros jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static bool UsaEnvases
        {
            get { return FRdConfig.usaEnvases; }
            set { FRdConfig.usaEnvases = value; }
        }

        private static bool soporteTraspasos;

        public static bool SoporteTraspasos
        {
            get { return FRdConfig.soporteTraspasos; }
            set { FRdConfig.soporteTraspasos = value; }
        }

        private static string rubro1Jornada;
        /// <summary>
        /// Descripción del rubro 1 de jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static string Rubro1Jornada
        {
            get { return FRdConfig.rubro1Jornada; }
            set { FRdConfig.rubro1Jornada = value; }
        }

        private static string rubro2Jornada;
        /// <summary>
        /// Descripción del rubro 2 de jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static string Rubro2Jornada
        {
            get { return FRdConfig.rubro2Jornada; }
            set { FRdConfig.rubro2Jornada = value; }
        }

        private static string rubro3Jornada;
        /// <summary>
        /// Descripción del rubro 3 de jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static string Rubro3Jornada
        {
            get { return FRdConfig.rubro3Jornada; }
            set { FRdConfig.rubro3Jornada = value; }
        }

        private static string rubro4Jornada;
        /// <summary>
        /// Descripción del rubro 4 de jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static string Rubro4Jornada
        {
            get { return FRdConfig.rubro4Jornada; }
            set { FRdConfig.rubro4Jornada = value; }
        }

        private static string rubro5Jornada;
        /// <summary>
        /// Descripción del rubro 1 de jornada.
        /// Variable cargada desde los parametros de FR.
        /// </summary>
        public static string Rubro5Jornada
        {
            get { return FRdConfig.rubro5Jornada; }
            set { FRdConfig.rubro5Jornada = value; }
        }

        #region MejorasFRTostadoraElDorado600R6 JEV
        private static bool utilizaTomaFisica;
        private static bool facturarDiferencias;
        private static string clienteRutero;

        /// <summary>
        /// Contiene el código del cliente
        /// asociado al rutero
        /// </summary>
        public static string ClienteRutero
        {
            get { return FRdConfig.clienteRutero; }
            set { FRdConfig.clienteRutero = value; }
        }

        /// <summary>
        /// Indica si se deben o no
        /// facturar las diferencias del inventario físico
        /// </summary>
        public static bool FacturarDiferencias
        {
            get { return FRdConfig.facturarDiferencias; }
            set { FRdConfig.facturarDiferencias = value; }
        }

        /// <summary>
        /// Indica si se utiliza la toma física
        /// en la ruta
        /// </summary>
        public static bool UtilizaTomaFisica
        {
            get { return FRdConfig.utilizaTomaFisica; }
            set { FRdConfig.utilizaTomaFisica = value; }
        }
        #endregion MejorasFRTostadoraElDorado600R6 JEV

        public static  void CargarImpresora()
        {
            try
            {
                string sentencia = "select count(*) from sqlite_master where name='" + Table.ERPADMIN_GLOBALES_FR + "' AND sql like '%NOMBREIMPRESORA%'";
                int res = Convert.ToInt16(GestorDatos.cnx.ExecuteScalar(sentencia));
                if (res > 0)
                {
                    string sql = "SELECT NOMBREIMPRESORA from " + Table.ERPADMIN_GLOBALES_FR;
                    FRmConfig.Impresora = GestorDatos.cnx.ExecuteScalar(sql).ToString();
                }
            }
            catch
            {
            }
        }

        public static void CargarTamañoPapel()
        {
            try
            {
                string sentencia = "select count(*) from sqlite_master where name='" + Table.ERPADMIN_GLOBALES_FR + "' AND sql like '%TAMANOPAPEL%'";
                int res = Convert.ToInt16(GestorDatos.cnx.ExecuteScalar(sentencia));
                if (res > 0)
                {
                    string sql = "SELECT TAMANOPAPEL from " + Table.ERPADMIN_GLOBALES_FR;
                    FRmConfig.TamañoPapel = Convert.ToDouble(GestorDatos.cnx.ExecuteScalar(sql).ToString());
                }
            }
            catch
            {
            }
        }

        public static void CargarGlobales()
        {
            CargarImpresora();
            CargarTamañoPapel();
            #region Cambios Motor Precios 7.0 - KFC
            // KFCP se quita el esquema descuento
            /* string sentencia =
                 @" SELECT CAM_LST_PRC,CAM_PAIS,CAM_CLA_PED,CAM_CON_PG,ESQUEMA_DESC," +
                     @" APLICA_RECARGO,PORC_RECARGO,USA_CONSIGNACION,COBRO_MAL_ESTADO," +
                     @" USA_RUTEO,COBROS_FACTURA_CONTADO,USA_SUG_VENTA " +
                 "FROM " + Table.ERPADMIN_GLOBALES_FR ;*/
            #endregion

            string sentencia =
                @" SELECT CAM_LST_PRC,CAM_PAIS,CAM_CLA_PED,CAM_CON_PG," +
                @" APLICA_RECARGO,PORC_RECARGO,USA_CONSIGNACION,COBRO_MAL_ESTADO," +
                @" USA_RUTEO,COBROS_FACTURA_CONTADO,USA_SUG_VENTA,SOPORTE_TRASIEGOS,USA_RUBROS_JORNADA,Rubro1_JORNADA,Rubro2_JORNADA,Rubro3_JORNADA,Rubro4_JORNADA,Rubro5_JORNADA,USA_ENVASES,PRONTO_PAGO_TOTALES,PORCENTAJE_RESOLUCION " +
                " FROM " + Table.ERPADMIN_GLOBALES_FR;
            //MejorasGrupoPelon600R6 - KF. Se agrega USA_SUG_VENTA a la consulta

            SQLiteDataReader reader = null;
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                if (reader.Read())
                {
                    FRdConfig.PermiteCambiarNivelPrecio = reader.GetString(0).Equals("S");
                    FRdConfig.PermiteCambiarPais = reader.GetString(1).Equals("S");
                    FRdConfig.PermiteCambiarClase = reader.GetString(2).Equals("S");
                    FRdConfig.PermiteCambiarCondicionPago = reader.GetString(3).Equals("S");
                    FRdConfig.UsaFacturacion = reader.GetString(8).Equals("S");

                    // Cambios Motor Precios 7.0 - KFC 
                    //FRdConfig.EsquemaDescuento = (FRArticulo.EsquemaDescuento)Convert.ToChar(reader.GetString(4));

                    FRdConfig.AplicaRecargo = reader.GetString(4).Equals("S");

                    FRdConfig.PorcentajeRecargo = reader.GetDecimal(5);
                    FRdConfig.UsaConsignacion = reader.GetString(6).Equals("S");
                    FRdConfig.CobrarMalEstado = reader.GetString(7).Equals("S");

                    // Modificaciones en funcionalidad de generacion de recibos de contado - KFC
                    FRdConfig.ReciboFacturasContado = reader.GetString(9).Equals("S");
                    //FRdConfig.ReciboFacturasContado = reader.GetString(10);


                    //MejorasGrupoPelon600R6 - KF Sugerido de ventas
                    FRdConfig.UsaSugeridoVenta = reader.GetString(10).Equals("S");

                    //Proyecto Gas Z

                    FRdConfig.SoporteTraspasos = reader.GetString(11).Equals("S");

                    FRdConfig.UsaRubrosJornada = reader.GetString(12).Equals("S");

                    FRdConfig.Rubro1Jornada = reader.GetString(13);

                    FRdConfig.Rubro2Jornada = reader.GetString(14);

                    FRdConfig.Rubro3Jornada = reader.GetString(15);

                    FRdConfig.Rubro4Jornada = reader.GetString(16);

                    FRdConfig.Rubro5Jornada = reader.GetString(17);

                    FRdConfig.UsaEnvases = reader.GetString(18).Equals("S");

                    //Proyecto Descuento pronto pago - pagos parciales
                    
                    FRdConfig.ProntoPagoTotales = reader.GetString(19).Equals("S");

                    //Proyecto Resoluciones
                    FRdConfig.PorcentajeUsoResoluciones = reader.GetInt32(20);

                    //Proyecto Sincro Desatendida

                    //FRdConfig.PermiteSincroDesatendida = reader.GetString(21).Equals("S");

                }
                else
                    throw new Exception("No se obtuvo información de los parámetros globales");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}

