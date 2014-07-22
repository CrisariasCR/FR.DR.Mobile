using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
using FR.Core.Model;
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
    public class AplicarPagoContadoViewModel : DialogViewModel<Dictionary<string, object>>
    {
#pragma warning disable 169
#pragma warning disable 649
#pragma warning disable 414

        #region Propiedades

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

        private bool localCheck;
        public bool LocalCheck
        {
            get { return localCheck; }
            set { localCheck = value; RaisePropertyChanged("LocalCheck"); }
        }

        private bool dolarCheck;
        public bool DolarCheck
        {
            get { return dolarCheck; }
            set { dolarCheck = value; RaisePropertyChanged("DolarCheck"); }
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

        #region Propiedades Visuales
        private bool principalVisible=true;
        public bool PrincipalVisible
        {
            get { return principalVisible; }
            set { principalVisible = value; RaisePropertyChanged("PrincipalVisible");if(value)CargaInfoCobro();}
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private bool tomaEfectVisible;
        public bool TomaEfectVisible
        {
            get { return tomaEfectVisible; }
            set { tomaEfectVisible = value; RaisePropertyChanged("TomaEfectVisible"); if (value)CargaInicialTE(); }
        }
        private bool ncVisible;
        public bool NcVisible
        {
            get { return ncVisible; }
            set { ncVisible = value; RaisePropertyChanged("NcVisible"); if (value)CargaInicialNC(); }
        }
        private bool ncConsultaVisible;
        public bool NcConsultaVisible
        {
            get { return ncConsultaVisible; }
            set { ncConsultaVisible = value; RaisePropertyChanged("NcConsultaVisible"); if (value)CargaInicialNCC(); }
        }
        private bool chqVisible;
        public bool ChqVisible
        {
            get { return chqVisible; }
            set { chqVisible = value; RaisePropertyChanged("ChqVisible"); if (value)CargaInicialCHQ(); }
        }
        private bool descuentoVisible;
        public bool DescuentoVisible
        {
            get { return descuentoVisible; }
            set { descuentoVisible = value; RaisePropertyChanged("DescuentoVisible"); }
        }
        #endregion

        #region Principal

        public static bool genCobro;
        /// <summary>
        /// cuando se invoca desde CreacionRecibo
        /// </summary>
        /// <param name="messageId"></param>
        public AplicarPagoContadoViewModel(string messageId)
            : base(messageId)
        {            
                this.esFacturaContado = true;
                this.pedido = Gestor.Pedidos.Gestionados[0];

                //Inicializar el cobro
                Cobros.IniciarCobro(GlobalUI.ClienteActual, pedido.Compania);
                GlobalUI.ClienteActual.CargarNotasCredito(pedido.Compania);
                //Tipo de cobro
                Cobros.TipoCobro = TipoCobro.FacturaActual;
                Cobros.AplicarDescuentosProntoPago = true;

                if (AplicarPagoContadoViewModel.genCobro)
                {
                    //Si la parametrizacion indica que no se hara desglose del pago se genera un recibo en efectivo
                    Cobros.MontoEfectivo = pedido.MontoNeto;
                    GenerarReciboEfectivo();
                }
            
                CargaInicialContado();
        }

        public void Result(bool correcto)
        {
            Dictionary<string, object> Result = new Dictionary<string, object>();

            Result.Add("correcto", correcto);

            ReturnResult(Result);
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
            if (!RbLocal && !RbDolar)
            {
                RbLocal = true;
                RbDolar = false;
            }
            CargaInfoCobro();
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
            get { return new MvxRelayCommand(CancelarPago); }
        }
        #endregion Comandos

        #region Acciones

        /// <summary>
        /// funcion que despliega la pantalla de notas de credito
        /// </summary>
        /// <returns></returns>
        private void MostrarPantallaNotas()
        {
            // Facturas de contado y recibos en FR - KFC
            // En caso de que la parametrizacion lo permita se llamara a la pantalla de seleccion de Notas de Crédito
            if (Cobros.AplicarLasNotasCredito == FRmConfig.SeleccionNotasCredito)
            {
                PrincipalVisible = false;
                NcVisible = true;
            }
            else
            {
                PrincipalVisible = false;
                ncConsultaVisible = true;
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
            PrincipalVisible = false;
            ChqVisible = true;
            
		}

        /// <summary>
		/// funcion que despliega la pantalla de toma de efectivo
		/// </summary>
		private void MostrarPantallaEfectivo()
		{
            TomaEfectVisible = true;
            PrincipalVisible = false;
		}

        public void CerrarVentana() 
        {            
            //ReturnResult(DialogResult.Yes);
            this.DoClose();
            //Dictionary<string, object> par = new Dictionary<string, object>();
            //par.Add("habilitarPedidos", true);
            //RequestNavigate<MenuClienteViewModel>(par);
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
                                Result(true);
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
                                            Result(true);
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
                                    Result(true);
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
        public void CancelarPago()
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
                            Result(true);
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
                            Result(true);
                        }
                    }
                    );
            }
        }

        public void Regresar() 
        {
            if (PrincipalVisible)
                CancelarPago();
            if (NcConsultaVisible)
                RegresarNCC();
            if (NcVisible)
                RegresarNC();
            if (TomaEfectVisible)
                cancela();
            if (ChqVisible)
                CancelaCHQ();

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
                LocalCheck = true;
                DolarCheck = false;
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
                DolarCheck = true;
                LocalCheck = false;
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
            if (!esFacturaContado) CalcularDescuentos();

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
            Result(true);
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

        #endregion

        #region TomaEfectivo

        #region Propiedades

        private string monedaTomaEfect;
        public string MonedaTomaEfect
        {
            get { return monedaTomaEfect; }
            set { monedaTomaEfect = value; this.RaisePropertyChanged("MonedaTomaEfect"); }
        }

        private decimal salPag;
        public decimal SalPag
        {
            get { return salPag; }
            set { salPag = value; this.RaisePropertyChanged("SalPag"); }
        }

        private decimal totPagar;
        public decimal TotPagar
        {
            get { return totPagar; }
            set { totPagar = value; this.RaisePropertyChanged("TotPagar"); }
        }

        private decimal montoEfect;
        public decimal MontoEfect
        {
            get { return montoEfect; }
            set
            {
                bool cambiar = montoEfect != value;
                montoEfect = value; this.RaisePropertyChanged("MontoEfect");
                if (cambiar)
                {
                    CambioTexto(MontoEfect);
                }
            }
        }


        #endregion

        #region mobile

        #region funciones usadas por los eventos

        /// <summary>
        /// Carga inicial de la ventana
        /// </summary>
        private void CargaInicialTE()
        {
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
            //this.lblNomClt.Visible = false;
            this.MonedaTomaEfect = Cobros.TipoMoneda.ToString();

            this.MontoEfect = 0;

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                this.TotPagar = Cobros.MontoFacturasLocal;
            else
                this.TotPagar = Cobros.MontoFacturasDolar;

            this.SalPag = Cobros.MontoPendiente;
        }

        /// <summary>
        /// muestra la pantalla de aplicar pago
        /// </summary>
        private void acepta()
        {
            Cobros.MontoEfectivo += this.MontoEfect;
            TomaEfectVisible = false;
            PrincipalVisible = true;
        }
        /// <summary>
        /// metodo que cancela lo que se haya realizado en la pantalla y
        /// muestra la pantalla de aplicar pago
        /// </summary>
        public void cancela()
        {

            this.mostrarMensaje(Mensaje.Accion.Decision, " cancelar, la información se perderá ¿Esta seguro?", result =>
            {
                if (result == DialogResult.Yes || result == DialogResult.OK)
                {
                    Cobros.MontoEfectivo = 0;
                    TomaEfectVisible = false;
                    PrincipalVisible = true;
                }
            }
            );

        }

        /// <summary>
        /// Este evento se da cuando se cambia el texto en el control,
        /// asi mismo se calcula el monto pendiente con el monto en efectivo que se 
        /// digito.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CambioTexto(decimal monto)
        {
            decimal montoEfectivo = monto;

            decimal montoPendiente = (decimal)Math.Round((decimal)Cobros.MontoPendiente, FRmConfig.CantidadDecimales);

            if (montoEfectivo > montoPendiente && Cobros.MontoFacturasLocal != 0)
            {
                montoEfectivo -= montoEfectivo;

                this.mostrarMensaje(Mensaje.Accion.Informacion, "El monto ingresado es mayor al pendiente.");
            }
            else
            {
                decimal montPend = Cobros.MontoPendiente - montoEfectivo;
                this.SalPag = montPend;
            }
            MontoEfect = montoEfectivo;
        }

        #endregion

        #region Comandos

        public ICommand ComandoCancelarTE
        {
            get { return new MvxRelayCommand(cancela); }
        }

        public ICommand ComandoAceptarTE
        {
            get { return new MvxRelayCommand(acepta); }
        }

        #endregion



        #endregion

        #endregion

        #region NC

        #region Propiedades

        #region Items e ItemActual

        private NotaCredito itemActual;
        public NotaCredito ItemActual
        {
            get { return itemActual; }
            set
            {
                itemActual = value;
                RaisePropertyChanged("ItemActual");
                SeleccionarNotaCredito(value);

            }
        }

        private IObservableCollection<NotaCredito> items;
        public IObservableCollection<NotaCredito> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }

        private List<NotaCredito> ItemsSeleccionados
        {
            get
            {
                return new List<NotaCredito>(this.Items.Where<NotaCredito>(x => x.Selected));
            }

        }

        /// <summary>
        /// retorma la coleccion de Indices seleccionados
        /// </summary>
        public List<int> SelectedIndex
        {
            get
            {
                List<int> result = new List<int>();
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Selected)
                        result.Add(i);
                }
                return result;
            }
        }

        #endregion Items e ItemActual
       

        private DateTime fechaCreacionNC;
        public DateTime FechaCreacionNC
        {
            get { return fechaCreacionNC; }
            set
            {
                if (value != fechaCreacionNC)
                {
                    fechaCreacionNC = value;
                    RaisePropertyChanged("FechaCreacionNC");
                }
            }
        }

        private decimal montoNC;
        public decimal MontoNC
        {
            get { return montoNC; }
            set
            {
                if (value != montoNC)
                {
                    montoNC = value;
                    RaisePropertyChanged("MontoNC");
                }
            }
        }

        private decimal totalNotasCredito;
        public decimal TotalNotasCredito
        {
            get { return totalNotasCredito; }
            set
            {
                if (value != totalNotasCredito)
                {
                    totalNotasCredito = value;
                    RaisePropertyChanged("TotalNotasCredito");
                }
            }
        }

        private decimal totalSaldoFacturas;
        public decimal TotalSaldoFacturas
        {
            get { return totalSaldoFacturas; }
            set
            {
                if (value != totalSaldoFacturas)
                {
                    totalSaldoFacturas = value;
                    RaisePropertyChanged("TotalSaldoFacturas");
                }
            }
        }

        
        #endregion Propiedades

        //Para guardar los indices de las notas de credito selecciondas
        List<string> indicesMarcados = new List<string>();       
        private bool haySeleccionPrevia = false;

        #region CargaInicial

        public void CargaInicialNC()
        {
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            NotaCredito.ItemCheck = ItemCheck;
            
            haySeleccionPrevia = indicesMarcados.Count > 0;

            try
            {
                List<NotaCredito> lista = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).NotasCredito;

                Cobros.MontoNotasCreditoSelLocal = 0;

                this.Items = new SimpleObservableCollection<NotaCredito>(lista);
                RaisePropertyChanged("Items");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando notas del cliente. " + ex.Message);
            }

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
            {
                this.TotalNotasCredito = Cobros.MontoNotasCreditoSelLocal;
                this.TotalSaldoFacturas = Cobros.MontoPendiente;
            }
            else
            {
                this.TotalNotasCredito = Cobros.MontoNotasCreditoSelDolar;
                this.TotalSaldoFacturas = Cobros.MontoPendiente;
            }

            
            CargarMarcadas();            
        }

        private void ItemCheck(NotaCredito facChecked, bool newValueChecked)
        {
            //LJR 09/06/2009 Caso 35843: La forma de seleccionar una factura se modifica
            //Forzar el evento check como la seleccion con click para refrescar datos
            ItemActual = facChecked;

            //Marcar la factura como seleccionada
            //this.SeleccionarFactura(facChecked);
            this.CalculaMontoTotalNotas();
            ValidarMonto();
        }

        #endregion CargaInicial

      

        #region Comandos

        public ICommand ComandoAceptarNC
        {
            get { return new MvxRelayCommand(AceptaNC); }
        }

        public ICommand ComandoSeleccionar
        {
            get { return new MvxRelayCommand(SeleccionarNC); }
        }

        #endregion Comandos

        #region Acciones

        /// <summary>
        /// Funcion que se ejecuta con el cambio de indice del
        /// listview muestra la informacion de la nota de credito seleccionada
        /// </summary>
        public void SeleccionarNotaCredito(NotaCredito itemActual)
        {
            NotaCredito nota;
            if (ItemActual != null)
            {
                if (ItemsSeleccionados.Count == 0)
                    this.MontoNC = 0;
                if (ItemsSeleccionados.Count == 1)
                {
                    nota = itemActual;
                    //Verifica que exista una linea valida seleccionada
                    if (nota != null)
                    {
                        // binding fechaObtenerFechaString
                        this.FechaCreacionNC = ((NotaCredito)nota).FechaRealizacion;

                        // binding fechaObtenerFechaString
                        if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                            this.MontoNC = ((NotaCredito)nota).SaldoDocLocal;
                        else
                            this.MontoNC = ((NotaCredito)nota).SaldoDocDolar;
                    }
                }
                else
                {
                    nota = itemActual;
                    //Verifica que exista una linea valida seleccionada
                    if (nota != null)
                    {
                        // binding fechaObtenerFechaString
                        this.FechaCreacionNC = ((NotaCredito)nota).FechaRealizacion;

                        // binding fechaObtenerFechaString
                        if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                            this.MontoNC = ((NotaCredito)nota).SaldoDocLocal;
                        else
                            this.MontoNC = ((NotaCredito)nota).SaldoDocDolar;
                    }
                    this.CalculaMontoTotalNotas();
                }
            }
            else
            {
                if (ItemsSeleccionados.Count > 0)
                {
                    if (ItemsSeleccionados.Count == 1)
                    {
                        nota = ItemsSeleccionados[0];
                        //Verifica que exista una linea valida seleccionada
                        if (nota != null)
                        {
                            // binding fechaObtenerFechaString
                            this.FechaCreacionNC = ((NotaCredito)nota).FechaRealizacion;

                            // binding fechaObtenerFechaString
                            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                                this.MontoNC = ((NotaCredito)nota).SaldoDocLocal;
                            else
                                this.MontoNC = ((NotaCredito)nota).SaldoDocDolar;
                        }
                    }
                    else
                    {
                        this.CalculaMontoTotalNotas();
                    }
                }
                else
                {
                    this.MontoNC = 0;
                }
            }

        }

        public void SeleccionarNC()
        {
            SeleccionarNotaCredito(ItemActual);
            ValidarMonto();
        }

        /// <summary>
        /// Muestra la pantalla de aplicar pago
        /// </summary>
        private void AceptaNC()
        {
                this.CalculaMontoTotalNotas();
                LlenarNotasSeleccionadas();

                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    Cobros.MontoNotasCreditoSelLocal += (decimal)this.TotalNotasCredito;
                else
                    Cobros.MontoNotasCreditoSelDolar += (decimal)this.TotalNotasCredito;

                this.RegresarNC();               
            
        }

        public void RegresarNC() 
        {
            NcVisible = false;
            PrincipalVisible = true;
        }

        #endregion Accioness
	

        #region Variables

        /// <summary>
        /// notas de credito a consultar
        /// </summary>
        private List<NotaCredito> notasCredito;


        /// <summary>
        /// Indica la factura selecccionada
        /// </summary>
        private NotaCredito notaSeleccionada = new NotaCredito();

        /// <summary>
        /// Indica si hay una factura seleccionada actualmente
        /// </summary>
        private bool esSeleccionada = false;

        



        #endregion        
		
		#region Funciones Usadas por los eventos

        
        /// <summary>
        /// Remarca las notas de credito que fueron marcadas previamente
        /// </summary>
        private void CargarMarcadas()
        {
            if (Cobros.notasSeleccionadas.Count > 0)
            {
                    foreach (NotaCredito item in this.Items)
                    {                        
                        foreach (NotaCredito nota in Cobros.notasSeleccionadas)
                        {
                            // añadir a lista de notas seleccionadas
                            if (nota.Numero == item.Numero)
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                    }
                    RaisePropertyChanged("Items");
                    this.CalculaMontoTotalNotas();  
                
            }
        }

        /// <summary>
        /// Saca el monto total que hay que pagar de las facturas seleccionadas
        /// </summary>
        /// <param name="esCheck">si el ultimo click chequeo un item</param>
        /// <param name="index">el item que fue chequeado o deschequeado</param>
        private void CalculaMontoTotalNotas()
        {
            decimal montoTotalNotas = 0;

            foreach (NotaCredito item in this.ItemsSeleccionados)
            {              

                //Validar contra el ultimo chequeo de item
                if (item.Selected == true)
                {
                    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                        montoTotalNotas += item.SaldoDocLocal;
                    else
                        montoTotalNotas += item.SaldoDocDolar;
                }
            }

            this.TotalNotasCredito = montoTotalNotas;

        }

        /// <summary>
        /// Recorre la lista guardando en un arreglo las notas de credito seleccionadas
        /// </summary>
        private void LlenarNotasSeleccionadas()
        {
            Cobros.notasSeleccionadas.Clear();

            foreach (NotaCredito nota in this.Items.Where<NotaCredito>(x => x.Selected))
            {
                // añadir a lista de notas seleccionadas
                Cobros.notasSeleccionadas.Add(nota);
            }
        }
        

        /// <summary>
        /// Funcion que valida y advierte al usuario que la seleccion de N/C sobrepasa el monto a pagar
        /// el usuario puede aceptar presionando SI y se aplicaran las N/C desde la mas antigua en adelante
        /// dentro de las que fueran seleccionadas. Si presiona NO se desmarcará la ultima N/C seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool ValidarMonto()
        {
            bool res = false;
            if (this.TotalNotasCredito > this.TotalSaldoFacturas)
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, " seleccionar la nota de crédito el monto seleccionado es mayor al saldo a cancelar",
                    result =>
                    {
                        if (result == DialogResult.No)
                        {
                            itemActual.Selected = false;
                            RaisePropertyChanged("ItemActual");
                            RaisePropertyChanged("Items");
                            this.MontoNC = 0;
                        }
                        else
                        {
                            res = true;
                        }
                    }
                );
                return res;
            }
            else
            {
                return true;
            }
            
        }

		#endregion

        #endregion

        #region NcConsulta

        #region Propiedades

        #region Items e ItemActual
        private NotaCredito itemActualNCC;
        public  NotaCredito ItemActualNCC
        {
            get { return itemActualNCC; }
            set
            {
               itemActualNCC = value;
               SeleccionarNotaNCC(value);
               RaisePropertyChanged("ItemActualNCC");
                
            }
        }

        private List<NotaCredito> ItemsSeleccionadosNCC
        {
            get
            {
                return new List<NotaCredito>(this.ItemsNCC.Where<NotaCredito>(x => x.Selected));
            }

        }

        /// <summary>
        /// retorma la coleccion de Indices seleccionados
        /// </summary>
        public List<int> SelectedIndexNCC
        {
            get
            {
                List<int> result = new List<int>();
                for (int i = 0; i < ItemsNCC.Count; i++)
                {
                    if (ItemsNCC[i].Selected)
                        result.Add(i);
                }
                return result;
            }
        }

        private IObservableCollection<NotaCredito> itemsNCC;
        public IObservableCollection<NotaCredito> ItemsNCC
        {
            get { return itemsNCC; }
            set { itemsNCC = value; RaisePropertyChanged("ItemsNCC"); }
        } 

        #endregion Items e ItemActual

        private DateTime fechaCreacionNCC;
        public DateTime FechaCreacionNCC
        {
            get { return fechaCreacionNCC; }
            set
            {
                if (value != fechaCreacionNCC)
                {
                    fechaCreacionNC = value;
                    RaisePropertyChanged("FechaCreacionNCC");
                }
            }
        }

        private decimal montoNCC;
        public decimal MontoNCC
        {
            get { return montoNCC; }
            set
            {
                if (value != montoNCC)
                {
                    montoNC = value;
                    RaisePropertyChanged("MontoNCC");
                }
            }
        }

        private decimal totalNotasCreditoNCC;
        public decimal TotalNotasCreditoNCC
        {
            get { return totalNotasCreditoNCC; }
            set
            {
                if (value != totalNotasCreditoNCC)
                {
                    totalNotasCredito = value;
                    RaisePropertyChanged("TotalNotaCreditoNCC");
                }
            }
        }

        private decimal totalSaldoFacturasNCC;
        public decimal TotalSaldoFacturasNCC
        {
            get { return totalSaldoFacturasNCC; }
            set
            {
                if (value != totalSaldoFacturasNCC)
                {
                    totalSaldoFacturas = value;
                    RaisePropertyChanged("TotalSaldoFacturasNCC");
                }
            }
        }

        
        #endregion Propiedades

        #region CargaInicial

        public void CargaInicialNCC()
        {
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            try
            {
                List<NotaCredito> lista = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).NotasCredito;
                this.ItemsNCC = new SimpleObservableCollection<NotaCredito>(lista);
                RaisePropertyChanged("ItemsNCC");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando notas del cliente. " + ex.Message);
            }

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
            {
                this.TotalNotasCreditoNCC = Cobros.MontoNotasCreditoLocal;
                this.TotalSaldoFacturasNCC = Cobros.MontoFacturasLocal;
            }
            else
            {
                this.TotalNotasCreditoNCC = Cobros.MontoNotasCreditoDolar;
                this.TotalSaldoFacturasNCC = Cobros.MontoFacturasDolar;
            }
        }

        #endregion CargaInicial

        #region Acciones

        /// <summary>
        /// Funcion que se ejecuta con el cambio de indice del
        /// listview muestra la informacion de la nota de credito seleccionada
        /// </summary>
        private void SeleccionarNotaNCC(NotaCredito nota)
        {            
            //Verifica que exista una linea valida seleccionada
            if (nota != null)
            {                
                // binding fechaObtenerFechaString
                this.FechaCreacionNCC = nota.FechaRealizacion;

                // binding fechaObtenerFechaString
                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    this.MontoNCC = nota.MontoDocLocal;
                else
                    this.MontoNCC = nota.MontoDocDolar;
            }
        }

        public void RegresarNCC()
        {
            NcConsultaVisible = false;
            PrincipalVisible = true;
        }

        #endregion Accioness

        #endregion

        #region Cheques

        #region Constructores
         /// <summary>
        /// Crea un nuevo formulario de cheques para agregar cheques al recibo en gestión.
        /// </summary>
        public void CargaInicialCHQ()        
        {            
            this.chequesIngresados = Cobros.Recibo.ChequesAsociados;
            this.ChequesCHQ = new SimpleObservableCollection<Cheque>(this.chequesIngresados);
            RaisePropertyChanged("ChequesCHQ");
            this.esConsulta = false;
            this.TotalCheques = Cobros.MontoCheques;
            if (Cobros.TipoMoneda.ToString().Equals("L"))
            {
                this.monedaCHQ = TipoMoneda.LOCAL;
            }
            else
            {
                this.monedaCHQ = TipoMoneda.DOLAR;
            }
            this.LoadPantalla();
        }       


        #endregion

        #region Propiedades

       // public Recibo Recibo { get { return ConsultaCobroViewModel.ReciboSeleccionado; } }

        //public IObservableCollection<Cheque> Cheques { get { return new SimpleObservableCollection<Cheque>(Recibo.ChequesAsociados); } }
        public IObservableCollection<Cheque> ChequesCHQ { get; set ; }
        private Cheque itemActualCHQ;
        public Cheque ItemActualCHQ
        {
            get { return itemActualCHQ; }
            set
            {
                itemActualCHQ = value;
                RaisePropertyChanged("ItemActualCHQ");
            }
        }
        public IObservableCollection<string> Bancos { get; set; }
        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        private List<Cheque> chequesIngresados = new List<Cheque>();
        private List<Banco> BancosList = new List<Banco>();

        private string bancoActual;
        public string BancoActual
        {
            get { return bancoActual; }
            set
            {
                bancoActual = value;
                RaisePropertyChanged("BancoActual");
            }
        }

        private string cheque;
        public string Cheque
        {
            get { return cheque; }
            set
            {
                cheque = value;
                RaisePropertyChanged("Cheque");
            }
        }

        private string cuenta;
        public string Cuenta
        {
            get { return cuenta; }
            set
            {
                cuenta = value;
                RaisePropertyChanged("Cuenta");
            }
        }

        private DateTime fechaCHQ;
        public DateTime FechaCHQ
        {
            get { return fechaCHQ; }
            set
            {
                fechaCHQ = value;
                RaisePropertyChanged("FechaCHQ");
            }
        }

        private decimal montoCheque;
        public decimal MontoCheque
        {
            get { return montoCheque; }
            set
            {
                montoCheque = value;
                RaisePropertyChanged("MontoCheque");
            }
        }

        private decimal saldoCHQ;
        public decimal SaldoCHQ
        {
            get { return saldoCHQ; }
            set
            {
                saldoCHQ = value;
                RaisePropertyChanged("SaldoCHQ");
            }
        }

        private bool esConsulta;

        private decimal totalCheques;
        public decimal TotalCheques
        {
            get { return totalCheques; }
            set
            {
                totalCheques = value;
                RaisePropertyChanged("TotalCheques");
            }
        }

        private bool verCheques = true;
        public bool VerCheques
        {
            get { return verCheques; }
            set { verCheques = value; RaisePropertyChanged("VerCheques"); }
        }

        private TipoMoneda monedaCHQ;

        #endregion

        #region Comandos

        public ICommand ComandoAgregarCHQ
        {
            get { return new MvxRelayCommand(AgregaCHQ); }
        }

        public ICommand ComandoEliminarCHQ
        {
            get { return new MvxRelayCommand(EliminaCHQ); }
        }

        public ICommand ComandoCancelarCHQ
        {
            get { return new MvxRelayCommand(CancelaCHQ); }
        }

        public ICommand ComandoAceptarCHQ
        {
            get { return new MvxRelayCommand(AceptarCHQ); }
        }

        public ICommand ComandoVerChequesCHQ
        {
            get { return new MvxRelayCommand(cambiarVerCheques); }
        }

        #endregion Comandos        

        #region Metodos de la logica de Negocio

        #region Funciones usadas por los eventos
        /// <summary>
		/// Funcion que carga los datos iniciales.
		/// </summary>
		private void LoadPantalla()
		{            
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNomClt.Visible = false;
			
				//CrearDateTimePicker();

                this.FechaCHQ = DateTime.Today;			

				try
				{
					BancosList = Banco.ObtenerBancos(Cobros.Recibo.Compania,false);
				}
				catch(Exception exc)
				{
					this.mostrarAlerta("Error cargando las entidades financieras. " + exc.Message);
				}
                Bancos = new SimpleObservableCollection<string>(BancosList.ConvertAll(x => x.Descripcion));
                RaisePropertyChanged("Bancos");
			this.CargarCheques();
			this.CargarInfo();
		}

		/// <summary>
		/// Funcion que despliega en pantalla la informacion inicial
		/// </summary>
		private void CargarInfo()
		{			
			this.Cheque=string.Empty;
			this.Cuenta=string.Empty;
			this.MontoCheque=0;
            this.TotalCheques = this.totalCheques;
            this.SaldoCHQ = Cobros.MontoPendiente;
		}

		
		/// <summary>
		/// funcion que hace el llamado a la pantalla de aplicar pago
		/// y hace persistente en un arreglo global los cheques agregados.
		/// </summary>
        private void AceptarCHQ()
		{
			if (!this.esConsulta)
				Cobros.Recibo.ChequesAsociados = this.chequesIngresados;
            ChqVisible = false;
            PrincipalVisible = true;
		}
		
		
		/// <summary>
		/// Funcion que valida que en realidad se desea cancelar el ingreso de cheques y 
		/// no tomar en cuenta ningun proceso de la toma de cheques
		/// </summary>
        public void CancelaCHQ()
		{
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "el ingreso de cheques", result =>
                {
                    //si la respuesta al mensaje es positiva se debe cancelar todo lo que se haya realizado
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {                        
                        this.chequesIngresados.Clear();//se limpia el arreglo de cheques
                        ChequesCHQ = new SimpleObservableCollection<Cheque>(chequesIngresados);
                        RaisePropertyChanged("ChequesCHQ");
                        Cobros.MontoCheques = 0;
                        ChqVisible = false;
                        PrincipalVisible = true;
                    }
                });			
			
		}
		
		/// <summary>
		/// Funcion que agrega un nuevo cheque
		/// </summary>
        private void AgregaCHQ()
		{
			if(this.Validacion())
			{
				decimal montoCheque = this.MontoCheque;//toma el valor digitado 
			
				if(montoCheque > Cobros.MontoPendiente && Cobros.MontoFacturasLocal != 0)//valida que el monto del cheque no sobrepase el monto que esta pendiente
				{
				    this.mostrarMensaje(Mensaje.Accion.Informacion,"El monto ingresado es mayor al monto que se debe."); 
				}
				else{

                    Cheque miCheque = new Cheque(GlobalUI.RutaActual.Codigo, GlobalUI.ClienteActual.Codigo,Cobros.Recibo.Compania);

					//Suma el monto del cheque al monto actual y global
					this.totalCheques += montoCheque;
					Cobros.MontoCheques = this.totalCheques;
				
					//miCheque.Banco.Codigo = bancos[this.cboBancos.SelectedIndex-1].Codigo;//le asigna el codigo del banco al cheque
                    miCheque.Banco.Codigo = ((Banco)BancosList.Find(x => x.Descripcion == BancoActual)).Codigo;
					//miCheque.Banco.Descripcion = bancos[this.cboBancos.SelectedIndex-1].Descripcion;
                    miCheque.Banco.Descripcion = ((Banco)BancosList.Find(x => x.Descripcion == BancoActual)).Codigo;
					miCheque.Banco.Cuenta = this.Cuenta;//le asigna el numero de cuenta
					miCheque.Numero = this.Cheque;//le asigna el numero de cheque
                    miCheque.Fecha = this.FechaCHQ;//le asigna la fecha
					miCheque.Monto = this.MontoCheque;//le asigna el monto
					this.chequesIngresados.Add(miCheque);//agrega el cheque a un arreglo
				
					//this.AgregarChequeListView(miCheque);//invoca la funcion que carga el cheque en el listView
                    this.AgregarChequeListView();//invoca la funcion que carga el cheque en el listView
					Cobros.SacarCuentas();//invoca al metodo que calcula los montos
					this.CargarInfo();//invoca la funcion que carga la informacion en pantalla
				}

			}
		}

		/// <summary>
		/// Funcion que tiene la logica para la realizacion de la eliminacion de un cheque
		/// </summary>
        private void EliminaCHQ()
		{
            if (ItemActualCHQ == null)
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"un cheque"); 
			else
			{				

                this.mostrarMensaje(Mensaje.Accion.Retirar, "el cheque número: " +
                    this.ItemActualCHQ.Numero + " Monto:" + this.ItemActualCHQ.Monto, result =>
                    {
                        //si la respuesta del mensaje es afirmativo entonces 
                        //se procede a eliminar el cheque de del arreglo de cheques agregados
                        //y se recalculan los montos
                        if (result == DialogResult.Yes || result==DialogResult.OK)
                        {
                            Cheque chequeEliminar = ItemActualCHQ;

                            //Restamos el monto del cheque al monto de cheques actual y global
                            this.TotalCheques -= chequeEliminar.Monto;
                            Cobros.MontoCheques = this.TotalCheques;

                            this.chequesIngresados.Remove(ItemActualCHQ);//se remueve el cheque del arreglo
                            
                            //this.lsvcheque.Items.RemoveAt(j);// se remueve el cheque del listview
                            this.ChequesCHQ = new SimpleObservableCollection<Cheque>(chequesIngresados); RaisePropertyChanged("ChequesCHQ");
                            RaisePropertyChanged("ChequesCHQ");
                            Cobros.SacarCuentas();//se reacalculan los montos
                            this.CargarInfo();// se invoca a la funcion de carga  de informacion
                        }
                    }); 			
			}
		}

		/// <summary>
		/// Agrega al un cheque al final de la lista visual (listView)
		/// </summary>
		/// <param name="nuevoCheque"></param>
		private void AgregarChequeListView()
		{
            ChequesCHQ = new SimpleObservableCollection<Cheque>(this.chequesIngresados); RaisePropertyChanged("ChequesCHQ");
            //string [] infoCheque = new string[3];

            //infoCheque[0] = nuevoCheque.Numero;
            //infoCheque[1] = nuevoCheque.Banco.Descripcion;
            //if (this.moneda == TipoMoneda.LOCAL)
            //    infoCheque[2] = GestorUtilitario.FormatNumero(nuevoCheque.Monto);
            //else
            //    infoCheque[2] = GestorUtilitario.FormatNumero(nuevoCheque.Monto, true);
			
            //ListViewItem row = new ListViewItem(infoCheque);//crea una nueva linea del listview
            //this.lsvcheque.Items.Add(row);//agrega la linea al listview
		}

		/// <summary>
		/// Funcion encargada de cargar los cheques al listView
		/// </summary>
		private void CargarCheques()
		{
            this.ChequesCHQ.Clear();
		
			// se recorre el arreglo de cheques ingresados y se van creando nuevas lineas del listview 
            //foreach(Cheque cheque in this.chequesIngresados)//recorre arreglo de cheques ingresados
            //    this.AgregarChequeListView(cheque);
            ChequesCHQ = new SimpleObservableCollection<Cheque>(this.chequesIngresados);
            RaisePropertyChanged("ChequesCHQ");
		}



		#endregion
		
		#region Funciones de validacion
		/// <summary>
		/// funcion que valida los campos que deben ser llenados esten correctos
		/// y que el monto del cheque no sobrepase el monto que se debe pagar 
		/// </summary>
		/// <param name="montoCheque"></param>
		/// <returns>retorna un verdadero si todo es correcto y un falso si hay algo incorrecto</returns>
		private bool Validacion()
		{
			if(this.MontoCheque==0)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"El monto del cheque no puede ser 0."); 
				return false;
			}
			else if (this.MontoCheque.ToString()=="")//valida que el campo de monto del cheque no este vacio
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Debe digitar el monto del cheque."); 
				return false;
			}
			else if(string.IsNullOrEmpty(BancoActual))//valida que haya un banco seleccionado
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"el banco al que pertenece el cheque"); 
				return false;
			}
			else if(this.Cheque=="")//valida que el campo de numero de cheque no este vacio
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Debe digitar el número de cheque."); 
			    return false;
			}
			else if(this.Cuenta=="")//valida que el campo del numero de cuenta no este vacio
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Debe digitar el número de cuenta."); 
				return false;
			}
			else
			{
				return true;
			}
		}

        public void cambiarVerCheques() 
        {
            if (VerCheques)
            {
                VerCheques = false;
            }
            else
            {
                VerCheques = true;
            }
        }
		#endregion

        #endregion  

        #endregion



    }
}