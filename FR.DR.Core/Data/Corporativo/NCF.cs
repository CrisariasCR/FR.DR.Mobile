using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;

namespace Softland.ERP.FR.Mobile.Cls
{
    public class NCFUtilitario
    {

        public const string FACTURA = "FAC";
        public const string NOTACREDITO = "N/C";
        private const string SEPARADOR = "#|#";

        public static List<NCFBase> listaNCF = new List<NCFBase>();

        public static NCFBase consecutivoNCF;

        public static string Error = string.Empty;

        private static SQLiteDataReader reader = null;

        private static bool obtenerNCF(string compania, string tipoDoc, string tipoContribuyente, bool todo)
        {
            bool resultado = true;

            try
            {
                StringBuilder sqlCmd = new StringBuilder();

                sqlCmd.AppendLine("SELECT COMPANIA, HANDHELD, PREFIJO, DESCRIPCION, ULTIMO_VALOR, SECUENCIA_INICIAL, SECUENCIA_FINAL, TIPO, TIPO_DOC, TIPO_CONTRIBUYENTE ");
                sqlCmd.AppendLine(string.Format("FROM {0} ", Table.ERPADMIN_NCF_CONSECUTIVO_RT));

                if (!todo)
                {
                    if (string.IsNullOrEmpty(compania))
                    {
                        listaNCF = null;
                        resultado = false;
                        return resultado;
                    }
                    else
                    {
                        sqlCmd.AppendLine(string.Format("WHERE COMPANIA = '{0}' ", compania));

                        if (!string.IsNullOrEmpty(tipoDoc))
                            sqlCmd.AppendLine(string.Format(" AND  TIPO_DOC = '{0}' ", tipoDoc));

                        if (!string.IsNullOrEmpty(tipoContribuyente) && tipoDoc == FACTURA)
                            sqlCmd.AppendLine(string.Format(" AND TIPO_CONTRIBUYENTE = '{0}' ", tipoContribuyente));
                    }
                }

                reader = GestorDatos.EjecutarConsulta(sqlCmd.ToString());

                while (reader.Read())
                {
                    NCFBase ncf = new NCFBase();
                    ncf = new NCFBase(reader.GetString(0),
                                        reader.GetString(1),
                                        reader.GetString(2),
                                        reader.GetString(3),
                                        reader.GetString(4),
                                        reader.GetString(5),
                                        reader.GetString(6),
                                        reader.GetString(7),
                                        reader.GetString(8),
                                        reader.GetString(9));

                    if (string.IsNullOrEmpty(tipoContribuyente))
                        listaNCF.Add(ncf);
                    else
                    {
                        consecutivoNCF = ncf;
                        break;
                    }
                }

            }
            catch (SQLiteException ex)
            {
                Error = "Error ejecutando consultando NCF. " + ex.Message;
                resultado = false;
            }
            catch (Exception ex)
            {
                Error = "Error procesando consulta de NCF. " + ex.Message;
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
        /// Carga los NCF asociados a una compania
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        public static bool obtenerNCF(string compania)
        {
            return obtenerNCF(compania, null, null, false);
        }

        /// <summary>
        /// Carga los NCF para un tipo de documento y compania
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="tipoDoc"></param>
        /// <returns></returns>
        public static bool obtenerNCF(string compania, string tipoDoc)
        {
            return obtenerNCF(compania, tipoDoc, null, false);
        }

        /// <summary>
        /// Carga el NCF para un documento y tipo de contribuyente
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="tipoDoc"></param>
        /// <param name="tipoContribuyente"></param>
        /// <returns></returns>
        public static bool obtenerNCF(string compania, string tipoDoc, string tipoContribuyente)
        {
            return obtenerNCF(compania, tipoDoc, tipoContribuyente, false);
        }

        /// <summary>
        /// Carga todos los NCF
        /// </summary>
        /// <returns></returns>
        public static bool obtenerNCF()
        {
            return obtenerNCF(null, null, null, true);
        }

        public static string ObtenerSentenciaActualizacionNCF()
        {
            string sentencia = string.Empty;

            if (obtenerNCF())
            {
                foreach (NCFBase ncf in listaNCF)
                {
                    sentencia +=
                        "UPDATE %CIA.NCF_CONSECUTIVO_RT set " +
                        "ULTIMO_VALOR = '" + ncf.UltimoValor + "' " +
                        "WHERE COMPANIA = '" + ncf.Compania + "' " +
                        "AND   HANDHELD = '" + ncf.Handheld + "' " +
                        "AND   PREFIJO = '" + ncf.Prefijo + "' " +
                        "AND   TIPO_DOC = '" + ncf.TipoDoc + "' " +
                        "AND   TIPO_CONTRIBUYENTE = '" + ncf.TipoContribuyente + "'";

                    sentencia += SEPARADOR;
                }
            }

            return sentencia;
        }
    }
}
