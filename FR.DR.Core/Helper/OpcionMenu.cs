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
                    case "C�digo":return "Codigo";
                    case "C�digo Barras":return "Barras";
                    case "Descripci�n":return "Descripcion";                    
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
                    case "C�digo": return "Codigo";
                    case "C�digo Barras": return "Barras";
                    case "Descripci�n": return "Descripcion";
                    case "Pedido Actual": return "PedidoActual";
                    case "Factura Actual": return "FacturaActual";
                    case "Venta Actual": return "VentaActual";
                    case "Boleta Actual": return "BoletaActual";
                    case "Mi�rcoles": return "Miercoles";
                    case "S�bado": return "Sabado";
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