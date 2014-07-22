using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.ViewModels;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ExactusXMLViewModel : BaseViewModel
    {
        private string nombreServer;
        public string NombreServer
        {
            get { return nombreServer; }
            set
            {
                if (value != nombreServer)
                {
                    nombreServer = value;
                    RaisePropertyChanged("NombreServer");
                }
            }
        }
        private string dominio;
        public string Dominio
        {
            get { return dominio; }
            set
            {
                if (value != dominio)
                {
                    dominio = value;
                    RaisePropertyChanged("Dominio");
                }
            }
        }
        private string nombreHandHeld;
        public string NombreHandHeld
        {
            get { return nombreHandHeld; }
            set
            {
                if (value != nombreHandHeld)
                {
                    nombreHandHeld = value;
                    RaisePropertyChanged("NombreHandHeld");
                }
            }
        }

        private bool guardarenSD;
        public bool GuardarenSD
        {
            get { return guardarenSD; }
            set
            {
                guardarenSD = value;
                RaisePropertyChanged("GuardarenSD");
               
            }
        }


        public ExactusXMLViewModel()
        {
        }

        public void CrearXML() 
        {
            string xml=string.Empty;
            #region Version XMLWritter


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = ASCIIEncoding.UTF8;
            settings.OmitXmlDeclaration = true;
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw,settings))
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
                    xw.WriteString(NombreServer);
                    xw.WriteEndElement();
                    xw.WriteStartElement("Dominio");
                    xw.WriteString(Dominio);
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("HandHeld");
                    xw.WriteStartElement("Nombre");
                    xw.WriteString(NombreHandHeld);
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("GuardarenSD");
                    xw.WriteStartElement("Valor");
                    if(GuardarenSD)
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
                Java.IO.File file = new Java.IO.File(folder,"Config.xml");
                if (file.Length() == 0) 
                {
                    Java.IO.FileWriter writer = new Java.IO.FileWriter(file,false);
                    writer.Write(xml);
                    writer.Close();                    
                }
            }
            #endregion
        }

        public void Regresando() 
        {
            #if DEBUG
                        //string folder = "/sdcard";
            string folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
            #else
                string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            #endif
                Java.IO.File file = new Java.IO.File(folder,"Config.xml");
                if (file.Length() == 0)
                {
                    this.mostrarAlerta("No puede continuar hasta haber creado el Archivo Config.xml");
                }
                else 
                {
                    this.DoClose();
                }
        }

        public void CambiarConexion(SQLiteConnection cnx) 
        {
            if (GestorDatos.cnx == null)
            {
                GestorDatos.cnx = cnx;
            }
        }

        public bool Verificar() 
        {
            return string.IsNullOrEmpty(NombreServer) || string.IsNullOrEmpty(NombreHandHeld);
        }

        public void Aceptar() 
        {
            if (Verificar())
            {
                this.mostrarAlerta("Debe llenar todos los Campos");
            }
            else 
            {
                this.CrearXML();
                this.DoClose();
                RequestNavigate<LoginViewModel>();
            }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }
    }
}