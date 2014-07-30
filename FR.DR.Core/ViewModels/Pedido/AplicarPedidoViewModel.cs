using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class AplicarPedidoViewModel : DialogViewModel<DialogResult>
    {
        public AplicarPedidoViewModel(string messageId):base(messageId)
        {
            CargaInicial();
        }        

        #region Propiedades

        #region Logica de Negocio

        private bool cargandoFormulario = true;
        private decimal descuentoPorCIA = decimal.Zero;
        private bool actualizandoDescuento = false;
        private bool Imprimir = false;
        private bool contContado = false;
        public static bool ventanaInactiva = false;
        public static string Error = string.Empty;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }
 

        #endregion

        #region Binding

        private DireccionEntrega direccionSeleccionada;
        public DireccionEntrega DireccionSeleccionada
        {
            get { return direccionSeleccionada; }
            set { direccionSeleccionada = value; CambioDireccion(); RaisePropertyChanged("DireccionSeleccionada"); }
        }

        private IObservableCollection<DireccionEntrega> direcciones;
        public IObservableCollection<DireccionEntrega> Direcciones
        {
            get { return direcciones; }
            set { direcciones = value; RaisePropertyChanged("Direcciones"); }
        }

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; CargarDireccionesEntrega(); RaisePropertyChanged("CompaniaActual"); }
        }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private DateTime fechaEntrega;
        public DateTime FechaEntrega
        {
            get { return fechaEntrega; }
            set { fechaEntrega = value; CambioFechaEntrega(); RaisePropertyChanged("FechaEntrega"); }
        }

        private Pedido pedido;
        public Pedido Pedido
        {
            get { return pedido; }
            set { pedido = value; RaisePropertyChanged("Pedido"); }
        }

        private Garantia garantia;
        public Garantia Garantia
        {
            get { return garantia; }
            set { garantia = value; RaisePropertyChanged("Garantia"); }
        }

        public decimal TotalBruto
        {
            get { return Gestor.Pedidos.TotalBruto; }
        }

        private decimal totalNeto;
        public decimal TotalNeto
        {
            set { totalNeto = value; RaisePropertyChanged("TotalNeto"); }
            get { return totalNeto; }
        }

        private decimal totalGarantias;
        public decimal TotalGarantias
        {
            set { totalGarantias = value; RaisePropertyChanged("TotalGarantias"); }
            get { return totalGarantias; }
        }

        private decimal granTotal;
        public decimal GranTotal
        {
            set { granTotal = value; RaisePropertyChanged("GranTotal"); }
            get { return granTotal; }
        }

        //public decimal Descuento
        //{
        //    get { return Gestor.Pedidos.TotalDescuentoLineas; }
        //}
        private decimal descuento;
        public decimal Descuento
        {
            set { descuento = value; RaisePropertyChanged("Descuento"); }
            get { return descuento; }
        }


        private decimal impuestoVentas;
        public decimal ImpuestoVentas
        {
            set { impuestoVentas = value; RaisePropertyChanged("ImpuestoVentas"); }
            get { return impuestoVentas; }
        }

        private decimal consumo;
        public decimal Consumo
        {
            set { consumo = value; RaisePropertyChanged("Consumo"); }
            get { return consumo; }
        }

        private decimal retenciones;
        public decimal Retenciones
        {
            set { consumo = value; RaisePropertyChanged("Retenciones"); }
            get { return consumo; }
        }

        private decimal porcDescuento1;
        public decimal PorcDescuento1
        {
            set { if (!ventanaInactiva) { porcDescuento1 = value; CambioTextoDescuento(true); RaisePropertyChanged("PorcDescuento1"); } }
            get { return porcDescuento1; }
        }

        private decimal porcDescuento2;
        public decimal PorcDescuento2
        {
            set { porcDescuento2 = value; CambioTextoDescuento(false); RaisePropertyChanged("PorcDescuento2"); }
            get { return porcDescuento2; }
        }

        private decimal descuento1;
        public decimal Descuento1
        {
            set { descuento1 = value; RaisePropertyChanged("Descuento1"); }
            get { return descuento1; }
        }

        private decimal descuento2;
        public decimal Descuento2
        {
            set { descuento2 = value; RaisePropertyChanged("Descuento2"); }
            get { return descuento2; }
        }
        
        public bool Desc1Enabled
        {
            get { return Pedidos.HabilitarDescuento1; }
        }

        public bool Desc2Enabled
        {
            get { return Pedidos.HabilitarDescuento2; }
        }

        private bool direccionVisible = false;
        public bool DireccionVisible
        {
            get { return direccionVisible; }
            set { direccionVisible = value; RaisePropertyChanged("DireccionVisible"); }
        }

        private string labelImp1;
        public string LabelImpuesto1
        {
            get { return labelImp1; }
            set { labelImp1 = value; RaisePropertyChanged("LabelImpuesto1"); }
        }

        private string labelImp2;
        public string LabelImpuesto2
        {
            get { return labelImp2; }
            set { labelImp2 = value; RaisePropertyChanged("LabelImpuesto2"); }
        }

        private bool entregaEnabled;
        public bool EntregaEnabled
        {
            get { return entregaEnabled; }
            set { entregaEnabled = value; RaisePropertyChanged("EntregaEnabled"); }
        }

        private bool retencionEnabled;
        public bool RetencionEnabled
        {
            get { return retencionEnabled; }
            set { retencionEnabled = value; RaisePropertyChanged("RetencionEnabled"); }
        }

        #endregion

        #endregion

        #region Metodos Logica de Negocio

        private void CargaInicial()
        {
            ventanaInactiva = false;
            contContado = false;
            cargandoFormulario = true;

            //this.lblWarning.Text = FRmConfig.MensajeCreditoExcedido;

            EntregaEnabled = !Pedidos.FacturarPedido;            

            bool dolar = Gestor.Pedidos.Gestionados[0].Configuracion.Nivel.Moneda == TipoMoneda.DOLAR;

            try
            {
                //Retenciones                
                if (Gestor.Pedidos.Gestionados[0].Configuracion.Compania.Retenciones)
                {
                    Gestor.Pedidos.Gestionados[0].CalcularRetenciones();
                    RetencionEnabled = false;
                    AplicarPedidoViewModel.Error = string.Empty;
                }
                else
                {
                    RetencionEnabled = true;
                }

                CalcularImpVentas();
                                
                MostrarDatos();
                if (Gestor.Pedidos.Gestionados.Count > 1)
                {
                    LabelImpuesto1 = "Impuesto 1:";
                    LabelImpuesto2 = "Impuesto 2:";
                }
                else
                {
                    LabelImpuesto1 = Gestor.Pedidos.Gestionados[0].Configuracion.Compania.Impuesto1Descripcion + ":";
                    LabelImpuesto2 = Gestor.Pedidos.Gestionados[0].Configuracion.Compania.Impuesto2Descripcion + ":";
                }

                descuentoPorCIA = 100 * Gestor.Pedidos.Gestionados[Gestor.Pedidos.Gestionados.Count - 1].PorcDescGeneral;
            }
            catch (Exception exc)
            {
                AplicarPedidoViewModel.Error = exc.Message;
            }

            cargandoFormulario = false;
        }

        private void CalcularImpVentas()
        {
            try
            {
                foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                {
                    pedido.RecalcularImpuestos(pedido.MontoSubTotal < pedido.Configuracion.Compania.MontoMinimoExcento);
                }
                Gestor.Pedidos.SacarMontosTotales();
            }
            catch (Exception exc)
            {
                throw new Exception("Error al recalcular montos. " + exc.Message);
            }
        }

        private void MostrarDatos()
        {
            Pedido = Gestor.Pedidos.Gestionados[0];
            FechaEntrega = pedido.FechaEntrega;
            PorcDescuento1 = Gestor.Pedidos.PorcDesc1;
            PorcDescuento2 = Gestor.Pedidos.PorcDesc2;
            Descuento1 = Gestor.Pedidos.TotalDescuento1;
            Descuento2 = Gestor.Pedidos.TotalDescuento2;
            Descuento=Gestor.Pedidos.TotalDescuentoLineas;
            ImpuestoVentas = Gestor.Pedidos.TotalImpuesto1;
            Consumo = Gestor.Pedidos.TotalImpuesto2;
            Retenciones = Gestor.Pedidos.TotalRetenciones;
            TotalNeto = Gestor.Pedidos.TotalNeto;

            if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
            {
                Garantia = Gestor.Garantias.Gestionados[0];
                TotalGarantias = Gestor.Garantias.TotalNeto;
                GranTotal = Gestor.Garantias.TotalNeto + Gestor.Pedidos.TotalNeto;
            }
        }

        private void CargarCias()
        {
            Companias = new SimpleObservableCollection<string>(
                Gestor.Pedidos.Gestionados.Select(x => x.Compania).ToList()
                );

            CompaniaActual = Companias.Count > 0 ? Companias[0] : null;
        }

        private void CargarDireccionesEntrega()
        {
            try
            {
                if(Direcciones != null)
                    Direcciones.Clear();

                if (!string.IsNullOrEmpty(CompaniaActual))
                {
                    try
                    {
                        GlobalUI.ClienteActual.ObtenerDireccionesEntrega(CompaniaActual);

                        ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual);

                        cliente.ObtenerDireccionesEntrega();

                        Pedido pedido = Gestor.Pedidos.Gestionados[0];

                        Direcciones = new SimpleObservableCollection<DireccionEntrega>(cliente.DireccionesEntrega);

                        DireccionSeleccionada = Direcciones.First(x =>
                            (x.Codigo == cliente.DireccionEntregaDefault &&
                            pedido.DireccionEntrega == cliente.DireccionEntregaDefault) ||
                            x.Codigo == pedido.DireccionEntrega);
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando direccion de entrega del cliente. " + ex.Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.mostrarAlerta("Error al cargar las direcciones de embarque" + ex.Message);
            }
        }

        public void SeleccionarDireccion(string codigoDireccion)
        {
            if (DireccionSeleccionada.Codigo != codigoDireccion)
            {
                DireccionSeleccionada = Direcciones.First(x => x.Codigo == codigoDireccion);
            }
        }

        private void CambioDireccion()
        {
            try
            {
                Gestor.Pedidos.DefineDirEntregaPedido(CompaniaActual, DireccionSeleccionada);
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error al definir la dirección de embarque" + ex.Message);
            }
        }

        private bool ValidaCantidadDeLineas()
        {
            try
            {
                foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                {
                    if (pedido.Detalles.Lista.Count > Pedidos.MaximoLineasDetalle)
                    {
                        this.mostrarAlerta("Se excede el máximo de líneas permitidas (" + Pedidos.MaximoLineasDetalle + ") para la compañía " + pedido.Compania);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Pedidos.FacturarPedido)
                    this.mostrarAlerta("Error validando la cantidad de líneas de la factura. " + ex.Message);
                else
                    this.mostrarAlerta("Error validando la cantidad de líneas del pedido. " + ex.Message);

                return false;
            }

            return true;
        }

        private bool ValidaDireccionDefinida()
        {
            try
            {
                foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                {
                    if (pedido.DireccionEntrega == string.Empty)
                    {
                        this.mostrarAlerta("Debe definir la dirección de entrega para la compañía " + pedido.Compania);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error validando dirección de entrega. " + ex.Message);
                return false;
            }

            return true;
        }

        private bool ValidarUtilidad()
        {
            decimal costoTotal = 0;
            decimal montoPedido = 0;

            foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
            {
                costoTotal = pedido.ObtenerCostoTotal();

                montoPedido = pedido.MontoBruto - pedido.MontoTotalDescuento;

                if (montoPedido < costoTotal)
                {
                    this.mostrarAlerta("No se puede vender por debajo del costo total.");
                    return false;
                }
            }

            return true;
        }

        private void TerminarGestion()
        {
                string mensaje = " terminar la gestión " + (Pedidos.FacturarPedido ? "de la factura" : "del pedido");

                this.mostrarMensaje(Mensaje.Accion.Decision, mensaje, resultado =>
                    {
                        if (resultado == DialogResult.Yes || resultado == DialogResult.OK)
                        {
                            if (FRdConfig.ReciboFacturasContado && ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.DiasNeto == 0) && (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura)))
                            {
                                VerificarImpresionContado();
                            }
                            else
                            {
                                VerificarImpresion();
                            }

                        }
                        else
                            return;
                    });        
        }

        private void VerificarImpresion()
        {
            if (Pedidos.ValidarLimiteCredito != Pedidos.LIMITECREDITO_NOAPLICA)
            {
                if ((Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_AMBOS) || (Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_FACTURA && Pedidos.FacturarPedido))
                {
                    if (GlobalUI.ClienteActual.LimiteCreditoExcedido(Gestor.Pedidos.Gestionados[0].Compania, (decimal)Gestor.Pedidos.Gestionados[0].MontoNeto))
                    {
                        this.mostrarAlerta("El monto excede el de crédito permitido para este cliente, no se podrá gestionar el documento.");
                        return;
                    }
                }
                if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
                {
                    if (GlobalUI.ClienteActual.LimiteCreditoExcedido(Gestor.Pedidos.Gestionados[0].Compania, (((decimal)Gestor.Pedidos.Gestionados[0].MontoNeto)+(decimal)Gestor.Garantias.Gestionados[0].MontoNeto)))
                    {
                        this.mostrarAlerta("El monto excede el de crédito permitido para este cliente, no se podrá gestionar el documento.");
                        return;
                    }
                }
            }

            if (!FRmConfig.EnConsulta)
            {
                try
                {
                    Gestor.Pedidos.CargarConsecutivos();
                    if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
                    {
                        Gestor.Garantias.CargarConsecutivos();
                    }
                }
                catch (Exception ex)
                {
                    if (Pedidos.FacturarPedido)
                        this.mostrarAlerta(ex.Message + " de la factura.");
                    else
                        this.mostrarAlerta(ex.Message + " del pedido.");

                    return;
                }
            }

            // Si es factura de contado se genera el cobro - KFC
            //if ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.Codigo == "0")
            if (FRdConfig.ReciboFacturasContado)
            {
                //TODO GARANTIAS
                if ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.DiasNeto == 0) && (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura))
                    GenerarCobro();
            }

            //DialogResult res = DialogResult.None;
            this.Imprimir = false;
            if (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura && Pedidos.DesgloseLotesFactura)
            {
                if (!Pedidos.DesgloseLotesFacturaObliga)
                {
                    this.mostrarMensaje(Mensaje.Accion.Decision, "realizar el desglose de lotes para la factura en proceso", result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            if (Impresora.SugerirImprimir)
                            {
                                string msj = Pedidos.FacturarPedido ? "el detalle de las facturas realizadas" : "el detalle de los pedidos realizados";

                                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Facturas" : "Impresión de Detalle de Pedidos";

                                this.mostrarMensaje(Mensaje.Accion.Imprimir, msj, r2 =>
                                {
                                    if (r2 == DialogResult.Yes)
                                    {
                                        Imprimir = true;
                                        this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                        {
                                            TerminarGestion3();
                                        });
                                    }
                                    else
                                    {
                                        Imprimir = false;
                                        this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                        {
                                            TerminarGestion3();
                                        });
                                    }
                                    
                                });
                            }
                            else
                            {
                                this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                {
                                    TerminarGestion2();
                                });
                            }                            
                        }
                        //TerminarGestion2();
                    });
                }
                else
                {
                    if (Impresora.SugerirImprimir)
                    {
                        string msj = Pedidos.FacturarPedido ? "el detalle de las facturas realizadas" : "el detalle de los pedidos realizados";

                        string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Facturas" : "Impresión de Detalle de Pedidos";

                        this.mostrarMensaje(Mensaje.Accion.Imprimir, msj, r2 =>
                        {
                            if (r2 == DialogResult.Yes)
                            {
                                Imprimir = true;
                                this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                {
                                    TerminarGestion3();
                                });
                            }
                            else
                            {
                                
                                this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                {
                                    TerminarGestion3();
                                });
                            }

                        });
                    }
                    else
                    {
                        
                        this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                        {
                            TerminarGestion2();
                        });
                    }
                }
            }
            else{
                TerminarGestion2();
            }

            
        }

        private void VerificarImpresionContado()
        {
            if (Pedidos.ValidarLimiteCredito != Pedidos.LIMITECREDITO_NOAPLICA)
            {
                if ((Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_AMBOS) || (Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_FACTURA && Pedidos.FacturarPedido))
                {
                    if (GlobalUI.ClienteActual.LimiteCreditoExcedido(Gestor.Pedidos.Gestionados[0].Compania, (decimal)Gestor.Pedidos.Gestionados[0].MontoNeto))
                    {
                        this.mostrarAlerta("El monto excede el de credito permitido para este cliente, no se podra gestionar el documento.");
                        return;
                    }
                }
            }

            if (!FRmConfig.EnConsulta)
            {
                try
                {
                    Gestor.Pedidos.CargarConsecutivos();
                    if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
                    {
                        Gestor.Garantias.CargarConsecutivos();
                    }
                }
                catch (Exception ex)
                {
                    if (Pedidos.FacturarPedido)
                        this.mostrarAlerta(ex.Message + " de la factura.");
                    else
                        this.mostrarAlerta(ex.Message + " del pedido.");

                    return;
                }
            }

            // Si es factura de contado se genera el cobro - KFC
            //if ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.Codigo == "0")
            if (FRdConfig.ReciboFacturasContado)
            {
                if ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.DiasNeto == 0) && (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura))
                    GenerarCobro();
            }


        }

        public void ContinuarContado() 
        {


            //DialogResult res = DialogResult.None;
            this.Imprimir = false;
            if (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura && Pedidos.DesgloseLotesFactura)
            {
                if (!Pedidos.DesgloseLotesFacturaObliga)
                {
                    this.mostrarMensaje(Mensaje.Accion.Decision, "realizar el desglose de lotes para la factura en proceso", result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            if (Impresora.SugerirImprimir)
                            {
                                string msj = Pedidos.FacturarPedido ? "el detalle de las facturas realizadas" : "el detalle de los pedidos realizados";

                                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Facturas" : "Impresión de Detalle de Pedidos";

                                this.mostrarMensaje(Mensaje.Accion.Imprimir, msj, r2 =>
                                {
                                    if (r2 == DialogResult.Yes)
                                    {
                                        Imprimir = true;
                                        this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                        {
                                            TerminarGestion3();
                                        });
                                    }
                                    else
                                    {
                                        Imprimir = false;
                                        this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                        {
                                            TerminarGestion3();
                                        });
                                    }

                                });
                            }
                            else
                            {
                                this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                {
                                    TerminarGestion2();
                                });
                            }
                        }
                        //TerminarGestion2();
                    });
                }
                else
                {
                    if (Impresora.SugerirImprimir)
                    {
                        string msj = Pedidos.FacturarPedido ? "el detalle de las facturas realizadas" : "el detalle de los pedidos realizados";

                        string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Facturas" : "Impresión de Detalle de Pedidos";

                        this.mostrarMensaje(Mensaje.Accion.Imprimir, msj, r2 =>
                        {
                            if (r2 == DialogResult.Yes)
                            {
                                Imprimir = true;
                                this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                {
                                    TerminarGestion3();
                                });
                            }
                            else
                            {
                                this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                                {
                                    TerminarGestion3();
                                });
                            }

                        });
                    }
                    else
                    {
                        this.RequestDialogNavigate<LineasFacturaLoteViewModel, bool>(null, r1 =>
                        {
                            TerminarGestion2();
                        });
                    }
                }
            }
            else
            {
                TerminarGestion2();
            }
        }

        private void TerminarGestion2()
        {
            if (Impresora.SugerirImprimir)
            {
                string msj = Pedidos.FacturarPedido ? "el detalle de las facturas realizadas" : "el detalle de los pedidos realizados";

                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Facturas" : "Impresión de Detalle de Pedidos";

                this.mostrarMensaje(Mensaje.Accion.Imprimir, msj, result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        ImpresionViewModel.OriginalEn = true;
                        ImpresionViewModel.OnImprimir = ImprimirDocumento;
                        this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = titulo, mostrarCriterioOrden = true });
                    }
                    else
                    {
                        if (this.GuardaDocumento())
                        {
                            this.LimpiarCerrarFormulario();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
                            {
                                this.mostrarAlerta(AplicarPedidoViewModel.Error, res => { LimpiarCerrarFormulario(); });
                            }
                        }
                    }

                });
            }

            else
            {

                if (this.GuardaDocumento())
                {
                    this.LimpiarCerrarFormulario();
                }
                else
                {
                    if (!string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
                    {
                        this.mostrarAlerta(AplicarPedidoViewModel.Error, res => { LimpiarCerrarFormulario(); });
                    }
                }

            }
        }

        private void TerminarGestion3()
        {
            if (Impresora.SugerirImprimir)
            {
                string msj = Pedidos.FacturarPedido ? "el detalle de las facturas realizadas" : "el detalle de los pedidos realizados";

                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Facturas" : "Impresión de Detalle de Pedidos";


                if (Imprimir)
                {
                    ImpresionViewModel.OriginalEn = true;
                    ImpresionViewModel.OnImprimir = ImprimirDocumento;
                    this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = titulo, mostrarCriterioOrden = true });
                }
                else
                {
                    if (this.GuardaDocumento())
                    {
                        if (string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
                            this.LimpiarCerrarFormulario();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
                        {
                            this.mostrarAlerta(AplicarPedidoViewModel.Error, res => { LimpiarCerrarFormulario(); });
                        }
                    }
                }


            }

            else
            {

                if(this.GuardaDocumento())
                {
                    this.LimpiarCerrarFormulario();
                }
                else
                {
                    if (!string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
                    {
                        this.mostrarAlerta(AplicarPedidoViewModel.Error, res => { LimpiarCerrarFormulario(); });
                    }
                }

            }
        }

        private void LimpiarCerrarFormulario()
        {
            AplicarPedidoViewModel.Error = string.Empty;
            Pedidos.FacturarPedido = false;
            Gestor.Pedidos = new Pedidos();
            Gestor.Garantias = new Garantias();

            if (FRmConfig.EnConsulta)
            {
                ReturnResult(DialogResult.Yes);
               // this.DoClose();
            }
            else
            {
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"habilitarPedidos", true}
                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
                this.DoClose();
            }
        }

        private bool GuardaDocumento()
        {
            bool result = true;
            if (FRmConfig.EnConsulta)
            {
                Pedido pedido = null;

                try
                {
                    pedido = Gestor.Pedidos.Gestionados[0];
                    pedido.Actualizar(true);
                }
                catch (Exception ex)
                {
                    if (pedido.Tipo == TipoPedido.Factura)
                        AplicarPedidoViewModel.Error="Error al actualizar factura. " + ex.Message;
                    else
                        AplicarPedidoViewModel.Error = "Error al actualizar pedido. " + ex.Message;
                    result = false;
                }
            }
            else
            {
                try
                {
                    
                    Gestor.Pedidos.GuardarPedidos();
                    if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
                    {
                        Gestor.Garantias.GuardarGarantias(Gestor.Pedidos.Gestionados[0].Numero);                        
                    }
                    if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
                    {
                        ActualizarJornada(GlobalUI.RutaActual.Codigo, decimal.Round(Convert.ToDecimal(TotalNeto), 2), decimal.Round(Convert.ToDecimal(TotalGarantias), 2));
                    }
                    else
                    {
                        ActualizarJornada(GlobalUI.RutaActual.Codigo, decimal.Round(Convert.ToDecimal(TotalNeto), 2));
                    }
                }
                catch (Exception ex)
                {
                    if (pedido.Tipo == TipoPedido.Factura)
                        AplicarPedidoViewModel.Error = "Error al guardar factura. " + ex.Message;                        
                    else
                        AplicarPedidoViewModel.Error = "Error al guardar pedido. " + ex.Message;
                    result = false;
                        
                }
            }
            return result;
        }

        private void ActualizarJornada(string ruta, decimal monto)
        {
            TipoMoneda moneda = Gestor.Pedidos.Gestionados[0].Configuracion.Nivel.Moneda;
            string colCantidad = "";
            string colMonto = "";
            string colCantidadCondPago = "";
            string colMontoCondPago = "";

            if (Pedidos.FacturarPedido)
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.FACTURAS_LOCAL;
                    colMonto = JornadaRuta.MONTO_FACTURAS_LOCAL;
                    if (this.Pedido.CondicionPago.DiasNeto == 0)
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
                    colCantidad = JornadaRuta.FACTURAS_DOLAR;
                    colMonto = JornadaRuta.MONTO_FACTURAS_DOLAR;
                    if (this.Pedido.CondicionPago.DiasNeto == 0)
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
            }
            else
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.PEDIDOS_LOCAL;
                    colMonto = JornadaRuta.MONTO_PEDIDOS_LOCAL;
                }
                else
                {
                    colCantidad = JornadaRuta.PEDIDOS_DOLAR;
                    colMonto = JornadaRuta.MONTO_PEDIDOS_DOLAR;
                }
            }

            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, 1);
                JornadaRuta.ActualizarRegistro(ruta, colMonto, monto);

                if (Pedidos.FacturarPedido)
                {
                    JornadaRuta.ActualizarRegistro(ruta, colCantidadCondPago, 1);
                    JornadaRuta.ActualizarRegistro(ruta, colMontoCondPago, monto);
                }

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                this.mostrarAlerta("Error al actualizar datos. " + ex.Message);
            }
        }

        private void ActualizarJornada(string ruta, decimal monto,decimal montoGarantias)
        {
            TipoMoneda moneda = Gestor.Pedidos.Gestionados[0].Configuracion.Nivel.Moneda;
            string colCantidad = "";
            string colMonto = "";
            string colCantidadGar = "";
            string colMontoGar = "";
            string colCantidadCondPago = "";
            string colMontoCondPago = "";
            string colCantidadCondPagoGar = "";
            string colMontoCondPagoGar = "";

            if (Pedidos.FacturarPedido)
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.FACTURAS_LOCAL;
                    colMonto = JornadaRuta.MONTO_FACTURAS_LOCAL;
                    colCantidadGar = JornadaRuta.GARANTIAS_LOCAL;
                    colMontoGar = JornadaRuta.MONTO_GARANTIAS_LOCAL;
                    if (this.Pedido.CondicionPago.DiasNeto == 0)
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_LOCAL_CONT;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_LOCAL_CONT;
                        colCantidadCondPagoGar = JornadaRuta.GARANTIAS_LOCAL_CONT;
                        colMontoCondPagoGar = JornadaRuta.MONTO_GARANTIAS_LOCAL_CONT;
                    }
                    else
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_LOCAL_CRE;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_LOCAL_CRE;
                        colCantidadCondPagoGar = JornadaRuta.GARANTIAS_LOCAL_CRE;
                        colMontoCondPagoGar = JornadaRuta.MONTO_GARANTIAS_LOCAL_CRE;
                    }

                }
                else
                {
                    colCantidad = JornadaRuta.FACTURAS_DOLAR;
                    colMonto = JornadaRuta.MONTO_FACTURAS_DOLAR;
                    colCantidadGar = JornadaRuta.GARANTIAS_DOLAR;
                    colMontoGar = JornadaRuta.MONTO_GARANTIAS_DOLAR;
                    if (this.Pedido.CondicionPago.DiasNeto == 0)
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_DOLAR_CONT;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_DOLAR_CONT;
                        colCantidadCondPagoGar = JornadaRuta.GARANTIAS_DOLAR_CONT;
                        colMontoCondPagoGar = JornadaRuta.MONTO_GARANTIAS_DOLAR_CONT;
                    }
                    else
                    {
                        colCantidadCondPago = JornadaRuta.FACTURAS_DOLAR_CONT;
                        colMontoCondPago = JornadaRuta.MONTO_FACTURAS_DOLAR_CONT;
                        colCantidadCondPagoGar = JornadaRuta.GARANTIAS_DOLAR_CONT;
                        colMontoCondPagoGar = JornadaRuta.MONTO_GARANTIAS_DOLAR_CONT;
                    }
                }
            }
            else
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.PEDIDOS_LOCAL;
                    colMonto = JornadaRuta.MONTO_PEDIDOS_LOCAL;
                }
                else
                {
                    colCantidad = JornadaRuta.PEDIDOS_DOLAR;
                    colMonto = JornadaRuta.MONTO_PEDIDOS_DOLAR;
                }
            }

            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, 1);
                JornadaRuta.ActualizarRegistro(ruta, colMonto, monto);
                if (Pedidos.FacturarPedido)
                {
                    JornadaRuta.ActualizarRegistro(ruta, colCantidadGar, 1);
                    JornadaRuta.ActualizarRegistro(ruta, colMontoGar, montoGarantias);                
                    JornadaRuta.ActualizarRegistro(ruta, colCantidadCondPago, 1);
                    JornadaRuta.ActualizarRegistro(ruta, colMontoCondPago, monto);
                    JornadaRuta.ActualizarRegistro(ruta, colCantidadCondPagoGar, 1);
                    JornadaRuta.ActualizarRegistro(ruta, colMontoCondPagoGar, montoGarantias);
                }

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                this.mostrarAlerta("Error al actualizar datos. " + ex.Message);
            }
        }

        private void GenerarCobro()
        {
            //Dictionary<string, object> Parametros = new Dictionary<string, object>();
            //Parametros.Add("facContado", "S");
            //Parametros.Add("genCobro", "N");

            if (FRdConfig.FormaGenerarRecibo == FRmConfig.Consultar)
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, "desglosar el cobro de la factura", res =>
                    {
                        if (res == DialogResult.Yes)
                        {                            
                            AplicarPagoContadoViewModel.genCobro = false;
                            this.RequestDialogNavigate<AplicarPagoContadoViewModel, Dictionary<string, object>>(null, result =>
                                {
                                    bool correcto = (bool)result["correcto"];
                                    contContado = true;
                                    
                                });
                            //frmAplicarPago cobro = new frmAplicarPago(false, Gestor.Pedidos.Gestionados[0]);
                            //cobro.ShowDialog();
                        }
                        else
                        {
                            //Generar el pago en efectivo automaticamente                            
                            AplicarPagoContadoViewModel.genCobro = true;
                            var cobro = new AplicarPagoContadoViewModel(null);
                            ContinuarContado();
                            //frmAplicarPago cobro = new frmAplicarPago(true, Gestor.Pedidos.Gestionados[0]);
                        }
                    });
            } //FormaGenerarRecibo por ReciboFacturasContado 
            if (FRdConfig.FormaGenerarRecibo == FRmConfig.ObligarDesglose) // obliga a hacer el desglose del pago
            {
                //llamar a la pantalla de aplicar pago                
                AplicarPagoContadoViewModel.genCobro = false;
                this.RequestDialogNavigate<AplicarPagoContadoViewModel, Dictionary<string, object>>(null, result =>
                {
                    bool correcto = (bool)result["correcto"];
                    contContado = true;
                    
                });
                //frmAplicarPago cobro = new frmAplicarPago(false, Gestor.Pedidos.Gestionados[0]);
                //cobro.ShowDialog();
            }
            if (FRdConfig.FormaGenerarRecibo == FRmConfig.GenerarEfectivo) // no muestra la pantalla de pago y genera automaticamente un recibo en efectivo
            {
                //Generar el pago en efectivo automaticamente                
                AplicarPagoContadoViewModel.genCobro = true;
                var cobro = new AplicarPagoContadoViewModel(null);
                ContinuarContado();
                //frmAplicarPago cobro = new frmAplicarPago(true, Gestor.Pedidos.Gestionados[0]);
            }
        }

        

        private void CambioTextoDescuento(bool descuento1)
        {
            if (!actualizandoDescuento && !cargandoFormulario)
            {
                try
                {
                    if(descuento1)
                        this.ModificarDescuento(EDescuento.DESC1);
                    else
                        this.ModificarDescuento(EDescuento.DESC2);
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error modificando descuento. " + ex.Message);
                }
            }
        }

        private void ModificarDescuento(EDescuento numeroDescuento)
        {
            actualizandoDescuento = true;

            try
            {
                if (ValidarDescuentos(numeroDescuento))
                {
                    PorcDescuento1 = PorcDescuento2 = decimal.Zero;
                }

                switch (numeroDescuento)
                {
                    case EDescuento.DESC1:
                        Descuento1 = Gestor.Pedidos.DefinirDescuento(EDescuento.DESC1, PorcDescuento1);
                        Descuento2 = Gestor.Pedidos.DefinirDescuento(EDescuento.DESC2, PorcDescuento2);
                        break;
                    case EDescuento.DESC2:
                        Descuento2 = Gestor.Pedidos.DefinirDescuento(EDescuento.DESC2, PorcDescuento2);                        
                        break;
                }

                this.CalcularImpVentas();
                this.MostrarDatos();
            }
            catch (Exception exc)
            {
                this.mostrarAlerta(exc.Message);
            }

            actualizandoDescuento = false;
        }

        private bool ValidarDescuentos(EDescuento numeroDescuento)
        {
            Boolean limpiarDescuentos = false;
            if (PorcDescuento1 < 0)
            {
                this.mostrarAlerta("El Descuento 1 debe ser mayor a cero");
                limpiarDescuentos = true;
            }
            if (limpiarDescuentos == false && PorcDescuento2 < 0)
            {
                this.mostrarAlerta("El Descuento 2 debe ser mayor a cero");
                limpiarDescuentos = true;
            }
            
            if (!limpiarDescuentos)
            {
                if ((PorcDescuento1 + PorcDescuento2) > 100)
                {
                    this.mostrarAlerta("La suma del porcentaje de descuento 1 y 2 no puede exceder el 100%");
                    limpiarDescuentos = true;
                }
                if (EDescuento.DESC1 == numeroDescuento && Pedidos.MaximoDescuentoPermitido1 < PorcDescuento1)
                {
                    //Esto para el caso en que el descuento para cada cliente sea mayor al decuento
                    //establecido en el Config.xml 
                    if (descuentoPorCIA <= Pedidos.MaximoDescuentoPermitido1)
                    {
                        limpiarDescuentos = true;
                        this.mostrarAlerta("La suma máxima permitida para el Descuento 1 es de " + Pedidos.MaximoDescuentoPermitido1.ToString());
                        PorcDescuento1 = decimal.Zero;
                    }
                    if (descuentoPorCIA > Pedidos.MaximoDescuentoPermitido1 && descuentoPorCIA < PorcDescuento1)
                    {
                        limpiarDescuentos = true;
                        this.mostrarAlerta("La suma máxima permitida para el Descuento 1 es de " + String.Format("{0:0.##}", descuentoPorCIA));//.ToString("00", CultureInfo.InvariantCulture));
                        PorcDescuento1 = decimal.Zero;
                    }
                }

                if (EDescuento.DESC2 == numeroDescuento && Pedidos.MaximoDescuentoPermitido2 < PorcDescuento2)
                {
                    limpiarDescuentos = true;
                    this.mostrarAlerta("La suma máxima permitida para el Descuento 2 es de " + Pedidos.MaximoDescuentoPermitido2.ToString());
                    PorcDescuento2 = decimal.Zero;
                }
            }
            return limpiarDescuentos;
        }

        private void CambioFechaEntrega()
        {
            foreach (Pedido ped in Gestor.Pedidos.Gestionados)
            {
                ped.FechaEntrega = FechaEntrega;
            }
        }

        //Caso 25452 LDS 19/10/2007
        /// <summary>
        /// Imprime los documentos que han sido gestionados.
        /// </summary>
        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {

            //Caso 32081 LDS 09/04/2008
            //Guardamos los pedidos o facturas.
            //this.GuardaDocumento();         
            //Caso 32081 LDS 09/04/2008
            //Realizamos la impresión de los pedidos o facturas.
            try
            {
                int cantidad = 0;

                cantidad = cantidadCopias;

                string textGarantia = string.Empty;

                if (Pedidos.FacturarPedido)
                {

                    if (cantidad >= 0 || esOriginal)
                    {
                        if (FRdConfig.UsaEnvases && Gestor.Garantias.Gestionados.Count > 0)
                        {
                            if (esOriginal)
                                foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                                    garantia.LeyendaOriginal = true;

                            Garantias garantiasImprimir = new Garantias(Gestor.Garantias.Gestionados, GlobalUI.ClienteActual);
                            garantiasImprimir.ImprimeDetalleGarantia(cantidad, (DetalleSort.Ordenador)ordernarPor, ref textGarantia);


                            if (esOriginal)
                                foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                                    garantia.Impreso = true;


                        }

                        if (esOriginal)
                            foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                                pedido.LeyendaOriginal = true;


                        Pedidos pedidosImprimir = new Pedidos(Gestor.Pedidos.Gestionados, GlobalUI.ClienteActual);
                        pedidosImprimir.ImprimeDetalleFactura(cantidad, (DetalleSort.Ordenador)ordernarPor, textGarantia);

                        if (esOriginal)
                            foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                                pedido.Impreso = true;



                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "Solo se guardará la factura.");
                    }
                }
                else
                {
                    if (cantidad >= 0)
                    {
                        Pedidos pedidosImprimir = new Pedidos(Gestor.Pedidos.Gestionados, GlobalUI.ClienteActual);
                        pedidosImprimir.ImprimeDetallePedido(cantidad, (DetalleSort.Ordenador)ordernarPor);

                        foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                            pedido.Impreso = true;
                    }
                }
            }
            catch (FormatException)
            {
                this.mostrarAlerta("Error obteniendo la cantidad de copias. Formato inválido.");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(ex.Message);
            }
            //Caso 32081 LDS 09/04/2008
            //Guardamos los pedidos o facturas gestionadas y luego limpiamos y regresamos al formulario que invocó el presente formulario.
            if (this.GuardaDocumento())
            {
                //Caso 32081 LDS 09/04/2008
                //Primero guardamos el pedido o factura y luego realizamos la impresión, ya que si ocurre un error con la impresión
                //solo se debe imprimir el documento o no volver a cargar.
                this.LimpiarCerrarFormulario();
            }
            else
            {
                if (!string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
                {
                    this.mostrarAlerta(AplicarPedidoViewModel.Error, res => { LimpiarCerrarFormulario(); });
                }
            }

        }


        #endregion

        #region Comandos

        public ICommand ComandoDireccion
        {
            get { return new MvxRelayCommand(MostrarDireccion); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarPedido); }
        }

        public ICommand ComandoConsultarRetenciones
        {
            get { return new MvxRelayCommand(ConsultarRetenciones); }
        }

        public ICommand ComandoEditar
        {
            get { return new MvxRelayCommand(MontosPedido); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(ValidarDocumento); }
        }

        #endregion

        #region Acciones

        private void MostrarDireccion()
        {
            DireccionVisible = !DireccionVisible;
            if (DireccionVisible)
            {
                CargarCias();
            }
        }

        private void ConsultarPedido()
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("invocadesde", Formulario.AplicarPedido.ToString());
            Parametros.Add("mostrarControles", "N");
            if (Pedidos.FacturarPedido)
            {
                Parametros.Add("esFactura", "S");
            }
            else
            {
                Parametros.Add("esFactura", "N");
            }
            this.RequestNavigate<DetallePedidoViewModel>(Parametros);
        }

        private void ConsultarRetenciones()
        {
            if (Gestor.Pedidos.Gestionados[0].iArregloRetenciones != null && Gestor.Pedidos.Gestionados[0].iArregloRetenciones.Count > 0)
                this.RequestNavigate<ConsultaRetencionesViewModel>();
            else
                this.mostrarAlerta("No hay retenciones gestionadas.");
        }

        private void MontosPedido()
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"facturaConsignacion", false}
                };
            this.RequestNavigate<MontosPedidoViewModel>(Parametros);
        }

        private void ValidarDocumento()
        {
            AplicarPedidoViewModel.Error = string.Empty;
            if (!ValidaCantidadDeLineas() || !ValidaDireccionDefinida()
                || (Pedidos.CambiarPrecio && !ValidarUtilidad()))
                return;

            if (Gestor.Pedidos.Gestionados.Count == 0)
            {
                this.mostrarAlerta("No hay pedidos gestionados");
                return;
            }
            TerminarGestion();           
        }

        public void Regresar()
        {
            if(DireccionVisible)
            {
                DireccionVisible = false;
            }
            else
            {
                this.DoClose();
            }
        }

        public void OnResume()
        {
            if (!string.IsNullOrEmpty(AplicarPedidoViewModel.Error))
            {
                this.mostrarAlerta(AplicarPedidoViewModel.Error, res => this.LimpiarCerrarFormulario());
            }
            else
            {
                ventanaInactiva = false;
                RaisePropertyChanged("PorcDescuento1");
                if (contContado)
                {
                    this.ContinuarContado();
                }
            }
        }  

        #endregion

    }
}