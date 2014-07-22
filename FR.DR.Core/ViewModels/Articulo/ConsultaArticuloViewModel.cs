//using System;
//using System.Net;
//using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

using Cirrious.MvvmCross.Commands;
//using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaArticuloViewModel : BaseViewModel
    {
        #region Atributos y Propiedades

        /// <summary>
        /// Es asignado por la pantalla que ocupa consultar el articulo
        /// antes de invocarlo, es decir antes, de hacer this.RequestNavigate<ConsultaArticuloViewModel>();
        /// </summary>
        public static Articulo Articulo { get; set; }

        #endregion Atributos y Propiedades

        #region Constructor
        public ConsultaArticuloViewModel()
        {
            ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(Articulo.Compania);

            TryMsj(() => Articulo.CargarPrecio(cliente.NivelPrecio), "Error cargando precios del artículo.");
            TryMsj(() => Articulo.CargarDescuentos(cliente), "Error al cargar descuentos escalonado.");
            TryMsj(() => Articulo.CargarBonificaciones(cliente), "Error al cargar bonificaciones.");
        }
        #endregion Constructor

        #region Comandos
        public ICommand ComandoDetalleDescuentos
        {
            get { return new MvxRelayCommand(() => VerDetalle(true)); }
        }
        public ICommand ComandoDetalleBonificaciones
        {
            get { return new MvxRelayCommand(() => VerDetalle(false)); }
        }
        #endregion Comandos

        #region Acciones

        /// <summary>
        /// Ver detalles del articulo
        /// </summary>
        private void VerDetalle(bool esDescuento)
        {
            bool hayDetalle = esDescuento? 
                Articulo.DescuentosEscalonados.Count > 0 : 
                Articulo.Bonificaciones.Count > 0;
            
            if (hayDetalle)
            {
                Dictionary<string, string> pars = new Dictionary<string, string>() { { "esDescuento", esDescuento ? "S" : "N" } };
                ConsultaBonYDescViewModel.Articulo = ConsultaArticuloViewModel.Articulo;
                this.RequestNavigate<ConsultaBonYDescViewModel>(pars);
            }
        }
        #endregion
    }
}