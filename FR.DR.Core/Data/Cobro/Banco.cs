using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Clase Banco que incluye la cuenta bancaria
    /// </summary>
    public class BancoAsociado
    {
        /// <summary>
        /// Constructor del banco asociado
        /// </summary>
        /// <param name="codigo">codigo de banco</param>
        /// <param name="descripcion">descripcion del banco</param>
        public BancoAsociado(string codigo, string descripcion)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;
            this.cuenta = string.Empty;
            this.moneda = TipoMoneda.LOCAL;
        }
        /// <summary>
        /// Constructor del banco asociado
        /// </summary>
        /// <param name="codigo">codigo de banco</param>
        /// <param name="descripcion">descripcion del banco</param>
        /// <param name="moneda">moneda</param>
        public BancoAsociado(string codigo, string descripcion, TipoMoneda moneda)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;
            this.cuenta = string.Empty;
            this.moneda = moneda;
        }
        /// <summary>
        /// Constructor del banco asociado
        /// </summary>
        /// <param name="codigo">codigo de banco</param>
        /// <param name="descripcion">descripcion del banco</param>
        /// <param name="cuenta">cuenta asociada</param>
        public BancoAsociado(string codigo, string descripcion, string cuenta)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;
            this.cuenta = cuenta;
            this.moneda = TipoMoneda.LOCAL;
        }
        /// <summary>
        /// Constructor del banco asociado
        /// </summary>
        /// <param name="codigo">codigo de banco</param>
        /// <param name="descripcion">descripcion del banco</param>
        /// <param name="cuenta">cuenta asociada</param>
        /// <param name="moneda">moneda</param>
        public BancoAsociado(string codigo, string descripcion, string cuenta, TipoMoneda moneda)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;
            this.cuenta = cuenta;
            this.moneda = moneda;
        }
        /// <summary>
        /// Constructor del banco asociado
        /// </summary>
        public BancoAsociado()
        { }
        
        private string codigo = string.Empty;
        /// <summary>
        /// Codigo de banco
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }
        private string descripcion = string.Empty;
        /// <summary>
        /// Descripcion del banco 
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        private string cuenta;
        /// <summary>
        /// Cuenta asociada al banco
        /// </summary>
        public string Cuenta
        {
            get { return cuenta; }
            set { cuenta = value; }
        }
        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Tipo de moneda de la cuenta bancaria
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        } 
    }

    public class Banco
    {
        #region Variables y Propiedades de instancia

        private string compania = string.Empty;
        /// <summary>
        /// Codigo de la compania a al que esta asociado el banco.
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); }
        }

        private string codigo = string.Empty;
        /// <summary>
        /// Codigo de la entidad bancaria.
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string descripcion = string.Empty;
        /// <summary>
        /// Descripcion de la entidad bancaria.
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        private List<CuentaBanco> cuentasBancarias = new List<CuentaBanco>();
        /// <summary>
        /// Cuentas bancarias asociadas al banco.
        /// </summary>
        public List<CuentaBanco> CuentasBancarias
        {
            get { return cuentasBancarias; }
            set { cuentasBancarias = value; }
        }

        #endregion
        
        /// <summary>
        /// Constructor de la instancia
        /// </summary>
        public Banco() { }

        /// <summary>
        /// Obtener una serie de bancos pertenecientes a una compania
        /// </summary>
        /// <param name="compania">compania de la cual se tomaran los bancos asociados</param>
        /// <param name="registraCuentas">si se incluyen solo los bancos con cuentas bancarias asociadas</param>
        /// <returns>lista de bancos de la compania</returns>
        public static List<Banco> ObtenerBancos(string compania, bool registraCuentas) 
        {
            List<Banco> bancos = new List<Banco>();
            bancos.Clear();
            SQLiteDataReader reader = null;
            string sentencia;

            if (registraCuentas)
                sentencia =
                    " SELECT DISTINCT BCO.COD_BCO,BCO.DES_BCO " +
                    " FROM " + Table.ERPADMIN_alCXC_BCO +" BCO, " + Table.ERPADMIN_alCXC_CTA_BCO + " CTA " +
                    " WHERE UPPER(BCO.COD_CIA) = @COMPANIA " +
                    " AND   BCO.COD_BCO = CTA.COD_BCO " +
                    " AND   BCO.COD_CIA = CTA.COD_CIA";
            else
                sentencia =
                    " SELECT COD_BCO,DES_BCO FROM " + Table.ERPADMIN_alCXC_BCO  +
                    " WHERE  UPPER(COD_CIA) = @COMPANIA";

            try
            {
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper())});
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Banco miBanco = new Banco();//se crea un objeto banco
                    miBanco.Compania = compania;//se le asigna el codigo de compañia
                    miBanco.Codigo = reader.GetString(0);//se le asigna el codigo del banco
                    miBanco.Descripcion = reader.GetString(1);//se le asigan la descripcion del banco
                    bancos.Add(miBanco);//se agrega el banco a un arreglo
                }
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

            return bancos; //se retorna el arreglo de bancos
        }
    }
}
