using System;
//using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ReporteVisitasViewModel : ListViewModel
    {
        #region Propiedades

        #region Items
        public IObservableCollection<Visita> Items { get; set; }
        #endregion Items e ItemActual

        #endregion Propiedades

        public ReporteVisitasViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
                // binding:
                // Cliente.Nombre
                // Razon.Descripcion
                // FechaInicio.ToLongTimeString()
                // ComandoImprimir
            try
            {
                List<Visita> lista = Visita.ReporteVisitas();
                this.Items = new SimpleObservableCollection<Visita>(lista);
                RaisePropertyChanged("Items");
            }
            catch (Exception e)
            {
                this.mostrarAlerta("Error en la carga del reporte. " + e.Message);
            }
        }

        #endregion CargaInicial

        #region Comandos y Acciones

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }

        private void Imprime()
        {
            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el reporte de visitas",
                result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        if (this.Items.Count != 0)
                        {
                            ReporteVisitas reporte = new ReporteVisitas(this, this.Items);
                            reporte.Imprime();
                        }
                        else
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "El reporte no tiene información.");
                    }
                });
        }

        #endregion Accioness
    }
}
