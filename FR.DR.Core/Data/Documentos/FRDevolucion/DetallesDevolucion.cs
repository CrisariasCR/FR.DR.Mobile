using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Corporativo;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion
{
    /// <summary>
    /// Representa la lista de detalles de devolucion asociada a un cliente para una compania
    /// </summary>
    public class DetallesDevolucion
    {
        /// <summary>
        /// Generar detalles
        /// </summary>
        /// <param name="compania">compania asociada</param>
        /// <param name="zona">ruta asociada</param>
        /// <param name="numero">consecutivo de la devolucion asociada</param>
        public DetallesDevolucion(string compania, string zona, string numero)
        {
            this.encabezado.Compania = compania;
            this.encabezado.Zona = zona;
            this.encabezado.Numero = numero;
        }
        public DetallesDevolucion() { }

        #region Propiedades

        private List<DetalleDevolucion> lista = new List<DetalleDevolucion>();
        /// <summary>
        /// Detalles de la Devolucion
        /// </summary>
        public List<DetalleDevolucion> Lista
        {
            get { return lista; }
            set { lista = value; }
        }

        private Encabezado encabezado = new Encabezado();
        /// <summary>
        /// Encabezado de devolucion
        /// </summary>
        public Encabezado Encabezado
        {
            get { return encabezado; }
            set { encabezado = value; }
        }
        
        private string bodega;
        /// <summary>
        /// Bodega de la devolucion
        /// </summary>
        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }

        private string documentoAsociado = string.Empty;
        /// <summary>
        /// Factura asociada en una devolucion con documento
        /// </summary>
        public string DocumentoAsociado
        {
            get { return documentoAsociado; }
            set { documentoAsociado = value; }
        }
        private int nivelPrecio = -1;
        /// <summary>
        /// Nivel de precio asociado a la devolucion
        /// </summary>
        public int NivelPrecio
        {
            get { return nivelPrecio; }
            set { nivelPrecio = value; }
        }
        /// <summary>
        /// Obtiene el descuento total por lineas.
        /// </summary>
        public decimal MontoDescuentoLineas
        {
            get
            {
                //Sumamos los descuentos de todas las lineas
                decimal monto = 0;

                foreach (DetalleDevolucion detalle in lista)
                    monto += detalle.MontoDescuento;

                return monto;
            }
        }
        /// <summary>
        /// Total de articulos del detalle
        /// </summary>
        public decimal TotalArticulos
        {
            get
            {
                decimal totalArticulos = decimal.Zero;
                
                foreach(DetalleDevolucion detalle in lista)
                    totalArticulos += detalle.UnidadesAlmacen;
                
                return totalArticulos;
                
            }
        }
        /// <summary>
        /// Total de lineas del detalle
        /// </summary>
        public int TotalLineas
        {
            get
            {
                return lista.Count;
            }
        }

        #endregion 

        #region Logica 

        #region Detalle de Devolucion

        /// <summary>
        /// Indica la existencia de detalles
        /// </summary>
        /// <returns></returns>
        public bool Vacio()
        {
            return (lista.Count == 0);
        }
        
        /// <summary>
        /// Busca una serie de detalles que coincidan con un criterio de busqueda
        /// </summary>
        /// <param name="criterio">criterio a filtrar</param>
        /// <param name="valor">cadena a buscar</param>
        /// <param name="exacto">si la cadena debe ser exacta</param>
        /// <param name="estado">estado de los detalles</param>
        /// <returns>lista de detalles coincidentes</returns>
        public DetallesDevolucion Buscar(CriterioArticulo criterio, string valor, bool exacto, Estado estado)
        {
            DetallesDevolucion coincidencias = new DetallesDevolucion();
            coincidencias.Encabezado = this.encabezado;
            coincidencias.Lista.Clear();
            
            foreach (DetalleDevolucion detalle in lista)
            {
                if (detalle.Articulo.BusquedaDesconectada(criterio, valor, exacto) && (detalle.Estado == estado || estado == Estado.NoDefinido) )
                {
                    coincidencias.Lista.Add(detalle);
                }
            }
            return coincidencias;
        }
        /// <summary>
        /// Buscar un detalle en la lista
        /// </summary>
        /// <param name="articulo">codigo de articulo asociado</param>
        /// <param name="estado">estado del detalle</param>
        /// <returns>detalle buscado</returns>
        public DetalleDevolucion Buscar(string articulo, Estado estado)
        {
            foreach (DetalleDevolucion detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo) && detalle.Estado == estado)
                {
                    return detalle;
                }
            }
            return null;
        }

        #endregion


        #endregion
        #region Acceso Datos

        /// <summary>
        /// Guardar los detalles de una devolucion
        /// </summary>
        /// <param name="devolucionConDocumento">si la devolucion es con documento actualizar el historico</param>
        public void Guardar(bool devolucionConDocumento)
        {
            string sentencia =
                "INSERT INTO " + Table.ERPADMIN_alFAC_DET_DEV  +
                "        ( COD_CIA,  NUM_DEV,  COD_ART,  CNT_MAX,  CNT_MIN,  IND_DEV,  OBS_DEV, LOTE,   MON_TOT, MON_PRC_MX,  MON_PRC_MN,  MON_DSC,  POR_DSC_AP,  COD_ZON,   LST_PRE) " +
                " VALUES (@COD_CIA, @NUM_DEV, @COD_ART, @CNT_MAX, @CNT_MIN, @IND_DEV, @OBS_DEV, @LOTE, @MON_TOT, @MON_PRC_MX, @MON_PRC_MN, @MON_DSC, @POR_DSC_AP, @COD_ZON, @NVL_PRE )";

            int linea = 1;
            //Primero se guardan los detalles
            foreach (DetalleDevolucion detalle in this.lista)
            {
                detalle.NumeroLinea = linea++;

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, encabezado.Compania),
                    GestorDatos.SQLiteParameter("@NUM_DEV",SqlDbType.NVarChar, encabezado.Numero),
                    GestorDatos.SQLiteParameter("@NUM_LN", SqlDbType.Int,detalle.NumeroLinea),
                    GestorDatos.SQLiteParameter("@COD_ART", SqlDbType.NVarChar,detalle.Articulo.Codigo),
                    GestorDatos.SQLiteParameter("@CNT_MAX",SqlDbType.Decimal,detalle.UnidadesAlmacen),
                    GestorDatos.SQLiteParameter("@CNT_MIN",SqlDbType.Decimal,detalle.UnidadesDetalle),
                    GestorDatos.SQLiteParameter("@IND_DEV", SqlDbType.NVarChar,((char)detalle.Estado).ToString()),
                    GestorDatos.SQLiteParameter("@OBS_DEV", SqlDbType.NVarChar,detalle.Observaciones),                                          
                    GestorDatos.SQLiteParameter("@LOTE", SqlDbType.NVarChar,detalle.Lote),    
                    GestorDatos.SQLiteParameter("@MON_TOT",SqlDbType.Decimal,detalle.MontoTotal),                                       
                    GestorDatos.SQLiteParameter("@MON_PRC_MX",SqlDbType.Decimal,detalle.Precio.Maximo),   
                    GestorDatos.SQLiteParameter("@MON_PRC_MN",SqlDbType.Decimal,detalle.Precio.Minimo),   
                    GestorDatos.SQLiteParameter("@MON_DSC",SqlDbType.Decimal,detalle.MontoDescuento),   
                    GestorDatos.SQLiteParameter("@POR_DSC_AP",SqlDbType.Decimal,detalle.CalcularPorcentajeDescuento()),
                    GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, encabezado.Zona),
                    GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int , nivelPrecio)});
                int rows = GestorDatos.EjecutarComando(sentencia, parametros);
				    
                if(rows != 1)
					    throw new Exception("No se puede guardar el detalle '" + detalle.Articulo.Codigo + "' de la devolución '" + encabezado.Numero + "'.");


                #region Caso PA2-01549-0KNV JEV
                //if (detalle.Estado == Estado.Bueno && !devolucionConDocumento)
                if (detalle.Estado == Estado.Bueno)
                #endregion Caso PA2-01549-0KNV JEV
                {
                    detalle.Articulo.Bodega = new Bodega(Bodega);
                    detalle.AumentarExistencia();
                    detalle.DisminuirExistenciaEnvase(Bodega);
                }

                if (devolucionConDocumento)
                {
                    this.ActualizarDetalleHistorico(detalle, documentoAsociado);
                }
				//this.sumarArticulos(detalle);

            }
        }
        //ABC 36136
        /// <summary>
        /// Actualiza la cantidad devuelta del articulo en el historico de facturas
        /// </summary>
        /// <param name="detalle">detalle a actualizar</param>
        /// <param name="factura">Numero de Pedido</param>
        public void ActualizarDetalleHistorico(DetalleDevolucion detalle, string factura)
        {
            string sentencia =
                    " UPDATE  " + Table.ERPADMIN_alFAC_DET_HIST_FAC +
                    " SET CANT_DEV = CANT_DEV + @CANT_DEV " +
                    " WHERE NUM_FAC = @CONSECUTIVO " +
                    "   AND UPPER(COD_CIA) = @COMPANIA" +
                    "   AND COD_ART = @ARTICULO" +
                    "   AND (EST_BON IS NULL OR EST_BON = 'N')";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar, encabezado.Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar, factura),
                GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar,detalle.Articulo.Codigo),
                GestorDatos.SQLiteParameter("@CANT_DEV",SqlDbType.Decimal,detalle.TotalAlmacen)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows != 1)
                throw new Exception("Error Actualizando cantidad devuelta de " + detalle.Articulo.Codigo + "' en la Factura '" + factura + ".");
        }
        /// <summary>
        /// Eliminar los detalles de una devolucion
        /// </summary>
        public void Eliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_DET_DEV  +
                " WHERE COD_CIA = @COD_CIA" +
                " AND NUM_DEV = @NUM_DEV";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, encabezado.Compania),
                GestorDatos.SQLiteParameter("@NUM_DEV", SqlDbType.NVarChar, encabezado.Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows < 1)
                throw new Exception("No se afectó ninguna línea de la devolución.");
        }

        /// <summary>
        /// Carga los detalles de la devolucion desde la base datos.
        /// </summary>
        public void Obtener()
        {
            lista.Clear();
            SQLiteDataReader reader = null;

            string sentencia =
                " SELECT D.COD_ART,D.IND_DEV,D.CNT_MAX,D.CNT_MIN,D.OBS_DEV,D.LOTE,D.MON_TOT,D.MON_DSC,D.POR_DSC_AP,D.MON_PRC_MX,D.MON_PRC_MN " + //Articulo.CAMPOS_ARTICULO +
                @" FROM " + Table.ERPADMIN_alFAC_DET_DEV + " D " + //Articulo.INNER_JOIN +
                @" WHERE D.NUM_DEV = @CONSECUTIVO " +
                @" AND   UPPER(D.COD_CIA) = @COMPANIA ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Encabezado.Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar, Encabezado.Numero)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DetalleDevolucion detalle = new DetalleDevolucion();
                    detalle.Articulo.Codigo = reader.GetString(0);
                    detalle.Articulo.Compania = Encabezado.Compania;               
                    detalle.Articulo.Cargar();
                    detalle.Articulo.Bodega.Codigo = Bodega;
                    detalle.Estado = (Estado)Convert.ToChar(reader.GetString(1));
                    detalle.UnidadesAlmacen = reader.GetDecimal(2);
                    detalle.UnidadesDetalle = reader.GetDecimal(3);
                    if (!reader.IsDBNull(4))
                        detalle.Observaciones = reader.GetString(4);
                    if (!reader.IsDBNull(5))
                        detalle.Lote = reader.GetString(5);

                    detalle.MontoTotal = reader.GetDecimal(6);
                    detalle.MontoDescuento = reader.GetDecimal(7);
                    detalle.Descuento = new Descuento();
                    detalle.Descuento.Monto = reader.GetDecimal(8);
                    detalle.Precio = new Precio(reader.GetDecimal(9),reader.GetDecimal(10));
                    lista.Add(detalle);
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
        }

        #endregion

        /// <summary>
        /// Anular los detalles de la devolucion restableciendo las existencias (disminuir en camion)
        /// </summary>
        public void Anular(bool esConsignacion)
        {
            foreach (DetalleDevolucion detalle in Lista)
            {
                if (detalle.Estado == Estado.Bueno)
                {
                    if (esConsignacion)
                        detalle.Articulo.Bodega.Codigo = this.Bodega;
                    detalle.DisminuirExistencia();
                }
            }
        }
    }
}
