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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using FR.Core.Model;

using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class MontosPedidoViewModel : BaseViewModel
    {

        public MontosPedidoViewModel(bool facturaConsignacion)
        {
            this.facturaConsignacion = facturaConsignacion;
            CargaInicial();
        }

        #region Propiedades

        private bool facturaConsignacion = false;

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; CambioCompania(); RaisePropertyChanged("CompaniaActual"); }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private string textoNota;
        public string TextoNota
        {
            get { return textoNota; }
            set { textoNota = value; RaisePropertyChanged("TextoNota"); }
        }

        private Pedido pedidoActual;
        public Pedido Pedido
        {
            get { return pedidoActual; }
            set { pedidoActual = value; RaisePropertyChanged("Pedido"); }
        }

        private string warning;
        public string Warning
        {
            get { return warning; }
            set { warning = value; RaisePropertyChanged("Warning"); }
        }

        private bool warningVisible = false;
        public bool WarningVisible
        {
            get { return warningVisible; }
            set { warningVisible = value; RaisePropertyChanged("WarningVisible"); }
        }

        private string labelImp1;
        public string LabelImpuesto1
        {
            get { return labelImp1; }
            set { labelImp1 = value; RaisePropertyChanged("LabelImpuesto1"); }
        }

        private string labelImp2;
        public string LabelImpuesto2
        {
            get { return labelImp2; }
            set { labelImp2 = value; RaisePropertyChanged("LabelImpuesto2"); }
        }

        private string labelNotas = "Nota a Pedido:";
        public string LabelNotas
        {
            get { return labelNotas; }
            set { labelNotas = value; RaisePropertyChanged("LabelNotas"); }
        }

        private object credito;
        public object Credito
        {
            get { return credito; }
            set { credito = value; RaisePropertyChanged("Credito"); }
        }


        #endregion

        #region Metodos Logica Negocio

        private void MostrarMontos()
        {
            Warning = FRmConfig.MensajeCreditoExcedido;
            if (this.facturaConsignacion)
                Pedido = Gestor.DesglosesConsignacion.Facturas.Buscar(CompaniaActual);
            else
                Pedido = Gestor.Pedidos.Buscar(CompaniaActual);

            ClienteCia cliente = GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual);

            Credito = cliente.LimiteCredito > 0? Credito = cliente.LimiteCredito : "NO";

            if (cliente.LimiteCredito > 0 && Pedido.MontoNeto > cliente.LimiteCredito)
            {
                WarningVisible = true;
            }

            LabelImpuesto1 = Pedido.Configuracion.Compania.Impuesto1Descripcion != ""
                ? Pedido.Configuracion.Compania.Impuesto1Descripcion + ":" : "Imp. Ventas:";

            LabelImpuesto2 = Pedido.Configuracion.Compania.Impuesto2Descripcion != ""
                ? Pedido.Configuracion.Compania.Impuesto2Descripcion + ":" : "Imp. Consumo:";
        }

        private void CargaInicial()
        {
            if (Pedidos.FacturarPedido)
            {
                LabelNotas = "Nota a Factura:";
            }

            if (this.facturaConsignacion)
            {
                Companias = new SimpleObservableCollection<string>(
                    Gestor.DesglosesConsignacion.Facturas.Gestionados.Select(x => x.Compania).ToList()
                    );
            }
            else
            {
                Companias = new SimpleObservableCollection<string>(
                    Gestor.Pedidos.Gestionados.Select(x => x.Compania).ToList()
                    );
            }
            CompaniaActual = Companias.Count > 0 ? Companias[0] : null;
        }

        private void CambioCompania()
        {
            if (!string.IsNullOrEmpty(CompaniaActual))
            {
                MostrarMontos();
                CargarNota();
            }
        }

        private void CargarNota()
        {
            Pedido pedido;
            if (this.facturaConsignacion)
                pedido = Gestor.DesglosesConsignacion.Facturas.Buscar(CompaniaActual);
            else
                pedido = Gestor.Pedidos.Buscar(CompaniaActual);

            if (pedido != null)
                TextoNota = pedido.Notas;
        }

        private void AgregarNota()
        {
            Pedido pedido;
            if (this.facturaConsignacion)
                pedido = Gestor.DesglosesConsignacion.Facturas.Buscar(CompaniaActual);
            else
                pedido = Gestor.Pedidos.Buscar(CompaniaActual);

            if (pedido != null)
                pedido.Notas = TextoNota;
        }

        #endregion

        #region Comandos y Acciones

        public ICommand ComandoGuardar
        {
            get { return new MvxRelayCommand(GuardarNota); }
        }

        private void GuardarNota()
        {
            if (TextoNota.Length != 0)
                AgregarNota();
            this.DoClose();
        }
        #endregion
    }
}