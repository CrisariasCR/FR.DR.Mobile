using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using EMF.Printing;
using System.Collections;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario
{
    /// <summary>
    /// Representa la lista de detalles de inventario asociada a un cliente para una compania
    /// </summary>
    public class DetallesInventario : IPrintable //Para cada compania
    {
        /// <summary>
        /// Crear una lista de detalles
        /// </summary>
        public DetallesInventario()
        {}

        /// <summary>
        /// Crear una lista de detalles
        /// </summary>
        /// <param name="compania"></param>
        public DetallesInventario(string compania)
        {
            this.encabezado.Compania = compania;
        }
        /// <summary>
        /// Crear una lista de detalles
        /// </summary>
        /// <param name="compania">Compania asociada a la lista de los detalles de inventario</param>
        /// <param name="zona">Zona asociada a la lista de los detalles de inventario</param>
        /// <param name="numero">Numero de inventario asociado a la lista</param>
        public DetallesInventario(string compania, string zona, string numero)
        {
            this.encabezado.Compania = compania;
            this.encabezado.Zona = zona;
            this.encabezado.Numero = numero;
        }

        #region Propiedades

        private List<DetalleInventario> lista = new List<DetalleInventario>();
        /// <summary>
        /// Detalles del Inventario
        /// </summary>
        public List<DetalleInventario> Lista
        {
            get { return lista; }
            set { lista = value; }
        }

        private Encabezado encabezado = new Encabezado();
        /// <summary>
        /// Encabezado del inventario
        /// </summary>
        public Encabezado Encabezado
        {
            get { return encabezado; }
            set { encabezado = value; }
        }

        /// <summary>
        /// Total de lineas del inventario
        /// </summary>
        public int TotalLineas
        {
            get
            {
                return lista.Count;
            }        
        }

        #endregion 

        #region Acceso Datos
       
        /// <summary>
        /// Guardar los detalles del inventario
        /// </summary>
        public void Guardar()
        {
            string sentencia =
                "INSERT INTO " + Table.ERPADMIN_alFAC_DET_INV  +
                "        ( COD_CIA,  NUM_INV,  NUM_LN,  COD_ART,  CNT_MAX,  CNT_MIN,  COD_ZON) " +
                " VALUES (@COD_CIA, @NUM_INV, @NUM_LN, @COD_ART, @CNT_MAX, @CNT_MIN, @COD_ZON)";
            
            int linea = 1;
            //Primero se guardan los detalles de inventario
            foreach (DetalleInventario detalle in this.lista)
            {
                detalle.NumeroLinea = linea++;

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, encabezado.Compania),
                GestorDatos.SQLiteParameter("@NUM_INV",SqlDbType.NVarChar, encabezado.Numero),
                GestorDatos.SQLiteParameter("@NUM_LN", SqlDbType.Int, detalle.NumeroLinea),
                GestorDatos.SQLiteParameter("@COD_ART", SqlDbType.NVarChar,detalle.Articulo.Codigo),
                GestorDatos.SQLiteParameter("@CNT_MAX",SqlDbType.Decimal,detalle.UnidadesAlmacen),
                GestorDatos.SQLiteParameter("@CNT_MIN",SqlDbType.Decimal,detalle.UnidadesDetalle),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, encabezado.Zona)});

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
        }
        /// <summary>
        /// Eliminar los detalles del inventario
        /// </summary>
        public void Eliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_DET_INV +
                " WHERE COD_CIA = @COD_CIA" +
                " AND NUM_INV = @NUM_INV";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, encabezado.Compania),
                GestorDatos.SQLiteParameter("@NUM_INV", SqlDbType.NVarChar, encabezado.Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows < 1)
                throw new Exception("No se afectó ninguna línea del inventario.");
        }

        /// <summary>
        /// Carga los detalles del inventario desde la base datos.
        /// </summary>
        public void Obtener()
        {
            lista.Clear();

            SQLiteDataReader reader = null;

            string sentencia =
                " SELECT D.COD_ART,D.CNT_MAX,D.CNT_MIN "+// Articulo.CAMPOS_ARTICULO +
                @" FROM " + Table.ERPADMIN_alFAC_DET_INV + " D " + //Articulo.INNER_JOIN +
                @" WHERE D.NUM_INV = @CONSECUTIVO " +
                @" AND   UPPER(D.COD_CIA) = @COMPANIA ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Encabezado.Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar, Encabezado.Numero)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DetalleInventario detalle = new DetalleInventario();

                    detalle.UnidadesAlmacen = reader.GetDecimal(1);
                    detalle.UnidadesDetalle = reader.GetDecimal(2);

                    detalle.Articulo.Codigo = reader.GetString(0);
                    detalle.Articulo.Compania = Encabezado.Compania;
                    detalle.Articulo.Cargar();
                    //detalle.Articulo.Cargar(ref reader, 3);

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

        #region Detalle de Inventario

        /// <summary>
        /// Determinar si existen detalles del inventario
        /// </summary>
        /// <returns>existen detalles o no</returns>
        public bool Vacio()
        {
            return (lista.Count == 0);
        }

        /// <summary>
        /// Buscar un conjunto de detalles de la lista que cumplen con cierto criterio
        /// </summary>
        /// <param name="criterio">criterio de busqueda</param>
        /// <param name="valor">cadena de busqueda</param>
        /// <param name="exacto">si es una busqueda exacta o no</param>
        /// <returns>Los detalles que coinciden en la busqueda</returns>
        public DetallesInventario Buscar(CriterioArticulo criterio, string valor, bool exacto)
        {
            DetallesInventario coincidencias = new DetallesInventario();
            coincidencias.Encabezado = this.encabezado;
            coincidencias.Lista.Clear();

            foreach (DetalleInventario detalle in lista)
            {
                if (detalle.Articulo.BusquedaDesconectada(criterio, valor, exacto))
                {
                    coincidencias.Lista.Add(detalle);
                }
            }
            return coincidencias;
        }
        /// <summary>
        /// Buscar un detalle/linea de inventario segun el codigo del articulo
        /// </summary>
        /// <param name="articulo"></param>
        /// <returns></returns>
        public DetalleInventario Buscar(string articulo)
        {
            foreach (DetalleInventario detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                {
                    return detalle;
                }
            }
            return null;
        }
        /// <summary>
        /// Eliminar del inventario un articulo
        /// </summary>
        /// <param name="articulo">codigo del articulo a eliminar</param>
        public void Eliminar(string articulo)
        {
            int pos = -1;
            int cont = 0;

            foreach (DetalleInventario detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                {
                    pos = cont;
                    break;
                }

                cont++;
            }
            if (pos != -1)
                lista.RemoveAt(pos);
        }
        /// Metodo encargado de realizar gestion del detalle de inventario.		
        /// </summary>
        /// <param name="articulo">Articulo que contiene toda la informacion correspondiente al articulo a gestionar en el detalle</param>
        /// <param name="cantidadDetalle">unidades de detalle a gestionar del articulo</param>
        /// <param name="cantidadAlmacen">unidades de almacen a gestionar del articulo</param>
        public void Gestionar(Articulo articulo, decimal cantidadDetalle, decimal cantidadAlmacen)
        {
            DetalleInventario detalle = Buscar(articulo.Codigo);

            //El detalle no existe
            if (detalle == null)
            {
                //Crear el detalle
                detalle = new DetalleInventario(articulo, cantidadDetalle, cantidadAlmacen);
                this.lista.Add(detalle);
            }
            //El detalle Existe
            else
            {
                //Actualizamos la cantidad
                detalle.UnidadesAlmacen = cantidadAlmacen;
                detalle.UnidadesDetalle = cantidadDetalle;
            }
        }
        #endregion

        #region Reporte
        
        /// <summary>
        /// Cargar los detalles de inventario propios de una compania
        /// </summary>
        /// <param name="detalles">detalles de todas las companias</param>
        public void CargarEnCompania(ICollection<DetalleInventario> detalles)
        {
            foreach (DetalleInventario detalle in detalles)
            {
                if (detalle.Articulo.Compania.Equals(Encabezado.Compania))
                    lista.Add(detalle);
            }		        
        }

        #endregion

        #region IPrintable Members

        public string GetObjectName()
        {
            return "DETALLECOMPANIA";
        }

        public object GetField(string name)
        {
            if (name == "CODIGO")
                return this.Encabezado.Compania;

            if (name == "DETALLESINVENTARIO")
                return new ArrayList(lista);

            return null;
        }

        #endregion
    }
}
