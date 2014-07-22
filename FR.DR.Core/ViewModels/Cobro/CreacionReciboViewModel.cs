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


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class CreacionReciboViewModel : BaseViewModel
    {

        #region Propiedades

        #region Companías y CompaniaActual
        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                    string companiaAnterior = companiaActual;
                    companiaActual = value;
                    CambioCompania(companiaAnterior); // se envia la vieja
                    RaisePropertyChanged("CompaniaActual");                
            }
        }
        public IObservableCollection<string> Companias { get; set; }
        #endregion Companías y CompaniaActual

        private TipoMoneda monedaActual;
        public TipoMoneda MonedaActual
        {
            get { return monedaActual; }
            set
            {
                if (value != monedaActual)
                {
                    monedaActual = value;
                    RaisePropertyChanged("MonedaActual");
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

        public IObservableCollection<TipoMoneda> Monedas { get; set; }

        private TipoCobro tipoPagoActual;
        public TipoCobro TipoPagoActual
        {
            get { return tipoPagoActual; }
            set
            {
                if (value != tipoPagoActual)
                {
                    tipoPagoActual = value;
                    RaisePropertyChanged("TipoPagoActual");
                }
            }
        }
        private IObservableCollection<TipoCobro> tipoPagos;
        public IObservableCollection<TipoCobro> TipoPagos
        {
            get { return tipoPagos; }
            set
            {
                tipoPagos = value;
                RaisePropertyChanged("TipoPagos");                
            }
        }

        private string numeroRecibo;
        public string NumeroRecibo
        {
            get { return numeroRecibo; }
            set
            {
                if (value != numeroRecibo)
                {
                    numeroRecibo = value;
                    RaisePropertyChanged("NumeroRecibo");
                }
            }
        }

        // binding editable 
        public bool NumberoReciboEditable { get { return Cobros.CambiarNumeroRecibo; } }
        public bool MonedaEnabled 
        { 
            get 
            {
                bool result = false;
                if (CompaniaActual != null)
                {
                    ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual);
                    if (cliente != null)
                    {
                        result = cliente.MultiMoneda;
                    }
                }
                return result;
            } 
        }

        private bool numReciboVisible=true;
        public bool NumReciboVisible
        {
            get { return numReciboVisible; }
            set
            {
                    numReciboVisible = value;
                    RaisePropertyChanged("NumReciboVisible");
                
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
        #endregion Propiedades

        public CreacionReciboViewModel()
        {

            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            TipoPagos = new SimpleObservableCollection<TipoCobro>();
            if (Cobros.CambiarNumeroRecibo)
            {
                NumReciboVisible = false;
            }
            ////Solamente agregamos el tipo de pago por monto total cuando asi este configurado en Config.xml
            if (Cobros.HabilitarMontoTotal)
            {
                TipoPagos.Add(TipoCobro.MontoTotal);
                TipoPagoActual = TipoCobro.MontoTotal;
            }
            RaisePropertyChanged("TipoPagos"); // se actualiza pues acaba de cambiar

            this.Monedas = new SimpleObservableCollection<TipoMoneda>() 
            {
                TipoMoneda.LOCAL,
                TipoMoneda.DOLAR
            };
            this.MonedaActual = Cobros.TipoMoneda;
            RaisePropertyChanged("Monedas"); // se actualiza pues acaba de cambiar

            //CargarCompañiasCobrosPendientes();

            Companias = new SimpleObservableCollection<string>(Util.CargarCiasClienteActual().ConvertAll(x => x.Compania));
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
        }

        /// <summary>
        /// Se cargan unicamente las compañias que cuentan con facturas pendientes
        /// </summary>
        private void CargarCompañiasCobrosPendientes()
        {
            Companias = new SimpleObservableCollection<string>();

            //LDA
            foreach (Cls.FRCliente.ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                if (GlobalUI.ClienteActual.ObtenerClienteCia(clt.Compania).FacturasPendientes.Count > 0)
                {
                    Companias.Add(clt.Compania);
                }
            }

            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
            RaisePropertyChanged("Companias"); // se actualiza pues acaba de cambiar
        }

        #endregion CargaInicial

        #region Comandos

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Regresar); }
        }

        #endregion Comandos

        #region Acciones

        /// <summary>
        /// Continuar el cobro
        /// </summary>
        private void Continuar()
        {
            if (this.CompaniaActual == null)
            {
                this.mostrarAlerta("Debe seleccionar una compañía");
                return;
            }

            //ABC 35772
            if (Cobros.CambiarNumeroRecibo)
            {
                if (this.NumeroRecibo != string.Empty)
                {
                    bool existe = Recibo.ExisteNumeroRecibo(this.NumeroRecibo);

                    if (existe)
                    {
                        this.mostrarAlerta("El número de recibo para el cobro ya fue utilizado");
                        return;
                    }
                    else
                        Cobros.NumeroReciboIndicado = this.NumeroRecibo;
                }
                else
                {
                    this.mostrarAlerta("Debe ingresar un número de recibo para el cobro");
                    return;
                }
            }

            Cobros.TipoCobro = this.TipoPagoActual;

            Dictionary<string, object> pars = new Dictionary<string, object>();
            if (Cobros.TipoCobro == TipoCobro.SeleccionFacturas)
            {
                //Se muestra la pantalla de seleccion de facturas
                this.RequestDialogNavigate<SeleccionFacturasViewModel, bool>(null, result =>
                {
                    if (Cobros.Recibo == null)
                    {
                        //Se cancelo la gestion del recibo
                        RequestNavigate<MenuClienteViewModel>();
                        DoClose();
                    }
                    else
                    {
                        this.RequestDialogNavigate<AplicarPagoViewModel, bool>(pars, r2 =>
                            {
                                Dictionary<string, object> par = new Dictionary<string, object>();
                                par.Add("habilitarPedidos", true);
                                RequestNavigate<MenuClienteViewModel>(par);
                                this.DoClose();
                            });
                        //this.RequestNavigate<AplicarPagoViewModel>();
                        //this.DoClose();
                    }
                });
            }
            else
            {
                if (Cobros.Recibo == null)
                {
                    //Se cancelo la gestion del recibo
                    RequestNavigate<MenuClienteViewModel>();
                    DoClose();
                }
                else
                {
                    this.RequestDialogNavigate<AplicarPagoViewModel, bool>(pars, res =>
                    {
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                        this.DoClose();
                    });
                    //this.RequestNavigate<AplicarPagoViewModel>();
                    //this.DoClose();
                }
            }


        }

        public void Regresar() 
        {
                this.mostrarMensaje(Mensaje.Accion.Cancelar, "el cobro", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                        this.DoClose();
                    }
                });
        }

        public override void DoClose()
        {
            Cobros.FinalizarCobro();
            base.DoClose();
        }

        #endregion Acciones

        #region Metodos de instancia

        /// <summary>
        /// Cambio de la compania
        /// </summary>
        private void CambioCompania(string companiaOld)
        {
            //si el indice de seleccion del combo es distinto al de la compannia seleccionada anteriormente se 
            //cancela todo lo realizado sobre la compannia que se selecciono inicialmente 
            //y se procede a trabajar sobre la seleccionada.
            if (!CompaniaActual.Equals(companiaOld))
            {
                try
                {
                    Cobros.IniciarCobro(GlobalUI.ClienteActual, CompaniaActual);

                    //Cargamos las notas de credito del cliente para esta compannia
                    //Cargamos los pendientes de cobro del cliente
                    GlobalUI.ClienteActual.CargarNotasCredito(CompaniaActual);
                    GlobalUI.ClienteActual.CargarFacturasCobro(CompaniaActual);

                    if (GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual).FacturasPendientes.Count == 0)
                    {
                        TipoPagos.Remove(TipoCobro.SeleccionFacturas);
                    }
                    else
                    {
                        TipoPagos.Add(TipoCobro.SeleccionFacturas);
                    }

                    if (this.TipoPagos.Count > 0)
                    {
                        this.TipoPagoActual = TipoPagos[0];
                    }
                    RaisePropertyChanged("TipoPagos"); // se actualiza pues acaba de cambiar

                    MonedaActual = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual).Moneda;
                    RaisePropertyChanged("MonedaEnabled"); // se actualiza pues depende de la CompaniaActual que acaba de cambiar
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta(ex.Message);
                }
            }
        
        }

		#endregion

    }
}
