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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleHistoricosPedidoViewModel : ListViewModel
    {
        public DetalleHistoricosPedidoViewModel()
        {
            Pedido = HistoricoPedidosViewModel.PedidoSeleccionado;
            Detalles = new SimpleObservableCollection<DetallePedido>();
            CargarDetalles();
        }

        #region Metodos Logica de Negocio

        private void CargarDetalles()
        {
            try
            {
                Detalles.Clear();

                if (Pedido.Detalles.Vacio())
                {
                    try
                    {
                        //Cargamos los detalles del pedido
                        Pedido.Detalles.Encabezado = Pedido;
                        Pedido.Detalles.ObtenerDetallesHistoricoPedido();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando detalles del pedido. " + ex.Message);
                    }
                }

                //Detalles = new SimpleObservableCollection<DetallePedido>(Pedido.Detalles.Lista);

                foreach (DetallePedido det in Pedido.Detalles.Lista)
                {
                    this.AgregarDetalle(det, false);

                    if (det.LineaBonificada != null)
                        this.AgregarDetalle(det.LineaBonificada, true);
                }
            }
            catch (System.Exception ex)
            {
                this.mostrarAlerta("Error al cargar los detalles del pedido. " + ex.Message);
            }
        }

        private void AgregarDetalle(DetallePedido detalle, bool esBonificada)
        {
            string lineaBonifica = string.Empty;

            if (esBonificada)
                lineaBonifica = Detalles.Count.ToString();

            detalle.LineaBonifica = lineaBonifica;
            Detalles.Add(detalle);
        }

        #endregion

        #region Propiedades

        private IObservableCollection<DetallePedido> detalles;
        public IObservableCollection<DetallePedido> Detalles
        {
            get { return detalles; }
            set { detalles = value; RaisePropertyChanged("Detalles"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        public Pedido Pedido { get; set; }

        #endregion
    }
}