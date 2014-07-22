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

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaConsignacionViewModel : ListViewModel
    {
        public static bool SeleccionCliente;

        public ConsultaConsignacionViewModel(string tipoFormulario)
        {
            //TipoPedido = (TipoPedido)tipoPedido;
            //Estados = new List<TipoConsulta>() { TipoConsulta.Activos, TipoConsulta.Anulados };
            //EstadoSeleccionado = TipoConsulta.Activos;
            //FRmConfig.EnConsulta = true;
            //Anulando = anular;
            this.seInvocoDe = this.seInvocoDe = GlobalUI.RetornaFormulario(tipoFormulario);
            this.CargarVentana();
            
        }

        #region Propiedades

        private Formulario seInvocoDe;
        public Formulario SeInvocoDe
        {
            get { return seInvocoDe; }
          
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }
        //
        private bool anularVisible;
        //BINDING
        public bool AnularVisible
        {
            get { return anularVisible; }
            set { anularVisible = value; RaisePropertyChanged("AnularVisible"); }
        }              

        private IObservableCollection<VentaConsignacion> items;
        public IObservableCollection<VentaConsignacion> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }               

        #endregion

        #region Comandos

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarConsignacion); }
        }

        public ICommand ComandoEliminar
        {
            get { return new MvxRelayCommand(EliminarConsignacion); }
        }

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }

        #endregion

        #region Acciones



        #endregion

        #region CodigoMobile
        private void CargarVentana()
		{
            //GestorUtilitario.FormatCalcEntero(ref this.txtCantidadCopias);
            //this.pnlImpresion.Location = new Point(0, this.pnlImpresion.Location.Y );
            //this.pnlImpresion.Hide();
            //this.pnlImpresion.SendToBack();            

            //this.lblNombreCliente.Text = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
            //                             " Cliente: " + GlobalUI.ClienteActual.Nombre;
            //this.lblNombreCliente.Visible = false;

			if(this.seInvocoDe == Formulario.AnulacionConsignacion)
			{
				this.anularVisible = true;				
			}
			else
			{
                //this.ibtImprimir.Location = this.ibtConsultaDetalles.Location;
                //this.ibtConsultaDetalles.Location = this.ibtAnular.Location;
                this.anularVisible = false;				
			}

			this.CargarListView();
		}
		/// <summary>
		/// Carga las ventas en consignación que posee el cliente en las compañías en las cuales se encuentra asociado.
		/// </summary>
		private void CargarListView()
		{
            List<VentaConsignacion> boletas = new List<VentaConsignacion>();
            //Se obtienen los códigos de las compañías de la boleta de venta en consignación.
			try
			{
				if(this.seInvocoDe == Formulario.AnulacionConsignacion)
				    boletas = VentaConsignacion.ObtenerBoletas(GlobalUI.ClienteActual,GlobalUI.RutaActual,true);
				else
					boletas = VentaConsignacion.ObtenerBoletas(GlobalUI.ClienteActual,GlobalUI.RutaActual,false);
			}
			catch(Exception ex)
			{
				this.mostrarAlerta(ex.Message);
			}
            Items = new SimpleObservableCollection<VentaConsignacion>(boletas);
			//El cliente solo puede poseer una boleta de venta en consignación por compañía.
			         
                
                //    //BINDING
                //    //string [] itemes = {   venta.Compania,
                //    //                       GestorUtilitario.ObtenerFechaString(venta.FechaRealizacion),
                //    //                       GestorUtilitario.FormatNumero(venta.MontoBruto),
                //    //                       venta.Configuracion.Pais.Nombre,
                //    //                       venta.Configuracion.CondicionPago.Descripcion,
                //    //                       venta.Configuracion.Nivel.Nivel,
                //    //                       venta.Configuracion.Clase.ToString(),
                //    //                       venta.Estado.ToString()};
            RaisePropertyChanged("Items");
			
		}

        private void Imprime()
        {
            this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las boletas seleccionadas", res =>
                {
                    if (res.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                    {
                        ImpresionViewModel.OnImprimir = ImprimirDocumento;
                        this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión de Consignaciones", mostrarCriterioOrden = true});
                    }
                });            

        }

        /// <summary>
        /// Busca el encabezado de consignacion y carga sus lineas
        /// para realizar la impresión.
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="numPed"></param>
        /// <returns></returns>
        private VentaConsignacion CargarDetalleVentaImprime(string compania)
        {
            foreach (VentaConsignacion venta in this.items)
            {
                if (venta.Compania == compania)
                {
                    if (venta.Detalles.Vacio())
                    {
                        //Cargamos los detalles del pedido que se imprimirá
                        venta.ObtenerDetalles();
                    }
                    return venta;
                }
            }

            throw new Exception("No se encontró la consiganción en los encabezados.");

        }

        //private void VerificarImpresion()
        //{
        //    DialogResult res = Mensaje.mostrarMensaje(Mensaje.Accion.Imprimir,"el detalle de las boletas seleccionadas");
        //    if(res.Equals(DialogResult.Yes))
        //        this.RelocalizarImpresion();			
        //}

        private List<VentaConsignacion> ItemsSeleccionados
        {
            get
            {
                return new List<VentaConsignacion>(this.Items.Where<VentaConsignacion>(x => x.Selected));
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
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Selected)
                        result.Add(i);
                }
                return result;
            }
        }

		private void ConsultarConsignacion()
		{			
			//Indica si se permite o no la modificación de la venta en consignación consultada.
			bool permitirModificarVentaConsignacion = false;
			try
			{
                if (ItemsSeleccionados.Count > 1)
                {
                    //Hay más de una venta en consignación seleccionada.
                    this.mostrarMensaje(Mensaje.Accion.Informacion, "Debe seleccionar solo una venta en consignación");
                    return; 
                }
				if (ItemsSeleccionados.Count == 0)
				{
                    this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una venta en consignación");
					return;
				}
                Gestor.Consignaciones = new VentasConsignacion();
                this.ItemsSeleccionados[0].ObtenerDetalles();
                Gestor.Consignaciones.Gestionados.Add(this.ItemsSeleccionados[0]);
                Gestor.Consignaciones.ConfigDocumentoCia.Add(this.ItemsSeleccionados[0].Compania.ToUpper(), this.ItemsSeleccionados[0].Configuracion);
                if (this.ItemsSeleccionados[0].Estado == EstadoConsignacion.NoSincronizada)
                {
                    permitirModificarVentaConsignacion = true;
                }
			}
			catch(Exception ex)
			{
				this.mostrarAlerta(ex.Message);
			}
            string permitirModificarVenta = string.Empty;
            if (permitirModificarVentaConsignacion)
            {
                permitirModificarVenta = "S";
            }
            else
            {
                permitirModificarVenta = "N"; 
            }
            Dictionary<string, object> p = new Dictionary<string, object>();            
            //p.Add("ventasConsignacion", Gestor.Consignaciones);
            p.Add("invocaDesde", this.seInvocoDe.ToString());            
			if (this.seInvocoDe == Formulario.AnulacionConsignacion)
			{
				//Se consulta la venta en consignación desde la opción de anulación.                
                p.Add("ventasSugeridas", "N");
                p.Add("permitirModificarVentaConsignacion", permitirModificarVenta);
                this.RequestDialogNavigate<DetalleVentaConsignacionViewModel, DialogResult>(p, resp =>
                    {
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

                        if (resp == DialogResult.OK || resp==DialogResult.Yes)
                        {
                            //Limpiamos las ventas en consignación que han sido gestionadas.
                            Gestor.Consignaciones = new VentasConsignacion();
                        }
                    });
			}
			else
			{
                p.Add("ventasSugeridas", "S");
                p.Add("permitirModificarVentaConsignacion", permitirModificarVenta);
				//Se consulta la venta en consignación desde la opción de consulta, por lo que se puede modificar.
                this.RequestNavigate<DetalleVentaConsignacionViewModel>(p);                                				
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

			}
		}
        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {
            if (this.ItemsSeleccionados.Count > 0)
            {
                //Limpiamos las ventas en consignación que han sido gestionadas.
                try
                {
                    Gestor.Consignaciones.Gestionados = this.ItemsSeleccionados;

                    if (Gestor.Consignaciones.Gestionados.Count == 1)
                    {
                        Gestor.Consignaciones.Gestionados[0].LeyendaOriginal = true;
                    }
                    else if (Gestor.Consignaciones.Gestionados.Count > 1)
                    {
                        foreach (VentaConsignacion ventaConsignacion in Gestor.Consignaciones.Gestionados)
                            if (!ventaConsignacion.Impreso)
                                ventaConsignacion.LeyendaOriginal = true;
                    }
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta(ex.Message);
                }

                string cantidadCopiasText = cantidadCopias.ToString();

                try
                {
                    int cantidad = cantidadCopias;                    

                    if (cantidad >= 0)
                    {
                        Gestor.Consignaciones.Cliente = GlobalUI.ClienteActual;
                        //Caso 34848, cambio en los parametros
                        Gestor.Consignaciones.ImprimeDetalles(cantidad, (DetalleSort.Ordenador)ordernarPor);
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

                //Limpiamos las ventas en consignación que han sido gestionadas.
                Gestor.Consignaciones.Gestionados.Clear();
            }
        }
		/// <summary>
		/// Elimina la venta en consignación ya que no ha sido procesada y reestablece las existencias.
		/// </summary>
		private void EliminarConsignacion()
		{
			

			//Indicador para buscar la venta en consignación a consultar.
            if (ItemsSeleccionados.Count > 1)
            {
                //Hay más de una venta en consignación seleccionada.
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Debe seleccionar solo una venta en consignación");
                return;
            }
            if (ItemsSeleccionados.Count == 0)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una venta en consignación");
                return;
            }

            this.mostrarMensaje(Mensaje.Accion.Anular, "la venta en consignación", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        VentasConsignacion ventas = new VentasConsignacion();
                        //Debemos cargar los detalles para reversar los consumos
                        if (this.ItemsSeleccionados[0].Detalles.Vacio())
                            this.ItemsSeleccionados[0].ObtenerDetalles();
                        ventas.Gestionados.Add(this.ItemsSeleccionados[0]);
                        ventas.ConfigDocumentoCia.Add(this.ItemsSeleccionados[0].Compania.ToUpper(), this.ItemsSeleccionados[0].Configuracion);

                        if (Gestor.Consignaciones.ExisteSaldo())
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "La venta en consignación posee saldos, para anularla deberá primero desglosar los detalles que poseen saldos.");
                            return;
                        }

                        try
                        {
                            ventas.Gestionados[0].EliminarBoletaNoProcesada();
                            this.CargarListView();
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta(ex.Message);
                        }
                    }
                }
                );			
		}

        public void Regresar()
        {
            
            if (this.seInvocoDe == Formulario.SeleccionarCliente)
            {
                this.DoClose();
            }
            else
            {
                Dictionary<string, object> par = new Dictionary<string, object>();
                par.Add("habilitarPedidos", true);
                this.DoClose();
                RequestNavigate<MenuClienteViewModel>(par);
            }
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
           
        }

		#endregion

        
    }
}