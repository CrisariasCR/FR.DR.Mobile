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

using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;
using FR.Core.Model;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaCorporacionViewModel : BaseViewModel
    {
        public ConsultaCorporacionViewModel()
        {
            corporacion = new Corporacion();
            corporacion.Cargar();
            
        }

        #region Propiedades       
        
        private Corporacion corporacion;
        public Corporacion Corporacion
        {
            get { return corporacion; }
            set { corporacion = value; RaisePropertyChanged("Corporacion"); }
        }

        #endregion
    }
}