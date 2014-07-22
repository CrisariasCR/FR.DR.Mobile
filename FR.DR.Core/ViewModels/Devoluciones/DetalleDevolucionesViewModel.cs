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

using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.Core.Model;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleDevolucionesViewModel : ListaArticulosViewModel
    {
        public DetalleDevolucionesViewModel(bool esConsulta)
            : base()
        {
            EsConsulta = esConsulta;
            CargaInicial();
        }

        #region Propiedades

        public bool EsConsulta { get; set; }
        public static Devoluciones Devoluciones;
        private DetallesDevolucion DetallesDevolucion = new DetallesDevolucion();
        private DetallesPedido detallesFactura = null;
        private bool EliminoLineas = false;
        public bool EsBarras = false;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }
        

        private IObservableCollection<DetalleDevolucion> detalles;
        public IObservableCollection<DetalleDevolucion> Detalles
        {
            get { return detalles; }
            set { detalles = value; RaisePropertyChanged("Detalles"); }
        }

        private DetalleDevolucion detalleSeleccionado;
        public DetalleDevolucion DetalleSeleccionado
        {
            get { return detalleSeleccionado; }
            set { detalleSeleccionado = value; RaisePropertyChanged("DetalleSeleccionado"); }
        }

        public override string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { textoBusqueda = value; TextoCambia(); RaisePropertyChanged("TextoBusqueda"); }
        }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private string companiaSeleccionada;
        public string CompaniaSeleccionada
        {
            get { return companiaSeleccionada; }
            set { companiaSeleccionada = value; EjecutarBusquedaDetalles(); RaisePropertyChanged("CompaniaSeleccionada"); }
        }


        private decimal totalArticulos;
        public decimal TotalArticulos
        {
            get { return totalArticulos; }
            set { totalArticulos = value; RaisePropertyChanged("TotalArticulos"); }
        }

        #endregion

        #region Metodos Logica de Negocio


        private void CargaInicial()
        {
            var lista = Devoluciones.Gestionados.Select(x => x.Compania).ToList();
            Companias = new SimpleObservableCollection<string>(lista);
            CompaniaSeleccionada = Companias.Count > 0 ? Companias[0] : null;
        }

        public void EjecutarBusquedaDetalles()
        {
            if (string.IsNullOrEmpty(CompaniaSeleccionada))
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }
            try
            {
                Devolucion devolucion = Devoluciones.Buscar(CompaniaSeleccionada);

                if (devolucion != null)
                    this.DetallesDevolucion = devolucion.Detalles.Buscar(CriterioActual, TextoBusqueda, false, Estado.NoDefinido);
                else
                    this.DetallesDevolucion = new DetallesDevolucion();

                if (this.DetallesDevolucion.Vacio()&&CriterioActual!=CriterioArticulo.Barras)
                    this.mostrarMensaje(Mensaje.Accion.BusquedaMala);

                Detalles = new SimpleObservableCollection<DetalleDevolucion>(DetallesDevolucion.Lista);

                TotalArticulos = Detalles.Sum(p => p.UnidadesAlmacen);
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
            }
        }

        private void TextoCambia()
        {
            if (TextoBusqueda != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                //if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                //    TextoBusqueda = TextoBusqueda.Substring(0, TextoBusqueda.Length - 1);
                CriterioActual = CriterioArticulo.Barras;
                this.EjecutarBusquedaDetalles();
                //TextoBusqueda = string.Empty;
                if(this.Detalles.Count>0)
                    EsBarras = true;
            }
        }

        private void Elimina()
        {
            if (DetalleSeleccionado == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un detalle");
                return;
            }

            this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo: " + DetalleSeleccionado.Articulo.Codigo, result =>
            {
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        Devoluciones.Gestionar(DetalleSeleccionado.Articulo,
                            GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaSeleccionada), GlobalUI.RutaActual.Codigo, GlobalUI.RutaActual.Bodega, DetalleSeleccionado.Estado, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

                        if (detallesFactura != null)
                        {
                            DetallePedido det = this.detallesFactura.Buscar(DetalleSeleccionado.Articulo.Codigo);
                            det.CantidadDevuelta -= DetalleSeleccionado.TotalAlmacen;
                        }

                        this.Detalles.Remove(DetalleSeleccionado);
                        this.EliminoLineas = true;
                        TotalArticulos = Detalles.Sum(p => p.UnidadesAlmacen);
                        DetalleSeleccionado = null;
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error eliminando detalle. " + ex.Message);
                    }
                }
            });


        }

        private void Regresar()
        {
            if (this.EliminoLineas)
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, "guardar los cambios", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            foreach (Devolucion dev in Devoluciones.Gestionados)
                                dev.Actualizar(true);
                            if (TomaDevolucionesDocumentosViewModel.esConsulta)
                            {
                                DoClose();
                                this.RequestNavigate<TomaDevolucionesDocumentosViewModel>(new Dictionary<string, object>() { { "tipo", TomaDevolucionesDocumentosViewModel.stipoDevolucion }, { "pais", TomaDevolucionesDocumentosViewModel.sPais }, { "divA", TomaDevolucionesDocumentosViewModel.sDiv1 }, { "divB", TomaDevolucionesDocumentosViewModel.sDiv2 } });
                            }
                            else
                            {
                                if (TomaDevolucionesViewModel.esConsulta)
                                {
                                    DoClose();
                                    this.RequestNavigate<TomaDevolucionesViewModel>(new Dictionary<string, object>() { { "tipo", TomaDevolucionesViewModel.stipoDevolucion }, { "pais", TomaDevolucionesViewModel.sPais }, { "divA", TomaDevolucionesViewModel.sDiv1 }, { "divB", TomaDevolucionesViewModel.sDiv2 } });
                                }
                                else
                                {
                                    DoClose();
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error actualizando devoluciones. " + ex.Message, x =>
                            {
                                if (TomaDevolucionesDocumentosViewModel.esConsulta)
                                {
                                    DoClose();
                                    this.RequestNavigate<TomaDevolucionesDocumentosViewModel>(new Dictionary<string, object>() { { "tipo", TomaDevolucionesDocumentosViewModel.stipoDevolucion }, { "pais", TomaDevolucionesDocumentosViewModel.sPais }, { "divA", TomaDevolucionesDocumentosViewModel.sDiv1 }, { "divB", TomaDevolucionesDocumentosViewModel.sDiv2 } });
                                }
                                else
                                {
                                    if (TomaDevolucionesViewModel.esConsulta)
                                    {
                                        DoClose();
                                        this.RequestNavigate<TomaDevolucionesViewModel>(new Dictionary<string, object>() { { "tipo", TomaDevolucionesViewModel.stipoDevolucion }, { "pais", TomaDevolucionesViewModel.sPais }, { "divA", TomaDevolucionesViewModel.sDiv1 }, { "divB", TomaDevolucionesViewModel.sDiv2 } });
                                    }
                                    else
                                    {
                                        DoClose();
                                    }
                                }
                            });
                        }
                    }
                });
            }
            else
            {
                if (TomaDevolucionesDocumentosViewModel.esConsulta)
                {
                    DoClose();
                    this.RequestNavigate<TomaDevolucionesDocumentosViewModel>(new Dictionary<string, object>() { { "tipo", TomaDevolucionesDocumentosViewModel.stipoDevolucion }, { "pais", TomaDevolucionesDocumentosViewModel.sPais }, { "divA", TomaDevolucionesDocumentosViewModel.sDiv1 }, { "divB", TomaDevolucionesDocumentosViewModel.sDiv2 } });
                }
                else
                {
                    if (TomaDevolucionesViewModel.esConsulta)
                    {
                        DoClose();
                        this.RequestNavigate<TomaDevolucionesViewModel>(new Dictionary<string, object>() { { "tipo", TomaDevolucionesViewModel.stipoDevolucion }, { "pais", TomaDevolucionesViewModel.sPais }, { "divA", TomaDevolucionesViewModel.sDiv1 }, { "divB", TomaDevolucionesViewModel.sDiv2 } });
                    }
                    else
                    {
                        DoClose();
                    }
                }
            }
        }

        private void Consultar()
        {
            if (DetalleSeleccionado == null)
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un artículo");
            else
            {
                Devolucion devolucion = Devoluciones.Buscar(DetalleSeleccionado.Articulo.Compania);

                if (devolucion == null)
                {
                    this.mostrarAlerta("No se encontró el encabezado de la devolución.");
                    return;
                }

                ConsultaArticuloViewModel.Articulo = DetalleSeleccionado.Articulo;
                this.RequestNavigate<ConsultaArticuloViewModel>();
            }
        }

        #endregion

        #region Comandos

        public override ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(EjecutarBusquedaDetalles); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(Consultar); }
        }
        public ICommand ComandoEliminar
        {
            get { return new MvxRelayCommand(Elimina); }
        }

        public ICommand ComandoRegresar
        {
            get { return new MvxRelayCommand(Regresar); }
        }

        #endregion
    }
}