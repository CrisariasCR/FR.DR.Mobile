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

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConfiguracionGarantiaViewModel : BaseViewModel
    {
        public ConfiguracionGarantiaViewModel(string documento)
        {           
            FacturaPedidoChecked = true;
            FacturaPedidoEnabled = false;            
            CargaInicial();
        }

        public ConfiguracionGarantiaViewModel()
        {
            CargaInicial();
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #region Propiedades

        #region Elementos Visuales
        
        public bool FacturaPedidoChecked { get; set; }        

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

                    ConfigDocCia config = Gestor.Garantias.ObtenerConfiguracionVenta(compania);

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
                ConfigDocCia config = Gestor.Garantias.ObtenerConfiguracionVenta(CompaniaActual.Compania);
                if (config != null)
                    config.Pais = this.listPaises.Find(x => x.Nombre == PaisActual);
            }
        }

        private void CambioNivelPrecio()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Garantias.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                {
                    NivelPrecio lista = this.ListNiveles.Find(x => x.Nivel == NivelActual);
                    config.Nivel = lista;
                }
            }
        }

        private void CambioCondicion()
        {
            if (CompaniaActual != null)
            {
                ConfigDocCia config = Gestor.Garantias.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                {
                    config.CondicionPago = this.listCondiciones.Find(x => x.Descripcion == CondicionActual);                    
                }
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

        private void LlenarComboCondPago(string compania)
        {
            try
            {
                this.listCondiciones = CondicionPago.ObtenerCondicionesPago(compania);
                Condiciones = new SimpleObservableCollection<string>(listCondiciones.ConvertAll(x => x.Descripcion));
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

        public void LlenarComboCompanias()
        {
            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());

            foreach (ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                ConfigDocCia config = this.CargarConfiguracionCliente(clt.Compania);

                clt.ObtenerDireccionesEntrega();
                config.ClienteCia = clt;

                Gestor.Garantias.CargarConfiguracionVenta(clt.Compania, config);
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
        }

        private ConfigDocCia CargarConfiguracionCliente(string compania)
        {
            return Gestor.Garantias.CargarConfiguracionCliente(GlobalUI.ClienteActual, compania);
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
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de garantía", result =>
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
                    TomaGarantiaViewModel.NivelPrecio = this.ListNiveles.Find(x => x.Nivel == NivelActual);
                    Dictionary<string, object> Parametros = new Dictionary<string, object>() { { "pedidoActual", false } };
                    this.RequestNavigate<TomaGarantiaViewModel>(Parametros);

                    DoClose();               
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