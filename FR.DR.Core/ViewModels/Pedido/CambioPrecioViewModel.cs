using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls;
using System.Globalization;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class CambioPrecioViewModel : DialogViewModel<Dictionary<string,object>>
    {
        public CambioPrecioViewModel(string messageId)
            : base(messageId)
        {
            Codigo = Detalle.Articulo.Codigo;
            Descripcion = Detalle.Articulo.Descripcion;
            Costo = Lista.Moneda == TipoMoneda.LOCAL ? Detalle.Articulo.CostoLocal : Detalle.Articulo.CostoDolar;

            PorcentajeRecargo = Detalle.Articulo.Precio.PorcentajeRecargo;
            PrecioMax = Detalle.Precio.Maximo;
            PrecioMin = Detalle.Precio.Minimo;
            UnidadEmpaque = Detalle.Articulo.UnidadEmpaque;

            try
            {
                MargenUtilidad = Detalle.Articulo.CargarMargenUtilidad(Lista);
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando el margen de utilidad del artículo. " + ex.Message);
            }

            Detalle = null;
            Lista = null;

            MostrarComponentes();
            CargaInicial();
        }

        #region Propiedades

        public static DetalleLinea Detalle { get; set; }
        public static NivelPrecio Lista { get; set; }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private string codigo;
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; RaisePropertyChanged("Codigo"); }
        }

        
        public string SimMoneda
        {
            get { return GestorUtilitario.SimboloMonetario; }            
        }

        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; RaisePropertyChanged("Descripcion"); }
        }

        private decimal costo;
        public decimal Costo
        {
            get { return costo; }
            set { costo = value; RaisePropertyChanged("Costo"); }
        }

        private decimal porcentajeRecargo;
        public decimal PorcentajeRecargo
        {
            get { return porcentajeRecargo; }
            set { porcentajeRecargo = value; RaisePropertyChanged("PorcentajeRecargo"); }
        }

        private decimal precioMax;
        public decimal PrecioMax
        {
            get { return precioMax; }
            set { precioMax = value; CalcularPrecioDetalle(); RaisePropertyChanged("PrecioMax"); }
        }

        private decimal precioMin;
        public decimal PrecioMin
        {
            get { return precioMin; }
            set { precioMin = value; RaisePropertyChanged("PrecioMin"); }
        }

        private decimal unidadEmpaque;
        public decimal UnidadEmpaque
        {
            get { return unidadEmpaque; }
            set { unidadEmpaque = value; RaisePropertyChanged("UnidadEmpaque"); }
        }

        private decimal margenUtilidad;
        public decimal MargenUtilidad
        {
            get { return margenUtilidad; }
            set { margenUtilidad = value; RaisePropertyChanged("MargenUtilidad"); }
        }

        private string utilidadDefinida;
        public string UtilidadDefinida
        {
            get { return utilidadDefinida; }
            set { utilidadDefinida = value; RaisePropertyChanged("UtilidadDefinida"); }
        }

        private string nuevaUtilidad;
        public string NuevaUtilidad
        {
            get { return nuevaUtilidad; }
            set { nuevaUtilidad = value; RaisePropertyChanged("NuevaUtilidad"); }
        }

        private bool nuevaUtilidadVisible;
        public bool NuevaUtilidadVisible
        {
            get { return nuevaUtilidadVisible; }
            set { nuevaUtilidadVisible = value; RaisePropertyChanged("NuevaUtilidadVisible"); }
        }

        private bool utilidadDefinidaVisible;
        public bool UtilidadDefinidaVisible
        {
            get { return utilidadDefinidaVisible; }
            set { utilidadDefinidaVisible = value; RaisePropertyChanged("UtilidadDefinidaVisible"); }
        }

        private bool costoVisible;
        public bool CostoVisible
        {
            get { return costoVisible; }
            set { costoVisible = value; RaisePropertyChanged("CostoVisible"); }
        }

        #endregion

        #region Logica

        private void CargaInicial()
        {
            decimal NuevaUtilidad = Decimal.Zero;

            //Porcentaje de utilidad actual
            UtilidadDefinida = Decimal.Round(this.MargenUtilidad * 100, 2).ToString() + "%";

            //Se calcula el margen de utilidad
            if (this.PrecioMax != 0)
            {
                NuevaUtilidad = 1 - (this.Costo / this.PrecioMax);
            }
            else
            {
                CultureInfo cultura = new CultureInfo("es-CR");
                NuevaUtilidad = 1 - (this.Costo / (this.PrecioMax + Convert.ToDecimal("0.00000001", cultura)));
            }
            //Nueva porcentaje de utilidad del artículo
            this.NuevaUtilidad = Decimal.Round(NuevaUtilidad * 100, 2).ToString() + "%";
        }

        private void MostrarComponentes()
        {
            NuevaUtilidadVisible = Pedidos.MostrarNuevaUtilidad;
            UtilidadDefinidaVisible = Pedidos.MostrarUtilidadDefinida;
         
            if (Pedidos.MostrarCostoArticulo)
            {
                if (!Pedidos.MostrarUtilidadDefinida)
                {
                    //Se corren los controles hacia arriba

                    //Nueva Utilidad
                    //this.lblUtilidadNueva.Location = this.lblPrecioDetalle.Location;
                    //this.ltbPorcUtilidadNueva.Location = this.ltbPrecioDetalle.Location;

                    ////Precio Detalle
                    //this.lblPrecioDetalle.Location = this.lblPrecioAlmacen.Location;
                    //this.ltbPrecioDetalle.Location = this.ltbPrecioAlmacen.Location;

                    ////Precio Almacen
                    //this.lblPrecioAlmacen.Location = this.lblUtilidadActual.Location;
                    //this.ltbPrecioAlmacen.Location = this.ltbPorcUtilidadActual.Location;
                }
            }
            else
            {
                CostoVisible = false;

                if (Pedidos.MostrarUtilidadDefinida)
                {
                    if (Pedidos.MostrarNuevaUtilidad)
                    {
                        //Nueva Utilidad
                        //this.lblUtilidadNueva.Location = this.lblPrecioDetalle.Location;
                        //this.ltbPorcUtilidadNueva.Location = this.ltbPrecioDetalle.Location;
                    }
                    //Precio Detalle
                    //this.lblPrecioDetalle.Location = this.lblPrecioAlmacen.Location;
                    //this.ltbPrecioDetalle.Location = this.ltbPrecioAlmacen.Location;

                    ////Precio Almacen
                    //this.lblPrecioAlmacen.Location = this.lblUtilidadActual.Location;
                    //this.ltbPrecioAlmacen.Location = this.ltbPorcUtilidadActual.Location;

                    ////Utilidad Definida
                    //this.lblUtilidadActual.Location = this.lblCosto.Location;
                    //this.ltbPorcUtilidadActual.Location = this.ltbCosto.Location;

                }
                else
                {
                    if (Pedidos.MostrarNuevaUtilidad)
                    {
                        //Nueva Utilidad
                        //this.lblUtilidadNueva.Location = this.lblPrecioAlmacen.Location;
                        //this.ltbPorcUtilidadNueva.Location = this.ltbPrecioAlmacen.Location;
                    }

                    //Precio Almacen
                    //this.lblPrecioAlmacen.Location = this.lblCosto.Location;
                    //this.ltbPrecioAlmacen.Location = this.ltbCosto.Location;

                    ////Precio Detalle
                    //this.lblPrecioDetalle.Location = this.lblUtilidadActual.Location;
                    //this.ltbPrecioDetalle.Location = this.ltbPorcUtilidadActual.Location;
                }
            }
        }

        private void CalcularPrecioDetalle()
        {
            //Precio detalle del artículo
            decimal PrecioDetalle = Decimal.Zero;
            //Precio almacén del artículo
            decimal PrecioAlmacen = PrecioMax;
            //Nuevo margen de utilidad del artículo
            decimal NuevaUtilidad = Decimal.Zero;

            //Caso 27302 LDS 02/08/2007
            //Se calcula el precio almacén del artículo.			
            if ((FRdConfig.AplicaRecargo) && (this.unidadEmpaque > 1))
            {
                decimal MontoRecargo = (PrecioAlmacen / this.unidadEmpaque) * (this.PorcentajeRecargo / 100);
                PrecioDetalle = (PrecioAlmacen / this.unidadEmpaque) + MontoRecargo;
            }
            else
            {
                if (this.unidadEmpaque == 0)
                    this.unidadEmpaque = 1;
                PrecioDetalle = PrecioAlmacen / this.unidadEmpaque;
            }

            //Precio detalle del artículo
            PrecioMin = PrecioDetalle;

            //Se calcula el margen de utilidad
            if (PrecioAlmacen != 0)
            {
                NuevaUtilidad = 1 - (this.Costo / PrecioAlmacen);
            }
            else
            {
                CultureInfo cultura = new CultureInfo("es-CR");
                NuevaUtilidad = 1 - (this.Costo / (PrecioAlmacen + Convert.ToDecimal("0.00000001", cultura)));
            }

            this.NuevaUtilidad = Decimal.Round(NuevaUtilidad * 100, 2).ToString() + "%";
        }

        private void ValidarPrecio()
        {
            //Precio de almacén del artículo.
            decimal PrecioAlmacen = PrecioMax;
            decimal PrecioDetalle = PrecioMin;

            bool valido = true;


            if (PrecioAlmacen < this.Costo)
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "El precio debe ser mayor o igual al costo.");
                valido = false;
                return;
            }

            if (Pedidos.ModoCambiosPrecios == Pedidos.CAMBIOPRECIO_HACIA_ARRIBA
                && ((PrecioAlmacen < this.PrecioMax) || (PrecioDetalle < this.PrecioMin)))
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "El precio no puede ser menor al establecido en la lista de precios.");

                //Precio almacén del artículo
                //this.ltbPrecioAlmacen.Text = Decimal.Round(this.PrecioMax, 2).ToString();

                //Precio detalle del artículo
                //this.ltbPrecioDetalle.Text = Decimal.Round(this.PrecioMin, 2).ToString();

                valido = false;

                return;
            }
            else
                valido = true;

            if (valido)
            {
                Result(true);
            }
        }

        #endregion

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(ValidarPrecio); }
        }

        private void Cancelar()
        {
            Result(false);
        }

        public void Result(bool correcto)
        {
            Dictionary<string, object> Result = new Dictionary<string, object>();

            Result.Add("correcto", correcto);
            Result.Add("precioAlmacen", PrecioMax);
            Result.Add("precioDetalle", PrecioMin);

            ReturnResult(Result);
        }

        #endregion
    }
}