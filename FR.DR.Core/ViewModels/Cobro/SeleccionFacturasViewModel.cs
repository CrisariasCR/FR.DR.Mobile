using System;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Cobro;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SeleccionFacturasViewModel : DialogViewModel<bool>
    {
        #region Propiedades

        #region Items e ItemActual
        public Factura itemActual;
        public Factura ItemActual 
        {
            get { return itemActual; }
            set
            {
                if (value != itemActual)
                {
                    itemActual = value;
                    RaisePropertyChanged("ItemActual");
                    SeleccionarFactura(value);
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

        public IObservableCollection<Factura> Items { get; set; }

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }


        private List<Factura> ItemsSeleccionados
        {
            get
            {
                return new List<Factura>(this.Items.Where<Factura>(x => x.Seleccionado));
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
                    if (Items[i].Seleccionado)
                        result.Add(i);
                }
                return result;
            }
        }

        #endregion Items e ItemActual

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

        private DateTime fechaVence;
        public DateTime FechaVence
        {
            get { return fechaVence; }
            set
            {
                if (value != fechaVence)
                {
                    fechaVence = value;
                    RaisePropertyChanged("FechaVence");
                }
            }
        }

        public TipoMoneda TipoMoneda
        {
            get { RaisePropertyChanged("TipoMoneda"); return Cobros.TipoMoneda; }            
        }

        private decimal montoDoc;
        public decimal MontoDoc
        {
            get { return montoDoc; }
            set
            {
                if (value != montoDoc)
                {
                    montoDoc = value;
                    RaisePropertyChanged("MontoDoc");
                }
            }
        }

        private decimal saldoDoc;
        public decimal SaldoDoc
        {
            get { return saldoDoc; }
            set
            {
                if (value != saldoDoc)
                {
                    saldoDoc = value;
                    RaisePropertyChanged("SaldoDoc");
                }
            }
        }

        private decimal montoPagar;
        public decimal MontoPagar
        {
            get { return montoPagar; }
            set
            {
                if (value != montoPagar)
                {
                    montoPagar = value;
                    this.CambioMontoPagar(montoPagar);
                    RaisePropertyChanged("MontoPagar");
                }
            }
        }

        private decimal totalPagar;
        public decimal TotalPagar
        {
            get { return totalPagar; }
            set
            {
                if (value != totalPagar)
                {
                    totalPagar = value;
                    RaisePropertyChanged("TotalPagar");
                }
            }
        }

        private bool montoPagarEnabled;
        public bool MontoPagarEnabled
        {
            get { return montoPagarEnabled; }
            set
            {
                montoPagarEnabled = value;
                    RaisePropertyChanged("MontoPagarEnabled");
            }
        }

        // muestra
        public bool MontoPagarVisibility { get; set; }
        
        bool canClose = false;

        #endregion Propiedades

        public SeleccionFacturasViewModel(string messageId)
            : base(messageId)
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {

            this.CargarFacturas();

            // signa la funcion para ejecutar cuando una factura es seleccionada
            Factura.ItemCheck = ItemCheck;

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;
        }

        /// <summary>
        /// Funcion que carga las facturas que estan pendientes por pagar
        /// </summary>
        private void CargarFacturas()
        {
            List<Factura> Facturas = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).FacturasPendientes;

            //binding
            // factura.Tipo ConvertidorTipoDocumento
            // factura.Numero;//agrega el numero de la factura a la posicion 1 del arreglo
            // Cobros.TipoMoneda.ToString();

            // if factura.MontoAPagarDocLocal == 0 // se crea propiedad para que retorne el valor apropiado
            //  factura.MontoAPagarViewLocal, Converter=FormatoNumero; Visibility Moneda,Converter=MonedaCobrosToVisibility,ConverterParameter='False'"
            //  factura.MontoAPagarViewLocal, Converter=FormatoNumero,ConverterParameter='True'; Visibility Moneda,Converter=MonedaCobrosToVisibility,ConverterParameter='True'"

            // factura.SaldoDocLocal, Converter=FormatoNumero;
            // factura.SaldoDocDolar, Converter=FormatoNumero,ConverterParameter='True';
            // factura.TipoCambio , Converter=FormatoNumero;
            // factura.Seleccionado

            // selecciona las facturas que estan incluidas en el Detalle del Recibo de Cobro
            foreach (Factura fac in Facturas)
            {
                int count = Cobros.Recibo.Detalles.Where(rec =>
                    rec.NumeroDocAfectado == fac.Numero &&
                    (rec.Tipo == TipoDocumento.Factura || rec.Tipo == TipoDocumento.FacturaContado)).Count();

                fac.Seleccionado = count > 0;
            }

            Items = new SimpleObservableCollection<Factura>(Facturas);
            RaisePropertyChanged("Items");

            this.CalculaMontoTotalPagar(false, null);
        }

        /// <summary>
        /// Saca el monto total que hay que pagar de las facturas seleccionadas
        /// </summary>
        /// <param name="esCheck">si el ultimo click chequeo un item</param>
        /// <param name="index">el item que fue chequeado o deschequeado</param>
        private void CalculaMontoTotalPagar(bool esCheck, Factura facCheck)
        {
            //LJR 09/06/2009 Caso 35843: Se agregan parametros al metodo para determinar si el item
            //Que activa este evento fue checked o unchecked
            decimal montoTotalPagar = 0;

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                montoTotalPagar = this.Items.Where(fac => (fac.Seleccionado && fac != facCheck) || (fac == facCheck && esCheck)).Sum(fac => fac.MontoAPagarDocLocal);
            else
                montoTotalPagar = this.Items.Where(fac => (fac.Seleccionado && fac != facCheck) || (fac == facCheck && esCheck)).Sum(fac => fac.MontoAPagarDocDolar);

            this.TotalPagar = montoTotalPagar;
        }

        #endregion CargaInicial

        #region Comandos

        public ICommand ComandoSeleccionar
        {
            get { return new MvxRelayCommand<Factura>(SeleccionarFactura); }
        }

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        #endregion Comandos

        #region Acciones

        /// <summary>
        /// Funcion que se ejecuta con el cambio de indice del
        /// listview muestra la informacion de la nota de credito seleccionada
        /// </summary>
        private void SeleccionarFactura(Factura facSeleccionada)
        {
            //Verifica que exista una linea valida seleccionada
            if (ItemActual != null)
            {
                facSeleccionada = ItemActual;
            }
            else 
            {
                return;
            }
            if (facSeleccionada != null)
            {
                //PA2-01482-MKP6 - KFC
                //Se redondean a 2 decimales los totales 
                facSeleccionada.MontoDocLocal = RedondeoAlejandoseDeCero(facSeleccionada.MontoDocLocal, 2);
                facSeleccionada.SaldoDocLocal = RedondeoAlejandoseDeCero(facSeleccionada.SaldoDocLocal, 2);
                facSeleccionada.SaldoDocDolar = RedondeoAlejandoseDeCero(facSeleccionada.SaldoDocDolar, 2);
                facSeleccionada.MontoDocDolar = RedondeoAlejandoseDeCero(facSeleccionada.MontoDocDolar, 2);

                //LJR 09/06/2009 Caso 35843: Correccion para diferenciar Tipo de moneda utilizada			
			    switch(Cobros.TipoMoneda)
			    {
				    case TipoMoneda.LOCAL:
					    this.FechaVence = facSeleccionada.FechaVencimiento;
                        this.MontoDoc = facSeleccionada.MontoDocLocal;
                        this.SaldoDoc = facSeleccionada.SaldoDocLocal;
                        this.MontoPagar = facSeleccionada.MontoAPagarDocLocal == 0? facSeleccionada.SaldoDocLocal : facSeleccionada.MontoAPagarDocLocal;
                        break;
				    case TipoMoneda.DOLAR:
					    this.FechaVence = facSeleccionada.FechaVencimiento;
                        this.MontoDoc = facSeleccionada.MontoDocDolar;
                        this.SaldoDoc = facSeleccionada.SaldoDocDolar;
                        this.MontoPagar = facSeleccionada.MontoAPagarDocLocal == 0? facSeleccionada.SaldoDocDolar : facSeleccionada.MontoAPagarDocDolar;
					    break;
			    }

			    //A las facturas de contado no se le puede cambiar el monto a pagar
                // binding Enabled MontoPagarEnabled
			    MontoPagarEnabled = facSeleccionada.Tipo != TipoDocumento.FacturaContado;

            }
        }

                /// <summary>
        /// Continuar con el cobro
        /// </summary>
		private void Continuar()
		{
			Cobros.facturasSeleccionadas.Clear();
			Cobros.MontoFacturasSeleccion = 0;

			//Obtenemos el tipo de cambio del dolar de la compania asociada a las facturas
            string compania = Cobros.Recibo.Compania;
			decimal tipoCambio = 0;
			
			try
			{
                tipoCambio = Cobros.Recibo.TipoCambio;//Compania.ObtenerTipocambio(compania);
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error cargando tipo de cambio del dolar. " + ex.Message);
                return;
			}
			
			foreach(Factura factura in this.Items)
			{
				if (factura.Seleccionado)
				{
                    //LJR 09/06/2009 Caso 35843: Correccion para diferenciar Tipo de moneda utilizada
					if(Cobros.TipoMoneda == TipoMoneda.LOCAL)
					{
						factura.MontoAPagarDocDolar = factura.MontoAPagarDocLocal / factura.TipoCambio;
                        Cobros.MontoFacturasSeleccion += factura.MontoAPagarDocLocal;
                    }
					else
					{
						//factura.MontoAPagarDocDolar = factura.MontoAPagarDocLocal;
                        factura.MontoAPagarDocLocal = factura.MontoAPagarDocDolar * factura.TipoCambio;
                        Cobros.MontoFacturasLocal += factura.MontoAPagarDocDolar;
                        Cobros.MontoFacturasSeleccion += factura.MontoAPagarDocDolar;
					}

					Cobros.facturasSeleccionadas.Add(factura);
				}
			}
            
            if (Cobros.facturasSeleccionadas.Count != 0)
            {
                this.canClose = true; // indica que es un continuar
                this.ReturnResult(true);
            }
            else
                this.mostrarAlerta("Debe seleccionar al menos una factura.");
		}


        #endregion Acciones

	            /// <summary>
        /// Se utiliza para evadir el metodo de redondeo default utilizado por Decimal y Math en el
        /// cual se redondea del modo "banquero"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static decimal RedondeoAlejandoseDeCero(decimal value, int digits)
        {
            return System.Math.Round(value +
                Convert.ToDecimal(System.Math.Sign(value) / System.Math.Pow(10, digits + 1)), digits);
        }


        // es el cancel, auqneu hay que ver si es equivalente, la idea es que se activa al darle regresar boton de android
		/// <summary>
		/// Funcion que realiza la cancelacion de todo lo que se realizo en la parte de pago
		/// </summary>
		public void Regresar()
		{
            if (!canClose)
            {
			    this.mostrarMensaje(Mensaje.Accion.Cancelar, "el cobro", result =>
                    {
			            if(result == DialogResult.Yes)
			            {
				            Cobros.FinalizarCobro();
                            canClose = true;
                            base.DoClose();
                            this.ReturnResult(false);
			            }
                    }); 
            }
		}

        		// LDS Caso: 27900 02/04/2007
		/// <summary>
		/// Procedimiento que valida el cambio del monto a pagar
		/// </summary>
		private void CambioMontoPagar(decimal monto)
		{
			bool lbok = true;

			if (monto < 0)
			{
				lbok = false;
				this.mostrarAlerta("El monto a pagar no puede ser menor a cero.", 
                    r => { AsignarMontoPagar(ItemActual); });
			}
			else if (monto == 0 && ItemActual.Seleccionado)
			{
				lbok = false;
				this.mostrarAlerta("El monto a pagar no puede ser igual a cero.", 
                    r => { AsignarMontoPagar(ItemActual); });
			}
			else if (monto > 0)
			{
				if(Cobros.TipoMoneda == TipoMoneda.LOCAL)
				{
                    // PA2-01482-MKP6 - KFC
                    // Ahora los redondeos se hacen en el metodo SeleccionarFactura
                    // if(monto > Decimal.Round(this.facSeleccionada.SaldoDocLocal,5))
                    
                    if (monto > ItemActual.SaldoDocLocal) 
					{	
						lbok = false;
						this.mostrarAlerta("El monto a pagar no puede ser mayor al saldo.", 
                            r => { AsignarMontoPagar(ItemActual); });
					}				
				}
				else				
				{
					if(monto > ItemActual.SaldoDocDolar)
					{	
						lbok = false;
						this.mostrarAlerta("El monto a pagar no puede ser mayor al saldo.", 
                            r => { AsignarMontoPagar(ItemActual); });
					}
				}
			}

            if (lbok)
            {
                //LJR 35843: Correccion para diferenciar Tipo de moneda utilizada
                //Definimos el monto a pagar por la factura
                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                {
                    ItemActual.MontoAPagarDocLocal = this.montoPagar;
                    ItemActual.MontoAPagarDocDolar = ItemActual.MontoAPagarDocLocal / ItemActual.TipoCambio;
                }
                else
                {
                    ItemActual.MontoAPagarDocDolar = this.montoPagar;
                    ItemActual.MontoAPagarDocLocal = ItemActual.MontoAPagarDocDolar * ItemActual.TipoCambio;

                }
                this.CalculaMontoTotalPagar(false, null);
            }
            // El AsignarMontoPagar que se ejecuta luego de las Alertas, es el equivalente a lo que se ejecutaba en este else
            //else { }
		}

        private void AsignarMontoPagar(Factura facSeleccionada)
        {
            if (Cobros.TipoMoneda == TipoMoneda.LOCAL) 
                this.MontoPagar = facSeleccionada.SaldoDocLocal;
            else
                this.MontoPagar = facSeleccionada.SaldoDocDolar;
        }

        private void ItemCheck(Factura facChecked, bool newValueChecked)
        {
            //LJR 09/06/2009 Caso 35843: La forma de seleccionar una factura se modifica
            //Forzar el evento check como la seleccion con click para refrescar datos
            ItemActual = facChecked;

            //Marcar la factura como seleccionada
            //this.SeleccionarFactura(facChecked);
            this.CalculaMontoTotalPagar(newValueChecked, facChecked);
        }

/*
        private void ltbMontoPagar_TextChanged(object sender, EventArgs e)
        {
            
        }

		
 */

    }
}
