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
using Softland.ERP.FR.Mobile.ViewModels;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Xml;
using System.IO;
using FR.Droid.CustomViews;

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "Creacion XML",NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ExactusXmlViewX : Activity
    {
        RadioButton rdSi;
        RadioButton rdNo;
        ImageButton btnAceptar;
        EditText txtServer, txtHandHeld, txtDominio;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ExactusXMLX);
            rdSi = (RadioButton)this.FindViewById(Resource.Id.rdSi);
            rdNo = (RadioButton)this.FindViewById(Resource.Id.rdNo);
            txtServer = (EditText)this.FindViewById(Resource.Id.txtServer);
            txtHandHeld = (EditText)this.FindViewById(Resource.Id.txtHandHeld);
            txtDominio = (EditText)this.FindViewById(Resource.Id.txtDominio);
            btnAceptar = (ImageButton)this.FindViewById(Resource.Id.btnAceptar);                        
        }

        protected override void OnStart()
        {
            base.OnStart();            
        }

        protected override void OnPause()
        {
            btnAceptar.Click -= new EventHandler(btnAceptar_Click);
            base.OnPause();
        }

        protected override void OnResume()
        {
            btnAceptar.Click += new EventHandler(btnAceptar_Click);
            base.OnResume();
        }

        void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!((string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtHandHeld.Text))))
            {
                if (rdSi.Checked)
                {
                    //string folder = "/sdcard";
                    string folder = Util.ObtenerRutaSD();
                    Java.IO.File dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
                    if (!dir.Exists())
                    {
                        ShowMessage sm = new ShowMessage(this, "No ha insertado un dispositivo externo!", true);
                        RunOnUiThread(() => { sm.Mostrar(); return; });                        
                    }
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
            this.Aceptar();
        }        


        public override void OnBackPressed()
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
                ShowMessage ms=new ShowMessage(this,"No puede continuar hasta haber creado el Archivo Config.xml",false);
                RunOnUiThread(() => ms.Mostrar());
            }
            else
            {
                base.OnBackPressed();
            }
        }

        #region Métodos

        /// <summary>
        /// Cambia el source de la conexión
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
        /// Acepta
        /// </summary>
        public void Aceptar()
        {
            if (string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtHandHeld.Text))
            {
                ShowMessage ms = new ShowMessage(this, "Debe llenar todos los campos", false);
                RunOnUiThread(() => ms.Mostrar());
            }
            else
            {
                this.CrearXML();
                var intent = new Intent(this, typeof(LoginViewX));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                this.StartActivity(intent);
            }
        }

        /// <summary>
        /// Crea el xml
        /// </summary>
        public void CrearXML()
        {
            string xml = string.Empty;
            #region Version XMLWritter


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = ASCIIEncoding.UTF8;
            settings.OmitXmlDeclaration = true;
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, settings))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("configuracion");

                    xw.WriteStartElement("BasesDeDatos");
                    xw.WriteStartElement("BaseDeDatos");
                    xw.WriteStartElement("Nombre");
                    xw.WriteString("FRM600");
                    xw.WriteEndElement();
                    xw.WriteStartElement("Compañia");
                    xw.WriteString("ERPADMIN");
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("ServidoresWeb");
                    xw.WriteStartElement("ServidorWeb");
                    xw.WriteStartElement("Nombre");
                    xw.WriteString(txtServer.Text);
                    xw.WriteEndElement();
                    xw.WriteStartElement("Dominio");
                    xw.WriteString(txtDominio.Text);
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("HandHeld");
                    xw.WriteStartElement("Nombre");
                    xw.WriteString(txtHandHeld.Text);
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("GuardarenSD");
                    xw.WriteStartElement("Valor");
                    if (rdSi.Checked)
                        xw.WriteString("Si");
                    else
                        xw.WriteString("No");
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteEndDocument();
                    xw.Close();

                }
                xml = sw.ToString();
                xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + xml;
#if DEBUG
                //string folder = "/sdcard";
                string folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
                string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
                Java.IO.File file = new Java.IO.File(folder, "Config.xml");
                if (file.Length() == 0)
                {
                    Java.IO.FileWriter writer = new Java.IO.FileWriter(file, false);
                    writer.Write(xml);
                    writer.Close();
                }
            }
            #endregion
        }       

        #endregion
    }
}