using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using EMF.Printing;
using System.Collections;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion
{
    /// <summary>
    /// Representa una venta en consignacion de un cliente en una compania determinada
    /// </summary>
    public class VentaConsignacion : EncabezadoDocumento, IPrintable
    {
        #region Variables y Propiedades de instancia

        public DetallesVenta detalles = new DetallesVenta();
        /// <summary>
        /// Detalles de la Venta Consignacion
        /// </summary>
        public virtual DetallesVenta Detalles
        {
            get { return detalles; }
            set { detalles = value; }
        }

        #region Configuracion Previa a la Toma

        private ConfigDocCia configuracion;
        /// <summary>
        /// Configuracion de Pago
        /// </summary>
        public ConfigDocCia Configuracion
        {
            get { return configuracion; }
            set { configuracion = value; }
        }

        #endregion

        private string bodegaConsigna = string.Empty;
        /// <summary>
        /// Bodega de cliente consignatario
        /// </summary>
        public string BodegaConsigna
        {
            get { return bodegaConsigna; }
            set { bodegaConsigna = value; }
        }

        private EstadoConsignacion estado = EstadoConsignacion.NoSincronizada;
        /// <summary>
        /// Indica el estado de la venta en consignación.
        /// </summary>
        public EstadoConsignacion Estado
        {
            get { return estado; }
            set { estado = value; }
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

        #region Descuento

        private bool descuentoCascada;
        /// <summary>
        /// Indica si la venta es realizada con descuento en cascada.
        /// </summary>
        public bool DescuentoCascada
        {
            get { return descuentoCascada; }
            set { descuentoCascada = value; }
        }

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
        /// Porcentaje de descuento general asignado a la venta en consignación.
        /// PorcentajeDescuento 1 + PorcentajeDescuento 2.
        /// </summary>
        private decimal PorcDescGeneral
        {
            get
            {
                if (this.DescuentoCascada)
                {
                    if (MontoBruto > Decimal.Zero)
                    {
                        decimal porcentaje = ((MontoBruto * (this.PorcentajeDescuento1 / 100)) + ((MontoBruto - (MontoBruto * (this.PorcentajeDescuento1 / 100))) * (this.PorcentajeDescuento2/ 100))) / MontoBruto;
                        return porcentaje;
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

        #endregion

        #endregion

        #region Constructor

        public VentaConsignacion() 
        {
            this.FechaRealizacion = this.HoraInicio = DateTime.Now;
            this.Numero = "CONSIG" + this.FechaRealizacion.ToShortDateString().Replace("/", "") + this.HoraInicio.ToLongTimeString().Substring(0, 7).Replace(":", "");        
        }
        public VentaConsignacion(ConfigDocCia configuracion,string bodega, string zona)
        {
            this.FechaRealizacion = this.HoraInicio = this.HoraFin = DateTime.Now;
            this.Numero = "CONSIG" + this.FechaRealizacion.ToShortDateString().Replace("/", "") + this.HoraInicio.ToLongTimeString().Substring(0, 7).Replace(":", "");        
            this.configuracion = configuracion;
            this.Compania = configuracion.Compania.Codigo;
            this.Zona = zona;
            this.Cliente = this.Configuracion.ClienteCia.Codigo;
            this.DescuentoCascada = this.Configuracion.Compania.DescuentoCascada;
            this.PorcentajeDescuento1 = this.Configuracion.ClienteCia.Descuento;
            this.NivelPrecio = this.Configuracion.Nivel.Codigo;
            this.BodegaConsigna = this.Configuracion.ClienteCia.Bodega;
            this.Bodega = bodega;
            this.Estado = EstadoConsignacion.NoSincronizada;
        }
      
        #endregion

        #region Logica Negocio

        /// <summary>
        /// Guarda en la base de datos la venta en consignación.
        /// </summary>
        public void Guardar(bool commit)
        {
            try
            {
                GestorDatos.BeginTransaction();

                int cantidadDetalles = ObtenerCantidadDetallesVenta();
                detalles.Encabezado = this;

                //existeVentaConsignacion
                if (cantidadDetalles>0)
                    Restablecer(cantidadDetalles);

                DBGuardar(false);

                if (commit)
                    GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al guardar venta en consignación. "+ ex.Message);
            }
        }

        /// <summary>
        /// Si existe consignacion antes de guardar, restable existencias de articulos y elimina
        /// </summary>
        /// <param name="cantidadDetalles"></param>
        private void Restablecer(int cantidadDetalles)
        {
            //Caso 32007 LDS 07/04/2008
            //Restablecemos las existencias de la venta en consignación antes de eliminarla.
            Bitacora.Escribir("Se restablece las existencias de los detalles de la venta en consignación para la compañía '" + this.Compania + "', ruta '" + this.Zona + "' y cliente '" + this.Cliente + "' por causa de la modificación de sus detalles.");
            detalles.RestablecerExistenciasConsignacion(this.Bodega, cantidadDetalles);
            Bitacora.Escribir("Se restablece exitosamente las existencias de los detalles de la venta en consignación.");

            Bitacora.Escribir("Se eliminará la venta en consignación para la compañía '" + this.Compania + "', ruta '" + this.Zona + "' y cliente '" + this.Cliente + "' por causa de la modificación de sus detalles.");
            DBEliminar(cantidadDetalles);
            Bitacora.Escribir("Se eliminó exitosamente la venta en consignación por lo que se procede a guardar la nueva venta en consignación.");

        }

        /// <summary>
        /// Elimina la boleta de venta en consignación y las líneas respectivas.
        /// Aumenta las existencias en la bodega ya que la boleta de venta en consignación no ha sido procesada.
        /// </summary>
        public void EliminarBoletaNoProcesada()
        {
            if (this.Estado == EstadoConsignacion.Sincronizada)
                throw new Exception("La boleta de venta en consignación para la compañía '" + this.Compania + "', ruta '" + this.Zona+ "' y cliente '" + this.Cliente + "' ya ha sido sincronizada.");

            if (this.Estado == EstadoConsignacion.Procesada)
                throw new Exception("La boleta de venta en consignación para la compañía '" + this.Compania + "', ruta '" + this.Zona + "' y cliente '" + this.Cliente + "' ya ha sido procesada.");

            try
            {
                GestorDatos.BeginTransaction();

                Devoluciones devoluciones = new Devoluciones();
                foreach (DetalleVenta detalleBoleta in this.Detalles.Lista)
                {
                    this.GenerarDevolucion(detalleBoleta, devoluciones);
                    this.ActualizarExistenciasEliminar(detalleBoleta);
                }
                this.GuardarDevolucionesGeneradas(devoluciones);
                DBEliminar(this.Detalles.Lista.Count);

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                throw new Exception("Error al eliminar la boleta de venta en consignación no procesada para la compañía '" + Compania + "', ruta '" + Zona + "' y cliente '" + Cliente + "'. " + ex.Message);
            }
        }

        /// <summary>
        /// Genera una devolución basado en el Detalle de la venta en consignación.
        /// </summary>
        /// <param name="detalle"></param>
        /// <param name="devoluciones"></param>
        public void GenerarDevolucion(DetalleVenta detalle, Devoluciones devoluciones)
        {
            // Si detalle.TotalAlmacenSaldo la consignación es nueva por lo que no se realizan devoluciones.
            if (detalle.TotalAlmacenSaldo == 0)
                return;

            Articulo articulo = (Articulo)detalle.Articulo.Clone();
            articulo.Precio = new Precio();
            articulo.CargarPrecio(this.Configuracion.Nivel);
            detalle.Lote = String.Empty;


            // Se genero un desglose y se restablecio la boleta anterior.
            if (detalle.TotalAlmacenSaldo > Decimal.Zero)
            {
                devoluciones.GestionarConsignado(this.Numero, articulo, this.Configuracion.ClienteCia
                    , detalle.UnidadesAlmacenSaldo, detalle.UnidadesDetalleSaldo, FRArticulo.Estado.Bueno
                    , detalle.Lote, detalle.Observaciones, this.Bodega, this.BodegaConsigna, this.Zona);
            }
            // Se genero un desglose y se dejaron solo los saldos de la boleta anterior.
            if (detalle.TotalAlmacenSaldo < Decimal.Zero)
            {
                devoluciones.GestionarConsignado(this.Numero, articulo, this.Configuracion.ClienteCia
                    , detalle.UnidadesAlmacen, detalle.UnidadesDetalle, FRArticulo.Estado.Bueno
                    , detalle.Lote, detalle.Observaciones, this.Bodega, this.BodegaConsigna, this.Zona);
            }
        }

        /// <summary>
        /// Guarda en la base de datos las devoluciones generadas por el desglose de la boleta de venta en consignación.
        /// </summary>
        public void GuardarDevolucionesGeneradas(Devoluciones devoluciones)
        {
            foreach (Devolucion devolucion in devoluciones.Gestionados)
            {
                devolucion.Guardar(false);
            }
        }

        /// <summary>
        /// Actualiza las existencias del detalle despues de realizar devoluciones.
        /// </summary>
        /// <param name="detalle"></param>
        public void ActualizarExistenciasEliminar(DetalleVenta detalle)
        {
            detalle.Articulo.Bodega = new Bodega(this.Bodega);
            Decimal cantidad = Decimal.Zero;
            cantidad = detalle.TotalAlmacenSaldo >= 0 ? detalle.UnidadesAlmacenSaldo : detalle.UnidadesAlmacen;
            detalle.Articulo.ActualizarBodega((detalle.TotalAlmacen - cantidad));
        }

        /// <summary>
        /// Elimina un detalle de la venta en consignación.
        /// </summary>
        /// <param name="articulo">Código del artículo a eliminar.</param>
        /// <returns>
        /// Retorna la cantidad de líneas eliminadas. Puede incluir líneas bonificadas del artículo a eliminar.
        /// </returns>
        public int EliminarDetalle(string articulo)
        {
            DetalleVenta detalleEncontrado = this.detalles.Eliminar(articulo);

            if (detalleEncontrado != null)
            {
                //Hay que restar los montos de la línea.
                this.RestarMontosLinea(detalleEncontrado);
                //Actualizamos los montos de descuentos 1 y 2.
                this.ReCalcularDescuentosGenerales();
                return 1;
            }
            else
                return 0;

        }

        /// <summary>
        /// Agrega un detalle a la venta en consignación en gestión.
        /// </summary>
        /// <param name="articulo">Artículo que se va a agregar a la venta en consignación.</param>
        /// <param name="unidadesAlmacen">Cantidad en unidades de almacén a consignar.</param>
        /// <param name="unidadesDetalle">Cantidad en unidades de detalle a consignar.</param>
        /// <param name="unidadesAlmacenSaldo"></param>
        /// <param name="unidadesDetalleSaldo"></param>
        /// <param name="precio">Precio.</param>
        public void AgregarDetalle(
            Articulo articulo,
            decimal unidadesAlmacen,decimal unidadesDetalle,
            decimal unidadesAlmacenSaldo,decimal unidadesDetalleSaldo,
            Precio precio, 
            bool validarCantidadLineas)
        {
            DetalleVenta detalle = Detalles.Buscar(articulo.Codigo);

            if (detalle == null)
            {
                //Se crea un nuevo detalle si alguna cantidad es mayor a cero.
                if (unidadesAlmacen == 0 && unidadesDetalle == 0)
                    return;

                if (validarCantidadLineas && this.Detalles.Lista.Count == Pedidos.MaximoLineasDetalle)
                    throw new Exception("Se alcanzó el máximo de líneas permitidas (" + Pedidos.MaximoLineasDetalle + ")");

                detalle = new DetalleVenta();//LDA EL ARTICULO LLEGA MAL
                detalle.Articulo = (Articulo)articulo.Clone();
                detalle.CambiarValores(unidadesAlmacen, unidadesDetalle, unidadesAlmacenSaldo, unidadesDetalleSaldo,
                                        precio, configuracion.ClienteCia,
                            this.PorcDescGeneral);

                this.Detalles.Lista.Add(detalle);
                this.AgregarMontosLinea(detalle);

            }
            else
            {
                //El detalle ya se encuentra en la venta en consignación.
                if (detalle.UnidadesAlmacen != unidadesAlmacen ||
                    detalle.UnidadesDetalle != unidadesDetalle ||
                    detalle.UnidadesAlmacenSaldo != unidadesAlmacenSaldo ||
                    detalle.UnidadesDetalleSaldo != unidadesDetalleSaldo ||
                    detalle.Precio.Maximo != precio.Maximo)
                {
                    //Restamos los montos actuales de la línea.
                    this.RestarMontosLinea(detalle);

                    detalle.CambiarValores(unidadesAlmacen, unidadesDetalle, unidadesAlmacenSaldo, unidadesDetalleSaldo,
                                    precio, configuracion.ClienteCia,
                        this.PorcDescGeneral);

                    this.AgregarMontosLinea(detalle);

                }
            }
        }

        /// <summary>
        /// Recalcula los montos de descuentos 1 y 2 basados en los porcentajes de los mismos,
        /// el monto bruto del pedido y el total de descuentos por línea.
        /// </summary>
        private void ReCalcularDescuentosGenerales()
        {
            try
            {
                //Ajustamos el monto del descuento 1
                this.MontoDescuento1 = Decimal.Round(MontoBruto  * PorcentajeDescuento1 / 100, 2);
                // En caso que el parámetro "Aplicación de Descuentos generales 1 y 2 en cascada" de la compañía 
                // esté asignado el descuento se deba aplicar en cascada, de lo contrario se aplica normalmente.
                if (this.DescuentoCascada)
                    this.MontoDescuento2 = Decimal.Round((MontoBruto - MontoDescuento1) * PorcentajeDescuento2 / 100, 2);
                else
                    this.MontoDescuento2 = Decimal.Round(MontoBruto * PorcentajeDescuento2 / 100, 2);
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
        private void AgregarMontosLinea(DetalleVenta detalle)
        {
            //Actualizamos los montos.
            //this.MontoDescuento += detalle.MontoDescuento;
            this.MontoBruto += detalle.MontoTotal;
            this.Impuesto.MontoImpuesto1 += detalle.Impuesto.MontoImpuesto1;
            this.Impuesto.MontoImpuesto2 += detalle.Impuesto.MontoImpuesto2;
            ReCalcularDescuentosGenerales();
        }
        /// <summary>
        /// Resta a los montos del pedido los montos de una línea de detalle.
        /// </summary>
        /// <param name="detalle"></param>
        private void RestarMontosLinea(DetalleVenta detalle)
        {
            //this.MontoDescuento -= detalle.MontoDescuento;
            this.MontoBruto -= detalle.MontoTotal;
            this.Impuesto.MontoImpuesto1 -= detalle.Impuesto.MontoImpuesto1;
            this.Impuesto.MontoImpuesto2 -= detalle.Impuesto.MontoImpuesto2;
            ReCalcularDescuentosGenerales();
        }
        /// <summary>
        /// Calcula el impuesto 1 y el impuesto 2 basado en el descuento general y las líneas de la venta en consignación.
        /// </summary>
        public void CalcularImpuestos()
        {
            Impuesto.MontoImpuesto1 = 0;
            Impuesto.MontoImpuesto2 = 0;

            //Recorremos los detalles de la venta en consignación.
            foreach (DetalleVenta detalle in this.Detalles.Lista)
            {
                detalle.CalcularImpuestos(this.configuracion.ClienteCia.ExoneracionImp1, this.configuracion.ClienteCia.ExoneracionImp2, this.PorcDescGeneral);
                this.Impuesto.MontoImpuesto1 += detalle.Impuesto.MontoImpuesto1;
                this.Impuesto.MontoImpuesto2 += detalle.Impuesto.MontoImpuesto2;
            }
            // Caso: 27900 LDS 03/04/2007
            // Se redondea a 2 decimales ya que FA lo hace de esa manera.
            this.Impuesto.MontoImpuesto1 = Decimal.Round(this.Impuesto.MontoImpuesto1, 2);
            this.Impuesto.MontoImpuesto2 = Decimal.Round(this.Impuesto.MontoImpuesto2, 2);
        }
        #endregion
        
        #region Acceso Datos

        /// <summary>
        /// Guarda en la base de datos la venta en consignación.
        /// Caso: 32809 ABC 30/05/2008
        /// Rebaja las existencias cuando factura un desglose de boleta en Consignación
        /// Se incorpora el nuevo parametro para control de desglose.
        /// </summary>
        /// <param name="desglose">Indica si el proceso es invocado a la hora de desglose.</param>
        public void DBGuardar(bool desglose)
        {
            this.HoraFin = DateTime.Now;

            //ABC 19/08/2008 Caso:32163, 32882 , 32884 parametro NUM_CSG en la sentencia
            string sentencia =
               "INSERT INTO " + Table.ERPADMIN_alFAC_ENC_CONSIG +
               "        ( COD_CIA, COD_ZON, COD_CLT, NUM_CSG, HOR_INI, HOR_FIN, FEC_BOLETA, NUM_ITM, COD_BOD, LST_PRE, COD_CND, COD_PAIS, CLASE, OBSERVACIONES, DESC1, DESC2, MONT_DESC1, MONT_DESC2, DESCUENTO_CASCADA, MON_IMP_VT, MON_IMP_CS, MON_SIV, IMPRESO)" +
                "VALUES (@COD_CIA,@COD_ZON,@COD_CLT,@NUM_CSG,@HOR_INI,@HOR_FIN,@FEC_BOLETA,@NUM_ITM,@COD_BOD,@NVL_PRE,@COD_CND,@COD_PAIS,@CLASE,@OBSERVACIONES,@DESC1,@DESC2,@MONT_DESC1,@MONT_DESC2,@DESCUENTO_CASCADA,@MON_IMP_VT,@MON_IMP_CS,@MON_SIV,@IMPRESO)";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_CSG",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, HoraInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, HoraFin),
                GestorDatos.SQLiteParameter("@FEC_BOLETA",SqlDbType.DateTime, FechaRealizacion),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.TotalLineas),
                GestorDatos.SQLiteParameter("@COD_BOD",SqlDbType.NVarChar, this.BodegaConsigna),
                GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int, Configuracion.Nivel.Codigo),
                GestorDatos.SQLiteParameter("@COD_CND", SqlDbType.NVarChar, Configuracion.CondicionPago.Codigo),
                GestorDatos.SQLiteParameter("@COD_PAIS", SqlDbType.NVarChar, Configuracion.Pais.Codigo),
                GestorDatos.SQLiteParameter("@CLASE", SqlDbType.NVarChar, ((char)Configuracion.Clase).ToString()),
                GestorDatos.SQLiteParameter("@OBSERVACIONES",SqlDbType.NVarChar, Notas),   
                GestorDatos.SQLiteParameter("@DESC1",SqlDbType.Decimal, PorcentajeDescuento1), 
                GestorDatos.SQLiteParameter("@DESC2",SqlDbType.Decimal, PorcentajeDescuento2), 
                GestorDatos.SQLiteParameter("@MONT_DESC1",SqlDbType.Decimal,MontoDescuento1), 
                GestorDatos.SQLiteParameter("@MONT_DESC2",SqlDbType.Decimal,MontoDescuento2),
                GestorDatos.SQLiteParameter("@DESCUENTO_CASCADA", SqlDbType.NVarChar,(DescuentoCascada? "S" : "N" )),
                GestorDatos.SQLiteParameter("@MON_IMP_VT",SqlDbType.Decimal,Impuesto.MontoImpuesto1),
                GestorDatos.SQLiteParameter("@MON_IMP_CS", SqlDbType.Decimal,Impuesto.MontoImpuesto2),
                GestorDatos.SQLiteParameter("@MON_SIV",SqlDbType.Decimal, MontoBruto),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, (Impreso? "S" : "N"))});

            int encabezado = GestorDatos.EjecutarComando(sentencia, parametros);

            //se guardan los detalles de Pedido
            detalles.Encabezado = this;
            detalles.Guardar(desglose);
        }

        /// <summary>
        /// Elimina la boleta de venta en consignación y las líneas respectivas.
        /// </summary>
        /// <param name="detalles">Indica la cantidad de detalles que posee la boleta de venta en consignación.</param>
        public void DBEliminar( int detalles)
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " WHERE COD_CIA = @COD_CIA" +
                " AND COD_ZON = @COD_ZON " +
                " AND NUM_CSG = @NUM_CSG";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_CSG", SqlDbType.NVarChar, Numero)});

            int registro = GestorDatos.EjecutarComando(sentencia, parametros);

            if (registro != 1)
                throw new Exception("No se eliminó el encabezado de la boleta de la compañía '" + Compania + "' para el cliente '" + Cliente + "' en la ruta '" + Zona + "'.");

            this.detalles.Eliminar(detalles);
        }

        /// <summary>
        /// Actualiza el estado del campo Impreso indicando que se ha realizado la impresión del documento siempre y 
        /// cuando el valor de la variable Impreso sea verdadero, de lo contrario indica que no se ha realizado la 
        /// impresión.
        /// </summary>
        public void DBActualizarImpresion()
        {
            string sentencia =
                " UPDATE " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " SET IMPRESO = @IMPRESO " +
                " WHERE COD_ZON = @COD_ZON " +
                " AND NUM_CSG = @NUM_CSG ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_CSG", SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@IMPRESO", SqlDbType.NVarChar, (Impreso? "S" : "N"))});

            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando el indicador de impresión. " + ex.Message);
            }

            parametros = null;
        }

        /// <summary>
        /// Obtiene la cantidad de líneas de la venta en consignación.
        /// </summary>
        public int ObtenerCantidadDetallesVenta()
        {
            string sentencia = 
                "SELECT NUM_ITM FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG  +
                " WHERE UPPER(COD_CIA) = @COMPANIA " +
                " AND   COD_ZON = @ZONA " +
                " AND   COD_CLT = @CLIENTE ";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@ZONA",SqlDbType.NVarChar,Zona),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar,Cliente)                                          });

            try
            {
                return GestorDatos.NumeroRegistros(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error leyendo la cantidad de líneas de la venta en consignación en la compañía '" + Compania + "'. " + ex.Message);
            }
        }
        /// <summary>
        /// Obtener una venta en consignacion especifica
        /// </summary>
        /// <param name="cliente">cliente asociado</param>
        /// <param name="zona">zona asociada</param>
        /// <param name="compania">Compania asociada</param>
        /// <returns>venta en consignacion indicada</returns>
        public static VentaConsignacion ObtenerVentaConsignacion(Cliente cliente, Ruta zona, string compania)
        { 
            foreach(VentaConsignacion venta in ObtenerBoletas(cliente,zona,false))
            {
                if (venta.Compania.Equals(compania))
                    return venta;
               
            }
            return null;

        }
        /// <summary>
        /// Obtener los detalles de una venta en consulta
        /// </summary>
        public void ObtenerDetalles()
        {
            detalles.Encabezado = this;
            detalles.Obtener(false, this.configuracion.Nivel);
        }
        /// <summary>
        /// Obtener Boletas 
        /// </summary>
        /// <param name="cliente">Cliente asociado</param>
        /// <param name="zona">Zona asociada</param>
        /// <param name="soloNoSincronizadas">Estado de las boletas</param>
        /// <returns>Lista de las ventas del cliente</returns>
        public static List<VentaConsignacion> ObtenerBoletas(Cliente cliente, Ruta zona, bool soloNoSincronizadas)
        {
            List<VentaConsignacion> boletas = new List<VentaConsignacion>();
            boletas.Clear();

            //ABC 20/08/2008 Caso:32163, 32882, 32884 Se agrega NUM_CSG en la sentencia para optener el numero de consignación de la boleta.
            string sentencia =
                " SELECT COD_CIA, NUM_CSG,HOR_INI,HOR_FIN,FEC_BOLETA,COD_CND,COD_PAIS,CLASE,COD_BOD,LST_PRE,OBSERVACIONES," +
                "        DESC1,DESC2,MONT_DESC1,MONT_DESC2,DESCUENTO_CASCADA,MON_IMP_VT,MON_IMP_CS,MON_SIV,DOC_PRO,IMPRESO " +
                "FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " WHERE COD_CLT = @CLIENTE" +
                " AND   COD_ZON = @ZONA";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                      new SQLiteParameter("@CLIENTE", cliente.Codigo),
                      new SQLiteParameter("@ZONA", zona.Codigo)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    VentaConsignacion boleta = new VentaConsignacion();
                    boleta.Estado = (EstadoConsignacion)Convert.ToChar(reader.GetString(19));

                    if (soloNoSincronizadas && boleta.Estado != EstadoConsignacion.NoSincronizada)
                        break;

                    boleta.Cliente = cliente.Codigo;
                    boleta.Zona = zona.Codigo;
                    boleta.Bodega = zona.Bodega;
                    boleta.Compania = reader.GetString(0);
                    boleta.Numero = reader.GetString(1);
                    boleta.HoraInicio = reader.GetDateTime(2);
                    boleta.HoraFin = reader.GetDateTime(3);
                    boleta.FechaRealizacion = reader.GetDateTime(4);

                    boleta.Configuracion = new ConfigDocCia();
                    boleta.configuracion.Compania.Codigo = boleta.Compania;
                    boleta.Configuracion.ClienteCia = cliente.ObtenerClienteCia(boleta.Compania);
                    boleta.Configuracion.Pais.Codigo = reader.GetString(6);
                    boleta.Configuracion.Clase = (ClaseDoc)Convert.ToChar(reader.GetString(7));
                    boleta.Configuracion.CondicionPago.Codigo = reader.GetString(5);
                    boleta.NivelPrecio = (reader.GetInt32(9));
                    boleta.Configuracion.Nivel = boleta.Configuracion.ClienteCia.NivelPrecio;
                    boleta.Configuracion.ClienteCia = cliente.ObtenerClienteCia(boleta.Compania);
                    boleta.BodegaConsigna = reader.GetString(8);
                    if (!reader.IsDBNull(10)) boleta.Notas = reader.GetString(10);
                    boleta.PorcentajeDescuento1 = reader.GetDecimal(11);
                    boleta.PorcentajeDescuento2 = reader.GetDecimal(12);
                    boleta.MontoDescuento1 = reader.GetDecimal(13);
                    boleta.MontoDescuento2 = reader.GetDecimal(14);
                    boleta.DescuentoCascada = reader.GetString(15).Equals("S");

                    boleta.Impuesto = new Impuesto(reader.GetDecimal(16), reader.GetDecimal(17));
                    boleta.MontoBruto = reader.GetDecimal(18);                 
                    boleta.Impreso = reader.GetString(20).Equals("S");

                    boleta.Configuracion.Cargar();

                    boletas.Add(boleta);
                }
                return boletas;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando información de la boleta de venta en consignación. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        /// <summary>
        /// Indica si hay consignaciones sin sincronizar
        /// </summary>
        /// <returns></returns>
        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG + " WHERE DOC_PRO = '" + (char)EstadoConsignacion.NoSincronizada + "'";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }
        /// <summary>
        /// Indica si existe consignacion para el cliente en una zona
        /// </summary>
        /// <param name="zona">zona a consultar</param>
        /// <param name="cliente">cliente indicado</param>
        /// <returns>existencia de la consignacion</returns>
        public static bool ExisteConsignacion( string zona , string cliente)
        {
            string sentencia =
                "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " WHERE COD_ZON = @ZONA " +
                "   AND COD_CLT = @CLIENTE ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@ZONA", zona),
                                            new SQLiteParameter("@CLIENTE", cliente)});

            return (GestorDatos.NumeroRegistros(sentencia,parametros) != 0);
        }
        /// <summary>
        /// Obtiene los códigos de las compañías asociadas a la boleta de venta en consignación del cliente.
        /// </summary>
        /// <param name="zona">Código de la ruta.</param>
        /// <param name="cliente">Código del cliente.</param>
        /// <param name="estado">
        /// Indica si se debe obtener los códigos de las compañías de la boleta que no ha sido sincronizada o de la que ha sido procesada.
        /// </param>
        /// <returns>Retorna un arreglo con los códigos de las compañías.</returns>
        public static List<string> ObtenerCompanias(string zona, string cliente, EstadoConsignacion estado)
        {
            List<string> companias = new List<string>();
            string sentencia = string.Empty;

            sentencia =
                " SELECT COD_CIA FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " WHERE COD_ZON = @ZONA " +
                "   AND COD_CLT = @CLIENTE " +
                (estado != EstadoConsignacion.NoDefinido ? " AND DOC_PRO = '" + (char)estado + "' " : "") +
                "ORDER BY COD_CIA";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@ZONA", zona),
                                                    new SQLiteParameter("@CLIENTE", cliente)});
            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                while (reader.Read())
                    companias.Add(reader.GetString(0));
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando compañías de venta en consignación del cliente. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return companias;
        }
        /// <summary>
        /// Indica si existe o no alguna boleta de venta en consignación procesada en el ERP para un cliente determinado.
        /// </summary>
        /// <param name="zona">Código de la ruta.</param>
        /// <param name="cliente">Código del cliente.</param>
        /// <returns>Verdadero indica que exite almenos una boleta de venta en consignación procesada en el ERP para el cliente en la ruta definida.</returns>
        public static bool ExisteConsignacionProcesada(string zona, string cliente)
        {
            string sentencia =
                "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " WHERE COD_ZON = @ZONA " +
                "   AND COD_CLT = @CLIENTE " +
                "   AND DOC_PRO = '" + (char)EstadoConsignacion.Procesada + "' ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@ZONA", zona),
                                                    new SQLiteParameter("@CLIENTE", cliente)});

            return (GestorDatos.NumeroRegistros(sentencia, parametros) != 0);
        }

        public static bool ExisteConsignacionSaldos(string zona, string cliente)
        {
            return ExisteConsignacionSaldos(string.Empty, zona, cliente);
        }

        /// <summary>
        /// Indica si existen detalles de la venta en consignación que poseen saldos, lo cual indica que hay detalles consignados en la boleta que aún no ha sido sincronizada.
        /// </summary>
        /// <param name="zompania">Código de la compañía.</param>
        /// <param name="zona">Código de la ruta.</param>
        /// <param name="cliente">Código del cliente.</param>
        /// <returns>Verdadero indica que exite almenos un detalle de la boleta de venta en consignación que posee saldos en consignación.</returns>
        public static bool ExisteConsignacionSaldos(string compania, string zona, string cliente)
        {
            string sentencia =
                " SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_CONSIG +
                " WHERE COD_ZON = @ZONA " +
                "   AND COD_CLT = @CLIENTE " +
                "   AND DOC_PRO = '" + (char)EstadoConsignacion.NoSincronizada + "' ";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Clear();

            parametros.Add(new SQLiteParameter("@ZONA", zona));
            parametros.Add(new SQLiteParameter("@CLIENTE", cliente));
            if(compania != string.Empty) 
            {
                sentencia += " AND UPPER(COD_CIA) = @COMPANIA ";
                parametros.Add(new SQLiteParameter("@COMPANIA", compania.ToUpper()));
            }
            
            int encabezados = GestorDatos.NumeroRegistros(sentencia,parametros);

            if (encabezados > 0)
            {
                sentencia =
                    " SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_DET_CONSIG +
                    " WHERE COD_ZON = @ZONA " +
                    "   AND COD_CLT = @CLIENTE ";

                SQLiteParameterList parametros2 = new SQLiteParameterList();
                parametros2.Clear();

                parametros2.Add(new SQLiteParameter("@ZONA", zona));
                parametros2.Add(new SQLiteParameter("@CLIENTE", cliente));
                if (compania != string.Empty)
                {
                    sentencia += " AND UPPER(COD_CIA) = @COMPANIA ";
                    parametros2.Add(new SQLiteParameter("@COMPANIA", compania.ToUpper()));
                }

                sentencia += " AND (SALDO_MAX > 0 OR SALDO_MIN > 0)";
                int detalles = (int)GestorDatos.EjecutarScalar(sentencia,parametros2);

                if (detalles > 0)
                    return true;
            }
            return false;
        }
        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "VENTACONSIGNACION";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                //Caso:32682 ABC 23/05/2008 Mostrar Total de Articulo y Total de Lineas
                case "DETALLES": return new ArrayList(detalles.Lista);
                case "TOTAL_ARTICULOS": return detalles.TotalArticulos;
                case "TOTAL_LINEAS": return detalles.TotalArticulos;
                default:
                    object field = this.Configuracion.ClienteCia.GetField(name);
                    if (!field.Equals(string.Empty))
                        return field;
                    else
                        return base.GetField(name);                    
            }        
        }
        #endregion


    }
}
