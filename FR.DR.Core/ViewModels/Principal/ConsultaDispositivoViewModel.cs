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

using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;
using FR.Core.Model;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaDispositivoViewModel : BaseViewModel
    {
#pragma warning disable 649

        public ConsultaDispositivoViewModel()
        {
            corporacion = new Corporacion();
            corporacion.Cargar();
            CargarPropiedades();
            
        }

        private void CargarPropiedades() 
        {
            try
            {
                fecha = GestorUtilitario.ObtenerFechaActual();
                hora = GestorUtilitario.ObtenerHoraActual();
                handheld = Ruta.NombreDispositivo();
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(String.Format("Problemas cargando la información del Dispositivo", ex.Message));
            }
        }

        #region Propiedades       
        
        private Corporacion corporacion; 
        public Corporacion Corporacion
        {
            get { return corporacion; }
            //set { corporacion = value; RaisePropertyChanged("Corporacion"); }
        }
        private string fecha;
        public string Fecha
        {
            get { return fecha; }
            //set { fecha = value; RaisePropertyChanged("Fecha"); }
        }
        private string hora;
        public string Hora
        {
            get { return hora; }
            //set { hora = value; RaisePropertyChanged("Hora"); }
        }
        private string handheld;
        public string HandHeld
        {
            get { return handheld; }
            //set { handheld = value; RaisePropertyChanged("HandHeld"); }
        }
        private string codigoArea;
        public string CodigoArea
        {
            get { return codigoArea; }
            //set { codigoArea = value; RaisePropertyChanged("CodigoArea"); }
        }
        private string version;
        public string Version
        {
            get { return version; }
            set { version = value; RaisePropertyChanged("Version"); }
        }

        #endregion
    }
}