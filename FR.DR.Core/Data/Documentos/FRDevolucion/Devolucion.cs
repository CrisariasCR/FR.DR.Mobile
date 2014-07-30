using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using System.Collections;
using FR.DR.Core.Data.Documentos.Retenciones;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion
{
    public class Devolucion : EncabezadoDocumento, IPrintable
    {
        #region Variables y Propiedades de instancia

        public bool Seleccionado { get; set; }

        private DetallesDevolucion detalles = new DetallesDevolucion();
        /// <summary>
        /// Detalles de la Devolucion
        /// </summary>
        public DetallesDevolucion Detalles
        {
            get { return detalles; }
            set { detalles = value; }
        }

        #region Estado

        private EstadoDocumento estado = EstadoDocumento.Activo;
        /// <summary>
        /// Indica si la devolución está activa o anulada.
        /// </summary>
        public EstadoDocumento Estado
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
        /// indica numero de consignacion (si fue generada por el desglose de una boleta de venta en consignación.)/factura (si es devolucion con documento) de la devolución 
        /// </summary>
        public string NumRef
        {
            get { return numRef; }
            set { numRef = value; }
        }

        private string tipoDevolucion = string.Empty;
        /// <summary>
        /// Indica el tipo de devolucion
        /// </summary>
        public string TipoDevolucion
        {
            get { return tipoDevolucion; }
            set { tipoDevolucion = value; }
        }

        private string automatica = string.Empty;
        /// <summary>
        /// Indica si la devolucion genero automaticamente
        /// </summary>
        public string DevAutomatica
        {
            get { return automatica; }
            set { automatica = value; }
        }
        #endregion

        private string bodegaConsigna = string.Empty;
        /// <summary>
        /// Bodega del cliente de donde proviene la devolucion
        /// </summary>
        public string BodegaConsigna
        {
            get { return bodegaConsigna; }
            set { bodegaConsigna = value; }
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

        private string NcfRef = string.Empty;
        /// <summary>
        /// Indica el NCF del documento referencia
        /// </summary>
        public string NCFRef
        {
            get { return NcfRef; }
            set { NcfRef = value; }
        }

        //KFC Sincronizacion de Listas de Precios
        private string moneda;
        /// <summary>
        /// Indica la moneda con la que se realiza la devolución
        /// </summary>
        public string Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private string nivelPrecioCod = string.Empty;
        /// <summary>
        /// Indica la moneda con la que se realiza la devolución
        /// </summary>
        public string NivelPrecioCod
        {
            get { return nivelPrecioCod; }
            set { nivelPrecioCod = value; }
        }

        #endregion
        private ClienteCia clienteCia;
        public Devolucion()
        { }
        /// <summary>
        /// Crear devolucion
        /// </summary>
        /// <param name="articulo">articulo inicial</param>
        /// <param name="cliente">cliente que se le asocia</param>
        /// <param name="zona">ruta que se le asocia</param>
        /// <param name="bodega">bodega donde devuelve</param>
        public Devolucion(Articulo articulo, ClienteCia cliente, string zona, string bodega)
        {
            clienteCia = cliente;
            Compania = articulo.Compania;
            Cliente = cliente.Codigo;
            Numero = ParametroSistema.ObtenerDevolucion(articulo.Compania, zona);
            if (Numero == string.Empty)
                throw new Exception("No se obtuvo consecutivo de Devolución.");

            HoraInicio = DateTime.Now;
            FechaRealizacion = DateTime.Now.Date;

            PorcentajeDescuento1 = cliente.Descuento;
            NivelPrecio = cliente.NivelPrecio.Codigo;

            //KFC Sincronización listas de Precios
            NivelPrecioCod = cliente.NivelPrecio.Nivel;
            Moneda = cliente.NivelPrecio.Moneda.Equals(TipoMoneda.LOCAL)? "L": "D"; 
            
            
            Bodega = bodega;
            Zona = zona;

            configuracion = new ConfigDocCia();
            configuracion.ClienteCia = cliente;
            configuracion.Nivel = cliente.NivelPrecio;
            configuracion.Compania.Codigo = articulo.Compania;
            configuracion.Compania.CargarConfiguracionImpuestos();

            Configuracion.Compania.Cargar();
            if (Configuracion.Compania.UsaNCF && cliente.TipoContribuyente != string.Empty)
            {
                NCFUtilitario.obtenerNCF(Compania, NCFUtilitario.NOTACREDITO, cliente.TipoContribuyente);
                NCF = NCFUtilitario.consecutivoNCF;
                NCF.cargarNuevoNCF();
            }

            if (ResolucionUtilitario.usaResolucionDevolucion(this.Zona))
            {
                ResolucionUtilitario.obtenerConsecResolucion(this.Compania, ConsecResoluBase.TIPODEVOLUCION, this.Zona);
                this.ConsecResolucion = ResolucionUtilitario.consecutivoResolucion;
            }
            else
            {
                this.ConsecResolucion = null;
            }
        }
        #region Logica Negocios

        /// <summary>
        /// Eliminar un detalle de la devolucion
        /// </summary>
        /// <param name="articulo">articulo asociado al detalle a eliminar</param>
        /// <param name="estado">estado en el que se encuentra el detalle</param>
        public decimal EliminarDetalle(string articulo, Estado estado)
        {
            decimal cantidadRestaurar = 0;
            int pos = -1;
            int cont = 0;

            foreach (DetalleDevolucion detalle in this.Detalles.Lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo) && detalle.Estado == estado)
                {
                    MontoBruto -= detalle.MontoTotal - detalle.MontoDescuento;
                    Impuesto.MontoImpuesto1 -= detalle.Impuesto.Impuesto1;
                    Impuesto.MontoImpuesto2 -= detalle.Impuesto.Impuesto2;
                    //Recalculamos el monto del descuento de la devolución.
                    MontoDescuento1 = MontoBruto * PorcentajeDescuento1 / 100;
                    //this.restarArticulos(detalle);
                    cantidadRestaurar = detalle.TotalAlmacen;
                    pos = cont;
                    break;
                }

                cont++;
            }
            if (pos != -1)
                this.Detalles.Lista.RemoveAt(pos);

            return cantidadRestaurar;
        }

        /// <summary>
        /// Gestionar un nuevo articulo en la devolucion
        /// </summary>
        /// <param name="cliente">cliente al que se asocia la devolucion</param>
        /// <param name="articulo">articulo a gestionar</param>
        /// <param name="unidadesDetalle">unidades en detalle a devolver</param>
        /// <param name="unidadesAlmacen">unidades en almacen a devolver</param>
        /// <param name="estado">estado del articulo a devolver</param>
        /// <param name="observaciones">observaciones al detalle devuelto</param>
        /// <param name="lote">lote del articulo</param>
        public void Gestionar(ClienteCia cliente, Articulo articulo,decimal unidadesDetalle,decimal unidadesAlmacen,
            Estado estado,string observaciones,string lote)
        {
            DetalleDevolucion detalle = detalles.Buscar(articulo.Codigo, estado);

            if (detalle == null)
            {
                if (unidadesAlmacen == 0 && unidadesDetalle == 0)
                    return;

                //Crea un nuevo detalle.
                detalle = new DetalleDevolucion();
                detalle.Articulo = (Articulo)articulo.Clone();
                detalle.Articulo.Bodega.Codigo = this.Bodega;
                detalle.UnidadesAlmacen = unidadesAlmacen;
                detalle.UnidadesDetalle = unidadesDetalle;
                detalle.Estado = estado;
                detalle.Observaciones = observaciones;
                detalle.Lote = lote;
                detalle.Impuesto = articulo.Impuesto;
                detalle.Precio = articulo.Precio;
                this.Detalles.Lista.Add(detalle);
            }
            else
            {
                //Se verifica que la devolución no tenga detalles asociados 
                //para agregarle un detalle si la devolución tiene detalles asociados entonces 
                //se modifica la línea de detalle correspondiente.
                if (unidadesAlmacen == 0 && unidadesDetalle == 0)
                {
                    EliminarDetalle(articulo.Codigo, estado);
                    return;
                }
                //Se modifica el detalle. Restando las cantidades anteriores
                this.MontoBruto -= detalle.MontoTotal;
                this.Impuesto.MontoImpuesto1 -= detalle.Impuesto.MontoImpuesto1;  
                this.Impuesto.MontoImpuesto2 -= detalle.Impuesto.MontoImpuesto2; 

                detalle.UnidadesAlmacen = unidadesAlmacen;
                detalle.UnidadesDetalle = unidadesDetalle;
                detalle.Observaciones = observaciones;
                detalle.Lote = lote;
            }
            //Cambio en las cantidades de la linea
            
            detalle.CalcularMontos(cliente);
            this.MontoBruto += detalle.MontoTotal;            

            this.Impuesto.MontoImpuesto1 += detalle.Impuesto.MontoImpuesto1;
            this.Impuesto.MontoImpuesto2 += detalle.Impuesto.MontoImpuesto2;

            //Recalculamos el monto del descuento de la devolución.
            //this.MontoDescuento1 = this.MontoBruto * (this.PorcentajeDescuento1 / 100);

            this.MontoDescuentoLineas = detalles.MontoDescuentoLineas;

            this.MontoDescuento1 = (this.MontoBruto - this.MontoDescuentoLineas) * (this.PorcentajeDescuento1 / 100);
        }

        //ABC 36136
        /// <summary>
        /// Indicar el total devuelto para una devolucion con documento
        /// </summary>
        /// <param name="articulo"></param>
        /// <param name="totalDev"></param>
        /// <param name="cantBueno"></param>
        /// <param name="cantMalo"></param>
        public void TotalDevuelto(string articulo, ref decimal totalDev, ref decimal cantBueno, ref decimal cantMalo)
        {
                DetalleDevolucion detBueno = this.Detalles.Buscar(articulo, FRArticulo.Estado.Bueno);
                DetalleDevolucion detMalo = this.Detalles.Buscar(articulo, FRArticulo.Estado.Malo);

                if (detBueno != null)
                {
                    cantBueno = (detBueno.UnidadesAlmacen * detBueno.Articulo.UnidadEmpaque) +
                             detBueno.UnidadesDetalle;
                    totalDev = cantBueno;
                }

                if (detMalo != null)
                {
                    cantMalo = (detMalo.UnidadesAlmacen * detMalo.Articulo.UnidadEmpaque) +
                            detMalo.UnidadesDetalle;
                    totalDev += cantMalo;
                }
        }

        //ABC 36136
        /// <summary>
        /// Disponible para devolver de una factura 
        /// </summary>
        /// <param name="art"></param>
        /// <param name="det"></param>
        /// <param name="cantAlmacen"></param>
        /// <param name="cantDetalle"></param>
        /// <param name="estado"></param>
        /// <param name="modificar"></param>
        /// <param name="eliminar"></param>
        /// <returns></returns>
        public bool DisponibleDevolver(Articulo art, DetallePedido det, decimal cantAlmacen, decimal cantDetalle, FRArticulo.Estado estado, bool modificar, bool eliminar)
        {
            decimal cantBueno = 0;
            decimal cantMalo = 0;
            decimal diferencia = 0;
            decimal total = 0;
            bool bandera = true;

            decimal totalDevolver = (cantAlmacen * art.UnidadEmpaque) + cantDetalle;
            TotalDevuelto(art.Codigo, ref total, ref cantBueno, ref cantMalo);

            if (estado == FRArticulo.Estado.Bueno)
                diferencia = totalDevolver - cantBueno;
            else
                diferencia = totalDevolver - cantMalo;

            if (!eliminar)
            {
                diferencia = (det.CantidadDevuelta * art.UnidadEmpaque) + diferencia;

                // CR2-11133-V3CW - KFC 
                // Nota: para que se pueda devolver correctamente unidades fraccionadas deberan coincidir el numero de
                //       decimales para existencias en los parametros de CI y el numero de decimales en la configuracion HH
                // if (diferencia > (det.CantidadFacturada * art.UnidadEmpaque))
                if (Math.Round((diferencia / art.UnidadEmpaque), FRmConfig.CantidadDecimales) > Math.Round(det.CantidadFacturada, FRmConfig.CantidadDecimales))
                    bandera = false;

                if (bandera && modificar)
                {
                    if (diferencia == 0)
                        det.CantidadDevuelta = det.CantidadDevuelta + (totalDevolver / art.UnidadEmpaque);
                    else
                        det.CantidadDevuelta = diferencia / art.UnidadEmpaque;
                }
            }
            else
                det.CantidadDevuelta = det.CantidadDevuelta + (diferencia / art.UnidadEmpaque);

            return bandera;
        }

        public void actualizarMontosPercepcion()
        {
            if (this.MontoSubTotal < Configuracion.Compania.MontoMinimoExcento)
            {
                foreach(DetalleDevolucion detalle in this.Detalles.Lista)
                    detalle.Impuesto.MontoImpuesto2 = 0;

                this.Impuesto.MontoImpuesto2 = 0;
            }                        
        }


        #endregion
        
        #region Metodos de Instancia

        /// <summary>
        /// Actualizar una devolucion
        /// </summary>
        /// <param name="commit">indicar si la transaccion se debe cerrar</param>
        public void Actualizar(bool commit)
        {
            if (detalles.Lista.Count == 0)
                return;
            try
            {
                GestorDatos.BeginTransaction();
                DBEliminar();
                DBGuardar();
                if(commit)
                    GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                if(commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al Actualizar Devolución " + ex.Message);
            }
        }

        public static bool CantidadExedida(string lsArticulo, string lsBodega, decimal ldCantidad)
        {
            #region Definición de variables
            bool procesoExitoso = true;
            bool exedida = false;
            string insertBoleta = string.Empty;
            decimal cant = 0;
            #endregion Definición de variables

            try
            {

                if (procesoExitoso)
                {
                    //Se crea la sentencia para insertar los datos
                    insertBoleta = " SELECT  EXISTENCIA  FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA;
                    insertBoleta = insertBoleta + " WHERE ARTICULO=@ARTICULO AND BODEGA=@BODEGA ";

                    //Se crean los parámetros
                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, lsArticulo),
                                                    GestorDatos.SQLiteParameter("@BODEGA", SqlDbType.NVarChar, lsBodega)
                                                  });

                    //Se ejecuta la sentencia
                    object obj = GestorDatos.EjecutarScalar(insertBoleta, parametros);
                    cant = Convert.ToDecimal(obj);
                    exedida = ldCantidad > cant;
                }
            }
            catch (Exception ex)
            {
                procesoExitoso = false;
                throw ex;
            }


            return exedida;


        }

        /// <summary>
        /// Guardar una devolucion
        /// </summary>
        /// <param name="commit">indicar si la transaccion se debe cerrar</param>
        public void Guardar(bool commit)
        {
            try
            {
                GestorDatos.BeginTransaction();

                if (this.Configuracion.Compania.UsaNCF && clienteCia.TipoContribuyente != string.Empty)
                    NCF.IncrementarConsecutivoNCF();

                if (this.Configuracion.Compania.Retenciones)
                {
                    this.CalcularRetenciones();
                }

                DBGuardar();

                if (!ResolucionUtilitario.usaResolucionDevolucion(this.Zona))
                {
                    ParametroSistema.IncrementarDevolucion(Compania, Zona);
                }
                else
                {
                    this.ConsecResolucion.IncrementarConsecutivoRes();
                }                              

                if(commit)
                    GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                if(commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al Guardar Devolución. " + ex.Message);
            }
        }
        /// <summary>
        /// Devuelve true si es Tipo Efectivo
        /// </summary>
        /// <returns></returns>
        public bool esTipoEfectivo()
        {
            try
            {
                string sentencia = "SELECT FORMA_PAGO_DEV FROM " + Table.ERPADMIN_alFAC_ENC_DEV + " WHERE NUM_DEV=@NUMDEVOL AND COD_ZON=@RUTA";
                SQLiteParameterList parametros = new SQLiteParameterList();
                parametros.Add("NUMDEVOL", Numero);
                parametros.Add("RUTA", this.Zona);
                return Convert.ToString(GestorDatos.EjecutarScalar(sentencia, parametros)).Equals(FRmConfig.TipoPagoEfectivo);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve true si es Moneda Local
        /// </summary>
        /// <returns></returns>
        public bool esMonedaLocal()
        {
            try
            {
                string sentencia = "SELECT MONEDA FROM " + Table.ERPADMIN_alFAC_ENC_DEV + "  WHERE NUM_DEV=@NUMDEVOL AND COD_ZON=@RUTA";
                SQLiteParameterList parametros = new SQLiteParameterList();
                parametros.Add("NUMDEVOL", Numero);
                parametros.Add("RUTA", this.Zona);
                return Convert.ToString(GestorDatos.EjecutarScalar(sentencia, parametros)).Equals("L");
            }
            catch (Exception e)
            {
                return false;
            }
        }

        ///// <summary>
        ///// Actualiza los valores en la tabla JORNADA_RUTAS 
        ///// </summary>
        ///// <param name="ruta"></param>
        ///// <param name="monto"></param>
        private bool ActualizarJornada(string ruta, decimal monto)
        {
            string colCantidad = "";
            string colMonto = "";

            if (!esTipoEfectivo())
            {
                if (esMonedaLocal())
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_LOCAL;
                    colMonto = JornadaRuta.MONTO_DEVOLUCIONES_LOCAL;
                }
                else
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_DOLAR;
                    colMonto = JornadaRuta.MONTO_DEVOLUCIONES_DOLAR;
                }
            }
            else
            {
                if (esMonedaLocal())
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_EFC_LOCAL;
                    colMonto = JornadaRuta.MONTO_DEVOLUCION_EFC_LOCAL;
                }
                else
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_EFC_DOLAR;
                    colMonto = JornadaRuta.MONTO_DEVOLUCION_EFC_DOLAR;
                }
            }

            try
            {
                JornadaRuta.ActualizarRegistro(ruta, colCantidad, -1);
                JornadaRuta.ActualizarRegistro(ruta, colMonto, -monto);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Anular una devolucion
        /// </summary>
        /// <param name="commit">indicar si la transaccion se debe cerrar</param>
        public void Anular(bool commit)
        {
            try
            {
                GestorDatos.BeginTransaction();
                if (ActualizarJornada(Zona, MontoNeto))
                {
                    this.DBAnular();
                    if (commit)
                        GestorDatos.CommitTransaction();
                }
                else
                {
                    if (commit)
                        GestorDatos.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                if(commit)
                    GestorDatos.RollbackTransaction();
                throw new Exception("Error al Anular Devolución. " + ex.Message);
            }
        }

        #region Manipulacion Detalles

        /// <summary>
        /// Obtener los detalles de la devolucion
        /// </summary>
        public void ObtenerDetalles()
        {
            detalles.Encabezado.Compania = this.Compania;
            detalles.Encabezado.Numero = this.Numero;
            detalles.Encabezado.Zona = this.Zona;
            detalles.Bodega = this.Bodega;
            detalles.Obtener();

            this.MontoDescuentoLineas = detalles.MontoDescuentoLineas;
        }

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
                        new SQLiteParameter("@TIPO_DOC_RETENCION",SqlDbType.VarChar,"D"),
                    });

                    int registro = GestorDatos.EjecutarComando(comando, parametros);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error guardando las retenciones del documento." + ex.Message);
            }

        }

        #endregion Retenciones

        #endregion

        #region Acceso Datos

        /// <summary>
        /// Obtener los detalles de la devolucion
        /// </summary>
        /// <param name="cliente">cliente asociado</param>
        /// <param name="indiceAnulacion">estado de la devolucion</param>
        /// <returns>lista de detalles</returns>
        public static List<Devolucion> ObtenerDevoluciones(string cliente, EstadoDocumento indiceAnulacion)
        {
            string sentencia =
                "SELECT NUM_DEV,COD_CIA,COD_ZON,HOR_INI,HOR_FIN," +
                      " FEC_DEV,LST_PRE,MON_SIV,MON_DSC,POR_DSC_AP," +
                      " MON_IMP_VT,MON_IMP_CS,COD_BOD,IMPRESO,CONSIGNACION,NUM_REF, NCF_PREFIJO, NCF, NIVEL_PRECIO, MONEDA " +
                " FROM " + Table.ERPADMIN_alFAC_ENC_DEV +
                " WHERE COD_CLT = @CLIENTE" +
                " AND EST_DEV = @ESTADO AND DOC_PRO IS NULL";

            SQLiteDataReader reader = null;
            List<Devolucion> devoluciones = new List<Devolucion>();
            devoluciones.Clear();

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                        new SQLiteParameter("@CLIENTE", cliente), 
                        new SQLiteParameter("@ESTADO", ((char)indiceAnulacion).ToString())});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Devolucion devolucion = new Devolucion();
                    devolucion.Estado = indiceAnulacion;
                    devolucion.Numero = reader.GetString(0);
                    devolucion.Compania = reader.GetString(1);
                    devolucion.Zona = reader.GetString(2);
                    devolucion.HoraInicio = reader.GetDateTime(3);
                    devolucion.HoraFin = reader.GetDateTime(4);
                    devolucion.FechaRealizacion = reader.GetDateTime(5);
                    devolucion.NivelPrecio = reader.GetInt32(6);
                    devolucion.MontoBruto = reader.GetDecimal(7);
                    devolucion.MontoDescuento1 = reader.GetDecimal(8);
                    devolucion.PorcentajeDescuento1 = reader.GetDecimal(9);
                    devolucion.Impuesto = new Impuesto(reader.GetDecimal(10), reader.GetDecimal(11));
                    devolucion.Bodega = reader.GetString(12);
                    devolucion.Impreso = reader.GetString(13).Equals("S");
                    devolucion.EsConsignacion = reader.GetString(14).Equals("S");
                    if (!reader.IsDBNull(15))
                        devolucion.NumRef = reader.GetString(15);
                    devolucion.Cliente = cliente;

                    if (!reader.IsDBNull(16) && !reader.IsDBNull(17) )
                    {
                        devolucion.NCF = new NCFBase();
                        devolucion.NCF.Prefijo = reader.GetString(16);
                        devolucion.NCF.UltimoValor= reader.GetString(17);
                    }

                    devoluciones.Add(devolucion);
                }

                return devoluciones;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        /// <summary>
        /// Guardar la devolucion en la base de datos
        /// </summary>
        private void DBGuardar() 
        {
            this.HoraFin = DateTime.Now;
            string serie = string.Empty;
            //Verifica si tiene resolucion para cambiar el numero del pedido
            if (ResolucionUtilitario.usaResolucionFactura(this.Zona))
            {
                serie = ConsecResolucion.Serie;
            }

            //Se procede a guardar el encabezado
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_ENC_DEV +
                "        ( COD_CIA,  COD_ZON,  COD_CLT,  NUM_DEV,  HOR_INI,  HOR_FIN,  FEC_DEV, NUM_ITM,   EST_DEV,  COD_BOD,  LST_PRE,  MON_SIV,  MON_DSC,  POR_DSC_AP, MON_IMP_VT,  MON_IMP_CS,  IMPRESO,  CONSIGNACION, NUM_REF, AUTOMATICA, TIPO, NCF_PREFIJO, NCF, NIVEL_PRECIO, MONEDA,COD_PAIS,COD_GEO1,COD_GEO2) " +
                " VALUES (@COD_CIA, @COD_ZON, @COD_CLT, @NUM_DEV, @HOR_INI, @HOR_FIN, @FEC_DEV, @NUM_ITM, @EST_DEV, @COD_BOD, @NVL_PRE, @MON_SIV, @MON_DSC, @POR_DSC_AP, @MON_IMP_VT, @MON_IMP_CS, @IMPRESO, @CONSIGNACION, @NUM_REF, @AUTOMATICA, @TIPO, @NCF_PREFIJO, @NCF, @NVL_PRE_COD, @MONEDA,@PAIS,@CODGEO1,@CODGEO2)";  //DOC_PRO OBS_DEV

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_DEV",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, HoraInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, HoraFin),
                GestorDatos.SQLiteParameter("@FEC_DEV",SqlDbType.DateTime, FechaRealizacion),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.Lista.Count),
                GestorDatos.SQLiteParameter("EST_DEV", SqlDbType.NVarChar,((char)Estado).ToString()),
                GestorDatos.SQLiteParameter("@COD_BOD",SqlDbType.NVarChar, (EsConsignacion ? BodegaConsigna : Bodega)),
                GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int, NivelPrecio),
                GestorDatos.SQLiteParameter("@MON_SIV",SqlDbType.Decimal, MontoBruto),
                GestorDatos.SQLiteParameter("@MON_DSC",SqlDbType.Decimal, MontoDescuento1),
                GestorDatos.SQLiteParameter("@POR_DSC_AP",SqlDbType.Decimal, PorcentajeDescuento1), 
                GestorDatos.SQLiteParameter("@MON_IMP_VT",SqlDbType.Decimal,Impuesto.MontoImpuesto1),
                GestorDatos.SQLiteParameter("@MON_IMP_CS", SqlDbType.Decimal,Impuesto.MontoImpuesto2),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, (Impreso? "S" : "N")),
                GestorDatos.SQLiteParameter("@CONSIGNACION",SqlDbType.NVarChar, (EsConsignacion ? "S" : "N")),
                GestorDatos.SQLiteParameter("@NUM_REF",SqlDbType.NVarChar, numRef),
                GestorDatos.SQLiteParameter("@AUTOMATICA",SqlDbType.NVarChar, automatica),
                GestorDatos.SQLiteParameter("@TIPO",SqlDbType.NVarChar, tipoDevolucion),
                //ABC Manejo NCF
                GestorDatos.SQLiteParameter("@NCF_PREFIJO", SqlDbType.NVarChar,(NCF != null ? NCF.Prefijo : string.Empty)),
                GestorDatos.SQLiteParameter("@NCF", SqlDbType.NVarChar,(NCF != null ? NCF.UltimoValor: string.Empty)) ,
            //,GestorDatos.SQLiteParameter("@OBS_DET",SqlDbType.NVarChar, Notas),                          };

                //KFC Sincronización Listas de precio
                GestorDatos.SQLiteParameter("@NVL_PRE_COD",SqlDbType.NVarChar, NivelPrecioCod),
                GestorDatos.SQLiteParameter("@MONEDA",SqlDbType.NVarChar, Moneda),
                //Retenciones
                GestorDatos.SQLiteParameter("@PAIS",SqlDbType.NVarChar, this.Configuracion.Pais.Codigo),
                GestorDatos.SQLiteParameter("@CODGEO1",SqlDbType.NVarChar, this.Configuracion.Pais.DivisionGeografica1),
                GestorDatos.SQLiteParameter("@CODGEO2",SqlDbType.NVarChar, this.Configuracion.Pais.DivisionGeografica2)
            
            });

            int encabezado = GestorDatos.EjecutarComando(sentencia, parametros);

            if (encabezado != 1)
                throw new Exception("No se puede guardar el encabezado de la devolución '" + this.Numero + "'.");

            //se guardan los detalles de devolucion
            detalles.Bodega = Bodega;
            detalles.Encabezado = this;
            detalles.NivelPrecio = this.NivelPrecio;
            detalles.DocumentoAsociado = numRef;
            bool devolucionConDocumento = (!EsConsignacion && !numRef.Equals(string.Empty));

            detalles.Guardar(devolucionConDocumento);

            //Guarda las retenciones
            if (iArregloRetenciones != null && iArregloRetenciones.Count > 0)
            {
                this.GuardarRetenciones();
            }

        }
        /*private void DBGuardar()
        {
            this.HoraFin = DateTime.Now;

            //Se procede a guardar el encabezado
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alFAC_ENC_DEV +
                "        ( COD_CIA,  COD_ZON,  COD_CLT,  NUM_DEV,  HOR_INI,  HOR_FIN,  FEC_DEV, NUM_ITM,   EST_DEV,  COD_BOD,  LST_PRE,  MON_SIV,  MON_DSC,  POR_DSC_AP, MON_IMP_VT,  MON_IMP_CS,  IMPRESO,  CONSIGNACION, NUM_REF, AUTOMATICA, TIPO, NCF_PREFIJO, NCF) " +//, NIVEL_PRECIO, MONEDA) " +
                " VALUES (@COD_CIA, @COD_ZON, @COD_CLT, @NUM_DEV, @HOR_INI, @HOR_FIN, @FEC_DEV, @NUM_ITM, @EST_DEV, @COD_BOD, @NVL_PRE, @MON_SIV, @MON_DSC, @POR_DSC_AP, @MON_IMP_VT, @MON_IMP_CS, @IMPRESO, @CONSIGNACION, @NUM_REF, @AUTOMATICA, @TIPO, @NCF_PREFIJO, @NCF)"; //, @NVL_PRE_COD, @MONEDA)";  //DOC_PRO OBS_DEV
            
            SQLiteParameterList parametros = {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@NUM_DEV",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, HoraInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, HoraFin),
                GestorDatos.SQLiteParameter("@FEC_DEV",SqlDbType.DateTime, FechaRealizacion),
                GestorDatos.SQLiteParameter("@NUM_ITM",SqlDbType.Int, detalles.Lista.Count),
                GestorDatos.SQLiteParameter("EST_DEV", SqlDbType.NVarChar,((char)Estado).ToString()),
                GestorDatos.SQLiteParameter("@COD_BOD",SqlDbType.NVarChar, (EsConsignacion ? BodegaConsigna : Bodega)),
                GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int, NivelPrecio),
                GestorDatos.SQLiteParameter("@MON_SIV",SqlDbType.Decimal, MontoBruto),
                GestorDatos.SQLiteParameter("@MON_DSC",SqlDbType.Decimal, MontoDescuento1),
                GestorDatos.SQLiteParameter("@POR_DSC_AP",SqlDbType.Decimal, PorcentajeDescuento1), 
                GestorDatos.SQLiteParameter("@MON_IMP_VT",SqlDbType.Decimal,Impuesto.MontoImpuesto1),
                GestorDatos.SQLiteParameter("@MON_IMP_CS", SqlDbType.Decimal,Impuesto.MontoImpuesto2),
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar, (Impreso? "S" : "N")),
                GestorDatos.SQLiteParameter("@CONSIGNACION",SqlDbType.NVarChar, (EsConsignacion ? "S" : "N")),
                GestorDatos.SQLiteParameter("@NUM_REF",SqlDbType.NVarChar, numRef),
                GestorDatos.SQLiteParameter("@AUTOMATICA",SqlDbType.NVarChar, automatica),
                GestorDatos.SQLiteParameter("@TIPO",SqlDbType.NVarChar, tipoDevolucion),
                //ABC Manejo NCF
                GestorDatos.SQLiteParameter("@NCF_PREFIJO", SqlDbType.NVarChar,(NCF != null ? NCF.Prefijo : string.Empty)),
                GestorDatos.SQLiteParameter("@NCF", SqlDbType.NVarChar,(NCF != null ? NCF.UltimoValor: string.Empty)) };
               //,GestorDatos.SQLiteParameter("@OBS_DET",SqlDbType.NVarChar, Notas),                          };
                
                //KFC Sincronización Listas de precio

            int encabezado = GestorDatos.EjecutarComando(sentencia, parametros);

            if (encabezado != 1)
                throw new Exception("No se puede guardar el encabezado de la devolución '" + this.Numero + "'.");

            //se guardan los detalles de devolucion
            detalles.Bodega = Bodega;
            detalles.Encabezado = this;
            detalles.NivelPrecio = this.NivelPrecio;
            detalles.DocumentoAsociado = numRef;
            bool devolucionConDocumento = (!EsConsignacion && !numRef.Equals(string.Empty));

            detalles.Guardar(devolucionConDocumento);

        }*/

        /// <summary>
        /// Eliminar la devolucion de la base de datos 
        /// </summary>
        private void DBEliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_ENC_DEV +
                " WHERE COD_CIA = @COD_CIA" +
                " AND COD_ZON = @COD_ZON " +
                " AND NUM_DEV = @NUM_DEV";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_DEV", SqlDbType.NVarChar, Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows != 1)
                throw new Exception((rows < 1) ? "No se eliminó ningún registro de devolución." : "Se eliminó más de un registro de devolución.");

            detalles.Eliminar();

        }
        /// <summary>
        /// Anular la devolucion de la base de datos 
        /// </summary>
        public void DBAnular()
        {
            this.Estado = EstadoDocumento.Anulado;

            string sentencia =
                " UPDATE " + Table.ERPADMIN_alFAC_ENC_DEV +
                " SET EST_DEV = @ESTADO " +
                " WHERE COD_CIA = @COD_CIA " +
                " AND COD_ZON = @COD_ZON " +
                " AND NUM_DEV = @NUM_DEV " +
				" AND DOC_PRO IS NULL";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_DEV", SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar, ((char)Estado).ToString())});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows != 1)
                throw new Exception((rows < 1) ? "No se anuló ningún registro de devolución." : "Se anuló más de un registro de devolución.");
            
            //Rebaja existencias en los detalles
            detalles.Bodega = this.Bodega;
            detalles.Anular(this.esConsignacion);
            
        }
        /// <summary>
        /// Actualizar la colilla de impresion
        /// </summary>
        public void DBActualizarImpresion()
        {
            string sentencia =
                " UPDATE " + Table.ERPADMIN_alFAC_ENC_DEV +
                " SET IMPRESO = @IMPRESO " +
                " WHERE COD_ZON = @COD_ZON " +
                " AND NUM_DEV = @NUM_DEV " +
                " AND DOC_PRO IS NULL";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_ZON", SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_DEV", SqlDbType.NVarChar, Numero),
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
        /// Indicar si hay devoluciones pendientes de carga
        /// </summary>
        /// <returns>devoluciones pendientes</returns>
        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_alFAC_ENC_DEV + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }


        #region  Facturas de contado y recibos en FR - KFC

        public void ActualizarTipoDevolucion(string documento, string tipoDev)
        {
            string sentencia =
                    " UPDATE " + Table.ERPADMIN_alFAC_ENC_DEV +
                    // CR2-11740-4NDQ - KFC / Cambio de campo TIPO a FORMA_PAGO_DEV
                    " SET FORMA_PAGO_DEV = @TIPO_PAGO " +
                    //" DOC_PRO = @PROCESADO " +
                    " WHERE NUM_DEV = @NUM_DEV";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                        GestorDatos.SQLiteParameter("@TIPO_PAGO",SqlDbType.NVarChar, tipoDev),
                        GestorDatos.SQLiteParameter("@NUM_DEV",SqlDbType.NVarChar, documento)//,
                       // GestorDatos.SQLiteParameter("@PROCESADO",SqlDbType.NVarChar, "S") // para efecto de las devol en efectivo
                                                  });
            try
            {
                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        


        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            // TODO:  Add Devolucion.GetObjectName implementation
            return "DEVOLUCION";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                //Caso:32682 ABC 23/05/2008 Mostrar Total de Articulo y Total de Lineas
                //Caso:36136 ABC Devolucion con documento
                case "DETALLE_DEVOLUCION": return new ArrayList(detalles.Lista);
                case "TOTAL_ARTICULOS": return detalles.TotalArticulos;
                case "TOTAL_LINEAS": return detalles.TotalArticulos;
                case "DOC_REFERENCIA": return NumRef;
                case "NCF_REFERENCIA": return NcfRef;
                default:
                    return base.GetField(name);
                //Caso: 32380 ABC 14/05/2008 Disponibilizar nuevos datos para reporte Venta en Consignación
                /*if ("CLIENTE_TELEFONO" == name)
                    return this.ClienteAsociado.Telefono;*/
            }
        }

        #endregion

    }
}
