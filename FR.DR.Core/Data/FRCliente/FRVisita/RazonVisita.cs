using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita
{

    /// <summary>
    /// Representa una razon de visita.
    /// </summary>
    public class RazonVisita
    {
        #region Variables y Propiedades

        public override string ToString()
        {
            return this.Descripcion;
        }

        private string codigo;
        /// <summary>
        /// Codigo de la razon de la visita
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        } 

        private string descripcion;
        /// <summary>
        /// descripcion de la visita
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        #endregion

        public RazonVisita()
        {
            this.codigo = FRdConfig.NoDefinido;
            this.descripcion = string.Empty;
        }

        /// <summary>
        /// Obtener las razones de visita de la base de datos
        /// </summary>
        /// <returns></returns>
        public static List<RazonVisita> ObtenerRazonesVisita()
        {
            List<RazonVisita> razones = new List<RazonVisita>();
            razones.Clear();
            SQLiteDataReader reader = null;

            try
            {
                string sentencia =
                    " SELECT DISTINCT COD_RZN,DES_RZN FROM " + Table.ERPADMIN_alFAC_RZN_VIS ;

                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    RazonVisita razon = new RazonVisita();
                    razon.Codigo = reader.GetString(0);
                    razon.Descripcion = reader.GetString(1);
                    razones.Add(razon);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando razones de visita. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return razones;
        }

    }
}
