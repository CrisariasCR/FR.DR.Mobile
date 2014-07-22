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

using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using FR.Core.Model;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaDevolucionesViewModel : ListViewModel
    {
        public static bool SeleccionCliente;
        public ConsultaDevolucionesViewModel(string anular)
        {
            Anulando = anular.Equals("S");

            Criterios = new SimpleObservableCollection<TipoConsulta>()
            {
                TipoConsulta.Activos, TipoConsulta.Anulados
            };
            TipoConsultaSeleccionado = TipoConsulta.Activos;
        }

        #region Propiedades

        public bool Anulando { get; set; }

        private IObservableCollection<Devolucion> devoluciones;
        public IObservableCollection<Devolucion> Devoluciones
        {
            get { return devoluciones; }
            set { devoluciones = value; RaisePropertyChanged("Devoluciones"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        public IObservableCollection<TipoConsulta> Criterios { get; set; }

        private TipoConsulta tipoConsultaSeleccionado;
        public TipoConsulta TipoConsultaSeleccionado
        {
            get { return tipoConsultaSeleccionado; }
            set
            {
                tipoConsultaSeleccionado = value;
                RefrescarDevoluciones();
                RaisePropertyChanged("TipoConsultaSeleccionado");
            }
        }

        public Devolucion DevolucionSeleccionada { get; set; }

        private List<Devolucion> ItemsSeleccionados
        {
            get
            {
                return new List<Devolucion>(this.Devoluciones.Where<Devolucion>(x => x.Seleccionado));
            }

        }

        /// <summary>
        /// retorma la coleccion de Indices seleccionados
        /// </summary>
        public List<int> SelectedIndex
        {
            get
            {
                List<int> result = new List<int>();
                for (int i = 0; i < Devoluciones.Count; i++)
                {
                    if (Devoluciones[i].Seleccionado)
                        result.Add(i);
                }
                return result;
            }
        }

        #endregion

        #region Metodos Logica de Negocio

        private void RefrescarDevoluciones()
        {
            EstadoDocumento indiceAnulacion = TipoConsultaSeleccionado == TipoConsulta.Activos ?
                EstadoDocumento.Activo : EstadoDocumento.Anulado;
            var lista = Devolucion.ObtenerDevoluciones(GlobalUI.ClienteActual.Codigo, indiceAnulacion);
            Devoluciones = new SimpleObservableCollection<Devolucion>(lista);

        }

        private void Imprime()
        {
            //Caso 25452 LDS 30/10/2007
            //this.seleccionados = new List<Devolucion>();

            if (this.Devoluciones.Count == 0)
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay información a imprimir");
                return;
            }

            try
            {
                //foreach (ListViewItem item in this.lsvConDev.Items)
                //    if (item.Checked) this.seleccionados.Add(this.encabezados[item.Index]);

                if (this.ItemsSeleccionados.Count > 0)
                {
                    this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las devoluciones seleccionadas", res =>
                        {
                            if (res.Equals(DialogResult.Yes))
                            {
                                ImpresionViewModel.OriginalEn = true;
                                //Mejora para no imprimir original si esta ya se imprimio
                                if (this.ItemsSeleccionados.Count == 1)
                                {
                                    ImpresionViewModel.OriginalEn = !this.ItemsSeleccionados[0].Impreso;
                                }
                                else if (this.ItemsSeleccionados.Count > 1)
                                {
                                    ImpresionViewModel.OriginalEn = false;
                                }
                                //
                                //Caso 25452 LDS 30/10/2007
                                ImpresionViewModel.OnImprimir = ImprimirDocumento;
                                this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Detalle de Devoluciones", mostrarCriterioOrden = true });
                            }
                        });

                    
                }
                else
                {
                    this.mostrarMensaje(Mensaje.Accion.Imprimir, "el resumen de devoluciones", res => 
                    {
                        List<Devolucion> encabezados=new List<Devolucion>();
                        foreach(Devolucion dev in this.Devoluciones)
                        {
                            encabezados.Add(dev);
                        }
                        if (res.Equals(DialogResult.Yes))
                        {                         
                            (new Devoluciones(encabezados, GlobalUI.ClienteActual)).ImprimeResumenDevolucion(this);                         
                        }
                    });
                    
                }

            }
            catch (Exception es)
            {
                this.mostrarAlerta(es.Message);
            }
        }

        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {
            ImpresionViewModel.OriginalEn = true;
            //Cargamos los detalles de las devoluciones seleccionadas
            if (ItemsSeleccionados == null || ItemsSeleccionados.Count == 0)
            {
                this.mostrarAlerta("Debe seleccionar al menos una devolucion a imprimir");
                return;
            }            
            foreach (Devolucion dev in this.ItemsSeleccionados)
            {
                if (dev.Detalles.Vacio())
                {
                    try
                    {
                        //Cargamos los detalles de la devolucion
                        dev.ObtenerDetalles();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error al cargar el detalle de la devolución. " + ex.Message);
                    }
                }
            }

            if (this.ItemsSeleccionados.Count == 1)
            {
                if (esOriginal)
                    (this.ItemsSeleccionados[0]).LeyendaOriginal = true;
                else
                {
                    this.ItemsSeleccionados[0].LeyendaOriginal = false;
                }
            }
            else if (this.ItemsSeleccionados.Count > 1)
            {                
                if (esOriginal)
                    foreach (Devolucion devolucion in this.ItemsSeleccionados)
                        if (!devolucion.Impreso)
                            devolucion.LeyendaOriginal = true;
            }
            try
            {
                int cantidad = 0;
                cantidad = Util.CantidadCopias(cantidadCopias);

                if (cantidad >= 0)
                    (new Devoluciones(this.ItemsSeleccionados, GlobalUI.ClienteActual)).ImprimeDetalleDevolucion(cantidad, (DetalleSort.Ordenador)ordernarPor);
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(ex.Message);
            }
        }

        #endregion

        #region Comandos

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarDevolucion); }
        }

        public ICommand ComandoAnular
        {
            get { return new MvxRelayCommand(AnularDevolucion); }
        }

        #endregion

        #region Acciones

        private void ConsultarDevolucion()
        {

            int CantSeleccionados = Devoluciones.Count(x => x.Seleccionado);

            if (CantSeleccionados == 0)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una devolución");
                return;
            }

            else if (CantSeleccionados > 1)
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Debe seleccionar solo una devolución");
                return;
            }

            else
            {
                DevolucionSeleccionada = Devoluciones.First(x => x.Seleccionado);

                if (DevolucionSeleccionada.Detalles.Vacio())
                {
                    try
                    {
                        DevolucionSeleccionada.ObtenerDetalles();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando detalles. " + ex.Message);
                    }
                }

                Dictionary<string, object> Pars = new Dictionary<string, object>();
                Pars.Add("esConsulta", true);

                DetalleDevolucionesViewModel.Devoluciones = new Devoluciones();
                DetalleDevolucionesViewModel.Devoluciones.Gestionados.Add(DevolucionSeleccionada);

                try
                {
                    this.RequestNavigate<DetalleDevolucionesViewModel>(Pars);
                }
                catch (Exception e)
                {
                    var a = e;
                }
            }
        }

        private void AnularDevolucion()
        {
            var Seleccionados = Devoluciones.Where(x => x.Seleccionado).ToList();

            //Recorre la lista de devoluciones.
            foreach (Devolucion DevActual in Seleccionados)
            {
                this.mostrarMensaje(Mensaje.Accion.Anular, "la devolución '" + DevActual.Numero + "'", resultActual =>
                {
                    if (resultActual == DialogResult.Yes)
                    {
                        if (DevActual.Detalles.Vacio())
                        {
                            try
                            {
                                DevActual.ObtenerDetalles();
                            }
                            catch (Exception ex)
                            {
                                this.mostrarAlerta("Error cargando los detalles de la devolución. " + ex.Message);
                                return;
                            }
                        }
                        //LDS 12/02/2008 - Venta en consignación.
                        //Se debe eliminar las devoluciones que han sido generadas por el desglose de una boleta de venta en consignación.
                        //En caso de existir una boleta de venta en consignación se debe agregar los detalles de la devolución a la boleta con los precios actuales.
                        //En caso de no existir una boleta de venta en consignación se debe crear una boleta de venta en consignación con los detalles de la devolución al precio actual.
                        if (DevActual.EsConsignacion)
                        {
                            try
                            {
                                GestorDatos.BeginTransaction();
                                BoletasConsignacion boletas = new BoletasConsignacion();
                                boletas.Gestionados.Add(VentaConsignacion.ObtenerVentaConsignacion(GlobalUI.ClienteActual, GlobalUI.RutaActual, DevActual.Compania));
                                DevActual.BodegaConsigna = DevActual.Bodega;
                                DevActual.Bodega = GlobalUI.RutaActual.Bodega;
                                boletas.Devoluciones.Gestionados.Add(DevActual);
                                boletas.ActualizarBoletaPorDevolucion(true);
                                DevActual.Anular(false);
                                GestorDatos.CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                GestorDatos.RollbackTransaction();
                                this.mostrarAlerta("Error anulando la devolución '" + DevActual.Numero + "' generada por desglose de boleta de venta en consignación. " + ex.Message);
                                return;
                            }
                        }
                        else
                        {
                            //Caso 27702 LDS 19/03/2007
                            try
                            {
                                DevActual.Anular(true);
                                Devoluciones.Remove(DevActual);
                            }
                            catch (Exception ex)
                            {
                                this.mostrarAlerta("Error anulando devolución. " + ex.Message);
                            }
                        }
                    }
                });
            }
        }

        public void regresar()
        {            
            if (!SeleccionCliente)
            {
                this.DoClose();
                Dictionary<string, object> par = new Dictionary<string, object>();
                par.Add("habilitarPedidos", true);
                RequestNavigate<MenuClienteViewModel>(par);
            }
            else
            {
                this.DoClose();
            }
        }

        #endregion
    }
}