using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;

namespace Softland.ERP.FR.Mobile.Cls
{
    public class NCFBase
    {
        public const string TIPOCONSUMIDORFINAL = "F";
        public const string TIPOGUBERNAMENTAL = "G";
        public const string TIPOORGANIZADO = "O";
        public const string TIPOREGIMENESPECIAL = "E";
        public const string TIPONOTACREDITO = "T";      


        #region Constructor

        public NCFBase()
        { }

        public NCFBase(string comp, string hh, string pref, string desc, string ultValor, string secInicial, string secFinal, string tip, string tipDoc, string tipContrib)
        {
            this.compania = comp;
            this.handheld = hh;
            this.prefijo = pref;
            this.descripcion = desc;
            this.ultimoValor = ultValor;
            this.secuenciaInicial = secInicial;
            this.secuenciaFinal = secFinal;
            this.tipo = tip;
            this.tipoDoc = tipDoc;
            this.tipoContribuyente = tipContrib;
        }

        #endregion Constructor

        #region Propiedades de Clase

        private string compania = string.Empty;

        public string Compania
        {
            get { return compania; }
        }

        private string prefijo = string.Empty;

        public string Prefijo
        {
            get { return prefijo; }
            set { prefijo = value; }
        }

        private string descripcion = string.Empty;

        public string Descripcion
        {
            get { return descripcion; }
        }

        private string ultimoValor = string.Empty;

        public string UltimoValor
        {
            get { return ultimoValor; }
            set { ultimoValor = value; }
        }

        private string valorNCF = string.Empty;

        public string ValorNCF
        {
            get { return prefijo + ultimoValor; }
        }

        private string secuenciaInicial = string.Empty;

        public string SecuenciaInicial
        {
            get { return secuenciaInicial; }
        }

        private string secuenciaFinal = string.Empty;

        public string SecuenciaFinal
        {
            get { return secuenciaFinal; }
        }

        private string tipo = string.Empty;

        public string Tipo
        {
            get { return tipo; }
        }

        private string tipoDoc = string.Empty;

        public string TipoDoc
        {
            get { return tipoDoc; }
        }

        private string tipoContribuyente = string.Empty;

        public string TipoContribuyente
        {
            get { return tipoContribuyente; }
        }

        private string handheld = string.Empty;

        public string Handheld
        {
            get { return handheld; }
        }

        public string TituloDocumento
        {
            get { return tituloDocumento(); }
        }

        #endregion Propiedades de Clase

        #region Metodos Publicos
        /// <summary>
        /// Incrementar un consecutivo
        /// </summary>
        /// <param name="c">Tipo de consecutivo</param>
        /// <param name="consecutivoActual">numero de consecutivo actual</param>
        /// <returns>El siguiente consecutivo</returns>
        public string IncrementarConsecutivoNCF()
        {
            //string nuevoConsecutivo = GestorUtilitario.proximoCodigo(ultimoValor, 20);
            string sentencia =
                        " UPDATE " + Table.ERPADMIN_NCF_CONSECUTIVO_RT + " SET ULTIMO_VALOR = @CONSECUTIVO " +
                        " WHERE UPPER(COMPANIA) = @COMPANIA " +
                        " AND   HANDHELD = @HANDHELD " +
                        " AND   PREFIJO = @PREFIJO ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar,ultimoValor),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,compania.ToUpper()),
                GestorDatos.SQLiteParameter("@HANDHELD",SqlDbType.NVarChar,handheld),
                GestorDatos.SQLiteParameter("@PREFIJO",SqlDbType.NVarChar,prefijo)});

            int actualizo = GestorDatos.EjecutarComando(sentencia, parametros);
            if (actualizo != 1)
                throw new Exception("No se pudo incrementar el consecutivo.");

            //ultimoValor = nuevoConsecutivo;

            //return nuevoConsecutivo;
            return ultimoValor;
        }        

        #endregion Metodos Publicos

        #region Metodos de Clase

        private string tituloDocumento()
        {
            string titulo = string.Empty;

            switch (tipoContribuyente)
            {
                case TIPOCONSUMIDORFINAL:
                    titulo = "Factura Consumidor Final ";
                    break;
                case TIPOGUBERNAMENTAL:
                    titulo = "Factura Gubernamental ";
                    break;
                case TIPOORGANIZADO:
                    titulo = "Factura Válida para Crédito Fiscal";
                    break;
                case TIPOREGIMENESPECIAL:
                    titulo = "Factura Régimen Especial ";
                    break;
                default:
                    titulo = "";
                    break;
            }

            return titulo;
        }

        public void cargarNuevoNCF()
        {
            ultimoValor = GestorUtilitario.proximoCodigo(ultimoValor, 20);            
        }

        #endregion Metodos de Clase

    }
}
