using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Clase que representa la cuenta de un banco en particular
    /// </summary>
    public class CuentaBanco
    {
        #region Variables y Propiedades de instancia

        private string numero = string.Empty;
        /// <summary>
        /// Numero de la cuenta bancaria.
        /// </summary>
        public string Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        private string descripcion = string.Empty;
        /// <summary>
        /// Descripcion de la cuenta bancaria.
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Moneda de la cuenta bancaria.
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        #endregion
        
        /// <summary>
        /// Constructor de la cuenta bancaria
        /// </summary>
        public CuentaBanco() { }

        /// <summary>
        /// Carga las cuentas bancarias de un tipo de modena especifico asociadas al banco.
        /// </summary>
        /// <param name="compania">compania de las cuentas bancarias a cargar</param>
        /// <param name="banco">banco de las cuentas a cargar</param>
        /// <param name="moneda">moneda de las cuentas a cargar</param>
        /// <returns>lista de cuentas bancarias</returns>
        public static List<CuentaBanco> ObtenerCuentasBancarias(string compania, string banco, TipoMoneda moneda)
        {
            List<CuentaBanco> cuentas= new List<CuentaBanco>();
            cuentas.Clear();
            SQLiteDataReader reader = null;
            
            string sentencia =
                " SELECT CTA_BCO,NOM_CTA FROM " + Table.ERPADMIN_alCXC_CTA_BCO +
                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND   COD_BCO = @BANCO" +
                " AND   MONEDA = @MONEDA";             
            try
            {            
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                      new SQLiteParameter("@BANCO", banco),
                      new SQLiteParameter("@MONEDA", ((char)moneda).ToString())}); 

                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);//se ejecuta la consulta

                while (reader.Read())
                {
                    CuentaBanco cuenta = new CuentaBanco();

                    cuenta.Numero = reader.GetString(0);
                    cuenta.Descripcion = reader.GetString(1);
                    cuenta.Moneda = moneda;

                    cuentas.Add(cuenta);
                }
                return cuentas;
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
