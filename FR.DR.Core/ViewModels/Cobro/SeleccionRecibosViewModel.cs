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
using System.Windows.Forms;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SeleccionRecibosViewModel : BaseViewModel
    {
        #region Constructor

        public SeleccionRecibosViewModel()
        {
            CompaniasList = Compania.Obtener();
            Companias = new SimpleObservableCollection<string>(CompaniasList.ConvertAll(x => x.Codigo));
            if (Companias.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.elDeposito.Compania))
                {
                    CompaniaActual = this.elDeposito.Compania;
                }
                else
                {
                    CompaniaActual = Companias[0];
                }
            }
        }

        #endregion

        #region Propiedades        
        public IObservableCollection<string> Companias { get; set; }
        private List<Compania> CompaniasList = new List<Compania>();
        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual"); CambiarCompania();
            }
        }

        public IObservableCollection<string> Entidades { get; set; }
        private List<Banco> EntidadesList = new List<Banco>();
        private string entidadActual;
        public string EntidadActual
        {
            get { return entidadActual; }
            set
            {
                entidadActual = value;
                RaisePropertyChanged("EntidadActual");
            }
        }

        private IObservableCollection<Recibo> items;
        public IObservableCollection<Recibo> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }

        private List<Recibo> ItemsSeleccionados
        {
            get
            {
                return new List<Recibo>(this.Items.Where<Recibo>(x => x.Seleccionado));
            }           
        }

        private List<Recibo> lstRecibos = new List<Recibo>();
        private List<Recibo> recibos = new List<Recibo>();

        private Deposito elDeposito = new Deposito();

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

        private bool localSeleccionado=true;
        public bool LocalSeleccionado
        {
            get { return localSeleccionado; }
            set { localSeleccionado = value; RaisePropertyChanged("LocalSeleccionado"); CambioRadioGroup(); }
        }

        private bool dolarSeleccionado;
        public bool DolarSeleccionado
        {
            get { return dolarSeleccionado; }
            set { dolarSeleccionado = value; RaisePropertyChanged("DolarSeleccionado"); CambioRadioGroup(); }
        }

        #endregion

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Salir); }
        }

        public ICommand ComandoTerminar
        {
            get { return new MvxRelayCommand(Continuar); }
        }

        public ICommand ComandoSeleccionarTodos
        {
            get { return new MvxRelayCommand(SeleccionarTodos); }
        }

        public ICommand ComandoSeleccionarNinguno
        {
            get { return new MvxRelayCommand(SeleccionarNinguno); }
        }

        #endregion

        #region Metodos

        /// <summary>
		/// LLama a la funcion encargada de realizar la consulta de los recibos asociados a la compañía
		/// y de cargar los datos en el ListView
		/// </summary>
		private void CargaRecibos(TipoMoneda moneda)
		{
			try
			{
				this.lstRecibos.Clear();

				if(!string.IsNullOrEmpty(CompaniaActual))
				{
                    string codigoCompania = CompaniaActual;

					this.elDeposito.Compania = codigoCompania ;
					this.elDeposito.Moneda = moneda;

                    this.recibos = Recibo.CargaRecibosDeposito(codigoCompania, moneda);

					foreach(Recibo recibo in this.recibos)
					{
						if(recibo.ChequesAsociados.Count == 0)
						{
							try
							{
								recibo.CargaChequesAplicados();
							}
							catch(Exception ex)
							{
								this.mostrarAlerta("Error cargando cheques aplicados al recibo '" + recibo.Numero + "'. " + ex.Message);
							}
						}
					}
			
					int cont = 0;
					//Caso 29530 LDS 24/08/2007
					//Contiene los recibos que poseen monto Cero.
					List<Recibo> recibosTemp = new List<Recibo>();

					foreach(Recibo recibo in this.recibos)
					{
						//Solo agregamos los recibos con montos mayores a cero
						if (recibo.MontoDocLocal > 0 || recibo.MontoDocDolar > 0)
						{
							Recibo encabezadoRecibo = new Recibo();
                            encabezadoRecibo = recibo;
                            
							//De acuerdo a la moneda mostramos el monto del recibo
							//if(moneda == TipoMoneda.LOCAL)
                            if (recibo.Moneda == TipoMoneda.LOCAL)
								encabezadoRecibo.Monto = GestorUtilitario.FormatNumero(recibo.MontoDocLocal);
							else
								encabezadoRecibo.Monto = GestorUtilitario.FormatNumero(recibo.MontoDocDolar, true);							

							this.lstRecibos.Add(encabezadoRecibo);
                            
				
							//Seleccionamos los recibos que ya el deposito tenga asociados
							foreach(DetalleDeposito detDep in this.elDeposito.Detalles)
							{
								if(recibo.Numero.Equals(detDep.Recibo.Numero))
								{
									this.lstRecibos[cont].Seleccionado = true;
									break;
								}
							}

							//Caso 29530 LDS 24/08/2007
							//Solo se toma en cuenta los que se incluyen en el list view
							cont++;

                            Items = new SimpleObservableCollection<Recibo>(lstRecibos);
						}
						else
						{
							//Caso 29530 LDS 24/08/2007
							//Se agregan los recibos que poseen monto Cero
							recibosTemp.Add(recibo);							
						}						
					}
                    if (lstRecibos.Count == 0)
                    {
                        Items = new SimpleObservableCollection<Recibo>(lstRecibos);
                    }

					//Caso 29530 LDS 24/08/2007
					//Se quitan del arreglo de recibos aquellos que posean monto Cero
					foreach(Recibo recibo in recibosTemp)
						this.recibos.Remove(recibo);

					//Caso 29530 LDS 24/08/2007					
					recibosTemp = null;
				}
			}
			catch(System.Exception ex)
			{
				this.mostrarAlerta(ex.Message);
			}
		}

		/// <summary>
		/// Cierra la pantalla
		/// </summary>
		private void Salir()
		{


            this.mostrarMensaje(Mensaje.Accion.Cancelar, "el depósito", res =>
                {
                    if (res.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                    {
                        this.AbrirFormPrincipal();
                    }
                });		
		}

		/// <summary>
		/// Cierra el form actual y abre el formulario principal.
		/// </summary>
		private void AbrirFormPrincipal()
		{
			this.elDeposito = null;
            RequestNavigate<MenuPrincipalViewModel>(true);			
            //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que llibera de memoria propiedas de los componentes
            this.DoClose();
		}

		/// <summary>
		/// Pasa a la pantalla donde se selecciona la cuenta bancaria y se finaliza el deposito
		/// </summary>
		private void Continuar()
		{
			Banco banco;

			if(string.IsNullOrEmpty(EntidadActual))
			{
				this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"la entidad financiera");
				return;
			}
			else
				banco = (EntidadesList.Find(x=>x.Descripcion==EntidadActual) as Banco);

            this.elDeposito.Banco = new BancoAsociado(banco.Codigo, banco.Descripcion, elDeposito.Moneda);
			
			try
			{
				banco.CuentasBancarias = CuentaBanco.ObtenerCuentasBancarias(elDeposito.Compania,elDeposito.Banco.Codigo, elDeposito.Moneda);
			}
			catch(Exception ex)
			{
				this.mostrarAlerta("Error cargando cuentas asociadas a la entidad. " + ex.Message);
			}

			if(banco.CuentasBancarias.Count > 0)
			{
				//Agregamos los recibos al deposito
				elDeposito.Detalles.Clear();

                if (Items == null || Items.Count == 0)
                {
                    this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "al menos un recibo a depositar");
                    return;
                }

				foreach(Recibo item in this.ItemsSeleccionados)
				{					
				    //elDeposito.AgregarDetalle((Recibo) this.recibos[item.Index]);
                    elDeposito.AgregarDetalle(item);
				}

				if (this.elDeposito.Detalles.Count == 0)
				{
					this.mostrarMensaje(Mensaje.Accion.SeleccionNula,"al menos un recibo a depositar");
					return;
				}

				elDeposito.CalculeMontoTotalDeposito();

                Gestor.Deposito = this.elDeposito;
                Gestor.Cuentas = banco.CuentasBancarias;                
                this.RequestDialogNavigate<AplicarDepositoViewModel,bool>(null,resp=>
                    {                       
                        if (resp.Equals(true))
                        {
                            this.AbrirFormPrincipal();
                        }
                    }
                );
                
			}
			else
				this.mostrarMensaje(Mensaje.Accion.Informacion,"La entidad financiera seleccionada no tiene cuentas definidas para la moneda seleccionada.");
		}

		/// <summary>
		/// Carga las entidades bancarias asociadas a la compañía.
		/// </summary>
		private void CargarEntidadesBancarias()
		{
			try
			{
				if(!string.IsNullOrEmpty(CompaniaActual))
				{
                    EntidadesList = Banco.ObtenerBancos(CompaniaActual, true);
                    //cboEntidadBancaria.DataSource = Banco.ObtenerBancos((this.cboCompania.SelectedItem as Compania).Codigo,true);
                    //this.cboEntidadBancaria.SelectedValue = this.elDeposito.Banco.Codigo;
                    Entidades = new SimpleObservableCollection<string>(EntidadesList.ConvertAll(x => x.Descripcion));
                    if (Entidades.Count > 0) 
                    {
                        EntidadActual = Entidades[0];
                    }
				}
			}
			catch(Exception exc)
			{
				this.mostrarAlerta("Error cargando las entidades financieras. " + exc.Message);
			}
		}

        private void CambiarCompania()
        {
            if (!string.IsNullOrEmpty(CompaniaActual))
            {
                try
                {
                    elDeposito.TipoCambio =(CompaniasList.Find(x=>x.Codigo==CompaniaActual) as Compania).TipoCambio;
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando tipo de cambio del dolar. " + ex.Message);
                }

                this.CargarEntidadesBancarias();

                this.CargaRecibos(this.LocalSeleccionado ? TipoMoneda.LOCAL : TipoMoneda.DOLAR);
            }        
        }

        private void CambioRadioGroup()
        {
            if (LocalSeleccionado)
                CargaRecibos(TipoMoneda.LOCAL);
            if (DolarSeleccionado)
                CargaRecibos(TipoMoneda.DOLAR);
        }




        #region PA2-01490-78K2 - KFC

        public void SeleccionarTodos()
        {
            for (int i = 0; i < lstRecibos.Count; i++)
            {
                lstRecibos[i].Seleccionado = true;
            }
            Items = new SimpleObservableCollection<Recibo>(lstRecibos);
            RaisePropertyChanged("Items");
        }

        public void SeleccionarNinguno()
        {
            for (int i = 0; i < lstRecibos.Count; i++)
            {
                lstRecibos[i].Seleccionado = false;
            }
            Items = new SimpleObservableCollection<Recibo>(lstRecibos);
            RaisePropertyChanged("Items");
        }

        #endregion

		#endregion     


    }
}