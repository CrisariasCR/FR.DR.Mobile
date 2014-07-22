using System;
using System.Collections.Generic;
using System.Text;

using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public static class Gestor
    {
        public static Inventarios Inventario = new Inventarios();
        public static Pedidos Pedidos = new Pedidos();
        public static Garantias Garantias = new Garantias();
        public static Deposito Deposito = new Deposito();
        public static List<CuentaBanco> Cuentas = new List<CuentaBanco>();
        public static BoletasConsignacion DesglosesConsignacion = new BoletasConsignacion();
        public static VentasConsignacion Consignaciones = new VentasConsignacion();
        public static Devoluciones Devoluciones = new Devoluciones();
        public static List<FacturaDescuento> DescuentosFacturas = new List<FacturaDescuento>();
    }
}
