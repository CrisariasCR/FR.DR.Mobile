using System;
using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using Softland.ERP.FR.Mobile.Cls.Utilidad; // ubicacion
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class JornadaLaboralViewModel : BaseViewModel
    {
        #region Propiedades

        public List<string> Opciones { get; set; }

        #endregion Propiedades

        #region Constructor e Inicialización
        public JornadaLaboralViewModel()
        {
            List<Ruta> rutas = Ruta.ObtenerRutas();
            JornadaRuta jornada = new JornadaRuta();
            jornada = JornadaRuta.ObtenerJornada(rutas[0].Codigo, DateTime.Now);

            if ((jornada != null) && (JornadaRuta.VerificarJornadaAbierta()))
            {
                if (JornadaRuta.VerificarJornadaCerrada())
                {
                    Inicio = jornada.FechaHoraInicio.ToString();
                    Cierre = jornada.FechaHoraFin.ToString();
                    InicioVisible = cierreVisible = false;
                    //btnInicioJornada.Enabled = btnCierreJornada.Enabled = lblInicio.Enabled = lblCierre.Enabled = false;
                }
                else
                {
                    Inicio = jornada.FechaHoraInicio.ToString();
                    Cierre = "No Asignado";
                    InicioVisible = false;
                    CierreVisible = true;
                }
            }
            else 
            {
                Inicio = "No Asignado";
                Cierre = "No Asignado";
                inicioVisible = true;
            }
            
        }

        #endregion Constructor e Inicialización

        #region Comandos y Acciones

        public ICommand ComandoInicio
        {
            get { return new MvxRelayCommand(EjecutarInicio); }
        }

        public ICommand ComandoCierre
        {
            get { return new MvxRelayCommand(EjecutarCierre); }
        }

        #endregion Comandos y Acciones

        #region Métodos

        private void EjecutarInicio() 
        {
            if (inicioVisible)
            {
                try
                {
                    JornadaRuta.AbrirJornada();
                    this.mostrarMensaje(Mensaje.Accion.Informacion, "Jornada abierta exitosamente.", res =>
                        {
                            this.DoClose();
                        });
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error al abrir jornada. " + ex.Message);
                }
               
            }
        }

        private void EjecutarCierre()
        {
            if (cierreVisible)
            {
                try
                {
                    if (JornadaRuta.VerificarJornadaAbierta())
                    {
                        if (JornadaRuta.VerificarTomaFísica(true))
                        {
                            JornadaRuta.CerrarJornada();
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "Jornada cerrada exitosamente.", res =>
                                {
                                    this.DoClose();
                                });
                        }
                        else
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "Debe realizar la toma física antes de cerrar la Jornada.", res =>
                            {
                                this.DoClose();
                            });
                        }                    
                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "Aun no ha sido abierta la jornada.");
                    }

                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error al cerrar jornada. " + ex.Message);
                }
            }
        }

        #endregion

        #region Propiedades
        private bool inicioVisible;        
        public bool InicioVisible
        {
            get { return inicioVisible; }
            set { inicioVisible = value; RaisePropertyChanged("InicioVisible"); }
        }

        private bool cierreVisible;
        public bool CierreVisible
        {
            get { return cierreVisible; }
            set { cierreVisible = value; RaisePropertyChanged("CierreVisible"); }
        }

        private string inicio;
        public string Inicio
        {
            get { return inicio; }
            set { inicio = value; RaisePropertyChanged("Inicio"); }
        }

        private string cierre;
        public string Cierre
        {
            get { return cierre; }
            set { cierre = value; RaisePropertyChanged("Cierre"); }
        }
        #endregion

    }
}