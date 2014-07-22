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
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using System.Windows.Forms;

using Cirrious.MvvmCross.Commands;
using System.Windows.Input;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SugeridoVentaViewModel : BaseViewModel
    {
        public SugeridoVentaViewModel()
        {
            CargaInicial();
        }

        #region Propiedades

        private IObservableCollection<SugeridoVenta> sugeridos;
        public IObservableCollection<SugeridoVenta> Sugeridos
        {
            get { return sugeridos; }
            set { sugeridos = value; RaisePropertyChanged("Sugeridos"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private SugeridoVenta sugeridoSeleccionado;
        public SugeridoVenta SugeridoSeleccionado
        {
            get { return sugeridoSeleccionado; }
            set { sugeridoSeleccionado = value; RaisePropertyChanged("SugeridoSeleccionado"); }
        }

        public static NivelPrecio NivelPrecios { get; set; }

        #endregion

        #region Logica

        public void ValidarAlResumir() 
        {
            if (Sugeridos.Count == 0)
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "El cliente no tiene sugeridos de venta asociados.", res =>
                {
                    this.MostrarPantallaPedido();
                });

            }
        }

        private void CargaInicial()
        {
            this.CargarSugeridosVenta();

           

            // MejorasGrupoPelon600R6 - KF //
            // si solo tiene un sugerido y el parametro esta activado pasa directo a la Toma de Pedido
            if ((FRdConfig.UsaSugeridoVenta) && (Sugeridos.Count == 1))
            {
                SugeridoSeleccionado = Sugeridos[0];
                this.Continuar();
            }
        }

        private void CargarSugeridosVenta()
        {
            try
            {
                Sugeridos = new SimpleObservableCollection<SugeridoVenta>(SugeridoVenta.ObtenerSugeridosVenta(GlobalUI.ClienteActual.Codigo));
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando sugeridos de venta. " + ex.Message);
            }
        }

        private void Continuar()
        {
            bool bok = true;
            if (SugeridoSeleccionado == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un sugerido de venta");
                return;
            }
            try
            {
                SugeridoSeleccionado.CargarDetalle();
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando detalles del sugerido de venta. " + ex.Message);
                return;
            }

            //Cargamos el sugerido al pedido que se va a crear
            int lineasNoIncluidas;

            try
            {
                lineasNoIncluidas = this.CargarDetallesAlPedido(SugeridoSeleccionado);

                if (lineasNoIncluidas > 0)
                {
                    bok = false;
                    this.mostrarMensaje(
                        Mensaje.Accion.Informacion,
                        "Se eliminaron " + lineasNoIncluidas + " líneas " +
                        "ya que el artículo definido no existe " +
                        "dentro del catálogo de artículos, o no posee suficiente existencia para facturar", res =>
                        {
                            this.MostrarPantallaPedido();
                        });
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando detalles del sugerido de venta al pedido. " + ex.Message);
                return;
            }

            if (bok)
            {
                this.MostrarPantallaPedido();
            }
            
        }

        public int CargarDetallesAlPedido(SugeridoVenta sugerido)
        {
            //Variable que indica la cantidad de lineas no incluidas
            int lineasNoIncluidas = 0;

            foreach (DetalleDocumento sugeridoDetalle in sugerido.Detalle)
            {
                if (sugeridoDetalle.CargarDetalleAlPedido(Pedidos.FacturarPedido, Gestor.Pedidos.ObtenerConfiguracionVenta(sugeridoDetalle.Articulo.Compania), GlobalUI.RutaActual.Bodega))
                {
                    Gestor.Pedidos.Gestionar(sugeridoDetalle.Articulo, GlobalUI.RutaActual.Codigo, sugeridoDetalle.Articulo.Precio, sugeridoDetalle.UnidadesAlmacen, sugeridoDetalle.UnidadesDetalle, false, "");
                }
                else lineasNoIncluidas++;
            }
            return lineasNoIncluidas;
        }

        private void MostrarPantallaPedido()
        {
            TomaPedidoViewModel.NivelPrecio = NivelPrecios;
            Dictionary<string, object> Parametros = new Dictionary<string, object>() { { "pedidoActual", false } };
            this.RequestNavigate<TomaPedidoViewModel>(Parametros);
            DoClose();
        }

        private void Cancelar()
        {
            string proceso = Pedidos.FacturarPedido ? "la toma de la factura" : "la toma del pedido";

            this.mostrarMensaje(Mensaje.Accion.Cancelar, proceso, result => {
                if (result == DialogResult.Yes)
                {
                    //this.RequestNavigate<MenuClienteViewModel>();
                    DoClose();
                }
            });
            
        }

        #endregion

        #region Comandos

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        #endregion
    }
}