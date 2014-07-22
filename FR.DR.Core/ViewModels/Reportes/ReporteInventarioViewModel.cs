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
    public class ReporteInventarioViewModel : ListaArticulosViewModel
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

        public static bool ActualizarAlResumir;

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

   

        /// <summary>
        /// Corresponde a la bodega de donde se debe realizar el inventario.
        /// </summary>
        private string bodega;

        #endregion Propiedades

        public ReporteInventarioViewModel()
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            ActualizarAlResumir = false;
            bodega = Ruta.ObtenerBodega();
            if (!bodega.Equals(string.Empty))
            {
                List<string> cias = new List<string>();
                cias.Add("TODAS");
                cias.AddRange(Compania.ObtenerEnInventario().Select(c => c.Codigo).ToList());
                this.Companias = new SimpleObservableCollection<string>(cias);
                if (Companias.Count >= 2) CompaniaActual = Companias[1];
                else CompaniaActual = Companias[0];
                

                CriterioActual = CriterioArticulo.Codigo;

                #region MejorasFRTostadoraElDorado600R6 JEV
                //if (FRdConfig.UtilizaTomaFisica)
                //{
                //    btTomaFisica.Enabled = true;
                //}
                //else
                //{
                //    btTomaFisica.Enabled = false;
                //}
                #endregion MejorasFRTostadoraElDorado600R6 JEV
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

        public ICommand ComandoTomaFisica
        {
            get { return new MvxRelayCommand(TomaFisica); }
        }

        public ICommand ComandoTomaTrasiego
        {
            get { return new MvxRelayCommand(TomaTrasiego); }
        }


        public override ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(EjecutarCriterio); }
        }

        //Caso: 32617 ABC 16/05/2008
        //Agregar Criterio de Busqueda Reporte Inventario FR
        /// <summary>
        /// Ejecuta el criterio de la busqueda
        /// </summary>
        public void EjecutarCriterio()
        {
            if (this.CriterioActual != CriterioArticulo.Ninguno && this.CompaniaActual != null)
            {
                this.CargarInventario(false);
            }
        }
        private void TomaTrasiego()
        {
            ActualizarAlResumir = false;
            if (CompaniaActual.ToUpper().Equals("TODAS"))
            {
                this.mostrarAlerta("Debe seleccionar una compañía para continuar.");
                return;
            }
            else
            {
                Dictionary<string, object> d = new Dictionary<string, object>();
                d.Add("pCompania", CompaniaActual);
                d.Add("pBodega", bodega);

                //TODO
                if ((JornadaRuta.VerificarJornadaAbierta() && !JornadaRuta.VerificarJornadaCerrada()) || !FRdConfig.UsaJornadaLaboral)
                {
                    this.RequestDialogNavigate<TomaTrasiegoViewModel, bool>(d,
                     result =>
                     {
                         ActualizarAlResumir = result;
                     });
                }
                else
                {
                    if (!JornadaRuta.VerificarJornadaAbierta())
                        this.mostrarAlerta("Debe abrir la jornada antes de hacer un traspaso");
                    else
                        this.mostrarAlerta("No puede realizar traspasos pues ya cerró la jornada");
                }
            }
        }

        public void ActualizarLista() 
        {
            if (ActualizarAlResumir)
            {
                CargarInventario(true);
                ActualizarAlResumir = false;
            }
        }

        private void TomaFisica()
        {
            ActualizarAlResumir = false;
            if (CompaniaActual.ToUpper().Equals("TODAS"))
            {
                this.mostrarAlerta("Debe seleccionar una compañía para continuar.");
                return;
            }
            else
            {
                if (FRdConfig.FacturarDiferencias && string.IsNullOrEmpty(FRmConfig.ClienteRutero))
                {
                    this.mostrarAlerta("No se tiene configurado el código de cliente para facturar las diferencias de la toma física.");
                }
                else
                {
                    Dictionary<string, object> d = new Dictionary<string, object>();
                    d.Add("pCompania", CompaniaActual);
                    d.Add("pBodega", bodega);

                    //TODO
                    if ((JornadaRuta.VerificarJornadaAbierta() && !JornadaRuta.VerificarJornadaCerrada()) || !FRdConfig.UsaJornadaLaboral)
                    {
                        this.RequestDialogNavigate<ConexionTomFisicaViewModel, bool>(d,
                         result =>
                         {

                             //Se recarga el inventario en la pantalla
                             ActualizarAlResumir = result;
                         });
                    }
                    else
                    {
                        if (!JornadaRuta.VerificarJornadaAbierta())
                            this.mostrarAlerta("Debe abrir la jornada antes de hacer una toma física");
                        else
                            this.mostrarAlerta("No puede realizar toma física pues ya cerró la jornada.");
                    }
                }
            }
        }
        /// Permite realizar la impresion del reporte siempre y cuando
        /// haya información para imprimirlo
        /// </summary>
        private void Imprime()
		{
            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el reporte de inventario",
                resp =>
                {
                    if (resp == DialogResult.Yes)
                    {

                        if (this.Items.Count != 0)
                        {
                            ReporteInventario reporte = null;
                            if (this.CompaniaActual == null)
                            {
                                //Caso:32617 ABC 19/05/2008
                                reporte = new ReporteInventario(this, this.bodega, this.Companias, this.Items);
                            }
                            else
                            {
                                //Caso:32617 ABC 19/05/2008
                                reporte = new ReporteInventario(this, this.bodega, new List<string> { CompaniaActual }, this.Items);
                            }
                            reporte.Imprime();
                        }
                        else
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "El reporte no tiene información.");
                        }
                    }
                });
		}

        #endregion Acciones

        /// <summary>
        /// Carga el inventario en el ListView según la bodega especificada.
        /// </summary>
        public void CargarInventario(bool cambioCias)
        {
            if (this.CompaniaActual != null && this.CriterioActual != CriterioArticulo.Ninguno)
            {
                try
                {
                    //Caso: 32617 ABC 16/05/2008
                    //Agregar Criterio de Busqueda Reporte Inventario FR
                    if (datos == null || this.SinExistencia || cambioCias)
                    {
                        //ABC 22/10/2008
                        //Caso: 33706 Reflejo Multicompañia FR 5.0 a FR 6.0
                        if (this.CompaniaActual.Equals("TODAS"))
                        {
                            datos = Bodega.CargarInventario(bodega, string.Empty, this.SinExistencia);
                        }
                        else
                        {
                            datos = Bodega.CargarInventario(bodega, this.CompaniaActual, this.SinExistencia);
                        }
                    }

                    if (this.TextoBusqueda != string.Empty)
                        Items = new SimpleObservableCollection<DetalleInventario>(datos.Buscar(this.CriterioActual, TextoBusqueda, false).Lista);
                    else
                        Items = new SimpleObservableCollection<DetalleInventario>(datos.Lista);

                    //binding
                    //  Articulo.Codigo
                    //  Articulo.Descripcion
                    //  UnidadesAlmacen
                    //  UnidadesDetalle
                    // Articulo.codigoBarras
                    // Articulo.Familia
                    // Articulo.Clase
                        
                    if (SinExistencia) datos = null;

                    this.TotalLineas = this.Items.Count;
                    //Se suma las unidades de detalle de articulo.
                    this.TotalArticulos = Items.Sum(r => r.UnidadesAlmacen);
                    RaisePropertyChanged("Items");
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta( "Error cargando el inventario. " + ex.Message);
                }
            }
        }

        public void CambioDeTextoEnBusqueda(string textoConsulta)
        {
            if (TextoBusqueda != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                //if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                //    textoConsulta = textoConsulta.Substring(0, textoConsulta.Length - 1);
                this.TextoBusqueda = textoConsulta;
                this.EjecutarCriterio();
                if(Items.Count>0)
                    EsBarras = true;
            }
        }
    }
}
