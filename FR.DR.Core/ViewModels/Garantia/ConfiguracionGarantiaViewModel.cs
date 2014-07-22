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

        private string condicionActual;
        public string CondicionActual
        {
            get { return condicionActual; }
            set { condicionActual = value; RaisePropertyChanged("CondicionActual");}
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
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de garant�a", result =>
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
                Garantias.FacturarGarantia = FacturaPedidoChecked;
                    //Caso 28087 LDS 28087
                    //Nos vamos a la toma de pedido
                    //Caso R0-102009-S00S LDA
                    TomaGarantiaViewModel.NivelPrecio = this.ListNiveles.Find(x => x.Nivel == NivelActual);
                    Dictionary<string, object> Parametros = new Dictionary<string, object>() { { "pedidoActual", false } };
                    this.RequestNavigate<TomaGarantiaViewModel>(Parametros);

                    DoClose();
                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                
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
                    mensaje = Mensaje.Accion.SeleccionNula + "un pa�s";
                    return false;
                }

                if (CondicionActual == null)
                {
                    mensaje = Mensaje.Accion.SeleccionNula + "una condici�n de pago";
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
                            mensaje = "El cliente tiene su l�mite de credito excedido y no se podra gestionar el documento.";
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