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
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.Utilidad;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TipoCambioViewModel : DialogViewModel<bool>
    {

        #region Propiedades

        private decimal tipoCambio;
        public decimal TipoCambio 
        {
            get { return tipoCambio;}
            set { tipoCambio = value;this.RaisePropertyChanged("TipoCambio"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private decimal totalPagar;
        public decimal TotalPagar
        {
            get { return totalPagar; }
            set { totalPagar = value;this.RaisePropertyChanged("TotalPagar"); }
        }

        private decimal total;
        public decimal Total
        {
            get { return total; }
            set { total = value; this.RaisePropertyChanged("Total"); }
        }

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set { nombreCliente = value; this.RaisePropertyChanged("NombreCliente"); }
        }

        private string tipoModena;
        public string TipoModena
        {
            get { return tipoModena; }
            set { tipoModena = value; this.RaisePropertyChanged("TipoModena"); }
        }

        private string lbltotal;
        public string lblTotal
        {
            get { return lbltotal; }
            set { lbltotal = value; this.RaisePropertyChanged("lblTotal"); }
        }

        private bool calculando;

        #endregion

        public TipoCambioViewModel(string messageid)
            : base(messageid)
        {
            CargaInicial();
        }

        #region mobile

        #region Metodos

        private void ModificarTipoCambio()
        {
            if (calculando)
                return;

            calculando = true;

            decimal monto = 0;
            decimal tipoCambio = TipoCambio;
            decimal pagar = TotalPagar;

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                monto = pagar / tipoCambio;
            else
                monto = pagar * tipoCambio;

            this.Total = monto;

            calculando = false;

        }

        //private void ModificarMonto()
        //{
        //    if (calculando)
        //        return;

        //    calculando = true;

        //    decimal tipoCambio = 0;
        //    decimal monto = GestorUtilitario.ParseDecimal(this.txtExTotalCambio.Text);
        //    decimal pagar = GestorUtilitario.ParseDecimal(this.txtExtotpagar.Text);

        //    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
        //        tipoCambio = pagar / monto;
        //    else
        //        tipoCambio = monto / pagar;

        //    this.txtexTipCambio.Value = tipoCambio;

        //    calculando = false;
        //}

        private void CargaInicial()
        {
            calculando = true;
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
            //this.lblNomClt.Visible = false;
            //LJR 09/06/2009 Caso 35844: Formato de campos
            this.TotalPagar = Cobros.MontoSumaChequeEfectivo;
            this.TipoCambio = Cobros.TipoCambio;
            this.TipoModena = Cobros.TipoMoneda.ToString();

            if (Cobros.TipoMoneda == TipoMoneda.DOLAR)
                this.lblTotal = "Total Local:";
            else
                this.lblTotal = "Total Dolar:";

            calculando = false;
            this.ModificarTipoCambio();
            
        }

        #endregion
        
        #region Eventos de los controles

        //private void txtexTipCambio_TextChanged(object sender, System.EventArgs e)
        //{
        //    ModificarTipoCambio();
        //}

        //private void txtExTotalCambio_TextChanged(object sender, System.EventArgs e)
        //{
        //    ModificarMonto();
        //}

		#endregion

        #endregion

    }
}
