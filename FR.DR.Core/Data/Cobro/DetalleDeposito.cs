using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Detalle o linea de cada deposito
    /// </summary>
    public class DetalleDeposito
    {
        #region Variables y Propiedades de instancia

        private decimal montoCheques= 0;
        /// <summary>
        /// Monto por concepto de cheques
        /// </summary>
        public decimal MontoCheques
        {
            get { return montoCheques; }
            set { montoCheques = value; }
        }

        private decimal montoEfectivo = 0;
        /// <summary>
        /// Monto por concepto de efectivo
        /// </summary>
        public decimal MontoEfectivo
        {
            get { return montoEfectivo; }
            set { montoEfectivo = value; }
        }

        private decimal montoTotal = 0;
        /// <summary>
        /// monto total de la linea
        /// </summary>
        public decimal MontoTotal
        {
            get { return montoTotal; }
            set { montoTotal= value; }
        }

        private Recibo recibo;
        /// <summary>
        /// recibo de cobro asociada al detalle
        /// </summary>
        public Recibo Recibo
        {
            get { return recibo; }
            set { recibo = value; }
        }
        #endregion

        public DetalleDeposito() { }

        /// <summary>
        /// Asociar un recibo a la linea
        /// </summary>
        public void AsociarRecibo()
        {
            foreach (Cheque cheque in recibo.ChequesAsociados)
                this.montoCheques += cheque.Monto;

            if (recibo.Moneda == TipoMoneda.LOCAL)
            {
                this.MontoTotal = this.recibo.MontoDocLocal;
            }
            else
            {
                this.montoTotal = this.recibo.MontoDocDolar;
            }
            this.montoEfectivo = this.montoTotal - this.MontoCheques;

        }
        
        /// <summary>
        /// guardar el detalle del deposito
        /// </summary>
        /// <param name="deposito">numero de deposito asociado</param>
        public void Guardar(decimal deposito)
        {
            string referencias = Recibo.ObtenerReferenciasCheques();

            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alCXC_DET_DEP +
                "       ( NUM_DEP, NUM_DOC, MON_CHE, MON_EFE, REF) " +
                " VALUES(@NUM_DEP,@NUM_DOC,@MON_CHE,@MON_EFE,@REF)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@NUM_DEP",SqlDbType.Decimal,(decimal) deposito),                
                    GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, Recibo.Numero),
                    GestorDatos.SQLiteParameter("@MON_CHE",SqlDbType.Decimal, MontoCheques),
                    GestorDatos.SQLiteParameter("@MON_EFE",SqlDbType.Decimal, MontoEfectivo),
                    GestorDatos.SQLiteParameter("@REF",SqlDbType.NVarChar, referencias)});

            GestorDatos.EjecutarComando(sentencia, parametros);

            string sentencia2 =
                " UPDATE " + Table.ERPADMIN_alCXC_DOC_APL +
                " SET ASOC_DEP = 'S' " +
                " WHERE NUM_REC = @RECIBO";

            SQLiteParameterList parametros2 = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@RECIBO",SqlDbType.NVarChar, Recibo.Numero)});

            GestorDatos.EjecutarComando(sentencia2, parametros2);
        }
    }
}
