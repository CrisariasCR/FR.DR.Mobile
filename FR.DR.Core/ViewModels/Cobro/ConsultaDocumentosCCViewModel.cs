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

using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaDocumentosCCViewModel : ListViewModel
    {
        public static bool SeleccionCliente;

        public ConsultaDocumentosCCViewModel(int tipo)
        {
            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());
            TipoMoneda = TipoMoneda.LOCAL;
            TipoDocumento = (TipoDocumento)tipo;

            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }
        }

        #region Propiedades

        public TipoDocumento TipoDocumento;

        private ClienteCia companiaActual;
        public ClienteCia CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                if (value != companiaActual)
                {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                    CambioCompania();
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

        public IObservableCollection<ClienteCia> Companias { get; set; }

        public IObservableCollection<DocumentoContable> Documentos { get; set; }

        private TipoMoneda tipoMoneda;
        public TipoMoneda TipoMoneda 
        {
            get { return tipoMoneda; }
            set { tipoMoneda = value; RaisePropertyChanged("TipoMoneda"); }
        }

        private decimal saldoLocal;
        public decimal SaldoLocal
        {
            get { return saldoLocal; }
            set { saldoLocal = value; RaisePropertyChanged("SaldoLocal"); }
        }

        private decimal saldoDolar;
        public decimal SaldoDolar
        {
            get { return saldoDolar; }
            set { saldoDolar = value; RaisePropertyChanged("SaldoDolar"); }
        }

        private decimal montoLocal;
        public decimal MontoLocal
        {
            get { return montoLocal; }
            set { montoLocal = value; RaisePropertyChanged("MontoLocal"); }
        }

        private decimal montoDolar;
        public decimal MontoDolar
        {
            get { return montoDolar; }
            set { montoDolar = value; RaisePropertyChanged("MontoDolar"); }
        }

        #endregion

        #region Metodos

        private void CambioCompania()
        {
            Documentos = new SimpleObservableCollection<DocumentoContable>();

            if (TipoDocumento == TipoDocumento.Factura)
            {
                var facs = Factura.ObtenerFacturasPendientesCobro(CompaniaActual.Compania, GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona);
                Documentos = new SimpleObservableCollection<DocumentoContable>(facs.ConvertAll<DocumentoContable>(x => (DocumentoContable)x));
            }
            else
            {
                var notas = NotaCredito.ObtenerNotasCredito(CompaniaActual.Compania, GlobalUI.ClienteActual.Codigo, GlobalUI.ClienteActual.Zona);
                Documentos = new SimpleObservableCollection<DocumentoContable>(notas.ConvertAll<DocumentoContable>(x => (DocumentoContable)x));
            }

            RaisePropertyChanged("Documentos");

            MontoLocal = Documentos.Sum(x => x.MontoDocLocal);
            SaldoLocal = Documentos.Sum(x => x.SaldoDocLocal);
            MontoDolar = Documentos.Sum(x => x.MontoDocDolar);
            SaldoDolar = Documentos.Sum(x => x.SaldoDocDolar);
        }

        public override void DoClose()
        {
            if (!SeleccionCliente)
            {
                base.DoClose();
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                                {
                                    {"habilitarPedidos", true}
                                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
            }
            else
            {
                base.DoClose();            
                //this.RequestNavigate<OpcionesClienteViewModel>();
                
            }
        }

        #endregion
    }
}