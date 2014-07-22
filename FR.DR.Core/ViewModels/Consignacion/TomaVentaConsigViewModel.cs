using System;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaVentaConsigViewModel : BaseViewModel
    {
        /// <summary>
        /// Variable utilizada para evitar doble procesamiento en calculo de unidades.
        /// </summary>
        private bool inicial = true;
        private bool cargando = false;
        public bool EsBarras = false;
        public static bool ventanaInactiva = false;   
        public bool Cargando { get { return cargando; } }

        #region Propiedades

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Hola" }; } }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #region ListaInventarios       

       
        #endregion ListaInventarios



        #endregion Propiedades

        public TomaVentaConsigViewModel(string invocaDesde, string ventasSugeridas) 
        {
            ventanaInactiva = false;
            this.seInvocoDe = GlobalUI.RetornaFormulario(invocaDesde);
            this.ventasSugeridas = ventasSugeridas.Equals("S");
            this.detalle = new DetalleVenta();
            this.CargarVentana();
            //LDA CR1-05072-RGHK
            //Se descuadraban las existencias del Rutero
            ActualizarExistencias();
        }

       

        /// <summary>
        /// Funcion que hace la conversion de unidades de detalle a unidades de almacen tomando
        /// en cuenta las unidades de empaque.
        /// </summary>
        public void CalculaUnidades()
        {
            // hay seleccionados
            if (ItemActual != null)
            {
                if (inicial)
                {
                    inicial = false;
                    decimal cantDetalle;
                    decimal cantAlmacen;

                    GestorUtilitario.CalculaUnidades(this.UnidadAlmacen,
                                                    this.UnidadDetalle,
                                                    Convert.ToInt32(this.ItemActual.UnidadEmpaque),
                                                    out cantAlmacen,
                                                    out cantDetalle);

                    this.UnidadDetalle = cantDetalle;
                    this.UnidadAlmacen = cantAlmacen;
                    inicial = true;
                }
            }
        }
       

        /// <summary>
        /// Metodo se encarga de llamar a la funcion que busca en la base de datos los datos
        /// del articulo segun el codigo ingresado. 
        /// </summary>
        public void BuscarArticulo()
        {
            if (this.ItemActual== null)
            {
                //No se ha seleccionado un artículo
                return;
            }

            if (this.CompaniaActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }

            //Obtenemos el articulo de la lista de articulos
            //this.elArticulo = this.ListaArticulos[this.lstArticulos.SelectedIndices[0]];

            
            DetalleInventario detalle = Gestor.Inventario.BuscarDetalleEnInventario(this.ItemActual);

            if (detalle == null)
                detalle = new DetalleInventario();

            this.lbUnidadAlmacen = this.ItemActual.TipoEmpaqueAlmacen + ":";
            this.lbUnidadDetalle = this.ItemActual.TipoEmpaqueDetalle + ":";
            this.UnidadDetalle = detalle.UnidadesDetalle;
            this.UnidadAlmacen = detalle.UnidadesAlmacen;
        }

        /// <summary>
        /// Metodo utilizando cuando el filtro de búsqueda es modificado.
        /// </summary>
        public void LecturaCodigoBarra(string textoConsulta)
        {
            if (!string.IsNullOrEmpty(textoConsulta)&&CriterioActual == CriterioArticulo.Barras)
            {
                this.cargando = true;
                //Cambiamos el criterio de busqueda a Codigo de Barras
                this.CriterioActual= CriterioArticulo.Barras;
                //if ((textoConsulta.EndsWith(FRmConfig.CaracterDeRetorno)))
                //    textoConsulta = textoConsulta.Substring(0, textoConsulta.Length - 1);
                this.TextoBusqueda = textoConsulta;

                this.RealizarBusqueda();

                // Items.Seleccionados > 0
                if (this.ItemActual != null)
                {
                    //this.ItemActual.Items[0].Focused = true;
                    //this.ItemActual.Items[0].Selected = true;
                    //Caso 35220 LJR 27/03/2009, Estandarizar codigos de barras, incremento de cantidades automatico

                    //Debemos incrementar la cantidad ingresada e inmediatamente agregarlo al inventario
                    //Se incrementa la cantidad en detalle pues el codigo de barras cargado a Frm es el 
                    //de ventas = unidades de detalle.
                    this.UnidadDetalle++;

                    this.CalculaUnidades();                    
                    this.AgregarDetalle();
                    EsBarras = true;
                }                
                this.cargando = false;
            }
        }

        #region Comandos

        public ICommand ComandoConsultarDetalle
        {
            get { return new MvxRelayCommand(MostrarDetalles); }
        }

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(AgregarDetalle); }
        }

        public ICommand ComandoRetirarDetalle
        {
            get { return new MvxRelayCommand(RetirarDetalle); }
        }

        public ICommand ComandoCancelarToma
        {
            get { return new MvxRelayCommand(CancelarToma); }
        }
        
        public ICommand ComandoAceptarToma
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(RealizarBusqueda); }
        }

        #endregion Comandos

        #region Acciones

        ///<summary>
        /// Metodo que llama al form que despliega el detalle de los articulo ya
        /// incluidos en el inventario
        /// </summary>
        //private void ConsultarDetalles()
        //{
        //    if (Gestor.Inventario.ExistenArticulosGestionados())
        //    {
        //        Dictionary<string, string> Parametros = new Dictionary<string, string>() { { "inventarios", "S" } };
        //        this.RequestNavigate<DetalleInventarioViewModel>(Parametros); 
        //    }
        //    else
        //        this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay detalles incluídos");
        //}

        ///// <summary>
        ///// Metodo que se encarga  llamar a la funcion que realiza la gestion]
        ///// de inventario y detalles.
        ///// </summary>
        //private void AgregarDetalle()
        //{
        //    // Items.Seleccionados > 0
        //    if (SelectedItems.Count == 0)
        //    {
        //        this.mostrarAlerta("Debe seleccionar un articulo para inventario.");
        //        return;
        //    }

        //    if (this.UnidadAlmacen == 0 && this.UnidadDetalle == 0)
        //    {
        //        this.mostrarAlerta("Ambas cantidades no pueden ser cero.");
        //        return;
        //    }

        //    //Se procede a agregar el detalle del Inventario.
        //    try
        //    {
        //        Gestor.Inventario.Gestionar(this.ItemActual,
        //            GlobalUI.ClienteActual.Codigo, GlobalUI.RutaActual.Codigo,
        //            UnidadDetalle, UnidadAlmacen);

        //    }
        //    catch (Exception ex)
        //    {
        //        this.mostrarAlerta("No se pudo agregar el detalle del inventario. " + ex.Message);
        //    }
        //}

        /////<summary>
        ///// Metodo que gestiona el retiro de una linea de detalle
        ///// incluida en el inventario
        ///// </summary>
        //private void RetirarDetalle()
        //{
        //    if (this.SelectedItems.Count == 0)
        //    {
        //        //No se ha seleccionado un artículo
        //        return;
        //    }

        //    if (this.CompaniaActual == null)
        //    {
        //        this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
        //        return;
        //    }
        //    //Obtenemos el articulo de la lista de articulos
        //    //this.elArticulo = this.ListaArticulos[this.lstArticulos.SelectedIndices[0]]; ;
        //    DetalleInventario detalle = Gestor.Inventario.BuscarDetalleEnInventario(this.ItemActual);

        //    if (detalle == null)
        //    {
        //        this.mostrarAlerta("El artículo no ha sido ingresado al inventario, por esto no puede ser eliminado.");
        //        return;
        //    }

        //    this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo", result =>
        //        {

        //            if (result == DialogResult.Yes)
        //            {
        //                try
        //                {
        //                    Gestor.Inventario.Gestionar(this.ItemActual, GlobalUI.ClienteActual.Codigo, GlobalUI.RutaActual.Codigo, 0, 0);

        //                    this.UnidadAlmacen = 0;
        //                    this.UnidadDetalle = 0;

        //                    //dialog.Dismiss();
        //                }
        //                catch (Exception ex)
        //                {
        //                    this.mostrarAlerta("Error al gestionar el inventario. " + ex.Message);
        //                }
        //            }
        //        });
        //}

        /////<summary>
        ///// Metodo gestiona  la cancelacion de la toma del inventario.
        ///// </summary>
        //private void CancelarToma()
        //{
        //    this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma del inventario", result =>
        //        {
        //            if (result == DialogResult.Yes)
        //            {
        //                Gestor.Inventario = new Inventarios();
        //                this.CloseCommand.Execute(null);
        //            }
        //        });
        //}

        ///// <summary>
        ///// Metodo que gestiona el inventario.
        ///// </summary>
        //private void GestionarInventario()
        //{
        //    this.mostrarMensaje(Mensaje.Accion.Terminar, "la toma del inventario", result =>
        //        {

        //            if (result == DialogResult.Yes)
        //            {
        //                try
        //                {
        //                    if (FRmConfig.EnConsulta)
        //                        Gestor.Inventario.Actualizar();
        //                    else
        //                        Gestor.Inventario.Guardar();
        //                }
        //                catch (Exception ex)
        //                {
        //                    this.mostrarAlerta("Error " + (FRmConfig.EnConsulta ? "actualizando" : "guardando") + " inventario. " + ex.Message);
        //                }
        //                //Limpiamos la lista de inventarios gestionados
        //                Gestor.Inventario = new Inventarios();

        //                this.CloseCommand.Execute(null);
        //            }
        //        });
        //}

        

        #endregion Accioness

        #region mobile

        #region Variables de instancia
		
        private List<Articulo> ListaArticulos=new List<Articulo>();

        private Articulo itemActual;
        public Articulo ItemActual
        {
            get { return itemActual; }
            set { itemActual = value; RaisePropertyChanged("ItemActual");
            MostrarDatosArticulo();
            }
        }

        private IObservableCollection<Articulo> items;
        public IObservableCollection<Articulo> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }
        
        public bool carga = false;

		/// <summary>
		/// Indica el formulario del cual es llamado este formulario.
		/// </summary>
		private Formulario seInvocoDe;
        public Formulario SeInvocoDe
        {
            get { return seInvocoDe; }
          
        } 
        private string ltbTotal;
        public string LtbTotal
        {
            get { return ltbTotal; }
            set
            {
                if (value != ltbTotal)
                {
                    ltbTotal = value;
                    RaisePropertyChanged("LtbTotal");
                }
            }
        }
		/// <summary>
		/// Indica que se han sugerido ventas en consignación luego de haber realizado el desglose de boletas
		/// de venta en consignación del cliente.
		/// </summary>
		private bool ventasSugeridas;
		
        private DetalleVenta detalle;


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
                    CambioCriterio();
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
                if(!ventanaInactiva)
                {
                    if (value != textoBusqueda)
                    {
                        textoBusqueda = value;
                        RaisePropertyChanged("TextoBusqueda");
                        LecturaCodigoBarra(textoBusqueda);
                    }
                }
            }
        }

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

        private string lbunidadAlmacen;
        public string lbUnidadAlmacen
        {
            get { return lbunidadAlmacen; }
            set
            {
                if (value != lbunidadAlmacen)
                {
                    lbunidadAlmacen = value;
                    RaisePropertyChanged("lbUnidadAlmacen");
                }
            }
        }

        private string lbunidadDetalle;
        public string lbUnidadDetalle
        {
            get { return lbunidadDetalle; }
            set
            {
                if (value != lbunidadDetalle)
                {
                    lbunidadDetalle = value;
                    RaisePropertyChanged("lbUnidadDetalle");
                }
            }
        }

        private string lbunidadInventario;
        public string lbUnidadInventario
        {
            get { return lbunidadInventario; }
            set
            {
                if (value != lbunidadInventario)
                {
                    lbunidadInventario = value;
                    RaisePropertyChanged("lbUnidadInventario");
                }
            }
        }

        private string lbunidadexistencia;
        public string lbUnidadExistencia
        {
            get { return lbunidadexistencia; }
            set
            {
                if (value != lbunidadexistencia)
                {
                    lbunidadexistencia = value;
                    RaisePropertyChanged("lbUnidadExistencia");
                }
            }
        }

        private decimal unidadAlmacen;
        public decimal UnidadAlmacen
        {
            get { return unidadAlmacen; }
            set
            {
                if (value != unidadAlmacen)
                {
                    unidadAlmacen = value;
                    RaisePropertyChanged("UnidadAlmacen");
                }
            }
        }

        private decimal unidadDetalle;
        public decimal UnidadDetalle
        {
            get { return unidadDetalle; }
            set
            {
                if (value != unidadDetalle)
                {
                    unidadDetalle = value;
                    RaisePropertyChanged("UnidadDetalle");
                }
            }
        }

        private decimal unidadExistencia;
        public decimal UnidadExistencia
        {
            get { return unidadExistencia; }
            set
            {
                if (value != unidadExistencia)
                {
                    unidadExistencia = value;
                    RaisePropertyChanged("UnidadExistencia");
                }
            }
        }

        private decimal unidadInventario;
        public decimal UnidadInventario
        {
            get { return unidadInventario; }
            set
            {
                if (value != unidadInventario)
                {
                    unidadInventario = value;
                    RaisePropertyChanged("UnidadInventario");
                }
            }
        }

        private decimal subTotal;
        public decimal SubTotal
        {
            get { return subTotal; }
            set
            {
                if (value != subTotal)
                {
                    subTotal = value;
                    RaisePropertyChanged("SubTotal");
                }
            }
        }

        private decimal total;
        public decimal Total
        {
            get { return total; }
            set
            {
                if (value != total)
                {
                    total = value;
                    RaisePropertyChanged("Total");
                }
            }
        }

        private decimal precioAlmacen;
        public decimal PrecioAlmacen
        {
            get { return precioAlmacen; }
            set
            {
                if (value != precioAlmacen)
                {
                    precioAlmacen = value;
                    RaisePropertyChanged("PrecioAlmacen");
                }
            }
        }

        private decimal precioDetalle;
        public decimal PrecioDetalle
        {
            get { return precioDetalle; }
            set
            {
                if (value != precioDetalle)
                {
                    precioDetalle = value;
                    RaisePropertyChanged("PrecioDetalle");
                }
            }
        }

        private bool busquedaVisible;
        public bool BusquedaVisible
        {
            get { return busquedaVisible; }
            set { busquedaVisible = value; RaisePropertyChanged("BusquedaVisible"); }
        }

        private bool refrescarVisible;
        public bool RefrescarVisible
        {
            get { return refrescarVisible; }
            set { refrescarVisible = value; RaisePropertyChanged("RefrescarVisible"); }
        }

		#endregion

			

		#region Métodos de instancia

		#region Métodos privados
		/// <summary>
		/// Carga los valores por defecto de la ventana.
		/// </summary>
		private void CargarVentana()
		{
            bool anteriorCargando = this.cargando;
            this.cargando = true;            
            this.cargando = anteriorCargando;

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

			this.LlenarComboCias();
            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      CriterioArticulo.Familia,
                      CriterioArticulo.Clase,
                      CriterioArticulo.VentaActual
                    };
            CriterioActual = CriterioArticulo.VentaActual;
            switch(Pedidos.CriterioBusquedaDefault)
            {
                case 0:CriterioActual = CriterioArticulo.Codigo;break;
                case 1:CriterioActual = CriterioArticulo.Barras;break;
                case 2:CriterioActual = CriterioArticulo.Descripcion;break;
                case 3:CriterioActual = CriterioArticulo.Familia;break;
                case 4:CriterioActual = CriterioArticulo.Clase;break;
                case 5:CriterioActual = CriterioArticulo.VentaActual; break;
            }           

            //this.CargarConfiguracion();

			Gestor.Consignaciones.SacarMontosTotales();
            LtbTotal = GestorUtilitario.Formato(Gestor.Consignaciones.TotalNeto);
            this.lbUnidadAlmacen = "U. Almacén:";
            this.lbUnidadDetalle = "U. Detalle:";
            this.lbUnidadInventario = "Inventario(UND)";
            this.lbUnidadExistencia = "Existencia(UND)";
			//El foco debe estar en el filtro de búsqueda.
			//this.ltbArticulo.Focus();
		}

        // Caso# CR0-02945-TW44 LAS. Se carga la configuración de la venta en esta ventana y no en la de configuración,
        // para que se puede realizar la búqueda de los artículos después de realizar el desglose.
        /// <summary>
        /// Método encargado de cargar la configuración de la venta en consignación
        /// </summary>
        private void CargarConfiguracion()
        {

            foreach (Softland.ERP.FR.Mobile.Cls.FRCliente.ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                ConfigDocCia config = this.CargarConfiguracionCliente(clt.Compania);

                clt.ObtenerDireccionesEntrega();
                config.ClienteCia = clt;

                Gestor.Consignaciones.CargarConfiguracionVenta(clt.Compania, config);
            }
        }

		/// <summary>
		/// Método encargado de llenar el combo con las compañías asociadas al cliente.
		/// </summary>
        private void LlenarComboCias()
        {
            if (this.seInvocoDe == Formulario.SeleccionarCliente || this.seInvocoDe == Formulario.ConsultaConsignacion)
            {
                Companias = new SimpleObservableCollection<string>(Util.CargarCiasConsignacion(Gestor.Consignaciones.Gestionados, EstadoConsignacion.NoDefinido));
                if (Companias.Count > 0)
                {
                    CompaniaActual = Companias[0];
                }
            }
            else
            {
                Companias = new SimpleObservableCollection<string>(Util.CargarCiasClienteConsignacion());
                if (Companias.Count > 0)
                {
                    CompaniaActual = Companias[0];
                } 
            }
        }

        /// <summary>
        /// Carga la configuracion del pedido a partir de la informacion del cliente.
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        private ConfigDocCia CargarConfiguracionCliente(string compania)
        {
            return Gestor.Consignaciones.CargarConfiguracionCliente(GlobalUI.ClienteActual, compania);
        }
		/// <summary>
		/// Método utilizado cuando se selecciona un criterio del combo.
		/// </summary>
		private void CambioCriterio()
		{
            //criterio = Util.CambiarCriterio(cboCriterio.SelectedIndex);

			if (CriterioActual==CriterioArticulo.VentaActual)
			{
				//Se seleccionó la opción 'Venta Actual' por lo que debemos mostrar el detalle de la venta en consignación actual.
				this.MostrarVentaActual();

				//Deshabilitamos la opción de búsqueda
				this.BusquedaVisible = false;
				this.RefrescarVisible = false;

				this.TextoBusqueda = string.Empty;
				this.UnidadAlmacen = 0;
				this.UnidadDetalle = 0;
				this.PrecioAlmacen = 0;
				this.PrecioDetalle = 0;
				this.SubTotal = 0;
			}
			else
			{
				//Habilitamos la opción de búsqueda
                this.BusquedaVisible = true;
                this.RefrescarVisible = true;
			}
		}

		/// <summary>
		/// Muestra en el listview solamente las líneas de la venta en consginación actual.
		/// </summary>
        private void MostrarVentaActual()
        {
            this.ListaArticulos.Clear();
            //this.lstArticulos.Items.Clear();            
            

            VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(CompaniaActual);

            if (ventaConsignacion != null)
            {
                foreach (DetalleVenta detalle in ventaConsignacion.Detalles.Lista)
                {
                    ListaArticulos.Add(detalle.Articulo);
                    //string[] itemes = { detalle.Articulo.Codigo, detalle.Articulo.Descripcion, GestorUtilitario.Formato(detalle.Articulo.UnidadEmpaque) };
                    //this.ListaArticulos.Add(detalle.Articulo);
                    //this.lstArticulos.Items.Add(new ListViewItem(itemes));
                }
                Items = new SimpleObservableCollection<Articulo>(ListaArticulos);
            }
        }

        public  bool ValidarDatos(CriterioArticulo criterio, string compania, string textoConsulta)
        {
            if (criterio == CriterioArticulo.Ninguno)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un criterio de búsqueda");
                return false;
            }

            if (compania == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return false;
            }

            if (textoConsulta.Equals(string.Empty))
            {
                this.mostrarAlerta("Ingrese el texto a consultar.");
                return false;
            }
            return true;
        }
		/// <summary>
		/// Realiza una búsqueda por aproximación según el criterio de búsqueda seleccionado en el grupo de artículos de
		/// la ruta actual de la compañía seleccionada.
		/// </summary>
		public void RealizarBusqueda()
		{
            if (!ValidarDatos(CriterioActual, CompaniaActual, this.TextoBusqueda))
                return;

			try
			{
                if (Items != null)
                {
                    Items.Clear();
                }
                ListaArticulos.Clear();                			
					   	
                if (CriterioActual == CriterioArticulo.Ninguno)
                    this.mostrarAlerta("Opción de búsqueda inválida");

                string companiaSeleccionada = CompaniaActual;
                List<FiltroArticulo> filtros = new List<FiltroArticulo> { FiltroArticulo.GrupoArticulo, FiltroArticulo.NivelPrecio, FiltroArticulo.Existencia };

                ListaArticulos =
                    Articulo.ObtenerArticulos(
                        filtros.ToArray(),
                        new CriterioBusquedaArticulo(CriterioActual, TextoBusqueda, false),
                        companiaSeleccionada,
                        GlobalUI.RutaActual.GrupoArticulo,
                        Gestor.Consignaciones.ObtenerConfiguracionVenta(companiaSeleccionada).Nivel.Nivel);

                if (ListaArticulos.Count == 0)
                {
                    ItemActual = null;
                    this.mostrarMensaje(Mensaje.Accion.BusquedaMala);
                }
                else
                {
                    Items = new SimpleObservableCollection<Articulo>(ListaArticulos);
                    ItemActual = Items[0];
                }
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
			}
		}

		/// <summary>
		/// Método que se encarga de buscar los datos del artículo seleccionado. 
		/// </summary>
		public void MostrarDatosArticulo()
		{
			if(this.ItemActual== null)
			{
				//No se ha seleccionado un artículo.
				return;
			}

			if (string.IsNullOrEmpty(CompaniaActual))
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"una compañía");
				return;
			}            
            
            DetalleVenta detalle = Gestor.Consignaciones.BuscarDetalle(this.ItemActual);
			
			if (detalle == null)
			{
				detalle = new DetalleVenta();

				//Cargamos el precio del artículo si no ha sido cargado.
				this.CargarPrecioArticulo();
                detalle.Precio = this.ItemActual.Precio;

			}
			else
                this.ItemActual.Precio = detalle.Precio;
			
			this.PrecioAlmacen = detalle.Precio.Maximo;
			this.PrecioDetalle = detalle.Precio.Minimo;
						
			this.cargando = true;
            
            this.lbUnidadAlmacen = this.ItemActual.TipoEmpaqueAlmacen + ":";
            this.lbUnidadDetalle = this.ItemActual.TipoEmpaqueDetalle + ":";
            this.lbUnidadInventario = "Inventario " + this.ItemActual.TipoEmpaqueDetalle + ":";

			try
			{
                this.UnidadInventario = Inventario.CantidadEnInventario(this.ItemActual, GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona);
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error consultando cantidades inventariadas. " + ex.Message);
			}

            this.lbUnidadExistencia = "Existencia " + this.ItemActual.TipoEmpaqueDetalle + ":";
			
			this.CargarExistenciasArticulo();

			this.UnidadAlmacen = detalle.UnidadesAlmacen;
			this.UnidadDetalle = detalle.UnidadesDetalle;

			this.SubTotal = detalle.MontoTotal;

            //TextoBusqueda = ItemActual.Codigo;

			this.cargando = false;
            
		}

		/// <summary>
		/// Carga los datos de precios del artículo, basados en la lista de precios seleccionada
		/// para la realización de la venta en consignación.
		/// </summary>
		private void CargarPrecioArticulo()
		{
            //Obtenemos la configuracion de pedido para la compania del articulo.
            ConfigDocCia config = Gestor.Consignaciones.ObtenerConfiguracionVenta(ItemActual.Compania);

            try
            {
                this.ItemActual.CargarPrecio(config.Nivel);
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando precios del artículo. " + ex.Message);
            }
        }

		/// <summary>
		/// Carga las existencias del artículo según la bodega de la ruta y las presenta en pantalla.
		/// </summary>
        private void CargarExistenciasArticulo()
        {
            try
            {
                //Sacamos las existencias de acuerdo a la bodega asignada a la ruta
                ItemActual.CargarExistencia(GlobalUI.RutaActual.Bodega);
                this.UnidadExistencia = ItemActual.Bodega.Existencia;
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando las existencias en planta. " + ex.Message);
            }
        }

		/// <summary>
		/// Actualiza las unidades de almacén y calcula el subTotal de la línea.
		/// </summary>
		public void CambioUnidadesAlmacen()
		{
			try
			{
				if (!this.cargando && ItemActual!=null)
					this.CalculaSubTotal();
			}
			catch(Exception)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
				this.UnidadAlmacen = 0;
			}
		}

		/// <summary>
		/// Actualiza las unidades de almacén, las unidades de detalle y calcula el subTotal de la línea.
		/// </summary>
		public void CambioUnidadesDetalle()
		{
			try
			{
                if (!this.cargando && ItemActual!=null)
					this.CalculaSubTotal();
			}
			catch(Exception)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
				this.UnidadDetalle = 0;
			}
		}

		/// <summary>
		/// Hace la conversión de unidades de detalle a unidades de almacén tomando en cuenta las 
		/// unidades de empaque y calcula el subtotal de la línea de detalle para estas cantidades 
		/// tomando en cuenta los precios del artículo.
		/// </summary>
		private void CalculaSubTotal()
		{
			decimal precioAlmacen = 0;
			decimal precioDetelle = 0;
			decimal subTotal = 0;
			decimal cantDetalle = 0;
			decimal cantAlmacen = 0;

			this.cargando = true;
					
			GestorUtilitario.CalculaUnidades(this.UnidadAlmacen,
				this.UnidadDetalle,
				Convert.ToInt32(this.ItemActual.UnidadEmpaque),
				out cantAlmacen,
                out cantDetalle);

			precioAlmacen = this.PrecioAlmacen;
			precioDetelle = this.PrecioDetalle;

			subTotal = (precioAlmacen * cantAlmacen) + (precioDetelle * cantDetalle);

			this.UnidadAlmacen = cantAlmacen;
			this.UnidadDetalle = cantDetalle;
			this.SubTotal = subTotal;

			this.cargando = false;
		}

		/// <summary>
		/// Método utilizado cuando el usuario desea ver las líneas gestionadas de la venta en consignación.
		/// </summary>
		private void MostrarDetalles()		
		{
			if(!Gestor.Consignaciones.ExistenArticulosGestionados())
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"No hay líneas incluídas");
				return;
			}

            Dictionary<string, object> p = new Dictionary<string, object>();
            p.Add("invocaDesde", Formulario.TomaVentaConsignacion.ToString());
            if (this.ventasSugeridas)
            {
                p.Add("ventasSugeridas","S");
            }
            else 
            {
                p.Add("ventasSugeridas","N");
            }            
            p.Add("permitirModificarVentaConsignacion","S");

            this.RequestDialogNavigate<DetalleVentaConsignacionViewModel, DialogResult>(p, resp =>
            {
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

                if (resp == DialogResult.OK)
                {
                    //Limpiamos las ventas en consignación que han sido gestionadas.
                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

                    //Recalculamos los montos totales de la venta en consignación.
                    Gestor.Consignaciones.SacarMontosTotales();
                    
                    //Actualizamos el total pués en la consulta pudieron haber borrado líneas.
                    this.MostrarDatosArticulo();
                    this.Total = Gestor.Consignaciones.TotalNeto;
                    //this.ltbArticulo.Focus();
                }
            });

            
            
		}

		/// <summary>
		/// Agrega un detalle a la gestión de la venta en consignación.
		/// </summary>
		private void AgregarDetalle()		
		{
			//No se ha seleccionado un artículo y el criterio es venta actual.
			if(this.ItemActual == null && this.CriterioActual == CriterioArticulo.Ninguno)
				return;

			if (string.IsNullOrEmpty(CompaniaActual))
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"una compañía");
				return;
			}

			//Obtenemos las unidades a ingresar.
            decimal unidadesAlm = this.UnidadAlmacen;
			decimal unidadesDet = this.UnidadDetalle;

            if (this.ItemActual == null)
			{
				//No se ha seleccionado un artículo.
				if (this.TextoBusqueda != "" && (unidadesAlm > 0 || unidadesDet > 0))
				{
					//Es un ingreso rápido de artículos.
					this.RealizarBusqueda();

					if (this.Items.Count == 1)
					{
						//Asignamos el artículo que acabamos de encontrar.
                        this.ItemActual = this.Items[0];

						//Cargamos la información de precios.
						this.CargarPrecioArticulo();

                        this.PrecioAlmacen = this.ItemActual.Precio.Maximo;
                        this.PrecioDetalle = this.ItemActual.Precio.Minimo;

						//Calculamos el subtotal y hacemos la conversión de detalle a almacén.
						this.CalculaSubTotal();

						//Extraemos nuevamente las unidades pués pudieron cambiar luego de la conversión.
						unidadesAlm = this.UnidadAlmacen;
						unidadesDet = this.UnidadDetalle;
						
						//Se cargan las existencias del articulo.
						this.CargarExistenciasArticulo();
					}
					else
					{
						//No hay articulos o salieron más de uno.
						return;
					}
				}
				else
				{
					//No es un ingreso rápido de artículos.
					return;
				}
			}

			if(this.ItemActual.Precio.Maximo == decimal.Zero && this.ItemActual.Precio.Minimo == decimal.Zero)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"No se puede consignar el artículo porque no se encuentra en el Nivel de Precios seleccionado.");
				return;
			}

			try
			{
				if (unidadesAlm > 0 || unidadesDet > 0)
				{
					//Si el detalle ha sido gestionado cuando se realizó la venta sugerida luego del desglose de una
					//boleta de venta en consignación se debe validar que la cantidad a consignar no sea menor al saldo que no fue desglosado.
					decimal saldoConsignadoAlmacen = decimal.Zero;
					decimal saldoConsignadoDetalle = decimal.Zero;
					if(!Gestor.Consignaciones.ValidarCantidadConsignar(this.ItemActual,unidadesAlm,unidadesDet, ref saldoConsignadoAlmacen, ref saldoConsignadoDetalle))
					{
						this.mostrarMensaje(Mensaje.Accion.Informacion,"La cantidad a consignar no puede ser menor al saldo previamente consignado en el desglose.");
						return;
					}

                    // Se utiliza el detalle en consignación, en caso de existir.
                    DetalleVenta detalle = Gestor.Consignaciones.BuscarDetalle(this.ItemActual);
                    if (detalle == null)
                    {
                        detalle = new DetalleVenta();
                        detalle.Articulo = (Articulo)this.ItemActual.Clone();
                    }

                    //Debemos verificar que hayan suficientes existencias en el camión para satisfacer la 
					//venta en consignación del detalle.
					decimal existencias = this.UnidadExistencia;
					decimal cantidadConsignar = unidadesAlm + (unidadesDet / this.ItemActual.UnidadEmpaque);
					decimal cantidadSaldo = saldoConsignadoAlmacen + (saldoConsignadoDetalle / this.ItemActual.UnidadEmpaque);

					decimal cantidadRequerida = cantidadConsignar - cantidadSaldo;

                    // Sumamos las existencias del desglose, en caso de que se restablezca la boleta.
                    if (existencias > 0 && detalle.UnidadesAlmacenSaldo >= 0 && !detalle.FaltaExistencia)
                        existencias += detalle.TotalAlmacenDesglose;
					if (existencias < cantidadRequerida)
					{
						decimal cantidadAlmacenRequerida = decimal.Zero;
						decimal cantidadDetalleRequerida = decimal.Zero;
                        decimal cantidadAlmacenFaltante = cantidadRequerida - existencias;
						cantidadAlmacenRequerida = Convert.ToInt32(cantidadRequerida);
						cantidadDetalleRequerida = (cantidadRequerida - cantidadAlmacenRequerida) * this.ItemActual.UnidadEmpaque;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"No hay suficientes existencias. Faltan '" + cantidadAlmacenFaltante.ToString() + "' unidades de almacén y '" + cantidadDetalleRequerida.ToString("0.00") + "' unidades de detalle.");
						return;
					}
					//Obtenemos los precios definidos por el usuario.
					decimal precioMax = PrecioAlmacen;
					decimal precioMin = PrecioDetalle;

					try
					{
						//Agregamos o cambiamos la línea de la venta en consignación.
						Gestor.Consignaciones.Gestionar(this.ItemActual,GlobalUI.ClienteActual.Zona,unidadesAlm,unidadesDet,this.detalle.UnidadesAlmacenSaldo, this.detalle.UnidadesDetalleSaldo,new Precio(precioMax,precioMin),false);
					}
					catch(Exception ex)
					{
						this.mostrarAlerta(ex.Message);
					}

					this.Total  = Gestor.Consignaciones.TotalNeto; 
                    
                    //Muestra la venta Actual
                    this.MostrarVentaActual();
				}
				else
				{
					this.mostrarAlerta("Ambas cantidades no pueden ser cero.");   
				}
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error al agregar línea. " + ex.Message);
			}
            ItemActual.CantidadAlmacen = unidadesAlm;
            ItemActual.CantidadDetalle = unidadesDet;
            RaisePropertyChanged("Articulos");

			//Seteamos el foco en el filtro de búsqueda por si la búsqueda es con lector de códigos de barra.
            //this.ltbArticulo.Focus();

		}

		/// <summary>
		/// Método que gestiona el retiro de una línea de detalle incluida en la venta en consignación gestionada.
		/// </summary>
		private void RetirarDetalle()		
		{
			//No se ha seleccionado un artículo.
			if(this.ItemActual == null)
				return;

			if (string.IsNullOrEmpty(CompaniaActual))
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"una compañía");
				return;
			}
			
			DetalleVenta detalle = Gestor.Consignaciones.BuscarDetalle(this.ItemActual);

			if (detalle == null)
			{
				this.mostrarAlerta("El artículo no ha sido ingresado a la venta en consignación, por esto no puede ser eliminado.");
				return;
			}

            this.mostrarMensaje(Mensaje.Accion.Retirar, ("el artículo: " + detalle.Articulo.Codigo),
                result =>
                {
                    if (result == DialogResult.OK || result == DialogResult.Yes)
                    {
                        // Si es negativo, el saldo se encuentra en UnidadesAlmacen ó UnidadesDetalle 
                        decimal saldoAlmacen = detalle.SaldoDesgloseAlmacen;
                        decimal saldoDetalle = detalle.SaldoDesgloseDetalle;

                        if (saldoAlmacen > decimal.Zero || saldoDetalle > decimal.Zero)
                        {
                            this.mostrarAlerta("El detalle no se puede eliminar ya que posee saldo luego del desglose.");
                            return;
                        }

                        try
                        {
                            Gestor.Consignaciones.EliminarDetalle(this.ItemActual);
                            this.ItemActual.ActualizarBodega(detalle.TotalAlmacenDesglose);
                            this.UnidadExistencia= GestorUtilitario.ParseAndRoundDecimal(this.ItemActual.Bodega.Existencia.ToString());
                            this.UnidadAlmacen = 0;
                            this.UnidadDetalle = 0;
                            this.SubTotal = 0;
                            this.Total = Gestor.Consignaciones.TotalNeto;
                            this.MostrarVentaActual();
                        }
                        catch (Exception exc)
                        {
                            this.mostrarAlerta("Error al gestionar el retiro del detalle. " + exc.Message);
                        }
                        RealizarBusqueda();
                    }
                });

			//El criterio es Venta Actual, por lo tanto debemos refrescar el listview.
			//if (CriterioActual == CriterioArticulo.VentaActual)
				

			//Seteamos el foco en el filtro de búsqueda por si la búsqueda es con lector de códigos de barra.
            //Caso: 32892 ABC 02/07/2008
            //Cambiamos el focus de tal maneta que apunte hacia la lista y en caso de que sea codigo de barras
            //this.ltbArticulo.Focus();
		}

		/// <summary>
		/// Método utilizado cuando el usuario desea cancelar la toma de la venta en consignación.
		/// </summary>
		private void CancelarToma()		
		{
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de la Venta en Consignación", result =>
                {
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {
                        if (this.ventasSugeridas && Gestor.Consignaciones.ExisteSaldo())
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "La venta en consignación posee detalles que tienen saldo previamente consignado en el desglose, debe terminar la toma de la venta en consignación.");
                            return;
                        }
                        //Limpiar las consignaciones
                        Gestor.Consignaciones = new VentasConsignacion();

                        if (this.seInvocoDe == Formulario.ConsultaConsignacion || this.seInvocoDe == Formulario.SeleccionarCliente)
                        {
                            this.DoClose();
                            RequestNavigate<ConsultaConsignacionViewModel>();
                            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                            
                        }
                        else
                        {
                            Dictionary<string, object> par = new Dictionary<string, object>();
                            par.Add("habilitarPedidos", true);
                            this.DoClose();
                            RequestNavigate<MenuClienteViewModel>(par);
                            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                            
                        }
                    }
                }
                );
		}

		/// <summary>
		/// Método utilizado cuando el usuario desea continuar con la gestión de la venta en consignación.
		/// </summary>
		private void Continuar()		
		{
			if (Gestor.Consignaciones.Gestionados.Count > 0)
			{
				
                Dictionary<string, object> p = new Dictionary<string, object>();
                if (this.seInvocoDe == Formulario.ConsultaConsignacion || this.seInvocoDe == Formulario.SeleccionarCliente)
                {                    
                    p.Add("invocaDesde",this.seInvocoDe.ToString());
                    if (this.ventasSugeridas)
                    {
                        p.Add("ventasSugeridas", "S");
                    }
                    else
                    {
                        p.Add("ventasSugeridas", "N");
                    }
                    this.DoClose();
                    RequestNavigate<AplicarVentaConsignacionViewModel>(p);                    
                }
                else
                {
                    p.Add("invocaDesde",Formulario.TomaVentaConsignacion.ToString());
                    if (this.ventasSugeridas)
                    {
                        p.Add("ventasSugeridas", "S");
                    }
                    else
                    {
                        p.Add("ventasSugeridas", "N");
                    }
                    this.DoClose();
                    RequestNavigate<AplicarVentaConsignacionViewModel>(p);                    
                }				
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

			}
			else
				this.mostrarMensaje(Mensaje.Accion.Informacion,"No hay ventas en consignación gestionadas aún.");
		}

        /// <summary>
        /// Actualiza las existencias del rutero
        /// LDA CR1-05072-RGHK
        /// Se descuadraban las existencias del Rutero
        /// </summary>
        private void ActualizarExistencias()
        {
            ListaArticulos.Clear();
            VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(CompaniaActual);
            if (ventaConsignacion != null)
            {                
                foreach (DetalleVenta detalle in ventaConsignacion.Detalles.Lista)
                {
                    detalle.Articulo.CargarExistencia(GlobalUI.RutaActual.Bodega);
                    this.ListaArticulos.Add(detalle.Articulo);
                }
                Items = new SimpleObservableCollection<Articulo>(ListaArticulos);
            }
        }

        public void DoNothing() 
        {
            CancelarToma();
        }

        public void OnResume()
        {
            ventanaInactiva = false;
            RaisePropertyChanged("TextoBusqueda");
        }  

        #endregion

		#endregion	

        
        

        #endregion

    }
}
