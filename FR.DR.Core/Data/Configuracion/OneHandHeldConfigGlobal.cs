using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Softland.ERP.FR.Mobile.Cls.Configuracion
{
    public class OneHandHeldConfigGlobal
    {
        #region Constructor
        public OneHandHeldConfigGlobal() { }
        #endregion

        #region Atributos

        private string modulo;
        private string nombre;
        private string tipo;
        private string valor;
        private string texto;  
      
        #endregion

        #region Propiedades
        public string Modulo
        {
            get { return modulo; }
            set { modulo = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }


        public string Texto
        {
            get { return texto; }
            set { texto = value; }
        }
        #endregion
    }
}
