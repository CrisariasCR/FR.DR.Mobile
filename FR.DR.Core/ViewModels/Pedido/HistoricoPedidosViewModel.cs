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
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using System.Collections;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class HistoricoPedidosViewModel : ListViewModel
    {
        public static bool SeleccionCliente;

        public HistoricoPedidosViewModel()
        {
            Estados = new SimpleObservableCollection<string>() { "Aprobado", "Backorder", "Facturado", "Cancelado", "Normal", "Todos" };
            CargaInicial();
        }

        #region Propiedades

        public IObservableCollection<string> Estados { get; set; }

        private string estadoSeleccionado;
        public string EstadoSeleccionado
        {
            get { return estadoSeleccionado; }
            set{estadoSeleccionado = value; Refrescar(); RaisePropertyChanged("EstadoSeleccionado");}
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private IObservableCollection<Pedido> pedidos;
        public IObservableCollection<Pedido> Pedidos
        {
            get { return pedidos; }
            set { pedidos = value; RaisePropertyChanged("Pedidos"); }
        }

        private ListaEncabezadoPedido encabezados;

        public static Pedido PedidoSeleccionado { get; set; }

        private ArrayList encPed = new ArrayList();

        #endregion

        #region Metodos Logica de Negocio

        private void CargaInicial()
        {
            Pedidos = new SimpleObservableCollection<Pedido>();

            try
            {
                encabezados = Pedido.ObtenerHistoricosPedido(GlobalUI.ClienteActual.Codigo, GlobalUI.RutaActual.Codigo);
                EstadoSeleccionado = Estados[0];
            }
            catch (System.Exception ex)
            {
                this.mostrarAlerta("Error al cargar los encabezados de los pedidos. " + ex.Message);
            }
        }

        public void Refrescar()
        {
            if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
            {
                EstadoPedido estado = (EstadoPedido)Convert.ToChar(EstadoSeleccionado.Substring(0, 1));

                Pedidos.Clear();
                try
                {
                    
                    this.encPed = encabezados[estado, 0];

                    if (encPed != null)
                    {
                        foreach (object obj in encPed)
                        {
                            Pedido ped = (Pedido)obj;
                            Pedidos.Add(ped);
                        }
                        //for (int cont = 0; cont < encPed.Count; cont++)
                        //{
                        //    Pedido ped = (Pedido)encPed[cont];
                        //    string[] pedido = {
                        //                           ped.Compania,
                        //                           ped.Numero,
                        //                           ped.Estado.ToString(),
                        //                           GestorUtilitario.FormatNumero(ped.MontoBruto),
                        //                           ped.FechaRealizacion.ToString("MM/dd/yyyy")
                        //                       };
                        //    ListViewItem items = new ListViewItem(pedido);

                        //    this.lstvEncPedido.Items.Add(items);
                        //}

                    }

                    //this.lstvEncPedido.EndUpdate();
                }
                catch (System.Exception ex)
                {
                    this.mostrarAlerta("Error en la carga de los pedidos" + ex.Message);
                    //this.lstvEncPedido.EndUpdate();
                }
            }
            else if (EstadoSeleccionado == "Todos")
            {
                //this.encabezadoPedido.FillListBox();
            }
        }
        #endregion

        #region Comandos y Acciones

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarPedido); }
        }

        private void ConsultarPedido()
        {
            IObservableCollection<Pedido> peds = new SimpleObservableCollection<Pedido>();

            if (PedidoSeleccionado == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un pedido");
            }
            else
            {
                this.RequestNavigate<DetalleHistoricosPedidoViewModel>();
            }
        }

        public void Regresar()
        {
            if (!SeleccionCliente)
            {
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"habilitarPedidos", true}
                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
                this.DoClose();
            }
            else
            {
                //this.RequestNavigate<OpcionesClienteViewModel>();
                this.DoClose();
            }
        }


        #endregion
    }
}