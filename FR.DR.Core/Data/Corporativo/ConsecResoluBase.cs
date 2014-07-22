using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System.Data;

namespace Softland.ERP.FR.Mobile.Cls
{
    public class ConsecResoluBase
    {
        public const string TIPOFACTURA = "F";
        public const string TIPODEVOLUCION = "D";

        public const string ESTADOVIGENTE = "V";
        public const string ESTADOPENDIENTE = "P";
        public const string ESTADODESCONTINUADO = "D";
        public const string ESTADOAGOTADO = "A";


        #region Constructor

        public ConsecResoluBase()
        { }

        public ConsecResoluBase(string comp, string hh, string codigo, string serie, string ultValor,
            string secInicial, string secFinal, string estado, string tipDoc, string mascara, DateTime? fechaResolucion,
            DateTime? fechaAutorizacion, DateTime? fechaCreacion,string usuarioCreacion,string ruta)
        {
            this.compania = comp;
            this.handheld = hh;
            this.Codigo = codigo;
            this.serie = serie;
            this.ultimoValor = ultValor;
            this.secuenciaInicial = secInicial;
            this.secuenciaFinal = secFinal;
            this.Estado=estado;
            this.tipoDoc = tipDoc;
            this.FechaAutorizacion = fechaAutorizacion;
            this.FechaCreacion = fechaCreacion;
            this.FechaResolucion = fechaResolucion;
            this.Mascara = mascara;
            this.UsuarioCreacion = usuarioCreacion;
            this.ruta = ruta;
        }

        #endregion Constructor

        #region Propiedades de Clase

        private string compania = string.Empty;

        public string Compania
        {
            get { return compania; }
        }

        private string mascara = string.Empty;

        public string Mascara
        {
            get { return mascara; }
            set { mascara = value; }
        }

        private DateTime? fechaAutorizacion;

        public DateTime? FechaAutorizacion
        {
            get { return fechaAutorizacion; }
            set { fechaAutorizacion = value; }
        }

        private DateTime? fechaResolucion;

        public DateTime? FechaResolucion
        {
            get { return fechaResolucion; }
            set { fechaResolucion = value; }
        }

        private DateTime? fechaCreacion;

        public DateTime? FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; }
        }

        private string serie = string.Empty;

        public string Serie
        {
            get { return serie; }
            set { serie = value; }
        }

        private string codigo = string.Empty;

        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string usuarioCreacion = string.Empty;

        public string UsuarioCreacion
        {
            get { return usuarioCreacion; }
            set { usuarioCreacion = value; }
        }

        private string estado = string.Empty;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        private string ultimoValor = string.Empty;

        public string UltimoValor
        {
            get { return ultimoValor; }
            set { ultimoValor = value; }
        }

        private string valorResolucion = string.Empty;

        public string ValorResolucion
        {
            get 
            {
              return serie + ultimoValor;                
            }
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

        private string tipoDoc = string.Empty;

        public string TipoDoc
        {
            get { return tipoDoc; }
        }

        private string handheld = string.Empty;

        public string Handheld
        {
            get { return handheld; }
        }

        private string ruta = string.Empty;

        public string Ruta
        {
            get { return ruta; }
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
        public string IncrementarConsecutivoRes()
        {
            //Verifica si ya se alcanzo el limite de consecutivos para ponerlo agotado y no incrementarlo.
            if (UltimoValor.Equals(SecuenciaFinal))
            {
                CambiarEstadoConsecutivoRes(ESTADOAGOTADO);
                return ultimoValor;
            }
           string nuevoConsecutivo = GestorUtilitario.proximoCodigo(ultimoValor, 50);
            
            string sentencia =
                        " UPDATE " + Table.ERPADMIN_RESOLUCION_CONSECUTIVO_RT + " SET VALOR = @CONSECUTIVO " +
                        " WHERE COMPANIA = @COMPANIA " +
                        " AND   HANDHELD = @HANDHELD " +
                        " AND   RESOLUCION = @CODIGO";

            SQLiteParameterList parametros =new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar,nuevoConsecutivo),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,compania),
                GestorDatos.SQLiteParameter("@HANDHELD",SqlDbType.NVarChar,handheld),
                GestorDatos.SQLiteParameter("@CODIGO",SqlDbType.NVarChar,codigo)});

            int actualizo = GestorDatos.EjecutarComando(sentencia, parametros);
            if (actualizo != 1)
                throw new Exception("No se pudo incrementar el consecutivo.");

            ultimoValor = nuevoConsecutivo;

            return ultimoValor;
        }        

        #endregion Metodos Publicos

        #region Validaciones

        private bool ValidacionAgotado() 
        {
            return estado.Equals("A");
        }

        private bool ValidacionVencimiento() 
        {
            if (FechaAutorizacion == null)
            {
                return false;
            }
            else
            {
                return (Convert.ToDateTime(FechaAutorizacion.Value.ToShortDateString()) <= Convert.ToDateTime(DateTime.Today.ToShortDateString())) ;
            }
        }

        public bool ValidarConsecutivoResolucion(ref string error,ref string advertencia) 
        {
            bool result = true;
            try
            {
                //Primero valida que no este agotado
                if (ValidacionAgotado())
                {
                    result = false;
                    advertencia = "El Consecutivo de resolución se encuentra agotado no se podra generar el documento";
                    CambiarEstadoConsecutivoRes(ESTADOAGOTADO);
                }
                //Segundo valida que no este vencido
                if (result && ValidacionVencimiento())
                {
                    result = false;
                    advertencia = "El Consecutivo de resolución se encuentra vencido no se podra generar el documento";
                    CambiarEstadoConsecutivoRes(ESTADODESCONTINUADO);
                }
                //Tercero valida el porcentaje de uso de resoluciones.
                if (result && VerificaPorcUsoResolucion(ref error))
                {

                    advertencia = "El Porcentaje de uso del consecutivo de resolución utilizado supera el " +
                        FRdConfig.PorcentajeUsoResoluciones + " porciento.";

                }
                else
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                error = "Se produjo un error en la validación del consecutivo de resolución. "+ex.Message;
            }
            return result;
        }
       

        private bool VerificaPorcUsoResolucion(ref string error) 
        {
            decimal valorFinalNum=0;
            decimal valorInicialNum = 0;
            decimal valorActualNum = 0;
            decimal porcentaje = 0;
            bool result = true;

            try
            {
                valorFinalNum = Convert.ToDecimal(SecuenciaFinal);
                valorInicialNum = Convert.ToDecimal(SecuenciaInicial);
                valorActualNum = Convert.ToDecimal(UltimoValor);
            }
            catch
            {
                result = false;
                error="Se produjo un error al obtener el porcentaje de uso del consecutivo de resolución ";
                //throw newex;                
            }
            if (result)
            { 
                valorFinalNum -= valorInicialNum;
                valorActualNum -= valorInicialNum;
                try
                {
                    porcentaje = (valorActualNum / valorFinalNum) * 100;
                }
                catch
                {
                    result = false;
                    error = "Se produjo un error al obtener el porcentaje de uso del consecutivo de resolución ";
                }
            }
            if (result)
            {
                result = FRdConfig.PorcentajeUsoResoluciones > 0 && FRdConfig.PorcentajeUsoResoluciones < porcentaje;
            }
            return result;
        }

        #endregion Validaciones

        #region Metodos de Clase

        private string tituloDocumento()
        {
            string titulo = string.Empty;

            //switch (tipoContribuyente)
            //{
            //    case TIPOCONSUMIDORFINAL:
            //        titulo = "Factura Consumidor Final ";
            //        break;
            //    case TIPOGUBERNAMENTAL:
            //        titulo = "Factura Gubernamental ";
            //        break;
            //    case TIPOORGANIZADO:
            //        titulo = "Factura Válida para Crédito Fiscal";
            //        break;
            //    case TIPOREGIMENESPECIAL:
            //        titulo = "Factura Régimen Especial ";
            //        break;
            //    default:
            //        titulo = "";
            //        break;
            //}

            return titulo;
        }

        public void cargarNuevoConsecRes()
        {
            ultimoValor = GestorUtilitario.proximoCodigo(ultimoValor, 20);            
        }

        /// <summary>
        /// Cambia el estado a un consecutivo
        /// </summary>
        /// <param name="c">Tipo de consecutivo</param>
        /// <param name="consecutivoActual">numero de consecutivo actual</param>
        /// <returns>El siguiente consecutivo</returns>
        public void CambiarEstadoConsecutivoRes(string estado)
        {
            //string nuevoConsecutivo = GestorUtilitario.proximoCodigo(ultimoValor, 20);
            string sentencia =
                        " UPDATE " + Table.ERPADMIN_RESOLUCION_CONSECUTIVO_RT + " SET ESTADO = @ESTADO " +
                        " WHERE COMPANIA = @COMPANIA " +
                        " AND   HANDHELD = @HANDHELD " +
                        " AND   RESOLUCION = @CODIGO";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                GestorDatos.SQLiteParameter("@ESTADO",SqlDbType.NVarChar,estado),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,compania),
                GestorDatos.SQLiteParameter("@HANDHELD",SqlDbType.NVarChar,handheld),
                GestorDatos.SQLiteParameter("@CODIGO",SqlDbType.NVarChar,codigo)});

            int actualizo = GestorDatos.EjecutarComando(sentencia, parametros);
            if (actualizo != 1)
                throw new Exception("No se pudo cambiar el estado del consecutivo de resolución.");
        } 

        #endregion Metodos de Clase

    }
}
