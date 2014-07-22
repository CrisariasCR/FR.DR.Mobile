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
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class AplicarVentaConsignacionViewModel : DialogViewModel<DialogResult>
    {
        public AplicarVentaConsignacionViewModel(string messageId, string invocaDesde, string ventasSugeridas)
            : base(messageId)
        {
            this.seInvocoDe=GlobalUI.RetornaFormulario(invocaDesde);
            this.ventasSugeridas=ventasSugeridas.Equals("S");
            this.cargandoFormulario = false;
            this.actualizandoDescuento = false;
            this.CargarVentana();
        }

        #region Propiedades

        public static bool ventanaInactiva = false;   

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

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

      
        private string fecha;
        public string Fecha
        {
            get { return fecha; }
            set { fecha = value;  RaisePropertyChanged("Fecha"); }
        }

        private string hora;
        public string Hora
        {
            get { return hora; }
            set { hora = value; RaisePropertyChanged("Hora"); }
        }

        private decimal totalBruto;
        public decimal TotalBruto
        {
            get { return totalBruto; }
            set { totalBruto = value; RaisePropertyChanged("TotalBruto"); }
        }

        private decimal totalNeto;
        public decimal TotalNeto
        {
            set { totalNeto = value; RaisePropertyChanged("TotalNeto"); }
            get { return totalNeto; }
        }

        public decimal Descuento
        {
            get { return Gestor.Pedidos.TotalDescuentoLineas; }
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
            set { if (!ventanaInactiva) { porcDescuento1 = value; RaisePropertyChanged("PorcDescuento1"); ModificarDescuento(EDescuento.DESC1); } }
            get { return porcDescuento1; }
        }

        private decimal porcDescuento2;
        public decimal PorcDescuento2
        {
            set { porcDescuento2 = value; RaisePropertyChanged("PorcDescuento2"); ModificarDescuento(EDescuento.DESC2); }
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
        
        public bool Desc1Enabled
        {
            get { return Pedidos.HabilitarDescuento1; }
        }

        public bool Desc2Enabled
        {
            get { return Pedidos.HabilitarDescuento2; }
        }

        private bool creditoExceedVisible = false;
        public bool CreditoExceedVisible
        {
            get { return creditoExceedVisible; }
            set { creditoExceedVisible = value; RaisePropertyChanged("CreditoExceedVisible"); }
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

        private string lblWarning;
        public string LblWarning
        {
            get { return lblWarning; }
            set { lblWarning = value; RaisePropertyChanged("LblWarning"); }
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

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(MostrarDetalles); }
        }

        public ICommand ComandoEditar
        {
            get { return new MvxRelayCommand(MostrarMontosXEmpresa); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(ValidarDocumento); }
        }

        #endregion    

        #region Variables de instancia

        #region Variables privadas
        /// <summary>
        /// Indica que se esta cargando el formulario. Se utiliza para que no se disparen eventos.
        /// </summary>
        private bool cargandoFormulario;
        /// <summary>
        /// Indica que el descuento 1 o 2 esta siendo calculado y de esta manera 
        /// no se disparen ciertos eventos.
        /// </summary>
        private bool actualizandoDescuento;
        /// <summary>
        /// Indica el formulario del cual es llamado este formulario.
        /// </summary>
        private Formulario seInvocoDe;
        /// <summary>
        /// Indica que se han sugerido ventas en consignación luego de haber realizado el desglose de boletas
        /// de venta en consignación del cliente.
        /// </summary>
        private bool ventasSugeridas;

        /// <summary>
        /// Indica el valor global del descuento por compania
        /// </summary>
        //Caso: 33192 LDA 11/02/2010
        //Agentes pueden hacer Facturas con un 100% de descuento
        private decimal descuentoPorCIA = GestorUtilitario.ParseDecimal("0");

        #endregion

        #endregion
     
		#region Métodos de instancia

		#region Métodos privados
		/// <summary>
		/// Carga los valores por defecto de la ventana.
		/// </summary>
		private void CargarVentana()
		{
            ventanaInactiva = false;
            //LJR 11/03/2009 Caso 35058, el formato de los textbox debe realizarse cuando la bandera de cargandoFormulario esta activa
            //para omitir codigo en el textchanged que modifique datos indebidamente.
            this.cargandoFormulario = true;

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNombreCliente.Visible = false;

			//Se inicializa el mensaje de alerta de que el cliente tiene excedido el crédito.
			this.LblWarning = FRmConfig.MensajeCreditoExcedido;
			
            //this.pnlImpresion.Location = new Point(0, this.pnlImpresion.Location.Y);
            //this.pnlImpresion.Hide();
            //this.pnlImpresion.SendToBack();
			try
			{
				this.MostrarFechas();
				this.MostrarMontos();
                //Caso: 33192 LDA 11/02/2010
                //Agentes pueden hacer Facturas con un 100% de descuento
                descuentoPorCIA = Gestor.Consignaciones.Gestionados[Gestor.Consignaciones.Gestionados.Count - 1].PorcentajeDescuento1;
			}
			catch(Exception exc)
			{
				this.mostrarAlerta(exc.Message);
			}

			this.cargandoFormulario = false;
		}
		/// <summary>
		/// Método encargado de obtener los datos de fecha y hora correspondientes.
		/// </summary>
		private void MostrarFechas()
		{
			this.Fecha  = GestorUtilitario.ObtenerFechaActual();
			this.Hora = GestorUtilitario.ObtenerHoraActual();

			//Obtenemos el primer encabezado.
			VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Gestionados[0];

            this.Fecha = GestorUtilitario.ObtenerFechaString(ventaConsignacion.FechaRealizacion);
			this.Hora = GestorUtilitario.ObtenerHoraString(ventaConsignacion.HoraInicio);
		}
		/// <summary>
		/// Método encargado de obtener los montos totales de la venta en consignación.
		/// </summary>
		private void MostrarMontos()
		{
			bool creditoExcedido = false;

			try
			{
				//Verificamos si hay ventas en consignación sin líneas.
				foreach(VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
					if (ventaConsignacion.Detalles.Lista.Count == 0)
						Gestor.Consignaciones.Borrar(ventaConsignacion.Compania);

				//Recalculamos los montos totales de la venta en consignación.
				creditoExcedido = Gestor.Consignaciones.SacarMontosTotales();
			}
			catch(System.Data.DataException exception)
			{
				this.mostrarAlerta(exception.Message);
			}
			catch(Exception exception)
			{
				this.mostrarAlerta(exception.Message);
			}

			if(creditoExcedido)
			{
                this.CreditoExceedVisible = true;
				
			}
			else
			{
                this.CreditoExceedVisible = false;				
			}

			Gestor.Consignaciones.SacarMontosTotales();
			//Se muestran los montos.
            this.TotalBruto = Gestor.Consignaciones.TotalBruto;
            this.PorcDescuento1 = Gestor.Consignaciones.PorcDesc1;
            this.PorcDescuento2 = Gestor.Consignaciones.PorcDesc2;
            this.Descuento1 = Gestor.Consignaciones.TotalDescuento1;
            this.Descuento2 = Gestor.Consignaciones.TotalDescuento2;
            this.ImpuestoVentas = Gestor.Consignaciones.TotalImpuesto1;
            this.Consumo = Gestor.Consignaciones.TotalImpuesto2;
            this.TotalNeto = Gestor.Consignaciones.TotalNeto;
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
            decimal desc1 = this.PorcDescuento1;
            decimal desc2 = this.PorcDescuento2;
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
		/// <param name="numeroDescuento">Número del descuento a modificar.</param>
		public void ModificarDescuento(EDescuento numeroDescuento)
		{
            if (!this.actualizandoDescuento && !this.cargandoFormulario)
            {
                this.actualizandoDescuento = true;
                try
                {
                    decimal porcentajeDesc1 = this.PorcDescuento1;
                    decimal porcentajeDesc2 = this.PorcDescuento2;

                    //Caso: 33192 LDA 10/02/2010
                    //Agentes pueden hacer Facturas con un 100% de descuento
                    //Verifica si hay algun problema con los descuentos, en cuyo caso formatea los mismos
                    if (validarDescuentos(numeroDescuento))
                    {
                        this.PorcDescuento1 = 0;
                        this.PorcDescuento2 = 0;
                        porcentajeDesc1 = 0;
                        porcentajeDesc2 = 0;
                    }

                    switch (numeroDescuento)
                    {
                        case EDescuento.DESC1:
                            this.Descuento1 = Gestor.Consignaciones.DefinirDescuento(EDescuento.DESC1, porcentajeDesc1);
                            this.Descuento2 = Gestor.Consignaciones.DefinirDescuento(EDescuento.DESC2, porcentajeDesc2);
                            break;
                        case EDescuento.DESC2:
                            this.Descuento2 = Gestor.Consignaciones.DefinirDescuento(EDescuento.DESC2, porcentajeDesc2);
                            break;
                    }

                    this.CalcularImpuestos();
                    this.MostrarMontos();
                }
                catch (Exception exc)
                {
                    this.mostrarAlerta("Error modificando descuento. " + exc.Message);
                }

                actualizandoDescuento = false;
            }
		}
		/// <summary>
		/// Calcula el impuesto de ventas y el impuesto de consumo cuando el porcentajeDesc1 o porcentajeDesc2 ha cambiado.
		/// Además recalcula los montos de las ventas en consignación gestiondas.
		/// </summary>
		private void CalcularImpuestos()
		{
			try
			{
				foreach(VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
					ventaConsignacion.CalcularImpuestos();
			}
			catch(Exception exc)
			{
				throw new Exception("Error al recalcular montos. " + exc.Message);
			}
		}
		/// <summary>
		/// Muestra los detalles de la venta en consignación gestionada.
		/// </summary>
		private void MostrarDetalles()
		{
			int contador = 0;

			foreach(VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
				contador += ventaConsignacion.Detalles.Lista.Count;

			if(contador == 0)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"No hay líneas incluídas");
				return;
			}
            Dictionary<string, object> p = new Dictionary<string, object>();
            p.Add("invocaDesde", Formulario.FinalizarConsignacion.ToString());
            if (this.ventasSugeridas)
            {
                p.Add("ventasSugeridas", "S");
            }
            else
            {
                p.Add("ventasSugeridas", "N");
            }
            p.Add("permitirModificarVentaConsignacion", "S");
            this.RequestNavigate<DetalleVentaConsignacionViewModel>(p);            
            //frmDetalleVentaConsignacion detallesVentaConsignacion = new frmDetalleVentaConsignacion(Gestor.Consignaciones,Formulario.FinalizarConsignacion,this.ventasSugeridas,true);
            //detallesVentaConsignacion.ShowDialog();
            //detallesVentaConsignacion.Close();
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

			this.MostrarMontos();
		}
		/// <summary>
		/// Muestra los montos de la venta en consignación por empresa.
		/// </summary>
		private void MostrarMontosXEmpresa()
		{
			if (Gestor.Consignaciones.Gestionados.Count == 0)
			{ 
				this.mostrarAlerta("No hay ventas en consignación gestionadas.");
				return;
			}
            this.RequestDialogNavigate<MontoConsignacionCiaViewModel, DialogResult>(null, resp =>
            {
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

                if (resp == DialogResult.OK || resp == DialogResult.Yes)
                {
                    //Limpiamos las ventas en consignación que han sido gestionadas.
                    ;
                }
            });
            //frmMontoConsignacionCia montos = new frmMontoConsignacionCia();
            //montos.ShowDialog();
            //montos.Close();
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

		}
		/// <summary>
		/// Permite regresar a la ventana de la toma de la venta en consignación.
		/// </summary>
        //private void Regresar()
        //{
        //    frmTomaVentaConsig tomaVentaConsignacion = new frmTomaVentaConsig(this.seInvocoDe, this.ventasSugeridas);
        //    tomaVentaConsignacion.Show();
        //    this.Close();
        //    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

        //}
		/// <summary>
		/// Valida si se terminá adecuadamente el documento para ser guardado.
		/// </summary>
		private void ValidarDocumento()
		{
			if (!this.ValidaCantidadDeLineas())
				return;

			if (Gestor.Consignaciones.Gestionados.Count == 0)
			{ 
				this.mostrarAlerta("No hay ventas en consignación gestionadas.");
				return;
			}

			this.MostrarMensajeTerminar();
           
		}
		/// <summary>
		/// Valida que no hayan ventas en consignación que se pasen del máximo de líneas permitidas.
		/// </summary>
		/// <returns>
		/// Si alguna venta en consignación excede el máximo de líneas se presenta un mensaje y el método retorna falso.
		/// </returns>
		private bool ValidaCantidadDeLineas()
		{
			try
			{
				foreach(VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
					if (ventaConsignacion.Detalles.Lista.Count > Pedidos.MaximoLineasDetalle)
					{
						this.mostrarAlerta("Se excede el máximo de líneas permitidas (" + Pedidos.MaximoLineasDetalle + ") para la compañía " + ventaConsignacion.Compania);
						return false;
					}
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error validando la cantidad de líneas de la venta en consignación. " + ex.Message);
				return false;
			}

			return true;
		}
		/// <summary>
		/// Valida si el usuario desea imprimir el detalle de las ventas gestionadas.
		/// </summary>
		private void VerificarImpresion(Boolean sugerirImprimir)
		{
            //caso S2A35723 LDAV 10/02/2010
            //Verificar sugerir Imprimir
            if (sugerirImprimir)
            {

                this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las ventas en consignación realizadas", resultado =>
                    {
                        if (resultado.Equals(DialogResult.Yes))
                            this.RelocalizarImpresion();
                        else
                        {
                            //Caso 32081 LDS 08/04/2008
                            this.GuardaDocumento();
                            this.LimpiarSalirFormulario();
                        }
                    }
                    );
                
            }
            //caso S2A35723 LDAV 10/02/2010
            //Verificar sugerir Imprimir
            else
            {
                //Caso 32081 LDS 08/04/2008
                this.GuardaDocumento();
                this.LimpiarSalirFormulario();
            }
		}
		/// <summary>
		/// Relocaliza los controles para la impresión de las ventas en consignación.
		/// </summary>
		private void RelocalizarImpresion()
		{
            ImpresionViewModel.OriginalEn = true;
            ImpresionViewModel.OnImprimir = ImprimirDocumento;
            this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresion Aplicar Venta Consignacion", mostrarCriterioOrden = true });

		}

        /// <summary>
        /// Permite regresar a la ventana de la toma de la venta en consignación.
        /// </summary>
        public void Regresar()
        {
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("invocaDesde", this.seInvocoDe.ToString());
            if (this.ventasSugeridas)
                parametros.Add("ventasSugeridas", "S");
            else
                parametros.Add("ventasSugeridas", "N");
            //Nos vamos a la toma de la venta en consignación
            this.DoClose();
            RequestNavigate<TomaVentaConsigViewModel>(parametros);	
        }

		//Caso 32081 LDS 08/04/2008
		/// <summary>
		/// Guarda las ventas en consignación gestionadas en la base de datos.
		/// </summary>
		private void GuardaDocumento()
		{
			try
			{
				//Guardamos las ventas en consignación.
                Gestor.Consignaciones.Guardar();
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error al generar la venta en consignación. " + ex.Message);
			}
		}
		//Caso 32081 LDS 08/04/2008
		/// <summary>
		/// Limpia las ventas en consignación gestionadas y luego regresa al formulario que realizó la
		/// invocación del presente formulario.
		/// </summary>
		private void LimpiarSalirFormulario()
		{
			Gestor.Consignaciones = new VentasConsignacion();

            //Limpiamos los degloses en memoria
            Gestor.DesglosesConsignacion.Gestionados.Clear();
            Dictionary<string, object> Parametros = new Dictionary<string, object>();
            if(this.seInvocoDe == Formulario.ConsultaConsignacion)
			{                
                Parametros.Add("tipoFormulario", Softland.ERP.FR.Mobile.UI.Formulario.ConsultaConsignacion);
                this.DoClose();
                RequestNavigate<ConsultaConsignacionViewModel>(Parametros);				
				
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
			}
			if(this.seInvocoDe == Formulario.SeleccionarCliente)
			{
                this.DoClose();
                RequestNavigate<SeleccionClienteViewModel>();
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
				
			}
			if(this.seInvocoDe == Formulario.TomaVentaConsignacion)
			{
                this.DoClose();
                RequestNavigate<MenuClienteViewModel>();				
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                
			}
		}
		/// <summary>
		/// Muestra el dialogo que pregunta al usuario si desea terminar el proceso.
		/// </summary>
		/// <returns>Indica si se desea terminar o no con la gestión de la venta en consignación.</returns>
	    private void MostrarMensajeTerminar()
		{
            this.mostrarMensaje(Mensaje.Accion.Decision," terminar la gestión de la venta en consignación", result =>
            {
                if (result == DialogResult.Yes || result == DialogResult.OK) 
                {
                    this.VerificarImpresion(Impresora.SugerirImprimir);
                }
            }
            );
		}
		/// <summary>
		/// Realiza la impresión de las ventas en consignación y actualiza el indicador de impresión de cada
		/// venta en consignación que se deba imprimir con la leyenda original.
		/// </summary>
        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
		{
            string cantidadCopiasS = cantidadCopias.ToString();

            //Caso 32081 LDS 08/04/2008
            //Primero se guarda la venta en consignación y luego se realiza la impresión de la misma.
            //El procedimiento de impresión realiza la actualización del campo impreso que indica que se 
            //realizó la impresión del Original de la venta en consignación.
            //Guardamos las ventas en consignación.
            this.GuardaDocumento();
            //Caso 32081 LDS 08/04/2008
            //Se realiza la impresión de las ventas en consignación.
            try
            {
                int cantidad = cantidadCopias;


                if (cantidad >= 0 || esOriginal)
                {
                    if (esOriginal)
                        foreach (VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
                            ventaConsignacion.LeyendaOriginal = true;
                    Gestor.Consignaciones.Cliente = GlobalUI.ClienteActual;
                    //Caso 34848, cambio en los parametros
                    Gestor.Consignaciones.ImprimeDetalles(cantidad, (DetalleSort.Ordenador)ordernarPor);

                    if (esOriginal)
                        foreach (VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
                            ventaConsignacion.Impreso = true;
                }
                else
                {
                    this.mostrarMensaje(Mensaje.Accion.Informacion, "Solo se guardará la venta en consignación.");
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
            //Caso 32081 LDS 08/04/2008
            //Primero se guarda la venta en consignación y luego se realiza la impresión de la misma.
            //El procedimiento de impresión realiza la actualización del campo impreso que indica que se 
            //realizó la impresión del Original de la venta en consignación.
            this.LimpiarSalirFormulario();
		}

        public void OnResume()
        {
            ventanaInactiva = false;
            RaisePropertyChanged("PorcDescuento1");
        }                


		#endregion

		#endregion

		
               

    }
}