using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;

namespace Softland.ERP.FR.Mobile.Cls
{
    public class ResolucionUtilitario
    {

        public const string FACTURA = "FAC";
        public const string NOTACREDITO = "N/C";
        private const string SEPARADOR = "#|#";

        public static List<ConsecResoluBase> listaResoluciones = new List<ConsecResoluBase>();

        public static ConsecResoluBase consecutivoResolucion;

        public static string Error = string.Empty;

        private static SQLiteDataReader reader = null;

        private static bool obtenerResolucion(string compania, string tipoDoc,string ruta, bool todo)
        {
            bool resultado = true;

            try
            {
                StringBuilder sqlCmd = new StringBuilder();

                sqlCmd.AppendLine("SELECT COMPANIA, HANDHELD, RESOLUCION,SERIE,VALOR,CONSEC_INICIAL,CONSEC_FINAL,ESTADO,TIPO_DOCUMENTO," +
                "MASCARA,FECHA_RESOLUCION,FECHA_AUTORIZACION,FECHA_CREACION,USUARIO_CREACION,RUTA");
                sqlCmd.AppendLine(string.Format("FROM {0} ", Table.ERPADMIN_RESOLUCION_CONSECUTIVO_RT));

                if (!todo)
                {
                    if (string.IsNullOrEmpty(compania))
                    {
                        listaResoluciones = null;
                        resultado = false;
                        return resultado;
                    }
                    else
                    {
                        sqlCmd.AppendLine(string.Format("WHERE COMPANIA = '{0}' ", compania));

                        if (!string.IsNullOrEmpty(tipoDoc))
                            sqlCmd.AppendLine(string.Format(" AND  TIPO_DOCUMENTO = '{0}' ", tipoDoc));

                        if (!string.IsNullOrEmpty(tipoDoc))
                            sqlCmd.AppendLine(string.Format(" AND  RUTA = '{0}' ", ruta));
                    }
                }

                reader = GestorDatos.EjecutarConsulta(sqlCmd.ToString());
                DateTime? dtresolucion=null;
                DateTime? dtautorizacion = null;
                DateTime? dtcreacion = null;
                string mascara = string.Empty;

                while (reader.Read())
                {
                    if(reader.GetValue(10)!=DBNull.Value)
                    {
                        dtresolucion = reader.GetDateTime(10);
                    }
                    if (reader.GetValue(11) != DBNull.Value)
                    {
                        dtautorizacion = reader.GetDateTime(11);
                    }
                    if (reader.GetValue(12) != DBNull.Value)
                    {
                        dtcreacion = reader.GetDateTime(12);
                    }
                    if (reader.GetValue(9) != DBNull.Value)
                    {
                        mascara = reader.GetString(9);
                    }

                    ConsecResoluBase conscRes = new ConsecResoluBase();
                    conscRes = new ConsecResoluBase(reader.GetString(0),
                                        reader.GetString(1),
                                        reader.GetString(2),
                                        reader.GetString(3),
                                        reader.GetString(4),
                                        reader.GetString(5),
                                        reader.GetString(6),
                                        reader.GetString(7),
                                        reader.GetString(8),
                                        mascara,
                                        dtresolucion,
                                        dtautorizacion,
                                        dtcreacion,
                                        reader.GetString(13),
                                        reader.GetString(14));

                    if (!string.IsNullOrEmpty(tipoDoc))
                        consecutivoResolucion=conscRes;
                    else
                        listaResoluciones.Add(conscRes);                    
                }

            }
            catch (SQLiteException ex)
            {
                Error = "Error ejecutando consultando consecutivo resolución. " + ex.Message;
                resultado = false;
            }
            catch (Exception ex)
            {
                Error = "Error procesando consulta de consecutivos resoluciones. " + ex.Message;
                resultado = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }

        /// <summary>
        /// Carga las resoluciones asociados a una compania
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        public static bool obtenerConsecResolucion(string compania)
        {
            return obtenerResolucion(compania,null,null,false);
        }

        /// <summary>
        /// Carga las resoluciones para un tipo de documento,ruta y compania
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="tipoDoc"></param>
        /// <returns></returns>
        public static bool obtenerConsecResolucion(string compania, string tipoDoc,string ruta)
        {
            return obtenerResolucion(compania, tipoDoc,ruta,false);
        }

        /// <summary>
        /// Carga todos las resoluciones
        /// </summary>
        /// <returns></returns>
        public static bool obtenerConsecResolucion()
        {
            return obtenerResolucion(null, null,null,true);
        }

        public static string ObtenerSentenciaActualizacionResolucion()
        {
            string sentencia = string.Empty;

            if (obtenerConsecResolucion())
            {
                foreach (ConsecResoluBase conscRes in listaResoluciones)
                {
                    sentencia +=
                        "UPDATE %CIA.RESOLUCION_CONSECUTIVO_RT set " +
                        "VALOR = '" + conscRes.UltimoValor + "' " +
                        ",ESTADO = '" + conscRes.Estado + "' " +
                        "WHERE COMPANIA = '" + conscRes.Compania + "' " +
                        "AND   RUTA = '" + conscRes.Ruta + "' " +
                        "AND   RESOLUCION = '" + conscRes.Codigo + "' " +
                        "AND   SERIE = '" + conscRes.Serie + "' " +
                        "AND   TIPO_DOCUMENTO = '" + conscRes.TipoDoc + "' ";

                    sentencia += SEPARADOR;
                }
            }

            return sentencia;
        }

        public static bool usaResolucionFactura(string ruta)
        {
            bool resultado = false;
            try
            {
                string consulta = "SELECT COUNT(0) FROM " + Table.ERPADMIN_RESOLUCION_CONSECUTIVO_RT +
                    " WHERE RUTA=@RUTA AND TIPO_DOCUMENTO=@TIPO";
                 SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{ 
                       new SQLiteParameter("@RUTA",ruta),
                       new SQLiteParameter("@TIPO",ConsecResoluBase.TIPOFACTURA)});               

                int count = Convert.ToInt16(GestorDatos.EjecutarScalar(consulta,parametros));
                resultado = count > 0;
            }
            catch (Exception ex)
            {
                resultado = false;
                throw ex;                
            }
            return resultado;
        }

        public static bool usaResolucionDevolucion(string ruta)
        {
            bool resultado = false;
            try
            {
                string consulta = "SELECT COUNT(0) FROM " + Table.ERPADMIN_RESOLUCION_CONSECUTIVO_RT +
                    " WHERE RUTA=@RUTA AND TIPO_DOCUMENTO=@TIPO";
                SQLiteParameterList parametros =new SQLiteParameterList(new SQLiteParameter[]{ 
                      new SQLiteParameter("@RUTA",ruta),
                      new SQLiteParameter("@TIPO",ConsecResoluBase.TIPODEVOLUCION)});
                int count = Convert.ToInt16(GestorDatos.EjecutarScalar(consulta, parametros));
                resultado = count > 0;
            }
            catch (Exception ex)
            {
                resultado = false;
                throw ex;
            }
            return resultado;
        }
    }
}
