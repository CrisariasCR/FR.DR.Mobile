using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using EMMClient;
using EMMClient.EMMClientLogic;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MenuActualizadorViewModel : BaseViewModel
    {
        #region Propiedades

        public List<string> Opciones { get; set; }

        #endregion Propiedades

        public MenuActualizadorViewModel()
        {
            ConfigMenu();
            this.CargarConfiguracion();
        }

        public void ConfigMenu()
        {
            Opciones = new List<string>() { "Core", "Reportes", "Otros" };
        }

        /// <summary>
        /// Carga la configuración del xml.
        /// </summary>
        private void CargarConfiguracion()
        {
#if DEBUG
            FRmConfig.FullAppPath = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
                FRmConfig.FullAppPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif
            //FRmConfig.FullAppPath = "/sdcard";

            try
            {
                //se lee el xml para conocer las bases de datos del xml
                ConfigurationClass.ReadXml(System.IO.Path.Combine(FRmConfig.FullAppPath, "Config.xml"));
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(string.Format("Error cargando archivo de configuración '{0}'. Detalle: {1}",
                    FRmConfig.FullAppPath, ex.Message));
            }
        }

        #region Comandos y Acciones

        public ICommand MenuSelected
        {
            get { return new MvxRelayCommand<string>(Ejecutar); }
        }

        public void Ejecutar(string opcion)
        {
            switch (opcion)
            {
                case "Core": //this.Actualizar(TipoArchivo.Core); break;
                    this.mostrarAlerta(string.Format("La opción '{0}' no esta disponible para Android!", opcion));
                    break;
                case "Reportes": this.Actualizar(TipoArchivo.Reporte); break;
                case "Otros": this.Actualizar(TipoArchivo.Otro); break;
                default:
                    this.mostrarAlerta(string.Format("La opción '{0}' no existe!", opcion));
                    break;
            }
        }

        #endregion Comandos y Acciones

        private void Actualizar(TipoArchivo tipo)
        {
            ActualizarCustomViewModel.TipoArchivo = tipo; 
            this.RequestNavigate<ActualizarCustomViewModel>(); 
        }

    }
}