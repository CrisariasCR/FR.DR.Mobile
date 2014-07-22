using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Ordenamiento de una lista de detalles
    /// </summary>
    public class DetalleSort : IComparer<DetallePedido>,IComparer<DetalleGarantia>, IComparer<DetalleDevolucion>, IComparer<DetalleVenta>
    {
        #region Comparer

        /// <summary>
        /// Criterios de Ordenacion del documento
        /// </summary>
        public enum Ordenador
        { Ninguno = 0, Descripcion = 1, Clase = 2, Familia = 3, Codigo = 4, Linea = 5 /*solo aplica en el pedido*/}

        /// <summary>
        /// Obtener el ordenador
        /// </summary>
        /// <param name="enumeration">Tipo de Ordenador de Documento</param>
        /// <param name="puedeOrdenarXLinea">Se pueden ordenar por numero de linea</param>
        /// <returns></returns>
        public static IEnumerable<Ordenador> GetValues(Ordenador enumeration, bool puedeOrdenarXLinea)
        {
            List<Ordenador> enumerations = new List<Ordenador>();
            foreach (FieldInfo fieldInfo in enumeration.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumerations.Add((Ordenador)fieldInfo.GetValue(enumeration));
                if (((Ordenador)fieldInfo.GetValue(enumeration) == Ordenador.Linea) && !puedeOrdenarXLinea)
                    enumerations.Remove(Ordenador.Linea);
            }
            return enumerations;
        }

        private Ordenador comparador;
        /// <summary>
        /// Ordenador de articulos
        /// </summary>
        /// <param name="comparador">Criterio de comparacion</param>
        public DetalleSort(Ordenador comparador)
        {
            this.comparador = comparador;
        }
        /// <summary>
        /// Comparador de objeto generico
        /// </summary>
        /// <param name="x">primer objeto</param>
        /// <param name="y">segundo objeto</param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            if (x is DetallePedido)
                return Compare((DetallePedido)x, (DetallePedido)y);
            else if (x is DetalleGarantia)
                return Compare((DetalleGarantia)x, (DetalleGarantia)y);
            else if (x is DetalleDevolucion)
                return Compare((DetalleDevolucion)x, (DetalleDevolucion)y);
            else if (x is DetalleVenta)
                return Compare((DetalleVenta)x, (DetalleVenta)y);
            else if (x is Articulo)
                return Compare((Articulo)x, (Articulo)y);
            else
                return 0;
        }
        /// <summary>
        /// Comparador por detalles de pedidos
        /// </summary>
        /// <param name="x">primer articulo</param>
        /// <param name="y">segundo articulo</param>
        /// <returns></returns>
        public int Compare(DetallePedido x, DetallePedido y)
        {
            switch (comparador)
            {
                case DetalleSort.Ordenador.Linea:
                    return Compare(x.NumeroLinea, y.NumeroLinea);
                default:
                    return Compare(x.Articulo, y.Articulo);
            }
        }
        /// <summary>
        /// Comparador por detalles de garantias
        /// </summary>
        /// <param name="x">primer articulo</param>
        /// <param name="y">segundo articulo</param>
        /// <returns></returns>
        public int Compare(DetalleGarantia x, DetalleGarantia y)
        {
            switch (comparador)
            {
                case DetalleSort.Ordenador.Linea:
                    return Compare(x.NumeroLinea, y.NumeroLinea);
                default:
                    return Compare(x.Articulo, y.Articulo);
            }
        }
        /// <summary>
        /// Comparador por detalle de devolucion
        /// </summary>
        /// <param name="x">primer articulo</param>
        /// <param name="y">segundo articulo</param>
        /// <returns></returns>
        public int Compare(DetalleDevolucion x, DetalleDevolucion y)
        {
            return Compare(x.Articulo, y.Articulo);
        }
        /// <summary>
        /// Comparador por detalles de consignaciones
        /// </summary>
        /// <param name="x">primer articulo consignado</param>
        /// <param name="y">segundo articulo consignado</param>
        /// <returns></returns>
        public int Compare(DetalleVenta x, DetalleVenta y)
        {
            return Compare(x.Articulo, y.Articulo);

        }

        /// <summary>
        /// Comparador por articulos
        /// </summary>
        /// <param name="x">primer articulo</param>
        /// <param name="y">segundo articulo</param>
        /// <returns></returns>
        private int Compare(Articulo x, Articulo y)
        {
            switch (comparador)
            {
                case DetalleSort.Ordenador.Descripcion:
                    return (x.Descripcion.CompareTo(y.Descripcion));
                case DetalleSort.Ordenador.Clase:
                    return (x.Clase.CompareTo(y.Clase));
                case DetalleSort.Ordenador.Familia:
                    return (x.Familia.CompareTo(y.Familia));
                case DetalleSort.Ordenador.Codigo:
                default:
                    return (x.Codigo.CompareTo(y.Codigo));
            }
        }
        #endregion
    }
}
