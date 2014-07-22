using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
//using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using FR.DR.Core.Helper;
using FR.Core.Model;
namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MenuReportesViewModel : BaseViewModel
    {
        #region Propiedades

        public SimpleObservableCollection<OpcionMenu> Opciones { get; set; }

        #endregion Propiedades

        public MenuReportesViewModel()
        {
            ConfigMenu();
        }

        public void ConfigMenu()
        {
            List<OpcionMenu> Opciones = new List<OpcionMenu>()
            {
                new OpcionMenu("Monto Cobrado y vendido"),new OpcionMenu("Cierre de Cobro"),new OpcionMenu("Visitas")
            };

            //if (FRdConfig.UsaFacturacion)
            {
                Opciones.Add(new OpcionMenu("Inventario")); 
            }

            if (FRdConfig.UsaEnvases)
            {
                Opciones.Add(new OpcionMenu("Liquidación Inventario")); 
            }

            Opciones.AddRange(new List<OpcionMenu>() {new OpcionMenu("Devolucion"),new OpcionMenu("Liquidación Jornada")});
            this.Opciones = new SimpleObservableCollection<OpcionMenu>(Opciones);
        }

        #region Comandos y Acciones

        public ICommand MenuSelected
        {
            get { return new MvxRelayCommand<OpcionMenu>(ExecutarReporte); }
        }

        public void ExecutarReporte(OpcionMenu reporte)
        {
            switch (reporte.Descripcion)
            {
                case "Monto Cobrado y vendido": this.RequestNavigate<ReporteVisitaMontosViewModel>(); break;
                case "Cierre de Cobro": this.RequestNavigate<ReporteCierreViewModel>(); break;
                case "Visitas": this.RequestNavigate<ReporteVisitasViewModel>(); break;
                case "Devolucion": this.RequestNavigate<ReporteDevueltoViewModel>(); break;
                case "Inventario": this.RequestNavigate<ReporteInventarioViewModel>(); break;
                case "Liquidación Inventario": this.RequestNavigate<ReporteLiquidacionInventarioViewModel>(); break;
                case "Liquidación Jornada":
                    if (JornadaRuta.VerificarJornadaAbierta())
                    {
                        this.RequestNavigate<ReporteDocDiariosViewModel>(); break;
                    }
                    else
                    {
                        this.mostrarAlerta("No existe ningún registro de jornada actualmente.");
                    }
                    break;
                default:
                    this.mostrarAlerta(string.Format("El reporte '{0}' no existe!", reporte));
                    break;
            }
        }

        #endregion Comandos y Acciones
    }
}