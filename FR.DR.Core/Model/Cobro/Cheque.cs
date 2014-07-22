using System;
using System.Collections.Generic;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;
using System.Data.SQLiteBase;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Clase que representa un cheque como foma de pago en cobros
    /// </summary>
    public class Cheque : Encabezado, IPrintable
    {

        #region Variables y Propiedades de instancia

        private BancoAsociado banco = new BancoAsociado();
        /// <summary>
        /// Banco asociada al cliente
        /// </summary>
        public BancoAsociado Banco
        {
            get { return banco; }
            set { banco = value; }
        }

        private string recibo = string.Empty;
        /// <summary>
        /// Numero de recibo asociado al cheque
        /// </summary>
        public string Recibo
        {
            get { return recibo; }
            set { recibo = value; }
        }

        private decimal monto = 0;
        /// <summary>
        /// Monto del cheque
        /// </summary>
        public decimal Monto
        {
            get { return monto; }
            set { monto = value; }
        }

        private DateTime fecha = DateTime.Now;
        /// <summary>
        /// Fecha de emision
        /// </summary>
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Tipo Moneda Cheque
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }
        /// <summary>
        /// Constructor del cheque
        /// </summary>
        /// <param name="zona">zona asociada al documento que emite el cheque</param>
        /// <param name="cliente">cliente asociada al documento que emite el cheque</param>
        /// <param name="compania">compania asociada al cheque</param>
        public Cheque(string zona, string cliente, string compania)
        {
            this.Zona = zona;
            this.Cliente = cliente;
            this.Compania = compania;
        }
        public Cheque()
        { }

        public List<Cheque> ObtenerChequesRecibo() { return null; }
        #endregion

        #region Acceso Datos

        /// <summary>
        /// Guarda en la base de datos el cheque.
        /// </summary>
        /// <param name="transaccion"></param>
        /// <param name="tipoDoc">Tipo de documento asociado al cheque.</param>
        public void Guardar(TipoDocumento tipo)
        {
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alCXC_CHE  +
                "        ( COD_CIA, COD_ZON, COD_BCO, COD_CLT, NUM_REC, NUM_CHE, NUM_CTA, FEC_CHE, MON_CHE, TIP_DOC) "+
                " VALUES (@COD_CIA,@COD_ZON,@COD_BCO,@COD_CLT,@NUM_REC,@NUM_CHE,@NUM_CTA,@FEC_CHE,@MON_CHE,@TIP_DOC)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_BCO",SqlDbType.NVarChar, banco.Codigo),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo),
                GestorDatos.SQLiteParameter("@NUM_CHE",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@NUM_CTA",SqlDbType.NVarChar, banco.Cuenta),
                GestorDatos.SQLiteParameter("@FEC_CHE",SqlDbType.DateTime, fecha),
                GestorDatos.SQLiteParameter("@MON_CHE",SqlDbType.Decimal, monto),
                GestorDatos.SQLiteParameter("@TIP_DOC", SqlDbType.NVarChar,((int)tipo).ToString()) });

            GestorDatos.EjecutarComando(sentencia, parametros);
        }

        /// <summary>
        /// Carga los cheques que se giraron para la cancelacion del recibo
        /// </summary>
        /// <param name="recibo">numero de recibo</param>
        /// <param name="compania">compania asocida al recibo de cobro</param>
        /// <param name="cliente">cliente asociada al recibo de cobro</param>
        /// <returns></returns>
        public static List<Cheque> ObtenerChequesAplicados(string recibo, string compania, string cliente)
        {
            List<Cheque> cheques = new List<Cheque>();
            cheques.Clear();
            
            SQLiteDataReader reader = null;

            try
            {
                string sentencia =
                    " SELECT B.COD_BCO,B.DES_BCO,C.NUM_CTA,C.NUM_CHE,C.MON_CHE,C.FEC_CHE " +
                    " FROM " + Table.ERPADMIN_alCXC_CHE +" C, " +  Table.ERPADMIN_alCXC_BCO  + " B " +
                    " WHERE UPPER(C.COD_CIA) = @COMPANIA " +
                    " AND   C.COD_CLT = @CLIENTE " +
                    " AND   C.NUM_REC = @RECIBO " +
                    " AND   C.COD_BCO = B.COD_BCO " +
                    " AND   C.COD_CIA = B.COD_CIA";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                    new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                    new SQLiteParameter("@RECIBO", recibo),
                    new SQLiteParameter("@CLIENTE", cliente)}); 

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Cheque cheque = new Cheque();
                    cheque.Banco = new BancoAsociado(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                    cheque.Numero= reader.GetString(3);
                    cheque.Monto = reader.GetDecimal(4);
                    cheque.fecha = reader.GetDateTime(5);
                    cheques.Add(cheque);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando cheques asociados. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return cheques;
        }
        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "CHEQUE";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                case "NUMERO": return Numero;
                case "CUENTA": return banco.Cuenta;
                case "NOMBRE_BANCO": return banco.Descripcion;
                case "CODIGO_BANCO": return banco.Codigo;
                case "MONTO":return monto;
                default: return string.Empty;
            }
        }

        #endregion
    }
}
