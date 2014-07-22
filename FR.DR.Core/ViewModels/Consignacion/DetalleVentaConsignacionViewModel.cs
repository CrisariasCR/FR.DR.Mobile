using System;
//using System.Net;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls; // FrmConfig
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.UI; //GlobalUI
using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleVentaConsignacionViewModel : DialogViewModel<DialogResult>
    {

        #region Propiedades

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set
            {
                if (value != nombreCliente)
                {
                    nombreCliente = value;
                    RaisePropertyChanged("NombreCliente");
                }
            }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        public bool EsBarras=false;       
        

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

        private bool consultarVisible;
        public bool ConsultarVisible
        {
            get { return consultarVisible; }
            set { consultarVisible = value; RaisePropertyChanged("ConsultarVisible"); }
        }
        private bool eliminarVisible;
        public bool EliminarVisible
        {
            get { return eliminarVisible; }
            set { eliminarVisible = value; RaisePropertyChanged("EliminarVisible"); }
        }
        private bool continuarVisible;
        public bool ContinuarVisible
        {
            get { return continuarVisible; }
            set { continuarVisible = value; RaisePropertyChanged("ContinuarVisible"); }
        }
        public IObservableCollection<string> Companias { get; set; }
        #endregion Companías y CompaniaActual

        #region Criterios y CriterioActual

        private CriterioArticulo criterioActual;
        public CriterioArticulo CriterioActual
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

        /// <summary>
        /// datasource de los criterios de filtro, deben conincidir con los del enumerado CriterioArticulo
        /// </summary>
        public IObservableCollection<CriterioArticulo> Criterios { get; set; }

        #endregion Criterios y CriterioActual   


        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set
            {
                if (value != textoBusqueda)
                {
                    textoBusqueda = value;
                    RaisePropertyChanged("TextoBusqueda");
                    CambioDeTextoEnBusqueda();
                }
            }
        }

        private IObservableCollection<DetalleVenta> items;
        public IObservableCollection<DetalleVenta> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }

        private DetalleVenta itemActual;
        public DetalleVenta ItemActual
        {
            get { return itemActual; }
            set { itemActual = value;RaisePropertyChanged("ItemActual"); }
        }      
     
        

        /// <summary>
        /// Ventas en consignación gestionadas durante la toma de la venta en consignación.
        /// </summary>
        private VentasConsignacion ventasConsignacion = new VentasConsignacion();
        /// <summary>
        ///  Lista de detales que se estan mostrando.
        /// </summary>
        private DetallesVenta detallesConsignacion = new DetallesVenta();
        /// <summary>
        /// Indica el criterio de búsqueda.
        /// </summary>
        //private CriterioArticulo criterio = CriterioArticulo.Ninguno;
        /// <summary>
        /// Indica que se han sugerido ventas en consignación luego de haber realizado el desglose de boletas
        /// de venta en consignación del cliente.
        /// </summary>
        private bool ventasSugeridas = false;
        /// <summary>
        /// Indica que el formulario ha sido llamado desde la ventana de consulta de ventas en consignación.
        /// </summary>
        private bool consultaVentaConsignacion;
        /// <summary>
        /// Indica el formulario del cual es llamado este formulario.
        /// </summary>
        private Formulario seInvocoDe;
        public Formulario SeInvocoDe
        {
            get { return seInvocoDe; }

        }

        
        /// <summary>
        /// Indica si se debe permitir modificar la venta en consignación consultada.
        /// </summary>
        private bool permitirModificarVentaConsignacion;
           

        // hay que hacer binding de estas propiedades

       
        #endregion Propiedades

        public DetalleVentaConsignacionViewModel(string messageId,string invocaDesde, string ventasSugeridas, string permitirModificarVentaConsignacion)
            : base(messageId) 
        {
            
            this.ventasConsignacion = Gestor.Consignaciones;
            this.consultaVentaConsignacion = false;
            this.seInvocoDe = GlobalUI.RetornaFormulario(invocaDesde);
            this.ventasSugeridas = ventasSugeridas.Equals("S");
            this.permitirModificarVentaConsignacion = permitirModificarVentaConsignacion.Equals("S");

            CargaInicial();
        }

        #region Métodos de instancia

        #region Acciones

        /// <summary>
        /// invocar en TextoBusqueda_TextChanged
        /// </summary>
        public void BusquedaCodigoBarras(string texto)
        {
            // si termina con caracter de retorno (enter)
            if (texto.EndsWith(FRmConfig.CaracterDeRetorno))
            {
                texto = texto.Substring(0, texto.Length - 1);
                TextoBusqueda = texto;

                //cambiamos el criterio a codigo de barras
                CriterioActual = CriterioArticulo.Barras;

                // se refresca la consulta
                this.EjecutarBusquedaDetalles();
            }
        }

        /// <summary>
        /// Carga la ventana.
        /// </summary>
        private void CargaInicial()
        {

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;
            TextoBusqueda = string.Empty;
            Companias = new SimpleObservableCollection<string>(Util.CargarCiasConsignacion( Gestor.Consignaciones.Gestionados, EstadoConsignacion.NoDefinido));
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      //CriterioArticulo.Familia,
                      CriterioArticulo.Clase
                    };
            CriterioActual = CriterioArticulo.Descripcion;
            //this.cboCriterio.SelectedIndex = 3;

            if (!this.consultaVentaConsignacion && this.seInvocoDe != Formulario.AnulacionConsignacion)
            {
                if (this.permitirModificarVentaConsignacion)
                {
                    //if(this.seInvocoDe == Formulario.TomaVentaConsignacion || this.seInvocoDe == Formulario.FinalizarConsignacion || this.seInvocoDe == Formulario.SugerirBoleta)
                    //this.ibtRegresar.Visible = false;
                    //else
                    //this.ibtRegresar.Visible = true;

                    this.ConsultarVisible = true;
                    this.EliminarVisible = true;
                    this.ContinuarVisible = true;
                }
                else
                {
                    //this.ibtRegresar.Visible = true;
                    this.ConsultarVisible = true;
                    this.EliminarVisible = false;
                    this.ContinuarVisible = false;

                    //this.ibtRegresar.Location = this.ibtEliminarDetalle.Location;
                    //this.ibtConsultaArticulo.Location = this.ibtContinuar.Location;
                }
            }
            if (this.seInvocoDe == Formulario.AnulacionConsignacion)
            {
                //this.ibtRegresar.Visible = true;
                this.ConsultarVisible = true;
                this.EliminarVisible = false;
                this.ContinuarVisible = false;

                //this.ibtRegresar.Location = this.ibtEliminarDetalle.Location;
                //this.ibtConsultaArticulo.Location = this.ibtContinuar.Location;
            }

        }

        /// <summary>
        /// Obtiene los detalles de la venta en consignación de una compañía filtraddo por el criterio seleccionado.
        /// </summary>
        private void EjecutarCriterio()
        {           
            //this.CriterioActual = Util.CambiarCriterio(this.cboCriterio.SelectedIndex);
            //Util.AcomodaListViewConsignacion(ref this.lstVentaConsignacion, this.criterio);

            if (this.CriterioActual != CriterioArticulo.Ninguno && !string.IsNullOrEmpty(this.CompaniaActual))
                this.EjecutarBusquedaDetalles();
        }

        /// <summary>
        /// Obtiene los detalles de la venta en consignación de una compañía.
        /// </summary>
        /// <param name="retirarDetalle">
        /// Indica que se acaba de retirar un detalle.
        /// </param>
        public void EjecutarBusquedaDetalles()
        {
            if (this.criterioActual == CriterioArticulo.Ninguno)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un criterio de búsqueda");
                return;
            }

            if (string.IsNullOrEmpty(this.CompaniaActual))
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }


            VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(this.CompaniaActual);

            if (ventaConsignacion != null)
                this.detallesConsignacion = ventaConsignacion.Detalles.Buscar(this.CriterioActual, this.TextoBusqueda, false);
            else
                this.detallesConsignacion = new DetallesVenta();

            if (this.detallesConsignacion.Vacio())
                this.mostrarMensaje(Mensaje.Accion.BusquedaMala);

            //Refrescamos el listView
            this.CargarListView();
        }

        /// <summary>
        /// Carga el listview con una lista de detalles determinada.
        /// </summary>
        private void CargarListView()
        {
            //Caso: 32842 ABC 18/06/2008 Total de Articulos para mostrar en el reporte
            decimal totalArticulos = 0;
            List<DetalleVenta> lstVentaConsignacion = new List<DetalleVenta>();
            lstVentaConsignacion.Clear();

            foreach (DetalleVenta detalle in this.detallesConsignacion.Lista)
            {                
                lstVentaConsignacion.Add(detalle);
                //Caso: 32842 ABC 18/06/2008 Se suma las unidades de detalle de articulo.
                totalArticulos += detalle.UnidadesAlmacen;
            }
            //Caso: 32842 ABC 18/06/2008 Se asigna los valores a los nuevos campos incluidos  
            this.TotalLineas = lstVentaConsignacion.Count;
            this.TotalArticulos = totalArticulos;
            Items = new SimpleObservableCollection<DetalleVenta>(lstVentaConsignacion);
        }

        /// <summary>
        /// Realiza la búsqueda del artículo cuando se utiliza la selección por código de barras.
        /// </summary>
        public void CambioDeTextoEnBusqueda()
        {
            string textoConsulta = this.TextoBusqueda;

            if (!string.IsNullOrEmpty(textoConsulta) && CriterioActual == CriterioArticulo.Barras)
            {
                textoConsulta = textoConsulta.Substring(0, textoConsulta.Length - 1);
                this.TextoBusqueda = textoConsulta;
                this.EjecutarBusquedaDetalles();
                if (this.Items.Count > 0)
                {
                    EsBarras = true;
                }
            }
        }
        /// <summary>
        /// Realiza una búsqueda por aproximación según el criterio de búsqueda seleccionado para la venta en consignación
        /// de la compañía seleccionada.
        /// </summary>
        public void RealizarBusqueda()
        {
            try
            {
                this.EjecutarBusquedaDetalles();
            }
            catch (Exception exc)
            {
                this.mostrarAlerta("Error :" + exc.Message);
            }

            this.TextoBusqueda = string.Empty;
        }
        /// <summary>
        /// Método que se encarga de llamar al form de propiedades de artículos para
        /// ver las propiedades del artículo seleccionado.
        /// </summary>
        private void MostrarDetalleArticulo()
        {
            if (this.ItemActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un artículo");
            }
            else
            {

                DetalleVenta detalle = this.ItemActual;

                //Obtenemos el encabezado de la devolucion a la que pertenece el detalle
                
                VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(detalle.Articulo.Compania);

                if (ventaConsignacion == null)
                {
                    this.mostrarAlerta("No se encontró el encabezado de la venta en consignacíón.");
                    return;
                }
                Dictionary<string, object> p = new Dictionary<string, object>();
                ConsultaArticuloViewModel.Articulo = this.ItemActual.Articulo;
                this.RequestNavigate<ConsultaArticuloViewModel>();
                //frmConsultaArticulo consulta = new frmConsultaArticulo(detalle.Articulo);
                //consulta.ShowDialog();
                //consulta.Close();
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

            }
        }
        /// <summary>
        /// Método que de retirar un detalle seleccionado del listview.
        /// </summary>
        private void GestionRetiroDetalle()
        {
            if (string.IsNullOrEmpty(CompaniaActual))
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }

            if (this.ItemActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un artículo");
                return;
            }

            DetalleVenta detalle = ItemActual;

            //
            this.mostrarMensaje(Mensaje.Accion.Retirar,( "el artículo: " + detalle.Articulo.Codigo),
                   result =>
                   {
                       if (result == DialogResult.Yes || result==DialogResult.OK)
                       {
                           if (detalle.UnidadesAlmacenSaldo > decimal.Zero || detalle.UnidadesDetalleSaldo > decimal.Zero)
                           {
                               this.mostrarAlerta("El detalle no se puede eliminar ya que posee saldo luego del desglose.");
                               return;
                           }

                           try
                           {
                               //Gestionamos el inventario con cantidades 0
                               this.ventasConsignacion.EliminarDetalle(detalle.Articulo);

                               //Lo quitamos de la lista de detalles y del listview
                               //this.detallesConsignacion.Lista.RemoveAt(ind);                               
                               //this.lstVentaConsignacion.Items.RemoveAt(ind);
                               Items.Remove(detalle);
                               RaisePropertyChanged("Items");
                               //Caso:32160 ABC 14/05/2008 Mostrar cantidad total de unidades pedidas, vendidas y en consignación
                               ActualizarTotales(detalle);
                           }
                           catch (Exception ex)
                           {
                               this.mostrarAlerta("Error eliminando la línea. " + ex.Message);
                           }
                       }
                   });
            //

        }
        /// <summary>
        /// Regresa a la ventana desde la cual se realizó el llamado a la ventana de consulta de detalles.
        /// </summary>
        public void Regresar()
        {
            if (this.seInvocoDe == Formulario.SeleccionarCliente || this.seInvocoDe == Formulario.ConsultaConsignacion)
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("seInvocoDe", this.SeInvocoDe.ToString());
                this.DoClose();
                RequestNavigate<ConsultaConsignacionViewModel>(parametros);
                //frmConsultaConsignacion consultaConsignacion = new frmConsultaConsignacion(this.seInvocoDe);
                //consultaConsignacion.Show();                
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

            }
            else
            {
                if (this.seInvocoDe == Formulario.AnulacionConsignacion || this.SeInvocoDe==Formulario.FinalizarConsignacion)
                {
                    //Indicamos que la acción es continuar.                    
                    this.ReturnResult(DialogResult.OK);
                    //this.DialogResult = DialogResult.OK;                    
                }
                else 
                {
                    //Caso 32006 LDS 20080407
                    //Se limpian las ventas en consignación gestionadas cuando se realiza la modificación de la venta en consignación seleccionada.
                    if (this.seInvocoDe == Formulario.TomaVentaConsignacion)
                    {
                        ReturnResult(DialogResult.Yes);
                    }
                    else
                    {                        
                        Gestor.Consignaciones = new VentasConsignacion();
                        this.DoClose();
                    }
                }
            }            
        }
        /// <summary>
        /// Cierra la ventana para luego continuar con la gestión de la venta en consignación.
        /// </summary>
        private void Continuar()
        {
            if (this.seInvocoDe == Formulario.SugerirBoleta || this.seInvocoDe == Formulario.SeleccionarCliente || this.seInvocoDe == Formulario.ConsultaConsignacion)
            {
                //Verificamos si hay ventas en consignación sin líneas.
                foreach (VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
                    if (ventaConsignacion.Detalles.Vacio())
                        Gestor.Consignaciones.Borrar(ventaConsignacion.Compania);

                //Recalculamos los montos totales de la venta en consignación.
                Gestor.Consignaciones.SacarMontosTotales();
                Dictionary<string,string> p=new Dictionary<string,string>();
                p.Add("invocaDesde",Formulario.ConsultaConsignacion.ToString());
                if(this.ventasSugeridas)
                {
                    p.Add("ventasSugeridas","S");
                }
                else
                {
                    p.Add("ventasSugeridas","N");
                }
                //Nos vamos a la toma de la venta en consignación
                //frmTomaVentaConsig tomaVentaConsignacion= new frmTomaVentaConsig(this.seInvocoDe,this.ventasSugeridas);
                //tomaVentaConsignacion.Show();
                this.DoClose();
                RequestNavigate<TomaVentaConsigViewModel>(p);
                
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            }
            if (this.seInvocoDe == Formulario.TomaVentaConsignacion || this.seInvocoDe == Formulario.FinalizarConsignacion)
            {
                //Indicamos que la acción es continuar.                
                ReturnResult(DialogResult.OK);
            }
        }

        public void ActualizarTotales(DetalleVenta articuloRetirado)
        {
            //Caso:32682 ABC 23/05/2008 Mostrar Total de Articulo y Total de Lineas
            decimal totalArticulos = Convert.ToDecimal(this.TotalArticulos);
            totalArticulos = totalArticulos - articuloRetirado.UnidadesAlmacen;
            this.TotalArticulos = totalArticulos;
            this.TotalLineas = this.Items.Count;
        }
        #endregion

        #endregion

        #region Comandos

        public ICommand ComandoConsultaArticulo
        {
            get { return new MvxRelayCommand(MostrarDetalleArticulo); }
        }

        public ICommand ComandoRetirarDetalle
        {
            get { return new MvxRelayCommand(GestionRetiroDetalle); }
        }


        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(RealizarBusqueda); }
        }

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        #endregion Comandos        

    }
}
