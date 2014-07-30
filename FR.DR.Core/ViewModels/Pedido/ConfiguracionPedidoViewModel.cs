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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using FR.DR.Core.Data.Corporativo;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConfiguracionPedidoViewModel : BaseViewModel
    {
        public ConfiguracionPedidoViewModel(string documento)
        {
            SugeridoChecked = FRdConfig.UsaSugeridoVenta;

            if (SugeridoChecked)
            {
                switch (documento)
                {
                    case Pedidos.VALOR_AMBOS:
                        SugeridoEnabled = false;
                        break;
                    case Pedidos.VALOR_PEDIDO:
                        FacturaPedidoChecked = false;
                        //Continuar();
                        break;
                    case Pedidos.VALOR_FACTURA:
                        FacturaPedidoChecked = true;
                        //Continuar();
                        break;
                }
            }
            else
            {
                switch (documento)
                {
                    case Pedidos.VALOR_PEDIDO:
                        FacturaPedidoChecked = false;
                        FacturaPedidoEnabled = false;
                        break;
                    case Pedidos.VALOR_FACTURA:
                        FacturaPedidoChecked = true;
                        FacturaPedidoEnabled = false;
                        break;
                }
            }
            CargaInicial();
        }

        public ConfiguracionPedidoViewModel()
        {
            CargaInicial();
        }

        #region Propiedades

        #region Elementos Visuales

        public bool SugeridoChecked { get; set; }
        public bool FacturaPedidoChecked { get; set; }

        private bool sugeridoEnabled = true;
        public bool SugeridoEnabled
        {
            get { return sugeridoEnabled; }
            set { sugeridoEnabled = value; RaisePropertyChanged("SugeridoEnabled"); }
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

        private bool facturaPedidoEnabled = true;
        public bool FacturaPedidoEnabled
        {
            get { return facturaPedidoEnabled; }
            set { facturaPedidoEnabled = value; RaisePropertyChanged("FacturaPedidoEnabled"); }
        }

        private bool nivelPrecioEnabled = true;
        public bool ComboNivelPrecioEnabled
        {
            get { return nivelPrecioEnabled; }
            set { nivelPrecioEnabled = value; RaisePropertyChanged("ComboNivelPrecioEnabled"); }
        }

        private bool paisEnabled = true;
        public bool ComboPaisEnabled
        {
            get { return paisEnabled; }
            set { paisEnabled = value; RaisePropertyChanged("ComboPaisEnabled"); }
        }

        private bool div1Enabled = true;
        public bool ComboDiv1Enabled
        {
            get { return div1Enabled; }
            set { div1Enabled = value; RaisePropertyChanged("ComboDiv1Enabled"); }
        }

        private bool div2Enabled = true;
        public bool ComboDiv2Enabled
        {
            get { return div2Enabled; }
            set { div2Enabled = value; RaisePropertyChanged("ComboDiv2Enabled"); }
        }

        private bool condicionEnabled = true;
        public bool ComboCondicionEnabled
        {
            get { return condicionEnabled; }
            set { condicionEnabled = value; RaisePropertyChanged("ComboCondicionEnabled"); }
        }

        private bool radioNormalChecked;
        public bool RadioNormalChecked
        {
            get { return radioNormalChecked; }
            set { radioNormalChecked = value; CambioRadios(); RaisePropertyChanged("RadioNormalChecked"); }
        }

        private bool radiosEnabled = true;
        public bool RadiosEnabled
        {
            get { return radiosEnabled; }
            set { radiosEnabled = value; RaisePropertyChanged("RadiosEnabled"); }
        }

        private bool facturaVisible = true;
        public bool CheckFacturaPedidoVisible
        {
            get { return facturaVisible; }
            set { facturaVisible = value; RaisePropertyChanged("CheckFacturaPedidoVisible"); }
        }

        #endregion

        #region Combos

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
            set { companiaActual = value; CambioCompania(); RaisePropertyChanged("CompaniaActual"); }
        }

        List<Pais> listPaises = new List<Pais>();
        List<DivisionGeografica> listDivisiones1 = new List<DivisionGeografica>();
        List<DivisionGeografica> listDivisiones2 = new List<DivisionGeografica>();

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

        List<CondicionPago> listCondiciones = new List<CondicionPago>();
        private IObservableCollection<string> condiciones;
        public IObservableCollection<string> Condiciones
        {
            get { return condiciones; }
            set { condiciones = value; RaisePropertyChanged("Condiciones"); }
        }

        private string condicionActual;
        public string CondicionActual
        {
            get { return condicionActual; }
            set { condicionActual = value; RaisePropertyChanged("CondicionActual"); CambioCondicion(); }
        }

        private List<NivelPrecio> ListNiveles = new List<NivelPrecio>();

        private IObservableCollection<string> nivelesPrecio;
        public IObservableCollection<string> NivelesPrecio
        {
            get { return nivelesPrecio; }
            set { nivelesPrecio = value;  RaisePropertyChanged("NivelesPrecio"); }
        }

        private string nivelActual;
        public string NivelActual
        {
            get { return nivelActual; }
            set { nivelActual = value; RaisePropertyChanged("NivelActual"); CambioNivelPrecio(); }
        }
        
        #endregion

        #endregion

        #region Logica

        private void CargaInicial()
        {
            ComboNivelPrecioEnabled = FRdConfig.PermiteCambiarNivelPrecio;
            ComboPaisEnabled = FRdConfig.PermiteCambiarPais;
            ComboDiv1Enabled = ComboPaisEnabled;
            ComboDiv2Enabled = ComboPaisEnabled;
            ComboCondicionEnabled = FRdConfig.PermiteCambiarCondicionPago;

            RadioNormalChecked = true;
            RadiosEnabled = FRdConfig.PermiteCambiarClase;

            //Esta opcion debe ser visible unicamente cuando se usa facturacion
            //y la bodega de la ruta sea diferente de ND.
            CheckFacturaPedidoVisible = FRdConfig.UsaFacturacion && Pedidos.ExisteBodega;

            Gestor.Pedidos.ConfigDocumentoCia.Clear();

            LlenarComboCompanias();
            //this.companiasCbo_SelectedIndexChanged(null, null);
        }

        private void CambioCompania()
        {
            if (CompaniaActual != null)
            {
                try
                {
                    //Cargamos la compania seleccionada
                    string compania = CompaniaActual.Compania;
                    LlenarComboPaises(compania);
                    LlenarComboCondPago(compania);
                    LlenarComboNivelesPrecio(compania);

                    ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(compania);

                    if (config != null)
                    {
                        this.MostrarConfiguracion(config);
                    }
                }
                catch (System.Exception exc)
                {
                    this.mostrarAlerta(exc.Message);
                }                
            }
        }

        private void CambioPais()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual.Compania);
                if (config != null)
                    config.Pais = this.listPaises.Find(x => x.Nombre == PaisActual);
                LabelDiv1 = this.listPaises.Find(x => x.Nombre == PaisActual).LblDivisionGeografica1;
                LabelDiv2 = this.listPaises.Find(x => x.Nombre == PaisActual).LblDivisionGeografica2;
            }
            this.LlenarComboDivisiones1(CompaniaActual.Compania);
        }

        private void CambioDiv1()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual.Compania);
                if (config != null)
                {
                    if (!Div1Actual.Equals("Ninguna"))
                        config.Pais.DivisionGeografica1 = this.listDivisiones1.Find(x => x.Nombre == Div1Actual).Codigo;
                    else
                        config.Pais.DivisionGeografica1 = Div1Actual;
                }
            }
            this.LlenarComboDivisiones2(CompaniaActual.Compania);
        }

        private void CambioDiv2()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual.Compania);
                if (config != null)
                {
                    if (!Div2Actual.Equals("Ninguna"))
                        config.Pais.DivisionGeografica2 = this.listDivisiones2.Find(x => x.Nombre == Div2Actual).Codigo;
                    else
                        config.Pais.DivisionGeografica2 = Div2Actual;
                }
                

            }
        }

        private void CambioCondicion()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                {                    
                    config.CondicionPago = this.listCondiciones.Find(x => x.Descripcion == CondicionActual);
                    if (config.CondicionPago.DiasNeto == 0)
                        Pedidos.PermiteGarantia = true;
                    else
                        Pedidos.PermiteGarantia = false;
                }
            }
        }

        private void CambioNivelPrecio()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                {
                    NivelPrecio lista = this.ListNiveles.Find(x => x.Nivel == NivelActual);
                    config.Nivel = lista;
                }
            }
        }

        private void CambioRadios()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Pedidos.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                    config.Clase = RadioNormalChecked ? ClaseDoc.Normal : ClaseDoc.CreditoFiscal;
            }
        }

        private void LlenarComboNivelesPrecio(string compania)
        {
            try
            {
                this.ListNiveles = NivelPrecio.ObtenerNivelesPrecio(compania);
                NivelesPrecio = new SimpleObservableCollection<string>(ListNiveles.ConvertAll(x=>x.Nivel));

                if (NivelesPrecio.Count == 0)
                {
                    NivelesPrecio.Add((GlobalUI.ClienteActual.ObtenerClienteCia(compania).NivelPrecio).Nivel);
                }
                else
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    NivelActual = ListNiveles.Find(x => x.Codigo == cliente.NivelPrecio.Codigo).Nivel;
                }

            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando los niveles de precios. " + ex.Message);
            }
        }

        private void LlenarComboCondPago(string compania)
        {
            try
            {                
                this.listCondiciones = CondicionPago.ObtenerCondicionesPago(compania);
                Condiciones = new SimpleObservableCollection<string>(listCondiciones.ConvertAll(x=>x.Descripcion));
                if (Condiciones.Count > 0)
                {
                    //CondicionActual = Condiciones[0];
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    CondicionActual = listCondiciones.Find(x => x.Codigo == cliente.CondicionPago).Descripcion;
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando las condiciones de pago. " + ex.Message);
            }
        }

        private void LlenarComboPaises(string compania)
        {
            try
            {                
                this.listPaises = Pais.ObtenerPaises(compania);
                Paises = new SimpleObservableCollection<string>(this.listPaises.ConvertAll(x=>x.Nombre));
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
                if(listDivisiones1.Count>0)
                    Divisiones1 = new SimpleObservableCollection<string>(this.listDivisiones1.ConvertAll(x => x.Nombre));
                else
                    Divisiones1 = new SimpleObservableCollection<string>(new List<string>(new string[]{"Ninguna"}));
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
                if(listDivisiones1.Count>0)
                    this.listDivisiones2 = Pais.ObtenerDivisionesGeograficas2(compania,this.listDivisiones1.Find(x => x.Nombre == Div1Actual).Codigo, this.listPaises.Find(x => x.Nombre == PaisActual).Codigo);
                if (listDivisiones2.Count > 0)
                    Divisiones2 = new SimpleObservableCollection<string>(this.listDivisiones2.ConvertAll(x => x.Nombre));
                else
                    Divisiones2 = new SimpleObservableCollection<string>(new List<string>(new string[] { "Ninguna" }));
                if (Divisiones2.Count > 0)
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    if (!string.IsNullOrEmpty(cliente.DivisionGeografica2) && listDivisiones2.Count>0)
                        Div2Actual = listDivisiones2.Find(x => x.Codigo == cliente.DivisionGeografica2).Nombre;
                    if (string.IsNullOrEmpty(Div2Actual))
                        Div2Actual= Divisiones2[0];
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando las Divisiones Geográficas 2. " + ex.Message);
            }
        }



        public void LlenarComboCompanias()
        {
            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());

            foreach (ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                ConfigDocCia config = this.CargarConfiguracionCliente(clt.Compania);

                clt.ObtenerDireccionesEntrega();
                config.ClienteCia = clt;

                Gestor.Pedidos.CargarConfiguracionVenta(clt.Compania, config);
            }

            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
        }

        private void MostrarConfiguracion(ConfigDocCia config)
        {
            NivelActual = config.Nivel.Nivel;
            PaisActual = config.Pais.Nombre;
            CondicionActual = config.CondicionPago.Descripcion;

            if (config.Clase == ClaseDoc.Normal)
                RadioNormalChecked = true;
            else
                RadioNormalChecked = false;
        }

        private ConfigDocCia CargarConfiguracionCliente(string compania)
        {
            return Gestor.Pedidos.CargarConfiguracionCliente(GlobalUI.ClienteActual, compania);
        }

        private void Inicializar()
        {
            if (CompaniaActual != null)
            {
                try
                {
                    //Cargamos la compania seleccionada

                    ConfigDocCia config = this.CargarConfiguracionCliente(CompaniaActual.Compania);

                    Gestor.Consignaciones.CargarConfiguracionVenta(CompaniaActual.Compania, config);

                    this.MostrarConfiguracion(config);
                }
                catch (System.Exception exc)
                {
                    this.mostrarAlerta(exc.Message);
                }
            }
        }

        private void Cancelar()
        {
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de pedido", result =>
            {
                if (result == DialogResult.Yes)
                {
                    DoClose();
                }
            });
        }

        private void Continuar()
        {
            string mensaje = null;
            if (this.ValidarDatos(ref mensaje))
            {
                //Este check indica que los pedidos seran facturados
                Pedidos.FacturarPedido = FacturaPedidoChecked;

                if (SugeridoChecked)
                {
                    SugeridoVentaViewModel.NivelPrecios = this.ListNiveles.Find(x => x.Nivel == NivelActual);
                    this.RequestNavigate<SugeridoVentaViewModel>();
                    DoClose();
                }
                else
                {
                    TomaPedidoViewModel.NivelPrecio = this.ListNiveles.Find(x => x.Nivel == NivelActual);
                    Dictionary<string, object> Parametros = new Dictionary<string, object>() { { "pedidoActual", false } };
                    this.RequestNavigate<TomaPedidoViewModel>(Parametros);

                    DoClose();                    
                }
            }
            else if(!string.IsNullOrEmpty(mensaje))
            {
                this.mostrarAlerta(mensaje);
            }
        }

        private bool ValidarDatos(ref string mensaje)
        {
            try
            {
                if (PaisActual == null)
                {
                    mensaje = Mensaje.Accion.SeleccionNula + "un país";
                    return false;
                }

                if (CondicionActual == null)
                {
                    mensaje = Mensaje.Accion.SeleccionNula + "una condición de pago";
                    return false;
                }

                if (NivelActual == null)
                {
                    mensaje = Mensaje.Accion.SeleccionNula + "un nivel de precios";
                    return false;
                }

                if (Pedidos.ValidarLimiteCredito != Pedidos.LIMITECREDITO_NOAPLICA)
                {
                    if ((Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_AMBOS) || (Pedidos.ValidarLimiteCredito == Pedidos.LIMITECREDITO_FACTURA && FacturaPedidoChecked))
                    {
                        if (GlobalUI.ClienteActual.LimiteCreditoExcedido(CompaniaActual.Compania, 0))
                        {
                            mensaje = "El cliente tiene su límite de credito excedido y no se podra gestionar el documento.";
                            return false;
                        }
                    }
                }

                if (Pedidos.ValidarDocVencidos != Pedidos.DOCVENCIDOS_PERMITE)
                {
                    if (GlobalUI.ClienteActual.TieneDocumentosVencidos)
                    {
                        if (Pedidos.ValidarDocVencidos == Pedidos.DOCVENCIDOS_NOPERMITE)
                        {
                            this.mostrarAlerta("El cliente tiene documentos vencidos pendientes de cobro.", result =>
                            {
                                InicioCobro();
                            });
                            mensaje = null;
                            return false;
                        }

                        if (Pedidos.ValidarDocVencidos == Pedidos.DOCVENCIDOS_ADVERTIR)
                        {

                            this.mostrarAlerta("El cliente tiene documentos vencidos pendientes de cobro. Desea Continuar?", result =>
                            {
                                if (result == DialogResult.No)
                                {
                                    InicioCobro();
                                }
                            });
                            mensaje = null;
                            return false;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.mostrarAlerta(exc.Message);
                return false;
            }

            return true;
        }

        private void InicioCobro()
        {
            //GC.Collect();
            Bitacora.Escribir("Se inicia la toma de cobro.");
            this.RequestNavigate<CreacionReciboViewModel>();
            DoClose();
        }

        #endregion

        #region Comandos

        public ICommand ComandoInicializar
        {
            get { return new MvxRelayCommand(Inicializar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        #endregion
    }
}