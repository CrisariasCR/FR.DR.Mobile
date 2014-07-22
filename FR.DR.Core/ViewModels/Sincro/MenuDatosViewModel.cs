using System;
using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using System.Threading;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using Softland.ERP.FR.Mobile.Cls.Utilidad; // ubicacion
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MenuDatosViewModel : BaseViewModel 
    {
        #region Propiedades

        public static bool estaEnviando = false;

        private string opcion;
        public string Opcion
        {
            get { return opcion; }
            set { opcion = value; RaisePropertyChanged("Opcion"); }
        }

        private bool purgaVisible;
        public bool PurgaVisible
        {
            get { return purgaVisible; }
            set { purgaVisible = value; RaisePropertyChanged("PurgaVisible"); }
        }

        private bool cobrosVisible=true;
        public bool CobrosVisible
        {
            get { return cobrosVisible; }
            set { cobrosVisible = value; RaisePropertyChanged("CobrosVisible"); }
        }

        public Android.Content.Context Contexto { get; set; }

        public List<string> Opciones { get; set; }

        #endregion Propiedades

        #region Constructor e Inicialización
        public MenuDatosViewModel()
        {
            ConfigMenu();
            GestorSincronizar.viewModel = this;
        }

        ~MenuDatosViewModel()
        {
            GestorSincronizar.viewModel = null;
        }

        public void ConfigMenu()
        {
            Opciones = new List<string>() { "Cargar Datos", "Enviar Datos"};

            if (FRmConfig.PugarDocumentosManualmente)
            {
                Opciones.Add("Purga");
                PurgaVisible = false;
            }
            else
            {
                PurgaVisible = true;
                CobrosVisible = true;
            }


            Opciones.Add("Actualizar"); // mostrar opcion de Actualizar
        }

        #endregion Constructor e Inicialización

        #region Comandos y Acciones

        public ICommand MenuSelected
        {
            get { return new MvxRelayCommand<string>(Ejecutar); }
        }

        public void Ejecutar(string opcion)
        {
            return;
            //switch (opcion)
            //{
            //    case "Cargar Datos": this.CargaDatos(); break;
            //    case "Enviar Datos": this.DescargaDatos(); break;
            //    case "Purga": this.PugarDatos(false, this.Contexto, true); break;
            //    case "Actualizar": this.RequestNavigate<MenuActualizadorViewModel>(); break;
            //    case "Cobros": this.RequestNavigate<ConsultaNotasCreditoViewModel>(); break;
            //    default:
            //        this.mostrarAlerta(string.Format("La opción '{0}' no existe!", opcion));
            //        break;
            //}
        }

        public void Actualizar() 
        {
            this.RequestNavigate<MenuActualizadorViewModel>();
        }

        public void Cobros()
        {
            this.RequestNavigate<ConsultaNotasCreditoViewModel>();
        }

        public void AlertaOpciones()
        {
            this.mostrarAlerta(string.Format("La opción '{0}' no existe!", opcion));
        }

        #endregion Comandos y Acciones

        #region Carga de datos
        public void CargaDatos()
        {
            bool continuar = true;

            if (FRmConfig.PurgarDocumentosAutomaticamente) continuar = PugarDatos(true,this.Contexto,false);

            if (continuar && GestorSincronizar.CargaDatos(this.Contexto))
            {
                //Luego de otra carga desde el servidor debemos cargar estos parametros pues
                //pueden haber cambiado
                CargaParametros(true, this);
            }
        }

        public static bool CargaParametros(bool logBitacora, BaseViewModel viewModel)
        {
            string mensaje = string.Empty;
            if (GestorSincronizar.CargaParametros(logBitacora))
            {
                try
                {
                    HandHeldConfig config = new HandHeldConfig();
                    //Carga nuevamente los parametros de la Pocket
                    config.cargarConfiguracionHandHeld(ref mensaje);
                    config.cargarConfiguracionGlobalHandHeld(ref mensaje);
                    GlobalUI.Rutas = Ruta.ObtenerRutas();
                    
                    // Inicia el proceso de obtención de la posición.
                    if (FRmConfig.NoDetenerGPS)
                    {
                        Ubicacion.Iniciar(Ubicacion.NO_PARAR);
                    }

                    //Crea el registro en caso de que no se utilizen jornadas
                    //Si se utilizan Jornadas, el registro se creara hasta que se abra la jornada - KFC
                    if ((!FRdConfig.UsaJornadaLaboral) && (!JornadaRuta.VerificarJornadaAbierta()))
                    {
                        JornadaRuta.AbrirJornada();
                    }

                    // LAS. Mejora Paquetes y Reglas. Liberar los paquetes de las regalias.
                    Regalias.Liberar();
                }
                catch (Exception ex)
                {
                    viewModel.mostrarAlerta("Error al cargar los parametros de dispositivo. " + ex.Message);
                    return false;
                }
                return true;
            }
            else
                return false;

        }
        
        #endregion Carga de datos

        #region Purga de datos
        /// <summary>
        /// Purga los documentos asociados a la HH de acuerdo con la cantidad de días
        /// y el código de la ruta.
        /// </summary>
        /// <param name="utilizarCantidadDias"></param>
        /// <returns></returns>
        public bool PugarDatos(bool utilizarCantidadDias, Android.Content.Context contexto,bool soloPurga)
        {
            bool resultado = false;
            int cantidadDias = utilizarCantidadDias ? FRmConfig.PurgarCantidadDias : -1;

            GlobalUI.Rutas = Ruta.ObtenerRutas();
            foreach (Ruta ruta in Ruta.ObtenerRutas())
            {
                resultado = GestorSincronizar.PurgarDatos(ruta.Codigo, cantidadDias, utilizarCantidadDias,contexto,soloPurga);
            }
            return resultado;
        }

        #endregion Purga de datos

        #region Descarga de datos

        public void DescargaDatos()
        {
            estaEnviando = true;
            bool continuar = GestorSincronizar.DescargaDatos(GlobalUI.Rutas,this.Contexto);
            Thread.Sleep(2000);
            if (continuar && FRmConfig.PurgarDocumentosAutomaticamente)
            {                
                //MessageBox.Show("Purgando documentos sincronizados. Espere por favor ...");
                PugarDatos(true,this.Contexto,false);
            }
        }
        
        #endregion Descarga de datos
    }
}