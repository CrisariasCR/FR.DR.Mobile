using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMF.GPS;
using System.Threading;


namespace Softland.ERP.FR.Mobile.Cls.Utilidad
{
    public static class Ubicacion
    {
        #region Atributos
        /// <summary>
        ///  Constante para el tiempo de espera para obtener la ubicación.
        /// </summary>
        private static int tiempoEspera = 60;
        /// <summary>
        /// Indica si el proceso se encuentra iniciado.
        /// </summary>
        private static bool iniciado = false;
        /// <summary>
        /// Indica cuando el tiempo de espera se ha agotado.
        /// </summary>
        private static bool tiempoAcabado = false;
        /// <summary>
        /// Posición actual obtenida
        /// </summary>
        private static FRGpsPosition posicionActual = null;
        /// <summary>
        /// Controlador de la ubicación. Se encarga de manejar el hilo 
        /// que ejecuta el precoeso para obtener la ubicación.
        /// </summary>
        private static Location controlador = new Location();
        /// <summary>
        /// Estado actual de la toma de ubicación.
        /// </summary>
        private static string estado = String.Empty;
        /// <summary>
        /// Estado del dispositivo
        /// </summary>
        private static GpsServiceState estadoDispositivo = GpsServiceState.Unknown;
        /// <summary>
        /// Evento para el cambio de estado
        /// </summary>
        private static event EventHandler stateChanged;
        /// <summary>
        /// Cola para el manejo de un rango de posiciones
        /// </summary>
        private static Queue<FRGpsPosition> posiciones = new Queue<FRGpsPosition>(CANTIDAD_POSICIONES);
        /// <summary>
        /// Contante para la cantidad de posiciones dentro de la cola.
        /// </summary>
        private static int CANTIDAD_POSICIONES = 25;
        /// <summary>
        /// Indica el valor cuando no se quiere detener el proceso.
        /// </summary>
        public static int NO_PARAR = -1;
        /// <summary>
        /// Representa el valor del timpo de espera por omisión( 60 segundos)
        /// </summary>
        public static int TIEMPO_ESPERA_OMISION = 60;
        /// <summary>
        /// Cantidad de segundos aceptables antes de la fecha de inicio.
        /// </summary>
        private static uint segundosAceptables = 10; 
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene la posición actual.
        /// </summary>
        public static FRGpsPosition PosicioActual
        {
            get
            {
                return Ubicacion.posicionActual;
            }
        }
        /// <summary>
        /// Obtiene el mensaje de error si falla al obtener la ubicación.
        /// </summary>
        public static String Error
        {
            get
            {
                return controlador.Error;
            }
        }
        /// <summary>
        /// Inidica si el tiempo de espera se ha terminado.
        /// </summary>
        public static bool TiempoAcabado
        {
            get
            {
                return tiempoAcabado;
            }
        }
        /// <summary>
        /// Obtiene el estado actual de la toma de ubicación.
        /// </summary>
        public static String Estado
        {
            get
            {
                return Ubicacion.estado;
            }
        }
        /// <summary>
        /// Estado actual del dispositivo
        /// </summary>
        public static GpsServiceState EstadoDispositivo
        {
            get
            {
                return estadoDispositivo;
            }
        }
        public static event EventHandler StateChanged
        {
            add
            {
                stateChanged += value;
            }
            remove
            {
                stateChanged -= value;
            }
        }
        /// <summary>
        /// Obtiene un arreglo con las pociones almacenadas.
        /// </summary>
        public static FRGpsPosition[] Posiciones
        {
            get
            {
                return posiciones.ToArray();
            }
        }

        /// <summary>
        /// Obtiene la cantidad de segundos que lleva el proceso ejecutandose.
        /// </summary>
        public static int TiempoCorrida
        {
            get
            {
                return controlador.RunningTime;
            }
        }
        /// <summary>
        /// Cantidad de segundos aceptables antes de la fecha de inicio.
        /// </summary>
        public static uint SegundosAceptables
        {
            get
            {
                return segundosAceptables;
            }
            set
            {
                segundosAceptables = value;
            }
        }
        #endregion

        #region Eventos
        private static void controlador_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            posicionActual = new FRGpsPosition(args.Position,DateTime.Now);
            GuardarPosicion();
        }

        private static void controlador_DeviceStateChanged(object sender, DeviceStateChangedEventArgs args)
        {
            Ubicacion.estadoDispositivo = args.DeviceState;
            switch (args.DeviceState)
            {
                case GpsServiceState.Off:
                    estado = "Dispositivo apagado";
                    break;
                case GpsServiceState.On:
                    estado = "Dispositivo encendido";
                    break;
                case GpsServiceState.StartingUp:
                    estado = "Iniciando la recepción de la posición global";
                    break;
                case GpsServiceState.ShuttingDown:
                    estado = "Apagando el dispositivo";
                    break;
                case GpsServiceState.Uninitialized:
                    estado = "Dispositivo no incializado";
                    break;
                case GpsServiceState.Unknown:
                    estado = "Estado del dispositivo desconocido";
                    break;
                case GpsServiceState.Unloading:
                    estado = "Dispositivo no cargado";
                    break;
            }

            if (stateChanged != null)
            {
                EventArgs e = new EventArgs();
                stateChanged(sender, new EventArgs());
            }
        }

        #endregion

        #region Métodos
        /// <summary>
        /// Inicializa los objetos de la instancia.
        /// </summary>
        public static void Inicializar()
        {
            // TODO
            //if (controlador == null)
            //{
            //    controlador = new Location();
            //    controlador.DeviceStateChanged += new DeviceStateChangedEventHandler(controlador_DeviceStateChanged);
            //    controlador.LocationChanged += new LocationChangedEventHandler(controlador_LocationChanged);
            //}
        }
        /// <summary>
        /// Libera la memoria de los objetos utilizados. 
        /// </summary>
        public static void Finalizar()
        {
            controlador.StopPositionSearch();
            controlador.Dispose();
            posicionActual = null;
            controlador = null;
            posiciones = null;
            controlador.DeviceStateChanged -= new DeviceStateChangedEventHandler(controlador_DeviceStateChanged);
            controlador.LocationChanged -= new LocationChangedEventHandler(controlador_LocationChanged);
        }
        /// <summary>
        /// Inicia el proceso para obtener la ubicación.
        /// </summary>
        /// <param name="tiempoEspera">Cantidad de segundos por esperar</param>
        public static void Iniciar(int tiempoEspera)
        {
            if (!iniciado)
            {
                iniciado = true;
            }
            else
            {
                controlador.StopPositionSearch();
                posicionActual = null;
                posiciones.Clear();
            }
            Ubicacion.tiempoEspera = tiempoEspera;
            controlador.StartPositionSearch();
            
            /*Thread thread = new Thread(GuardarPosicion);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();*/
        }
        /// <summary>
        /// Detiene el proceso de obtención de datos.
        /// </summary>
        public static void Parar()
        {
            controlador.StopPositionSearch();
            iniciado = false;
        }
        /// <summary>
        /// Guarda la posición obtenida
        /// </summary>
        private static void GuardarPosicion()
        {
            tiempoAcabado = tiempoEspera != NO_PARAR && controlador.RunningTime >= tiempoEspera;
            if (tiempoAcabado)
            {
                controlador.StopPositionSearch();
                estado = "Tiempo agotado para la recepción de la posición.";
            }
            if (!tiempoAcabado && EsPosicionValida(posicionActual))
            {
                estado = "Posición obtenida correctamente.";
                Enqueue(posicionActual);

            }
            if (estadoDispositivo == GpsServiceState.Unknown)
            {
                estado = "No se ha podido iniciar el dispositivo. Por favor revisar las configuraciones GPS del dispositivo.";
            }
        }
        /// <summary>
        /// Pone en la cola la posición. Si la cola está llena elimina el elemento
        /// más antiguo.
        /// </summary>
        /// <param name="posicion"></param>
        private static void Enqueue(FRGpsPosition posicion)
        {
            if (posicion.GpsPosition.PositionDilutionOfPrecision <= 6)
            {
                if (posiciones.Count >= CANTIDAD_POSICIONES)
                {
                    posiciones.Dequeue();
                }
                posiciones.Enqueue(posicion);
            }
        }
        /// <summary>
        /// Obtiene la mejor posició dentro de un rango de fechas. Si se asignan los parámetros (latitud,longitud)
        /// es método utilizara el algoritmo de la menor distancia. Por lo contrario, utilizara el de mejor PDOP.
        /// </summary>
        /// <param name="inicio">Fecha de inicio del rango</param>
        /// <param name="fin">Fecha de finalización del rango</param>
        /// <param name="latitud">Latitud de ubicación a comparar.</param>
        /// <param name="longitud">Longitud de la ubicación a comparar.</param>
        /// <returns></returns>
        public static FRGpsPosition ObtenerMejorPosicion(DateTime inicio, DateTime fin, double latitud, double longitud)
        {
            FRGpsPosition resultado = null;
            // Se considera posición valida inclusive 10 segundos antes.
            inicio = inicio.AddSeconds(segundosAceptables * -1);
            bool usarDistancia = !Double.IsNaN(latitud) && !Double.IsNaN(longitud);
            try
            {
                double menorDistancia = 0;
                DateTime i = inicio;
                EscribirPosicionesBitacora(latitud,longitud);
                var query = (from FRGpsPosition posicion in Posiciones
                             //where EstaEntreFecha(posicion.Time,inicio,fin) == true
                             where posicion.LocalSystemTime.ToLocalTime().CompareTo(inicio) >= 0
                             && posicion.LocalSystemTime.ToLocalTime().CompareTo(fin) <= 0
                            orderby posicion.GpsPosition.PositionDilutionOfPrecision ascending
                            select posicion).ToArray();
                if (query.Count() > 0)
                {
                    //Asigna la mejor distancia a la primera posición
                    resultado = query[0];
                    if (usarDistancia)
                    {
                        menorDistancia = ObtenerDistancia(latitud, longitud, resultado.GpsPosition.Latitude, resultado.GpsPosition.Longitude);
                    }
                }
                if (usarDistancia)
                {
                    // Busca la posición con la mejor distancia.
                    foreach (FRGpsPosition posicion in query)
                    {
                        double distnacia = ObtenerDistancia(latitud, longitud, posicion.GpsPosition.Latitude, posicion.GpsPosition.Longitude);
                        if (distnacia < menorDistancia)
                        {
                            menorDistancia = distnacia;
                            resultado = posicion;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo la mejor posición", ex);
            }
            return resultado;
        }
        /// <summary>
        /// Obtiene la distancia entr dos puntos
        /// </summary>
        /// <param name="latitud1"></param>
        /// <param name="longitud1"></param>
        /// <param name="latitud2"></param>
        /// <param name="longitud2"></param>
        /// <returns></returns>
        private static double ObtenerDistancia(double latitud1, double longitud1, double latitud2, double longitud2)
        {
            double resultado = 0;
            double dtr = Math.PI / 180;
            double rtd = 180 / Math.PI;
            resultado = Math.Acos((Math.Sin(latitud1 * dtr) * Math.Sin(latitud2 * dtr))
                + (Math.Cos(latitud1 * dtr) * Math.Cos(latitud2 * dtr) * Math.Cos((longitud1 - longitud2) * dtr)))
                * rtd * 111.302 * 1000;
            return resultado;
        }

        /// <summary>
        /// Verifica que los datos de la posición sean válidos.
        /// </summary>
        /// <param name="posicion"></param>
        /// <returns></returns>
        private static bool EsPosicionValida(FRGpsPosition posicion)
        {
            return posicion.GpsPosition != null && posicion.GpsPosition.TimeValid
                && posicion.GpsPosition.PositionDilutionOfPrecisionValid
                && posicion.GpsPosition.LatitudeValid && posicion.GpsPosition.LongitudeValid
                && posicion.GpsPosition.SeaLevelAltitudeValid;
        }
        private static bool EstaEntreFecha(DateTime fecha, DateTime inicio, DateTime fin)
        {
            bool a = fecha.ToLocalTime().CompareTo(inicio) >= 0;
            bool b = fecha.ToLocalTime().CompareTo(fin) <= 0;
            return a && b;
        }
        private static void EscribirPosicionesBitacora(double latitud, double longitud)
        {
            Bitacora.Escribir(String.Format("Posiciones - La:{0},Lo{1}",latitud,longitud));
            foreach (FRGpsPosition posicion in Posiciones)
            {
                double distancia = ObtenerDistancia(latitud, longitud, posicion.GpsPosition.Latitude, posicion.GpsPosition.Longitude);
                Bitacora.Escribir(String.Format("Posicion- Dis:{4},La:{0},Lo:{1},Al{2},Ti:{3}"
                    , posicion.GpsPosition.Latitude, posicion.GpsPosition.Longitude, posicion.GpsPosition.SeaLevelAltitude, posicion.GpsPosition.Time,distancia));
            }
        }
        #endregion
    }
}
