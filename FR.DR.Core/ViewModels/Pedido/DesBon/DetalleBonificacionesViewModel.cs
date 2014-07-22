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

using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleBonificacionesViewModel: DialogViewModel<Dictionary<string,object>>
    {
        public DetalleBonificacionesViewModel(string messageId, string nivelPrecio = "", string ruta = "")
            : base(messageId)
        {
            this.nivelPrecio = nivelPrecio;
            this.ruta = ruta;
            Cargar();
        }

        #region Propiedades y Atributos

        private string nivelPrecio;
        private string ruta;
        public static Pedido Pedido { get; set; }
        private bool aplicar;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private IObservableCollection<DetallePedido> detalles;
        public IObservableCollection<DetallePedido> Detalles
        {
            get { return detalles; }
            set { detalles = value; RaisePropertyChanged("Detalles"); }
        }

        public DetallePedido DetalleSeleccionado { get; set; }

        #endregion

        #region Logica

        private void Cargar()
        {
            Detalles = new SimpleObservableCollection<DetallePedido>();

            foreach (DetallePedido detalle in Pedido.Detalles.Lista)
            {
                DetallePedido bonificacion = detalle.LineaBonificada;
                LineaRegalia regalia = detalle.RegaliaBonificacion;
                if (bonificacion != null && regalia != null)
                {
                    Detalles.Add(detalle);
                }
            }

            RaisePropertyChanged("Detalles");
        }

        private void CambiarBonificacion()
        {
            if (DetalleSeleccionado != null)
            {
                DetallePedido detalle = DetalleSeleccionado;
                LineaRegalia regalia = DetalleSeleccionado.RegaliaBonificacion;
                //DetallePedido bonificacion = detalle.EsBonificada? detalle : detalle.LineaBonificada;
                DetallePedido bonificacion = detalle.LineaBonificada;
                ClienteCia cliente = ObtenerClienteCia();


                CambioBonificacionViewModel.Cliente = cliente;
                CambioBonificacionViewModel.Detalle = bonificacion;
                CambioBonificacionViewModel.Regalia = regalia;

                Dictionary<string, object> Parametros = new Dictionary<string, object>();
                Parametros.Add("nivelPrecio", nivelPrecio);
                Parametros.Add("ruta", ruta);

                this.RequestDialogNavigate<CambioBonificacionViewModel, DetallePedido>(Parametros, result =>
                {
                    detalle.LineaBonificada = result;
                    Cargar();
                });
                //CambioBonificacionForm cambio = new CambioBonificacionForm(cliente, ref bonificacion, ref regalia, nivelPrecio, ruta);
                //cambio.ShowDialog();
                
            }
            else
            {
                this.mostrarAlerta("No se ha seleccionado ninguna bonificación.");
            }

        }

        private ClienteCia ObtenerClienteCia()
        {
            List<ClienteCia> clientes = ClienteCia.ObtenerClientes(Pedido.Cliente, false);
            return clientes.Count > 0 ? clientes[0] : null;
        }

        #endregion

        #region Comandos y Acciones

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoBonificaciones
        {
            get { return new MvxRelayCommand(CambiarBonificacion); }
        }

        private void Aceptar()
        {
            aplicar = true;
            EndResult();
        }

        private void Cancelar()
        {
            aplicar = false;
            EndResult();
        }

        private void EndResult()
        {
            Dictionary<string,object> result = new Dictionary<string,object>();
            result.Add("Aplicar", aplicar);
            result.Add("Pedido", Pedido);
            this.ReturnResult(result);
        }
        
        #endregion
    }
}