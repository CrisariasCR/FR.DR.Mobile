using System;
//using System.Net;
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
    public class SeleccionNotasCreditoViewModel : DialogViewModel<bool>
    {
#pragma warning disable 169
#pragma warning disable 414

        #region Propiedades

        #region Items e ItemActual
        private NotaCredito itemActual;
        public NotaCredito ItemActual
        {
            get { return itemActual; }
            set
            {
                    itemActual = value;                    
                    SeleccionarNotaCredito(value);
                    RaisePropertyChanged("ItemActual");
                
            }
        }

        private IObservableCollection<NotaCredito> items;
        public IObservableCollection<NotaCredito> Items {
            get{return items;}
            set { items = value; RaisePropertyChanged("Items"); }
        }
        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        private List<NotaCredito> ItemsSeleccionados
        {
            get
            {
                return new List<NotaCredito>(this.Items.Where<NotaCredito>(x => x.Selected));
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

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private DateTime fechaCreacionNC;
        public DateTime FechaCreacionNC
        {
            get { return fechaCreacionNC; }
            set
            {
                if (value != fechaCreacionNC)
                {
                    fechaCreacionNC = value;
                    RaisePropertyChanged("FechaCreacionNC");
                }
            }
        }

        private decimal montoNC;
        public decimal MontoNC
        {
            get { return montoNC; }
            set
            {
               montoNC = value;
               RaisePropertyChanged("MontoNC");
                
            }
        }

        private decimal totalNotasCredito;
        public decimal TotalNotasCredito
        {
            get { return totalNotasCredito; }
            set
            {
                    totalNotasCredito = value;
                    RaisePropertyChanged("TotalNotasCredito");
                
            }
        }

        private decimal totalSaldoFacturas;
        public decimal TotalSaldoFacturas
        {
            get { return totalSaldoFacturas; }
            set
            {
                if (value != totalSaldoFacturas)
                {
                    totalSaldoFacturas = value;
                    RaisePropertyChanged("TotalSaldoFacturas");
                }
            }
        }

        
        #endregion Propiedades

        //Para guardar los indices de las notas de credito selecciondas
        List<string> indicesMarcados = new List<string>();       
        private bool haySeleccionPrevia = false;

        public SeleccionNotasCreditoViewModel(string messageId)
            : base(messageId) 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            // signa la funcion para ejecutar cuando una factura es seleccionada
            NotaCredito.ItemCheck = ItemCheck;
            
            haySeleccionPrevia = indicesMarcados.Count > 0;

            try
            {
                List<NotaCredito> lista = GlobalUI.ClienteActual.ObtenerClienteCia(Cobros.Recibo.Compania).NotasCredito;

                Cobros.MontoNotasCreditoSelLocal = 0;

                this.Items = new SimpleObservableCollection<NotaCredito>(lista);
                RaisePropertyChanged("Items");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando notas del cliente. " + ex.Message);
            }

            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
            {
                this.TotalNotasCredito = Cobros.MontoNotasCreditoSelLocal;                
                this.TotalSaldoFacturas = Cobros.MontoPendiente;
            }
            else
            {
                this.TotalNotasCredito = Cobros.MontoNotasCreditoSelDolar;
                this.TotalSaldoFacturas = Cobros.MontoPendiente;
            }

            
            CargarMarcadas();            
        }

        private void ItemCheck(NotaCredito facChecked, bool newValueChecked)
        {
            //LJR 09/06/2009 Caso 35843: La forma de seleccionar una factura se modifica
            //Forzar el evento check como la seleccion con click para refrescar datos
            ItemActual = facChecked;           

            //Marcar la factura como seleccionada
            //this.SeleccionarFactura(facChecked);
            this.CalculaMontoTotalNotas();
            ValidarMonto();
        }

        #endregion CargaInicial

        #region Comandos

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Acepta); }
        }

        public ICommand ComandoSeleccionar
        {
            get { return new MvxRelayCommand(Seleccionar); }
        }

        #endregion Comandos

        #region Acciones

        /// <summary>
        /// Funcion que se ejecuta con el cambio de indice del
        /// listview muestra la informacion de la nota de credito seleccionada
        /// </summary>
        public void SeleccionarNotaCredito(NotaCredito itemActual)
        {
            NotaCredito nota;
            if (ItemActual != null)
            {
                if (ItemsSeleccionados.Count == 0)
                    this.MontoNC = 0;
                if (ItemsSeleccionados.Count == 1)
                {
                    nota = itemActual;
                    //Verifica que exista una linea valida seleccionada
                    if (nota != null)
                    {
                        // binding fechaObtenerFechaString
                        this.FechaCreacionNC = ((NotaCredito)nota).FechaRealizacion;

                        // binding fechaObtenerFechaString
                        if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                            this.MontoNC = ((NotaCredito)nota).SaldoDocLocal;
                        else
                            this.MontoNC = ((NotaCredito)nota).SaldoDocDolar;
                    }
                }
                else
                {
                    nota = itemActual;
                    //Verifica que exista una linea valida seleccionada
                    if (nota != null)
                    {
                        // binding fechaObtenerFechaString
                        this.FechaCreacionNC = ((NotaCredito)nota).FechaRealizacion;

                        // binding fechaObtenerFechaString
                        if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                            this.MontoNC = ((NotaCredito)nota).SaldoDocLocal;
                        else
                            this.MontoNC = ((NotaCredito)nota).SaldoDocDolar;
                    }
                    this.CalculaMontoTotalNotas();
                }
            }
            else
            {
                if (ItemsSeleccionados.Count > 0)
                {
                    if (ItemsSeleccionados.Count == 1)
                    {
                        nota = ItemsSeleccionados[0];
                        //Verifica que exista una linea valida seleccionada
                        if (nota != null)
                        {
                            // binding fechaObtenerFechaString
                            this.FechaCreacionNC = ((NotaCredito)nota).FechaRealizacion;

                            // binding fechaObtenerFechaString
                            if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                                this.MontoNC = ((NotaCredito)nota).SaldoDocLocal;
                            else
                                this.MontoNC = ((NotaCredito)nota).SaldoDocDolar;
                        }
                    }
                    else
                    {
                        this.CalculaMontoTotalNotas();
                    }
                }
                else
                {
                    this.MontoNC = 0;
                }
            }

        }

        public  void Seleccionar() 
        {            
            SeleccionarNotaCredito(ItemActual);
            ValidarMonto();
        }

        /// <summary>
        /// Muestra la pantalla de aplicar pago
        /// </summary>
        private void Acepta()
        {
                this.CalculaMontoTotalNotas();
                LlenarNotasSeleccionadas();

                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    Cobros.MontoNotasCreditoSelLocal += (decimal)this.TotalNotasCredito;
                else
                    Cobros.MontoNotasCreditoSelDolar += (decimal)this.TotalNotasCredito;

                //this.DoClose();
                //this.RequestNavigate<AplicarPagoViewModel>();
                this.ReturnResult(true);
            
        }

        public void Regresar() 
        {
            this.mostrarMensaje(Mensaje.Accion.Cancelar, " la seleccion", res =>
            {
                if (DialogResult.Yes == res)
                {
                    //this.DoClose();
                    //this.RequestNavigate<AplicarPagoViewModel>();
                    ReturnResult(false);
                }
            });            
        }

        #endregion Accioness
	

        #region Variables

        /// <summary>
        /// notas de credito a consultar
        /// </summary>
        private List<NotaCredito> notasCredito;


        /// <summary>
        /// Indica la factura selecccionada
        /// </summary>
        private NotaCredito notaSeleccionada = new NotaCredito();

        /// <summary>
        /// Indica si hay una factura seleccionada actualmente
        /// </summary>
        private bool esSeleccionada = false;

        



        #endregion

        #region Constructor

        //public frmSeleccionNotasCredito(ref ArrayList notasMarcadas)
        //{
        //    this.FormatTextBox();

        //    this.indicesMarcados = notasMarcadas;

        //}

		#endregion
		
		#region Funciones Usadas por los eventos

        
        /// <summary>
        /// Remarca las notas de credito que fueron marcadas previamente
        /// </summary>
        private void CargarMarcadas()
        {
            if (Cobros.notasSeleccionadas.Count > 0)
            {
                    foreach (NotaCredito item in this.Items)
                    {                        
                        foreach (NotaCredito nota in Cobros.notasSeleccionadas)
                        {
                            // añadir a lista de notas seleccionadas
                            if (nota.Numero == item.Numero)
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                    }
                    RaisePropertyChanged("Items");
                    this.CalculaMontoTotalNotas();  
                
            }
        }

        /// <summary>
        /// Saca el monto total que hay que pagar de las facturas seleccionadas
        /// </summary>
        /// <param name="esCheck">si el ultimo click chequeo un item</param>
        /// <param name="index">el item que fue chequeado o deschequeado</param>
        private void CalculaMontoTotalNotas()
        {
            decimal montoTotalNotas = 0;

            foreach (NotaCredito item in this.ItemsSeleccionados)
            {              

                //Validar contra el ultimo chequeo de item
                if (item.Selected == true)
                {
                    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                        montoTotalNotas += item.SaldoDocLocal;
                    else
                        montoTotalNotas += item.SaldoDocDolar;
                }
            }

            this.TotalNotasCredito = montoTotalNotas;

        }

        /// <summary>
        /// Recorre la lista guardando en un arreglo las notas de credito seleccionadas
        /// </summary>
        private void LlenarNotasSeleccionadas()
        {
            Cobros.notasSeleccionadas.Clear();

            foreach (NotaCredito nota in this.Items.Where<NotaCredito>(x => x.Selected))
            {
                // añadir a lista de notas seleccionadas
                Cobros.notasSeleccionadas.Add(nota);
            }
        }
        

        /// <summary>
        /// Funcion que valida y advierte al usuario que la seleccion de N/C sobrepasa el monto a pagar
        /// el usuario puede aceptar presionando SI y se aplicaran las N/C desde la mas antigua en adelante
        /// dentro de las que fueran seleccionadas. Si presiona NO se desmarcará la ultima N/C seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool ValidarMonto()
        {
            bool res = false;
            if (this.TotalNotasCredito > this.TotalSaldoFacturas)
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, " seleccionar la nota de crédito el monto seleccionado es mayor al saldo a cancelar",
                    result =>
                    {
                        if (result == DialogResult.No)
                        {
                            itemActual.Selected = false;
                            RaisePropertyChanged("ItemActual");
                            RaisePropertyChanged("Items");
                            this.MontoNC = 0;
                        }
                        else
                        {
                            res = true;
                        }
                    }
                );
                return res;
            }
            else
            {
                return true;
            }
            
        }

		#endregion
    }
}
