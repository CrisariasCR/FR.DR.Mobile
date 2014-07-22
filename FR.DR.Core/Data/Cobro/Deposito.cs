using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Clase que representa la realizacion de un deposito bancario para una compania
    /// </summary>
    public class Deposito
    {
        #region Config.xml Configuracion
 
        private static bool cambioMonto = true;
        //ABC 11/08/2008 Caso: 33488 - Depositos
        /// <summary>
        /// Indica que se permite modificar los montos de los depositos
        /// Variables cargada de Config.xml
        /// </summary>
        public static bool CambioMonto
        {
            get { return Deposito.cambioMonto; }
            set { Deposito.cambioMonto = value; }
        }

        private static decimal diferenciaMax = 0;
        //ABC 11/08/2008 Caso: 33488 - Depositos
        /// <summary>
        /// Indica el cambio maximo posible de dieferencia con el monto real del deposito
        /// Variables cargada de Config.xml
        /// </summary>
        public static decimal DiferenciaMax
        {
            get { return Deposito.diferenciaMax; }
            set { Deposito.diferenciaMax = value; }
        }

        private static decimal diferenciaMin = 0;
        //ABC 11/08/2008 Caso: 33488 - Depositos
        /// <summary>
        /// Indica el cambio min posible de dieferencia con el monto real del deposito
        /// Variables cargada de Config.xml
        /// </summary>
        public static decimal DiferenciaMin
        {
            get { return Deposito.diferenciaMin; }
            set { Deposito.diferenciaMin = value; }
        }

        #endregion

        #region Variables y Propiedades de instancia

        private decimal numero = 0;
        /// <summary>
        /// Numero del recibo
        /// </summary>
        public decimal Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        private decimal montoCheques = 0;
        /// <summary>
        /// Monto en cheques depositado
        /// </summary>
        public decimal MontoCheques
        {
            get { return montoCheques; }
            set { montoCheques = value; }
        }

        private decimal montoEfectivo = 0;
        /// <summary>
        /// Monto en efectivo depositado
        /// </summary>
        public decimal MontoEfectivo
        {
            get { return montoEfectivo; }
            set { montoEfectivo = value; }
        }

        private decimal montoTotal = 0;
        /// <summary>
        /// Monto que se esta depositando
        /// </summary>
        public decimal MontoTotal
        {
            get { return montoTotal; }
            set { montoTotal = value; }
        }

        private DateTime fechaRealizacion = DateTime.Now;
        /// <summary>
        /// Fecha en que se esta generando el deposito
        /// </summary>
        public DateTime FechaRealizacion
        {
            get { return fechaRealizacion; }
            set { fechaRealizacion = value; }
        }

        private string compania = string.Empty;
        /// <summary>
        /// Compania asociada al deposito
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); }
        }

        private decimal tipoCambio = 0;
        /// <summary>
        /// Monto Tipo Cambio
        /// </summary>
        public decimal TipoCambio
        {
            get { return tipoCambio; }
            set { tipoCambio = value; }
        }

        private BancoAsociado banco = new BancoAsociado();
        /// <summary>
        /// Banco donde se realiza el deposito.
        /// </summary>
        public BancoAsociado Banco
        {
            get { return banco; }
            set { banco = value; }
        }

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Tipo de moneda del deposito.
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private List<DetalleDeposito> detalles = new List<DetalleDeposito>();
        /// <summary>
        /// Lista de los detalles del deposito, contiene los recibos asociados
        /// </summary>
        public List<DetalleDeposito> Detalles
        {
            get { return detalles; }
            set { detalles = value; }
        }

        #endregion

        #region Logica Negocios

        /// <summary>
        /// Agrega un recibo a la lista de detalles del deposito
        /// </summary>
        /// <param name="docAfectado"></param>
        public void AgregarDetalle(Recibo docAfectado)
        {
            DetalleDeposito detalle = new DetalleDeposito();
            detalle.Recibo = docAfectado;
            detalle.AsociarRecibo();
            this.Detalles.Add(detalle);
        }

        /// <summary>
        /// Hace el deposito y sus detalles persistentes en la base de datos.
        /// </summary>
        /// <param name="monto">Monto del Deposito Cambiado</param>
        //ABC 11/08/2008 Caso:33488
        //Se incluye nuevo parametro si se hizo un cambio de monto del deposto.
        public void GuardaDepositoEnBD(string monto)
        {
            GestorDatos.BeginTransaction();
            
            decimal nuevoMonto = 0;
            try
            {
                //ABC 11/08/2008 Caso:33488
                //Se procede a validar el nuevo monto antes de almacenarlos en la base de datos.
                if (monto != string.Empty)
                {
                    nuevoMonto = GestorUtilitario.ParseDecimal(monto);
                    if (this.calcularDiferencia(nuevoMonto))
                        this.MontoTotal = nuevoMonto;
                    else
                    {
                        throw new Exception("El monto a depositar no se puede tramitar, la diferencia Máxima es de: " + GestorUtilitario.SimboloMonetario + Deposito.DiferenciaMax + " y la diferencia Mínima es de: " + GestorUtilitario.SimboloMonetario + Deposito.DiferenciaMin);
                    }
                }

                DBGuardar();
                foreach (DetalleDeposito det in this.Detalles)
                    det.Guardar(numero);

                GestorDatos.CommitTransaction();
            }
            catch (SQLiteException exc)
            {
                GestorDatos.RollbackTransaction();
                throw new Exception("Error al guardar el depósito. " + exc.Message);
            }
            catch (Exception exc)
            {
                GestorDatos.RollbackTransaction();
                throw new Exception("Error al guardar el depósito. " + exc.Message);
            }
        }
        
        /// <summary>
        /// Calcula el monto en cheques efectivo y total de el deposito
        /// </summary>
        public void CalculeMontoTotalDeposito()
        {
            this.MontoCheques = 0;
            this.MontoEfectivo = 0;
            this.MontoTotal = 0;

            foreach (DetalleDeposito det in this.Detalles)
            {
                if (det.Recibo.Moneda == TipoMoneda.LOCAL)
                {
                    if (Banco.Moneda == TipoMoneda.LOCAL)
                    {
                        this.MontoTotal += det.MontoTotal;
                        this.MontoCheques += det.MontoCheques;
                        this.MontoEfectivo += det.MontoEfectivo;
                    }
                    else
                    {
                        this.MontoTotal += det.MontoTotal / this.TipoCambio;
                        this.MontoCheques += det.MontoCheques / this.TipoCambio;
                        this.MontoEfectivo += det.MontoEfectivo / this.TipoCambio;
                    }
                }
                else
                {
                    if (banco.Moneda == TipoMoneda.LOCAL)
                    {
                        this.MontoTotal += det.MontoTotal * this.TipoCambio;
                        this.MontoCheques += det.MontoCheques * this.TipoCambio;
                        this.MontoEfectivo += det.MontoEfectivo * this.TipoCambio;
                    }
                    else
                    {
                        this.MontoTotal += det.MontoTotal;
                        this.MontoCheques += det.MontoCheques;
                        this.MontoEfectivo += det.MontoEfectivo;
                    }
                }
            }
        }
        /// <summary>
        /// ABC 11/08/2008 Caso:33488
        /// Calcula la diferencia entre el monto real vs el monto incluido por el usuario.
        /// </summary>
        /// <param name="montoNuevo"></param>
        /// <returns></returns>
        private bool calcularDiferencia(decimal montoNuevo)
        {
            decimal diferencia = montoNuevo - this.MontoTotal;

            if (diferencia >= 0 && diferencia <= Deposito.DiferenciaMax)
            {
                return true;
            }
            else if (diferencia < 0 && (diferencia * -1) <= Deposito.DiferenciaMin)
            {
                return true;
            }
            return false;
        }
        
        #endregion

        #region Acceso Datos

        /// <summary>
        /// Guarda el encabezado del deposito generado
        /// </summary>
        private void DBGuardar()
        {
            string sentencia = 
                " INSERT INTO " + Table.ERPADMIN_alCXC_ENC_DEP +
                "       ( CTA_BCO, COD_BCO, COD_CIA, NUM_DEP, MON_DEP, FEC_DEP, IND_MON) "+
                " VALUES(@CTA_BCO,@COD_BCO,@COD_CIA,@NUM_DEP,@MON_DEP,@FEC_DEP,@IND_MON) ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CTA_BCO",SqlDbType.NVarChar, banco.Cuenta),                
                GestorDatos.SQLiteParameter("@COD_BCO",SqlDbType.NVarChar, banco.Codigo),
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, compania),
                GestorDatos.SQLiteParameter("@NUM_DEP",SqlDbType.NVarChar, numero),
                GestorDatos.SQLiteParameter("@MON_DEP",SqlDbType.Decimal, montoTotal),
                GestorDatos.SQLiteParameter("@FEC_DEP",SqlDbType.DateTime, fechaRealizacion),
                GestorDatos.SQLiteParameter("@IND_MON", SqlDbType.NVarChar, ((char)this.Moneda).ToString()) });

            GestorDatos.EjecutarComando(sentencia, parametros);

        }

        //Caso 29219 LDS 30/07/2007
        /// <summary>		
        /// Valida que el número del depósito debe ser único en cuanto a: Banco y Cuenta Bancaria definidos en el depósito.
        /// </summary>
        /// <returns>
        /// Retorna true en caso de que exista el número del depósito, por lo tanto no se debe hacer uso de ese número.
        /// </returns>
        public bool ExisteNumeroDeposito()
        {
            bool Existe = false;

            string sentencia = 
                " SELECT COUNT(*) FROM " + Table.ERPADMIN_alCXC_ENC_DEP + 
                " WHERE COD_BCO = @BANCO" + 
                " AND CTA_BCO = @CUENTA" + 
                " AND NUM_DEP = @DEPOSITO" ;
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@BANCO", Banco.Codigo),
                      new SQLiteParameter("@CUENTA", Banco.Cuenta),
                      new SQLiteParameter("@DEPOSITO", numero)}); 
            try
            {
                object valor = GestorDatos.EjecutarScalar(sentencia,parametros);
                if (valor != DBNull.Value && Convert.ToInt32(valor.ToString()) > 0)
                        Existe = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validando la existencia del número del depósito. " + ex.Message);
            }

            return Existe;
        }
        #endregion
    }
}
