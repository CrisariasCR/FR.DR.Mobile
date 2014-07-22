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
    public class NotaDetalleDesgloseViewModel : DialogViewModel<bool>
    {
        public NotaDetalleDesgloseViewModel(string notaDetalle)
            : base(null)
        {
            //CompaniaActual = compania;
            //CargarVentana();
            NotaDetalleDesgloseViewModel.Observacion = notaDetalle.Trim();
            Notas=notaDetalle;
        }

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

        public static string Observacion;

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


    }
}