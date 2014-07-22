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
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.Core.Model;

using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class CambioBonificacionViewModel : DialogViewModel<DetallePedido>
    {
        public CambioBonificacionViewModel(string messageId, string nivelPrecio = "", string ruta = "")
            : base(messageId)
        {
            this.nivelPrecio = nivelPrecio;
            this.ruta = ruta;
            Cargar();
        }

        #region Atributos y Propiedades

        public static ClienteCia Cliente { get; set; }
        public static DetallePedido Detalle { get; set; }
        public static LineaRegalia Regalia { get; set; }

        private string nivelPrecio;
        private string ruta;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private IObservableCollection<Articulo> articulos;
        public IObservableCollection<Articulo> Articulos
        {
            get { return articulos; }
            set { articulos = value; RaisePropertyChanged("Articulos"); }
        }

        public Articulo ArticuloSeleccionado { get; set; }

        private decimal cantidadMinima;
        public decimal CantidadMinima
        {
            get { return cantidadMinima; }
            set { cantidadMinima = value; RaisePropertyChanged("CantidadMinima"); }
        }

        private decimal cantidadMaxima;
        public decimal CantidadMaxima
        {
            get { return cantidadMaxima; }
            set { cantidadMaxima = value; RaisePropertyChanged("CantidadMaxima"); }
        }

        private decimal cantidadAlmacen;
        public decimal CantidadAlmacen
        {
            get { return cantidadAlmacen; }
            set { cantidadAlmacen = value; RaisePropertyChanged("CantidadAlmacen"); }
        }

        private decimal cantidadDetalle;
        public decimal CantidadDetalle
        {
            get { return cantidadDetalle; }
            set { cantidadDetalle = value; RaisePropertyChanged("CantidadDetalle"); }
        }

        private string regla;
        public string Regla
        {
            get { return regla; }
            set { regla = value; RaisePropertyChanged("Regla"); }
        }

        #endregion

        #region Logica

        private void Cargar()
        {
            Regla regla = Regalia.Regla;
            ConvertidorFiltro convertidor = new ConvertidorFiltro();
            String filtro = String.IsNullOrEmpty(regla.FiltroArticuloBonificacion) ? String.Empty : convertidor.ConvertirFiltroDevExASQL(regla.FiltroArticuloBonificacion);

            Articulos = new SimpleObservableCollection<Articulo>(ProbadorFiltros.ObtenerArticulos(regla.Compania, filtro, nivelPrecio, ruta));
            List<Articulo> temp = new List<Articulo>();

            foreach (Articulo art in Articulos)
            {
                //Sólo se pueden cambiar por otros artículos bonificados que tienen igual factor de empaque que el artículo en la escala
                if (art.UnidadEmpaque == Detalle.Articulo.UnidadEmpaque)
                {
                    temp.Add(art);
                }
            }
            Articulos = new SimpleObservableCollection<Articulo>(temp);

            Regla = regla.Codigo;// Regla1;
            CantidadMinima = Regalia.Escala.CantidadMinima;
            CantidadMaxima = Regalia.Escala.CantidadMaxima;
            CantidadAlmacen = Detalle.UnidadesAlmacen;
            CantidadDetalle = Detalle.UnidadesDetalle;
        }

        private void Aceptar()
        {
            if (ArticuloSeleccionado != null)
            {
                Articulo articulo = ArticuloSeleccionado;
                Cliente.CargarNivelPrecio();
                articulo.Cargar(new FiltroArticulo[] { FiltroArticulo.NivelPrecio }, Cliente.NivelPrecio.Nivel);
                articulo.CargarPrecio(Cliente.NivelPrecio);
                if (articulo != null)
                {
                    decimal cantAlmacen = Detalle.UnidadesAlmacen;
                    decimal cantDetalle = Detalle.UnidadesDetalle;

                    Detalle = new DetallePedido();
                    Detalle.Articulo = articulo;

                    Detalle.UnidadesAlmacen = cantAlmacen;
                    Detalle.UnidadesDetalle = cantDetalle;

                    Detalle.Precio = articulo.Precio;
                    Detalle.EsBonificada = true;
                }
            }
            ReturnResult(Detalle);
        }

        #endregion

        #region Comandos

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(DoClose); }
        }

        #endregion


    }
}