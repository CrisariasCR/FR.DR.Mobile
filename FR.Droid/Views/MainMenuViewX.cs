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
using FR.Droid.CustomViews;
using FR.Droid.Views.Principal;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Corporativo;

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Menú Principal", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainMenuViewX : Activity
    {        
        
        ImageButton btnClientes, btnDepositos, btnReportes, btnDatos, btnParametros, btnJornada;
        string errorLoad;

        #region lifecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainMenu);

            btnClientes = FindViewById<ImageButton>(Resource.Id.btnClientes);
            btnDepositos = FindViewById<ImageButton>(Resource.Id.btnDepositos);
            btnReportes = FindViewById<ImageButton>(Resource.Id.btnReportes);
            btnDatos = FindViewById<ImageButton>(Resource.Id.btnDatos);
            btnParametros = FindViewById<ImageButton>(Resource.Id.btnParametros);
            btnJornada = FindViewById<ImageButton>(Resource.Id.btnJornada);

            errorLoad = string.Empty;
        }

        protected override void OnStart()
        {
            base.OnStart();            
        }

        protected override void OnPause()
        {
            btnClientes.Click -= btnClientes_Click;
            btnDepositos.Click -= btnDepositos_Click;
            btnReportes.Click -= btnReportes_Click;
            btnDatos.Click -= btnDatos_Click;
            btnParametros.Click -= btnParametros_Click;
            btnJornada.Click -= btnJornada_Click;
            base.OnPause();
        }

        protected override void OnResume()
        {
            btnClientes.Click += btnClientes_Click;
            btnDepositos.Click += btnDepositos_Click;
            btnReportes.Click += btnReportes_Click;
            btnDatos.Click += btnDatos_Click;
            btnParametros.Click += btnParametros_Click;
            btnJornada.Click += btnJornada_Click;            
            if (!string.IsNullOrEmpty(errorLoad))
            {
                ShowMessage ms = new ShowMessage(this, errorLoad, true);
                RunOnUiThread(() => ms.Mostrar());
                errorLoad = string.Empty;
            }
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            base.OnResume();

        }

        

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region Eventos


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var MenuItem1 = menu.Add(0, 1, 1, "Cerrar Sesión");
            //var MenuItem2 = menu.Add(0, 2, 2, "Salir");

            MenuItem1.SetIcon(Resource.Drawable.ic_login);
            //MenuItem2.SetIcon(Resource.Drawable.ic_cancelar);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 1:
                    this.CerrarSesion();
                    return true;
                default:
                    return true;
            }
        }

        public override void OnBackPressed()
        {
            this.MoveTaskToBack(true);
        }

        void btnJornada_Click(object sender, EventArgs e)
        {
            try
            {
                this.JoranadaLaboral();
            }
            catch (Exception ex)
            {
                ShowMessage ms = new ShowMessage(this, "Error al ingresar a jornada." + ex.Message, true);
                RunOnUiThread(() => ms.Mostrar());
            }
        }

        void btnParametros_Click(object sender, EventArgs e)
        {
            try
            {
                this.MenuParametros();
            }
            catch (Exception ex)
            {
                ShowMessage ms = new ShowMessage(this, "Error al ingresar a parámetros." + ex.Message, true);
                RunOnUiThread(() => ms.Mostrar());
            }
        }

        void btnDatos_Click(object sender, EventArgs e)
        {
            try
            {
                this.MenuDatos();
            }
            catch (Exception ex)
            {
                ShowMessage ms = new ShowMessage(this, "Error al ingresar a datos." + ex.Message, true);
                RunOnUiThread(() => ms.Mostrar());
            }
        }

        void btnReportes_Click(object sender, EventArgs e)
        {
            try
            {
                this.MenuReportes();
            }
            catch (Exception ex)
            {
                ShowMessage ms = new ShowMessage(this, "Error al ingresar a reportes." + ex.Message, true);
                RunOnUiThread(() => ms.Mostrar());
            }
        }

        void btnDepositos_Click(object sender, EventArgs e)
        {
            try
            {
                this.IrPantallaDepositos();
            }
            catch (Exception ex)
            {
                ShowMessage ms = new ShowMessage(this, "Error al ingresar a depósitos." + ex.Message, true);
                RunOnUiThread(() => ms.Mostrar());
            }
        }

        void btnClientes_Click(object sender, EventArgs e)
        {
            try
            {
                this.BuscarClientes();
            }
            catch (Exception ex)
            {
                ShowMessage ms = new ShowMessage(this, "Error al ingresar a seleccion de clientes."+ex.Message,true);
                RunOnUiThread(() => ms.Mostrar());
            }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Carga la pantalla
        /// </summary>
        private void CargaInicial() 
        {
            try
            {
                CrearRegistroJornada();
                ActivarBotonJornada();
            }
            catch (Exception ex)
            {
                errorLoad = "Ocurrio un error al cargar la pantalla." + ex.Message;
            }
        }

        /// <summary>
        /// Crea un registro de la jornada
        /// </summary>
        private void CrearRegistroJornada()
        {
            //Crea el registro en caso de que no se utilizen jornadas
            //Si se utilizan Jornadas, el registro se creara hasta que se abra la jornada - KFC
            try
            {
                if ((!FRdConfig.UsaJornadaLaboral) && (!JornadaRuta.VerificarJornadaAbierta()))
                    JornadaRuta.AbrirJornada();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Oculta o visualiza el botón de jornada
        /// </summary>
        private void ActivarBotonJornada()
        {

            if (FRdConfig.UsaJornadaLaboral)
            {
                btnJornada.Visibility = ViewStates.Visible;
                HabilitarAccionesJornada();
            }
            else
            {
                btnJornada.Visibility = ViewStates.Gone;
            }
        }

        
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

            btnClientes.Enabled = habilitar;
            btnDepositos.Enabled = habilitar;            
        }

        /// <summary>
        /// Abre la pantalla de seleccion de clientes
        /// </summary>
        public void BuscarClientes()
        {
            if (JornadaRuta.VerificarJornadaAbierta() && !JornadaRuta.VerificarJornadaCerrada())
            {
                GC.Collect();
                //TODO
                
            }
            else
            {
                if (!JornadaRuta.VerificarJornadaAbierta())
                {
                    ShowMessage ms = new ShowMessage(this, "Debe abrir la jornada antes de hacer la ruta");
                    RunOnUiThread(() => ms.Mostrar());
                }
                else
                {
                    ShowMessage ms = new ShowMessage(this, "No puede realizar más documentos ya que cerró la jornada");
                    RunOnUiThread(() => ms.Mostrar());
                }
                HabilitarAccionesJornada();
            }
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
                //TODO
                //RequestNavigate<SeleccionRecibosViewModel>();                
            }
            else
            {
                if (!JornadaRuta.VerificarJornadaAbierta())
                {
                    ShowMessage ms = new ShowMessage(this, "Debe abrir la jornada antes de hacer la ruta");
                    RunOnUiThread(() => ms.Mostrar());
                }
                else
                {
                    ShowMessage ms = new ShowMessage(this, "No puede realizar más documentos ya que cerró la jornada");
                    RunOnUiThread(() => ms.Mostrar());
                }
                HabilitarAccionesJornada();
            }
        }

        /// <summary>
        /// Ir a pantalla de reportes
        /// </summary>
        public void MenuReportes()
        {
            //TODO
            //this.RequestNavigate<MenuReportesViewModel>();
        }

        /// <summary>
        /// Ir al menú de datos
        /// </summary>
        public void MenuDatos()
        {
            if (Softland.ERP.FR.Mobile.App.Prefijo == "(SYNC) ")
            {
                ShowMessage ms = new ShowMessage(this, "No se pueden sincronizar los datos porque hay una sincronización en proceso. Debera esperar hasta que termine.",false);
                RunOnUiThread(() => ms.Mostrar());
            }
            else
            {
                //TODO
                //this.RequestNavigate<MenuDatosViewModel>();
            }
        }

        /// <summary>
        /// Ir al menú de parámetros
        /// </summary>
        public void MenuParametros()
        {
            var intent=new Intent(this,typeof(MenuParametrosViewX));
            StartActivity(intent);
        }

        /// <summary>
        /// Ir a Pantalla de Jornada
        /// </summary>
        public void JoranadaLaboral()
        {
            //TODO
            //this.RequestNavigate<JornadaLaboralViewModel>();
        }

        /// <summary>
        /// Regresa a la pantalla de login
        /// </summary>
        public void CerrarSesion()
        {
            var intent = new Intent(this, typeof(LoginViewX));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            this.StartActivity(intent);
        }   

        #endregion

    }
}