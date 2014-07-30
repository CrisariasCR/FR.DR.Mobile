using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.ViewModels;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
    public class Garantia : EncabezadoDocumento, IPrintable
    {
        #region Propiedades Globales
        
        #endregion

        #region Variables y Propiedades de instancia


        private DetallesGarantias detalles = new DetallesGarantias();
        /// <summary>
        /// Detalles del Pedido
        /// </summary>
        public DetallesGarantias Detalles
        {
            get { return detalles; }
            set { detalles = value; }
        }

        private decimal montoAPagarDocLocal = 0;
        /// <summary>
        /// monto a pagar por el documento
        /// </summary>
        public decimal MontoAPagarDocLocal
        {
            get { return montoAPagarDocLocal; }
            set { montoAPagarDocLocal = value; }
        }

        private decimal montoAPagarDocDolar = 0;
        /// <summary>
        /// Monto a pagar por el documento
        /// </summary>
        public decimal MontoAPagarDocDolar
        {
            get { return montoAPagarDocDolar; }
            set { montoAPagarDocDolar = value; }
        }

        private decimal saldoDocLocal = 0;
        /// <summary>
        /// monto del saldo
        /// </summary>
        public decimal SaldoDocLocal
        {
            get { return saldoDocLocal; }
            set { saldoDocLocal = value; }
        }

        private decimal saldoDocDolar = 0;
        /// <summary>
        /// monto del saldo
        /// </summary>
        public decimal SaldoDocDolar
        {
            get { return saldoDocDolar; }
            set { saldoDocDolar = value; }
        }

        private List<NotaCredito> notasCreditoAplicadas = new List<NotaCredito>();
        /// <summary>
        /// Notas de credito aplicadas a la factura
        /// </summary>
        public List<NotaCredito> NotasCreditoAplicadas
        {
            get { return notasCreditoAplicadas; }
            set { notasCreditoAplicadas = value; }
        }
        /// <summary>
        /// Retorna el total de notas aplicadas a la factura. El valor devuelto esta en la moneda de la factura.
        /// </summary>
        public decimal TotalNotasCredito
        {

            get
            {
                decimal total_notas = 0;

                foreach (NotaCredito nota in this.NotasCreditoAplicadas)
                {
                    if (this.Moneda == TipoMoneda.LOCAL.ToString())
                        total_notas += nota.MontoDocLocal;
                    else
                        total_notas += nota.MontoDocDolar;
                }
                return total_notas;
            }
        }
        
        #region Fechas

        private DateTime fechaEntrega = DateTime.Now;
        /// <summary>
        /// Variable que indica la fecha prometida para la entrega del pedido
        /// </summary>
        public DateTime FechaEntrega
        {
            get { return fechaEntrega; }
            set { fechaEntrega = value; }
        }

        #endregion

        #region Configuracion Previa a la Toma
        
        private TipoPedido tipo;
        /// <summary>
        /// Tipo de documento generado para el pedido
        /// </summary>
        public TipoPedido Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private ConfigDocCia configuracion;
        /// <summary>
        /// Configuracion de Pago
        /// </summary>
        public ConfigDocCia Configuracion
        {
            get { return configuracion; }
            set { configuracion = value; }
        }

        #region Modificaciones en funcionalidad de generacion de recibos de contado - KFC

        private CondicionPago condicionPago;
        /// <summary>
        /// Configuracion de Pago
        /// </summary>
        public CondicionPago CondicionPago
        {
            get { return condicionPago; }
            set { condicionPago = value; }
        }

        #endregion

        /// <summary>
        /// Porcentaje del descuento 1 expresando como entero (0 - 100)
        /// </summary>
        public override decimal PorcentajeDescuento1
        {
            get { return base.PorcentajeDescuento1; }
            set
            {
                base.PorcentajeDescuento1 = value;
                ReCalcularDescuentosGenerales();
            }
        }

        public override decimal PorcentajeDescuento2
        {
            get { return base.PorcentajeDescuento2; }
            set
            {
                base.PorcentajeDescuento2 = value;
                ReCalcularDescuentosGenerales();
            }
        }
        /// <summary>
        /// Porcentaje de descuento general asignado al pedido.
        /// Porcentaje 1 + Porcentaje 2.
        /// </summary>
        public decimal PorcDescGeneral
        {
            get
            {			
                if (this.DescuentoCascada)
                {
                    if (this.MontoBruto > Decimal.Zero)
                    {
                        decimal Porcentaje = ((this.MontoBruto * (this.PorcentajeDescuento1 / 100)) 
                                           + ((this.MontoBruto - 
                                              (this.MontoBruto * (this.PorcentajeDescuento1 / 100))) 
                                           *  (this.PorcentajeDescuento2 / 100))) / this.MontoBruto;
                        return Porcentaje;
                    }
                    else
                    {
                        //ABC Caso 37017
                        return this.PorcentajeDescuento1 / 100;
                    }
                }
                else
                {
                    return (this.PorcentajeDescuento1 + this.PorcentajeDescuento2) / 100;
                }
            }
        }

        public bool Seleccionado { get; set; }

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

        #region Referencia Consignacion
        
        private bool esConsignacion = false;
        /// <summary>
        /// indica si es consignada la devolución si fue generada por el desglose de una boleta de venta en consignación.
        /// </summary>
        public bool EsConsignacion
        {
            get { return esConsignacion; }
            set { esConsignacion = value; }
        }
        private string numRef = string.Empty;
        /// <summary>
        /// indica numero de consignacion de la devolución si fue generada por el desglose de una boleta de venta en consignación.
        /// </summary>
        public string NumRef
        {
            get { return numRef; }
            set { numRef = value; }
        }
        #endregion

        #region otros

        private string direccionEntrega = string.Empty;
        /// <summary>
        /// Indica la direccion de entrega del pedido
        /// </summary>
        public string DireccionEntrega
        {
            get { return direccionEntrega; }
            set { direccionEntrega = value; }
        }
        
        #endregion

        #region Descuento

        private bool descuentoCascada;
        /// <summary>
        /// Indica si el pedido es realizado con descuento en cascada.
        /// </summary>
        public bool DescuentoCascada
        {
            get { return descuentoCascada; }
            set { descuentoCascada = value; }
        }

        private LineaRegalia regaliaDescuentoGeneral;
        public LineaRegalia RegaliaDescuentoGeneral
        {
            get { return regaliaDescuentoGeneral; }
            set { regaliaDescuentoGeneral = value; }
        }

        #endregion

        //KFC Sincronizacion de Listas de Precios
        private string moneda = string.Empty;
        /// <summary>
        /// Indica la moneda con la que se realiza el pedido
        /// </summary>
        public string Moneda 
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private string nivelPrecioCod = string.Empty;
        /// <summary>
        /// Indica la moneda con la que se realiza el pedido
        /// </summary>
        public string NivelPrecioCod
        {
            get { return nivelPrecioCod; }
            set { nivelPrecioCod = value; }
        }
        #endregion

        #region Logica de Negocios

        public Garantia()
        {
            this.HoraInicio = DateTime.Now;
            this.FechaRealizacion = DateTime.Now;
        }
        /// <summary>
        /// Constructor del pedido en Devolucion con documento
        /// </summary>
        /// <param name="numero">Numero de factura</param>
        /// <param name="compania">Compania de la factura</param>
        /// <param name="cliente">Cliente asociado</param>
        /// <param name="zona">Zona asociada al cliente</param>
        public Garantia(string numero, string compania, string cliente, string zona)
        {
            Numero = numero;
            Compania = compania;
            Cliente = cliente;
            Zona = zona;
            Tipo = TipoPedido.Factura;
            Detalles.Encabezado =this;

        }

        public Garantia(string numero, string compania, string cliente, string zona, string ncf)
        {
            Numero = numero;
            Compania = compania;
            Cliente = cliente;
            Zona = zona;
            Tipo = TipoPedido.Factura;
            Detalles.Encabezado = this;
            NCF = new NCFBase();
            NCF.UltimoValor = ncf;

            
        }

        /// <summary>
        /// Crear un pedido sencillo
        /// </summary>
        /// <param name="articulo"></param>
        /// <param name="zona"></param>
        public Garantia(Articulo articulo, string zona, ConfigDocCia configuracion)
        {
            this.Compania = articulo.Compania;
            this.Zona = zona;
            this.Configuracion = configuracion;
            this.Cliente = this.Configuracion.ClienteCia.Codigo;
            this.DescuentoCascada = this.Configuracion.Compania.DescuentoCascada;
            this.Bodega = articulo.Bodega.Codigo;
            this.DireccionEntrega = this.Configuracion.ClienteCia.DireccionEntregaDefault;
            this.PorcentajeDescuento1 = this.Configuracion.ClienteCia.Descuento;
            this.NivelPrecio = this.Configuracion.Nivel.Codigo;
            this.HoraInicio = DateTime.Now;
            this.FechaRealizacion = DateTime.Now;
            this.HoraFin = DateTime.Now;
            this.Estado = EstadoPedido.Facturado;
            this.Tipo = TipoPedido.Factura;
            this.FechaEntrega = this.FechaRealizacion;

            this.Configuracion.Compania.Cargar();
            //if (this.Configuracion.Compania.UsaNCF && this.Configuracion.ClienteCia.TipoContribuyente != string.Empty)
            //{
            //    NCFUtilitario.obtenerNCF(this.Compania, NCFUtilitario.FACTURA, this.Configuracion.ClienteCia.TipoContribuyente);
            //    this.NCF = NCFUtilitario.consecutivoNCF;
            //}


            //KFC Sincronización listas de Precios
            this.NivelPrecioCod = this.Configuracion.Nivel.Nivel;
            this.Moneda = this.Configuracion.Nivel.Moneda.Equals(TipoMoneda.LOCAL) ? "L" : "D";
        }
        /// <summary>
        /// Retornar un Pedido en Consignacion
        /// </summary>
        public static Garantia Consignado(string consignacion, Articulo articulo, string zona, ConfigDocCia configuracion, bool descuentoCascada,
            decimal porcDesc1, decimal porcDesc2, string nota)
        {
            //Se asigna la configuración de la boleta de venta en consignación a la factura
            //que se va a generar por el desglose.
            Garantia pedido = new Garantia(articulo, zona, configuracion);
            
            //Se aplica el descuento en cascada en la factura siempre y cuando se indique en la boleta
            //de venta en consignación que se debe aplicar el descuento en cascada.
            pedido.DescuentoCascada = descuentoCascada;

            //Asignamos el porcentaje de descuento 1 definido en la venta en consignación.
            //Si no está definido en la venta en consignación entonces asignamos el descuento general del cliente.
            if (porcDesc1 > decimal.Zero)
                pedido.PorcentajeDescuento1  = porcDesc1;

            //Asignamos el porcentaje de descuento 2 definido en la venta en consignación.
            pedido.PorcentajeDescuento2 = porcDesc2;
   
            //Se asigna el código de la bodega de venta en consignación del cliente
            pedido.Bodega = configuracion.ClienteCia.Bodega;

            //Se indica que la factura está siendo generada por el desglose de una boleta de venta en consignación.
            pedido.EsConsignacion = true;
            pedido.NumRef = consignacion;

            //Se asigna la nota a la factura que se está gestionando.
            pedido.Notas = nota;

            return pedido;
        }

        //Caso 28087 LDS 04/05/2007
        /// <summary>
        /// Metodo encargado de agregar una linea de detalle al pedido.
        /// </summary>
        /// <param name="articulo"></param>
        /// <param name="precioMax">
        /// Es el precio de almacén del artículo, se puede cambiar durante la toma del pedido o factura.
        /// </param>
        /// <param name="precioMin">
        /// Es el precio de detalle del artículo, se puede cambiar durante la toma del pedido o factura.
        /// </param>
        /// <param name="cantidadDetalle"></param>
        /// <param name="cantidadAlmacen"></param>
        /// <param name="validarCantidadLineas"></param>
        public void AgregarLineaDetalle(Articulo articulo,Precio precio,
            decimal unidadesDetalle, decimal unidadesAlmacen,
            bool validarCantidadLineas, decimal unidadAlmacenAdicional, decimal unidadDetalleAdicional,string tope)
        {
            DetalleGarantia detalle = Detalles.Buscar(articulo.Codigo);

            if (detalle == null)
            {
                //Se crea un nuevo detalle si alguna cantidad es mayor a cero.
                if (unidadesAlmacen == 0 && unidadesDetalle == 0)
                    return;

                if (validarCantidadLineas && this.Detalles.Lista.Count == Garantias.MaximoLineasDetalle)
                        throw new Exception("Se alcanzó el máximo de líneas permitidas (" + Garantias.MaximoLineasDetalle + ")");

                detalle = new DetalleGarantia();
                detalle.Articulo = articulo;
                //Cesar Iglesias
                detalle.Tope = tope;
                detalle.Articulo.Bodega.Codigo = this.Bodega;
                //No debe cargar Bonificaciones ni descuentos.
                detalle.RecalcularDetalle(unidadesAlmacen,unidadesDetalle,
                           precio, configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral,false);
                
                //Validamos el margen de utilidad, se levanta frmExcepcion si no cumple.
                if (Garantias.CambiarPrecio)
                    //detalle.ValidarMargenUtilidad(configuracion.Nivel, this.PorcDescGeneral);
                    detalle.ValidarMargenUtilidad(configuracion.Nivel, 0);

                if (unidadAlmacenAdicional > 0 || unidadDetalleAdicional > 0)
                    detalle.CargarBonificacionAdicional(unidadAlmacenAdicional, unidadDetalleAdicional);
                
                this.Detalles.Lista.Add(detalle);
                this.AgregarMontosLinea(detalle);
                //Actualizamos los montos de descuentos 1 y 2.
                this.ReCalcularDescuentosGenerales();
            }
            else
            {
                detalle.Tope = tope;
                //El detalle del pedido ya se encuentra en el pedido por lo que se procede a modificarlo.
                if (detalle.UnidadesAlmacen != unidadesAlmacen ||detalle.UnidadesDetalle != unidadesDetalle ||
                    detalle.Precio.Maximo != precio.Maximo || detalle.Precio.Minimo != precio.Minimo)                
                {
                    //Restamos los montos actuales de la línea.
                    this.RestarMontosLinea(detalle);

                    //Se desea cambiar la cantidad o los precios de la línea del pedido por lo que 
                    //guardamos los valores del detalle para este caso.
                    Precio precioAnt = detalle.Precio;
                    decimal undsAlmAnt = detalle.UnidadesAlmacen;
                    decimal undsDetAnt = detalle.UnidadesDetalle;
                    //No debe cargar Bonificaciones ni descuentos.
                    detalle.RecalcularDetalle(unidadesAlmacen, unidadesDetalle,
                               precio, configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral, false);
                    try
                    {
                        //Validamos el margen de utilidad, se levanta frmExcepcion si no cumple.
                        if (Garantias.CambiarPrecio)
                            detalle.ValidarMargenUtilidad(configuracion.Nivel, this.PorcDescGeneral);
                    }
                    catch (Exception ex)
                    {
                        detalle.RecalcularDetalle(undsAlmAnt,undsDetAnt,precioAnt,
                            configuracion.Nivel,configuracion.ClienteCia, PorcDescGeneral,false);
                        throw ex;
                    }
                    finally
                    {
                        this.AgregarMontosLinea(detalle);
                        //Actualizamos los montos de descuentos 1 y 2.
                        this.ReCalcularDescuentosGenerales();
                    }
                }

                if (detalle.LineaBonificadaAdicional != null &&
                    (detalle.LineaBonificadaAdicional.UnidadesAlmacen != unidadAlmacenAdicional ||
                     detalle.LineaBonificadaAdicional.UnidadesDetalle != unidadDetalleAdicional))
                {
                    detalle.LineaBonificadaAdicional.UnidadesAlmacen = unidadAlmacenAdicional;
                    detalle.LineaBonificadaAdicional.UnidadesDetalle = unidadDetalleAdicional;
                }
            }
        }

        public void AgregarLineaDetalle(Articulo articulo, Precio precio,
            decimal unidadesDetalle, decimal unidadesAlmacen,
            bool validarCantidadLineas, string tope)
        {
           AgregarLineaDetalle(articulo, precio, unidadesDetalle, unidadesAlmacen, validarCantidadLineas, 0, 0, tope);        
        }

        /// <summary>
        /// Elimina un detalle del pedido.
        /// </summary>
        /// <param name="codArt">Código del articulo que se debe eliminar.</param>
        /// <returns>Retorna la cantidad de lineas eliminadas</returns>
        public int EliminarDetalle(string articulo)
        {
            DetalleGarantia detalleEncontrado = detalles.Eliminar(articulo);

            //Hay que restar los montos de la línea.
            this.RestarMontosLinea(detalleEncontrado);
            //Actualizamos los montos de descuentos 1 y 2.
            this.ReCalcularDescuentosGenerales();

            //Debemos revisar si la línea tiene artículos bonificados.
            if (detalleEncontrado.LineaBonificada != null)
                return 2;
            else return 1;
        }
        /// <summary>
        /// Elimina un detalle bonificado de una factura gestionada por el desglose de una boleta de venta en consignación.
        /// </summary>
        /// <param name="codigoArticulo">Código del artículo que posee el artículo bonificado que se desea quitar de la factura generada por el desglose de una boleta de venta en consignación.</param>
        /// <returns>Inidca la cantidad de líneas bonificadas que se quitaron de la factura gestionada.</returns>
        public int EliminarDetalleBonificado(string articulo)
        {
            int posicionDetalle = detalles.BuscarPos(articulo);
            DetalleGarantia detalleEncontrado = detalles.Lista[posicionDetalle];
            //Debemos revisar si la línea tiene artículos bonificados.
            if (detalleEncontrado.LineaBonificada != null)
            {
                if (this.GestionarRetiroDetalleBonificado(ref detalleEncontrado))
                {
                    detalles.Lista[posicionDetalle] = detalleEncontrado;
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// Gestiona el retiro de un detalle bonificado de la factura generada por el desglose de una boleta de venta
        /// en consignación.
        /// </summary>
        /// <param name="detalle">Detalle que contiene el detalle bonificado que se desea gestionar el retiro de la factura generada por el desglose de una boleta de venta en consignación.</param>
        /// <returns>Indica que se realizó el retiro del detalle bonificado.</returns>
        private bool GestionarRetiroDetalleBonificado(ref DetalleGarantia detalle)
        {
            bool detalleRetirado = false;
            try
            {
                //Restamos los montos actuales de la línea.
                this.RestarMontosLinea(detalle);
                //Se recalcula el detalle y no se carga ni se toma en cuenta las bonificaciones del detalle.
                detalle.RecalcularDetalle(detalle.UnidadesAlmacen,detalle.UnidadesDetalle,
                    detalle.Precio,
                    configuracion.Nivel, configuracion.ClienteCia, this.PorcDescGeneral, false);
                //Sumamos los montos actuales de la línea.
                this.AgregarMontosLinea(detalle);
                //Actualizamos los montos de descuentos 1 y 2.
                this.ReCalcularDescuentosGenerales();

                detalleRetirado = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al gestionar el retiro del detalle bonificado. " + ex.Message);
            }
            return detalleRetirado;
        }

        public bool UsuarioCambioDescuentos()
        {
            if (this.PorcentajeDescuento2 != 0)
                return true;

            if (Configuracion.ClienteCia.Descuento != this.PorcentajeDescuento1)
                return true;

            return false;
        }
        public void RecalcularImpuestos(bool percepcion)
        {
            Impuesto.MontoImpuesto1 = 0;
            Impuesto.MontoImpuesto2 = 0;
           
            //Recorremos los detalles del pedido
            foreach (DetalleGarantia detalle in this.Detalles.Lista)
            {   
                // PA2-01482-MKP6 - KFC
                //detalle.CalcularImpuestos(this.Configuracion.ClienteCia.ExoneracionImp1, this.Configuracion.ClienteCia.ExoneracionImp2, this.PorcDescGeneral, percepcion);
                detalle.CalcularImpuestos(this.Configuracion.ClienteCia.ExoneracionImp1, this.Configuracion.ClienteCia.ExoneracionImp2, this.PorcDescGeneral, percepcion);
                //this.Impuesto.MontoImpuesto1 += detalle.Impuesto.MontoImpuesto1;
                //this.Impuesto.MontoImpuesto2 += detalle.Impuesto.MontoImpuesto2;
                this.Impuesto.MontoImpuesto1 = 0;
                this.Impuesto.MontoImpuesto2 = 0;
            }
        }


        // PA2-01482-MKP6 - KFC
        public static decimal RedondeoAlejandoseDeCero(decimal value, int digits)
        {
            return System.Math.Round(value +
                Convert.ToDecimal(System.Math.Sign(value) / System.Math.Pow(10, digits + 1)), digits);
        }

        public void RecalcularImpuestos()
        {
            this.RecalcularImpuestos(false);
        }
        /// <summary>
        /// Obtiene el costo total del pedido.
        /// </summary>
        /// <returns></returns>
        public decimal ObtenerCostoTotal()
        {
            decimal costoTotal = 0;
            foreach (DetalleGarantia detalle in this.Detalles.Lista)
                costoTotal += detalle.ObtenerCosto(Configuracion.Nivel.Moneda);
            return costoTotal;
        }
        /// <summary>
        /// Recalcula los montos de descuentos 1 y 2 basados en los porcentajes de los mismos,
        /// el monto bruto del pedido y el total de descuentos por línea.
        /// </summary>
        public void ReCalcularDescuentosGenerales()
        {
            try
            {
                //Ajustamos el monto del descuento 1
                this.MontoDescuento1 = Decimal.Round((MontoBruto - MontoDescuentoLineas) * PorcentajeDescuento1 / 100, 2);

                // En caso que el parámetro "Aplicación de Descuentos generales 1 y 2 en cascada" de la compañía 
                // esté asignado el descuento se deba aplicar en cascada, de lo contrario se aplica normalmente.
                if (this.DescuentoCascada)
                    this.MontoDescuento2 = Decimal.Round(((MontoBruto - MontoDescuentoLineas) - this.MontoDescuento1) * PorcentajeDescuento2 / 100, 2);
                else
                    this.MontoDescuento2 = Decimal.Round((MontoBruto - MontoDescuentoLineas) * PorcentajeDescuento2 / 100, 2);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al calcular los descuentos generales. " + ex.Message);
            }
        }
        /// <summary>
        /// Realiza los ajustes al pedido cuando se cambia las cantidades o los precios a una linea de detalle:
        /// - Suma los nuevos montos de la linea al pedido.Se asume que los montos anteriores fueron restados.
        /// - Carga o modifica la bonificacion de la linea, si es que hay.
        /// </summary>
        /// <param name="detalle"></param>
        public void AgregarMontosLinea(DetalleGarantia detalle)
        {
            //Actualizamos los montos.
            this.MontoDescuentoLineas += detalle.MontoDescuento;
            this.MontoBruto += detalle.MontoTotal;
            this.Impuesto.MontoImpuesto1 += detalle.Impuesto.MontoImpuesto1;
            this.Impuesto.MontoImpuesto2 += detalle.Impuesto.MontoImpuesto2;
        }
        /// <summary>
        /// Resta a los montos del pedido los montos de una línea de detalle.
        /// </summary>
        /// <param name="detalle"></param>
        public void RestarMontosLinea(DetalleGarantia detalle)
        {
            this.MontoDescuentoLineas -= detalle.MontoDescuento;
            this.MontoBruto -= detalle.MontoTotal;
            this.Impuesto.MontoImpuesto1 -= detalle.Impuesto.MontoImpuesto1;
            this.Impuesto.MontoImpuesto2 -= detalle.Impuesto.MontoImpuesto2;
        }

        #endregion

        #region Metodos de Instancia 

        public void Actualizar(bool commit,string numPed)
        {
            if (detalles.Lista.Count == 0)
                return;
            try
            {
                GestorDatos.BeginTransaction();
                DBEliminar();
                DBGuardar(numPed);
                if (commit)
                    GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al Actualizar Pedido " + ex.Message);
            }
        }

        public void Guardar(bool commit,string pedido)
        {
            try
            {
                GestorDatos.BeginTransaction();

                //Garantias no usan NCF
                //if (this.Tipo == TipoPedido.Factura && this.Configuracion.Compania.UsaNCF && this.configuracion.ClienteCia.TipoContribuyente != string.Empty)
                  //  NCF.IncrementarConsecutivoNCF();

                DBGuardar(pedido);
                ////Se guarda el cobro con un recibo de contado
                //Recibo recibo=Cobros.AplicarPagoContadoGarantia(this, Compania, GlobalUI.ClienteActual);
                //GuardarRecibo(recibo);
                //try
                //{
                //    if (!Cobros.CambiarNumeroRecibo)
                //        ParametroSistema.IncrementarRecibo(this.Compania, this.Zona);
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception("Error actualizando consecutivos. " + ex.Message);
                //}

                ParametroSistema.IncrementarGarantia(Compania, Zona);

                if (commit)
                    GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al guardar " + (this.Tipo == TipoPedido.Factura ? "Garantía. " : "Pedido. ") + ex.Message);
            }
        }



        /// <summary>
        /// Modificaciones en funcionalidad de recibos de contado - KFC
        /// Metodo para anular Facturas
        /// </summary>
        /// <param name="commit"></param>
        /// <param name="ruta"></param>
        public void Anular(bool commit, string ruta, BaseViewModel viewModel)
        {
            try
            {
                GestorDatos.BeginTransaction();
                
                //Si es factura y hay aplicaciones no se puede anular
                Factura factura = ValidarPendientesCobro();
                
                DBAnular();

                if (this.Tipo == TipoPedido.Factura)
                {
                    EliminarPendientesCobro(factura);
                    //Rebaja existencias en los detalles
                    detalles.Anular(this.esConsignacion);
                }

                if (commit)
                    GestorDatos.CommitTransaction();

                if (factura != null)
                {
                    if (factura.Moneda == TipoMoneda.LOCAL)
                        ActualizarJornada(ruta, factura.MontoDocLocal, factura.Moneda, viewModel);
                    if (factura.Moneda == TipoMoneda.DOLAR)
                        ActualizarJornada(ruta, factura.MontoDocDolar, factura.Moneda, viewModel);
                }
            }
            catch (Exception ex)
            {
                if (commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al Anular " + (this.Tipo == TipoPedido.Factura ? "Garantía. " : "Pedido. ") + ex.Message);
            }
        }
         

        private void GenerarPendientesCobro()
        {
            int diasCredito = CondicionPago.ObtenerDiasNeto(Configuracion.CondicionPago.Codigo, Compania);

            // Recibos y Facturas de Contado - KFC 
            // se quita una validacion, siempre se van a hacer recibos para facturas de contado
            //if ((Tipo == TipoPedido.Factura && (diasCredito != 0 || FRdConfig.ReciboFacturasContado)) || esConsignacion)
            //if ((Tipo == TipoPedido.Factura && (diasCredito != 0)) || esConsignacion)
            if ((Tipo == TipoPedido.Factura && (diasCredito != 0)) || esConsignacion)
            {
                try
                {
                    //Debemos crear el pendiente de cobro
                    Factura factura = CrearFacturaPendiente(diasCredito);
                    factura.Guardar();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error generando pendiente de cobro. " + ex.Message);
                }
            }
        }


        private Factura ValidarPendientesCobro()
        {
            int diasCredito = CondicionPago.ObtenerDiasNeto(Configuracion.CondicionPago.Codigo, Compania);
            Factura factura = null;

            if (this.Tipo == TipoPedido.Factura)
            {
                bool aplicacionesRealizadas = false;
                try
                {
                    //Pendiente de cobro asociado a la factura.
                    factura = CrearFacturaPendiente(diasCredito);
                    aplicacionesRealizadas = factura.TieneAplicaciones();

                }
                catch (Exception ex)
                {
                    throw new Exception("Error verificando si existen aplicaciones realizadas. " + ex.Message);
                }

                if (aplicacionesRealizadas)
                {
                    throw new Exception("La factura tiene aplicaciones realizadas. No se puede anular");
                }
            }
            return factura;    
        }


        private void EliminarPendientesCobro(Factura factura)
        {
            int diasCredito = CondicionPago.ObtenerDiasNeto(Configuracion.CondicionPago.Codigo, Compania);

            // Recibos y Facturas de Contado - KFC   
            // se quita una validacion, siempre se van a hacer recibos para facturas de contado
            //if ((diasCredito != 0 || FRdConfig.ReciboFacturasContado) || esConsignacion)
            if ((diasCredito != 0) || esConsignacion) 
            {
                try
                {
                    factura.Eliminar();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error eliminando el pendiente de cobro. " + ex.Message);
                }
            }
        }
        /// <summary>
        /// Crea una factura pendiente de cobro con la informacion de la factura.
        /// </summary>
        /// <returns></returns>
        private Factura CrearFacturaPendiente(int diasCredito)
        {
            Compania cia = Corporativo.Compania.Obtener(Compania);

            Factura factura = new Factura();
            factura.Compania = Compania;
            factura.Numero = Numero;
            factura.Cliente = Cliente;
            factura.FechaRealizacion = DateTime.Now;
            factura.FechaUltimoProceso = DateTime.Now;
            factura.FechaVencimiento = DateTime.Now.AddDays(diasCredito);
            factura.Zona = Zona;
            factura.Estado = EstadoDocumento.Activo;
            //factura.Moneda = TipoMoneda.LOCAL;
            factura.Moneda = Configuracion.Nivel.Moneda;
            factura.TipoCambio = cia.TipoCambio;
            if (factura.Moneda == TipoMoneda.LOCAL)
            {
                // PA2-01482-MKP6 - KFC
                factura.MontoDocLocal = Decimal.Round(MontoNeto, 5); 
                factura.MontoDocDolar = factura.LocalADolar();
            }
            else
            {
                factura.MontoDocDolar = Decimal.Round(MontoNeto, 2);
                factura.MontoDocLocal = factura.DolarALocal();
            }

            factura.SaldoDocLocal = factura.MontoDocLocal;

            factura.SaldoDocDolar = factura.MontoDocDolar;

            if (Configuracion.CondicionPago.Codigo == cia.CondicionPagoContado)
                factura.Tipo = TipoDocumento.Garantia;
            factura.CondicionPago = Configuracion.CondicionPago.Codigo;
            return factura;
        }

        #region Manipulacion Detalles

        public void ObtenerDetalles()
        {
            detalles.Encabezado.Compania = this.Compania;
            detalles.Encabezado.Numero = this.Numero;
            detalles.Encabezado.Zona = this.Zona;
            detalles.NivelPrecio = NivelPrecio;
            detalles.Bodega = this.Bodega;
            detalles.Obtener(configuracion.ClienteCia,PorcDescGeneral);
        }

        #endregion

        #endregion

        #region Acceso Datos

        public static List<Garantia> ObtenerVentas(Cliente cliente, string zona)
        {
            List<Garantia> ventas = new List<Garantia>();
            ventas.Clear();

            string sentencia =
                " SELECT COD_CIA,NUM_GAR,NUM_PED,COD_CLT,TIP_DOC,MON_TOT,FEC_GAR,COD_ZON,"+
                " NUM_ITM,MONEDA,LST_PRE,OBS_GAR,COD_BOD,COD_PAIS,IMPRESO,NIVEL_PRECIO,COD_CND"+
                " FROM " + Table.ERPADMIN_alFAC_ENC_GARANTIA  + 
                " WHERE COD_CLT = @CLIENTE" +
                " AND   COD_ZON = @ZONA" +
                " AND   DOC_PRO IS NULL ";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList( new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente.Codigo),
                      new SQLiteParameter("@ZONA", zona)}); 
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Garantia garantia = new Garantia();
                    garantia.Cliente = cliente.Codigo;
                    garantia.Zona = zona;
                    garantia.Compania = reader.GetString(0);
                    garantia.Numero = reader.GetString(1);
                    if(reader.GetValue(2) != DBNull.Value)
                        garantia.PedidoGarantia = reader.GetString(2);
                    garantia.MontoBruto = reader.GetDecimal(5);
                    garantia.NivelPrecio = reader.GetInt32(10);
                    garantia.FechaRealizacion = reader.GetDateTime(6);
                    garantia.Configuracion = new ConfigDocCia();
                    garantia.Configuracion.Compania.Codigo = garantia.Compania;
                    garantia.Configuracion.Pais.Codigo = reader.GetString(13);
                    garantia.Configuracion.CondicionPago.Codigo = reader.GetString(16);
                    garantia.Configuracion.Nivel.Codigo = garantia.NivelPrecio;
                    garantia.Configuracion.ClienteCia = cliente.ObtenerClienteCia(garantia.Compania);

                    garantia.Bodega = reader.GetString(12);
                    if (!reader.IsDBNull(11))
                        garantia.Notas = reader.GetString(11);
                    garantia.Impreso = reader.GetString(14).Equals("S");

                    garantia.Configuracion.Cargar();

                    ventas.Add(garantia);
                }
                return ventas;
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Error cargando Garantias del cliente. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        #region Insert de las aplicaciones de las notas a las facturas en la BD

        /// <summary>
        /// Guarda en la base de datos la informacion que se genero de 
        /// las facturas que se vieron afectadas por las notas de credito (movimientos de notas)
        /// </summary>
        public void GuardarAfectadaNC(string recibo)
        {
            if (NotasCreditoAplicadas.Count > 0)
            {
                foreach (NotaCredito notaAplicada in NotasCreditoAplicadas)
                {
                    string sentencia =
                        " INSERT INTO " + Table.ERPADMIN_alCXC_MOV_DIR +
                        "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO,    MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) " +
                        " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC,date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                    GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)notaAplicada.Tipo).ToString()),
                    GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                    GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo),
                    GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, notaAplicada.Numero),
                    GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                    GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, Numero),
                    GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                    GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()),
                    GestorDatos.SQLiteParameter("@FEC_DOC",SqlDbType.DateTime, FechaRealizacion),                
                    // mvega: se usa la funcion date('now','localtime') de SQLite
                    //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
                    //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                    GestorDatos.SQLiteParameter("@MON_MOV_LOCAL",SqlDbType.Decimal,Decimal.Round(notaAplicada.MontoDocLocal,2)),
                    GestorDatos.SQLiteParameter("@MON_MOV_DOL",SqlDbType.Decimal,Decimal.Round(notaAplicada.MontoDocDolar,2)),
                    GestorDatos.SQLiteParameter("@MON_SAL_LOC",SqlDbType.Decimal,Decimal.Round(notaAplicada.SaldoDocLocal,2)),
                    GestorDatos.SQLiteParameter("@MON_SAL_DOL",SqlDbType.Decimal,Decimal.Round(notaAplicada.SaldoDocDolar,2))});

                    GestorDatos.EjecutarComando(sentencia, parametros);
                }

            }
        }
        #endregion

        private void DBGuardar(string pedido)
        {
            this.HoraFin = DateTime.Now;

            // PA2-01482-MKP6 - KFC
            // decimal montoCIV = Decimal.Round(this.MontoBruto + this.Impuesto.MontoImpuesto1, 2);
            decimal montoCIV = Decimal.Round(this.MontoBruto + this.Impuesto.MontoImpuesto1, 5);
            
            //Se procede a guardar el encabezado
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_ENC_GARANTIA +
                "        ( COD_CIA,  COD_ZON,  COD_CLT,NUM_GAR,NUM_PED,FEC_GAR,NUM_ITM,COD_BOD,LST_PRE, MON_TOT, IMPRESO, OBS_GAR,COD_PAIS, NIVEL_PRECIO, MONEDA,TIP_DOC,COD_CND) " +
                " VALUES (@COD_CIA, @COD_ZON, @COD_CLT,@NUM_GAR, @NUM_PED,@FEC_GAR,@NUM_ITM, @COD_BOD,@NVL_PRE,@MON_TOT, @IMPRESO,@OBS_GAR,@COD_PAIS,@NVL_PRE_COD, @MONEDA,@TIPDOC,@COD_CND)";  //DOC_PRO 

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_GAR",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@NUM_PED",SqlDbType.NVarChar, pedido),//TODO
                GestorDatos.SQLiteParameter("@FEC_GAR",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.TotalLineas),
                GestorDatos.SQLiteParameter("@COD_BOD",SqlDbType.NVarChar, (EsConsignacion ? Configuracion.ClienteCia.Bodega :this.Bodega)),
                GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int, Configuracion.Nivel.Codigo),
                GestorDatos.SQLiteParameter("@MON_TOT",SqlDbType.Decimal, MontoNeto),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, (Impreso? "S" : "N")),
                GestorDatos.SQLiteParameter("@OBS_GAR",SqlDbType.NVarChar, Notas),
                GestorDatos.SQLiteParameter("@COD_PAIS", SqlDbType.NVarChar, Configuracion.Pais.Codigo),

                //KFC Sincronización Listas de precio
                GestorDatos.SQLiteParameter("@NVL_PRE_COD",SqlDbType.NVarChar, NivelPrecioCod),
                GestorDatos.SQLiteParameter("@MONEDA",SqlDbType.NVarChar, Moneda),
                GestorDatos.SQLiteParameter("@TIPDOC",SqlDbType.NVarChar, (string.IsNullOrEmpty(pedido)? "C" : "E")),
                GestorDatos.SQLiteParameter("@COD_CND", SqlDbType.NVarChar, Configuracion.CondicionPago.Codigo)});

                //GestorDatos.SQLiteParameter("@NUM_REF",SqlDbType.NVarChar, numRef),
            int encabezado = GestorDatos.EjecutarComando(sentencia, parametros);

            if (encabezado != 1)
                throw new Exception("No se puede guardar el encabezado " +(Tipo.Equals(TipoPedido.Factura)? "de la Garantía '": "del pedido'" ) + this.Numero + "'.");

            //se guardan los detalles de Pedido
            detalles.Bodega = Bodega;
            detalles.NivelPrecio = Configuracion.Nivel.Codigo;
            detalles.Tipo = Tipo;
            detalles.Encabezado = this;
            detalles.Guardar();

           // GenerarPendientesCobro();
        }

        private void DBEliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_ENC_PED +
                " WHERE COD_CIA = @COD_CIA" +
                " AND COD_ZON = @COD_ZON " +
                " AND NUM_PED = @NUM_PED";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_PED", SqlDbType.NVarChar, Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows != 1)
                throw new Exception((rows < 1) ? "No se eliminó ningún registro de Pedido." : "Se eliminó más de un registro de Pedido.");

            detalles.Eliminar();
        }

        private void DBAnular()
        {
            this.Estado = EstadoPedido.Cancelado;

            string sentencia =
                " UPDATE " + Table.ERPADMIN_alFAC_ENC_PED +
                " SET ESTADO = @ESTADO " +
                " WHERE COD_CIA = @COD_CIA " +
                " AND COD_ZON = @COD_ZON " +
                " AND NUM_PED = @NUM_PED " +
                " AND DOC_PRO IS NULL";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_PED", SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar, ((char)Estado).ToString())});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows != 1)
                throw new Exception((rows < 1) ? "No se anuló ningún registro de Pedido." : "Se anuló más de un registro de Pedido.");
        }

        public void DBActualizarImpresion()
        {
            string sentencia =
                " UPDATE " + Table.ERPADMIN_alFAC_ENC_PED  +
                " SET IMPRESO = @IMPRESO " +
                " WHERE COD_ZON = @COD_ZON " +
                " AND NUM_PED = @NUM_PED ";
                //" AND DOC_PRO IS NULL";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_PED", SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@IMPRESO", SqlDbType.NVarChar, (Impreso? "S" : "N"))});

            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando el indicador de impresión. " + ex.Message);
            }
        }
        
        /// <summary>
        /// Valida si existe el consecutivo en un pedido ya realizado.
        /// Si esto pasa se aumenta el consecutivo hasta que ya no se repita.
        /// </summary>
        /// <param name="consecutivo"></param>
        /// <param name="compania"></param>
        /// <returns>Consecutivo valido.</returns>
        /// ABC 09/10/2008 Caso:33376A
        /// Se ingresa la compania como parametro para la validación del cosecutivo siguiente
        public string ValidarExistenciaConsecutivo(string consecutivo)
        {
            int existe = 0;

            do
            {
                //ABC 09/10/2008 Caso:33376A
                //Se incluye la compañia como nueva condicion en la validación del consecutivo.
                string sentencia =
                    " SELECT COUNT(NUM_PED) FROM " + Table.ERPADMIN_alFAC_ENC_GARANTIA +
                    " WHERE NUM_GAR = @CONSECUTIVO " +
                    " AND UPPER(COD_CIA) = @COMPANIA";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar,consecutivo),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Compania.ToUpper())});
                existe = (int)GestorDatos.EjecutarScalar(sentencia, parametros);

                if (existe != 0)
                {
                    Bitacora.Escribir("El consecutivo de garantia '" + consecutivo + "' ya existe.");
                    consecutivo = GestorUtilitario.proximoCodigo(consecutivo, 20);
                }

            } while (existe != 0);

            return consecutivo;
        }

        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_GARANTIA + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);     
        }

        /// <summary>
        ///  Carga de Historicos de los Garantias
        /// </summary>
        public static ListaEncabezadoPedido ObtenerHistoricosPedido(string cliente, string zona)
        {
            ListaEncabezadoPedido historicos = new ListaEncabezadoPedido();
            //ABC Caso 37189
            //Se deja de condicionar por ruta, ya que el cliente puede pertenecer a varias rutas distintas
            string sentencia =
                " SELECT COD_CIA,NUM_PED,EST_PED,MON_PED,FEC_PED " +
                " FROM " + Table.ERPADMIN_alFAC_ENC_HIST_PED +
                " WHERE COD_CLT = @CLIENTE " +
                "ORDER BY FEC_PED ASC";


            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente)});

            //new SQLiteParameter("@ZONA", zona)
            //" AND   COD_ZON = @ZONA " +
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Pedido ped = new Pedido();

                    if (!reader.IsDBNull(0))
                        ped.Compania = reader.GetString(0);

                    if (!reader.IsDBNull(1))
                        ped.Numero = reader.GetString(1);

                    ped.Estado = (EstadoPedido)Convert.ToChar(reader.GetString(2));


                    if (!reader.IsDBNull(3))
                        ped.MontoBruto = reader.GetDecimal(3);

                    if (!reader.IsDBNull(4))
                        ped.FechaRealizacion = reader.GetDateTime(4);

                    historicos.AgregaEncabezadoPedido(ped);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar los historicos de los Garantias. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return historicos;
        }

        //ABC Caso 35136
        /// <summary>
        /// Carga las facturas historicas
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        public static List<Pedido> ObtenerHistoricoFactura(string cliente, string zona, string compania)
        {
            List<Pedido> lista = new List<Pedido>();

            //ABC Caso 37189
            //Se deja de condicionar por ruta, ya que el cliente puede pertenecer a varias rutas distintas
            string sentencia =
                "SELECT NUM_FAC,FEC_FAC,MON_FAC, NCF, COD_CIA,COD_CND FROM " + Table.ERPADMIN_alFAC_ENC_HIST_FAC +
                " WHERE UPPER(COD_CLT) = @CLIENTE" +

                " AND   UPPER(COD_CIA) = @COMPANIA " +
                " ORDER BY FEC_FAC ASC";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente.ToUpper()),

                      new SQLiteParameter("@COMPANIA", compania.ToUpper())});

            //new SQLiteParameter("@ZONA", zona),
            //" AND   COD_ZON = @ZONA " +

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Pedido ped = new Pedido();

                    if (!reader.IsDBNull(4))
                        ped.Compania = reader.GetString(4);

                    if (!reader.IsDBNull(0))
                        ped.Numero = reader.GetString(0);

                    if (!reader.IsDBNull(2))
                        ped.MontoBruto = reader.GetDecimal(2);

                    if (!reader.IsDBNull(1))
                        ped.FechaRealizacion = reader.GetDateTime(1);

                    if (!reader.IsDBNull(3))
                    {
                        ped.NCF = new NCFBase();
                        ped.NCF.UltimoValor = reader.GetString(3);
                    }
                    else
                    {
                        //KFC - Validacion para compañías que no utilizan NCF
                        ped.NCF = new NCFBase();
                        ped.NCF.UltimoValor = string.Empty;
                    }

                    #region Modificaciones en funcionalidad de generacion de recibos de contado - KFC

                    ped.CondicionPago = new CondicionPago();
                    ped.CondicionPago.Codigo = reader.GetString(5);
                    ped.CondicionPago.DiasNeto = CondicionPago.ObtenerDiasNeto(ped.CondicionPago.Codigo, ped.Compania);
                    #endregion

                    lista.Add(ped);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar los historicos de los Garantias. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return lista;
        }

        public int ObtenerSaldoFacturaHistorica()
        {
            int valor = int.MinValue;
            decimal saldo_dolar = decimal.Zero;
            decimal saldo_local = decimal.Zero;
            decimal monto_dolar = decimal.Zero;
            decimal monto_local = decimal.Zero;
            
            string sentencia =
                "SELECT SALDO_DOLAR, SALDO_LOCAL, MONTO_DOLAR, MONTO_LOCAL" +
                " FROM " + Table.ERPADMIN_alCXC_PEN_COB +
                " WHERE COD_TIP_DC IN (@TIPO1, @TIPO2, @TIPO3, @TIPO4, @TIPO5, @TIPO6, @TIPO7) AND NUM_DOC = @FACTURA AND COD_CLT = @CLIENTE";


            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@FACTURA", this.Numero),
                      new SQLiteParameter("@CLIENTE", this.Cliente),
                      new SQLiteParameter("@TIPO1", (int)TipoDocumento.Factura),
                      new SQLiteParameter("@TIPO2", (int)TipoDocumento.NotaDebito),
                      new SQLiteParameter("@TIPO3", (int)TipoDocumento.LetraCambio),
                      new SQLiteParameter("@TIPO4", (int)TipoDocumento.OtroDebito),
                      new SQLiteParameter("@TIPO5", (int)TipoDocumento.Intereses),
                      new SQLiteParameter("@TIPO6", (int)TipoDocumento.BoletaVenta),
                      new SQLiteParameter("@TIPO7", (int)TipoDocumento.InteresCorriente)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        saldo_dolar = reader.GetDecimal(0);
                    if (!reader.IsDBNull(2))
                        saldo_local = reader.GetDecimal(1);
                    if (!reader.IsDBNull(1))
                        monto_dolar = reader.GetDecimal(2);
                    if (!reader.IsDBNull(1))
                        monto_local = reader.GetDecimal(3);

                    if (saldo_local == monto_local && saldo_dolar == monto_dolar)
                        valor = 1;
                    else if (saldo_local > 0 && saldo_dolar > 0)
                        valor = 2;
                    else
                        valor = 0;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar saldos de historico de facturas. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return valor;
        }

        private string ObtenerCondicionPagoPedido()
        {
            if (this.configuracion.CondicionPago.Codigo == Corporativo.Compania.Obtener(this.Compania).CondicionPagoContado)
            {
                return "Contado";
            }
            else
            {
                try
                {
                    //Corporativo.CondicionPago.ObtenerDiasNeto(condicionPago,compania)
                    return "Crédito a " + this.configuracion.CondicionPago.DiasNeto + " día(s)";
                }
                catch (Exception ex)
                {
                    throw new Exception("Error obteniendo los días de crédito. " + ex.Message);
                }
            }
        }

        private string ObtenerFechaVenceFactura()
        {
            string resultado = string.Empty;
             
            try
            {
                if (tipo == TipoPedido.Factura)
                {
                    DateTime fechaVence = FechaRealizacion;
                    double dias = (double)(this.configuracion.CondicionPago.DiasNeto);
                    fechaVence = fechaVence.AddDays(dias);

                    resultado = fechaVence.ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error fecha vencimiento de factura. " + ex.Message);
            }

            return resultado;
        }

        #endregion 
        
        #region IPrintable Members

        public override string GetObjectName()
        {
            return "PEDIDO";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                //Caso:32682 ABC 23/05/2008 Mostrar Total de Articulo y Total de Lineas
                case "DETALLES": return detalles.ExploteLineas();
                case "TOTAL_ARTICULOS": return detalles.TotalArticulos;
                case "TOTAL_LINEAS": return detalles.TotalArticulos;
                //Caso CR1-04845-CM4Q LDA 05/05/2011
                case "CONDICIONPAGOPEDIDO": return ObtenerCondicionPagoPedido();
                case "FECHAVENCEFACTURA": return ObtenerFechaVenceFactura();
                default:
                    object field = this.Configuracion.ClienteCia.GetField(name);
                    if (!field.Equals(string.Empty))
                        return field;
                    else
                        return base.GetField(name);

            }
        }


        /// <summary>
        /// Guarda en la base de datos las facturas con el registro 
        /// del recibo al que pertenece y el monto del mismo que se le aplico.
        /// Corresponde a movimientos de efectivo y cheques
        /// </summary>
        /// <param name="recibo">consecutivo de recibo</param>
        public void GuardarRecibo(Recibo recibo)
        {
            if (this.MontoNeto != 0)
            {
                string sentencia =
                    " INSERT INTO " + Table.ERPADMIN_alCXC_MOV_DIR +
                    "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, NUM_DOC, COD_TIP_DA, NUM_DOC_AF, COD_CLT, IND_ANL, FEC_DOC, FEC_PRO,     MON_MOV_LOCAL, MON_SAL_LOC, MON_MOV_DOL, MON_SAL_DOL) " +
                    " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@NUM_DOC,@COD_TIP_DA,@NUM_DOC_AF,@COD_CLT,@IND_ANL,@FEC_DOC, date('now','localtime'),@MON_MOV_LOCAL,@MON_SAL_LOC,@MON_MOV_DOL,@MON_SAL_DOL) ";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)TipoDocumento.Recibo).ToString()),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, recibo.Numero),
					GestorDatos.SQLiteParameter("@NUM_DOC",SqlDbType.NVarChar, recibo.Numero),
                GestorDatos.SQLiteParameter("@COD_TIP_DA",SqlDbType.NVarChar, ((int)TipoDocumento.FacturaContado).ToString()),
                GestorDatos.SQLiteParameter("@NUM_DOC_AF",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()),//LJR Caso 36172
                GestorDatos.SQLiteParameter("@FEC_DOC",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),                
                // mvega: se usa la funcion date('now','localtime') de SQLite
                //GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, DateTime.Now.Date),
                //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                GestorDatos.SQLiteParameter("@MON_MOV_LOCAL",SqlDbType.Decimal,Decimal.Round(recibo.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MON_MOV_DOL",SqlDbType.Decimal,Decimal.Round(recibo.MontoDocDolar,2)),
                GestorDatos.SQLiteParameter("@MON_SAL_LOC",SqlDbType.Decimal,Decimal.Round(recibo.SaldoDocLocal,2)),
                GestorDatos.SQLiteParameter("@MON_SAL_DOL",SqlDbType.Decimal,Decimal.Round(recibo.SaldoDocDolar,2))});

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
        }

        #region Modificaciones en funcionalidad de recibos de contado - KFC

        /// <summary>
        /// Metodo que busca y devuelve el recibo asociado a la factura de contado
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        public string ReciboAsociado(string pedido)
        {
            string numRecibo = string.Empty;

            string sentencia =
                "SELECT NUM_REC" +
                " FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE NUM_DOC_AF = (@GARANTIA)";


            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@GARANTIA", this.Numero)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        numRecibo = reader.GetString(0);
                }

                if (numRecibo.Length == 0)
                    numRecibo = string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verificando si existen documentos asociados a la Garantia. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return numRecibo;
        }

        
        /// <summary>
        /// Metodo que devuelve una lista con los codigos de las notas de credito aplicadas
        /// a la factura de contado
        /// </summary>
        /// <param name="numRecibo"></param>
        /// <returns></returns>
        public List<string> NotasCreditoAsociadas(string numRecibo)
        {
            List<string> notasAsociadas = new List<string>();

            string sentencia =
                "SELECT NUM_DOC FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE COD_TIP_DC in ('7','15') AND NUM_REC = @RECIBO ";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@RECIBO", numRecibo)});    

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    string nota = string.Empty;

                    if (!reader.IsDBNull(0))
                        nota = reader.GetString(0);

                    notasAsociadas.Add(nota);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar las notas de crédito asociadas. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return notasAsociadas;            
        }

        
        /// <summary>
        /// Anula el recibo asociado a la factura de contado
        /// </summary>
        /// <param name="numRecibo"></param>
        /// <param name="anularTodo"></param>
        /// <param name="ruta"></param>
        public void AnularReciboAsociado(string numRecibo, bool anularTodo, string ruta, BaseViewModel viewModel)
        {
            List<Recibo> recibos;
            recibos = Recibo.CargaRecibosCliente(this.Cliente,this.Zona,EstadoDocumento.Activo); // o contado?

            // Obtiene el recibo a anular de la lista
            Recibo rec = recibos.Find(
                                        delegate(Recibo bk)
                                        {
                                            return bk.Numero == numRecibo;
                                        }
                                     );
            rec.CargaDetalles();            
            rec.Anular(anularTodo,ruta, viewModel);   
        }


        /// <summary>
        /// Le cambia el tipo al recibo en caso que este no se anule junto con la factura
        /// para que pueda subir como un recibo de credito a CC
        /// </summary>
        /// <param name="numRecibo"></param>
        public void CambiarTipoRecibo(string numRecibo)
        {
            string sentencia =
                " UPDATE " + Table.ERPADMIN_alCXC_DOC_APL +
                " SET IND_ANL = @IND_ANL " +
                " WHERE COD_ZON = @COD_ZON " +
                " AND NUM_REC = @NUM_REC ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {               
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@IND_ANL", SqlDbType.NVarChar, ((char)EstadoDocumento.Activo).ToString()),
                GestorDatos.SQLiteParameter("@NUM_REC", SqlDbType.NVarChar, numRecibo)});

            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);

                EliminarAplicacionesRecibo(numRecibo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error cambiando el tipo de recibo. " + ex.Message);
            }

        }


        /// <summary>
        /// Elimina de la tabla ALCXC_MOV_DIR las aplicaciones de una factura anulada
        /// a la que no se le anulo el recibo asociado 
        /// </summary>
        /// <param name="numRecibo"></param>
        public void EliminarAplicacionesRecibo(string numRecibo)
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE COD_ZON = @COD_ZON " +
                " AND NUM_REC = @NUM_REC ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {               
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC", SqlDbType.NVarChar, numRecibo)});

            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error cambiando el tipo de recibo. " + ex.Message);
            }
        }


        /// <summary>
        /// Reversa las notas de credito que fueron aplicadas a la factura que se esta anulando
        /// </summary>
        /// <param name="numRecibo"></param>
        public void ReversarNotasCredito(string numRecibo)
        {
            List<string> notasAsociadas = NotasCreditoAsociadas(numRecibo);

            foreach (string notaCredito in notasAsociadas)
            {
                ReversarAplicacionNota(numRecibo, notaCredito);
            }
        }

        
        /// <summary>
        /// funcion que reversa la palicacion de una N/C al anular una factura
        /// para que la nota pueda seguir viva
        /// </summary>
        /// <param name="numRecibo"></param>
        /// <param name="numNota"></param>
        private void ReversarAplicacionNota(string numRecibo, string numNota)
        {
            // Se obtienen los montos aplicados de la nota de credito
            decimal montoLocal = decimal.Zero;
            decimal montoDolar = decimal.Zero;

            string sentencia =
                "SELECT MON_MOV_LOCAL,MON_MOV_DOL" +
                " FROM " + Table.ERPADMIN_alCXC_MOV_DIR +
                " WHERE NUM_REC = (@RECIBO)" +
                " AND NUM_DOC = (@DOCUMENTO)" +
                " AND IND_ANL in (@IND_ANL,@IND_CONT)";


            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@RECIBO", numRecibo),
                      new SQLiteParameter("@DOCUMENTO", numNota),
                      new SQLiteParameter("@IND_ANL", ((char)EstadoDocumento.Anulado).ToString()),
                      new SQLiteParameter("@IND_CONT", ((char)EstadoDocumento.Contado).ToString())});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        montoLocal = reader.GetDecimal(0);
                    if (!reader.IsDBNull(1))
                        montoDolar = reader.GetDecimal(1);
                }

                //llamado a la funcion de actualizacion de saldos
                ActualizarSaldosNota(numNota, montoLocal, montoDolar);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error verificando si existen documentos asociados a la Factura. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }         

        }


        /// <summary>
        /// Se actualizan los saldos de las notas de credito
        /// </summary>
        /// <param name="numNota"></param>
        /// <param name="saldoLocal"></param>
        /// <param name="saldoDolar"></param>
        private void ActualizarSaldosNota(string numNota,decimal saldoLocal, decimal saldoDolar)
        {

            string sentencia =
                " UPDATE " + Table.ERPADMIN_alCXC_PEN_COB +
                " SET SALDO_LOCAL = @SALDO_LOCAL ," +
                " SALDO_DOLAR = @SALDO_DOLAR " +
                " WHERE COD_ZON = @COD_ZON " +
                " AND NUM_DOC = @NUM_DOC ";            

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {               
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@SALDO_LOCAL", SqlDbType.NVarChar, saldoLocal),
                GestorDatos.SQLiteParameter("@SALDO_DOLAR", SqlDbType.NVarChar, saldoDolar),
                GestorDatos.SQLiteParameter("@NUM_DOC", SqlDbType.NVarChar, numNota)});

            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando el indicador de impresión. " + ex.Message);
            }

        }

        
        /// <summary>
        /// Actualiza los valores en la tabla JORNADA_RUTAS 
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="monto"></param>
        private void ActualizarJornada(string ruta, decimal montoTot, TipoMoneda moneda, BaseViewModel viewModel)
        {
            string colCantidad = "";
            string colMontoTot = "";

            if (moneda == TipoMoneda.LOCAL)
            {
                colCantidad = JornadaRuta.FACTURAS_LOCAL;
                colMontoTot = JornadaRuta.MONTO_FACTURAS_LOCAL;
            }
            else
            {
                colCantidad = JornadaRuta.FACTURAS_DOLAR;
                colMontoTot = JornadaRuta.MONTO_FACTURAS_DOLAR;
            }

            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, -1);
                JornadaRuta.ActualizarRegistro(ruta, colMontoTot, -montoTot);

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                viewModel.mostrarAlerta("Error al actualizar datos. " + ex.Message);
            }
        }

        /// <summary>
        /// Aplica la nota de credito a la factura
        /// </summary>
        /// <param name="notaCredito"></param>
        public void AplicaNotaLocal(NotaCredito notaCredito)
        {
            decimal calculo = this.MontoAPagarDocLocal - notaCredito.SaldoDocLocal;

            if (calculo <= 0)//verifica que el monto del calculo sea menor o igual a 0
            {
                //Cobros.MontoNotasCreditoSeleccion += this.MontoAPagarDocLocal;//se le suma a el monto al monto de notas de credito el monto a pagar por la factura
                Cobros.MontoNotasCreditoSeleccion += this.MontoAPagarDocLocal;//se le suma a el monto al monto de notas de credito el monto a pagar por la factura

                //el saldo de la nota se le asigna el monto que dio la resta de los montos
                //pero se niega el calculo ya que este es negativo
                notaCredito.SaldoDocLocal -= this.MontoAPagarDocLocal;
                notaCredito.SaldoDocDolar -= this.MontoAPagarDocDolar;

                notaCredito.MontoMovimientoLocal += this.MontoAPagarDocLocal;//al monto movimineto de la nota se le suma el monto a pagar por la factura
                notaCredito.MontoMovimientoDolar += this.MontoAPagarDocDolar;

                this.SaldoDocLocal -= this.MontoAPagarDocLocal;//el saldo de la factura se le resta el monto a pagar de la misma
                this.SaldoDocDolar -= this.MontoAPagarDocDolar;

                //se guarda el numero de nota y el monto aplicado en un arreglo asi como el saldo de la factura
                NotaCredito notaAplicada = new NotaCredito();
                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = this.MontoAPagarDocLocal;
                notaAplicada.MontoDocDolar = this.MontoAPagarDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                this.MontoAPagarDocLocal = 0;//el total a pagar de la factura se iguala a 0
                this.MontoAPagarDocDolar = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;                

            }
            else
            {
                this.MontoAPagarDocLocal = calculo;//el monto a pagar es igual al calculo
                //KFC - se agrega la actualizacion al monto en dolares
                this.MontoAPagarDocDolar = calculo / Cobros.TipoCambio;

                Cobros.MontoNotasCreditoSeleccion += notaCredito.SaldoDocLocal;//se le suma  al monto de notas de credito el saldo de la nota de credito

                notaCredito.MontoMovimientoLocal += notaCredito.SaldoDocLocal;//monto del movimiento de la nota se le suma el saldo de la nota
                notaCredito.MontoMovimientoDolar += notaCredito.SaldoDocDolar;

                this.SaldoDocLocal -= notaCredito.SaldoDocLocal;//al saldo de la factura se le resta el saldo de la nota
                this.SaldoDocDolar -= notaCredito.SaldoDocDolar;

                NotaCredito notaAplicada = new NotaCredito();

                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = notaCredito.SaldoDocLocal;
                notaAplicada.MontoDocDolar = notaCredito.SaldoDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                notaCredito.SaldoDocLocal = 0;//se iguala saldo de la nota a 0
                notaCredito.SaldoDocDolar = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;
            }
            //notaCredito.indProcesado = true;//se cambia el indice de procesamineto de la nota
            //this.indProcesado = true;//se cambia el indice de procesamineto de la factura
        }

        /// <summary>
        /// Aplica la nota de credito a la factura
        /// </summary>
        /// <param name="notaCredito">nota de credito a aplicar</param>
        /// <returns>retorna un objeto de tipo documento</returns>
        public void AplicaNotaDolar(NotaCredito notaCredito)
        {
            decimal calculo = this.MontoAPagarDocDolar - notaCredito.SaldoDocDolar;

            if (calculo <= 0)//verifica que el monto del calculo sea menor o igual a 0
            {
                Cobros.MontoNotasCreditoSeleccion += this.MontoAPagarDocDolar;//se le suma a el monto al monto de notas de credito el monto a pagar por la factura

                //el saldo de la nota se le asigna el monto que dio la resta de los montos
                //pero se niega el calculo ya que este es negativo
                notaCredito.SaldoDocDolar = -calculo;
                notaCredito.SaldoDocLocal = notaCredito.SaldoDocDolar * Cobros.TipoCambio;

                notaCredito.MontoMovimientoDolar += this.MontoAPagarDocDolar;//al monto movimineto de la nota se le suma el monto a pagar por la factura
                notaCredito.MontoMovimientoLocal += this.MontoAPagarDocLocal;

                this.SaldoDocDolar -= notaCredito.SaldoDocDolar;//el saldo de la factura se le resta el monto a pagar de la misma
                this.SaldoDocLocal -= notaCredito.SaldoDocLocal;

                NotaCredito notaAplicada = new NotaCredito();

                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = this.MontoAPagarDocLocal;
                notaAplicada.MontoDocDolar = this.MontoAPagarDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                this.MontoAPagarDocDolar = 0;//el total a pagar de la factura se iguala a 0
                this.MontoAPagarDocLocal = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;
            }
            else
            {
                Cobros.MontoNotasCreditoSeleccion += notaCredito.SaldoDocDolar;//se le suma  al monto de notas de credito el saldo de la nota de credito

                notaCredito.MontoMovimientoDolar += notaCredito.SaldoDocDolar;//monto del movimiento de la nota se le suma el saldo de la nota
                notaCredito.MontoMovimientoLocal += notaCredito.SaldoDocLocal;

                this.MontoAPagarDocDolar -= notaCredito.SaldoDocDolar;//al saldo de la factura se le resta el saldo de la nota
                this.MontoAPagarDocLocal -= notaCredito.SaldoDocLocal;

                this.SaldoDocDolar -= notaCredito.SaldoDocDolar;
                this.SaldoDocLocal -= notaCredito.SaldoDocLocal;//al saldo de la factura se le resta el saldo de la nota

                NotaCredito notaAplicada = new NotaCredito();

                notaAplicada.Numero = notaCredito.Numero;
                notaAplicada.Tipo = notaCredito.Tipo;
                notaAplicada.MontoDocLocal = notaCredito.SaldoDocLocal;
                notaAplicada.MontoDocDolar = notaCredito.SaldoDocDolar;
                notaAplicada.SaldoDocLocal = this.SaldoDocLocal;
                notaAplicada.SaldoDocDolar = this.SaldoDocDolar;

                notaCredito.SaldoDocDolar = 0;//se iguala saldo de la nota a 0
                notaCredito.SaldoDocLocal = 0;
                this.NotasCreditoAplicadas.Add(notaAplicada);// se agrega el arreglo de la nota y el monto aplicado en un arreglo de notas aplicadas a la factura
                //this.tieneNotas = true;
            }
        }


        #endregion

        #endregion

    }
}
