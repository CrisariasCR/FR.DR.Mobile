using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;
using Softland.ERP.FR.Mobile.ViewModels;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls;

namespace FR.DR.Core.Data.Documentos.Retenciones
{
    /// <summary>
    /// Clase funcional para acceso a la tabla de retenciones a los documentos de CC.
    /// </summary>
    public class fclsRetencionDocCC
    {
        #region Variables
        private string sDBName;						/* Separacion=ND */
        private string sDBList;						/* Separacion=ND */
        private string sVarList;						/* Separacion=ND */
        private string sAsignList;						/* Separacion=ND */
        private string sWhereCond;						/* Separacion=ND */
        public string sTipo;						/* Separacion=ND */
        public string sDocumento;						/* Separacion=ND */
        public string sCodigoRetencion;						/* Separacion=ND */
        public string sRetencion;						/* Separacion=ND */
        public string sDocReferencia;						/* Separacion=ND */
        public decimal nMonto;						/* Separacion=ND */
        public string sEstado;						/* Separacion=ND */
        public decimal nBaseGravada;						/* Separacion=ND */
        public string sAutoretenedora;						/* Separacion=ND */
        #endregion Variables

        /// <summary>
        /// Constructor de la clase fclsRetencionDocCC
        /// </summary>
        public fclsRetencionDocCC()
        {
            InitInstance();
        }

        #region Funciones

        /// <summary>
        /// Inicialice la instancia.
        /// </summary>
        /* Separacion=ND*/
        public bool InitInstance()
        {

            sDBName = Table.ERPADMIN_RETENCIONES_DOC_CC.ToString();
            // NBH >> INICIO: Proyecto Ajustes Colombia
            /*
                // @ WAM - FACE LIFT
                // Set sVarList = ':sTipo, :sDocumento, :sCodigoRetencion, :sRetencion, :sDocReferencia, :nMonto, :sEstado, :isRowPointer'
                // @ WAM - FACE LIFT
                // Set sDBList = 'tipo, documento, codigo_retencion, retencion, doc_referencia, monto, estado, RowPointer'
                // Set sAsignList = 'retencion = :sRetencion, doc_referencia = :sDocReferencia, monto = :nMonto, estado = :sEstado'
            */
            sVarList = ":sTipo, :sDocumento, :sCodigoRetencion, :sRetencion, :sDocReferencia, :nMonto, :sEstado, :nBaseGravada, :sAutoretenedora";
            sDBList = "tipo, documento, codigo_retencion, retencion, doc_referencia, monto, estado, base, autoretenedora";
            sAsignList = "retencion = :sRetencion, doc_referencia = :sDocReferencia, monto = :nMonto, estado = :sEstado, base = :nBaseGravada, autoretenedora = :sAutoretenedora ";
            // NBH >> FIN: Proyecto Ajustes Colombia
            sWhereCond = " tipo = :sTipo AND documento = :sDocumento AND codigo_retencion = :sCodigoRetencion";
            return true;
        }

        /// <summary>
        /// Vaciar la instancia.
        /// </summary>
        /* Separacion=ND*/
        public void ClearInstance()
        {

            sTipo = "";
            sDocumento = "";
            sCodigoRetencion = "";
            sRetencion = "";
            sDocReferencia = "";
            nMonto = 0;
            sEstado = fclsRetencion.CC_RET_APLICADA;
            // @ WAM - FACE LIFT
            //isRowPointer = null;
            // NBH >> INICIO: Proyecto Ajustes Colombia
            nBaseGravada = 0;
            sAutoretenedora = null;
            // NBH >> FIN: Proyecto Ajustes Colombia
        }

        /// <summary>
        /// Hacer la llave para el bloqueo.
        /// </summary>
        /* Separacion=ND*/
        public string MakeLockKey()
        {

            if ((sTipo == ""))
            {
                return "";
            }
            if ((sDocumento == ""))
            {
                return "";
            }
            return ((("ret_doc_cc" + sTipo) + sDocumento) + sCodigoRetencion);
        }        

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public bool ReadOnCodigo(string prmsTipo, string prmsDocumento, string prmsCodigoRetencion)
        {
            string sSqlCmd;
            bool bExiste = false;

            sSqlCmd = "SELECT " + sDBList + " FROM " + sDBName + " WHERE tipo = '" + prmsTipo +
                 "' AND documento = '" + prmsDocumento + "' AND codigo_retencion = '" + prmsCodigoRetencion + "'";
            bExiste = false;

            SQLiteDataReader reader = null;
            try
            {
                reader = GestorDatos.EjecutarConsulta(sSqlCmd);                
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        sTipo = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        sDocumento = reader.GetString(1);                    
                    if (!reader.IsDBNull(2))
                        sCodigoRetencion = reader.GetString(2);
                    if (!reader.IsDBNull(3))
                        sRetencion = reader.GetString(3);
                    if (!reader.IsDBNull(4))
                        sDocReferencia = reader.GetString(4);
                    if (!reader.IsDBNull(5))
                        nMonto = reader.GetDecimal(5);
                    if (!reader.IsDBNull(6))
                        sEstado = reader.GetString(6);
                    if (!reader.IsDBNull(7))
                        nBaseGravada = reader.GetDecimal(7);
                    if (!reader.IsDBNull(8))
                        sAutoretenedora = reader.GetString(8);
                    bExiste = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar las retenciones. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return bExiste;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void FillInstance(string prmsTipo, string prmsDocumento, string prmsCodigoRetencion, string prmsRetencion, string prmsDocReferencia, decimal prmnMonto, string prmsEstado, decimal prmnBaseGravada, string prmsAutoretenedora)
        {

            sTipo = prmsTipo;
            sDocumento = prmsDocumento;
            sCodigoRetencion = prmsCodigoRetencion;
            sRetencion = prmsRetencion;
            sDocReferencia = prmsDocReferencia;
            nMonto = prmnMonto;
            sEstado = prmsEstado;
            // NBH >> INICIO: Proyecto Ajustes Colombia
            nBaseGravada = prmnBaseGravada;
            sAutoretenedora = prmsAutoretenedora;
            // NBH >> FIN: Proyecto Ajustes Colombia
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void GetInstance(ref string prmsTipo, ref string prmsDocumento, ref string prmsCodigoRetencion, ref string prmsRetencion, ref string prmsDocReferencia, ref decimal prmnMonto, ref string prmsEstado, ref decimal prmnBaseGravada, ref string prmsAutoretenedora)
        {

            prmsTipo = sTipo;
            prmsDocumento = sDocumento;
            prmsCodigoRetencion = sCodigoRetencion;
            prmsRetencion = sRetencion;
            prmsDocReferencia = sDocReferencia;
            prmnMonto = nMonto;
            prmsEstado = sEstado;
            // NBH >> INICIO: Proyecto Ajustes Colombia
            prmnBaseGravada = nBaseGravada;
            prmsAutoretenedora = sAutoretenedora;
            // NBH >> FIN: Proyecto Ajustes Colombia
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public bool ExistsKey(string prmsTipo, string prmsDocumento, string prmsCodigoRetencion, ref bool bParamExiste)
        {            
            string sSqlCmd;
            bool lbOK;

            sSqlCmd = "SELECT tipo, documento, codigo_retencion FROM " + sDBName + " WHERE tipo = '" + prmsTipo + "' AND documento = '" +
                 prmsDocumento + "' AND codigo_retencion = '" + prmsCodigoRetencion + "'";
            lbOK = (GestorDatos.NumeroRegistros(sSqlCmd) != 0);
            return lbOK;
        }

        #endregion Funciones

    } // fclsRetencionDocCC
}