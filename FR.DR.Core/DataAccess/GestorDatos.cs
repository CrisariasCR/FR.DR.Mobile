using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//using System.Data.Common;
using System.Data.SQLiteBase;

namespace Softland.ERP.FR.Mobile.Cls.AccesoDatos
{
    /// <summary>
    /// Contiene la logica de acceso a la base de datos SQL Compact Edition
    /// </summary>
    public class GestorDatos
    {
        #region Variables

        /// <summary>
        /// Objeto de la conexion.
        /// </summary>
        public static SQLiteConnection cnx;
        
        /// <summary>
        /// Objeto para la ejecucion de los comandos. 
        /// </summary>
        //private static SQLiteCommand cmd = new SQLiteCommand();

        private static string servidorWS = string.Empty;
        /// <summary>
        /// Nombre del Servidor Web Service donde se encuentra el EMM.
        /// </summary>
        public static string ServidorWS
        {
            get { return GestorDatos.servidorWS; }
            set { GestorDatos.servidorWS = value; }
        }

        private static string dominio = string.Empty;
        /// <summary>
        /// Dominio del servidor. 
        /// </summary>
        public static string Dominio
        {
            get { return GestorDatos.dominio; }
            set { GestorDatos.dominio = value; }
        }

        private static bool licenciaValida;
        /// <summary>
        /// Nombre de usuario para la conexión.
        /// </summary>
        public static bool LicenciaValida
        {
            get { return GestorDatos.licenciaValida; }
            set { GestorDatos.licenciaValida = value; }
        }

        private static string nombreUsuario = string.Empty;
        /// <summary>
        /// Nombre de usuario para la conexión.
        /// </summary>
        public static string NombreUsuario
        {
            get { return GestorDatos.nombreUsuario; }
            set { GestorDatos.nombreUsuario = value; }
        }
        private static string contrasenaUsuario = string.Empty;
        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        public static string ContrasenaUsuario
        {
            get { return GestorDatos.contrasenaUsuario; }
            set { GestorDatos.contrasenaUsuario = value; }
        }

        private static string owner = string.Empty;
        /// <summary>
        /// Esquema al que hace referencias las tablas.
        /// </summary>
        public static string Owner
        {
            get { return GestorDatos.owner; }
            set { GestorDatos.owner = value; }
        }
        
        private static string baseDatos = string.Empty;
        /// <summary>
        /// Nombre de la base de datos pocket.
        /// </summary>
        public static string BaseDatos
        {
            get { return GestorDatos.baseDatos; }
            set { GestorDatos.baseDatos = value; }
        }

        //private static SqlliteTransaction transaccion;
        ///// <summary>
        ///// Transaccion general del gestor para manejar sentencias transaccionales.
        ///// </summary>
        //public static SqlCeTransaction Transaccion
        //{
        //    get { return GestorDatos.transaccion; }
        //}
        
        #endregion

        #region Metodos

        /// <summary>
        /// Inicio de una transaccion.
        /// </summary>
        public static void BeginTransaction()
        {
            if (!cnx.IsInTransaction)
                cnx.BeginTransaction();
            //else
            //    throw new Exception("Imposible iniciar nueva transacción. Existe una transacción activa");
        }

        /// <summary>
        /// Realizar el commit de la transaccion.
        /// </summary>
        public static void CommitTransaction()
        {
            if (cnx.IsInTransaction)
                cnx.Commit();
        }

        /// <summary>
        /// Realizar un rollback en la transaccion. 
        /// </summary>
        public static void RollbackTransaction()
        {
            if (cnx.IsInTransaction)
                cnx.Rollback();
        }
        /// <summary>
        /// Conectar a Fuente de Datos .sdf
        /// </summary>
        public static void Conectar()
        {
            if (cnx != null)
            {
                if (cnx.isOpen) return;

                if (string.IsNullOrEmpty( cnx.DatabasePath ))
                {
                    throw new Exception("TODO : Falta implementar para GestoSincronizar y HandeHaledConfig");
                }
                try 
                {
                    //cnx.ConnectionString = "Data Source= " + baseDatos + ".sdf";                
                    cnx.Open();
                }
                catch (SQLiteException sqlex)
                {
                    throw new Exception(sqlex.Message);
                }                
            }
            
        }

        /// <summary>
        /// Desconectar de Fuente de Datos .sdf
        /// </summary>
        public static void Desconectar()
        {
            if (!cnx.isOpen)
                return;

            try
            {
                cnx.Close();
            }
            catch (SQLiteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Numero de registros de una sentencia
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <returns>Cantidad de Registros</returns>
        public static int NumeroRegistros(string sentencia)
        {
            int registros = 0;

            try
            {
                registros = Convert.ToInt32(cnx.ExecuteScalar(sentencia));
                return registros;
            }
            catch (SQLiteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Numero de registros de una sentencia
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <param name="parametros">Parametros de la sentencia</param>
        /// <returns>Cantidad de Registros</returns>
        public static int NumeroRegistros(string sentencia, SQLiteParameterList parametros)
        {
            return Convert.ToInt32(EjecutarScalar(sentencia, parametros));
        }
        /// <summary>
        /// Verifica si una sentencia retorna datos
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <param name="parametros">Parametros de la sentencia</param>
        /// <returns></returns>
        public static bool RetornaDatos(string sentencia, SQLiteParameterList parametros)
        {
            return NumeroRegistros(sentencia, parametros)>0;
        }

        /// <summary>
        /// Ejecutar Escalar
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <param name="parametros">Parametros de la consulta</param>
        /// <returns>Objeto retornado por la consulta</returns>
        public static object EjecutarScalar(string sentencia, SQLiteParameterList parametros)
        {
            try
            {
                return cnx.ExecuteScalar(sentencia, parametros);
            }
            catch (Exception ex) //UNDONE Excepcion SQLite
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Ejecutar Escalar
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <returns>Objeto retornado por la consulta</returns>
        public static object EjecutarScalar(string sentencia)
        {
            return EjecutarScalar(sentencia, new SQLiteParameterList());
        }

        /// <summary>
        /// Ejecutar consulta SQL
        /// </summary>
        /// <param name="sentencia">sentencia SQL</param>
        /// <param name="parametros">parametros de la consulta</param>        
        /// <returns>Conjunto de datos devueltos por la consulta</returns>
        public static SQLiteDataReader EjecutarConsulta(string sentencia, SQLiteParameterList parametros)
        {
            try
            {
                return cnx.ExecuteDataReader(sentencia, parametros);
            }
            catch (Exception ex) //UNDONE Excepciones SQLite
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Ejecutar consulta SQL
        /// </summary>
        /// <param name="sentencia">sentencia SQL</param>
        /// <returns>Conjunto de datos devueltos por la consulta</returns>
        //public static SQLiteDataReader EjecutarConsulta(string sentencia)
        //{
        //    return EjecutarConsulta(sentencia, new Array());
        //}

        public static SQLiteDataReader EjecutarConsulta(string sentencia)
        {
            return EjecutarConsulta(sentencia, new SQLiteParameterList());
        }
        /// <summary>
        /// Ejecutar sentencia SQL
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <param name="parametros">Parametros de la sentencia</param>
        /// <param name="transaccion">Transaccion a la que pertenece la sentencia</param>
        /// <returns>Resultado de la ejecucion del comando</returns>
        public static int EjecutarComando(string sentencia, SQLiteParameterList parametros)
        {
            try
            {
                int resultado = (int)cnx.ExecuteNonQuery(sentencia, parametros);
                return resultado;
            }
            catch (SQLiteException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Ejecutar sentencia SQL
        /// </summary>
        /// <param name="sentencia">Sentencia SQL</param>
        /// <returns></returns>
        public static int EjecutarComando(string sentencia)
        {
            return EjecutarComando(sentencia, null);
        }

        /// <summary>
        /// Generar un parametro SQL
        /// </summary>
        /// <param name="name">Nombre del parametro</param>
        /// <param name="dataType">tipo de dato</param>
        /// <param name="value">Valor del parametro</param>
        /// <returns>Parametro generado</returns>
        public static SQLiteParameter SQLiteParameter(string name, SqlDbType dataType, object value) 
        {
            SQLiteParameter parametro = new SQLiteParameter(name, dataType);
            if (value is string && dataType.Equals(SqlDbType.DateTime))
            {
                parametro.Value = Convert.ToDateTime(value);
            }
            else
            {
                parametro.Value = value;
            }            
            return parametro;
        }
        #endregion
    }
}
