using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita
{
    /// <summary>
    /// Clase que representa una visita al cliente
    /// </summary>
    public class Visita: IPrintable
    {
        #region Variables y Propiedades de instancia

        private DateTime fechaInicio = DateTime.Now;
        /// <summary>
        /// Indica la fecha en que inicia la visita
        /// </summary>
        public DateTime FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; }
        }

        private DateTime fechaFin = DateTime.Now;
        /// <summary>
        /// Indica la fecha en que termina la visita
        /// </summary>		
        public DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }

        private RazonVisita razon;
        /// <summary>
        /// Razon de la visita.
        /// </summary>
        public RazonVisita Razon
        {
            get { return razon; }
            set { razon = value; }
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

        private DateTime fechaProximaVisita= DateTime.Now.AddDays(6);
        /// <summary>
        /// Fecha de proxima visita.
        /// </summary>
        public DateTime FechaProximaVisita
        {
            get { return fechaProximaVisita; }
            set { fechaProximaVisita = value; }
        }

        private string nota = string.Empty;
        /// <summary>
        /// Observaciones de la visita.
        /// </summary>
        public string Nota
        {
            get { return nota; }
            set { nota = value; }
        }

        private TipoVisita tipo = TipoVisita.Telefonica;
        /// <summary>
        /// Tipo de la visita.
        /// </summary>
        public TipoVisita Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        #endregion

        #region Constructor

        public Visita()
        {
            this.cliente = new Cliente();
            this.FechaInicio = DateTime.Now;
            this.FechaFin = DateTime.Now;
            this.fechaProximaVisita = DateTime.Now.AddDays(6);
            this.Tipo = TipoVisita.Telefonica;
            this.Nota = string.Empty;
            this.Razon = new RazonVisita();
        }

        #endregion

        #region Acceso Datos

        /// <summary>
        /// Generar la lista de visitas para el reporte
        /// </summary>
        /// <returns>lista de visitas</returns>
        public static List<Visita> ReporteVisitas() 
        {
            SQLiteDataReader reader = null;
            List<Visita> visitas = new List<Visita>();
            visitas.Clear();
            try
            {
                string sentencia =
                    " SELECT RAZON,CLIENTE,INICIO FROM " + Table.ERPADMIN_VISITA  +
                    " WHERE julianday(date(INICIO)) = julianday(date('now','localtime'))" +
                    " ORDER BY INICIO ASC";

                SQLiteParameterList parametros = new SQLiteParameterList ( new SQLiteParameter[] { new SQLiteParameter("@HOY", DateTime.Now.ToString("yyyyMMdd")) });
                
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Visita visita = new Visita();

                    visita.razon.Codigo = reader.GetString(0);
                    visita.cliente.Codigo = reader.GetString(1);
                    visita.fechaInicio = reader.GetDateTime(2);

                    visitas.Add(visita);
                }
                reader.Close();
                reader = null;
                foreach (Visita visita in visitas)
                {
                    sentencia =
                        " SELECT R.DES_RZN, C.NOM_CLT " +
                        " FROM " + Table.ERPADMIN_alFAC_RZN_VIS + " R, " + Table.ERPADMIN_CLIENTE + " C " +
                        " WHERE R.COD_RZN =  @RAZON" +
                        " AND C.COD_CLT= @CLIENTE";

                    SQLiteParameterList parametros2 = new SQLiteParameterList(new SQLiteParameter[] { 
                                   new SQLiteParameter("@RAZON", visita.Razon.Codigo),
                                   new SQLiteParameter("@CLIENTE",visita.Cliente.Codigo)});
                
                    reader = GestorDatos.EjecutarConsulta(sentencia,parametros2);

                    while (reader.Read())
                    {
                        visita.razon.Descripcion = reader.GetString(0);
                        visita.cliente.Nombre = reader.GetString(1);
                    }
                    reader.Close();
                    reader = null;
                }
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

            return visitas;
        }
        /// <summary>
        /// Guardar una visita y los rastros de los documentos generados
        /// </summary>
        public void Guardar()
        {
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_VISITA  +
                       " ( CLIENTE, RUTA, INICIO, FIN, RAZON, FECHA_PLAN, TIPO, NOTAS )"+
                " VALUES (@COD_CLT,@COD_ZON,@INICIO,@FIN,@RAZON, date('now','localtime'),@TIPO,@NOTAS)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, cliente.Codigo),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, cliente.Zona),
                GestorDatos.SQLiteParameter("@INICIO",SqlDbType.DateTime, fechaInicio),
                GestorDatos.SQLiteParameter("@FIN",SqlDbType.DateTime, fechaFin),
                GestorDatos.SQLiteParameter("@RAZON",SqlDbType.NVarChar, razon.Codigo),
                GestorDatos.SQLiteParameter("@TIPO",SqlDbType.NVarChar, ((char)tipo).ToString()),
                GestorDatos.SQLiteParameter("@NOTAS",SqlDbType.NText, nota)});

            GestorDatos.EjecutarComando(sentencia, parametros);

            try
            {
                foreach (DocumentoVisita doc in DocumentoVisita.ObtenerDocumentosSinRegistrar(cliente.Codigo))
                {
                    doc.Guardar(cliente.Codigo, cliente.Zona, fechaInicio);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando documentos del cliente." + ex.Message);
            }
        }
        /// <summary>
        /// Validar si ya un cliente se le realizo una visita previa
        /// </summary>
        /// <param name="cliente">cliente a validar</param>
        /// <returns>el cliente fue visitado previamente</returns>
        public bool ValidarVisita(string cliente)
        {
            try
            {
                string sentencia =
                    " SELECT COUNT(INICIO) FROM " + Table.ERPADMIN_VISITA +
                    " WHERE CLIENTE = @CLIENTE" +
                    " AND julianday(date(INICIO)) = julianday(date('now','localtime'))";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente)});

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
        /// Pendientes de carga segun el estado de procesamiento de los documentos de visita
        /// </summary>
        /// <returns>Pendientes de carga</returns>
        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_VISITA + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }

        #endregion

        #region IPrintable Members

        public string GetObjectName()
        {
            return "VISITA";
        }

        public object GetField(string name)
        {
            switch (name)
            {
                case "CODIGO_CLIENTE": return cliente.Codigo;
                case "NOMBRE_CLIENTE": return cliente.Nombre;
                case "DESCRIPCION": return razon.Descripcion;
                case "HORA": return fechaInicio.ToString();
                default: return null;
            }
        }

        #endregion
    }
}
