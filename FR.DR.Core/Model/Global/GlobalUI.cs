using System;
using System.Collections.Generic;
using System.Text;

namespace Softland.ERP.FR.Mobile.UI
{
    public static class GlobalUI
    {
        #region Variables de clase

        /// <summary>
        /// Variable que mantiene la informacion actual de la visita.
        /// </summary>
        public static Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita.Visita VisitaActual;
        public static List<Softland.ERP.FR.Mobile.Cls.Corporativo.Ruta> Rutas;
        public static Softland.ERP.FR.Mobile.Cls.Corporativo.Ruta RutaActual;
        public static Softland.ERP.FR.Mobile.Cls.FRCliente.Cliente ClienteActual;

        #endregion

        public static Formulario RetornaFormulario(string formulario)
        {
            switch (formulario) 
            {
                case "BusquedaArticulo": return Formulario.BusquedaArticulo;
                case "AplicarPedido": return Formulario.AplicarPedido;
                case "ConfiguracionPedido": return Formulario.ConfiguracionPedido;
                case "MenuPrincipal": return Formulario.MenuPrincipal;
                case "SeleccionarCliente": return Formulario.SeleccionarCliente;
                case "CantidadPedido": return Formulario.CantidadPedido;
                case "DevolucionCantidades": return Formulario.DevolucionCantidades;
                case "DevolucionDetalle": return Formulario.DevolucionDetalle;
                case "TomaPedido": return Formulario.TomaPedido;
                case "MontosPedido": return Formulario.MontosPedido;
                case "ConsultaPedido": return Formulario.ConsultaPedido;
                case "ConsultaPedidoAnulado": return Formulario.ConsultaPedidoAnulado;
                case "ConsultaFactura": return Formulario.ConsultaFactura;
                case "InventarioCantidades": return Formulario.InventarioCantidades;
                case "InventarioDetalle": return Formulario.InventarioDetalle;
                case "ConsultaInventario": return Formulario.ConsultaInventario;
                case "FacturaConsignacion": return Formulario.FacturaConsignacion;
                case "SugerirBoleta": return Formulario.SugerirBoleta;
                case "FinalizarConsignacion": return Formulario.FinalizarConsignacion;
                case "ConfiguracionConsignacion": return Formulario.ConfiguracionConsignacion;
                case "ConsultaConsignacion": return Formulario.ConsultaConsignacion;
                case "AnulacionConsignacion": return Formulario.AnulacionConsignacion;
                case "FormularioNoDefinido": return Formulario.FormularioNoDefinido;
                case "TomaVentaConsignacion": return Formulario.TomaVentaConsignacion; 
            }
            return Formulario.MenuPrincipal;
        }
    }
        public enum TipoConsulta
        {
            Activos,
            Anulados,
            Anular
        }

        //LDS 03/02/2008 - Venta en consignación.
        //Se agrega el identificador "FacturaConsignacion" para el formulario frmAplicarFacturaConsignacion que es donde
        //se puede aplicar la factura generada por el desglose de la boleta de venta en consignación.
        public enum Formulario
        {
            BusquedaArticulo,
            AplicarPedido,
            ConfiguracionPedido,
            MenuPrincipal,
            SeleccionarCliente,
            CantidadPedido,
            DevolucionCantidades,
            DevolucionDetalle,
            TomaPedido,
            MontosPedido,
            ConsultaPedido,
            ConsultaPedidoAnulado,
            ConsultaFactura,
            InventarioCantidades,
            InventarioDetalle,
            ConsultaInventario,
            FacturaConsignacion,
            SugerirBoleta,
            FinalizarConsignacion,
            ConfiguracionConsignacion,
            ConsultaConsignacion,
            AnulacionConsignacion,
            FormularioNoDefinido,
            TomaVentaConsignacion
        }
        
}
