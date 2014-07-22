using System;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;

namespace Softland.ERP.FR.Mobile.Cls.Seguridad
{
	/// <summary>
	/// Summary description for GestorSeguridad.
	/// </summary>
	public class GestorSeguridad
	{
		#region Metodos

		public static int VerificarUsuario(string usuario)
		{   
			//-1 indica que no existe el usuario
			int intentosPermitidos = -1;

            string sentencia = 
                " SELECT MAX_INTENTOS_CONEX FROM " +  Table.SYSTEM_USUARIO +
                " WHERE USUARIO = @USUARIO AND ACTIVO = @ESTADO";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {                
                GestorDatos.SQLiteParameter("@USUARIO",SqlDbType.NVarChar,usuario),
                GestorDatos.SQLiteParameter("@ESTADO",SqlDbType.NVarChar,"S")});

			object valor = GestorDatos.EjecutarScalar(sentencia,parametros);

            if (valor != null && !(valor is DBNull))
            {
                intentosPermitidos = Convert.ToInt32(valor);
            }
			
			return intentosPermitidos;

		}

		public static bool AutenticarUsuario(string usuario, string password)
		{
            string sentencia =
                " SELECT CLAVE FROM SYSTEM_USUARIO " +
                " WHERE USUARIO = @USUARIO";
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@USUARIO", usuario);

            object valor = GestorDatos.EjecutarScalar(sentencia,parametros);

			if (valor != null && !(valor is DBNull))
			{
				string encriptedPass = valor.ToString();
					
				Cryptography encripter = new Cryptography();

				string unEncriptedPass = encripter.DecryptString(encriptedPass,string.Empty);
                //string unEncriptedPass = "exlaza"; // 

				//Se comparan las contrasenas. Ambas deben ser IDENTICAS con CASE-SENSITIVE
				//Por disposicion de Sergio, lo pasamos todo a mayusculas
				if (unEncriptedPass.ToUpper() == password.ToUpper()) 
					return true;
			}

			return false;
        }

        #region MejorasFRTostadoraElDorado600R6 JEV
        /// <summary>
        /// Busca si un usuario tiene privielgios en una opción específica en un grupo
        /// </summary>
        /// <param name="usuario">Código del usuario</param>
        /// <param name="accion">Número de acción</param>
        /// <param name="tienePermiso">Retorna el resultado si el usuario tiene privilegio</param>
        /// <returns></returns>
        protected static bool VerificarPermisoEnGrupo(string usuario, int accion, out bool tienePermiso)
        {
            bool procesoExitoso = true;
            bool tienePriv = false;
            string sentencia = string.Empty;


            try
            {
                sentencia = "SELECT COUNT(0) FROM SYSTEM_PRIVILEGIO_EX pe WHERE pe.USUARIO IN( \n";
                sentencia += String.Format(" SELECT m.GRUPO FROM SYSTEM_MEMBRESIA m WHERE m.USUARIO = '{0}') AND pe.ACCION = {1} ", usuario, accion);

                object valor = GestorDatos.EjecutarScalar(sentencia);

                if (valor != null && !(valor is DBNull) && Convert.ToInt32(valor) > 0)
                {
                    tienePriv = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error verificando si el usuario forma parte de un grupo.", ex);
            }

            tienePermiso = tienePriv;

            return procesoExitoso;
        }

        /// <summary>
        /// Verifica en la tabla de privilegios si el usuario tiene asociado un permiso.
        /// </summary>
        /// <param name="permiso">Numero de permiso a buscar.</param>
        /// <returns></returns>
        public static bool VerificarPermiso(string usuario, int permiso, out bool tienePrivilegio)
        {
            bool tienePermiso = true;
            bool procesoExitoso = true;

            try
            {
                if (procesoExitoso)
                {
                    procesoExitoso = VerificarPermisoEnGrupo(usuario, permiso, out tienePermiso);
                }

                //Se valida los privilegios individuales
                if (!tienePermiso && procesoExitoso)
                {
                    // TODO: Verificar los privilegios por grupos y por usuario.
                    string sentencia =
                        "SELECT COUNT(0) FROM SYSTEM_PRIVILEGIO_EX " +
                        "WHERE usuario = '" + usuario + "' " +
                        "AND ACCION = " + permiso;


                    object valor = GestorDatos.EjecutarScalar(sentencia);

                    if (valor != null && !(valor is DBNull) && Convert.ToInt32(valor) > 0)
                        tienePermiso = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error verificando permisos de usuario.", ex);
            }

            tienePrivilegio = tienePermiso;

            return procesoExitoso;
        }
        #endregion MejorasFRTostadoraElDorado600R6 JEV

        #endregion
    }
}
