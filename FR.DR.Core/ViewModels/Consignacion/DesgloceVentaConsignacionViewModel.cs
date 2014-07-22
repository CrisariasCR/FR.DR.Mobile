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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DesgloseVentaConsignacionViewModel : BaseViewModel
    {
        /// <summary>
        /// Variable utilizada para evitar doble procesamiento en calculo de unidades.
        /// </summary>
        //private bool inicial = true;
        public bool cargando = false;
        private bool desgloseSaldos;
        private bool validarCambioPrecios;
        string observacionDetalle;
        public bool EsBarras;
        public static bool ventanaInactiva = false;
        public bool Cargando { get { return cargando; } }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        /// <summary>
		/// Permite realizar el desglose de la boleta de venta en consignación.
		/// </summary>
		/// <param name="desgloseSaldos">
		/// Indica si se debe desglosar solo los artículos que poseen saldos.
		/// Los artículos poseen saldos solo cuando se ha creado una boleta de venta en consignación a partir 
		/// del desglose de una boleta de venta en consignación que quedó con saldos sin desglosar. 
		/// </param>
        public DesgloseVentaConsignacionViewModel(string desgloseSaldos, string validarCambioPrecios) 
        {
            this.desgloseSaldos = desgloseSaldos.Equals("S");
            this.validarCambioPrecios = validarCambioPrecios.Equals("S");
            this.CargarVentana();
        }

        #region Propiedades

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Hola" }; } }


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

        private List<DetalleVenta> ListaDetalles = new List<DetalleVenta>();

        private DetalleVenta itemActual;
        public DetalleVenta ItemActual
        {
            get { return itemActual; }
            set
            {
                itemActual = value; RaisePropertyChanged("ItemActual");
                MostrarDatosArticulo();
            }
        }

        private IObservableCollection<DetalleVenta> items;
        public IObservableCollection<DetalleVenta> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }

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

        private CriterioArticulo criterio = new CriterioArticulo();

        /// <summary>
        /// datasource de los criterios de filtro, deben conincidir con los del enumerado CriterioArticulo
        /// </summary>
        public IObservableCollection<CriterioArticulo> Criterios { get; set; }

        private bool busquedaEnabled;
        public bool BusquedaEnabled
        {
            get { return busquedaEnabled; }
            set
            {
                busquedaEnabled = value; RaisePropertyChanged("BusquedaEnabled");
            }
        }

        private bool refrescarEnabled;
        public bool RefrescarEnabled
        {
            get { return refrescarEnabled; }
            set
            {
                refrescarEnabled = value; RaisePropertyChanged("RefrescarEnabled");
            }
        }

        private bool lotesEnabled;
        public bool LotesEnabled
        {
            get { return lotesEnabled; }
            set
            {
                lotesEnabled = value; RaisePropertyChanged("LotesEnabled");
            }
        }

        private decimal unidadAlmacenConsignado;
        public decimal UnidadAlmacenConsignado
        {
            get { return unidadAlmacenConsignado; }
            set
            {
                unidadAlmacenConsignado = value; RaisePropertyChanged("UnidadAlmacenConsignado");
            }
        }

        private decimal unidadDetalleConsignado;
        public decimal UnidadDetalleConsignado
        {
            get { return unidadDetalleConsignado; }
            set
            {
                unidadDetalleConsignado = value; RaisePropertyChanged("UnidadDetalleConsignado");
            }
        }

        private decimal unidadAlmacenVendido;
        public decimal UnidadAlmacenVendido
        {
            get { return unidadAlmacenVendido; }
            set
            {
                unidadAlmacenVendido = value; RaisePropertyChanged("UnidadAlmacenVendido");
            }
        }

        private decimal unidadDetalleVendido;
        public decimal UnidadDetalleVendido
        {
            get { return unidadDetalleVendido; }
            set
            {
                unidadDetalleVendido = value; RaisePropertyChanged("UnidadDetalleVendido");
            }
        }

        private decimal unidadAlmacenBueno;
        public decimal UnidadAlmacenBueno
        {
            get { return unidadAlmacenBueno; }
            set
            {
                unidadAlmacenBueno = value; RaisePropertyChanged("UnidadAlmacenBueno");
            }
        }

        private decimal unidadDetalleBueno;
        public decimal UnidadDetalleBueno
        {
            get { return unidadDetalleBueno; }
            set
            {
                unidadDetalleBueno = value; RaisePropertyChanged("UnidadDetalleBueno");
            }
        }

        private decimal unidadAlmacenMalo;
        public decimal UnidadAlmacenMalo
        {
            get { return unidadAlmacenMalo; }
            set
            {
                unidadAlmacenMalo = value; RaisePropertyChanged("UnidadAlmacenMalo");
            }
        }

        private decimal unidadDetalleMalo;
        public decimal UnidadDetalleMalo
        {
            get { return unidadDetalleMalo; }
            set
            {
                unidadDetalleMalo = value; RaisePropertyChanged("UnidadDetalleMalo");
            }
        }

        private decimal unidadAlmacenSaldo;
        public decimal UnidadAlmacenSaldo
        {
            get { return unidadAlmacenSaldo; }
            set
            {
                unidadAlmacenSaldo = value; RaisePropertyChanged("UnidadAlmacenSaldo");
            }
        }	

        private decimal unidadDetalleSaldo;
        public decimal UnidadDetalleSaldo
        {
            get { return unidadDetalleSaldo; }
            set
            {
                unidadDetalleSaldo = value; RaisePropertyChanged("UnidadDetalleSaldo");
            }
        }

        private string lblUnidadAlmacenBoleta;
        public string LblUnidadAlmacenBoleta
        {
            get { return lblUnidadAlmacenBoleta; }
            set
            {
                lblUnidadAlmacenBoleta = value; RaisePropertyChanged("LblUnidadAlmacenBoleta");
            }
        }

        private string lblUnidadDetalleBoleta;
        public string LblUnidadDetalleBoleta
        {
            get { return lblUnidadDetalleBoleta; }
            set
            {
                lblUnidadDetalleBoleta = value; RaisePropertyChanged("LblUnidadDetalleBoleta");
            }
        }

        private string lblUnidadAlmacenVendido;
        public string LblUnidadAlmacenVendido
        {
            get { return lblUnidadAlmacenVendido; }
            set
            {
                lblUnidadAlmacenVendido = value; RaisePropertyChanged("LblUnidadAlmacenVendido");
            }
        }        

        private string lblUnidadDetalleVendido;
        public string LblUnidadDetalleVendido
        {
            get { return lblUnidadDetalleVendido; }
            set
            {
                lblUnidadDetalleVendido = value; RaisePropertyChanged("LblUnidadDetalleVendido");
            }
        }            

        private string lblUnidadAlmacenBueno;
        public string LblUnidadAlmacenBueno
        {
            get { return lblUnidadAlmacenBueno; }
            set
            {
                lblUnidadAlmacenBueno = value; RaisePropertyChanged("LblUnidadAlmacenBueno");
            }
        }
        
        private string lblUnidadDetalleBueno;
        public string LblUnidadDetalleBueno
        {
            get { return lblUnidadDetalleBueno; }
            set
            {
                lblUnidadDetalleBueno = value; RaisePropertyChanged("LblUnidadDetalleBueno");
            }
        }

        private string lblUnidadAlmacenMalo;
        public string LblUnidadAlmacenMalo
        {
            get { return lblUnidadAlmacenMalo; }
            set
            {
                lblUnidadAlmacenMalo = value; RaisePropertyChanged("LblUnidadAlmacenMalo");
            }
        }
        
        private string lblUnidadDetalleMalo;
        public string LblUnidadDetalleMalo
        {
            get { return lblUnidadDetalleMalo; }
            set
            {
                lblUnidadDetalleMalo = value; RaisePropertyChanged("LblUnidadDetalleMalo");
            }
        }            

        private string lblUnidadAlmacenSaldo;
        public string LblUnidadAlmacenSaldo
        {
            get { return lblUnidadAlmacenSaldo; }
            set
            {
                lblUnidadAlmacenSaldo = value; RaisePropertyChanged("LblUnidadAlmacenSaldo");
            }
        }

        private string lblUnidadDetalleSaldo;
        public string LblUnidadDetalleSaldo
        {
            get { return lblUnidadDetalleSaldo; }
            set
            {
                lblUnidadDetalleSaldo = value; RaisePropertyChanged("LblUnidadDetalleSaldo");
            }
        }

        private string lote;
        public string Lote
        {
            get { return lote; }
            set
            {
                lote = value; RaisePropertyChanged("Lote");
            }
        }       
            
            

        #endregion Propiedades

        #region Comandos

        public ICommand ComandoNota
        {
            get { return new MvxRelayCommand(DefinirObservacion); }
        }

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(AgregarDesgloseDetalle); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(CancelarDesglose); }
        }
        
        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(RealizarBusqueda); }
        }

        #endregion Comandos

        #region mobile
        

		#region Métodos de instancia
		
		#region Métodos privados
		/// <summary>
		/// Carga los valores por defecto de la ventana.
		/// </summary>
		private void CargarVentana()
		{
            bool cargaAnterior = this.cargando;
            cargando=true;            
            this.cargando=cargaAnterior;

            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                         " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNombreCliente.Visible = false;

			if(Gestor.DesglosesConsignacion.Gestionados.Count == 0)
				this.CargarBoletasVentaConsignacion();

            Companias = new SimpleObservableCollection<string>(Util.CargarCiasConsignacion(Gestor.DesglosesConsignacion.Gestionados,
                (this.desgloseSaldos ? EstadoConsignacion.NoSincronizada : EstadoConsignacion.Procesada)));
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }

			//Se selecciona la boleta actual.
            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      CriterioArticulo.Familia,
                      CriterioArticulo.Clase,
                      CriterioArticulo.BoletaActual
                    };
            CriterioActual = CriterioArticulo.BoletaActual;
			this.observacionDetalle = string.Empty;
			
		}

		/// <summary>
		/// Carga las boletas de venta en consignación para el cliente en la ruta actual.
		/// </summary>
		private void CargarBoletasVentaConsignacion()
		{
			Gestor.DesglosesConsignacion.CargarBoletasVentaConsignacion(GlobalUI.RutaActual,GlobalUI.ClienteActual,this.desgloseSaldos,false);
			//Establece el saldo a los detalles de la venta en consignación.
			
            if(!this.desgloseSaldos)
			{
                Gestor.DesglosesConsignacion.GenerarSaldos();

			}
            if (this.validarCambioPrecios && Gestor.DesglosesConsignacion.CambioEnPrecios())
            {
                Bitacora.Escribir("Exiten cambios en los precios de los artículos consignados.");
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Exiten cambios en los precios de los artículos consignados.");
            }
		}
		/// <summary>
		/// Método utilizado cuando se selecciona un criterio del combo.
		/// </summary>
		private void CambioCriterio()
		{
			//this.criterio = Util.CambiarCriterio(this.cboCriterio.SelectedIndex);

            if (this.CriterioActual == CriterioArticulo.BoletaActual)
			{
				//Se seleccionó la opción 'Boleta Actual' por lo que debemos mostrar el detalle de la boleta de venta en consignación actual.
				this.MostrarBoletaActual();

				//Deshabilitamos la opción de búsqueda
				this.BusquedaEnabled = false;
				this.RefrescarEnabled = false;
			}
			else
			{
				//Habilitamos la opción de búsqueda
				this.BusquedaEnabled = true;
				this.RefrescarEnabled = true;
			}
		}
		/// <summary>
		/// Muestra en el listview solamente las líneas de la boleta de venta en consginación actual.
		/// </summary>
		private void MostrarBoletaActual()
		{

			if(Gestor.DesglosesConsignacion.Gestionados.Count> 0)
			{
				this.Items.Clear();
				this.ListaDetalles.Clear();

                VentaConsignacion boletaActual = Gestor.DesglosesConsignacion.Buscar(CompaniaActual);
			
				if (boletaActual != null)
					try
					{
						foreach(DetalleVenta detalle in boletaActual.Detalles.Lista)
						{
							string [] itemes = {detalle.Articulo.Codigo,detalle.Articulo.Descripcion,GestorUtilitario.Formato(detalle.Articulo.UnidadEmpaque)};
				    	                                                  
							this.ListaDetalles.Add(detalle);
							//this.lstArticulos.Items.Add(new ListViewItem(itemes));  
						}
                        Items = new SimpleObservableCollection<DetalleVenta>(ListaDetalles);
					}
					catch(Exception ex)
					{
						this.mostrarMensaje(Mensaje.Accion.Alerta,ex.Message);
						return;
					}
			}
		}
		/// <summary>
		/// Realiza la búsqueda del artículo cuando se utiliza la selección por código de barras.
		/// </summary>
		public void CambioDeTextoEnBusqueda()
		{
			if(this.TextoBusqueda.Equals(string.Empty))
            {
                if (Items != null)
                {
                    this.Items.Clear();
                }
            }

            else if (!string.IsNullOrEmpty(TextoBusqueda) && CriterioActual == CriterioArticulo.Barras)
			{				
				this.cargando = true;
				this.cargando = false;
				
				//Seleccionamos el criterio de búsqueda por código de barras automáticamente.
				this.CriterioActual = CriterioArticulo.Barras;

				this.RealizarBusqueda();

				if(this.Items.Count!=0)
				{
                    this.ItemActual = Items[0];
                    EsBarras = true;
				}
                this.cargando = true;                
                this.cargando = false;
			}
		}
		/// <summary>
		/// Realiza una búsqueda por aproximación según el criterio de búsqueda seleccionado en el grupo de artículos de
		/// la ruta actual de la compañía seleccionada.
		/// </summary>
		public void RealizarBusqueda()
		{
            if (!Util.ValidarDatos(((CriterioArticulo)this.CriterioActual),CompaniaActual,TextoBusqueda,this))
                return;
			try
			{
				this.Items.Clear();
				this.ListaDetalles.Clear();
		   					  
				string companiaSeleccionada = CompaniaActual;
                this.criterio = CriterioActual;
				
				if (this.criterio == CriterioArticulo.Ninguno)
					this.mostrarAlerta("Opción de búsqueda inválida"); 
                
                VentaConsignacion venta = Gestor.DesglosesConsignacion.Buscar(companiaSeleccionada);
                this.ListaDetalles = venta.Detalles.Buscar(this.criterio, TextoBusqueda,false).Lista;

                if (this.ListaDetalles.Count == 0)
                    this.mostrarMensaje(Mensaje.Accion.BusquedaMala);
                else
                    Items = new SimpleObservableCollection<DetalleVenta>(ListaDetalles);
                    //foreach(DetalleVenta detalle in this.listaDetalles)
                    //{
                    //    string[] itemes = { detalle.Articulo.Codigo, detalle.Articulo.Descripcion, GestorUtilitario.Formato(detalle.Articulo.UnidadEmpaque) };
                    //    this.lstArticulos.Items.Add(new ListViewItem(itemes));  
                    //}
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
			}
		}
		/// <summary>
		/// Método que se encarga de buscar los datos del artículo seleccionado. 
		/// </summary>
		private void MostrarDatosArticulo()
		{
			//No se ha seleccionado un artículo.
			if(ItemActual==null)
				return;

			if (string.IsNullOrEmpty(CompaniaActual))
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"una compañía");
				return;
			}

			this.ItemActual = this.Items[0];
			//Se obtiene la nota de observación del detalle.
			this.observacionDetalle = this.ItemActual.Observaciones;

			this.cargando = true;
			if(this.desgloseSaldos)
			{
                decimal almacenSaldo = this.ItemActual.UnidadesAlmacenSaldo;
                decimal detalleSaldo = this.ItemActual.UnidadesDetalleSaldo;
                decimal almacenSaldoTemp = (int)(this.ItemActual.TotalAlmacenDesglose);
                decimal detalleSaldoTemp = (this.ItemActual.TotalAlmacenDesglose - almacenSaldoTemp) * this.ItemActual.Articulo.UnidadEmpaque;
				
				almacenSaldo += almacenSaldoTemp;
				detalleSaldo += detalleSaldoTemp;

                this.UnidadAlmacenConsignado = almacenSaldo;
                this.UnidadDetalleConsignado = detalleSaldo;
			}
			else
			{
                this.UnidadAlmacenConsignado = this.ItemActual.UnidadesAlmacen;
                this.UnidadDetalleConsignado = this.ItemActual.UnidadesDetalle;
			}
            this.UnidadAlmacenVendido = this.ItemActual.UnidadesAlmacenVendido;
            this.UnidadDetalleVendido = this.ItemActual.UnidadesDetalleVendido;
            this.UnidadAlmacenBueno = this.ItemActual.UnidadesAlmacenBuenEstado;
            this.UnidadDetalleBueno = this.ItemActual.UnidadesDetalleBuenEstado;
            this.UnidadAlmacenMalo = this.ItemActual.UnidadesAlmacenMalEstado;
            this.UnidadDetalleMalo = this.ItemActual.UnidadesDetalleMalEstado;
            this.UnidadAlmacenSaldo = this.ItemActual.UnidadesAlmacenSaldo;
            this.UnidadDetalleSaldo = this.ItemActual.UnidadesDetalleSaldo;

            this.LblUnidadAlmacenBoleta = this.ItemActual.Articulo.TipoEmpaqueAlmacen;
            this.LblUnidadDetalleBoleta = this.ItemActual.Articulo.TipoEmpaqueDetalle;
            this.LblUnidadAlmacenVendido = this.ItemActual.Articulo.TipoEmpaqueAlmacen;
            this.LblUnidadDetalleVendido = this.ItemActual.Articulo.TipoEmpaqueDetalle;
            this.LblUnidadAlmacenBueno = this.ItemActual.Articulo.TipoEmpaqueAlmacen;
            this.LblUnidadDetalleBueno = this.ItemActual.Articulo.TipoEmpaqueDetalle;
            this.LblUnidadAlmacenMalo = this.ItemActual.Articulo.TipoEmpaqueAlmacen;
            this.LblUnidadDetalleMalo = this.ItemActual.Articulo.TipoEmpaqueDetalle;
            this.LblUnidadAlmacenSaldo = this.ItemActual.Articulo.TipoEmpaqueAlmacen;
            this.LblUnidadDetalleSaldo = this.ItemActual.Articulo.TipoEmpaqueDetalle;

			//Se valida si se permite definir el lote.
            this.LotesEnabled = ItemActual.Articulo.UsaLotes;

			this.cargando = false;
		}
		/// <summary>
		/// Actualiza las unidades de almacén vendidas.
		/// </summary>
		public void CambioUnidadesAlmacenVendidas()
		{
			try
			{
				if (!this.cargando && this.ItemActual != null)
				{
					this.cargando = true;
					decimal cantidadDetalle = decimal.Zero;
					decimal cantidadAlmacen = decimal.Zero;
					decimal cantidadDetalleInicial = decimal.Zero;

					cantidadDetalleInicial = UnidadDetalleVendido;
					
					cantidadAlmacen = UnidadAlmacenVendido;
					cantidadDetalle = UnidadDetalleVendido;
                    this.ItemActual.CalcularUnidades(ref cantidadAlmacen, ref cantidadDetalle);

					this.UnidadAlmacenVendido = cantidadAlmacen;
					this.UnidadDetalleVendido = cantidadDetalle;

                    if (!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
					{
						this.UnidadAlmacenVendido = 0;
						this.UnidadDetalleVendido = cantidadDetalleInicial;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					}
					this.cargando = false;
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados. " + ex.Message);
			}
		}
		/// <summary>
		/// Actualiza las unidades de detalle vendidas.
		/// </summary>
        public void CambioUnidadesDetalleVendidas()
		{
			try
			{
				if (!this.cargando && ItemActual != null)
				{
					this.cargando = true;
					decimal cantidadDetalle = 0;
					decimal cantidadAlmacen = 0;
					decimal cantidadAlmacenInicial = decimal.Zero;

					cantidadAlmacenInicial = UnidadAlmacenVendido;
					
					cantidadAlmacen = UnidadAlmacenVendido;
					cantidadDetalle = UnidadDetalleVendido;
                    this.ItemActual.CalcularUnidades(ref cantidadAlmacen, ref cantidadDetalle);
                    
					this.UnidadAlmacenVendido = cantidadAlmacen;
					this.UnidadDetalleVendido = cantidadDetalle;

                    if (!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
					{
						this.UnidadDetalleVendido = 0;
						this.UnidadAlmacenVendido = cantidadAlmacenInicial;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					}
					this.cargando = false;
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados. " + ex.Message);
			}
		}
		/// <summary>
		/// Actualiza las unidades de almacén devueltas en buen estado.
		/// </summary>
        public void CambioUnidadesAlmacenBuenEstado()
		{
			try
			{
				if (!this.cargando && this.ItemActual != null)
				{
					this.cargando = true;
					decimal cantidadDetalle = 0;
					decimal cantidadAlmacen = 0;
					decimal cantidadDetalleInicial = decimal.Zero;

					cantidadDetalleInicial = UnidadDetalleBueno;

                    cantidadAlmacen = this.UnidadAlmacenBueno;
					cantidadDetalle = this.UnidadDetalleBueno;
                    this.ItemActual.CalcularUnidades(ref cantidadAlmacen, ref cantidadDetalle);

					this.UnidadAlmacenBueno = cantidadAlmacen;
					this.UnidadDetalleBueno = cantidadDetalle;

					if(!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
					{
						this.UnidadAlmacenBueno = 0;
						this.UnidadDetalleBueno = cantidadDetalleInicial;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					}
					this.cargando = false;
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados. " + ex.Message);
			}
		}
		/// <summary>
		/// Actualiza las unidades de detalle devueltas en buen estado.
		/// </summary>
        public void CambioUnidadesDetalleBuenEstado()
		{
			try
			{
				if (!this.cargando && this.ItemActual != null)
				{
					this.cargando = true;
					decimal cantidadDetalle = 0;
					decimal cantidadAlmacen = 0;
					decimal cantidadAlmacenInicial = decimal.Zero;

					cantidadAlmacenInicial = this.UnidadAlmacenBueno;
					
					cantidadAlmacen = this.UnidadAlmacenBueno;
					cantidadDetalle = this.UnidadDetalleBueno;
					this.ItemActual.CalcularUnidades(ref cantidadAlmacen, ref cantidadDetalle);

					this.UnidadAlmacenBueno = cantidadAlmacen;
					this.UnidadDetalleBueno = cantidadDetalle;

					if(!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
					{
						this.UnidadDetalleBueno = 0;
						this.UnidadAlmacenBueno = cantidadAlmacenInicial;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					}
					this.cargando = false;
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados. " + ex.Message);
			}
		}
		/// <summary>
		/// Actualiza las unidades de almacén devueltas en mal estado.
		/// </summary>
        public void CambioUnidadesAlmacenMalEstado()
		{
			try
			{
				if (!this.cargando && this.ItemActual != null)
				{
					this.cargando = true;
					decimal cantidadDetalle = decimal.Zero;
					decimal cantidadAlmacen = decimal.Zero;
					decimal cantidadDetalleInicial = decimal.Zero;

					cantidadDetalleInicial = this.UnidadDetalleMalo;
					
					cantidadAlmacen = this.UnidadAlmacenMalo;
					cantidadDetalle = this.UnidadDetalleMalo;
					 this.ItemActual.CalcularUnidades(ref cantidadAlmacen, ref cantidadDetalle);
					
					this.UnidadAlmacenMalo = cantidadAlmacen;
					this.UnidadDetalleMalo = cantidadDetalle;

					if(!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
					{
						this.UnidadAlmacenMalo = 0;
						this.UnidadDetalleMalo = cantidadDetalleInicial;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					}
					this.cargando = false;
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados. " + ex.Message);
			}
		}
		/// <summary>
		/// Actualiza las unidades de detalle devueltas en mal estado.
		/// </summary>
        public void CambioUnidadesDetalleMalEstado()
		{
			try
			{
				if (!this.cargando && this.ItemActual != null)
				{
					this.cargando = true;
					decimal cantidadDetalle = 0;
					decimal cantidadAlmacen = 0;
					decimal cantidadAlmacenInicial = decimal.Zero;

					cantidadAlmacenInicial = this.UnidadAlmacenMalo;
					
					cantidadAlmacen = this.UnidadAlmacenMalo;
					cantidadDetalle = this.UnidadDetalleMalo;
					this.ItemActual.CalcularUnidades(ref cantidadAlmacen, ref cantidadDetalle);

					this.UnidadAlmacenMalo = cantidadAlmacen;
					this.UnidadDetalleMalo = cantidadDetalle;

					if(!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
					{
						this.UnidadDetalleMalo = 0;
						this.UnidadAlmacenMalo = cantidadAlmacenInicial;
						this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					}
					this.cargando = false;
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados. " + ex.Message);
			}
		}
		/// <summary>
		/// Valida la distribución de las cantidades del detalle.
		/// </summary>
		/// /// <param name="unidadEmpaque">Unidad de empaque del detalle a desglosar.</param>
		/// <returns>En caso en que la distribución sea válida se retorna verdadero y se actualizan los saldos.</returns>
		private bool ValidarDesgloseCantidades(decimal unidadEmpaque)
		{
			decimal cantidadAlmacenConsignado = decimal.Zero;
			decimal cantidadDetalleConsignado = decimal.Zero;
			decimal cantidadAlmacenVendido = decimal.Zero;
			decimal cantidadDetalleVendido = decimal.Zero;
			decimal cantidadAlmacenBueno = decimal.Zero;
			decimal cantidadDetalleBueno = decimal.Zero;
			decimal cantidadAlmacenMalo = decimal.Zero;
			decimal cantidadDetalleMalo = decimal.Zero;
			decimal desgloseAlmacen = decimal.Zero;
			decimal desgloseDetalle = decimal.Zero;
			decimal saldoAlmacen = decimal.Zero;
			decimal saldoDetalle = decimal.Zero;
			decimal saldoTotalAlmacen = decimal.Zero;

			if(this.UnidadAlmacenConsignado.ToString().Trim().Length > 0)
				cantidadAlmacenConsignado = this.UnidadAlmacenConsignado;
            if (this.UnidadDetalleConsignado.ToString().Trim().Length > 0)
				cantidadDetalleConsignado = this.UnidadDetalleConsignado;
            if (this.UnidadAlmacenVendido.ToString().Trim().Length > 0)
				cantidadAlmacenVendido = this.UnidadAlmacenVendido;
            if (this.UnidadDetalleVendido.ToString().Trim().Length > 0)
				cantidadDetalleVendido = this.UnidadDetalleVendido;
            if (this.UnidadAlmacenBueno.ToString().Trim().Length > 0)
				cantidadAlmacenBueno = this.UnidadAlmacenBueno;
            if (this.UnidadDetalleBueno.ToString().Trim().Length > 0)
				cantidadDetalleBueno = this.UnidadDetalleBueno;
            if (this.UnidadAlmacenMalo.ToString().Trim().Length > 0)
				cantidadAlmacenMalo = this.UnidadAlmacenMalo;
            if (this.UnidadDetalleMalo.ToString().Trim().Length > 0)
				cantidadDetalleMalo = this.UnidadDetalleMalo;

			if(cantidadAlmacenConsignado < 0 ||
				cantidadDetalleConsignado < 0 ||
				cantidadAlmacenVendido < 0 ||
				cantidadDetalleVendido < 0 ||
				cantidadAlmacenBueno < 0 ||
				cantidadDetalleBueno < 0 ||
				cantidadAlmacenMalo < 0 ||
				cantidadDetalleMalo < 0)
				return false;

			desgloseAlmacen = cantidadAlmacenVendido + cantidadAlmacenBueno + cantidadAlmacenMalo;
			desgloseDetalle = cantidadDetalleVendido + cantidadDetalleBueno + cantidadDetalleMalo;

			saldoTotalAlmacen = (cantidadAlmacenConsignado + (cantidadDetalleConsignado / unidadEmpaque)) - (desgloseAlmacen + (desgloseDetalle / unidadEmpaque));

			saldoAlmacen = (int) saldoTotalAlmacen; 
			saldoDetalle = (saldoTotalAlmacen - saldoAlmacen) * unidadEmpaque;

			if(saldoTotalAlmacen >= 0)
			{
                this.UnidadAlmacenSaldo = saldoAlmacen;
                this.UnidadDetalleSaldo = saldoDetalle;
				return true;
			}

			return false;
		}
		/// <summary>
		/// Permite definir la observación para un detalle de la boleta de venta en consignación que es devuelto.
		/// </summary>
		private void DefinirObservacion()
		{
			decimal cantidadAlmacenBueno = decimal.Zero;
			decimal cantidadDetalleBueno = decimal.Zero;
			decimal cantidadAlmacenMalo = decimal.Zero;
			decimal cantidadDetalleMalo = decimal.Zero;

			cantidadAlmacenBueno = this.UnidadAlmacenBueno;
			cantidadDetalleBueno = this.UnidadDetalleBueno;
			cantidadAlmacenMalo = this.UnidadAlmacenMalo;
			cantidadDetalleMalo = this.UnidadDetalleMalo;

			if(cantidadAlmacenBueno > 0 ||
				cantidadDetalleBueno > 0 ||
				cantidadAlmacenMalo > 0 ||
				cantidadDetalleMalo > 0)
			{
                Dictionary<string,object> parametros=new Dictionary<string,object>();
                parametros.Add("notaDetalle",this.observacionDetalle);
                this.RequestDialogNavigate<NotaDetalleDesgloseViewModel, bool>(parametros, result =>
                    {
                       this.observacionDetalle = NotaDetalleDesgloseViewModel.Observacion;
                    }
                );
                //frmNotaDetalleDesglose notaDetalle = new frmNotaDetalleDesglose(this.observacionDetalle);			
                //DialogResult result = notaDetalle.ShowDialog();						
				//notaDetalle = null;
			}
		}
		/// <summary>
		/// Agrega el desglose del detalle de la boleta de venta en consignación.
		/// </summary>
		private void AgregarDesgloseDetalle()
		{
			if(this.ItemActual != null)
			{
                if (!this.ValidarDesgloseCantidades(this.ItemActual.Articulo.UnidadEmpaque))
				{
					this.mostrarMensaje(Mensaje.Accion.Informacion,"Verifique los datos ingresados.");
					return;
				}

                this.ItemActual.AplicarDesglose(
					this.UnidadAlmacenVendido,
					this.UnidadDetalleVendido,
					this.UnidadAlmacenBueno,
					this.UnidadDetalleBueno,
					this.UnidadAlmacenMalo,
					this.UnidadDetalleMalo,
					this.Lote.Trim(),
					ref this.observacionDetalle,
					this.UnidadAlmacenSaldo,
					this.UnidadDetalleSaldo);

				//Seteamos el foco en el filtro de búsqueda por si la búsqueda es con lector de códigos de barra.
				//this.ltbArticulo.Focus();
			}
		}
		/// <summary>
		/// Método utilizado cuando el usuario desea cancelar el desglose de la boleta de venta en consignación.
		/// </summary>
		public void CancelarDesglose()
		{


            this.mostrarMensaje(Mensaje.Accion.Cancelar, "el desglose de la Boleta de Venta en Consignación", result =>
                {
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {
                        Gestor.DesglosesConsignacion.Gestionados.Clear();

                        //frmMenuPrincipal principal = new frmMenuPrincipal();
                        //principal.Show();
                        this.DoClose();
                        RequestNavigate<MenuClienteViewModel>();
                        
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                    } 
                });			
			
		}
		/// <summary>
		/// Se continua con el proceso de desglose de la boleta de venta en consignación.
		/// </summary>
		private void Continuar()
		{

			//Se verifica que se haya realizado el desglose.
			if(!Gestor.DesglosesConsignacion.RealizoDesglose())
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"No se ha realizado el desglose de la boleta.");
				return;
			}
			if(!this.desgloseSaldos)
				this.RealizarDesglose();
			else
				this.RealizarDesgloseSaldos();
		}

		private void RealizarDesglose()
		{
			try
			{
                //Se genera la devolución en caso de haberse desglosado detalles para devolver.
                Gestor.DesglosesConsignacion.GenerarDevolucion();

				//Se verifica si se debe generar la factura.
				//Se genera la factura en caso de haberse desglosado detalles vendidos.
				if(Gestor.DesglosesConsignacion.ValidarGenerarFactura())
				{		
					if(Gestor.DesglosesConsignacion.GenerarFactura())
					{
						//Se debe guardar la factura y en caso de generarse la devolución también debe ser guardada
						//en la ventana 'frmAplicarFacturaConsignacion'.
                        Dictionary<string, object> parametros = new Dictionary<string, object>();
                        parametros.Add("desgloseSaldos", "N");
                        this.DoClose();
                        RequestNavigate<AplicarFacturaConsignacionViewModel>(parametros);
						//frmAplicarFacturaConsignacion generacionFactura = new frmAplicarFacturaConsignacion(false);
						//generacionFactura.Show();
                        
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
					}
				}
				else
				{
					//Se verifica si existe saldo o no para algún detalle luego del desglose.
					if(!Gestor.DesglosesConsignacion.ExisteSaldo())
					{
                        this.mostrarMensaje(Mensaje.Accion.Decision," reabastecer producto consignado?",result=>
                            {
                                if (result == DialogResult.Yes || result == DialogResult.OK)
                                {
                                    //La devolución fue generada y debe guardarse en esa ventana 'frmSugerirBoleta'.
                                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                                    parametros.Add("existeSaldo", "S");
                                    this.DoClose();
                                    RequestNavigate<SugerirBoletaViewModel>(parametros);
                                    //frmSugerirBoleta sugerirBoelta = new frmSugerirBoleta(true);
                                    //sugerirBoelta.Show();
                                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                                }
                                else
                                    FinalizarDesglose(false);
                            }
                        );				
					}
					else
					{
						//Se debe guardar la devolución generada en la ventana 'frmSugerirBoleta'.
                        Dictionary<string, object> parametros = new Dictionary<string, object>();
                        parametros.Add("existeSaldo", "S");
                        this.DoClose();
                        RequestNavigate<SugerirBoletaViewModel>(parametros);
                        //frmSugerirBoleta sugerirBoleta= new frmSugerirBoleta(true);
                        //sugerirBoleta.Show();
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                        
					}
				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Alerta,ex.Message);
			}
		}

		private void RealizarDesgloseSaldos()
		{
			try
			{
                //Se genera la devolución en caso de haberse desglosado detalles para devolver.
                Gestor.DesglosesConsignacion.GenerarDevolucion();

				if(Gestor.DesglosesConsignacion.ValidarGenerarFactura())
				{
                    if (Gestor.DesglosesConsignacion.GenerarFactura())
					{
						//Se debe guardar la factura y en caso de generarse la devolución también debe ser guardada
						//en la ventana 'frmAplicarFacturaConsignacion'.
                        Dictionary<string, object> parametros = new Dictionary<string, object>();
                        parametros.Add("desgloseSaldos", "S");
                        this.DoClose();
                        RequestNavigate<AplicarFacturaConsignacionViewModel>(parametros);
                        //frmAplicarFacturaConsignacion generacionFactura = new frmAplicarFacturaConsignacion(true);
                        //generacionFactura.Show();
                        
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
					}
				}
				else
                    FinalizarDesglose(true);
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Alerta,ex.Message);
			}
		}

        private void FinalizarDesglose (bool existeSaldo)
        {
            Gestor.DesglosesConsignacion.FinalizarDesglose(existeSaldo,false);

            //Se muestran los códigos de las devoluciones generadas.
            foreach (Devolucion devolucion in Gestor.DesglosesConsignacion.Devoluciones.Gestionados)
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Se generó la devolución '" + devolucion.Numero + "' para la compañía '" + devolucion.Compania + "'.");

            //Se limpian las boletas facturas y devoluciones que han sido cargadas.
            Gestor.DesglosesConsignacion.LimpiarGestor();
            //Se retorna al menú pricipal en caso de error o en caso de haberse generado la devolución.
            this.DoClose();  
            RequestNavigate<MenuClienteViewModel>();
            //frmMenuPrincipal principal = new frmMenuPrincipal();
            //principal.Show();
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
              
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
