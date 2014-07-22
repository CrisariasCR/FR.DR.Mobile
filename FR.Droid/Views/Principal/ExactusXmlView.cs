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
using System.Data.SQLite;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "Creacion XML", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ExactusXmlView : MvxBindingActivityView<ExactusXMLViewModel>
    {
        RadioButton rdSi;
        RadioButton rdNo;
        ImageButton btnAceptar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ExactusXML);
            rdSi = (RadioButton)this.FindViewById(Resource.Id.rdSi);
            rdNo = (RadioButton)this.FindViewById(Resource.Id.rdNo);
            btnAceptar = (ImageButton)this.FindViewById(Resource.Id.btnAceptar);            

            rdSi.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdSi_CheckedChange);
            rdNo.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdNo_CheckedChange);
            btnAceptar.Click += new EventHandler(btnAceptar_Click);
        }

        protected override void OnViewModelSet()
        {            
        }

        protected override void OnStart()
        {
            base.OnStart();            
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
        }

        void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!ViewModel.Verificar())
            {
                if (rdSi.Checked)
                {
                    //string folder = "/sdcard";
                    string folder = Util.ObtenerRutaSD();
                    Java.IO.File dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
                    if (!dir.Exists())
                    {
                        ViewModel.mostrarAlerta("No ha insertado un dispositivo externo!", res =>
                            {
                                return;
                            });
                    }
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
            ViewModel.Aceptar();
        }        


        public override void OnBackPressed()
        {
            ViewModel.Regresando();
        }


        private void rdNo_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.GuardarenSD = false;
                
            }
        }

        private void rdSi_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.GuardarenSD = true;
                
            }
        }
    }
}