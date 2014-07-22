using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Cirrious.MvvmCross.Commands;

using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Seguridad;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRJornada;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        public LoginViewModel()
        {
            bDispose = false;
            //NombreUsuario = "LAZA";
            //Password = "EXLAZA";
            CargarAplicacion();
        }

        public Android.Content.Context Contexto { get; set; }

        #region Binding

        public int intentos=0;
        public string Version { get; set; }
        public string Footer { get; set; }

        public bool bDispose { get; set; }

        private string nombreUsuario = string.Empty;
        public string NombreUsuario
        {
            get { return nombreUsuario; }
            set { nombreUsuario = value; RaisePropertyChanged("NombreUsuario"); }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged("Password"); }
        }

        private int numeroIntentos;
        private int intentosPermitidos;

        #endregion

        #region Metodos

        private void CargarAplicacion()
        {
            //Leemos Config.xml
            //LeerArchivoConfiguracion();

            MostrarVersion();

            //Inicializamos la bitacora
            Bitacora.Inicializar();
            Bitacora.Escribir("\r\nInicio de bitacora " + DateTime.Now + "\r\n");

            //Inicializar la instancia de Ubicación para el manejo de GPS.
            Ubicacion.Inicializar();

            if (NombreUsuario != string.Empty && Password != string.Empty)
            {
                //ValidarIngreso();
            }
        }

        private void LeerArchivoConfiguracion()
        {
            try
            {
                // This is the full directory and exe name
                //string fullAppName = Assembly.GetExecutingAssembly().GetName().CodeBase;
                //FRmConfig.FullAppPath = Path.GetDirectoryName(fullAppName);
#if DEBUG
                 //FRmConfig.FullAppPath = "/sdcard";
                FRmConfig.FullAppPath = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
                FRmConfig.FullAppPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
                //FRmConfig.FullAppPath = "/sdcard";

                //se lee el xml para conocer las bases de datos del xml
                ConfigurationClass.ReadXml(System.IO.Path.Combine(FRmConfig.FullAppPath, "Config.xml"));
            }
            catch (FileNotFoundException)
            {
                this.mostrarAlerta("No se encuentra el xml de configuración. Verifique que lo tenga instalado.", result =>
                {
                    this.DoClose();
                }
                );

            }
            catch (Exception exc)
            {
                this.mostrarAlerta("Verifique que el xml de configuración este bien definido: " + exc.Message + ".", result =>
                {
                    this.DoClose();
                }
                );
            }
        }

        private void MostrarVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Version = "Versión " + version.ToString();
            if (version.Minor > 0) Version += " R" + version.Minor;
            if (version.Build > 0) Version += " SP" + version.Build;

            Footer = "Copyright © " + DateTime.Now.Year.ToString() + " Softland Inversiones S.L.";
        }

        public bool ValidarExactusXml()
        {
#if DEBUG
                        //string folder = "/sdcard";
            string folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
            Java.IO.File file = new Java.IO.File(folder, "Config.xml");
            if (file.Length() == 0)
            {
                RequestNavigate<ExactusXMLViewModel>();

            }
            else
            {
                //Leemos Config.xml
                //DoNothing                    
                //ValidarIngreso();
                return true;
            }
            return false;
        }

        public void ValidarIngreso()
        {

            if (NombreUsuario == string.Empty)
            {
                this.mostrarAlerta("Digite el usuario.");
            }
            else if (Password == string.Empty)
            {
                this.mostrarAlerta("Digite la contraseña.");
            }
            else
            {
                NombreUsuario = NombreUsuario.ToUpper();
                bool existeBD = ValidarExistBaseDeDatos();

                if (existeBD)
                {
                    //Carga Globales FR
                    FRdConfig.CargarGlobales();
                    if (NombreUsuario != GestorDatos.NombreUsuario)
                    {
                        GestorDatos.NombreUsuario = NombreUsuario;
                        GestorDatos.ContrasenaUsuario = Password.ToUpper();

                        try
                        {
                            //Como el usuario cambio, volvemos a sacar los intentos
                            intentosPermitidos = GestorSeguridad.VerificarUsuario(NombreUsuario);
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error verificando el usuario. " + ex.Message);
                            return;
                        }

                        //y ponemos el numero de intentos a cero
                        numeroIntentos = 0;

                        if (intentosPermitidos == -1)
                        {
                            this.mostrarMensaje(Mensaje.Accion.UsuarioInvalido);

                            Password = string.Empty;
                            NombreUsuario = string.Empty;
                            return;
                        }

                        if (intentosPermitidos == 0)
                        {
                            this.mostrarAlerta("Este usuario no puede ingresar al sistema", result =>
                            {
                                Password = string.Empty;
                                NombreUsuario = string.Empty;
                            });
                            return;
                        }
                    }

                    bool autenticado;

                    try
                    {
                        autenticado = GestorSeguridad.AutenticarUsuario(NombreUsuario, Password);
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error autenticando al usuario. " + ex.Message);
                        return;
                    }

                    if (autenticado)
                    {
                        if (ValidarConfiguracion())
                        {
                            IngresoPermitidoSistema();
                        }
                    }
                    else
                    {
                        numeroIntentos++;

                        this.mostrarMensaje(Mensaje.Accion.ContrasenaInvalida);

                        Password = string.Empty;

                        if (numeroIntentos == intentosPermitidos)
                        {
                            this.mostrarAlerta("Se sobrepasó el máximo número de intentos permitido para este usuario.");
                            this.DoClose();
                        }
                    }
                }
            }
        }




        public bool ValidarBaseDeDatos()
        {
            string folder = string.Empty;
#if DEBUG
                       { 
                        if(!FRmConfig.GuardarenSD)
                        {
                            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        }
                        else
                        {
                            //folder = "/sdcard";
                            folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                        }
                        }
#else
            {
                if (!FRmConfig.GuardarenSD)
                {
                    folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                else
                {
                    //folder = "/sdcard";
                    folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                }
            }
#endif

                       Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
            if (file.Length() == 0)
            {

                this.mostrarMensaje(Mensaje.Accion.NoExisteBD, "No existe base de datos. ¿Desea proceder a crearla?", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        string errorMSJ = string.Empty;
                        if (GestorSincronizar.CrearBaseDatos(this.Contexto, FRmConfig.NombreHandHeld, ref errorMSJ))
                        {
                            if (string.IsNullOrEmpty(errorMSJ))
                            {
                                this.mostrarMensaje(Mensaje.Accion.Informacion, "La carga finalizó con éxito.", result2 =>
                                    ValidarBaseDeDatos());
                            }
                            else
                            {
                                this.mostrarAlerta(errorMSJ);
                            }
                        }
                        else
                        {
                            this.mostrarAlerta(errorMSJ);
                        }
                    }
                    //La BD no existe y el usuario no procedio a crearla    
                });
                return false;
            }
            try
            {
                GestorDatos.Conectar();
                return true;
            }
            catch (Exception)
            {
                this.mostrarAlerta("No se pudo conectar a la base de datos.");
                return false;
            }
        }

        public bool ValidarExistBaseDeDatos()
        {
            string folder = string.Empty;
#if DEBUG
                       { 
                        if(!FRmConfig.GuardarenSD)
                        {
                            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        }
                        else
                        {
                            //folder = "/sdcard";
                            folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                        }
                        }
#else
            {
                if (!FRmConfig.GuardarenSD)
                {
                    folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                else
                {
                    //folder = "/sdcard";
                    folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                }
            }
#endif

                       Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
            //var a = System.IO.Path.Combine(FRmConfig.FullAppPath, GestorDatos.BaseDatos + ".db");
            //var path = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
            //path = Path.Combine(path, GestorDatos.BaseDatos + ".db");            
            if (file.Length() == 0)
            //if (File.Exists(path))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void CrearBD()
        {
            string folder = string.Empty;
#if DEBUG
                       { 
                        if(!FRmConfig.GuardarenSD)
                        {
                            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        }
                        else
                        {
                            //folder = "/sdcard";
                            folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                        }
                        }
#else
            {
                if (!FRmConfig.GuardarenSD)
                {
                    folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                else
                {
                    //folder = "/sdcard";
                    folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                }
            }
#endif

                       Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
            //var a = System.IO.Path.Combine(FRmConfig.FullAppPath, GestorDatos.BaseDatos + ".db");
            //var path = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
            //path = Path.Combine(path, GestorDatos.BaseDatos + ".db");            
            if (file.Length() == 0)
            //if (File.Exists(path))
            {
                string errorMSJ = string.Empty;
                if (GestorSincronizar.CrearBaseDatos(this.Contexto, FRmConfig.NombreHandHeld, ref errorMSJ))
                {
                    if (string.IsNullOrEmpty(errorMSJ))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "La carga finalizó con éxito.", result2 =>
                            CrearBD());
                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, " Se produjo un error. " +
                            errorMSJ, res =>
                            {
                                if (DialogResult.Yes == res || DialogResult.OK == res)
                                {
                                   // CrearBD();
                                }
                            });
                            
                    }
                }
                else
                {
                    this.mostrarMensaje(Mensaje.Accion.Informacion, " Se produjo un error. " +
                            errorMSJ, res =>
                            {
                                if (DialogResult.Yes == res || DialogResult.OK == res)
                                {
                                    //CrearBD();
                                }
                            });
                }
            }
            try
            {
                GestorDatos.Conectar();
            }
            catch (Exception)
            {
                this.mostrarAlerta("No se pudo conectar a la base de datos.");
            }
        }

        private bool ValidarConfiguracion()
        {
            bool exito = false;
            string mensaje = string.Empty;
            HandHeldConfig config = new HandHeldConfig();

            exito = config.cargarConfiguracionHandHeld(ref mensaje);
            if (exito)
                exito = config.cargarConfiguracionGlobalHandHeld(ref mensaje);
            if (!exito)
            {
                this.mostrarAlerta(mensaje);
            }

            return exito;
        }

        private void IngresoPermitidoSistema()
        {
            if (MenuDatosViewModel.CargaParametros(false, this))
            {
                FRdConfig.UsaEnvases = true;
                //this.DoClose();  
                if (!FRdConfig.UsaRubrosJornada || (FRdConfig.UsaRubrosJornada && Jornada.ValidarJornada()))
                {
                    bDispose = true;
                    this.DoClose();
                    this.RequestNavigate<MenuPrincipalViewModel>(true);
                }
                else
                {
                    bDispose = true;
                    this.DoClose();                    
                    this.RequestNavigate<RubrosJornadaViewModel>();                    
                }
                // mvega: estas propiedades nunca se usan, por eso no se crean
                //principal.User = ltbUsuario.Text;
                //principal.Password = ltbContrasenna.Text;
                //this.DoClose(); // mvega: cierra el View, pues nunca más se vuelve a utilizar
                //this.RequestClose(this);
            }
        }

        public bool VerificarExistExactusXML()
        {
#if DEBUG
                                   // string folder = "/sdcard";
            string folder =  Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
            Java.IO.File file = new Java.IO.File(folder, "Config.xml");
            if (file.Length() != 0)
            {
                LeerArchivoConfiguracion();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EsenSD()
        {
            return FRmConfig.GuardarenSD;
        }

        public void BorrarExactusXML()
        {
            this.mostrarMensaje(Mensaje.Accion.Decision, " eliminar el archivo Config.xml", r1 =>
            {
                if (r1.Equals(DialogResult.Yes))
                {
#if DEBUG
                                    //string folder = "/sdcard";
                    string folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
                    string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
                    Java.IO.File file = new Java.IO.File(folder, "Config.xml");
                    if (file.Length() != 0)
                    {
                        bool res = file.Delete();
                        if (res)
                            this.mostrarAlerta("El Archivo se borro corectamente");
                    }
                }
            });

        }

        public void CambiarConexion(SQLiteConnection cnx)
        {
            if (GestorDatos.cnx == null)
            {
                GestorDatos.cnx = cnx;
            }
        }

        public void BorrarBaseDatos()
        {
            if (VerificarExistExactusXML())
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, " eliminar la base de datos", r1 =>
                {
                    if (r1.Equals(DialogResult.Yes))
                    {
                        string folder = string.Empty;
#if DEBUG
                   { 
                        if(!FRmConfig.GuardarenSD)
                        {
                            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        }
                        else
                        {
                            //folder = "/sdcard";
                            folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                        }
                    }
#else
                        {
                            if (!FRmConfig.GuardarenSD)
                            {
                                folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                            }
                            else
                            {
                                //folder = "/sdcard";
                                folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                            }
                        }
#endif

                   Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
                        if (file.Length() != 0)
                        {
                            bool res = file.Delete();
                            if (res)
                                this.mostrarAlerta("La Base de Datos se borró corectamente");
                        }
                    }
                });
            }
            else
            {
                this.mostrarAlerta("No es posible leer la ubicación de la BD del archivo Config.xml");
            }

        }

        public override void DoClose()
        {
            //Do Nothing;
        }

        //public override bool CanClose()
        //{
        //    return false;
        //}

        #endregion

        #region Comandos y Acciones

        //public ICommand ComandoLogin
        //{
        //    get { return new MvxRelayCommand(ValidarExactusXml); }
        //}

        #endregion
    }
}