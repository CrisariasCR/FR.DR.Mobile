using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FR.Droid
{
    public static class Util
    {
        public static Connection cnxDefault() 
        {
            if (Softland.ERP.FR.Mobile.App.enSD())
            {
                string folder = Util.ObtenerRutaSD();
                var path = System.IO.Path.Combine(folder, "FRM600.db");
                return new Connection(path);
            }
            else
            {
                string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var path = System.IO.Path.Combine(folder, "FRM600.db");
                return new Connection(path);
            }
        }

        public static string ObtenerRutaSD()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            if (new  Java.IO.File("/storage/sdcard").Exists())
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