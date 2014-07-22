using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class LineaRegalia
    {
        #region Attributes
        private Regla regla;
        private Escala escala;

        private string articuloBonificado;
        private decimal cantidadBonificada;
        private decimal montoDescuento;
        private decimal porcentajeDescuento;


        private decimal cantidadPedida;

        private TipoRegalia tipo;

        #endregion

        #region Properties
        public Regla Regla
        {
            get { return regla; }
            set { regla = value; }
        }
        public Escala Escala
        {
            get { return escala; }
            set { escala = value; }
        }
        public string ArticuloBonificado
        {
            get { return articuloBonificado; }
            set { articuloBonificado = value; }
        }
        public decimal CantidadBonificada
        {
            get { return cantidadBonificada; }
            set { cantidadBonificada = value; }
        }
        public decimal MontoDescuento
        {
            get { return montoDescuento; }
            set { montoDescuento = value; }
        }
        public decimal PorcentajeDescuento
        {
            get { return porcentajeDescuento; }
            set { porcentajeDescuento = value; }
        }
        public TipoRegalia Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construye una linea de regalia apartir de una bonificacion
        /// </summary>
        /// <param name="regla"></param>
        /// <param name="articuloBonificado"></param>
        /// <param name="cantidadBonificada"></param>
        public LineaRegalia(Regla regla, Escala escala, String articuloBonificado, decimal cantidadPedida)
        {
            this.regla = regla;
            this.escala = escala;
            this.articuloBonificado = articuloBonificado;
            this.cantidadPedida = cantidadPedida;
            tipo = TipoRegalia.Bonificacion;
        }
        /// <summary>
        /// Contruye una linea de regalia apartir de un descuento.
        /// </summary>
        /// <param name="regla"></param>
        /// <param name="montoDescuento"></param>
        public LineaRegalia(Regla regla, decimal montoDescuento, decimal porcentajeDescuento, TipoRegalia tipo)
        {
            this.regla = regla;
            this.montoDescuento = montoDescuento;
            this.porcentajeDescuento = porcentajeDescuento;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Obtiene la linea por bonificar calculada según la regla
        /// </summary>
        /// <returns></returns>
        public DetallePedido ObtenerLineaBonificacion(ClienteCia cliente)
        {
            DetallePedido detalle = null;
            if (tipo == TipoRegalia.Bonificacion)
            {
                Articulo articulo = new Articulo(articuloBonificado,regla.Compania);
                cliente.CargarNivelPrecio();
                articulo.Cargar(new FiltroArticulo[] { FiltroArticulo.NivelPrecio }, cliente.NivelPrecio.Nivel);
                articulo.CargarPrecio(cliente.NivelPrecio);   
                if (articulo != null)
                {
                    detalle = new DetallePedido();
                    detalle.Articulo = articulo;
                    decimal cantAlmacen, cantDetalle = 0;
                    cantidadBonificada = ObtenerCantidadBonificada(cantidadPedida, escala.ObtenerFactorAlmacen(!this.regla.UtilizarArticuloLinea), escala.ObtenerCantidadInicialConsolidada(this.regla.UtilizarArticuloLinea), escala.ObtenerCantidadRegalia(this.regla.UtilizarArticuloLinea));
                    CalcularUnidades(cantidadBonificada, articulo.UnidadEmpaque, out cantAlmacen, out cantDetalle);

                    detalle.UnidadesAlmacen = cantAlmacen;
                    detalle.UnidadesDetalle = cantDetalle;

                    detalle.Precio = articulo.Precio;
                    detalle.EsBonificada = true;
                }
                
            }

            return detalle;
        }

        /// <summary>
        /// Calcula las cantidad en unidades de almacén y detalle
        /// </summary>
        /// <param name="cantidadPedida"></param>
        /// <param name="factor"></param>
        /// <param name="almacen"></param>
        /// <param name="detalle"></param>
        private void CalcularUnidades(decimal cantidadPedida, decimal factor, out decimal almacen, out decimal detalle)
        {
            decimal cantPedidaFraccion = cantidadPedida - Decimal.Floor(cantidadPedida);
            decimal cantPedidaEntera = Decimal.Floor(cantidadPedida);
            almacen = detalle = 0;
            if (factor > 1)
            {
                detalle = (factor * cantPedidaFraccion);
                almacen = cantPedidaEntera;
                detalle =  Math.Round(detalle, 0);
            }
            else
            {
                almacen = cantPedidaEntera;
                detalle = cantPedidaFraccion;
            }
        }

        /// <summary>
        /// Obtiene la cantidad a bonificar según los parámetros de la escala
        /// </summary>
        /// <param name="pedida">Cantidad que se está facturando</param>
        /// <param name="factor">Cantidades que por cada compra va a bonificar</param>
        /// <param name="inicial">Cantidad inical para la bonificación</param>
        /// <param name="bonificar">Cantidades a bonificar del artículo</param>
        /// <returns>Retorna la cantidad a bonificar</returns>
        private decimal ObtenerCantidadBonificada(decimal pedida, decimal factor, decimal inicial, decimal bonificar)
        {
            decimal resultado = inicial;
            if (factor > 0)
            {
                resultado = (Decimal.Truncate(pedida / factor) * bonificar) + inicial;
            }
            return resultado;
        }

        
        #endregion


    }

    public enum TipoRegalia
    {
        Bonificacion = 'B',
        DescuentoLinea = 'L',
        DescuentoGeneral = 'G'
    }
}
