using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using System.Windows.Input;
using FR.DR.Core.Helper;
using EMF.GPS;
using Softland.ERP.FR.Mobile;
using Cirrious.MvvmCross.Commands;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class UbicacionClienteViewModel : DialogViewModel<bool>
    {
        public UbicacionClienteViewModel(string messageId)
            : base(messageId)
        {
            CargaInicial();
        }


        #region Propiedades

        //Propiedades GPS

        public double latidud;
        public double longitud;
        public double altitud;
        public DateTime gpsTime=DateTime.Now;
        public bool gpsOff = true;

        private string mensaje;
        public string Mensaje
        {
            get { return mensaje; }
            set { mensaje = value; RaisePropertyChanged("Mensaje"); }
        }

        #endregion

        #region Metodos Logica

        private void CargaInicial()
        {
            Mensaje = "Obteniendo Ubicación...";
        }

        /// <summary>
        /// Guarda la ubicación seleccionada.
        /// </summary>
        /// <param name="pos"></param>
        public void GuardarUbicacionCliente()
        {
            try
            {
                GpsPosition androidGPR = new GpsPosition();
                androidGPR.Latitude = this.latidud;
                androidGPR.Longitude = this.longitud;
                androidGPR.SeaLevelAltitude = this.altitud;
                androidGPR.Time = this.gpsTime;
                ClienteUbicacion cu = new ClienteUbicacion(GlobalUI.ClienteActual, androidGPR, Ubicacion.Error);
                cu.Guardar();
                this.mostrarMensaje(Mobile.Mensaje.Accion.Informacion,string.Format( "Ubicación almacenda correctamente. Los valores obtenidos son Latitud: {0}, Longintud: {1}, Altitud: {2}"
                     , androidGPR.Latitude, androidGPR.Longitude, androidGPR.SeaLevelAltitude), res =>
                     { 
                         this.DoClose();
                     });
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mobile.Mensaje.Accion.Informacion, string.Format("Error al guardar Ubicación. "+ex.Message), res =>
                     {
                         this.DoClose();
                     });
            }
        }

        public void regresar() 
        {
            this.mostrarMensaje(Mobile.Mensaje.Accion.Decision, " cancelar el proceso", res =>
                {
                    if (res == System.Windows.Forms.DialogResult.Yes || res == System.Windows.Forms.DialogResult.OK)
                    {
                        this.DoClose();
                    }
                });
        }

        

        #endregion
        
    }
}