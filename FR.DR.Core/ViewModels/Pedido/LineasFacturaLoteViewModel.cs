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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class LineasFacturaLoteViewModel : DialogViewModel<bool>
    {
        public LineasFacturaLoteViewModel(string messageId)
            : base(messageId)
        {
            InicializarPantalla();
        }

        #region Propiedades
        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        private IObservableCollection<DetallePedido> articulosLote;
        public IObservableCollection<DetallePedido> ArticulosLote
        {
            get { return articulosLote; }
            set { articulosLote = value; RaisePropertyChanged("ArticulosLote"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private DetallePedido articuloSeleccionado;
        public DetallePedido ArticuloSeleccionado
        {
            get { return articuloSeleccionado; }
            set { articuloSeleccionado = value; CambioArticuloSeleccionado(); RaisePropertyChanged("ArticuloSeleccionado"); }
        }

        private IObservableCollection<ClienteCia> companias;
        public IObservableCollection<ClienteCia> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private ClienteCia companiaActual;
        public ClienteCia CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; CargarArticulosLote(); RaisePropertyChanged("CompaniaActual"); }
        }

        private string bodega = string.Empty;

        #endregion

        #region Logica

        private void InicializarPantalla()
        {
            this.LlenarComboCias();
            this.CargarArticulosLote();
        }

        private void LlenarComboCias()
        {
            List<ClienteCia> lista = null;

            if (FRmConfig.EnConsulta)
            {
                //lista = Util.CargarCiasPedido(Gestor.Pedidos.Gestionados);
            }
            else
                lista = Util.CargarCiasClienteActual();

            Companias = new SimpleObservableCollection<ClienteCia>(lista);
            if (Companias.Count > 0)
            {
                companiaActual = Companias[0];
            }
        }

        private void CargarArticulosLote()
        {
            ArticulosLote = new SimpleObservableCollection<DetallePedido>();
            foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
            {
                if (pedido.Compania == CompaniaActual.Compania)
                {
                    bodega = pedido.Bodega;
                    foreach (DetallePedido detalle in pedido.Detalles.Lista)
                    {
                        if (detalle.Articulo.UsaLotes)
                        {
                            ArticulosLote.Add(detalle);
                        }
                    }
                }
            }
            RaisePropertyChanged("ArticulosLote");
        }

        public void Cancelar()
        {
            if (ValidarLotes())
            {
                this.DoClose();
                ReturnResult(true);
            }
        }

        private bool ValidarLotes()
        {
            bool result = true;

            foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
            {
                if (pedido.Compania == CompaniaActual.Compania)
                {
                    foreach (DetallePedido detalle in pedido.Detalles.Lista)
                    {
                        if (detalle.Articulo.UsaLotes && !detalle.DesglosoLote)
                        {
                            result = false;
                            this.mostrarAlerta("El articulo:" + detalle.Articulo.Codigo + " esta pendiende de desglose");
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private void CambioArticuloSeleccionado()
        {
            if (ArticuloSeleccionado == null)
                return;

            if (CompaniaActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }

            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("bodega", bodega);
            AsignaLotesViewModel.DetalleLinea = ArticuloSeleccionado;
            this.RequestNavigate<AsignaLotesViewModel>(Parametros);
        }

        #endregion

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        #endregion
    }
}