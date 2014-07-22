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

using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Binding.Droid;

using Softland.ERP.FR.Mobile;
using System.Data.SQLite;
using FR.DR.Core.Converters;

namespace FR.Droid
{
    public class Setup : MvxBaseAndroidBindingSetup
    {
        public Setup(Context applicationContext)
            : base(applicationContext)
        {
        }

        protected override MvxApplication CreateApp()
        {
#if DEBUG
            //string folder = "/sdcard";
            string folder = Util.ObtenerRutaSD();
#else
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif

            var path = System.IO.Path.Combine(folder, "FRM600.db");
            Connection conn = new Connection(path);
            Cirrious.MvvmCross.Binding.MvxBindingTrace.TraceBindingLevel = Cirrious.MvvmCross.Interfaces.Platform.Diagnostics.MvxTraceLevel.Diagnostic;

            return new App(conn);
        }

        public static FR.Droid.NativeServices.MessageDisplayer messageHandler;
        protected override void InitializeLastChance()
        {
            messageHandler = new FR.Droid.NativeServices.MessageDisplayer(ApplicationContext);

            //Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();
            ////Cirrious.MvvmCross.Plugins.Visibility.PluginLoader.Instance.EnsureLoaded();
            //Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            ////Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader.Instance.EnsureLoaded();
            base.InitializeLastChance();
        }
        protected override IEnumerable<Type> ValueConverterHolders
        {
            get
            {
                return new[] {typeof (Converters)}; 
            }
        }


    }
}