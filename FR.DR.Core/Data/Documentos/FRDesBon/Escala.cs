using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class Escala
    {
        #region Attributes
        private String compania;
        private int escala;
        private String regla;
        private String articulo;
        private decimal factorEmpaque = 1;

        private decimal cantidadMinima;
        private decimal cantidadMaxima;

        private decimal cantidadMinimaDetalle;
        private decimal cantidadMaximaDetalle;

        private decimal factor;
        private decimal factorDet;
        private decimal cantidadInicial;
        private decimal cantidadInicialDet;
        private decimal cantidadInicialConsol;

        private decimal cantidadPedida;
        private decimal unidadAlmacen;
        private decimal unidadDetalle;
        private decimal factorEmpaqueArtBonificado = 1;

        #endregion

        #region Properties
        public String Compania
        {
            get { return compania; }
            set { compania = value; }
        }
        public int Escala1
        {
            get { return escala; }
            set { escala = value; }
        }
        public String Regla
        {
            get { return regla; }
            set { regla = value; }
        }
        public String Articulo
        {
            get { return articulo; }
            set { articulo = value; }
        }

        public decimal FactorEmpaqueArticulo
        {
            get { return factorEmpaque; }
            set
            {
                if (value == 0)
                {
                    factorEmpaque = 1;
                }
                else
                {
                    factorEmpaque = value;
                }
            }
        }

        public decimal CantidadMinima
        {
            get { return cantidadMinima; }
            set { cantidadMinima = value; }
        }
        public decimal CantidadMaxima
        {
            get { return cantidadMaxima; }
            set { cantidadMaxima = value; }
        }
        public decimal Factor
        {
            get { return factor; }
            set { factor = value; }
        }
        public decimal FactorDet
        {
            get { return factorDet; }
            set { factorDet = value; }
        }
        public decimal CantidadInicial
        {
            get { return cantidadInicial; }
            set { cantidadInicial = value; }
        }
        public decimal CantidadInicialDet
        {
            get { return cantidadInicialDet; }
            set { cantidadInicialDet = value; }
        }
        public decimal CantidadInicialConsolidada
        {
            get { return cantidadInicialConsol; }
            set { cantidadInicialConsol = value; }
        }
        public decimal CantidadPedida
        {
            get { return cantidadPedida; }
            set { cantidadPedida = value; }
        }
        public decimal UnidadAlmacen
        {
            get { return unidadAlmacen; }
            set { unidadAlmacen = value; }
        }
        public decimal UnidadDetalle
        {
            get { return unidadDetalle; }
            set { unidadDetalle = value; }
        }
        public decimal CantidadMinimaDetalle
        {
            get { return cantidadMinimaDetalle; }
            set { cantidadMinimaDetalle = value; }
        }
        public decimal CantidadMaximaDetalle
        {
            get { return cantidadMaximaDetalle; }
            set { cantidadMaximaDetalle = value; }
        }
        public decimal FactorEmpaqueArtBonificado
        {
            get { return factorEmpaqueArtBonificado; }
            set
            {
                if (value == 0)
                {
                    factorEmpaqueArtBonificado = 1;
                }
                else
                {
                    factorEmpaqueArtBonificado = value;
                }
            }
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
        public static List<Escala> FindAll(String compania, String regla)
        {
            List<Escala> resultado = new List<Escala>();
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT dbesb.COMPANIA, dbesb.ESCALA, dbesb.REGLA, dbesb.ARTICULO, dbesb.CANTIDAD_MINIMA, dbesb.CANTIDAD_MAXIMA, dbesb.CANTIDAD_INICIAL, dbesb.FACTOR, dbesb.UNIDAD_ALMACEN, dbesb.CANTIDAD_PEDIDA, dbesb.UNIDAD_DETALLE,  ");
            sentencia.AppendLine(" dbesb.CANT_MAXIMA_DET, dbesb.CANT_MINIMA_DET, dbesb.FACTOR_DET, dbesb.CANTIDAD_INICIAL_DET, dbesb.CANTIDAD_INICIAL_CONSOL, art.UND_EMP ");
            sentencia.AppendLine(String.Format(" FROM {0} dbesb LEFT OUTER JOIN {1} art", Table.ERPADMIN_DES_BON_ESCALA_BONIFICACION, Table.ERPADMIN_ARTICULO));
            sentencia.AppendLine(" ON dbesb.ARTICULO = art.COD_ART AND dbesb.COMPANIA = art.COD_CIA ");
            sentencia.AppendLine(" WHERE dbesb.COMPANIA = @COMPANIA AND dbesb.REGLA = @REGLA");
            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()), new SQLiteParameter("@REGLA", regla) });

            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                while (reader.Read())
                {
                    Escala escala = new Escala();

                    escala.Compania = reader.GetString(0);
                    escala.Escala1 = reader.GetInt32(1);
                    escala.regla = reader.GetString(2);
                    escala.Articulo = reader.IsDBNull(3) ? null : reader.GetString(3);
                    escala.CantidadMinima = reader.GetDecimal(4);
                    escala.CantidadMaxima = reader.GetDecimal(5);
                    escala.CantidadInicial = reader.GetDecimal(6);
                    escala.Factor = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7);
                    escala.UnidadAlmacen = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8);
                    escala.CantidadPedida = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9);
                    escala.UnidadDetalle = reader.IsDBNull(10) ? 0 : reader.GetDecimal(10);
                    escala.cantidadMaximaDetalle = reader.IsDBNull(11) ? 0 : reader.GetDecimal(11);
                    escala.cantidadMinimaDetalle = reader.IsDBNull(12) ? 0 : reader.GetDecimal(12);
                    escala.FactorDet = reader.IsDBNull(13) ? 0 : reader.GetDecimal(13);
                    escala.CantidadInicialDet = reader.IsDBNull(14) ? 0 : reader.GetDecimal(14);
                    escala.CantidadInicialConsolidada = reader.IsDBNull(15) ? 0 : reader.GetDecimal(15);
                    escala.FactorEmpaqueArtBonificado = reader.IsDBNull(16) ? 0 : reader.GetDecimal(16);

                    resultado.Add(escala);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error cargando los paquetes de descuentos-bonificación para la compañía '{0}'. {1}  '", compania, ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el Factor convertido en unidadades de almacén según el factor de empaque del artículo que se está procesando
        /// </summary>
        /// <param name="UsaFactorEmpArtEscala">Indica si se debe usar el factor de empaque asignado a la propiedad FactorEmpaqueArticulo o el factor de empaque de artículo en la escala</param>
        /// <returns>Factor convertido en unidadades de almacén</returns>
        public decimal ObtenerFactorAlmacen(bool UsaFactorEmpArtEscala)
        {
            decimal resultado = 0;

            if (UsaFactorEmpArtEscala)
            {
                resultado = this.factor + (factorDet / this.factorEmpaqueArtBonificado);
            }
            else
            {
                resultado = this.factor + (factorDet / this.factorEmpaque);
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la cantidad mínima convertido en unidadades de almacén según el factor de empaque del artículo que se está procesando
        /// </summary>
        /// <param name="UsaFactorEmpArtEscala">Indica si se debe usar el factor de empaque asignado a la propiedad FactorEmpaqueArticulo o el factor de empaque de artículo en la escala</param>
        /// <returns>Factor convertido en unidadades de almacén</returns>
        public decimal ObtenerCantMinimaAlmacen(bool UsaFactorEmpArtEscala)
        {
            decimal resultado = 0;

            if (UsaFactorEmpArtEscala)
            {
                resultado = this.cantidadMinima + (this.cantidadMinimaDetalle / this.factorEmpaqueArtBonificado);
            }
            else
            {
                resultado = this.cantidadMinima + (this.cantidadMinimaDetalle / this.factorEmpaque);
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la cantidad máxima convertido en unidadades de almacén según el factor de empaque del artículo que se está procesando
        /// </summary>
        /// <param name="UsaFactorEmpArtEscala">Indica si se debe usar el factor de empaque asignado a la propiedad FactorEmpaqueArticulo o el factor de empaque de artículo en la escala</param>
        /// <returns>Factor convertido en unidadades de almacén</returns>
        public decimal ObtenerCantMaximaAlmacen(bool UsaFactorEmpArtEscala)
        {
            decimal resultado = 0;

            if (UsaFactorEmpArtEscala)
            {
                resultado = this.cantidadMaxima + (this.cantidadMaximaDetalle / this.factorEmpaqueArtBonificado);
            }
            else
            {
                resultado = this.cantidadMaxima + (this.cantidadMaximaDetalle / this.factorEmpaque);
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la Cantidad Inicial Consolidada. Para el caso del uso del mismo artículo, esa cantidad se calcula con el factor de empaque del artículo bonificado
        /// </summary>
        /// <param name="UsaMismoArticulo">Indica si se debe o no calcular la cantidad inicial, o si se obtiene de la escala directamente</param>
        /// <returns>Retorna la cantidad inicial consolidada</returns>
        public decimal ObtenerCantidadInicialConsolidada(bool UsaMismoArticulo)
        {
            decimal resultado = 0;

            if (UsaMismoArticulo)
            {
                resultado = this.cantidadInicial + (this.cantidadInicialDet / this.factorEmpaque);
            }
            else
            {
                resultado = this.cantidadInicialConsol;
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la Cantidad A Facturar Consolidada. Para el caso del uso del mismo artículo, esa cantidad se calcula con el factor de empaque del artículo bonificado
        /// </summary>
        /// <param name="UsaMismoArticulo">Indica si se debe o no calcular la cantidad inicial, o si se obtiene de la escala directamente</param>
        /// <returns>Retorna la Cantidad A Facturar consolidada</returns>
        public decimal ObtenerCantidadRegalia(bool UsaMismoArticulo)
        {
            decimal resultado = 0;

            if (UsaMismoArticulo)
            {
                resultado = this.unidadAlmacen + (this.unidadDetalle / this.factorEmpaque);
            }
            else
            {
                resultado = this.cantidadPedida;
            }

            return resultado;
        }

        #endregion
    }
}
