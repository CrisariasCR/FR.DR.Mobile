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
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using FR.DR.Core.Helper;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class AplicarDepositoViewModel : DialogViewModel<bool>
    {

        #region Constructor

        /// <summary>
        /// Crear un nuevo formulario para crar depósitos.
        /// </summary>
        /// <param name="deposito">Deposito con el que se esta trabajando.</param>
        /// <param name="cuentas">cuentas del banco asociado</param>
        public AplicarDepositoViewModel(string messageId)
            : base(messageId)
		{
			this.elDeposito = Gestor.Deposito;
            this.listCuentas = Gestor.Cuentas;
            this.CargarInfoInicial();
		}

        #endregion

        #region Propiedades

        private Deposito elDeposito = new Deposito();
        private List<CuentaBanco> listCuentas = new List<CuentaBanco>();

        private string cuentaActual;
        public string CuentaActual
        {
            get { return cuentaActual; }
            set
            {
                if (value != cuentaActual)
                {
                    cuentaActual = value;
                    RaisePropertyChanged("CuentaActual"); DefineCuenta();
                }
            }
        }

        private bool txtMontoEnabled;
        public bool TxtMontoEnabled
        {
            get { return txtMontoEnabled; }
            set
            {
                if (value != txtMontoEnabled)
                {
                    txtMontoEnabled = value;
                    RaisePropertyChanged("TxtMontoEnabled");
                }
            }
        }

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set
            {
                nombre = value;
                RaisePropertyChanged("Nombre");                
            }
        }

        private string fecha;
        public string Fecha
        {
            get { return fecha; }
            set
            {
                fecha = value;
                RaisePropertyChanged("Fecha");
            }
        }

        private string compania;
        public string Compania
        {
            get { return compania; }
            set
            {
                compania = value;
                RaisePropertyChanged("Compania");
            }
        }

        private string banco;
        public string Banco
        {
            get { return banco; }
            set
            {
                banco = value;
                RaisePropertyChanged("Banco");
            }
        }

        private string moneda;
        public string Moneda
        {
            get { return moneda; }
            set
            {
                moneda = value;
                RaisePropertyChanged("Moneda");
            }
        }

        private string deposito;
        public string DepositoProperty
        {
            get { return deposito; }
            set
            {
                deposito = value;
                RaisePropertyChanged("Deposito");
            }
        }

        private decimal monto;
        public decimal Monto
        {
            get { return monto; }
            set
            {
                monto = value;
                RaisePropertyChanged("Monto");
            }
        }

        public IObservableCollection<string> Cuentas { get; set; }

        #endregion

        #region Comandos

        public ICommand ComandoFinalizar
        {
            get { return new MvxRelayCommand(Finalizar); }
        }


        #endregion

        #region Metodos

        /// <summary>
        /// Carga la informacion inicial en pantalla
        /// </summary>
        private void CargarInfoInicial()
        {
            try
            {
                //this.cboCuentas.DataSource =this.listCuentas;
                Cuentas = new SimpleObservableCollection<string>(listCuentas.ConvertAll(x => x.Numero));
                if (Cuentas.Count > 0)
                {
                    CuentaActual = Cuentas[0];
                }

                //ABC 11/08/2008 Caso:33488 Habilita cambio de monto de deposito
                TxtMontoEnabled = Deposito.CambioMonto;
                this.DefineInfoControles();

            }
            catch (System.Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "Error cargando la información del depósito. " + ex.Message);
            }

        }

        /// <summary>
        /// Define la informacion en los controles.
        /// </summary>
        private void DefineInfoControles()
        {
            string montoDeposito = string.Empty;
            if (!string.IsNullOrEmpty(CuentaActual))
            {
                Nombre = (listCuentas.Find(x => x.Numero == CuentaActual) as CuentaBanco).Descripcion;
            }
            this.Fecha = elDeposito.FechaRealizacion.ToString("dd/MM/yyyy");
            this.Compania = elDeposito.Compania;
            this.Banco = elDeposito.Banco.Descripcion;
            this.Moneda = elDeposito.Moneda.ToString();
            this.DepositoProperty = elDeposito.Numero.ToString();
            //this.txtMonto.Format = GestorUtilitario.FormatTextBox(elDeposito.Moneda);
            this.Monto = elDeposito.MontoTotal;
        }

        /// <summary>
        /// Carga las cuentas bancarias
        /// </summary>
        private void DefineCuenta()
        {
            try
            {
                if (!string.IsNullOrEmpty(CuentaActual))
                {
                    this.elDeposito.Banco.Cuenta = CuentaActual;
                    DefineInfoControles();
                }
            }
            catch (System.Exception esx)
            {
                this.mostrarAlerta("Error cargando las cuentas bancarias. " + esx.Message);
            }
        }

        /// <summary>
        /// Finaliza la creacion de depósitos
        /// </summary>
        private void Finalizar()
        {
            this.mostrarMensaje(Mensaje.Accion.Terminar, "el depósito", res =>
            {
                if (res.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                {
                    if (!string.IsNullOrEmpty(CuentaActual))
                    {
                        try
                        {
                            //Caso 27625 LDS 09/07/2007
                            //Se acordó que el número mayor que puede tener un depósito es 9999999999.
                            //Caso 29219 LDS 30/07/2007
                            //El menor número de depósito permitido será 1.
                            if (Convert.ToDecimal(this.DepositoProperty) <= Convert.ToDecimal("9999999999") &&
                                (Convert.ToDecimal(this.DepositoProperty) >= Convert.ToDecimal("1")))
                            {
                                //LDS Fecha: 20070308 Caso: 27625
                                // Se cambio de entero a decimal
                                this.elDeposito.Numero = GestorUtilitario.ParseDecimal(this.DepositoProperty);

                                //Caso 29219 LDS 30/07/2007
                                if (!this.elDeposito.ExisteNumeroDeposito())
                                {
                                    //ABC 11/08/2008 Casos:33490
                                    //Se toma el nuevo monto del deposito
                                    string montoNuevo = string.Empty;

                                    if (Deposito.CambioMonto && this.Monto.ToString() != this.elDeposito.MontoTotal.ToString())
                                        montoNuevo = this.Monto.ToString();

                                    this.elDeposito.GuardaDepositoEnBD(montoNuevo);

                                    //KFC - 22/02/2012 
                                    //Se actualiza la tabla JORNADA_RUTAS
                                    if (GlobalUI.RutaActual != null)
                                    {
                                        ActualizarJornada(GlobalUI.RutaActual.Codigo, this.Monto);
                                    }

                                    //Le retornamos al formulario padre que el deposito se realizo con exito
                                    base.DoClose();
                                    ReturnResult(true);
                                    // this.DoClose();
                                }
                                else
                                {
                                    this.mostrarMensaje(Mensaje.Accion.Informacion, "El número del depósito '" + this.elDeposito.Numero.ToString() + "' ya ha sido ingresado, debe ingresar otro número de depósito.");
                                }
                            }
                            else
                            {
                                this.mostrarMensaje(Mensaje.Accion.Informacion, "El número del depósito '" + this.elDeposito.Numero.ToString() + "' debe ser menor o igual a 9999999999 y mayor a Cero.");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta(ex.Message);
                        }
                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "Debe seleccionar una cuenta bancaria.. ");
                    }
                }
            }
                );

        }

        //MejorasGrupoPelon600R6 - KFC
        /// <summary>
        /// Actualiza los valores en la tabla JORNADA_RUTAS 
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="monto"></param>
        private void ActualizarJornada(string ruta, decimal monto)
        {
            TipoMoneda moneda = (TipoMoneda)elDeposito.Moneda;
            string colCantidad = "";
            string colMonto = "";

            if (moneda == TipoMoneda.LOCAL)
            {
                colCantidad = JornadaRuta.DEPOSITOS_LOCAL;
                colMonto = JornadaRuta.MONTO_DEPOSITOS_LOCAL;
            }
            else
            {
                colCantidad = JornadaRuta.DEPOSITOS_DOLAR;
                colMonto = JornadaRuta.MONTO_DEPOSITOS_DOLAR;
            }

            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, 1);
                JornadaRuta.ActualizarRegistro(ruta, colMonto, monto);

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                this.mostrarAlerta("Error al actualizar datos. " + ex.Message);
            }
        }

        #endregion
    }
}
