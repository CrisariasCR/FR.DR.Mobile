using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Softland.ERP.FR.Mobile.Cls;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.UI;
using Cirrious.MvvmCross.Commands;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleGarantiaViewModel : ListaArticulosViewModel
    {
#pragma warning disable 169

        public DetalleGarantiaViewModel()
            : base()
        {
            Encabezados = new Garantias();
            Garantia = ConsultaGarantiasViewModel.GarantiaSeleccionado;
            Encabezados.Gestionados.Add(Garantia);
            Encabezados.ConfigDocumentoCia.Add(Garantia.Compania.ToUpper(), Garantia.Configuracion);                   
            Companias = new SimpleObservableCollection<string>(Encabezados.Gestionados.Select(item => item.Compania).ToList());
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
            this.CargarCriterios();
        }

        #region Propiedades

        private bool eliminoLineas;
        public bool EliminoLineas
        {
            get { return eliminoLineas; }
        }

        public string NameCliente
        {
            get
            {                
                  return " - Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private string DesdeInvocacion;

        public bool EsBarras = false;

        protected new string textoBusqueda = string.Empty;
        public new string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { if (value != textoBusqueda) { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); BusquedaCodigoBarras(value); } }
        }

        public new IObservableCollection<CriterioArticulo> Criterios { get; set; }

        protected new CriterioArticulo criterioActual;
        public new CriterioArticulo CriterioActual
        {
            get { return criterioActual; }
            set
            {
                if (value != criterioActual)
                {
                    criterioActual = value;
                    RaisePropertyChanged("CriterioActual");
                }
            }
        }

        private String companiaActual;
        public String CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                if (value != companiaActual)
                {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                    Refrescar();
                }
            }
        }

        public IObservableCollection<String> Companias { get; set; }

        public Garantias Encabezados { get; set; }

        private DetallesGarantias detalles;
        public DetallesGarantias Detalles
        {
            get { return detalles; }
            set { detalles = value; RaisePropertyChanged("Detalles"); }
        }

        public DetalleGarantia DetalleSeleccionado { get; set; }

        private Garantia garantia;
        public Garantia Garantia
        {
            get;
            set;
            //get { return ConsultaPedidosViewModel.PedidoSeleccionado; }
        }

        private decimal totalArticulos;
        public decimal TotalArticulos
        {
            get { return totalArticulos; }
            set { totalArticulos = value; RaisePropertyChanged("TotalArticulos"); }
        }

        public static NivelPrecio NivelPrecio { get; set; }        

        #endregion

        #region Comandos

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarArticulo); }
        }

        #endregion

        #region Acciones

        public override void Refrescar()
        {
            Garantia garantia = Encabezados.Buscar(CompaniaActual);

            if (garantia != null)
                this.Detalles = garantia.Detalles.Buscar(CriterioActual, TextoBusqueda, false, true);
            else
                this.Detalles = new DetallesGarantias();

            TotalArticulos = Detalles.Lista.Sum(p => p.UnidadesAlmacen);
        }

        public void Regresar()
        {
            //Indicamos que la accion es continuar

            this.Encabezados.LimpiarValores();
            this.DoClose();            
        }

        private void ConsultarArticulo()
        {
            if (DetalleSeleccionado != null)
            {
                ConsultaArticuloViewModel.Articulo = DetalleSeleccionado.Articulo;
                this.RequestNavigate<ConsultaArticuloViewModel>();
            }
        }

        public void Limpiar()
        {
            this.Encabezados.LimpiarValores();
        }

        private void CargarCriterios() 
        {
            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      CriterioArticulo.Familia,
                      CriterioArticulo.Clase
                    };
            CriterioActual = CriterioArticulo.Descripcion;
            
        }

        private void BusquedaCodigoBarras(string texto)
        {
            string textoConsulta = texto;

            if (textoConsulta != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                //if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                //    textoConsulta = textoConsulta.Substring(0, textoConsulta.Length - 1);
                this.Refrescar();
                if(Detalles.Lista.Count>0)
                    EsBarras = true;
            }
        }

        public override void DoClose()
        {
            base.DoClose();   
        }

        #endregion
    }
}