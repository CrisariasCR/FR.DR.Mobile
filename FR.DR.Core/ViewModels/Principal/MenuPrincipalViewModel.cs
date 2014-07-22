using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Configuracion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MenuPrincipalViewModel : BaseViewModel
    {
        #region Constructor y Propiedades

        public MenuPrincipalViewModel()
        {
            CrearRegistroJornada();
            ActivarBotonJornada();
            //Comentar cuando este el login
            //bool exito = false;
            //string mensaje = string.Empty;
            //HandHeldConfig config = new HandHeldConfig();
            //ParametroSistema.CargarParametros();

            //exito = config.cargarConfiguracionHandHeld(ref mensaje);
            //if (exito)
            //    exito = config.cargarConfiguracionGlobalHandHeld(ref mensaje);
            //if (!exito)
            //{
            //    //System.Windows.Forms.MessageBox.Show(mensaje, Mensaje.titulo, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk, System.Windows.Forms.MessageBoxDefaultButton.Button1);                
            //}
            //HacerLogin();
        }

        private bool clientesEnabled;
        public bool ClientesEnabled 
        {
            get { return clientesEnabled; }
            set
            {
                clientesEnabled = value;
                RaisePropertyChanged("ClientesEnabled");
            }
        }

        private bool depositosEnabled;
        public bool DepositosEnabled
        {
            get { return depositosEnabled; }
            set
            {
                depositosEnabled = value;
                RaisePropertyChanged("DepositosEnabled");
            }
        }

        private bool jornadaVisible;
        public bool JornadaVisible
        {
            get { return jornadaVisible; }
            set
            {
                jornadaVisible = value;
                RaisePropertyChanged("JornadaVisible");
            }
        }


        #endregion Constructor y Propiedades

        #region Comandos

        public ICommand BusquedaClientesCommand
        {
            get { return new MvxRelayCommand(BuscarClientes); }
        }

        public ICommand ReportesCommand
        {
            get { return new MvxRelayCommand(MenuReportes); }
        }

        public ICommand DatosCommand
        {
            get { return new MvxRelayCommand(MenuDatos); }
        }

        public ICommand ParametrosCommand
        {
            get { return new MvxRelayCommand(MenuParametros); }
        }

        public ICommand JornadaLaboralCommand
        {
            get { return new MvxRelayCommand(JoranadaLaboral); }
        }

        public ICommand DepositosCommand
        {
            get { return new MvxRelayCommand(IrPantallaDepositos); }
        }

        #endregion Comandos

        #region Acciones

        public void BuscarClientes()
        {
            if (JornadaRuta.VerificarJornadaAbierta() && !JornadaRuta.VerificarJornadaCerrada())
            {
                GC.Collect();
            this.RequestNavigate<SeleccionClienteViewModel>(true);
            }
            else
            {
                if (!JornadaRuta.VerificarJornadaAbierta())
                    this.mostrarAlerta("Debe abrir la jornada antes de hacer la ruta");
                else
                    this.mostrarAlerta("No puede realizar más documentos ya que cerró la jornada");
                HabilitarAccionesJornada();
            }
        }

        public void MenuReportes()
        {
            this.RequestNavigate<MenuReportesViewModel>();
        }

        public void MenuDatos()
        {
            if (Softland.ERP.FR.Mobile.App.Prefijo == "(SYNC) ")
            {
                this.mostrarAlerta("No se pueden sincronizar los datos porque hay una sincronización en proceso. Debera esperar hasta que termine.");
            }
            else
            {
                this.RequestNavigate<MenuDatosViewModel>();
            }
        }

        public void MenuParametros()
        {
            this.RequestNavigate<MenuParametrosViewModel>();
        }

        /// <summary>
        /// Funcion que despliega la pantalla para realizar depósitos.
        /// </summary>
        private void IrPantallaDepositos()
        {
            //if ((FRdConfig.UsaJornadaLaboral) && (JornadaRuta.VerificarJornadaAbierta()))
            if (JornadaRuta.VerificarJornadaAbierta() && !JornadaRuta.VerificarJornadaCerrada())
            {
                GC.Collect();
                RequestNavigate<SeleccionRecibosViewModel>();                
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que llibera de memoria propiedas de los componentes                
            }
            else
            {
            
                 if (!JornadaRuta.VerificarJornadaAbierta())
                    this.mostrarAlerta("Debe abrir la jornada antes de hacer un depósito.");
                else
                    this.mostrarAlerta("No puede realizar depósitos ya que ya cerró la jornada.");
                HabilitarAccionesJornada();
            }
        }

        // Manejos para Jornada Laboral - KFC
        /// <summary>
        /// Habilita o deshabilita los botones de Clientes y Depositos segun sea el estado de la Jornada 
        /// </summary>
        private void HabilitarAccionesJornada()
        {
            bool habilitar = false;

            if (JornadaRuta.VerificarJornadaAbierta())
            {
                if (JornadaRuta.VerificarJornadaCerrada())
                    habilitar = false;
                else
                    habilitar = true;
            }
            else
                habilitar = false;

            //btnClientes.Enabled = btnDepositos.Enabled = lbbClientes.Enabled = lbbDepositos.Enabled = habilitar;
            ClientesEnabled=DepositosEnabled = habilitar;
        }

        private void CrearRegistroJornada()
        {
            //Crea el registro en caso de que no se utilizen jornadas
            //Si se utilizan Jornadas, el registro se creara hasta que se abra la jornada - KFC
            if ((!FRdConfig.UsaJornadaLaboral) && (!JornadaRuta.VerificarJornadaAbierta()))
                JornadaRuta.AbrirJornada();
        }

        public void JoranadaLaboral()
        {
            this.RequestNavigate<JornadaLaboralViewModel>();
        }
        

        // Manejos para Jornada Laboral - KFC
        /// <summary>
        /// Activa el boton de ir a Manejo de Jornada Laboral si el parametro esta marcado
        /// </summary>
        private void ActivarBotonJornada()
        {

            if (FRdConfig.UsaJornadaLaboral)
            {
                JornadaVisible = true;
                HabilitarAccionesJornada();
            }
            else
            {
                JornadaVisible = false;
            }
        }

        #endregion Acciones


        public void CerrarSesion() 
        {            
            RequestNavigate<LoginViewModel>();
            this.DoClose();
        }
    }
}
