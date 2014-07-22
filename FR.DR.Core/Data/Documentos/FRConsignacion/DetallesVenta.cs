using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion
{

    /// <summary>
    /// Representa una lista de detalles/Lineas asociados a una venta en consignacion
    /// </summary>
    public class DetallesVenta
    {
        
        #region Propiedades

        private List<DetalleVenta> lista = new List<DetalleVenta>();
        /// <summary>
        /// Detalles de la venta
        /// </summary>
        public List<DetalleVenta> Lista
        {
            get { return lista; }
            set { lista = value; }
        }

        private Encabezado encabezado = new Encabezado();
        /// <summary>
        /// Encabezado de la consignacion
        /// </summary>
        public Encabezado Encabezado
        {
            get { return encabezado; }
            set { encabezado = value; }
        }

        private bool selected;
        /// <summary>
        /// Indica el estado de la venta en consignación.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// Total de Articulos de la venta
        /// </summary>
        public decimal TotalArticulos
        {
            get
            {
                decimal totalArticulos = decimal.Zero;

                foreach (DetalleVenta detalle in lista)
                    totalArticulos += detalle.UnidadesAlmacen;

                return totalArticulos;

            }
        }

        /// <summary>
        /// Total de lineas de la venta
        /// </summary>
        public int TotalLineas
        {
            get
            {
                return lista.Count;
            }
        }

        /// <summary>
        /// Existe cambio de precios entre lo que indica el detalle y el articulo asociado
        /// </summary>
        public bool CambioEnPrecio
        {
            get
            {
                foreach(DetalleVenta detalle in this.lista)
                {
                    if (detalle.Articulo.Precio.Maximo != detalle.Precio.Maximo)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Existe detalles
        /// </summary>
        /// <returns>Existe detalles</returns>
        public bool Vacio()
        {
            return (lista.Count == 0);
        }

        #endregion
        
        #region Logica Negocio

        /// <summary>
        /// Buscar un detalle de la venta
        /// </summary>
        /// <param name="articulo">Codigo del articulo</param>
        /// <returns>Detalle</returns>
        public DetalleVenta Buscar(string articulo)
        {
            foreach (DetalleVenta detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                    return detalle;

            }
            return null;
        }
        /// <summary>
        /// Busca líneas de detalle que cumplan con un criterio de búsqueda.
        /// </summary>
        /// <param name="criterio">Criterio de búsqueda del artículo.</param>
        /// <param name="valor">Valor a buscar.</param>
        /// <param name="exacto">Indica que la búsqueda debe ser exacta según el criterio.</param>
        /// <returns>Retorna el conjunto de detalles resultantes de la búsqueda.</returns>
        public DetallesVenta Buscar(CriterioArticulo criterio, string valor, bool exacto)
        {
            DetallesVenta coincidencias = new DetallesVenta();
            coincidencias.Encabezado = this.encabezado;
            coincidencias.Lista.Clear();

            foreach (DetalleVenta detalle in this.Lista)
            {
                if (detalle.Articulo.BusquedaDesconectada(criterio, valor, exacto))
                {
                    coincidencias.Lista.Add(detalle);
                }
            }
            //Caso:32997 ABC 18/06/2008
            //Cuando se filtra por codigo de barras y no retorna resultados, hacerlo tambien por codigo de articulo
            if (criterio == CriterioArticulo.Barras && coincidencias.Vacio())
            {
                foreach (DetalleVenta detalle in this.Lista)
                {
                    if (detalle.Articulo.BusquedaDesconectada(CriterioArticulo.Codigo, valor, exacto))
                    {
                        coincidencias.Lista.Add(detalle);
                    }
                }
            }
            return coincidencias;
        }
        /// <summary>
        /// Busca dentro de los detalles de una boleta de venta en consignación aquellos detalles
        /// que coincidan con el filtro de búsqueda.
        /// </summary>
        /// <param name="criterio">Corresponde al criterio de búsqueda.</param>
        /// <param name="filtro">Corresponde al filtro de búsqueda.</param>
        /// <returns>Retorna una lista de detalles que cumplen por aproximación el filtro y criterio de búsqueda.</returns>
        public List<DetalleVenta> Buscar(CriterioArticulo criterio,string filtro)
        {
            List<DetalleVenta> articulos = new List<DetalleVenta>();
            articulos.Clear();

            foreach (DetalleVenta detalle in this.lista)
            {
                if (detalle.Articulo.BuscarCadenaAproximada(criterio, filtro))
                    articulos.Add(detalle);
            }
        
            return articulos;
        }
        /// <summary>
        /// Eliminar un articulo de la lista de detalles de venta
        /// </summary>
        /// <param name="articulo"></param>
        /// <returns></returns>
        public DetalleVenta Eliminar(string articulo)
        {
            int pos = -1;
            int cont = 0;
            DetalleVenta detalleEliminado = null;

            foreach (DetalleVenta detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                {
                    pos = cont;
                    break;
                }
                cont++;
            }
            if (pos != -1)
            {
                detalleEliminado = lista[pos];
                lista.RemoveAt(pos);
            }
            else
                throw new Exception("No se pudo eliminar el detalle del pedido o factura.");

            return detalleEliminado;
        }
        #endregion

        #region Acceso Datos

        /// <summary>
        /// Guarda el detalle de la venta en consignación en la base de datos.
        /// Rebaja las existencias de la bodega.
        /// Caso: 32809 ABC 30/05/2008
        /// Rebaja las existencias cuando factura un desglose de boleta en Consignación        
        /// </summary>
        /// <param name="desglose">Si es un desglose para rebajar existencias.</param>
        public void Guardar(bool desglose)
        {
            int contador = 0;
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_DET_CONSIG +
                "        ( COD_CIA,  NUM_CSG,  NUM_LN,  COD_ART,  CNT_MAX,  CNT_MIN,  MON_TOT,  MON_PRC_MX,  MON_PRC_MN,  SALDO_MAX,  SALDO_MIN,  COD_ZON, COD_CLT ) " +
                " VALUES (@COD_CIA, @NUM_CSG, @NUM_LN, @COD_ART, @CNT_MAX, @CNT_MIN, @MON_TOT, @MON_PRC_MX, @MON_PRC_MN, @SALDO_MAX, @SALDO_MIN, @COD_ZON, @COD_CLT )";

            foreach (DetalleVenta detalle in this.lista)
            {
                Bitacora.Escribir("Se guarda el detalle '" + detalle.ToString() + "'.");
                detalle.NumeroLinea = ++contador;

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, encabezado.Compania),
                    GestorDatos.SQLiteParameter("@NUM_CSG",SqlDbType.NVarChar, encabezado.Numero),
                    GestorDatos.SQLiteParameter("@NUM_LN", SqlDbType.Int,detalle.NumeroLinea),
                    GestorDatos.SQLiteParameter("@COD_ART", SqlDbType.NVarChar,detalle.Articulo.Codigo),
                    GestorDatos.SQLiteParameter("@CNT_MAX",SqlDbType.Decimal,detalle.UnidadesAlmacen),
                    GestorDatos.SQLiteParameter("@CNT_MIN",SqlDbType.Decimal,detalle.UnidadesDetalle),
                    GestorDatos.SQLiteParameter("@MON_TOT",SqlDbType.Decimal,detalle.MontoTotal),                                       
                    GestorDatos.SQLiteParameter("@MON_PRC_MX",SqlDbType.Decimal,detalle.Precio.Maximo),   
                    GestorDatos.SQLiteParameter("@MON_PRC_MN",SqlDbType.Decimal,detalle.Precio.Minimo),   
                    GestorDatos.SQLiteParameter("@SALDO_MAX",SqlDbType.Decimal,detalle.UnidadesAlmacenSaldo),   
                    GestorDatos.SQLiteParameter("@SALDO_MIN",SqlDbType.Decimal,detalle.UnidadesDetalleSaldo),
                    
                    GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, encabezado.Zona),
                    GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, encabezado.Cliente) });

                int linea = GestorDatos.EjecutarComando(sentencia, parametros);
                if (linea != 1)
                    throw new Exception("No se puede guardar el detalle '" + detalle.Articulo.Codigo + "' del pedido '" + encabezado.Numero + "'.");

                //Caso: 32809 ABC 30/05/2008
                //Rebaja las existencias cuando factura un desglose de boleta en Consignación
                //Condiciona cuando no se debe hacer la rebaja de existencias si es producto de desglose.
                if (!desglose)
                    detalle.RebajarExistencia();
            }
        }

        /// <summary>
        /// Elimina las líneas de la boleta de venta en consignación.
        /// </summary>
        /// <param name="detalles">Indica la cantidad de detalles que posee la boleta de venta en consignación.</param>
        public void Eliminar(int detalles)
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_DET_CONSIG +
                " WHERE COD_CIA = @COD_CIA" +
                " AND NUM_CSG = @NUM_CSG";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, encabezado.Compania),
                GestorDatos.SQLiteParameter("@NUM_CSG", SqlDbType.NVarChar, encabezado.Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows != detalles)
                throw new Exception("Error al eliminar los detalles de boleta de la compañía '" + encabezado.Compania + "'.");
        }

        //Caso 32007 LDS 07/04/2008
        /// <summary>
        /// Restablece las existencias de los detalles de una venta en consignación que no ha sido sincronizada.
        /// </summary>
        /// <param name="codigoBodega">Código de la bodega en la cual se debe restablecer las existencias.</param>
        /// <param name="detalles">Cantidad de detalles de la venta en consignación.</param>
        public void RestablecerExistenciasConsignacion(string codigoBodega, int detalles)
        {
            SQLiteDataReader reader=null;
            Bodega bodega = new Bodega(codigoBodega);
            
            int cantidadDetalles = 0;
            string articulo;
            decimal cantidadAlmacen;
            decimal cantidadDetalle;
            decimal cantidadTotalAlmacen;
            decimal saldoAlmacen;
            decimal saldoDetalle;
            decimal saldoTotalAlmacen;
            decimal cantidadRestablecer;
            decimal factorEmpaque;

            string sentencia =
                "SELECT COD_ART,CNT_MAX,CNT_MIN,SALDO_MAX,SALDO_MIN FROM " + Table.ERPADMIN_alFAC_DET_CONSIG +
                " WHERE COD_CIA = @COD_CIA" +
                " AND NUM_CSG = @NUM_CSG " +
                " ORDER BY NUM_LN";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, encabezado.Compania),
                GestorDatos.SQLiteParameter("@NUM_CSG", SqlDbType.NVarChar, encabezado.Numero)});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    articulo = reader.GetString(0);
                    cantidadAlmacen = reader.GetDecimal(1);
                    cantidadDetalle = reader.GetDecimal(2);
                    saldoAlmacen = reader.GetDecimal(3);
                    saldoDetalle = reader.GetDecimal(4);

                    factorEmpaque = Articulo.ObtenerFactorEmpaque(articulo, encabezado.Compania);

                    cantidadTotalAlmacen = cantidadAlmacen + (cantidadDetalle / factorEmpaque);
                    saldoTotalAlmacen = saldoAlmacen + (saldoDetalle / factorEmpaque);
                    
                    cantidadRestablecer = cantidadTotalAlmacen - saldoTotalAlmacen;
                    bodega.ActualizarExistencia(encabezado.Compania, articulo, cantidadRestablecer);

                    cantidadDetalles++;
                }

                if (cantidadDetalles != detalles)
                    throw new Exception("La cantidad de líneas de la venta en consignación en la compañía '" + encabezado.Compania + "' es incorrecta.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error restableciendo las existencias de las líneas de la venta en consignación en la compañía '" + encabezado.Compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }


        /// <summary>
        /// Carga los detalles de la consignacion desde la base datos.
        /// </summary>
        /// <param name="excluirSinSaldos">Solo obtener detalles pendientes de saldar</param>
        /// <param name="nivelPrecio">Nivel de precio asociado a la venta</param>
        public void Obtener(bool excluirSinSaldos, NivelPrecio nivelPrecio)
        {
            lista.Clear();
            SQLiteDataReader reader = null;

            if(excluirSinSaldos && !VentaConsignacion.ExisteConsignacionSaldos(this.encabezado.Compania, this.encabezado.Zona, this.encabezado.Cliente))
                throw new Exception("La venta en consignación para el cliente '" + this.encabezado.Cliente + "', compañía '" + this.encabezado.Compania + "' y ruta '" + this.encabezado.Zona + "' no posee detalles con saldos.");

            string sentencia =
                " SELECT D.COD_ART,D.CNT_MAX,D.CNT_MIN,SALDO_MAX,SALDO_MIN,D.MON_PRC_MX,D.MON_PRC_MN, MON_TOT " +// Articulo.CAMPOS_ARTICULO +
                @" FROM " + Table.ERPADMIN_alFAC_DET_CONSIG + " D " +
                @" WHERE D.NUM_CSG = @CONSECUTIVO " +
                @" AND   UPPER(D.COD_CIA) = @COMPANIA " +
                (excluirSinSaldos ? " AND (SALDO_MAX > 0 OR SALDO_MIN > 0) " : "") +
                @" ORDER BY NUM_LN";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Encabezado.Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar, Encabezado.Numero)});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DetalleVenta detalle = new DetalleVenta();
                    detalle.Articulo.Codigo = reader.GetString(0);
                    detalle.Articulo.Compania = Encabezado.Compania;
                    //Carga la información del artículo correspondiente al detalle.
                    detalle.Articulo.Cargar();

                    detalle.UnidadesAlmacen = reader.GetDecimal(1);
                    detalle.UnidadesDetalle = reader.GetDecimal(2);
                    detalle.UnidadesAlmacenSaldo = reader.GetDecimal(3);
                    detalle.UnidadesDetalleSaldo = reader.GetDecimal(4);
                    detalle.Precio = new Precio(reader.GetDecimal(5), reader.GetDecimal(6));
                    detalle.MontoTotal = reader.GetDecimal(7);
                    ///////////////////////////////////////////////////////////////////////
                    //Requerido para saber si cambiaron los precios en el desglose
                    detalle.Articulo.CargarPrecio(nivelPrecio);
                    ///////////////////////////////////////////////////////////////////////
                    lista.Add(detalle);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando la boleta de venta en consignación. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }
        /// <summary>
        /// Obtener los detalles a restablecer de una boleta en consignacion
        /// </summary>
        /// <param name="numero">numero de la boleta</param>
        /// <param name="compania">compania de la boleta</param>
        /// <returns>Lista de detalles</returns>
        public static List<DetalleRestablecer> ObtenerInformacionBoleta(string numero, string compania)
        {
            string sentencia = string.Empty;
            List<DetalleRestablecer> detalles = new List<DetalleRestablecer>();
            DetalleRestablecer detalleRestablecer;
            SQLiteDataReader reader = null;

            sentencia =
                " SELECT COD_ART,CNT_MAX,CNT_MIN,SALDO_MAX,SALDO_MIN,MON_PRC_MX,MON_PRC_MN FROM " + Table.ERPADMIN_alFAC_DET_CONSIG +
                " WHERE COD_CIA = @COD_CIA " +
                " AND   NUM_CSG = @NUM_CSG "+
                " ORDER BY COD_ART";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, compania),
                GestorDatos.SQLiteParameter("@NUM_CSG", SqlDbType.NVarChar, numero)});

                try
                {
                    reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                    while (reader.Read())
                    {
                        detalleRestablecer = new DetalleRestablecer(reader.GetString(0),reader.GetDecimal(1), reader.GetDecimal(2), reader.GetDecimal(3), reader.GetDecimal(4), new Precio(reader.GetDecimal(5), reader.GetDecimal(6)));
                        detalleRestablecer.Articulo.Compania = compania;
                        detalles.Add(detalleRestablecer);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error leyendo los detalles de la boleta de la compañía '" + compania + "'. " + ex.Message);
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
