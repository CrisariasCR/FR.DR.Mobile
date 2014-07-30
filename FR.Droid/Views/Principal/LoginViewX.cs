using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data.SQLite;
using Android.Views.InputMethods;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using Java.IO;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.Seguridad;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRJornada;
using FR.Droid.CustomViews;
using System.Threading.Tasks;

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "Facturación de Rutero",MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class LoginViewX : Activity
    {
        public static bool reportesCreados = false;
        private int intentosPermitidos;
        private int numeroIntentos;
        Button btnAceptar;
        EditText txtUser;
        EditText txtPass;

        #region lifecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.CargarAplicacion();
            //0: a typical phone screen (480x800 hdpi, etc).
            //1: a typical phone screen (240x320 ldpi, 320x480 mdpi).
            //2: a tweener tablet like the Streak (480x800 mdpi).
            //3: a 7” tablet (600x1024 mdpi).
            //4: a 10” tablet (720x1280 mdpi, 800x1280 mdpi, etc).  
            //5: a typical phone screen (720x1280 xhdpi, etc).
            switch (ResolutionsDisp.Resolucion(WindowManager))
            {
                case 0: SetContentView(Resource.Layout.LoginHDPIX); break;
                case 1: SetContentView(Resource.Layout.LoginHDPIX); break;
                case 2: SetContentView(Resource.Layout.LoginT7X); break;
                case 3: SetContentView(Resource.Layout.LoginT7X); break;
                case 4: SetContentView(Resource.Layout.LoginT7X); break;
                case 5: SetContentView(Resource.Layout.LoginXHDPIX); break;
            }
            CopiarReportes();
            btnAceptar = (Button)this.FindViewById(Resource.Id.btnAceptar);
            txtUser = (EditText)this.FindViewById(Resource.Id.txtUser);
            txtPass = (EditText)this.FindViewById(Resource.Id.txtPass);           
            
        }

        protected override void OnStart()
        {
            base.OnStart();            
        }



        protected override void OnPause()
        {
            btnAceptar.Click -= new EventHandler(btnAceptar_Click);
            this.SavePreferences();
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            btnAceptar.Click += new EventHandler(btnAceptar_Click);
            this.LoadPreferences();
            base.OnResume();
        }

        #endregion

        #region Eventos


        public override void OnBackPressed()
        {
            //ViewModel.DoClose();
            this.MoveTaskToBack(true);
        }
        

        public void btnAceptar_Click(object sender, EventArgs e)
        {
            this.LoginProcess();
        }
        

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var MenuItem1 = menu.Add(0, 1, 1, "Borrar Config.xml");
            var MenuItem2 = menu.Add(0, 2, 2, "Borrar Base de Datos");

			MenuItem1.SetIcon(Resource.Drawable.ic_anular);
			MenuItem2.SetIcon(Resource.Drawable.ic_remover);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 1:
                    this.BorrarExactusXML();
                    return true;
                case 2:
                    this.BorrarBaseDatos();
                    return true;
                default:
                    return true;
            }
        }

        #endregion


        #region Metodos

        /// <summary>
        /// Llama al proceso de login
        /// </summary>
        public void LoginProcess()
        {
            if (this.VerificarExistExactusXML())
            {
                if (FRmConfig.GuardarenSD)
                {
                    //string folder = "/sdcard";
                    string folder = Util.ObtenerRutaSD();
                    var path = System.IO.Path.Combine(folder, "FRM600.db");
                    Connection conn = new Connection(path);
                    this.CambiarConexion(conn);
                }
                else
                {
                    string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var path = System.IO.Path.Combine(folder, "FRM600.db");
                    Connection conn = new Connection(path);
                    this.CambiarConexion(conn);
                }
            }
            if (this.ValidarExactusXml())
            {
                if (!this.ValidarExistBaseDeDatos())
                {
                    string folder = string.Empty;
#if DEBUG
                    {
                        if (!FRmConfig.GuardarenSD)
                        {
                            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        }
                        else
                        {
                            folder = Util.ObtenerRutaSD();
                        }
                    }
#else
                    {
                        if (!ViewModel.EsenSD())
                        {
                            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        }
                        else
                        {
                            //folder = "/sdcard";
                            folder = Util.ObtenerRutaSD();
                        }
                    }
#endif
                    Java.IO.File file = new Java.IO.File(folder, Softland.ERP.FR.Mobile.Cls.AccesoDatos.GestorDatos.BaseDatos + ".db");
                    if (file.Length() == 0)
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Atención");
                        alert.SetMessage("No existe la base de datos. ¿Desea proceder a crearla?");
                        alert.SetPositiveButton("Si", delegate { CrearBDProcess(); });
                        alert.SetNegativeButton("No", delegate { return; });
                        RunOnUiThread(() => { alert.Show(); });
                    }
                }
                else
                {
                    this.ValidarIngreso();
                }
            }
        }

        /// <summary>
        /// Verifica la existencia del archivo xml
        /// </summary>
        /// <returns></returns>
        public bool VerificarExistExactusXML()
        {
#if DEBUG
            // string folder = "/sdcard";
            string folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
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

        /// <summary>
        /// Lee el xml de configuración
        /// </summary>
        private void LeerArchivoConfiguracion()
        {
            try
            {
                // This is the full directory and exe name                
#if DEBUG
                FRmConfig.FullAppPath = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
                FRmConfig.FullAppPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
                
                //se lee el xml para conocer las bases de datos del xml
                ConfigurationClass.ReadXml(System.IO.Path.Combine(FRmConfig.FullAppPath, "Config.xml"));
            }
            catch (FileNotFoundException)
            {
                ShowMessage sm = new ShowMessage(this, "No se encuentra el xml de configuración. Verifique que lo tenga instalado.", true);
                RunOnUiThread(() => sm.Mostrar());                
            }
            catch (Exception exc)
            {
                ShowMessage sm = new ShowMessage(this, "No se encuentra el xml de configuración. Verifique que lo tenga instalado." + exc.Message, true);
                RunOnUiThread(() => sm.Mostrar());
            }
        }

        /// <summary>
        /// Cambia la conexion en caso de que alla cambiado el source
        /// </summary>
        /// <param name="cnx"></param>
        public void CambiarConexion(SQLiteConnection cnx)
        {
            if (GestorDatos.cnx == null)
            {
                GestorDatos.cnx = cnx;
            }
        }

        /// <summary>
        /// Valida el ExactusXML
        /// </summary>
        /// <returns></returns>
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
                Intent intent = new Intent(this, typeof(ExactusXmlViewX));
                this.StartActivity(intent);
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// VAlida si existe la base de datos
        /// </summary>
        /// <returns></returns>
        public bool ValidarExistBaseDeDatos()
        {
            string folder = string.Empty;
#if DEBUG
            {
                if (!FRmConfig.GuardarenSD)
                {
                    folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                else
                {                    
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
                    folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                }
            }
#endif

            Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
            if (file.Length() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Método encargado de copiar los reportes al folder de la aplicacion.
        /// </summary>
        public void CopiarReportes()
        {
            if (!reportesCreados)
            {
#if DEBUG
                // string folder = "/sdcard";
                string folder = folder = Util.ObtenerRutaSD();
#else
                string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif

                folder += Java.IO.File.Separator + "Reportes";
                //Crea el directorio si no existe
                Java.IO.File Dir = new Java.IO.File(folder);
                Java.IO.File Dir2 = new Java.IO.File(folder + "2");
                Java.IO.File Dir3 = new Java.IO.File(folder + "3");
                if (!Dir.Exists())
                    Dir.Mkdirs();
                if (!Dir2.Exists())
                    Dir2.Mkdirs();
                if (!Dir3.Exists())
                    Dir3.Mkdirs();

                for (int it = 1; it <= 3; it++)
                {
                    for (int i = 1; i <= 19; i++)
                    {
                        string name = string.Empty;
                        string nameAs = string.Empty;
                        string path = string.Empty;
                        switch (i)
                        {
                            case 1:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "DetalleDevolucion.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "DetalleDevolucion.rdl";
                                        nameAs = "DetalleDevolucion_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "DetalleDevolucion.rdl";
                                        nameAs = "DetalleDevolucion_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 2:
                                {
                                    //Detalle Devolucion   
                                    if (it == 1)
                                    {
                                        name = "DetalleRecibos.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "DetalleRecibos.rdl";
                                        nameAs = "DetalleRecibos_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "DetalleRecibos.rdl";
                                        nameAs = "DetalleRecibos_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 3:
                                {
                                    //Detalle Devolucion   
                                    if (it == 1)
                                    {
                                        name = "Factura.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "Factura.rdl";
                                        nameAs = "Factura_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "Factura.rdl";
                                        nameAs = "Factura_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 4:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "Pedido.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "Pedido.rdl";
                                        nameAs = "Pedido_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "Pedido.rdl";
                                        nameAs = "Pedido_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 5:
                                {
                                    //Detalle Devolucion      
                                    if (it == 1)
                                    {
                                        name = "ReporteCierre.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteCierre.rdl";
                                        nameAs = "ReporteCierre_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteCierre.rdl";
                                        nameAs = "ReporteCierre_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 6:
                                {
                                    //Detalle Devolucion  
                                    if (it == 1)
                                    {
                                        name = "ReporteDevoluciones.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteDevoluciones.rdl";
                                        nameAs = "ReporteDevoluciones_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteDevoluciones.rdl";
                                        nameAs = "ReporteDevoluciones_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 7:
                                {
                                    //Detalle Devolucion
                                    if (it == 1)
                                    {
                                        name = "ReporteInventario.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteInventario.rdl";
                                        nameAs = "ReporteInventario_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteInventario.rdl";
                                        nameAs = "ReporteInventario_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 8:
                                {
                                    //Detalle Devolucion 
                                    if (it == 1)
                                    {
                                        name = "ReporteInventarioTomaFisica.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteInventarioTomaFisica.rdl";
                                        nameAs = "ReporteInventarioTomaFisica_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteInventarioTomaFisica.rdl";
                                        nameAs = "ReporteInventarioTomaFisica_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 9:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "ReporteJornada.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteJornada.rdl";
                                        nameAs = "ReporteJornada_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteJornada.rdl";
                                        nameAs = "ReporteJornada_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 10:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "ReporteMontos.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteMontos.rdl";
                                        nameAs = "ReporteMontos_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteMontos.rdl";
                                        nameAs = "ReporteMontos_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 11:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "ReporteVisita.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteVisita.rdl";
                                        nameAs = "ReporteVisita_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteVisita.rdl";
                                        nameAs = "ReporteVisita_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 12:
                                {
                                    //Detalle Devolucion    
                                    if (it == 1)
                                    {
                                        name = "ResumenDevoluciones.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ResumenDevoluciones.rdl";
                                        nameAs = "ResumenDevoluciones_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ResumenDevoluciones.rdl";
                                        nameAs = "ResumenDevoluciones_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 13:
                                {
                                    //Detalle Devolucion         
                                    if (it == 1)
                                    {
                                        name = "ResumenFacturas.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ResumenFacturas.rdl";
                                        nameAs = "ResumenFacturas_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ResumenFacturas.rdl";
                                        nameAs = "ResumenFacturas_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 14:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "ResumenPedidos.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ResumenPedidos.rdl";
                                        nameAs = "ResumenPedidos_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ResumenPedidos.rdl";
                                        nameAs = "ResumenPedidos_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 15:
                                {
                                    //Detalle Devolucion                        
                                    if (it == 1)
                                    {
                                        name = "ResumenRecibos.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ResumenRecibos.rdl";
                                        nameAs = "ResumenRecibos_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ResumenRecibos.rdl";
                                        nameAs = "ResumenRecibos_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 16:
                                {
                                    //Detalle Devolucion         
                                    if (it == 1)
                                    {
                                        name = "VentaConsignacion.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "VentaConsignacion.rdl";
                                        nameAs = "VentaConsignacion_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "VentaConsignacion.rdl";
                                        nameAs = "VentaConsignacion_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 17:
                                {
                                    //Detalle Devolucion   
                                    if (it == 1)
                                    {
                                        name = "Garantia.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "Garantia.rdl";
                                        nameAs = "Garantia_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "Garantia.rdl";
                                        nameAs = "Garantia_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
                            case 18:
                                {
                                    //Detalle Devolucion   
                                    if (it == 1)
                                    {
                                        name = "ReporteLiquidacionInventario.rdl";
                                        path = System.IO.Path.Combine(folder, name);
                                        Java.IO.File file = new Java.IO.File(folder, name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(name))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 2)
                                    {
                                        name = "ReporteLiquidacionInventario.rdl";
                                        nameAs = "ReporteLiquidacionInventario_2.rdl";
                                        path = System.IO.Path.Combine(folder + "2", name);
                                        Java.IO.File file = new Java.IO.File(folder + "2", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    }
                                    if (it == 3)
                                    {
                                        name = "ReporteLiquidacionInventario.rdl";
                                        nameAs = "ReporteLiquidacionInventario_3.rdl";
                                        path = System.IO.Path.Combine(folder + "3", name);
                                        Java.IO.File file = new Java.IO.File(folder + "3", name);
                                        if (file.Length() == 0)
                                        {
                                            Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                                            using (System.IO.Stream stream = this.Assets.Open(nameAs))
                                            {
                                                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                                                string text = reader.ReadToEnd();
                                                writer.Write(text);
                                                writer.Close();
                                            }
                                        }
                                    } break;
                                }
						case 19:
							{
								//Detalle Devolucion   
								if (it == 1)
								{
									name = "ReporteCierreCaja.rdl";
									path = System.IO.Path.Combine(folder, name);
									Java.IO.File file = new Java.IO.File(folder, name);
									if (file.Length() == 0)
									{
										Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
										using (System.IO.Stream stream = this.Assets.Open(name))
										{
											System.IO.StreamReader reader = new System.IO.StreamReader(stream);
											string text = reader.ReadToEnd();
											writer.Write(text);
											writer.Close();
										}
									}
								}
								if (it == 2)
								{
									name = "ReporteCierreCaja.rdl";
									nameAs = "ReporteCierreCaja_2.rdl";
									path = System.IO.Path.Combine(folder + "2", name);
									Java.IO.File file = new Java.IO.File(folder + "2", name);
									if (file.Length() == 0)
									{
										Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
										using (System.IO.Stream stream = this.Assets.Open(nameAs))
										{
											System.IO.StreamReader reader = new System.IO.StreamReader(stream);
											string text = reader.ReadToEnd();
											writer.Write(text);
											writer.Close();
										}
									}
								}
								if (it == 3)
								{
									name = "ReporteCierreCaja.rdl";
									nameAs = "ReporteCierreCaja_3.rdl";
									path = System.IO.Path.Combine(folder + "3", name);
									Java.IO.File file = new Java.IO.File(folder + "3", name);
									if (file.Length() == 0)
									{
										Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
										using (System.IO.Stream stream = this.Assets.Open(nameAs))
										{
											System.IO.StreamReader reader = new System.IO.StreamReader(stream);
											string text = reader.ReadToEnd();
											writer.Write(text);
											writer.Close();
										}
									}
								} break;
							}
                        }

                    }
                }
                reportesCreados = true;
            }

        }

        /// <summary>
        /// Validar el Ingreso
        /// </summary>
        public void ValidarIngreso()
        {

            if (txtUser.Text == string.Empty)
            {
                ShowMessage ms=new ShowMessage(this,"Digite el Usuario",false);
                RunOnUiThread(()=>ms.Mostrar());
            }
            else if (txtPass.Text == string.Empty)
            {
                ShowMessage ms = new ShowMessage(this, "Digite el contraseña", false);
                RunOnUiThread(() => ms.Mostrar());
            }
            else
            {
                txtUser.Text = txtUser.Text.ToUpper();
                bool existeBD = ValidarExistBaseDeDatos();

                if (existeBD)
                {
                    //Carga Globales FR
                    FRdConfig.CargarGlobales();
                    if (txtUser.Text != GestorDatos.NombreUsuario)
                    {
                        GestorDatos.NombreUsuario = txtUser.Text;
                        GestorDatos.ContrasenaUsuario = txtPass.Text.ToUpper();

                        try
                        {
                            //Como el usuario cambio, volvemos a sacar los intentos
                            intentosPermitidos = GestorSeguridad.VerificarUsuario(txtUser.Text);
                        }
                        catch (Exception ex)
                        {
                            ShowMessage sm = new ShowMessage(this, "Verificando el Usuario." + ex.Message, true);
                            RunOnUiThread(() => sm.Mostrar());                            
                            return;
                        }

                        //y ponemos el numero de intentos a cero
                        numeroIntentos = 0;

                        if (intentosPermitidos == -1)
                        {
                            ShowMessage ms = new ShowMessage(this, Mensaje.Accion.UsuarioInvalido.ToString());
                            RunOnUiThread(() => ms.Mostrar());
                            txtPass.Text = string.Empty;
                            txtUser.Text = string.Empty;
                            return;
                        }

                        if (intentosPermitidos == 0)
                        {
                            ShowMessage ms = new ShowMessage(this, "Este usuario no puede ingresar al sistema", false);
                            RunOnUiThread(() => ms.Mostrar());
                            txtUser.Text = string.Empty;
                            txtPass.Text = string.Empty;
                            return;
                        }
                    }

                    bool autenticado;

                    try
                    {
                        autenticado = GestorSeguridad.AutenticarUsuario(txtUser.Text, txtPass.Text);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage sm = new ShowMessage(this, "Autenticando el Usuario." + ex.Message, true);
                        RunOnUiThread(() => sm.Mostrar());                         
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
                        ShowMessage ms = new ShowMessage(this, Mensaje.Accion.ContrasenaInvalida.ToString());
                        RunOnUiThread(() => ms.Mostrar());
                        txtPass.Text = string.Empty;

                        if (numeroIntentos == intentosPermitidos)
                        {
                            ms = new ShowMessage(this, "Se sobrepasó el máximo número de intentos permitido para este usuario.",false);
                            RunOnUiThread(() => ms.Mostrar());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inicializa variables
        /// </summary>
        private void CargarAplicacion()
        {

            //Inicializamos la bitacora
            Bitacora.Inicializar();
            Bitacora.Escribir("\r\nInicio de bitacora " + DateTime.Now + "\r\n");
        }

        /// <summary>
        /// Valida la configuración
        /// </summary>
        /// <returns></returns>
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
                ShowMessage ms = new ShowMessage(this, mensaje,false);
                RunOnUiThread(() => ms.Mostrar());
            }

            return exito;
        }

        /// <summary>
        /// Ingresa al Sistema
        /// </summary>
        private void IngresoPermitidoSistema()
        {
            if (this.CargaParametros(true))
            {
                FRdConfig.UsaEnvases = true;
                //this.DoClose();  
                if (!FRdConfig.UsaRubrosJornada || (FRdConfig.UsaRubrosJornada && Jornada.ValidarJornada()))
                {
                    var intent=new Intent(this,typeof(MainMenuViewX));
                    intent.SetFlags(ActivityFlags.ReorderToFront);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(this, typeof(JornadaViewX));
                    StartActivity(intent);
                }
            }
        }

        /// <summary>
        /// Carga los parámetros globales
        /// </summary>
        /// <param name="logBitacora"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public bool CargaParametros(bool logBitacora)
        {
            string mensaje = string.Empty;
            if (GestorSincronizar.CargaParametrosX(logBitacora))
            {
                try
                {
                    HandHeldConfig config = new HandHeldConfig();
                    //Carga nuevamente los parametros de la Pocket
                    config.cargarConfiguracionHandHeld(ref mensaje);
                    config.cargarConfiguracionGlobalHandHeld(ref mensaje);
                    GlobalUI.Rutas = Ruta.ObtenerRutas();                    

                    //Crea el registro en caso de que no se utilizen jornadas
                    //Si se utilizan Jornadas, el registro se creara hasta que se abra la jornada - KFC
                    if ((!FRdConfig.UsaJornadaLaboral) && (!JornadaRuta.VerificarJornadaAbierta()))
                    {
                        JornadaRuta.AbrirJornada();
                    }

                    // LAS. Mejora Paquetes y Reglas. Liberar los paquetes de las regalias.
                    Regalias.Liberar();
                }
                catch (Exception ex)
                {
                    ShowMessage sm = new ShowMessage(this, "Al cargar los parámetros al dispositivo." + ex.Message, true);
                    RunOnUiThread(() => sm.Mostrar());                      
                    return false;                    
                }
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// Borra el archivo xml de configuración
        /// </summary>
        public void BorrarExactusXML()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Atención");
            alert.SetMessage("¿Desea eliminar el archivo Config.xml?");
            alert.SetPositiveButton("Si", delegate { BorrarXML(); });
            alert.SetNegativeButton("No", delegate { return; });
            RunOnUiThread(() => { alert.Show(); });
        }

        /// <summary>
        /// Borra el archivo xml de configuración 
        /// </summary>
        private void BorrarXML() 
        {
#if DEBUG
            string folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
                    string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
            Java.IO.File file = new Java.IO.File(folder, "Config.xml");
            if (file.Length() != 0)
            {
                bool res = file.Delete();
                if (res)
                {
                    ShowMessage ms = new ShowMessage(this, "El archivo se borro correctamente");
                    RunOnUiThread(() => ms.Mostrar());
                }
            }
        }

        /// <summary>
        /// Borra la base de datos
        /// </summary>
        public void BorrarBaseDatos()
        {
            if (this.VerificarExistExactusXML())
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Atención");
                alert.SetMessage("¿Desea eliminar el archivo de base de datos?");
                alert.SetPositiveButton("Si", delegate { BorrarBD(); });
                alert.SetNegativeButton("No", delegate { return; });
                RunOnUiThread(() => { alert.Show(); });
            }
            else
            {
                ShowMessage ms = new ShowMessage(this, "No es posible leer la ubicación de la BD del archivo Config.xml", false);
                RunOnUiThread(() => ms.Mostrar());
            }

        }

        /// <summary>
        /// Borra la base de datos
        /// </summary>
        private void BorrarBD() 
        {
            string folder = string.Empty;
#if DEBUG
            {
                if (!FRmConfig.GuardarenSD)
                {
                    folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                else
                {
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
                                folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                            }
                        }
#endif

            Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
            if (file.Length() != 0)
            {
                bool res = file.Delete();
                if (res)
                {
                    ShowMessage ms = new ShowMessage(this, "La base de datos se borró corectamente");
                    RunOnUiThread(() => ms.Mostrar());
                }
            }
        }

        /// <summary>
        /// Proceso asincrono para crear la BD
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> CrearBDAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            string x = await Task.Factory.StartNew(() =>
            {
                string folder = string.Empty;
                string error;
#if DEBUG
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
                    if (GestorSincronizar.CrearBaseDatos(this, FRmConfig.NombreHandHeld, ref errorMSJ))
                    {
                        if (string.IsNullOrEmpty(errorMSJ))
                        {
                            error = "true";
                            return error;
                        }
                        else
                        {
                            error = errorMSJ;
                            return error;
                        }
                    }
                    else
                    {
                        error = errorMSJ;
                        return error;
                    }
                }
                try
                {
                    GestorDatos.Conectar();
                }
                catch (Exception ex)
                {
                    error = "No se pudo conectar con la Base de Datos." + ex.Message;
                    return error;
                }
                error = "true";
                return error;
            }, cancellationToken);
            return x;        }

        /// <summary>
        /// Proceso de creacion de BD
        /// </summary>
        async void CrearBDProcess()
        {
            var progressDialog = Android.App.ProgressDialog.Show(this, "Creando Base Datos", "Por favor espere...", true);
            string resultado = await CrearBDAsync();
            if (resultado.Equals("true"))
            {
                resultado = await CrearBDAsync();
                if (resultado.Equals("true"))
                {
                    ShowMessage ms = new ShowMessage(this, "La carga finalizó con éxito.");
                    RunOnUiThread(() => progressDialog.Dismiss());
                    RunOnUiThread(new Java.Lang.Runnable(() => ms.Mostrar()));
                }
                else
                {
                    ShowMessage ms = new ShowMessage(this, resultado, true);
                    RunOnUiThread(() => progressDialog.Dismiss());
                    RunOnUiThread(new Java.Lang.Runnable(() => ms.Mostrar()));
                }
            }
            else
            {
                ShowMessage ms = new ShowMessage(this, resultado, true);
                RunOnUiThread(() => progressDialog.Dismiss());
                RunOnUiThread(new Java.Lang.Runnable(() => ms.Mostrar()));

            }
        }   

        #endregion

        #region Preferencias

        /// <summary>
        /// Guarda las preferencias de la pantalla
        /// </summary>
        private void SavePreferences()
        {
            var prefs = this.GetSharedPreferences("LoginViewX.preferences", FileCreationMode.Private);
            var editor = prefs.Edit();
            editor.PutString("User", txtUser.Text);
            editor.PutString("Pass", txtPass.Text);
            editor.Commit();
        }

        /// <summary>
        /// Carga las preferencias de la pantalla
        /// </summary>
        private void LoadPreferences()
        {
            var prefs = this.GetSharedPreferences("LoginViewX.preferences", FileCreationMode.Private);
            if (prefs.Contains("User"))
            {
                txtUser.Text = prefs.GetString("User", "");
            }
            if (prefs.Contains("Pass"))
            {
                txtPass.Text = prefs.GetString("Pass", "");
            }            
        }


        #endregion

    }
}
