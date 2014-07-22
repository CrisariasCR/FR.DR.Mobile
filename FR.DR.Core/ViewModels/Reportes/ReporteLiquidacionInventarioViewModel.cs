using System;
//using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
//using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Corporativo;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ReporteLiquidacionInventarioViewModel : ListaArticulosViewModel
    {
        #region Propiedades

        #region Companías y CompaniaActual
        private string companiaActual;
        public string CompaniaActual
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

        private string loteActual;
        public string LoteActual
        {
            get { return loteActual; }
            set { loteActual = value; RaisePropertyChanged("LoteActual"); }
        }

        private IObservableCollection<string> lotes;
        public IObservableCollection<string> Lotes
        {
            get { return lotes; }
            set { lotes = value; RaisePropertyChanged("Lotes"); }
        }

        private List<string> lotesCombo = new List<string>();

        private string articuloDescripcion;
        public string ArticuloDescripcion 
        {
            get { return articuloDescripcion; }
            set { articuloDescripcion = value; }
        }

        private bool lotesVisible;
        public bool LotesVisible
        {
            get { return lotesVisible; }
            set { lotesVisible = value; RaisePropertyChanged("LotesVisible"); }
        }

        protected new string textoBusqueda = string.Empty;
        public new string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { if (value != textoBusqueda) { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); CambioDeTextoEnBusqueda(value); } }
        }

        public bool EsBarras = false;

        public IObservableCollection<string> Companias { get; set; }
        #endregion Companías y CompaniaActual

        private bool sinExistencia;
        public bool SinExistencia
        {
            get { return sinExistencia; }
            set
            {
                if (value != sinExistencia)
                {
                    sinExistencia = value;
                    RaisePropertyChanged("SinExistencia");
                } 
            }
        }

        Articulo articuloDatos = null;

        #region Items
        public IObservableCollection<DetalleInventario> Items { get; set; }
        #endregion Items

        /// <summary>
        /// Corresponde al conjunto de detalles del inventario en la bodega.
        /// </summary>
        private DetallesInventario datos;

        private decimal totalLineas;
        public decimal TotalLineas
        {
            get { return totalLineas; }
            set
            {
                if (value != totalLineas)
                {
                    totalLineas = value;
                    RaisePropertyChanged("TotalLineas");
                }
            }
        }


        private decimal totalArticulos;
        public decimal TotalArticulos
        {
            get { return totalArticulos; }
            set
            {
                if (value != totalArticulos)
                {
                    totalArticulos = value;
                    RaisePropertyChanged("TotalArticulos");
                }
            }
        }

        private decimal cantInicial;
        public decimal CantInicial
        {
            get { return cantInicial; }
            set { cantInicial = value; RaisePropertyChanged("CantInicial"); }
        }
        private decimal cantInicialDet;
        public decimal CantInicialDet
        {
            get { return cantInicialDet; }
            set { cantInicialDet = value; RaisePropertyChanged("CantInicialDet"); }
        }

        private decimal cantDisponible;
        public decimal CantDisponible
        {
            get { return cantDisponible; }
            set { cantDisponible = value; RaisePropertyChanged("CantDisponible"); }
        }
        private decimal cantDisponibleDet;
        public decimal CantDisponibleDet
        {
            get { return cantDisponibleDet; }
            set { cantDisponibleDet = value; RaisePropertyChanged("CantDisponibleDet"); }
        }

        private decimal cantDevuelta;
        public decimal CantDevuelta
        {
            get { return cantDevuelta; }
            set { cantDevuelta = value; RaisePropertyChanged("CantDevuelta"); }
        }
        private decimal cantDevueltaDet;
        public decimal CantDevueltaDet
        {
            get { return cantDevueltaDet; }
            set { cantDevueltaDet = value; RaisePropertyChanged("CantDevueltaDet"); }
        }

        private decimal cantVendida;
        public decimal CantVendida
        {
            get { return cantVendida; }
            set { cantVendida = value; RaisePropertyChanged("CantVendida"); }
        }
        private decimal cantVendidaDet;
        public decimal CantVendidaDet
        {
            get { return cantVendidaDet; }
            set { cantVendidaDet = value; RaisePropertyChanged("CantVendidaDet"); }
        }

        private decimal cantGarantia;
        public decimal CantGarantia
        {
            get { return cantGarantia; }
            set { cantGarantia = value; RaisePropertyChanged("CantGarantia"); }
        }
        private decimal cantGarantiaDet;
        public decimal CantGarantiaDet
        {
            get { return cantGarantiaDet; }
            set { cantGarantiaDet = value; RaisePropertyChanged("CantGarantiaDet"); }
        }

        private decimal cantVacios;
        public decimal CantVacios
        {
            get { return cantVacios; }
            set { cantVacios = value; RaisePropertyChanged("CantVacios"); }
        }
        private decimal cantVaciosDet;
        public decimal CantVaciosDet
        {
            get { return cantVaciosDet; }
            set { cantVaciosDet = value; RaisePropertyChanged("CantVaciosDet"); }
        }

        private decimal cantTotal;
        public decimal CantTotal
        {
            get { return cantTotal; }
            set { cantTotal = value; RaisePropertyChanged("CantTotal"); }
        }

        /// <summary>
        /// Corresponde a la bodega de donde se debe realizar el inventario.
        /// </summary>
        private string bodega;

        private string bodegaActual;
        public string BodegaActual 
        {
            get { return bodegaActual; }
            set { bodegaActual = value; RaisePropertyChanged("BodegaActual"); }
        }

        private SimpleObservableCollection<string> bodegas;

        public SimpleObservableCollection<string> Bodegas
        {
            get { return bodegas; }
            set { bodegas = value; RaisePropertyChanged("Bodegas"); }
        }

        #endregion Propiedades

        public ReporteLiquidacionInventarioViewModel()
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            bodega = Ruta.ObtenerBodega();
            if (!bodega.Equals(string.Empty))
            {
                List<string> l = new List<string>();
                l.Add(bodega);
                Bodegas = new SimpleObservableCollection<string>(l);
                List<string> cias = new List<string>();
                cias.AddRange(Compania.ObtenerEnInventario().Select(c => c.Codigo).ToList());
                this.Companias = new SimpleObservableCollection<string>(cias);
                if (Companias.Count == 2) CompaniaActual = Companias[1];
                else CompaniaActual = Companias[0];

                CriterioActual = CriterioArticulo.Codigo;                
            }
            else
            {
                this.mostrarAlerta("No existe bodega definida para facturación.", 
                    result => this.DoClose());
            }    
        }

        #endregion CargaInicial

        #region Comandos y Acciones

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }


        public override ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(Refrescar); }
        }

        /// Permite realizar la impresion del reporte siempre y cuando
        /// haya información para imprimirlo
        /// </summary>
        private void Imprime()
		{
            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el reporte de liquidación de Inventario.", resp =>
            {
                if (resp == DialogResult.Yes)
                {

                        ReporteLiquidacionInventario reporte = null;
                        reporte = new ReporteLiquidacionInventario(CantInicial, CantDisponible, CantGarantia,CantGarantiaDet, CantDevuelta,CantDevueltaDet, CantVacios, CantVendida,CantVendidaDet, BodegaActual,TextoBusqueda);
                        reporte.Imprime();
                   
                }
            });
		}

        #endregion Acciones

        public void Refrescar() 
        {
            CambioDeTextoEnBusqueda(TextoBusqueda);
        }

       
        public bool CambioDeTextoEnBusqueda(string Texto)
        {
            bool result = false;
            //TextoBusqueda = Texto;
            if (Texto.EndsWith(FRmConfig.CaracterDeRetorno))
            {
                TextoBusqueda = Texto.Substring(0, Texto.Length - 1).ToUpper();
            }

            if (!string.IsNullOrEmpty(Texto))
            {
                articuloDatos = new Articulo(Texto, CompaniaActual);
                if (!articuloDatos.CargarDatosArticulo(Texto, companiaActual))
                {
                    result = false;
                }
                else
                {
                    if (ObtenerDatosArticulo(true))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool ObtenerDatosArticulo(bool cargarLotes)
        {
            //articuloDatos = new Articulo();
            bool procesoExitoso = true;
            string loteArt = string.Empty;
            try
            {
                //Se cargan los datos del artículo
                if (procesoExitoso)
                {
                    procesoExitoso = articuloDatos.CargarDatosArticuloTrasiego(TextoBusqueda, CompaniaActual,true);
                }
                if (procesoExitoso)
                {
                    procesoExitoso = articuloDatos.CargarReporteLiquidacion(bodega);
                }
                //Se realiza la asignacion de datos del artículo en la pantalla
                if (procesoExitoso)
                {
                    //Se pone la descripción del artículo
                    ArticuloDescripcion = articuloDatos.Descripcion;
                    CantDisponible = articuloDatos.CantDisponible;
                    CantInicial = articuloDatos.CantInicial;
                    CantGarantia = articuloDatos.CantGarantia;
                    cantGarantiaDet = articuloDatos.CantGarantiaDet;
                    CantDevuelta = articuloDatos.CantDevueltos;
                    cantDevueltaDet = articuloDatos.CantDevueltosDet;
                    CantVendida = articuloDatos.CantVendidas;
                    CantVendidaDet = articuloDatos.CantVendidasDet;
                    CantVacios = articuloDatos.CantVacios;

                }
                else
                {
                    this.mostrarAlerta("El artículo indicado no existe en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "Problemas al tratar de mostrar los datos del artículo seleccionado."+ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }
    }
}
