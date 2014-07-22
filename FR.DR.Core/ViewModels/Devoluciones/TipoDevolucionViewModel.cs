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

using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.UI;

using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using FR.DR.Core.Data.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TipoDevolucionViewModel : ListViewModel
    {
        public TipoDevolucionViewModel()
        {
            TipoDevolucionVisible = true;
            CargaInicial();
        }

        #region Propiedades

        public bool EligiendoPago = false;

        private bool panelPagoVisible;
        public bool PanelPagoVisible
        {
            get { return panelPagoVisible; }
            set { panelPagoVisible = value; RaisePropertyChanged("PanelPagoVisible"); }
        }

        private bool panelUbicacionVisible;
        public bool PanelUbicacionVisible
        {
            get { return panelUbicacionVisible; }
            set { panelUbicacionVisible = value; RaisePropertyChanged("PanelUbicacionVisible"); }
        }


        private bool paisEnabled = true;
        public bool ComboPaisEnabled
        {
            get { return paisEnabled; }
            set { paisEnabled = value; RaisePropertyChanged("ComboPaisEnabled"); }
        }

        List<Pais> listPaises = new List<Pais>();
        List<DivisionGeografica> listDivisiones1 = new List<DivisionGeografica>();
        List<DivisionGeografica> listDivisiones2 = new List<DivisionGeografica>();
        private Pais paisSeleccionado;

        private IObservableCollection<string> paises;
        public IObservableCollection<string> Paises
        {
            get { return paises; }
            set { paises = value; RaisePropertyChanged("Paises"); }
        }


        private string paisActual;
        public string PaisActual
        {
            get { return paisActual; }
            set { paisActual = value; RaisePropertyChanged("PaisActual"); CambioPais(); }
        }

        private IObservableCollection<string> divisiones1;
        public IObservableCollection<string> Divisiones1
        {
            get { return divisiones1; }
            set { divisiones1 = value; RaisePropertyChanged("Divisiones1"); }
        }
        private string div1Actual;
        public string Div1Actual
        {
            get { return div1Actual; }
            set { div1Actual = value; RaisePropertyChanged("Div1Actual"); CambioDiv1(); }
        }

        private IObservableCollection<string> divisiones2;
        public IObservableCollection<string> Divisiones2
        {
            get { return divisiones2; }
            set { divisiones2 = value; RaisePropertyChanged("Divisiones2"); }
        }
        private string div2Actual;
        public string Div2Actual
        {
            get { return div2Actual; }
            set { div2Actual = value; RaisePropertyChanged("Div2Actual"); CambioDiv2(); }
        }

        private string labelDiv1;
        public string LabelDiv1
        {
            get { return labelDiv1; }
            set { labelDiv1 = value; RaisePropertyChanged("LabelDiv1"); }
        }

        private string labelDiv2;
        public string LabelDiv2
        {
            get { return labelDiv2; }
            set { labelDiv2 = value; RaisePropertyChanged("LabelDiv2"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private bool devolverVisible;
        public bool DevolverVisible
        {
            get { return devolverVisible; }
            set { devolverVisible = value; RaisePropertyChanged("DevolverVisible"); }
        }

        private bool devolverEnabled;
        public bool DevolverEnabled
        {
            get { return devolverEnabled; }
            set { devolverEnabled = value; RaisePropertyChanged("DevolverEnabled"); }
        }

        private bool listaEnabled;
        public bool ListaEnabled
        {
            get { return listaEnabled; }
            set { listaEnabled = value; RaisePropertyChanged("ListaEnabled"); }
        }

        private bool conDocumentoChecked;
        public bool ConDocumentoChecked
        {
            get { return conDocumentoChecked; }
            set { conDocumentoChecked = value; CambioRadios(); }
        }

        private IObservableCollection<ClienteCia> companias;
        public IObservableCollection<ClienteCia> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private ClienteCia companiaActual;
        public ClienteCia CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; CargarPantalla(); RaisePropertyChanged("CompaniaActual"); }
        }


        private IObservableCollection<string> tiposDevolucion;
        public IObservableCollection<string> TiposDevolucion
        {
            get { return tiposDevolucion; }
            set { tiposDevolucion = value; RaisePropertyChanged("TiposDevolucion"); }
        }

        private string tipoDevolucionSeleccionado;
        public string TipoDevolucionSeleccionado
        {
            get { return tipoDevolucionSeleccionado; }
            set { tipoDevolucionSeleccionado = value; RaisePropertyChanged("TipoDevolucionSeleccionado"); }
        }

        public bool TipoDevolucionEnabled { get; set; }
        public bool TipoDevolucionVisible { get; set; }

        private IObservableCollection<Pedido> historicoFacturas;
        public IObservableCollection<Pedido> HistoricoFacturas
        {
            get { return historicoFacturas; }
            set { historicoFacturas = value; RaisePropertyChanged("HistoricoFacturas"); }
        }

        private Pedido historicoSeleccionado;
        public Pedido HistoricoSeleccionado
        {
            get { return historicoSeleccionado; }
            set { historicoSeleccionado = value; RaisePropertyChanged("HistoricoSeleccionado"); }
        }

        public string TipoPagoDevolucion { get; set; }
        private Devolucion devolucionActual = new Devolucion();
        public Devolucion DevolucionActual
        {
            get { return devolucionActual; }
            set { devolucionActual = value; RaisePropertyChanged("DevolucionActual"); }
        }

        private bool ingresandoDatos = true;
        public bool IngresandoDatos
        {
            get { return ingresandoDatos; }
            set { ingresandoDatos = value; RaisePropertyChanged("IngresandoDatos"); }
        }

        #endregion

        #region Metodos Logica

        private void CargaInicial()
        {
            PanelPagoVisible = true;
            PanelUbicacionVisible = false;
            IngresandoDatos = true;
            ComboPaisEnabled = FRdConfig.PermiteCambiarPais;
            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());
            CompaniaActual = Companias.Count > 0 ? Companias[0] : null;

            if (Devoluciones.DevolucionAutomatica)
                DevolverVisible = true;

            if (Devoluciones.TiposDevoluciones)
                TipoDevolucion();
            
        }

        private void TipoDevolucion()
        {
            TipoDevolucionVisible = true;
            TipoDevolucionEnabled = true;
            
            TiposDevolucion = new SimpleObservableCollection<string>(Devoluciones.tiposDevolucion);
            TipoDevolucionSeleccionado = TiposDevolucion.Count > 0 ? TiposDevolucion[0] : null;
        }

        private void CargarPantalla()
        {
            this.LlenarComboPaises(CompaniaActual.Compania);

            if (CompaniaActual == null || !ConDocumentoChecked)
                return;

            try
            {
                var lista = Pedido.ObtenerHistoricoFactura(GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona, CompaniaActual.Compania);
                HistoricoFacturas = new SimpleObservableCollection<Pedido>(lista);
            }
            catch (Exception)
            {
                this.mostrarAlerta("Problemas a la hora de cargar la información de las facturas");
            }
        }

        #region Ubicacion

        private void CambioPais()
        {
            if (CompaniaActual != null)
            {
                paisSeleccionado = this.listPaises.Find(x => x.Nombre == PaisActual);
                LabelDiv1 = this.listPaises.Find(x => x.Nombre == PaisActual).LblDivisionGeografica1;
                LabelDiv2 = this.listPaises.Find(x => x.Nombre == PaisActual).LblDivisionGeografica2;
            }
            this.LlenarComboDivisiones1(CompaniaActual.Compania);
        }

        private void CambioDiv1()
        {
            if (CompaniaActual != null)
            {
                if (!Div1Actual.Equals("Ninguna"))
                    paisSeleccionado.DivisionGeografica1 = this.listDivisiones1.Find(x => x.Nombre == Div1Actual).Codigo;
                else
                    paisSeleccionado.DivisionGeografica1 = Div1Actual;

            }
            this.LlenarComboDivisiones2(CompaniaActual.Compania);
        }

        private void CambioDiv2()
        {
            if (CompaniaActual != null)
            {

                if (!Div2Actual.Equals("Ninguna"))
                    paisSeleccionado.DivisionGeografica2 = this.listDivisiones2.Find(x => x.Nombre == Div2Actual).Codigo;
                else
                    paisSeleccionado.DivisionGeografica2 = Div2Actual;


            }
        }

        private void LlenarComboPaises(string compania)
        {
            try
            {
                this.listPaises = Pais.ObtenerPaises(compania);
                Paises = new SimpleObservableCollection<string>(this.listPaises.ConvertAll(x => x.Nombre));
                if (Paises.Count > 0)
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    PaisActual = listPaises.Find(x => x.Codigo == cliente.Pais).Nombre;
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando los paises. " + ex.Message);
            }
        }

        private void LlenarComboDivisiones1(string compania)
        {
            try
            {
                this.listDivisiones1 = Pais.ObtenerDivisionesGeograficas1(compania, this.listPaises.Find(x => x.Nombre == PaisActual).Codigo);
                if (listDivisiones1.Count > 0)
                    Divisiones1 = new SimpleObservableCollection<string>(this.listDivisiones1.ConvertAll(x => x.Nombre));
                else
                    Divisiones1 = new SimpleObservableCollection<string>(new List<string>(new string[] { "Ninguna" }));
                if (Divisiones1.Count > 0)
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    if (!string.IsNullOrEmpty(cliente.DivisionGeografica1) && listDivisiones1.Count > 0)
                        Div1Actual = listDivisiones1.Find(x => x.Codigo == cliente.DivisionGeografica1).Nombre;
                    if (string.IsNullOrEmpty(Div1Actual))
                        Div1Actual = Divisiones1[0];
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando las Divisiones Geográficas 1. " + ex.Message);
            }
        }

        private void LlenarComboDivisiones2(string compania)
        {
            try
            {
                if (listDivisiones1.Count > 0)
                    this.listDivisiones2 = Pais.ObtenerDivisionesGeograficas2(compania, this.listDivisiones1.Find(x => x.Nombre == Div1Actual).Codigo,this.listPaises.Find(x => x.Nombre == PaisActual).Codigo);
                if (listDivisiones2.Count > 0)
                    Divisiones2 = new SimpleObservableCollection<string>(this.listDivisiones2.ConvertAll(x => x.Nombre));
                else
                    Divisiones2 = new SimpleObservableCollection<string>(new List<string>(new string[] { "Ninguna" }));
                if (Divisiones2.Count > 0)
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    if (!string.IsNullOrEmpty(cliente.DivisionGeografica2) && listDivisiones2.Count > 0)
                        Div2Actual = listDivisiones2.Find(x => x.Codigo == cliente.DivisionGeografica2).Nombre;
                    if (string.IsNullOrEmpty(Div2Actual))
                        Div2Actual = Divisiones2[0];
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando las Divisiones Geográficas 2. " + ex.Message);
            }
        }

        #endregion

        private void CambioRadios()
        {
            if (ConDocumentoChecked)
            {
                ListaEnabled = true;
                DevolverEnabled = true;
            }
            else
            {
                HistoricoFacturas = new SimpleObservableCollection<Pedido>();
                HistoricoSeleccionado = null;
                ListaEnabled = false;
                DevolverEnabled = false;
            }

            CargarPantalla();
        }

        public bool GenerarDevolucionAutomatica(Pedido encabezadoPedido)
        {
            try
            {
                encabezadoPedido.Detalles.ObtenerDetallesHistoricoFactura();
                List<DetallePedido> ListaArticulos = encabezadoPedido.Detalles.Lista;
                bool estado = true;
                foreach (DetallePedido det in ListaArticulos)
                {

                    decimal cantAlm = 0;
                    decimal cantDet = 0;

                    if (det.Articulo.UsaLotes)
                    {
                        this.mostrarAlerta("No es posible realizar la devolución automática, la factura presenta al menos un artículo con lotes. Proceder a realizarla manualmente");
                        estado = false;
                        break;
                    }
                    else
                    {

                        this.MostrarCantidadDevolver(encabezadoPedido, det.Articulo.Codigo, ref cantAlm, ref cantDet);

                        if (cantAlm > 0 || cantDet > 0)
                        {
                            //Si la devolucion aun no existe, se genera
                            if (Gestor.Devoluciones.Gestionados.Count == 0)
                            {
                                Gestor.Devoluciones.Gestionados.Add(new Devolucion(det.Articulo, GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual.Compania),
                                    GlobalUI.RutaActual.Codigo, GlobalUI.RutaActual.Bodega));
                                Gestor.Devoluciones.Gestionados[0].NumRef = encabezadoPedido.Numero;
                                Gestor.Devoluciones.Gestionados[0].DevAutomatica = "S";
                                if (Devoluciones.TiposDevoluciones)
                                    Gestor.Devoluciones.Gestionados[0].TipoDevolucion = TipoDevolucionSeleccionado;
                            }

                            //Se procede a agregar el detalle del Inventario.
                            try
                            {
                                Gestor.Devoluciones.Gestionar(det.Articulo,
                                    GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual.Compania),
                                    GlobalUI.RutaActual.Codigo, GlobalUI.RutaActual.Bodega,
                                    Estado.Bueno, "", "", cantDet, cantAlm, encabezadoPedido.Numero, encabezadoPedido.NCF.UltimoValor);

                            }
                            catch (Exception exc)
                            {
                                this.mostrarAlerta(exc.Message);
                                estado = false;
                            }

                        }
                        else
                        {
                            this.mostrarAlerta("La cantidad para el artículo: " + det.Articulo.Codigo + " es 0 o invalida, se detendra la generación de la devolución");
                            estado = false;
                            break;
                        }
                    }
                }
                if (estado)
                {
                    //Se procede a guardar la nueva devolucion
                    try
                    {
                        Gestor.Devoluciones.Guardar();

                        // Modificaciones en funcionalidad de generacion de recibos de contado - KFC
                        this.DevolucionActual = Gestor.Devoluciones.Gestionados[0];
                        AsignarTipoPago();

                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error guardando devolución. " + ex.Message);
                    }
                }

                if (Gestor.Devoluciones.Gestionados.Count == 1)
                    Gestor.Devoluciones.Gestionados.Remove(Gestor.Devoluciones.Gestionados[0]);

                return estado;

            }
            catch (Exception exc)
            {
                this.mostrarAlerta(exc.Message);
                return false;
            }
        }

        private bool MostrarCantidadDevolver(Pedido encabezadoPedido, string articulo, ref decimal cantAlmacen, ref decimal cantDetalle)
        {
            DetallePedido det = encabezadoPedido.Detalles.Buscar(articulo);

            if (det != null)
            {
                if (det.CantidadFacturada >= 0 && det.CantidadFacturada >= det.CantidadDevuelta)
                {
                    decimal cantDevueltaAlmacen = 0;
                    decimal cantDevueltaDetalle = 0;

                    GestorUtilitario.CalcularCantidadBonificada(det.CantidadFacturada, det.Articulo.UnidadEmpaque, ref cantAlmacen, ref cantDetalle);
                    GestorUtilitario.CalcularCantidadBonificada(det.CantidadDevuelta, det.Articulo.UnidadEmpaque, ref cantDevueltaAlmacen, ref cantDevueltaDetalle);

                    cantAlmacen = cantAlmacen - cantDevueltaAlmacen;
                    cantDetalle = cantDetalle - cantDevueltaDetalle;

                    if (cantDetalle < 0)
                    {
                        cantAlmacen = cantAlmacen - 1;
                        cantDetalle = (1 * det.Articulo.UnidadEmpaque) + cantDetalle;
                    }

                }
                else
                {
                    this.mostrarAlerta("La cantidad Facturada es menos de 0");
                    return false;
                }
            }
            return true;

        }

        private void AsignarTipoPago()
        {
            Pedido ped = HistoricoSeleccionado;
            try
            {
                if (Devoluciones.tipoPagoDevolucion == FRmConfig.TipoPagoAmbos)
                {
                    if (ped.CondicionPago.DiasNeto == 0)
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "Seleccione un tipo de pago para la devolución");
                        ElegirTipoPago();
                    }
                    else
                    {
                        ActualizarJornada(GlobalUI.RutaActual.Codigo, Gestor.Devoluciones.Gestionados[0].MontoNeto);
                    }
                }
                else
                {
                    if (ped.CondicionPago.DiasNeto == 0)
                    {
                        this.TipoPagoDevolucion = FRmConfig.TipoPagoEfectivo;
                        GuardarTipoPago();
                    }
                    else
                    {
                        this.TipoPagoDevolucion = FRmConfig.TipoPagoCredito;
                        ActualizarJornada(GlobalUI.RutaActual.Codigo, Gestor.Devoluciones.Gestionados[0].MontoNeto);
                    }
                }
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Devolución generada con éxito");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error asignando tipo de pago. " + ex.Message);
            }
        }

        private void ActualizarJornada(string ruta, decimal monto)
        {
            TipoMoneda moneda = this.DevolucionActual.Configuracion.Nivel.Moneda;
            string colCantidad = "";
            string colMonto = "";

            if (string.IsNullOrEmpty(this.TipoPagoDevolucion) || this.TipoPagoDevolucion.Equals(FRmConfig.TipoPagoCredito))
            {
                if (moneda == TipoMoneda.LOCAL)
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
                if (moneda == TipoMoneda.LOCAL)
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

        private void ElegirTipoPago()
        {
            IngresandoDatos = false;
            panelUbicacionVisible = false;
            PanelPagoVisible = true;
            EligiendoPago = true;
            //this.pnlTipoPago.Show();
            //this.pnlTipoPago.BringToFront();
            //this.lblTipoPagoDev.Visible = true;
            //this.rbtCredito.Checked = true;
            //this.picLogo.BringToFront();
        }

        private void ElegirDireccion() 
        {
            if (IngresandoDatos)
            {
                IngresandoDatos = false;
                PanelUbicacionVisible = true;
            }
            else
            {
                IngresandoDatos = true;
                PanelUbicacionVisible = false;
            }
        }

        public void GuardarTipoPago()
        {
            if (this.TipoPagoDevolucion.Equals(string.Empty))
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "Debe seleccionar un tipo de pago");
                ElegirTipoPago();
            }
            else
            {
                if (this.TipoPagoDevolucion.Equals(FRmConfig.TipoPagoCredito))
                {
                    try
                    {
                        Devoluciones.GenerarNotaCredito(this.DevolucionActual, GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual.Compania).DiasCredito);
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error generando Nota de Crédito. " + ex.Message);
                    }
                }
                else
                {
                    this.DevolucionActual.ActualizarTipoDevolucion(this.DevolucionActual.Numero, FRmConfig.TipoPagoEfectivo);
                }
                EligiendoPago = false;
                ActualizarJornada(GlobalUI.RutaActual.Codigo, this.DevolucionActual.MontoNeto);
                this.DevolucionActual = null;
            }
        }

        #endregion

        #region Comandos

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(GenerarDevolucion); }
        }

        public ICommand ComandoDireccion
        {
            get { return new MvxRelayCommand(ElegirDireccion); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoDevolver
        {
            get { return new MvxRelayCommand(Devolver); }
        }

        public ICommand ComandoGuardar
        {
            get { return new MvxRelayCommand(GuardarTipoPago); }
        }

        #endregion

        #region Acciones

        private void GenerarDevolucion()
        {
            string textoTipoDevolucion = string.Empty;
            if (Devoluciones.TiposDevoluciones)
                textoTipoDevolucion = TipoDevolucionSeleccionado;

            if (!ConDocumentoChecked)
            {
                if (CompaniaActual != null)
                {
                    Compania comp = new Compania(companiaActual.Compania);
                    comp.Cargar();

                    if (comp.UsaNCF)
                    {
                        this.mostrarAlerta("La compañia seleccionada utiliza NCF por lo que unicamente se puede realizar devoluciones con documento.");
                        return;
                    }
                    else
                    {
                        this.RequestNavigate<TomaDevolucionesViewModel>(
                            new Dictionary<string, object>() { { "tipo", textoTipoDevolucion }, { "pais", paisSeleccionado.Codigo }, { "divA", paisSeleccionado.DivisionGeografica1 }, { "divB", paisSeleccionado.DivisionGeografica2 } }
                            );
                    }
                }
            }
            else
            {
                if (HistoricoSeleccionado != null)
                {
                    TomaDevolucionesDocumentosViewModel.EncabezadoPedido = HistoricoSeleccionado;
                    this.RequestNavigate<TomaDevolucionesDocumentosViewModel>(new Dictionary<string, object>() { { "tipo", textoTipoDevolucion },{ "pais", paisSeleccionado.Codigo }, { "divA", paisSeleccionado.DivisionGeografica1 }, { "divB", paisSeleccionado.DivisionGeografica2 } });
                }
                else
                {
                    this.mostrarAlerta("Debe seleccionar un documento para la devolución.");
                    return;
                }
            }
        }

        private void Cancelar()
        {
            if (PanelUbicacionVisible)
            {
                PanelPagoVisible = true;
                PanelUbicacionVisible = false;
                IngresandoDatos = true;
                return;
            }
            if (!EligiendoPago)
            {
                this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de las devoluciones", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                        this.DoClose();
                    }
                });
            }
            else
            {
                this.mostrarAlerta("Debe elegir un tipo de pago y continuar");
            }
        }

        private void Devolver()
        {
            string mensaje = string.Empty;
            if (HistoricoSeleccionado != null)
            {
                // Modificaciones en funcionalidad de generacion de recibos de contado - KFC
                Pedido ped = new Pedido(HistoricoSeleccionado.Numero, CompaniaActual.Compania, GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona, HistoricoSeleccionado.NCF.ValorNCF);
                //Pedido ped = new Pedido(lstvEncPedido.FocusedItem.Text, companiaActual, GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona);

                int tipo = ped.ObtenerSaldoFacturaHistorica();

                switch (tipo)
                {
                    case 0:
                        mensaje = "La factura: " + HistoricoSeleccionado.Numero + " presenta problemas con los saldos, no se recomienda su generación automatica. Desea realizarla";
                        break;
                    case 1:
                        mensaje = "realizar la devolución automatica de la factura: " + HistoricoSeleccionado.Numero + "";
                        break;
                    case 2:
                        mensaje = "La factura: " + HistoricoSeleccionado.Numero + " presenta aplicaciones, por lo que generará un saldo a favor del cliente. Desea realizar la devolución";
                        break;
                    default:
                        mensaje = "realizar la devolución automatica de la factura: " + HistoricoSeleccionado.Numero + "";
                        break;
                }

                this.mostrarMensaje(Mensaje.Accion.Decision, mensaje, result =>{
                    if (result == DialogResult.Yes)
                    {
                        if (!GenerarDevolucionAutomatica(ped))
                            this.mostrarAlerta("Error al generar devolución automatica para la factura: " + HistoricoSeleccionado.Numero);
                        else
                        {
                            this.mostrarAlerta("Devolución generada con éxito", r => { DoClose(); });
                        }
                    }
                });
                

            }
            else
            {
                this.mostrarAlerta("Debe seleccionar un documento para la devolución.");
                return;
            }
        }

        #endregion
    }
}