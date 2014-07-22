using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario
{
    public class Inventario : EncabezadoDocumento
    {

        #region Variables y Propiedades de instancia

        private DetallesInventario detalles = new DetallesInventario();
        /// <summary>
        /// Detalles del Inventario
        /// </summary>
        public DetallesInventario Detalles
        {
            get { return detalles; }
            set { detalles = value; }
        }

        #endregion

        #region Metodos de Instancia

        /// <summary>
        /// Actualizar un inventario
        /// </summary>
        public void Actualizar()
        {
            if (detalles.Lista.Count == 0)
                return;
            try
            {
                GestorDatos.BeginTransaction();
                DBEliminar();
                DBGuardar();
                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                throw new Exception("Error al Actualizar Inventario "+ ex.Message);
            }
        }
        
        /// <summary>
        /// Guardar un inventario
        /// </summary>
        public void Guardar()
        {
            try
            {
                GestorDatos.BeginTransaction();
                DBGuardar();
                ParametroSistema.IncrementarInventario(Compania,Zona);
                GestorDatos.CommitTransaction();   
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                throw new Exception("Error al Guardar Inventario. "+ ex.Message);
            }
        }
        
        #region Manipulacion Detalles

        /// <summary>
        /// Obtener los detalles del inventario
        /// </summary>
        public void ObtenerDetalles()
        {
            detalles.Encabezado.Compania = this.Compania;
            detalles.Encabezado.Numero = this.Numero;
            detalles.Encabezado.Zona = this.Zona;
            detalles.Obtener();
        }        
        
        #endregion
        
        #endregion

        #region Metodos de Clase

        /// <summary>
        /// Obtener la cantidad en inventario para un articulo
        /// </summary>
        /// <param name="articulo">articulo a consultar</param>
        /// <param name="cliente">cliente asociado</param>
        /// <param name="zona">zona asociada</param>
        /// <returns>cantidad en unidades de almacen de inventario del articulo</returns>
        public static Decimal CantidadEnInventario(Articulo articulo, string cliente, string zona)
        {
            decimal[] cantidad = ConsultarInventario(articulo.Codigo,articulo.Compania, cliente, zona);
            return cantidad[1] + (cantidad[0] * articulo.UnidadEmpaque);
        }
        
        #endregion

        #region Acceso Datos

        /// <summary>
        /// Eliminar de la base de datos el inventario.
        /// </summary>
        /// <param name="transaccion"></param>
        private void DBEliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_ENC_INV  +
                " WHERE COD_CIA = @COD_CIA" +
                " AND COD_ZON = @COD_ZON "+
                " AND NUM_INV = @NUM_INV" ;

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_INV", SqlDbType.NVarChar, Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows !=1)
                throw new Exception((rows<1)?"No se afectó ningún registro de inventario.":"Se afectó más de un registro de inventario.");

            detalles.Eliminar();

        }

        /// <summary>
        /// Metodo encargado hacer persistente el inventario y  los detalles del mismo
        /// </summary>
        /// <param name="transaccion"></param>
        /// <param name="actualizarConsecutivo">Indica si se debe actualizar el consecutivo.</param>
        private void DBGuardar()
        {
            this.HoraFin = DateTime.Now;

            //Se procede a guardar el encabezado del inventario
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_ENC_INV +
                "        ( COD_CIA,  COD_ZON,  COD_CLT,  NUM_INV,  HOR_INI,  HOR_FIN,  FEC_INV,  NUM_ITM) " +
                " VALUES (@COD_CIA, @COD_ZON, @COD_CLT, @NUM_INV, @HOR_INI, @HOR_FIN, @FEC_INV, @NUM_ITM)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_INV",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, HoraInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, HoraFin),
                GestorDatos.SQLiteParameter("@FEC_INV",SqlDbType.DateTime, FechaRealizacion),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.Lista.Count)});

            GestorDatos.EjecutarComando(sentencia, parametros);
            
            //se guardan los detalles de inventario
            detalles.Encabezado = this;
            detalles.Guardar();
        }

        /// <summary>
        /// Obtener inventarios por cliente
        /// </summary>
        /// <param name="cliente">cliente asociado</param>
        /// <returns>lista de inventarios</returns>
        public static List<Inventario> ObtenerInventarios(string cliente)
        {
            List<Inventario> inventarios = new List<Inventario>();
            inventarios.Clear();

            string sentencia =
                " SELECT COD_CIA,NUM_INV,HOR_INI,HOR_FIN,FEC_INV ,COD_ZON" +
                " FROM " + Table.ERPADMIN_alFAC_ENC_INV +
                " WHERE COD_CLT = @CLIENTE " +
                " AND DOC_PRO IS NULL";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList( new SQLiteParameter[] { new SQLiteParameter("@CLIENTE", cliente) });

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Inventario inventario = new Inventario();

                    inventario.Cliente = cliente;
                    inventario.Compania = reader.GetString(0);
                    inventario.Numero = reader.GetString(1);
                    inventario.HoraInicio = reader.GetDateTime(2);
                    inventario.HoraFin = reader.GetDateTime(3);
                    inventario.FechaRealizacion = reader.GetDateTime(4);
                    inventario.Zona = reader.GetString(5);
                    inventarios.Add(inventario);
                }
                return inventarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando inventarios del cliente. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        /// <summary>
        /// Consultar en la base de datos la cantidad del articulo inventariada
        /// </summary>
        /// <param name="articulo">codigo del articulo a buscar</param>
        /// <param name="compania">compania asociada al articulo</param>
        /// <param name="cliente">cliente asociado</param>
        /// <param name="zona">zona asociada</param>
        /// <returns></returns>
        private static Decimal[] ConsultarInventario(string articulo, string compania, string cliente, string zona)
        {
			SQLiteDataReader reader = null;
			decimal[] cantidad = new decimal[2];
	
 			string sentencia =
                " SELECT SUM(CNT_MAX),SUM(CNT_MIN) FROM " + Table.ERPADMIN_alFAC_ENC_INV + " E " +
                " INNER JOIN " + Table.ERPADMIN_alFAC_DET_INV + " D ON " +
                " (UPPER(E.COD_CIA) = UPPER(D.COD_CIA) AND UPPER(E.COD_CIA) = UPPER(D.COD_CIA) AND E.NUM_INV = D.NUM_INV) " +
                " WHERE D.COD_ART = @ARTICULO AND " +
                "       UPPER(E.COD_CIA) = @COMPANIA AND " + 
	            "       E.COD_ZON = @ZONA AND " + 
	            "       E.COD_CLT = @CLIENTE ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,compania.ToUpper()),
                GestorDatos.SQLiteParameter("@ZONA",SqlDbType.NVarChar, zona),
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente),
                GestorDatos.SQLiteParameter("@ARTICULO",SqlDbType.NVarChar, articulo)  });

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                if (reader.Read())
                {
                    if(!reader.IsDBNull(0))
                    {
                    cantidad[0] = reader.GetDecimal(0);
                    cantidad[1] = reader.GetDecimal(1);
                    }
                    else
                    cantidad[0] = cantidad[1] = 0;
                }
                else
                    cantidad[0] = cantidad[1] = 0;
                
                

            }
            catch (Exception ex)
            {
                throw new Exception("Error consultando cantidades inventariadas para el articulo. " + ex.Message); 
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
			return cantidad;
        }
        /// <summary>
        /// Determina si hay inventarios pendientes de carga
        /// </summary>
        /// <returns>inventarios pendientes de carga</returns>
        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_INV + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }

        #endregion


    }
}
