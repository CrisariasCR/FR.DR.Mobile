using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace FR.DR.Core.Data.Corporativo
{
    public class DivisionGeografica
    {
        #region Variables y Propiedades de instancia

        private string codigo = string.Empty;
        /// <summary>
        /// Codigo de la divisi[on geográfica
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string compania = string.Empty;
        /// <summary>
        /// Compania de la divisi[on geográfica
        /// </summary>
        public string Compania
        {
            get { return compania; }
            set { compania = value; }
        }

        private string nombre = string.Empty;
        /// <summary>
        /// Nombre de la divisi[on geográfica
        /// </summary>
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        #endregion
    }
}