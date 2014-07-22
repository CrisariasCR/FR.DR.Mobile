using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using EMF.Printing;
using System.Data;
using System.Collections;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    public class Factura : DocumentoContable, IPrintable
    {
        #region Variables y Propiedades de instancia
                
        private List<NotaCredito> notasCreditoAplicadas = new List<NotaCredito>();
        /// <summary>
        /// Notas de credito aplicadas a la factura
        /// </summary>
        public List<NotaCredito> NotasCreditoAplicadas
        {
            get { return notasCreditoAplicadas; }
            set { notasCreditoAplicadas = value; }
        }
        /// <summary>
        /// Retorna el total de notas aplicadas a la factura. El valor devuelto esta en la moneda de la factura.
        /// </summary>
        public decimal TotalNotasCredito
        {

            get
            {
                decimal total_notas = 0;

                foreach (NotaCredito nota in this.NotasCreditoAplicadas)
                {
                    if (this.Moneda == TipoMoneda.LOCAL)
                        total_notas += nota.MontoDocLocal;
                    else
                        total_notas += nota.MontoDocDolar;
                }
                return total_notas;
            }
        }

        /// <summary>
        /// propiedad que indica si la factura está seleccionada
        /// </summary>
        private bool seleccionado;
        public bool Seleccionado
        {
            get
            {
                return seleccionado;
            }
            set
            {
                bool oldValue = value;
                seleccionado = value;
                if (ItemCheck != null)
                {
                    ItemCheck(this, oldValue);
                }
            }
        }

        public static Action<Factura, bool> ItemCheck { get; set; }

        /// <summary>
        /// monto a pagar por el documento
        /// </summary>
        public decimal MontoAPagarViewLocal
        {
            get { return MontoAPagarDocLocal == 0? SaldoDocLocal: MontoAPagarDocLocal; }
        }

        /// <summary>
        /// Monto a pagar por el documento
        /// </summary>
        public decimal MontoAPagarViewDolar
        {
            get { return MontoAPagarDocDolar == 0 ? SaldoDocDolar : MontoAPagarDocDolar; }
        }


        #endregion

        /// <summary>
        /// Constructor de la factura
        /// </summary>
        public Factura()
        {
            this.Tipo = TipoDocumento.Factura;
        }
        /// <summary>
        /// Convertir monto local a dolar
        /// </summary>
        /// <returns></returns>
        public decimal LocalADolar()
        {
            return Decimal.Round(MontoDocLocal / TipoCambio, 2);
        }

        /// <summary>
        /// Convertir monto local a dolar
        /// </summary>
        /// <returns></returns>
        public decimal DolarALocal()
        {
            return Decimal.Round(MontoDocDolar  * TipoCambio, 2);
        }

        /// <summary>
        /// Aplica la nota de credito a la factura
        /// </summary>
        /// <param name="notaCredito"></param>
        public void AplicaNotaLocal(NotaCredito notaCredito)
        {
            decimal calculo = this.MontoAPagarDocLocal - notaCredito.SaldoDocLocal;            

            if (calculo <= 0)//verifica que el monto del calculo sea menor o igual a 0
            {
                //Cobros.MontoNotasCreditoSeleccion += this.MontoAPagarDocLocal;//se le suma a el monto al monto de notas de credito el monto a pagar por la factura
                Cobros.MontoNotasCreditoSeleccion += this.MontoAPagarDocLocal;//se le suma a el monto al monto de notas de credito el monto a pagar por la factura

                //el saldo de la nota se le asigna el monto que dio la resta de los montos
                //pero se niega el calculo ya que este es negativo
                notaCredito.SaldoDocLocal -= this.MontoAPagarDocLocal;
                notaCredito.SaldoDocDolar -= this.MontoAPagarDocDolar;

                notaCredito.MontoMovimientoLocal += this.MontoAPagarDocLocal;//al monto movimineto de la nota se le suma el monto a pagar por la factura
                notaCredito.MontoMovimientoDolar += this.MontoAPagarDocDolar;

                this.SaldoDocLocal -= this.MontoAPagarDocLocal;//el saldo de la factura se le resta el monto a pagar de la misma
                this.SaldoDocDolar -= this.MontoAPagarDocDolar;

                //se guarda el numero de nota y el monto aplicado en un arreglo asi como el saldo de la factura
                NotaCredito notaAplicada = new NotaCredito();
                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = this.MontoAPagarDocLocal;
                notaAplicada.MontoDocDolar = this.MontoAPagarDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                this.MontoAPagarDocLocal = 0;//el total a pagar de la factura se iguala a 0
                this.MontoAPagarDocDolar = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;                

            }
            else
            {
                this.MontoAPagarDocLocal = calculo;//el monto a pagar es igual al calculo
                //KFC - se agrega la actualizacion al monto en dolares
                this.MontoAPagarDocDolar = calculo / Cobros.TipoCambio;

                Cobros.MontoNotasCreditoSeleccion += notaCredito.SaldoDocLocal;//se le suma  al monto de notas de credito el saldo de la nota de credito

                notaCredito.MontoMovimientoLocal += notaCredito.SaldoDocLocal;//monto del movimiento de la nota se le suma el saldo de la nota
                notaCredito.MontoMovimientoDolar += notaCredito.SaldoDocDolar;

                this.SaldoDocLocal -= notaCredito.SaldoDocLocal;//al saldo de la factura se le resta el saldo de la nota
                this.SaldoDocDolar -= notaCredito.SaldoDocDolar;

                NotaCredito notaAplicada = new NotaCredito();

                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = notaCredito.SaldoDocLocal;
                notaAplicada.MontoDocDolar = notaCredito.SaldoDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                notaCredito.SaldoDocLocal = 0;//se iguala saldo de la nota a 0
                notaCredito.SaldoDocDolar = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;
            }
            //notaCredito.indProcesado = true;//se cambia el indice de procesamineto de la nota
            //this.indProcesado = true;//se cambia el indice de procesamineto de la factura
        }

        /// <summary>
        /// Aplica la nota de credito a la factura
        /// </summary>
        /// <param name="notaCredito">nota de credito a aplicar</param>
        /// <returns>retorna un objeto de tipo documento</returns>
        public void AplicaNotaDolar(NotaCredito notaCredito)
        {
            decimal calculo = this.MontoAPagarDocDolar - notaCredito.SaldoDocDolar;

            if (calculo <= 0)//verifica que el monto del calculo sea menor o igual a 0
            {
                Cobros.MontoNotasCreditoSeleccion += this.MontoAPagarDocDolar;//se le suma a el monto al monto de notas de credito el monto a pagar por la factura

                //el saldo de la nota se le asigna el monto que dio la resta de los montos
                //pero se niega el calculo ya que este es negativo
                notaCredito.SaldoDocDolar = -calculo;
                notaCredito.SaldoDocLocal = notaCredito.SaldoDocDolar * Cobros.TipoCambio;

                notaCredito.MontoMovimientoDolar += this.MontoAPagarDocDolar;//al monto movimineto de la nota se le suma el monto a pagar por la factura
                notaCredito.MontoMovimientoLocal += this.MontoAPagarDocLocal;

                this.SaldoDocDolar -= notaCredito.SaldoDocDolar;//el saldo de la factura se le resta el monto a pagar de la misma
                this.SaldoDocLocal -= notaCredito.SaldoDocLocal;

                NotaCredito notaAplicada = new NotaCredito();

                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = this.MontoAPagarDocLocal;
                notaAplicada.MontoDocDolar = this.MontoAPagarDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;
   
                this.MontoAPagarDocDolar= 0;//el total a pagar de la factura se iguala a 0
                this.MontoAPagarDocLocal = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;
            }
            else
            {
                Cobros.MontoNotasCreditoSeleccion += notaCredito.SaldoDocDolar;//se le suma  al monto de notas de credito el saldo de la nota de credito

                notaCredito.MontoMovimientoDolar += notaCredito.SaldoDocDolar;//monto del movimiento de la nota se le suma el saldo de la nota
                notaCredito.MontoMovimientoLocal += notaCredito.SaldoDocLocal;

                this.MontoAPagarDocDolar -= notaCredito.SaldoDocDolar;//al saldo de la factura se le resta el saldo de la nota
                this.MontoAPagarDocLocal -= notaCredito.SaldoDocLocal;

                this.SaldoDocDolar -= notaCredito.SaldoDocDolar;
                this.SaldoDocLocal -= notaCredito.SaldoDocLocal;//al saldo de la factura se le resta el saldo de la nota

                NotaCredito notaAplicada = new NotaCredito();

                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = notaCredito.SaldoDocLocal;
                notaAplicada.MontoDocDolar = notaCredito.SaldoDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                notaCredito.SaldoDocDolar = 0;//se iguala saldo de la nota a 0
                notaCredito.SaldoDocLocal = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;
            }
        }

        #region Acceso Datos

        /// <summary>
        /// Verifica si el cliente tiene facturas vencidas. 
        /// </summary>
        /// <param name="cliente">cliente a verificar</param>
        /// <param name="documento">tipo de factura</param>
        /// <param name="vencidas">si incluye solo facturas vencidas</param>
        /// <returns>si tiene pendientes</returns>
        public static bool TieneFacturasPendientes(string cliente, TipoDocumento documento, bool vencidas)
        {
            SQLiteParameterList parametros;

            string sentencia =
                " SELECT COUNT(NUM_DOC) FROM " + Table.ERPADMIN_alCXC_PEN_COB;

            if (documento == TipoDocumento.TodosDocumentosDebito)
                sentencia = sentencia + " WHERE COD_TIP_DC IN (@TIPO1, @TIPO2, @TIPO3, @TIPO4, @TIPO5, @TIPO6, @TIPO7) ";
            else
                sentencia = sentencia + " WHERE COD_TIP_DC = @TIPO";

            sentencia = sentencia +" AND   COD_CLT = @CLIENTE " +
            " AND   SALDO_LOCAL > 0 AND SALDO_DOLAR > 0 " +
            ((vencidas) ? " AND   FEC_VEN < GETDATE() " : string.Empty);

            if (documento == TipoDocumento.TodosDocumentosDebito)
            {
                parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                  new SQLiteParameter("@CLIENTE", cliente),
                  new SQLiteParameter("@TIPO1", ((int)TipoDocumento.Factura).ToString()),
                  new SQLiteParameter("@TIPO2", ((int)TipoDocumento.NotaDebito).ToString()),
                  new SQLiteParameter("@TIPO3", ((int)TipoDocumento.LetraCambio).ToString()),
                  new SQLiteParameter("@TIPO4", ((int)TipoDocumento.OtroDebito).ToString()),
                  new SQLiteParameter("@TIPO5", ((int)TipoDocumento.Intereses).ToString()),
                  new SQLiteParameter("@TIPO6", ((int)TipoDocumento.BoletaVenta).ToString()),
                  new SQLiteParameter("@TIPO7", ((int)TipoDocumento.InteresCorriente).ToString())});

            }
            else
            {
                parametros = new SQLiteParameterList();
                parametros.Add("@CLIENTE", cliente);
                parametros.Add("@TIPO", ((int)documento).ToString());
            }

            int cantidad = Convert.ToInt32(GestorDatos.EjecutarScalar(sentencia,parametros));

            if (cantidad > 0)
                return true;

            return false;
        }

        public static bool TieneDocumentosPendientes(string cliente, bool vencidas)
        {
            return TieneFacturasPendientes(cliente, TipoDocumento.TodosDocumentosDebito, vencidas);
        }

        /// <summary>
        /// Obtener los pendientes de cobro de un cliente en una cia
        /// </summary>
        /// <param name="compania">compania asociada</param>
        /// <param name="cliente">cliente asociado</param>
        /// <returns></returns>
        public static List<Factura> ObtenerFacturasPendientesCobro(string compania, string cliente, string zona)
        {
            SQLiteDataReader reader = null;
            List<Factura> facturas = new List<Factura>();
            facturas.Clear();
            //Cargamos los pendientes de cobro de manera que 
            //las primeras facturas que se obtengan sean las 
            //facturas de contado y luego las de credito. 
            //Luego necesitamos que las de credito se ordenen por fecha de vencimiento.

            //LJR: Agrega zona  para clientes en multiples rutas
            string sentencia =
                " SELECT DISTINCT COD_ZON,NUM_DOC,FEC_DOC_FT,FEC_PRO,FEC_VEN,MONTO_LOCAL,SALDO_LOCAL,MONTO_DOLAR,SALDO_DOLAR," +
                " COD_TIP_DC,IND_MON,TIP_CMB_DOL,CONDICION_PAGO FROM " + Table.ERPADMIN_alCXC_PEN_COB +
                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND   COD_CLT = @CLIENTE" +
                " AND   IND_ANL IN(@ESTADO,@ESTADO2)" +
                " AND   SALDO_DOLAR != 0 AND SALDO_LOCAL != 0 " +
                " AND   COD_TIP_DC IN (@TIPO1, @TIPO2, @TIPO3, @TIPO4, @TIPO5, @TIPO6, @TIPO7, @TIPO8) " +
                " AND   COD_ZON = @ZONA" +
                " ORDER BY COD_TIP_DC, FEC_VEN ASC";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                      new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Activo).ToString()),
                      new SQLiteParameter("@ESTADO2", "Activo"),
                      new SQLiteParameter("@TIPO1", ((int)TipoDocumento.Factura).ToString()),
                      new SQLiteParameter("@TIPO2", ((int)TipoDocumento.NotaDebito).ToString()),
                      new SQLiteParameter("@TIPO3", ((int)TipoDocumento.LetraCambio).ToString()),
                      new SQLiteParameter("@TIPO4", ((int)TipoDocumento.OtroDebito).ToString()),
                      new SQLiteParameter("@TIPO5", ((int)TipoDocumento.Intereses).ToString()),
                      new SQLiteParameter("@TIPO6", ((int)TipoDocumento.BoletaVenta).ToString()),
                      new SQLiteParameter("@TIPO7", ((int)TipoDocumento.InteresCorriente).ToString()),
                      new SQLiteParameter("@TIPO8", ((int)TipoDocumento.FacturaContado).ToString()),
                      new SQLiteParameter("@ZONA", zona)}); 
                     // new SQLiteParameter("@CONTADO", 0/**/)}; 
            try
            {               
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);
                while (reader.Read())
                {
                    Factura factura = new Factura();
                    factura.Compania = compania;
                    factura.Cliente= cliente;
                    factura.Zona = reader.GetString(0);
                    factura.Numero = reader.GetString(1);
                    factura.FechaRealizacion = reader.GetDateTime(2);
                    factura.FechaUltimoProceso = reader.GetDateTime(3);
                    factura.FechaVencimiento = reader.GetDateTime(4);
                    factura.MontoDocLocal= reader.GetDecimal(5);
                    factura.SaldoDocLocal = reader.GetDecimal(6);
                    factura.MontoDocDolar = reader.GetDecimal(7);
                    factura.SaldoDocDolar = reader.GetDecimal(8);
                    factura.Estado = EstadoDocumento.Activo;
                    factura.Tipo = (TipoDocumento) Convert.ToInt32(reader.GetString(9));
                    factura.Moneda = (TipoMoneda) Convert.ToChar(reader.GetString(10));
                    factura.TipoCambio = reader.GetDecimal(11);
                    factura.CondicionPago = reader.GetString(12);
                    //Existen montos muy pequenos que no tienen que tomarse en cuenta
                    if (Math.Round(factura.SaldoDocLocal, 3) > 0 ||
                        Math.Round(factura.SaldoDocDolar, 3) > 0)
                        facturas.Add(factura);
                }
                return facturas;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo facturas pendientes de cobro " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Verifica en la base de datos si la factura tiene aplicaciones realizadas.
        /// </summary>
        public bool TieneAplicaciones()
        {
            string sentencia =
                " SELECT COUNT(NUM_DOC) " +
                " FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE NUM_DOC_AF = @DOC" +
                " AND COD_TIP_DA = @TIPO" + 
                " AND IND_ANL = @ESTADO" ;
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@DOC", this.Numero),
                      new SQLiteParameter("@TIPO", (int)this.Tipo),
                      new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Activo).ToString())});

            int cantidad = Convert.ToInt32(GestorDatos.EjecutarScalar(sentencia,parametros));

                if (cantidad > 0)
                    return true;

                return false;
        }

        /// <summary>
        /// Guarda la factura en las facturas pendientes de cobro.
        /// </summary>
        public void Guardar()
        {
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alCXC_PEN_COB  +
                "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_DOC, COD_CLT, FEC_DOC_FT, FEC_PRO, FEC_VEN, IND_MON, IND_ANL, SALDO_DOLAR, SALDO_LOCAL, MONTO_DOLAR, MONTO_LOCAL, TIP_CMB_DOL, CONDICION_PAGO) " +
                " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_DOC,@COD_CLT,@FEC_DOC_FT,@FEC_PRO,@FEC_VEN,@IND_MON,@IND_ANL,@SALDO_DOLAR,@SALDO_LOCAL,@MONTO_DOLAR,@MONTO_LOCAL,@TIP_CMB_DOL, @CONDICION_PAGO) "; 

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                    new SQLiteParameter("@COD_CIA", this.Compania),
                    new SQLiteParameter("@COD_TIP_DC", ((int)Tipo).ToString()),
                    new SQLiteParameter("@COD_ZON", this.Zona),
                    new SQLiteParameter("@NUM_DOC", this.Numero),                              
                    new SQLiteParameter("@COD_CLT", this.Cliente),
                    new SQLiteParameter("@FEC_DOC_FT", this.FechaRealizacion.ToShortDateString()),
                    new SQLiteParameter("@FEC_PRO", this.FechaUltimoProceso.ToShortDateString()),
                    new SQLiteParameter("@FEC_VEN", this.FechaVencimiento.ToShortDateString()),
                    new SQLiteParameter("@IND_MON", ((char)this.Moneda).ToString()),
                    new SQLiteParameter("@IND_ANL",((char)this.Estado).ToString()),
                    new SQLiteParameter("@SALDO_DOLAR", this.SaldoDocDolar),
                    new SQLiteParameter("@SALDO_LOCAL", this.SaldoDocLocal),
                    new SQLiteParameter("@MONTO_DOLAR", this.MontoDocDolar),
                    new SQLiteParameter("@MONTO_LOCAL", this.MontoDocLocal),
                    new SQLiteParameter("@TIP_CMB_DOL", this.TipoCambio),
                    new SQLiteParameter("@CONDICION_PAGO",this.CondicionPago)}); 

            int factura = GestorDatos.EjecutarComando(sentencia, parametros);
            if (factura != 1)
                throw new Exception("No se generó el pendiente de cobro para la factura '" + this.Numero + "'.");
        }

        /// <summary>
        /// Borra la factura pendiente de cobro de la base de datos.
        /// </summary>
        public void Eliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alCXC_PEN_COB +
                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND   COD_TIP_DC = @TIPO" +
                " AND   NUM_DOC = @CONSECUTIVO" +
                " AND   COD_CLT = @CLIENTE";
             
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@COMPANIA", this.Compania.ToUpper()),
                      new SQLiteParameter("@TIPO", (int)this.Tipo),
                      new SQLiteParameter("@CONSECUTIVO", this.Numero),
                      new SQLiteParameter("@CLIENTE", this.Cliente)});

            int afectados = GestorDatos.EjecutarComando(sentencia, parametros);

            if (afectados > 1)
                throw new Exception("Más de un documento afectado.");
            if (afectados < 1)
                throw new Exception("Ningún documento afectado.");
        }

        /// <summary>
        /// Verifica si el cliente tiene facturas vencidas. 
        /// </summary>
        /// <param name="cliente">cliente a verificar</param>
        /// <param name="documento">tipo de factura</param>
        /// <param name="vencidas">si incluye solo facturas vencidas</param>
        /// <returns>si tiene pendientes</returns>
        public static bool ValidaLimiteCredito(string cliente, string compania, decimal montoFactActual)
        {
            bool result = false;
            decimal saldo = decimal.Zero;
            decimal limiteCredito = decimal.Zero;
            SQLiteDataReader reader = null;

            string sentencia =
                " SELECT SUM(SALDO_LOCAL) as LOCAL FROM " + Table.ERPADMIN_alCXC_PEN_COB +
                " WHERE COD_TIP_DC IN ('1', '2', '10', '11', '12', '13', '14') " +
                " AND   COD_CLT = '" + cliente + "' " +
                " AND   SALDO_LOCAL > 0 AND SALDO_DOLAR > 0 ";

            try
            {

                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        saldo = reader.GetDecimal(0);
                    //kfc batcca
                    saldo = saldo + montoFactActual;
                }

                sentencia = string.Empty;
            }
            catch (Exception ex) { }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }
            try
            {

                sentencia =
                    " SELECT LIM_CRE FROM " + Table.ERPADMIN_CLIENTE_CIA +
                    " WHERE COD_CIA = '" + compania + "' " +
                    " AND   COD_CLT = '" + cliente + "' ";

                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        limiteCredito = reader.GetDecimal(0);
                }

                if (saldo > 0 && limiteCredito > 0 && saldo >= limiteCredito)
                    result = true;
            }
            catch (Exception ex) { }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }
            return result;
        }

        #region Insert de el registro del recibo de dinero por factura

        /// <summary>
        /// Guarda en la base de datos las facturas con el registro 
        /// del recibo al que pertenece y el monto del mismo que se le aplico.
        /// Corresponde a movimientos de efectivo y cheques
        /// </summary>
        /// <param name="recibo">consecutivo de recibo</param>
        public void GuardarRecibo(string recibo)
        {
            if (this.MontoMovimientoLocal != 0)
            {
                string sentencia =
                    " INSERT INTO "+ Table.ERPADMIN_alCXC_MOV_DIR +
                    "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO,    MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) "+
                    " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC,date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)TipoDocumento.Recibo).ToString()),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo),
                GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, recibo),
                GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()),//LJR Caso 36172
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
        
        #endregion

        #region Insert de las aplicaciones de las notas a las facturas en la BD

        /// <summary>
        /// Guarda en la base de datos la informacion que se genero de 
        /// las facturas que se vieron afectadas por las notas de credito (movimientos de notas)
        /// </summary>
        public void GuardarAfectadaNC(string recibo)
        {
            if (NotasCreditoAplicadas.Count>0)
            {
                foreach (NotaCredito notaAplicada in NotasCreditoAplicadas)
                {
                    string sentencia =
                        " INSERT INTO " + Table.ERPADMIN_alCXC_MOV_DIR +
                        "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO,    MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) " +
                        " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC,date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                    GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)notaAplicada.Tipo).ToString()),
                    GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                    GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo),
                    GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, notaAplicada.Numero),
                    GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                    GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, Numero),
                    GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                    GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()),
                    GestorDatos.SQLiteParameter("@FEC_DOC",SqlDbType.DateTime, FechaRealizacion),                
                    // mvega: se usa la funcion date('now','localtime') de SQLite
                    //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
                    //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                    GestorDatos.SQLiteParameter("@MON_MOV_LOCAL",SqlDbType.Decimal,Decimal.Round(notaAplicada.MontoDocLocal,2)),
                    GestorDatos.SQLiteParameter("@MON_MOV_DOL",SqlDbType.Decimal,Decimal.Round(notaAplicada.MontoDocDolar,2)),
                    GestorDatos.SQLiteParameter("@MON_SAL_LOC",SqlDbType.Decimal,Decimal.Round(notaAplicada.SaldoDocLocal,2)),
                    GestorDatos.SQLiteParameter("@MON_SAL_DOL",SqlDbType.Decimal,Decimal.Round(notaAplicada.SaldoDocDolar,2))});

                    GestorDatos.EjecutarComando(sentencia, parametros);
                }

            }
        }
        #endregion

        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "FACTURA";
        }

        public override object GetField(string name)
        {

            if (name == "TIENE_NOTAS_APLICADAS")
                return (this.NotasCreditoAplicadas.Count>0);
            
            if (name == "NOTAS_CREDITO_APLICADAS")
                return new ArrayList(NotasCreditoAplicadas);

            return base.GetField(name);

        }

        #endregion
    }
}
