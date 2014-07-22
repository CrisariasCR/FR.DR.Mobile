using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using EMF.Printing;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion
{
    /// <summary>
    /// Representa un detalle/linea especifica de una venta en consignacion
    /// </summary>
    public class DetalleVenta : DetalleLinea,IPrintable
    {
        #region Variables y Propiedades de instancia

        #region Impuestos

        private bool esExentoImpuesto1 = false;
        /// <summary>
        /// Variable que indica si la linea es exenta de impuesto de venta.
        /// </summary>
        public bool EsExentoImpuesto1
        {
            get { return esExentoImpuesto1; }
            set { esExentoImpuesto1 = value; }
        }

        private bool esExentoImpuesto2 = false;
        /// <summary>
        /// Variable que indica si la linea es exenta de impuesto de consumo.
        /// </summary>
        public bool EsExentoImpuesto2
        {
            get { return esExentoImpuesto2; }
            set { esExentoImpuesto2 = value; }
        }

        #endregion

        #region Unidades

        private decimal unidadesAlmacenSaldo = 0;
        /// <summary>
        /// Saldo en unidades de almacén del detalle luego de haber realizado el desglose de una boleta de venta en consignación.
        /// </summary>
        public decimal UnidadesAlmacenSaldo
        {
            get { return unidadesAlmacenSaldo; }
            set { unidadesAlmacenSaldo = value; }
        }

        private decimal unidadesDetalleSaldo = 0;
        /// <summary>
        /// Saldo en unidades de detalle del detalle luego de haber realizado el desglose de una boleta de venta en consignación.
        /// </summary>
        public decimal UnidadesDetalleSaldo
        {
            get { return unidadesDetalleSaldo; }
            set { unidadesDetalleSaldo = value; }
        }

        #endregion

        #region Unidades para el desglose

        private decimal unidadesAlmacenExistencia = decimal.Zero;
        /// <summary>
        /// Existencia de unidades de almacen en el desglose
        /// </summary>
        public decimal UnidadesAlmacenExistencia
        {
            get { return unidadesAlmacenExistencia; }
            set { unidadesAlmacenExistencia = value; }
        }
        private decimal unidadesDetalleExistencia = decimal.Zero;
        /// <summary>
        /// Existencia de unidades de detalle en el desglose
        /// </summary>
        public decimal UnidadesDetalleExistencia
        {
            get { return unidadesDetalleExistencia; }
            set { unidadesDetalleExistencia = value; }
        }
        private decimal unidadesAlmacenVendido = decimal.Zero;
        /// <summary>
        /// unidades de almacen vendidas en el desglose
        /// </summary>
        public decimal UnidadesAlmacenVendido
        {
            get { return unidadesAlmacenVendido; }
            set { unidadesAlmacenVendido = value; }
        }
        private decimal unidadesDetalleVendido = decimal.Zero;
        /// <summary>
        /// unidades de detalle vendidas en el desglose
        /// </summary>
        public decimal UnidadesDetalleVendido
        {
            get { return unidadesDetalleVendido; }
            set { unidadesDetalleVendido = value; }
        }
        private decimal unidadesAlmacenBuenEstado = decimal.Zero;
        /// <summary>
        /// unidades de almacen devueltas en buen estado en el desglose
        /// </summary>
        public decimal UnidadesAlmacenBuenEstado
        {
            get { return unidadesAlmacenBuenEstado; }
            set { unidadesAlmacenBuenEstado = value; }
        }
        private decimal unidadesDetalleBuenEstado = decimal.Zero;
        /// <summary>
        /// unidades de detalle devueltas en buen estado en el desglose
        /// </summary>
        public decimal UnidadesDetalleBuenEstado
        {
            get { return unidadesDetalleBuenEstado; }
            set { unidadesDetalleBuenEstado = value; }
        }
        private decimal unidadesAlmacenMalEstado = decimal.Zero;
        /// <summary>
        /// unidades de almacen devueltas en mal estado en el desglose
        /// </summary>
        public decimal UnidadesAlmacenMalEstado
        {
            get { return unidadesAlmacenMalEstado; }
            set { unidadesAlmacenMalEstado = value; }
        }
        private decimal unidadesDetalleMalEstado = decimal.Zero;
        /// <summary>
        /// unidades de detalle devueltas en mal estado en el desglose
        /// </summary>
        public decimal UnidadesDetalleMalEstado
        {
            get { return unidadesDetalleMalEstado; }
            set { unidadesDetalleMalEstado = value; }
        }

        #endregion

        #region Totales

        /// <summary>
        /// Obtiene el saldo total en unidades de almacén del detalle que permanecen en la venta en consignación.
        /// Suma de unidades de almacén más unidades de detalle.
        /// </summary>
        public decimal TotalAlmacenSaldo
        {
            get
            {
                return unidadesAlmacenSaldo + (unidadesDetalleSaldo / Articulo.UnidadEmpaque);
            }
        }
        #endregion

        #region Totales para el Desglose

        /// <summary>
        /// Unidades de almacén totales del detalle de la boleta de venta en consignación que han sido desglosadas.
        /// </summary>
        public decimal TotalAlmacenDesglose
        {
            get
            {
                decimal cantidadTotalDesgloseAlmacen = this.unidadesAlmacenVendido + this.unidadesAlmacenBuenEstado + this.unidadesAlmacenMalEstado;
                decimal cantidadTotalDesgloseDetalle = this.unidadesDetalleVendido + this.unidadesDetalleBuenEstado + this.unidadesDetalleMalEstado;
                return (cantidadTotalDesgloseAlmacen + (cantidadTotalDesgloseDetalle / this.Articulo.UnidadEmpaque));
            }
        }

        /// <summary>
        /// Unidades en almacén totales que existen en la bodega de ruteo para el detalle de la boleta de venta en consignación.
        /// </summary>
        private decimal totalAlmacenExistencia;
        /// <summary>
        /// Unidades en almacén totales que existen en la bodega de ruteo para el detalle de la boleta de venta en consignación.
        /// </summary>
        public decimal TotalAlmacenExistencia
        {
            set
            {
                this.totalAlmacenExistencia = value;
                unidadesAlmacenExistencia = decimal.ToInt32(this.totalAlmacenExistencia);
                this.unidadesDetalleExistencia = (this.totalAlmacenExistencia - this.unidadesAlmacenExistencia) * Articulo.UnidadEmpaque;
            }
            get
            {
                return this.totalAlmacenExistencia;
            }
        }

        #endregion

        #region Datos del Desglose de Devolucion

        private string lote;
        /// <summary>
        /// Indica el lote donde debe ser devuelto el artículo.
        /// </summary>
        public string Lote
        {
            get { return lote; }
            set { lote = value; }

        }

        private string observaciones = string.Empty;
        /// <summary>
        /// Observaciones que se hicieron al devolver el producto
        /// </summary
        public string Observaciones
        {
            get { return observaciones; }
            set { observaciones = value; }
        }
        #endregion

        #endregion

        public DetalleVenta()
        {

        }

        #region Logica Impuestos (Igual a la logica de Pedido, posteriormente heredar)
        /// <summary>
        /// Permite indicar si el detalle del pedido es exento de impuesto de venta. El
        /// detalle es exento de impuesto de ventas si el porcentaje de impuesto de ventas del artículo es
        /// Cero o si el cliente del pedido tiene definido un porcentaje de exoneración del impuesto de ventas.		
        /// </summary>
        /// <param name="exoneracionIV">
        /// Porcentaje de exoneracion de impuesto de ventas del cliente.
        /// </param>
        private void DefinirExentoImpuestoVenta(decimal exoneracionIV)
        {
            if (this.Articulo.Impuesto.Impuesto1 == decimal.Zero || exoneracionIV > 0)
                this.EsExentoImpuesto1 = true;
        }

        /// <summary>
        /// Permite indicar si el detalle del pedido es exento de impuesto de consumo. El
        /// detalle es exento de impuesto de consumo si el porcentaje de impuesto de consumo del artículo es
        /// Cero o si el cliente del pedido tiene definido un porcentaje de exoneración del impuesto de consumo.		
        /// </summary>
        /// <param name="exoneracionIC">
        /// Porcentaje de exoneracion de impuesto de consumo del cliente.
        /// </param>
        private void DefinirExentoImpuestoConsumo(decimal exoneracionIC)
        {
            if (this.Articulo.Impuesto.Impuesto2 == decimal.Zero || exoneracionIC > 0)
                this.EsExentoImpuesto2 = true;
        }
        /// <summary>
        /// Metodo que calcula los impuestos de la linea.
        /// </summary>
        /// <param name="exoneracionIV">Porcentaje de exoneracion de impuesto 1 del cliente.</param>
        /// <param name="exoneracionIC">Porcentaje de exoneracion de impuesto 2 del cliente.</param>
        /// <param name="porcDescGeneral">Porcentaje Descuento 1 + Porcentaje Descuento 2 del pedido.</param>
        public void CalcularImpuestos(decimal exoneracionIV, decimal exoneracionIC, decimal porcDescGeneral)
        {
            try
            {
                this.Articulo.CargarImpuesto();
                Impuesto.MontoImpuesto1 = Decimal.Round(
                    Articulo.Impuesto.ImpuestoVentaLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIV), 2);

                this.DefinirExentoImpuestoVenta(exoneracionIV);

                Impuesto.MontoImpuesto2 = Decimal.Round(
                    Articulo.Impuesto.ImpuestoConsumoLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIC), 2);

                this.DefinirExentoImpuestoConsumo(exoneracionIC);
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculando impuestos de la línea '" + this.Articulo.Codigo + "'." + ex.Message);
            }
        }
        #endregion

        #region

        /// <summary>
        /// Método que cambia las propiedades del detalle de la venta en consignación.
        /// Calcula los monto de la línea.
        /// Carga los descuentos del detalle.
        /// Carga las bonificaciones del detalle.
        /// Calcula los impuestos del detalle.
        /// </summary>
        /// <param name="cantidadAlmacen">Cantidad en unidades de almacén.</param>
        /// <param name="cantidadDetetalle">Cantidad en unidades de detalle.</param>
        /// <param name="cantidadAlmacenSaldo">Saldo en unidades de almacén del detalle, solo cuando se ha realizado el desglose de una boleta de venta en consignación.</param>
        /// <param name="cantidadDetetalleSaldo">Saldo en unidades de detalle del detalle, solo cuando se ha realizado el desglose de una boleta de venta en consignación.</param>
        /// <param name="precio">Precio del detalle.</param>
        /// <param name="cliente">Cliente de la venta en consignación.</param>
        /// <param name="porcDescuentoGeneral">
        /// Porcentaje de descuento general asignado a la venta en consignación.
        /// PorcentajeDescuento 1 + PorcentajeDescuento 2.
        /// </param>
        public void CambiarValores(
            decimal cantidadAlmacen,decimal cantidadDetetalle,decimal cantidadAlmacenSaldo,decimal cantidadDetetalleSaldo,
            Precio precio,FRCliente.ClienteCia cliente,decimal porcDescuentoGeneral)
        {
            this.UnidadesAlmacen = cantidadAlmacen;
            this.UnidadesDetalle = cantidadDetetalle;
            this.unidadesAlmacenSaldo = cantidadAlmacenSaldo;
            this.unidadesDetalleSaldo = cantidadDetetalleSaldo;
            this.Precio = precio;

            try
            {
                this.CalcularMontoLinea();
                //Calculamos nuevamente los impuestos
                this.CalcularImpuestos(cliente.ExoneracionImp1, cliente.ExoneracionImp2, porcDescuentoGeneral);
            
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculando impuestos de la línea." + ex.Message);
            }
        }

        /// <summary>
        /// Rebajar existencias de la linea
        /// </summary>
        public void RebajarExistencia()
        {
            //Debemos rebajar existencias
            decimal cantidadRebajar = TotalAlmacen - TotalAlmacenSaldo;
            if (cantidadRebajar > decimal.Zero)
            {
                try
                {
                       Articulo.ActualizarBodega( cantidadRebajar*-1);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error rebajando existencias al artículo '" + Articulo.Codigo + "'. " + ex.Message);
                }
            }
        }
        #endregion

        #region Logica de Desglose de la venta
        /// <summary>
        /// Aplica el desglose realizado al detalle de la boleta de venta en consignación.
        /// </summary>
        /// <param name="unidadesAlmacenVendido">Unidades de almacén vendidas del detalle de la boleta de venta en consignación.</param>
        /// <param name="unidadesDetalleVendido">Unidades de detalle vendidas del detalle de la boleta de venta en consignación.</param>
        /// <param name="unidadesAlmacenBuenEstado">Unidades de almacén devueltas en buen estado del detalle de la boleta de venta en consignación.</param>
        /// <param name="unidadesDetalleBuenEstado">Unidades de detalle devueltas en buen estado del detalle de la boleta de venta en consignación.</param>
        /// <param name="unidadesAlmacenMalEstado">Unidades de almacén devueltas en mal estado del detalle de la boleta de venta en consignación.</param>
        /// <param name="unidadesDetalleMalEstado">Unidades de detalle devueltas en mal estado del detalle de la boleta de venta en consignación.</param>
        /// <param name="loteDetalle">Lote del detalle devuelto tanto en buen estado como en mal estado.</param>
        /// <param name="observacion">Indica la nota de observación que se le puede asignar a un detalle que es devuelto.</param>
        /// <param name="unidadesAlmacenSaldo">Saldo de las unidades de almacén del detalle de la boleta de venta en consignación.</param>
        /// <param name="unidadesDetalleSaldo">Saldo de las unidades de detalle del detalle de la boleta de venta en consignación.</param>
        public void AplicarDesglose(
            decimal unidadesAlmacenVendido,
            decimal unidadesDetalleVendido,
            decimal unidadesAlmacenBuenEstado,
            decimal unidadesDetalleBuenEstado,
            decimal unidadesAlmacenMalEstado,
            decimal unidadesDetalleMalEstado,
            string loteDetalle,
            ref string observacion,
            decimal unidadesAlmacenSaldo,
            decimal unidadesDetalleSaldo)
        {
            this.unidadesAlmacenVendido = unidadesAlmacenVendido;
            this.unidadesDetalleVendido = unidadesDetalleVendido;
            this.unidadesAlmacenBuenEstado = unidadesAlmacenBuenEstado;
            this.unidadesDetalleBuenEstado = unidadesDetalleBuenEstado;
            this.unidadesAlmacenMalEstado = unidadesAlmacenMalEstado;
            this.unidadesDetalleMalEstado = unidadesDetalleMalEstado;
            this.UnidadesAlmacenSaldo = unidadesAlmacenSaldo;
            this.UnidadesDetalleSaldo = unidadesDetalleSaldo;

            if (!UnidadesDevueltas())
                observacion = loteDetalle = string.Empty;

            this.lote = loteDetalle;
            this.Observaciones = observacion;
        }

        /// <summary>
        /// Calcular cantidades en almacen y detalle
        /// </summary>
        /// <param name="cantidadAlmacen">cantidad en unidades de almacen</param>
        /// <param name="cantidadDetalle">cantidad en unidades de detalle</param>
        public void CalcularUnidades(ref decimal cantidadAlmacen, ref decimal cantidadDetalle)
        {
            decimal totalUnidadAlmacen = cantidadAlmacen + (cantidadDetalle / Articulo.UnidadEmpaque);
            cantidadAlmacen = (int)totalUnidadAlmacen;
            cantidadDetalle = (totalUnidadAlmacen - cantidadAlmacen) * Articulo.UnidadEmpaque;        
        }
        /// <summary>
        /// Existen unidades devueltas
        /// </summary>
        /// <returns>unidades devueltas</returns>
        public bool UnidadesDevueltas()
        {
            return (UnidadesDevueltasBuenEstado() ||
                UnidadesDevueltasMalEstado());
        }
        /// <summary>
        /// Existen unidades pendientes de saldar
        /// </summary>
        /// <returns>unidades pendientes de saldar</returns>
        public bool UnidadesSaldo()
        {
            return (unidadesDetalleSaldo > Decimal.Zero ||unidadesAlmacenSaldo > Decimal.Zero);
        }
        /// <summary>
        /// Existen unidades vendidas
        /// </summary>
        /// <returns>unidades vendidas</returns>
        public bool UnidadesVendidas()
        {
            return (unidadesAlmacenVendido > Decimal.Zero || unidadesDetalleVendido  > Decimal.Zero);
        }
        /// <summary>
        /// Existen Unidades devueltas en mal estado
        /// </summary>
        /// <returns>Unidades devueltas en mal estado</returns>
        public bool UnidadesDevueltasMalEstado()
        {
            return ((UnidadesAlmacenMalEstado > Decimal.Zero || UnidadesDetalleMalEstado > Decimal.Zero) );
        }
        /// <summary>
        /// Existen Unidades devueltas en buen estado
        /// </summary>
        /// <returns>Unidades devueltas en buen estado</returns>
        public bool UnidadesDevueltasBuenEstado()
        {
            return ((UnidadesAlmacenBuenEstado > Decimal.Zero || UnidadesDetalleBuenEstado > Decimal.Zero));
        }
        /// <summary>
        /// Existen unidades vendidas y devueltas
        /// </summary>
        /// <returns>unidades desglosadas</returns>
        public bool UnidadesDesglosadas()
        {
            return UnidadesDevueltas() || UnidadesVendidas();
        }

        /// <summary>
        /// Verificar la existencia del articulo asociado al detalle
        /// </summary>
        /// <param name="bodega">bodega a verificar existencia</param>
        /// <returns>existe existencia</returns>
        public bool VerificarExistencia(string bodega)
        {
            //Cantidad total requerida del detalle en unidades de almacén.
            decimal existenciasTotalAlmacenDetalleRequerido = decimal.Zero;

            //Obtenemos las existencias en unidades de almacén del artículo en la bodega de la ruta.
            this.Articulo.CargarExistencia(bodega);

            //Calculamos el faltante requerido del detalle en unidades de almacén.
            existenciasTotalAlmacenDetalleRequerido = TotalAlmacen - TotalAlmacenSaldo;
            //Se verifica si el detalle posee suficientes existencias.
            if (Articulo.Bodega.Existencia < existenciasTotalAlmacenDetalleRequerido)
            {
                TotalAlmacenExistencia = Articulo.Bodega.Existencia;
                return false;
            }
            else
            {
                //Caso: 32597	ABC 21/05/2008
                //Error al sugerir venta en consignación caundo no hay suficientes existencias.
                //Si hay existencias en caso contrario consignar el numero de unidades en almacen
                if (existenciasTotalAlmacenDetalleRequerido > 0)
                    TotalAlmacenExistencia = existenciasTotalAlmacenDetalleRequerido;
                else
                    TotalAlmacenExistencia = TotalAlmacen;
                return true;
            }

        }
        #endregion

        #region Saldos Desglose

        private decimal saldoDesgloseAlmacen = Decimal.Zero;

        public decimal SaldoDesgloseAlmacen
        {
            get
            {
                return saldoDesgloseAlmacen;
            }
            set
            {
                this.saldoDesgloseAlmacen = value;
            }
        }

        private decimal saldoDesgloseDetalle = Decimal.Zero;

        public decimal SaldoDesgloseDetalle
        {
            get
            {
                return saldoDesgloseDetalle;
            }
            set
            {
                this.saldoDesgloseDetalle = value;
            }
        }

        private bool faltaExistencias = false;

        public bool FaltaExistencia
        {
            get
            {
                return this.faltaExistencias;
            }
            set
            {
                this.faltaExistencias = value;
            }
        }

        #endregion
        
        #region BindData

        public string[] OrdenarItems(CriterioArticulo criterio)
        {
            string[] itemes = new string[6];

            itemes[0] = this.Articulo.ObtenerDato(criterio);
            string montoLinea = GestorUtilitario.FormatNumero(MontoTotal);

            switch (criterio)
            {
                case CriterioArticulo.Codigo:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = montoLinea;

                    break;
                case CriterioArticulo.Descripcion:
                    itemes[1] = Articulo.Codigo;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = montoLinea;

                    break;
                case CriterioArticulo.Barras:
                case CriterioArticulo.Familia:
                case CriterioArticulo.Clase:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = Articulo.Codigo;
                    itemes[3] = UnidadesAlmacen.ToString("#0.00");
                    itemes[4] = UnidadesDetalle.ToString("#0.00");
                    itemes[5] = montoLinea;
                    break;

            }

            return itemes;
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

        #endregion
        

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "LINEA_CONSIGNACION";

        }
        public override object GetField(string name)
        {
            this.Articulo.Cargar();
            switch (name)
            {
                //Caso 32567 LDS 20080514 Saldo consignado.
                case "SALDO_ALMACEN": return this.unidadesAlmacenSaldo;
                case "SALDO_DETALLE":return this.unidadesDetalleSaldo;

                //Caso 32567 LDS 20080514 Cantidad a reabastecer
                case "REABASTECER_ALMACEN": return this.UnidadesAlmacen - this.unidadesAlmacenSaldo;
                case "REABASTECER_DETALLE": return this.UnidadesDetalle - this.unidadesDetalleSaldo;

                //Caso 28269 LDS 14/09/2007
                case "EXENTO_IMP_VENTA":
                    if (this.Articulo.Impuesto.Impuesto1 == decimal.Zero)
                        return "E";
                    else
                        return "G";

                case "EXENTO_IMP_CONSUMO":
                    if (this.Articulo.Impuesto.Impuesto2 == decimal.Zero)
                        return "C";
                    else
                        return "";

                default:
                    return base.GetField(name);
            }
        }

        #endregion
    }
}
