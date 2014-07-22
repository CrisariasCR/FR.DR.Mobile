using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using System.Collections;


namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
    /// <summary>
    /// Representa la lista de detalles de pedido asociada a un cliente para una compania
    /// </summary>
    public class DetallePedido : DetalleLinea, IPrintable
    {
        #region Variables y Propiedades de instancia

        #region Bonificaciones

        private string tope = string.Empty;

        public string Tope
        {
            get { return tope; }
            set { tope = value; }
        }

        private DetallePedido lineaBonificada = null;
        /// <summary>
        /// Detalle de la linea bonificada por esta linea de pedido.
        /// </summary>
        public DetallePedido LineaBonificada
        {
            get { return lineaBonificada; }
            set { lineaBonificada = value; }
        }

        private bool esBonificada = false;
        /// <summary>
        /// Variable que indica si la linea es bonificada o no.
        /// </summary>
        public bool EsBonificada
        {
            get { return esBonificada; }
            set { esBonificada = value; }
        }

        private DetallePedido lineaBonificadaAdicional = null;
        /// <summary>
        /// Detalle de la linea bonificada por esta linea de pedido.
        /// </summary>
        public DetallePedido LineaBonificadaAdicional
        {
            get { return lineaBonificadaAdicional; }
            set { lineaBonificadaAdicional = value; }
        }

        private bool esBonificadaAdicional = false;
        /// <summary>
        /// Variable que indica si la linea es bonificada o no.
        /// </summary>
        public bool EsBonificadaAdicional
        {
            get { return esBonificadaAdicional; }
            set { esBonificadaAdicional = value; }
        }

        private List<Lotes> lotesLinea = new List<Lotes>();
        /// <summary>
        /// Lotes Asociados a la factura.
        /// </summary>
        public List<Lotes> LotesLinea
        {
            get { return lotesLinea; }
            set { lotesLinea = value; }
        }

        public string LineaBonifica { get; set; }

        #endregion

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

        #region Precios

        private decimal margenUtilidad = 0;
        /// <summary>
        /// Margen de utilidad de la linea de pedido.
        /// </summary>
        public decimal MargenUtilidad
        {
            get { return margenUtilidad; }
            set { margenUtilidad = value; }
        }
        #endregion

        #region CantidadesPedido

        private decimal cantidadDevuelta = 0;
        /// <summary>
        /// ABC 36136
        /// </summary>
        public decimal CantidadDevuelta
        {
            get { return cantidadDevuelta; }
            set { cantidadDevuelta = value; }
        }

        private decimal unidadBonificadaAlmacen;
        /// <summary>
        /// ABC 36137
        /// </summary>
        public decimal UnidadBonificadaAlmacen
        {
            get { return unidadBonificadaAlmacen; }
            set { unidadBonificadaAlmacen = value; }
        }


        private decimal unidadBonificadaDetalle;
        /// <summary>
        /// ABC 36137
        /// </summary>
        public decimal UnidadBonificadaDetalle
        {
            get { return unidadBonificadaDetalle; }
            set { unidadBonificadaDetalle = value; }
        }

        private decimal cantidadPedida = 0;

        public decimal CantidadPedida
        {
            get { return cantidadPedida; }
            set { cantidadPedida = value; }
        }
        private decimal cantidadFacturada = 0;

        public decimal CantidadFacturada
        {
            get { return cantidadFacturada; }
            set { cantidadFacturada = value; }
        }
        private decimal cantidadGarantia= 0;

        public decimal CantidadGarantia
        {
            get { return cantidadGarantia; }
            set { cantidadGarantia = value; }
        }
        private decimal cantidadCancelada = 0;

        public decimal CantidadCancelada
        {
            get { return cantidadCancelada; }
            set { cantidadCancelada = value; }
        }
        private decimal cantidadBackOrder = 0;

        public decimal CantidadBackOrder
        {
            get { return cantidadBackOrder; }
            set { cantidadBackOrder = value; }
        }
        #endregion

        #region Estado

        private EstadoPedido estado = EstadoPedido.Normal;
        /// <summary>
        /// Indica el estado de la linea.
        /// </summary>
        public EstadoPedido Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        #endregion

        //ABC 36137
        /// <summary>
        /// Total de unidades de datalle Bonificadas.
        /// </summary>
        public decimal TotalBonificado
        {
            get
            {
                return this.totalBonificado();
            }
        }

        private bool desglosoLote = false;
        /// <summary>
        /// Valida si se desgloso el detalle de lotes
        /// </summary>
        public bool DesglosoLote
        {
            get { return desglosoLote; }
            set { desglosoLote = value; }
        }

        #endregion

        #region Constantes

        /// <summary>
        /// Valor que indentifica que una linea de pedido es bonificada.
        /// </summary>
        public const string BONIFICADA = "B";

        /// <summary>
        /// Valor que identifica que una linea no es bonificada.
        /// </summary>
        public const string NOBONIFICADA = "0";

        /// <summary>
        /// Valor que indentifica que una linea de pedido es bonificada.
        /// </summary>
        public const string BONIFICADAADICIONAL = "A";

        #endregion


        #region Logica Impuestos
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
        /// <param name="percepcion">Si hay prcepción</param>
        public void CalcularImpuestos(decimal exoneracionIV, decimal exoneracionIC, decimal porcDescGeneral, bool percepcion)
        {
            try
            {
                this.Articulo.CargarImpuesto();
                
                //PA2-01482-MKP6 - KFC 
                // Se cambia el metodo de redondeo
                Impuesto.MontoImpuesto1 = Decimal.Round(
                    Articulo.Impuesto.ImpuestoVentaLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIV), 5);
                //Impuesto.MontoImpuesto1 = RedondeoAlejandoseDeCero(
                 //   Articulo.Impuesto.ImpuestoVentaLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIV), 2);


                this.DefinirExentoImpuestoVenta(exoneracionIV);

                if (percepcion)
                    Impuesto.MontoImpuesto2 = 0;
                else
                {
                    Impuesto.MontoImpuesto2 = Decimal.Round(
                        Articulo.Impuesto.ImpuestoConsumoLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIC), 5);
                }

                this.DefinirExentoImpuestoConsumo(exoneracionIC);
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculando impuestos de la línea '" + this.Articulo.Codigo + "'." + ex.Message);
            }
        }


        //------------------------------------------------------ pruebas kfc decimales --------------
        public void CalcularImpuestosEXT(decimal exoneracionIV, decimal exoneracionIC, decimal porcDescGeneral, bool percepcion)
        {
            try
            {
                this.Articulo.CargarImpuesto();

                //PA2-01482-MKP6 - KFC 
                // Se cambia el metodo de redondeo
                //Impuesto.MontoImpuesto1 = Decimal.Round(
                //    Articulo.Impuesto.ImpuestoVentaLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIV), 2);
                Impuesto.MontoImpuesto1 = RedondeoAlejandoseDeCero(
                    Articulo.Impuesto.ImpuestoVentaLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIV), 5);


                this.DefinirExentoImpuestoVenta(exoneracionIV);

                if (percepcion)
                    Impuesto.MontoImpuesto2 = 0;
                else
                {
                    Impuesto.MontoImpuesto2 = RedondeoAlejandoseDeCero(
                    Articulo.Impuesto.ImpuestoConsumoLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIC), 2);
                    //Impuesto.MontoImpuesto2 = Decimal.Round(
                    //    Articulo.Impuesto.ImpuestoConsumoLinea(MontoTotal, MontoDescuento, porcDescGeneral, exoneracionIC), 2);
                }

                this.DefinirExentoImpuestoConsumo(exoneracionIC);
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculando impuestos de la línea '" + this.Articulo.Codigo + "'." + ex.Message);
            }
        }


        //-------------------------------------------------------------------------------------------<=<


        /// <summary>
        /// Metodo que calcula los impuestos de la linea.
        /// </summary>
        /// <param name="exoneracionIV">Porcentaje de exoneracion de impuesto 1 del cliente.</param>
        /// <param name="exoneracionIC">Porcentaje de exoneracion de impuesto 2 del cliente.</param>
        /// <param name="porcDescGeneral">Porcentaje Descuento 1 + Porcentaje Descuento 2 del pedido.</param>
        public void CalcularImpuestos(decimal exoneracionIV, decimal exoneracionIC, decimal porcDescGeneral)
        {
            this.CalcularImpuestos(exoneracionIV, exoneracionIC, porcDescGeneral, false);
        }
        #endregion

        #region Metodos publicos de clase
        /// <summary>
        /// Obtiene el costo del detalle según la moneda del pedido.
        /// </summary>
        /// <param name="moneda">Tipo de moneda</param>
        /// <returns>Retorna el monto del costo de la línea.</returns>
        public decimal ObtenerCosto(TipoMoneda moneda)
        {
            try
            {
                if (moneda == TipoMoneda.LOCAL)
                    return Articulo.CostoLocal * TotalAlmacen;
                else
                    return this.Articulo.CostoDolar * TotalAlmacen;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el monto del costo de la línea '" + Articulo.Codigo + "'. " + ex.Message);
            }
        }

        /// <summary>
        /// Carga la bonificacion segun la cantidad pedida.
        /// </summary>
        /// <returns></returns>
        public void CargarBonificacion(Cls.FRCliente.ClienteCia cliente)
        {
            this.LineaBonificada = null;
            this.Articulo.Bonificaciones = Bonificacion.ObtenerBonificaciones(cliente, Articulo.Codigo, UnidadesAlmacen);
            if (this.Articulo.Bonificaciones.Count != 0)
            {
                Bonificacion bonificacion = (Bonificacion)this.Articulo.Bonificaciones[0];

                decimal cantBonAlm, cantBonDet = 0;

                //Verificamos si existe un factor de conversión.
                if (bonificacion.Factor != 0)
                    cantBonAlm = Decimal.Truncate(TotalAlmacen /**/ / bonificacion.Factor) * bonificacion.Cantidad;
                else
                    cantBonAlm = bonificacion.Cantidad;

                if (cantBonAlm > 0)
                {
                    //Validar existencias de las bonificadas cuando se factura
                    //bonificacion.ArticuloBonificado.CargarExistencia()
                    //if (!(Pedidos.FacturarPedido && !bonificacion.ArticuloBonificado.Bodega.SuficienteExistencia(cantBonAlm)))

                    DetallePedido detalle = new DetallePedido();

                    detalle.Articulo = bonificacion.ArticuloBonificado;
                    GestorUtilitario.CalcularCantidadBonificada(cantBonAlm, bonificacion.ArticuloBonificado.UnidadEmpaque, ref cantBonAlm, ref cantBonDet);
                    detalle.UnidadesAlmacen = cantBonAlm;
                    detalle.UnidadesDetalle = cantBonDet;

                    detalle.Precio = bonificacion.ArticuloBonificado.Precio;
                    detalle.EsBonificada = true;

                    this.LineaBonificada = detalle;
                }
            }
        }

        /// <summary>
        /// Carga la bonificacion Adicional segun la cantidad indicada.
        /// </summary>
        /// <returns></returns>
        public void CargarBonificacionAdicional(decimal cantidadAlmacen, decimal cantidadDetalle)//Cls.FRCliente.ClienteCia cliente)
        {
            this.LineaBonificadaAdicional = null;

            DetallePedido detalle = new DetallePedido();

            detalle.Articulo = Articulo;
            detalle.UnidadesAlmacen = cantidadAlmacen;
            detalle.UnidadesDetalle = cantidadDetalle;

            detalle.Precio = Articulo.Precio;
            detalle.EsBonificadaAdicional = true;

            this.LineaBonificadaAdicional = detalle;
        }
        /// <summary>
        /// Realiza el recálculo del detalle.
        /// </summary>
        /// <param name="cantAlm">Cantidad en unidades de almacén del detalle.</param>
        /// <param name="cantDet">Cantidad en unidades de detalle del detalle.</param>
        /// <param name="precioMax">Precio de la unidad de almacén del detalle.</param>
        /// <param name="precioMin">Precio de la unidad de detalle del detalle.</param>
        /// <param name="listaPrecio">Lista de precio asociada al pedido.</param>
        /// <param name="cliente">Cliente asociado al pedido.</param>
        /// <param name="porcDescuentoGeneral">Porcentaje del descuento general del pedido.</param>
        /// <param name="cargarBonificaciones">Indica si se debe cargar y tomar en cuenta las bonificaciones del detalle.</param>
        public void RecalcularDetalle(decimal unidadesAlmacen, decimal unidadesDetalle, Precio precio,
            NivelPrecio nivel, FRCliente.ClienteCia cliente, decimal porcDescuentoGeneral, bool cargarBonificaciones)
        {
            UnidadesAlmacen = unidadesAlmacen;
            UnidadesDetalle = unidadesDetalle;
            Precio = precio;

            //Cargamos el descuento de la línea.
            //LDA caso R0-02209-S00S 
            //Error en los niveles de precios
            cliente.NivelPrecio = nivel;

            Descuento = Descuento.ObtenerDescuento(cliente, this.Articulo, this.TotalAlmacen);
            //CalcularMontoDescuento();

            //Cargamos la bonificación de la línea.
            if (cargarBonificaciones)
                CargarBonificacion(cliente);
            else
                LineaBonificada = null;

            this.CalcularMontoLinea();
            this.CalcularMontoDescuento();

            //Calculamos nuevamente los impuestos.
            this.CalcularImpuestos(cliente.ExoneracionImp1, cliente.ExoneracionImp1, porcDescuentoGeneral);
        }

        public void RecalcularDetalleTomaFisica(decimal unidadesAlmacen, decimal unidadesDetalle, Precio precio,
           NivelPrecio nivel, FRCliente.ClienteCia cliente, decimal porcDescuentoGeneral, bool cargarBonificaciones)
        {
            UnidadesAlmacen = unidadesAlmacen;
            UnidadesDetalle = unidadesDetalle;
            Precio = precio;

            //Cargamos el descuento de la línea.
            //LDA caso R0-02209-S00S 
            //Error en los niveles de precios
            cliente.NivelPrecio = nivel;

            Descuento = Descuento.ObtenerDescuento(cliente, this.Articulo, this.TotalAlmacen);
            //CalcularMontoDescuento();

            //Cargamos la bonificación de la línea.
            //Como es toma fisica no carga bonificaciones
            //if (cargarBonificaciones)
            //    CargarBonificacion(cliente);
            //else
            //    LineaBonificada = null;

            this.CalcularMontoLinea();
            this.CalcularMontoDescuento();

            //Calculamos nuevamente los impuestos.
            this.CalcularImpuestos(cliente.ExoneracionImp1, cliente.ExoneracionImp1, porcDescuentoGeneral);
        }


        /// <summary>
        /// Valida el margen de utilidad del detalle basados en la lista de precios.
        /// </summary>
        /// <param name="moneda"></param>
        /// <remarks>Levanta una FrmExcepcion en caso de no cumplir con el margen de utilidad.</remarks>
        public void ValidarMargenUtilidad(NivelPrecio nivel, decimal descuentoGeneral)
        {
            decimal margenLista = GestorUtilitario.ObtenerPorcentaje(Articulo.CargarMargenUtilidad(nivel));
            decimal margenVenta = GestorUtilitario.ObtenerPorcentaje(this.ObtenerMargenUtilidad(nivel.Moneda, descuentoGeneral));

            if (Decimal.Round(margenVenta, 2) < Decimal.Round(margenLista, 2))
                throw new Exception("La línea '" + this.Articulo.Codigo + "' no cumple con el margen de utilidad definido.");
        }

        /// <summary>
        /// Obtiene el margen de utilidad de la línea de pedido o factura.
        /// </summary>
        /// <param name="moneda">Tipo de moneda.</param>
        /// <param name="descuentoGeneral">Porcentaje del descuento general del pedido o factura.</param>
        /// <returns>Retorna la utilidad.</returns>
        public decimal ObtenerMargenUtilidad(TipoMoneda moneda, decimal descuentoGeneral)
        {
            decimal costo = this.ObtenerCosto(moneda);
            decimal precio = this.MontoTotal;
            decimal utilidad = Decimal.Zero;

            try
            {
                //Quitamos el descuento por línea y el descuento general.
                precio -= this.MontoDescuento - (this.MontoTotal * descuentoGeneral);

                if (precio == 0)
                    utilidad = 1 - (costo / 0.00000001m);
                else
                    utilidad = 1 - (costo / precio);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el margen de utilidad de la línea '" + this.Articulo.Codigo + "'. " + ex.Message);
            }
            return utilidad;
        }

        /// <summary>
        /// Asigna el lote a la línea de la factura
        /// </summary>
        /// <param name="detallelote">Lote Asignar</param>
        public void AgregarLote(Lotes detallelote)
        {
            try
            {
                int index = this.buscarLote(detallelote);
                if (index == -1)
                    this.LotesLinea.Add(detallelote);
                else
                {
                    this.LotesLinea[index].CantidadAlmacen = detallelote.CantidadAlmacen;
                    this.LotesLinea[index].CantidadDetalle = detallelote.CantidadDetalle;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar lote al detalle " + ex.Message);
            }
        }

        public void EliminarLote(Lotes detallelote)
        {
            try
            {
                int index = this.buscarLote(detallelote);

                if (index != -1)
                    this.LotesLinea.Remove(detallelote);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al borrar lote al detalle " + ex.Message);
            }
        }

        /// <summary>
        /// Busca el lote si ya esta asignado
        /// </summary>
        /// <param name="lote">Lote</param>
        /// <returns>Index != -1 No encontro</returns>
        public int buscarLote(Lotes lote)
        {
            int index = -1;
            int contador = 0;

            if (this.LotesLinea.Count > 0)
            {
                foreach (Lotes buscar in this.LotesLinea)
                {
                    if (buscar.Lote == lote.Lote && buscar.FechaVencimiento == lote.FechaVencimiento)
                    {
                        index = contador;
                        break;
                    }
                    contador++;
                }
            }

            return index;
        }
        #endregion Metodos publicos de clase


        #region Metodos de clase
        //ABC 36137
        /// <summary>
        /// Retorna la cantidad total de unidades bonificadas en unidades de detalle
        /// </summary>
        /// <returns></returns>
        private decimal totalBonificado()
        {
            decimal totalBonificado = 0;

            if (this.Articulo.Bonificaciones.Count != 0)
            {
                Bonificacion bonificacion = this.Articulo.Bonificaciones[0];

                decimal cantBonAlm, cantBonDet = 0;

                //Verificamos si existe un factor de conversión.
                if (bonificacion.Factor != 0)
                    cantBonAlm = Decimal.Truncate(this.UnidadesAlmacen / bonificacion.Factor) * bonificacion.Cantidad; //Caso: 27839 LDS 04/04/2007
                else
                    cantBonAlm = bonificacion.Cantidad;

                if (cantBonAlm > 0)
                    GestorUtilitario.CalcularCantidadBonificada(cantBonAlm, bonificacion.ArticuloBonificado.UnidadEmpaque, ref cantBonAlm, ref cantBonDet);

                totalBonificado = (cantBonAlm * bonificacion.ArticuloBonificado.UnidadEmpaque) + cantBonDet;

                this.unidadBonificadaAlmacen = cantBonAlm;
                this.UnidadBonificadaDetalle = cantBonDet;

            }

            return totalBonificado;
        }

        #region Descuentos-Bonificaciones por Paquete

        private LineaRegalia regaliaDescuentoLinea;
        public LineaRegalia RegaliaDescuentoLinea
        {
            get { return regaliaDescuentoLinea; }
            set { regaliaDescuentoLinea = value; }
        }

        private LineaRegalia regaliaDescuentoGeneral;
        public LineaRegalia RegaliaDescuentoGeneral
        {
            get { return regaliaDescuentoGeneral; }
            set { regaliaDescuentoGeneral = value; }
        }

        private LineaRegalia regaliaBonificacion;
        public LineaRegalia RegaliaBonificacion
        {
            get { return regaliaBonificacion; }
            set { regaliaBonificacion = value; }
        }

        #endregion


        #region PA2-01482-MKP6  - KFC

        /// <summary>
        /// Metodo para el redondeo alejandose de cero
        /// Esta funcion se crea debido a que no sirve la sobrecarga de Math.Round  que incluye MidpointRounding
        /// en esta clase. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static decimal RedondeoAlejandoseDeCero(decimal value, int digits)
        {
            return System.Math.Round(value + 
                Convert.ToDecimal(System.Math.Sign(value) / System.Math.Pow(10, digits + 1)), digits);
        }

        #endregion



        #endregion Metodos de clase

        #region BindData

        public string[] OrdenarItems(CriterioArticulo criterio)
        {
            int colum = 6;
            string[] itemes = new string[8];

            itemes[0] = this.Articulo.ObtenerDato(criterio);

            string montoDescuento = GestorUtilitario.FormatNumero(MontoDescuento);
            string montoLinea = GestorUtilitario.FormatNumero(MontoTotal);

            switch (criterio)
            {
                case CriterioArticulo.Codigo:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = montoDescuento;
                    itemes[5] = montoLinea;
                    break;
                case CriterioArticulo.Descripcion:
                    itemes[1] = Articulo.Codigo;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = montoDescuento;
                    itemes[5] = montoLinea;
                    break;
                case CriterioArticulo.Barras:
                case CriterioArticulo.Familia:
                case CriterioArticulo.Clase:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = Articulo.Codigo;
                    itemes[3] = UnidadesAlmacen.ToString("#0.00");
                    itemes[4] = UnidadesDetalle.ToString("#0.00");
                    itemes[5] = montoDescuento;
                    itemes[6] = montoLinea;
                    colum = 7;
                    break;

            }

            if (EsBonificada)
                itemes[colum] = DetallePedido.BONIFICADA;
            else if (EsBonificadaAdicional)
                itemes[colum] = DetallePedido.BONIFICADAADICIONAL;
            else
                itemes[colum] = DetallePedido.NOBONIFICADA;

            if (colum == 6)
                itemes[colum + 1] = "";

            return itemes;
        }
        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "LINEA_PEDIDO";
        }
        public override object GetField(string name)
        {
            switch (name)
            {
                case "BONIFICADA":
                    if (this.EsBonificada)
                        return "*";
                    else
                        return "";
                //Caso 28269 LDS 14/09/2007
                case "EXENTO_IMP_VENTA":
                    if (this.EsExentoImpuesto1)
                        return "E";
                    else
                        return "G";

                case "EXENTO_IMP_CONSUMO":
                    if (this.EsExentoImpuesto2)
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