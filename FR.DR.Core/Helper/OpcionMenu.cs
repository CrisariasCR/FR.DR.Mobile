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

namespace FR.DR.Core.Helper
{
    public class OpcionMenu
    {
        private string descripcion;

        public string Descripcion
        {
            get 
            {
                switch (descripcion)
                {
                    case "Código":return "Codigo";
                    case "Código Barras":return "Barras";
                    case "Descripción":return "Descripcion";                    
                    case "Pedido Actual":return "PedidoActual";
                    case "Factura Actual":return "FacturaActual";
                    case "Venta Actual":return "VentaActual";
                    case "Boleta Actual":return "BoletaActual";
                    default: return descripcion; 
                }
            }
            set { descripcion = value; }
        }

        public string DescripcionEnum 
        {
            get
            {
                switch (descripcion)
                {
                    case "Código": return "Codigo";
                    case "Código Barras": return "Barras";
                    case "Descripción": return "Descripcion";
                    case "Pedido Actual": return "PedidoActual";
                    case "Factura Actual": return "FacturaActual";
                    case "Venta Actual": return "VentaActual";
                    case "Boleta Actual": return "BoletaActual";
                    case "Miércoles": return "Miercoles";
                    case "Sábado": return "Sabado";
                    case "No Visitados": return "NoVisitados";
                    default: return descripcion;
                }
            }
        }

        public OpcionMenu(object descripcion) 
        {
            this.Descripcion = descripcion.ToString();
        }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}