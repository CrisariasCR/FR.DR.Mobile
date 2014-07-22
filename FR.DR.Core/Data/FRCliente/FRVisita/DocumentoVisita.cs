using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
namespace Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita
{
    /// <summary>
    /// Informacion sobre los documentos realizados durante una visita.
    /// </summary>
    public struct DocumentoVisita
    {
        #region Variables y Propiedades

        private string codigo;
        /// <summary>
        /// Codigo del documento.
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private DocumentoFR tipoDocumento;
        /// <summary>
        /// Tipo de documento generado en la visita.
        /// </summary>
        public DocumentoFR TipoDocumento
        {
            get { return tipoDocumento; }
            set { tipoDocumento = value; }
        }

        private string compania;
        /// <summary>
        /// Compania a la que pertenece el documento.
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); }
        }
        #endregion

        #region Logica

        /// <summary>
        /// Verifica si existen documentos (pedidos, devoluciones, inventarios, recibos de cobro y consignaciones) sin registrar para un cliente
        /// </summary>
        /// <param name="cliente">cliente a verificar</param>
        /// <returns>existencia de documentos pendientes de registrar en la DB</returns>
        public static bool ExistenDocumentos(string cliente)
        {
            return
            (ExistenDocsSinRegistrar(sentencia("NUM_PED", "TIP_DOC", "FEC_PED", Table.ERPADMIN_alFAC_ENC_PED , "COUNT(*)"), cliente)
            || ExistenDocsSinRegistrar(sentencia("NUM_DEV", "'4'", "FEC_DEV", Table.ERPADMIN_alFAC_ENC_DEV , "COUNT(*)"), cliente)
            || ExistenDocsSinRegistrar(sentencia("NUM_INV", "'8'", "FEC_INV", Table.ERPADMIN_alFAC_ENC_INV , "COUNT(*)"), cliente)
            || ExistenDocsSinRegistrar(sentencia("NUM_REC", "'5'", "FEC_PRO", Table.ERPADMIN_alCXC_DOC_APL, "COUNT(*)"), cliente)
            || ExistenDocsSinRegistrar(sentencia("NUM_CSG", "'9'", "FEC_BOLETA", Table.ERPADMIN_alFAC_ENC_CONSIG, "COUNT(*)"), cliente));
        }
        /// <summary>
        /// Generar sentencia de DB para verificar la existencia de documentos en la visita
        /// </summary>
        /// <param name="consecutivo">campo del consecutivo</param>
        /// <param name="tipoDoc">numeracion de documento</param>
        /// <param name="fechaRegistro">campo de la fecha</param>
        /// <param name="tabla">tabla a buscar</param>
        /// <param name="columna">columna a buscar</param>
        /// <returns>sentencia</returns>
        private static string sentencia(string consecutivo, string tipoDoc, string fechaRegistro, Table tabla, string columna)
        { 
            //Script para sacar los documentos registrados el dia de hoy
            string docsRegistrados =
                " SELECT DOCUMENTO FROM "+ Table.ERPADMIN_VISITA_DOCUMENTO  +
                " WHERE CLIENTE = @CLIENTE " +
                " AND  julianday(date(INICIO)) = julianday(date('now','localtime')) ";

            if (!columna.Equals("COUNT(*)"))
                columna = " COD_CIA, "+ consecutivo + ", "+ tipoDoc ;
            return
                " SELECT " + columna + " FROM " + tabla + 
                " WHERE (DOC_PRO IS NULL OR DOC_PRO = 'N')" +
                " AND COD_CLT = @CLIENTE " +
                " AND  julianday(date("+fechaRegistro+")) = julianday(date('now','localtime'))" +
                " AND " + consecutivo + " NOT IN (" + docsRegistrados + ")";
            //julianday(date(INICIO)) = julianday(date('now','localtime'))
        }
        /// <summary>
        /// Obtener los documentos sin registrar de un cliente
        /// </summary>
        /// <param name="cliente">cliente</param>
        /// <returns>lista de documentos pendientes de registrar</returns>
        public static List<DocumentoVisita> ObtenerDocumentosSinRegistrar(string cliente)
        {
            List<DocumentoVisita> docs = new List<DocumentoVisita>();
            docs.Clear();

            //Verificamos si se han realizado pedidos el dia de hoy sin sincronizar al cliente
            docs.AddRange(DocsSinRegistrar(sentencia("NUM_PED", "TIP_DOC", "FEC_PED", Table.ERPADMIN_alFAC_ENC_PED, "DOCUMENTO"), cliente));
            docs.AddRange(DocsSinRegistrar(sentencia("NUM_DEV", "'4'", "FEC_DEV", Table.ERPADMIN_alFAC_ENC_DEV, "DOCUMENTO"), cliente));
            docs.AddRange(DocsSinRegistrar(sentencia("NUM_INV", "'8'", "FEC_INV", Table.ERPADMIN_alFAC_ENC_INV, "DOCUMENTO"), cliente));
            docs.AddRange(DocsSinRegistrar(sentencia("NUM_REC", "'5'", "FEC_PRO", Table.ERPADMIN_alCXC_DOC_APL, "DOCUMENTO"), cliente));
            docs.AddRange(DocsSinRegistrar(sentencia("NUM_CSG", "'9'", "FEC_BOLETA", Table.ERPADMIN_alFAC_ENC_CONSIG, "DOCUMENTO"), cliente));

            return docs;
        }
        #endregion

        #region Acceso Datos

        /// <summary>
        /// Obtener documentos sin registrar para un cliente
        /// </summary>
        /// <param name="sentencia">sentencia formada</param>
        /// <param name="cliente">cliente a verificar</param>
        /// <returns>lista de documentos de visita pendientes de registrar</returns>
        private static List<DocumentoVisita> DocsSinRegistrar(string sentencia, string cliente)
        {
            SQLiteDataReader reader = null;
            List<DocumentoVisita> docs = new List<DocumentoVisita>();
            try
            {
                docs.Clear();
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyyMMdd"))});            
                
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DocumentoVisita doc = new DocumentoVisita();
                    doc.Compania = reader.GetString(0);
                    doc.Codigo = reader.GetString(1);
                    doc.TipoDocumento = (DocumentoFR)reader.GetString(2)[0];
                    docs.Add(doc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando documentos realizados. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                {                    
                    reader.Close();
                    reader = null;
                }
            }

            return docs;
        }
        /// <summary>
        /// verificar si existen documentos sin registrar para un cliente
        /// </summary>
        /// <param name="sentencia">sentencia formada</param>
        /// <param name="cliente">cliente a verificar</param>
        /// <returns>existencia de pendientes de registrar</returns>
        private static bool ExistenDocsSinRegistrar(string sentencia, string cliente)
        {
            try
            {
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente),
                      new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyyMMdd"))});

                int registros = Convert.ToInt32(GestorDatos.EjecutarScalar(sentencia, parametros));

                if(registros>0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verificando documentos sin registrar. " + ex.Message);
            }
        }
        /// <summary>
        /// Guardar los documentos realizados en la visita del cliente
        /// </summary>
        /// <param name="cliente">cliente visitado</param>
        /// <param name="zona">zona o rutaen la que se visito</param>
        /// <param name="inicio">fecha de inicio de la visita</param>
        public void Guardar(string cliente, string zona, DateTime inicio)
        {
            string sentenciaSql =
                "INSERT INTO " + Table.ERPADMIN_VISITA_DOCUMENTO + " ( CLIENTE, RUTA, INICIO, COMPANIA, DOCUMENTO, TIPO_DOCUMENTO) " +
                                 " VALUES (@COD_CLT,@COD_ZON,@INICIO,@COD_CIA,@DOCUMENTO,@TIPO_DOCUMENTO)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, cliente),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, zona),
                GestorDatos.SQLiteParameter("@INICIO",SqlDbType.DateTime, inicio),
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@DOCUMENTO",SqlDbType.NVarChar, Codigo),
                GestorDatos.SQLiteParameter("@TIPO_DOCUMENTO",SqlDbType.NVarChar, ((char)TipoDocumento).ToString())});
            try
            {
                GestorDatos.EjecutarComando(sentenciaSql, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando documentos de la visita. " + ex.Message);
            }   
        }

        #endregion
    }
}
