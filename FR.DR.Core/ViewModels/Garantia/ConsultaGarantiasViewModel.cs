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

using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaGarantiasViewModel : ListViewModel
    {
        public static bool SeleccionCliente;

        public ConsultaGarantiasViewModel()
        {      
            FRmConfig.EnConsulta = true;
            this.Refrescar();
        }

        #region Propiedades

        public List<TipoConsulta> Estados { get; set; }

        private TipoConsulta estadoSeleccionado;
        public TipoConsulta EstadoSeleccionado
        {
            get { return estadoSeleccionado; }
            set { estadoSeleccionado = value; Refrescar(); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        public TipoPedido TipoPedido { get; set; }

        private IObservableCollection<Garantia> garantias;
        public IObservableCollection<Garantia> Garantias
        {
            get { return garantias; }
            set { garantias = value; RaisePropertyChanged("Pedidos"); }
        }

        public static Garantia GarantiaSeleccionado { get; set; }

        private List<Garantia> ItemsSeleccionados
        {
            get
            {
                return new List<Garantia>(this.Garantias.Where<Garantia>(x => x.Seleccionado));
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
                for (int i = 0; i < Garantias.Count; i++)
                {
                    if (Garantias[i].Seleccionado)
                        result.Add(i);
                }
                return result;
            }
        }

        public bool Anulando { get; set; }

        #endregion

        #region Comandos

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarGarantia); }
        }

        #endregion

        #region Acciones

        private void ConsultarGarantia()
        {
            var CantSeleccionados = Garantias.Count(p => p.Seleccionado);

            if (CantSeleccionados == 1)
            {
                GarantiaSeleccionado = Garantias.First(p => p.Seleccionado);

                if (GarantiaSeleccionado.Detalles.Vacio())
                {
                    try
                    {
                        GarantiaSeleccionado.ObtenerDetalles();
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando detalles. " + ex.Message);
                        return;
                    }
                }

                bool bEsFactura=TipoPedido == TipoPedido.Factura;
                string sEsFactura;
                if(bEsFactura)
                {
                    sEsFactura="S";
                }
                else
                {
                    sEsFactura="N";
                }
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"invocadesde",Formulario.ConsultaPedido.ToString()},{"mostrarControles", "S"}, {"esFactura",sEsFactura }
                };

                RequestNavigate<DetalleGarantiaViewModel>();

                
            }
            else
            {
                string alerta = "una garantía";
                if (CantSeleccionados == 0)
                {
                    this.mostrarMensaje(Mensaje.Accion.SeleccionNula, alerta);
                }
                else
                {
                    this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "sólo " + alerta);
                }
            }
        }

        private void Refrescar()
        {

            Garantias = new SimpleObservableCollection<Garantia>(
                Garantia.ObtenerVentas(GlobalUI.ClienteActual, GlobalUI.ClienteActual.Zona));
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
        

        //Caso 25452 LDS 19/10/2007
        /// <summary>
        /// Imprime los documentos que han sido gestionados.
        /// </summary>
        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {          
            if (this.ItemsSeleccionados.Count == 1)
            {
                if (esOriginal)
                    this.ItemsSeleccionados[0].LeyendaOriginal = true;
                else
                {
                    this.ItemsSeleccionados[0].LeyendaOriginal = false;
                }
            }
            else if (this.ItemsSeleccionados.Count > 1)
            {
                if (this.TipoPedido == TipoPedido.Factura && esOriginal)
                    foreach (Garantia garantia in this.ItemsSeleccionados)
                    {
                        if (!garantia.Impreso)
                        {
                            garantia.LeyendaOriginal = true;
                        }
                    }
            }

            
            try
            {
                int cantidad = 0;
                cantidad = cantidadCopias;

                if (cantidad >= 0)
                {
                    Garantias garantias = new Garantias(this.ItemsSeleccionados, GlobalUI.ClienteActual);
                        garantias.ImprimeDetalleGarantia(cantidad, (DetalleSort.Ordenador)ordernarPor);
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

            //this.seleccionados.Clear();
        }

        /// <summary>
        /// Funcion encargada de realizar la impresion de los 
        /// pedidos seleccionados
        /// </summary>
        private void Imprime()
        {
            FRmConfig.EnConsulta = true;
            //Caso 25452 LDS 19/10/2007
            //this.seleccionados.Clear();

            if (this.Garantias.Count != 0)
            {
                string codCia = string.Empty;

                foreach (Garantia item in this.Garantias)
                {
                    if (item.Seleccionado)
                    {
                        string compania = item.Compania;
                        string numPed = item.Numero;

                        try
                        {
                            if (item.Detalles.Vacio())
                            {
                                //Cargamos los detalles del pedido que se imprimirá
                                item.ObtenerDetalles();
                            }
                            //Pedido pedido = CargarDetallePedidoImprime(compania, numPed);
                            //this.seleccionados.Add(pedido);
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error cargando información de la garantía '" + numPed + "'. " + ex.Message);
                        }
                    }
                }

                if (this.ItemsSeleccionados.Count != 0)
                {
                    //Caso 25452 LDS 19/10/2007
                    this.VerificarImpresion();
                }
            }
        }

        //Caso 25452 LDS 19/10/2007
        /// <summary>
        /// Valida si el usuario desea imprimir el detalle de los pedidos gestionados.
        /// </summary>
        private void VerificarImpresion()
        {

            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las garantías seleccionadas", res =>
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
                        ImpresionViewModel.OnImprimir = ImprimirDocumento;
                        this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Detalle de Garantías", mostrarCriterioOrden = true });
                    }
                });

        }

        #endregion
    }
}