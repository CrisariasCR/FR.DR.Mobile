using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{
    /// <summary>
    /// Direcciones de entrega de un cliente
    /// </summary>
    public struct DireccionEntrega
    {
        #region Variables y Propiedades de instancia

        private string codigo;
        /// <summary>
        /// codigo de la direccion de entrega
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string descripcion;
        /// <summary>
        /// descripcion de la direccion de entrega
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        #endregion

        /// <summary>
        /// Obtener las direcciones de entrega de un cliente en una compania
        /// </summary>
        /// <param name="compania">compania asociada</param>
        /// <param name="cliente">cliente asociado</param>
        /// <returns>lista de direcciones de entrega</returns>
        public static List<DireccionEntrega> ObtenerDireccionesEntrega(string compania, string cliente)
        {
            List<DireccionEntrega> direccionesEntrega = new List<DireccionEntrega>();
            direccionesEntrega.Clear();

            string sentencia = 
                @" SELECT DIR_EMB, DETA_EMB "+
                @" FROM " + Table.ERPADMIN_alFAC_DIR_EMB_CLT +
                @" WHERE UPPER(COD_CIA) = @COMPANIA" +
                @" AND COD_CLT = @CLIENTE ";
            
            SQLiteDataReader reader = null;            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@COMPANIA", compania),
                new SQLiteParameter("@CLIENTE", cliente)});
            
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    DireccionEntrega dir = new DireccionEntrega();

                    dir.Codigo = reader.GetString(0);

                    if (!reader.IsDBNull(1))
                        dir.Descripcion = reader.GetString(1);

                    direccionesEntrega.Add(dir);
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
            return direccionesEntrega;
        }

        public override string ToString()
        {
            return this.Codigo;
        }
    }
}
