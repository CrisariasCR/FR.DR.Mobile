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
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using System.Threading;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class VisitaViewModel : DialogViewModel<bool>
    {
        public VisitaViewModel(string messageId)
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

        private IObservableCollection<RazonVisita> razonesVisita;
        public IObservableCollection<RazonVisita> RazonesVisita
        {
            get { return razonesVisita; }
            set { razonesVisita = value; RaisePropertyChanged("RazonesVisita"); }
        }

        private RazonVisita razonVisitaSeleccionada;
        public RazonVisita RazonVisitaSeleccionada
        {
            get { return razonVisitaSeleccionada; }
            set { razonVisitaSeleccionada = value; RaisePropertyChanged("RazonVisitaSeleccionada"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set { nombreCliente = value; RaisePropertyChanged("NombreCliente"); }
        }

        public DateTime Fecha { get; set; }

        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }

        private IObservableCollection<TipoVisita> tiposVisita;
        public IObservableCollection<TipoVisita> TiposVisita
        {
            get { return tiposVisita; }
            set { tiposVisita = value; RaisePropertyChanged("TiposVisita"); }
        }

        private TipoVisita tipoVisitaSeleccionado;
        public TipoVisita TipoVisitaSeleccionado
        {
            get { return tipoVisitaSeleccionado; }
            set { tipoVisitaSeleccionado = value; RaisePropertyChanged("TipoVisitaSeleccionado"); }
        }

        private bool tipoVisitaEnabled;
        public bool TipoVisitaEnabled
        {
            get { return tipoVisitaEnabled; }
            set { tipoVisitaEnabled = value; RaisePropertyChanged("TipoVisitaEnabled"); }
        }

        private bool observacionesVisible=false;
        public bool ObservacionesVisible
        {
            get { return observacionesVisible; }
            set { observacionesVisible = value; RaisePropertyChanged("ObservacionesVisible"); }
        }

        private string textoNota;
        public string TextoNota
        {
            get { return textoNota; }
            set { textoNota = value; RaisePropertyChanged("TextoNota"); }
        }

        #endregion

        #region Metodos Logica

        private void CargaInicial()
        {
            try
            {
                try
                {
                    RazonesVisita = new SimpleObservableCollection<RazonVisita>(RazonVisita.ObtenerRazonesVisita());
                    RazonVisitaSeleccionada = RazonesVisita.Count > 0 ? RazonesVisita[0] : null;
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando razones de visita. " + ex.Message);
                }
                
                NombreCliente = GlobalUI.ClienteActual.Nombre;
                Fecha = DateTime.Now;
                HoraInicio = GlobalUI.VisitaActual.FechaInicio;//.TimeOfDay;
                HoraFin = System.DateTime.Now;//.TimeOfDay;

                //Caso: 34430 LJR 22/12/2008 - Tipos Visita
                CargaComboTipoVisita();
                //this.cargaComboEstados();

                TextoNota = GlobalUI.VisitaActual.Nota;
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(ex.Message);
            }
        }

        private void CargaComboTipoVisita()
        {
            TiposVisita = new SimpleObservableCollection<TipoVisita>();

            TiposVisita.Add(TipoVisita.Real);
            TiposVisita.Add(TipoVisita.Telefonica);

            if (FRmConfig.SeleccionXCodBarras)
            {
                if (GlobalUI.VisitaActual.Tipo == TipoVisita.Real)
                {
                    TipoVisitaSeleccionado = TipoVisita.Real;
                }
                else
                {
                    TipoVisitaSeleccionado = TipoVisita.Telefonica;
                }
                TipoVisitaEnabled = false;
            }
        }

        public void Registrar()
        {
            if (ValidaSeleccionRazon() && ValidaSeleccionTipo())
            {
                GlobalUI.VisitaActual.Razon = RazonVisitaSeleccionada;

                //Caso: 34431 LJR 22/12/2008 - Tipos de Visita
                GlobalUI.VisitaActual.Tipo = TipoVisitaSeleccionado;
                GlobalUI.VisitaActual.FechaFin = DateTime.Now;
                GlobalUI.VisitaActual.Nota = TextoNota;
                try
                {
                    GlobalUI.VisitaActual.Guardar();
                    GuardarUbicacionesGPS();
                    GlobalUI.VisitaActual = null;
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error registrando visita. " + ex.Message);
                }

                //Indicamos que la visita se registro.
                ReturnResult(true);
                //DoClose();
            }
        }

        public void RegistrarAdvertencia()
        {
            if (FRdConfig.PermiteSincroDesatendida)
            {
                this.mostrarAlerta("No se pueden sincronizar los datos porque hay una sincronización en proceso. Debera hacerlo manualmente o en la proxima visita.", res =>
                    {
                        this.Registrar();
                    });
            }
            else
            {
                this.Registrar();
            }            
        }

        private bool ValidaSeleccionRazon()
        {
            if (RazonVisitaSeleccionada == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, " un estado");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ValidaSeleccionTipo()
        {
            #pragma warning disable 472
            if (TipoVisitaSeleccionado == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, " un tipo de visita");
                return false;
            }
            else
            {
                return true;
            }
        }

        #region Ubicacion
        /// <summary>
        /// Verifica que se tenga permiso de guarda la ubicación de la visita
        /// y guarda la visita.
        /// </summary>
        private void GuardarUbicacionesGPS()
        {
            if (FRmConfig.GuardarUbicacionVisita && !gpsOff)
                GuardarVisitaUbicacion();
            //Ubicacion.Parar();
        }        

        /// <summary>
        /// Gurda la ubicación de la visita. Si la posición no se ha obtenido 
        /// se espera hasta que el tiempo se agote o se obtenga la posición
        /// </summary>
        private void GuardarVisitaUbicacion()
        {
            try
            {
                Visita visita = GlobalUI.VisitaActual;
                if (visita != null)
                {
                    ClienteUbicacion clienteUbicacion = ClienteUbicacion.Cargar(visita.Cliente.Zona, visita.Cliente.Codigo);
                    Bitacora.Escribir(String.Format("Visita C:{0}, R:{1}, Fi:{2}, Ff:{3}"
                        , visita.Cliente.Codigo, visita.Cliente.Zona, visita.FechaInicio, DateTime.Now));
                    GpsPosition androidGPR = new GpsPosition();
                    androidGPR.Latitude = this.latidud;
                    androidGPR.Longitude = this.longitud;
                    androidGPR.SeaLevelAltitude = this.altitud;
                    androidGPR.Time = this.gpsTime;
                    // Se guarda la posición si se pudo obtener.
                    //VisitaUbicacion vu = new VisitaUbicacion(visita, posicion.GpsPosition, posicion.LocalSystemTime, Ubicacion.Error);
                    VisitaUbicacion vu = new VisitaUbicacion(visita, androidGPR, DateTime.Now.ToLocalTime(), Ubicacion.Error);
                    vu.Guardar();
                    if (!FRmConfig.NoDetenerGPS)
                    {
                        Ubicacion.Parar();
                    }
                }                
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Ha ocurrido un error guardando la ubicación asociada a la visita." + ex);                
            }
        }
        #endregion

        private void Cancelar()
        {
            ReturnResult(false);
            DoClose();
        }

        #endregion

        #region Comandos y Acciones

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Registrar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoNotas
        {
            get { return new MvxRelayCommand(NotasChange); }
        }

        public void NotasChange() 
        {
            if (ObservacionesVisible)
            {
                ObservacionesVisible = false;
            }
            else
            {
                ObservacionesVisible = true; 
            }
        }

        public void Regresar()
        {
            if (ObservacionesVisible)
            {
                ObservacionesVisible = false;
            }
            else
            {
                this.DoClose();
            }
        }

        #endregion
    }
}