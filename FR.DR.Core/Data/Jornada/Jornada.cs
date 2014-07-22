using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using System.Data;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente.FRJornada
{
    /// <summary>
    /// Clase que representa una visita al cliente
    /// </summary>
    public class Jornada: IPrintable
    {
        #region Variables y Propiedades de instancia

        //Rubros

        private string rubro1;
        public string Rubro1
        {
            get { if (string.IsNullOrEmpty(rubro1)) return string.Empty; else return rubro1; }
            set { rubro1 = value; }
        }

        private string rubro2;
        public string Rubro2
        {
            get { if (string.IsNullOrEmpty(rubro2)) return string.Empty; else return rubro2; }
            set { rubro2 = value; }
        }

        private string rubro3;
        public string Rubro3
        {
            get { if (string.IsNullOrEmpty(rubro3)) return string.Empty; else return rubro3; }
            set { rubro3 = value; }
        }

        private string rubro4;
        public string Rubro4
        {
            get { if (string.IsNullOrEmpty(rubro4)) return string.Empty; else return rubro4; }
            set { rubro4 = value; }
        }

        private string rubro5;
        public string Rubro5
        {
            get { if (string.IsNullOrEmpty(rubro5)) return string.Empty; else return rubro5; }
            set { rubro5 = value; }
        }

        //RubrosDesc
        public string Rubro1Desc
        {
            get { return FRdConfig.Rubro1Jornada; }
        }
        
        public string Rubro2Desc
        {
            get { return FRdConfig.Rubro2Jornada; }         
        }

        public string Rubro3Desc
        {
            get { return FRdConfig.Rubro3Jornada; }        
        }

        public string Rubro4Desc
        {
            get { return FRdConfig.Rubro4Jornada; }        
        }

        public string Rubro5Desc
        {
            get { return FRdConfig.Rubro5Jornada; }        
        }

        private ClienteBase cliente ;
        /// <summary>
        /// Nombre del Cliente visitado (para el reporte).
        /// </summary>
        public ClienteBase Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        private DateTime fechaJornada= DateTime.Now;
        /// <summary>
        /// Fecha de proxima visita.
        /// </summary>
        public DateTime FechaJornada
        {
            get { return fechaJornada; }
            set { fechaJornada = value; }
        }

        #endregion

        #region Constructor

        public Jornada()
        {
            this.cliente = new Cliente();            
            this.FechaJornada = DateTime.Now;            
        }

        #endregion

        #region Acceso Datos

        /// <summary>
        /// Generar la lista de visitas para el reporte TODO
        /// </summary>
        /// <returns>lista de visitas</returns>
        public static List<Jornada> ReporteJornadas(bool Todas) 
        {
            SQLiteDataReader reader = null;
            List<Jornada> jornadas = new List<Jornada>();
            jornadas.Clear();
            try
            {
                string sentencia =
                    " SELECT Dia,Rubro1,Rubro2,Rubro3,Rubro4,Rubro5 FROM " + Table.ERPADMIN_JORNADA +
                    " WHERE julianday(date(Dia)) = julianday(date('now','localtime'))" +
                    " AND HandHeld=@HH" +
                    " ORDER BY Dia ASC";

                string sentenciaB =
                   " SELECT Dia,Rubro1,Rubro2,Rubro3,Rubro4,Rubro5 FROM " + Table.ERPADMIN_JORNADA +                   
                   " WHERE HandHeld=@HH" +
                   " ORDER BY Dia ASC";
                
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@HH",Ruta.NombreDispositivo()) });

                if(!Todas)
                    reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                else
                    reader = GestorDatos.EjecutarConsulta(sentenciaB, parametros);

                while (reader.Read())
                {
                    Jornada jornada = new Jornada();

                    jornada.FechaJornada = reader.GetDateTime(0);
                    jornada.Rubro1 = reader.GetString(1);
                    jornada.Rubro2 = reader.GetString(2);
                    jornada.Rubro3 = reader.GetString(3);
                    jornada.Rubro4 = reader.GetString(4);
                    jornada.Rubro5 = reader.GetString(5);
                    

                    jornadas.Add(jornada);
                }
                reader.Close();
                reader = null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando datos del reporte. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return jornadas;
        }
        /// <summary>
        /// Guardar una visita y los rastros de los documentos generados
        /// </summary>
        public void Guardar()
        {
            try
            {
             
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_JORNADA  +
                       " ( HandHeld, Dia, Rubro1, Rubro2, Rubro3, Rubro4, Rubro5)" +
                " VALUES (@HH,@DIA,@RUB1,@RUB2,@RUB3,@RUB4,@RUB5)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                GestorDatos.SQLiteParameter("@HH",SqlDbType.NVarChar, Ruta.NombreDispositivo()),
                GestorDatos.SQLiteParameter("@DIA",SqlDbType.DateTime, FechaJornada),
                GestorDatos.SQLiteParameter("@RUB1",SqlDbType.NVarChar, Rubro1),
                GestorDatos.SQLiteParameter("@RUB2",SqlDbType.NVarChar, Rubro2),
                GestorDatos.SQLiteParameter("@RUB3",SqlDbType.NVarChar, Rubro3),
                GestorDatos.SQLiteParameter("@RUB4",SqlDbType.NVarChar, Rubro4),
                GestorDatos.SQLiteParameter("@RUB5",SqlDbType.NVarChar,Rubro5 )});

            GestorDatos.EjecutarComando(sentencia, parametros);

              
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando  rubros de jornada del cliente." + ex.Message);
            }
        }

        /// <summary>
        /// Validar si ya un cliente se le aisgnaron los rubros de jornada
        /// </summary>
        /// <param name="cliente">cliente a validar</param>
        /// <returns>el cliente fue visitado previamente</returns>
        public static bool ValidarJornada()
        {
            try
            {
                string HandHeld = Ruta.NombreDispositivo();
                string sentencia =
                    " SELECT COUNT(DIA) FROM " + Table.ERPADMIN_JORNADA +
                    " WHERE HandHeld = @HH" +
                    " AND julianday(date(Dia)) = julianday(date('now','localtime'))";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@HH", HandHeld)});

                int registros = Convert.ToInt32(GestorDatos.EjecutarScalar(sentencia, parametros));

                if (registros > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validando visita del cliente. " + ex.Message);
            }
        }
        /// <summary>
        /// Pendientes de carga segun el estado de procesamiento de las jornadas
        /// </summary>
        /// <returns>Pendientes de carga</returns>
        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_JORNADA + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }

        #endregion

        #region IPrintable Members

        public string GetObjectName()
        {
            return "JORNADA";
        }

        public object GetField(string name)
        {
            switch (name)
            {
                case "CODIGO_CLIENTE": return cliente.Codigo;
                case "NOMBRE_CLIENTE": return cliente.Nombre;
                case "FECHA_HORA": return fechaJornada.ToString();
                case "RUBRO1": return Rubro1;
                case "RUBRO2": return Rubro2;
                case "RUBRO3": return Rubro3;
                case "RUBRO4": return Rubro4;
                case "RUBRO5": return Rubro5;
                default: return null;
            }
        }

        #endregion
    }
}
