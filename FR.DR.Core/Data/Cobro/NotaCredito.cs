using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Nota de credito
    /// </summary>
    public class NotaCredito : DocumentoContable, IPrintable
    {
        /// <summary>
        /// Constructor de la nota de credito
        /// </summary>
        /// <param name="numero">numero asignado</param>
        /// <param name="tipoMoneda">tipo de moneda</param>
        /// <param name="compania">compania asociada</param>
        /// <param name="zona">zona asociada</param>
        /// <param name="cliente">cliente a quien corresponde</param>
        public NotaCredito(string numero, TipoMoneda tipoMoneda, string compania, string zona, string cliente)
        {
            this.Tipo = TipoDocumento.NotaCredito;
            this.Numero = numero;
            
            this.Compania = compania;
            this.Zona = zona;
            this.Cliente = cliente;

            this.FechaRealizacion = DateTime.Now;
            this.Moneda = tipoMoneda;
            this.Estado = EstadoDocumento.Activo;
        }
        /// <summary>
        /// Constructor de la nota de credito
        /// </summary>
        public NotaCredito()
        { }

        #region Variables y Propiedades de instancia

        private bool selected;
        /// <summary>
        /// Indica el estado de la venta en consignación.
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                bool oldValue = value;
                selected = value;
                if (ItemCheck != null)
                {
                    ItemCheck(this, oldValue);
                }
                RaisePropertyChanged("Selected");
            }
        }

        public static Action<NotaCredito, bool> ItemCheck { get; set; }

        public bool Seleccionado { get; set; }

        private decimal montoChequesLocal = 0;
        /// <summary>
        /// Monto por concepto de cheques
        /// </summary>
        public decimal MontoChequesLocal
        {
            get { return montoChequesLocal; }
            set { montoChequesLocal = value; }
        }

        private decimal montoEfectivoLocal = 0;
        /// <summary>
        /// Monto por concepto de efectivo
        /// </summary>
        public decimal MontoEfectivoLocal
        {
            get { return montoEfectivoLocal; }
            set { montoEfectivoLocal = value; }
        }

        private decimal montoChequesDolar = 0;
        /// <summary>
        /// Monto en dolares por concepto de cheques 
        /// </summary>
        public decimal MontoChequesDolar
        {
            get { return montoChequesDolar; }
            set { montoChequesDolar = value; }
        }

        private decimal montoEfectivoDolar = 0;
        /// <summary>
        /// Monto en dolares por concepto de efectivo 
        /// </summary>
        public decimal MontoEfectivoDolar
        {
            get { return montoEfectivoDolar; }
            set { montoEfectivoDolar = value; }
        }

        #endregion

        #region Logica Negocios

        /// <summary>
        /// Guardar la nota de credito
        /// </summary>
        public void Guardar()
        {
            GestorDatos.BeginTransaction();

            try
            {
                this.DBGuardar();

                try
                {
                    this.GuardarCheques();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando cheques. " + ex.Message);
                }

                try
                {
                    if(!Cobros.CambiarNumeroRecibo)
                        ParametroSistema.IncrementarRecibo(this.Compania,this.Zona);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error actualizando consecutivos. " + ex.Message);
                }

                GestorDatos.CommitTransaction();
            }
            catch (SQLiteException exc)
            {
                 GestorDatos.RollbackTransaction();
                throw new Exception(exc.Message);
            }
            catch (Exception exc)
            {
                GestorDatos.RollbackTransaction();
                throw new Exception(exc.Message);
            }
        }

        #endregion

        #region Acceso Datos

        /// <summary>
        /// Notas de credito de un cliente en una compania determinada
        /// </summary>
        /// <param name="compania">compania del cliente</param>
        /// <param name="cliente">cliente a obtener notas de credito</param>
        /// <returns>lista con las notas de credito</returns>
        public static List<NotaCredito> ObtenerNotasCredito(string compania, string cliente, string zona)
        {
            SQLiteDataReader reader = null;
            List<NotaCredito> notasCredito = new List<NotaCredito>();
            notasCredito.Clear();
            //LJR: Agrega zona  para clientes en multiples rutas
            string sentencia =
                
                
            " SELECT COD_ZON,NUM_DOC,FEC_DOC_FT,FEC_PRO," +
                "        FEC_VEN,MONTO_LOCAL,SALDO_LOCAL,MONTO_DOLAR,SALDO_DOLAR," +
                "        COD_TIP_DC,IND_MON,TIP_CMB_DOL " +
                " FROM " + Table.ERPADMIN_alCXC_PEN_COB +
                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND   COD_CLT = @CLIENTE" +
                " AND   IND_ANL IN (@ESTADO,@ESTADO2)" +
                " AND   SALDO_DOLAR != 0 and SALDO_LOCAL != 0 " +
                " AND   COD_TIP_DC in (@NOTACREDITO,@NOTA_NUEVA_DEVOLUCION) " + //Facturas de contado y recibos en FR - KFC, se agrega el codigo de las N/C creadas en la pocket
                " AND   COD_ZON = @ZONA" +
                " ORDER BY FEC_VEN ASC";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                      new SQLiteParameter("@ESTADO2", ((char)EstadoDocumento.Activo).ToString()),
                      new SQLiteParameter("@ESTADO", "Activo"),
                      new SQLiteParameter("@NOTACREDITO", (int)TipoDocumento.NotaCredito),
                      new SQLiteParameter("@NOTA_NUEVA_DEVOLUCION", (int)TipoDocumento.NotaCreditoNueva),
                      new SQLiteParameter("@ZONA", zona)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    NotaCredito notaCredito = new NotaCredito();

                    notaCredito.Compania = compania;
                    notaCredito.Cliente = cliente;
                    notaCredito.Zona = reader.GetString(0);
                    notaCredito.Numero = reader.GetString(1);
                    notaCredito.FechaRealizacion = reader.GetDateTime(2);
                    notaCredito.FechaUltimoProceso = reader.GetDateTime(3);
                    notaCredito.FechaVencimiento = reader.GetDateTime(4);
                    notaCredito.MontoDocLocal = reader.GetDecimal(5);
                    notaCredito.SaldoDocLocal = reader.GetDecimal(6);
                    notaCredito.MontoDocDolar = reader.GetDecimal(7);
                    notaCredito.SaldoDocDolar = reader.GetDecimal(8);
                    notaCredito.Estado = EstadoDocumento.Activo;
                    notaCredito.Tipo = (TipoDocumento)Convert.ToInt32(reader.GetString(9));
                    notaCredito.Moneda = (TipoMoneda)Convert.ToChar(reader.GetString(10));
                    notaCredito.TipoCambio = reader.GetDecimal(11);

                    //suma al monto de notas de credito el saldo de la nota de credito
                    Cobros.MontoNotasCreditoLocal += notaCredito.SaldoDocLocal;
                    Cobros.MontoNotasCreditoDolar += notaCredito.SaldoDocDolar;

                    notasCredito.Add(notaCredito);//agrega la nota a un arreglo
                }
                return notasCredito;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar las notas de crédito. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Metodo que guarda el encabezado del recibo por notas de credito en la base de datos
        /// </summary>
        private void DBGuardar()
        {
            //Caso 25452 LDS 30/10/2007 Se agrega el campo impreso a la sentencia. Se indica en N ya que no se imprimen los recibos que generan notas de crédito.
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alCXC_DOC_APL +
                "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, COD_CLT, FEC_PRO, MON_DOC_LOC, IND_MON, IND_ANL, MON_EFE_LOCAL, MON_EFE_DOLAR, MON_CHE_DOLAR, MON_CHE_LOCAL, MON_DOC_DOL, IMPRESO, FEC_VEN) " +
                " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@COD_CLT,@FEC_PRO,@MON_DOC_LOC,@IND_MON,@IND_ANL,@MON_EFE_LOCAL,@MON_EFE_DOLAR,@MON_CHE_DOLAR,@MON_CHE_LOCAL,@MON_DOC_DOL,@IMPRESO, @FEC_VEN) ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, FechaRealizacion),
                //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                GestorDatos.SQLiteParameter("@MON_DOC_LOC",SqlDbType.Decimal,Decimal.Round(this.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@IND_MON",SqlDbType.NVarChar, ((char)this.Moneda).ToString()),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()),
                GestorDatos.SQLiteParameter("@MON_EFE_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoEfectivoLocal,2)),
                GestorDatos.SQLiteParameter("@MON_EFE_DOLAR",SqlDbType.Decimal,Decimal.Round(MontoEfectivoDolar,2)),
                GestorDatos.SQLiteParameter("@MON_CHE_DOLAR",SqlDbType.Decimal,Decimal.Round(MontoChequesDolar,2)),
                GestorDatos.SQLiteParameter("@MON_CHE_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoChequesLocal,2)),
                GestorDatos.SQLiteParameter("@MON_DOC_DOL",SqlDbType.Decimal,Decimal.Round(MontoDocDolar,2)),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, "N"),

            #region  Facturas de contado y recibos en FR - KFC

                //GestorDatos.SQLiteParameter("@DOC_APLICADO",SqlDbType.NVarChar, "N"),
                GestorDatos.SQLiteParameter("@FEC_VEN",SqlDbType.DateTime, FechaVencimiento)});

            #endregion

            GestorDatos.EjecutarComando(sentencia, parametros);
        }
        #region Insert de las Notas de credito en la BD

        /// <summary>
        /// Guarda en la base de datos las notas de credito incluidas en el recibo.
        /// </summary>
        public void Guardar(Recibo recibo)
        {
            
            int tipoDoc,tipoDocAsoc,diasCondPago=-1;
           // string condPagoPedido = string.Empty;
            string   numeroFac = this.Numero;

            if (recibo.Pedido.Configuracion != null)
            {
                diasCondPago = recibo.Pedido.Configuracion.CondicionPago.DiasNeto;
                numeroFac = recibo.Pedido.Numero;
            }


            if (diasCondPago == 0)
            {
                tipoDoc = (int)TipoDocumento.NotaCreditoNueva;
                tipoDocAsoc = (int)TipoDocumento.FacturaContado;
            }
            else
            {
                tipoDoc = (int)TipoDocumento.NotaCreditoAux;
                tipoDocAsoc = (int)this.Tipo;
            }

            if (this.MontoMovimientoLocal != 0)
            {
                string sentencia =
                    " INSERT INTO " + Table.ERPADMIN_alCXC_MOV_DIR +
                    "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO, MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) " +
                    " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC,date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
            GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
            GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, (tipoDoc).ToString()),                
            GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
            GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo.Numero),
            GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, this.Numero),
            GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, (tipoDocAsoc.ToString())),
            GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, numeroFac),
            GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
            GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)recibo.Estado).ToString()), //LJR Caso 36172
            GestorDatos.SQLiteParameter("@FEC_DOC",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),                
            // mvega: se usa la funcion date('now','localtime') de SQLite
            //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
            //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
            GestorDatos.SQLiteParameter("@MON_MOV_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoMovimientoLocal,2)),
            GestorDatos.SQLiteParameter("@MON_MOV_DOL",SqlDbType.Decimal,Decimal.Round(MontoMovimientoDolar,2)),
            GestorDatos.SQLiteParameter("@MON_SAL_LOC",SqlDbType.Decimal,Decimal.Round(SaldoDocLocal,2)),
            GestorDatos.SQLiteParameter("@MON_SAL_DOL",SqlDbType.Decimal,Decimal.Round(SaldoDocDolar,2))});

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
        }


        /*
         * public void Guardar(Recibo recibo)
        {
            if (this.MontoMovimientoLocal != 0)
            {
                string sentencia =
                    " INSERT INTO "+ Table.ERPADMIN_alCXC_MOV_DIR +
                    "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO,    MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) "+
                    " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC,date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                SQLiteParameterList parametros = {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),

                if(recibo.Pedido.Configuracion.CondicionPago.Codigo == "0")

                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)TipoDocumento.NotaCreditoAux).ToString()),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo),
                GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, this.Numero),
                GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()), //LJR Caso 36172
                GestorDatos.SQLiteParameter("@FEC_DOC",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),                
                // mvega: se usa la funcion date('now','localtime') de SQLite
                //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
                //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                GestorDatos.SQLiteParameter("@MON_MOV_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoMovimientoLocal,2)),
                GestorDatos.SQLiteParameter("@MON_MOV_DOL",SqlDbType.Decimal,Decimal.Round(MontoMovimientoDolar,2)),
                GestorDatos.SQLiteParameter("@MON_SAL_LOC",SqlDbType.Decimal,Decimal.Round(SaldoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MON_SAL_DOL",SqlDbType.Decimal,Decimal.Round(SaldoDocDolar,2))};

                GestorDatos.EjecutarComando(sentencia, parametros);            
            }
        }
         * 
         * */


        /// <summary>
        /// Guarda en la base de datos las notas de credito incluidas en el recibo.
        /// </summary>
        public void GuardarPorDescuento(string recibo,TipoDocumento tipo, string numeroFactura)
        {
            if (this.MontoMovimientoLocal != 0)
            {
                string sentencia =
                    " INSERT INTO " + Table.ERPADMIN_alCXC_MOV_DIR +
                    "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO,   MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) " +
                    " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC,date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)tipo).ToString()),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo),
                GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, this.Numero),
                GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, numeroFactura),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()), //LJR Caso 36172
                GestorDatos.SQLiteParameter("@FEC_DOC",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),                
                // mvega: se usa la funcion date('now','localtime') de SQLite
                //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
                //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                GestorDatos.SQLiteParameter("@MON_MOV_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoMovimientoLocal,2)),
                GestorDatos.SQLiteParameter("@MON_MOV_DOL",SqlDbType.Decimal,Decimal.Round(MontoMovimientoDolar,2)),
                GestorDatos.SQLiteParameter("@MON_SAL_LOC",SqlDbType.Decimal,Decimal.Round(SaldoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MON_SAL_DOL",SqlDbType.Decimal,Decimal.Round(SaldoDocDolar,2))});

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
        }

        #region  Facturas de contado y recibos en FR - KFC

        /*public void ActualizarTipoDevolucion(string documento,string tipoDev)
        {
            string sentencia =
                    " UPDATE " + Table.ERPADMIN_alFAC_ENC_DEV +
                    " SET TIPO = @TIPO_PAGO " +
                    " WHERE NUM_DOC = @NUM_DOC";

            SQLiteParameterList parametros = {
                GestorDatos.SQLiteParameter("@TIPO_PAGO",SqlDbType.NVarChar, tipoDev),
                GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, documento)
                                          };
            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch(Exception)
            {
                throw;
            }

        }*/


        /// <summary>
        /// Guarda en la base de datos las notas de credito generadas por una devolucion de credito en caliente.
        /// </summary>
        public void GuardarPendienteCobro(NotaCredito nota)
        {
            if (nota.MontoDocLocal != 0)
            {
                string sentencia =
                    " INSERT INTO " + Table.ERPADMIN_alCXC_PEN_COB +
                    "       ( COD_CIA, COD_ZON, COD_TIP_DC, NUM_DOC, COD_CLT, SALDO_DOLAR, SALDO_LOCAL, MONTO_DOLAR, MONTO_LOCAL, FEC_DOC_FT, FEC_PRO,    FEC_VEN, IND_MON, IND_ANL, TIP_CMB_DOL, CONDICION_PAGO) " +
                    " VALUES(@COD_CIA,@COD_ZON,@COD_TIP_DC,@NUM_DOC,@COD_CLT,@SALDO_DOLAR,@SALDO_LOCAL,@MONTO_DOLAR,@MONTO_LOCAL,@FEC_DOC_FT,date('now','localtime'),@FEC_VEN,@IND_MON,@IND_ANL,@TIP_CMB_DOL,@CONDICION_PAGO) ";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, (15).ToString()), // TIPO DE DOCUMENTO NOTA CREDITO GENERADA EN CALIENTE - KFC
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, nota.Numero),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()), 
                GestorDatos.SQLiteParameter("@IND_MON",SqlDbType.NVarChar, ((char)this.Moneda).ToString()), 
                GestorDatos.SQLiteParameter("@FEC_DOC_FT",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),                
                // mvega: se usa la funcion date('now','localtime') de SQLite
                //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
                GestorDatos.SQLiteParameter("@FEC_VEN",SqlDbType.DateTime, FechaVencimiento),
                GestorDatos.SQLiteParameter("@TIP_CMB_DOL",SqlDbType.NVarChar, nota.TipoCambio),
                GestorDatos.SQLiteParameter("@CONDICION_PAGO",SqlDbType.NVarChar, nota.CondicionPago),

                // PA2-01482-MKP6  - KFC
                // se cambia el metodo de redondeo 
                GestorDatos.SQLiteParameter("@SALDO_DOLAR",SqlDbType.Decimal,RedondeoAlejandoseDeCero(nota.MontoDocDolar,2)),
                GestorDatos.SQLiteParameter("@SALDO_LOCAL",SqlDbType.Decimal,RedondeoAlejandoseDeCero(nota.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MONTO_LOCAL",SqlDbType.Decimal,RedondeoAlejandoseDeCero(nota.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MONTO_DOLAR",SqlDbType.Decimal,RedondeoAlejandoseDeCero(nota.MontoDocDolar,2))});


                /*GestorDatos.SQLiteParameter("@SALDO_DOLAR",SqlDbType.Decimal,Decimal.Round(nota.MontoDocDolar,2)),
                GestorDatos.SQLiteParameter("@SALDO_LOCAL",SqlDbType.Decimal,Decimal.Round(nota.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MONTO_LOCAL",SqlDbType.Decimal,Decimal.Round(nota.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MONTO_DOLAR",SqlDbType.Decimal,Decimal.Round(nota.MontoDocDolar,2))};*/


                try
                {
                    GestorDatos.EjecutarComando(sentencia, parametros);
                }
                catch (Exception)
                {
                    throw;
                }
                
            }
        }


        #region PA2-01482-MKP6  - KFC

        /// <summary>
        /// Metodo para el redondeo alejandose de cero
        /// Esta funcion se crea debido a que no sirve la sobrecarga de Math.Round  que incluye MidpointRounding
        /// en esta clase. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static decimal RedondeoAlejandoseDeCero(decimal value, int digits)
        {
            return System.Math.Round(value +
                Convert.ToDecimal(System.Math.Sign(value) / System.Math.Pow(10, digits + 1)), digits);
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "NOTA_CREDITO";
        }

        public override object GetField(string name)
        {
            return base.GetField(name);
        }

        #endregion
    }
}
