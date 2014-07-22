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

using FR.Core.Model;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using FR.DR.Core.Helper;

using Cirrious.MvvmCross.Commands;
using System.Windows.Input;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleCobroViewModel : ListViewModel
    {
        public DetalleCobroViewModel()
        {
            ConsultaCobroViewModel.ReciboSeleccionado.CargaDetalles();

            if (Recibo.ChequesAsociados.Count == 0)
            {
                try
                {
                    Recibo.CargaChequesAplicados();
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando cheques" + ex.Message);
                }
            }

            Detalles = new SimpleObservableCollection<DetalleRecibo>(Recibo.Detalles);
        }

        #region Propiedades

        public Recibo Recibo { get { return ConsultaCobroViewModel.ReciboSeleccionado; } }
        public IObservableCollection<DetalleRecibo> Detalles { get; set; }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #endregion

        #region Comandos

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarCheques); }
        }

        #endregion

        #region Acciones

        private void ConsultarCheques()
        {
            this.RequestNavigate<ConsultaChequesViewModel>();
        }

        #endregion
    }
}