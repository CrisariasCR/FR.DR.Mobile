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

using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;
using FR.Core.Model;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaClienteViewModel : BaseViewModel
    {
        public ConsultaClienteViewModel()
        {   
            Cliente = GlobalUI.ClienteActual;
            CambioCompania();
        }

        public static bool SeleccionCliente;

        private void CambioCompania()
        {
            GlobalUI.Rutas = Cls.Corporativo.Ruta.ObtenerRutas();
            GlobalUI.RutaActual = Cls.Corporativo.Ruta.ObtenerRuta(GlobalUI.Rutas, Cliente.Zona);

            Companias = new SimpleObservableCollection<ClienteCia>(Util.CargarCiasClienteActual());
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }

            List<Cls.Dias> dias = GlobalUI.ClienteActual.ObtenerDiasVisita(GlobalUI.RutaActual.Codigo);
            Dias = GestorUtilitario.ListaDias(dias);

            ClienteCia = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual.Compania);
            LimiteCredito = ClienteCia.LimiteCredito > 0 ? GestorUtilitario.FormatNumero(ClienteCia.LimiteCredito) : "Sin Límite de Crédito";

            ExoneracionImp1 = GestorUtilitario.Formato(ClienteCia.ExoneracionImp1) + "%";
            ExoneracionImp2 = GestorUtilitario.Formato(ClienteCia.ExoneracionImp2) + "%";

            FacturasPendientes = (Cliente.TieneFacturasPendientes ? "Si" : "No");
            DescGeneral = GestorUtilitario.Formato(ClienteCia.Descuento) + "%";
        }

        #region Propiedades

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

        public Cliente Cliente { get; set; }
        public ClienteCia ClienteCia { get; set; }

        private string dias;
        public string Dias
        {
            get { return dias; }
            set { dias = value; RaisePropertyChanged("Dias"); }
        }

        private string limiteCredito;
        public string LimiteCredito
        {
            get { return limiteCredito; }
            set { limiteCredito = value; RaisePropertyChanged("LimiteCredito"); }
        }

        private string exonera1;
        public string ExoneracionImp1
        {
            get { return exonera1; }
            set { exonera1 = value; RaisePropertyChanged("ExoneracionImp1"); }
        }

        private string exonera2;
        public string ExoneracionImp2
        {
            get { return exonera2; }
            set { exonera2 = value; RaisePropertyChanged("ExoneracionImp2"); }
        }

        private string facturasPendientes;
        public string FacturasPendientes
        {
            get { return facturasPendientes; }
            set { facturasPendientes = value; RaisePropertyChanged("FacturasPendientes"); }
        }

        private string descGeneral;
        public string DescGeneral
        {
            get { return descGeneral; }
            set { descGeneral = value; RaisePropertyChanged("DescGeneral"); }
        }

        public void Regresar()
        {

            if (!SeleccionCliente)
            {
                    this.DoClose();
                    RequestNavigate<MenuClienteViewModel>();
            }
            else
            {
                this.DoClose();
            }
        }

        #endregion
    }
}