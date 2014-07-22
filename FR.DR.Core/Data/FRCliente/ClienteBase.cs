using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.Printing;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{
    /// <summary>
    /// Datos basicos de un cliente
    /// </summary>
    public abstract class ClienteBase : IPrintable
    {
        #region Variables y Propiedades

        private string zona = string.Empty;
        /// <summary>
        /// Codigo de la zona o ruta asociada al cliente.
        /// </summary>
        public string Zona
        {
            get { return zona; }
            set { zona = value; }
        }

        private string codigo = string.Empty;
        /// <summary>
        /// Código del cliente.
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string nombre = string.Empty;
        /// <summary>
        /// Nombre del cliente.
        /// </summary>
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        private string direccion = string.Empty;
        /// <summary>
        /// Direccion del cliente
        /// </summary>
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }       

        #endregion

        public string GetObjectName()
        {
            return "CLIENTE";
        }
        public virtual object GetField(string name)
        {
            switch (name)
            {
                case "CLIENTE_CODIGO":
                case "CODIGO": return codigo;
                case "CLIENTE_NOMBRE": 
                case "NOMBRE": return nombre;
                case "CLIENTE_DIRECCION": return direccion;
                case "DIRECCION": return direccion;   
                default: return string.Empty;
            }
        }
    }
}
