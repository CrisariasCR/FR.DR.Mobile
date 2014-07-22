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
    public class DetallePedidoViewModel : ListaArticulosViewModel
    {
#pragma warning disable 169

        public DetallePedidoViewModel(string invocadesde,string mostrarControles, string esFactura)
            : base()
        {
            
            Encabezados = new Pedidos();
            DesdeInvocacion = invocadesde;
            if (invocadesde == Formulario.TomaPedido.ToString())
            {                
                this.Encabezados = Gestor.Pedidos;
                MostrarControlesEditar = false;
            }
            else 
            {
                MostrarControlesEditar = mostrarControles.Equals("S");
                if (invocadesde == Formulario.AplicarPedido.ToString())
                {
                    this.Encabezados = Gestor.Pedidos;
                }
                else
                {
                    if (invocadesde == Formulario.FacturaConsignacion.ToString())
                    {
                        Encabezados = Gestor.DesglosesConsignacion.Facturas;                        
                    }
                    Pedido = ConsultaPedidosViewModel.PedidoSeleccionado;
                    Encabezados.Gestionados.Add(Pedido);
                    Encabezados.ConfigDocumentoCia.Add(Pedido.Compania.ToUpper(), Pedido.Configuracion);
                }
            }            
            Companias = new SimpleObservableCollection<string>(Encabezados.Gestionados.Select(item => item.Compania).ToList());
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }

            MostrarControles = mostrarControles.Equals("S");
            EsFactura = esFactura.Equals("S");
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

        public Pedidos Encabezados { get; set; }

        private DetallesPedido detalles;
        public DetallesPedido Detalles
        {
            get { return detalles; }
            set { detalles = value; RaisePropertyChanged("Detalles"); }
        }

        public DetallePedido DetalleSeleccionado { get; set; }

        private Pedido pedido;
        public Pedido Pedido {get;set;
            //get { return ConsultaPedidosViewModel.PedidoSeleccionado; }
        }

        private decimal totalArticulos;
        public decimal TotalArticulos
        {
            get { return totalArticulos; }
            set { totalArticulos = value; RaisePropertyChanged("TotalArticulos"); }
        }

        public static NivelPrecio NivelPrecio { get; set; }

        public bool MostrarControles { get; set; }
        public bool MostrarControlesEditar { get; set; }

        public bool EsFactura { get; set; }

        #endregion

        #region Comandos

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarArticulo); }
        }

        public ICommand ComandoEliminar
        {
            get { return new MvxRelayCommand(EliminarDetalle); }
        }

        public ICommand ComandoEditar
        {
            get { return new MvxRelayCommand(EditarPedido); }
        }

        #endregion

        #region Acciones

        public override void Refrescar()
        {
            Pedido pedido = Encabezados.Buscar(CompaniaActual);

            if (pedido != null)
                this.Detalles = pedido.Detalles.Buscar(CriterioActual, TextoBusqueda, false, true);
            else
                this.Detalles = new DetallesPedido();

            TotalArticulos = Detalles.Lista.Sum(p => p.UnidadesAlmacen);
        }

        public void Regresar()
        {
            //Indicamos que la accion es continuar

            this.Encabezados.LimpiarValores();

            if (DesdeInvocacion == Formulario.TomaPedido.ToString() || DesdeInvocacion == Formulario.AplicarPedido.ToString())
            {
                this.DoClose();
            }
            else            
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, "guardar el pedido pues ha sido modificado", res =>
                    {
                        if (res == DialogResult.Yes)
                        {
                            try
                            {
                                this.Encabezados.ActualizarPedidos();
                                this.DoClose();
                            }
                            catch (Exception ex)
                            {
                                this.mostrarAlerta("Error actualizando pedidos. " + ex.Message);
                            }
                        }
                        else 
                        {
                            this.DoClose();
                        }
                        
                    });
            }
            
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

        public void Guardar()
        {
            try
            {
                this.Encabezados.ActualizarPedidos();
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error actualizando pedidos. " + ex.Message);
            }
        }

        private void EliminarDetalle()
        {
            if (DetalleSeleccionado != null)
            {
                if (DetalleSeleccionado.EsBonificada)
                {
                    this.mostrarMensaje(Mensaje.Accion.Informacion, "La línea seleccionada es bonificada. No se puede eliminar.");
                    return;
                }

                if (DetalleSeleccionado.EsBonificadaAdicional)
                {
                    this.mostrarMensaje(Mensaje.Accion.Informacion, "La línea seleccionada es bonificada adicional. No se puede eliminar.");
                    return;
                }

                this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo: " + DetalleSeleccionado.Articulo.Codigo, result =>
                    {
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                if (DetalleSeleccionado.LineaBonificada != null ||
                                    DetalleSeleccionado.LineaBonificadaAdicional != null)
                                {
                                    this.Detalles.Lista.Remove(DetalleSeleccionado);
                                }
                                if (DetalleSeleccionado.EsBonificadaAdicional)
                                    Encabezados.EliminarDetalle(DetalleSeleccionado.Articulo);
                                else
                                    Encabezados.EliminarDetalle(DetalleSeleccionado.Articulo);

                                this.Detalles.Lista.Remove(DetalleSeleccionado);
                                this.eliminoLineas = true;
                                Refrescar();

                            }
                            catch (Exception e)
                            {
                                this.mostrarAlerta("Error eliminando la línea. " + e.Message);
                            }
                        }
                    });

                
            }
        }

        private void EditarPedido()
        {
            Gestor.Pedidos = new Pedidos();
            Gestor.Pedidos.Gestionados.Add(Pedido);
            Gestor.Pedidos.ConfigDocumentoCia.Add(Pedido.Compania.ToUpper(), Pedido.Configuracion);
            TomaPedidoViewModel.NivelPrecio = Pedido.Configuracion.Nivel;
            Dictionary<string, object> Parametros = new Dictionary<string, object>() { { "pedidoActual", true } };
            this.RequestNavigate<TomaPedidoViewModel>(Parametros);
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