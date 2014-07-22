using System;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    /// <summary>
    /// Administracion de los precios de articulos
    /// </summary>
    public struct Precio : ICloneable
    {
        #region Variables y Propiedades

        private decimal maximo;
        /// <summary>
        /// Precio para el articulo en unidades de almacen.
        /// </summary>
        public decimal Maximo
        {
            get { return Math.Round(maximo,2); }
            set { maximo = Math.Round(value,2); }
        }

        private decimal minimo;
        /// <summary>
        /// Precio para el articulo en unidades de detalle.
        /// </summary>
        public decimal Minimo
        {
            get { return Math.Round(minimo,2); }
            set { minimo = Math.Round(value,2); }
        }

        private decimal porcentajeRecargo;
        /// <summary>
        /// Factor de precio
        /// Corresponde al porcentaje de recargo definido para el artículo, si no posee se aplica el 
        /// porcentaje de recargo definido en las globales. Se aplica el porcentaje de recargo a la 
        /// cantidad de detalle pedida o a facturar solo en caso en que la variable 'AplicaRecargo' de 
        /// la clase 'VariablesGlobales' sea TRUE y el atributo del artículo 'unidadEmpaque' sea mayor a 1.
        /// </summary>
        public decimal PorcentajeRecargo
        {
            get { return porcentajeRecargo; }
            set
            {
                if (value > decimal.Zero)
                    porcentajeRecargo = value;
                else
                    porcentajeRecargo = FRdConfig.PorcentajeRecargo;
            }
        }

        private decimal margenUtilidad;
        /// <summary>
        /// Margen de utilidad de un articulo
        /// </summary>
        public decimal MargenUtilidad
        {
            get { return margenUtilidad; }
            set { margenUtilidad = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del precio
        /// </summary>
        /// <param name="max">indicador de precio maximo</param>
        /// <param name="min">indicador de precio minimo</param>
        public Precio(decimal max, decimal min)
        {
            maximo = max;
            minimo = min;
            margenUtilidad = Decimal.MinValue;
            porcentajeRecargo = Decimal.MinValue;
        }

        public Precio(decimal max, decimal min, decimal margen, decimal porcentajeRec)
        {
            maximo = max;
            minimo = min;
            margenUtilidad = margen;
            porcentajeRecargo = porcentajeRec;
        }

        public object Clone()
        {
            Precio clon = new Precio(this.maximo, this.minimo);
            clon.margenUtilidad = this.margenUtilidad;
            clon.porcentajeRecargo = this.porcentajeRecargo;
            return clon;
        }

        #endregion

        #region Logica
        /// <summary>
        /// Calcular los precios
        /// </summary>
        /// <param name="unidadEmpaque">factor de empaque para el articulo asociado</param>
        public void CalcularPrecio(decimal unidadEmpaque)
        {
            if (maximo == decimal.Zero)
            {
                maximo = minimo = decimal.Zero;
            }
            else
            {
                if ((FRdConfig.AplicaRecargo) && (unidadEmpaque > 1))
                {
                    decimal MontoRecargo = (maximo / unidadEmpaque) * (porcentajeRecargo / 100);
                    minimo = (maximo / unidadEmpaque) + MontoRecargo;
                }
                else
                {
                    minimo = maximo / unidadEmpaque;
                }
            }
        }

        /// <summary>
        /// Cargar datos del precio del articulo en un nivel de precio determinado indicando su factor de empaque
        /// </summary>
        /// <param name="articulo">articulo para cargar el precio</param>
        /// <param name="nivel">nivel asociado al articulo</param>
        /// <param name="unidadEmpaque">factor de empaque del articulo</param>
        public void CargarDatosPrecio(string articulo, NivelPrecio nivel, decimal unidadEmpaque)
        {
            CargarPrecio(articulo, nivel);
            CalcularPrecio(unidadEmpaque);
        }

        /// <summary>
        /// Cargar datos del precio del articulo segun el historico de una factura para devoluciones
        /// </summary>
        /// <param name="articulo">codigo del articulo a cargar el precio</param>
        /// <param name="compania">compania asociada al articulo</param>
        /// <param name="factura">factura de donde se toman los precios</param>
        /// <param name="unidadEmpaque">factor de empaque del articulo</param>
        public void CargarDatosPrecioHistorico(string articulo, string compania, string factura, decimal unidadEmpaque)
        {
            CargarPrecioHistorico(articulo, compania, factura);
            CalcularPrecio(unidadEmpaque);
        }

        #endregion

        #region Acceso Datos
        /// <summary>
        /// Cargar precio de un articulo
        /// </summary>
        /// <param name="articulo">codigo del articulo</param>
        /// <param name="nivel">nivel de precio asociado</param>
        private void CargarPrecio(string articulo, NivelPrecio nivel)
        {
            if (maximo != decimal.Zero)
                return;

            string sentencia =
                " SELECT PRECIO FROM " + Table.ERPADMIN_ARTICULO_PRECIO +
                " WHERE ARTICULO = @ARTICULO" +
                " AND UPPER(COMPANIA) = @COMPANIA" +
                " AND NIVEL_PRECIO = @NIVEL" +
                " AND MONEDA = @MONEDA";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@ARTICULO", articulo);
            parametros.Add("@COMPANIA", nivel.Compania.ToUpper());
            parametros.Add("@NIVEL", nivel.Nivel);
            parametros.Add("@MONEDA", ((char)nivel.Moneda).ToString());

            try
            {
                object precio = GestorDatos.EjecutarScalar(sentencia, parametros);
                if (precio != null)
                {
                    maximo = Convert.ToDecimal(precio.ToString());

                    if (nivel.Impuesto1Incluido == Impuesto1Incluido.SI)
                    {
                        Articulo art = new Articulo(articulo, nivel.Compania);
                        art.Cargar(new FiltroArticulo[] { FiltroArticulo.NivelPrecio }, nivel.Nivel);
                        art.CargarImpuesto();

                        maximo = maximo / ((art.Impuesto.Impuesto1 / 100) + 1);
                    }
                }
            }
            catch (Exception)
            {
                if (Documentos.FRPedido.Pedidos.FacturarPedido)
                    throw new Exception("No se encontró precio del Articulo: '" + articulo + "'.");
            }

        }

        /// <summary>
        /// Carga los precios basados en la factura historica para devolucion.
        /// </summary>
        /// <param name="articulo">articulo a buscar</param>
        /// <param name="compania">compania asociada a la factura</param>
        /// <param name="factura">consecutivo de factura</param>
        private void CargarPrecioHistorico(string articulo, string compania, string factura)
        {
            if (maximo != decimal.Zero)
                return;

            //Caso 28087 LDS 04/05/2007
            string sentencia =
                " SELECT DISTINCT PRE_UNI FROM " + Table.ERPADMIN_alFAC_DET_HIST_FAC +
                " WHERE COD_ART = @ARTICULO" +
                " AND   UPPER(COD_CIA) = @COMPANIA" +
                " AND   NUM_FAC = @CONSECUTIVO";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@ARTICULO", articulo);
            parametros.Add("@COMPANIA", compania.ToUpper());
            parametros.Add("@CONSECUTIVO", factura);

            try
            {
                object precio = GestorDatos.EjecutarScalar(sentencia, parametros);
                if (precio != DBNull.Value)
                    maximo = Convert.ToDecimal(precio.ToString());
            }
            catch (Exception)
            {
                throw new Exception("No se encontró precio del Articulo: '" + articulo + "'.");
            }
        }

        /// <summary>
        /// cargar el margen de utilidad de un articulo
        /// </summary>
        /// <param name="articulo">codigo del articulo</param>
        /// <param name="nivel">nivel de precio asociado</param>
        public void CargarMargenUtilidad(string articulo, NivelPrecio nivel)
        {
            if (margenUtilidad != decimal.MinValue)
                return;

            string sentencia =
                " SELECT MARGEN_UTILIDAD FROM " + Table.ERPADMIN_ARTICULO_PRECIO +
                " WHERE ARTICULO = @ARTICULO" +
                " AND UPPER(COMPANIA) = @COMPANIA" +
                " AND NIVEL_PRECIO = @NIVEL" +
                " AND MONEDA = @MONEDA";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@ARTICULO", articulo);
            parametros.Add("@COMPANIA", nivel.Compania.ToUpper());
            parametros.Add("@NIVEL", nivel.Nivel);
            parametros.Add("@MONEDA", ((char)nivel.Moneda).ToString());
            try
            {
                object margen = GestorDatos.EjecutarScalar(sentencia, parametros);

                if (margen != DBNull.Value)
                    margenUtilidad = Convert.ToDecimal(margen.ToString());
                else
                    margenUtilidad = 0;
            }
            catch (Exception)
            {
                throw new Exception("Error al cargar margen de utilidad del Articulo: '" + articulo + "'.");
            }
        }
        #endregion
    }
}
