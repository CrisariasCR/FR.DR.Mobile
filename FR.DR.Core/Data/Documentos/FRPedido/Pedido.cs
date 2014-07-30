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
using FR.DR.Core.Data.Documentos.Retenciones;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
    public class Pedido : EncabezadoDocumento, IPrintable
    {
        #region Propiedades Globales
        
        #endregion

        #region Variables y Propiedades de instancia


        private DetallesPedido detalles = new DetallesPedido();
        /// <summary>
        /// Detalles del Pedido
        /// </summary>
        public DetallesPedido Detalles
        {
            get { return detalles; }
            set { detalles = value; }
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

        public Pedido()
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
        public Pedido(string numero, string compania, string cliente, string zona)
        {
            Numero = numero;
            Compania = compania;
            Cliente = cliente;
            Zona = zona;
            Tipo = TipoPedido.Factura;
            Detalles.Encabezado =this;

        }

        public Pedido(string numero, string compania, string cliente, string zona, string ncf)
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
        public Pedido(Articulo articulo, string zona, ConfigDocCia configuracion)
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
                if(this.Configuracion.CondicionPago!=null)
                {
                    this.CondicionPago = Configuracion.CondicionPago;
                }                
                this.HoraInicio = DateTime.Now;
                this.FechaRealizacion = DateTime.Now;
                this.HoraFin = DateTime.Now;
                
                if (Pedidos.FacturarPedido)
                {
                    this.Estado = EstadoPedido.Facturado;
                    this.Tipo = TipoPedido.Factura;
                    this.FechaEntrega = this.FechaRealizacion; 
                    
                    this.Configuracion.Compania.Cargar();
                    if (this.Configuracion.Compania.UsaNCF && this.Configuracion.ClienteCia.TipoContribuyente != string.Empty)
                    {
                        NCFUtilitario.obtenerNCF(this.Compania, NCFUtilitario.FACTURA, this.Configuracion.ClienteCia.TipoContribuyente);
                        this.NCF = NCFUtilitario.consecutivoNCF;
                    }
                    if (ResolucionUtilitario.usaResolucionFactura(this.Zona))
                    {
                        ResolucionUtilitario.obtenerConsecResolucion(this.Compania, ConsecResoluBase.TIPOFACTURA, this.Zona);
                        this.ConsecResolucion = ResolucionUtilitario.consecutivoResolucion;
                    }
                    else
                    {
                        this.ConsecResolucion = null;
                    }
                }
                else
                {
                    this.Estado = EstadoPedido.Normal;
                    this.Tipo = TipoPedido.Prefactura;
                    this.FechaEntrega = DateTime.Now.AddDays(1);
                }

                //KFC Sincronización listas de Precios
                this.NivelPrecioCod = this.Configuracion.Nivel.Nivel;
                this.Moneda = this.Configuracion.Nivel.Moneda.Equals(TipoMoneda.LOCAL) ? "L" : "D";
        }
        /// <summary>
        /// Retornar un Pedido en Consignacion
        /// </summary>
        public static Pedido Consignado(string consignacion,Articulo articulo, string zona, ConfigDocCia configuracion, bool descuentoCascada,
            decimal porcDesc1, decimal porcDesc2, string nota)
        {
            //Se asigna la configuración de la boleta de venta en consignación a la factura
            //que se va a generar por el desglose.
            Pedido pedido = new Pedido(articulo, zona, configuracion);
            
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
            DetallePedido detalle = Detalles.Buscar(articulo.Codigo);

            if (detalle == null)
            {
                //Se crea un nuevo detalle si alguna cantidad es mayor a cero.
                if (unidadesAlmacen == 0 && unidadesDetalle == 0)
                    return;

                if (validarCantidadLineas && this.Detalles.Lista.Count == Pedidos.MaximoLineasDetalle)
                        throw new Exception("Se alcanzó el máximo de líneas permitidas (" + Pedidos.MaximoLineasDetalle + ")");

                detalle = new DetallePedido();
                detalle.Articulo = articulo;
                //Cesar Iglesias
                detalle.Tope = tope;
                detalle.Articulo.Bodega.Codigo = this.Bodega;
                detalle.RecalcularDetalle(unidadesAlmacen,unidadesDetalle,
                           precio, configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral, true);
                
                //Validamos el margen de utilidad, se levanta frmExcepcion si no cumple.
                if (Pedidos.CambiarPrecio)
                    detalle.ValidarMargenUtilidad(configuracion.Nivel, this.PorcDescGeneral);

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

                    detalle.RecalcularDetalle(unidadesAlmacen, unidadesDetalle,
                               precio, configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral, true);
                    try
                    {
                        //Validamos el margen de utilidad, se levanta frmExcepcion si no cumple.
                        if (Pedidos.CambiarPrecio)
                            detalle.ValidarMargenUtilidad(configuracion.Nivel, this.PorcDescGeneral);
                    }
                    catch (Exception ex)
                    {
                        detalle.RecalcularDetalle(undsAlmAnt,undsDetAnt,precioAnt,
                            configuracion.Nivel,configuracion.ClienteCia, PorcDescGeneral,true);
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
        public void AgregarLineaDetalleTomaFisica(Articulo articulo, Precio precio,
            decimal unidadesDetalle, decimal unidadesAlmacen,
            bool validarCantidadLineas, decimal unidadAlmacenAdicional, decimal unidadDetalleAdicional, string tope)
        {
            DetallePedido detalle = Detalles.Buscar(articulo.Codigo);

            if (detalle == null)
            {
                //Se crea un nuevo detalle si alguna cantidad es mayor a cero.
                if (unidadesAlmacen == 0 && unidadesDetalle == 0)
                    return;

                if (validarCantidadLineas && this.Detalles.Lista.Count == Pedidos.MaximoLineasDetalle)
                    throw new Exception("Se alcanzó el máximo de líneas permitidas (" + Pedidos.MaximoLineasDetalle + ")");

                detalle = new DetallePedido();
                detalle.Articulo = articulo;
                //Cesar Iglesias
                detalle.Tope = tope;
                detalle.Articulo.Bodega.Codigo = this.Bodega;
                detalle.RecalcularDetalleTomaFisica(unidadesAlmacen, unidadesDetalle,
                           precio, configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral, true);

                //Validamos el margen de utilidad, se levanta frmExcepcion si no cumple.
                if (Pedidos.CambiarPrecio)
                    detalle.ValidarMargenUtilidad(configuracion.Nivel, this.PorcDescGeneral);                

                this.Detalles.Lista.Add(detalle);
                this.AgregarMontosLinea(detalle);
                //Actualizamos los montos de descuentos 1 y 2.
                this.ReCalcularDescuentosGenerales();
            }
            else
            {
                detalle.Tope = tope;
                //El detalle del pedido ya se encuentra en el pedido por lo que se procede a modificarlo.
                if (detalle.UnidadesAlmacen != unidadesAlmacen || detalle.UnidadesDetalle != unidadesDetalle ||
                    detalle.Precio.Maximo != precio.Maximo || detalle.Precio.Minimo != precio.Minimo)
                {
                    //Restamos los montos actuales de la línea.
                    this.RestarMontosLinea(detalle);

                    //Se desea cambiar la cantidad o los precios de la línea del pedido por lo que 
                    //guardamos los valores del detalle para este caso.
                    Precio precioAnt = detalle.Precio;
                    decimal undsAlmAnt = detalle.UnidadesAlmacen;
                    decimal undsDetAnt = detalle.UnidadesDetalle;

                    detalle.RecalcularDetalleTomaFisica(unidadesAlmacen, unidadesDetalle,
                               precio, configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral, true);
                    try
                    {
                        //Validamos el margen de utilidad, se levanta frmExcepcion si no cumple.
                        if (Pedidos.CambiarPrecio)
                            detalle.ValidarMargenUtilidad(configuracion.Nivel, this.PorcDescGeneral);
                    }
                    catch (Exception ex)
                    {
                        detalle.RecalcularDetalleTomaFisica(undsAlmAnt, undsDetAnt, precioAnt,
                            configuracion.Nivel, configuracion.ClienteCia, PorcDescGeneral, true);
                        throw ex;
                    }
                    finally
                    {
                        this.AgregarMontosLinea(detalle);
                        //Actualizamos los montos de descuentos 1 y 2.
                        this.ReCalcularDescuentosGenerales();
                    }
                }
            }
        }

        public void AgregarLineaDetalle(Articulo articulo, Precio precio,
            decimal unidadesDetalle, decimal unidadesAlmacen,
            bool validarCantidadLineas, string tope)
        {
           AgregarLineaDetalle(articulo, precio, unidadesDetalle, unidadesAlmacen, validarCantidadLineas, 0, 0, tope);        
        }

        public void AgregarLineaDetalleTomaFisica(Articulo articulo, Precio precio,
            decimal unidadesDetalle, decimal unidadesAlmacen,
            bool validarCantidadLineas, string tope)
        {
            AgregarLineaDetalleTomaFisica(articulo, precio, unidadesDetalle, unidadesAlmacen, validarCantidadLineas, 0, 0, tope);
        }

        /// <summary>
        /// Elimina un detalle del pedido.
        /// </summary>
        /// <param name="codArt">Código del articulo que se debe eliminar.</param>
        /// <returns>Retorna la cantidad de lineas eliminadas</returns>
        public int EliminarDetalle(string articulo)
        {
            DetallePedido detalleEncontrado = detalles.Eliminar(articulo);

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
            DetallePedido detalleEncontrado = detalles.Lista[posicionDetalle];
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
        private bool GestionarRetiroDetalleBonificado(ref DetallePedido detalle)
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
            foreach (DetallePedido detalle in this.Detalles.Lista)
            {   
                // PA2-01482-MKP6 - KFC
                //detalle.CalcularImpuestos(this.Configuracion.ClienteCia.ExoneracionImp1, this.Configuracion.ClienteCia.ExoneracionImp2, this.PorcDescGeneral, percepcion);
                detalle.CalcularImpuestos(this.Configuracion.ClienteCia.ExoneracionImp1, this.Configuracion.ClienteCia.ExoneracionImp2, this.PorcDescGeneral, percepcion);
                this.Impuesto.MontoImpuesto1 += detalle.Impuesto.MontoImpuesto1;
                this.Impuesto.MontoImpuesto2 += detalle.Impuesto.MontoImpuesto2;
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
            foreach (DetallePedido detalle in this.Detalles.Lista)
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
        public void AgregarMontosLinea(DetallePedido detalle)
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
        public void RestarMontosLinea(DetallePedido detalle)
        {
            this.MontoDescuentoLineas -= detalle.MontoDescuento;
            this.MontoBruto -= detalle.MontoTotal;
            this.Impuesto.MontoImpuesto1 -= detalle.Impuesto.MontoImpuesto1;
            this.Impuesto.MontoImpuesto2 -= detalle.Impuesto.MontoImpuesto2;
        }

        #endregion

        #region Metodos de Instancia 

        public void Actualizar(bool commit)
        {
            if (detalles.Lista.Count == 0)
                return;
            try
            {
                GestorDatos.BeginTransaction();
                DBEliminar();
                DBGuardar();
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

        public void Guardar(bool commit)
        {
            try
            {
                GestorDatos.BeginTransaction();

                if (this.Tipo == TipoPedido.Factura && this.Configuracion.Compania.UsaNCF && this.configuracion.ClienteCia.TipoContribuyente != string.Empty)
                    NCF.IncrementarConsecutivoNCF();

                DBGuardar();
                if (this.Tipo == TipoPedido.Factura)
                {
                    if (!ResolucionUtilitario.usaResolucionFactura(this.Zona))
                    {
                        ParametroSistema.IncrementarFactura(Compania, Zona);
                    }
                    else
                    {
                        this.ConsecResolucion.IncrementarConsecutivoRes();
                    }

                }

                else
                {
                    if (this.UsuarioCambioDescuentos())
                        ParametroSistema.IncrementarPedidoDesc(Compania, Zona);
                    else
                        ParametroSistema.IncrementarPedido(Compania, Zona);
                }

                if (commit)
                    GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (commit)
                    GestorDatos.RollbackTransaction();
                AplicarPedidoViewModel.Error = "Error al guardar " + (this.Tipo == TipoPedido.Factura ? "Factura. " : "Pedido. ") + ex.Message;
                throw new Exception("Error al guardar " + (this.Tipo == TipoPedido.Factura ? "Factura. " : "Pedido. ") + ex.Message);
            }
        }

        public void GuardarFactDiferencias(bool commit)
        {
            string colCantidad;
            string colMonto;
            string colCantidadCondPago = "";
            string colMontoCondPago = "";
            try
            {
                GestorDatos.BeginTransaction();

                if (this.Tipo == TipoPedido.Factura && this.Configuracion.Compania.UsaNCF && this.configuracion.ClienteCia.TipoContribuyente != string.Empty)
                    NCF.IncrementarConsecutivoNCF();

                DBGuardarTomaFisica();
                if (this.Tipo == TipoPedido.Factura)
                    ParametroSistema.IncrementarFactura(Compania, Zona);

                else
                {
                    if (this.UsuarioCambioDescuentos())
                        ParametroSistema.IncrementarPedidoDesc(Compania, Zona);
                    else
                        ParametroSistema.IncrementarPedido(Compania, Zona);
                }

                if (commit)
                    GestorDatos.CommitTransaction();
                if (Moneda == TipoMoneda.LOCAL.ToString())
                {
                    colCantidad = JornadaRuta.FACTURASTF_LOCAL;
                    colMonto = JornadaRuta.MONTO_FACTURASTF_LOCAL;
                    if (this.CondicionPago.DiasNeto == 0)
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_LOCAL_CONT;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_LOCAL_CONT;
                    }
                    else
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_LOCAL_CRE;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_LOCAL_CRE;
                    }
                }
                else
                {
                    colCantidad = JornadaRuta.FACTURASTF_DOLAR;
                    colMonto = JornadaRuta.MONTO_FACTURASTF_DOLAR;
                    if (this.CondicionPago.DiasNeto == 0)
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_DOLAR_CONT;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_DOLAR_CONT;
                    }
                    else
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_DOLAR_CONT;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_DOLAR_CONT;
                    }
                }
                try
                {
                    GestorDatos.BeginTransaction();

                    JornadaRuta.ActualizarRegistro(this.Zona, colCantidad, 1);
                    JornadaRuta.ActualizarRegistro(this.Zona,colMonto,this.MontoNeto);

                    JornadaRuta.ActualizarRegistro(this.Zona, colCantidadCondPago, 1);
                    JornadaRuta.ActualizarRegistro(this.Zona, colMontoCondPago, this.MontoNeto);

                    GestorDatos.CommitTransaction();
                }
                catch (Exception ex)
                {
                    GestorDatos.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                if (commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al guardar " + (this.Tipo == TipoPedido.Factura ? "Factura. " : "Pedido. ") + ex.Message);
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
                throw new Exception("Error al Anular " + (this.Tipo == TipoPedido.Factura ? "Factura. " : "Pedido. ") + ex.Message);
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
                factura.Tipo = TipoDocumento.FacturaContado;
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

        #region Retenciones

        /// <summary>
        /// Permite Obtener el monto retención de las retenciones que funcionan como base
        /// </summary>
        /* Separacion=ND*/
        public void __ObtenerMontoBaseRet(ref decimal pnMontoRetBase, string psRetBase, int pnCantidad)
        {
            
            //while (lnContador < pnCantidad)
            foreach (fclsRetencion ret in iArregloRetenciones)
            {
                if (psRetBase.Equals(ret.sCodigoRetencion))
                {
                    pnMontoRetBase = ret.nMonto;
                    break;
                }
            }
        }

        #region calcular retenciones
        /// <summary>
        /// Calcula las retenciones con los montos del documento.
        /// </summary>
        /* Separacion=ND*/
        public bool CalcularRetenciones()
        {
            fclsRetencion fciRetencion;
            StringBuilder sSqlCmd;
            SQLiteDataReader reader = null;
            bool pbEsDevolucion = false;
            int lnIndice;
            int lnIndiceArticulo;
            int lnCantidad;
            bool bOk;
            decimal lnMontoDoc = this.MontoNeto;
            decimal lnSubTotal = this.MontoSubTotal;
            decimal lnDescuento = this.MontoTotalDescuento;
            decimal lnImpuesto1 = this.Impuesto.MontoImpuesto1;
            decimal lnImpuesto2 = this.Impuesto.MontoImpuesto2;
            decimal lnRubro1 = 0;
            decimal lnRubro2 = 0;
            string lsCliente;
            string lsCodigoRetencion;
            string lsDescripcionRetencion;
            decimal lnMontoRetencion = 0;
            decimal lnBaseRetencion = 0;
            bool lbRetencionEncontrada;
            decimal lnMontoRetBase = 0;
            int lnNumRegistros;
            string lsArticulo;
            int? lnLineaAnt;
            decimal lnMonto;
            //string[] lsArrTempRetModArt = new string[]();
            List<string> lsArrTempRetModArt = new List<string>();
            bool lbMismoArticulo;

            this.iArregloRetenciones = new List<fclsRetencion>();
            //Sal.ArraySetUpperBound(iArregloRetenciones, 1, -(1));
            //Sal.ArraySetUpperBound(lsArrTempRetModArt, 1, -(1));
            inTotalRetenciones = 0;
            lnCantidad = 0;
            lnLineaAnt = null;
            bOk = true;
            //fciRetencion.InitInstance();
            fciRetencion = new fclsRetencion(this.Compania);
            // Obtenemos el cliente
            lsCliente = this.Cliente;
            // Obtenemos los montos
            //TODO
            //ObtenerTotalesRET(lnMontoDoc, lnSubTotal, lnDescuento, lnImpuesto1, lnImpuesto2, lnRubro1, lnRubro2);
            // NBH >> Ajustes Colombia R5
            // Se cargan las retenciones del modelo de retenciones asociado a cada artículo
            if (bOk)
            {
                lnIndiceArticulo = 0;
                while (((lnIndiceArticulo < this.detalles.Lista.Count) && bOk))
                {
                    lsArticulo = this.detalles.Lista[lnIndiceArticulo].Articulo.Codigo;//  fciLineasDeDocumento[lnIndiceArticulo].sArticulo;
                    // MRL Ajuste Colombia R5 --> Se valida el régimen del cliente, para determinar que retenciones se deben cargar
                    sSqlCmd = new StringBuilder();
                    sSqlCmd.AppendLine(string.Format(" SELECT ret.codigo_retencion, ret.descripcion FROM {0} ret,{1} art, ", Table.ERPADMIN_RETENCIONES, Table.ERPADMIN_ARTICULO));
                    sSqlCmd.AppendLine(string.Format("{0} dmr,{1} cli,{2} exr,{3} reg ", Table.ERPADMIN_DET_MOD_RETENCION, Table.ERPADMIN_CLIENTE_CIA, Table.ERPADMIN_EXCEP_REGIMEN, Table.ERPADMIN_REGIMENES));
                    sSqlCmd.AppendLine(string.Format(" WHERE art.cod_art = '{1}' AND ret.estado = '{0}'", fclsRetencion.CC_RET_ACTIVA, lsArticulo));
                    sSqlCmd.AppendLine(string.Format(" AND art.modelo_retencion = dmr.modelo_retencion AND ret.codigo_retencion = dmr.codigo_retencion AND dmr.aplica IN('{0}','{1}')", fclsRetencion.AS_AMBAS, fclsRetencion.AS_VENTA));
                    sSqlCmd.AppendLine(string.Format(" AND cli.cod_clt = '{0}' AND cli.regimen_trib = reg.regimen AND exr.codigo = reg.regimen AND exr.codigo_retencion = ret.codigo_retencion", lsCliente));
                    sSqlCmd.AppendLine(string.Format(" AND ret.COD_CIA = '{0}' AND dmr.COD_CIA=ret.COD_CIA AND exr.COD_CIA=ret.COD_CIA AND reg.COD_CIA=ret.COD_CIA", Compania));
                    // MRL Ajuste Colombia R5 <--
                    try
                    {
                        // Se ejecuta la sentencia de consulta
                        reader = GestorDatos.EjecutarConsulta(sSqlCmd.ToString());
                        lsArrTempRetModArt = new List<string>();
                        // Se recorren los registros obtenidos
                        while (reader.Read())
                        {
                            lsCodigoRetencion = reader.GetString(0);
                            lsDescripcionRetencion = reader.GetString(1);


                            fciRetencion.ReadOnKey(lsCodigoRetencion);

                            if (!string.IsNullOrEmpty(fciRetencion.isRetencionBase))
                            {
                                __ObtenerMontoBaseRet(ref lnMontoRetBase, fciRetencion.isRetencionBase, lnCantidad);
                            }

                            fciRetencion.SetDatosCalcularMonto(lnMontoRetBase, this.Configuracion.Pais.Codigo, this.Configuracion.Pais.DivisionGeografica1, this.Configuracion.Pais.DivisionGeografica2, this.Configuracion.ClienteCia.Regimen);
                            // MRL Ajustes Colombia R5 --> Se hace por linea y no por el total del documento
                            /*
                                // Set lnMontoRetencion = fciRetencion.CalcularMonto ( lnMontoDoc, lnSubTotal, lnDescuento, lnImpuesto1, lnImpuesto2, lnRubro1, lnRubro2, 1, lnBaseRetencion )
                            */

                            lnMonto = this.Detalles.Lista[lnIndiceArticulo].MontoTotal + this.Detalles.Lista[lnIndiceArticulo].Impuesto.MontoImpuesto1 + this.Detalles.Lista[lnIndiceArticulo].Impuesto.MontoImpuesto2;

                            lnMontoRetencion = fciRetencion.CalcularMonto(lnMonto, this.Detalles.Lista[lnIndiceArticulo].MontoTotal, this.Detalles.Lista[lnIndiceArticulo].MontoDescuento, this.Detalles.Lista[lnIndiceArticulo].Impuesto.MontoImpuesto1, this.Detalles.Lista[lnIndiceArticulo].Impuesto.MontoImpuesto2, 0, 0, 1, ref lnBaseRetencion, this.Configuracion.Compania.PaisInstalado);

                            // MRL Ajustes Colombia R5 <--
                            // Si el monto de retención calculado es mayor a cero, se agrega al arreglo de retenciones
                            if ((lnMontoRetencion > 0))
                            {
                                // MRL Ajustes Colombia R5 -->
                                // Recorremos el arreglo de retenciones en caso de que ya exista la retencion
                                lnIndice = 0;
                                lbRetencionEncontrada = false;
                                //while ((lnIndice < lnCantidad))
                                foreach (fclsRetencion ret in iArregloRetenciones)
                                {
                                    if ((ret.sCodigoRetencion == lsCodigoRetencion))
                                    {
                                        lbRetencionEncontrada = true;
                                        break;
                                    }
                                } // while
                                //lsArrTempRetModArt[lnCantidad] = lsCodigoRetencion;
                                lsArrTempRetModArt.Add(lsCodigoRetencion);
                                // MRL Ajustes Colombia R5 <--
                                // Si la retención no fue encontrada se carga al arreglo
                                if (!(lbRetencionEncontrada))
                                {
                                    fclsRetencion newReten = new fclsRetencion(this.Compania);
                                    newReten.sCodigoRetencion = lsCodigoRetencion;
                                    newReten.sRetencion = lsDescripcionRetencion;
                                    newReten.nMonto = lnMontoRetencion;
                                    newReten.nBaseGravada = lnBaseRetencion;
                                    newReten.sAutoretenedora = fciRetencion.sEsAutoretenedor;
                                    newReten.sTipo = fciRetencion.sTipo;
                                    if (pbEsDevolucion)
                                    {
                                        newReten.sDocReferencia = ("RED-#" + (lnCantidad + 1).ToString());
                                    }
                                    else
                                    {
                                        newReten.sDocReferencia = ("RET-#" + (lnCantidad + 1).ToString());
                                    }
                                    if ((fciRetencion.sTipo == fclsRetencion.CP_RET_REGISTRO))
                                    {
                                        newReten.sEstado = fclsRetencion.CC_RET_APLICADA;
                                    }
                                    else
                                    {
                                        newReten.sEstado = fclsRetencion.CC_RET_PENDIENTE;
                                    }
                                    iArregloRetenciones.Add(newReten);
                                    inTotalRetenciones = (inTotalRetenciones + lnMontoRetencion);
                                    lnCantidad = (lnCantidad + 1);
                                }
                                //Si la retención existe pero para otro artículo se debe acumular el monto
                                else
                                {
                                    if ((lbRetencionEncontrada && (lnLineaAnt != null)))
                                    {
                                        if ((lnIndiceArticulo != lnLineaAnt))
                                        {
                                            // Agregamos el monto a la retención que existe ya en el arreglo
                                            iArregloRetenciones.Find(x => x.sCodigoRetencion == lsCodigoRetencion).nMonto = (iArregloRetenciones.Find(x => x.sCodigoRetencion == lsCodigoRetencion).nMonto + lnMontoRetencion);
                                            iArregloRetenciones.Find(x => x.sCodigoRetencion == lsCodigoRetencion).nBaseGravada = (iArregloRetenciones.Find(x => x.sCodigoRetencion == lsCodigoRetencion).nBaseGravada + lnBaseRetencion);
                                            // Calculamos el total de retenciones del documento
                                            inTotalRetenciones = (inTotalRetenciones + lnMontoRetencion);
                                        }
                                    }
                                }
                            }
                        } // while
                        // MRL Ajustes Colombia R5 -->
                        // Se cargan las retenciones por artículo
                        if (bOk)
                        {
                            // Se envía la ciudad asociada al documento y el régimen del cliente
                            // MRL Proyecto Colombia R4 --> Se pasa la ubicación geográfica y el regimen del cliente
                            // CO4-01751-M4VP lgonzalez ini --->
                            /*
                                // Set bOk = fciLineasDeDocumento [ lnIndiceArticulo ].CalcularRetencion( hsqlPrmQuery, ifciCliente.sPais, ifciCliente.isDivisionGeo1, ifciCliente.isDivisionGeo2, ifciCliente.isRegimen )
                            */
                            if (!string.IsNullOrEmpty(this.Configuracion.Pais.Codigo) && !string.IsNullOrEmpty(this.Configuracion.Pais.DivisionGeografica1) && !string.IsNullOrEmpty(this.Configuracion.Pais.DivisionGeografica2))
                            {
                                bOk = this.Detalles.Lista[lnIndiceArticulo].CalcularRetencion(this.Configuracion.Pais.Codigo, this.Configuracion.Pais.DivisionGeografica1, this.Configuracion.Pais.DivisionGeografica2, this.Configuracion.ClienteCia.Regimen, this.Configuracion.Compania.PaisInstalado, this.Compania);
                            }
                            if (bOk)
                            {
                                if ((this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.nMonto > 0))
                                {
                                    // MRL Ajustes Colombia R5 -->
                                    lnIndice = 0;
                                    lbMismoArticulo = false;
                                    while ((lnIndice < lnCantidad))
                                    {
                                        if ((lsArrTempRetModArt[lnIndice] == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion))
                                        {
                                            lbMismoArticulo = true;
                                            break;
                                        }
                                        lnIndice = (lnIndice + 1);
                                    } // while
                                    // MRL Ajustes Colombia R5 <--
                                    if (!(lbMismoArticulo))
                                    {
                                        // Recorremos el arreglo de retenciones en caso de que ya exista la retención
                                        lnIndice = 0;
                                        lbRetencionEncontrada = false;
                                        foreach (fclsRetencion ret in iArregloRetenciones)
                                        {
                                            if ((ret.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion))
                                            {
                                                lbRetencionEncontrada = true;
                                                break;
                                            }
                                        } // while
                                        // MRL Cambios Colombia R5 -->
                                        /*
                                            // If lbRetencionEncontrada
                                            // Agregamos el monto a la retención que existe ya en el arreglo
                                            // Else
                                            // Insertamos la retención al final del arreglo
                                            // Calculamos el total de retenciones del documento
                                            // Set inTotalRetenciones = inTotalRetenciones + fciLineasDeDocumento [ lnIndiceArticulo ].iRetencionAsociada.nMonto
                                        */
                                        if (!(lbRetencionEncontrada))
                                        {
                                            // Insertamos la retención al final del arreglo
                                            iArregloRetenciones.Add(this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada);
                                            if (pbEsDevolucion)
                                            {
                                                iArregloRetenciones.Find(x => x.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion).sDocReferencia = ("RED-#" + (lnCantidad + 1).ToString());
                                            }
                                            else
                                            {
                                                iArregloRetenciones.Find(x => x.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion).sDocReferencia = ("RET-#" + (lnCantidad + 1).ToString());
                                            }
                                            // Calculamos el total de retenciones del documento
                                            inTotalRetenciones = (inTotalRetenciones + this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.nMonto);
                                            lnCantidad = (lnCantidad + 1);
                                        }
                                        else
                                        {
                                            if ((lbRetencionEncontrada && (lnLineaAnt != null)))
                                            {
                                                if ((lnIndiceArticulo != lnLineaAnt))
                                                {
                                                    // Agregamos el monto a la retención que existe ya en el arreglo
                                                    iArregloRetenciones.Find(x => x.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion).nMonto = iArregloRetenciones.Find(x => x.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion).nMonto + this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.nMonto;
                                                    iArregloRetenciones.Find(x => x.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion).nBaseGravada = iArregloRetenciones.Find(x => x.sCodigoRetencion == this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.sCodigoRetencion).nBaseGravada + this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.nBaseGravada;
                                                    // Calculamos el total de retenciones del documento
                                                    inTotalRetenciones = (inTotalRetenciones + this.Detalles.Lista[lnIndiceArticulo].iRetencionAsociada.nMonto);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string msj = "Error cargando retención asociada al artículo '" + this.Detalles.Lista[lnIndiceArticulo].Articulo.Codigo;
                                throw new Exception(msj);
                            }
                        }
                        lnLineaAnt = lnIndiceArticulo;
                        // MRL Ajustes Colombia R5 -->
                        // Pasamos a la siguiente línea de documento
                        lnIndiceArticulo = (lnIndiceArticulo + 1);

                    }
                    catch (Exception ex)
                    {
                        bOk = false;
                        throw ex;
                        //fciReporteErrores.AgregueError(0, "fcDocumento.CalcularRetenciones", "Error verificando las retenciones del modelo de retenciones asociado al artículo.");

                        //if (!(bDebugging))
                        //{
                        //    bOk = false;
                        //}                        
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                            reader = null;
                        }
                    }

                } // while
            }
            if (bOk)
            {
                sSqlCmd = new StringBuilder();
                sSqlCmd.AppendLine(string.Format("SELECT clr.codigo_retencion, ret.descripcion FROM {0} clr,{1} ret ", Table.ERPADMIN_CLIENTE_RETENCION, Table.ERPADMIN_RETENCIONES));
                sSqlCmd.AppendLine(string.Format("WHERE clr.cliente = '{0}'  AND ret.estado = '{1}'", lsCliente, fclsRetencion.CC_RET_ACTIVA));
                sSqlCmd.AppendLine(string.Format(" AND clr.codigo_retencion = ret.codigo_retencion AND ret.cod_cia='{0}' AND ret.cod_cia=clr.cod_cia UNION ALL SELECT ret.codigo_retencion, ret.descripcion FROM ", this.Compania));
                sSqlCmd.AppendLine(string.Format(" {0} clt,", Table.ERPADMIN_CLIENTE_CIA));
                sSqlCmd.AppendLine(string.Format(" {0} det,", Table.ERPADMIN_DET_MOD_RETENCION));
                sSqlCmd.AppendLine(string.Format(" {0} ret,", Table.ERPADMIN_RETENCIONES));
                sSqlCmd.AppendLine(string.Format(" {0} reg,", Table.ERPADMIN_REGIMENES));
                sSqlCmd.AppendLine(string.Format(" {0} er", Table.ERPADMIN_EXCEP_REGIMEN));
                sSqlCmd.AppendLine(string.Format(" WHERE clt.cod_clt = '{0}' AND ret.estado = '{1}' ", lsCliente, fclsRetencion.CC_RET_ACTIVA));
                sSqlCmd.AppendLine(string.Format(" AND det.aplica IN ('{0}','{1}') ", fclsRetencion.AS_AMBAS, fclsRetencion.AS_VENTA));
                sSqlCmd.AppendLine(" AND det.codigo_retencion = ret.codigo_retencion AND clt.regimen_trib = reg.regimen AND reg.modelo_retencion = det.modelo_retencion AND er.codigo = reg.regimen AND er.codigo_retencion = ret.codigo_retencion ");
                sSqlCmd.AppendLine(string.Format(" AND ret.cod_cia='{0}' AND det.cod_cia=ret.cod_cia AND reg.cod_cia=ret.cod_cia AND er.cod_cia=ret.cod_cia", this.Compania));

                try
                {
                    //bOk = SqlPrepareAndExecute(hsqlPrmQuery, sSqlCmd, this, local);
                    reader = GestorDatos.EjecutarConsulta(sSqlCmd.ToString());
                    if (bOk)
                    {
                        lnIndice = 0;
                        // NBH >> Ajustes Colombia R5
                        /*
                            // Set lnCantidad = 0
                        */
                        while (reader.Read())
                        {
                            lsCodigoRetencion = reader.GetString(0);
                            lsDescripcionRetencion = reader.GetString(1);
                            //TODO
                            fciRetencion.ReadOnKey(lsCodigoRetencion);
                            // MRL Proyecto Colombia R4 --> Se obtiene el monto base de la retención
                            if (!string.IsNullOrEmpty(fciRetencion.isRetencionBase))
                            {
                                __ObtenerMontoBaseRet(ref lnMontoRetBase, fciRetencion.isRetencionBase, lnCantidad);
                            }
                            fciRetencion.SetDatosCalcularMonto(lnMontoRetBase, this.Configuracion.Pais.Codigo, this.Configuracion.Pais.DivisionGeografica1, this.Configuracion.Pais.DivisionGeografica2, this.Configuracion.ClienteCia.Regimen);
                            // MRL Proyecto Colombia R4 <--
                            lnMontoRetencion = fciRetencion.CalcularMonto(lnMontoDoc, lnSubTotal, lnDescuento, lnImpuesto1, lnImpuesto2, lnRubro1, lnRubro2, 1, ref lnBaseRetencion, this.Configuracion.Compania.PaisInstalado);

                            if ((lnMontoRetencion > 0))
                            {
                                // NBH >> Ajustes Colombia R5
                                // Recorremos el arreglo de retenciones en caso de que ya exista la retencion
                                lnIndice = 0;
                                lbRetencionEncontrada = false;
                                foreach (fclsRetencion ret in iArregloRetenciones)
                                {
                                    if ((ret.sCodigoRetencion == lsCodigoRetencion))
                                    {
                                        lbRetencionEncontrada = true;
                                        break;
                                    }
                                } // while
                                // NBH << Ajustes Colombia R5
                                // NBH >> Ajustes Colombia R5 >> Se condiciona la inserción de la retención en el arreglo
                                if (!(lbRetencionEncontrada))
                                {
                                    fclsRetencion newReten = new fclsRetencion(this.Compania);
                                    newReten.sCodigoRetencion = lsCodigoRetencion;
                                    newReten.sRetencion = lsDescripcionRetencion;
                                    newReten.nMonto = lnMontoRetencion;
                                    newReten.nBaseGravada = lnBaseRetencion;
                                    newReten.sAutoretenedora = fciRetencion.sEsAutoretenedor;
                                    newReten.sTipo = fciRetencion.sTipo;
                                    if (pbEsDevolucion)
                                    {
                                        newReten.sDocReferencia = ("RED-#" + (lnCantidad + 1).ToString());
                                    }
                                    else
                                    {
                                        newReten.sDocReferencia = ("RET-#" + (lnCantidad + 1).ToString());
                                    }
                                    if ((fciRetencion.sTipo == fclsRetencion.CP_RET_REGISTRO))
                                    {
                                        newReten.sEstado = fclsRetencion.CC_RET_APLICADA;
                                    }
                                    else
                                    {
                                        newReten.sEstado = fclsRetencion.CC_RET_PENDIENTE;
                                    }
                                    iArregloRetenciones.Add(newReten);

                                    inTotalRetenciones = (inTotalRetenciones + lnMontoRetencion);
                                    lnCantidad = (lnCantidad + 1);
                                }
                            }
                            lnIndice = (lnIndice + 1);
                        } // while
                    }
                }
                catch (Exception ex)
                {
                    bOk = false;
                    throw ex;
                    //fciReporteErrores.AgregueError(0, "fcDocumento.CalcularRetenciones", "Error verificando las retenciones del modelo de retenciones asociado al Cliente.");

                    //if (!(bDebugging))
                    //{
                    //    bOk = false;
                    //}                        
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
            //fciRetencion.ReleInstance();
            return bOk;
        }
        #endregion

        private void GuardarRetenciones()
        {
            string comando = string.Empty;
            try
            {
                foreach (fclsRetencion ret in iArregloRetenciones)
                {
                    comando = "INSERT INTO " + Table.ERPADMIN_DOC_FR_RETENCION + "(DOCUMENTO,COD_CIA,TIPO,COD_ZON,CODIGO_RETENCION,AUTORETENEDORA,MONTO,BASE,DOC_REFERENCIA,TIPO_DOC_RETENCION)" +
                        " VALUES( @DOCUMENTO,@CIA,@TIPO,@RUTA,@COD_RETENCION,@AUTORETENEDORA,@MONTO,@BASE,@DOC_REF,@TIPO_DOC_RETENCION)";
                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] 
                    {
                        new SQLiteParameter("@DOCUMENTO",SqlDbType.VarChar,this.Numero),
                        new SQLiteParameter("@CIA",SqlDbType.VarChar,this.Compania),
                        new SQLiteParameter("@TIPO",SqlDbType.VarChar,ret.sTipo),
                        new SQLiteParameter("@RUTA",SqlDbType.VarChar,this.Zona),
                        new SQLiteParameter("@COD_RETENCION",SqlDbType.VarChar,ret.sCodigoRetencion),
                        new SQLiteParameter("@AUTORETENEDORA",SqlDbType.VarChar,ret.sAutoretenedora),
                        new SQLiteParameter("@MONTO",SqlDbType.Decimal,ret.nMonto),
                        new SQLiteParameter("@BASE",SqlDbType.Decimal,ret.nBaseGravada),
                        new SQLiteParameter("@DOC_REF",SqlDbType.VarChar,ret.sDocReferencia),
                        new SQLiteParameter("@TIPO_DOC_RETENCION",SqlDbType.VarChar,"F"),
                    });

                    int registro=GestorDatos.EjecutarComando(comando, parametros);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error guardando las retenciones del documento." + ex.Message);
            }

        }

        #endregion Retenciones


        #region Acceso Datos

        public static List<Pedido> ObtenerVentas(Cliente cliente, string zona, EstadoPedido estado, TipoPedido tipo)
        {
            List<Pedido> ventas = new List<Pedido>();
            ventas.Clear();

            string sentencia =
                " SELECT COD_CIA,NUM_PED,MON_SIV,MON_DSC,MON_IMP_VT,MON_IMP_CS,LST_PRE,"+
                "        FEC_PED,FEC_DES,HOR_INI,HOR_FIN,DESC1,DESC2,MONT_DESC1,MONT_DESC2,DIR_ENT,"+
                "        COD_PAIS,CLASE,COD_CND,COD_BOD,OBS_PED,DESCUENTO_CASCADA,IMPRESO,CONSIGNACION, NCF_PREFIJO, NCF, NIVEL_PRECIO, MONEDA " +
                " FROM " + Table.ERPADMIN_alFAC_ENC_PED  + 
                " WHERE COD_CLT = @CLIENTE" +
                " AND   COD_ZON = @ZONA" +
                " AND   TIP_DOC = @TIPO" +
                " AND   ESTADO = @ESTADO" +
                " AND   DOC_PRO IS NULL ";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList( new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente.Codigo),
                      new SQLiteParameter("@ZONA", zona),
                      new SQLiteParameter("@TIPO", ((char)tipo).ToString()),
                      new SQLiteParameter("@ESTADO", ((char)estado).ToString())}); 
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Pedido pedido = new Pedido();
                    pedido.Cliente = cliente.Codigo;
                    pedido.Estado = estado;
                    pedido.Tipo = tipo;
                    pedido.Zona = zona;
                    pedido.Compania = reader.GetString(0);
                    pedido.Numero = reader.GetString(1);
                    pedido.MontoBruto = reader.GetDecimal(2);
                    pedido.MontoDescuentoLineas = reader.GetDecimal(3);
                    pedido.Impuesto = new Impuesto(reader.GetDecimal(4), reader.GetDecimal(5));
                    pedido.NivelPrecio = reader.GetInt32(6);
                    pedido.FechaRealizacion = reader.GetDateTime(7);
                    pedido.FechaEntrega = reader.GetDateTime(8);
                    pedido.HoraInicio = reader.GetDateTime(9);
                    pedido.HoraFin = reader.GetDateTime(10);
                    pedido.PorcentajeDescuento1 = reader.GetDecimal(11);
                    pedido.PorcentajeDescuento2 = reader.GetDecimal(12);
                    pedido.MontoDescuento1 = reader.GetDecimal(13);
                    pedido.MontoDescuento2 = reader.GetDecimal(14);
                    pedido.DireccionEntrega = reader.GetString(15);
                    pedido.Configuracion = new ConfigDocCia();
                    pedido.Configuracion.Compania.Codigo = pedido.Compania;
                    pedido.Configuracion.Pais.Codigo = reader.GetString(16);
                    pedido.Configuracion.Clase = (ClaseDoc)Convert.ToChar(reader.GetString(17));
                    pedido.Configuracion.CondicionPago.Codigo = reader.GetString(18);
                    pedido.Configuracion.Nivel.Codigo = pedido.NivelPrecio;
                    pedido.Configuracion.ClienteCia = cliente.ObtenerClienteCia(pedido.Compania);

                    pedido.Bodega = reader.GetString(19);
                    if (!reader.IsDBNull(20))
                        pedido.Notas = reader.GetString(20);
                    pedido.DescuentoCascada = reader.GetString(21).Equals("S");				
                    pedido.Impreso = reader.GetString(22).Equals("S");
                    pedido.EsConsignacion = reader.GetString(23).Equals("S");
                    //if (!reader.IsDBNull(21))
                    //    pedido.NumRef = reader.GetString(20);

                    if (!reader.IsDBNull(24) && !reader.IsDBNull(25))
                    {
                        pedido.NCF = new NCFBase();
                        pedido.NCF.Prefijo = reader.GetString(24);
                        pedido.NCF.UltimoValor = reader.GetString(25);
                    }

                    pedido.Configuracion.Cargar();

                    ventas.Add(pedido);
                }
                return ventas;
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Error cargando " + ((tipo == TipoPedido.Factura) ? "facturas" : "pedidos") +"  del cliente. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        

        private void DBGuardar()
        {
            this.HoraFin = DateTime.Now;
            string serie = string.Empty;
            if (ResolucionUtilitario.usaResolucionFactura(this.Zona) && this.ConsecResolucion!=null)
            {
                serie = this.ConsecResolucion.Serie;
            }
            if (this.Configuracion.Compania.Retenciones)
            {
 
            }

            // PA2-01482-MKP6 - KFC
            // decimal montoCIV = Decimal.Round(this.MontoBruto + this.Impuesto.MontoImpuesto1, 2);
            decimal montoCIV = Decimal.Round(this.MontoBruto + this.Impuesto.MontoImpuesto1, 5);
            
            //Se procede a guardar el encabezado
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_ENC_PED +
                "        ( COD_CIA,  COD_ZON,  COD_CLT,  NUM_PED,  HOR_INI,  HOR_FIN,  FEC_PED,  FEC_DES,  NUM_ITM,  ESTADO, COD_BOD,  TIP_DOC,  LST_PRE,  MON_SIV,  MON_CIV,  MON_DSC,  MON_IMP_VT,  MON_IMP_CS,  IMPRESO,  CONSIGNACION,   OBS_PED,  DIR_ENT,   COD_PAIS,  CLASE, COD_CND,  DESC1,  DESC2,  MONT_DESC1,  MONT_DESC2,  DESCUENTO_CASCADA, NCF_PREFIJO, NCF, NIVEL_PRECIO, MONEDA,SERIE_RESOLUCION,COD_GEO1,COD_GEO2) " +
                " VALUES (@COD_CIA, @COD_ZON, @COD_CLT, @NUM_PED, @HOR_INI, @HOR_FIN, @FEC_PED, @FEC_DES, @NUM_ITM, @ESTADO, @COD_BOD, @TIP_DOC, @NVL_PRE, @MON_SIV, @MON_CIV, @MON_DSC, @MON_IMP_VT, @MON_IMP_CS, @IMPRESO, @CONSIGNACION,  @OBS_PED, @DIR_ENT, @COD_PAIS, @CLASE, @COD_CND, @DESC1, @DESC2, @MONT_DESC1, @MONT_DESC2, @DESCUENTO_CASCADA, @NCF_PREFIJO, @NCF, @NVL_PRE_COD, @MONEDA,@SERIE_RESOLUCION,@GEO1,@GEO2)";  //DOC_PRO 

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_PED",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, HoraInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, HoraFin), //LJR Caso 36309
                GestorDatos.SQLiteParameter("@FEC_PED",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),
                GestorDatos.SQLiteParameter("@FEC_DES",SqlDbType.DateTime, FechaEntrega.ToShortDateString()),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.TotalLineas),
                GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar,((char)Estado).ToString()),
                GestorDatos.SQLiteParameter("@COD_BOD",SqlDbType.NVarChar, (EsConsignacion ? Configuracion.ClienteCia.Bodega :this.Bodega)),
                GestorDatos.SQLiteParameter("@TIP_DOC",SqlDbType.NVarChar, ((char)Tipo).ToString()),
                GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int, Configuracion.Nivel.Codigo),
                GestorDatos.SQLiteParameter("@MON_SIV",SqlDbType.Decimal, MontoBruto),
                GestorDatos.SQLiteParameter("@MON_CIV",SqlDbType.Decimal, montoCIV),
                GestorDatos.SQLiteParameter("@MON_DSC",SqlDbType.Decimal, MontoDescuentoLineas),
                GestorDatos.SQLiteParameter("@MON_IMP_VT",SqlDbType.Decimal,Impuesto.MontoImpuesto1),
                GestorDatos.SQLiteParameter("@MON_IMP_CS", SqlDbType.Decimal,Impuesto.MontoImpuesto2),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, (Impreso? "S" : "N")),
                GestorDatos.SQLiteParameter("@CONSIGNACION",SqlDbType.NVarChar, (EsConsignacion ? "S" : "N")),
                GestorDatos.SQLiteParameter("@OBS_DET",SqlDbType.NVarChar, Notas), 
                GestorDatos.SQLiteParameter("@DIR_ENT", SqlDbType.NVarChar, DireccionEntrega),
                GestorDatos.SQLiteParameter("@COD_PAIS", SqlDbType.NVarChar, Configuracion.Pais.Codigo),
                GestorDatos.SQLiteParameter("@CLASE", SqlDbType.NVarChar, ((char)Configuracion.Clase).ToString()),
                GestorDatos.SQLiteParameter("@COD_CND", SqlDbType.NVarChar, Configuracion.CondicionPago.Codigo),
                GestorDatos.SQLiteParameter("@DESC1",SqlDbType.Decimal, PorcentajeDescuento1), 
                GestorDatos.SQLiteParameter("@DESC2",SqlDbType.Decimal, PorcentajeDescuento2), 
                GestorDatos.SQLiteParameter("@MONT_DESC1",SqlDbType.Decimal,MontoDescuento1), 
                GestorDatos.SQLiteParameter("@MONT_DESC2",SqlDbType.Decimal,MontoDescuento2),
                GestorDatos.SQLiteParameter("@OBS_PED", SqlDbType.NVarChar, Notas),
                GestorDatos.SQLiteParameter("@DESCUENTO_CASCADA", SqlDbType.NVarChar,(DescuentoCascada? "S" : "N" )),
                //ABC Manejo NCF
                GestorDatos.SQLiteParameter("@NCF_PREFIJO", SqlDbType.NVarChar,(NCF != null ? NCF.Prefijo : string.Empty)),
                GestorDatos.SQLiteParameter("@NCF", SqlDbType.NVarChar,(NCF != null ? NCF.UltimoValor : string.Empty)),

                //KFC Sincronización Listas de precio
                GestorDatos.SQLiteParameter("@NVL_PRE_COD",SqlDbType.NVarChar, NivelPrecioCod),
                GestorDatos.SQLiteParameter("@MONEDA",SqlDbType.NVarChar, Moneda),
                //Resoluciones
                GestorDatos.SQLiteParameter("@SERIE_RESOLUCION",SqlDbType.NVarChar, serie),
                //Retenciones
                GestorDatos.SQLiteParameter("@GEO1",SqlDbType.NVarChar, this.Configuracion.Pais.DivisionGeografica1),
                GestorDatos.SQLiteParameter("@GEO2",SqlDbType.NVarChar, this.Configuracion.Pais.DivisionGeografica2)

            });
                

                //GestorDatos.SQLiteParameter("@NUM_REF",SqlDbType.NVarChar, numRef),
            int encabezado = GestorDatos.EjecutarComando(sentencia, parametros);

            if (encabezado != 1)
                throw new Exception("No se puede guardar el encabezado " +(Tipo.Equals(TipoPedido.Factura)? "de la factura '": "del pedido'" ) + this.Numero + "'.");

            //se guardan los detalles de Pedido
            detalles.Bodega = Bodega;
            detalles.NivelPrecio = Configuracion.Nivel.Codigo;
            detalles.Tipo = Tipo;
            detalles.Encabezado = this;
            detalles.Guardar();

            //Guarda las retenciones
            if (iArregloRetenciones!=null && iArregloRetenciones.Count > 0)
            {
                this.GuardarRetenciones();
            }

            GenerarPendientesCobro();
        }        

        private void DBGuardarTomaFisica()
        {
            this.HoraFin = DateTime.Now;
            string serie = string.Empty;
            if (ResolucionUtilitario.usaResolucionFactura(this.Zona) && this.ConsecResolucion != null)
            {
                serie = this.ConsecResolucion.Serie;
            }

            // PA2-01482-MKP6 - KFC
            // decimal montoCIV = Decimal.Round(this.MontoBruto + this.Impuesto.MontoImpuesto1, 2);
            decimal montoCIV = Decimal.Round(this.MontoBruto + this.Impuesto.MontoImpuesto1, 5);

            //Se procede a guardar el encabezado
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_ENC_PED +
                "        ( COD_CIA,  COD_ZON,  COD_CLT,  NUM_PED,  HOR_INI,  HOR_FIN,  FEC_PED,  FEC_DES,  NUM_ITM,  ESTADO, COD_BOD,  TIP_DOC,  LST_PRE,  MON_SIV,  MON_CIV,  MON_DSC,  MON_IMP_VT,  MON_IMP_CS,  IMPRESO,  CONSIGNACION,   OBS_PED,  DIR_ENT,   COD_PAIS,  CLASE, COD_CND,  DESC1,  DESC2,  MONT_DESC1,  MONT_DESC2,  DESCUENTO_CASCADA, NCF_PREFIJO, NCF, NIVEL_PRECIO, MONEDA,SERIE_RESOLUCION) " +
                " VALUES (@COD_CIA, @COD_ZON, @COD_CLT, @NUM_PED, @HOR_INI, @HOR_FIN, @FEC_PED, @FEC_DES, @NUM_ITM, @ESTADO, @COD_BOD, @TIP_DOC, @NVL_PRE, @MON_SIV, @MON_CIV, @MON_DSC, @MON_IMP_VT, @MON_IMP_CS, @IMPRESO, @CONSIGNACION,  @OBS_PED, @DIR_ENT, @COD_PAIS, @CLASE, @COD_CND, @DESC1, @DESC2, @MONT_DESC1, @MONT_DESC2, @DESCUENTO_CASCADA, @NCF_PREFIJO, @NCF, @NVL_PRE_COD, @MONEDA,@SERIE_RESOLUCION)";  //DOC_PRO 

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_PED",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, HoraInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, HoraFin), //LJR Caso 36309
                GestorDatos.SQLiteParameter("@FEC_PED",SqlDbType.DateTime, FechaRealizacion.ToShortDateString()),
                GestorDatos.SQLiteParameter("@FEC_DES",SqlDbType.DateTime, FechaEntrega.ToShortDateString()),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.TotalLineas),
                GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar,((char)Estado).ToString()),
                GestorDatos.SQLiteParameter("@COD_BOD",SqlDbType.NVarChar, (EsConsignacion ? Configuracion.ClienteCia.Bodega :this.Bodega)),
                GestorDatos.SQLiteParameter("@TIP_DOC",SqlDbType.NVarChar, ((char)Tipo).ToString()),
                GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int, Configuracion.Nivel.Codigo),
                GestorDatos.SQLiteParameter("@MON_SIV",SqlDbType.Decimal, MontoBruto),
                GestorDatos.SQLiteParameter("@MON_CIV",SqlDbType.Decimal, montoCIV),
                GestorDatos.SQLiteParameter("@MON_DSC",SqlDbType.Decimal, MontoDescuentoLineas),
                GestorDatos.SQLiteParameter("@MON_IMP_VT",SqlDbType.Decimal,Impuesto.MontoImpuesto1),
                GestorDatos.SQLiteParameter("@MON_IMP_CS", SqlDbType.Decimal,Impuesto.MontoImpuesto2),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, (Impreso? "S" : "N")),
                GestorDatos.SQLiteParameter("@CONSIGNACION",SqlDbType.NVarChar, (EsConsignacion ? "S" : "N")),
                GestorDatos.SQLiteParameter("@OBS_DET",SqlDbType.NVarChar, Notas), 
                GestorDatos.SQLiteParameter("@DIR_ENT", SqlDbType.NVarChar, DireccionEntrega),
                GestorDatos.SQLiteParameter("@COD_PAIS", SqlDbType.NVarChar, Configuracion.Pais.Codigo),
                GestorDatos.SQLiteParameter("@CLASE", SqlDbType.NVarChar, ((char)Configuracion.Clase).ToString()),
                GestorDatos.SQLiteParameter("@COD_CND", SqlDbType.NVarChar, Configuracion.CondicionPago.Codigo),
                GestorDatos.SQLiteParameter("@DESC1",SqlDbType.Decimal, PorcentajeDescuento1), 
                GestorDatos.SQLiteParameter("@DESC2",SqlDbType.Decimal, PorcentajeDescuento2), 
                GestorDatos.SQLiteParameter("@MONT_DESC1",SqlDbType.Decimal,MontoDescuento1), 
                GestorDatos.SQLiteParameter("@MONT_DESC2",SqlDbType.Decimal,MontoDescuento2),
                GestorDatos.SQLiteParameter("@OBS_PED", SqlDbType.NVarChar, Notas),
                GestorDatos.SQLiteParameter("@DESCUENTO_CASCADA", SqlDbType.NVarChar,(DescuentoCascada? "S" : "N" )),
                //ABC Manejo NCF
                GestorDatos.SQLiteParameter("@NCF_PREFIJO", SqlDbType.NVarChar,(NCF != null ? NCF.Prefijo : string.Empty)),
                GestorDatos.SQLiteParameter("@NCF", SqlDbType.NVarChar,(NCF != null ? NCF.UltimoValor : string.Empty)),

                //KFC Sincronización Listas de precio
                GestorDatos.SQLiteParameter("@NVL_PRE_COD",SqlDbType.NVarChar, NivelPrecioCod),
                GestorDatos.SQLiteParameter("@MONEDA",SqlDbType.NVarChar, Moneda),
                //Resoluciones
                GestorDatos.SQLiteParameter("@SERIE_RESOLUCION",SqlDbType.NVarChar, serie)});


            //GestorDatos.SQLiteParameter("@NUM_REF",SqlDbType.NVarChar, numRef),
            int encabezado = GestorDatos.EjecutarComando(sentencia, parametros);

            if (encabezado != 1)
                throw new Exception("No se puede guardar el encabezado " + (Tipo.Equals(TipoPedido.Factura) ? "de la factura '" : "del pedido'") + this.Numero + "'.");

            //se guardan los detalles de Pedido
            detalles.Bodega = Bodega;
            detalles.NivelPrecio = Configuracion.Nivel.Codigo;
            detalles.Tipo = Tipo;
            detalles.Encabezado = this;
            detalles.GuardarTomaFisica();

            GenerarPendientesCobro();
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
                    " SELECT COUNT(NUM_PED) FROM " + Table.ERPADMIN_alFAC_ENC_PED +
                    " WHERE NUM_PED = @CONSECUTIVO " +
                    " AND UPPER(COD_CIA) = @COMPANIA";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar,consecutivo),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Compania.ToUpper())});
                existe = (int)GestorDatos.EjecutarScalar(sentencia, parametros);

                if (existe != 0)
                {
                    Bitacora.Escribir("El consecutivo '" + consecutivo + "' ya existe.");
                    consecutivo = GestorUtilitario.proximoCodigo(consecutivo, 20);
                }

            } while (existe != 0);

            return consecutivo;
        }

        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_PED + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);     
        }

        /// <summary>
        ///  Carga de Historicos de los pedidos
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
                throw new Exception("Error al cargar los historicos de los pedidos. " + ex.Message);
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
                throw new Exception("Error al cargar los historicos de los pedidos. " + ex.Message);
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
            if ((this.CondicionPago!=null && this.CondicionPago.DiasNeto==0) ||this.configuracion.CondicionPago.Codigo == Corporativo.Compania.Obtener(this.Compania).CondicionPagoContado
                )
            {
                return "Contado";
            }
            else
            {
                try
                {
                    //Corporativo.CondicionPago.ObtenerDiasNeto(condicionPago,compania)
                    //return "Crédito a " + this.configuracion.CondicionPago.DiasNeto + " día(s)";
                    if (this.CondicionPago != null)
                    {
                        return "Crédito a " + this.CondicionPago.DiasNeto + " día(s)";
                    }
                    else
                    {
                        return "Crédito a " + this.Configuracion.CondicionPago.DiasNeto + " día(s)";
                    }
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
                    double dias=0;
                    if (this.CondicionPago != null)
                    {
                        dias = (double)(this.CondicionPago.DiasNeto);
                    }
                    else
                    {
                        dias = (double)(this.configuracion.CondicionPago.DiasNeto);
                    }
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
                " WHERE NUM_DOC_AF = (@FACTURA)";


            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@FACTURA", this.Numero)});
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
                throw new Exception("Error verificando si existen documentos asociados a la Factura. " + ex.Message);
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


        #endregion

        #endregion

    }
}
