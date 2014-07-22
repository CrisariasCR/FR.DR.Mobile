using System;
using Cirrious.MvvmCross.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    /// <summary>
    /// Clase que representa una bonificacion y o un descuento escalonado
    /// se utiliza en el view ConsultaBonYDesc para mostrar bonificaciones o descuentos
    /// </summary>
    public class BonYDesc : MvxNotifyPropertyChanged
    {
        public DateTime fecha;
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; RaisePropertyChanged("Fecha"); }
        }

        /// <summary>
        /// Código de la bonificacion, para Descuento es string.Empty
        /// </summary>
        public string codigo;
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; RaisePropertyChanged("Codigo"); }
        }

        public decimal minima;
        public decimal Minima
        {
            get { return minima; }
            set { minima = value; RaisePropertyChanged("Minima"); }
        }

        public decimal maxima;
        public decimal Maxima
        {
            get { return maxima; }
            set { maxima = value; RaisePropertyChanged("Maxima"); }
        }

        public decimal cantidad;
        public decimal Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; RaisePropertyChanged("Cantidad"); }
        }


        public BonYDesc(Bonificacion x)
        {
           Fecha = x.FechaFin;
            Codigo = x.ArticuloBonificado.Codigo;
            Minima = x.BaseMinima;
            Maxima = x.BaseMaxima;
            Cantidad = x.Cantidad;
        }

        public BonYDesc(Descuento x)
        {
            Fecha = x.FechaFin;
            Codigo = "";
            Minima = x.UdsMinimas;
            Maxima = x.UdsMaximas;
            Cantidad = x.Monto;
        }

    }
}
