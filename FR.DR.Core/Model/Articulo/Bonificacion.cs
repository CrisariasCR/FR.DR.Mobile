using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using System.Globalization;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    /// <summary>
    /// Clase que representa una bonificacion
    /// </summary>
    public class Bonificacion
    {

        #region Variables y Propiedades de instancia

        private decimal baseMinima = 0;
        /// <summary>
        /// Indica la base minima para bonificar
        /// </summary>
        public decimal BaseMinima
        {
            get { return baseMinima; }
            set { baseMinima = value; }
        }

        private decimal baseMaxima = 0;
        /// <summary>
        /// Indica la base maxima para bonificar
        /// </summary>
        public decimal BaseMaxima
        {
            get { return baseMaxima; }
            set { baseMaxima = value; }
        }

        private decimal factor = 0;
        /// <summary>
        /// Indica el factor de bonificacion
        /// </summary>
        public decimal Factor
        {
            get { return factor; }
            set { factor = value; }
        }

        private decimal cantidad = 0;
        /// <summary>
        /// Indica la cantidad bonificada
        /// </summary>
        public decimal Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        private DateTime fechaInicio = DateTime.Now;
        /// <summary>
        /// Indica la fecha en que inicia la bonificación
        /// </summary>
        public DateTime FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; }
        }

        private DateTime fechaFin = DateTime.Now;
        /// <summary>
        /// Indica la fecha en que termina la bonificación
        /// </summary>		
        public DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }

        private Articulo articuloBonificado;
        /// <summary>
        /// Articulo a bonificar
        /// </summary>
        public Articulo ArticuloBonificado
        {
            get { return articuloBonificado; }
            set { articuloBonificado = value; }
        }

        #endregion

        /// <summary>
        /// Obtener bonificacion por cliente y por articulo
        /// </summary>
        /// <param name="cliente">cliente asociado para obtener bonificacion</param>
        /// <param name="articulo">codigo de articulo a obtener su bonificion</param>
        /// <param name="rangoUnidadesAlmacen">Rango de cantidad para ver si existe bonificacion, Enviar un decimal.MinValue si no es por rango</param>
        /// <returns></returns>
        public static List<Bonificacion> ObtenerBonificaciones(ClienteCia cliente, string articulo, decimal rangoUnidadesAlmacen)
        {
            SQLiteDataReader reader = null;
            List<Bonificacion> bonificaciones = new List<Bonificacion>();
            bonificaciones.Clear();
            string select, select2 = string.Empty;
            string sentencia = string.Empty, sentencia2 = string.Empty;
            string from = string.Empty, from2 = string.Empty;                
            SQLiteParameterList parametros = new SQLiteParameterList();

            List<Bonificacion> bonifListaPrec = new List<Bonificacion>();
            List<Bonificacion> bonifExcepCliente = new List<Bonificacion>();

            if (rangoUnidadesAlmacen == decimal.MinValue)
            {
                parametros.Add("@CANTIDAD", 0.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                parametros.Add("@CANTIDAD", rangoUnidadesAlmacen.ToString(CultureInfo.InvariantCulture));
            }
            parametros.Add("@ARTICULO", articulo);
            parametros.Add("@COMPANIA", cliente.Compania.ToUpper());
            
            select = 
                " SELECT B.FECHA_INICIO, B.FECHA_FIN, B.CANT_MIN, B.CANT_MAX," +
                @" B.FACTOR_BONIF, B.UNIDADES_BONIF, B.ARTICULO_BONIF ";

            sentencia = sentencia2 =
                    " WHERE B.ARTICULO = @ARTICULO" +
                    @" AND UPPER(B.COMPANIA) = @COMPANIA" +
                    @" AND B.FECHA_INICIO <= date('now','localtime') " +
                    @" AND B.FECHA_FIN    >= date('now','localtime') ";

            from += "FROM " + Table.ERPADMIN_BONIFICACION_NIVEL + " B , " + Table.ERPADMIN_NIVEL_LISTA + " L";
            parametros.Add("@NIVEL", cliente.NivelPrecio.Codigo);            
            sentencia +=
                 " AND L.LISTA = @NIVEL " +
                @" AND B.NIVEL_PRECIO = L.NIVEL_PRECIO " +
                @" AND B.MONEDA = L.MONEDA " +
                @" AND B.COMPANIA = L.COMPANIA ";


            from2 += "FROM " + Table.ERPADMIN_BONIFICACION_CLIART + " B ";
            parametros.Add("@CLIENTE", cliente.Codigo);            
            sentencia2 += " AND CLIENTE = @CLIENTE ";


            if (rangoUnidadesAlmacen != decimal.MinValue)
            {
                sentencia += " AND @CANTIDAD BETWEEN CANT_MIN AND CANT_MAX ";
                sentencia2 += " AND @CANTIDAD BETWEEN CANT_MIN AND CANT_MAX ";
            }

            // L = "Lista Precios" / C = "Excepcion"
            sentencia = select + ",'L' " + from + sentencia + " UNION " + select + ",'C' " + from2 + sentencia2;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                while (reader.Read())
                {
                    Bonificacion bon = new Bonificacion();

                    bon.FechaInicio = reader.GetDateTime(0);
                    bon.FechaFin = reader.GetDateTime(1);
                    bon.BaseMinima = reader.GetDecimal(2);
                    bon.BaseMaxima = reader.GetDecimal(3);

                    if (!reader.IsDBNull(4))
                        bon.Factor = reader.GetDecimal(4);

                    bon.Cantidad = reader.GetDecimal(5);
                    try
                    {
                        bon.ArticuloBonificado = new Articulo(reader.GetString(6),cliente.Compania);
                        cliente.CargarNivelPrecio();
                        bon.articuloBonificado.Cargar(new FiltroArticulo[]{ FiltroArticulo.NivelPrecio},cliente.NivelPrecio.Nivel);
                        bon.articuloBonificado.CargarPrecio(cliente.NivelPrecio);                    
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error cargando información de artículo bonificado. " + ex.Message);
                    }

                    if (reader.GetString(7) == "L")
                        bonifListaPrec.Add(bon);

                    if ((reader.GetString(7) == "C") && (rangoUnidadesAlmacen >= bon.BaseMinima) && (rangoUnidadesAlmacen <= bon.BaseMaxima))
                        bonifExcepCliente.Add(bon);

                    // Precedencia entre Bonificaciones por excepcion o por lista de precios
                    if (bonifExcepCliente.Count > 0)
                        bonificaciones = bonifExcepCliente;
                    else
                        bonificaciones = bonifListaPrec;
                }
            }
            catch (Exception ex)
            {
                //if (FRdConfig.EsquemaDescuento == EsquemaDescuento.NivelesPrecio)
                if ((bonifExcepCliente.Count == 0) && (bonifListaPrec.Count > 0))
                    throw new Exception("Error cargando bonificaciones para el artículo '" + articulo + "' en la compañía '" + cliente.Compania + "' y lista de precios '" + cliente.NivelPrecio.Nivel + "'. " + ex.Message);
                else
                    throw new Exception("Error cargando bonificaciones para el artículo '" + articulo + "' en la compañía '" + cliente.Compania + "' y cliente '" + cliente.Codigo + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return bonificaciones;
        }

    }
}
