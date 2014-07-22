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
                new OpcionMenu("Parámetros del Sistema"), new OpcionMenu("Consulta NCF"),new OpcionMenu("Parámetros del Dispositivo"),new OpcionMenu("Corporación"),new OpcionMenu("Parámetros de Impresión")
                //"Parámetros del Sistema","Consulta NCF","Parámetros del Dispositivo","Corporación","Parámetros de Impresión"
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
                case "Parámetros del Sistema": this.RequestNavigate<ParametrosSistemaViewModel>(); break;
                case "Consulta NCF": this.RequestNavigate<ConsultaConsecNCFViewModel>(); break;
                case "Parámetros del Dispositivo": this.RequestNavigate<ConsultaDispositivoViewModel>(); break;
                case "Corporación": this.RequestNavigate<ConsultaCorporacionViewModel>(); break;
                case "Parámetros de Impresión": this.RequestNavigate<ParametrosImpresoraViewModel>(); break;                
                default:
                    this.mostrarAlerta(string.Format("La opción '{0}' no existe!", opcion));
                    break;
            }
        }

        #endregion Comandos y Acciones
    }
}