//using System;
//using System.Net;
//using System.Windows;
using System.Collections.Generic;

//using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
//using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;
//using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaBonYDescViewModel : ListViewModel
    {
        #region Atributos y Propiedades

        public IObservableCollection<BonYDesc> Items { get; set; }

        /// <summary>
        /// articulo a consultar
        /// </summary>
        public static Articulo Articulo { get; set; }

        /// <summary>
        /// Determina si se consulta una bonificacion o un descuento
        /// </summary>
        private bool esDescuento = false;
        public bool EsDescuento { get { return esDescuento; } }

        #endregion Atributos y Propiedades

        #region Constructor
        public ConsultaBonYDescViewModel(string esDescuento)
        {
            this.esDescuento = (esDescuento=="S");
            CargaInicial();
        }
        #endregion Constructor

        private void CargaInicial()
        {
            List<BonYDesc> lista;

            if (esDescuento)
            {
                lista = Articulo.DescuentosEscalonados.ConvertAll(x => new BonYDesc(x));
            }
            else
            {
                lista = Articulo.Bonificaciones.ConvertAll(x => new BonYDesc(x));
            }
            Items = new SimpleObservableCollection<BonYDesc>(lista);
            RaisePropertyChanged("Items");
        }

        #region Comandos
        #endregion

        #region Acciones
        
        #endregion
    }

}