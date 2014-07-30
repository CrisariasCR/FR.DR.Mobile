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
    public class ReporteCierreCajaViewModel : BaseViewModel
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

        
        private string facContado;
        public string FacContado
        {
            get { return facContado; }
            set
            { facContado = value; RaisePropertyChanged("FacContado"); }
        }
        private string facFisContado;
        public string FacFisContado
        {
            get { return facFisContado; }
            set
            { facFisContado = value; RaisePropertyChanged("FacFisContado"); }
        }
        private string facCredito;
        public string FacCredito
        {
            get { return facCredito; }
            set
            { facCredito = value; RaisePropertyChanged("FacCredito"); }
        }
        private string facFisCredito;
        public string FacFisCredito
        {
            get { return facFisCredito; }
            set
            { facFisCredito = value; RaisePropertyChanged("FacFisCredito"); }
        }
        private string devContado;
        public string DevContado
        {
            get { return devContado; }
            set
            { devContado = value; RaisePropertyChanged("DevContado"); }
        }
        private string devCredito;
        public string DevCredito
        {
            get { return devCredito; }
            set
            { devCredito = value; RaisePropertyChanged("DevCredito"); }
        }
        private string devEfecContado;
        public string DevEfecContado
        {
            get { return devEfecContado; }
            set
            { devEfecContado = value; RaisePropertyChanged("DevEfecContado"); }
        }
        private string devEfecCredito;
        public string DevEfecCredito
        {
            get { return devEfecCredito; }
            set
            { devEfecCredito = value; RaisePropertyChanged("DevEfecCredito"); }
        }
        private string contTotal;
        public string ConTotal
        {
            get { return contTotal; }
            set
            { contTotal = value; RaisePropertyChanged("ConTotal"); }
        }

        private string creTotal;
        public string CreTotal
        {
            get { return creTotal; }
            set
            { creTotal = value; RaisePropertyChanged("RecTotal"); }
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

        private string totalCobro;
        public string TotalCobro
        {
            get { return totalCobro; }
            set
            { totalCobro = value; RaisePropertyChanged("TotalCobro"); }
        }

        private string total;
        public string Total
        {
            get { return total; }
            set
            { total = value; RaisePropertyChanged("Total"); }
        }
        
        private string depTotal;
        public string DepTotal
        {
            get { return depTotal; }
            set
            { depTotal = value; RaisePropertyChanged("DepTotal"); }
        }

        private string garContado;
        public string GarContado
        {
            get { return garContado; }
            set
            { garContado = value; RaisePropertyChanged("GarContado"); }
        }

        private string garCredito;
        public string GarCredito
        {
            get { return garCredito; }
            set
            { garCredito = value; RaisePropertyChanged("GarCredito"); }
        }

        /// <summary>
        /// Guarda la ruta seleccionada en la seleccion de clientes
        /// </summary>
        private static int rutaSeleccionada;

        private static string rutaCodigoSeleccionada;

        private static JornadaRuta jornada;

        #endregion Propiedades

        public ReporteCierreCajaViewModel() 
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
                    this.DevContado = GestorUtilitario.FormatNumero(0);
                    this.DevCredito = GestorUtilitario.FormatNumero(jornada.MontoDevolucionesLocal.Value);
                    this.DevEfecContado = GestorUtilitario.FormatNumero(jornada.MontoDevolucionesEfectivoLocal.Value);
                    this.DevEfecCredito = GestorUtilitario.FormatNumero(0);
                    this.FacContado = GestorUtilitario.FormatNumero(jornada.MontoFacturasLocalCont.Value);
                    this.FacCredito = GestorUtilitario.FormatNumero(jornada.MontoFacturasLocalCre.Value);
                    this.FacFisContado = GestorUtilitario.FormatNumero(jornada.MontoFacturasTomaFisicaLocalCont.Value);
                    this.FacFisCredito = GestorUtilitario.FormatNumero(jornada.MontoFacturasTomaFisicaLocalCre.Value);
                    this.GarContado = GestorUtilitario.FormatNumero(jornada.MontoGarantiasLocalCont.Value);
                    this.GarCredito = GestorUtilitario.FormatNumero(jornada.MontoGarantiasLocalCre.Value);
                    this.ConTotal = GestorUtilitario.FormatNumero(jornada.MontoFacturasLocalCont.Value + jornada.MontoFacturasTomaFisicaLocalCont.Value + jornada.MontoGarantiasLocalCont.Value);
                    this.CreTotal = GestorUtilitario.FormatNumero(jornada.MontoFacturasLocalCre.Value + jornada.MontoFacturasTomaFisicaLocalCre.Value + jornada.MontoGarantiasLocalCre.Value);
                    
                    //Cobros
                    this.EfecTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosEfectivoLocal.Value);
                    this.ChequesTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosChequeLocal.Value);
                    this.NcTotal = GestorUtilitario.FormatNumero(jornada.MontoCobrosNotaCreditoLocal.Value);
                    this.TotalCobro = GestorUtilitario.FormatNumero((jornada.MontoCobrosNotaCreditoLocal + jornada.MontoCobrosChequeLocal + jornada.MontoCobrosEfectivoLocal).Value);                    
                    this.DepTotal = GestorUtilitario.FormatNumero(jornada.MontoDepositosLocal.Value);
                    this.Total = GestorUtilitario.FormatNumero((jornada.MontoCobrosNotaCreditoLocal + jornada.MontoCobrosChequeLocal + jornada.MontoCobrosEfectivoLocal ).Value - (jornada.MontoFacturasLocalCont.Value + jornada.MontoFacturasTomaFisicaLocalCont.Value + jornada.MontoGarantiasLocalCont.Value) - jornada.MontoDevolucionesEfectivoLocal.Value - jornada.MontoDepositosLocal.Value);
                    
                }
                else
                {
                    this.DevContado = GestorUtilitario.FormatNumeroDolar(0);
                    this.DevCredito = GestorUtilitario.FormatNumeroDolar(jornada.MontoDevolucionesDolar.Value);
                    this.DevEfecContado = GestorUtilitario.FormatNumeroDolar(jornada.MontoDevolucionesEfectivoDolar.Value);
                    this.DevEfecCredito = GestorUtilitario.FormatNumeroDolar(0);
                    this.FacContado = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasDolarCont.Value);
                    this.FacCredito = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasDolarCre.Value);
                    this.FacFisContado = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasTomaFisicaDolarCont.Value);
                    this.FacFisCredito = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasTomaFisicaDolarCre.Value);
                    this.GarContado = GestorUtilitario.FormatNumeroDolar(jornada.MontoGarantiasDolarCont.Value);
                    this.GarCredito = GestorUtilitario.FormatNumeroDolar(jornada.MontoGarantiasDolarCre.Value);
                    this.ConTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasDolarCont.Value + jornada.MontoFacturasTomaFisicaDolarCont.Value + jornada.MontoGarantiasDolarCont.Value);
                    this.CreTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoFacturasDolarCre.Value + jornada.MontoFacturasTomaFisicaDolarCre.Value + jornada.MontoGarantiasDolarCre.Value);

                    //Cobros
                    this.EfecTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosEfectivoDolar.Value);
                    this.ChequesTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosChequeDolar.Value);
                    this.NcTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoCobrosNotaCreditoDolar.Value);
                    this.TotalCobro = GestorUtilitario.FormatNumeroDolar((jornada.MontoCobrosNotaCreditoDolar + jornada.MontoCobrosChequeDolar + jornada.MontoCobrosEfectivoDolar).Value);
                    this.DepTotal = GestorUtilitario.FormatNumeroDolar(jornada.MontoDepositosDolar.Value);
                    this.Total = GestorUtilitario.FormatNumeroDolar((jornada.MontoCobrosNotaCreditoDolar + jornada.MontoCobrosChequeDolar + jornada.MontoCobrosEfectivoDolar).Value - (jornada.MontoFacturasDolarCont.Value + jornada.MontoFacturasTomaFisicaDolarCont.Value + jornada.MontoGarantiasDolarCont.Value) - jornada.MontoDevolucionesEfectivoDolar.Value - jornada.MontoDepositosDolar.Value);
                }
            }
            else
            {
                LimpiarCampos();
            }
        }

        private void Imprime()
        {
            this.mostrarMensaje(Mensaje.Accion.Imprimir,"el reporte de cierre de caja.",resp =>
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
            this.DevContado = "0";
            this.DevCredito = "0";
            this.FacContado = "0";
            this.FacCredito = "0";
            this.ConTotal = "0";
            this.CreTotal = "0";
            this.EfecTotal = "0";
            this.ChequesTotal = "0";
            this.NcTotal = "0";
            this.Total = "0";
            this.TotalCobro = "0";
            this.DepTotal = "0";
            this.DevEfecContado = "0";
            this.DevEfecCredito = "0";
            this.GarContado = "0";
            this.GarCredito = "0";
        }
        #endregion
    }
}
