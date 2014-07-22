using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    public class Ruta
    {
        #region Variables y Propiedades de instancia

        private string codigo = string.Empty;
        /// <summary>
        /// Codigo de ruta (zona)
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public string Descripcion 
        {
            get { return codigo; }
        }

        private string handHeld = string.Empty;
        /// <summary>
        /// Codigo de handheld
        /// </summary>
        public string HandHeld
        {
            get { return handHeld; }
            set { handHeld = value; }
        }

        private string bodega = string.Empty;
        /// <summary>
        /// Bodega que se utiliza en Ruteo para gestion de pedidos y facturas.
        /// </summary>
        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }

        private string grupoArticulo = string.Empty;
        /// <summary>
        /// Grupo de articulos que maneja la ruta.
        /// </summary>
        public string GrupoArticulo
        {
            get { return grupoArticulo; }
            set { grupoArticulo = value; }
        }
        #endregion

        static string nombreDispositivo = null;
        /// <summary>
        /// Obtener el nombre del dispositivo, el valor se cachea para solo obtenerlo una vez en la vida de la aplicación.
        /// </summary>
        /// <returns></returns>
        public static string NombreDispositivo()
        {
            if (GestorDatos.cnx.isOpen)
            {
                SQLiteDataReader reader = null;

                try
                {
                    string sentencia = "SELECT Handheld FROM " + Table.ERPADMIN_CONFIG_HH;

                    reader = GestorDatos.EjecutarConsulta(sentencia);

                    if (reader.Read()) nombreDispositivo = reader.GetString(0); //se lee el primero row
                }
                catch (Exception ex)
                {
                    throw new Exception("Error cargando nombre del dispositivo." + ex.Message);
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
            if (!nombreDispositivo.Equals(FRmConfig.NombreHandHeld))
            {
                nombreDispositivo = FRmConfig.NombreHandHeld;
                return FRmConfig.NombreHandHeld;
            }
            else
            {
                return nombreDispositivo;
            }           
            
        }
        /// <summary>
        /// Obtener la lista de rutas de la pocket
        /// </summary>
        /// <returns></returns>
        public static List<Ruta> ObtenerRutas()
        {
            List<Ruta> rutas = new List<Ruta>();
            rutas.Clear();

            string sentencia =
                @" SELECT RUTA,GRUPO_ART,BODEGA " +
                @" FROM " + Table.ERPADMIN_RUTA_CFG  +
                @" WHERE HANDHELD = @DISPOSITIVO ";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            string nomb = NombreDispositivo();
            //string nomb = "PPC1";
            parametros.Add("@DISPOSITIVO", nomb);
                       
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Ruta ruta = new Ruta();
                    ruta.HandHeld = nomb;
                    ruta.Codigo = reader.GetString(0);
                    ruta.GrupoArticulo = reader.GetString(1);
                    ruta.Bodega = reader.GetString(2);

                    rutas.Add(ruta);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando rutas. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return rutas;
        }
        
        /// <summary>
        /// Obtener bodega asociada al dispositivo
        /// </summary>
        /// <returns></returns>
        public static string ObtenerBodega()
        {
            string sentencia =
                @" SELECT DISTINCT BODEGA " +
                @" FROM " + Table.ERPADMIN_RUTA_CFG +
                @" WHERE HANDHELD = @DISPOSITIVO ";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@DISPOSITIVO", NombreDispositivo());
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    if (reader.GetString(0) != FRdConfig.NoDefinido)
                    {
                        return reader.GetString(0);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error cargando la bodega. " + e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return FRdConfig.NoDefinido;
        }
        
        /// <summary>
        /// Validar que se cargaron rutas para el dispositivo
        /// </summary>
        /// <param name="codigoHandHeld"></param>
        /// <returns></returns>
        public static bool ValidaCargaCorrecta(string codigoHandHeld)
        {
            string sentencia=
                    @" SELECT COUNT(*) " +
                    @" FROM " + Table.ERPADMIN_RUTA_CFG +
                    @" WHERE HANDHELD = @DISPOSITIVO";
            
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@DISPOSITIVO", NombreDispositivo());

            int registros = Convert.ToInt32(GestorDatos.EjecutarScalar(sentencia,parametros));

            if (registros == 0)
                return false;
            else
                return true;
        }
        
        /// <summary>
        /// Obtener dias de visita para un cliente en una ruta especificada
        /// </summary>
        /// <param name="cliente">codigo del cliente</param>
        /// <param name="ruta">codigo de la ruta</param>
        /// <returns>lista de dias de visita</returns>
        public static List<Dias> DiasVisita(string cliente, string ruta)
        {
            List<Dias> dias = new List<Dias>();
            dias.Clear();

            string sentencia =
                @" SELECT DIA FROM " + Table.ERPADMIN_alFAC_RUTA_ORDEN  +
                @" WHERE COD_CLT = @CLIENTE " +
                @" AND COD_ZON = @RUTA ";


            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
                     parametros.Add("@CLIENTE", cliente);
                     parametros.Add("@RUTA", ruta);
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    dias.Add((Dias)Convert.ToChar(reader.GetString(0)));
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return dias;
        }
        
        /// <summary>
        /// Obtiene la fecha fin de los descuentos.
        /// </summary>
        /// <returns></returns>
        public static DateTime ObtenerFechaFin()
        {
            string sentencia =
                "SELECT MIN(FECHA_FIN) FROM " + Table.ERPADMIN_RUTA_CFG;

            object valor = GestorDatos.EjecutarScalar(sentencia);

            if (valor != DBNull.Value)
                return Convert.ToDateTime(valor);

            return DateTime.Now.AddDays(6);
        }

        /// <summary>
        /// Caso 34430 LJR 16/01/2009
        /// Obtener el objeto ruta segun el codigo
        /// </summary>
        /// <param name="zona">codigo de la ruta a buscar</param>
        /// <returns></returns>
        public static Ruta ObtenerRuta(List<Ruta> rutas, string zona)
        {
            foreach (Ruta ruta in rutas)
            {
                if (ruta.Codigo.Equals(zona))
                    return ruta;
            }
            return null;
        }

        public override string ToString()
        {
            return this.Codigo;
        }
    }

}
