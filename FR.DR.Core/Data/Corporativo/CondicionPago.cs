using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    /// <summary>
    /// Descripcion de la condicion de pago
    /// </summary>
    public class CondicionPago
    {
        #region Variables y Propiedades de instancia

        private string codigo = string.Empty;
        /// <summary>
        /// Codigo de la condicion de pago
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string compania = string.Empty;
        /// <summary>
        /// compania de la condicion de pago
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); }
        }

        private int diasNeto = -1;
        /// <summary>
        /// Dias establecidos para el pago
        /// </summary>
        public int DiasNeto
        {
            get { return diasNeto; }
            set { diasNeto = value; }
        }

        private string descripcion = string.Empty;
        /// <summary>
        /// Descripcion de la condicion de pago
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        public override string ToString()
        {
            return Descripcion;
        }
        #endregion

        /// <summary>
        /// Constructor de la condicion de pago
        /// </summary>
        public CondicionPago() { }

        #region Acceso Datos

        /// <summary>
        /// Obtener condiciones de pago para una compania en particular
        /// </summary>
        /// <param name="compania">compania para filtrar condiciones de pago</param>
        /// <returns>lista de condiciones de pago</returns>
        public static List<CondicionPago> ObtenerCondicionesPago(string compania)
        {
            List<CondicionPago> condiciones = new List<CondicionPago>();
            condiciones.Clear();

            string sentencia = 
                @" SELECT COD_CND, DES_CND, CAN_DIA "+
                @" FROM " + Table.ERPADMIN_alFAC_CND_PAG +
                @" WHERE UPPER(COD_CIA) = @COMPANIA" +
                @" ORDER BY DES_CND";
            
            SQLiteDataReader reader = null;            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {new SQLiteParameter("@COMPANIA", compania.ToUpper())});
            
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    CondicionPago condicion = new CondicionPago();

                    condicion.Compania = compania;
                    condicion.codigo = reader.GetString(0);
                    condicion.descripcion = reader.GetString(1);
                    condicion.diasNeto = reader.GetInt32(2);

                    condiciones.Add(condicion);
                }

                return condiciones;
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede obtener condición de pagos para la compañía '" + compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }
        /// <summary>
        /// Obtener condicion de pago basado en un cliente en una compania 
        /// </summary>
        /// <param name="cia">codigo de la compania</param>
        /// <param name="cliente">codigo del cliente</param>
        public void ObtenerCondicionPago(string cia, string cliente)
        {
            string sentencia = 
                @" SELECT CP.COD_CND, CP.DES_CND, CP.CAN_DIA "+
                @" FROM " + Table.ERPADMIN_alFAC_CND_PAG + " CP, " + Table.ERPADMIN_CLIENTE_CIA  + " C" +
                @" WHERE UPPER(C.COD_CIA) = @COMPANIA " + 
                @" AND C.COD_CLT = @CLIENTE "+
                @" AND C.COD_CND = CP.COD_CND "+
                @" AND UPPER(C.COD_CIA) = UPPER(CP.COD_CIA) ";
            
            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@COMPANIA", cia),
                new SQLiteParameter("@CLIENTE", cliente)});
                       
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                if (reader.Read())
                {
                    compania = cia;
                    codigo = reader.GetString(0);
                    descripcion = reader.GetString(1);
                    diasNeto = reader.GetInt32(2);
                }
                else
                    throw new Exception("No se puede obtener la condicion de pago del cliente");
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
        }

        /// <summary>
        /// Busca los dias neto de la condición de pago.
        /// </summary>
        /// <param name="condicion">codigo de la condicion de pago</param>
        /// <param name="compania">codigo de la compania </param>
        /// <returns>cantidad de dias</returns>
        public static int ObtenerDiasNeto(string condicion, string compania)
        {
            string sentencia =
                " SELECT CAN_DIA "+
                " FROM " + Table.ERPADMIN_alFAC_CND_PAG +
                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND COD_CND = @CONDICION";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                new SQLiteParameter("@CONDICION", condicion)});
            try
            {
                int dias = Convert.ToInt32(GestorDatos.EjecutarScalar(sentencia, parametros));
                return dias;
            }      
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo los días de crédito. " + ex.Message);
            }
            
        }
        /// <summary>
        /// Cargar una condicion de pago segun una compania
        /// </summary>
        /// <param name="cia">codigo de la compania a filtrar</param>
        public void Cargar(string cia)
        {
            string sentencia =
                @" SELECT DES_CND, CAN_DIA " +
                " FROM " + Table.ERPADMIN_alFAC_CND_PAG +
                " WHERE UPPER(COD_CIA) = @COMPANIA" +
                " AND COD_CND = @CONDICION";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@COMPANIA", cia.ToUpper()),
                new SQLiteParameter("@CONDICION",codigo)});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    compania = cia;
                    descripcion = reader.GetString(0);
                    diasNeto = reader.GetInt32(1);
                }
                else
                    throw new Exception("No se puede obtener la condicion de pago");
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
        }
        #endregion
    }
}
