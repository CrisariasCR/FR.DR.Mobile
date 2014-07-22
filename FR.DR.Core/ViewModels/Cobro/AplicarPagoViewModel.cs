using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;

using Cirrious.MvvmCross.Commands;

using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.UI;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class AplicarPagoViewModel : DialogViewModel<bool>
    {
#pragma warning disable 649

        #region Propiedades

        public bool bcargarInfoCobro;

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set
            {
                if (value != nombreCliente)
                {
                    nombreCliente = value;
                    RaisePropertyChanged("NombreCliente");
                }
            }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private decimal saldoTotal;
        public decimal SaldoTotal
        {
            get { return saldoTotal; }
            set { saldoTotal = value; RaisePropertyChanged("SaldoTotal"); }
        }


        private decimal montoNotas;
        public decimal MontoNotas
        {
            get { return montoNotas; }
            set { montoNotas = value; RaisePropertyChanged("MontoNotas"); }
        }

        private decimal cheques;
        public decimal Cheques
        {
            get { return cheques; }
            set { cheques = value; RaisePropertyChanged("Cheques"); }
        }

        private decimal efectivo;
        public decimal Efectivo
        {
            get { return efectivo; }
            set { efectivo = value; RaisePropertyChanged("Efectivo"); }
        }

        private decimal descuento;
        public decimal Descuento
        {
            get { return descuento; }
            set { descuento = value; RaisePropertyChanged("Descuento"); }
        }

        private decimal totalPagar;
        public decimal TotalPagar
        {
            get { return totalPagar; }
            set { totalPagar = value; RaisePropertyChanged("TotalPagar"); }
        }

        private decimal saldoPendiente;
        public decimal SaldoPendiente
        {
            get { return saldoPendiente; }
            set { saldoPendiente = value; RaisePropertyChanged("SaldoPendiente"); }
        }
        
        TipoMoneda moneda;
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set
            {
                if (value != moneda)
                {
                    moneda = value;
                    CargaInfoCobro();
                    RaisePropertyChanged("Moneda");
                }
            }
        }

        public bool DescuentoEnabled { get; set; }

        public bool FacturasEnabled { get; set; }
        public bool FacturasVisible { get; set; }

        public bool NotasCreditoEnabled { get; set; }

        /// <summary>
        /// Caso 25452 LDS 30/10/2007
        /// Contiene el recibo que se está gestionando.
        /// </summary>
        private Recibo recibo;
        
        private Pedido pedido;
        /// <summary>
        /// Lista de desucentos por pronto pago.
        /// </summary>
        private List<FacturaDescuento> descuentos = new List<FacturaDescuento>();

        //Facturas de contado y recibos en FR - KFC
        bool esFacturaContado = false;

        /// <summary>
        /// Caso 25452 LDS 30/10/2007
        /// Contiene el recibo que se está gestionando.
        /// </summary>
        private Recibo Recibo;

        private bool rbLocal;
        public bool RbLocal
        {
            get { return rbLocal; }
            set { rbLocal = value; }
        }
        private bool rbDolar;
        public bool RbDolar
        {
            get { return rbDolar; }
            set { rbDolar = value;}
        }

        #endregion Propiedades
        /// <summary>
        /// cuando se invoca desde CreacionRecibo
        /// </summary>
        /// <param name="messageId"></param>
        public AplicarPagoViewModel(string messageId)
            : base(messageId)
        {

                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                {
                    rbLocal = true;
                }
                else
                {
                    rbDolar = true;

                }
            

            if (this.esFacturaContado)
                CargaInicialContado();
            else
                CargaInicial();
        }


        #region CargaInicial

        /// <summary>
        /// Realiza la carga inicial
        /// </summary>
        private void CargaInicial()
        {
            //Caso 25452 LDS 30/10/2007
            //this.pnlImpresion.Location = new System.Drawing.Point(0, this.pnlImpresion.Location.Y);
            //this.pnlImpresion.Hide();
            //this.pnlImpresion.SendToBack();

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                " Cliente: " + GlobalUI.ClienteActual.Nombre;


            this.SacarMontosFacturas();
            DescuentoEnabled = Cobros.AplicarDescuentosProntoPago;
            if (Cobros.TipoCobro == TipoCobro.MontoTotal)
                CalcularDescuentosMontoTotal();
            else
                CalcularDescuentos();

            if (Cobros.TipoCobro != TipoCobro.SeleccionFacturas)
            {
                this.FacturasEnabled = false;
            }

            this.Moneda = Cobros.TipoMoneda;

            //Reubicamos la etiqueta que se muestra cuando las notas de credito no seran aplicadas
            //this.noAplicaNClbl.Location = btnExWarning.Location;
            //this.noAplicaNClbl.Visible = false;

            //Si el monto de las notas de credito es mayor a cero significa que hay al menos 
            //una nota de credito, por lo tanto habilitamos el boton para ver las notas
            if (Cobros.MontoNotasCreditoLocal > 0)
            {
                NotasCreditoEnabled = true;

                // Facturas de contado y recibos en FR - KFC
                //if (!Cobros.AplicarLasNotasCredito)
                ///if (Cobros.AplicarLasNotasCredito == FRmConfig.NoAplicaNotasCredito)
                ///    btnExWarning.Visible = true;
                ///else
                ///    btnExWarning.Visible = false;
            }
            else
            {
                NotasCreditoEnabled = false;
                //btnExWarning.Visible = false;
            }
        }

        /// <summary>
        /// Calcula los montos de facturas utilizados durante la gestion de cobro.
        /// </summary>
        private void SacarMontosFacturas()
        {
            Cobros.MontoFacturasDolar = 0;
            Cobros.MontoFacturasLocal = 0;

            //Facturas de contado y recibos en FR - KFC
            if (esFacturaContado)
            {
                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                {
                    Cobros.MontoFacturasLocal = pedido.MontoNeto;
                }
                else
                {
                    Cobros.MontoFacturasDolar = pedido.MontoNeto;
                }
            }
            else
            {
                foreach (Factura fac in GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).FacturasPendientes)
                {
                    //se suma el saldo a el monto de facturas por pagar
                    Cobros.MontoFacturasDolar += fac.SaldoDocDolar;
                    Cobros.MontoFacturasLocal += fac.SaldoDocLocal;
                }
            }
        }

        /// <summary>
        /// Calcula el monto total de los cuentos y llena la lista de descuentos.
        /// </summary>
        private void CalcularDescuentos()
        {
            if (Cobros.AplicarDescuentosProntoPago)
            {
                decimal montoDescuentos = decimal.Zero;
                descuentos.Clear();
                List<Factura> facturas = new List<Factura>();
                if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                {
                    facturas = Cobros.facturasSeleccionadas;
                }
                else if (Cobros.TipoCobro == TipoCobro.MontoTotal)
                {
                    facturas = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).FacturasPendientes;
                }

                foreach (Factura factura in facturas)
                {
                    FacturaDescuento descuento = new FacturaDescuento(factura, Cobros.Recibo);
                    if (descuento.AplicaDescuento)
                    {
                        descuentos.Add(descuento);
                        montoDescuentos += descuento.Monto;
                    }
                }

                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                {
                    Cobros.MontoDescuentoProntoPagoLocal = montoDescuentos;
                    Cobros.MontoDescuentoProntoPagoDolar = montoDescuentos / Cobros.TipoCambio;
                }
                else
                {
                    Cobros.MontoDescuentoProntoPagoDolar = montoDescuentos;
                    Cobros.MontoDescuentoProntoPagoLocal = montoDescuentos * Cobros.TipoCambio;
                }
                Cobros.Recibo.Descuentos = descuentos;
            }
        }

        /// <summary>
        /// Calcula el monto total de los cuentos y llena la lista de descuentos.
        /// </summary>
        private void CalcularDescuentosMontoTotal()
        {
            if (Cobros.AplicarDescuentosProntoPago)
            {
                decimal montoDescuentos = decimal.Zero;
                descuentos.Clear();
                List<Factura> facturas = new List<Factura>();
                if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                    facturas = Cobros.facturasSeleccionadas;
                if (Cobros.TipoCobro == TipoCobro.MontoTotal)
                    facturas = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).FacturasPendientes;
                //Cobros.montoTotalPagarTemporal = 0;
                //Cobros.montoTotalPagarTemporal = Cobros.montoTotalPagarObtener;
                foreach (Factura factura in facturas)
                {

                    Cobros.pagaFacturasMontoTotales(factura);
                }
                foreach (Factura factura in facturas)
                {
                    FacturaDescuento descuento = new FacturaDescuento(factura, Cobros.Recibo, true);
                    if (descuento.AplicaDescuento)
                    {
                        if (descuento.Monto > 0)
                        {
                            bool continuar = (FRdConfig.ProntoPagoTotales && factura.SaldoDocLocal == 0 && factura.SaldoDocDolar == 00) || (!FRdConfig.ProntoPagoTotales);
                            if (continuar)
                            {
                                bool existe = descuentos.Exists(x => x.Factura == descuento.Factura);
                                if (existe)
                                {
                                    descuentos.Remove(descuento);
                                }
                                descuentos.Add(descuento);
                                montoDescuentos += descuento.Monto;

                            }
                        }
                    }
                }
                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                {
                    Cobros.MontoDescuentoProntoPagoLocal = montoDescuentos;
                    Cobros.MontoDescuentoProntoPagoDolar = montoDescuentos / Cobros.TipoCambio;
                }
                else
                {
                    Cobros.MontoDescuentoProntoPagoDolar = montoDescuentos;
                    Cobros.MontoDescuentoProntoPagoLocal = montoDescuentos * Cobros.TipoCambio;
                }
                Cobros.Recibo.Descuentos = descuentos;
            }

        }


        /// <summary>
        /// Realiza la carga inicial para facturas de contado
        /// </summary>
        private void CargaInicialContado()
        {
            //this.pnlImpresion.Location = new System.Drawing.Point(0, this.pnlImpresion.Location.Y);
            //this.pnlImpresion.Hide();
            //this.pnlImpresion.SendToBack();

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                " Cliente: " + GlobalUI.ClienteActual.Nombre;

            this.SacarMontosFacturas();

            DescuentoEnabled = true;
            FacturasVisible = false;

            this.Moneda = Cobros.TipoMoneda;
            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
            {
                // Descuentos
                Cobros.MontoDescuentoProntoPagoLocal = pedido.MontoTotalDescuento;
                Cobros.MontoDescuentoProntoPagoDolar = pedido.MontoTotalDescuento / Cobros.TipoCambio;
            }
            else
            {
                // Descuentos
                Cobros.MontoDescuentoProntoPagoLocal = pedido.MontoTotalDescuento;
                Cobros.MontoDescuentoProntoPagoDolar = pedido.MontoTotalDescuento * Cobros.TipoCambio;
            }

            //Reubicamos la etiqueta que se muestra cuando las notas de credito no seran aplicadas
            //this.noAplicaNClbl.Location = btnExWarning.Location;
            //this.noAplicaNClbl.Visible = false;

            //Si el monto de las notas de credito es mayor a cero significa que hay al menos 
            //una nota de credito, por lo tanto habilitamos el boton para ver las notas
            if (Cobros.MontoNotasCreditoLocal > 0)
            {
                NotasCreditoEnabled = true;

                //if (Cobros.AplicarLasNotasCredito == FRmConfig.NoAplicaNotasCredito)
                //    btnExWarning.Visible = true;
                //else
                //    btnExWarning.Visible = false;
            }
            else
            {
                NotasCreditoEnabled = false;
                //btnExWarning.Visible = false;
            }
        }

        #endregion CargaInicial

        #region Metodos Logica de Negocio

        /// <summary>
        /// Genera un recibo en efectivo para la factura de contado
        /// </summary>
        private void GenerarReciboEfectivo()
        {
            bool aplicarPago = false;
            decimal saldo = 0;

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
            {
                saldo = pedido.MontoNeto;
                saldo -= Cobros.AplicarDescuentosProntoPago ? Cobros.MontoDescuentoProntoPagoLocal : 0;
                saldo -= Cobros.MontoEfectivo;
            }

            else // Dolar
            {
                saldo = pedido.MontoNeto;
                saldo -= Cobros.AplicarDescuentosProntoPago ? Cobros.MontoDescuentoProntoPagoDolar : 0;
                saldo = (saldo * Cobros.TipoCambio) - (Cobros.MontoEfectivo * Cobros.TipoCambio);
            }

            saldo = (decimal)Math.Round(saldo, 2);

            if (saldo < 0) saldo = 0;

            if ((Cobros.MontoPendiente != 0 && saldo == 0) ||
                Cobros.MontoPendiente == 0)
            {
                aplicarPago = true;
            }

            if (aplicarPago)
            {
                try
                {
                    //Invoca el metodo que aplica el pago                                     
                    this.Recibo = Cobros.AplicarPagoContado(this.pedido);

                    //Llamado a ActualizarJornada
                    ActualizarJornada(GlobalUI.RutaActual.Codigo,
                                       TotalPagar,
                                       Cheques,
                                       Efectivo,
                                       MontoNotas);

                    //Limpiamos valores globales
                    Cobros.FinalizarCobro();
                    this.Recibo = null;

                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error al aplicar cobro." + ex.Message);
                }
            }
        }

        // MejorasGrupoPelon600R6 - KFC
        /// <summary>
        /// Actualiza los valores en la tabla JORNADA_RUTAS 
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="monto"></param>
        private void ActualizarJornada(string ruta, decimal montoTot, decimal montoChq, decimal montoEfc, decimal montoNC)
        {
            string colCantidad = "";
            string colMontoTot, colMontoChq, colMontoEfc, colMontoNC = "";

            if (Moneda == TipoMoneda.LOCAL)
            {
                colCantidad = JornadaRuta.COBROS_LOCAL;
                colMontoTot = JornadaRuta.MONTO_COBROS_LOCAL;
                colMontoChq = JornadaRuta.MONTO_COBROS_CHEQUE_LOCAL;
                colMontoEfc = JornadaRuta.MONTO_COBROS_EFECTIVO_LOCAL;
                colMontoNC = JornadaRuta.MONTO_COBROS_NOTA_CREDITO_LOCAL;
            }
            else
            {
                colCantidad = JornadaRuta.COBROS_DOLAR;
                colMontoTot = JornadaRuta.MONTO_COBROS_DOLAR;
                colMontoChq = JornadaRuta.MONTO_COBROS_CHEQUE_DOLAR;
                colMontoEfc = JornadaRuta.MONTO_COBROS_EFECTIVO_DOLAR;
                colMontoNC = JornadaRuta.MONTO_COBROS_NOTA_CREDITO_DOLAR;
            }

            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, 1);
                JornadaRuta.ActualizarRegistro(ruta, colMontoTot, montoTot);
                JornadaRuta.ActualizarRegistro(ruta, colMontoChq, montoChq);
                JornadaRuta.ActualizarRegistro(ruta, colMontoEfc, montoEfc);
                JornadaRuta.ActualizarRegistro(ruta, colMontoNC, montoNC);

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                this.mostrarAlerta("Error al actualizar datos. " + ex.Message);
            }
        }

        #endregion

        #region Comandos

        public ICommand ComandoNotasCredito
        {
            get { return new MvxRelayCommand(MostrarPantallaNotas); }
        }

        public ICommand ComandoCheques
        {
            get { return new MvxRelayCommand(MostrarPantallaCheques); }
        }

        public ICommand ComandoEfectivo
        {
            get { return new MvxRelayCommand(MostrarPantallaEfectivo); }
        }

        public ICommand ComandoDescuento
        {
            get { return new MvxRelayCommand(MostrarDescuentos); }
        }

        public ICommand ComandoTipoCambio
        {
            get { return new MvxRelayCommand(MostrarPantallaCambio); }
        }

        public ICommand ComandoFacturas
        {
            get { return new MvxRelayCommand(MostrarPantallaFacturas); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }
        #endregion Comandos

        #region Acciones

        /// <summary>
        /// funcion que despliega la pantalla de notas de credito
        /// </summary>
        /// <returns></returns>
        private void MostrarPantallaNotas()
        {
            //Guarda lo que hay en cheques antes de ingresar
            Cobros.montoEntradaNC = Math.Round(MontoNotas,2);
            bcargarInfoCobro = true;
            // Facturas de contado y recibos en FR - KFC
            // En caso de que la parametrizacion lo permita se llamara a la pantalla de seleccion de Notas de Crédito
            if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
            {
                //this.DoClose();
                this.RequestDialogNavigate<SeleccionNotasCreditoViewModel, bool>(null, result =>
                    {
                        //CargaInfoCobro();
                    });
            }
            else
            {
                //this.DoClose();
                //despliega la pantalla de notas de credito
                this.RequestNavigate<ConsultaNotasCreditoViewModel>();
            }
        }


        /// <summary>
        /// Abre la pantalla de descuentos con la lista de descuentos por aplicar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MostrarDescuentos()
        {
            Gestor.DescuentosFacturas = this.descuentos;
            this.RequestNavigate<DescuentoProntoPagoViewModel>();
        }

        /// <summary>
		/// funcion que despliega la pantalla de cheques
		/// </summary>
		private void MostrarPantallaCheques()
		{
            //Guarda lo que hay en cheques antes de ingresar
            Cobros.montoEntradaCheque = Math.Round(Cheques, 2);
            Dictionary<string, object> p = new Dictionary<string, object>();
            p.Add("monedaRecibo", Cobros.TipoMoneda.ToString());
            bcargarInfoCobro = true;
            //this.DoClose();
            //muestra la pantalla de ingreso de cheques
			this.RequestDialogNavigate<ChequesViewModel, bool>(p, result =>
                {
                    //CargaInfoCobro();
                });
		}

        /// <summary>
		/// funcion que despliega la pantalla de toma de efectivo
		/// </summary>
		private void MostrarPantallaEfectivo()
		{
            //Se guarda en una variable lo que hay
            Cobros.montoEntradaEfectivo = Math.Round(Efectivo, 2);
            bcargarInfoCobro = true;
            //this.DoClose();
            //despliega la pantalla de toma de efectivo
            this.RequestDialogNavigate<TomaEfectivoViewModel, bool>(null, result =>
                {
                   
                        //CargaInfoCobro();                   
                }
            );
            
		}

        public void CerrarVentana() 
        {
            this.DoClose();
            //ReturnResult(true);
            Dictionary<string, object> par = new Dictionary<string, object>();
            par.Add("habilitarPedidos", true);
            RequestNavigate<MenuClienteViewModel>(par);
        }

        private void MostrarPantallaCambio()
		{
            this.RequestNavigate<TipoCambioViewModel>();
		}

        private void MostrarPantallaFacturas()
		{
            if (GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).FacturasPendientes.Count != 0)
            {               
                //si hay compannia seleccionada se muestra la pantalla de seleccion de factura

                this.RequestDialogNavigate<SeleccionFacturasViewModel, bool>(null, res =>
                    {

                        //Caso 25452 LDS 01/11/2007
                        //Se quita el dispose ya que al realizar la invocación de ese método no funciona el control txtCantidadCopias.

                        if (Cobros.Recibo == null)
                        {
                            //Se cancelo la gestion del recibo
                            Dictionary<string, object> Parametros = new Dictionary<string, object>()
                                {
                                    {"habilitarPedidos", true}
                                };
                            this.RequestNavigate<MenuClienteViewModel>(Parametros);                           
                            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                            this.DoClose();
                        }
                        else
                        {
                            //El usuario pudo cambiar la seleccion de facturas por lo que es necesario
                            //cambiar los valores mostrados
                            this.CargaInfoCobro();
                        }
                    });
                //frmSeleccionFacturas fac = new frmSeleccionFacturas();
                //fac.ShowDialog();
            }
            else
            {
                this.mostrarAlerta("El cliente no tiene facturas pendientes.");
            }
		}

        /// <summary>
        /// Funcion que permite la aplicacion del pago.
        /// </summary>
        private void Aceptar()
        {
            decimal montoNotasCreditoAplicadas = 0;

            if (Cobros.MontoFacturasLocal <= 0)
            {
                //Si el cliente no tiene facturas pendientes se puede realizar un abono a la cuenta

                //El monto del abono debe ser mayor a cero
                //Caso # CR0-02753-9KQP LAS
                //Se utiliza la propiedad Value del objeto ltbTotal en lugar de Text
                //ltbTotal.Text tiene el simbolo de moneda
                decimal montoAbono = this.TotalPagar != 0 ? this.TotalPagar : 0;

                if (montoAbono == 0)
                {
                    this.mostrarAlerta("El monto a pagar debe ser mayor a cero.");
                    return;
                }

                this.mostrarMensaje(Mensaje.Accion.Decision, "aplicar el pago como un abono a la cuenta", r =>
                    {
                        if (r.Equals(DialogResult.Yes))
                        {
                            bool ok = true;
                            try
                            {
                                Cobros.CrearNotaCredito();
                            }
                            catch (Exception e)
                            {
                                this.mostrarAlerta("Error creando nota de crédito," + e.Message, r2 =>
                                    {
                                        //Limpiamos valores globales
                                        Cobros.FinalizarCobro();
                                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                                        this.DoClose();
                                    });
                                ok = false;
                            }

                            if (ok)
                            {
                                //Limpiamos valores globales
                                Cobros.FinalizarCobro();
                                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                                CerrarVentana();
                            }
                        }
                    });

                return;
            }

            bool aplicarPago = false;

            #region switch (Cobros.TipoCobro)
            switch (Cobros.TipoCobro)
            {
                case TipoCobro.SeleccionFacturas:
                    {
                        decimal saldo = 0;

                        if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                        {
                            // LAS. Se resta el monto de los decuentos por pronto pago si se está aplicando y las notas de crédito.
                            saldo = Cobros.MontoFacturasSeleccion;

                            // Facturas de contado y recibos en FR - KFC
                            //Cobros.AplicarLasNotasCredito
                            //original; saldo -= Cobros.AplicarLasNotasCredito=="A" ? Cobros.MontoNotasCreditoLocal : 0;
                            if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                                saldo -= Cobros.MontoNotasCreditoLocal;
                            else if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                                saldo -= Cobros.MontoNotasCreditoSelLocal;

                            saldo -= Cobros.AplicarDescuentosProntoPago ? Cobros.MontoDescuentoProntoPagoLocal : 0;
                            //saldo = (saldo / Cobros.TipoCambio) - (Cobros.MontoSumaChequeEfectivo / Cobros.TipoCambio);
                            saldo = saldo - Cobros.MontoSumaChequeEfectivo;
                            //saldo = (((Cobros.MontoFacturasSeleccion - Cobros.MontoNotasCreditoLocal - Cobros.MontoDescuentoProntoPagoLocal)
                            //    / Cobros.TipoCambio) - (Cobros.MontoSumaChequeEfectivo / Cobros.TipoCambio));
                        }

                        else
                        {
                            // LAS. Se resta el monto de los decuentos por pronto pago si se está aplicando y las notas de crédito.
                            saldo = Cobros.MontoFacturasSeleccion;

                            // Facturas de contado y recibos en FR - KFC
                            //Cobros.AplicarLasNotasCredito
                            //original ,saldo -= Cobros.AplicarLasNotasCredito == "A" ? Cobros.MontoNotasCreditoDolar: 0;
                            if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                                saldo -= Cobros.MontoNotasCreditoDolar;
                            else if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                                saldo -= Cobros.MontoNotasCreditoSelDolar;

                            saldo -= Cobros.AplicarDescuentosProntoPago ? Cobros.MontoDescuentoProntoPagoDolar : 0;
                            saldo = (saldo * Cobros.TipoCambio) - (Cobros.MontoSumaChequeEfectivo * Cobros.TipoCambio);
                            //saldo = (((Cobros.MontoFacturasSeleccion - Cobros.MontoNotasCreditoDolar - Cobros.MontoDescuentoProntoPagoDolar)
                            //    * Cobros.TipoCambio) - (Cobros.MontoSumaChequeEfectivo * Cobros.TipoCambio));
                        }


                        saldo = (decimal)Math.Round(saldo, 2);

                        if (saldo < 0)
                            saldo = 0;

                        if ((Cobros.MontoPendiente != 0 && saldo == 0) ||
                            Cobros.MontoPendiente == 0)
                        {
                            aplicarPago = true;
                        }
                        else
                        {
                            this.mostrarAlerta("Debe completar el monto que se va a pagar.");
                        }

                        break;
                    }
                case TipoCobro.MontoTotal:
                    {
                        if (Cobros.MontoTotalPagar == 0)
                        {
                            //Si no hay notas de credito que aplicar ni montos en efectivo o en cheque
                            //no se puede realizar el cobro

                            decimal montoNotas = 0;

                            // Facturas de contado y recibos en FR - KFC
                            //if (Cobros.AplicarLasNotasCredito)
                            if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                            {
                                montoNotas = Cobros.TipoMoneda == TipoMoneda.LOCAL ?
                                    Cobros.MontoNotasCreditoLocal : Cobros.MontoNotasCreditoDolar;
                            }
                            else if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                            {
                                montoNotas = Cobros.TipoMoneda == TipoMoneda.LOCAL ?
                                    Cobros.MontoNotasCreditoSelLocal : Cobros.MontoNotasCreditoSelDolar;
                            }

                            if (montoNotas == 0) 
                                this.mostrarAlerta("No hay ningún monto que permita el pago de facturas.");
                            else
                                aplicarPago = true;
                        }
                        else
                            aplicarPago = true;

                        if (aplicarPago && FRdConfig.UsaFacturacion)
                        {
                            //Cuando se usa facturacion puede que en la gestion de cobro
                            //se incluyan facturas de contado las cuales deben ser canceladas 
                            //durante la visita actual.

                            //Cuando el cobro es por monto total, el monto que se va a pagar debe ser 
                            //igual o mayor al total de facturas de contado pendientes

                            decimal saldoContado = this.ObtenerSumaFacturasContado();

                            decimal montoPagar = Cobros.MontoTotalPagar;

                            // Facturas de contado y recibos en FR - KFC
                            //if (Cobros.AplicarLasNotasCredito)
                            if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                            {
                                montoPagar += Cobros.TipoMoneda == TipoMoneda.LOCAL ?
                                    Cobros.MontoNotasCreditoLocal : Cobros.MontoNotasCreditoDolar;
                            }
                            else if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                            {
                                montoPagar += Cobros.TipoMoneda == TipoMoneda.LOCAL ?
                                    Cobros.MontoNotasCreditoSelLocal : Cobros.MontoNotasCreditoSelDolar;
                            }

                            if (montoPagar < saldoContado)
                            {
                                this.mostrarMensaje(Mensaje.Accion.Informacion, "El monto a cancelar debe ser igual o mayor al total de facturas de contado pendientes.");
                                aplicarPago = false;
                            }
                        }
                        break;
                    }

                // Facturas de contado y recibos en FR - KFC
                // Tipo de cobro añadido: FacturaActual
                case TipoCobro.FacturaActual:
                    {
                        decimal saldo = 0;

                        if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                        {
                            saldo = pedido.MontoNeto;

                            //saldo -= Cobros.AplicarLasNotasCredito == "A" ? Cobros.MontoNotasCreditoLocal : 0;
                            if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                                saldo -= Cobros.MontoNotasCreditoLocal;
                            else if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                                saldo -= Cobros.MontoNotasCreditoSelLocal;

                            saldo -= Cobros.AplicarDescuentosProntoPago ? Cobros.MontoDescuentoProntoPagoLocal : 0;

                            saldo = saldo - Cobros.MontoSumaChequeEfectivo;

                        }
                        else
                        {
                            saldo = pedido.MontoNeto;

                            //saldo -= Cobros.AplicarLasNotasCredito == "A" ? Cobros.MontoNotasCreditoDolar : 0;
                            if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                                saldo -= Cobros.MontoNotasCreditoDolar;
                            else if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                                saldo -= Cobros.MontoNotasCreditoSelDolar;

                            saldo -= Cobros.AplicarDescuentosProntoPago ? Cobros.MontoDescuentoProntoPagoDolar : 0;
                            saldo = (saldo * Cobros.TipoCambio) - (Cobros.MontoSumaChequeEfectivo * Cobros.TipoCambio);
                        }


                        saldo = (decimal)Math.Round(saldo, 2);

                        if (saldo < 0)
                            saldo = 0;

                        if ((Cobros.MontoPendiente != 0 && saldo == 0) ||
                            Cobros.MontoPendiente == 0)
                        {
                            aplicarPago = true;
                        }
                        else
                        {
                            this.mostrarAlerta("Debe completar el monto que se va a pagar.");
                        }

                        break;
                    }
            }
            #endregion switch (Cobros.TipoCobro)

            if (aplicarPago)
            {
                this.mostrarMensaje(Mensaje.Accion.Terminar, "el recibo", res =>
                    {
                        if (res.Equals(DialogResult.Yes))
                        {
                            try
                            {
                                //Caso 25452 LDS 30/10/2007
                                //Invoca el metodo que aplica el pago
                                //Facturas de contado y recibos en FR - KFC
                                if (esFacturaContado)
                                {
                                    this.recibo = Cobros.AplicarPagoContado(this.pedido);
                                }
                                else
                                {
                                    this.recibo = Cobros.AplicarPago();
                                }

                                //MejorasGrupoPelon600R6 - KFC
                                //Llamado a ActualizarJornada
                                if ((this.MontoNotas + this.Efectivo + this.Cheques + this.Descuento) <= this.TotalPagar)
                                {
                                    montoNotasCreditoAplicadas = Convert.ToDecimal(this.MontoNotas);
                                }
                                else
                                {
                                    montoNotasCreditoAplicadas = Convert.ToDecimal(this.TotalPagar - this.Efectivo - this.Cheques - this.Descuento);
                                }


                                ActualizarJornada(GlobalUI.RutaActual.Codigo,
                                                   Convert.ToDecimal(this.TotalPagar),
                                                   Convert.ToDecimal(this.Cheques),
                                                   Convert.ToDecimal(this.Efectivo),
                                                   montoNotasCreditoAplicadas);

                                //caso S2A35723 LDAV 10/02/2010
                                //Verificar sugerir Imprimir
                                if (Impresora.SugerirImprimir)
                                {
                                    this.mostrarMensaje(Mensaje.Accion.Imprimir, "el recibo", res2 =>
                                    {
                                        //Caso 25452 LDS 30/10/2007
                                        if (res2.Equals(DialogResult.Yes))
                                        {
                                            ImpresionViewModel.OriginalEn = true;
                                            ImpresionViewModel.OnImprimir = ImprimirDocumento;
                                            this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Recibo", mostrarCriterioOrden = true });
                                        }
                                        else
                                        {
                                            //Limpiamos valores globales
                                            Cobros.FinalizarCobro();
                                            this.recibo = null;
                                            CerrarVentana();
                                            //frmMenuPrincipal menu = new frmMenuPrincipal();
                                            //menu.Show();
                                            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                                           
                                        }
                                    });
                                }
                                //caso S2A35723 LDAV 10/02/2010
                                //Verificar sugerir Imprimir    
                                else
                                {
                                    //Limpiamos valores globales
                                    Cobros.FinalizarCobro();
                                    this.recibo = null;
                                    CerrarVentana();
                                    //frmMenuPrincipal menu = new frmMenuPrincipal();
                                    //menu.Show();
                                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                this.mostrarAlerta("Error al aplicar cobro." + ex.Message);
                            }
                        }
                    });
            }
        }


        /// <summary>
        /// Funcion que realiza la cancelacion de todo lo que se realizo en la parte de pago
        /// </summary>
        public void Cancelar()
        {
            if (esFacturaContado)
            {
                /// Muestra el dialogo que pregunta al usuario si desea generar el cobro de la fatura en efectivo
                string mensaje = "¿Desea generar recibo en efectivo para la factura? ";
                this.ShowMessage(Mensaje.titulo, mensaje, MessageBoxButtons.YesNo, MessageBoxIcon.Question, result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            Cobros.MontoEfectivo = pedido.MontoNeto;
                            this.GenerarReciboEfectivo();
                            //frmMenuPrincipal menu = new frmMenuPrincipal();
                            //menu.Show();                            
                            CerrarVentana();
                        }
                    });

            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.Cancelar, "el cobro", result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            //si la confirmacion es positiva se cancela todo los procesos y se despliega la pantalla de menu priincipal
                            Cobros.FinalizarCobro();
                            //frmMenuPrincipal menu = new frmMenuPrincipal();
                            //menu.Show();
                            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                            CerrarVentana();
                        }
                    }
                    );
            }
        }
        #endregion Acciones

        #region Metodos de instancia


        /// <summary>
        ///	Obtiene el saldo total de facturas de contado.
        /// </summary>
        private decimal ObtenerSumaFacturasContado()
        {
            decimal monto = 0;

            foreach (Factura factura in GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).FacturasPendientes)
            {
                if (factura.Tipo == TipoDocumento.FacturaContado)
                {
                    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                        monto += factura.SaldoDocLocal;
                    else
                        monto += factura.SaldoDocDolar;
                }
            }

            return monto;
        }

        #endregion


        /// <summary>
        /// Muestra los valores estaticos en la pantalla segun la moneda
        /// </summary>
        private void MostrarValores()
        {
            if (Moneda == TipoMoneda.LOCAL)
            {
                //Se muestran los valores en moneda local
                //Caso:32842 ABC 19/06/2008 Se cambia el .Text por .Value con el fin de solucionar un problema con los decimales en los texbox
                this.SaldoTotal = Cobros.MontoFacturasLocal;

                if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                    this.MontoNotas = Cobros.MontoNotasCreditoSelLocal;
                else
                    this.MontoNotas = Cobros.MontoNotasCreditoLocal;

                Descuento = Cobros.MontoDescuentoProntoPagoLocal;

                if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                    this.TotalPagar = Cobros.MontoFacturasSeleccion;
                else if (Cobros.TipoCobro == TipoCobro.FacturaActual)
                    this.TotalPagar = Cobros.MontoFacturasLocal;
                else
                    this.TotalPagar = Cobros.MontoTotalPagar;
            }
            else
            {
                //Se muestran los valores en dolares
                this.SaldoTotal = Cobros.MontoFacturasDolar;

                if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
                    this.MontoNotas = Cobros.MontoNotasCreditoSelDolar;
                else
                    this.MontoNotas = Cobros.MontoNotasCreditoDolar;

                Descuento = Cobros.MontoDescuentoProntoPagoDolar;

                if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                    this.TotalPagar = Cobros.MontoFacturasSeleccion;
                else if (Cobros.TipoCobro == TipoCobro.FacturaActual)
                    this.TotalPagar = Cobros.MontoFacturasDolar;
                else
                    this.TotalPagar = Cobros.MontoTotalPagar;
            }

            this.SaldoPendiente = Cobros.MontoPendiente;
            this.Cheques = Cobros.MontoCheques;
            this.Efectivo = Cobros.MontoEfectivo;
        }

        
            
        

        /// <summary>
        /// Realiza los cambios de moneda segun sea necesario y muestra los valores en pantalla.
        /// </summary>
        private void ConvertirValores()
        {
            decimal cambioDolar = Cobros.TipoCambio;

            decimal montoAPagar = 0;

            if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                montoAPagar = Cobros.MontoFacturasSeleccion;
            else
                montoAPagar = Cobros.MontoTotalPagar;

            decimal saldoTotal = 0;
            decimal notas = 0;
            decimal total = 0;
            decimal cheques = 0;
            decimal efectivo = 0;
            decimal saldoPend = 0;
            decimal descuentos = Decimal.Zero;

            if (rbLocal)
            {
                //De Dolar a local
                saldoTotal = Cobros.MontoFacturasDolar * cambioDolar;

                if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                    notas = Cobros.MontoNotasCreditoDolar * cambioDolar;
                else
                    notas = Cobros.MontoNotasCreditoSelDolar * cambioDolar;

                cheques = Cobros.MontoCheques * Cobros.TipoCambio;
                efectivo = Cobros.MontoEfectivo * Cobros.TipoCambio;
                descuentos = Cobros.MontoDescuentoProntoPagoDolar * Cobros.TipoCambio;

                total = montoAPagar * cambioDolar;

                if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                    saldoPend = Cobros.MontoFacturasSeleccion;
                else
                    saldoPend = Cobros.MontoFacturasDolar;

                // Facturas de contado y recibos en FR - KFC
                //if (Cobros.AplicarLasNotasCredito)
                if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                    saldoPend -= Cobros.MontoNotasCreditoDolar;
                if (Cobros.AplicarDescuentosProntoPago)
                    saldoPend -= Cobros.MontoDescuentoProntoPagoDolar;

                saldoPend *= cambioDolar;
                saldoPend -= Cobros.MontoSumaChequeEfectivo * Cobros.TipoCambio;

            }
            else
            {
                //De local a Dolar
                saldoTotal = Cobros.MontoFacturasLocal / cambioDolar;

                if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                    notas = Cobros.MontoNotasCreditoLocal / cambioDolar;
                else
                    notas = Cobros.MontoNotasCreditoSelLocal / cambioDolar;

                cheques = Cobros.MontoCheques / Cobros.TipoCambio;
                efectivo = Cobros.MontoEfectivo / Cobros.TipoCambio;
                descuentos = Cobros.MontoDescuentoProntoPagoLocal / Cobros.TipoCambio;

                total = montoAPagar / cambioDolar;

                if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
                    saldoPend = Cobros.MontoFacturasSeleccion;
                else
                    saldoPend = Cobros.MontoFacturasLocal;


                // Facturas de contado y recibos en FR - KFC
                //if (Cobros.AplicarLasNotasCredito)
                if (Cobros.AplicarLasNotasCredito == FRmConfig.AplicacionAutomatica)
                    saldoPend -= Cobros.MontoNotasCreditoLocal;
                if (Cobros.AplicarDescuentosProntoPago)
                    saldoPend -= Cobros.MontoDescuentoProntoPagoLocal;

                saldoPend /= cambioDolar;
                saldoPend -= Cobros.MontoSumaChequeEfectivo / Cobros.TipoCambio;
            }

            //LJR 10/06/2009 Caso: 35884
            //Si notas credito saldan el saldo (independientemente cual sea la moneda)
            if (Cobros.MontoPendiente <= 0)
                saldoPend = 0;

            //Se asignan los valores a los controles visuales
            this.SaldoTotal = saldoTotal;
            this.MontoNotas = notas;
            this.TotalPagar = total;
            this.Cheques = cheques;
            this.Efectivo = efectivo;
            this.SaldoPendiente = saldoPend;
            Descuento = descuentos;

            //Se asignan los valores a los controles visuales
            //this.SaldoTotal = saldoTotal;
            //this.MontoNotas = notas;
            //this.TotalPagar = total;
            //this.Cheques = cheques;
            //this.Efectivo = efectivo;
            //this.SaldoPendiente = saldoPend;
            //Descuento = descuentos;
        }

        /// <summary>
        /// Calcular la informacion de montos y los muestra en pantalla.
        /// </summary>
        public void CargaInfoCobro()
        {
            //Calcula descuentos solamente si no es una factura de contado
            if (Cobros.TipoCobro == TipoCobro.MontoTotal)
            {
                Cobros.SacarCuentasMontoTotales();
                CalcularDescuentosMontoTotal();
            }
            else
            {
                if (!esFacturaContado)
                    //Calcula descuentos solamente si no es una factura de contado
                    CalcularDescuentos();
            }           

            Cobros.SacarCuentas();

            if ((rbLocal && Cobros.TipoMoneda == TipoMoneda.LOCAL) ||
                (rbDolar && Cobros.TipoMoneda == TipoMoneda.DOLAR))
            {
                //Si el tipo de moneda con que se esta realizando el cobro es el mismo 
                //con que el usuario quiere mostrar los valores, entonces no hay que realizar
                //ningun cambio.
                MostrarValores();
            }
            else
            {
                //Debemos convertir los valores a la moneda correspondiente
                ConvertirValores();

                //PA2-01482-MKP6 - KFC
                //MostrarValores();
            }

        }

		//Caso 25452 LDS 30/10/2007
		/// <summary>
		/// Relocaliza los controles para la impresión de los recibos.
		/// </summary>
        //private void RelocalizarImpresion()
        //{
        //    this.pnlImpresion.Show();
        //    this.pnlImpresion.BringToFront();
        //    this.chkOriginal.Show();
        //    this.chkOriginal.Checked = true;
			
        //    this.txtCantidadCopias.Text = Impresora.CantidadCopias.ToString();			
        //}

		//Caso 25452 LDS 30/10/2007
		/// <summary>
		/// Imprime el recibo que ha sido gestionado.
		/// </summary>
        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {
            try
            {
                if (cantidadCopias >= 0 || esOriginal)
                {
                    if (esOriginal)
                    {
                        this.recibo.LeyendaOriginal = true;
                    }
                    this.recibo.ClienteDatos = GlobalUI.ClienteActual;
                    this.recibo.ImprimeDetalleRecibo(cantidadCopias);
                }
                else
                {
                    viewModel.mostrarMensaje(Mensaje.Accion.Informacion, "Solo se guardará el recibo.");
                }													
            }
            catch(FormatException)
            {
                viewModel.mostrarAlerta("Error obteniendo la cantidad de copias. Formato inválido.");
            }
            catch(Exception ex)
            {
                viewModel.mostrarAlerta(ex.Message);
            }

            //Limpiamos valores globales
            //frmMenuPrincipal menu= new frmMenuPrincipal();
            //menu.Show();
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            Cobros.FinalizarCobro();
            this.recibo = null;
            CerrarVentana();
            
            //viewModel.DoClose();
            //this.DoClose();
        }        

		

        //private void imgbtnCancela_Click(object sender, EventArgs e)
        //{
        //    this.Cancelar();
        //}


        //private void picLogo_MouseDown(object sender, MouseEventArgs e)
        //{
        //    this.lblNomClt.Show();
        //    this.lblNomClt.BringToFront();
        //}

        //private void picLogo_MouseUp(object sender, MouseEventArgs e)
        //{
        //    this.lblNomClt.SendToBack();
        //    this.lblNomClt.Hide();
        //}

        //private void btnExWarning_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    this.noAplicaNClbl.Show();
        //    this.noAplicaNClbl.BringToFront();
        //}

        //private void btnExWarning_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    this.noAplicaNClbl.SendToBack();
        //    this.noAplicaNClbl.Hide();
        //}

        

        ////Caso 25452 LDS 30/10/2007
        //private void ibtContinuar_Click(object sender, System.EventArgs e)
        //{
        //    this.ImprimirDocumento();
        //}

    }
}