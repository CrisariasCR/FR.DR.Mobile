using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion
{

    #region Clase DetalleRestablecer
    /// <summary>
    /// Representa un detalle que se desea restablecer cuando se requiere eliminar una factura o devolución que
    /// fue generada por el desglose de una boleta de venta en consignación.
    /// </summary>
    public class DetalleRestablecer
    {

        #region Variables de instancias

        #region Variables privadas
        
        /// <summary>
        /// Artículo asociado al detalle que se requiere restablecer.
        /// </summary>
        private Articulo articulo;
        /// <summary>
        /// Cantidad en unidades de almacén del detalle que se requiere restablecer.
        /// </summary>
        private decimal cantidadAlmacen;
        /// <summary>
        /// Cantidad en unidades de detalle del detalle que se requiere restablecer.
        /// </summary>
        private decimal cantidadDetalle;
        /// <summary>
        /// Saldo en unidades de almacén del detalle que se requiere restablecer.
        /// </summary>
        private decimal saldoAlmacen;
        /// <summary>
        /// Saldo en unidades de detalle del detalle que se requiere restablecer.
        /// </summary>
        private decimal saldoDetalle;

        private Precio precio;
        /// <summary>
        /// Precio del detalle que se requiere restablecer.
        /// </summary>
        public Precio Precio
        {
            get { return precio; }
            set { precio = value; }
        }

        #endregion

        #endregion

        #region Propiedades de instancia

        #region Propiedades públicas
        /// <summary>
        /// Artículo asociado al detalle que se requiere restablecer.
        /// </summary>
        public Articulo Articulo
        {
            set
            {
                this.articulo = value;
            }
            get
            {
                return this.articulo;
            }
        }
        /// <summary>
        /// Cantidad en unidades de almacén del detalle que se requiere restablecer.
        /// </summary>
        public decimal CantidadAlmacen
        {
            get
            {
                return this.cantidadAlmacen;
            }
        }
        /// <summary>
        /// Cantidad en unidades de detalle del detalle que se requiere restablecer.
        /// </summary>
        public decimal CantidadDetalle
        {
            get
            {
                return this.cantidadDetalle;
            }
        }
        /// <summary>
        /// Saldo en unidades de almacén del detalle que se requiere restablecer.
        /// </summary>
        public decimal SaldoAlmacen
        {
            get
            {
                return this.saldoAlmacen;
            }
        }
        /// <summary>
        /// Saldo en unidades de detalle del detalle que se requiere restablecer.
        /// </summary>
        public decimal SaldoDetalle
        {
            get
            {
                return this.saldoDetalle;
            }
        }
        #endregion

        #endregion

        #region Constructor
        
        /// <summary>
        /// Representa un detalle que se desea restablecer cuando se requiere eliminar una factura o devolución que
        /// fue generada por el desglose de una boleta de venta en consignación.
        /// Se debe utilizar para generar el detalle a partir de un detalle de una boleta de venta en consignación.
        /// </summary>
        /// <param name="articulo">detalle que se desea restablecer.</param>
        /// <param name="cantidadAlmacen">Cantidad en unidad de almacén del detalle que se desea restablecer.</param>
        /// <param name="cantidadDetalle">Cantidad en unidad de detalle del detalle que se desea restablecer.</param>
        /// <param name="saldoAlmacen">Saldo en unidades de almacén del detalle que se requiere restablecer.</param>
        /// <param name="saldoDetalle">Saldo en unidades de detalle del detalle que se requiere restablecer.</param>
        public DetalleRestablecer(string articulo, decimal cantidadAlmacen, decimal cantidadDetalle, decimal saldoAlmacen, decimal saldoDetalle, Precio precio)
        {

            this.cantidadAlmacen = cantidadAlmacen;
            this.cantidadDetalle = cantidadDetalle;
            this.saldoAlmacen = saldoAlmacen;
            this.saldoDetalle = saldoDetalle;
            this.articulo = new Articulo();
            this.articulo.Codigo = articulo;
            this.Precio = precio;
        }
        #endregion

        #region Métodos de instancia

        #region Métodos públicos
        /// <summary>
        /// Agrega las cantidades en unidad de almacén y/o unidad de detalle al detalle que se requiere restablecer.
        /// </summary>
        /// <param name="cantidadAlmacen">Cantidad en unidades de almacén que se desea agregar al detalle.</param>
        /// <param name="cantidadDetalle">Cantidad en unidades de detalle que se desea agregar al detalle.</param>
        public void AgregarCantidades(decimal cantidadAlmacen, decimal cantidadDetalle)
        {
            this.cantidadAlmacen += cantidadAlmacen;
            this.cantidadDetalle += cantidadDetalle;
            this.saldoAlmacen += cantidadAlmacen;
            this.saldoDetalle += cantidadDetalle;
        }
        /// <summary>
        /// Disminuye las cantidades en unidad de almacén y/o unidad de detalle al detalle que se requiere restablecer.
        /// </summary>
        /// <param name="cantidadAlmacen">Cantidad en unidades de almacén que se desea disminuir al detalle.</param>
        /// <param name="cantidadDetalle">Cantidad en unidades de detalle que se desea disminuir al detalle.</param>
        public void DisminuirCantidades(decimal cantidadAlmacen, decimal cantidadDetalle)
        {
            decimal totalAlmacenDisminuir = cantidadAlmacen + (cantidadDetalle / this.articulo.UnidadEmpaque);
            decimal totalAlmacenRestablecer = this.cantidadAlmacen + (this.cantidadDetalle / this.articulo.UnidadEmpaque);
            decimal totalSaldoRestablecer = this.saldoAlmacen + (this.saldoDetalle / this.articulo.UnidadEmpaque);
            decimal cantidadAlmacenDisminuir = (int)totalAlmacenDisminuir;
            decimal cantidadDetalleDisminuir = (totalAlmacenDisminuir - cantidadAlmacenDisminuir) * this.articulo.UnidadEmpaque;
            this.cantidadAlmacen = (int)totalAlmacenRestablecer;
            this.cantidadDetalle = (totalAlmacenRestablecer - this.cantidadAlmacen) * this.articulo.UnidadEmpaque;
            this.saldoAlmacen = (int)totalSaldoRestablecer;
            this.saldoDetalle = (totalSaldoRestablecer - this.saldoAlmacen) * this.articulo.UnidadEmpaque;

            if (this.articulo.UnidadEmpaque == 1)
            {
                this.cantidadAlmacen += this.cantidadDetalle;
                this.cantidadDetalle = decimal.Zero;
                this.saldoAlmacen += this.saldoDetalle;
                this.saldoDetalle = decimal.Zero;
                cantidadAlmacenDisminuir += cantidadDetalleDisminuir;
                cantidadDetalleDisminuir = decimal.Zero;
            }

            this.cantidadAlmacen -= cantidadAlmacenDisminuir;
            this.cantidadDetalle -= cantidadDetalleDisminuir;
            this.saldoAlmacen -= cantidadAlmacenDisminuir;
            this.saldoDetalle -= cantidadDetalleDisminuir;
        }
        #endregion

        #endregion

    }
    #endregion


}
