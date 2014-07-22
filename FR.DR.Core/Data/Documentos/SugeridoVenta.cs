using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Sugerido de Ventas para pedidos
    /// </summary>
    public class SugeridoVenta
    {
        #region Variables y Propiedades de instancia
       
        
        private string codigo = string.Empty;
        /// <summary>
        /// Codigo del sugerido de venta
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }
        
        private string descripcion = string.Empty;        
        /// <summary>
        /// Descripcion del sugerido de venta
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        
        private string cliente = string.Empty;
        /// <summary>
        /// Cliente asociado al sugerido de venta
        /// </summary>
        public string Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        private List<DetalleDocumento> detalle = new List<DetalleDocumento>();
        /// <summary>
        /// Detalles del sugerido de venta
        /// </summary>
        public List<DetalleDocumento> Detalle
        {
            get { return detalle; }
            set { detalle = value; }
        }
       
        #endregion


        #region Acceso Datos

        /// <summary>
        /// Obtiene los sugeridos de venta asociados a un cliente
        /// </summary>
        /// <param name="cliente">Cliente a asociar</param>
        /// <returns>Sugeridos</returns>
        public static List<SugeridoVenta> ObtenerSugeridosVenta(string cliente)
        {
            List<SugeridoVenta> sugeridos = new List<SugeridoVenta>();
            sugeridos.Clear();

            string sentencia =
                " SELECT S.SUGERIDO,S.DESCRIPCION " +
                " FROM " + Table.ERPADMIN_SUGERIDOVENTA + " S, " + Table.ERPADMIN_SUGERIDO_ASOC + " A " +
                " WHERE A.SUGERIDO = S.SUGERIDO " +
                " AND A.CLIENTE = @CLIENTE";

            SQLiteDataReader reader = null;
            try
            {
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@CLIENTE",cliente)});

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    SugeridoVenta sugerido = new SugeridoVenta();
                    sugerido.codigo = reader.GetString(0);
                    sugerido.descripcion = reader.GetString(1);
                    sugerido.cliente = cliente;
                    sugeridos.Add(sugerido);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando sugeridos de venta. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return sugeridos;
        }

        /// <summary> 
        /// Carga los detalles/lineas de un sugerido de ventas de un cliente
        /// </summary>
        public void CargarDetalle()
        {
            SQLiteDataReader reader = null;
            string sentencia = string.Empty;

            sentencia =
                " SELECT ARTICULO,COMPANIA,CANTALM,CANTDET " +
                " FROM " + Table.ERPADMIN_SUGERIDO_ASOC_LINEA +
                " WHERE SUGERIDO = @SUGERIDO" +
                " AND CLIENTE = @CLIENTE";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@CLIENTE",cliente),
                new SQLiteParameter("@SUGERIDO",codigo)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Articulo a = new Articulo();

                    a.Codigo = reader.GetString(0);
                    a.Compania = reader.GetString(1);

                    DetalleDocumento sugeridoDetalle = new DetalleDocumento();
                    sugeridoDetalle.Articulo = a;
                    sugeridoDetalle.UnidadesAlmacen = reader.GetDecimal(2);
                    sugeridoDetalle.UnidadesDetalle = reader.GetDecimal(3);

                    detalle.Add(sugeridoDetalle);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando detalles del sugerido de venta. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        #endregion
    }
}
