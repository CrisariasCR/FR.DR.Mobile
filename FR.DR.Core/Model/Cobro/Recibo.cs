using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using System.Collections;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.Cobro 
{
    public class Recibo : DocumentoContable, IPrintable
    {
        #region Constructores

        /// <summary>
        /// Creacion del recibo de cobro
        /// </summary>
        public Recibo()
        {
            this.Tipo = TipoDocumento.Recibo;
            this.FechaRealizacion = DateTime.Now;
            this.Moneda = Cobros.TipoMoneda;
            this.Estado = EstadoDocumento.Activo;
        }
        /// <summary>
        /// Creacion del recibo de cobro a un cliente
        /// </summary>
        public Recibo(string cliente)
        {
            this.Tipo = TipoDocumento.Recibo;
            this.Cliente = cliente;
            this.FechaRealizacion = DateTime.Now;
            this.Moneda = Cobros.TipoMoneda;
            this.Estado = EstadoDocumento.Activo;
        }
        #endregion

        #region Variables y Propiedades de instancia

        private ClienteBase clienteDatos;
        /// <summary>
        /// Conserva los datos basicos del cliente para los reportes
        /// </summary>
        public ClienteBase ClienteDatos
        {
            get { return clienteDatos; }
            set { clienteDatos = value; }
        }

        public decimal SumaCobros
        {
            get { return TotalEfectivo + TotalCheques; }
        }

        private string monto = string.Empty;
        /// <summary>
        /// monto(para uso de listview SeleccionRecibos android)
        /// </summary>
        public string Monto
        {
            get { return monto; }
            set { monto = value; }
        }

        private decimal montoChequesLocal = 0;
        /// <summary>
        /// monto por concepto de cheques
        /// </summary>
        public decimal MontoChequesLocal
        {
            get { return montoChequesLocal; }
            set { montoChequesLocal = value; }
        }

        private decimal montoEfectivoLocal = 0;
        /// <summary>
        /// monto por concepto de efectivo
        /// </summary>
        public decimal MontoEfectivoLocal
        {
            get { return montoEfectivoLocal; }
            set { montoEfectivoLocal = value; }
        }

        private decimal montoChequesDolar = 0;
        /// <summary>
        /// Monto en dolares por concepto de cheques
        /// </summary>
        public decimal MontoChequesDolar
        {
            get { return montoChequesDolar; }
            set { montoChequesDolar = value; }
        }

        private decimal montoEfectivoDolar = 0;
        /// <summary>
        /// Monto en dolares por concepto de efectivo
        /// </summary>
        public decimal MontoEfectivoDolar
        {
            get { return montoEfectivoDolar; }
            set { montoEfectivoDolar = value; }
        }

        private DateTime fechaInicio = DateTime.Now;
        /// <summary>
        /// Fecha de inicio del recibo
        /// </summary>
        public DateTime FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; }
        }

        private DateTime fechaFinalizacion = DateTime.Now;
        /// <summary>
        /// Fecha de finalizacion del recibo
        /// </summary>
        public DateTime FechaFinalizacion
        {
            get { return fechaFinalizacion; }
            set { fechaFinalizacion = value; }
        }

        private List<DetalleRecibo> detalles = new List<DetalleRecibo>();
        /// <summary>
        /// detalles del recibo
        /// </summary>
        public List<DetalleRecibo> Detalles
        {
            get { return detalles; }
            set { detalles = value; }
        }
        
        private List<NotaCredito> notasCreditoAsociadas = new List<NotaCredito>();
        /// <summary>
        /// Notas de credito asociadas al recibo
        /// </summary>
        public List<NotaCredito> NotasCreditoAsociadas
        {
            get { return notasCreditoAsociadas; }
            set { notasCreditoAsociadas = value; }
        }

        private List<Factura> facturasAsociadas = new List<Factura>();
        /// <summary>
        /// facturas asociadas al recibo
        /// </summary>
        public List<Factura> FacturasAsociadas
        {
            get { return facturasAsociadas; }
            set { facturasAsociadas = value; }
        }

        /// <summary>
        /// Se usa para saber si el checkbox asociado está o no seleccionado
        /// </summary>
        public bool Seleccionado { get; set; }

        #region  Facturas de contado y recibos en FR - KFC

        private string numeroPedido = string.Empty;
        /// <summary>
        /// Es el numero de factura en caso de ser una factura de contado
        /// </summary>
        public string NumeroPedido
        {
            get { return numeroPedido; }
            set { numeroPedido = value; }
        }

        private Pedido pedido = new Pedido();
        /// <summary>
        /// Es el numero de factura en caso de ser una factura de contado
        /// </summary>
        public Pedido Pedido
        {
            get { return pedido; }
            set { pedido = value; }
        }

        #endregion
        


        /// <summary>
        /// Retorna el monto de las facturas segun la moneda del recibo
        /// </summary>
        public decimal TotalFacturas
        {

            get
            {
                decimal totalCobrado = 0;
                foreach (Factura factura in this.FacturasAsociadas)
                {
                    if (this.Moneda == TipoMoneda.LOCAL)
                        totalCobrado += factura.MontoMovimientoLocal;
                    else
                        totalCobrado += factura.MontoMovimientoDolar;
                }

                return totalCobrado;
            }
        }

        /// <summary>
        /// Retorna el monto en cheques segun la moneda del recibo
        /// </summary>
        public decimal TotalCheques
        {
            get
            {
                if (this.Moneda == TipoMoneda.LOCAL)
                    return this.MontoChequesLocal;
                else
                    return this.MontoChequesDolar;
            }

        }

        /// <summary>
        /// Retorna el monto en efectivo segun la moneda del recibo
        /// </summary>
        public decimal TotalEfectivo
        {
            get
            {
                if (this.Moneda == TipoMoneda.LOCAL)
                    return this.MontoEfectivoLocal;
                else
                    return this.MontoEfectivoDolar;
            }

        }

        /// <summary>
        /// Retorna el monto en notas de credito segun la moneda del recibo
        /// </summary>
        public decimal TotalNotasCredito
        {

            get
            {
                decimal total = 0;

                foreach (Factura factura in this.FacturasAsociadas)
                    total += factura.TotalNotasCredito;

                return total;
            }

        }
        /// <summary>
        /// Lista de descuentos por pronto pago
        /// </summary>
        private List<FacturaDescuento> descuentos;

        /// <summary>
        /// Obtiene o cambia la lista de desucento por pronto pago.
        /// </summary>
        public List<FacturaDescuento> Descuentos
        {
            get
            {
                return descuentos;
            }
            set
            {
                descuentos = value;
            }
        }

        #endregion

        #region Logica Negocio

        /// <summary>
        /// Obtiene las referencias de los cheques asociados
        /// </summary>
        /// <returns></returns>
        public string ObtenerReferenciasCheques()
        {
            string referencias = string.Empty;
            foreach (Cheque cheque in ChequesAsociados)
            {
                    if (referencias == string.Empty)
                        referencias = cheque.Numero;
                    else
                        referencias += "," + cheque.Numero;
            }
            if (referencias.Length > 80)
            {
                //El campo de referencias es de 80 caracteres solamente
                referencias = referencias.Substring(0, 80);
            }  
            return referencias;
        }

        /// <summary>
        /// Metodo encargado de hacer persistente en la base de
        /// datos toda la informacion que se recolecto del cobro.
        /// </summary>
        public void Guardar()
        {
            GestorDatos.BeginTransaction();

            this.FechaFinalizacion = System.DateTime.Now;

            try
            {
                this.DBGuardar();

                /*try
                {
                    if (Cobros.AplicarDescuentosProntoPago)
                        this.GuardarNotasCreditoPorDescuento();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando movimientos de notas de crédito por descuentos por pronto pago. " + ex.Message);
                }
                */
                try
                {
                    this.GuardaNotasCredito();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando notas de credito. " + ex.Message);
                }

                try
                {
                    this.GuardaFacturasAfectadasNC();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando movimientos de notas. " + ex.Message);
                }

                // LAS. Se incluye el proceso de guardar las notas de crédito por descuentos por pronto pago.
                try
                {
                    if(Cobros.AplicarDescuentosProntoPago)
                        this.GuardarNotasCreditoPorDescuento();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando movimientos de notas de crédito por descuentos por pronto pago. " + ex.Message);
                }

                try
                {
                    this.GuardaReciboFactura();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando movimiento de efectivo y cheques. " + ex.Message);
                }

                try
                {
                    this.GuardarCheques();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando cheques. " + ex.Message);
                }

                try
                {
                    this.ActualizaPendientesCobros();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error actualizando pendientes de cobro. " + ex.Message);
                }

                try
                {
                    if (!Cobros.CambiarNumeroRecibo)
                        ParametroSistema.IncrementarRecibo(this.Compania, this.Zona);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error actualizando consecutivos. " + ex.Message);
                }

                this.CambiarMonedaADetalles();

                GestorDatos.CommitTransaction();

            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                throw ex;
            }
        }
        

        /// <summary>
        /// Anula el recibo en la base de datos.
        /// </summary>
        public void Anular(bool anularTodo, string ruta, BaseViewModel viewModel)
        {
            
            GestorDatos.BeginTransaction();
            try
            {
                this.AnularEncabezado(anularTodo);
                this.AnularDetalle(anularTodo);
                try
                {
                    this.ModificaPendientesCobros();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error actualizando pendientes de cobro. " + ex.Message);
                }

                GestorDatos.CommitTransaction();

                // Modificaciones en funcionalidad de recibos de contado - KFC
                if (anularTodo)
                {
                    decimal totalFactura = this.MontoChequesLocal + this.MontoEfectivoLocal + this.TotalNotasCredito;
                    
                    if (this.Moneda == TipoMoneda.LOCAL)
                        ActualizarJornada(ruta, this.Moneda, -1, totalFactura, this.MontoChequesLocal, this.MontoEfectivoLocal, this.TotalNotasCredito, viewModel);
                    if (this.Moneda == TipoMoneda.DOLAR)
                        ActualizarJornada(ruta, this.Moneda, -1, this.MontoDocDolar, this.MontoChequesDolar, this.MontoEfectivoDolar, this.TotalNotasCredito, viewModel);
                }
                else
                {
                    if (this.Moneda == TipoMoneda.LOCAL)
                        ActualizarJornada(ruta, this.Moneda, 0, this.TotalNotasCredito, 0, 0, this.TotalNotasCredito, viewModel);
                    if (this.Moneda == TipoMoneda.DOLAR)
                        ActualizarJornada(ruta, this.Moneda, 0, this.TotalNotasCredito, 0, 0, this.TotalNotasCredito, viewModel);
                }
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                throw ex;
            }
        }

        /// <summary>
        /// Anula el encabezado del recibo
        /// </summary>
        private void AnularEncabezado(bool anularTodo)
        {            
            try
            {
                string sentencia =
                    " UPDATE " + Table.ERPADMIN_alCXC_DOC_APL +
                    " SET   IND_ANL = @ESTADO " +
                    " WHERE NUM_REC = @CONSECUTIVO " +
                    " AND   UPPER(COD_CIA) = @COMPANIA " +
                    " AND   COD_CLT = @CLIENTE ";

                if (!anularTodo)
                {
                    sentencia = sentencia + " AND COD_TIP_DC in ('7','15')";
                }

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {            
                    new SQLiteParameter("@CONSECUTIVO", this.Numero),
                    new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Anulado).ToString()),
                    new SQLiteParameter("@CLIENTE", this.Cliente),
                    new SQLiteParameter("@COMPANIA", this.Compania.ToUpper())});

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando encabezado. " + ex.Message);
            }

        }
        /// <summary>
        /// Anula los detalles del recibo
        /// </summary>
        private void AnularDetalle(bool anularTodo)
        {
            

            try
            {
                //LJR Caso 37677 -> Error al migrar
                string sentencia =
                    " UPDATE " + Table.ERPADMIN_alCXC_MOV_DIR +
                    " SET   IND_ANL = @ESTADO " +
                    " WHERE NUM_REC = @CONSECUTIVO " +
                    " AND   UPPER(COD_CIA) = @COMPANIA " +
                    " AND   COD_CLT = @CLIENTE ";

                if (!anularTodo)
                {
                    sentencia = sentencia + " AND COD_TIP_DC in ('7','15')";
                }

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {            
                    new SQLiteParameter("@CONSECUTIVO", this.Numero),
                    new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Anulado).ToString()),
                    new SQLiteParameter("@CLIENTE", this.Cliente),
                    new SQLiteParameter("@COMPANIA", this.Compania.ToUpper())});

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando detalles. " + ex.Message);
            }
        }

        


        /// <summary>
        /// Metodo que actualiza los montos y fechas de los documentos afectados en el 
        /// repositorio
        /// </summary>
        private void ActualizaPendientesCobros()
        {
            foreach (DetalleRecibo detalle in this.Detalles)
            {
                detalle.ActualizaPendienteCobro(this.Zona, this.Cliente, this.Compania);
            }
        }

		/// <summary>
		/// Actualiza los montos de los documentos pendientes de cobro que fueron 
		/// pagados con el cobro por anular.
		/// </summary>
        private void ModificaPendientesCobros()
        {
            foreach (DetalleRecibo detalle in this.Detalles)
            {
                detalle.AnulaPendienteCobro(this.Zona, this.Cliente, this.Compania);
            }
        }
		#region Insert de las Notas de credito

		/// <summary>
		/// Guarda en la base de datos las notas de credito incluidas en el recibo.
		/// </summary>
        private void GuardaNotasCredito()
        {
            foreach (NotaCredito nota in this.NotasCreditoAsociadas)
            {
                nota.Guardar(this);
            }
        }

        #endregion 

		#region Insert de las aplicaciones de las notas a las facturas en la BD

		/// <summary>
		/// Guarda en la base de datos la informacion que se genero de 
		/// las facturas que se vieron afectadas por las notas de credito
		/// </summary>
        private void GuardaFacturasAfectadasNC()
        {
            foreach (Factura factura in this.FacturasAsociadas)
            {
                factura.GuardarAfectadaNC(this.Numero);
            }
        }

        /// <summary>
        /// Guarda las notas de crédito generadas por los descuentos.
        /// </summary>
        private void GuardarNotasCreditoPorDescuento()
        {
            if (descuentos != null)
            {                
                foreach (FacturaDescuento descuento in descuentos)
                {
                    foreach (Factura factura in this.FacturasAsociadas)
                    {
                        if (factura.Numero == descuento.Factura.Numero)
                        {
                            //KFC - CR2-10044-6T0H - Solo crea N/C por pronto pago de las facturas que se cancelan totalmente
                            //CARIAS- Mejora se cambia para que se puedan hacer descuentos por pronto pago en pagos parciales al igual que en el ERP.
                            if ((factura.SaldoDocLocal >= 0 && !FRdConfig.ProntoPagoTotales) || factura.SaldoDocLocal == 0)
                            {
                                descuento.Guardar(this.Numero);
                                AgregarDetalle(descuento.NotaCredito);
                                break;
                            }
                        }                        
                    }
                }
            }
        }



        /// <summary>
        /// Cambia la moneda de los detalle del recibo por la moneda en la que se gestiono el recibo.
        /// Este metodo se llama para que cuando el recibo se imprima, los montos de los detalles
        /// salgan de acuerdo a la moneda del recibo.
        /// </summary>
        public void CambiarMonedaADetalles()
        {
            for (int cont = 0; cont < FacturasAsociadas.Count; cont++)
            {
                Factura factura = (Factura)FacturasAsociadas[cont];
                factura.Moneda = this.Moneda;
            }

            for (int cont = 0; cont < NotasCreditoAsociadas.Count; cont++)
            {
                NotaCredito nota = (NotaCredito)NotasCreditoAsociadas[cont];
                nota.Moneda = this.Moneda;
            }
        }
		/// <summary>
		/// Carga los cheques que se giraron para la cancelacion del recibo
		/// </summary>
        public void CargaChequesAplicados()
        {
            Cobros.MontoCheques = 0;
            this.ChequesAsociados = Cheque.ObtenerChequesAplicados(this.Numero, this.Compania, this.Cliente);
        }

        /// <summary>
        /// agrega un detalle al recibo
        /// </summary>
        /// <param name="detalle_p">Recibe un objeto de tipo documento</param>
        public void AgregarDetalle(DocumentoContable doc)
        {
            DetalleRecibo elDetalle = new DetalleRecibo();
            elDetalle.Tipo = doc.Tipo;
            elDetalle.Moneda = doc.Moneda;
            elDetalle.FechaRealizacion = doc.FechaRealizacion;
            elDetalle.NumeroDocAfectado = doc.Numero;
            elDetalle.FechaUltimoProceso = DateTime.Now;
            elDetalle.MontoMovimientoLocal = doc.MontoMovimientoLocal;
            elDetalle.MontoMovimientoDolar = doc.MontoMovimientoDolar;
            elDetalle.SaldoLocal = doc.SaldoDocLocal;
            elDetalle.SaldoDolar = doc.SaldoDocDolar;

            this.Detalles.Add(elDetalle);
        }

        /// <summary>
        /// Carga los detalles de un recibo.
        /// </summary>
        public void CargaDetalles()
        {
            List<DetalleRecibo> details = new List<DetalleRecibo>();
            this.Detalles.Clear();
            try
            {
                //Inicialmente se cargan los datos de las facturas con los respectivos montos
                //de movimiento y saldo que fueron afectadas por alguna nota de credito.
                details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.NotaCredito, TipoDocumento.TodosDocumentosDebito); //TipoDocumento.Factura);
                detalles.AddRange(details);
                this.AgregarFacturaParaConsulta(details, TipoDocumento.NotaCredito);

                /*
                //Inicialmente se cargan los datos de las notas de debito con los respectivos montos
                //de movimiento y saldo que fueron afectadas por alguna nota de credito.
                details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.NotaCredito, TipoDocumento.NotaDebito);
                detalles.AddRange(details);
                this.AgregarFacturaParaConsulta(details, TipoDocumento.NotaCredito);
                */

                //Se cargan los registros del recibo de dinero sobre
                //las facturas con sus respectivos montos de movimiento y saldo.
                details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.Recibo, TipoDocumento.TodosDocumentosDebito); //TipoDocumento.Factura);
                detalles.AddRange(details);
                this.AgregarFacturaParaConsulta(details, TipoDocumento.Recibo);

                /*
                //Se cargan los registros del recibo de dinero sobre
                //las notas de debito con sus respectivos montos de movimiento y saldo.
                details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.Recibo, TipoDocumento.NotaDebito);
                detalles.AddRange(details);
                this.AgregarFacturaParaConsulta(details, TipoDocumento.Recibo);
                */

                //Cargamos los saldos de las notas de credito que se vieron afectadas
                //en el recibo.
                detalles.AddRange(DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.NotaCreditoAux, TipoDocumento.NotaCredito));

                // Cargamos los saldos de las Notas de Credito creadas en la pocket - KFC >>
                details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.NotaCreditoNueva, TipoDocumento.TodosDocumentosDebito);// TipoDocumento.Factura);                
                detalles.AddRange(details);
                this.AgregarFacturaParaConsulta(details, TipoDocumento.NotaCreditoNueva);

                //details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.NotaCreditoNueva, TipoDocumento.FacturaContado);
                //detalles.AddRange(details);
                //this.AgregarFacturaParaConsulta(details, TipoDocumento.NotaCreditoNueva);


                // Cargamos los montos de las Notas de Credito a crear por pronto pago - KFC >>
                details = DetalleRecibo.Obtener(this.Cliente, this.Numero, this.Compania, this.Moneda, TipoDocumento.NotaCreditoCrear, TipoDocumento.TodosDocumentosDebito);// TipoDocumento.Factura);                
                detalles.AddRange(details);
                this.AgregarFacturaParaConsulta(details, TipoDocumento.NotaCreditoCrear);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Funcion encargada de crear apartir de los detalles
        /// las facturas y las N\C asociadas al Recibo
        /// </summary>
        /// <param name="detalle">linea de detalle a ingresar</param>
        /// <param name="codtipoDoc">tipo de la linea de detalle</param>
        private void AgregarFacturaParaConsulta(List<DetalleRecibo> detalles, TipoDocumento codtipoDoc)
        {
            Factura nuevaFactura = null;
            foreach (DetalleRecibo detalle in detalles)
            {
                foreach (Factura factura in this.FacturasAsociadas)
                {
                    if (factura.Numero == detalle.NumeroDocAfectado)
                    {
                        nuevaFactura = factura;
                        break;
                    }
                }

                if (nuevaFactura == null)
                {
                    nuevaFactura = new Factura();
                    nuevaFactura.Numero = detalle.NumeroDocAfectado;
                    if (!(codtipoDoc.Equals(TipoDocumento.NotaCredito)) && !(codtipoDoc.Equals(TipoDocumento.NotaCreditoNueva)))//No debemos sumar los montos de las N/C
                    {
                        nuevaFactura.MontoMovimientoLocal = detalle.MontoMovimientoLocal;
                        nuevaFactura.MontoMovimientoDolar = detalle.MontoMovimientoDolar;
                        nuevaFactura.SaldoDocLocal = detalle.SaldoLocal;
                        nuevaFactura.SaldoDocDolar = detalle.SaldoDolar;
                    }
                    nuevaFactura.Moneda = detalle.Moneda;
                    this.FacturasAsociadas.Add(nuevaFactura);
                }
                else
                {
                    if (!(codtipoDoc.Equals(TipoDocumento.NotaCredito)) && !(codtipoDoc.Equals(TipoDocumento.NotaCreditoNueva)))//No debemos sumar los montos de las N/C
                    {
                        nuevaFactura.MontoMovimientoLocal += detalle.MontoMovimientoLocal;
                        nuevaFactura.MontoMovimientoDolar += detalle.MontoMovimientoDolar;

                        if (nuevaFactura.SaldoDocLocal > detalle.SaldoLocal)
                        {
                            nuevaFactura.SaldoDocLocal = detalle.SaldoLocal;
                            nuevaFactura.SaldoDocDolar = detalle.SaldoDolar;
                        }
                    }
                }

                //if (codtipoDoc.Equals(TipoDocumento.NotaCredito))//Nota de credito aplicada a la factura
                if ((codtipoDoc.Equals(TipoDocumento.NotaCredito)) || (codtipoDoc.Equals(TipoDocumento.NotaCreditoNueva)) || (codtipoDoc.Equals(TipoDocumento.NotaCreditoCrear)))
                {
                    NotaCredito nota = new NotaCredito();
                    nota.Numero = detalle.Numero;
                    nota.MontoDocLocal= detalle.MontoMovimientoLocal;
                    nota.MontoDocDolar = detalle.MontoMovimientoDolar;
                    nota.SaldoDocLocal = detalle.SaldoLocal;
                    nota.SaldoDocDolar = detalle.SaldoDolar;
                    nuevaFactura.NotasCreditoAplicadas.Add(nota);
                }
                nuevaFactura = null;
            }
        }

        #endregion
        
		#region Insert de el registro del recibo de dinero por factura

		/// <summary>
		/// Guarda en la base de datos las facturas con el registro 
		/// del recibo al que pertenece y el monto del mismo que se le aplico.
		/// </summary>
		/// <param name="facturas"></param>
        private void GuardaReciboFactura()
        {
            if (this.NumeroPedido != string.Empty)
            {
                pedido.GuardarRecibo(this);                
            }
            else
            {
                foreach (Factura factura in this.FacturasAsociadas)
                {
                    factura.GuardarRecibo(this.Numero);
                }
            }

            //foreach (Factura factura in this.FacturasAsociadas)
            //{
            //    factura.GuardarRecibo(this.Numero);
            //}
        }

        #endregion

        #endregion 

        #region Acceso Datos

        /// <summary>
        /// Metodo que guarda el encabezado del recibo en la base de datos
        /// </summary>
        private void DBGuardar()
        {
            //Caso 25452 LDS 30/10/2007 Se agrega el campo impreso a la sentencia. Se indica en N ya que no se imprimen los recibos que generan notas de crédito.
            string sentencia =
                " INSERT INTO " + Table.ERPADMIN_alCXC_DOC_APL +
                "       ( COD_CIA, COD_TIP_DC, COD_ZON, NUM_REC, COD_CLT, FEC_PRO, MON_DOC_LOC, IND_MON, IND_ANL, MON_EFE_LOCAL, MON_EFE_DOLAR, MON_CHE_DOLAR, MON_CHE_LOCAL, MON_DOC_DOL, IMPRESO, HOR_INI, HOR_FIN) " +
                " VALUES(@COD_CIA,@COD_TIP_DC,@COD_ZON,@NUM_REC,@COD_CLT,@FEC_PRO,@MON_DOC_LOC,@IND_MON,@IND_ANL,@MON_EFE_LOCAL,@MON_EFE_DOLAR,@MON_CHE_DOLAR,@MON_CHE_LOCAL,@MON_DOC_DOL,@IMPRESO,@HOR_INI,@HOR_FIN) ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, Compania),
                GestorDatos.SQLiteParameter("@COD_TIP_DC",SqlDbType.NVarChar, ((int)this.Tipo).ToString()),
                GestorDatos.SQLiteParameter("@COD_ZON",SqlDbType.NVarChar, Zona),
                GestorDatos.SQLiteParameter("@NUM_REC",SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@COD_CLT",SqlDbType.NVarChar, Cliente),
                GestorDatos.SQLiteParameter("@FEC_PRO",SqlDbType.DateTime, FechaRealizacion),
                //LDS 03/07/2007 realiza el redondea a 2 decimales por problema de sincronización con montos muy pequeños.
                GestorDatos.SQLiteParameter("@MON_DOC_LOC",SqlDbType.Decimal,Decimal.Round(this.MontoDocLocal,2)),
                GestorDatos.SQLiteParameter("@IND_MON",SqlDbType.NVarChar, ((char)this.Moneda).ToString()),
                GestorDatos.SQLiteParameter("@IND_ANL",SqlDbType.NVarChar, ((char)this.Estado).ToString()),
                GestorDatos.SQLiteParameter("@MON_EFE_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoEfectivoLocal,2)),
                GestorDatos.SQLiteParameter("@MON_EFE_DOLAR",SqlDbType.Decimal,Decimal.Round(MontoEfectivoDolar,2)),
                GestorDatos.SQLiteParameter("@MON_CHE_DOLAR",SqlDbType.Decimal,Decimal.Round(MontoChequesDolar,2)),
                GestorDatos.SQLiteParameter("@MON_CHE_LOCAL",SqlDbType.Decimal,Decimal.Round(MontoChequesLocal,2)),
                GestorDatos.SQLiteParameter("@MON_DOC_DOL",SqlDbType.Decimal,Decimal.Round(MontoDocDolar,2)),
                GestorDatos.SQLiteParameter("@HOR_INI",SqlDbType.DateTime, this.fechaInicio),
                GestorDatos.SQLiteParameter("@HOR_FIN",SqlDbType.DateTime, this.fechaFinalizacion),
                //Caso 25452 LDS 30/10/2007
                GestorDatos.SQLiteParameter("@IMPRESO",SqlDbType.NVarChar,(this.Impreso? "S": "N"))});

                GestorDatos.EjecutarComando(sentencia, parametros);
        }
        /// <summary>
        /// Anula el encabezado del recibo
        /// </summary>
       /* private void AnularEncabezado()
        {
            try
            {
                string sentencia =
                    " UPDATE " + Table.ERPADMIN_alCXC_DOC_APL +
                    " SET   IND_ANL = @ESTADO " +
                    " WHERE NUM_REC = @CONSECUTIVO " +
                    " AND   COD_CIA = @COMPANIA " +
                    " AND   COD_CLT = @CLIENTE ";

                SQLiteParameterList parametros = {            
                    new SQLiteParameter("@CONSECUTIVO", this.Numero),
                    new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Anulado).ToString()),
                    new SQLiteParameter("@CLIENTE", this.Cliente),
                    new SQLiteParameter("@COMPANIA", this.Compania)};

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando encabezado. " + ex.Message);
            }
        
        }*/
        /// <summary>
        /// Anula los detalles del recibo
        /// </summary>
        /*private void AnularDetalle()
        {
            try
            {
                //LJR Caso 37677 -> Error al migrar
                string sentencia =
                    " UPDATE " + Table.ERPADMIN_alCXC_MOV_DIR +
                    " SET   IND_ANL = @ESTADO " +
                    " WHERE NUM_REC = @CONSECUTIVO " +
                    " AND   COD_CIA = @COMPANIA " +
                    " AND   COD_CLT = @CLIENTE ";

                SQLiteParameterList parametros = {            
                    new SQLiteParameter("@CONSECUTIVO", this.Numero),
                    new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Anulado).ToString()),
                    new SQLiteParameter("@CLIENTE", this.Cliente),
                    new SQLiteParameter("@COMPANIA", this.Compania)};

                GestorDatos.EjecutarComando(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando detalles. " + ex.Message);
            }        
        }*/

        //Caso 25452 LDS 30/10/2007
        /// <summary>
        /// Actualiza el estado del campo Impreso indicando que se ha realizado la impresión del documento siempre y 
        /// cuando el valor de la variable Impreso sea verdadero, de lo contrario indica que no se ha realizado la 
        /// impresión.
        /// </summary>
        public void ActualizarImpresion()
        {
            string sentenciaSQL =
                " UPDATE " + Table.ERPADMIN_alCXC_DOC_APL +
                " SET IMPRESO = ? " +
                " WHERE NUM_REC = ?";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@NUM_REC", SqlDbType.NVarChar, Numero),
                GestorDatos.SQLiteParameter("@IMPRESO", SqlDbType.NVarChar, (Impreso? "S" : "N"))});
            try
            {
                GestorDatos.EjecutarComando(sentenciaSQL, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando el indicador de impresión. " + ex.Message);
            }

            parametros = null;
        }

        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(*) FROM " + Table.ERPADMIN_alCXC_DOC_APL + " WHERE DOC_PRO IS NULL";
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }

        #region Todo lo utilizado para la consulta de cobros

        /// <summary>
        /// Carga los recibos de la compania que pueden ser 
        /// usados para un deposito.
        /// </summary>
        /// <param name="compania">compania a asociar</param>
        /// <returns>un arreglo de objetos Recibo</returns>
        public static List<Recibo> CargaRecibosDeposito(string compania, TipoMoneda moneda)
        {
            string condicion =
                " AND UPPER(CXC.COD_CIA) = @COMPANIA" +
                " AND CXC.IND_ANL in (@ESTADO_ACTIVO,@ESTADO_FACT_CONT) " +
                " AND CXC.IND_MON = @MONEDA " +
                " AND CXC.COD_TIP_DC != '7' " +  // Facturas de contado y recibos en FR - KFC. Se agrega linea para que no se muestre la nota de credito creada por la devolucion
                " AND CXC.ASOC_DEP IS NULL ";

            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                //new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Activo).ToString()), 
                // Facturas de contado y recibos en FR - KFC
                // Se cambia el estado por @ESTADO_ACTIVO y @ESTADO_FACT_CONT para incluir los datos de facturas de contado
                new SQLiteParameter("@ESTADO_ACTIVO", ((char)EstadoDocumento.Activo).ToString()),
                new SQLiteParameter("@ESTADO_FACT_CONT", ((char)EstadoDocumento.Contado).ToString()),
                new SQLiteParameter("@MONEDA", ((char)moneda).ToString())});
            
            return CargarRecibos(condicion,parametros,false);
        }

        /// <summary>
        /// Carga todos los recibos generados el dia de hoy y que estén activos.
        /// </summary>
        /// <returns>Lista de objetos Recibo</returns>
        public static List<Recibo> CargaRecibosGenerados()
        {
            //JDDR - 7-2-2007 - Se debe cargar los tipo Recibo salamente porque si no 
            //jala tambien las notas de credito y el reporte de cierre sale mal.

            string condicion =
                " AND CXC.IND_ANL in (@ESTADO_ACTIVO,@ESTADO_FACT_CONT) " +
                " AND CXC.COD_TIP_DC = @TIPO " +
                " AND julianday(date(CXC.FEC_PRO)) = julianday(date('now','localtime')) ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                new SQLiteParameter("@FECHA", DateTime.Now.ToString("yyyyMMdd")), 
                new SQLiteParameter("@TIPO", (int)TipoDocumento.Recibo),
                // new SQLiteParameter("@ESTADO", ((char)EstadoDocumento.Activo).ToString()),
                // Facturas de contado y recibos en FR - KFC
                // Se cambia el estado por @ESTADO_ACTIVO y @ESTADO_FACT_CONT para incluir los datos de facturas de contado
                new SQLiteParameter("@ESTADO_ACTIVO", ((char)EstadoDocumento.Activo).ToString()),
                new SQLiteParameter("@ESTADO_FACT_CONT", ((char)EstadoDocumento.Contado).ToString())});

            return CargarRecibos(condicion,parametros,false);
        }

        /// <summary>
        /// Carga los recibos del cliente que ya sean anulados o sin anular 
        /// dependiendo del indice de anulacion, y que no han sido procesados.
        /// </summary>
        /// <param name="indiceAnulacion">indica el indice de anulacion "ANULADO" o "No ANULADO"</param>
        /// <param name="cliente">cliente al que se le cargan los recibos</param>
        /// <returns>un arreglo de objetos Recibo</returns>
        public static List<Recibo> CargaRecibosCliente(string cliente, string zona, EstadoDocumento indiceAnulacion)
        {
            string condicion =
                " AND CXC.COD_CLT = @CLIENTE" +
                " AND CXC.IND_ANL in (@ESTADO_ACTIVO,@ESTADO_FACT_CONT)" +
                " AND CXC.ASOC_DEP IS NULL " +
                " AND CXC.DOC_PRO  IS NULL " +
                // ARS se comenta para que aparezcan en la consulta los cobros que son adelantos  " AND CXC.COD_TIP_DC != '7' " +  
                " ORDER BY CXC.COD_CIA ";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                new SQLiteParameter("@CLIENTE", cliente),
                //new SQLiteParameter("@ESTADO", ((char)indiceAnulacion).ToString()), 
                // Facturas de contado y recibos en FR - KFC
                // Se cambia el estado por @ESTADO_ACTIVO y @ESTADO_FACT_CONT para incluir los datos de facturas de contado
                new SQLiteParameter("@ESTADO_ACTIVO", ((char)indiceAnulacion).ToString()),
                new SQLiteParameter("@ESTADO_FACT_CONT", ((char)EstadoDocumento.Contado).ToString()),
                new SQLiteParameter("@ZONA", zona)});
            
            return CargarRecibos(condicion, parametros,true);
        }

        /// <summary>
        /// Carga los recibos de la base de datos. Se cargan de alCXC_DOC_APL as cxc
        /// y de CLIENTE as enc
        /// </summary>
        /// <param name="condicionDeCargaExtra">Condicion adicional para la carga de recibos</param>
        /// <param name="parametros">lista de parametros de la consulta</param>
        /// <returns>Un arreglo de objetos Recibo</returns>
        private static List<Recibo> CargarRecibos(string condicionDeCargaExtra, SQLiteParameterList parametros, bool validarZona)
        {
            //Caso 25452 LDS 30/10/2007
            //Se obtiene el valor del campo IMPRESO.
            string sentencia =
                " SELECT CXC.NUM_REC,CXC.FEC_PRO,CXC.IND_MON,CXC.COD_CIA,CXC.COD_CLT,ENC.NOM_CLT," +
                " CXC.MON_DOC_LOC,CXC.MON_DOC_DOL,CXC.MON_EFE_LOCAL,CXC.MON_EFE_DOLAR," +
                " CXC.MON_CHE_LOCAL,CXC.MON_CHE_DOLAR,CXC.COD_ZON,CXC.IMPRESO,CXC.IND_ANL " +
                " FROM " + Table.ERPADMIN_alCXC_DOC_APL + " CXC, " + Table.ERPADMIN_CLIENTE + " ENC " +
                " WHERE CXC.COD_CLT = ENC.COD_CLT " + (validarZona ? " AND CXC.COD_ZON = @ZONA " : string.Empty) + " AND CXC.COD_ZON = ENC.COD_ZON " +
                condicionDeCargaExtra;

            List<Recibo> recibos = new List<Recibo>();
            SQLiteDataReader reader = null;

            try
            {

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                    while (reader.Read())
                    {
                        Recibo recibo = new Recibo(reader.GetString(4));
                        recibo.Numero = reader.GetString(0);
                        recibo.FechaRealizacion = reader.GetDateTime(1);
                        recibo.Moneda = (TipoMoneda)Convert.ToChar(reader.GetString(2));
                        recibo.Compania = reader.GetString(3);
                        recibo.Cliente = reader.GetString(4);
                        recibo.clienteDatos = new Cliente();
                        recibo.clienteDatos.Codigo = recibo.Cliente;
                        recibo.clienteDatos.Nombre = reader.GetString(5);
                        recibo.MontoDocLocal = reader.GetDecimal(6);
                        recibo.MontoDocDolar = reader.GetDecimal(7);
                        recibo.MontoEfectivoLocal = reader.GetDecimal(8);
                        recibo.MontoEfectivoDolar = reader.GetDecimal(9);
                        recibo.MontoChequesLocal = reader.GetDecimal(10);
                        recibo.MontoChequesDolar = reader.GetDecimal(11);
                        recibo.Zona = reader.GetString(12);
                        //Caso 25452 LDS 30/10/2007
                        recibo.Impreso = reader.GetString(13).Equals("S");
                        //Modificaciones en funcionalidad de recibos de contado - KFC 
                        switch (reader.GetString(14))
                        {
                            case "A":
                                recibo.Estado = EstadoDocumento.Activo;
                                break;
                            case "C":
                                recibo.Estado = EstadoDocumento.Contado;
                                break;
                            case "N":
                                recibo.Estado = EstadoDocumento.Anulado;
                                break;

                        }
                        recibos.Add(recibo);
                    }                
            }
            catch(Exception e)
            {
                throw new Exception("Error cargando los recibos asociados a la compañía. "+e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return recibos;
        }

        //ABC 35772
        /// <summary>
        /// Validar el numero de recibo indicado por el usuario
        /// </summary>
        /// <param name="numero">numero indicado por el usuario</param>
        /// <returns>validez del numero</returns>
        public static bool ExisteNumeroRecibo(string numero)
        {
            string sentencia =
                " SELECT NUM_REC FROM " + Table.ERPADMIN_alCXC_DOC_APL +
                " WHERE NUM_REC = @CONSECUTIVO";
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { 
                new SQLiteParameter("@CONSECUTIVO", numero)});
            try
            {
                return GestorDatos.RetornaDatos(sentencia,parametros);

            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando el indicador de impresión. " + ex.Message);
            }
        }


        #region Impresion Recibo desde la toma del mismo

        //Caso 25452 LDS 30/10/2007
        /// <summary>
        /// Imprime el detalle del recibo que ha sido gestionado.
        /// </summary>
        /// <param name="cantidadCopias">Indica la cantidad de copias que se requiere imprimir del recibo.</param>
        public void ImprimeDetalleRecibo( int cantidadCopias)
        {
            List<Recibo> recibos = new List<Recibo>();
            recibos.Add(this);

            ReporteRecibos reporte = new ReporteRecibos(recibos, this.clienteDatos);
            reporte.ImprimeDetalleRecibos(cantidadCopias);
        }
        
        #endregion

        #endregion

        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "RECIBO";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                case "FACTURAS_ASOCIADAS": return new ArrayList(facturasAsociadas);
                case "TOTAL_NOTAS_APLICADAS": return TotalNotasCredito;
                case "TOTAL_COBRADO": return TotalFacturas;
                case "TOTAL_CHEQUES": return TotalCheques;
                case "TOTAL_EFECTIVO": return TotalEfectivo;                
                default:

                    object field = this.ClienteDatos.GetField(name);
                    if (!field.Equals(string.Empty))
                        return field;
                    else
                        return base.GetField(name);
            }
        }


        #region  Facturas de contado y recibos en FR - KFC


        /*
        /// <summary>
        /// Metodo para los recibos generados por las facturas de contado, se marcan como Sincronizados para que no suban a CC
        /// </summary>
        public void MarcarComoSincronizado(string numRecibo)
        {
            string sentenciaSQL =
                " UPDATE " + Table.ERPADMIN_alCXC_DOC_APL +
                " SET DOC_PRO = @DOC_PRO " +
                " WHERE NUM_REC = @NUM_REC";

            SQLiteParameterList parametros = {
                GestorDatos.SQLiteParameter("@NUM_REC", SqlDbType.NVarChar, numRecibo),
                GestorDatos.SQLiteParameter("@DOC_PRO", SqlDbType.NVarChar, "S" )};
            try
            {
                GestorDatos.EjecutarComando(sentenciaSQL, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error actualizando el estado del recibo. " + ex.Message);
            }

            parametros = null;
        }
        */


        /// <summary>
        /// Actualiza los valores en la tabla JORNADA_RUTAS 
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="monto"></param>
        private void ActualizarJornada(string ruta, TipoMoneda moneda,decimal numCobros, decimal montoTot, decimal montoChq, decimal montoEfc, decimal montoNC, BaseViewModel viewModel)
        {
            string colCantidad = "";
            string colMontoTot, colMontoChq, colMontoEfc, colMontoNC = "";

            if (moneda == TipoMoneda.LOCAL)
            {
                colCantidad = JornadaRuta.COBROS_LOCAL;
                colMontoTot = JornadaRuta.MONTO_COBROS_LOCAL;
                colMontoChq = JornadaRuta.MONTO_COBROS_CHEQUE_LOCAL;
                colMontoEfc = JornadaRuta.MONTO_COBROS_EFECTIVO_LOCAL;
                colMontoNC  =  JornadaRuta.MONTO_COBROS_NOTA_CREDITO_LOCAL;
            }
            else
            {
                colCantidad = JornadaRuta.COBROS_DOLAR;
                colMontoTot = JornadaRuta.MONTO_COBROS_DOLAR;
                colMontoChq = JornadaRuta.MONTO_COBROS_CHEQUE_DOLAR;
                colMontoEfc = JornadaRuta.MONTO_COBROS_EFECTIVO_DOLAR;
                colMontoNC  = JornadaRuta.MONTO_COBROS_NOTA_CREDITO_DOLAR;
            }

            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, numCobros);
                JornadaRuta.ActualizarRegistro(ruta, colMontoTot, -montoTot);
                JornadaRuta.ActualizarRegistro(ruta, colMontoChq, -montoChq);
                JornadaRuta.ActualizarRegistro(ruta, colMontoEfc, -montoEfc);
                JornadaRuta.ActualizarRegistro(ruta, colMontoNC, -montoNC);

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
