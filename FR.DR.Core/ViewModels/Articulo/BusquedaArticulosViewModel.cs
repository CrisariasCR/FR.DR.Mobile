using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

using FR.Core.Model;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI; //GlobalUI
using Softland.ERP.FR.Mobile.Cls;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class BusquedaArticulosViewModel : ListaArticulosViewModel
    {

        #region Propiedades

        public static bool SeleccionCliente;

        #region Articulos y ArticuloActual

        public Articulo ArticuloActual
        {
            get; set;
        }

        public bool EsBarras=false;

        public IObservableCollection<Articulo> Articulos { get; set; }

        #endregion Articulos

        #region Companías y CompaniaActual
        private ClienteCia companiaActual;
        public ClienteCia CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                if (value != companiaActual)
                {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                }
            }
        }

        protected new string textoBusqueda = string.Empty;
        public new string TextoBusqueda
        {
            get { return textoBusqueda; }
            set
            {
                if (value != textoBusqueda)
                { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); LecturaCodigoBarra(value); }
            }
        }

        public IObservableCollection<ClienteCia> Companias { get; set; }
        #endregion Companías y CompaniaActual

        #endregion Propiedades

        public BusquedaArticulosViewModel()
            : base()
        {
            CriterioActual = CriterioArticulo.Codigo;
            CargaInicial();
        }

        #region Comandos y Acciones

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand<Articulo>(ConsultarArticulo); }
        }

        public override void Refrescar()
        {
            if (TextoBusqueda != null && CompaniaActual != null)
            {
                Cls.FRArticulo.CriterioBusquedaArticulo criterio = new Cls.FRArticulo.CriterioBusquedaArticulo(CriterioActual, TextoBusqueda, false);
                List<Articulo> lista = Articulo.ObtenerArticulos(
                        criterio,
                        CompaniaActual.Compania,
                        GlobalUI.RutaActual.GrupoArticulo,
                        GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual.Compania).NivelPrecio.Nivel);
                Articulos = new SimpleObservableCollection<Articulo>(lista);
                RaisePropertyChanged("Articulos");
            }
        }

        private void LecturaCodigoBarra( string textoConsulta)
        {
            string texto = textoConsulta;
            if (texto.Equals(string.Empty) && CriterioActual != CriterioArticulo.Barras)
                this.Articulos.Clear();
            if (TextoBusqueda != string.Empty && (CriterioActual == CriterioArticulo.Barras || TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno)))
            {
                if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                    texto = texto.Substring(0, texto.Length - 1);
                if (this.Companias.Count > 0)
                {
                    try
                    {
                        CriterioActual = CriterioArticulo.Barras;
                        this.Refrescar();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
                    }
                }
                else
                {
                    this.mostrarAlerta("Seleccione una compañía.");
                    
                }
                //TextoBusqueda = string.Empty;
                if(Articulos.Count>0)
                    EsBarras = true;
            }
        }

        public void ConsultarArticulo(Articulo articuloSeleccionado)
        {
            if (articuloSeleccionado != null)
            {
                ConsultaArticuloViewModel.Articulo = articuloSeleccionado;
                this.RequestNavigate<ConsultaArticuloViewModel>();
            }
        }

        #endregion

        #region CargaInicial

        public void CargaInicial()
        {

            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
        }

        public void Regresar()
        {
            if (!SeleccionCliente)
            {
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"habilitarPedidos", true}
                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
                this.DoClose();
            }
            else
            {
                //this.RequestNavigate<OpcionesClienteViewModel>();
                this.DoClose();
            }
        }

        #endregion CargaInicial
    }

}