
namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    /// <summary>
    /// Informacion basica de una compania
    /// </summary>
    public abstract class CompaniaBase
    {
        #region Variables y Propiedades de instancia

        private string nombre = string.Empty;
        /// <summary>
        /// Nombre de la corporación. 
        /// </summary>
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        private string direccion = string.Empty;
        /// <summary>
        /// Dirección de la corporación. 
        /// </summary>
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }

        private string telefono = string.Empty;
        /// <summary>
        /// Teléfono de la corporación.
        /// </summary>
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; }
        }

        private string slogan = string.Empty;
        /// <summary>
        /// Slogan de la corporación. 
        /// </summary>
        public string Slogan
        {
            get { return slogan; }
            set { slogan = value; }
        }

        private string nit = string.Empty;
        /// <summary>
        /// Código de Nit de la corporación.
        /// </summary>
        public string Nit
        {
            get { return nit; }
            set { nit = value; }
        }

        #endregion

    }
}
