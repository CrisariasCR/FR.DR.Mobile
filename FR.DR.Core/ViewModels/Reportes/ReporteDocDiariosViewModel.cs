using System;
//using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ReporteDocDiariosViewModel : BaseViewModel
    {

#pragma warning disable 169

        #region Propiedades

        #region Combos

        private List<Ruta> rutasCombo = new List<Ruta>();
        public IObservableCollection<string> Rutas { get; set; }
        private string rutaActuals;
        public string RutaActuals
        {
            get { return rutaActuals; }
            set
            {
                rutaActuals = value;
                this.SeleccionRuta();
            }
        }
        private Ruta rutaActual;
        public Ruta RutaActual
        {
            get { return rutaActual; }
            set
            {
                rutaActual = value;
            }
        }

        public IObservableCollection<string> TipoDoc { get; set; }
        private string tipoActual;
        public string TipoActual
        {
            get { return tipoActual; }
            set
            {
                tipoActual = value; RaisePropertyChanged("TipoActual"); cboTipoDoc_SelectedIndexChanged();
            }
        }

        #endregion

        private string pedCantidad;
        public string PedCantidad
        {
            get { return pedCantidad; }
            set
            { pedCantidad = value; RaisePropertyChanged("PedCantidad"); }
        }
        private string pedTotal;
        public string PedTotal
        {
            get { return pedTotal; }
            set
            { pedTotal = value; RaisePropertyChanged("PedTotal"); }
        }
        private string facCantidad;
        public string FacCantidad
        {
            get { return facCantidad; }
            set
            { facCantidad = value; RaisePropertyChanged("FacCantidad"); }
        }
        private string facFisCantidad;
        public string FacFisCantidad
        {
            get { return facFisCantidad; }
            set
            { facFisCantidad = value; RaisePropertyChanged("FacFisCantidad"); }
        }
        private string facTotal;
        public string FacTotal
        {
            get { return facTotal; }
            set
            { facTotal = value; RaisePropertyChanged("FacTotal"); }
        }
        private string facFisTotal;
        public string FacFisTotal
        {
            get { return facFisTotal; }
            set
            { facFisTotal = value; RaisePropertyChanged("FacFisTotal"); }
        }
        private string devCantidad;
        public string DevCantidad
        {
            get { return devCantidad; }
            set
            { devCantidad = value; RaisePropertyChanged("DevCantidad"); }
        }
        private string devTotal;
        public string DevTotal
        {
            get { return devTotal; }
            set
            { devTotal = value; RaisePropertyChanged("DevTotal"); }
        }
        private string devEfecCantidad;
        public string DevEfecCantidad
        {
            get { return devEfecCantidad; }
            set
            { devEfecCantidad = value; RaisePropertyChanged("DevEfecCantidad"); }
        }
        private string devEfecTotal;
        public string DevEfecTotal
        {
            get { return devEfecTotal; }
            set
            { devEfecTotal = value; RaisePropertyChanged("DevEfecTotal"); }
        }
        private string recCantidad;
        public string RecCantidad
        {
            get { return recCantidad; }
            set
            { recCantidad = value; RaisePropertyChanged("RecCantidad"); }
        }
        private string recTotal;
        public string RecTotal
        {
            get { return recTotal; }
            set
            { recTotal = value; RaisePropertyChanged("RecTotal"); }
        }
        private string efecTotal;
        public string EfecTotal
        {
            get { return efecTotal; }
            set
            { efecTotal = value; RaisePropertyChanged("EfecTotal"); }
        }
        private string chequesTotal;
        public string ChequesTotal
        {
            get { return chequesTotal; }
            set
            { chequesTotal = value; RaisePropertyChanged("ChequesTotal"); }
        }
        private string ncTotal;
        public string NcTotal
        {
            get { return ncTotal; }
            set
            { ncTotal = value; RaisePropertyChanged("NcTotal"); }
        }
        private string total;
        public string Total
        {
            get { return total; }
            set
            { total = value; RaisePropertyChanged("Total"); }
        }
        private string depCantidad;
        public string DepCantidad
        {
            get { return depCantidad; }
            set
            { depCantidad = value; RaisePropertyChanged("DepCantidad"); }
        }
        private string depTotal;
        public string DepTotal
        {
            get { return depTotal; }
            set
            { depTotal = value; RaisePropertyChanged("DepTotal"); }
        }

        private string garCantidad;
        public string GarCantidad
        {
            get { return garCantidad; }
            set
            { garCantidad = value; RaisePropertyChanged("GarCantidad"); }
        }
        private string garTotal;
        public string GarTotal
        {
            get { return garTotal; }
            set
            { garTotal = value; RaisePropertyChanged("GarTotal"); }
        }

        /// <summary>
        /// Guarda la ruta seleccionada en la seleccion de clientes
        /// </summary>
        private static int rutaSeleccionada;

        private static string rutaCodigoSeleccionada;

        private static JornadaRuta jornada;

        #endregion Propiedades

        public ReporteDocDiariosViewModel() 
        {
            List<string> tipoDocs = new List<string>();
            tipoDocs.Add("Local");
            tipoDocs.Add("Dolar");
            TipoDoc = new SimpleObservableCollection<string>(tipoDocs);
            TipoActual = TipoDoc[0];
            this.CargarRutas();
            
        }

        #region Comandos y Acciones

        public ICommand ComandoImprimir
        {
            get { return new MvxRelayCommand(Imprime); }
        }

        #endregion Accioness

        #region mobile


        /// <summary>
        /// Carga las rutas asociadas al pda.
        /// </summary>
        private void CargarRutas()
        {
            List<string> rutas = new List<string>();
            rutas.Clear();
            rutasCombo.Clear();
            rutas.Add("-Ruta-");
            Rutas = new SimpleObservableCollection<string>(rutas);

            foreach (Ruta ruta in GlobalUI.Rutas)
            {
                rutas.Add(ruta.Codigo);
                rutasCombo.Add(ruta);
            }

            if (rutas.Count == 2)
            {
                Rutas = new SimpleObservableCollection<string>(rutas);
                RutaActuals = Rutas[1];
                RutaActual = rutasCombo.Find(x => x.Codigo == RutaActuals);                
            }
            else
                RutaActuals = Rutas[0];
        }

        /// <summary>
        /// Metodo que se ejecuta cuando se selecciona una ruta del combo.
        /// </summary>
        private void SeleccionRuta()
        {
            //Le asigna el indice de la ruta seleccionada a la variable global
            if (RutaActuals != null && RutaActuals != "-Ruta-")
            {
                rutaCodigoSeleccionada = GlobalUI.Rutas.Find(x=>x.Codigo==RutaActuals).Codigo;

                cargarJornada();
                mostrarValores();
            }
            else
            {
                jornada = null;
                rutaCodigoSeleccionada = String.Empty;
                cargarJornada();
                mostrarValores();
            }
        }

        private void cargarJornada()
        {
            if (!String.IsNullOrEmpty(rutaCodigoSeleccionada))
                jornada = JornadaRuta.ObtenerJornada(rutaCodigoSeleccionada, DateTime.Now.Date);
        }

        private void mostrarValores()
        {
            if (jornada != null)
            {
                if (TipoActual.Equals("Local"))
                {
                    this.PedCantidad = GestorUtilitario.Formato(jornada.PedidosLocal.Value);
                    this.PedTotal = GestorUtilitario.FormatNumero(jornada.MontoPedidosLocal.Value);
                    this.DevCantidad = GestorUtilitario.Formato(jornada.DevolucionesLocal.Value);
                    this.DevTotal = GestorUtilitario.FormatNumero(jornada.MontoDevolucionesLocal.Value);
                    this.FacCantidad = GestorUtilitario.Formato(jornada.FacturasLocal.Value);
                    this.FacTotal = GestorUtilitario.FormatNumero(jornada.MontoFacturasLocal.Value);
                    this.FacFisCantidad = GestorUtilitario.Formato(jornada.FacturasTomaFisicaLocal.Value);
                    this.FacFisTotal = GestorUtilitario.FormatNumero(jornada.MontoFacturasTomaFisicaLocal.Value);
                    this.RecCantidad = GestorUtilitario.Formato(jornada.CobrosLocal.Value);
                    this.RecTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosLocal.Value);

                    //Facturas de contado y recibos en FR - KFC
                    // Se resta al monto efectivo de la jornada el monto de las devoluciones en efectivo realizadas
                    //ltbMontoEfectivo.Text = GestorUtilitario.FormatNumero(jornada.MontoCobrosEfectivoLocal.Value);
                    //ltbMontoEfectivo.Value = (jornada.MontoCobrosEfectivoLocal.Value - jornada.MontoDevolucionesEfectivoLocal.Value);
                    this.EfecTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosEfectivoLocal.Value - jornada.MontoDevolucionesEfectivoLocal.Value);
                    //if (ltbMontoEfectivo.Value < 0) { ltbMontoEfectivo.ForeColor = Color.Red; }
                    this.DevEfecCantidad = GestorUtilitario.Formato(jornada.DevolucionesEfectivoLocal.Value);
                    this.DevEfecCantidad = GestorUtilitario.FormatNumero(jornada.MontoDevolucionesEfectivoLocal.Value);

                    this.ChequesTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosChequeLocal.Value);
                    this.NcTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosNotaCreditoLocal.Value);
                    this.Total = GestorUtilitario.FormatNumero((jornada.MontoCobrosNotaCreditoLocal + jornada.MontoCobrosChequeLocal + jornada.MontoCobrosEfectivoLocal - jornada.MontoDevolucionesEfectivoLocal).Value + jornada.MontoGarantiasLocal.Value);
                    this.DepCantidad = GestorUtilitario.Formato(jornada.DepositosLocal.Value);
                    this.DepTotal = GestorUtilitario.FormatNumero(jornada.MontoDepositosLocal.Value);
                    this.GarCantidad = GestorUtilitario.Formato(jornada.GarantiasLocal.Value);
                    this.GarTotal = GestorUtilitario.FormatNumero(jornada.MontoGarantiasLocal.Value);
                }
                else
                {
                    this.PedCantidad = GestorUtilitario.Formato(jornada.PedidosDolar.Value);
                    this.PedTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoPedidosDolar.Value);
                    this.DevCantidad = GestorUtilitario.Formato(jornada.DevolucionesDolar.Value);
                    this.DevTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoDevolucionesDolar.Value);
                    this.FacCantidad = GestorUtilitario.Formato(jornada.FacturasDolar.Value);
                    this.FacTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasDolar.Value);
                    this.FacFisCantidad = GestorUtilitario.Formato(jornada.FacturasTomaFisicaDolar.Value);
                    this.FacFisTotal = GestorUtilitario.FormatNumero(jornada.MontoFacturasTomaFisicaDolar.Value);
                    this.RecCantidad = GestorUtilitario.Formato(jornada.CobrosDolar.Value);
                    this.RecTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosDolar.Value);

                    //Facturas de contado y recibos en FR - KFC
                    // Se resta al monto efectivo de la jornada el monto de las devoluciones en efectivo realizadas
                    // ltbMontoEfectivo.Text = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosEfectivoDolar.Value);
                   // ltbMontoEfectivo.Value = (jornada.MontoCobrosEfectivoLocal.Value - jornada.MontoDevolucionesEfectivoLocal.Value);
                    this.EfecTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosEfectivoDolar.Value - jornada.MontoDevolucionesEfectivoDolar.Value);
                    //if (ltbMontoEfectivo.Value < 0) { ltbMontoEfectivo.ForeColor = Color.Red; }
                    this.DevEfecCantidad = GestorUtilitario.Formato(jornada.DevolucionesEfectivoDolar.Value);
                    this.DevEfecTotal = GestorUtilitario.FormatNumero(jornada.MontoDevolucionesEfectivoDolar.Value);

                    this.ChequesTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosChequeDolar.Value);
                    this.NcTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosNotaCreditoDolar.Value);
                    this.Total = GestorUtilitario.FormatNumeroDolar((jornada.MontoCobrosNotaCreditoDolar + jornada.MontoCobrosChequeDolar + jornada.MontoCobrosEfectivoDolar - jornada.MontoDevolucionesEfectivoDolar).Value + jornada.MontoGarantiasDolar.Value);
                    this.DepCantidad = GestorUtilitario.Formato(jornada.DepositosDolar.Value);
                    this.DepTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoDepositosDolar.Value);
                    this.GarCantidad = GestorUtilitario.Formato(jornada.GarantiasDolar.Value);
                    this.GarTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoGarantiasDolar.Value);
                }
            }
            else
            {
                LimpiarCampos();
            }
        }

        private void Imprime()
        {
            this.mostrarMensaje(Mensaje.Accion.Imprimir,"el reporte de liquidación de Jornada.",resp =>
                {
                    if (resp == DialogResult.Yes)
                    {
                        if (jornada != null)
                        {
                            ReporteJornada reporte = null;
                            reporte = new ReporteJornada(jornada);
                            reporte.Imprime();
                        }
                        else
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "El reporte no tiene información.");
                    }
                });

            
        }

        private void cboTipoDoc_SelectedIndexChanged()
        {
            cargarJornada();
            mostrarValores();
        }

        private void LimpiarCampos()
        {
            this.PedCantidad = "0";
            this.PedTotal = "0";
            this.DevCantidad = "0";
            this.DevTotal = "0";
            this.FacCantidad = "0";
            this.FacTotal = "0";
            this.RecCantidad = "0";
            this.RecTotal = "0";
            this.EfecTotal = "0";
            this.ChequesTotal = "0";
            this.NcTotal = "0";
            this.Total = "0";
            this.DepCantidad = "0";
            this.DepTotal = "0";

            //KFC Recibos>
            this.DevEfecCantidad = "0";
            this.DevEfecTotal = "0";
        }
        #endregion
    }
}
