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
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using FR.DR.Core.Helper;
using System.Windows.Forms;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaCobroViewModel : ListViewModel
    {
        public static bool SeleccionCliente;

        public ConsultaCobroViewModel(bool anulando)
        {
            Anulando = anulando;
            this.NombreCliente = "Cliente: " + GlobalUI.ClienteActual.Nombre;
            Estados = new List<TipoConsulta>() { TipoConsulta.Activos, TipoConsulta.Anulados };
            EstadoSeleccionado = TipoConsulta.Activos;
        }

        #region Propiedades

        public string NombreCliente { get; set; }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        public List<TipoConsulta> Estados { get; set; }

        private TipoConsulta estadoSeleccionado;
        public TipoConsulta EstadoSeleccionado
        {
            get { return estadoSeleccionado; }
            set { estadoSeleccionado = value; Refrescar(); }
        }

        private IObservableCollection<Recibo> recibos;
        public IObservableCollection<Recibo> Recibos
        {
            get { return recibos; }
            set { recibos = value; RaisePropertyChanged("Recibos"); }
        }

        public static Recibo ReciboSeleccionado;

        public Recibo itemSeleccionado;
        public Recibo ItemSeleccionado
        {
            get { return itemSeleccionado; }
            set
            {
                if (value != itemSeleccionado)
                {
                    itemSeleccionado = value;
                    ReciboSeleccionado = value;
                    RaisePropertyChanged("ItemSeleccionado");
                }
            }
        }

        private List<Recibo> ItemsSeleccionados
        {
            get
            {
                return new List<Recibo>(this.Recibos.Where<Recibo>(x => x.Seleccionado));
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
                for (int i = 0; i < Recibos.Count; i++)
                {
                    if (Recibos[i].Seleccionado)
                        result.Add(i);
                }
                return result;
            }
        }

        public bool Anulando { get; set; }

        #endregion


        #region Comandos

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarCobro); }
        }

        public ICommand ComandoAnular
        {
            get { return new MvxRelayCommand(Anular); }
        }

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }

        #endregion

        #region Acciones

        private void Refrescar()
        {
            var filtro = (EstadoSeleccionado == TipoConsulta.Anulados) ? Cls.EstadoDocumento.Anulado : Cls.EstadoDocumento.Activo;
            Recibos = new SimpleObservableCollection<Recibo>(Recibo.CargaRecibosCliente(GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona, filtro));
        }

        private void ConsultarCobro()
        {

            int cantSeleccionados = Recibos.Count(p => p.Seleccionado);
            
            if (cantSeleccionados == 1)
            {
                ReciboSeleccionado = Recibos.First(p => p.Seleccionado);
                this.RequestNavigate<DetalleCobroViewModel>();
            }
            else
            {
                //MOSTRAR MESSAGEBOX
            }
        }


        private void Anular()
        {
            if (ItemsSeleccionados.Count > 0)
            {
                Recibo recibo = ItemsSeleccionados[0];
                this.mostrarMensaje(Mensaje.Accion.Anular, "el recibo '" + recibo.Numero + "'", result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            if (recibo.Detalles.Count == 0)
                            {
                                try
                                {
                                    //Se debe cargar los detalles para hacer la reversion de aplicaciones
                                    recibo.CargaDetalles();
                                }
                                catch (Exception ex)
                                {
                                    this.mostrarAlerta("Error cargando detalles de recibo. " + ex.Message);
                                    return;
                                }
                            }

                            try
                            {
                                recibo.Anular(true, GlobalUI.RutaActual.Codigo, this);
                                Recibos.Remove(recibo);
                                RaisePropertyChanged("Recibos");
                                Anular();
                            }
                            catch (Exception ex)
                            {
                                this.mostrarAlerta("Error al anular el recibo." + ex.Message);
                            }
                        }
                        else
                        {
                            List<Recibo> listarecibos = new List<Recibo>();
                            foreach (Recibo rec in Recibos)
                            {
                                if (rec == recibo)
                                {
                                    rec.Seleccionado = false;
                                }
                                listarecibos.Add(rec);
                                Recibos = new SimpleObservableCollection<Recibo>(listarecibos);
                            }

                        }
                    });

            }
        }

        /// <summary>
        /// ABC 37948
        /// CArga el detalle de los cobros
        /// </summary>
        /// <param name="reciboConsultar"></param>
        public void CargarDetalleCobro(Recibo reciboConsultar)
        {
            if (reciboConsultar.Detalles.Count == 0)
            {
                try
                {
                    reciboConsultar.CargaDetalles();
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando detalles. " + ex.Message);
                    return;
                }
            }

            if (reciboConsultar.ChequesAsociados.Count == 0)
            {
                try
                {
                    reciboConsultar.CargaChequesAplicados();
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando cheques aplicados. " + ex.Message);
                }
            }
        }

        private void Imprime()
        {
            if (ItemsSeleccionados != null) 
            {
                ItemsSeleccionados.Clear();
            }            
            if (this.Recibos.Count != 0)
            {
                for (int i = 0; i < this.Recibos.Count; i++)
                {
                    if (this.Recibos[i].Seleccionado)
                    {
                        this.CargarDetalleCobro((Recibo)this.recibos[i]);
                        this.ItemsSeleccionados.Add((Recibo)this.recibos[i]);
                    }
                }

                if (this.ItemsSeleccionados.Count != 0)
                {
                    this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de los recibos seleccionados", res =>
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
                                //this.RelocalizarImpresion(); 
                                ImpresionViewModel.OnImprimir = ImprimirDocumento;
                                this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Detalle de Recibos", mostrarCriterioOrden = false });
                            }
                        });                    
                }
                else
                {
                    this.mostrarMensaje(Mensaje.Accion.Imprimir, "el resumen de los recibos", res =>
                        {
                            List<Recibo> listarecibos = new List<Recibo>();
                            foreach (Recibo re in Recibos) 
                            {
                                listarecibos.Add(re);
                            }
                            if (res.Equals(DialogResult.Yes))
                            {
                                ReporteRecibos reporte = new ReporteRecibos();
                                reporte.ImprimeResumenRecibos(listarecibos, GlobalUI.ClienteActual,this);
                            }
 
                        });                    
                    Cobros.MontoCheques = 0;
                    Cobros.MontoNotasCreditoLocal = Cobros.MontoNotasCreditoDolar = 0;
                    Cobros.MontoFacturasLocal = Cobros.MontoFacturasDolar = 0;
                    //this.recibosSeleccionados = null;
                }
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay información para imprimir.");
            }
        }

        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {
            foreach (Recibo recibo in this.ItemsSeleccionados)
            {
                if (recibo.Detalles.Count == 0)
                {
                    try
                    {
                        recibo.CargaDetalles();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando detalles del recibo '" + recibo.Numero + "'. " + ex.Message);
                    }
                }
            }

            foreach (Recibo recibo in this.ItemsSeleccionados)
            {
                if (recibo.ChequesAsociados.Count == 0)
                {
                    try
                    {
                        recibo.CargaChequesAplicados();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando cheques aplicados al recibo '" + recibo.Numero + "'. " + ex.Message);
                    }
                }
            }

            if (this.ItemsSeleccionados.Count == 1)
            {                
                if (esOriginal)
                    ((Recibo)this.ItemsSeleccionados[0]).LeyendaOriginal = true;
            }
            else if (this.ItemsSeleccionados.Count > 1)
            {
                if (esOriginal)
                    foreach (Recibo recibo in this.ItemsSeleccionados)
                        if (!recibo.Impreso)
                            recibo.LeyendaOriginal = true;
            }

            try
            {
                int cantidad = cantidadCopias;                    

                if (cantidad >= 0)
                {
                    ReporteRecibos reporte = new ReporteRecibos(this.ItemsSeleccionados, GlobalUI.ClienteActual);
                    reporte.ImprimeDetalleRecibos(cantidad);
                }
            }
            catch (FormatException)
            {
                this.mostrarAlerta("Error obteniendo la cantidad de copias. Formato inválido.");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(ex.Message);
            }

            //this.recibosSeleccionados = null;
        }

        public void Regresar() 
        {
            
            if (!SeleccionCliente)
            {
                if (Anulando)
                {
                    this.DoClose();
                    RequestNavigate<MenuClienteViewModel>();
                }
                else
                {
                    this.DoClose();
                    RequestNavigate<MenuClienteViewModel>();                    
                }
            }
            else
            {
                this.DoClose();
            }
        }

        #endregion
    }
}