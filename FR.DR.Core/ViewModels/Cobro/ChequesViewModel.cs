using System;
//using System.Net;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
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
    public class ChequesViewModel : DialogViewModel<bool>
    {
#pragma warning disable 414

        #region Constructores

        /// <summary>
		/// Crea un nuevo formulario de cheques para agregar cheques al recibo en gestión.
		/// </summary>
        public ChequesViewModel()
        :base("AplicaCheques")
        {
            
			this.chequesIngresados = Cobros.Recibo.ChequesAsociados;
            this.Cheques = new SimpleObservableCollection<Cheque>(this.chequesIngresados);
			this.esConsulta = false;
            this.TotalCheques = Cobros.MontoCheques;
            this.LoadPantalla();
        }

         /// <summary>
        /// Crea un nuevo formulario de cheques para agregar cheques al recibo en gestión.
        /// </summary>
        public ChequesViewModel(string monedaRecibo)
            :base(null)
        {            
            this.chequesIngresados = Cobros.Recibo.ChequesAsociados;
            this.Cheques = new SimpleObservableCollection<Cheque>(this.chequesIngresados);
            this.esConsulta = false;
            this.TotalCheques = Cobros.MontoCheques;
            if (monedaRecibo.Equals("L"))
            {
                this.moneda = TipoMoneda.LOCAL;
            }
            else
            {
                this.moneda = TipoMoneda.DOLAR;
            }
            this.LoadPantalla();
        }       


        #endregion

        #region Propiedades

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

       // public Recibo Recibo { get { return ConsultaCobroViewModel.ReciboSeleccionado; } }

        //public IObservableCollection<Cheque> Cheques { get { return new SimpleObservableCollection<Cheque>(Recibo.ChequesAsociados); } }
        public IObservableCollection<Cheque> Cheques { get; set ; }
        private Cheque itemActual;
        public Cheque ItemActual
        {
            get { return itemActual; }
            set
            {
                itemActual = value;
                RaisePropertyChanged("ItemActual");
            }
        }
        public IObservableCollection<string> Bancos { get; set; }
        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        private List<Cheque> chequesIngresados = new List<Cheque>();
        private List<Banco> BancosList = new List<Banco>();

        private string bancoActual;
        public string BancoActual
        {
            get { return bancoActual; }
            set
            {
                bancoActual = value;
                RaisePropertyChanged("BancoActual");
            }
        }

        private string cheque;
        public string Cheque
        {
            get { return cheque; }
            set
            {
                cheque = value;
                RaisePropertyChanged("Cheque");
            }
        }

        private string cuenta;
        public string Cuenta
        {
            get { return cuenta; }
            set
            {
                cuenta = value;
                RaisePropertyChanged("Cuenta");
            }
        }

        private DateTime fecha;
        public DateTime Fecha
        {
            get { return fecha; }
            set
            {
                fecha = value;
                RaisePropertyChanged("Fecha");
            }
        }

        private decimal montoCheque;
        public decimal MontoCheque
        {
            get { return montoCheque; }
            set
            {
                montoCheque = value;
                RaisePropertyChanged("MontoCheque");
            }
        }

        private decimal saldo;
        public decimal Saldo
        {
            get { return saldo; }
            set
            {
                saldo = value;
                RaisePropertyChanged("Saldo");
            }
        }

        private bool esConsulta;

        private decimal totalCheques;
        public decimal TotalCheques
        {
            get { return totalCheques; }
            set
            {
                totalCheques = value;
                RaisePropertyChanged("TotalCheques");
            }
        }

        private bool verCheques = true;
        public bool VerCheques
        {
            get { return verCheques; }
            set { verCheques = value; RaisePropertyChanged("VerCheques"); }
        }

        private TipoMoneda moneda;

        #endregion

        #region Comandos

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(Agrega); }
        }

        public ICommand ComandoEliminar
        {
            get { return new MvxRelayCommand(Elimina); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancela); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public ICommand ComandoVerCheques
        {
            get { return new MvxRelayCommand(cambiarVerCheques); }
        }

        #endregion Comandos        

        #region Metodos de la logica de Negocio

        #region Funciones usadas por los eventos
        /// <summary>
		/// Funcion que carga los datos iniciales.
		/// </summary>
		private void LoadPantalla()
		{            
            this.NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                  " Cliente: " + GlobalUI.ClienteActual.Nombre;
			//this.lblNomClt.Visible = false;
			
				//CrearDateTimePicker();

                this.Fecha = DateTime.Today;			

				try
				{
					BancosList = Banco.ObtenerBancos(Cobros.Recibo.Compania,false);
				}
				catch(Exception exc)
				{
					this.mostrarAlerta("Error cargando las entidades financieras. " + exc.Message);
				}
                Bancos = new SimpleObservableCollection<string>(BancosList.ConvertAll(x => x.Descripcion));		

			this.CargarCheques();
			this.CargarInfo();
		}

		/// <summary>
		/// Funcion que despliega en pantalla la informacion inicial
		/// </summary>
		private void CargarInfo()
		{			
			this.Cheque=string.Empty;
			this.Cuenta=string.Empty;
			this.MontoCheque=0;
            this.TotalCheques = this.totalCheques;
            this.Saldo = Cobros.MontoPendiente;
		}

		
		/// <summary>
		/// funcion que hace el llamado a la pantalla de aplicar pago
		/// y hace persistente en un arreglo global los cheques agregados.
		/// </summary>
		private void Aceptar()
		{
			if (!this.esConsulta)
				Cobros.Recibo.ChequesAsociados = this.chequesIngresados;
            ReturnResult(true);
            //this.DoClose();
            //this.RequestNavigate<AplicarPagoViewModel>();
		}
		
		
		/// <summary>
		/// Funcion que valida que en realidad se desea cancelar el ingreso de cheques y 
		/// no tomar en cuenta ningun proceso de la toma de cheques
		/// </summary>
		public void Cancela()
		{
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "el ingreso de cheques", result =>
                {
                    //si la respuesta al mensaje es positiva se debe cancelar todo lo que se haya realizado
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {                        
                        this.chequesIngresados.Clear();//se limpia el arreglo de cheques
                        Cheques = new SimpleObservableCollection<Cheque>(chequesIngresados);
                        Cobros.MontoCheques = 0;
                        //this.DoClose();
                        //this.RequestNavigate<AplicarPagoViewModel>();
                        this.ReturnResult(false);
                    }
                });			
			
		}
		
		/// <summary>
		/// Funcion que agrega un nuevo cheque
		/// </summary>
		private void Agrega()
		{
			if(this.Validacion())
			{
				decimal montoCheque = this.MontoCheque;//toma el valor digitado 
			
				if(montoCheque > Cobros.MontoPendiente && Cobros.MontoFacturasLocal != 0)//valida que el monto del cheque no sobrepase el monto que esta pendiente
				{
				    this.mostrarMensaje(Mensaje.Accion.Informacion,"El monto ingresado es mayor al monto que se debe."); 
				}
				else{

                    Cheque miCheque = new Cheque(GlobalUI.RutaActual.Codigo, GlobalUI.ClienteActual.Codigo,Cobros.Recibo.Compania);

					//Suma el monto del cheque al monto actual y global
					this.totalCheques += montoCheque;
					Cobros.MontoCheques = this.totalCheques;
				
					//miCheque.Banco.Codigo = bancos[this.cboBancos.SelectedIndex-1].Codigo;//le asigna el codigo del banco al cheque
                    miCheque.Banco.Codigo = ((Banco)BancosList.Find(x => x.Descripcion == BancoActual)).Codigo;
					//miCheque.Banco.Descripcion = bancos[this.cboBancos.SelectedIndex-1].Descripcion;
                    miCheque.Banco.Descripcion = ((Banco)BancosList.Find(x => x.Descripcion == BancoActual)).Codigo;
					miCheque.Banco.Cuenta = this.Cuenta;//le asigna el numero de cuenta
					miCheque.Numero = this.Cheque;//le asigna el numero de cheque
                    miCheque.Fecha = this.Fecha;//le asigna la fecha
					miCheque.Monto = this.MontoCheque;//le asigna el monto
					this.chequesIngresados.Add(miCheque);//agrega el cheque a un arreglo
				
					//this.AgregarChequeListView(miCheque);//invoca la funcion que carga el cheque en el listView
                    this.AgregarChequeListView();//invoca la funcion que carga el cheque en el listView
					Cobros.SacarCuentas();//invoca al metodo que calcula los montos
					this.CargarInfo();//invoca la funcion que carga la informacion en pantalla
				}

			}
		}

		/// <summary>
		/// Funcion que tiene la logica para la realizacion de la eliminacion de un cheque
		/// </summary>
		private void Elimina()
		{
			if(ItemActual==null)
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"un cheque"); 
			else
			{				

                this.mostrarMensaje(Mensaje.Accion.Retirar, "el cheque número: " +
                    this.ItemActual.Numero + " Monto:" + this.ItemActual.Monto, result =>
                    {
                        //si la respuesta del mensaje es afirmativo entonces 
                        //se procede a eliminar el cheque de del arreglo de cheques agregados
                        //y se recalculan los montos
                        if (result == DialogResult.Yes || result==DialogResult.OK)
                        {
                            Cheque chequeEliminar = ItemActual;

                            //Restamos el monto del cheque al monto de cheques actual y global
                            this.TotalCheques -= chequeEliminar.Monto;
                            Cobros.MontoCheques = this.TotalCheques;

                            this.chequesIngresados.Remove(ItemActual);//se remueve el cheque del arreglo
                            
                            //this.lsvcheque.Items.RemoveAt(j);// se remueve el cheque del listview
                            this.Cheques = new SimpleObservableCollection<Cheque>(chequesIngresados); RaisePropertyChanged("Cheques");

                            Cobros.SacarCuentas();//se reacalculan los montos
                            this.CargarInfo();// se invoca a la funcion de carga  de informacion
                        }
                    }); 			
			}
		}

		/// <summary>
		/// Agrega al un cheque al final de la lista visual (listView)
		/// </summary>
		/// <param name="nuevoCheque"></param>
		private void AgregarChequeListView()
		{
            Cheques = new SimpleObservableCollection<Cheque>(this.chequesIngresados); RaisePropertyChanged("Cheques");
            //string [] infoCheque = new string[3];

            //infoCheque[0] = nuevoCheque.Numero;
            //infoCheque[1] = nuevoCheque.Banco.Descripcion;
            //if (this.moneda == TipoMoneda.LOCAL)
            //    infoCheque[2] = GestorUtilitario.FormatNumero(nuevoCheque.Monto);
            //else
            //    infoCheque[2] = GestorUtilitario.FormatNumero(nuevoCheque.Monto, true);
			
            //ListViewItem row = new ListViewItem(infoCheque);//crea una nueva linea del listview
            //this.lsvcheque.Items.Add(row);//agrega la linea al listview
		}

		/// <summary>
		/// Funcion encargada de cargar los cheques al listView
		/// </summary>
		private void CargarCheques()
		{
			this.Cheques.Clear();
		
			// se recorre el arreglo de cheques ingresados y se van creando nuevas lineas del listview 
            //foreach(Cheque cheque in this.chequesIngresados)//recorre arreglo de cheques ingresados
            //    this.AgregarChequeListView(cheque);
            Cheques = new SimpleObservableCollection<Cheque>(this.chequesIngresados);
		}



		#endregion
		
		#region Funciones de validacion
		/// <summary>
		/// funcion que valida los campos que deben ser llenados esten correctos
		/// y que el monto del cheque no sobrepase el monto que se debe pagar 
		/// </summary>
		/// <param name="montoCheque"></param>
		/// <returns>retorna un verdadero si todo es correcto y un falso si hay algo incorrecto</returns>
		private bool Validacion()
		{
			if(this.MontoCheque==0)
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"El monto del cheque no puede ser 0."); 
				return false;
			}
			else if (this.MontoCheque.ToString()=="")//valida que el campo de monto del cheque no este vacio
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Debe digitar el monto del cheque."); 
				return false;
			}
			else if(string.IsNullOrEmpty(BancoActual))//valida que haya un banco seleccionado
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"el banco al que pertenece el cheque"); 
				return false;
			}
			else if(this.Cheque=="")//valida que el campo de numero de cheque no este vacio
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Debe digitar el número de cheque."); 
			    return false;
			}
			else if(this.Cuenta=="")//valida que el campo del numero de cuenta no este vacio
			{
				this.mostrarMensaje(Mensaje.Accion.Informacion,"Debe digitar el número de cuenta."); 
				return false;
			}
			else
			{
				return true;
			}
		}

        public void cambiarVerCheques() 
        {
            if (VerCheques)
            {
                VerCheques = false;
            }
            else
            {
                VerCheques = true;
            }
        }
		#endregion

        #endregion   
    }
}
