using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class ProbadorFiltros
    {
        #region Attributes
        private const String ERPADMIN_DES_BON_ARTICULO = "ERPADMIN_DES_BON_ARTICULO";
        private const String ERPADMIN_DES_BON_CLIENTE = "ERPADMIN_DES_BON_CLIENTE";
        #endregion

        #region Properties
        
        #endregion

        #region Methods
        /// <summary>
        /// Pruebe que el cliente se encuentre del filtro.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static bool ProbarCliente(String cliente, String filtro)
        {
            bool resultado = false;
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT COUNT(CLIENTE) ");
            sentencia.AppendLine(String.Format(" FROM {0} cli", ERPADMIN_DES_BON_CLIENTE));
            //sentencia.AppendLine(String.Format(" WHERE '{0}' IN (SELECT CLIENTE FROM {1} WHERE {2})", cliente, ERPADMIN_DES_BON_CLIENTE, filtro));
            sentencia.AppendLine(String.Format(" WHERE CLIENTE = '{0}' ", cliente));
            if (!String.IsNullOrEmpty(filtro))
            {
                sentencia.AppendLine(String.Format(" AND ({0}) ", filtro));
            }
            else
            {
                return true;
            }
            
            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString());

                if (reader.Read())
                {
                    int count = reader.GetInt32(0);
                    resultado = count > 0; 
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error probando el filtro de clientes. {0}", ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }
        /// <summary>
        /// Prueba que el artículo se encuentra dentro del filtro.
        /// </summary>
        /// <param name="articulo"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static bool ProbarArticulo(String compania, String articulo, String filtro)
        {
            bool resultado = false;
            StringBuilder sentencia = new StringBuilder();
            SQLiteParameterList parametros;
            sentencia.AppendLine(" SELECT COUNT(ARTICULO) ");
            sentencia.AppendLine(String.Format(" FROM {0} art", ERPADMIN_DES_BON_ARTICULO));
            sentencia.AppendLine(String.Format(" WHERE UPPER(COMPANIA) = @COMPANIA AND ARTICULO = '{0}' ", articulo));
            if (!String.IsNullOrEmpty(filtro))
            {
                sentencia.AppendLine(String.Format(" AND ({0}) ", filtro));
            }
            else
            {
                return true;
            }
            
            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()) });
            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                if (reader.Read())
                {
                    int count = reader.GetInt32(0);
                    resultado = count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error probando el filtro de artículos. {0}", ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }
        /// <summary>
        /// Obtiene la lista de artículos dentro del filtro.
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static List<Articulo> ObtenerArticulos(String compania,String filtro, String nivelPrecio, String ruta)
        {
            List<Articulo> resultado = null;
            StringBuilder sentencia = new StringBuilder();
            SQLiteParameterList parametros;
            sentencia.AppendLine(" SELECT art.ARTICULO ");
            sentencia.AppendLine(String.Format(" FROM {0} art ", ERPADMIN_DES_BON_ARTICULO));
            sentencia.AppendLine(String.Format(" WHERE UPPER(art.COMPANIA) = @COMPANIA "));
            sentencia.AppendLine(String.Format(" AND 0 NOT IN (SELECT COUNT(0) FROM {0} prc WHERE UPPER(prc.COMPANIA) = @COMPANIA AND prc.ARTICULO = art.ARTICULO AND prc.NIVEL_PRECIO = @NIVEL) ", Table.ERPADMIN_ARTICULO_PRECIO));
            sentencia.AppendLine(String.Format(" AND 0 NOT IN (SELECT COUNT(0) FROM {0} ruta WHERE UPPER(ruta.COMPANIA) = @COMPANIA AND ruta.GRUPO_ART = art.GRUPO_ARTICULO AND ruta.RUTA = @RUTA) ", Table.ERPADMIN_RUTA_CFG));
            
            if (!String.IsNullOrEmpty(filtro))
            {
                sentencia.AppendLine(String.Format(" AND ({0}) ", filtro));
            }


            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper())
                , new SQLiteParameter("@NIVEL", nivelPrecio) 
                , new SQLiteParameter("@RUTA", ruta)});
            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);
                resultado = new List<Articulo>();
                while (reader.Read())
                {
                    String codigo = reader.GetString(0);
                    Articulo articulo = new Articulo(codigo, compania);
                    articulo.Cargar();
                    resultado.Add(articulo);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error obteniendo los artículos según el filtro. {0}", ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }
        #endregion
    }
}
