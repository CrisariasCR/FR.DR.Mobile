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
    public class AplicarGarantiaViewModel : DialogViewModel<bool>
    {
        public AplicarGarantiaViewModel(string messageId)
            : base(messageId)
        {
          CargaInicial();
        }        

        #region Propiedades

        #region Logica de Negocio

        private bool cargandoFormulario = true;
        private decimal descuentoPorCIA = decimal.Zero;
        private bool actualizandoDescuento = false;
        private bool Imprimir = false;
        public static bool ventanaInactiva = false;   

        #endregion

        #region Binding

        private DireccionEntrega direccionSeleccionada;
        public DireccionEntrega DireccionSeleccionada
        {
            get { return direccionSeleccionada; }
            set { direccionSeleccionada = value; CambioDireccion(); RaisePropertyChanged("DireccionSeleccionada"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
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

        private Garantia garantia;
        public Garantia Garantia
        {
            get { return garantia; }
            set { garantia = value; RaisePropertyChanged("Pedido"); }
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

        #endregion

        #endregion

        #region Metodos Logica de Negocio

        private void CargaInicial()
        {
            ventanaInactiva = false;
            cargandoFormulario = true;

            //this.lblWarning.Text = FRmConfig.MensajeCreditoExcedido;

            EntregaEnabled = !Pedidos.FacturarPedido;

            bool dolar = Gestor.Garantias.Gestionados[0].Configuracion.Nivel.Moneda == TipoMoneda.DOLAR;

            try
            {
                CalcularImpVentas();
                MostrarDatos();
                if (Gestor.Garantias.Gestionados.Count > 1)
                {
                    LabelImpuesto1 = "Impuesto 1:";
                    LabelImpuesto2 = "Impuesto 2:";
                }
                else
                {
                    LabelImpuesto1 = Gestor.Garantias.Gestionados[0].Configuracion.Compania.Impuesto1Descripcion + ":";
                    LabelImpuesto2 = Gestor.Garantias.Gestionados[0].Configuracion.Compania.Impuesto2Descripcion + ":";
                }

                descuentoPorCIA = 100 * Gestor.Garantias.Gestionados[Gestor.Garantias.Gestionados.Count - 1].PorcDescGeneral;
            }
            catch (Exception exc)
            {
                this.mostrarAlerta(exc.Message);
            }

            cargandoFormulario = false;
        }

        private void CalcularImpVentas()
        {
            try
            {
                foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                {
                    garantia.RecalcularImpuestos(garantia.MontoSubTotal < garantia.Configuracion.Compania.MontoMinimoExcento);
                }
                Gestor.Garantias.SacarMontosTotales();
            }
            catch (Exception exc)
            {
                throw new Exception("Error al recalcular montos. " + exc.Message);
            }
        }

        private void MostrarDatos()
        {
            Garantia = Gestor.Garantias.Gestionados[0];
            FechaEntrega = garantia.FechaEntrega;
            PorcDescuento1 = Gestor.Garantias.PorcDesc1;
            PorcDescuento2 = Gestor.Garantias.PorcDesc2;
            Descuento1 = Gestor.Garantias.TotalDescuento1;
            Descuento2 = Gestor.Garantias.TotalDescuento2;
            Descuento = Gestor.Garantias.TotalDescuentoLineas;
            ImpuestoVentas = Gestor.Garantias.TotalImpuesto1;
            Consumo = Gestor.Garantias.TotalImpuesto2;
            TotalNeto = Gestor.Garantias.TotalNeto;
        }

        private void CargarCias()
        {
            Companias = new SimpleObservableCollection<string>(
                Gestor.Garantias.Gestionados.Select(x => x.Compania).ToList()
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

                        Garantia garantia = Gestor.Garantias.Gestionados[0];

                        Direcciones = new SimpleObservableCollection<DireccionEntrega>(cliente.DireccionesEntrega);

                        DireccionSeleccionada = Direcciones.First(x =>
                            (x.Codigo == cliente.DireccionEntregaDefault &&
                            garantia.DireccionEntrega == cliente.DireccionEntregaDefault) ||
                            x.Codigo == garantia.DireccionEntrega);
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
                Gestor.Garantias.DefineDirEntregaPedido(CompaniaActual, DireccionSeleccionada);
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
                foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                {
                    if (garantia.Detalles.Lista.Count > Pedidos.MaximoLineasDetalle)
                    {
                        this.mostrarAlerta("Se excede el máximo de líneas permitidas (" + Pedidos.MaximoLineasDetalle + ") para la compañía " + garantia.Compania);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Pedidos.FacturarPedido)
                    this.mostrarAlerta("Error validando la cantidad de líneas de la Garantía. " + ex.Message);
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
                foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                {
                    if (garantia.DireccionEntrega == string.Empty)
                    {
                        this.mostrarAlerta("Debe definir la dirección de entrega para la compañía " + garantia.Compania);
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

            foreach (Garantia garantia in Gestor.Garantias.Gestionados)
            {
                costoTotal = garantia.ObtenerCostoTotal();

                montoPedido = garantia.MontoBruto - garantia.MontoTotalDescuento;

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
            string mensaje = " terminar la gestión " + (Pedidos.FacturarPedido ? "de la Garantía" : "del pedido");

            this.mostrarMensaje(Mensaje.Accion.Decision, mensaje, resultado =>
                {
                    if (resultado == DialogResult.Yes || resultado == DialogResult.OK)
                    {
                            VerificarImpresion();                       
                    }
                    else
                        return;
                });
        }

        private void VerificarImpresion()
        {
            //if (Pedidos.ValidarLimiteCredito != Pedidos.LIMITECREDITO_NOAPLICA)
            //{
            //    if ((Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_AMBOS) || (Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_FACTURA && Pedidos.FacturarPedido))
            //    {
            //        if (GlobalUI.ClienteActual.LimiteCreditoExcedido(Gestor.Garantias.Gestionados[0].Compania, (decimal)Gestor.Garantias.Gestionados[0].MontoNeto))
            //        {
            //            this.mostrarAlerta("El monto excede el de crédito permitido para este cliente, no se podrá gestionar el documento.");
            //            return;
            //        }
            //    }
            //}

            if (!FRmConfig.EnConsulta)
            {
                try
                {
                    Gestor.Garantias.CargarConsecutivos();
                }
                catch (Exception ex)
                {
                    if (Pedidos.FacturarPedido)
                        this.mostrarAlerta(ex.Message + " de la garantía.");
                    else
                        this.mostrarAlerta(ex.Message + " del pedido.");

                    return;
                }
            }

            // Si es factura de contado se genera el cobro - KFC
            //if ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.Codigo == "0")
            //if (FRdConfig.ReciboFacturasContado)
            //{
            //    //TODO GARANTIAS
            //    if ((Gestor.Pedidos.Gestionados[0].Configuracion.CondicionPago.DiasNeto == 0) && (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura))
            //        GenerarCobro();
            //}

            //DialogResult res = DialogResult.None;
            this.Imprimir = false;
            if (Gestor.Garantias.Gestionados[0].Tipo == TipoPedido.Factura && Pedidos.DesgloseLotesFactura && false)//de momento las garantías no soportan lotes
            {
                if (!Pedidos.DesgloseLotesFacturaObliga)
                {
                    this.mostrarMensaje(Mensaje.Accion.Decision, "realizar el desglose de lotes para la garantía en proceso", result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            if (Impresora.SugerirImprimir)
                            {
                                string msj = Pedidos.FacturarPedido ? "el detalle de las garantías realizadas" : "el detalle de los pedidos realizados";

                                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de garantías" : "Impresión de Detalle de Pedidos";

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
                        string msj = Pedidos.FacturarPedido ? "el detalle de las garantías realizadas" : "el detalle de los pedidos realizados";

                        string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de garantías" : "Impresión de Detalle de Pedidos";

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

        public void ContinuarContado() 
        {


            //DialogResult res = DialogResult.None;
            this.Imprimir = false;
            if (Gestor.Pedidos.Gestionados[0].Tipo == TipoPedido.Factura && Pedidos.DesgloseLotesFactura)
            {
                if (!Pedidos.DesgloseLotesFacturaObliga)
                {
                    this.mostrarMensaje(Mensaje.Accion.Decision, "realizar el desglose de lotes para la garantía en proceso", result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            if (Impresora.SugerirImprimir)
                            {
                                string msj = Pedidos.FacturarPedido ? "el detalle de las garantías realizadas" : "el detalle de los pedidos realizados";

                                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de garantías" : "Impresión de Detalle de Pedidos";

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
                        string msj = Pedidos.FacturarPedido ? "el detalle de las garantías realizadas" : "el detalle de los pedidos realizados";

                        string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de garantías" : "Impresión de Detalle de Pedidos";

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
                string msj = Pedidos.FacturarPedido ? "el detalle de las garantías realizadas" : "el detalle de los pedidos realizados";

                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Garantías" : "Impresión de Detalle de Pedidos";

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
                        this.GuardaDocumento();
                        this.LimpiarCerrarFormulario();
                    }

                });
            }

            else
            {
                this.GuardaDocumento();
                this.LimpiarCerrarFormulario();
            }
        }

        private void TerminarGestion3()
        {
            if (Impresora.SugerirImprimir)
            {
                string msj = Pedidos.FacturarPedido ? "el detalle de las garantías realizadas" : "el detalle de los pedidos realizados";

                string titulo = Pedidos.FacturarPedido ? "Impresión de Detalle de Garantías" : "Impresión de Detalle de Pedidos";

                
                    if (Imprimir)
                    {
                        ImpresionViewModel.OriginalEn = true;
                        ImpresionViewModel.OnImprimir = ImprimirDocumento;
                        this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = titulo, mostrarCriterioOrden = true });
                    }
                    else
                    {
                        this.GuardaDocumento();
                        this.LimpiarCerrarFormulario();
                    }

                
            }

            else
            {
                this.GuardaDocumento();
                this.LimpiarCerrarFormulario();
            }
        }

        private void LimpiarCerrarFormulario()
        {
            Pedidos.FacturarPedido = false;
            Gestor.Garantias = new Garantias();

            if (FRmConfig.EnConsulta)
            {
                ReturnResult(true);
                DoClose();
            }
            else
            {
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"habilitarPedidos", true}
                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
            }
        }

        private void GuardaDocumento()
        {
            if (FRmConfig.EnConsulta)
            {
                Garantia garantia = null;

                try
                {
                    garantia = Gestor.Garantias.Gestionados[0];
                    garantia.Actualizar(true,"");
                }
                catch (Exception ex)
                {
                    if (garantia.Tipo == TipoPedido.Factura)
                        this.mostrarAlerta("Error al actualizar garantía. " + ex.Message);
                    else
                        this.mostrarAlerta("Error al actualizar pedido. " + ex.Message);
                }
            }
            else
            {
                try
                {
                    
                    Gestor.Garantias.GuardarGarantias("");
                    ActualizarJornada(GlobalUI.RutaActual.Codigo,decimal.Round(Convert.ToDecimal(TotalNeto), 2));
                    
                }
                catch (Exception ex)
                {
                    if (Pedidos.FacturarPedido)
                        this.mostrarAlerta("Error al generar garantía. " + ex.Message);
                    else
                        this.mostrarAlerta("Error al generar pedido. " + ex.Message);
                }
            }
        }

        private void ActualizarJornada(string ruta, decimal monto)
        {
            TipoMoneda moneda = Gestor.Garantias.Gestionados[0].Configuracion.Nivel.Moneda;
            string colCantidad = "";
            string colMonto = "";

            if (Pedidos.FacturarPedido)
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.GARANTIAS_LOCAL;
                    colMonto = JornadaRuta.MONTO_GARANTIAS_LOCAL;                    
                }
                else
                {
                    colCantidad = JornadaRuta.GARANTIAS_DOLAR;
                    colMonto = JornadaRuta.MONTO_GARANTIAS_DOLAR;
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

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                this.mostrarAlerta("Error al actualizar datos. " + ex.Message);
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

                if (Garantias.FacturarGarantia)
                {
                    if (cantidad >= 0 || esOriginal)
                    {
                        if (esOriginal)
                            foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                                garantia.LeyendaOriginal = true;

                        Garantias garantiasImprimir = new Garantias(Gestor.Garantias.Gestionados, GlobalUI.ClienteActual);
                        garantiasImprimir.ImprimeDetalleGarantia(cantidad, (DetalleSort.Ordenador)ordernarPor);

                        if (esOriginal)
                            foreach (Garantia garantia in Gestor.Garantias.Gestionados)
                                garantia.Impreso = true;
                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "Solo se guardará la garantía.");
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
            this.GuardaDocumento();
            //Caso 32081 LDS 09/04/2008
            //Primero guardamos el pedido o factura y luego realizamos la impresión, ya que si ocurre un error con la impresión
            //solo se debe imprimir el documento o no volver a cargar.
            this.LimpiarCerrarFormulario();
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
            if (!ValidaCantidadDeLineas() || !ValidaDireccionDefinida()
                || (Pedidos.CambiarPrecio && !ValidarUtilidad()))
                return;

            if (Gestor.Garantias.Gestionados.Count == 0)
            {
                this.mostrarAlerta("No hay garantías gestionadas");
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
            ventanaInactiva = false;
            //RaisePropertyChanged("PorcDescuento1");            
        }  

        #endregion

    }
}