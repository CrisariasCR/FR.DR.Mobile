using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FR.Core.Model;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
//using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.Corporativo;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MenuParametrosViewModel : BaseViewModel
    {
        #region Propiedades

        public SimpleObservableCollection<OpcionMenu> Opciones { get; set; }

        //private OpcionMenu opcionMenuActual;
        //public OpcionMenu OpcionMenuActual
        //{
        //    get { return opcionMenuActual; }
        //    set { opcionMenuActual = value; RaisePropertyChanged("OpcionMenuActual"); }
        //}

        #endregion Propiedades

        public MenuParametrosViewModel()
        {
            ParametroSistema.CargarParametros();
            Softland.ERP.FR.Mobile.UI.GlobalUI.Rutas=Ruta.ObtenerRutas();
            ConfigMenu();
        }

        public void ConfigMenu()
        {
            Opciones = new SimpleObservableCollection<OpcionMenu>(new List<OpcionMenu>()
            {
                new OpcionMenu("Par�metros del Sistema"), new OpcionMenu("Consulta NCF"),new OpcionMenu("Par�metros del Dispositivo"),new OpcionMenu("Corporaci�n"),new OpcionMenu("Par�metros de Impresi�n")
                //"Par�metros del Sistema","Consulta NCF","Par�metros del Dispositivo","Corporaci�n","Par�metros de Impresi�n"
            });
            RaisePropertyChanged("Opciones");
            
        }

        #region Comandos y Acciones

        public ICommand MenuSelected
        {
            get { return new MvxRelayCommand<OpcionMenu>(Executar); }
        }

        public void Executar(OpcionMenu opcion)
        {
            switch (opcion.Descripcion)
            {
                case "Par�metros del Sistema": this.RequestNavigate<ParametrosSistemaViewModel>(); break;
                case "Consulta NCF": this.RequestNavigate<ConsultaConsecNCFViewModel>(); break;
                case "Par�metros del Dispositivo": this.RequestNavigate<ConsultaDispositivoViewModel>(); break;
                case "Corporaci�n": this.RequestNavigate<ConsultaCorporacionViewModel>(); break;
                case "Par�metros de Impresi�n": this.RequestNavigate<ParametrosImpresoraViewModel>(); break;                
                default:
                    this.mostrarAlerta(string.Format("La opci�n '{0}' no existe!", opcion));
                    break;
            }
        }

        #endregion Comandos y Acciones
    }
}