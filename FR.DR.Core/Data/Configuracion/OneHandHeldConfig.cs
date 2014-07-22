using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Softland.ERP.FR.Mobile.Cls.Configuracion
{
    public class OneHandHeldConfig
    {
        #region Constructor
        public OneHandHeldConfig() { }
        #endregion

        #region Atributos
        private string handHeld;
        private string valor;
        private string constante;
        #endregion

        #region Propiedades
        public string HandHeld
        {
            get { return handHeld; }
            set { handHeld = value; }
        }

        public string Constante
        {
            get { return constante; }
            set { constante = value; }
        }

        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }
        #endregion
    }
}
