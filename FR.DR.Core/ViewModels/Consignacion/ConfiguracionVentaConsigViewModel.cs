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
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConfiguracionVentaConsigViewModel : BaseViewModel 
    {
        public ConfiguracionVentaConsigViewModel()
        {
            this.CargarVentana();
        }

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

        #region Propiedades



        #region Binding

        List<Pais> listPaises = new List<Pais>();
        List<NivelPrecio> listNiveles = new List<NivelPrecio>();
        List<CondicionPago> listCondiciones = new List<CondicionPago>();

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

        private ClienteCia companiaActual;
        public ClienteCia CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; CargarCompanias(); RaisePropertyChanged("CompaniaActual"); }
        }

        private IObservableCollection<ClienteCia> companias;
        public IObservableCollection<ClienteCia> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private string paisActual;
        public string PaisActual
        {
            get { return paisActual; }
            set { paisActual = value; RaisePropertyChanged("PaisActual"); CargarPaises(); }
        }

        private IObservableCollection<string> paises;
        public IObservableCollection<string> Paises
        {
            get { return paises; }
            set { paises = value; RaisePropertyChanged("Paises"); }
        }

        private string condicionActual;
        public string CondicionActual
        {
            get { return condicionActual; }
            set { condicionActual = value; CargarCondicionesPago(); RaisePropertyChanged("CondicionActual"); }
        }

        private IObservableCollection<string> condiciones;
        public IObservableCollection<string> Condiciones
        {
            get { return condiciones; }
            set { condiciones = value; RaisePropertyChanged("Condiciones"); }
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

        private bool condicionEnabled = true;
        public bool ComboCondicionEnabled
        {
            get { return condicionEnabled; }
            set { condicionEnabled = value; RaisePropertyChanged("ComboCondicionEnabled"); }
        }

        private string nivelActual;
        public string NivelActual
        {
            get { return nivelActual; }
            set { nivelActual = value; CargarListasPrecios(); RaisePropertyChanged("NivelActual"); }
        }

        private IObservableCollection<string> niveles;
        public IObservableCollection<string> Niveles
        {
            get { return niveles; }
            set { niveles = value; RaisePropertyChanged("Niveles"); }
        }

        private bool rbNormal;
        public bool RbNormal
        {
            get { return rbNormal; }
            set { rbNormal = value; RaisePropertyChanged("RbNormal"); }
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

        private bool rbCredito;
        public bool RbCredito
        {
            get { return rbCredito; }
            set { rbCredito = value; RaisePropertyChanged("RbCredito"); }
        }

        private IObservableCollection<string> radioButtons;
        public IObservableCollection<string> RadioButtons
        {
            get { return radioButtons; }
            set { radioButtons = value; RaisePropertyChanged("RadioButtons"); }
        }

        #endregion

        #endregion

        #region mobile


	

		#region Métodos de instancia

		#region Métodos privados
		/// <summary>
		/// Carga los valores por defecto de la ventana.
		/// </summary>
		private void CargarVentana()
		{
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                         " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNombreCliente.Visible = false;

            ComboNivelPrecioEnabled = FRdConfig.PermiteCambiarNivelPrecio;
            ComboPaisEnabled = FRdConfig.PermiteCambiarPais;
            ComboCondicionEnabled = FRdConfig.PermiteCambiarCondicionPago;

            
            RadiosEnabled = FRdConfig.PermiteCambiarClase;

            //Esta opcion debe ser visible unicamente cuando se usa facturacion
            //y la bodega de la ruta sea diferente de ND.
            CheckFacturaPedidoVisible = FRdConfig.UsaFacturacion && Pedidos.ExisteBodega;

            Gestor.Consignaciones.ConfigDocumentoCia.Clear();

            RadioButtons = new SimpleObservableCollection<string>(new List<string>() { "Normal", "Credito" });

			this.LlenarComboCompanias();            
            
		}

		/// <summary>
		/// Método encargado de llenar el combo con las compañías asociadas al cliente.
		/// </summary>
		private void LlenarComboCompanias()
		{
            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());
            //
            foreach (ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                ConfigDocCia config = this.CargarConfiguracionCliente(clt.Compania);

                clt.ObtenerDireccionesEntrega();
                config.ClienteCia = clt;

                Gestor.Consignaciones.CargarConfiguracionVenta(clt.Compania, config);
            }
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
                RaisePropertyChanged("NivelActual");
                RaisePropertyChanged("PaisActual");
                RaisePropertyChanged("CondicionActual");
            }
		}

        /// <summary>
        /// Carga la configuracion del pedido a partir de la informacion del cliente.
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        private ConfigDocCia CargarConfiguracionCliente(string compania)
        {
            return Gestor.Consignaciones.CargarConfiguracionCliente(GlobalUI.ClienteActual, compania);
        }

		/// <summary>
		/// Carga las compañías.
		/// </summary>
		private void CargarCompanias()
		{
            string compania;

			if (CompaniaActual!=null)
			{
				try
				{
					//Cargamos la compañía seleccionada.
                     compania = CompaniaActual.Compania;
			
					this.LlenarComboPaises(compania);
                    this.LlenarComboCondPago(compania);
                    this.LlenarComboNivelesPrecio(compania);

                    ConfigDocCia config = Gestor.Consignaciones.ObtenerConfiguracionVenta(compania);

                    if (config != null)
                        this.MostrarConfiguracion(config);
				}
				catch(System.Exception exc)
				{
					this.mostrarAlerta(exc.Message);
				}
			}
		}

        private void CargarPaises()
        {
            if (CompaniaActual!=null)
            {
                ConfigDocCia config = Gestor.Consignaciones.ObtenerConfiguracionVenta(CompaniaActual.Compania);
                if (config != null)
                {
                    config.Pais = listPaises.Find(x => x.Nombre == PaisActual);
                }
            }
        }

        private void CargarCondicionesPago()
        {
            if (CompaniaActual!=null)
            {
                ConfigDocCia config = Gestor.Consignaciones.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                {
                    config.CondicionPago = this.listCondiciones.Find(x => x.Descripcion == CondicionActual);
                }
                
            }
        }

        /// <summary>
        /// Carga las listas de precios.
        /// </summary>
        private void CargarListasPrecios()
        {
            if (CompaniaActual!=null)
            {
                ConfigDocCia config = Gestor.Consignaciones.ObtenerConfiguracionVenta(CompaniaActual.Compania);

                if (config != null)
                {
                    NivelPrecio lista = listNiveles.Find(x=>x.Nivel==NivelActual);
                    config.Nivel = lista;
                }
                
            }
        }


        private void LlenarComboPaises(string compania)
        {            
            try
            {
                Paises = new SimpleObservableCollection<string>(Pais.ObtenerPaises(compania).ConvertAll(x=>x.Nombre));
                this.listPaises = new List<Pais>(Pais.ObtenerPaises(compania));
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
                this.Condiciones = new SimpleObservableCollection<string>(CondicionPago.ObtenerCondicionesPago(compania).ConvertAll(x=>x.Descripcion));
                this.listCondiciones = CondicionPago.ObtenerCondicionesPago(compania);
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

        private void LlenarComboNivelesPrecio(string compania)
        {
            try
            {
                this.Niveles = new SimpleObservableCollection<string>(NivelPrecio.ObtenerNivelesPrecio(compania).ConvertAll(x=>x.Nivel));
                this.listNiveles = NivelPrecio.ObtenerNivelesPrecio(compania);
                if (Niveles.Count > 0)
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(compania);
                    NivelActual = cliente.NivelPrecio.Nivel;
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando los niveles de precios. " + ex.Message);
            }
        }

		/// <summary>
		/// Muestra una configuración de una venta en consignación en pantalla.
		/// </summary>
		/// <param name="config">Configuración de la venta en consignación.</param>
		private void MostrarConfiguracion(ConfigDocCia config)
		{
            NivelActual = config.Nivel.Nivel;
            PaisActual = config.Pais.Nombre;
            CondicionActual = config.CondicionPago.Descripcion;

            if (config.Clase == ClaseDoc.Normal)
                RbNormal=true;
            else
                RbCredito=true;
        }

		/// <summary>
		/// Inicializa la configuración de la venta en consignación en base a la información del cliente.
		/// </summary>
		private void Inicializar()
		{
            if (CompaniaActual!=null)
            {
                try
                {
                    //Cargamos la compania seleccionada
                    string compania = CompaniaActual.Compania;

                    ConfigDocCia config = this.CargarConfiguracionCliente(compania);

                    Gestor.Consignaciones.CargarConfiguracionVenta(compania, config);

                    this.MostrarConfiguracion(config);
                }
                catch (System.Exception exc)
                {
                    this.mostrarAlerta(exc.Message);
                }
            }
		}

		/// <summary>
		/// Se realiza la cancelación de la toma de la venta en consignación.
		/// </summary>
		public void Cancelar()
		{
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de la venta en consignación", result =>
                {
                    //Si la acción es confirmada se cancela todo los procesos y se despliega la pantalla de menu priincipal.
                    if (result == DialogResult.Yes)
                    {
                        Gestor.Consignaciones.ConfigDocumentoCia.Clear();
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                                               
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                        this.DoClose();
                    } 
                }
                );		
		}

		/// <summary>
		/// Se continúa con la toma de la venta en consignación.
		/// </summary>
		private void Continuar()
		{
			if (this.ValidarDatos())
			{
                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("invocaDesde", Formulario.ConfiguracionConsignacion.ToString());
                parametros.Add("ventasSugeridas", "N");
                //Nos vamos a la toma de la venta en consignación
                this.DoClose();
                RequestNavigate<TomaVentaConsigViewModel>(parametros);								
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
				
			}
		}

		/// <summary>
		/// Valida que los datos hayan sido seleccionados pués estos datos son requiridos para la gestion 
		/// de la venta en consignación.
		/// </summary>
		/// <returns>Verdadero indica que se ha realizado la configuración de la venta en consignación.</returns>
		private bool ValidarDatos()
		{
			if ( PaisActual==null)
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"un país");
				return false;
			}

			if (CondicionActual==null)
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"una condición de pago");
				return false;
			}

			if (NivelActual==null)
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"un nivel de precios");
				return false;
			}

			if(RbNormal || RbCredito)
			{
				if (CompaniaActual!=null) 
				{
                    string cia = CompaniaActual.Compania;

					if (Gestor.Consignaciones.ConfigDocumentoCia[cia] != null)
					{
                        if (RbNormal)
                            ((ConfigDocCia)Gestor.Consignaciones.ConfigDocumentoCia[cia]).Clase = ClaseDoc.Normal;
                        if (RbCredito)
							((ConfigDocCia)Gestor.Consignaciones.ConfigDocumentoCia[cia]).Clase = ClaseDoc.CreditoFiscal;
					}
				}
			}
			else
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"una clase de factura");
				return false;
			}

			return true;
		}
		#endregion

		#endregion
      

        #endregion


    }
}