using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion
{
    /// <summary>
    /// Representa la Gestion de las boletas de desglose de consignacion para un cliente Multi cia
    /// </summary>
    public class BoletasConsignacion : VentasConsignacion
    {

        #region Lista de Consignaciones

        private Pedidos facturas = new Pedidos();
        /// <summary>
        /// Contiene los pedidos a facturar en consignacion.
        /// </summary>
        public Pedidos Facturas
        {
            get { return facturas; }
            set { facturas = value; }
        }
        private Devoluciones devoluciones = new Devoluciones();
        /// <summary>
        /// Contiene las devoluciones en desglose de consignacion.
        /// </summary>
        public Devoluciones Devoluciones
        {
            get { return devoluciones; }
            set { devoluciones = value; }
        }
        #endregion

        #region Constantes de la instancia
        //Caso 31100 LDS 21/02/2008
        /// <summary>
        /// Constante para separar las sentencias de un conjunto de sentencias a ejecutar.
        /// </summary>
        private const string SEPARADOR = "#|#";
        #endregion

        #region Logica de Negocios

        /// <summary>
        /// Realiza la finalizacion de un desglose de la consignacion.
        /// Se debe tomar en cuanta el proceso que realiaza depenidendo de los parametros.
        /// Siempre se generan las dovoluciones y facturas.
        /// Si existen saldos, y no se desea mantener la boleta, 1. Eliminar boleta, 2. Crear Nueva, 3. Restablecer saldos por Dev o Fac.
        /// Si existen saldos, y se desea matener la boleta, 1. Eliminar boleta, 2. Crear Nueva con el detalle anterior.
        /// Si no existen saldos y no se desea mantener la boleta, 1. Eliminar Boleta.
        /// </summary>
        /// <param name="existeSaldo">Indica si existe algún saldo de la venta</param>
        /// <param name="mantenerBoleta">Indica si se desea mantener la boleta anterior.</param>
        public void FinalizarDesglose(bool existeSaldo, bool mantenerBoleta)
        {
            try
            {
                //Iniciamos la transacción.
                GestorDatos.BeginTransaction();

                // Se guardan las devoluciones y las facturas
                GuardarDevolucionesGeneradas();
                GuardarFacturasGeneradas();
                if (existeSaldo && !mantenerBoleta)
                {
                    // Se elimina de la BD la boleta de la Venta en Consignación
                    if (EliminarBoletaConsignacion())
                    {
                        // Se actualiza la boleta con las devoluciones y facturas.
                        ActualizarBoleta(false, true);
                    }

                }
                else
                {
                    if (mantenerBoleta)
                        ActualizarBoleta(false, false);
                }

                //Se debe eliminar las boletas de venta en consignación que han sido cargadas.

                if (!existeSaldo && !mantenerBoleta)
                    EliminarBoletaConsignacion();
                //ABC 22/08/2008 Caso:32163, 32882, 32884 Actualiza al cliente que si se hizo desglose.
                DesgloseCliente();

                //Se ejecuta el Commit.
                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                throw ex;
            }
        }
        /// <summary>
        /// Determina si se realizaron cambios en los precios de la venta en consignacion
        /// </summary>
        /// <returns></returns>
        public bool CambioEnPrecios()
        {
            foreach(VentaConsignacion venta in this.Gestionados)
            {
                if(venta.detalles.CambioEnPrecio)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Limpia las instancias de la gestion
        /// </summary>
        public void LimpiarGestor()
        {
            Devoluciones.Gestionados.Clear();
            Facturas.Gestionados.Clear();
            Gestionados.Clear();            
        }

        /// <summary>
        /// Verifica la existencia requerida de la cantidad total en unidad de almacén de los detalles 
        /// de la boleta de venta en consignación en la bodega de la ruta para realizar el reabastecimiento.
        /// </summary>
        /// <param name="bodega">Bodega a verificar existencias</param>
        /// <returns>Contiene los detalles de la venta que tienen el artículo sin suficientes existencias.</returns>
        public List<DetalleVenta> VerificarExistenciasDetalles(string bodega)
        {
            List<DetalleVenta> existenciaInvalida = new List<DetalleVenta>();
            existenciaInvalida.Clear();
            try
            {
                foreach (VentaConsignacion boletaConsignacion in this.Gestionados)
                    foreach (DetalleVenta detalleBoleta in boletaConsignacion.Detalles.Lista)
                    {
                        if (!detalleBoleta.VerificarExistencia(bodega))
                        {
                            detalleBoleta.FaltaExistencia = true;
                            existenciaInvalida.Add(detalleBoleta);
                        }
                    }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar las existencias de los detalles. " + ex.Message);
            }

            return existenciaInvalida;
        }
        /// <summary>
        /// Elimina la boleta de venta en consignación que haya sido cargada.
        /// </summary>
        /// <returns>Indica si se realizó la eliminación de la boleta de venta en consignación.</returns>
        public bool EliminarBoletaConsignacion()
        {
            foreach (VentaConsignacion boleta in this.Gestionados)
            {
                boleta.DBEliminar(boleta.detalles.Lista.Count);
            }
            return true;
        }
        /// <summary>
        /// Verfica que se haya realizado el desglose de la boleta de venta en consignación.
        /// </summary>
        /// <returns>
        /// En caso de haberse realizado el desglose para por lo menos un detalle de alguna de las boletas
        /// de venta en consignación se devuelve verdadero.
        /// </returns>
        public bool RealizoDesglose()
        {
            foreach (VentaConsignacion boletaConsignacion in this.Gestionados)
                foreach (DetalleVenta detalle in boletaConsignacion.Detalles.Lista)
                    if (detalle.UnidadesDesglosadas())
                        return true;

            return false;
        }

        #region Actualizar en el Desglose

        /// <summary>
        /// Detalles a Restablecer en la boleta luego de aplicar una factura o devolucion
        /// </summary>
        /// <param name="restablecer">Los detalles de toda la boleta</param>
        /// <param name="detalle">Detalle a restablecer producto del desglose</param>
        /// <param name="anular">indicar si se restablece por anulacion de documento o por actualizacion</param>
        private void DetallesARestablecer(ref List<DetalleRestablecer> restablecer, DetalleLinea detalle, bool anular)
        {
            //Se verifica que el detalle haya sido agregado por el recorrido de los detalles de la boleta.
            foreach (DetalleRestablecer detalleRestablecer in restablecer)
            {
                if (detalleRestablecer.Articulo.Codigo == detalle.Articulo.Codigo)
                {
                    detalleRestablecer.Articulo = detalle.Articulo;
                    
                    if (anular)
                        detalleRestablecer.AgregarCantidades(detalle.UnidadesAlmacen, detalle.UnidadesDetalle);
                    else
                        detalleRestablecer.DisminuirCantidades(detalle.UnidadesAlmacen, detalle.UnidadesDetalle);

                    if (detalleRestablecer.CantidadAlmacen == decimal.Zero && detalleRestablecer.CantidadDetalle == decimal.Zero)
                        restablecer.Remove(detalleRestablecer);
                    break;
                }
            }
        }
        /// <summary>
        /// Regenerar una venta en consignacion basado en los nuevos detalles a restablecer
        /// </summary>
        /// <param name="venta">venta a restablecer (eliminar y regenerar)</param>
        /// <param name="detalles">nuevos detalles a restablecer en la venta</param>
        private void RestablecerVenta(VentaConsignacion venta, List<DetalleRestablecer> detalles)
        {
            //Se gestiona la venta en consignación y se crea la boleta.
            if (detalles.Count > 0)
                RegenerarBoleta(venta, detalles, true);        
        }
        /// <summary>
        /// Restablecer una venta en consignacion por la aplicacion de una factura a la boleta
        /// </summary>
        public void ActualizarBoletaPorFactura(bool anular)
        {
            foreach (Pedido factura in this.facturas.Gestionados)
            {
                VentaConsignacion ventaAsociada = this.Buscar(factura.Compania);
                List<DetalleRestablecer> restablecer = DetallesVenta.ObtenerInformacionBoleta(ventaAsociada.Numero, ventaAsociada.Compania);

                foreach (DetalleLinea detalle in factura.Detalles.Lista)
                    DetallesARestablecer(ref restablecer, detalle, anular);

                RestablecerVenta(ventaAsociada, restablecer);
            }
            
        }

        /// <summary>
        /// Si se busca un articulo dentro de una consignacion y no se encuentra
        /// luego de "articuloEncontrado != "intentos se abandona la busqueda"
        /// no deberia ocurrir, se asigna como precaución
        /// </summary>
        private const int maxIntentos = 5000;

        /// <summary>
        /// Obtiene la informacion que le hace falta al articulo para la consignación, ya que
        /// solo cuenta con el codigo y la compañía
        /// </summary>
        /// <param name="consignacion">numero de la consigancion a la que pertenece</param>
        /// <param name="detalles">detalles con los articulos</param>
        private void ObtenerDetalleArticuloBoletaConsignacion(string consignacion, ref List<DetalleRestablecer> detalles)
        {
            bool boletaEncontrada = false;
            bool articuloEncontrado = false;
            for (int k = 0; k < Gestionados.Count && !boletaEncontrada; k++)
                //Busca la remision actual
                if (Gestionados[k].Numero == consignacion)
                {
                    //Revisa en la consigancion el articulo
                    for (int i = 0; i < Gestionados[k].Detalles.Lista.Count; i++)
                    {
                        articuloEncontrado = false;
                        for (int j = 0; j < detalles.Count && !articuloEncontrado && j< maxIntentos; j++)
                        {
                            
                            //copia la informacion del articulo a los detalles
                            if (detalles[j].Articulo.Codigo == Gestionados[k].Detalles.Lista[i].Articulo.Codigo)
                            {
                                detalles[j].Articulo = (Articulo)Gestionados[k].Detalles.Lista[i].Articulo.Clone();
                                articuloEncontrado = true;
                            }                            
                        }
                    }
                    boletaEncontrada = true;
                }
        }

        /// <summary>
        /// Actualiza la consignación con los datos de las consignaciones o facturas.
        /// </summary>
        /// <param name="anular">Indica si se quiere anular los documentos</param>
        public void ActualizarBoleta(bool anular, bool restablecerDetalle)
        {
            List<VentaConsignacion> copia = new List<VentaConsignacion>(Gestionados);
            foreach (VentaConsignacion venta in copia)
            {
                List<DetalleRestablecer> porRestablecer =
                        DetallesVenta.ObtenerInformacionBoleta(venta.Numero, venta.Compania);

                //LDA CR1-05106-LJQD
                //No aparece la informacion del articulo en la consignacion
                ObtenerDetalleArticuloBoletaConsignacion(venta.Numero, ref porRestablecer);

                if (restablecerDetalle)
                {
                    // Se obtiene la lista de los detalles modificados

                    // Restablece la boleta por las devoculciones realizadas
                    RestablecerDetalleDevolucion(ref porRestablecer, anular);
                    // Restablece la boleta por las facturas realizadas
                    RestablecerDetalleFactura(ref porRestablecer, anular);
                    // Crea la nueva boleta de la Venta en Consignación. Aplica los nuevos detalles.
                    RestablecerVenta(venta, porRestablecer);
                }
                else
                {
                    EliminarBoletaConsignacion();
                    RegenerarBoleta(venta, venta.Detalles.Lista, true);
                }

            }
            copia.Clear();
        }



        /// <summary>
        /// Modifica los detalles basados en las devoluciones realizadas.
        /// </summary>
        /// <param name="porRestablecer">Lista de Detalles por restablecer</param>
        /// <param name="anular">Indica si se quiere anular los documentos</param>
        private void RestablecerDetalleDevolucion(ref List<DetalleRestablecer> porRestablecer, bool anular)
        {
            foreach (Devolucion devolucion in this.Devoluciones.Gestionados)
            {
                foreach (DetalleLinea linea in devolucion.Detalles.Lista)
                    DetallesARestablecer(ref porRestablecer, linea, anular);
            }
        }

        /// <summary>
        /// Modifica los detalles basados en las facturas realizadas.
        /// </summary>
        /// <param name="porRestablecer">Lista de Detalles por restablecer</param>
        /// <param name="anular">Indica si se quiere anular los documentos</param>
        private void RestablecerDetalleFactura(ref List<DetalleRestablecer> porRestablecer, bool anular)
        {
            foreach (Pedido factura in this.Facturas.Gestionados)
            {
                foreach (DetalleLinea linea in factura.Detalles.Lista)
                    DetallesARestablecer(ref porRestablecer, linea, anular);
            }
        }

        /// <summary>
        /// Restablecer una venta en consignacion por la aplicacion de una factura a la boleta        
        /// </summary>
        public void ActualizarBoletaPorDevolucion(bool anular)
        {
            foreach (Devolucion devolucion in this.Devoluciones.Gestionados)
            {
                VentaConsignacion ventaAsociada = this.Buscar(devolucion.Compania);
                List<DetalleRestablecer> restablecer = DetallesVenta.ObtenerInformacionBoleta(ventaAsociada.Numero, ventaAsociada.Compania);

                foreach (DetalleLinea detalle in devolucion.Detalles.Lista)
                    DetallesARestablecer(ref restablecer, detalle, anular);

                RestablecerVenta(ventaAsociada, restablecer);
            }
        }

        /// <summary>
        /// Gestiona la venta en consignación para la compañía y guarda la boleta en la base de datos.
        /// </summary>
        /// <param name="codigoCompania">Código de la compañía para la cuál se va a crear la boleta de venta en consignación.</param>
        /// <param name="config">Configuración que se asignará a la venta en consignación.</param>
        /// <param name="detalles">Detalles con los cuales se creará la venta en consignación.</param>
        /// <param name="porcentajeDesc1">Porcentaje de descuento1 que se asignará a la venta en consignación.</param>
        /// <param name="porcentajeDesc2">Porcentaje de descuento2 que se asignará a la venta en consignación.</param>
        /// <param name="observaciones">Contiene las observaciones que se deben asignar a la boleta.</param>
        /// <param name="transaccion">Objeto transacción.</param>
        /// <param name="desglose">Indica si es por desglose la creacion de la boleta</param>

        /// <summary>
        /// Regenerar una venta en consignacion producto de aplicar una devolucion o factura
        /// </summary>
        /// <param name="venta"></param>
        /// <param name="detalles"></param>
        /// <param name="desglose"></param>
        public void RegenerarBoleta(VentaConsignacion venta, List<DetalleRestablecer> detalles, bool desglose)
        {
            venta.detalles.Lista.Clear();
            venta.FechaRealizacion = venta.HoraInicio = venta.HoraFin = DateTime.Now;
            venta.Numero = "CONSIG" + venta.FechaRealizacion.ToShortDateString().Replace("/", "") + venta.HoraInicio.ToLongTimeString().Substring(0, 7).Replace(":", "");
            venta.Estado = EstadoConsignacion.NoSincronizada;
            // Se borra el monto bruto, para que el calculo de los totales sea correcto.
            this.modificar(venta.Compania, venta);
            this.BorrarTotales();
            foreach (DetalleRestablecer restablecer in detalles)
                this.Gestionar(restablecer.Articulo,venta.Zona, restablecer.CantidadAlmacen, restablecer.CantidadDetalle, restablecer.SaldoAlmacen, restablecer.SaldoDetalle, restablecer.Precio, false);

            //Guardamos la venta en consignación en la base de datos.
            venta.DBGuardar(true);
            //VentaConsignacion.LimpiarValores();
            //GestorVentaConsig.VentasGestionadas.Clear();
        }
        /// <summary>
        /// Regenerar una venta en consignacion producto de aplicar una devolucion o factura. 
        /// Manteniendo el detalle de la boleta anterior.
        /// </summary>
        /// <param name="venta"></param>
        /// <param name="detalles"></param>
        /// <param name="desglose"></param>
        public void RegenerarBoleta(VentaConsignacion venta, List<DetalleVenta> detalles, bool desglose)
        {
            venta.FechaRealizacion = venta.HoraInicio = venta.HoraFin = DateTime.Now;
            venta.Numero = "CONSIG" + venta.FechaRealizacion.ToShortDateString().Replace("/", "") + venta.HoraInicio.ToLongTimeString().Substring(0, 7).Replace(":", "");
            venta.Estado = EstadoConsignacion.NoSincronizada;
            this.modificar(venta.Compania, venta);
            //Rebajar las existencias por restablecer a la bodega del camion.
            foreach (DetalleVenta detalle in detalles)
            {
                detalle.Articulo.Bodega = new Bodega(venta.Bodega);
                detalle.Articulo.Bodega.CargarExistencia(detalle.Articulo.Compania, detalle.Articulo.Codigo);
                // Si las existencias no son suficientes, se rebajan solo las existencias del camión.
                decimal disminuir = detalle.Articulo.Bodega.Existencia > detalle.TotalAlmacenDesglose ?
                    detalle.TotalAlmacenDesglose : detalle.Articulo.Bodega.Existencia;
                if (disminuir > 0)
                    detalle.Articulo.ActualizarBodega(disminuir * -1);
            }
            //Guardamos la venta en consignación en la base de datos.
            venta.DBGuardar(true);
        }
        /// <summary>
        /// Limpia los montos de la boleta.
        /// </summary>
        private void BorrarTotales()
        {
            foreach (VentaConsignacion venta in Gestionados)
            {
                venta.MontoBruto = 0;
                venta.MontoDescuento1 = 0;
                venta.MontoDescuento2 = 0;
                venta.MontoDescuentoLineas = 0;
                venta.Impuesto.MontoImpuesto1 = 0;
                venta.Impuesto.MontoImpuesto2 = 0;
            }
        }
        #endregion 

        #region Gestion De Facturas y Devoluciones
        /// <summary>
        /// Genera las facturas correspondientes al desglose de la boleta de venta en consignación.
        /// </summary>
        public bool GenerarFactura()
        {
            //Se indica que se generaran facturas.
            Pedidos.FacturarPedido = true;
            this.facturas.Gestionados.Clear();

            try
            {
                foreach (VentaConsignacion boleta in this.Gestionados)
                {
                    foreach (DetalleVenta detalle in boleta.Detalles.Lista)
                    {
                        //ABC 24/07/2008 Caso:33331 Se ingresa al articulo el precio del detalle consignado. El precio pudo variar
                        //El precio actual va en el articulo y el de la venta en el precio del detalle
                        Articulo articulo = (Articulo)detalle.Articulo.Clone();
                        articulo.Precio = new Precio();
                        articulo.CargarPrecio(boleta.Configuracion.Nivel);

                        if (detalle.UnidadesVendidas())
                            facturas.GestionarConsignado(boleta.Numero, articulo, boleta.Zona, detalle.Precio, detalle.UnidadesAlmacenVendido, detalle.UnidadesDetalleVendido, boleta.DescuentoCascada, boleta.PorcentajeDescuento1, boleta.PorcentajeDescuento2, boleta.Configuracion, boleta.Notas, true, "");
                        if ((detalle.UnidadesDevueltasMalEstado() && FRdConfig.CobrarMalEstado))
                            facturas.GestionarConsignado(boleta.Numero, articulo, boleta.Zona, detalle.Precio, (detalle.UnidadesAlmacenVendido + detalle.UnidadesAlmacenMalEstado), (detalle.UnidadesDetalleVendido + detalle.UnidadesDetalleMalEstado), boleta.DescuentoCascada, boleta.PorcentajeDescuento1, boleta.PorcentajeDescuento2, boleta.Configuracion, boleta.Notas, true, "");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar las facturas. " + ex.Message);
            }
            finally
            {
                //Se reestablece el valor de la variable estática.
                Pedidos.FacturarPedido = false;
            }
            return (facturas.Gestionados.Count > 0);
        }

        /// <summary>
        /// Genera las devoluciones correspondientes al desglose de la boleta de venta en consignación.
        /// </summary>
        public bool GenerarDevolucion()
        {
            this.devoluciones.Gestionados.Clear();

            try
            {
                foreach (VentaConsignacion boleta in this.Gestionados)
                {
                    foreach (DetalleVenta detalle in boleta.Detalles.Lista)
                    {
                        if (detalle.UnidadesDevueltas())
                        {
                            //ABC 24/07/2008 Caso:33331 Se ingresa al articulo el precio del detalle consignado. El precio pudo variar
                            //El precio actual va en el articulo y el de la venta en el precio del detalle
                            Articulo articulo = (Articulo)detalle.Articulo.Clone();
                            articulo.Precio = new Precio();
                            articulo.CargarPrecio(boleta.Configuracion.Nivel);

                            if (detalle.UnidadesDevueltasBuenEstado())
                                devoluciones.GestionarConsignado(boleta.Numero, articulo, boleta.Configuracion.ClienteCia, detalle.UnidadesAlmacenBuenEstado, detalle.UnidadesDetalleBuenEstado, Estado.Bueno, detalle.Lote, detalle.Observaciones, boleta.Bodega,boleta.BodegaConsigna, boleta.Zona);
                            if (detalle.UnidadesDevueltasMalEstado() && !FRdConfig.CobrarMalEstado)
                                devoluciones.GestionarConsignado(boleta.Numero, articulo, boleta.Configuracion.ClienteCia, detalle.UnidadesAlmacenMalEstado, detalle.UnidadesDetalleMalEstado, Estado.Malo, detalle.Lote, detalle.Observaciones, boleta.Bodega, boleta.BodegaConsigna, boleta.Zona);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar las devoluciones. " + ex.Message);
            }
            return (devoluciones.Gestionados.Count > 0);
        }

        /// <summary>
        /// Indica si se debe debe generar una factura luego del desglose realizado.
        /// </summary>
        /// <returns>En caso de que se deba generar la factura se retorna verdadero.</returns>
        public bool ValidarGenerarFactura()
        {
            foreach (VentaConsignacion boleta in this.Gestionados)
                foreach (DetalleVenta detalle in boleta.Detalles.Lista)
                {
                    if (detalle.UnidadesVendidas())
                        return true;
                    if ((detalle.UnidadesDevueltasMalEstado()) && FRdConfig.CobrarMalEstado)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Guarda en la base de datos las factura generadas por el desglose de la boleta de venta en consignación.
        /// </summary>
        public void GuardarFacturasGeneradas()
        {
            facturas.CargarConsecutivos();
            foreach (Pedido factura in facturas.Gestionados)
            {
                factura.Guardar(false);
            }
        }
        /// <summary>
        /// Guarda en la base de datos las devoluciones generadas por el desglose de la boleta de venta en consignación.
        /// </summary>
        public void GuardarDevolucionesGeneradas()
        {
            foreach (Devolucion devolucion in devoluciones.Gestionados)
            {
                devolucion.Guardar(false);
            }
        }

        /// <summary>
        /// Generar los saldos respectivos a una boleta a la cual se le realizara un desglose
        /// </summary>
        public void GenerarSaldos()
        {
            foreach (VentaConsignacion boleta in this.Gestionados)
            {
                foreach (DetalleVenta detalle in boleta.Detalles.Lista)
                {
                    if (detalle.TotalAlmacenSaldo == decimal.Zero)
                    {
                        detalle.UnidadesAlmacenSaldo = detalle.UnidadesAlmacen;
                        detalle.UnidadesDetalleSaldo = detalle.UnidadesDetalle;
                    }
                }
            }
        }
        #endregion

        #region Generar Sugeridos de Boletas
        /// <summary>
        /// Gestiona las nuevas ventas en consignación que serán sugeridas luego del desglose de la boleta de venta en consignación.
        /// </summary>
        /// <param name="soloSaldos">
        /// Indica que las nuevas ventas sugeridas que se van a gestionar deben ser solo con los detalles que poseen saldo de la boleta que ha sido desglosada.
        /// </param>
        public VentasConsignacion GestionarVentaConsignacionSugerida(bool soloSaldos)
        {
            VentasConsignacion nuevaVentaSugerida = new VentasConsignacion();

            nuevaVentaSugerida.Gestionados = this.Gestionados;
            nuevaVentaSugerida.ConfigDocumentoCia = this.ConfigDocumentoCia;

            foreach (VentaConsignacion boleta in Gestionados)
            {
                foreach (DetalleVenta detalle in boleta.Detalles.Lista)
                {
                    //Con saldo
                    bool soloDetallesConSaldo = (soloSaldos && (detalle.UnidadesAlmacenSaldo > decimal.Zero || detalle.UnidadesDetalleSaldo > decimal.Zero));
                    //para aquellos que posean existencias en bodega o si tambien su saldo es mayor de 0
                    bool soloDetallesExistenciaBodegaRuta = (!soloSaldos && (detalle.TotalAlmacenExistencia > decimal.Zero || detalle.UnidadesAlmacenSaldo > decimal.Zero));

                    if (soloDetallesConSaldo || soloDetallesExistenciaBodegaRuta)
                    {
                        if (soloDetallesConSaldo)
                            nuevaVentaSugerida.Gestionar(detalle.Articulo, boleta.Zona, detalle.UnidadesAlmacenSaldo, detalle.UnidadesDetalleSaldo, detalle.UnidadesAlmacenSaldo, detalle.UnidadesDetalleSaldo, detalle.Articulo.Precio, false);

                        if (soloDetallesExistenciaBodegaRuta)
                        {
                            //Error al sugerir venta en consignación caundo no hay suficientes existencias.
                            detalle.Articulo.CargarExistencia(boleta.Bodega);
                            decimal existenciasTotalAlmacenArticulo = detalle.Articulo.Bodega.Existencia;
                            //Calculamos el faltante requerido del detalle en unidades de almacén.
                            decimal existenciasTotalAlmacenDetalleRequerido = detalle.TotalAlmacen - detalle.TotalAlmacenSaldo;
                            //Se verifica si el detalle posee suficientes existencias.
                            if (existenciasTotalAlmacenArticulo < existenciasTotalAlmacenDetalleRequerido)
                            {
                                decimal nuevaUnidadesAlmacen = detalle.UnidadesAlmacen - (existenciasTotalAlmacenDetalleRequerido - existenciasTotalAlmacenArticulo);
                                nuevaVentaSugerida.Gestionar(detalle.Articulo, boleta.Zona, nuevaUnidadesAlmacen, detalle.UnidadesDetalle, detalle.UnidadesAlmacenSaldo, detalle.UnidadesDetalleSaldo, detalle.Articulo.Precio, false);
                            }
                            else
                            {
                                //Agregamos la línea a la venta en consignación.
                                nuevaVentaSugerida.Gestionar(detalle.Articulo, boleta.Zona, detalle.UnidadesAlmacen, detalle.UnidadesDetalle, detalle.UnidadesAlmacenSaldo, detalle.UnidadesDetalleSaldo, detalle.Articulo.Precio, false);
                            }
                        }
                    }
                }
                //Borrar las ventas en companias donde no quedo sugerido
                if (nuevaVentaSugerida.Buscar(boleta.Compania).Detalles.Vacio())
                    nuevaVentaSugerida.Borrar(boleta.Compania);

                //Cuando mantiene solo saldos
                if (soloSaldos) 
                    RemoverDetallesSinSaldo();
                //Cuando es mantener boleta anterior
                else 
                    RemoverDetallesSinExistencia();
            }
            return nuevaVentaSugerida;
        }
        /// <summary>
        /// En un sugerido remueve los detalles que quedan en 0. por no poder reabastecer existencias
        /// </summary>
        public void RemoverDetallesSinSaldo()
        {
            foreach (VentaConsignacion venta in this.Gestionados)
            {

                for (int y = venta.Detalles.Lista.Count - 1; y >= 0; y--)
                {
                    if (venta.Detalles.Lista[y].UnidadesSaldo())
                    {
                        venta.EliminarDetalle(venta.Detalles.Lista[y].Articulo.Codigo);
                    }
                }
            }
            SacarMontosTotales();
        }
        /// <summary>
        /// En un sugerido remueve los detalles que quedan en 0. por no poder reabastecer existencias
        /// </summary>
        public void RemoverDetallesSinExistencia()
        {
           foreach(VentaConsignacion venta in this.Gestionados)
           {

               for (int y = venta.Detalles.Lista.Count - 1; y>= 0; y--)
               {
                   if (!venta.Detalles.Lista[y].VerificarExistencia(venta.Bodega))
                   {
                       venta.EliminarDetalle(venta.Detalles.Lista[y].Articulo.Codigo);
                   }
               }
           }
           SacarMontosTotales();
        }
   
        #endregion

        /// <summary>
        /// Carga las boletas de venta en consignación del cliente en una ruta específica y en las compañías especificadas.
        /// </summary>
        /// <param name="companias">Lista de códigos de las compañías en las cuales el cliente posee boletas de venta en consignación.</param>
        /// <param name="codigoRuta">Código de la ruta.</param>
        /// <param name="codigoCliente">Código del cliente.</param>
        /// <param name="excluirSinSaldos">Indica si se debe obtener solo los detalles que poseen saldos de la boleta de venta en consignación que no ha sido sincronizada.</param>
        public void CargarBoletasVentaConsignacion( Ruta zona, Cliente cliente, bool excluirSinSaldos, bool soloNoSincronizadas)
        {
            this.Gestionados.Clear();
            this.facturas.Gestionados.Clear();
            this.devoluciones.Gestionados.Clear();

            List<VentaConsignacion> ventas = VentaConsignacion.ObtenerBoletas(cliente, zona, soloNoSincronizadas);

            foreach (VentaConsignacion venta in ventas)
            {
                venta.detalles.Encabezado = venta;
                venta.detalles.Obtener(excluirSinSaldos, venta.Configuracion.Nivel);
                this.Gestionados.Add(venta);
            }
        }

        //LDA CR1-04245-MVYY
        //Las ventas en consignacion no utilizan bonificaciones
        /// <summary>
        /// <summary>
        /// Verifica que se cuente con existencias suficientes para los artículos bonificados.
        /// Contiene en cada ítem el artículo asociado al detalle que contiene la línea bonificada que no cuenta con suficientes existencias.
        /// En caso de error el arreglo 'codigosArticulos' contiene los artículos asociados a los detalles que contienen las líneas bonificadas inválidas.
        /// </summary>
        /// <returns>Lista de articulos que no cuentan con suficiente existencia</returns>
        //public List<Articulo> ExistenciasBonificadosInvalidas()
        //{
        //    List<Articulo> detallesInvalidos = new List<Articulo>();

        //    foreach (Pedido factura in facturas.Gestionados)
        //        foreach (DetallePedido detalle in factura.Detalles.Lista)
        //            if (detalle.LineaBonificada != null)
        //                    Esto esta mal, no se deben buscar las existencias en la bodega del cliente,
        //                    se deben de buscar en la del retero, no se arregla puesto que se decide que
        //                    las ventas en consigacion no utilizaran bonificaciones
        //                if (!detalle.LineaBonificada.Articulo.Bodega.SuficienteExistencia(detalle.LineaBonificada.TotalAlmacen))
        //                    detallesInvalidos.Add(detalle.Articulo);

        //    return detallesInvalidos;
        //}

        /// <summary>
        /// LDA CR1-04245-MVYY
        /// Las ventas en cosignacion no utilizan bonificaciones
        /// Se eliminan los articulos bonificados
        /// </summary>
        public void EliminarBonificados()
        {
            List<Articulo> detallesInvalidos = new List<Articulo>();

            foreach (Pedido factura in facturas.Gestionados)
                foreach (DetallePedido detalle in factura.Detalles.Lista)
                    if (detalle.LineaBonificada != null)
                        detalle.LineaBonificada = null;
        }
        #endregion

        #region Acceso Datos
        //ABC 20/08/2008 Caso:32163, 32882 , 32884
        //Se encarga de actualizar el control de desglose en el cliete
        /// <summary>
        /// Actualiza el control de desglose de ventas en consignación en el cliente
        /// </summary>
        public void DesgloseCliente()
        {
            int rows = int.MinValue;
            string sentencia =
                " UPDATE " + Table.ERPADMIN_CLIENTE +
                " SET DESGLOSE = 'S' " +
                " WHERE COD_ZON = @ZONA AND COD_CLT = @CLIENTE";

            try
            {
                foreach (VentaConsignacion boleta in Gestionados)
                {
                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@ZONA", boleta.Zona),
                                                    new SQLiteParameter("@CLIENTE", boleta.Cliente)});

                    rows = GestorDatos.EjecutarComando(sentencia, parametros);
                    if (rows != 1)
                        throw new Exception("No se actualizó el deglose para el cliente '" +boleta.Cliente + "'en la compañia '" + boleta.Compania + "' ruta '" + boleta.Zona + "'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ejecutando la sentencia de actualización del estado de desglose para el cliente. " + ex.Message);
            }
        }
        /// <summary>
        /// LJR 04/03/2009 Caso: 34986,34987,34988
        /// Nueva generacion de la sentencia de actualizacion sobre las consignaciones        
        /// Se genera una sentencia que cambia el estado de las boletas de la HH a Historico, para pasarlas a ALDO
        /// </summary>
        /// <param name="sentenciaActualizacion">Actualizacion de las consignaciones de la HH actual en ALDO</param>
        /// <param name="rutas">Rutas (pocket) donde se debe generar la sentencia en el servidor</param>
        public static void GenerarSentenciaActualizacion(ref string sentenciaActualizacion, List<Ruta> rutas)
        {
            sentenciaActualizacion = string.Empty;

            //Obtenemos la sentencia de eliminación para cada ruta cargada.
            foreach (Ruta ruta in rutas)
            {
                SQLiteDataReader reader = null;
                string sentencia = 
                    " SELECT CLT.COD_CLT, CIA.COD_CIA FROM " +
                    Table.ERPADMIN_CLIENTE + " CLT, " + Table.ERPADMIN_CLIENTE_CIA + " CIA " +
                    " WHERE CLT.DESGLOSE = 'S' AND CLT.COD_CLT = CIA.COD_CLT " +
                    " ORDER BY CLT.COD_CLT";
                try
                {
                    reader = GestorDatos.EjecutarConsulta(sentencia);

                    while (reader.Read())
                    {
                        //ABC 20/08/2008 Caso:32163, 32882 , 32884
                        //Se actualiza la sentecia de tal manera que no elimine las consignaciónes sino las mantega y las cambie de estado
                        //Actualiza el encabezado en el servidor de las boletas de venta en consginación por ruta.
                        sentenciaActualizacion +=
                            "UPDATE %CIA.alFAC_ENC_CONSIG SET DOC_PRO = 'H' " +
                            "WHERE COD_CIA = '" + reader.GetString(1) + "' AND COD_ZON = '" + ruta.Codigo + "' AND COD_CLT = '" + reader.GetString(0) + "'";

                        sentenciaActualizacion += BoletasConsignacion.SEPARADOR;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error obteniendo la sentencia de actualizacion de ventas en consignación en la ruta '" + ruta.Codigo + "'. " + ex.Message);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }
        #endregion 

    }
}