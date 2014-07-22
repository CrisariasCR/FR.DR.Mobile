using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.GPS;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{
    public class ClienteUbicacion
    {

        #region Atributos
        /// <summary>
        /// Identificador de la ruta.
        /// </summary>
        private string ruta = null;
        /// <summary>
        /// Indentificador del cliente.
        /// </summary>
        private string cliente = null;
        /// <summary>
        /// Latitud de la lectura del cliente.
        /// </summary>
        private double latitud = 0;
        /// <summary>
        /// Longitud de la lectura del cliente.
        /// </summary>
        private double longitud = 0;
        /// <summary>
        /// Altitud de la lectura del cliente.
        /// </summary>
        private double altitud = 0;
        /// <summary>
        /// Error presentado en la lectura de la posición
        /// </summary>
        private string error = String.Empty;
        /// <summary>
        /// Estado del log que se va a generar
        /// </summary>
        private string estado = String.Empty;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o cambia la ruta
        /// </summary>
        public string Ruta
        {
            get
            {
                return ruta;
            }
            set
            {
                ruta = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el cliente.
        /// </summary>
        public string Cliente
        {
            get
            {
                return cliente;
            }
            set
            {
                cliente = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el valor de la latitud
        /// </summary>
        public double Latitud
        {
            get
            {
                return latitud;
            }
            set
            {
                latitud = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el valor de la longitud
        /// </summary>
        public double Longitud
        {
            get
            {
                return longitud;
            }
            set
            {
                latitud = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el valor de la altitud
        /// </summary>
        public double Altitud
        {
            get
            {
                return altitud;
            }
            set
            {
                altitud = value;
            }
        }
        /// <summary>
        /// Obtinen o cambia el error presentado en la lectura de la posición.
        /// </summary>
        public String Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
            }
        }
        #endregion

        #region Constructores
        public ClienteUbicacion(Cliente cliente, GpsPosition ubicacion, String error)
        {
            ruta = cliente.Zona;
            this.cliente = cliente.Codigo;
            if (ubicacion != null)
            {
                latitud = ubicacion.Latitude;
                longitud = ubicacion.Longitude;
                altitud = ubicacion.SeaLevelAltitude;
            }
            this.error = error ?? String.Empty;
        }

        public ClienteUbicacion(string ruta, string cliente, double latitud, double longitud, double altitud)
        {
            this.ruta = ruta;
            this.cliente = cliente;
            this.latitud = latitud;
            this.longitud = longitud;
            this.altitud = altitud;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Guarda los cambios realizados al registro.
        /// </summary>
        public void Guardar()
        {
            bool tieneUbicacion = TieneUbicacion(ruta, cliente);
            if (tieneUbicacion)
            {
                estado = String.IsNullOrEmpty(error)? "A" : "E";
                Actualizar();
            }
            else
            {
                estado = String.IsNullOrEmpty(error) ? "N" : "E";
                Crear();
            }
        }
        /// <summary>
        /// Guarda el registro de la ubicación del cliente.
        /// </summary>
        /// <returns></returns>
        public bool Crear()
        {
            bool resultado = false;
            try
            {
                resultado = GuardarLog();
                if (resultado)
                {
                    string sentencia =
                    " INSERT INTO " + Table.ERPADMIN_CLIENTE_UBICACION +
                    "       (RUTA, CLIENTE, LATITUD, LONGITUD, ALTITUD) " +
                    " VALUES(@RUTA,@CLIENTE,@LATITUD,@LONGITUD,@ALTITUD) ";

                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                        GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                        GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente),
                        GestorDatos.SQLiteParameter("@LATITUD",SqlDbType.Decimal, latitud),
                        GestorDatos.SQLiteParameter("@LONGITUD",SqlDbType.Decimal, longitud),
                        GestorDatos.SQLiteParameter("@ALTITUD",SqlDbType.Decimal, altitud)});

                    int registros = GestorDatos.EjecutarComando(sentencia, parametros);
                    resultado = registros > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultado;
        }

        /// <summary>
        /// Guarda el registro de la ubicación del cliente.
        /// </summary>
        /// <returns></returns>
        public bool GuardarLog()
        {
            bool resultado = false;
            try
            {
                string sentencia =
                    " INSERT INTO " + Table.ERPADMIN_CLIENTE_UBICACION_LOG +
                    "       (RUTA, CLIENTE, FECHA, ESTADO, LATITUD, LONGITUD, ALTITUD, ERROR) " +
                    " VALUES(@RUTA,@CLIENTE,@FECHA,@ESTADO,@LATITUD,@LONGITUD,@ALTITUD,@ERROR) ";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente),
                GestorDatos.SQLiteParameter("@FECHA",SqlDbType.DateTime, DateTime.Now),
                GestorDatos.SQLiteParameter("@ESTADO",SqlDbType.NVarChar, estado),
                GestorDatos.SQLiteParameter("@LATITUD",SqlDbType.Decimal, latitud),
                GestorDatos.SQLiteParameter("@LONGITUD",SqlDbType.Decimal, longitud),
                GestorDatos.SQLiteParameter("@ALTITUD",SqlDbType.Decimal, altitud),
                GestorDatos.SQLiteParameter("@ERROR",SqlDbType.NVarChar, error)});

                int registros = GestorDatos.EjecutarComando(sentencia, parametros);
                resultado = registros > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultado;
        }
        /// <summary>
        /// Indica si la ubicación del cliente ya ha sido registrada.
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public static bool TieneUbicacion(string ruta,string cliente)
        {
            bool resultado = false;
            try
            {
                string sentencia = String.Format("SELECT COUNT(0) FROM {0} WHERE RUTA = @RUTA AND CLIENTE = @CLIENTE"
                    ,Table.ERPADMIN_CLIENTE_UBICACION);

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente)});

                object registros = GestorDatos.EjecutarScalar(sentencia,parametros);
                resultado = registros != null && Convert.ToInt32(registros) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verificando si la ubicación del cliente existe", ex);
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza el registro de la ubicación del cliente.
        /// </summary>
        /// <returns></returns>
        private bool Actualizar()
        {
            bool resultado = false;
            try
            {
                resultado = GuardarLog();
                if (resultado)
                {
                    StringBuilder sentencia = new StringBuilder();
                    sentencia.AppendLine(String.Format("UPDATE {0} SET LATITUD = @LATITUD ", Table.ERPADMIN_CLIENTE_UBICACION));
                    sentencia.AppendLine(" , LONGITUD = @LONGITUD, ALTITUD = @ALTITUD ");
                    sentencia.AppendLine(" WHERE RUTA = @RUTA AND CLIENTE = @CLIENTE");

                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                        GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente),
                        GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                        GestorDatos.SQLiteParameter("@LATITUD",SqlDbType.Decimal, latitud),
                        GestorDatos.SQLiteParameter("@LONGITUD",SqlDbType.Decimal, longitud),
                        GestorDatos.SQLiteParameter("@ALTITUD",SqlDbType.Decimal, altitud)});

                    int registros = GestorDatos.EjecutarComando(sentencia.ToString(), parametros);
                    resultado = registros > 0;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultado;
        }

        /// <summary>
        /// Carga la información del cliente y su ubicación.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public static ClienteUbicacion Cargar(string ruta, string cliente)
        {
            ClienteUbicacion resultado = null;
            SQLiteDataReader reader = null;
            try
            {
                string sentencia = String.Format("SELECT ruta, cliente, latitud, longitud, altitud FROM {0} WHERE RUTA = @RUTA AND CLIENTE = @CLIENTE"
                    , Table.ERPADMIN_CLIENTE_UBICACION);

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente)});

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    double latitud = Convert.ToDouble(reader.GetDecimal(2));
                    double longitud = Convert.ToDouble(reader.GetDecimal(3));
                    double altitud = Convert.ToDouble(reader.GetDecimal(4));
                    resultado = new ClienteUbicacion(ruta, cliente, latitud,longitud,altitud);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando la información de la ubicación de un cliente.", ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
            return resultado;
        }
        #endregion
    }
}
