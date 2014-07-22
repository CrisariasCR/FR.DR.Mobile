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

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using System.Reflection;
using System.IO;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Seguridad;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Configuracion;

using System.Data.SQLite;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConexionTomFisicaViewModel : DialogViewModel<bool>
    {
#pragma warning disable 169

        public ConexionTomFisicaViewModel(string messageId,string pCompania, string pBodega)
            : base(messageId)
        {
            Compania = pCompania;
            BodegaCamion = pBodega;
            //txtUsuario.Focus();
            //CargarAplicacion();
        }

        #region Binding

        public string Version { get; set; }
        public string Footer { get; set; }

        private string nombreUsuario = string.Empty;
        public string NombreUsuario
        {
            get { return nombreUsuario; }
            set { nombreUsuario = value; RaisePropertyChanged("NombreUsuario"); }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged("Password"); }
        }

        private int numeroIntentos;
        private int intentosPermitidos;

        public bool UsuarioFocus{get;set;}
        public bool PasswordFocus { get; set; }

        #endregion

        #region Metodos

        private void Cancelar() 
        {
            this.ReturnResult(false);
        }

        private void Login() 
        {
            this.Conectar();
        }

        #endregion

        #region Comandos y Acciones

        public ICommand ComandoLogin
        {
            get { return new MvxRelayCommand(Login); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        #endregion

        #region mobile

        private string compania = string.Empty;

        public string Compania
        {
            get { return compania; }
            set { compania = value; }
        }
        private string bodega = string.Empty;

        public string BodegaCamion
        {
            get { return bodega; }
            set { bodega = value; }
        }

        protected bool Conectar()
        {
            bool procesoExitoso = true;
            bool tienePrivilegio = true;

            try
            {
                //Se valida que se tenga un código de usuario
                if (procesoExitoso)
                {
                    if (string.IsNullOrEmpty(NombreUsuario))
                    {
                        this.mostrarAlerta("Debe indicar un código de usuario válido.");
                        UsuarioFocus = true;
                        procesoExitoso = false;
                    }
                }

                //Se valida que se digite la clave del usuario
                if (procesoExitoso)
                {
                    if (string.IsNullOrEmpty(Password))
                    {
                        this.mostrarAlerta("Debe indicar una clave de acceso válida.");
                        PasswordFocus = true;
                        procesoExitoso = false;
                    }
                }

                //Se realiza la validación de los datos del usuario
                if (procesoExitoso)
                {
                    procesoExitoso = GestorSeguridad.AutenticarUsuario(NombreUsuario.ToUpper(),Password);

                    //Se valida si es la contraseña correcta
                    if (!procesoExitoso)
                    {
                        this.mostrarAlerta(String.Format("La contraseña indicada no coincide con la del usuario: '{0}'",NombreUsuario));
                    }
                }

                //Se valdia que el usuario tenga privilegios apra realizar toma fisica
                if (procesoExitoso)
                {
                    procesoExitoso = GestorSeguridad.VerificarPermiso(NombreUsuario.ToUpper(), Acciones.FR_TOMA_FISICA, out tienePrivilegio);
                }

                //Se valida el valor de retorno
                if (procesoExitoso)
                {
                    if (!tienePrivilegio)
                    {
                        this.mostrarAlerta(String.Format("El usuario: '{0}', no tiene privilegios para realizar toma física.", NombreUsuario));
                        procesoExitoso = false;
                    }
                }
                else
                {
                    this.mostrarAlerta(String.Format("No se logró validar los privilegios del usuario: '{0}'.", NombreUsuario));
                }

                //Si todo esta bien se ingresa a la pantalla de toma fisica
                if (procesoExitoso && tienePrivilegio)
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("pCompania", Compania);
                    parametros.Add("pBodega", BodegaCamion);
                    this.RequestDialogNavigate<TomaFisicaInventarioViewModel, bool>(parametros,
                         result =>
                         {

                             //Se recarga el inventario en la pantalla
                             ReturnResult(result);
                         });
                    //RequestNavigate<TomaFisicaInventarioViewModel>(parametros);
                    //this.DoClose();                     
                    //frmTomaFisicaInventario tomaFisica = new frmTomaFisicaInventario(Compania, BodegaCamion);
                    //tomaFisica.ShowDialog();
                    //this.DoClose();
                }

            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error al tratar de conectar a la toma física. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        #endregion
    }
}