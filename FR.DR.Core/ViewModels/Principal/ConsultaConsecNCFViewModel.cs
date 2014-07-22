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
using Softland.ERP.FR.Mobile.Cls;
using FR.Core.Model;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaConsecNCFViewModel : BaseViewModel
    {
        public ConsultaConsecNCFViewModel()
        {   
            
			companiaActual = string.Empty;
            CargarDatosNCF();
        }


        protected bool CargarDatosNCF()
        {
            bool procesoExitoso = true;
            string consulta = string.Empty;

            try
            {

                
                Companias = new SimpleObservableCollection<string>(Util.CargarCias());
                if (Companias.Count > 0)
                {
                    CompaniaActual = Companias[0];
                }
                //string compania = cmbCompania.Text;

                RefrescarParametros();
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(String.Format("Problemas cargando la información de los NCF's.", ex.Message));
                
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        private void RefrescarParametros()
        {
            if (NCFUtilitario.obtenerNCF(CompaniaActual))
            {
                foreach (NCFBase ncf in NCFUtilitario.listaNCF)
                {
                    switch (ncf.TipoContribuyente)
                    {
                        case NCFBase.TIPOCONSUMIDORFINAL:
                            //txtPrefConsFinal.Text = ncf.Prefijo;
                            //txtConsecConsFinal.Text = ncf.UltimoValor;
                            consumidorFinal = ncf.Prefijo + "   " + ncf.UltimoValor;
                            break;
                        case NCFBase.TIPOGUBERNAMENTAL:
                            //txtPrefGubernament.Text = ncf.Prefijo;
                            //txtConsecGubernament.Text = ncf.UltimoValor;
                            gubernamental = ncf.Prefijo + "   " + ncf.UltimoValor;
                            break;
                        case NCFBase.TIPOORGANIZADO:
                            //txtPrefOrganizado.Text = ncf.Prefijo;
                            //txtConsecOrganizado.Text = ncf.UltimoValor;
                            organizado = ncf.Prefijo + "   " + ncf.UltimoValor;
                            break;
                        case NCFBase.TIPOREGIMENESPECIAL:
                            //txtPrefRegEspecial.Text = ncf.Prefijo;
                            //txtConsecRegEspecial.Text = ncf.UltimoValor;
                            regimenEspecial = ncf.Prefijo + "   " + ncf.UltimoValor;
                            break;
                        case NCFBase.TIPONOTACREDITO:
                            //txtPrefNotaCredito.Text = ncf.Prefijo;
                            //txtConsecNotaCredito.Text = ncf.UltimoValor;
                            notaCredito = ncf.Prefijo + "   " + ncf.UltimoValor;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #region Propiedades

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
                    RefrescarParametros();
                }
            }
        }

        public IObservableCollection<string> Companias { get; set; }
        

        private string consumidorFinal;
        public string ConsumidorFinal
        {
            get { return consumidorFinal; }
            set { consumidorFinal = value; RaisePropertyChanged("ConsumidorFinal"); }
        }

        private string gubernamental;
        public string Gubernamental
        {
            get { return gubernamental; }
            set { gubernamental = value; RaisePropertyChanged("Gubernamental"); }
        }

        private string organizado;
        public string Organizado
        {
            get { return organizado; }
            set { organizado = value; RaisePropertyChanged("Organizado"); }
        }

        private string regimenEspecial;
        public string RegimenEspecial
        {
            get { return regimenEspecial; }
            set { regimenEspecial = value; RaisePropertyChanged("RegimenEspecial"); }
        }

        private string notaCredito;
        public string NotaCredito
        {
            get { return notaCredito; }
            set { notaCredito = value; RaisePropertyChanged("NotaCredito"); }
        }        

        #endregion
    }
}