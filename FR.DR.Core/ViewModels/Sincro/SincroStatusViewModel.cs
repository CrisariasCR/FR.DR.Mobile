using System;
//using System.Net;
//using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SincroStatusViewModel : BaseViewModel
    {
        #region Propiedades

        private string titulo;
        public string Titulo
        {
            get { return titulo; }
            set
            {
                if (value != titulo)
                {
                    titulo = value;
                    RaisePropertyChanged("Titulo");
                }
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                if (value != status)
                {
                    status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        #endregion Propiedades

        public SincroStatusViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
        }

        #endregion CargaInicial

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        #endregion Comandos

        #region Acciones

        public void Cancelar()
        {
            this.DoClose();
        }

        #endregion Accioness
    }
}
