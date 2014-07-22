using System;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    /// <summary>
    /// Datos de la corporacion
    /// </summary>
    public class Corporacion : CompaniaBase

    {
        #region Variables y Propiedades de instancia

        private string email = string.Empty;
        /// <summary>
        /// correo electronico de la corporacion
        /// </summary>
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        
        private string web = string.Empty;
        /// <summary>
        /// sitio web de la corporacion
        /// </summary>
        public string Web
        {
            get { return web; }
            set { web = value; }
        }
        
        private string fax = string.Empty;
        /// <summary>
        /// fax de la corporacion
        /// </summary>
        public string Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        public new string Nit
        {
            get { return base.Nit; }
            set { base.Nit = value; }
        }


        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructor de la corporacion
        /// </summary>
        public Corporacion()
        {
        }
        /// <summary>
        /// Cargar los datos de la corporacion
        /// </summary>
        public void Cargar()
        {
            string sentencia =
                " SELECT NOM_CRP,DIR_CRP,TEL_CRP,SLG_CRP,NIT_CRP,WEB_SIT,DIR_EML,NUM_FAX " +
                " FROM " + Table.ERPADMIN_GLOBALES_FR;

            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                if (reader.Read())
                {
                    Nombre = reader.GetString(0); 
                    Direccion = reader.GetString(1);
                    Telefono = reader.GetString(2);
                    Slogan = reader.GetString(3);
                    Nit = reader.GetString(4);
                    web = reader.GetString(5);
                    email = reader.GetString(6);
                    fax = reader.GetString(7);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando datos de la corporación. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }		
	}
        #endregion

}
