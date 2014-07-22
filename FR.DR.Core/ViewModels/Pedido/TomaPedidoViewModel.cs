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
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaPedidoViewModel : ListaArticulosViewModel
    {
        public TomaPedidoViewModel(bool pedidoActual)
            : base()
        {
            CargaInicial();
            this.ConsultarPedidoActual = pedidoActual;
        }

        #region Propiedades

        #region Propiedades Binding

        public bool bRaise=false;
        public bool selectAlmacen;
        private bool esDolar = false;
        public bool EsDolar 
        {
            get { return esDolar; }
            set
            {
                esDolar = value;
                RaisePropertyChanged("EsDolar");
            }
        }

        public int ArticuloCount { get { return Articulos.Count; } }

        private new CriterioArticulo criterioActual;
        public override CriterioArticulo CriterioActual
        {
            get { return criterioActual; }
            set
            {
                if (value != criterioActual)
                {
                    criterioActual = value;
                    RaisePropertyChanged("CriterioActual");
                }
            }
        }
        

        private  CriterioArticulo subFamiliaActual;
        public CriterioArticulo SubFamiliaActual
        {
            get { return subFamiliaActual; }
            set
            {
                if (value != subFamiliaActual)
                {
                    subFamiliaActual = value;
                    RaisePropertyChanged("SubFamiliaActual");
                }
            }
        }

        private String companiaActual;
        public String CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                    Refrescar();
            }
        }

        public IObservableCollection<String> Companias { get; set; }

        public bool ConsultarPedidoActual { get; set; }

        private List<string> CodigosFamilias;

        private IObservableCollection<string> familias;
        public IObservableCollection<string> Familias
        {
            get { return familias; }
            set { familias = value; RaisePropertyChanged("Familias"); }
        }
        

        private IObservableCollection<CriterioArticulo> subFamilias;
        public IObservableCollection<CriterioArticulo> SubFamilias
        {
            get { return subFamilias; }
            set { subFamilias = value; RaisePropertyChanged("SubFamilias"); }
        }

        private string familiaActual;
        public string FamiliaActual
        {
            get { return familiaActual; }
            set { familiaActual = value; RaisePropertyChanged("FamiliaActual"); RealizarBusqueda(false); }
        }

        private IObservableCollection<Articulo> articulos;
        public IObservableCollection<Articulo> Articulos
        {
            get { return articulos; }
            set { articulos = value; RaisePropertyChanged("Articulos"); }
        }

        private Articulo articuloSeleccionado;
        public Articulo ArticuloSeleccionado
        {
            get { return articuloSeleccionado; }
            set { articuloSeleccionado = value; RaisePropertyChanged("ArticuloSeleccionado"); MostrarDatosArticulo(); }
        }

        private Articulo articuloSeleccionadoEnvase;
        public Articulo ArticuloSeleccionadoEnvase
        {
            get { return articuloSeleccionadoEnvase; }
            set { articuloSeleccionadoEnvase = value; RaisePropertyChanged("ArticuloSeleccionadoEnvase");}
        }

        private DetallePedido detalleSeleccionado;
        public DetallePedido DetalleSeleccionado
        {
            get { return detalleSeleccionado; }
            set 
            { 
                detalleSeleccionado = value;                
                RaisePropertyChanged("DetalleSeleccionado");
                ActualizarPreciosDetalleSeleccionado();
            }
        }

        private DetalleGarantia detalleGarantiaSeleccionado;
        public DetalleGarantia DetalleGarantiaSeleccionado
        {
            get { return detalleGarantiaSeleccionado; }
            set
            {
                detalleGarantiaSeleccionado = value;
                RaisePropertyChanged("DetalleGarantiaSeleccionado");
            }
        }

        #region Montos y Cantidades

        private decimal precioAlmacen;
        public decimal PrecioAlmacen
        {
            get { return precioAlmacen; }
            set
            {
                precioAlmacen = value; RaisePropertyChanged("PrecioAlmacen"); 
            }
        }

        private decimal precioDetalle;
        public decimal PrecioDetalle
        {
            get { return precioDetalle; }
            set
            {
                precioDetalle = value; RaisePropertyChanged("PrecioDetalle");
            }
        }

        private decimal cantidadAlmacen;
        public decimal CantidadAlmacen
        {
            get { return cantidadAlmacen; }
            set
            {
                if (!calculando)
                {
                    cantidadAlmacen = value; RaisePropertyChanged("CantidadAlmacen"); CambioCantidades();
                }
            }
        }

        private decimal cantidadAlmacenGarantia;
        public decimal CantidadAlmacenGarantia
        {
            get { return cantidadAlmacenGarantia; }
            set
            {
                cantidadAlmacenGarantia = value; RaisePropertyChanged("CantidadAlmacenGarantia"); 
            }
        }

        private decimal cantidadDetalleGarantia;
        public decimal CantidadDetalleGarantia
        {
            get { return cantidadDetalleGarantia; }
            set
            {
                cantidadDetalleGarantia = value; RaisePropertyChanged("CantidadDetalleGarantia");
            }
        }

        private decimal cantidadDetalle;
        public decimal CantidadDetalle
        {
            get { return cantidadDetalle; }
            set
            {
                if (!calculando)
                {
                    cantidadDetalle = value; RaisePropertyChanged("CantidadDetalle"); CambioCantidades();
                }
            }
        }

        private decimal total;
        public decimal Total
        {
            get { return total; }
            set { total = value; RaisePropertyChanged("Total"); }
        }

        private decimal subtotal;
        public decimal Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; RaisePropertyChanged("Subtotal"); }
        }

        private decimal cantidadInventario;
        public decimal CantidadInventario
        {
            get { return cantidadInventario; }
            set { cantidadInventario = value; RaisePropertyChanged("CantidadInventario"); } 
        }

        private decimal cantBonifAlmacen;
        public decimal CantBonifAlmacen
        {
            get { return cantBonifAlmacen; }
            set { bool result = value != cantBonifAlmacen; cantBonifAlmacen = value; if (result) { CambiarBonificacion(); }; RaisePropertyChanged("CantBonifAlmacen"); } 
        }

        private decimal cantBonifDetalle;
        public decimal CantBonifDetalle
        {
            get { return cantBonifDetalle; }
            set { bool result = value != cantBonifDetalle; cantBonifDetalle = value; if (result) { CambiarBonificacion(); }; RaisePropertyChanged("CantBonifDetalle"); } 
        }

        private decimal adicionalesAlmacen;
        public decimal AdicionalesAlmacen
        {
            get { return adicionalesAlmacen; }
            set { adicionalesAlmacen = value; CambiarAdicionalBonificar(); RaisePropertyChanged("AdicionalesAlmacen"); } 
        }

        private decimal adicionalesDetalle;
        public decimal AdicionalesDetalle
        {
            get { return adicionalesDetalle; }
            set { adicionalesDetalle = value; CambiarAdicionalBonificar(); RaisePropertyChanged("AdicionalesDetalle"); } 
        }

        private decimal existencias;
        public decimal Existencias
        {
            get { return existencias; }
            set {  existencias = value; RaisePropertyChanged("Existencias"); } 
        }

        private decimal existenciasEnvase;
        public decimal ExistenciasEnvase
        {
            get { return existenciasEnvase; }
            set { existenciasEnvase = value; RaisePropertyChanged("ExistenciasEnvase"); }
        }


        private decimal totalAlmacenBonif;
        public decimal TotalAlmacenBonif
        {
            get { return totalAlmacenBonif; }
            set { totalAlmacenBonif = value; RaisePropertyChanged("TotalAlmacenBonif"); } 
        }

        private decimal descuentoA;
        public decimal DescuentoA
        {
            get { return descuentoA; }
            set { descuentoA = value; CambiarDescuento(); RaisePropertyChanged("DescuentoA"); } 
        }

        private decimal descuentoB;
        public decimal DescuentoB
        {
            get { return descuentoB; }
            set { descuentoB = value; RaisePropertyChanged("DescuentoB"); } 
        }

        private new string textoBusqueda;
        public new string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { if (textoBusqueda != value && !ventanaInactiva) { textoBusqueda = value; if (!inicial)RaisePropertyChanged("TextoBusqueda"); CambioDeTextoEnBusqueda(); } }
        }

        #endregion

        #endregion

        #region Propiedades Logica de Negocio

        private DetallePedido LineaBonifica;
        private DetallePedido LineaDescuento;        
        public static bool ventanaInactiva = false;
        private bool CargandoInicial;
        private int indiceBonifica;
        private int indiceDescuento;
        public bool EsBarras = false;
        private bool verResolucion;

        public static NivelPrecio NivelPrecio;
        private string detalleTope = string.Empty;

        private Articulo articuloBonificado;

        private bool estadoBonific = false;
        private bool estadoDesc = false;
        private bool cargando = false;
        private bool inicial = false;
        private bool tieneBonificaiones = false;

        private decimal porcentaje = decimal.Zero;

        public bool calculando = false;

        #endregion

        #region Propiedades Elementos Visuales

        public bool DescuentoEnabled { get; set; }
        public bool AdicionalEnabled { get; set; }
        public bool UnidadDetalleEnabled { get; set; }

        private bool subFilFamiliaVisible=false;

        public bool SubFilFamiliaVisible
        {
            set { subFilFamiliaVisible = value; RaisePropertyChanged("SubFilFamiliaVisible"); }
            get { return subFilFamiliaVisible; }
        }

        private bool comboFamiliasVisible;

        public bool ComboFamiliasVisible
        {
            set { comboFamiliasVisible = value; RaisePropertyChanged("ComboFamiliasVisible"); }
            get { return comboFamiliasVisible; }
        }

        public bool textoBusquedaVisible = true;

        public bool TextoBusquedaVisible
        {
            set { textoBusquedaVisible = value; RaisePropertyChanged("TextoBusquedaVisible"); }
            get { return textoBusquedaVisible;  }
        }

        private bool existenciaVisible;
        public bool ExistenciaVisible
        {
            get { return existenciaVisible; }
            set { existenciaVisible = value; RaisePropertyChanged("ExistenciaVisible"); }
        }

        private bool cambiarPrecioVisible;

        public bool CambiarPrecioVisible
        {
            set { cambiarPrecioVisible = value; RaisePropertyChanged("CambiarPrecioVisible"); }
            get { return cambiarPrecioVisible; }
        }

        private bool cambiarBonificacionEnabled;
        public bool CambiarBonificacionEnabled
        {
            set { cambiarBonificacionEnabled = value; RaisePropertyChanged("CambiarBonificacionEnabled"); }
            get { return cambiarBonificacionEnabled; }
        }

        public string NameCliente
        {
            get 
            {
              return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre; 
            }
        }

        private bool cambiarDescuentoVisible;
        public bool CambiarDescuentoVisible
        {
            set { cambiarDescuentoVisible = value; RaisePropertyChanged("CambiarDescuentoVisible"); }
            get { return cambiarDescuentoVisible; }
        }

        private bool montosGarantiaVisible;
        public bool MontosGarantiaVisible
        {
            set { montosGarantiaVisible = value; RaisePropertyChanged("MontosGarantiaVisible"); }
            get { return montosGarantiaVisible; }
        }

        private bool ingresandoDatos = true;
        public bool IngresandoDatos
        {
            get { return ingresandoDatos; }
            set { ingresandoDatos = value; RaisePropertyChanged("IngresandoDatos"); }
        }

        #endregion

        #endregion

        #region Metodos Logica de Negocio

        private void ActualizarPreciosDetalleSeleccionado()
        {
            if (DetalleSeleccionado != null)
            {
                PrecioAlmacen = DetalleSeleccionado.Precio.Maximo;
                PrecioDetalle = DetalleSeleccionado.Precio.Minimo;
            }
        }

        private void LimpiarCambioDescuento()
        {
            this.LineaDescuento = null;
        }

        private void LimpiarCambioBonificacion()
        {
            this.LineaBonifica = null;
            this.indiceBonifica = 0;
        }

        private void RealizarBusqueda(bool agil)
        {
            string variableBusqueda = string.Empty;

            try
            {
                LimpiarCambioBonificacion();

                MostarArtBonificacion(false);

                if (CriterioActual == CriterioArticulo.Familia)
                {
                    agil = true;
                    if (Pedidos.DatoFamiliaMostrar.Equals(0))
                        variableBusqueda = FamiliaActual;
                    else
                        variableBusqueda = CodigosFamilias[Familias.IndexOf(FamiliaActual)];
                }
                else
                    variableBusqueda = TextoBusqueda;


                List<FiltroArticulo> filtros = new List<FiltroArticulo>();
                filtros.Add(FiltroArticulo.GrupoArticulo);

                if (Pedidos.FacturarPedido)
                {
                    filtros.Add(FiltroArticulo.NivelPrecio);
                    filtros.Add(FiltroArticulo.Existencia);
                }

                List<Articulo> resultadoBusqueda = new List<Articulo>();

                if (CriterioActual == CriterioArticulo.Familia && FamiliaActual == "SIN FAMILIA")
                {
                    resultadoBusqueda =
                            Articulo.ObtenerArticulosSinFamilia(
                                filtros.ToArray(),
                                new CriterioBusquedaArticulo(CriterioActual, variableBusqueda, agil),
                                CompaniaActual,
                                GlobalUI.RutaActual.GrupoArticulo,
                                Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual).Nivel.Nivel);
                }
                else
                {
                    resultadoBusqueda =
                             Articulo.ObtenerArticulos(
                                 filtros.ToArray(),
                                 new CriterioBusquedaArticulo(CriterioActual, variableBusqueda, agil),
                                 CompaniaActual,
                                 GlobalUI.RutaActual.GrupoArticulo,
                                 Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual).Nivel.Nivel);
                }


                foreach (Articulo articulo in resultadoBusqueda)
                {
                    DetallePedido detalle = Gestor.Pedidos.BuscarDetalle(articulo);

                    if (detalle != null)
                    {
                        //if (contains(ColumnasPedido.CANTA.ToString()))
                        articulo.CantidadAlmacen = detalle.UnidadesAlmacen;
                        //if (contains(ColumnasPedido.CANTD.ToString()))
                        articulo.CantidadDetalle = detalle.UnidadesDetalle;
                    }

                    //if (contains(ColumnasPedido.PREA.ToString()) || contains(ColumnasPedido.PRED.ToString()))
                    if (FRdConfig.UsaEnvases)
                    {
                        articulo.CargarArticuloEnvase();
                    }
                    this.CargarPrecioArticulo(articulo);
                    
                }

                Articulos = new SimpleObservableCollection<Articulo>(resultadoBusqueda);

                //if (Pedidos.TomaAutomatica)
                //    this.habilitarEMFTextBox(false);
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
            }
        }

        private void CargarPrecioArticulo(Articulo articulo)
        {
            List<NivelPrecio> niveles = NivelPrecio.ObtenerNivelesPrecio(articulo.Compania);

            NivelPrecio nivel = null;
            nivel = niveles.Find(x => x.Nivel == NivelPrecio.Nivel);
            if (nivel != null)
            {
                NivelPrecio = nivel;
                CargarPreciosArticuloContinuacion(articulo);
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No se encontró el nivel de precios:" + NivelPrecio.Nivel +
                    " en la compañía: " + articulo.Compania + ". Se asignará el nivel de precios del cliente.",
                    result =>
                    {
                        ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(articulo.Compania);
                        NivelPrecio = cliente.NivelPrecio;
                        CargarPreciosArticuloContinuacion(articulo);
                    });
            }
        }

        private void CargarPreciosArticuloContinuacion(Articulo articulo)
        {
            ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(articulo.Compania);

            try
            {
                articulo.CargarPrecio(NivelPrecio);
                if (FRdConfig.UsaEnvases && articulo.ArticuloEnvase!=null)
                {
                    articulo.ArticuloEnvase.CargarPrecio(NivelPrecio);
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando precios del artículo. " + ex.Message);
            }
        }

        private void CargarExistenciasArticulo()
        {
            try
            {
                ArticuloSeleccionado.CargarExistenciaPedido(GlobalUI.RutaActual.Bodega);
                Existencias = ArticuloSeleccionado.Bodega.Existencia;
                if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
                {
                    ArticuloSeleccionado.ArticuloEnvase.CargarExistenciaPedido(GlobalUI.RutaActual.Bodega);
                    ExistenciasEnvase = ArticuloSeleccionado.ArticuloEnvase.Bodega.Existencia;
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando las existencias en planta. " + ex.Message);
            }
        }

        public void CargarBonifDesc()
        {
            this.ProcesarBonifcaciones();
            this.ProcesarDescuentos();
        }

        private void ProcesarBonifcaciones()
        {
            cantBonifAlmacen = decimal.Zero;
            cantBonifDetalle = decimal.Zero;
            TotalAlmacenBonif = decimal.Zero;
            Pedido pedido = null;
            ClienteCia cliente = new ClienteCia();

            if (NivelPrecio != null)
            {
                cliente.NivelPrecio = NivelPrecio;
            }
            else
            {
                pedido = Gestor.Pedidos.BuscarNivelPrecio(ArticuloSeleccionado);
                NivelPrecio = pedido.Configuracion.Nivel;
                cliente.NivelPrecio = pedido.Configuracion.Nivel;
            }

            cliente.Codigo = GlobalUI.ClienteActual.Codigo;
            GestorBonificacionDescuento.Bonificacion(cliente, ArticuloSeleccionado,
                CantidadAlmacen.ToString(), CantidadDetalle.ToString(), ref articuloBonificado,
                ref cantBonifAlmacen, ref cantBonifDetalle, ref totalAlmacenBonif);

            if (!Pedidos.CambiarBonificacion)
            {
                this.CantBonifAlmacen = this.cantBonifAlmacen;
                this.CantBonifAlmacen = this.cantBonifAlmacen;
            }
            else
            {
                this.CantBonifAlmacen = decimal.Zero;
                this.CantBonifDetalle = decimal.Zero;
            }
            RaisePropertyChanged("CantBonifAlmacen");
            RaisePropertyChanged("CantBonifDetalle");

            if (articuloBonificado != null)
                MostarArtBonificacion(true);
            else
            {
                this.LimpiarCambioBonificacion();
                MostarArtBonificacion(false);
            }
        }

        private void MostarArtBonificacion(bool ver)
        {
            //TOAST DE MOSTRAR DATOS BONIF
            string mostrar = string.Empty;
            string cantidad = string.Empty;
            if (ver)
            {
                if (articuloBonificado != null)
                {
                    mostrar = articuloBonificado.Codigo + "-" + articuloBonificado.Descripcion;
                    if (mostrar.Length > 34)
                        mostrar = mostrar.Substring(0, 34);

                    mostrar = mostrar + "\n";

                    cantidad = "Cant. Bonifica: " + articuloBonificado.TipoEmpaqueAlmacen + ": " + GestorUtilitario.Formato(this.cantBonifAlmacen) + " " +
                               articuloBonificado.TipoEmpaqueDetalle + ": " + GestorUtilitario.Formato(this.cantBonifDetalle);

                    mostrar = mostrar + cantidad;
                }
            }
            else
                mostrar = string.Empty;

            // this.lblArtBonif.Text = mostrar;
            
            CambiarBonificacionEnabled = ver;
        }

        private void ProcesarDescuentos()
        {
            decimal totalUnidadesAlmancen = decimal.Zero;
            this.porcentaje = decimal.Zero;

            totalUnidadesAlmancen = GestorUtilitario.TotalAlmacen(CantidadAlmacen.ToString(), CantidadDetalle.ToString(), Convert.ToInt32(ArticuloSeleccionado.UnidadEmpaque));

            ClienteCia cliente = new ClienteCia();
            cliente.Codigo = GlobalUI.ClienteActual.Codigo;
            if (NivelPrecio != null)
                cliente.NivelPrecio = NivelPrecio;
            this.porcentaje = GestorBonificacionDescuento.DescuentoMaximo(cliente, ArticuloSeleccionado, CantidadAlmacen.ToString(), CantidadDetalle.ToString());


            if (Pedidos.CambiarDescuento)
            {
                DescuentoB = this.porcentaje;
                DescuentoA = decimal.Zero;
            }
            else
            {
                DescuentoA = this.porcentaje;
                CambiarDescuentoVisible = false;
            }
        }

        private void MostrarDatosArticulo()
        {
            selectAlmacen = true;
            //if (Pedidos.TomaAutomatica)
            //    this.habilitarEMFTextBox(true);

            CargarDetalle();

            cargando = true;

            if (DetalleSeleccionado.LineaBonificada == null)
            {
                CantBonifAlmacen = decimal.Zero;
                CantBonifDetalle = decimal.Zero;
            }
            else
            {
                CantBonifAlmacen = DetalleSeleccionado.LineaBonificada.UnidadesAlmacen;
                CantBonifDetalle = DetalleSeleccionado.LineaBonificada.UnidadesDetalle;
            }

            if (DetalleSeleccionado.Descuento != null)
            {
                DescuentoA = DetalleSeleccionado.Descuento.Monto;
            }
            else
            {
                DescuentoA = decimal.Zero;
            }

            if (DetalleSeleccionado.LineaBonificadaAdicional != null)
            {
                AdicionalesAlmacen = DetalleSeleccionado.LineaBonificadaAdicional.UnidadesAlmacen;
                AdicionalesDetalle = DetalleSeleccionado.LineaBonificadaAdicional.UnidadesDetalle;
            }
            else
            {
                AdicionalesAlmacen = AdicionalesDetalle = decimal.Zero;
            }

            RaisePropertyChanged("DetalleSeleccionado");
            cantidadAlmacen = DetalleSeleccionado.UnidadesAlmacen;
            cantidadDetalle = DetalleSeleccionado.UnidadesDetalle;

            RaisePropertyChanged("CantidadAlmacen");
            RaisePropertyChanged("CantidadDetalle");

            if (FRdConfig.UsaEnvases)
            {
                if (DetalleGarantiaSeleccionado != null)
                {
                    CantidadAlmacenGarantia = DetalleGarantiaSeleccionado.UnidadesAlmacen;
                    CantidadDetalleGarantia = DetalleGarantiaSeleccionado.UnidadesDetalle;
                }
                else
                {
                    CantidadAlmacenGarantia = Decimal.Zero;
                    CantidadDetalleGarantia = Decimal.Zero;
                }
            }

            Subtotal = DetalleSeleccionado.MontoTotal;
            cargando = false;
        }

        private void CargarDetalle()
        {
            if (ArticuloSeleccionado == null)
            {
                DetalleSeleccionado = new DetallePedido();
                if (FRdConfig.UsaEnvases)
                {
                    DetalleGarantiaSeleccionado = new DetalleGarantia();
                }
            }
            else
            {
                DetalleSeleccionado = Gestor.Pedidos.BuscarDetalle(ArticuloSeleccionado);
                if (FRdConfig.UsaEnvases && articuloSeleccionado.ArticuloEnvase != null)
                {
                    DetalleGarantiaSeleccionado = Gestor.Garantias.BuscarDetalle(ArticuloSeleccionado.ArticuloEnvase);
                }
                else
                {
                    DetalleGarantiaSeleccionado = new DetalleGarantia();
                }

                if (DetalleSeleccionado == null)
                {
                    DetalleSeleccionado = new DetallePedido();

                    CargarPrecioArticulo(ArticuloSeleccionado);
                    this.detalleTope = string.Empty;
                    DetalleSeleccionado.Precio = ArticuloSeleccionado.Precio;
                }
                else
                {
                    this.detalleTope = DetalleSeleccionado.Tope;
                    ArticuloSeleccionado.Precio =
                        new Precio(PrecioAlmacen, PrecioDetalle,
                            DetalleSeleccionado.Precio.MargenUtilidad, DetalleSeleccionado.Precio.PorcentajeRecargo);
                }
                this.PrecioAlmacen = DetalleSeleccionado.Precio.Maximo;
                this.PrecioDetalle = DetalleSeleccionado.Precio.Minimo;
                try
                {
                    CantidadInventario = Inventario.CantidadEnInventario(ArticuloSeleccionado, GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona);
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error consultando cantidades inventariadas. " + ex.Message);
                }

                this.CargarExistenciasArticulo();
                this.CargarBonifDesc();
            }
        }

        public void CambioCantidades()
        {
            try
            {
                if (!cargando && ArticuloSeleccionado != null)
                    CalculaSubTotal();
            }
            catch (Exception)
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Verifique los datos ingresados.");
            }
        }

        private void CambiosBonificacionDescuento()
        {
            this.estadoBonific = false;
            this.estadoDesc = false;

            Pedido ped = Gestor.Pedidos.Buscar(ArticuloSeleccionado.Compania);
            GestorBonificacionDescuento gestor = new GestorBonificacionDescuento(ped);

            if (ped == null)
                return;

            if (Pedidos.CambiarBonificacion)
            {
                LineaBonifica = ped.Detalles.BuscarBonificado(ArticuloSeleccionado.Codigo, out this.indiceBonifica);
                this.estadoBonific = LineaBonifica != null;
                if (estadoBonific && LineaBonifica.RegaliaBonificacion == null)
                {
                    this.LimpiarCambioBonificacion();
                    MostarArtBonificacion(false);
                }
                else
                    MostarArtBonificacion(true);
            }

            if (Pedidos.CambiarDescuento)
            {
                LineaDescuento = ped.Detalles.BuscarDescuento(ArticuloSeleccionado.Codigo, out this.indiceDescuento);
                this.estadoDesc = LineaDescuento != null;
                if (!estadoDesc && (LineaDescuento == null || LineaDescuento.RegaliaDescuentoLinea == null))
                    this.LimpiarCambioDescuento();
            }

            //Cambios Descuentos Regalias - KFC
            //if (estadoBonific || estadoDesc)
            //{
            if ((estadoBonific && LineaBonifica != null && LineaBonifica.RegaliaBonificacion == null) ||
                (estadoDesc && LineaDescuento != null && LineaDescuento.RegaliaDescuentoLinea == null && LineaDescuento.Descuento.Monto >= 0))
            {
                DetallePedido detalle = Gestor.Pedidos.BuscarDetalle(ArticuloSeleccionado);

                detalle.Articulo.Precio = new Precio(detalle.Precio.Maximo, detalle.Precio.Minimo, detalle.Precio.MargenUtilidad, detalle.Precio.PorcentajeRecargo);

                gestor.CambiarBonificacionDescuento(detalle, this.indiceBonifica, this.indiceDescuento, this.estadoBonific, this.estadoDesc,
                                                    Gestor.Pedidos.Gestionados[0].Cliente, DescuentoA,
                                                    CantBonifAlmacen, CantBonifDetalle);
                Gestor.Pedidos.SacarMontosTotales();

                if (Pedidos.CalcularImpuestos)
                    Total = Gestor.Pedidos.TotalNeto;
                else
                    Total = Gestor.Pedidos.TotalNetoSinImpuesto;
            }
            else
            {
                this.LimpiarCambioBonificacion();
                this.LimpiarCambioDescuento();
            }
        }

        private void CambiarBonificacion()
        {
            if (!cargando && ArticuloSeleccionado != null)
            {
                decimal cantAlmacen = decimal.Zero;
                decimal cantDetalle = decimal.Zero;
                decimal cantidad = decimal.Zero;

                if (CantBonifAlmacen < 0)
                {
                    this.mostrarAlerta("Solo se permite valores mayores o igual de cero");
                    CantBonifAlmacen = decimal.Zero;
                    return;
                }
                else if (CantBonifDetalle < 0)
                {
                    this.mostrarAlerta("Solo se permite valores mayores o igual de cero");
                    CantBonifDetalle = decimal.Zero;
                    return;
                }
                else
                {
                    if (this.articuloBonificado != null)
                    {
                        GestorUtilitario.CalculaUnidades(CantBonifAlmacen, CantBonifDetalle,
                            Convert.ToInt32(this.articuloBonificado.UnidadEmpaque),
                            out cantAlmacen, out cantDetalle);

                        cantidad = GestorUtilitario.TotalAlmacen(CantBonifAlmacen.ToString(), CantBonifDetalle.ToString(), Convert.ToInt32(this.articuloBonificado.UnidadEmpaque));

                        if (Math.Round(cantidad, 2) <= Math.Round(this.totalAlmacenBonif, 2))
                        {
                            cantBonifAlmacen = cantAlmacen;
                            cantBonifDetalle = cantDetalle;
                            CambiosBonificacionDescuento();
                        }
                        else
                        {
                            this.mostrarAlerta("Se excede la cantidad maxima permitida de bonificaciones");
                            //this.ltbBonfAlm.Text = GestorUtilitario.Formato(this.cantBonifAlmacen);
                            //this.ltbBonifDet.Text = GestorUtilitario.Formato(this.cantBonifDetalle);
                        }
                    }
                    else
                    {
                        CantBonifDetalle = CantBonifAlmacen = decimal.Zero;
                    }
                }
            }
        }

        private void CambiarAdicionalBonificar()
        {
            if (!cargando && ArticuloSeleccionado != null)
            {
                decimal cantAlmacen = decimal.Zero;
                decimal cantDetalle = decimal.Zero;

                GestorUtilitario.CalculaUnidades(AdicionalesAlmacen,
                    AdicionalesDetalle,
                    Convert.ToInt32(ArticuloSeleccionado.UnidadEmpaque),
                    out cantAlmacen,
                    out cantDetalle);

                adicionalesDetalle = cantDetalle;
                adicionalesAlmacen = cantAlmacen;
            }
        }

        private void CambiarDescuento()
        {
            decimal porcentajeRutero = DescuentoA;

            if (Pedidos.CambiarDescuento)
            {
                if (porcentajeRutero >= 0)
                {
                    if (this.porcentaje != 0)
                    {
                        if (porcentajeRutero > this.porcentaje || Pedidos.ManejaTopes)
                        {
                            this.detalleTope = string.Empty;

                            if (Pedidos.ManejaTopes)
                            {
                                if (porcentajeRutero >= (porcentaje + Pedidos.Tope2) && porcentajeRutero <= 100)
                                {
                                    this.detalleTope = "Tope2";
                                    this.mostrarAlerta(Pedidos.MsjTope2);
                                }

                                else if (porcentajeRutero >= (porcentaje + Pedidos.Tope1))
                                {
                                    this.detalleTope = "Tope1";
                                    this.mostrarAlerta(Pedidos.MsjTope1);
                                }
                                else
                                {
                                    if (porcentajeRutero > 100)
                                    {
                                        this.mostrarAlerta("El porcentaje excede los topes permitidos, se colocara el maximo permitido normal");
                                        descuentoA = this.porcentaje;
                                    }
                                }
                            }
                            else
                            {
                                this.mostrarAlerta("Maximo descuento permitido: " + GestorUtilitario.Formato(this.porcentaje));
                                descuentoA = this.porcentaje;
                            }
                            CambiosBonificacionDescuento();
                        }
                        else
                        {
                            CambiosBonificacionDescuento();
                        }
                    }
                    else
                    {
                        if (descuentoA != decimal.Zero)
                            this.mostrarAlerta("Este articulo tiene un porcentaje de descuento : " + GestorUtilitario.Formato(this.porcentaje));
                        descuentoA = decimal.Zero;
                    }
                }
                else
                {
                    this.mostrarAlerta("El porcentaje deben ser valores mayores o igual que cero");
                }
            }
        }

        private void CambioDeTextoEnBusqueda()
        {
            if (TextoBusqueda == string.Empty && CriterioActual != CriterioArticulo.Barras)
                Articulos.Clear();
            else if (TextoBusqueda != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                this.inicial = true;

                //if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                //    TextoBusqueda = TextoBusqueda.Substring(0, TextoBusqueda.Length - 1);

                this.inicial = false;

                CriterioActual = CriterioArticulo.Barras;
                this.RealizarBusqueda(false);

                if (Articulos.Count != 0)
                {
                    this.inicial = true;
                    ArticuloSeleccionado = Articulos[0];
                    CargarPrecioArticulo(ArticuloSeleccionado);
                    CantidadDetalle++;
                    this.AgregarDetalle();
                    EsBarras = true;
                    this.inicial = false;
                    if (verResolucion)
                    {
                        this.ValidarConsecResoluciones();
                    }
                }
            }
            else
            {
                RaisePropertyChanged("TextoBusqueda");
                
            }
        }

        private void CalculaSubTotal()
        {
            calculando = true; //Evita que este metodo se llame por calculos, solo por interacción del usuario

            decimal precioAlm = 0;
            decimal precioDet = 0;
            decimal precioAlmGarantia = 0;
            decimal precioDetGarantia = 0;
            decimal subtot = 0;
            decimal cantDetalle = 0;
            decimal cantAlmacen = 0;
            decimal cantGarantia = CantidadAlmacenGarantia;

            GestorUtilitario.CalculaUnidades(CantidadAlmacen, CantidadDetalle, Convert.ToInt32(ArticuloSeleccionado.UnidadEmpaque),
                out cantAlmacen, out cantDetalle);

            ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual);

            if (cliente != null && cliente.AceptaFracciones && ArticuloSeleccionado.FactorVenta > 0)
            {
                GestorUtilitario.CalcularUnidadesMultipoVenta(CantidadAlmacen, CantidadDetalle
                    , Convert.ToInt32(ArticuloSeleccionado.UnidadEmpaque), ArticuloSeleccionado.FactorVenta
                    , ref cantAlmacen, ref cantDetalle);
            }

            if (NivelPrecio.Impuesto1Incluido == Impuesto1Incluido.SI)
            {
                precioAlm = ArticuloSeleccionado.Precio.Maximo;
                precioDet = ArticuloSeleccionado.Precio.Minimo;
            }
            else
            {
                precioAlm = this.PrecioAlmacen;
                precioDet = this.PrecioDetalle;
            }

            subtot = (precioAlm * cantAlmacen) + (precioDet * cantDetalle);

            
            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
            {
               precioAlmGarantia = ArticuloSeleccionado.ArticuloEnvase.Precio.Maximo;
               precioDetGarantia = ArticuloSeleccionado.ArticuloEnvase.Precio.Minimo;
               subtot += (precioAlmGarantia * cantGarantia);
            }

            CantidadAlmacen = cantAlmacen;
            CantidadDetalle = cantDetalle;
            Subtotal = subtot;
            this.CargarBonifDesc();
            calculando = false;
        }

        private void GestionarPaquetesReglas(bool allDetails)
        {
            Pedido pedido = Gestor.Pedidos.Buscar(CompaniaActual);
            DetallePedido detalle = allDetails ? null : pedido.Detalles.Buscar(ArticuloSeleccionado.Codigo);
            String mensaje = String.Empty;
            bool resultado = Regalias.AplicarRegalias(ref pedido, ref detalle, NivelPrecio.Nivel, GlobalUI.RutaActual.Codigo, out mensaje);
            tieneBonificaiones = Regalias.TieneBonificaciones;
            if (!resultado)
            {
                this.mostrarAlerta(String.Format("No se puedieron aplicar los paquete y reglas de descuentos-bonificación.Error:{0}.", mensaje));
            }
            Gestor.Pedidos.SacarMontosTotales();
            MostrarDatosArticulo();
        }

        #endregion

        #region Metodos

        public void MostrarPedidoActual()
        {
            Pedido pedido = Gestor.Pedidos.Buscar(CompaniaActual);

            if (pedido != null)
            {
                Articulos = new SimpleObservableCollection<Articulo>(pedido.Detalles.Lista.Select(x => x.Articulo).ToList());
            }
        }

        public void CambioComboCriterios(CriterioArticulo Criterio)
        {
            //if (CriterioActual != Criterio)
            //{
                CriterioActual = Criterio;
                RaisePropertyChanged("ComboFamiliasVisible");
                RaisePropertyChanged("TextoBusquedaVisible");
                if (CriterioActual == CriterioArticulo.Familia)
                {
                    CargarFamilias();
                    this.inicializarSubFiltroFamilia(false);
                }
                else if (CriterioActual == (CriterioArticulo)Criterios[Criterios.Count - 1])
                {
                    MostrarPedidoActual();                   
                }
                else
                    this.inicializarSubFiltroFamilia(true);
            //}
        }

        private void inicializarSubFiltroFamilia(bool inicio)
        {
            if (!inicio && Pedidos.SubFiltroFamilia)
            {
                //Se comenta por si se quiere implementar en un futuro
                //ComboFamiliasVisible = false;
                //TextoBusquedaVisible = true;
                //SubFilFamiliaVisible = true;
                //TextoBusqueda = string.Empty;
                ////Recarga la lista
                //this.RealizarBusqueda(false);
                ComboFamiliasVisible = true;
                TextoBusquedaVisible = false;
                SubFilFamiliaVisible = false;
                TextoBusqueda = string.Empty;
                //Recarga la lista
                this.RealizarBusqueda(false);
            }
            else
            {
                if (!inicio)
                {
                    ComboFamiliasVisible = true;
                    TextoBusquedaVisible = false;
                    SubFilFamiliaVisible = false;
                    TextoBusqueda = string.Empty;                    
                    //Recarga la lista
                    this.RealizarBusqueda(false);
                }
                else
                {

                    ComboFamiliasVisible = false;
                    SubFilFamiliaVisible = false;
                    TextoBusquedaVisible = true;       
                }
            }
        }

        private void CargarFamilias()
        {
            List<Articulo> familias = Articulo.CargarFamilias();
            List<string> lista = null;            
            CodigosFamilias = familias.Select(x => x.Familia).ToList();
            switch (Pedidos.DatoFamiliaMostrar)
            {
                case 0:
                    lista = CodigosFamilias;
                    break;
                case 1:
                    lista = familias.Select(x => x.FamiliaDesc).ToList(); break;
                case 2:
                    lista = familias.Select(x => x.FamiliaDesc + "-" + x.FamiliaDesc).ToList(); break;
            }

            Familias = new SimpleObservableCollection<string>(lista);
            FamiliaActual = Familias.Count > 0 ? Familias[0] : null;

            //Carga el combo SubFamilias
            if (Pedidos.SubFiltroFamilia)
            {
                SubFamilias = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Descripcion
                    };
                SubFamiliaActual = SubFamilias[Pedidos.DatoFamiliaMostrar];
            }
        }

        private void LlenarCompanias()
        {
            if (FRmConfig.EnConsulta)
                Companias = new SimpleObservableCollection<string>(Gestor.Pedidos.Gestionados.Select(item => item.Compania).ToList());
            else
                Companias = new SimpleObservableCollection<string>(Util.CargarCiasClienteActual().Select(item => item.Compania).ToList());

            CompaniaActual = Companias.Count > 0 ? Companias[0] : null;
        }

        private void CargaInicial()
        {
            ventanaInactiva = false;
            CargandoInicial = true;
            cargando = true;
            //this.FormatTextBox();
            cargando = false;

            Criterios.Add(Pedidos.FacturarPedido ? CriterioArticulo.FacturaActual : CriterioArticulo.PedidoActual);

            ExistenciaVisible = true;
            if (!Pedidos.FacturarPedido && !Pedidos.MostrarRefExistencias)
                ExistenciaVisible = false;
            Gestor.Pedidos.SacarMontosTotales();

            if (Pedidos.CalcularImpuestos)
                Total = Gestor.Pedidos.TotalNeto;
            else
                Total = Gestor.Pedidos.TotalNetoSinImpuesto;

            if (FRdConfig.UsaEnvases && Pedidos.FacturarPedido)
            {
                Gestor.Garantias.SacarMontosTotales();
                MontosGarantiaVisible = true;
            }
            else
            { 
                MontosGarantiaVisible = false;
            }

            LlenarCompanias();

            if (this.ConsultarPedidoActual)
                CriterioActual = Pedidos.FacturarPedido ? CriterioArticulo.FacturaActual : CriterioArticulo.PedidoActual;
            else
                CriterioActual = Pedidos.CriterioBusquedaDefaultBD;

            CambiarPrecioVisible = Pedidos.CambiarPrecio;
            DescuentoEnabled = Pedidos.CambiarDescuento || Pedidos.ManejaTopes;
            CambiarDescuentoVisible = Pedidos.CambiarDescuento;

            CambiarBonificacionEnabled = Pedidos.CambiarBonificacion;
            AdicionalEnabled = Pedidos.BonificacionAdicional && !Pedidos.FacturarPedido;

            //if (Pedidos.TomaAutomatica)
            //    this.cargarEMFTextBox();
            //else
            //{
            //    this.ZAutomatizarTextBox = false;
            //    deshabilitarAutomatico(Pedidos.TomaAutomatica);
            //}

            ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual);
            EsDolar = cliente.Moneda == TipoMoneda.DOLAR;
            UnidadDetalleEnabled = cliente != null && !cliente.AceptaFracciones;
            CargandoInicial = false;

        }

        #endregion

        #region Comandos

        public ICommand ComandoCambiarPrecio
        {
            get { return new MvxRelayCommand(CambiarPrecio); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarPedido); }
        }

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(AgregarDetalleComando); }
        }

        public ICommand ComandoRemover
        {
            get { return new MvxRelayCommand(RetirarDetalle); }
        }

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(CancelarToma); }
        }

        public ICommand ComandoContinuarCantidades
        {
            get { return new MvxRelayCommand(MostrarCantidades); }
        }

        public ICommand ComandoRLista
        {
            get { return new MvxRelayCommand(MostrarCantidades); }
        }

        #endregion

        #region Acciones

        public override void Refrescar()
        {
            if (CriterioActual != CriterioArticulo.Familia)
            {
                if (!CargandoInicial)
                {
                    if (CriterioActual == CriterioArticulo.VentaActual || CriterioActual == CriterioArticulo.PedidoActual ||
                        CriterioActual == CriterioArticulo.FacturaActual)
                    {
                        this.MostrarPedidoActual();
                    }
                    else
                    {
                        this.RealizarBusqueda(false);
                    }
                    
                }
            }
            else
            {
                // No se decidió implementar ya que no filtraba, cuando es familia, era simplemente hacer scroll
            }
        }

        private void ConsultarPedido()
        {
            if (Gestor.Pedidos.ExistenArticulosGestionados())
            {
                Dictionary<string, object> Parametros = new Dictionary<string, object>();
                Parametros.Add("invocadesde", Formulario.TomaPedido.ToString());
                Parametros.Add("mostrarControles", "S");
                if(Pedidos.FacturarPedido)
                {
                    Parametros.Add("esFactura", "S");
                }
                else
                {
                    Parametros.Add("esFactura", "N");
                }
                this.RequestNavigate<DetallePedidoViewModel>(Parametros);

                //this.RequestDialogNavigate<DetallePedidoViewModel, DialogResult>(Parametros, result =>
                //    {
                //        Gestor.Pedidos.SacarMontosTotales();

                //        if (Pedidos.CalcularImpuestos)
                //            Total = Gestor.Pedidos.TotalNeto;
                //        else
                //            Total = Gestor.Pedidos.TotalNetoSinImpuesto;
                //    });
            }
            else
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay detalles incluídos");

        }

        /// <summary>
        /// Valida el consecutivo de resolución
        /// </summary>
        private void ValidarConsecResoluciones()
        {
            //Verifica lo del consecutivo de resoluciones
            if (Gestor.Pedidos.Gestionados[0].ConsecResolucion != null)
            {
                string error = string.Empty;
                string advertencia = string.Empty;
                if (!Gestor.Pedidos.Gestionados[0].ConsecResolucion.ValidarConsecutivoResolucion(ref error, ref advertencia))
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        this.CancelarTomaResolucion(error);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(advertencia))
                        {
                            this.CancelarTomaResolucion(advertencia);
                        }
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(advertencia))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, advertencia);
                    }
                }
            }
        }


        private void AgregarDetalleComando() 
        {
            this.AgregarDetalle();
            if (verResolucion)
            {
                this.ValidarConsecResoluciones();
            }
        }

        private void AgregarDetalle()
        {
            
            
                bool agil = false;
                decimal unidadesAlm = decimal.Zero; 
                decimal unidadesDet = decimal.Zero;
                decimal unidadesAlmGarantia = decimal.Zero;
                decimal unidadesDetGarantia = decimal.Zero;
                decimal unidadesAlmAdicional = decimal.Zero; 
                decimal unidadesDetAdicional = decimal.Zero; 
                decimal unidadesAlmBonif = decimal.Zero; 
                decimal unidadesDetBonif = decimal.Zero; 
                decimal totalAlmBonificado = decimal.Zero; 

                if (ArticuloSeleccionado != null)
                {

                    unidadesAlm = CantidadAlmacen;
                    unidadesDet = CantidadDetalle;
                    unidadesAlmGarantia = CantidadAlmacenGarantia;
                    unidadesDetGarantia = CantidadDetalleGarantia;

                    unidadesAlmBonif = CantBonifAlmacen;
                    unidadesDetBonif = CantBonifDetalle;
                    totalAlmBonificado = unidadesAlmBonif + (unidadesDetBonif / ArticuloSeleccionado.UnidadEmpaque);

                    if (Pedidos.BonificacionAdicional && !Pedidos.FacturarPedido)
                    {
                        unidadesAlmAdicional = AdicionalesAlmacen;
                        unidadesDetAdicional = AdicionalesDetalle;
                    }
                }

                if (FRdConfig.UsaEnvases&&(unidadesAlm + totalAlmacenBonif + unidadesAlmAdicional) < unidadesAlmGarantia)
                {
                    this.mostrarAlerta("Los envases en garantia no pueden ser mayores al total de articulos en el detalle.");
                    return;
                }
                

                if (ArticuloSeleccionado == null)
                {
                    if (TextoBusqueda != string.Empty && (unidadesAlm > 0 || unidadesDet > 0))
                    {
                        this.RealizarBusqueda(true);
                        if (Articulos.Count == 1)
                        {
                            ArticuloSeleccionado = Articulos[0];
                            CargarPrecioArticulo(ArticuloSeleccionado);

                            CalculaSubTotal();

                            unidadesAlm = CantidadAlmacen;
                            unidadesDet = CantidadDetalle;

                            this.CargarExistenciasArticulo();

                            agil = true;
                        }

                        else
                            return;
                    }
                    else
                    {
                        this.mostrarAlerta("Debe seleccionar un articulo.");
                        return;
                    }
                }



                if (ArticuloSeleccionado.Precio.Maximo == decimal.Zero &&
                    ArticuloSeleccionado.Precio.Minimo == decimal.Zero
                    || (FRdConfig.UsaEnvases && ArticuloSeleccionado.Precio.Maximo == decimal.Zero &&
                    ArticuloSeleccionado.Precio.Minimo == decimal.Zero && ArticuloSeleccionado.ArticuloEnvase.Precio.Maximo == decimal.Zero))
                {
                    if (Pedidos.FacturarPedido)
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "No se puede agregar el artículo porque no se encuentra en el Nivel de Precios seleccionado.");
                        return;
                    }
                    else
                    {
                        if (Pedidos.ArtFueraNivPrecio)
                        {
                            this.mostrarMensaje(Mensaje.Accion.Decision, "Desea agregar un artículo fuera del nivel de precios",
                                result =>
                                {
                                    if (result == DialogResult.Yes)
                                    {
                                        this.mostrarMensaje(Mensaje.Accion.Informacion, "El artículo se agregará sin precio, por no estar definido en el Nivel de Precios seleccionado.");
                                    }
                                    else
                                    {
                                        this.mostrarMensaje(Mensaje.Accion.Informacion, "No se agregó el artículo!");
                                        return;
                                    }
                                });
                        }
                        else
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "No se puede agregar el artículo porque no se encuentra en el Nivel de Precios seleccionado.");
                            return;
                        }
                    }
                }
                try
                {
                    if (unidadesAlm > 0 || unidadesDet > 0)
                    {
                        if (Pedidos.FacturarPedido)
                        {
                            decimal existencias = Existencias;
                            decimal existenciasEnvase = ExistenciasEnvase;
                            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
                            {
                                existenciasEnvase -= CantidadAlmacenGarantia;
                            }

                            if ((Gestor.Pedidos.Gestionados.Count > 0) && (Gestor.Pedidos.Gestionados[0].Detalles.Lista.Count > 0))
                            {
                                foreach (DetallePedido detalle in Gestor.Pedidos.Gestionados[0].Detalles.Lista)
                                {
                                    if ((detalle.LineaBonificada != null) && (detalle.LineaBonificada.Articulo.Codigo == ArticuloSeleccionado.Codigo))
                                    {
                                        existencias -= detalle.TotalBonificado;
                                        if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
                                        {
                                            existenciasEnvase += detalle.TotalBonificado;
                                        }
                                    }
                                }
                            }

                            decimal cantidadPedida = unidadesAlm + (unidadesDet / ArticuloSeleccionado.UnidadEmpaque);                                                        
                            if (existencias < cantidadPedida)
                            {
                                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay suficientes existencias.");
                                return;
                            }
                            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
                            {
                                decimal cantidadPedidaGarantias = unidadesAlmGarantia + (unidadesDetGarantia / ArticuloSeleccionado.ArticuloEnvase.UnidadEmpaque);
                                if ((existenciasEnvase + cantidadPedida) < cantidadPedidaGarantias)
                                {
                                    this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay suficientes existencias de envases para dar en garantía.");
                                    return;
                                }
                            }

                            if ((this.articuloBonificado == null) || (ArticuloSeleccionado.Codigo == this.articuloBonificado.Codigo))
                            {
                                cantidadPedida += totalAlmBonificado;
                                if (existencias < cantidadPedida)
                                {
                                    this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay suficientes existencias.");
                                    return;
                                }
                            }
                            else
                            {
                                this.articuloBonificado.CargarExistenciaPedido(GlobalUI.RutaActual.Bodega);
                                existencias = this.articuloBonificado.Bodega.Existencia;

                                if ((Gestor.Pedidos.Gestionados.Count > 0) && (Gestor.Pedidos.Gestionados[0].Detalles.Lista.Count > 0))
                                {
                                    foreach (DetallePedido detalle in Gestor.Pedidos.Gestionados[0].Detalles.Lista)
                                    {
                                        if ((detalle.Articulo != null) && (detalle.Articulo.Codigo == this.articuloBonificado.Codigo))
                                        {
                                            existencias -= detalle.TotalAlmacen;
                                            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
                                            {
                                                existenciasEnvase += detalle.TotalAlmacen;
                                            }
                                        }
                                    }
                                }

                                if (existencias < totalAlmBonificado)
                                {
                                    this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay suficientes existencias del artículo a bonificar.");
                                    return;
                                }
                            }
                        }

                        decimal precioMax = PrecioAlmacen;
                        decimal precioMin = PrecioDetalle;

                        try
                        {
                            verResolucion = Gestor.Pedidos.Gestionados.Count == 0; 
                            Gestor.Pedidos.Gestionar(ArticuloSeleccionado, GlobalUI.RutaActual.Codigo, new Precio(precioMax, precioMin), unidadesAlm, unidadesDet, unidadesAlmAdicional, unidadesDetAdicional, true, this.detalleTope);
                            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null && (unidadesAlmGarantia > 0 || unidadesDetGarantia>0))
                            {
                                Gestor.Garantias.Gestionar(ArticuloSeleccionado.ArticuloEnvase, GlobalUI.RutaActual.Codigo, ArticuloSeleccionado.ArticuloEnvase.Precio, unidadesAlmGarantia, unidadesDetGarantia, 0, 0, true, this.detalleTope);
                            }

                            #region Cambios Descuentos Regalias - KFC
                            /* LAS. Aplica los descuentos y bonificaciones de forma normal, cuando no utiliza el esqeuma de paquetes y reglas.
                            if (FRdConfig.EsquemaDescuento != EsquemaDescuento.PaquetesReglas)
                            {
                                this.cambiosBonificacionDescuento();
                            }*/

                            #endregion

                            //Cambios Motor Precios - KFC
                            GestionarPaquetesReglas(false);

                            this.CambiosBonificacionDescuento();                            

                            //if (ArticuloSeleccionado != null)
                            //    this.MostarCantidad(this.lstArticulos.SelectedIndices[0], unidadesAlm, unidadesDet);
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta(ex.Message);
                        }

                        if (Pedidos.CalcularImpuestos)
                            Total = Gestor.Pedidos.TotalNeto;
                        else
                            Total = Gestor.Pedidos.TotalNetoSinImpuesto;
                    }
                    else
                    {
                        this.mostrarAlerta("Ambas cantidades no pueden ser cero.");
                    }
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error al agregar línea. " + ex.Message);
                }

                if (agil)
                {
                    ArticuloSeleccionado = null;
                    agil = false;
                }
                ArticuloSeleccionado.CantidadAlmacen = unidadesAlm;
                ArticuloSeleccionado.CantidadDetalle = unidadesDet;
            RaisePropertyChanged("Articulos");
        }

        private void RetirarDetalle()
        {
            if (ArticuloSeleccionado == null) 
            {
                this.mostrarAlerta("Debe Seleccionar un articulo");
                return;
                
            }

            DetalleSeleccionado = Gestor.Pedidos.BuscarDetalle(ArticuloSeleccionado);
            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
            {
                DetalleGarantiaSeleccionado = Gestor.Garantias.BuscarDetalle(ArticuloSeleccionado.ArticuloEnvase);
            }

            if (DetalleSeleccionado == null)
            {
                this.mostrarAlerta("El artículo no ha sido ingresado al pedido, por esto no puede ser eliminado.");
                return;
            }

            this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            this.LimpiarCambioBonificacion();
                            this.LimpiarCambioDescuento();
                            Gestor.Pedidos.EliminarDetalle(ArticuloSeleccionado);
                            if (FRdConfig.UsaEnvases && ArticuloSeleccionado.ArticuloEnvase != null)
                            {
                                Gestor.Garantias.EliminarDetalle(ArticuloSeleccionado.ArticuloEnvase);
                            }
                            if (Pedidos.CalcularImpuestos)
                                Total = Gestor.Pedidos.TotalNeto;
                            else
                                Total = Gestor.Pedidos.TotalNetoSinImpuesto;

                            MostarArtBonificacion(false);
                            ArticuloSeleccionado = null;
                        }
                        catch (Exception exc)
                        {
                            this.mostrarAlerta("Error al gestionar el pedido. " + exc.Message);
                        }
                    }

                    if (CriterioActual == (CriterioArticulo)Criterios[Criterios.Count - 1])
                    {
                        MostrarPedidoActual();
                    }
                    else
                    {
                        Refrescar();
                    }
                });
        }

        private void CambiarPrecio()
        {
            if (ArticuloSeleccionado != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(ArticuloSeleccionado.Compania);
                DetallePedido detalle = Gestor.Pedidos.BuscarDetalle(ArticuloSeleccionado);

                if (detalle == null)
                {
                    detalle = new DetallePedido();
                    detalle.Articulo = ArticuloSeleccionado;
                    detalle.Precio = ArticuloSeleccionado.Precio;
                }
                CambioPrecioViewModel.Detalle = detalle;
                CambioPrecioViewModel.Lista = config.Nivel;
                //frmCambioPrecio cambiarprecio = new frmCambioPrecio(detalle, config.Nivel);                
                this.RequestDialogNavigate<CambioPrecioViewModel, Dictionary<string, object>>(null, result =>
                    {                        
                        bool correcto = (bool)result["correcto"];
                        if (correcto)
                        {
                            PrecioAlmacen = (decimal)result["precioAlmacen"];
                            PrecioDetalle = (decimal)result["precioDetalle"];
                            //Cambia el precio del articulo seleccionado
                            ArticuloSeleccionado.PrecioMaximo = PrecioAlmacen;
                            ArticuloSeleccionado.PrecioMinimo = PrecioAlmacen;
                        }
                        CalculaSubTotal();
                    });
            }
            else
            {
                this.mostrarAlerta("Debe seleccionar un artículo");
            }
        }

        public void OnResume()
        {
            if (Gestor.Pedidos.Gestionados.Count > 0)
            {
                Gestor.Pedidos.Gestionados[0].inTotalRetenciones = 0;
            }
            Gestor.Pedidos.SacarMontosTotales();

            if (Pedidos.CalcularImpuestos)
                Total = Gestor.Pedidos.TotalNeto;
            else
                Total = Gestor.Pedidos.TotalNetoSinImpuesto;
            ventanaInactiva = false;
            RaisePropertyChanged("TextoBusqueda");            
        }                

        private void Continuar()
        {
            if (Gestor.Pedidos.Gestionados.Count > 0)
            {
                ///* KFC Se deja de utilizar el parametro de Esquemas Descuento y se programa precedencia de aplicaciones
                
                    Pedido pedido = Gestor.Pedidos.Buscar(CompaniaActual);
                    GestionarPaquetesReglas(true);
                    if (tieneBonificaiones)
                    {
                        //DetalleBonificacionesForm bonificaciones = new DetalleBonificacionesForm(pedido, nivelPrecio.Nivel, GlobalUI.RutaActual.Codigo);
                        Dictionary<string, object> Parametros = new Dictionary<string, object>();
                        Parametros.Add("nivelPrecio", NivelPrecio.Nivel);
                        Parametros.Add("ruta", GlobalUI.RutaActual.Codigo);
                        DetalleBonificacionesViewModel.Pedido = pedido;
                        ventanaInactiva = true;
                        this.RequestDialogNavigate<DetalleBonificacionesViewModel, Dictionary<string, object>>(Parametros, result =>
                        {
                            bool continuar = (bool)result["Aplicar"];

                            if (continuar)
                            {
                                pedido = (Pedido)result["Pedido"]; //Revisar si tiene logica
                                this.RequestNavigate<AplicarPedidoViewModel>();
                            }
                        }
                       ); 
                    }
                    else
                    {
                        Navegar();
                        //this.RequestNavigate<AplicarPedidoViewModel>();
                    }

               

            }
            else
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay pedidos gestionados aún.");
        }

        private void Navegar()
        {
            this.RequestDialogNavigate<AplicarPedidoViewModel, DialogResult>(null, result =>
            {

                if (result == DialogResult.Yes)
                {
                    DoClose();
                }
            });
        }

        public void MostrarCantidades()
        {
            if (ArticuloSeleccionado == null)
            {
                this.mostrarAlerta("Debe Seleccionar un Articulo antes de ingresar las cantidades");
                return;
            }
            else
            {
                IngresandoDatos = false;
            }
        }

        public void ResfrescarItemActual()
        {
            if(Articulos != null && Articulos.Count==1)
            {
                ArticuloSeleccionado = Articulos[0];
            }
        }

        public void Regresar() 
        {
            if (!IngresandoDatos)
            {
                IngresandoDatos = true;
            }
            else
            {
                CancelarToma();
            }

        }

        private void CancelarToma()
        {
            
                var mensaje = Pedidos.FacturarPedido ? "la toma de la factura" : "la toma del pedido";


                this.mostrarMensaje(Mensaje.Accion.Cancelar, mensaje, result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            Gestor.Pedidos = new Pedidos();

                            if (FRmConfig.EnConsulta)
                            {
                                this.DoClose();
                            }
                            else
                            {
                                this.DoClose();
                                Dictionary<string, object> par = new Dictionary<string, object>();
                                par.Add("habilitarPedidos", true);
                                this.RequestNavigate<MenuClienteViewModel>(par);
                            }
                        }
                    });
            
        }

        /// <summary>
        /// Metodo utilizado cuando la toma se cancela por un error de validacion de la resolucion.
        /// </summary>
        private void CancelarTomaResolucion(string motivo)
        {
            var mensaje = Pedidos.FacturarPedido ? "El consecutivo de resolución de factura no es válido. " : "El consecutivo de resolución de pedido no es válido. ";
            this.mostrarMensaje(Mensaje.Accion.Informacion, "El consecutivo de resolución de factura no es válido. " + motivo, res =>
                {
                    Gestor.Pedidos = new Pedidos();

                    if (FRmConfig.EnConsulta)
                    {
                        this.DoClose();
                    }
                    else
                    {
                        this.DoClose();
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        this.RequestNavigate<MenuClienteViewModel>(par);
                    }
                });

        }

        #endregion
    }
}