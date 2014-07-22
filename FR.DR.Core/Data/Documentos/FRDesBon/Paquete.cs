using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
//using System.Xml.Linq;
using System.Data;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class Paquete
    {
        #region Attributes
        private String compania;
        private String paquete;
        private String descripcion;
        private bool activo;
        private DateTime fechaDesde;
        private DateTime fechaHasta;
        private int prioridad;
        private String diasXML;
        private List<DayOfWeek> dias;

        private List<Regla> reglas;

        // KFC - Cambios Motor de Precios 7.0
        private bool aplicaTodaRuta;
        private List<string> rutas;

        #region Dias
        private const String DIAS_NAME_TAG = "name";
        private const String DIAS_VAL_TAG = "val";

        private const String LUNES = "L";
        private const String MARTES = "M";
        private const String MIERCOLES = "K";
        private const String JUEVES = "J";
        private const String VIERNES = "V";
        private const String SABADO = "S";
        private const String DOMINGO = "D";
        #endregion
        #endregion

        #region Properties
        public String Compania
        {
            get { return compania; }
            set { compania = value; }
        }
        public String Paquete1
        {
            get { return paquete; }
            set { paquete = value; }
        }
        public String Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        public bool Activo
        {
            get { return activo; }
            set { activo = value; }
        }

        public DateTime FechaDesde
        {
            get { return fechaDesde; }
            set { fechaDesde = value; }
        }
        public DateTime FechaHasta
        {
            get { return fechaHasta; }
            set { fechaHasta = value; }
        }
        public int Prioridad
        {
            get { return prioridad; }
            set { prioridad = value; }
        }
        public List<DayOfWeek> Dias
        {
            get { return dias; }
            set { dias = value; }
        }
        public String DiasXML
        {
            get { return diasXML; }
            set { diasXML = value; }
        }
        public List<Regla> Reglas
        {
            get { return reglas; }
            set { reglas = value; }
        }

        // KFC - Cambios Motor Precios 7.0
        public bool AplicaTodaRuta
        {
            get { return aplicaTodaRuta; }
            set { aplicaTodaRuta = value; }
        }

        public List<string> Rutas
        {
            get { return rutas; }
            set { rutas = value; }
        }


        #endregion

        #region Constructors
        
        #endregion

        #region Methods
        public static Paquete Find(String compania, String paquete)
        {
            Paquete resultado = null;

            return resultado;
        }
        public static List<Paquete> FindAll(String compania)
        {
            List<Paquete> resultado = new List<Paquete>();
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT COMPANIA,PAQUETE,DESCRIPCION,ACTIVO,FECHA_DESDE,FECHA_HASTA,PRIORIDAD,DIAS,APLICA_TODA_RUTA ");
            sentencia.AppendLine(String.Format(" FROM {0}", Table.ERPADMIN_DES_BON_PAQUETE));
            sentencia.AppendLine(" WHERE UPPER(COMPANIA) = @COMPANIA AND ACTIVO = 'S' ");
            sentencia.AppendLine(" ORDER BY PRIORIDAD ");
            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()) });

            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                while (reader.Read())
                {
                    Paquete paquete = new Paquete();

                    paquete.Compania = reader.GetString(0) ;
                    paquete.Paquete1 = reader.GetString(1);
                    paquete.Descripcion = reader.GetString(2);
                    paquete.Activo = reader.GetString(3) == "S";
                    paquete.FechaDesde = reader.GetDateTime(4);
                    paquete.FechaHasta = reader.GetDateTime(5);
                    paquete.Prioridad = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    paquete.DiasXML = reader.IsDBNull(7) ? null : reader.GetString(7);

                    paquete.Dias = ObtenerDias(paquete.DiasXML);
                    
                    // KFC - cambios mp 7
                    paquete.aplicaTodaRuta = reader.GetString(8) == "S";

                    if (!paquete.aplicaTodaRuta)
                    {
                        paquete.rutas = ObtieneRutasQueAplican(paquete);
                    }
                    //<< kfc


                    paquete.Reglas = Regla.FindAll(paquete.Compania, paquete.Paquete1);

                    resultado.Add(paquete);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error cargando los paquetes de descuentos-bonificación para la compañía '{0}'. {1}  '" , compania, ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }

        public static bool Verificar(Paquete paquete, DateTime now)
        {
            return paquete.Dias.Contains(now.DayOfWeek) && (now.Date >= paquete.FechaDesde.Date && now.Date <= paquete.FechaHasta.Date)
                && (now.TimeOfDay >= paquete.FechaDesde.TimeOfDay && now.TimeOfDay <= paquete.FechaHasta.TimeOfDay) ;
        }
        public bool Verificar(DateTime now)
        {
            return Verificar(this,now);
        }
        private static List<DayOfWeek> ObtenerDias(string xmlDias)
        {
            List<DayOfWeek> resultado = null;
            if (!String.IsNullOrEmpty(xmlDias))
            {
                System.Xml.Linq.XElement diasElement = System.Xml.Linq.XElement.Parse(xmlDias);
                resultado = new List<DayOfWeek>(diasElement.Nodes().Count());
                foreach (System.Xml.Linq.XElement dia in diasElement.Nodes())
                {
                    String valorDia = dia.Attribute(DIAS_VAL_TAG).Value;
                    switch (valorDia)
                    {
                        case LUNES:
                            resultado.Add(DayOfWeek.Monday);


                            break;
                        case MARTES:
                            resultado.Add(DayOfWeek.Tuesday);

                            break;
                        case MIERCOLES:
                            resultado.Add(DayOfWeek.Wednesday);


                            break;
                        case JUEVES:
                            resultado.Add(DayOfWeek.Thursday);


                            break;
                        case VIERNES:
                            resultado.Add(DayOfWeek.Friday);


                            break;
                        case SABADO:
                            resultado.Add(DayOfWeek.Saturday);


                            break;
                        case DOMINGO:
                            resultado.Add(DayOfWeek.Sunday);




                            break;
                    }
                }
            }
            return resultado;
        }
        #endregion

        #region Cambios Motor Precios KFC

        private static List<string> ObtieneRutasQueAplican(Paquete paquete)
        {
            List<string> rutasQueAplican = new List<string>();

            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT RUTA ");
            sentencia.AppendLine(String.Format(" FROM {0}", Table.ERPADMIN_DES_BON_PAQUETE));
            sentencia.AppendLine(string.Format(" WHERE PAQUETE = '{0}' ", paquete.Paquete1));

            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString());

                while (reader.Read())
                {
                    rutasQueAplican.Add(reader.GetString(0));
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error cargando las rutas que aplican para el paquete de descuentos {0}. Error: {1}  '", paquete.Paquete1, ex.Message));
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }

            return rutasQueAplican;
        }



        /// <summary>
        /// Verifica si el paquete aplica para el código de ruta que se indica por parámetro
        /// </summary>
        /// <param name="CodigoTienda">Código de la Ruta de la cual se debe verificar</param>
        /// <returns>True si la ruta aplica para el paquete</returns>
        public bool VerificaRuta(string CodigoRuta)
        {
            return VerificaRuta(this, CodigoRuta);
        }

        /// <summary>
        /// Verifica si el paquete aplica para el código de ruta que se indica por parámetro
        /// </summary>
        /// <param name="PaqueteVerificar">Información del Paquete del cual se verifica la aplicación de la ruta</param>
        /// <param name="CodigoRuta">Código de la Ruta de la cual se debe verificar</param>
        /// <returns>True si la ruta aplica para el paquete</returns>
        public static bool VerificaRuta(Paquete PaqueteVerificar, string CodigoRuta)
        {
            return (PaqueteVerificar.aplicaTodaRuta || PaqueteVerificar.rutas.Contains(CodigoRuta));
        }

        #endregion
    }
}
