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
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class NotaVentaConsignacionViewModel : DialogViewModel<DialogResult>
    {
        public NotaVentaConsignacionViewModel(string compania)
            : base(null)
        {
            CompaniaActual = compania;
            CargarVentana();
        }

        #region Comandos

        public ICommand ComandoNotas
        {
            get { return new MvxRelayCommand(EditarNotas); }
        }

        public void EditarNotas() 
        {
            if (!string.IsNullOrEmpty(CompaniaActual))
            {
                //frmNotaVentaConsignacion nota = new frmNotaVentaConsignacion(this.cboCompannias.SelectedItem.ToString());
                //nota.ShowDialog();
                //nota.Close();
                //Caso:32842 ABC 19/06/2008 Se deja de utiliza el dispose debido a que libera de memoria propiedas de los componentes
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
            }
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

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                if (value != companiaActual)
                {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                }
            }
        }

        private string notas;
        public string Notas
        {
            get { return notas; }
            set
            {
                if (value != notas)
                {
                    notas = value;
                    RaisePropertyChanged("Notas");
                }
            }
        }
     

        #endregion

        #region Metodos

        private void CargarVentana()
        {
            this.NombreCliente= " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                                         " Cliente: " + GlobalUI.ClienteActual.Nombre;
            //this.lblNombreCliente.Visible = false;        

            this.CargarNota();
        }

        /// <summary>
        /// Método encargado de cargar la nota asociada al encabezado de la venta en consignación.
        /// </summary>
        private void CargarNota()
        {
            VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(CompaniaActual);

            if (ventaConsignacion != null)
                this.Notas = ventaConsignacion.Notas;
        }

        /// <summary>
        /// Asigna la nota indicada al encabezado de la venta en consignación.
        /// </summary>
        public void AsignarNota()
        {
            VentaConsignacion ventaConsignacion = Gestor.Consignaciones.Buscar(CompaniaActual);

            if (ventaConsignacion != null)
                ventaConsignacion.Notas = this.Notas;
        }

        #endregion


    }
}