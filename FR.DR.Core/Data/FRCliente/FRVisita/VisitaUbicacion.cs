using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.GPS;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita
{
    public class VisitaUbicacion
    {
        #region Atributos
        /// <summary>
        /// Código de la ruta
        /// </summary>
        private string ruta = null;
        /// <summary>
        /// Indentificador del cliente.
        /// </summary>
        private string cliente = null;
        /// <summary>
        /// Fecha de inicio de la visita.
        /// </summary>
        private DateTime inicio;
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
        /// Tiempo tomado de los satélites GPS.
        /// </summary>
        private DateTime tiempoGPS;
        /// <summary>
        /// Tiempo tomado del sistema.
        /// </summary>
        private DateTime tiempoPDA;
        /// <summary>
        /// Indicador de la precisión de la señal con la que fue tomada la posición.
        /// </summary>
        private float positionDilutionOfPrecision;
        /// <summary>
        /// Error presentado al obtener la posición.
        /// </summary>
        private string error;
        /// <summary>
        /// Indica cuando la ubicación es válida
        /// </summary>
        private bool ubicacionValida = false;


        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene o cambia la ruta de donde fue realizado el pedido.
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
        /// Fecha de inicio de la visita
        /// </summary>
        public DateTime Inicio
        {
            get
            {
                return inicio;
            }
            set
            {
                inicio = value;
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
        /// Obtiene o cambia el tiempo obtenido desde los satélites
        /// </summary>
        public DateTime TiempoGPS
        {
            get
            {
                return tiempoGPS;
            }
            set
            {
                tiempoGPS = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el tiempo obtenido del sistema.
        /// </summary>
        public DateTime TiempoPDA
        {
            get
            {
                return tiempoPDA;
            }
            set
            {
                tiempoPDA = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el valor de la preción de la señal.
        /// </summary>
        public float PositionDilutionOfPrecision
        {
            get
            {
                return positionDilutionOfPrecision;
            }
            set
            {
                positionDilutionOfPrecision = value;
            }
        }
        /// <summary>
        /// Obtiene o cambia el error presentado al obtener la ubicación.
        /// </summary>
        public string Error
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
        public VisitaUbicacion(Visita visita, GpsPosition ubicacion, DateTime tiempoPDA, string error)
        {
            cliente = visita.Cliente.Codigo;
            ruta = visita.Cliente.Zona;
            inicio = visita.FechaInicio;
            if (ubicacion != null)
            {
                ubicacionValida = true;
                latitud = ubicacion.Latitude;
                longitud = ubicacion.Longitude;
                altitud = ubicacion.SeaLevelAltitude;
                tiempoGPS = ubicacion.Time.ToLocalTime();
                positionDilutionOfPrecision = ubicacion.PositionDilutionOfPrecision;
            }
            this.tiempoPDA = tiempoPDA;
            this.error = error ?? String.Empty;
            
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Guarda el registro de la ubicación del cliente.
        /// </summary>
        /// <returns></returns>
        public bool Guardar()
        {
            bool resultado = false;
            try
            {
                StringBuilder sentencia = new StringBuilder();
                if (ubicacionValida)
                {
                    sentencia.AppendLine(String.Format(" INSERT INTO {0} ", Table.ERPADMIN_VISITA_UBICACION));
                    sentencia.AppendLine(" (CLIENTE,  RUTA, INICIO, LATITUD, LONGITUD, ALTITUD, TIEMPO_GPS, TIEMPO_PDA, PDOP, ERROR) ");
                    sentencia.AppendLine(" VALUES(@CLIENTE, @RUTA, @INICIO, @LATITUD, @LONGITUD, @ALTITUD, @TIEMPO_GPS, @TIEMPO_PDA, @PDOP, @ERROR) ");
                }
                else
                {
                    sentencia.AppendLine(String.Format(" INSERT INTO {0} ", Table.ERPADMIN_VISITA_UBICACION));
                    sentencia.AppendLine(" (CLIENTE,  RUTA, INICIO, TIEMPO_PDA, ERROR) ");
                    sentencia.AppendLine(" VALUES(@CLIENTE, @RUTA, @INICIO, @TIEMPO_PDA, @ERROR) ");
                }

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente),
                GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                GestorDatos.SQLiteParameter("@INICIO",SqlDbType.DateTime, inicio),
                GestorDatos.SQLiteParameter("@LATITUD",SqlDbType.Decimal, latitud),
                GestorDatos.SQLiteParameter("@LONGITUD",SqlDbType.Decimal, longitud),
                GestorDatos.SQLiteParameter("@ALTITUD",SqlDbType.Decimal, altitud),
                GestorDatos.SQLiteParameter("@TIEMPO_GPS",SqlDbType.DateTime, tiempoGPS),
                GestorDatos.SQLiteParameter("@TIEMPO_PDA",SqlDbType.DateTime, tiempoPDA),
                GestorDatos.SQLiteParameter("@PDOP",SqlDbType.Decimal, positionDilutionOfPrecision),
                GestorDatos.SQLiteParameter("@ERROR",SqlDbType.NVarChar, error)});

                int registros = GestorDatos.EjecutarComando(sentencia.ToString(), parametros);
                resultado = registros > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando la ubicación de la visita", ex);
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza el registro de la ubicación de la visita del cliente.
        /// </summary>
        /// <returns></returns>
        public bool Actualizar()
        {
            bool resultado = false;
            try
            {
                StringBuilder sentencia = new StringBuilder();
                sentencia.AppendLine(String.Format("UPDATE {0} SET LATITUD = @LATITUD ",Table.ERPADMIN_VISITA_UBICACION));
                sentencia.AppendLine(" , LONGITUD = @LONGITUD, ALTITUD = @ALTITUD, TIEMPO_GPS = @TIEMPO_GPS ");
                sentencia.AppendLine(" , TIEMPO_PDA = @TIEMPO_PDA, PDOP = @PDOP, ERROR = @ERROR ");
                sentencia.AppendLine(" WHERE RUTA = @RUTA AND CLIENTE = @CLIENTE AND INICIO = @INICIO");

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar, cliente),
                GestorDatos.SQLiteParameter("@RUTA",SqlDbType.NVarChar, ruta),
                GestorDatos.SQLiteParameter("@INICIO",SqlDbType.DateTime, inicio),
                GestorDatos.SQLiteParameter("@LATITUD",SqlDbType.Decimal, latitud),
                GestorDatos.SQLiteParameter("@LONGITUD",SqlDbType.Decimal, longitud),
                GestorDatos.SQLiteParameter("@ALTITUD",SqlDbType.Decimal, altitud),
                GestorDatos.SQLiteParameter("@TIEMPO_GPS",SqlDbType.DateTime, tiempoGPS),
                GestorDatos.SQLiteParameter("@TIEMPO_PDA",SqlDbType.DateTime, tiempoPDA),
                GestorDatos.SQLiteParameter("@PDOP",SqlDbType.Decimal, positionDilutionOfPrecision),
                GestorDatos.SQLiteParameter("@ERROR",SqlDbType.NVarChar, error)});

                int registros = GestorDatos.EjecutarComando(sentencia.ToString(), parametros);
                resultado = registros > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando la ubicación de la visita.", ex);
            }
            return resultado;
        }


        #endregion
    }
}
