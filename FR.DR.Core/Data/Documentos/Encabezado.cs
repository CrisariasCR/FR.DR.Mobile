using System;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls;
using EMF.Printing;
using Cirrious.MvvmCross.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Clase base de un tipo de documento
    /// </summary>
    public class Encabezado : MvxNotifyPropertyChanged, IPrintable
    {
        #region Variables y Propiedades de instancia

        private string numero = string.Empty;
        /// <summary>
        /// Variable que indica el numero de documento de FR 
        /// </summary>
        public string Numero
        {
            get { return numero; }
            set { numero = value; RaisePropertyChanged("Numero"); }
        }

        private string compania = string.Empty;
        /// <summary>
        /// Variable de asociacion con la clase compania
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); RaisePropertyChanged("Compania"); }
        }

        private string cliente;
        /// <summary>
        /// Variable de asociacion con la clase cliente
        /// </summary>
        public string Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        private NCFBase ncf = null;

        public NCFBase NCF
        {
            get { return ncf; }
            set { ncf = value; }
        }

        private ConsecResoluBase consecResolucion = null;

        public ConsecResoluBase ConsecResolucion
        {
            get { return consecResolucion; }
            set { consecResolucion = value; }
        }

        #endregion

        #region Ruta

        private string zona = string.Empty;
        /// <summary>
        /// Codigo de la zona o ruta asociada al documento.
        /// </summary>
        public string Zona
        {
            get { return zona; }
            set { zona = value; }
        }

        #endregion

        #region Impresion

        //Caso 25452 LDS 30/10/2007
        private bool impreso;
        /// <summary>
        /// Indica si se ha sido impreso o no el original del documento.
        /// </summary>
        public bool Impreso
        {
            get { return impreso; }
            set { impreso = value; }
        }

        //Caso 25452 LDS 17/10/2007
        /// <summary>
        /// Verdadero indica que se debe imprimir la leyenda Original en el documento. 
        /// Falso indica que se debe imprimir la leyenda Copia en el documento.
        /// </summary>
        private bool leyendaOriginal;

        public bool LeyendaOriginal
        {
            get { return leyendaOriginal; }
            set { leyendaOriginal = value; }
        }

        #endregion

        #region IPrintable Members

        public virtual string GetObjectName()
        {
            return "ENCABEZADO";
        }

        public virtual object GetField(string name)
        {
            switch (name)
            {
                case "CLIENTE_CODIGO": return cliente;
                case "NUMERO": return numero;
                case "NUMERO_DOCUMENTO": return numero;                    
                case "COMPANIA": return compania;
                case "LEYENDA":
                    if(this.LeyendaOriginal)
                        return "Original";
                    else
                        return "Copia";
                case "NCF":
                    try
                    {
                        if (NCF.UltimoValor != null && NCF.UltimoValor != string.Empty)
                            return NCF.ValorNCF;
                        return "";
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                    
                case "TITULO_NCF":
                    try
                    {
                        if (NCF.UltimoValor != null && NCF.UltimoValor != string.Empty)
                            return ncf.TituloDocumento;
                        return "";
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                    
                case "RUTA": return zona;

                default: return string.Empty;
            }    
        }

        #endregion

    }
}