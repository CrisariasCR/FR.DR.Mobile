using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{
    /// <summary>
    /// Montos del cliente para impresion de reportes
    /// </summary>
    public struct ClienteMontos
    {
        #region Variables y Propiedades de instancia
        private decimal montoPagado ;
        /// <summary>
        /// Es el monto pagado por el cliente
        /// </summary>	
        public decimal MontoPagado
        {
            get { return montoPagado; }
            set { montoPagado = value; }
        }
	 
        private decimal montoComprado ;
        /// <summary>
        /// Es el monto total vendido al cliente.
        /// MontoComprado = MontoPreventa + MontoFacturas.
        /// </summary>
        public decimal MontoComprado
        {
            get { return montoComprado; }
            set { montoComprado = value; }
        }
        
        private decimal montoPreventa ;
        /// <summary>
        /// Es el monto total vendido en preventa al cliente.
        /// </summary>
        public decimal MontoPreventa
        {
            get { return montoPreventa; }
            set { montoPreventa = value; }
        }
       
        private decimal montoFacturas ;
        /// <summary>
        /// Es el monto total vendido facturado al cliente.
        /// </summary>
        public decimal MontoFacturas
        {
            get { return montoFacturas; }
            set { montoFacturas = value; }
        }

        private decimal montoNotasAplicadas;
        /// <summary>
        /// Es el monto total aplicado al cliente en notas de credito.
        /// </summary>
        public decimal MontoNotasAplicadas
        {
            get { return montoNotasAplicadas; }
            set { montoNotasAplicadas = value; }

        }

        private decimal montoDevuelto;
        /// <summary>
        /// ABC Caso 35771
        /// Es le monto total devuelto por el cliente
        /// </summary>
        public decimal MontoDevuelto
        {
            get { return montoDevuelto; }
            set { montoDevuelto = value; }
        }

        private decimal montoDevueltoConDoc;
        /// <summary>
        /// ABC Caso 35771
        /// Es le monto total devuelto por el cliente
        /// </summary>
        public decimal MontoDevueltoConDoc
        {
            get { return montoDevueltoConDoc; }
            set { montoDevueltoConDoc = value; }
        }

        private decimal montoDevueltoSinDoc;
        /// <summary>
        /// ABC Caso 35771
        /// Es le monto total devuelto por el cliente
        /// </summary>
        public decimal MontoDevueltoSinDoc
        {
            get { return montoDevueltoSinDoc; }
            set { montoDevueltoSinDoc = value; }
        }

        #endregion

        #region Montos para los reportes

        /// <summary>
        /// Carga el monto cobrado y el monto en notas de credito aplicadas al cliente.
        /// </summary>
        /// <param name="cliente">cliente a cargar montos</param>
        public void CargarMontoCobrado(string cliente)
        {
            object valor;
            //Carga el monto pagados
           /* string sentencia =
                " SELECT SUM(MON_MOV_LOCAL) AS MONTO " +
                " FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE COD_TIP_DC = @TIPO " + 
                " AND   COD_CLT    = @CLIENTE " + 
                " AND   IND_ANL    = @ESTADO " +
                " AND   CONVERT(NVARCHAR,FEC_PRO,112) = @HOY "+
                " GROUP BY COD_CLT ";*/
            string sentencia =
                " SELECT SUM(MON_MOV_LOCAL) AS MONTO " +
                " FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE COD_TIP_DC = @TIPO " +
                " AND   COD_CLT    = @CLIENTE " +
                " AND   IND_ANL    in (@ESTADO_ACTIVO,@ESTADO_FACT_CONT) " +
                " AND   julianday(date(FEC_PRO)) = julianday(date('now','localtime')) " +
                " GROUP BY COD_CLT ";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@TIPO", (int)TipoDocumento.Recibo),
                      new SQLiteParameter("@ESTADO_ACTIVO", ((char)EstadoDocumento.Activo).ToString()),
                      new SQLiteParameter("@ESTADO_FACT_CONT", ((char)EstadoPedido.Facturado).ToString()),
                      new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyy-MM-dd"))});
            try
            {
                valor = GestorDatos.EjecutarScalar(sentencia,parametros);
                if (valor != null && !(valor is DBNull))
                    montoPagado = Convert.ToDecimal(valor.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando el monto cobrado al cliente '" + cliente + "'. " + ex.Message);
            }
            try
            {
                ///se carga los montos en Notas de credito aplicados
                SQLiteParameterList parametros2 = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@TIPO", (int)TipoDocumento.NotaCredito),
                      new SQLiteParameter("@ESTADO_ACTIVO", ((char)EstadoDocumento.Activo).ToString()),
                      new SQLiteParameter("@ESTADO_FACT_CONT", ((char)EstadoDocumento.Contado).ToString()),
                      new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyyMMdd"))});

                valor = GestorDatos.EjecutarScalar(sentencia,parametros2);
                if (valor != null && !(valor is DBNull))
                    montoNotasAplicadas = Convert.ToDecimal(valor.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando el monto cobrado aplicado en notas de crédito del cliente '" + cliente + "'. " + ex.Message);
            }
            // KFC Recibos y Facturas de Contado >
            try
            {
                ///se carga los montos en Notas de credito aplicados
                SQLiteParameterList parametros2 = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@TIPO", (int)TipoDocumento.NotaCreditoNueva),
                      new SQLiteParameter("@ESTADO_ACTIVO", ((char)EstadoDocumento.Activo).ToString()),
                      new SQLiteParameter("@ESTADO_FACT_CONT", ((char)EstadoDocumento.Contado).ToString()),
                      new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyyMMdd"))});

                valor = GestorDatos.EjecutarScalar(sentencia, parametros2);
                if (valor != null && !(valor is DBNull))
                    montoNotasAplicadas += Convert.ToDecimal(valor.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando el monto cobrado aplicado en notas de crédito del cliente '" + cliente + "'. " + ex.Message);
            }
        }

        /// <summary>
        /// Carga el monto vendido al cliente.
        /// </summary>
        /// <param name="cliente">cliente a cargar montos</param>/// 
        public void CargarMontoVendido(string cliente)
        {
            //Carga los montos comprados por cada cliente
            string sentencia =
                " SELECT ESTADO, SUM(MON_CIV + MON_IMP_CS - MON_DSC - MONT_DESC1 - MONT_DESC2), LST_PRE, COD_CIA " +
                " FROM " + Table.ERPADMIN_alFAC_ENC_PED +
                " WHERE COD_CLT = @CLIENTE" +
                " AND ESTADO IN(@PEDIDO,@FACTURADO)" +
                " AND  julianday(date(FEC_PED)) = julianday(date('now','localtime'))" +
                " GROUP BY ESTADO, LST_PRE, COD_CIA";

            //julianday(date(INICIO)) = julianday(date('now','localtime'))

            SQLiteDataReader reader = null;            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@PEDIDO", ((char)EstadoPedido.Normal).ToString()),
                      new SQLiteParameter("@FACTURADO", ((char)EstadoPedido.Facturado).ToString()),
            });
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                EstadoPedido Estado;
                
                decimal monto = decimal.Zero;                
                while (reader.Read())
                {
                    if (FRArticulo.NivelPrecio.monedaLista(reader.GetString(3), reader.GetInt32(2)) == TipoMoneda.DOLAR)
                        monto = reader.GetDecimal(1) * Corporativo.Compania.Obtener(reader.GetString(3)).TipoCambio;
                    else
                        monto = reader.GetDecimal(1);

                    Estado = (EstadoPedido)Convert.ToChar(reader.GetString(0));

                    if (Estado == EstadoPedido.Normal)
                        this.MontoPreventa = this.MontoPreventa + monto;
                    else
                        this.MontoFacturas = this.MontoFacturas + monto;
                }

                
                this.MontoComprado = this.MontoPreventa + this.MontoFacturas;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando el monto vendido al cliente '" + cliente+ "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        #region Montos por Devolucion

        //ABC Caso 35771
        /// <summary>
        /// Cargar el monto por concepto de devoluciones por documento
        /// </summary>
        /// <param name="cliente">cliente a cargar montos</param>
        public void CargarMontoDevuelto(Cliente cliente)
        {
            //Carga los montos comprados por cada cliente
            string sentencia =
                " SELECT NUM_REF, SUM(MON_SIV + MON_IMP_CS + MON_IMP_VT - MON_DSC), COD_CIA, LST_PRE " +
                " FROM " + Table.ERPADMIN_alFAC_ENC_DEV + 
                " WHERE COD_CLT = @CLIENTE " +
                " AND   EST_DEV IN(@ESTADO,@ESTADO2) " +
                " AND julianday(date(FEC_DEV)) =  julianday(date('now','localtime'))" +
                " GROUP BY NUM_REF, COD_CIA, LST_PRE";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente.Codigo),
                      new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Activo).ToString()),
                      new SQLiteParameter("@ESTADO2", "Activo"),
                      new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyyMMdd"))});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                decimal monto = decimal.Zero;
                //recorre el datareader
                while (reader.Read())
                {
                    if (FRArticulo.NivelPrecio.monedaLista(reader.GetString(2), reader.GetInt32(3)) == TipoMoneda.DOLAR)
                        monto = reader.GetDecimal(1) * Corporativo.Compania.Obtener(reader.GetString(2)).TipoCambio;
                    else
                        monto = reader.GetDecimal(1);

                    if (reader.GetString(0) == "")
                        this.montoDevueltoSinDoc = this.montoDevueltoSinDoc + monto; //reader.GetDecimal(1);
                    else
                        this.montoDevueltoConDoc = this.montoDevueltoConDoc + monto; // reader.GetDecimal(1);
                }

                this.montoDevuelto = this.MontoDevueltoConDoc + this.MontoDevueltoSinDoc;
                
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

        #endregion
        
        #endregion

    }
}
