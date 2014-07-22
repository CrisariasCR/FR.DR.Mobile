using System;
//using System.Net;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls; // FrmConfig
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI; //GlobalUI
using Softland.ERP.FR.Mobile.ViewModels;
using FR.DR.Core.Helper;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaEfectivoViewModel : DialogViewModel<bool>
    {
        public TomaEfectivoViewModel(string messageid)
            : base(messageid)
        {
            CargaInicial();
        }

        #region Propiedades        

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set { nombreCliente = value; this.RaisePropertyChanged("NombreCliente"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private string moneda;
        public string Moneda
        {
            get { return moneda; }
            set { moneda = value; this.RaisePropertyChanged("Moneda"); }
        }

        private decimal salPag;
        public decimal SalPag
        {
            get { return salPag; }
            set { salPag = value; this.RaisePropertyChanged("SalPag"); }
        }

        private decimal totPagar;
        public decimal TotPagar
        {
            get { return totPagar; }
            set { totPagar = value; this.RaisePropertyChanged("TotPagar"); }
        }

        private decimal montoEfect;
        public decimal MontoEfect
        {
            get { return montoEfect; }
            set {
                bool cambiar = montoEfect != value;
                montoEfect = value; this.RaisePropertyChanged("MontoEfect");
                if (cambiar)
                {
                    CambioTexto(MontoEfect);
                }
            }
        }
        

        #endregion

        #region mobile

        #region funciones usadas por los eventos

        /// <summary>
        /// Carga inicial de la ventana
        /// </summary>
        private void CargaInicial()
        {            
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
            //this.lblNomClt.Visible = false;
            this.Moneda = Cobros.TipoMoneda.ToString();

            this.MontoEfect = 0;

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                this.TotPagar = Cobros.MontoFacturasLocal;
            else
                this.TotPagar = Cobros.MontoFacturasDolar;

            this.SalPag = Cobros.MontoPendiente;        
        }

		/// <summary>
		/// muestra la pantalla de aplicar pago
		/// </summary>
		private void acepta()
		{
            Cobros.MontoEfectivo += this.MontoEfect;			
            ReturnResult(true);
            //this.RequestDialogNavigate<AplicarPagoViewModel, bool>(null, res =>
            //{
            //});
            //this.DoClose();
            
		}
		/// <summary>
		/// metodo que cancela lo que se haya realizado en la pantalla y
		/// muestra la pantalla de aplicar pago
		/// </summary>
		public void cancela()
		{

            this.mostrarMensaje(Mensaje.Accion.Decision, "Si cancela la información se perderá ¿Esta seguro que desea cancelar?", result =>
                {
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {
                        Cobros.MontoEfectivo = 0;
                        //base.DoClose();
                        ReturnResult(false);
                        //this.RequestDialogNavigate<AplicarPagoViewModel, bool>(null, res =>
                        //    {
                        //    });
                    }
                }
            );

		}

		/// <summary>
		/// Este evento se da cuando se cambia el texto en el control,
		/// asi mismo se calcula el monto pendiente con el monto en efectivo que se 
		/// digito.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void CambioTexto(decimal monto)
		{
            decimal montoEfectivo = monto;
			
			decimal montoPendiente = (decimal)Math.Round((decimal)Cobros.MontoPendiente,FRmConfig.CantidadDecimales);

            if (montoEfectivo > montoPendiente && Cobros.MontoFacturasLocal != 0)
			{
                montoEfectivo -= montoEfectivo;

				this.mostrarMensaje(Mensaje.Accion.Informacion,"El monto ingresado es mayor al pendiente.");
			}
			else
			{
                decimal montPend = Cobros.MontoPendiente - montoEfectivo; 
				this.SalPag = montPend;
			}
            MontoEfect = montoEfectivo;
		}
		
		#endregion

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(cancela); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(acepta); }
        }

        #endregion

                

        #endregion

    }
}
