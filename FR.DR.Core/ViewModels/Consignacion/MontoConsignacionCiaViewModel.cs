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

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MontoConsignacionCiaViewModel : DialogViewModel<DialogResult>
    {
        public MontoConsignacionCiaViewModel(string messageId)
            : base(messageId)
        {
            CargarVentana();
        }

        #region Comandos

        public ICommand ComandoNotas
        {
            get { return new MvxRelayCommand(EditarNotas); }
        }

        public void EditarNotas() 
        {
            if (!string.IsNullOrEmpty(CompaniaActual))
            {
                //frmNotaVentaConsignacion nota = new frmNotaVentaConsignacion(this.cboCompannias.SelectedItem.ToString());
                //nota.ShowDialog();
                //nota.Close();
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
            }
        }

        #endregion

        #region Propiedades



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

        private decimal descuento;
        public decimal Descuento
        {
            set { descuento = value; RaisePropertyChanged("Descuento"); }
            get { return descuento; }
        }

        private string credito;
        public string Credito
        {
            set { credito = value; RaisePropertyChanged("Credito"); }
            get { return credito; }
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

        private string nivelPrecio;
        public string NivelPrecio
        {
            get { return nivelPrecio; }
            set { nivelPrecio = value; RaisePropertyChanged("NivelPrecio"); }
        }

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; RaisePropertyChanged("CompaniaActual"); MostrarMontos(); }
        }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        #endregion

        #endregion

        #region mobile
        

		#region Métodos de instancia

		#region Métodos privados
		/// <summary>
		/// Carga los valores por defecto de la ventana.
		/// </summary>
		private void CargarVentana()
		{
            //FormatTextBox();
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                         " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNombreCliente.Visible = false;			
            Companias = new SimpleObservableCollection<string>(Util.CargarCiasConsignacion(Gestor.Consignaciones.Gestionados, EstadoConsignacion.NoDefinido));
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }            
		}

		/// <summary>
		/// Obtiene los montos totales de la venta en consignación para la compañía seleccionada.
		/// </summary>
		private void MostrarMontos()
		{
			string companiaSeleccionada = CompaniaActual;

			VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(companiaSeleccionada);

			decimal montoBruto = ventaConsignacion.MontoBruto;
			decimal montoDescuento = ventaConsignacion.MontoTotalDescuento;
			decimal montoImpuestoVenta = ventaConsignacion.Impuesto.MontoImpuesto1;
            decimal montoImpuestoConsumo = ventaConsignacion.Impuesto.MontoImpuesto2;
			decimal montoNeto = ventaConsignacion.MontoNeto;

			if (ventaConsignacion.Configuracion.ClienteCia.LimiteCredito > 0)
			{
				
                this.Credito = ventaConsignacion.Configuracion.ClienteCia.LimiteCredito.ToString(); 
			}
			else
			{				
                this.Credito = "NO"; 
			}
			//ABC Caso 34622 20-01-2009
            if (ventaConsignacion.Configuracion.ClienteCia.LimiteCredito > 0 && montoNeto > ventaConsignacion.Configuracion.ClienteCia.LimiteCredito)
			{
				//Se excedió el crédito para la compañía específica.
				this.LblWarning = FRmConfig.MensajeCreditoExcedido;
				this.CreditoExceedVisible = true;
			}
			else
			{
                this.CreditoExceedVisible = false; 
			}

			this.NivelPrecio = ventaConsignacion.Configuracion.Nivel.Nivel;
            this.TotalBruto = montoBruto;
            this.Descuento = montoDescuento;
			this.ImpuestoVentas = montoImpuestoVenta;
			this.Consumo = montoImpuestoConsumo;
			this.TotalNeto = montoNeto;
		}
		#endregion

		#endregion

		#region Eventos de los controles visuales
        //private void picLogo_MouseDown(object sender, MouseEventArgs e)
        //{
        //    this.lblNombreCliente.Show();
        //    this.lblNombreCliente.BringToFront();
        //}
        //private void picLogo_MouseUp(object sender, MouseEventArgs e)
        //{
        //    this.lblNombreCliente.SendToBack();
        //    this.lblNombreCliente.Hide();
        //}
		#endregion

        #endregion


    }
}