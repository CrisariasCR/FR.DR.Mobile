using System;
using FR.DR.Core.Helper;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    /// <summary>
    /// Posibles criterios de busqueda de un articulo
    /// </summary>
    public enum CriterioArticulo
    {
        [Description("Código")]
        Codigo,
        [Description("Código Barras")]
        Barras,
        [Description("Descripción")]
        Descripcion,
        [Description("Clase")]
        Clase,
        [Description("Familia")]
        Familia,
        [Description("Ninguno")]
        Ninguno,
        [Description("Pedido Actual")]
        PedidoActual,
        [Description("Factura Actual")]
        FacturaActual,
        [Description("Venta Actual")]
        VentaActual,
        [Description("Boleta Actual")]
        BoletaActual        
    }
    /// <summary>
    /// Filtros de busqueda para articulos
    /// </summary>
    public enum FiltroArticulo
    {
        Existencia,
        NivelPrecio,
        GrupoArticulo
    }
    /// <summary>
    /// Clase que administra las busquedas de articulos
    /// </summary>
    public class CriterioBusquedaArticulo
    {
        private CriterioArticulo criterio = CriterioArticulo.Ninguno;
        /// <summary>
        /// campo para la busqueda
        /// </summary>
        public CriterioArticulo Criterio
        {
            get { return criterio; }
            set { criterio = value; }
        }
        private string valor;
        /// <summary>
        /// Valor a buscar
        /// </summary>
        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }
        private bool agil;
        /// <summary>
        /// Si la busqueda es exacta o no
        /// </summary>
        public bool Agil
        {
            get { return agil; }
            set { agil = value; }
        }
        /// <summary>
        /// Da formato al valor a buscar segun exista busqueda agil o no
        /// </summary>
        private void FormatoValor()
        {
            if (criterio != CriterioArticulo.Barras)
            {
                //La busqueda agil impide el uso de consultas con %
                if(! agil)
                    valor = "%" + valor + "%";
            }
        }
        public CriterioBusquedaArticulo(CriterioArticulo c, string value)
        {
            this.criterio = c;
            this.valor = value;
            FormatoValor();
        }
        public CriterioBusquedaArticulo(CriterioArticulo c, string value, bool agil)
        {
            this.criterio = c;
            this.valor = value;
            this.agil = agil;
            FormatoValor();
        }
    }

}
