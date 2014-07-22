using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.ViewModels;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRJornada;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class RubrosJornadaViewModel : BaseViewModel
    {

        #region Propiedades

        private Jornada jornada=new Jornada();

        private string rubro1Des;
        public string Rubro1Des
        {
            get { return rubro1Des; }
            set
            {
                    rubro1Des = value;
                    RaisePropertyChanged("Rubro1Des");
            }
        }

        private string rubro2Des;
        public string Rubro2Des
        {
            get { return rubro2Des; }
            set
            {
                rubro2Des = value;
                RaisePropertyChanged("Rubro2Des");
            }
        }

        private string rubro3Des;
        public string Rubro3Des
        {
            get { return rubro3Des; }
            set
            {
                rubro3Des = value;
                RaisePropertyChanged("Rubro3Des");
            }
        }

        private string rubro4Des;
        public string Rubro4Des
        {
            get { return rubro4Des; }
            set
            {
                rubro4Des = value;
                RaisePropertyChanged("Rubro4Des");
            }
        }

        private string rubro5Des;
        public string Rubro5Des
        {
            get { return rubro5Des; }
            set
            {
                rubro5Des = value;
                RaisePropertyChanged("Rubro5Des");
            }
        }

        private bool rubro1Enabled;
        public bool Rubro1Enabled
        {
            get { return rubro1Enabled; }
            set
            {
                rubro1Enabled = value;
                RaisePropertyChanged("Rubro1Enabled");
               
            }
        }

        private bool rubro2Enabled;
        public bool Rubro2Enabled
        {
            get { return rubro2Enabled; }
            set
            {
                rubro2Enabled = value;
                RaisePropertyChanged("Rubro2Enabled");

            }
        }

        private bool rubro3Enabled;
        public bool Rubro3Enabled
        {
            get { return rubro3Enabled; }
            set
            {
                rubro3Enabled = value;
                RaisePropertyChanged("Rubro3Enabled");

            }
        }

        private bool rubro4Enabled;
        public bool Rubro4Enabled
        {
            get { return rubro4Enabled; }
            set
            {
                rubro4Enabled = value;
                RaisePropertyChanged("Rubro4Enabled");

            }
        }

        private bool rubro5Enabled;
        public bool Rubro5Enabled
        {
            get { return rubro5Enabled; }
            set
            {
                rubro5Enabled = value;
                RaisePropertyChanged("Rubro5Enabled");

            }
        }

        public string Rubro1
        {
            get { return jornada.Rubro1; }
            set
            {
                jornada.Rubro1 = value;
                RaisePropertyChanged("Rubro1");
            }
        }

        public string Rubro2
        {
            get { return jornada.Rubro2; }
            set
            {
                jornada.Rubro2 = value;
                RaisePropertyChanged("Rubro2");
            }
        }

        public string Rubro3
        {
            get { return jornada.Rubro3; }
            set
            {
                jornada.Rubro3 = value;
                RaisePropertyChanged("Rubro3");
            }
        }

        public string Rubro4
        {
            get { return jornada.Rubro4; }
            set
            {
                jornada.Rubro4 = value;
                RaisePropertyChanged("Rubro4");
            }
        }

        public string Rubro5
        {
            get { return jornada.Rubro5; }
            set
            {
                jornada.Rubro5 = value;
                RaisePropertyChanged("Rubro5");
            }
        }

        #endregion

        #region Constructor

        public RubrosJornadaViewModel()
        {
            CargaInicial();
            jornada = new Jornada();
        }

        #endregion

        #region Métodos

        public void Aceptar() 
        {
            if (Validar())
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, " continuar con estos rubros, no los podrá cambiar más tarde", res =>
                    {
                        if (res == System.Windows.Forms.DialogResult.Yes || res == System.Windows.Forms.DialogResult.OK)
                        {
                            jornada.Guardar();
                            this.DoClose();
                            this.RequestNavigate<MenuPrincipalViewModel>(true);
                        }
                    }
                    );
            }
            else
            {
                this.mostrarAlerta("Debe llenar los rubros antes de continuar");
            }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public void CargaInicial() 
        {
            if (string.IsNullOrEmpty(FRdConfig.Rubro1Jornada))
            {
                Rubro1Des = "Etiqueta 1";
                Rubro1Enabled = false;
            }
            else
            {
                Rubro1Des = FRdConfig.Rubro1Jornada;
                Rubro1Enabled = true;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro2Jornada))
            {
                Rubro2Des = "Etiqueta 2";
                Rubro2Enabled = false;
            }
            else
            {
                Rubro2Des = FRdConfig.Rubro2Jornada;
                Rubro2Enabled = true;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro3Jornada))
            {
                Rubro3Des = "Etiqueta 3";
                Rubro3Enabled = false;
            }
            else
            {
                Rubro3Des = FRdConfig.Rubro3Jornada;
                Rubro3Enabled = true;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro4Jornada))
            {
                Rubro4Des = "Etiqueta 4";
                Rubro4Enabled = false;
            }
            else
            {
                Rubro4Des = FRdConfig.Rubro4Jornada;
                Rubro4Enabled = true;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro5Jornada))
            {
                Rubro5Des = "Etiqueta 5";
                Rubro5Enabled = false;
            }
            else
            {
                Rubro5Des = FRdConfig.Rubro5Jornada;
                Rubro5Enabled = true;
            }
        }

        public bool Validar() 
        {
            bool res = false;
            bool res1 = !Rubro1Enabled || (!string.IsNullOrEmpty(Rubro1) && Rubro1Enabled);
            bool res2 = !Rubro2Enabled || (!string.IsNullOrEmpty(Rubro2) && Rubro2Enabled);
            bool res3 = !Rubro3Enabled || (!string.IsNullOrEmpty(Rubro3) && Rubro3Enabled);
            bool res4 = !Rubro4Enabled || (!string.IsNullOrEmpty(Rubro4) && Rubro4Enabled);
            bool res5 = !Rubro5Enabled || (!string.IsNullOrEmpty(Rubro5) && Rubro5Enabled);
            res = res1 && res2 && res3 && res4 && res5;
            return res;
        }

        #endregion
    }
}