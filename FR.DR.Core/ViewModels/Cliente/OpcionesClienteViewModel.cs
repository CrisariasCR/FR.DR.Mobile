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
using FR.DR.Core.Helper;
using FR.Core.Model;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class OpcionesClienteViewModel : BaseViewModel
    {
        public OpcionesClienteViewModel()
        {
            GlobalUI.ClienteActual.ObtenerClientesCia();
            this.NombreCliente = "Cliente: " + GlobalUI.ClienteActual.Nombre;            
            EstablecerOpcionesCliente();
        }

        public void EstablecerOpcionesCliente()
        {
            List<OpcionMenu> Opciones = new List<OpcionMenu>()
            {
                new OpcionMenu("Detalle Cliente"),new OpcionMenu("Cobros"),new OpcionMenu("Documentos"),new OpcionMenu("Notas de Crédito"),new OpcionMenu("Facturas")
            };

            if (GlobalUI.ClienteActual.NivelesPreciosDefinidos)
            {
                Opciones.AddRange(new List<OpcionMenu>() { new OpcionMenu("Inventario"),new OpcionMenu("Pedidos"),new OpcionMenu("Pedidos - Estado"),new OpcionMenu("Devoluciones"),new OpcionMenu("Artículos") });
            }

            if (FRdConfig.UsaConsignacion && GlobalUI.ClienteActual.UsaConsignacion())
            {
                Opciones.Add(new OpcionMenu("Consignaciones"));
            }
            this.Opciones = new SimpleObservableCollection<OpcionMenu>(Opciones);
        }

        #region Comandos y Acciones

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand<OpcionMenu>(Consultar); }
        }

        public void Consultar(OpcionMenu Accion)
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            switch (Accion.Descripcion)
            {
                case "Detalle Cliente": ConsultaClienteViewModel.SeleccionCliente = true; this.RequestNavigate<ConsultaClienteViewModel>(); break;
                case "Cobros":
                    Parametros.Add("anulando", false);
                    ConsultaCobroViewModel.SeleccionCliente = true;
                    this.RequestNavigate<ConsultaCobroViewModel>(Parametros); break;
                case "Documentos":
                    ConsultaDocumentosCCViewModel.SeleccionCliente = true;
                    Parametros.Add("tipo", (int)TipoDocumento.Factura);
                    this.RequestNavigate<ConsultaDocumentosCCViewModel>(Parametros); 
                    break;
                case "Notas de Crédito":
                    ConsultaDocumentosCCViewModel.SeleccionCliente = true;
                    Parametros.Add("tipo", (int)TipoDocumento.NotaCredito);
                    this.RequestNavigate<ConsultaDocumentosCCViewModel>(Parametros); 
                    break;
                case "Pedidos":
                    ConsultaPedidosViewModel.SeleccionCliente = true;
                    Parametros.Add("tipoPedido","P");
                    Parametros.Add("anular", false);
                    this.RequestNavigate<ConsultaPedidosViewModel>(Parametros); 
                    break;
                case "Pedidos - Estado":
                    HistoricoPedidosViewModel.SeleccionCliente = true;
                    this.RequestNavigate<HistoricoPedidosViewModel>(); 
                    break;
                case "Facturas":
                    ConsultaPedidosViewModel.SeleccionCliente = true;
                    Parametros.Add("tipoPedido","F");
                    Parametros.Add("anular", false);
                    this.RequestNavigate<ConsultaPedidosViewModel>(Parametros); 
                    break;
                case "Artículos":
                    BusquedaArticulosViewModel.SeleccionCliente = true;
                    this.RequestNavigate<BusquedaArticulosViewModel>(); break;
                case  "Inventario":
                    ConsultaInventarioViewModel.SeleccionCliente = true;
                    this.RequestNavigate<ConsultaInventarioViewModel>(); break;
                case "Devoluciones":
                    ConsultaDevolucionesViewModel.SeleccionCliente = true;
                    Parametros.Add("anular", "N");
                    this.RequestNavigate<ConsultaDevolucionesViewModel>(Parametros); break;
                case "Consignaciones":
                    Parametros.Add("tipoFormulario", Softland.ERP.FR.Mobile.UI.Formulario.SeleccionarCliente);
                    this.RequestNavigate<ConsultaConsignacionViewModel>(Parametros); break;
            }
        }

        #endregion

        #region Propiedades

        public string NombreCliente { get; set; }

        public SimpleObservableCollection<OpcionMenu> Opciones { get; set; }

        public override void DoClose()
        {
            base.DoClose();
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #endregion
    }
}