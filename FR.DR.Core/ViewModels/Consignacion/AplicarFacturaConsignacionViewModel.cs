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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class AplicarFacturaConsignacionViewModel : DialogViewModel<DialogResult>
    {
#pragma warning disable 414

        /// <summary>
		/// Formulario para la finalización del desglose de la boleta de venta en consignación.
		/// En este formulario se puede generar la factura resultante por el desglose de la boleta de venta en consignación.
		/// </summary>
		/// <param name="desgloseSaldos">
		/// Indica si se debe desglosar solo los artículos que poseen saldos.
		/// Los artículos poseen saldos solo cuando se ha creado una boleta de venta en consignación a partir 
		/// del desglose de una boleta de venta en consignación que quedó con saldos sin desglosar.
		/// </param>
        public AplicarFacturaConsignacionViewModel(string messageId,string desgloseSaldos)
            : base(messageId)
        {
            CargarVentana();
            this.desgloseSaldos = desgloseSaldos.Equals("S");
        }

        #region Propiedades

        #region Logica de Negocio

        private bool cargandoFormulario = true;
        private decimal descuentoPorCIA = decimal.Zero;
        private bool actualizandoDescuento = false;
        private bool desgloseSaldos;
        private bool cargando;
        public static bool ventanaInactiva = false;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #endregion

        #region Binding

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

        private string direccionSeleccionada;
        public string DireccionSeleccionada
        {
            get { return direccionSeleccionada; }
            set { direccionSeleccionada = value;RaisePropertyChanged("DireccionSeleccionada"); this.DefinirDireccionEntrega(); }
        }

        private string direccionDescripcion;
        public string DireccionDescripcion
        {
            get { return direccionDescripcion; }
            set { direccionDescripcion = value;RaisePropertyChanged("DireccionDescripcion"); }
        }

        private IObservableCollection<string> direcciones;
        public IObservableCollection<string> Direcciones
        {
            get { return direcciones; }
            set { direcciones = value; RaisePropertyChanged("Direcciones"); }
        }

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; RaisePropertyChanged("CompaniaActual"); this.CargarDireccionesEntrega(this.CompaniaActual); }
        }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private DateTime fechaEntrega;
        public DateTime FechaEntrega
        {
            get { return fechaEntrega; }
            set { fechaEntrega = value; CambioFechaEntrega(); RaisePropertyChanged("FechaEntrega"); }
        }

        private Pedido pedido;
        public Pedido Pedido
        {
            get { return pedido; }
            set { pedido = value; RaisePropertyChanged("Pedido"); }
        }

        private decimal totalBruto;
        public decimal TotalBruto
        {
            set { totalBruto = value; RaisePropertyChanged("TotalBruto"); }
            get { return totalBruto; }
        }

        private decimal totalNeto;
        public decimal TotalNeto
        {
            set { totalNeto = value; RaisePropertyChanged("TotalNeto"); }
            get { return totalNeto; }
        }

        private decimal descuento;
        public decimal Descuento
        {
            set { descuento = value; RaisePropertyChanged("Descuento"); }
            get { return descuento; }
        }


        private decimal impuestoVentas;
        public decimal ImpuestoVentas
        {
            set { impuestoVentas = value; RaisePropertyChanged("ImpuestoVentas"); }
            get { return impuestoVentas; }
        }

        private decimal consumo;
        public decimal Consumo
        {
            set { consumo = value; RaisePropertyChanged("Consumo"); }
            get { return consumo; }
        }

        private decimal porcDescuento1;
        public decimal PorcDescuento1
        {
            set { porcDescuento1 = value;RaisePropertyChanged("PorcDescuento1"); }
            get { return porcDescuento1; }
        }

        private decimal porcDescuento2;
        public decimal PorcDescuento2
        {
            set { porcDescuento2 = value;RaisePropertyChanged("PorcDescuento2"); }
            get { return porcDescuento2; }
        }

        private decimal descuento1;
        public decimal Descuento1
        {
            set { descuento1 = value; RaisePropertyChanged("Descuento1"); }
            get { return descuento1; }
        }

        private decimal descuento2;
        public decimal Descuento2
        {
            set { descuento2 = value; RaisePropertyChanged("Descuento2"); }
            get { return descuento2; }
        }

        private bool desc1Enabled;
        public bool Desc1Enabled
        {
            set { desc1Enabled = value; RaisePropertyChanged("Desc1Enabled"); }
            get { return desc1Enabled; }
        }

        private bool desc2Enabled;
        public bool Desc2Enabled
        {
            set { desc2Enabled = value; RaisePropertyChanged("Desc2Enabled"); }
            get { return desc2Enabled; }
        }

        private bool direccionVisible = false;
        public bool DireccionVisible
        {
            get { return direccionVisible; }
            set { direccionVisible = value; RaisePropertyChanged("DireccionVisible"); }
        }

        private string labelImp1;
        public string LabelImpuesto1
        {
            get { return labelImp1; }
            set { labelImp1 = value; RaisePropertyChanged("LabelImpuesto1"); }
        }

        private string labelImp2;
        public string LabelImpuesto2
        {
            get { return labelImp2; }
            set { labelImp2 = value; RaisePropertyChanged("LabelImpuesto2"); }
        }

        private string fecha;
        public string Fecha
        {
            get { return fecha; }
            set { fecha = value; RaisePropertyChanged("Fecha"); }
        }

        private string hora;
        public string Hora
        {
            get { return hora; }
            set { hora = value; RaisePropertyChanged("Hora"); }
        }

        private string labelAdvertencia;
        public string LabelAdvertencia
        {
            get { return labelAdvertencia; }
            set { labelAdvertencia = value; RaisePropertyChanged("LabelAdvertencia"); }
        }

        private bool advertenciaVisible = false;
        public bool AdvertenciaVisible
        {
            get { return advertenciaVisible; }
            set { advertenciaVisible = value; RaisePropertyChanged("AdvertenciaVisible"); }
        }


        private bool entregaEnabled;
        public bool EntregaEnabled
        {
            get { return entregaEnabled; }
            set { entregaEnabled = value; RaisePropertyChanged("EntregaEnabled"); }
        }

        #endregion

        #endregion

        #region Comandos

        public ICommand ComandoDireccion
        {
            get { return new MvxRelayCommand(MostrarDireccion); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(Consultar); }
        }

        public ICommand ComandoEditar
        {
            get { return new MvxRelayCommand(MontosEmpresa); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(ValidarDocumento); }
        }

        #endregion

        #region Mobile

        #region Métodos de instancia

		#region Métodos privados

        public void Regresar()
        {

            foreach (Pedido factura in Gestor.DesglosesConsignacion.Facturas.Gestionados)
            {
                if (!factura.Notas.Equals(string.Empty) || factura.PorcentajeDescuento1 > decimal.Zero || factura.PorcentajeDescuento2 > decimal.Zero)
                {
                    VentaConsignacion venta = Gestor.DesglosesConsignacion.Buscar(factura.Compania);
                    venta.Notas = factura.Notas;
                    venta.PorcentajeDescuento1 = factura.PorcentajeDescuento1;
                    venta.PorcentajeDescuento2 = factura.PorcentajeDescuento2;
                }
            }
            Gestor.DesglosesConsignacion.Devoluciones.Gestionados.Clear();
            Gestor.DesglosesConsignacion.Facturas.Gestionados.Clear();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            if(this.desgloseSaldos)
            {
                parametros.Add("desgloseSaldos", "S");
            }
            else
            {
                parametros.Add("desgloseSaldos", "N");
            }
            parametros.Add("validarCambioPrecios", "N");
            this.DoClose();
            RequestNavigate<DesgloseVentaConsignacionViewModel>(parametros);
            //frmDesgloseVentaConsignacion desgloseBoleta = new frmDesgloseVentaConsignacion(this.desgloseSaldos, false);
            //desgloseBoleta.Show();
            
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
        
        }
		/// <summary>
		/// Carga los valores por defecto de la ventana.
		/// </summary>
		private void CargarVentana()
		{
            ventanaInactiva = false;
            this.cargando = true;
            //LJR 11/03/2009 Caso 35058, el formato de los textbox debe realizarse cuando la bandera de cargandoFormulario esta activa
            //para omitir codigo en el textchanged que modifique datos indebidamente.            
            Pedidos.FacturarPedido = true;
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNombreCliente.Visible = false;

			//Se inicializa el mensaje de alerta de que el cliente tiene excedido el crédito.
			this.LabelAdvertencia = FRmConfig.MensajeCreditoExcedido;

			//Fecha de entrega.
			//this.FechaEntrega;			
			this.EntregaEnabled = false;


            this.TotalBruto = 0;
			this.Descuento = 0;
			this.Descuento1 = 0;
			this.Descuento2 = 0;
			this.ImpuestoVentas = 0;
			this.Consumo = 0;
			this.TotalNeto = 0;

			this.MostrarFechas();
			this.MostrarMontos();
            //Caso: 33192 LDA 12/02/2010
            //Agentes pueden hacer Facturas con un 100% de descuento
            //descuentoPorCIA = Gestor.Consignaciones.Gestionados[Gestor.Consignaciones.Gestionados.Count - 1].PorcentajeDescuento1;
            descuentoPorCIA = Gestor.DesglosesConsignacion.Facturas.PorcDesc1;

			this.Desc1Enabled = Pedidos.HabilitarDescuento1;
            this.Desc2Enabled = Pedidos.HabilitarDescuento2;

            this.cargando = false;
		}
		/// <summary>
		/// Método encargado de obtener los datos de fecha y hora correspondientes.
		/// </summary>
		private void MostrarFechas()
		{
			this.fechaEntrega = DateTime.Now;
			Pedido pedido = Gestor.DesglosesConsignacion.Facturas.Gestionados[0];
			this.Fecha = GestorUtilitario.ObtenerFechaString(pedido.FechaRealizacion);
			this.Hora = GestorUtilitario.ObtenerHoraString(pedido.HoraInicio);
		}
		/// <summary>
		/// Método encargado de obtener los montos totales de la factura generada por el desglose de la boleta de venta en consignación.
		/// </summary>
		private void MostrarMontos()
		{
			bool creditoExcedido = false;

			try
			{
                creditoExcedido = Gestor.DesglosesConsignacion.Facturas.SacarMontosTotales();
			}
			catch(System.Data.DataException exc)
			{
				this.mostrarAlerta(exc.Message);
			}
			catch(Exception exc)
			{
				this.mostrarAlerta(exc.Message);
			}

			if(creditoExcedido)
			{
				this.AdvertenciaVisible = true;				
			}

            this.TotalBruto = Gestor.DesglosesConsignacion.Facturas.TotalBruto;
            this.Descuento = Gestor.DesglosesConsignacion.Facturas.TotalDescuentoLineas;
            this.ImpuestoVentas = Gestor.DesglosesConsignacion.Facturas.TotalImpuesto1;
            this.Consumo = Gestor.DesglosesConsignacion.Facturas.TotalImpuesto2;
            this.TotalNeto = Gestor.DesglosesConsignacion.Facturas.TotalNeto;
            this.PorcDescuento1 = Gestor.DesglosesConsignacion.Facturas.PorcDesc1;
            this.PorcDescuento2 = Gestor.DesglosesConsignacion.Facturas.PorcDesc2;
            this.Descuento1 = Gestor.DesglosesConsignacion.Facturas.TotalDescuento1;
            this.Descuento2 = Gestor.DesglosesConsignacion.Facturas.TotalDescuento2;
		}

        //Caso: 33192 LDA 11/02/2010
        //Agentes pueden hacer Facturas con un 100% de descuento
        /// <summary>
        /// Valida existe algun problema con el formato de los descuentos
        /// </summary>
        /// <param name="numeroDescuento"></param>
        /// <returns>Retorna true si existe algun problema con los descuentos</returns>
        /// ltbPorcentajeDesc1
        private bool validarDescuentos(EDescuento numeroDescuento)
        {
            //Si esta en true los descuentos son formateados a 0
            Boolean limpiarDescuentos = false;
            decimal desc1 = PorcDescuento1;
            decimal desc2 = PorcDescuento2;
            if ((desc1 + desc2) > 100)
            {
                this.mostrarAlerta("La suma del porcentaje de descuento 1 y 2 no puede exceder el 100%");
                limpiarDescuentos = true;
            }
            if (EDescuento.DESC1 == numeroDescuento && Pedidos.MaximoDescuentoPermitido1 < desc1)
            {
                //Esto para el caso en que el descuento para cada cliente sea mayor al decuento
                //establecido en el Config.xml 
                if (descuentoPorCIA < Pedidos.MaximoDescuentoPermitido1)
                {
                    limpiarDescuentos = true;
                    this.mostrarAlerta("La suma máxima permitida para el Descuento 1 es de " + Pedidos.MaximoDescuentoPermitido1.ToString());
                }
                if (descuentoPorCIA > Pedidos.MaximoDescuentoPermitido1 && descuentoPorCIA < desc1)
                {
                    limpiarDescuentos = true;
                    this.mostrarAlerta("La suma máxima permitida para el Descuento 1 es de " + String.Format("{0:0.##}", descuentoPorCIA));//.ToString("00", CultureInfo.InvariantCulture));
                }
            }

            if (EDescuento.DESC2 == numeroDescuento && Pedidos.MaximoDescuentoPermitido2 < desc2)
            {
                limpiarDescuentos = true;
                this.mostrarAlerta("La suma máxima permitida para el Descuento 2 es de " + Pedidos.MaximoDescuentoPermitido2.ToString());
            }
            return limpiarDescuentos;
        }

		/// <summary>
		/// Define el porcentaje de descuento y despliega el monto correspondiente.
		/// </summary>
		/// <param name="numeroDescuento">Código del descuento.</param>
		public void ModificarDescuento(EDescuento numeroDescuento)
		{
            if (!this.actualizandoDescuento && !this.cargando)
            {
                this.actualizandoDescuento = true;

                try
                {
                    decimal descuento1 = Descuento1;
                    decimal descuento2 = Descuento2;

                    //Caso: 33192 LDA 10/02/2010
                    //Agentes pueden hacer Facturas con un 100% de descuento
                    //Verifica si hay algun problema con los descuentos, en cuyo caso formatea los mismos
                    if (validarDescuentos(numeroDescuento))
                    {
                        this.PorcDescuento1 = 0;
                        this.PorcDescuento2 = 0;
                        descuento1 = 0;
                        descuento2 = 0;
                    }

                    switch (numeroDescuento)
                    {
                        case EDescuento.DESC1:
                            this.Descuento1 =
                                Gestor.DesglosesConsignacion.Facturas.DefinirDescuento(EDescuento.DESC1, descuento1);
                            this.Descuento2 =
                                Gestor.DesglosesConsignacion.Facturas.DefinirDescuento(EDescuento.DESC2, descuento2);
                            break;
                        case EDescuento.DESC2:
                            this.Descuento2 =
                                Gestor.DesglosesConsignacion.Facturas.DefinirDescuento(EDescuento.DESC2, descuento2);
                            break;
                    }

                    this.CalcularImpuestos();
                    this.MostrarMontos();
                }
                catch (Exception exc)
                {
                    this.mostrarAlerta("Error modificando descuento. " + exc.Message);
                }

                this.actualizandoDescuento = false;
            }
		}
		/// <summary>
		/// Recalcula el impuesto de ventas y el impuesto de consumo cuando el descuento1 o el descuento2 ha cambiado.
		/// </summary>
		private void CalcularImpuestos()
		{
			try
			{
                foreach (VentaConsignacion venta in Gestor.DesglosesConsignacion.Gestionados)
                    venta.CalcularImpuestos();
                foreach (Pedido pedido in Gestor.DesglosesConsignacion.Facturas.Gestionados)
					pedido.RecalcularImpuestos();
			}
			catch(Exception exc)
			{
				throw new Exception("Error al recalcular montos. " + exc.Message);
			}
		}
		/// <summary>
		/// Carga el combo de las compañías con la lista de las compañías con las que trabaja el cliente.
		/// </summary>
        private void CargarCompanias()
        {
            List<string> lista = new List<string>();
            if (Companias.Count == 0)
            {
                //ABC 22/10/2008
                //Caso: 33706 Reflejo Multicompañia FR 5.0 a FR 6.0
                this.Companias.Clear();

                foreach (Pedido pedido in Gestor.DesglosesConsignacion.Facturas.Gestionados)
                    lista.Add(pedido.Compania);
                Companias = new SimpleObservableCollection<string>(lista);
                if (Companias.Count > 0)
                {
                    this.CompaniaActual = Companias[0];
                }                
            }
        }
		/// <summary>
		/// Carga las direcciones de embarque del cliente para la compañía selecciona.
		/// </summary>
		/// <param name="codigoCompania">Código de la compañía de la factura.</param>
		private void CargarDireccionesEntrega(string cia)
		{
            List<string> lista = new List<string>();
            try
            {
                
                try
                {
                    GlobalUI.ClienteActual.ObtenerDireccionesEntrega(cia);
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando direccion de entrega del cliente. " + ex.Message);
                }

                ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(cia);
                int cont = 0;
                int selectedIndex = 0;
                Pedido pedido = Gestor.DesglosesConsignacion.Facturas.Gestionados[0];

                foreach (DireccionEntrega dir in cliente.DireccionesEntrega)
                {
                    lista.Add(dir.Codigo);

                    if (dir.Codigo.Equals(cliente.DireccionEntregaDefault))
                    {
                        if (pedido.DireccionEntrega.Equals(cliente.DireccionEntregaDefault))
                            selectedIndex = cont;
                    }
                    else if (pedido.DireccionEntrega.Equals(dir.Codigo))
                            selectedIndex = cont;

                    cont++;
                }

                Direcciones = new SimpleObservableCollection<string>(lista);

                if (this.Direcciones.Count == 1)
                {
                    this.DireccionSeleccionada = Direcciones[0];                    
                }
                else
                {
                    this.DireccionSeleccionada = Direcciones[selectedIndex];                    
                }
            }
            catch (System.Exception ex)
            {
                this.mostrarAlerta("Error al cargar las direcciones de embarque" + ex.Message);
            }
        }
		/// <summary>
		/// Define la dirección de entrega.
		/// </summary>
		private void DefinirDireccionEntrega()
		{
			try
			{
                if (!string.IsNullOrEmpty(CompaniaActual))
				{
					DireccionEntrega dir =
                        GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual).DireccionesEntrega.Find(x=>x.Codigo==DireccionSeleccionada);

                    this.DireccionDescripcion = dir.Descripcion;
                    Gestor.DesglosesConsignacion.Facturas.DefineDirEntregaPedido(CompaniaActual, dir);                    
				}
			}
			catch(System.Exception ex)
			{
				this.mostrarAlerta("Error al definir la dirección de embarque"+ex.Message);
			}
		}
		/// <summary>
		/// Valida si se terminá adecuadamente el documento para ser guardado.
		/// </summary>
		private void ValidarDocumento()
		{
			if (!this.ValidaCantidadDeLineas())
				return;

			if (!this.ValidaDireccionDefinida())
				return;

			if (Gestor.DesglosesConsignacion.Facturas.Gestionados.Count == 0)
			{ 
				this.mostrarAlerta("No hay facturas gestionadas");
				return;
			}

			bool result = this.MostrarMensajeTerminar();
						
			if(!result)
				return;
			else
			{
				this.GuardaDocumento();
			}
		}
		/// <summary>
		/// Valida que no hayan facturas que se pasen del máximo de líneas permitidas.
		/// </summary>
		/// <returns>Retorna falso en caso de que se exceda la cantidad de líneas.</returns>
		private bool ValidaCantidadDeLineas()
		{
			try
			{
				foreach(Pedido pedido in Gestor.DesglosesConsignacion.Facturas.Gestionados)
					if (pedido.Detalles.Lista.Count > Pedidos.MaximoLineasDetalle)
					{
						this.mostrarAlerta("Se excede el máximo de líneas permitidas '" + Pedidos.MaximoLineasDetalle + "' para la compañía '" + pedido.Compania + "'.");
						return false;
					}
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error validando la cantidad de líneas de la factura. " + ex.Message);
				return false;
			}

			return true;
		}
		/// <summary>
		/// Valida que se haya seleccionado una dirección de entrega para la factura.
		/// </summary>
		/// <returns>Retorna falso en caso de que no se haya definido la dirección de entrega.</returns>
		private bool ValidaDireccionDefinida()
		{
			string dirRow = string.Empty;
			
			try
			{ 
				foreach(Pedido pedido in Gestor.DesglosesConsignacion.Facturas.Gestionados)
					if (pedido.DireccionEntrega == "")
					{
						this.mostrarAlerta("Debe definir la dirección de entrega para la compañía '" + pedido.Compania + "'.");
						return false;
					}
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error validando dirección de entrega. " + ex.Message);
				return false;
			}

			return true;
		}
		/// <summary>
		/// Muestra el dialog que pregunta al usuario si desea terminar el proceso.
		/// </summary>
		private bool MostrarMensajeTerminar()
		{
			string mensaje = "";
            bool result = false;
			
			try
			{
                if (GlobalUI.ClienteActual.TieneFacturasVencidas)
					mensaje = FRmConfig.MensajeFacturasVencidas;
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error verificando si el cliente tiene facturas vencidas. " + ex.Message);
			}

			if (mensaje != "")
				mensaje += ". ";
			
			mensaje += " terminar la gestión de la factura";
            this.mostrarMensaje(Mensaje.Accion.Decision, mensaje, res =>
                {
                    if (res.Equals(DialogResult.OK) || res.Equals(DialogResult.Yes)) 
                    {
                        result = true;
                    }                    
                }
            );
            return result;
		}
		/// <summary>
		/// Aplica la o las facturas a la base de datos.
		/// </summary>
		private void GuardaDocumento()
		{
			List<Articulo> articulosDetalles = new List<Articulo>();

            //LDA CR1-04245-MVYY
            //Se elimina la verificacion de las existencias de la bonificaciones puesto
            //que no se soportaran bonificaciones en las ventas en cosignacion
            //articulosDetalles = Gestor.DesglosesConsignacion.ExistenciasBonificadosInvalidas();            
            //if (articulosDetalles.Count>0)
			//{
			//	foreach(Articulo articulo in articulosDetalles)
			//	{
			//		detalleFactura = Gestor.DesglosesConsignacion.Facturas.BuscarDetalle(articulo);
			//		Mensaje.mostrarMensaje(Mensaje.Accion.Alerta,"Debe excluir el artículo bonificado '" + detalleFactura.LineaBonificada.Articulo.Codigo + "' por el artículo '" + detalleFactura.Articulo.Codigo + "' en la factura de la compañía '" + detalleFactura.Articulo.Compania + "'.");
			//	}
			//	return;
			//}
            //Se eliminan los articulos bonificados del documento
            Gestor.DesglosesConsignacion.EliminarBonificados();
			if(!this.desgloseSaldos)
				this.GuardarDocumentos();
			else
				this.GuardarDocumentosDesgloseSaldos();
		}

		private void GuardarDocumentos()
		{
			bool existeSaldo;

			try
			{
                if (!(Gestor.DesglosesConsignacion.Facturas.Gestionados.Count > 0))
					throw new Exception("No se han gestionado facturas.");

                existeSaldo = Gestor.DesglosesConsignacion.ExisteSaldo();

				if(!existeSaldo)
				{
                    this.mostrarMensaje(Mensaje.Accion.Informacion,"¿Desea reabastecer producto consignado?",result=>
                        {
                            if (result == DialogResult.Yes || result == DialogResult.OK)
                            {
                                Dictionary<string, object> parametros = new Dictionary<string, object>();
                                if (existeSaldo)
                                {
                                    parametros.Add("existeSaldo", "S");
                                }
                                else
                                {
                                    parametros.Add("existeSaldo", "N");
                                }
                                //La factura y/o la devolución fue generada y debe guardarse en esa ventana 'frmSugerirBoleta'.
                                RequestNavigate<SugerirBoletaViewModel>(parametros);
                                //frmSugerirBoleta sugerirBoleta = new frmSugerirBoleta(existeSaldo);
                                //sugerirBoleta.Show();
                                this.DoClose();
                                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

                            }
                            else
                            {
                                Gestor.DesglosesConsignacion.FinalizarDesglose(false, false);
                                MostrarMensajesFinalizacion(this);
                                Gestor.DesglosesConsignacion.LimpiarGestor();
                                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                                {
                                    {"habilitarPedidos", true}
                                };
                                this.RequestNavigate<MenuClienteViewModel>(Parametros);
                                //frmMenuPrincipal principal = new frmMenuPrincipal();
                                //principal.Show();
                                this.DoClose();
                            }
                        }
                    );
					
				}
				else
				{
					//Se debe guardar en base de datos la factura generada y/o la devolución generada en la ventana 'frmSugerirBoleta'.
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    if (existeSaldo)
                    {
                        parametros.Add("existeSaldo", "S");
                    }
                    else
                    {
                        parametros.Add("existeSaldo", "N");
                    }                    
                    RequestNavigate<SugerirBoletaViewModel>(parametros);                    
                    this.DoClose();
                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

				}
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Alerta,ex.Message);
			}
		}

        public static void MostrarMensajesFinalizacion(BaseViewModel ViewModel )
        {
            //Se muestran los códigos de las devoluciones y las facturas guardadas.
            foreach (Devolucion devolucion in Gestor.DesglosesConsignacion.Devoluciones.Gestionados)
                ViewModel.mostrarMensaje(Mensaje.Accion.Informacion, "Se generó la devolución '" + devolucion.Numero + "' para la compañía '" + devolucion.Compania + "'.");

            foreach (Pedido factura in Gestor.DesglosesConsignacion.Facturas.Gestionados)
                ViewModel.mostrarMensaje(Mensaje.Accion.Informacion, "Se generó la factura '" + factura.Numero + "' para la compañía '" + factura.Compania + "'.");

            if (Gestor.DesglosesConsignacion.Facturas.Gestionados.Count > 0)
                Pedidos.FacturarPedido = false;     
        }

		private void GuardarDocumentosDesgloseSaldos()
		{
			try
			{
				if(!(Gestor.DesglosesConsignacion.Facturas.Gestionados.Count > 0))
					throw new Exception("No se han gestionado facturas.");

                Gestor.DesglosesConsignacion.FinalizarDesglose(true,false);
                MostrarMensajesFinalizacion(this);
                Gestor.DesglosesConsignacion.LimpiarGestor();
                RequestNavigate<MenuClienteViewModel>();
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                this.DoClose();
			}
			catch(Exception ex)
			{
				this.mostrarMensaje(Mensaje.Accion.Alerta,ex.Message);
			}
		}

        private void Consultar() 
        {
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            Parametros.Add("invocadesde", Formulario.FacturaConsignacion.ToString());
            Parametros.Add("mostrarControles", "N");
            Parametros.Add("esFactura", "S");
        
            this.RequestNavigate<DetallePedidoViewModel>(Parametros);
            //RequestNavigate<DetallePedidoViewModel>();
            //frmDetallePedido factura = new frmDetallePedido(Formulario.FacturaConsignacion, Gestor.DesglosesConsignacion.Facturas);
            //factura.ShowDialog();
            //factura.Close();
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes            
        }

        private void MostrarDireccion() 
        {
            this.CargarCompanias();
            //this.pnlDireccionEntrega.Show();
            if (DireccionVisible)
            {
                DireccionVisible = false;
            }
            else
            {
                DireccionVisible = true;
            }
            
            //this.pnlDireccionEntrega.BringToFront();            
        }

        private void MontosEmpresa() 
        {   
            Dictionary<string,object> parametros=new Dictionary<string,object>();
            parametros.Add("facturaConsignacion",true);
            RequestNavigate<MontosPedidoViewModel>(parametros);
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
        }

        private void CambioFechaEntrega()
        {
                foreach (Pedido pedido in Gestor.DesglosesConsignacion.Facturas.Gestionados)
                    pedido.FechaEntrega = this.FechaEntrega;
        }

        public void OnResume()
        {
            ventanaInactiva = false;
            RaisePropertyChanged("PorcDescuento1");
        }   
		
        #endregion

		#endregion   

        #endregion

    }
}