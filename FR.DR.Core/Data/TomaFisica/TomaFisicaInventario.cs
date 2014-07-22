using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Data.SQLiteBase;
using FR.Core.Model;
using Android.App;
using Cirrious.MvvmCross.ViewModels;


namespace Softland.ERP.FR.Mobile.Cls
{
    public class TomaFisicaInventario : MvxNotifyPropertyChanged
    {
        public TomaFisicaInventario()
        {
        }

        public TomaFisicaInventario(string pCompania, string pHandHeld, string pBodega, string pLocalizacion, DateTime pFecha)
        {
            Compania = pCompania;
            HandHeld = pHandHeld;
            Fecha = pFecha;
            BodegaCamion = pBodega;
            Localizacion = pLocalizacion;
        }

        #region Constantes
        /// <summary>
        /// Boleta Pendiente de aplicar
        /// </summary>
        public const string BOLETA_PENDIENTE = "P";
        /// <summary>
        /// Boleta aplicada al inventario
        /// </summary>
        public const string BOLETA_APLICADA = "A";
        /// <summary>
        /// Boleta Facturada
        /// </summary>
        public const string BOLETA_FACTURADA = "F";
        #endregion Constantes

        #region Definición de Propiedades
        string compania = string.Empty;
        string handHeld = string.Empty;
        DateTime fecha = DateTime.Now;
        string articulo = string.Empty;
        string descArticulo = string.Empty;
        string bodega = string.Empty;
        string localizacion = string.Empty;
        string lote = string.Empty;
        decimal cantidad = decimal.Zero;
        decimal cantFacturar = decimal.Zero;
        decimal cantNoFacturar = decimal.Zero;
        decimal cantDiferencia = decimal.Zero;
        bool selected;
        int consecutivo = 0;

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        public string Compania
        {
            get { return compania; }
            set { compania = value; }
        }
        public string HandHeld
        {
            get { return handHeld; }
            set { handHeld = value; }
        }
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
        public string Articulo
        {
            get { return articulo; }
            set { articulo = value; }
        }
        public string DescArticulo
        {
            get { return descArticulo; }
            set { descArticulo = value; }
        }
        public string BodegaCamion
        {
            get { return bodega; }
            set { bodega = value; }
        }
        public string Localizacion
        {
            get { return localizacion; }
            set { localizacion = value; }
        }
        public string Lote
        {
            get { return lote; }
            set { lote = value; }
        }
        public decimal Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
        public decimal CantidadFacturar
        {
            get { return cantFacturar; }
            set { cantFacturar = value; RaisePropertyChanged("CantidadFacturar"); }
        }
        public decimal CantidadNoFacturar
        {
            get { return cantNoFacturar; }
            set { cantNoFacturar = value; RaisePropertyChanged("CantidadNoFacturar"); }
        }
        public decimal CantidadDiferencia
        {
            get { return cantDiferencia; }
            set { cantDiferencia = value; }
        }
        public int Consecutivo
        {
            get { return consecutivo; }
            set { consecutivo = value; }
        }

        #endregion Definición de Propiedades

        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(0) FROM " + Table.ERPADMIN_TOMA_FISICA_INV ;
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }

        /// <summary>
        /// Se encarga de insertar la boleta
        /// de inventario físico en la base de datos
        /// </summary>
        /// <returns></returns>
        public bool RegistrarNuevaBoleta()
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string insertBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se obtiene el siguiente consecutivo
                procesoExitoso = ObtenerSigteConsecBoleta();

                if (procesoExitoso)
                {
                    //Se crea la sentencia para insertar los datos
                    insertBoleta = String.Format(" INSERT INTO {0} ( ", Table.ERPADMIN_TOMA_FISICA_INV);
                    insertBoleta = insertBoleta + " CONSECUTIVO, COMPANIA, HANDHELD, FECHA, ARTICULO, BODEGA, LOCALIZACION, LOTE, CANTIDAD, CANT_FACT, CANT_DIFERENCIA, ESTADO )";
                    insertBoleta = insertBoleta + " VALUES ( @CONSECUTIVO, @COMPANIA, @HANDHELD, @FECHA, @ARTICULO, @BODEGA, @LOCALIZACION, @LOTE, @CANTIDAD, @CANTFACT, @DIFERENCIA, @ESTADO )";

                    //Se crean los parámetros
                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@CONSECUTIVO", SqlDbType.Int, Consecutivo),
                                                    GestorDatos.SQLiteParameter("@COMPANIA", SqlDbType.NVarChar, Compania),
                                                    GestorDatos.SQLiteParameter("@HANDHELD", SqlDbType.NVarChar, HandHeld),
                                                    GestorDatos.SQLiteParameter("@FECHA", SqlDbType.DateTime, Fecha.ToShortDateString()), 
                                                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, Articulo),
                                                    GestorDatos.SQLiteParameter("@BODEGA", SqlDbType.NVarChar, BodegaCamion),
                                                    GestorDatos.SQLiteParameter("@LOCALIZACION", SqlDbType.NVarChar, Localizacion),
                                                    GestorDatos.SQLiteParameter("@LOTE", SqlDbType.NVarChar, Lote),
                                                    GestorDatos.SQLiteParameter("@CANTIDAD", SqlDbType.Decimal, Cantidad),
                                                    GestorDatos.SQLiteParameter("@CANTFACT", SqlDbType.Decimal, CantidadFacturar),
                                                    GestorDatos.SQLiteParameter("@DIFERENCIA", SqlDbType.Decimal, CantidadDiferencia),
                                                    GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar, BOLETA_PENDIENTE)
                                                  });

                    //Se ejecuta la sentencia
                    GestorDatos.EjecutarComando(insertBoleta, parametros);
                }
            }
            catch
            {                
                procesoExitoso = false;
            }


            return procesoExitoso;
        }

        public bool ActualizarBoleta()
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string insertBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se crea la sentencia para insertar los datos
                insertBoleta = String.Format(" UPDATE {0} SET ", Table.ERPADMIN_TOMA_FISICA_INV);
                insertBoleta = insertBoleta + " CANTIDAD = @CANTIDAD ";
                insertBoleta = insertBoleta + " WHERE CONSECUTIVO = @CONSECUTIVO ";

                //Se crean los parámetros
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                                                  GestorDatos.SQLiteParameter("@CANTIDAD", SqlDbType.Decimal, Cantidad),
                                                  GestorDatos.SQLiteParameter("@CONSECUTIVO", SqlDbType.Int, Consecutivo)
                                              });

                //Se ejecuta la sentencia
                GestorDatos.EjecutarComando(insertBoleta, parametros);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Problemas al tratar de registrar la nueva boleta de inventario físico." + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }
        
        public bool ActualizarEstadoBoleta(int consecBoleta, string nuevoEstado)
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string insertBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se crea la sentencia para insertar los datos
                insertBoleta = String.Format(" UPDATE {0} SET ", Table.ERPADMIN_TOMA_FISICA_INV);
                insertBoleta = insertBoleta + " ESTADO = @ESTADO ";
                insertBoleta = insertBoleta + " WHERE CONSECUTIVO = @CONSECUTIVO ";

                //Se crean los parámetros
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                                                  GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar, nuevoEstado),
                                                  GestorDatos.SQLiteParameter("@CONSECUTIVO", SqlDbType.Int, consecBoleta)
                                              });

                //Se ejecuta la sentencia
                GestorDatos.EjecutarComando(insertBoleta, parametros);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Problemas al tratar de registrar la nueva boleta de inventario físico." + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        public bool ActualizarCantBoleta()
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string actualizarBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se crea la sentencia para insertar los datos
                actualizarBoleta = String.Format(" UPDATE {0} SET ", Table.ERPADMIN_TOMA_FISICA_INV);
                actualizarBoleta = actualizarBoleta + " CANT_FACT = @CANTFACT, CANT_DIFERENCIA = @DIFERENCIA ";
                actualizarBoleta = actualizarBoleta + " WHERE CONSECUTIVO = @CONSECUTIVO ";

                //Se crean los parámetros
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                                                  GestorDatos.SQLiteParameter("@CANTFACT", SqlDbType.Decimal, CantidadFacturar),
                                                  GestorDatos.SQLiteParameter("@DIFERENCIA", SqlDbType.Decimal, CantidadDiferencia),
                                                  GestorDatos.SQLiteParameter("@CONSECUTIVO", SqlDbType.Int, Consecutivo)
                                              });

                //Se ejecuta la sentencia
                GestorDatos.EjecutarComando(actualizarBoleta, parametros);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
               // Mensaje.mostrarAlerta("Problemas al tratar de registrar la nueva boleta de inventario físico." + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        public bool EliminarBoleta()
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string insertBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se crea la sentencia para insertar los datos
                insertBoleta = String.Format(" DELETE FROM {0}", Table.ERPADMIN_TOMA_FISICA_INV);
                insertBoleta = insertBoleta + " WHERE CONSECUTIVO = @CONSECUTIVO ";

                //Se crean los parámetros
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                                                  GestorDatos.SQLiteParameter("@CONSECUTIVO", SqlDbType.Int, Consecutivo)
                                              });

                //Se ejecuta la sentencia
                GestorDatos.EjecutarComando(insertBoleta, parametros);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Problemas al tratar de registrar la nueva boleta de inventario físico." + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        public bool EliminarTomaFisica()
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string insertBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se crea la sentencia para insertar los datos
                insertBoleta = String.Format(" DELETE FROM {0}", Table.ERPADMIN_TOMA_FISICA_INV);
                insertBoleta = insertBoleta + " WHERE julianday(date(FECHA)) = julianday(date('now','localtime')) ";                

                //Se ejecuta la sentencia
                GestorDatos.EjecutarComando(insertBoleta);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Problemas al tratar de eliminar la toma física de inventarios." + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        public bool CargarBoleta()
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            SQLiteDataReader reader = null;
            #endregion Definición de Varaibles
            
            try
            {
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico
                
                consulta = " SELECT COMPANIA, HANDHELD, FECHA, ARTICULO, BODEGA, LOCALIZACION, LOTE, CANTIDAD, CONSECUTIVO " +
                           " FROM " + Table.ERPADMIN_TOMA_FISICA_INV +
                           " WHERE UPPER(COMPANIA) = '" + Compania.ToUpper() + "' " +
                           " AND HANDHELD = '" + HandHeld + "' " +
                           " AND julianday(date(FECHA)) = julianday(date('now','localtime')) " +
                           " AND ARTICULO = '" + Articulo + "' " +
                           " AND BODEGA = '" + BodegaCamion + "' " +
                           " AND LOCALIZACION = '" + Localizacion + "' " +              
                           " AND LOTE = '" + Lote + "' " + 
                           " AND ESTADO = '" + BOLETA_PENDIENTE + "'";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                if (reader.Read())
                {
                    Compania = reader.GetString(0);
                    HandHeld = reader.GetString(1);
                    Fecha = reader.GetDateTime(2);
                    Articulo = reader.GetString(3);
                    BodegaCamion = reader.GetString(4);
                    Localizacion = reader.GetString(5);
                    Lote = reader.GetString(6);
                    Cantidad = Convert.ToDecimal(reader.GetValue(7));
                    Consecutivo = Convert.ToInt16(reader.GetValue(8));
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;
        }

         
        ///<summary>
        ///Se encarga de cargar una lista de boletas
        ///que contienen una diferencia negativa
        ///</summary>
        ///<param name="lstArticulos">Retorna la lista de artículos con diferencias negativas</param>
        ///<returns></returns>
        public bool CargarArticulosBoleta(ref List<TomaFisicaInventario> lstArticulos)
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            string codArticulo = string.Empty;
            string descArticulo = string.Empty;
            string[] datos = new string[7];
            decimal cantidadBoleta = decimal.Zero;
            decimal diferencia = decimal.Zero;
            SQLiteDataReader reader = null;
            Bodega existenciaBodega = null;
            TomaFisicaInventario itemArticulo = null;
            #endregion Definición de Varaibles

            try
            {
                //Se inicializa la instancia de bodegas
                existenciaBodega = new Bodega(BodegaCamion);
                
                lstArticulos.Clear();

                //Se crea la consulta para obtener los datos de la boleta de inventario fisico
                consulta = " SELECT tm.ARTICULO, art.DES_ART, tm.CANT_DIFERENCIA, tm.LOTE " +
                           " FROM " + Table.ERPADMIN_TOMA_FISICA_INV + " tm" +
                           " INNER JOIN " + Table.ERPADMIN_ARTICULO + " art" +
                           " ON tm.ARTICULO = art.COD_ART" +
                           " WHERE UPPER(tm.COMPANIA) = '" + Compania.ToUpper() + "' " +
                           " AND tm.HANDHELD = '" + HandHeld + "' " +
                           " AND julianday(date(tm.FECHA)) = julianday(date('now','localtime')) " +
                           " AND tm.BODEGA = '" + BodegaCamion + "' " +
                           //" AND tm.LOCALIZACION = '" + Localizacion + "' " +
                           " AND tm.CANT_DIFERENCIA < 0 " + 
                           " AND tm.ESTADO = '" + BOLETA_PENDIENTE + "'";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                while (reader.Read())
                {
                    //Se obtiene el código del artículo
                    codArticulo = reader.GetString(0);
                    descArticulo = reader.GetString(1);
                    cantidadBoleta = string.IsNullOrEmpty(reader.GetValue(2).ToString()) ? 0 : Convert.ToDecimal(reader.GetValue(2));

                    //Se obtiene la existencia actual del artículo en el sistema
                    //existenciaBodega.ObtenerExistencia(Compania, codArticulo);

                    //Se valida si la existencia actual en el sistema es diferente a la indicada en la boleta de Inv Fisico
                    //if (existenciaBodega.Existencia != cantidadBoleta)
                    //{
                    //if (existenciaBodega.Existencia < cantidadBoleta)
                    //{
                    //Se obtiene la diferencia
                    //diferencia = existenciaBodega.Existencia - cantidadBoleta;

                    //Se toma el valor absoluto de la diferencia
                    diferencia = Math.Abs(cantidadBoleta);

                    //Se agrega al ListView los datos del artículo que se está procesando
                    itemArticulo = new TomaFisicaInventario();
                    itemArticulo.Articulo = codArticulo;
                    itemArticulo.descArticulo = descArticulo;
                    itemArticulo.Localizacion = Localizacion;
                    itemArticulo.Lote = string.IsNullOrEmpty(reader.GetValue(3).ToString()) ? "ND" : reader.GetString(3);
                    itemArticulo.cantFacturar = diferencia;
                    itemArticulo.cantDiferencia = diferencia;
                    itemArticulo.CantidadNoFacturar = 0;
                    
                    //datos[0] = codArticulo;
                    //datos[1] = descArticulo;
                    //datos[2] = diferencia.ToString();
                    //datos[3] = diferencia.ToString();
                    //datos[4] = "0";
                    //datos[5] = Localizacion;
                    //datos[6] = string.IsNullOrEmpty(reader.GetValue(3).ToString()) ? "ND" : reader.GetString(3);

                    //Se crea el nuevo item
                    //itemArticulo = new System.Windows.Forms.ListViewItem(datos);

                    //Se marca el item
                    itemArticulo.selected = true;

                    //Se agrega el item a la lista
                    lstArticulos.Add(itemArticulo);
                    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;
        }

        /// <summary>
        /// Se encarga de obtner los artículos que se
        /// deben facturar, carga solo las líneas donde 
        /// el campo CANT_FACT > 0
        /// Retorna un ListView en el siguiente orden:
        /// COMPANIA, ARTICULO, BODEGA, LOCALIZACION, LOTE Y CANTIDADFACTURAR
        /// </summary>
        /// <param name="lstArticulosFact"></param>
        /// <returns></returns>
        public bool CargarArticulosFacturar(ref List<TomaFisicaInventario> lstArticulosFact)
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            TomaFisicaInventario datos = null;
            SQLiteDataReader reader = null;            
            #endregion Definición de Varaibles

            try
            {
                //Se inicializa la instancia de bodegas
                lstArticulosFact.Clear();
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico
                consulta = " SELECT tm.ARTICULO, tm.CANT_FACT, tm.LOTE, tm.CONSECUTIVO " +
                           " FROM " + Table.ERPADMIN_TOMA_FISICA_INV + " tm" +
                           " WHERE tm.COMPANIA = '" + Compania + "' " +
                           " AND tm.HANDHELD = '" + HandHeld + "' " +
                           " AND julianday(date(tm.FECHA)) = julianday(date('now','localtime')) " +
                           " AND tm.BODEGA = '" + BodegaCamion + "' " +
                           " AND tm.LOCALIZACION = '" + Localizacion + "' " +
                           " AND CANT_FACT > 0 " +
                           " AND tm.ESTADO = '" + BOLETA_APLICADA + "' ";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                while (reader.Read())
                {
                    datos = new TomaFisicaInventario();
                    datos.Compania = Compania;
                    datos.Articulo = reader.GetString(0);
                    datos.BodegaCamion = BodegaCamion;
                    datos.Localizacion = Localizacion;
                    datos.Lote = string.IsNullOrEmpty(reader.GetValue(2).ToString()) ? "ND" : reader.GetString(2);
                    datos.CantidadFacturar = string.IsNullOrEmpty(reader.GetValue(1).ToString()) ? 0 : Convert.ToDecimal(reader.GetValue(1).ToString());
                    datos.Consecutivo = Convert.ToInt32(reader.GetValue(3).ToString());
                                       

                    //Se agrega el item a la lista
                    lstArticulosFact.Add(datos);
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;
        }


        protected bool CargarArticulosBoletaFisica(ref List<TomaFisicaInventario> lstArticulos)
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            TomaFisicaInventario datos = new TomaFisicaInventario();
            SQLiteDataReader reader = null;
            Bodega existenciaBodega = null;
            #endregion Definición de Varaibles

            try
            {
                //Se inicializa la instancia de bodegas
                existenciaBodega = new Bodega(BodegaCamion);
                lstArticulos.Clear();                
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico
                consulta = " SELECT tm.ARTICULO, tm.LOCALIZACION, tm.LOTE, tm.CANTIDAD, tm.CANT_FACT, tm.CANT_DIFERENCIA, tm.CONSECUTIVO " +
                           " FROM " + Table.ERPADMIN_TOMA_FISICA_INV + " tm" +
                           " WHERE UPPER(tm.COMPANIA) = '" + Compania.ToUpper() + "' " +
                           " AND tm.HANDHELD = '" + HandHeld + "' " +
                           " AND julianday(date(tm.FECHA)) = julianday(date('now','localtime')) " +
                           " AND tm.BODEGA = '" + BodegaCamion + "' " + 
                           " AND tm.ESTADO = '" + BOLETA_PENDIENTE + "' " +
                           " AND tm.CANT_DIFERENCIA <> 0";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                while (reader.Read())
                {
                    datos.Articulo = reader.GetString(0);
                    datos.BodegaCamion = BodegaCamion;
                    datos.Localizacion = reader.GetString(1);
                    datos.Lote = reader.GetString(2);
                    datos.Cantidad = string.IsNullOrEmpty(reader.GetValue(3).ToString()) ? 0 : Convert.ToDecimal( reader.GetValue(3).ToString());
                    datos.CantidadFacturar = string.IsNullOrEmpty(reader.GetValue(4).ToString()) ? 0 : Convert.ToDecimal(reader.GetValue(4).ToString());
                    datos.CantidadDiferencia = string.IsNullOrEmpty(reader.GetValue(5).ToString()) ? 0 : Convert.ToDecimal(reader.GetValue(5).ToString());
                    datos.Consecutivo = Convert.ToInt16(reader.GetValue(6).ToString());
                    //Se agrega al ListView los datos del artículo que se está procesando
                    //datos[0] = reader.GetString(0);
                    //datos[1] = BodegaCamion;
                    //datos[2] = reader.GetString(1);
                    //datos[3] = reader.GetString(2);
                    //datos[4] = string.IsNullOrEmpty(reader.GetValue(3).ToString()) ? "0" : reader.GetValue(3).ToString();
                    //datos[5] = string.IsNullOrEmpty(reader.GetValue(4).ToString()) ? "0" : reader.GetValue(4).ToString();
                    //datos[6] = string.IsNullOrEmpty(reader.GetValue(5).ToString()) ? "0" : reader.GetValue(5).ToString();
                    //datos[7] = reader.GetValue(6).ToString();

                    //Se agrega el item a la lista
                    lstArticulos.Add((datos));
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;
        }

        public bool ExisteBoleta(ref bool existe)
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;            
            string consulta = string.Empty;
            int count = 0;
            SQLiteDataReader reader = null;
            #endregion Definición de Varaibles

            try
            {
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico                
                consulta = " SELECT COUNT(0) " +
                           " FROM " + Table.ERPADMIN_TOMA_FISICA_INV +
                           " WHERE UPPER(COMPANIA) = '" + Compania.ToUpper() + "' " +
                           " AND HANDHELD = '" + HandHeld + "' " +
                           " AND julianday(date(FECHA)) = julianday(date('now','localtime')) " +
                           " AND ARTICULO = '" + Articulo + "' " +
                           " AND BODEGA = '" + BodegaCamion + "' ";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                if (reader.Read())
                {
                    count = Convert.ToInt16(reader.GetValue(0));
                }

                //Se asiga el valor de retorno
                existe = (count > 0);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;
        }

        public bool ObtenerSigteConsecBoleta()
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            int count = 0;
            SQLiteDataReader reader = null;
            #endregion Definición de Varaibles

            try
            {
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico
                consulta = " SELECT MAX(consecutivo) " +
                           " FROM " + Table.ERPADMIN_TOMA_FISICA_INV;

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                if (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetValue(0).ToString()))
                    {
                        count = Convert.ToInt16(reader.GetValue(0));
                    }
                    else
                    {
                        count = 0;
                    }
                }
                else
                {
                    count = 0;
                }

                Consecutivo = count + 1;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;
        }

        public bool AplicarTomaFisica()
        {
            #region Definicion de variables
            bool procesoExitoso = true;
            string actExistBodega = string.Empty;
            string actExistLote = string.Empty;
            decimal existenciaBodega = decimal.Zero;
            decimal existenciaLote = decimal.Zero;
            List<TomaFisicaInventario>  lstTomaFisica = null;
            #endregion Definicion de variables

            try
            {
                //Se crean los update a la base de datos
                #region Definición de Updates
                //Se crea el update para existencia_bodega
                actExistBodega = String.Format("UPDATE {0} SET existencia = existencia + @EXISTENCIABOD ", Table.ERPADMIN_ARTICULO_EXISTENCIA);
                actExistBodega += " WHERE articulo = @ARTICULO AND bodega = @BODEGA AND UPPER(compania) = @COMPANIA";

                //Se crea el update para existencia_lote
                actExistLote = String.Format("UPDATE {0} SET existencia = existencia + @EXISTENCIALOTE ", Table.ERPADMIN_ARTICULO_EXISTENCIA_LOTE);
                actExistLote += " WHERE articulo = @ARTICULO AND bodega = @BODEGA AND UPPER(compania) = @COMPANIA";
                actExistLote += " AND LOTE = @LOTE AND LOCALIZACION = @LOCALIZACION ";
                #endregion Definición de Updates


                //Se obtienen los datos de las boletas a aplicar
                if (procesoExitoso)
                {
                    //Se inicializa el ListView
                    lstTomaFisica = new List<TomaFisicaInventario>();

                    //Se obtiene las líneas de toma física a aplicar
                    procesoExitoso = CargarArticulosBoletaFisica(ref lstTomaFisica);
                }

                //Se realiza la aplicación de todas las boletas encontradas
                if (procesoExitoso)
                {
                    //Se valida que existan boletas para aplicar
                    if (lstTomaFisica.Count > 0)
                    {
                        foreach (TomaFisicaInventario item in lstTomaFisica)
                        {
                            //Se asigna los valores a las propiedades
                            Articulo = item.Articulo;
                            BodegaCamion = item.BodegaCamion;
                            Localizacion = item.Localizacion;
                            Lote = item.Lote;
                            existenciaBodega = item.cantDiferencia;
                            existenciaLote = existenciaBodega;

                            if (existenciaBodega != 0)
                            {
                                //Se crean los parámetros necesarios para cada actualizacion 
                                SQLiteParameterList paramExistBod =new SQLiteParameterList();
                                paramExistBod.Add("@EXISTENCIABOD", SqlDbType.Decimal, existenciaBodega);
                                paramExistBod.Add("@ARTICULO", SqlDbType.NVarChar, Articulo);
                                paramExistBod.Add("@BODEGA", SqlDbType.NVarChar, BodegaCamion);
                                paramExistBod.Add("@COMPANIA", SqlDbType.NVarChar, Compania.ToUpper());
                                

                                //Se crean los parámetros necesarios para cada actualizacion
                                SQLiteParameterList paramExistLote = new SQLiteParameterList();
                                paramExistLote.Add("@EXISTENCIALOTE", SqlDbType.Decimal, existenciaLote);
                                paramExistLote.Add("@ARTICULO", SqlDbType.NVarChar, Articulo);
                                paramExistLote.Add("@BODEGA", SqlDbType.NVarChar, BodegaCamion);
                                paramExistLote.Add("@COMPANIA", SqlDbType.NVarChar, Compania);
                                paramExistLote.Add("@LOCALIZACION", SqlDbType.NVarChar, Localizacion);
                                paramExistLote.Add("@LOTE", SqlDbType.NVarChar, Lote);                                    

                                //Se actualiza la existencia de la bodega
                                GestorDatos.EjecutarScalar(actExistBodega, paramExistBod);

                                //Se actualiza la existencia del lote
                                GestorDatos.EjecutarScalar(actExistLote, paramExistLote);
                            }

                            //Se cambia el estado de la boleta
                            procesoExitoso = ActualizarEstadoBoleta(Convert.ToInt16(item.Consecutivo), BOLETA_APLICADA);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Problemas aplicando boletas de inventario físico. " + ex.Message);
                //Mensaje.mostrarAlerta("Problemas aplicando boletas de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;            
        }

        public bool FacturarDiferencias(ref List<TomaFisicaInventario> lstArtFacturar)
        {
            bool procesoExitoso = true;
            //System.Windows.Forms.ListView lstArtFacturar = new System.Windows.Forms.ListView();

            try
            {
                procesoExitoso = CargarArticulosFacturar(ref lstArtFacturar);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Error al tratar de facturar las diferencias encontradas. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }
    }
}
