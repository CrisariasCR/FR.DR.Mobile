using System;
using System.Data;
using System.Data.SQLiteBase;
using System.Globalization;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using System.Collections.Generic;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    /// <summary>
    /// Representa la bodega con existencia para un artículo. 
    /// </summary>
    public class Bodega : ICloneable
    {

        #region Variables y Propiedades

        private string codigo = string.Empty;        
        /// <summary>
        /// Código de la Bodega.
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private decimal existencia = decimal.Zero;
        /// <summary>
        /// Existencia de un determinado artículo en la bodega
        /// </summary>
        public decimal Existencia
        {
            get { return existencia; }
            //set { existencia = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// constructor de la bodeda
        /// </summary>
        /// <param name="bodega">codigo de la bodega a asociar</param>
        public Bodega(string bodega)
        {
            codigo = bodega;
            existencia = decimal.MinValue;     
        }

        /// <summary>
        /// constructor de la bodega
        /// </summary>
        /// <param name="articulo">Articulo de la bodega</param>
        /// <param name="bodega">Codigo de la Bodega</param>
        public Bodega(Articulo articulo, string bodega)
        {
            CargarBodegaArticulo(articulo, bodega);
        }

        /// <summary>
        /// Carga las existencias de un articulo que se encuentra en una bodega
        /// </summary>
        /// <param name="articulo">Articulo de la Bodega</param>
        /// <param name="bodega">Codigo de la Bodega</param>
        private void CargarBodegaArticulo(Articulo articulo, string bodega)
        {
            string sentencia =
                @" SELECT BODEGA, EXISTENCIA FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA  +
                @" WHERE ARTICULO = @ARTICULO" +
                @" AND UPPER(COMPANIA) = @COMPANIA" +
                @" AND BODEGA = @BODEGA";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@ARTICULO",articulo.Codigo),
                new SQLiteParameter("@COMPANIA", articulo.Compania.ToUpper()),
                new SQLiteParameter("@BODEGA", bodega)});

            SQLiteDataReader reader = null;           

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    this.codigo = reader.GetString(0);
                    this.existencia = reader.GetDecimal(1);
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
                this.codigo = "";
                this.existencia = decimal.MinValue;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public object Clone()
        {
            Bodega clon = new Bodega(codigo);
            clon.existencia = this.existencia;
            return clon;
        }
        /// <summary>
        /// constructor de la bodeda
        /// </summary>
        public Bodega()
        {
            existencia = decimal.MinValue;
        }
        #endregion

        #region Logica

        /// <summary>
        /// Verificar si hay suficiente existencia
        /// </summary>
        /// <param name="cantidadSolicitada">cantidad a solicitar</param>
        /// <returns>si hay existencia</returns>
        public bool SuficienteExistencia(decimal cantidadSolicitada)
        {
            return ( existencia >= cantidadSolicitada);               
        }

        /// <summary>
        /// Sentencia que actualiza la existencia de los articulos en el servidor
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="articulo"></param>
        /// <returns></returns>
        public string SentenciaActualizacion(string compania, string articulo)
        {
            return
                 " UPDATE %CIA.ARTICULO_EXISTENCIA " +
                @" SET EXISTENCIA = " + existencia.ToString(CultureInfo.InvariantCulture) + 
                @" WHERE ARTICULO = '" + articulo + "' " +
                @" AND COMPANIA = '" + compania + "' " +
                @" AND BODEGA   = '" + codigo + "'";
        }
        /// <summary>
        /// Cargar la existencia del articulo
        /// </summary>
        /// <param name="compania">compania asociada al articulo</param>
        /// <param name="articulo">codigo del articulo</param>
        public void CargarExistencia(string compania, string articulo)
        {
            ObtenerExistencia(compania, articulo);
        }

        /// <summary>
        /// Cargar la existencia del articulo, cuando utiliza lotes
        /// </summary>
        /// <param name="compania">compania asociada al articulo</param>
        /// <param name="articulo">codigo del articulo</param>
        public void CargarExistenciaLote(string compania, string articulo)
        {
            ObtenerExistenciaLotes(compania, articulo);
        }

        #endregion

        #region Acceso Datos

        /// <summary>
        /// Obtiene las existencias del artículo en bodega para determinada compañía.
        /// </summary>
        /// <param name="compania">compania a obtener existencia</param>
        /// <param name="articulo">codigo del articulo a consultar</param>
        public void ObtenerExistencia(string compania, string articulo)
        {
            string sentencia =
                @" SELECT EXISTENCIA FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA  +
                @" WHERE ARTICULO = @ARTICULO" +
                @" AND UPPER(COMPANIA) = @COMPANIA" +
                @" AND BODEGA = @BODEGA";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@ARTICULO",articulo),
                new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                new SQLiteParameter("@BODEGA", codigo)});

            object valor = GestorDatos.EjecutarScalar(sentencia, parametros);

            if (valor != DBNull.Value)
                existencia = Convert.ToDecimal(valor.ToString());
            else
                throw new Exception("No se encontró existencia en bodega para el artículo");
        }

        /// <summary>
        /// Obtiene las existencias del artículo en bodega para determinada compañía cuando el articulo utiliza lotes
        /// </summary>
        /// <param name="compania">compania a obtener existencia</param>
        /// <param name="articulo">codigo del articulo a consultar</param>
        public void ObtenerExistenciaLotes(string compania, string articulo)
        {
            string sentencia =
                @" SELECT SUM(EXISTENCIA) AS EXISTENCIA FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA_LOTE +
                @" WHERE ARTICULO = @ARTICULO" +
                @" AND UPPER(COMPANIA) = @COMPANIA" +
                @" AND BODEGA = @BODEGA";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@ARTICULO",articulo),
                new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                new SQLiteParameter("@BODEGA", codigo)});

            object valor = GestorDatos.EjecutarScalar(sentencia, parametros);

            if (valor != DBNull.Value)
                existencia = Convert.ToDecimal(valor.ToString());
            else
                throw new Exception("No se encontró existencia en bodega para el artículo");
        }

        #region MejorasFRTostadoraElDorado600R6 JEV
        public void ObtenerExistenciaLotes(string compania, string articulo, string lote)
        {
            string sentencia =
                @" SELECT SUM(EXISTENCIA) AS EXISTENCIA FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA_LOTE +
                @" WHERE ARTICULO = @ARTICULO" +
                @" AND UPPER(COMPANIA) = @COMPANIA" +
                @" AND BODEGA = @BODEGA" +
                @" AND LOTE = @LOTE";

            try
            {

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                                            new SQLiteParameter("@ARTICULO",articulo),
                                            new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                                            new SQLiteParameter("@BODEGA", codigo),
                                            new SQLiteParameter("@LOTE", lote),
                                          });

                object valor = GestorDatos.EjecutarScalar(sentencia, parametros);

                if (valor != DBNull.Value)
                    existencia = Convert.ToDecimal(valor.ToString());
                else
                    throw new Exception("No se encontró existencia en bodega para el artículo");
            }
            catch (Exception ex)
            {
                throw new Exception("No se encontró existencia en bodega para el artículo" + ex.Message);
            }
        }

        public bool ObtenerLocalizacionLote(string compania, string articulo, string lote, out string localizacion)
        {
            bool procesoExitoso = true;
            string sentencia = string.Empty;
            string loc = string.Empty;
                
            try
            {

                //Se crea la consulta para obtener la localizacion asociada a un lote
                sentencia = 
                        @" SELECT LOCALIZACION FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA_LOTE +
                        @" WHERE ARTICULO = @ARTICULO" +
                        @" AND UPPER(COMPANIA) = @COMPANIA" +
                        @" AND BODEGA = @BODEGA" +
                        @" AND LOTE = @LOTE";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                                            new SQLiteParameter("@ARTICULO",articulo),
                                            new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                                            new SQLiteParameter("@BODEGA", codigo),
                                            new SQLiteParameter("@LOTE", lote),
                                          });

                object valor = GestorDatos.EjecutarScalar(sentencia, parametros);

                if (valor != DBNull.Value)
                    loc = valor.ToString();
                else
                {
                    //throw new Exception(String.Format("No se encontró la localización asociada al lote: '{0}'.", lote));
                    procesoExitoso = false;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception(String.Format("No se encontró la localización asociada al lote: '{0}'. {1}", lote, ex.Message));
                procesoExitoso = false;
            }

            //Se asigna el valor de retorno 
            localizacion = loc;

            return procesoExitoso;
        }
        #endregion MejorasFRTostadoraElDorado600R6 JEV

        /// <summary>
        /// Aumenta o disminuye sobre la cantidad disponible actualmente
        /// </summary>
        /// <param name="compania">compania a la cual pertenece el articulo a actualizar</param>
        /// <param name="articulo">articulo a actualizar</param>
        /// <param name="cantidad">cantidad a actualizar</param>
        public void ActualizarExistencia(string compania, string articulo, decimal cantidad)
		{
            int registros=0;
			string sentencia =
                @" UPDATE " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                @" SET EXISTENCIA = EXISTENCIA + @CANTIDAD " +
                @" WHERE ARTICULO = @ARTICULO" +
                @" AND UPPER(COMPANIA) = @COMPANIA" +
                @" AND BODEGA = @BODEGA";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@ARTICULO",SqlDbType.NVarChar,articulo),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,compania.ToUpper()),
                GestorDatos.SQLiteParameter("@BODEGA",SqlDbType.NVarChar,codigo),
                GestorDatos.SQLiteParameter("@CANTIDAD",SqlDbType.Decimal,cantidad)});
            try
            {
                registros = GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando existencias al artículo '" + articulo + "'. " + ex.Message);
            }	
			if (registros > 1)
				throw new Exception("Más de un registro afectado en Bodega.");
			else if (registros < 0)
				throw new Exception("Ningún registro afectado en Bodega.");			
		}

        /// <summary>
        /// Cargar el inventario para una compania en su bodega
        /// </summary>
        /// <param name="bodega">codigo de bodega</param>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="incluirArticulosSinExistencia">bandera para determinar si incluye articulos solo con existencias</param>
        /// <returns>detalles de inventario</returns>
        public static DetallesInventario CargarInventarioTomaFisica(string bodega, string compania)
        {
            return CargarInventario(bodega, compania, true, true);
        }

        /// <summary>
        /// Cargar el inventario para una compania en su bodega
        /// </summary>
        /// <param name="bodega">codigo de bodega</param>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="incluirArticulosSinExistencia">bandera para determinar si incluye articulos solo con existencias</param>
        /// <returns>detalles de inventario</returns>
        public static DetallesInventario CargarInventario(string bodega, string compania, bool incluirArticulosSinExistencia)
        {
            return CargarInventario(bodega, compania, incluirArticulosSinExistencia, false);
        }

        /// <summary>
        /// Cargar el inventario para una compania en su bodega
        /// </summary>
        /// <param name="bodega">codigo de bodega</param>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="incluirArticulosSinExistencia">bandera para determinar si incluye articulos solo con existencias</param>
        /// <param name="tomaFisica">Optiene los valores de la toma fisica</param>
        /// <returns>detalles de inventario</returns>
        public static DetallesInventario CargarInventario(string bodega, string compania, bool incluirArticulosSinExistencia, bool tomaFisica)
        {
            DetallesInventario detalles = new DetallesInventario();
            detalles.Lista.Clear();

            DetalleInventario detalle;
            decimal existenciaArticulo;
            
            SQLiteParameterList parametros = new SQLiteParameterList(); 
            string sentencia = string.Empty;

            if (!tomaFisica)
            {
                sentencia =
                   " SELECT A.COD_CIA, A.COD_ART, A.DES_ART, A.UND_EMP, E.EXISTENCIA, A.COD_BAR, A.COD_FAM, A.COD_CLS " +
                   " FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA + " E, " + Table.ERPADMIN_ARTICULO + " A " +
                   " WHERE E.BODEGA = @BODEGA " +
                   " AND   UPPER(A.COD_CIA) = UPPER(E.COMPANIA) " +
                   " AND   A.COD_ART = E.ARTICULO ";

                if (!compania.Equals(string.Empty))
                {
                    sentencia += " AND UPPER(E.COMPANIA) = @COMPANIA";
                    parametros.Add("@COMPANIA", SqlDbType.NVarChar, compania.ToUpper());
                }                
            }
            else
            {
                sentencia =
                   " SELECT A.COD_CIA, A.COD_ART, A.DES_ART, A.UND_EMP, E.EXISTENCIA, A.COD_BAR, A.COD_FAM, A.COD_CLS,TF.CANT_FACT, TF.CANT_DIFERENCIA, TF.ESTADO " +
                   " FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA + " E, " + Table.ERPADMIN_ARTICULO + " A, " + Table.ERPADMIN_TOMA_FISICA_INV + " TF " +
                   " WHERE E.BODEGA = @BODEGA " +
                   " AND UPPER(E.COMPANIA) = @COMPANIA" +                   
                   " AND   UPPER(A.COD_CIA) = UPPER(E.COMPANIA) " +
                   " AND   A.COD_ART = E.ARTICULO "+
                   " AND TF.ARTICULO = E.ARTICULO AND TF.COMPANIA = A.COD_CIA ";

                parametros.Add("@COMPANIA", SqlDbType.NVarChar, compania.ToUpper());
            }

            if (!incluirArticulosSinExistencia)
                sentencia += " AND E.EXISTENCIA > 0 ";

            sentencia+=  " ORDER BY A.COD_CIA, A.COD_ART";

            SQLiteDataReader reader = null;
            parametros.Add("@BODEGA",SqlDbType.NVarChar,bodega);

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    detalle = new DetalleInventario();
                    detalle.Articulo.Compania = reader.GetString(0);
                    detalle.Articulo.Codigo = reader.GetString(1);
                    detalle.Articulo.Descripcion = reader.GetString(2);
                    detalle.Articulo.UnidadEmpaque = reader.GetDecimal(3);

                    existenciaArticulo = reader.GetDecimal(4);
                    detalle.UnidadesAlmacen = decimal.Truncate(existenciaArticulo);
                    detalle.UnidadesDetalle = (existenciaArticulo - detalle.UnidadesAlmacen) * detalle.Articulo.UnidadEmpaque;

                    if (! reader.IsDBNull(5))
                        detalle.Articulo.CodigoBarras = reader.GetString(5);
                    if (!reader.IsDBNull(6))
                        detalle.Articulo.Familia = reader.GetString(6).ToString();
                    if (!reader.IsDBNull(7))
                        detalle.Articulo.Clase = reader.GetString(7).ToString();

                    if (tomaFisica)
                    {
                        if (!reader.IsDBNull(8))
                            detalle.CantidadFacturadaTomaFisica = Convert.ToDecimal(reader.GetValue(8));
                        if (!reader.IsDBNull(9))
                            detalle.DiferenciasTomaFisica = Convert.ToDecimal(reader.GetValue(9));
                        if (!reader.IsDBNull(10))
                            detalle.Estado = reader.GetString(10).ToString();
                    }
                    else
                    {
                        detalle.CantidadFacturadaTomaFisica = decimal.Zero;
                        detalle.DiferenciasTomaFisica = decimal.Zero;
                        detalle.Estado = string.Empty;
                    }


                    detalles.Lista.Add(detalle);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error cargando el inventario de la bodega '" + bodega + "'. " + e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return detalles;
        }        
 
        #endregion
    }
}
