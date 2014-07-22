using System;
//using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ImpresionViewModel : BaseViewModel
    {
        #region Propiedades


        private string tituloImpresion;
        public string TituloImpresion
        {
            get { return tituloImpresion; }
            set
            {
                if (value != tituloImpresion)
                {
                    tituloImpresion = value;
                    RaisePropertyChanged("TituloImpresion");
                }
            }
        }
        private bool original;
        public bool Original
        {
            get { return original; }
            set
            {
                if (value != original)
                {
                    original = value;
                    RaisePropertyChanged("Original");
                }
            }
        }

        public static bool OriginalEn;

        private bool originalEnable;
        public  bool OriginalEnable
        {
            get { return originalEnable; }
            set
            {
                    originalEnable = value;
                    RaisePropertyChanged("OriginalEnable");                
            }
        }


        private bool copia;
        public bool Copia
        {
            get { return copia; }
            set
            {
                if (value != copia)
                {
                    copia = value;
                    if (!value)
                    {
                        CantidadCopias = 0;
                    }                    
                    RaisePropertyChanged("Copia");
                }
            }
        }

        private int cantidadCopias = Impresora.CantidadCopias;
        public int CantidadCopias
        {
            get { return cantidadCopias; }
            set
            {
                if (copia)
                    cantidadCopias = value;
                else
                    cantidadCopias = 0;    
                    RaisePropertyChanged("CantidadCopias");
            }
        }


        #region CriteriosOrden y CriterioOrdenActual
        private DetalleSort.Ordenador criterioOrden;
        public DetalleSort.Ordenador CriterioOrden
        {
            get { return criterioOrden; }
            set
            {
                if (value != criterioOrden)
                {
                    criterioOrden = value;
                    RaisePropertyChanged("CriterioOrden");
                }
            }
        }

        public IObservableCollection<DetalleSort.Ordenador> CriteriosOrden { get; set; }

        public bool MostrarCriterioOrden { get; set; }
        #endregion CriteriosOrden y CriterioOrdenActual

        public static Action<bool, int, DetalleSort.Ordenador, BaseViewModel> OnImprimir { private get; set; }

        #endregion Propiedades

        public ImpresionViewModel(string tituloImpresion, bool mostrarCriterioOrden)
        {
            this.TituloImpresion = tituloImpresion;
            this.MostrarCriterioOrden = mostrarCriterioOrden;
            this.OriginalEnable = OriginalEn;
        }

        #region Comandos y Acciones

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(ImprimirDocumento); }
        }

        /// <summary>
        /// Invoca la impresion correspondiente 
        /// </summary>
        private void ImprimirDocumento()
        {
            OnImprimir(Original, CantidadCopias, CriterioOrden, this);
            this.DoClose();
        }

        #endregion Acciones
    }
}
