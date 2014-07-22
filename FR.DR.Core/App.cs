using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;

using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using System.Data.SQLiteBase;

using Android.App;

using Softland.MvvmCross.MessageService;
using Softland.MvvmCross.TinyMessenger;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls;

namespace Softland.ERP.FR.Mobile
{
    public class App : MvxApplication, IMvxServiceProducer
    {
        public App(SQLiteConnection cnx)
        {
            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.ResourceLoader.PluginLoader.Instance.EnsureLoaded();

            StartApplicationObject start = new StartApplicationObject();
            this.RegisterServiceInstance<IMvxStartNavigation>(start);

           GestorDatos.cnx = null;

            // inicializa el servicio de Mesages Visuales (messageboxs)
            var messageApplicationObject = new MessageApplicationObject();
            this.RegisterServiceInstance<IMessageReporter>(messageApplicationObject);
            this.RegisterServiceInstance<IMessageSource>(messageApplicationObject);

            var tinyMessenger = new TinyMessengerHub();
            this.RegisterServiceInstance<ITinyMessengerHub>(tinyMessenger);

            //Carga Globales FR
            //FRdConfig.CargarGlobales();
        }

        // para tener control de la actividad actual/current para los Mensajes!
        private static Activity currentActivity = null;

        public static Activity getCurrentActivity()
        {
            return currentActivity;
        }               

        public static void setCurrentActivity(Activity _currentActivity)
        {
            currentActivity = _currentActivity;
            if (!currentActivity.Title.Contains("(SYNC) "))
                currentActivity.Title = prefijo + currentActivity.Title;
        }

        private static string prefijo = string.Empty;

        public static string Prefijo 
        {
            get { return prefijo; }
            set 
            { 
                prefijo = value;
                if (!string.IsNullOrEmpty(prefijo))
                {
                    if (!currentActivity.Title.Contains("(SYNC) "))
                        currentActivity.Title = prefijo + currentActivity.Title;
                }
                else
                {                    
                    currentActivity.Title = currentActivity.Title.Replace("(SYNC) ", prefijo);
                }
            }
        }

        public static void VerificarConexionBaseDatos(SQLiteConnection cnx) 
        {
            try
            {
                if (GestorDatos.cnx == null)
                {
                    GestorDatos.cnx = cnx;
                }
                else
                {
                    cnx.Dispose();
                }
                GestorDatos.Conectar();
            }
            catch (Exception ex)
            {
                //Do Nothing for the moment.
            }
        }

        public static bool enSD() 
        {
            return FRmConfig.GuardarenSD;
        }

        public static string ObtenerRutaSD()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            if (new Java.IO.File("/storage/sdcard").Exists())
            {
                path = "/storage/sdcard";
            }
            else if (new Java.IO.File("/storage/sdcard0").Exists())
            {
                path = "/storage/sdcard0";
            }
            else if (new Java.IO.File("/storage/sdcard1").Exists())
            {
                path = "/storage/sdcard1";
            }
            return path;

        }

    }
}
