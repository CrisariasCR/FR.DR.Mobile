using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MenuClienteViewModel : BaseViewModel
    {
        public MenuClienteViewModel(bool habilitarPedidos)
        {
            FRmConfig.EnConsulta = false;
            GlobalUI.ClienteActual.ObtenerClientesCia();
            //HabilitarOpcionPedido = habilitarPedidos;
            HabilitarOpcionPedido = MenuClienteViewModel.habilitarPed;
            CargaInicial();
        }

        #region Propiedades

        public bool HabilitarOpcionPedido { get; set; }
        public static bool habilitarPed;
        private bool consignacionesEnabled = true;
        public bool ConsignacionesEnabled
        {
            get { return consignacionesEnabled; }
            set { consignacionesEnabled = value; RaisePropertyChanged("ConsignacionesEnabled"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private bool devolucionesEnabled = true;
        public bool DevolucionesEnabled
        {
            get { return devolucionesEnabled; }
            set { devolucionesEnabled = value; RaisePropertyChanged("DevolucionesEnabled"); }
        }

        private bool pedidosEnabled = true;
        public bool PedidosEnabled
        {
            get { return pedidosEnabled; }
            set { pedidosEnabled = value; RaisePropertyChanged("PedidosEnabled"); }
        }

        private bool inventariosEnabled = true;
        public bool InventariosEnabled
        {
            get { return inventariosEnabled; }
            set { inventariosEnabled = value; RaisePropertyChanged("InventariosEnabled"); }
        }

        private bool articulosEnabled = true;
        public bool ArticulosEnabled
        {
            get { return articulosEnabled; }
            set { articulosEnabled = value; RaisePropertyChanged("ArticulosEnabled"); }
        }

        public IObservableCollection<OpcionMenu> ItemsAnular { get; set; }
        private OpcionMenu itemAnularSeleccionado;
        public OpcionMenu ItemAnularSeleccionado
        {
            get { return itemAnularSeleccionado; }
            set { itemAnularSeleccionado = value; ItemAnular(); RaisePropertyChanged("ItemAnularSeleccionado"); }
        }

        public IObservableCollection<OpcionMenu> ItemsConsultar { get; set; }
        private OpcionMenu itemConsultarSeleccionado;
        public OpcionMenu ItemConsultarSeleccionado
        {
            get { return itemConsultarSeleccionado; }
            set { itemConsultarSeleccionado = value; ItemConsultar(); RaisePropertyChanged("ItemConsultarSeleccionado"); }
        }

        private bool ingresandoDatos = false;
        public bool IngresandoDatos
        {
            get { return ingresandoDatos; }
            set { ingresandoDatos = value; RaisePropertyChanged("IngresandoDatos"); }
        }

        private bool ingresandoAnular = true;
        public bool IngresandoAnular
        {
            get { return ingresandoAnular; }
            set { ingresandoAnular = value; RaisePropertyChanged("IngresandoAnular"); }
        }

        private bool ingresandoConsultar = true;
        public bool IngresandoConsultar
        {
            get { return ingresandoConsultar; }
            set { ingresandoConsultar = value; RaisePropertyChanged("IngresandoConsultar"); }
        }

        #endregion

        #region Metodos Logica de Negocio

        public void CargaInicial()
        {

            ItemsAnular = new SimpleObservableCollection<OpcionMenu>()
            {
                new OpcionMenu("Facturas"), new OpcionMenu("Pedidos"), new OpcionMenu("Cobros"), new OpcionMenu("Devoluciones"), new OpcionMenu("Consignaciones")
            };

            ItemsConsultar = new SimpleObservableCollection<OpcionMenu>()
            {
                new OpcionMenu("Cliente"),new OpcionMenu("Inventario"),new OpcionMenu("Cobros"),new OpcionMenu("Documentos"),new OpcionMenu("Notas de Crédito"),new OpcionMenu("Pedidos"),new OpcionMenu("Pedidos - Estado"),new OpcionMenu("Facturas"),new OpcionMenu("Devoluciones"),new OpcionMenu("Consignaciones"),new OpcionMenu("Garantías"),new OpcionMenu("Artículos")
            };


            //Bitacora.LogMemoria();

            if (!FRdConfig.UsaFacturacion)
            {
                //Quitamos las opciones de consulta y anulacion de facturas
                ItemsAnular.Remove(new OpcionMenu("Facturas"));
                ItemsConsultar.Remove(new OpcionMenu("Facturas"));
            }

            if (!GlobalUI.ClienteActual.NivelesPreciosDefinidos)
            {
                this.DeshabilitarOpciones();
            }
            else if (!HabilitarOpcionPedido)
            {
                this.DeshabilitarPedidos();
            }

            //Caso: 28088 LDS 02/05/2007
            DevolucionesEnabled = Devoluciones.HabilitarDevoluciones;

            //Si no venta en consignación no se debe permitir realizar la venta en consignación.
             
            if (!FRdConfig.UsaFacturacion || !FRdConfig.UsaConsignacion || !GlobalUI.ClienteActual.UsaConsignacion())
            {
                ItemsConsultar.Remove(new OpcionMenu("Consignaciones"));
                ItemsAnular.Remove(new OpcionMenu("Consignaciones"));
                ConsignacionesEnabled = false;
            }
            else
            {
                if (!FRdConfig.UsaConsignacion || !GlobalUI.ClienteActual.UsaConsignacion())
                {
                    ItemsConsultar.Remove(new OpcionMenu("Consignaciones"));
                    ItemsAnular.Remove(new OpcionMenu("Consignaciones"));
                    ConsignacionesEnabled = false;
                }
                else
                {
                    ConsignacionesEnabled = true;
                }
            }

        }

        private void DeshabilitarPedidos()
        {
            PedidosEnabled = false;
            ItemsConsultar.Remove(new OpcionMenu("Pedidos"));
            ItemsAnular.Remove(new OpcionMenu("Pedidos"));
        }

        private void DeshabilitarOpciones()
        {
            //ArticulosEnabled = false;
            ItemsConsultar.Remove(new OpcionMenu("Artículos"));

            DevolucionesEnabled = false;
            ItemsConsultar.Remove(new OpcionMenu("Devoluciones"));
            ItemsAnular.Remove(new OpcionMenu("Devoluciones"));

            InventariosEnabled = false;
            ItemsConsultar.Remove(new OpcionMenu("Inventario"));

            this.DeshabilitarPedidos();
        }

        private bool PermitirRegistroMultipleDocs(GestorDocumentos.Proceso proceso)
        {
            if (GestorDocumentos.ExisteDocumentoPrevio(proceso, GlobalUI.ClienteActual))
            {
                string msjPrev;
                string msjPost;
                switch (proceso)
                {
                    case GestorDocumentos.Proceso.Devolucion:
                    case GestorDocumentos.Proceso.Consignacion:
                        msjPrev = "Ya existe una ";
                        msjPost = " previamente asociada al cliente.";
                        break;
                    default:
                        msjPrev = "Ya existe un ";
                        msjPost = " previamente asociado al cliente.";
                        break;
                }
                switch (GestorDocumentos.PermiteRegistroMultiplesDocs)
                {
                    case GestorDocumentos.OpcionMultiple.Advertido:
                        this.mostrarMensaje(Mensaje.Accion.Informacion, msjPrev + proceso.ToString().ToLower() + msjPost);
                        return true;
                    case GestorDocumentos.OpcionMultiple.NoPermitido:
                        this.mostrarMensaje(Mensaje.Accion.Alerta, "Ya existe un " + proceso.ToString().ToLower() + " asociado al cliente");
                        return false;
                    default:
                        return true;
                }
            }
            return true;
        }

        private void ValidarVisita()
        {
            bool show = false;
            try
            {
                //Verificamos si el cliente tiene documentos afectados no registrados
                if (DocumentoVisita.ExistenDocumentos(GlobalUI.ClienteActual.Codigo))
                {
                    //this.irASeleccionClientes();
                    show = true;
                }
                else
                {
                    string pregunta;

                    if (GlobalUI.VisitaActual.ValidarVisita(GlobalUI.ClienteActual.Codigo))
                    {
                        pregunta = "registrar otra visita para el cliente";
                    }
                    else
                    {
                        pregunta = "registrar la visita del cliente";
                    }
                    //No se realizo ningun movimiento con el cliente
                    //sin embargo preguntamos si se desea registrar la visita.
                    this.mostrarMensaje(Mensaje.Accion.Decision, pregunta, r =>
                    {
                        if (r == DialogResult.Yes)
                            mostrarVisita();
                        else
                            this.irASeleccionClientes();
                    });
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(ex.Message);
            }
            if (show)
            {
                mostrarVisita();
            }
        }

        private void mostrarVisita()
        {            
            this.RequestDialogNavigate<VisitaViewModel, bool>(null, result =>
            {
                if (result)
                    this.irASeleccionClientes();                
            });
        }

        private void irASeleccionClientes()
        {
            //if (!FRmConfig.NoDetenerGPS)
            //{
            //    Ubicacion.Parar();
            //}
            this.RequestNavigate<SeleccionClienteViewModel>();
            this.DoClose();
        }

        #endregion

        #region Comandos

        public ICommand ComandoDevoluciones
        {
            get { return new MvxRelayCommand(IniciarGestionDevolucion); }
        }

        public ICommand ComandoPedidos
        {
            get { return new MvxRelayCommand(IniciarGestionPedido); }
        }

        public ICommand ComandoGarantias
        {
            get { return new MvxRelayCommand(IniciarGestionGarantia); }
        }

        public ICommand ComandoInventario
        {
            get { return new MvxRelayCommand(IniciarGestionInventario); }
        }

        public ICommand ComandoCobro
        {
            get { return new MvxRelayCommand(IniciarGestionCobro); }
        }

        public ICommand ComandoConsignaciones
        {
            get { return new MvxRelayCommand(IniciarGestionVentaConsignacion); }
        }

        public ICommand ComandoRegresar
        {
            get { return new MvxRelayCommand(Regresar); }
        }

        public ICommand ComandoAnular
        {
            get { return new MvxRelayCommand(MostrarAnular); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(MostrarConsultar); }
        }

        #endregion

        #region Acciones

        private void IniciarGestionDevolucion()
        {
            if (PermitirRegistroMultipleDocs(GestorDocumentos.Proceso.Devolucion))
            {
                //GC.Collect();
                //GC.Collect();
                //Bitacora.Escribir("Se inicia la toma de devolucion.");
                this.RequestNavigate<TipoDevolucionViewModel>();
            }
        }

        private void MostrarAnular() 
        {
            IngresandoAnular = false;
            IngresandoConsultar = true;
            IngresandoDatos = true;            
        }

        private void MostrarConsultar()
        {
            IngresandoAnular = true;
            IngresandoConsultar = false;
            IngresandoDatos = true;
        }

        private void MostrarMenu()
        {
            GC.Collect();
            IngresandoAnular = true;
            IngresandoConsultar = true;
            IngresandoDatos = false;
        }

        public void Resumir() 
        {
            this.MostrarMenu();
        }

        private void IniciarGestionPedido()
        {
            if (PermitirRegistroMultipleDocs(GestorDocumentos.Proceso.Pedido))
            {
                //GC.Collect();
                
                Bitacora.Escribir("Se inicia la toma de pedido.");
                Pedidos.ExisteBodega = GlobalUI.RutaActual.Bodega != FRdConfig.NoDefinido;

                // MejorasGrupoPelon600R6 - KF. //
                if (FRdConfig.UsaSugeridoVenta)
                {
                    if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_AMBOS))
                    {
                        this.RequestNavigate<ConfiguracionPedidoViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_AMBOS } });
                    }
                    else if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_PEDIDO))
                    {
                        this.RequestNavigate<ConfiguracionPedidoViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_PEDIDO } });
                    }
                    else if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_FACTURA))
                    {
                        this.RequestNavigate<ConfiguracionPedidoViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_FACTURA } });
                    }

                    // se llama al nuevo constructor que pasa directamente a la toma de Pedido
                    //pedido = new frmConfiguracionPedido(FRdConfig.UsaSugeridoVenta);
                }
                else
                {
                    if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_AMBOS))
                    {
                        this.RequestNavigate<ConfiguracionPedidoViewModel>();
                    }
                    else if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_PEDIDO))
                    {
                        this.RequestNavigate<ConfiguracionPedidoViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_PEDIDO } });
                    }
                    else if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_FACTURA))
                    {
                        this.RequestNavigate<ConfiguracionPedidoViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_FACTURA } });
                    }
                }


            }
        }

        private void IniciarGestionGarantia()
        {

            //GC.Collect();
            Bitacora.Escribir("Se inicia la toma de garantía.");
            Pedidos.ExisteBodega = GlobalUI.RutaActual.Bodega != FRdConfig.NoDefinido;


            if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_AMBOS))
            {
                this.RequestNavigate<ConfiguracionGarantiaViewModel>();
            }
            else if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_PEDIDO))
            {
                this.RequestNavigate<ConfiguracionGarantiaViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_PEDIDO } });
            }
            else if (Pedidos.DocumentoGenerar.Equals(Pedidos.VALOR_FACTURA))
            {
                this.RequestNavigate<ConfiguracionGarantiaViewModel>(new Dictionary<string, object>() { { "documento", Pedidos.VALOR_FACTURA } });
            }

        }

        private void IniciarGestionInventario()
        {
            //Caso 34431 Registro Multiple de documentos
            if (PermitirRegistroMultipleDocs(GestorDocumentos.Proceso.Inventario))
            {
                //GC.Collect();
                //GC.Collect();
                Bitacora.Escribir("Se inicia la toma de inventario.");
                FRmConfig.EnConsulta = false;
                this.RequestNavigate<TomaInventarioViewModel>();
            }
        }

        private void IniciarGestionCobro()
        {
            /*GC.Collect();
            Bitacora.Escribir("Se inicia la toma de cobro.");
            frmCreacionRecibo cobro =null;
            bool facturasPendientes = false;
            string compania = string.Empty;
            foreach(Cls.FRCliente.ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                compania = clt.Compania;
                //Solo ingresa a la ventana de cobros si existe alguna factura pendiente
                //en cualquiera de las compañias del usuario
                GlobalUI.ClienteActual.CargarNotasCredito(compania);
                GlobalUI.ClienteActual.CargarFacturasCobro(compania);
                if (GlobalUI.ClienteActual.ObtenerClienteCia(compania).FacturasPendientes.Count > 0 ||
                    GlobalUI.ClienteActual.ObtenerClienteCia(compania).NotasCredito.Count > 0)
                {
                    facturasPendientes = true;
                    cobro = new frmCreacionRecibo();
                    cobro.Show();
                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                    this.Close();
                    break;
                }
            }
            if(!facturasPendientes)
                System.Windows.Forms.MessageBox.Show("Este cliente no cuenta con cobros pendientes en ninguna Compañía", Mensaje.titulo, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk, System.Windows.Forms.MessageBoxDefaultButton.Button1);
            */
            //GC.Collect();
            //GC.Collect();
            Bitacora.Escribir("Se inicia la toma de cobro.");
            this.RequestNavigate<CreacionReciboViewModel>();
            DoClose();



        }

        private void Regresar()
        {
            //GC.Collect();
            if (!IngresandoAnular || !IngresandoConsultar)
            {
                this.MostrarMenu();
            }
            else
            {
                try
                {
                    if (GlobalUI.ClienteActual.TieneFacturasContadoPendientes)
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "El cliente tiene aún facturas de contado sin cancelar.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error verificando si hay facturas de contado sin cancelar. " + ex.Message);
                }

                try
                {
                    this.ValidarVisita();
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta(ex.Message);
                }
            }
        }

        private void IniciarGestionVentaConsignacion()
        {
           // GC.Collect();
            try
            {
                if (!VentaConsignacion.ExisteConsignacion(GlobalUI.RutaActual.Codigo, GlobalUI.ClienteActual.Codigo))
                {
                    Bitacora.Escribir("Se inicia la toma de la venta en consignación.");
                    this.RequestNavigate<ConfiguracionVentaConsigViewModel>();
                }
                else
                {
                    if (VentaConsignacion.ExisteConsignacionProcesada(GlobalUI.RutaActual.Codigo, GlobalUI.ClienteActual.Codigo))
                        this.DesgloseBoletaVentaConsignacion(false);
                    else
                    {
                        if (VentaConsignacion.ExisteConsignacionSaldos(GlobalUI.RutaActual.Codigo, GlobalUI.ClienteActual.Codigo))
                            this.DesgloseBoletaVentaConsignacion(true);
                        else
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "La boleta de venta en consignación ya existe.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Ocurrió un error con la gestión de venta en Consignación. " + ex.Message);
            }
        }

        private void DesgloseBoletaVentaConsignacion(bool boletaSaldos)
        {
            Bitacora.Escribir("Se inicia el desglose de la boleta de la venta en consignación.");
            Dictionary<string,object> parametros=new Dictionary<string,object>();
            if(boletaSaldos)
            {
                parametros.Add("desgloseSaldos","S");
            }
            else
            {
                parametros.Add("desgloseSaldos","N");
            }
            parametros.Add("validarCambioPrecios","S");
            this.RequestNavigate<DesgloseVentaConsignacionViewModel>(parametros);
            //frmDesgloseVentaConsignacion desgloseVentaConsignacion = new frmDesgloseVentaConsignacion(boletaSaldos, true);
            //desgloseVentaConsignacion.Show();
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            this.DoClose();
        }


        #endregion

        #region Listboxes de Opciones

        #region Listbox de Consultas

        private void ItemConsultar()
        {
            switch (ItemConsultarSeleccionado.Descripcion)
            {
                case "Cliente": ConsultaClienteViewModel.SeleccionCliente = false; this.RequestNavigate<ConsultaClienteViewModel>(); this.DoClose(); break;
                case "Inventario": ConsultaInventarioViewModel.SeleccionCliente = false; this.RequestNavigate<ConsultaInventarioViewModel>(); this.DoClose(); break;
                case "Cobros": ConsultarCobros(); break;
                case "Documentos": ConsultarDocsCC(TipoDocumento.Factura); break;
                case "Notas de Crédito": ConsultarDocsCC(TipoDocumento.NotaCredito); break;
                case "Pedidos": ConsultaPedido(TipoPedido.Prefactura); break;
                case "Garantías": ConsultaGarantia(); break;
                case "Facturas": ConsultaPedido(TipoPedido.Factura); break;
                case "Devoluciones": ConsultaDevolucion(); break;
                case "Artículos": BusquedaArticulosViewModel.SeleccionCliente = false; this.RequestNavigate<BusquedaArticulosViewModel>(); this.DoClose(); break;
                case "Pedidos - Estado": HistoricoPedidosViewModel.SeleccionCliente = false; this.RequestNavigate<HistoricoPedidosViewModel>(); this.DoClose(); break;
                case "Consignaciones": ConsultarConsignaciones(); break;
            }
        }

        private void ConsultarCobros()
        {
            ConsultaCobroViewModel.SeleccionCliente = false;
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("anulando", false);
            this.RequestNavigate<ConsultaCobroViewModel>(Parametros);
            this.DoClose();
        }

        private void ConsultaDevolucion()
        {
            ConsultaDevolucionesViewModel.SeleccionCliente = false;
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("anular", "N");
            this.RequestNavigate<ConsultaDevolucionesViewModel>(Parametros);
            this.DoClose();
        }

        private void ConsultaPedido(TipoPedido tipo)
        {

            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            if (tipo.Equals(TipoPedido.Factura))
            {
                Parametros.Add("tipoPedido","F");
            }
            else
            {
                Parametros.Add("tipoPedido","P");
            }
            ConsultaPedidosViewModel.SeleccionCliente = false;
            Parametros.Add("anular", false);
            this.RequestNavigate<ConsultaPedidosViewModel>(Parametros);

            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            this.DoClose();
        }

        private void ConsultaGarantia()
        {
            ConsultaGarantiasViewModel.SeleccionCliente = false;
            this.RequestNavigate<ConsultaGarantiasViewModel>();

            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            this.DoClose();
        }

        private void ConsultarDocsCC(TipoDocumento tipo)
        {
            ConsultaDocumentosCCViewModel.SeleccionCliente = false;
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("tipo", (int)tipo);
            this.RequestNavigate<ConsultaDocumentosCCViewModel>(Parametros);
            this.DoClose();
        }

        private void ConsultarConsignaciones()
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("tipoFormulario", Softland.ERP.FR.Mobile.UI.Formulario.ConsultaConsignacion.ToString());
            this.RequestNavigate<ConsultaConsignacionViewModel>(Parametros);
            this.DoClose();
        }

        #endregion

        #region Listbox de Anular

        private void ItemAnular()
        {
            switch (ItemAnularSeleccionado.Descripcion)
            {
                case "Facturas": AnularPedido(TipoPedido.Factura); break;
                case "Pedidos": AnularPedido(TipoPedido.Prefactura); break;
                case "Cobros": AnularCobros(); break;
                case "Devoluciones": AnularDevolucion(); break;
                case "Consignaciones": AnularConsignaciones(); break;
            }
        }

        private void AnularPedido(TipoPedido tipo)
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            if (tipo.Equals(TipoPedido.Factura))
            {
                Parametros.Add("tipoPedido","F");
            }
            else
            {
                Parametros.Add("tipoPedido","P");
            }
            ConsultaPedidosViewModel.SeleccionCliente = false;
            Parametros.Add("anular", true);
            this.RequestNavigate<ConsultaPedidosViewModel>(Parametros);
            this.DoClose();
        }

        private void AnularDevolucion()
        {
            ConsultaDevolucionesViewModel.SeleccionCliente = false;
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("anular", "S");
            this.RequestNavigate<ConsultaDevolucionesViewModel>(Parametros);
            this.DoClose();
        }

        private void AnularConsignaciones()
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("tipoFormulario", Softland.ERP.FR.Mobile.UI.Formulario.AnulacionConsignacion.ToString());
            this.RequestNavigate<ConsultaConsignacionViewModel>(Parametros);
            this.DoClose();
        }

        private void AnularCobros()
        {
            ConsultaCobroViewModel.SeleccionCliente = false;
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("anulando", true);
            this.RequestNavigate<ConsultaCobroViewModel>(Parametros);
            this.DoClose();
        }

        #endregion

        #endregion
    }
}