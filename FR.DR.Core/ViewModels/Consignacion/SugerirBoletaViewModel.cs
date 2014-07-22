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
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using FR.Core.Model;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using System.Windows.Forms;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SugerirBoletaViewModel : BaseViewModel
    {
        #region Constructor

        public SugerirBoletaViewModel(string existeSaldo)
        {
            this.existeSaldo = existeSaldo.Equals("S");
            this.CargarVentana();
        }

        #endregion

        #region Propiedades

        

        private bool bAnterior=true;
        public bool BAnterior
        {
            get { return bAnterior; }
            set { bAnterior = value; RaisePropertyChanged("BAnterior");}
        }

        private bool bSoloSaldos;
        public bool BsoloSaldos
        {
            get { return bSoloSaldos; }
            set { bSoloSaldos = value; RaisePropertyChanged("BsoloSaldos");}
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private bool bNinguno;
        public bool BNinguno
        {
            get { return bNinguno; }
            set { bNinguno = value; RaisePropertyChanged("BNinguno");}
        }

        private bool bAnteriorVisible;
        public bool BAnteriorVisible
        {
            get { return bAnteriorVisible; }
            set { bAnteriorVisible = value; RaisePropertyChanged("BAnteriorVisible"); }
        }

        private bool bSoloSaldosVisible;
        public bool BsoloSaldosVisible
        {
            get { return bSoloSaldosVisible; }
            set { bSoloSaldosVisible = value; RaisePropertyChanged("BsoloSaldosVisible"); }
        }

        private bool bNingunoVisible;
        public bool BNingunoVisible
        {
            get { return bNingunoVisible; }
            set { bNingunoVisible = value; RaisePropertyChanged("BNingunoVisible"); }
        }



        private bool existeSaldo{get;set;}

        #endregion

        #region Comandos

        public ICommand ComandoInicializar
        {
            get { return new MvxRelayCommand(this.Inicializar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(this.CancelarDesglose); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(this.Continuar); }
        }

       

        #endregion Comandos

		#region Métodos de instancia
		/// <summary>
		/// Carga la ventana.
		/// </summary>
		private void CargarVentana()
		{
			//Inicializa los controles de la pantalla.
			this.Inicializar();
		}
		/// <summary>
		/// Inicializa los controles de la pantalla.
		/// </summary>
		private void Inicializar()
		{			
            BAnteriorVisible = true;

			if(this.existeSaldo)
			{
                BsoloSaldosVisible = true;				
				//this.rbtSoloSaldos.Location = new System.Drawing.Point(8, 45);
                BNingunoVisible = false;				
				//this.rbtNoSugerir.Location = new System.Drawing.Point(8, 70);
			}
			else
			{
                BsoloSaldosVisible = false;			
				//this.rbtSoloSaldos.Location = new System.Drawing.Point(8, 70);
                BNingunoVisible = true;				
                
			}
            BAnterior = false;
            BsoloSaldos = false;
            BNinguno = false;
		}
		/// <summary>
		/// Permite cancelar el desglose realizado así como los documentos gestionados y creados.
		/// </summary>
		private void CancelarDesglose()
		{
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "el desglose", result =>
                {
                    //Si la confirmación es positiva se cancela todo los procesos y se despliega la pantalla de menú priincipal.
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {                        
                        this.DeshacerDesglose();
                        Gestor.DesglosesConsignacion.Gestionados.Clear();
                        Dictionary<string, object> Parametros = new Dictionary<string, object>()
                        {
                            {"habilitarPedidos", true}
                        };
                        this.RequestNavigate<MenuClienteViewModel>(Parametros);                       
                        this.DoClose();
                        //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                        
                    }
                });			
		}
		/// <summary>
		/// Se maneja el estado de la boleta de venta en consignación y los documentos generados.
		/// </summary>
		private void Continuar()
		{
            try
            {
                if (!this.BAnterior && !this.BsoloSaldos && !this.BNinguno)
                    return;

                Gestor.DesglosesConsignacion.FinalizarDesglose(this.existeSaldo,this.BAnterior);
                AplicarFacturaConsignacionViewModel.MostrarMensajesFinalizacion(this);
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, ex.Message);
            }
            try
            {
                //Se debe gestionar las nuevas ventas en consignación sugeridas. (Mantener o Solo Saldos)
                //El nuevo precio gestionado sera el precio actual de los articulos
                if (!this.BNinguno)
                {
                    //1. Si se va a mantener entonces la nueva boleta quedara igual a la anterior, pero modificando las existencias de manera que haya disponible para entregar producto
                    //2. El otro caso solo saldos, la genera apartir de los documentos con SALDOS PENDIENTES
                    if (this.BAnterior)
                    {
                        //Se realizará la boleta de venta en consignación con las existencias que se encuentran en el camión,
                        //no se podrá reabastecer por completo.
                        //Ya los documentos hay sido creados en la base de datos y se ha eliminado la boleta, se regenera la boleta con los cambios de precios y fechas y nuevas cantidades segun disponibilidad.
                        //Se muestran los detalles que no fueron incluidos o que no poseían suficientes existencias.
                        foreach (DetalleVenta detalle in Gestor.DesglosesConsignacion.VerificarExistenciasDetalles(GlobalUI.RutaActual.Bodega))
                        {
                            if (detalle.TotalAlmacenExistencia == decimal.Zero)
                                this.mostrarMensaje(Mensaje.Accion.Informacion, "El detalle '" + detalle.Articulo.Codigo + "' en la boleta de la compañía '" + detalle.Articulo.Compania + "' no podrá ser reabastecido ya que no posee existencias.");
                            else
                                this.mostrarMensaje(Mensaje.Accion.Informacion, "El detalle '" + detalle.Articulo.Codigo + "' en la boleta de la compañía '" + detalle.Articulo.Compania + "' no cuenta con suficientes existencias, se asignó '" + detalle.TotalAlmacenExistencia.ToString() + "' unidades de almacén.");
                        }
                    }

                    //Ya los documentos hay sido creados en la base de datos y se ha eliminado la boleta, se debe gestionar las nuevas boletas de venta en consignación sugerida con los nuevos precios y fechas de generación.
                    Gestor.Consignaciones = Gestor.DesglosesConsignacion.GestionarVentaConsignacionSugerida(this.BsoloSaldos);

                    // Se cargan los saldos después del desglose
                    this.CargarSaldosDesglose();
                    
                    //Se eliminan las facturas y/o devoluciones gestionadas y las boletas de venta en consignación que han sido cargadas.
                    this.DeshacerDesglose();
                    //Nos vamos a la ventana que muestra los detalles de la venta en consignación.
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("invocaDesde", Formulario.SugerirBoleta.ToString());
                    parametros.Add("ventasSugeridas", "S");
                    parametros.Add("permitirModificarVentaConsignacion", "S");
                    RequestNavigate<DetalleVentaConsignacionViewModel>(parametros);
                    //frmDetalleVentaConsignacion ventaConsignacion = new frmDetalleVentaConsignacion(Gestor.Consignaciones, Formulario.SugerirBoleta, true, true);
                    //ventaConsignacion.Show();
                    this.DoClose();
                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
                }
                else
                {
                    //Se eliminan las facturas y/o devoluciones gestionadas y las boletas de venta en consignación que han sido cargadas.
                    this.DeshacerDesglose();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("invocaDesde", Formulario.SugerirBoleta.ToString());
                    parametros.Add("ventasSugeridas", "S");                    
                    RequestNavigate<DetalleVentaConsignacionViewModel>(parametros);
                    //Nos vamos a la ventana que muestra los detalles de la venta en consignación.
                    //LJR Caso 34988 Si no hay sugerido pasar a una nueva toma de venta en consig
                    RequestNavigate<TomaVentaConsigViewModel>(parametros);
                    //frmTomaVentaConsig ventaConsignacion = new frmTomaVentaConsig(Formulario.SugerirBoleta, false);
                    //ventaConsignacion.Show();
                    this.DoClose();
                    //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes

                }
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, ex.Message);
            }
		}
		/// <summary>
		/// Deshace el desglose realizado incluyendo los documentos generados.
		/// </summary>
		private void DeshacerDesglose()
		{
			if(Gestor.DesglosesConsignacion.Facturas.Gestionados.Count > 0)
				Pedidos.FacturarPedido = false;

            Gestor.DesglosesConsignacion.Devoluciones.Gestionados.Clear();
            Gestor.DesglosesConsignacion.Facturas.Gestionados.Clear();
		}

        
        /// <summary>
        /// Escribe el valor de las propiedades SaldoDeglose,
        /// dependiendo de la acción (Mantener Boleta Anterior ó Solo Saldos)
        /// </summary>
        private void CargarSaldosDesglose()
        {
            foreach (VentaConsignacion venta in Gestor.Consignaciones.Gestionados)
            {
                foreach (DetalleVenta detalle in venta.Detalles.Lista)
                {
                    detalle.SaldoDesgloseDetalle = detalle.UnidadesDetalleSaldo;
                    detalle.SaldoDesgloseAlmacen = detalle.UnidadesAlmacenSaldo;
                    if (detalle.UnidadesAlmacenSaldo < 0 || detalle.UnidadesDetalleSaldo < 0)
                    {
                        detalle.SaldoDesgloseAlmacen = detalle.UnidadesAlmacen;
                        detalle.SaldoDesgloseDetalle = detalle.UnidadesDetalle;
                    }
                }
            }
        }

        public void DoNothing()
        {
            return;
        }

        
        #endregion


    }
}