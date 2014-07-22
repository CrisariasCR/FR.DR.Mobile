using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Detalle de recibo de cobro
    /// </summary>
    public class DetalleRecibo
    {
        #region Variables y Propiedades de instancia

        private decimal saldoLocal = 0;
        /// <summary>
        /// monto del saldo
        /// </summary>
        public decimal SaldoLocal
        {
            get { return saldoLocal; }
            set { saldoLocal = value; }
        }

        private decimal saldoDolar = 0;
        /// <summary>
        /// monto del saldo
        /// </summary>
        public decimal SaldoDolar
        {
            get { return saldoDolar; }
            set { saldoDolar = value; }
        }

        private decimal montoMovimientoLocal = 0;
        /// <summary>
        /// monto del movimiento
        /// </summary>
        public decimal MontoMovimientoLocal
        {
            get { return montoMovimientoLocal; }
            set { montoMovimientoLocal = value; }
        }

        private decimal montoMovimientoDolar = 0;
        /// <summary>
        /// monto del movimiento
        /// </summary>
        public decimal MontoMovimientoDolar
        {
            get { return montoMovimientoDolar; }
            set { montoMovimientoDolar = value; }
        }

        private DateTime fechaRealizacion = DateTime.Now;
        /// <summary>
        /// Fecha Realizacion
        /// </summary>
        public DateTime FechaRealizacion
        {
            get { return fechaRealizacion; }
            set { fechaRealizacion = value; }
        }

        private DateTime fechaUltimoProceso = DateTime.Now;
        /// <summary>
        /// Fecha ultima modificacion
        /// </summary>
        public DateTime FechaUltimoProceso
        {
            get { return fechaUltimoProceso; }
            set { fechaUltimoProceso = value; }
        }

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Moneda del documento.
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private TipoDocumento tipo;
        /// <summary>
        /// Tipo del documento.
        /// </summary>
        public TipoDocumento Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private string numero = string.Empty;
        /// <summary>
        /// Numero del recibo
        /// </summary>
        public string Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        private string numeroDocAfectado = string.Empty;
        /// <summary>
        /// Numero del documento afectado
        /// </summary>
        public string NumeroDocAfectado
        {
            get { return numeroDocAfectado; }
            set { numeroDocAfectado = value; }
        }

        #endregion

        /// <summary>
        /// Constructos del detalle de un recibo de cobro
        /// </summary>
        public DetalleRecibo() { }

        #region Acceso Datos

        /// <summary>
        /// Metodo que actualiza los montos y fechas de los documentos afectados en el 
        /// repositorio. 
        /// </summary>
        /// <param name="zona">zona asociada al pendiente de cobro</param>
        /// <param name="cliente">cliente asociado al pendiente de cobro</param>
        /// <param name="compania">compania asociada al pendiente de cobro</param>
        public void ActualizaPendienteCobro(string zona, string cliente, string compania)
        {
            string sentencia =
                " UPDATE " + Table.ERPADMIN_alCXC_PEN_COB +
                " SET FEC_PRO = @FEC_PRO, " +
                " SALDO_LOCAL = @SALDO_LOCAL, " +
                " SALDO_DOLAR = @SALDO_DOLAR " +

                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND   COD_ZON = @ZONA" +
                " AND   COD_CLT = @CLIENTE" +
                " AND   NUM_DOC = @CONSECUTIVO" +
                " AND   COD_TIP_DC = @TIPO";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                        GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, FechaRealizacion),
                        GestorDatos.SQLiteParameter("@SALDO_LOCAL",SqlDbType.Decimal,SaldoLocal),
                        GestorDatos.SQLiteParameter("@SALDO_DOLAR",SqlDbType.Decimal,SaldoDolar),
                        new SQLiteParameter("@CONSECUTIVO", NumeroDocAfectado),
                        new SQLiteParameter("@TIPO", ((int)Tipo).ToString()),
                        new SQLiteParameter("@ZONA", zona),
                        new SQLiteParameter("@CLIENTE", cliente),
                        new SQLiteParameter("@COMPANIA", compania.ToUpper())});
            GestorDatos.EjecutarComando(sentencia, parametros);
        }

        /// <summary>
        /// Actualiza los montos de los documentos pendientes de cobro que fueron 
        /// pagados con el cobro por anular.
        /// </summary>
        /// <param name="zona">zona asociada al pendiente de cobro</param>
        /// <param name="cliente">cliente asociado al pendiente de cobro</param>
        /// <param name="compania">compania asociada al pendiente de cobro</param>        
        public void AnulaPendienteCobro(string zona, string cliente, string compania)
        {
            string sentencia =
                " UPDATE " + Table.ERPADMIN_alCXC_PEN_COB +
                " SET FEC_PRO = @FEC_PRO, " +
                " SALDO_LOCAL = SALDO_LOCAL + @MONTO_LOCAL, "+//CONVERT(numeric(28,8),@MONTO_LOCAL), " +
                " SALDO_DOLAR = SALDO_DOLAR + @MONTO_DOLAR "+//CONVERT(numeric(28,8),@MONTO_DOLAR) " +

                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND   COD_ZON = @ZONA" +
                " AND   COD_CLT = @CLIENTE" +
                " AND   NUM_DOC = @CONSECUTIVO" +
                " AND   COD_TIP_DC = @TIPO";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                        GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now),
                        GestorDatos.SQLiteParameter("@MONTO_LOCAL",SqlDbType.Decimal,MontoMovimientoLocal),
                        GestorDatos.SQLiteParameter("@MONTO_DOLAR",SqlDbType.Decimal,MontoMovimientoDolar),
                        new SQLiteParameter("@CONSECUTIVO", NumeroDocAfectado),
                        new SQLiteParameter("@TIPO", ((int)Tipo).ToString()),
                        new SQLiteParameter("@ZONA", zona),
                        new SQLiteParameter("@CLIENTE", cliente),
                        new SQLiteParameter("@COMPANIA", compania.ToUpper())});
            try 
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Error actualizando pendientes de cobro. " + ex.Message);
            }
            
        }

        /// <summary>
        /// Obtener los detalles de un recibo
        /// </summary>
        /// <param name="cliente">cliente asociado</param>
        /// <param name="recibo">consecutivo de recibo</param>
        /// <param name="compania">compania asociada</param>
        /// <param name="moneda">tipo de moneda</param>
        /// <param name="tip_dc">tipo de documento</param>
        /// <param name="tip_da">tipo de documento</param>
        /// <returns></returns>
        public static List<DetalleRecibo> Obtener(string cliente, string recibo, string compania,TipoMoneda moneda, TipoDocumento tip_dc, TipoDocumento tip_da)
        {
            List<DetalleRecibo> detalles = new List<DetalleRecibo>();
            SQLiteParameterList parametros;
            detalles.Clear();

            string sentencia =
                " SELECT NUM_DOC,NUM_DOC_AF,COD_TIP_DA,MON_MOV_LOCAL,MON_SAL_LOC,MON_MOV_DOL,MON_SAL_DOL " +
                " FROM  " +  Table.ERPADMIN_alCXC_MOV_DIR  +
                " WHERE COD_CLT = @CLIENTE " +
                " AND   NUM_REC = @RECIBO " +
                " AND   UPPER(COD_CIA) = @COMPANIA " +
                //Modificaciones en funcionalidad de recibos de contado - KFC
                // Se valida que solo liste documentos sin anular
                " AND   IND_ANL <> 'N' " +   
                " AND   COD_TIP_DC = @COD_TIP_DC ";
                //" AND   COD_TIP_DA = @COD_TIP_DA " +
                //" ORDER BY COD_TIP_DA DESC ";

            if (tip_da == TipoDocumento.TodosDocumentosDebito)
            {
                sentencia = sentencia + " AND   COD_TIP_DA IN (@TIPO1, @TIPO2, @TIPO3, @TIPO4, @TIPO5, @TIPO6, @TIPO7, @TIPO8) ";

                SQLiteParameterList parametrostemp = new SQLiteParameterList(new SQLiteParameter[] { 
                        new SQLiteParameter("@RECIBO", recibo),
                        new SQLiteParameter("@CLIENTE", cliente),
                        new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                        new SQLiteParameter("@COD_TIP_DC", ((int)tip_dc).ToString()),
                        new SQLiteParameter("@TIPO1", ((int)TipoDocumento.Factura).ToString()),
                        new SQLiteParameter("@TIPO2", ((int)TipoDocumento.NotaDebito).ToString()),
                        new SQLiteParameter("@TIPO3", ((int)TipoDocumento.LetraCambio).ToString()),
                        new SQLiteParameter("@TIPO4", ((int)TipoDocumento.OtroDebito).ToString()),
                        new SQLiteParameter("@TIPO5", ((int)TipoDocumento.Intereses).ToString()),
                        new SQLiteParameter("@TIPO6", ((int)TipoDocumento.BoletaVenta).ToString()),
                        new SQLiteParameter("@TIPO7", ((int)TipoDocumento.InteresCorriente).ToString()),
                        new SQLiteParameter("@TIPO8", ((int)TipoDocumento.FacturaContado).ToString())});

                parametros = parametrostemp;
            }
            else
            {
                sentencia = sentencia + " AND   COD_TIP_DA = @COD_TIP_DA ";

                SQLiteParameterList parametrostemp = new SQLiteParameterList(new SQLiteParameter[] { 
                        new SQLiteParameter("@RECIBO", recibo),
                        new SQLiteParameter("@CLIENTE", cliente),
                        new SQLiteParameter("@COMPANIA", compania),
                        new SQLiteParameter("@COD_TIP_DC", ((int)tip_dc).ToString()),
                        new SQLiteParameter("@COD_TIP_DA", ((int)tip_da).ToString())});

                parametros = parametrostemp;
            }

            sentencia = sentencia + " ORDER BY COD_TIP_DA DESC ";

            
                SQLiteDataReader reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DetalleRecibo detalle = new DetalleRecibo();
                    detalle.Numero = reader.GetString(0);
                    detalle.NumeroDocAfectado = reader.GetString(1);
                    detalle.Tipo = (TipoDocumento)Convert.ToInt32(reader.GetString(2));
                    detalle.montoMovimientoLocal = reader.GetDecimal(3);
                    detalle.SaldoLocal = reader.GetDecimal(4);
                    detalle.MontoMovimientoDolar = reader.GetDecimal(5);
                    detalle.SaldoDolar = reader.GetDecimal(6);
                    detalle.moneda = moneda;

                    detalles.Add(detalle);
                }
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            
            return detalles;
        }
        #endregion

    }
}
