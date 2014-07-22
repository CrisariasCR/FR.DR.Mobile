using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using System.Globalization;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    public class Descuento
    {

        #region Variables de clase Config.xml

        /// <summary>
        /// Indica si el rutero puede utilizar el sistema con información
        /// de descuentos vencida.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool PermitirInfoDescuentosVencida;

        #endregion

        #region Variables y Propiedades de instancia

        private DateTime fechaInicio;
        /// <summary>
        /// Indica la fecha en que inicia el descuento
        /// </summary>
        public DateTime FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; }
        }

        private DateTime fechaFin;
        /// <summary>
        /// Indica la fecha en que termina el descuento
        /// </summary>		
        public DateTime FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; }
        }

        private TipoDescuento tipo;
        /// <summary>
        /// Tipo de descuento.
        /// </summary>
        public TipoDescuento Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private decimal monto;
        /// <summary>
        /// Monto del descuento. 
        /// El tipo indica si este monto es un porcentaje o un monto fijo. 
        /// Si es un porcentaje, la variable contiene un valor 
        /// entero (0 - 100) y no un porcentaje (0 - 1).
        /// </summary>
        public decimal Monto
        {
            get { return monto; }
            set { monto = value; }
        }

        private decimal udsMinimas;
        /// <summary>
        /// Unidades minimas de facturacion para un porcentaje 
        /// de descuento escalonado
        /// </summary>
        public decimal UdsMinimas
        {
            get { return udsMinimas; }
            set { udsMinimas = value; }
        }

        private decimal udsMaximas;
        /// <summary>
        /// Unidades maximas de facturacion para un porcentaje 
        /// de descuento escalonado
        /// </summary>
        public decimal UdsMaximas
        {
            get { return udsMaximas; }
            set { udsMaximas = value; }
        }
			
        #endregion

        /// <summary>
        /// Obtiene los descuentos por cliente en determinada compania y clasificacion del articulo 
        /// </summary>
        /// <param name="cliente">cliente asociado</param>
        /// <param name="articulo">articulo a asociar</param>
        /// <param name="compania">compania asociada al cliente</param>
        /// <param name="familia">Primera clasificacion del articulo</param>
        /// <param name="clase">segunda clasificacion del articulo</param>
        /// <returns>listas de descuentos</returns>
        public static List<Descuento> ObtenerDescuentosClasificados(string cliente, Articulo articulo, decimal rangoUnidadesAlmacen)
        {
            SQLiteDataReader reader = null;
            List<Descuento> descuentos = new List<Descuento>();
            descuentos.Clear();

            string sentencia =
                "SELECT FECHA_INICIO,FECHA_FIN,CANT_MIN,CANT_MAX,MONTO,TIPO,CLASIFICACION2 " +
                @"FROM " + Table.ERPADMIN_DESCUENTO_CLAS +
                @" WHERE UPPER(CLIENTE) = @CLIENTE" +
                @" AND UPPER(COMPANIA) = @COMPANIA" +
                @" AND CLASIFICACION1 = @FAMILIA" +
                @" AND FECHA_INICIO <= date('now','localtime') " +
                @" AND FECHA_FIN    >= date('now','localtime') ";

            if (rangoUnidadesAlmacen != decimal.MinValue)
                sentencia += " AND @CANTIDAD BETWEEN CANT_MIN AND CANT_MAX ";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@CLIENTE", cliente.ToUpper());
            parametros.Add("@COMPANIA", articulo.Compania.ToUpper());
            parametros.Add("@FAMILIA", articulo.Familia);
            parametros.Add("@CANTIDAD", rangoUnidadesAlmacen);

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                string clasif2;

                while (reader.Read())
                {
                    if (!reader.IsDBNull(6))
                        clasif2 = reader.GetString(6);
                    else
                        clasif2 = string.Empty;

                    if (clasif2 == string.Empty || clasif2 == articulo.Familia)
                    {
                        //Solamente si la clasificacion2 del descuento es la misma del articulo
                        //agregamos al descuento.

                        Descuento desc = new Descuento();

                        desc.FechaInicio = reader.GetDateTime(0);
                        desc.FechaFin = reader.GetDateTime(1);
                        desc.UdsMinimas = reader.GetDecimal(2);
                        desc.UdsMaximas = reader.GetDecimal(3);
                        desc.Monto = reader.GetDecimal(4);
                        desc.Tipo = (TipoDescuento)Convert.ToChar(reader.GetString(5));

                        descuentos.Add(desc);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return descuentos;
        }
        
        /// <summary>
        /// Obtener el descuento de un articulo para un cliente, en un rango especificado
        /// </summary>
        /// <param name="cliente">cliente a asociar al descuento</param>
        /// <param name="articulo">articulo a buscar el descuento</param>
        /// <param name="rangoUnidadesAlmacen">rango a buscar del descuento</param>
        /// <returns>descuento</returns>
        public static Descuento ObtenerDescuento(ClienteCia cliente, Articulo articulo, decimal rangoUnidadesAlmacen)
        { 
            List<Descuento> descuentos =ObtenerDescuentos(cliente,  articulo,  rangoUnidadesAlmacen);
            if (descuentos.Count == 0)
                return null;
            else
                return descuentos[0];
        }
        
        /// <summary>
        /// Obtener los descuentos 
        /// 1) Segun el esquema de nivel de precios
        /// 2) Segun el esquema de descuento por articulo por cliente
        /// </summary>
        /// <param name="cliente">cliente a asociar al descuento</param>
        /// <param name="articulo">articulo a buscar el descuento</param>
        /// <param name="rangoUnidadesAlmacen">rango a buscar del descuento</param>
        /// <returns>descuento</returns>
        public static List<Descuento> ObtenerDescuentos(ClienteCia cliente, Articulo articulo, decimal rangoUnidadesAlmacen)
        {
            SQLiteDataReader reader = null;
            List<Descuento> descuentos = new List<Descuento>();
            descuentos.Clear();
            string select = string.Empty;
            string sentencia = string.Empty;
            string select2 = string.Empty;
            string sentencia2 = string.Empty;
            string mensajeDescuento = string.Empty;
            SQLiteParameterList parametros = new SQLiteParameterList();

            List<Descuento> descListaPrec = new List<Descuento>();
            List<Descuento> descExcepCliente = new List<Descuento>();

            if (rangoUnidadesAlmacen == decimal.MinValue)
            {
               // parametros = new SqlCeParameter[4];
            }
            else
            {
               // parametros = new SqlCeParameter[5];
                rangoUnidadesAlmacen = Math.Round(rangoUnidadesAlmacen, 2);
                parametros.Add("@CANTIDAD", rangoUnidadesAlmacen.ToString(CultureInfo.InvariantCulture));
                
            }

            parametros.Add("@ARTICULO", articulo.Codigo);
            parametros.Add("@COMPANIA", cliente.Compania.ToUpper());
           


            sentencia = sentencia2 =
                    " WHERE D.ARTICULO = @ARTICULO" +
                    @" AND UPPER(D.COMPANIA) = @COMPANIA" +
                    @" AND D.FECHA_INICIO <= date('now','localtime') " +
                    @" AND D.FECHA_FIN    >= date('now','localtime') ";

            select = " SELECT PORC_DESC,'P', D.FECHA_INICIO, D.FECHA_FIN, D.CANT_MIN, D.CANT_MAX,'L' " +
                         @" FROM " + Table.ERPADMIN_DESCUENTO_NIVEL + " D , " + Table.ERPADMIN_NIVEL_LISTA + " L";
            
            parametros.Add("@NIVEL", cliente.NivelPrecio.Codigo);
            sentencia +=
                " AND L.LISTA = @NIVEL " +
                @" AND D.NIVEL_PRECIO = L.NIVEL_PRECIO " +
                @" AND D.MONEDA = L.MONEDA " +
                @" AND UPPER(D.COMPANIA) = L.COMPANIA ";




            select2 = "SELECT MONTO,TIPO,FECHA_INICIO,FECHA_FIN,CANT_MIN, CANT_MAX,'C' " +  //L para lista y C para cliente
                    @" FROM " + Table.ERPADMIN_DESCUENTO_CLIART + " D ";
            
            parametros.Add("@CLIENTE", cliente.Codigo.ToUpper());
            sentencia2 += " AND UPPER(CLIENTE) = @CLIENTE ";



            if (rangoUnidadesAlmacen != decimal.MinValue)
                sentencia += " AND @CANTIDAD BETWEEN CANT_MIN AND CANT_MAX ";

            // Cambios Descuentos Bonificaciones - KFC
            sentencia = select + sentencia + " UNION " + select2 + sentencia2;

            try
            {
                mensajeDescuento = "Error cargando el descuento para el artículo '" + articulo + "' en la compañía '" + cliente.Compania + "' para el cliente '" + cliente.Codigo + "'. ";

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Descuento desc = new Descuento();
                    desc.Monto = reader.GetDecimal(0);
                    desc.Tipo = (TipoDescuento)Convert.ToChar(reader.GetString(1));
                    desc.FechaInicio = reader.GetDateTime(2);
                    desc.FechaFin = reader.GetDateTime(3);
                    desc.UdsMinimas = reader.GetDecimal(4);
                    desc.UdsMaximas = reader.GetDecimal(5);                    

                    if (reader.GetString(6) == "L")
                        descListaPrec.Add(desc);

                    if ((reader.GetString(6) == "C") && (rangoUnidadesAlmacen >= desc.UdsMinimas) && (rangoUnidadesAlmacen <= desc.UdsMaximas))
                        descExcepCliente.Add(desc);

                    // Precedencia de Descuentos / Cambios Descuentos Bonificaciones - KFC 
                    if (descExcepCliente.Count > 0)
                        descuentos = descExcepCliente;
                    else
                        descuentos = descListaPrec;
                }

                if (descuentos.Count == 0)
                {
                    mensajeDescuento = "Error cargando el descuento por clasificación para el artículo '" + articulo + "' en la compañía '" + cliente.Compania + "' para el cliente '" + cliente + "'. ";
                    descuentos = Descuento.ObtenerDescuentosClasificados(cliente.Codigo, articulo, rangoUnidadesAlmacen);
                }

            }
            catch (Exception ex)
            {
                /* KFC
               if (FRdConfig.EsquemaDescuento == EsquemaDescuento.NivelesPrecio)**/
                if ((descExcepCliente.Count == 0) && (descListaPrec.Count > 0))
                    throw new Exception("Error cargando el descuento escalonado para el artículo '" + articulo + "' en la compañía '" + cliente.Compania + "' en el nivel de precios '" + cliente.NivelPrecio + "'. " + ex.Message);
                else
                    throw new Exception(mensajeDescuento + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return descuentos;
        }
    }
}
