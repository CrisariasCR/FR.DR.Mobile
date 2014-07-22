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
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "Facturación de Rutero", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class LoginView : MvxBindingActivityView<LoginViewModel>
    {
        public static bool reportesCreados = false;
        Button btnAceptar;
        EditText txtUser;
        EditText txtPass;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //0: a typical phone screen (480x800 hdpi, etc).
            //1: a typical phone screen (240x320 ldpi, 320x480 mdpi).
            //2: a tweener tablet like the Streak (480x800 mdpi).
            //3: a 7” tablet (600x1024 mdpi).
            //4: a 10” tablet (720x1280 mdpi, 800x1280 mdpi, etc).  
            //5: a typical phone screen (720x1280 xhdpi, etc).
            switch (ResolutionsDisp.Resolucion(WindowManager))
            {
                case 0: SetContentView(Resource.Layout.LoginHDPI); break;
                case 1: SetContentView(Resource.Layout.LoginHDPI); break;
                case 2: SetContentView(Resource.Layout.LoginT7); break;
                case 3: SetContentView(Resource.Layout.LoginT7); break;
                case 4: SetContentView(Resource.Layout.LoginT7); break;
                case 5: SetContentView(Resource.Layout.LoginXHDPI); break;
            }
            ViewModel.Contexto = this;
            CopiarReportes();
            btnAceptar = (Button)this.FindViewById(Resource.Id.btnAceptar);
            txtUser = (EditText)this.FindViewById(Resource.Id.txtUser);
            txtPass = (EditText)this.FindViewById(Resource.Id.txtPass);           
            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);            
        }

        protected override void OnViewModelSet()
        {
           
        }

        public void btnAceptar_Click(object sender, EventArgs e)
        {
            this.LoginProcess();
        }

        public void LoginProcess() 
        {
            if (ViewModel.VerificarExistExactusXML())
            {
                if (ViewModel.EsenSD())
                {
                    //string folder = "/sdcard";
                    string folder = Util.ObtenerRutaSD();
                    var path = System.IO.Path.Combine(folder, "FRM600.db");
                    Connection conn = new Connection(path);
                    ViewModel.CambiarConexion(conn);
                }
                else
                {
                    string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var path = System.IO.Path.Combine(folder, "FRM600.db");
                    Connection conn = new Connection(path);
                    ViewModel.CambiarConexion(conn);
                }
            }
            if (ViewModel.ValidarExactusXml())
            {
                if (!ViewModel.ValidarExistBaseDeDatos())
                {
                    string folder = string.Empty;
#if DEBUG
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

                        ViewModel.mostrarMensaje(Softland.ERP.FR.Mobile.Mensaje.Accion.NoExisteBD, "No existe base de datos. ¿Desea proceder a crearla?", result =>
                        {
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                CrearBDProcess();
                            }
                        });
                    }
                }
                else
                {
                    ViewModel.ValidarIngreso();
                }
            }
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
                    ViewModel.BorrarExactusXML();
                    return true;
                case 2:
                    ViewModel.BorrarBaseDatos();
                    return true;
                default:
                    return true;
            }
        }

        public void CrearBDProcess()
        {
            ViewModel.intentos = 0;
            var progressDialog = Android.App.ProgressDialog.Show(this, "Creando Base Datos", "Por favor espere...", true);
            new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(5000);
                ViewModel.CrearBD();
                RunOnUiThread(() => progressDialog.Hide());

            })).Start();
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
                Java.IO.File Dir2 = new Java.IO.File(folder+"2");
                Java.IO.File Dir3 = new Java.IO.File(folder+"3");
                if (!Dir.Exists())
                    Dir.Mkdirs();
                if (!Dir2.Exists())
                    Dir2.Mkdirs();
                if (!Dir3.Exists())
                    Dir3.Mkdirs();

                for (int it = 1; it <= 3; it++)
                {
                    for (int i = 1; i <= 18; i++)
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                                        path = System.IO.Path.Combine(folder+"2", name);
                                        Java.IO.File file = new Java.IO.File(folder+"2", name);
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
                                        path = System.IO.Path.Combine(folder+"3", name);
                                        Java.IO.File file = new Java.IO.File(folder+"3", name);
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
                        }

                    }
                }
                reportesCreados = true;
            }

        }

        protected override void OnPause()
        {
            btnAceptar.Click -= new EventHandler(btnAceptar_Click);
            base.OnPause();
        }

        protected override void OnStop()
        {            
            base.OnStop();
            if (ViewModel.bDispose)
                this.Finish();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();            
        }

        protected override void OnResume()
        {
            btnAceptar.Click += new EventHandler(btnAceptar_Click);
            base.OnResume();            
        }


        public override void OnBackPressed()
        {
            //ViewModel.DoClose();
            this.MoveTaskToBack(true);
        }

    }
}
