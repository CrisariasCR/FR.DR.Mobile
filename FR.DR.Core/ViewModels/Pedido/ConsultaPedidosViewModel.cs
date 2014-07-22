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
    public class ConsultaPedidosViewModel : ListViewModel
    {
        public static bool SeleccionCliente;

        public ConsultaPedidosViewModel(string tipoPedido, bool anular)
        {
            if (tipoPedido.Equals("P"))
            {
                TipoPedido = TipoPedido.Prefactura;
            }
            else
            {
                TipoPedido = TipoPedido.Factura;
            }
            
            Estados = new List<TipoConsulta>() { TipoConsulta.Activos, TipoConsulta.Anulados };
            EstadoSeleccionado = TipoConsulta.Activos;
            FRmConfig.EnConsulta = true;
            Anulando = anular;
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

        private IObservableCollection<Pedido> pedidos;
        public IObservableCollection<Pedido> Pedidos
        {
            get { return pedidos; }
            set { pedidos = value; RaisePropertyChanged("Pedidos"); }
        }

        public static Pedido PedidoSeleccionado { get; set; }

        private List<Pedido> ItemsSeleccionados
        {
            get
            {
                return new List<Pedido>(this.Pedidos.Where<Pedido>(x => x.Seleccionado));
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
                for (int i = 0; i < Pedidos.Count; i++)
                {
                    if (Pedidos[i].Seleccionado)
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
            get { return new MvxRelayCommand(ConsultarPedido); }
        }

        public ICommand ComandoAnular
        {
            get { return new MvxRelayCommand(AnularDocumento); }
        }

        #endregion

        #region Acciones

        private void ConsultarPedido()
        {
            var CantSeleccionados = Pedidos.Count(p => p.Seleccionado);

            if (CantSeleccionados == 1)
            {
                PedidoSeleccionado = Pedidos.First(p => p.Seleccionado);

                if (PedidoSeleccionado.Detalles.Vacio())
                {
                    try
                    {
                        PedidoSeleccionado.ObtenerDetalles();
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

                RequestNavigate<DetallePedidoViewModel>(Parametros);

                //this.RequestDialogNavigate<DetallePedidoViewModel, DialogResult>(Parametros, result =>
                //{
                //    if (result == DialogResult.OK)
                //    {
                //        Gestor.Pedidos = new Pedidos();
                //        Gestor.Pedidos.Gestionados.Add(pedido);
                //        Gestor.Pedidos.ConfigDocumentoCia.Add(pedido.Compania.ToUpper(), pedido.Configuracion);

                //        frmTomaPedido formPedido = new frmTomaPedido(Formulario.ConsultaPedido, pedido.Configuracion.Nivel);
                //        formPedido.Show();

                //        DoClose();
                //    }
                //});
            }
            else
            {
                string alerta = TipoPedido == TipoPedido.Factura ? "una factura" : "un pedido";
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
            EstadoPedido estadoPedido = (EstadoSeleccionado == TipoConsulta.Anulados ? EstadoPedido.Cancelado :
                                        (TipoPedido == TipoPedido.Factura ? EstadoPedido.Facturado : EstadoPedido.Normal));

            Pedidos = new SimpleObservableCollection<Pedido>(
                Pedido.ObtenerVentas(GlobalUI.ClienteActual, GlobalUI.ClienteActual.Zona, estadoPedido, TipoPedido));
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

        private void AnularDocumento()
        {
            Pedido[] docsPorAnular = Pedidos.Where(x => x.Seleccionado).ToArray<Pedido>();

            if (docsPorAnular.Length > 0) // anula si hay documentos por anular
            {
                string alert = (this.TipoPedido == TipoPedido.Factura ? "las facturas seleccionadas" : "los pedidos seleccionados");

                this.mostrarMensaje(Mensaje.Accion.Anular, alert, result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        // se anulan los documentos del arreglo, comenzando por el pedidoPorAnular[0]
                        AnularDocumento(docsPorAnular, 0);
                    }
                    Refrescar();
                });
            }
        }

        /// <summary>
        /// anula el pedidoPorAnular[index]
        /// Se hace recursivamente para así lograr que la secuencia de mensajes modales se muestre en el orden correcto.
        /// </summary>
        /// <param name="pedidosPorAnular"></param>
        /// <param name="index"></param>
        void AnularDocumento(Pedido[] docsPorAnular, int index)
        {
            if (index >= docsPorAnular.Length) return;

            Pedido pedido = docsPorAnular[index];

            if (pedido.Tipo == TipoPedido.Factura && pedido.Detalles.Lista.Count == 0)
            {
                try
                {
                    pedido.ObtenerDetalles();
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando detalles de la factura. " + ex.Message);
                    return;
                }
            }

            if (pedido.EsConsignacion)
            {
                try
                {
                    GestorDatos.BeginTransaction();
                    BoletasConsignacion boletas = new BoletasConsignacion();
                    boletas.Gestionados.Add(VentaConsignacion.ObtenerVentaConsignacion(GlobalUI.ClienteActual, GlobalUI.RutaActual, pedido.Compania));
                    boletas.Facturas.Gestionados.Add(pedido);
                    boletas.ActualizarBoletaPorFactura(true);
                    pedido.Anular(false, GlobalUI.RutaActual.Codigo, this);
                    GestorDatos.CommitTransaction();
                }
                catch (Exception ex)
                {
                    GestorDatos.RollbackTransaction();

                    this.mostrarAlerta("Error anulando la factura '" + pedido.Numero + "' generada por desglose de boleta de venta en consignación. " + ex.Message);
                    return;
                }
            }
            else
            {
                try
                {
                    string numeroRecibo = pedido.ReciboAsociado(pedido.Numero);

                    if ((numeroRecibo != string.Empty) && (pedido.Tipo == TipoPedido.Factura) && (pedido.Configuracion.CondicionPago.DiasNeto == 0))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Decision, " anular los documentos asociados a la factura '" + pedido.Numero + "'",
                            result2 =>
                            {
                                if (result2 == DialogResult.Yes)
                                {
                                    pedido.Anular(true, GlobalUI.RutaActual.Codigo, this);
                                    pedido.AnularReciboAsociado(numeroRecibo, true, GlobalUI.RutaActual.Codigo, this);
                                    pedido.ReversarNotasCredito(numeroRecibo);
                                }
                                else
                                {
                                    pedido.Anular(true, GlobalUI.RutaActual.Codigo, this);
                                    pedido.AnularReciboAsociado(numeroRecibo, false, GlobalUI.RutaActual.Codigo, this);
                                    pedido.ReversarNotasCredito(numeroRecibo);
                                    pedido.CambiarTipoRecibo(numeroRecibo);
                                }
                            });
                    }
                    else
                    {
                        pedido.Anular(true, GlobalUI.RutaActual.Codigo, this);
                        pedido.ReversarNotasCredito(numeroRecibo);
                    }
                }
                catch (Exception ex)
                {
                    if (pedido.Tipo == TipoPedido.Factura)
                        this.mostrarAlerta("Error anulando factura. " + ex.Message);
                    else
                        this.mostrarAlerta("Error anulando pedido. " + ex.Message);
                }
            }

            AnularDocumento(docsPorAnular, index + 1);
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
                    foreach (Pedido pedido in this.ItemsSeleccionados)
                    {
                        if (!pedido.Impreso)
                        {
                            pedido.LeyendaOriginal = true;
                        }
                    }
            }

            
            try
            {
                int cantidad = 0;
                cantidad = cantidadCopias;

                if (cantidad >= 0)
                {
                    Pedidos pedidos = new Pedidos(this.ItemsSeleccionados, GlobalUI.ClienteActual);
                    if (this.TipoPedido == TipoPedido.Factura)
                        pedidos.ImprimeDetalleFactura(cantidad, (DetalleSort.Ordenador)ordernarPor);
                    else
                        pedidos.ImprimeDetallePedido(cantidad, (DetalleSort.Ordenador)ordernarPor);
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

            if (this.Pedidos.Count != 0)
            {
                string codCia = string.Empty;

                foreach (Pedido item in this.Pedidos)
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
                            if (this.TipoPedido == TipoPedido.Factura)
                                this.mostrarAlerta("Error cargando información de la factura '" + numPed + "'. " + ex.Message);
                            else
                                this.mostrarAlerta("Error cargando información del pedido '" + numPed + "'. " + ex.Message);
                        }
                    }
                }

                if (this.ItemsSeleccionados.Count != 0)
                {
                    //Caso 25452 LDS 19/10/2007
                    this.VerificarImpresion();
                }
                else
                {

                    if (this.TipoPedido == TipoPedido.Factura)
                    {
                        this.mostrarMensaje(Mensaje.Accion.Imprimir, "el resumen de facturas", resp =>
                        {
                            if (resp.Equals(DialogResult.Yes))
                            {
                                List<Pedido> encabezados=new List<Pedido>();
                                foreach (Pedido ped in this.Pedidos)
                                {
                                    encabezados.Add(ped);
                                }
                                Pedidos pedidos = new Pedidos(encabezados, GlobalUI.ClienteActual);
                                pedidos.ImprimeResumen(this.TipoPedido,this);
                            }
                        });
                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Imprimir, "el resumen de pedidos", resp =>
                            {
                                if (resp.Equals(DialogResult.Yes))
                                {
                                    List<Pedido> encabezados = new List<Pedido>();
                                    foreach (Pedido ped in this.Pedidos)
                                    {
                                        encabezados.Add(ped);
                                    }
                                    Pedidos pedidos = new Pedidos(encabezados, GlobalUI.ClienteActual);
                                    pedidos.ImprimeResumen(this.TipoPedido,this);
                                }
                            });
                    }
                }
            }
        }

        //Caso 25452 LDS 19/10/2007
        /// <summary>
        /// Valida si el usuario desea imprimir el detalle de los pedidos gestionados.
        /// </summary>
        private void VerificarImpresion()
        {
            if (this.TipoPedido == TipoPedido.Factura)
            {
                this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las facturas seleccionadas", res =>
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
                            this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Detalle de Facturas", mostrarCriterioOrden = true });
                        }
                    });
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de los pedidos seleccionados", res =>
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
                            this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Detalle de Pedidos", mostrarCriterioOrden = true });
                        }
                    });
            }            
        }

        #endregion
    }
}