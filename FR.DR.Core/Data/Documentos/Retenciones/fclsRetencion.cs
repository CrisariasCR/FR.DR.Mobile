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
    /// 
    /// </summary>
    public class fclsRetencion : fclsRetencionDocCC
    {
        #region Constantes
        public const string CP_RET_REGISTRO = "R";	// Tipo de retención "al registrar"
        public const string CP_RET_ACTIVA = "A";	// Retención activa
        public const string CP_RET_INACTIVA = "I";	// Retención inactiva
        public const string CP_RET_APLICA = "S";	// Se da la aplicación
        public const string CP_RET_NO_APLICA = "N";	// No se da la aplicación
        public const string CP_RET_CANCELA = "C";	// Tipo de retención "al cancelar"

        public const string CP_RET_EXCEP_DEFAULT = "D";//Indica que para la retención no se usarán excepciones, sino que se tomará el porcentaje que esté definido en la retención
        public const string CP_RET_EXCEP_CIUDAD = "C";	//Indica que para la retención aplican excepciones por ciudad, por lo que se debe de validar la ciudad para determinar el porcentaje a utilizar
        public const string CP_RET_EXCEP_REGIMEN = "R"; //Indica que para la retención aplican excepciones por regimen, por lo que se debe de validar el regimen para determinar el porcentaje a utilizar
        public const string CP_RET_NINGUNA = "N";	//Indica que la retención no es ni autoretenedora ni asumida
        public const string CP_RET_AUTORET = "S";	// Indica que la retención es autoretenedora
        public const string CP_RET_ASUMIDA = "A";    // Indica que la retención es asumida
        public const string CP_NO = "N";
        public const string CP_SI = "S";
        public const string CP_RANGOS_UVT = "RANGOS_UVT";
        public const string AS = "AS";
        public const string AS_VALOR_UVT = "VALOR_UVT";
        public const string CC_RET_ACTIVA = "A";
        public const string AS_AMBAS = "A";
        public const string AS_VENTA = "V";
        public const string CC_RET_APLICADA = "A";
        public const string CC_RET_PENDIENTE = "P";
        public const string COLOMBIA = "COLOMBIA";

        #endregion

        #region Variables
        private string sDBName;						/* Separacion=ND */
        private string sDBList;						/* Separacion=ND */
        private string sVarList;						/* Separacion=ND */
        private string sAsignList;						/* Separacion=ND */
        private string sWhereCond;						/* Separacion=ND */
        private string sLockKey;						/* Separacion=ND */
        private int cnPrecisionDecimalEnRetenciones;						/* Separacion=ND */
        private decimal inPrecisionDecimalEnRetenciones;						/* Separacion=ND */
        public string sCodigoRetencion;						/* Separacion=ND */
        public string sDescRetencion;						/* Separacion=ND */
        private decimal nPorcentaje;						/* Separacion=ND */
        public string sTipo;						/* Separacion=ND */
        private string sAplicaMonto;						/* Separacion=ND */
        private string sAplicaSubtotal;						/* Separacion=ND */
        private string sAplicaSubDesc;						/* Separacion=ND */
        private string sAplicaImpuesto1;						/* Separacion=ND */
        private string sAplicaImpuesto2;						/* Separacion=ND */
        private string sAplicaRubro1;						/* Separacion=ND */
        private string sAplicaRubro2;						/* Separacion=ND */
        private decimal nMontoMinimo;						/* Separacion=ND */
        public string sEstado;						/* Separacion=ND */
        private string sCtrRetencion;						/* Separacion=ND */
        private string sCtaRetencion;						/* Separacion=ND */
        private string sRetPorArticulo;						/* Separacion=ND */
        public string sEsAutoretenedor;						/* Separacion=ND */
        private string sClasificacionArt;						/* Separacion=ND */
        private string sCtrAutoRetencion;						/* Separacion=ND */
        private string sCtaAutoRetencion;						/* Separacion=ND */
        private string isAplicaRetencion;						/* Separacion=ND */
        public string isRetencionBase;						/* Separacion=ND */
        private string isNoAplicProvAuto;						/* Separacion=ND */
        private string isTipoExcepcion;						/* Separacion=ND */
        private decimal inMontoOtraRet;						/* Separacion=ND */
        private string isPais;						/* Separacion=ND */
        private string isDivGeo1;						/* Separacion=ND */
        private string isDivGeo2;						/* Separacion=ND */
        private string isRegimen;						/* Separacion=ND */
        private fclsExcepcionCiudad fciExcepcionCiudad;						/* Separacion=ND */
        private fclsExcepcionRegimen fciExcepcionRegimen;						/* Separacion=ND */
        private string isAplicaImp1Desc;						/* Separacion=ND */
        private string isAplicaImp1NoDesc;						/* Separacion=ND */
        private decimal inMontoImp1Desc;						/* Separacion=ND */
        private decimal inMontoImp1NoDesc;						/* Separacion=ND */
        private string isUsaTarifaUVT;						/* Separacion=ND */
        private bool ibAplicarTarifaUVT;						/* Separacion=ND */
        private string isConsecVenta;						/* Separacion=ND */
        private string isConsecCompra;						/* Separacion=ND */
        //private fclsUtilValoresCertificados ifciValoresCertif;						/* Separacion=ND */
        //private fclsCPXml vfciCPXML;						/* Separacion=ND */
        private string isPermiteGenRetCero;						/* Separacion=ND */
        private string isTipoApliCancelar;						/* Separacion=ND */
        private string compania;
        #endregion Variables

        #region PropiedadesConsulta

        public string conCodigo 
        {
            get { return sCodigoRetencion; }
        }

        public string conDescripcion
        {
            get { return sRetencion; }
        }

        public decimal conMonto
        {
            get { return nMonto; }
        }

        public string conReferencia
        {
            get { return sDocReferencia; }
        }

        public string conRetenedora
        {
            get { if(sAutoretenedora.Equals("S")) return "Si"; else return "No";}
        }

        public decimal conBase
        {
            get { return nBaseGravada; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase fclsRetencion
        /// </summary>
        public fclsRetencion(string pCompania)
        {
            this.InitInstance();
            this.compania = pCompania;
        }

        #endregion

        #region Funciones
        /// <summary>
        /// Inicializa los nombres de la clase. Esta función debe ser redefinida en cada clase derivada. Vea los comentarios en las acciones de esta función.
        /// </summary>
        /* Separacion=ND*/
        //public decimal InitNombres()
        //{

        //    // Deben inicializarse las variables de clase: NombreRow, NombreTabla
        //    NombreTabla = "Retenciones";
        //    NombreRow = "Esta Retención";
        //}

        /// <summary>
        /// Inicialice la instancia.
        /// </summary>
        /* Separacion=ND*/
        public bool InitInstance()
        {
            //Inicializa las clases
            fciExcepcionCiudad = new fclsExcepcionCiudad();
            fciExcepcionRegimen = new fclsExcepcionRegimen();
            // Valor por defecto de la precisión decimal
            cnPrecisionDecimalEnRetenciones = 2;
            sDBName = Table.ERPADMIN_RETENCIONES.ToString();
            // @ WAM - FACE LIFT
            // MRL Proyecto Colombia R4 --> Se agrega isAplicaRetencion, isRetencionBase, isNoAplicProvAuto, isTipoExcepcion
            // MRL Ajustes Colombia R5 --> Se agrega isAplicaImp1Desc y isAplicaImp1NoDesc
            // MRL Rte Fuente Proveedores Independientes --> Se agrega isUsaTarifaUVT
            // ERG Caso CO2-01264-44K7 No generar retenciones en cero -->
            // ERG Caso Retenciones al Pago -->
            sVarList = ":sCodigoRetencion, :sDescRetencion, :nPorcentaje, :sTipo, :sAplicaMonto, :sAplicaSubtotal, :sAplicaSubDesc, :sAplicaImpuesto1, :sAplicaImpuesto2, :sAplicaRubro1, :sAplicaRubro2, :nMontoMinimo, :sEstado, :sCtrRetencion, :sCtaRetencion, :sRetPorArticulo, :sEsAutoretenedor, :sClasificacionArt, :sCtrAutoRetencion, :sCtaAutoRetencion, :isAplicaRetencion, :isRetencionBase, :isNoAplicProvAuto, :isTipoExcepcion, :isAplicaImp1Desc, :isAplicaImp1NoDesc, :isUsaTarifaUVT, :isConsecVenta, :isConsecCompra, :isPermiteGenRetCero, :isTipoApliCancelar, :isRowPointer";
            // @ WAM - FACE LIFT
            sDBList = "codigo_retencion, descripcion, porcentaje, tipo, aplica_monto, aplica_subtotal, aplica_sub_desc, aplica_impuesto1, aplica_impuesto2, aplica_rubro1, aplica_rubro2, monto_minimo, estado, ctr_retencion, cta_retencion, ret_por_articulo, es_autoretenedor, clasificacion_art, ctr_autoretencion, cta_autoretencion, aplica_retencion, retencion_base, noaplica_prov_auto, tipo_excepcion, aplica_imp1_desc, aplica_imp1_nodesc, utiliza_tarifa_uvt, consec_ret_venta, consec_ret_compra, per_generar_ret_cero, tipo_apli_cancelar";
            sLockKey = "retenciones";
            sAsignList = "descripcion = :sDescRetencion, porcentaje = :nPorcentaje, tipo = :sTipo, aplica_monto = :sAplicaMonto, aplica_subtotal = :sAplicaSubtotal, aplica_sub_desc = :sAplicaSubDesc, aplica_impuesto1 = :sAplicaImpuesto1, aplica_impuesto2 = :sAplicaImpuesto2, aplica_rubro1 = :sAplicaRubro1, aplica_rubro2 = :sAplicaRubro2, monto_minimo = :nMontoMinimo, estado = :sEstado, ctr_retencion = :sCtrRetencion, cta_retencion = :sCtaRetencion, ret_por_articulo = :sRetPorArticulo, es_autoretenedor = :sEsAutoretenedor, clasificacion_art = :sClasificacionArt, ctr_autoretencion = :sCtrAutoRetencion, cta_autoretencion = :sCtaAutoRetencion, aplica_retencion = :isAplicaRetencion, retencion_base = :isRetencionBase, noaplica_prov_auto = :isNoAplicProvAuto, tipo_excepcion = :isTipoExcepcion, aplica_imp1_desc = :isAplicaImp1Desc, aplica_imp1_nodesc = :isAplicaImp1NoDesc, utiliza_tarifa_uvt = :isUsaTarifaUVT, consec_ret_venta = :isConsecVenta, consec_ret_compra = :isConsecCompra, per_generar_ret_cero = :isPermiteGenRetCero , tipo_apli_cancelar = :isTipoApliCancelar ";
            // ERG Caso Retenciones al Pago <--
            // ERG Caso CO2-01264-44K7 No generar retenciones en cero <--
            // MRL Rte Fuente Proveedores Independientes <--
            // MRL Ajustes Colombia R5 <--
            // Se inicialia la instancia para obtener el porcentaje de excepción por ciudad
            if (!(fciExcepcionCiudad.InitInstance()))
            {
                throw new Exception("Error inicializando la instancia para obtener los porcentajes de excepción por ciudad.");
            }
            if (!(fciExcepcionRegimen.InitInstance()))
            {
                throw new Exception("Error inicializando la instancia para obtener los porcentajes de excepción por regimen.");
            }
            //if (!(vfciCPXML.InitInstance()))
            //{
            //    throw new Exception("Error inicializando componente para lectura de Rangos_UVT. No será posible la sugerencia del monto de retención.");                
            //}
            sWhereCond = "codigo_retencion = :sCodigoRetencion";
            return true;
        }

        /// <summary>
        /// Hace el set de la precision decimal usada par el calculo de las retenciones
        /// </summary>
        /* Separacion=ND*/
        public void SetPrecisionDecimal()
        {
            //cnPrecisionDecimalEnRetenciones =   pnPrecisionDecimalEnRetenciones;
            cnPrecisionDecimalEnRetenciones = FRmConfig.CantidadDecimales;
        }

        /// <summary>
        /// Vaciar la instancia.
        /// </summary>
        /* Separacion=ND*/
        public void ClearInstance()
        {

            sCodigoRetencion = "";
            sDescRetencion = "";
            nPorcentaje = 0;
            sTipo = CP_RET_REGISTRO;
            sAplicaMonto = CP_RET_NO_APLICA;
            sAplicaSubtotal = CP_RET_NO_APLICA;
            sAplicaSubDesc = CP_RET_NO_APLICA;
            sAplicaImpuesto1 = CP_RET_NO_APLICA;
            sAplicaImpuesto2 = CP_RET_NO_APLICA;
            sAplicaRubro1 = CP_RET_NO_APLICA;
            sAplicaRubro2 = CP_RET_NO_APLICA;
            nMontoMinimo = 0;
            sEstado = CP_RET_ACTIVA;
            sCtrRetencion = "";
            sCtaRetencion = "";
            sRetPorArticulo = CP_RET_NO_APLICA;
            sEsAutoretenedor = CP_RET_NINGUNA;
            sClasificacionArt = "";
            sCtrAutoRetencion = "";
            sCtaAutoRetencion = "";
            // MRL Proyecto Colombia R4 -->
            isAplicaRetencion = string.Empty;
            isRetencionBase = null;
            isNoAplicProvAuto = CP_NO;
            isTipoExcepcion = CP_RET_EXCEP_DEFAULT;
            inMontoOtraRet = 0;
            isPais = string.Empty;
            isDivGeo1 = string.Empty; ;
            isDivGeo2 = string.Empty;
            isRegimen = string.Empty;
            // MRL Proyecto Colombia R4 <--
            // MRL Ajustes Colombia R5 -->
            isAplicaImp1Desc = null;
            isAplicaImp1NoDesc = null;
            inMontoImp1Desc = 0;
            inMontoImp1NoDesc = 0;
            // MRL Ajustes Colombia R5 <--
            // MRL Rte Fuente Proveedores Independientes -->
            isUsaTarifaUVT = null;
            // MRL Rte Fuente Proveedores Independientes <--
            // ERG Caso CO2-01264-44K7 No generar retenciones en cero -->
            isPermiteGenRetCero = "";
            // ERG Caso CO2-01264-44K7 No generar retenciones en cero <--
            // @ WAM - FACE LIFT            
            // ERG Caso Retenciones al Pago -->
            isTipoApliCancelar = null;
            // ERG Caso Retenciones al Pago <--
        }

        //BORRRAR
        /// <summary>
        /// Hacer la llave para el bloqueo.
        /// </summary>
        /* Separacion=ND*/
        public string MakeLockKey()
        {

            if ((sCodigoRetencion == ""))
            {
                return "";
            }
            return (sLockKey + sCodigoRetencion);
        }

        //FINBORRRAR

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public bool ReadOnKey(string prmsCodigoRetencion)
        {
            string sSqlCmd;
            bool bExiste = false;

            sSqlCmd = " SELECT " + sDBList + " FROM " + sDBName + " WHERE codigo_retencion = @CODIGO and cod_cia= @COMPANIA";
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add(new SQLiteParameter("@CODIGO",prmsCodigoRetencion));
            parametros.Add(new SQLiteParameter("@COMPANIA", this.compania));

            bExiste = false;

            SQLiteDataReader reader = null;
            try
            {
                reader = GestorDatos.EjecutarConsulta(sSqlCmd,parametros);

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        sCodigoRetencion = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        sDescRetencion = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        nPorcentaje = reader.GetDecimal(2);
                    if (!reader.IsDBNull(3))
                        sTipo = reader.GetString(3);
                    if (!reader.IsDBNull(4))
                        sAplicaMonto = reader.GetString(4);
                    if (!reader.IsDBNull(5))
                        sAplicaSubtotal = reader.GetString(5);
                    if (!reader.IsDBNull(6))
                        sAplicaSubDesc = reader.GetString(6);
                    if (!reader.IsDBNull(7))
                        sAplicaImpuesto1 = reader.GetString(7);
                    if (!reader.IsDBNull(8))
                        sAplicaImpuesto2 = reader.GetString(8);
                    if (!reader.IsDBNull(9))
                        sAplicaRubro1 = reader.GetString(9);
                    if (!reader.IsDBNull(10))
                        sAplicaRubro2 = reader.GetString(10);
                    if (!reader.IsDBNull(11))
                        nMontoMinimo = reader.GetDecimal(11);
                    if (!reader.IsDBNull(12))
                        sEstado = reader.GetString(12);
                    if (!reader.IsDBNull(13))
                        sCtrRetencion = reader.GetString(13);
                    if (!reader.IsDBNull(14))
                        sCtaRetencion = reader.GetString(14);
                    if (!reader.IsDBNull(15))
                        sRetPorArticulo = reader.GetString(15);
                    if (!reader.IsDBNull(16))
                        sEsAutoretenedor = reader.GetString(16);
                    if (!reader.IsDBNull(17))
                        sClasificacionArt = reader.GetString(17);
                    if (!reader.IsDBNull(18))
                        sCtrAutoRetencion = reader.GetString(18);
                    if (!reader.IsDBNull(19))
                        sCtaAutoRetencion = reader.GetString(19);
                    if (!reader.IsDBNull(20))
                        isAplicaRetencion = reader.GetString(20);
                    if (!reader.IsDBNull(21))
                        isRetencionBase = reader.GetString(21);
                    if (!reader.IsDBNull(22))
                        isNoAplicProvAuto = reader.GetString(22);
                    if (!reader.IsDBNull(23))
                        isTipoExcepcion = reader.GetString(23);
                    if (!reader.IsDBNull(24))
                        isAplicaImp1Desc = reader.GetString(24);
                    if (!reader.IsDBNull(25))
                        isAplicaImp1NoDesc = reader.GetString(25);
                    if (!reader.IsDBNull(26))
                        isUsaTarifaUVT = reader.GetString(26);
                    if (!reader.IsDBNull(27))
                        isConsecVenta = reader.GetString(27);
                    if (!reader.IsDBNull(28))
                        isConsecCompra = reader.GetString(28);
                    if (!reader.IsDBNull(29))
                        isPermiteGenRetCero = reader.GetString(29);
                    if (!reader.IsDBNull(30))
                        isTipoApliCancelar = reader.GetString(30);
                    bExiste = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar los historicos de los pedidos. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
            return bExiste;
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void FillInstance(string prmsCodigoRetencion, string prmsDescRetencion, decimal prmnPorcentaje, bool prmbTipo, bool prmbAplicaMonto, bool prmbAplicaSubtotal, bool prmbAplicaSubDesc, bool prmbAplicaImpuesto1, bool prmbAplicaImpuesto2, bool prmbAplicaRubro1, bool prmbAplicaRubro2, decimal prmnMontoMinimo, bool prmbEstado, string prmsCtrRetencion, string prmsCtaRetencion, bool prmbRetPorArticulo, bool prmbEsAutoretenedor, string prmsClasificacionArt, string prmsCtrAutoRetencion, string prmsCtaAutoRetencion)
        {

            sCodigoRetencion = prmsCodigoRetencion;
            sDescRetencion = prmsDescRetencion;
            nPorcentaje = prmnPorcentaje;
            nMontoMinimo = prmnMontoMinimo;
            sTipo = CP_RET_CANCELA;
            if (prmbTipo)
            {
                sTipo = CP_RET_REGISTRO;
            }
            sAplicaMonto = CP_RET_NO_APLICA;
            if (prmbAplicaMonto)
            {
                sAplicaMonto = CP_RET_APLICA;
            }
            sAplicaSubtotal = CP_RET_NO_APLICA;
            if (prmbAplicaSubtotal)
            {
                sAplicaSubtotal = CP_RET_APLICA;
            }
            sAplicaSubDesc = CP_RET_NO_APLICA;
            if (prmbAplicaSubDesc)
            {
                sAplicaSubDesc = CP_RET_APLICA;
            }
            sAplicaImpuesto1 = CP_RET_NO_APLICA;
            if (prmbAplicaImpuesto1)
            {
                sAplicaImpuesto1 = CP_RET_APLICA;
            }
            sAplicaImpuesto2 = CP_RET_NO_APLICA;
            if (prmbAplicaImpuesto2)
            {
                sAplicaImpuesto2 = CP_RET_APLICA;
            }
            sAplicaRubro1 = CP_RET_NO_APLICA;
            if (prmbAplicaRubro1)
            {
                sAplicaRubro1 = CP_RET_APLICA;
            }
            sAplicaRubro2 = CP_RET_NO_APLICA;
            if (prmbAplicaRubro2)
            {
                sAplicaRubro2 = CP_RET_APLICA;
            }
            sEstado = CP_RET_INACTIVA;
            if (prmbEstado)
            {
                sEstado = CP_RET_ACTIVA;
            }
            sCtrRetencion = prmsCtrRetencion;
            sCtaRetencion = prmsCtaRetencion;
            sRetPorArticulo = CP_RET_NO_APLICA;
            if (prmbRetPorArticulo)
            {
                sRetPorArticulo = CP_RET_APLICA;
            }
            sClasificacionArt = prmsClasificacionArt;
            sCtrAutoRetencion = prmsCtrAutoRetencion;
            sCtaAutoRetencion = prmsCtaAutoRetencion;
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void GetInstance(ref string prmsCodigoRetencion, ref string prmsDescRetencion, ref decimal prmnPorcentaje, ref bool prmbTipo, ref bool prmbAplicaMonto, ref bool prmbAplicaSubtotal, ref bool prmbAplicaSubDesc, ref bool prmbAplicaImpuesto1, ref bool prmbAplicaImpuesto2, ref bool prmbAplicaRubro1, ref bool prmbAplicaRubro2, ref decimal prmnMontoMinimo, ref bool prmbEstado, ref string prmsCtrRetencion, ref string prmsCtaRetencion, ref bool prmbRetPorArticulo, ref bool prmbEsAutoretenedor, ref string prmsClasificacionArt, ref string prmsCtrAutoRetencion, ref string prmsCtaAutoRetencion)
        {

            prmsCodigoRetencion = sCodigoRetencion;
            prmsDescRetencion = sDescRetencion;
            prmnPorcentaje = nPorcentaje;
            prmnMontoMinimo = nMontoMinimo;
            prmbTipo = false;
            if ((sTipo == CP_RET_REGISTRO))
            {
                prmbTipo = true;
            }
            prmbAplicaMonto = false;
            if ((sAplicaMonto == CP_RET_APLICA))
            {
                prmbAplicaMonto = true;
            }
            prmbAplicaSubtotal = false;
            if ((sAplicaSubtotal == CP_RET_APLICA))
            {
                prmbAplicaSubtotal = true;
            }
            prmbAplicaSubDesc = false;
            if ((sAplicaSubDesc == CP_RET_APLICA))
            {
                prmbAplicaSubDesc = true;
            }
            prmbAplicaImpuesto1 = false;
            if ((sAplicaImpuesto1 == CP_RET_APLICA))
            {
                prmbAplicaImpuesto1 = true;
            }
            prmbAplicaImpuesto2 = false;
            if ((sAplicaImpuesto2 == CP_RET_APLICA))
            {
                prmbAplicaImpuesto2 = true;
            }
            prmbAplicaRubro1 = false;
            if ((sAplicaRubro1 == CP_RET_APLICA))
            {
                prmbAplicaRubro1 = true;
            }
            prmbAplicaRubro2 = false;
            if ((sAplicaRubro2 == CP_RET_APLICA))
            {
                prmbAplicaRubro2 = true;
            }
            prmbEstado = false;
            if ((sEstado == CP_RET_ACTIVA))
            {
                prmbEstado = true;
            }
            prmsCtrRetencion = sCtrRetencion;
            prmsCtaRetencion = sCtaRetencion;
            prmbRetPorArticulo = false;
            if ((sRetPorArticulo == CP_RET_APLICA))
            {
                prmbRetPorArticulo = true;
            }
            prmsClasificacionArt = sClasificacionArt;
            prmsCtrAutoRetencion = sCtrAutoRetencion;
            prmsCtaAutoRetencion = sCtaAutoRetencion;
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public bool ExistsKey(string prmsCodigoRetencion, ref bool bParamExiste)
        {
            string sSqlCmd;
            bool lbOK;

            sSqlCmd = "SELECT COUNT(codigo_retencion) FROM " + sDBName + " WHERE codigo_retencion = @CODIGO";
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("CODIGO", prmsCodigoRetencion);
            lbOK = (GestorDatos.NumeroRegistros(sSqlCmd,parametros) != 0);
            return lbOK;
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public bool ExistsRetencionXArticulo(string prmsCodigoRetencion, string prmsCodigoClasificacion)
        {
            bool bExiste = false;
            string sSqlCmd;
            SQLiteParameterList parametros = new SQLiteParameterList();


            sSqlCmd = "SELECT count(0) FROM " + sDBName + " WHERE clasificacion_art = @CASIFICACION";
            parametros.Add(new SQLiteParameter("CASIFICACION", prmsCodigoClasificacion));
            if (GestorDatos.NumeroRegistros(sSqlCmd, parametros) != 0)
            {
                sSqlCmd = " SELECT COUNT(codigo_retencion) FROM " + sDBName + " WHERE codigo_retencion = @RETENCION AND clasificacion_art = @CASIFICACION";
                parametros.Add(new SQLiteParameter("RETENSION", prmsCodigoClasificacion));
                if (GestorDatos.NumeroRegistros(sSqlCmd,parametros) != 0)
                {
                    bExiste = false;
                }
                else
                {
                    bExiste = true;
                }
            }
            else
            {
                bExiste = false;
            }
            return bExiste;
        }

        /// <summary>
        /// Esta función calcula el monto de la retención, según el porcentaje, definido, las flags sobre que rubros aplica y los detalles del crédito que se pasan como parámetro. Se asume que los datos de la retención estan cargados en las varibles de la instancia.
        /// </summary>
        /* Separacion=ND*/
        public decimal CalcularMonto(decimal nMontoTotal, decimal nSubTotal, decimal nDescuento, decimal nImpuesto1, decimal nImpuesto2, decimal nRubro1, decimal nRubro2, decimal nTipoCambio, ref decimal pnMontoBase, string sPaisInstalado)
        {
            decimal nMontoBase;
            decimal nMontoRetencion=0;

            nMontoBase = 0;
            // MRL Rte Fuente Proveedores Independientes -->
            if (!(((isUsaTarifaUVT == CP_SI) && ibAplicarTarifaUVT)))
            {
                // MRL Proyecto Colombia R4 --> Esta función setea la variable de instancia nPorcentaje con el porcentaje de excepción obtenido
                if (!(ObtenerPorcentajeExcepcion(isPais, isDivGeo1, isDivGeo2, isRegimen)))
                {
                    return 0;
                }
            }
            if ((sAplicaMonto == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + nMontoTotal);
            }
            if ((sAplicaSubtotal == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + nSubTotal);
            }
            if ((sAplicaSubDesc == CP_RET_APLICA))
            {
                nMontoBase = ((nMontoBase + nSubTotal) - nDescuento);
            }
            if ((sAplicaImpuesto1 == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + nImpuesto1);
            }
            if ((sAplicaImpuesto2 == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + nImpuesto2);
            }
            if ((sAplicaRubro1 == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + nRubro1);
            }
            if ((sAplicaRubro2 == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + nRubro2);
            }
            if ((isAplicaRetencion == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + inMontoOtraRet);
            }
            if ((isAplicaImp1Desc == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + inMontoImp1Desc);
            }
            if ((isAplicaImp1NoDesc == CP_RET_APLICA))
            {
                nMontoBase = (nMontoBase + inMontoImp1NoDesc);
            }
            if (((nMontoBase * nTipoCambio) > nMontoMinimo))
            {
                // MRL Rte Fuente Proveedores Independientes R7 -->
                if (((isUsaTarifaUVT == CP_SI) && ibAplicarTarifaUVT))
                {
                   // nMontoRetencion = CalcularMontoProvIndependientes(ref nMontoBase);
                }
                else
                {
                    // Set nMontoRetencion = TwoDecimals( nMontoBase * nPorcentaje / 100.00 )
                    // Set nMontoRetencion = NumberRound(nMontoBase * nPorcentaje / 100.00, cnPrecisionDecimalEnRetenciones)
                    // TAQ > 32287
                    // NBH >> Caso 38367
                    /*
                        // If IsBaseMil ( nPorcentaje )
                    */
                    if (((sPaisInstalado == COLOMBIA) && (nPorcentaje < 1)))
                    {
                        // NBH >> Caso 38367
                        /*
                            // NBH >> Caso #35878
                            // Set nMontoRetencion = NumberRound(nMontoBase * nPorcentaje, cnPrecisionDecimalEnRetenciones)
                            // Set nMontoRetencion = NumberRound(nMontoBase * nPorcentaje / 1000, cnPrecisionDecimalEnRetenciones)
                        */
                        nMontoRetencion = Math.Round((nMontoBase * nPorcentaje), cnPrecisionDecimalEnRetenciones);
                    }
                    else
                    {
                        nMontoRetencion = Math.Round(((nMontoBase * nPorcentaje) / 100), cnPrecisionDecimalEnRetenciones);
                    }
                }
            }
            else
            {
                nMontoRetencion = 0;
            }
            pnMontoBase = nMontoBase;
            // NBH >> FIN: Proyecto Ajustes Colombia
            return nMontoRetencion;
        }

        /// <summary>
        /// Retorna el Tipo de Retención
        /// </summary>
        /* Separacion=ND*/
        public string TipoRetencion()
        {

            return sTipo;
        }

        /// <summary>
        /// Indica si la retención es de tipo AutoRetenedora o no
        /// </summary>
        /* Separacion=ND*/
        public bool EsRetAutoRetenedora()
        {

            return (sEsAutoretenedor == CP_SI);
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void FillInstanceExt(string psAplicaRetencion, string psRetencionBase, string psNoAplicProvAuto, string psTipoExcepcion, string psTipoRetencion, string psAplicaImp1Desc, string psAplicaImp1NoDesc, string psUsaTarifaUVT, string psConsecutivoVenta, string psConsecutivoCompra)
        {

            isAplicaRetencion = psAplicaRetencion;
            isRetencionBase = psRetencionBase;
            isNoAplicProvAuto = psNoAplicProvAuto;
            isTipoExcepcion = psTipoExcepcion;
            sEsAutoretenedor = psTipoRetencion;
            // MRL Ajustes Colombia R5 -->
            isAplicaImp1Desc = psAplicaImp1Desc;
            isAplicaImp1NoDesc = psAplicaImp1NoDesc;
            // MRL Ajustes Colombia R5 <--
            // MRL Rte Fuente Proveedores Independientes -->
            isUsaTarifaUVT = psUsaTarifaUVT;
            // MRL Rte Fuente Proveedores Independientes <--
            // ERG Mejora Consecutivo Retenciones 6.0 R7 -->
            isConsecVenta = psConsecutivoVenta;
            isConsecCompra = psConsecutivoCompra;
            // ERG Mejora Consecutivo Retenciones 6.0 R7 <--
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void GetInstanceExt(ref string psAplicaRetencion, ref string psRetencionBase, ref string psNoAplicProvAuto, ref string psTipoExcepcion, ref string psTipoRetencion, ref string psAplicaImp1Desc, ref string psAplicaImp1NoDesc, ref string psUsaTarifaUVT, ref string psConsecutivoVenta, ref string psConsecutivoCompra)
        {

            psAplicaRetencion = isAplicaRetencion;
            psRetencionBase = isRetencionBase;
            psNoAplicProvAuto = isNoAplicProvAuto;
            psTipoExcepcion = isTipoExcepcion;
            psTipoRetencion = sEsAutoretenedor;
            // MRL Ajustes Colombia R5 -->
            psAplicaImp1Desc = isAplicaImp1Desc;
            psAplicaImp1NoDesc = isAplicaImp1NoDesc;
            // MRL Ajustes Colombia R5 <--
            // MRL Rte Fuente Proveedores Independientes -->
            psUsaTarifaUVT = isUsaTarifaUVT;
            // MRL Rte Fuente Proveedores Independientes <--
            // ERG Mejora Consecutivo Retenciones 6.0 R7 -->
            psConsecutivoVenta = isConsecVenta;
            psConsecutivoCompra = isConsecCompra;
            // ERG Mejora Consecutivo Retenciones 6.0 R7 <--
        }

        /// <summary>
        /// Obtiene el porcentaje de excepción que aplica para la retención, esto dependiendo de si es por ciudad o regimen
        /// </summary>
        /* Separacion=ND*/
        public bool ObtenerPorcentajeExcepcion(string psPais, string psDivGeo1, string psDivGeo2, string psRegimen)
        {

            if ((isTipoExcepcion == CP_RET_EXCEP_CIUDAD))
            {
                if (((psPais != null) && ((psDivGeo1 != null) && (psDivGeo2 != null))))
                {
                    if (fciExcepcionCiudad.ReadOnKey(psPais, psDivGeo1, psDivGeo2, sCodigoRetencion,compania))
                    {
                        nPorcentaje = fciExcepcionCiudad.Porcentaje;
                    }
                }
            }
            if ((isTipoExcepcion == CP_RET_EXCEP_REGIMEN))
            {
                if ((psRegimen != null))
                {
                    if (fciExcepcionRegimen.ReadOnKey(psRegimen, sCodigoRetencion,this.compania))
                    {
                        // MRL Ajustes Colombia R5 --> Si el porcentaje es 0 se toma el default
                        if ((fciExcepcionRegimen.Porcentaje != 0))
                        {
                            nPorcentaje = fciExcepcionRegimen.Porcentaje;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Libera las instancias que se inicializaron en el InitInstance( )
        /// </summary>
        /* Separacion=ND*/
        //public void ReleInstance()
        //{

        //    fciExcepcionCiudad.ReleInstance();
        //    fciExcepcionRegimen.ReleInstance();
        //    vfciCPXML.ReleaseInstance();
        //    //fclsLockRow.ReleInstance();
        //}

        /// <summary>
        /// Esta función permite setear los valores de ciudad y regimen, así como el monto base de la retención que serán utilizados para el calculo. IMPORTANTE: Se debe de invocar antes de calcular Monto para que el cálculo se haga tomando en cuenta las excepciones y el monto de la retención base en caso de la retenciones tipo bomberil.
        /// </summary>
        /* Separacion=ND*/
        public void SetDatosCalcularMonto(decimal pnMontoOtraRet, string psPais, string psDivGeo1, string psDivGeo2, string psRegimen)
        {

            inMontoOtraRet = pnMontoOtraRet;
            isPais = psPais;
            isDivGeo1 = psDivGeo1;
            isDivGeo2 = psDivGeo2;
            isRegimen = psRegimen;
        }

        /// <summary>
        /// Retorna la retencion base
        /// </summary>
        /* Separacion=ND*/
        public string RetencionBase()
        {
            return isRetencionBase;
        }

        /// <summary>
        /// Esta función permite setear los valores de impuesto 1 asumido descontable e impuesto 1 asumido no descontable. IMPORTANTE: Se debe de invocar antes de calcular Monto para que al efectuar el calculo de la retención estos valores estén debidamente cargados. Principalmente se deben utilizar para compras de consumo a régimen simplificado.
        /// </summary>
        /* Separacion=ND*/
        public void SetBasesImpuesto(decimal pnMontoImp1Desc, decimal pnMontoImp1NoDesc)
        {

            inMontoImp1Desc = pnMontoImp1Desc;
            inMontoImp1NoDesc = pnMontoImp1NoDesc;
        }

        /// <summary>
        /// Obtiene el centro de costo y la cuenta contable que aplica para la retención, esto dependiendo de si es por ciudad, regimen o default
        /// </summary>
        /* Separacion=ND*/
        public bool ObtenerDatosContables(string psPais, string psDivGeo1, string psDivGeo2, string psRegimen, ref string psCentroCosto, ref string psCuentaContable)
        {
            bool lbOk;

            // Se inicializan los parámetros receive
            psCentroCosto = null;
            psCuentaContable = null;
            // Se obtiene la información de la retención
            lbOk = ReadOnKey(sCodigoRetencion);
            // Se asignan los datos contables default de la retención
            if (lbOk)
            {
                psCentroCosto = sCtrRetencion;
                psCuentaContable = sCtaRetencion;
            }
            else
            {
                //SPSDisplayErrorMsg((("fclsRetencion.ObtenerDatosContables - Error leyendo la información de la retención: " + sCodigoRetencion) +
                //     "."), NombreTabla);
            }
            if (lbOk)
            {
                if ((isTipoExcepcion == CP_RET_EXCEP_CIUDAD))
                {
                    if (((!string.IsNullOrEmpty(psPais)) && ((!string.IsNullOrEmpty(psDivGeo1)) && (!string.IsNullOrEmpty(psDivGeo2)))))
                    {
                        if (fciExcepcionCiudad.ReadOnKey(psPais, psDivGeo1, psDivGeo2, sCodigoRetencion,this.compania))
                        {
                            psCentroCosto = fciExcepcionCiudad.CtrCtoExcepCiu;
                            psCuentaContable = fciExcepcionCiudad.CtaCtbExcepCiu;
                        }
                    }
                }
                else
                {
                    if ((isTipoExcepcion == CP_RET_EXCEP_REGIMEN))
                    {
                        if ((!string.IsNullOrEmpty(psRegimen)))
                        {
                            if (fciExcepcionRegimen.ReadOnKey(psRegimen, sCodigoRetencion,this.compania))
                            {
                                psCentroCosto = fciExcepcionRegimen.CtrCtoExcepReg;
                                psCuentaContable = fciExcepcionRegimen.CtaCtbExcepReg;
                            }
                        }
                    }
                }
            }
            return lbOk;
        }

        /// <summary>
        /// Esta función recalcula el monto de la retención, según el monto base dígitado por el usuario, por lo que no toma los montos que correspondan según configuración sino el monto enviado por parámetros. El porcentaje de calculo si se toma con base en la configuración de la retención.
        /// </summary>
        /* Separacion=ND*/
        public decimal RecalcularMonto(decimal pnMontoBase, decimal pnTipoCambio, string sPaisInstalado)
        {
            decimal lnMontoRetencion=0;

            // MRL Rte Fuente Proveedores Independientes -->
            if (!(((isUsaTarifaUVT == CP_SI) && ibAplicarTarifaUVT)))
            {
                if (!(ObtenerPorcentajeExcepcion(isPais, isDivGeo1, isDivGeo2, isRegimen)))
                {
                    return (0);
                }
            }
            if (((pnMontoBase * pnTipoCambio) > nMontoMinimo))
            {
                // MRL Rte Fuente Proveedores Independientes R7 -->
                if (((isUsaTarifaUVT == CP_SI) && ibAplicarTarifaUVT))
                {
                    //lnMontoRetencion = CalcularMontoProvIndependientes(ref pnMontoBase);
                }
                else
                {
                    if (((sPaisInstalado == COLOMBIA) && (nPorcentaje < 1)))
                    {
                        lnMontoRetencion = Math.Round((pnMontoBase * nPorcentaje), cnPrecisionDecimalEnRetenciones);
                    }
                    else
                    {
                        lnMontoRetencion = Math.Round(((pnMontoBase * nPorcentaje) / 100), cnPrecisionDecimalEnRetenciones);
                    }
                }
            }
            else
            {
                lnMontoRetencion = 0;
            }
            return lnMontoRetencion;
        }

        /// <summary>
        /// Setea el código de la retención en la instancia
        /// </summary>
        /* Separacion=ND*/
        public void SetCodigoRetencion(string psCodigo)
        {

            sCodigoRetencion = psCodigo;
        }

        //BORRAR

        /// <summary>
        /// Esta función calcula el monto de la retención, según los datos del proveedor independiente. Se asume que la instancia del proveedor independiente está cargada.
        /// </summary>
        /* Separacion=ND*/
        //public decimal CalcularMontoProvIndependientes(ref decimal pnMontoBase)
        //{
        //    decimal lnMontoRetencion;
        //    decimal lnValorUVT;
        //    decimal lnTotalADepurar;
        //    decimal lnBaseMenDepurada;
        //    decimal lnBaseMenUVT;
        //    decimal lnBaseDepurada;
        //    decimal lnBaseDepuradaUVT;
        //    decimal lnUVTMenosMens;
        //    decimal lnUVTMasMens;
        //    decimal lnUVTMenos;
        //    decimal lnUVTMas;
        //    decimal lnMontoRetUVT;

        //    lnMontoRetencion = 0;
        //    lnValorUVT = 0;
        //    lnTotalADepurar = 0;
        //    lnBaseMenDepurada = 0;
        //    lnBaseMenUVT = 0;
        //    lnBaseDepurada = 0;
        //    lnBaseDepuradaUVT = 0;
        //    lnUVTMenosMens = 0;
        //    lnUVTMasMens = 0;
        //    lnUVTMenos = 0;
        //    lnUVTMas = 0;
        //    lnMontoRetUVT = 0;
        //    // Si la tarifa es 0 el monto de la retención también
        //    if ((ifciValoresCertif.inPorcentajeRet == 0))
        //    {
        //        return lnMontoRetencion;
        //    }
        //    lnValorUVT = 0;
        //    string value = string.Empty;
        //    string sSqlCmd =
        //        "SELECT valor FROM " + ConcatCompany("globales") + " WHERE modulo = '" + AS + "' AND nombre = '" + AS_VALOR_UVT + "'";
        //    SQLiteDataReader reader = null;
        //    try
        //    {
        //        reader = GestorDatos.EjecutarConsulta(sSqlCmd);
        //        if (reader.Read())
        //        {
        //            if (!reader.IsDBNull(0))
        //                value = reader.GetString(0);
        //            lnValorUVT = Convert.ToDecimal(value);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al cargar los historicos de los pedidos. " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (reader != null)
        //            reader.Close();
        //    }

        //    // Se obtiene el acumulado de montos a depurar en la base gravada
        //    lnTotalADepurar = ((((ifciValoresCertif.inAportesSalud + ifciValoresCertif.inAportesPensionObl) + ifciValoresCertif.inAportesPensionVol) +
        //         ifciValoresCertif.inAportesAFC) +
        //         ifciValoresCertif.inInteresesPrestamo);
        //    // Se calcula la base mensual depurada
        //    lnBaseMenDepurada = (ifciValoresCertif.inPagosAbonos - lnTotalADepurar);
        //    // Se calcula la base de la retención depurada
        //    lnBaseDepurada = ((pnMontoBase * lnBaseMenDepurada) / ifciValoresCertif.inPagosAbonos);
        //    // Se calcula la base depurada de la retención en UVT
        //    if ((lnValorUVT > 0))
        //    {
        //        lnBaseDepuradaUVT = (lnBaseDepurada / lnValorUVT);
        //    }
        //    else
        //    {
        //        return lnMontoRetencion;
        //        throw new Exception("No se ha establecido el valor UVT en las globales de Administración del Sistema, por lo que no se pudo calcular la base en UVT y por ende la retención.");
        //    }
        //    if (!(vfciCPXML.ObtenerValoresUVTCalculo(CP_RANGOS_UVT, ifciValoresCertif.inPorcentajeRet, lnUVTMenosMens, lnUVTMasMens)))
        //    {
        //        return lnMontoRetencion;
        //    }
        //    lnUVTMenos = ((lnBaseDepuradaUVT * lnUVTMenosMens) / ifciValoresCertif.inBaseRetUVT);
        //    lnUVTMas = ((lnBaseDepuradaUVT * lnUVTMasMens) / ifciValoresCertif.inBaseRetUVT);
        //    // Se calcula el monto de la retención en UVT
        //    lnMontoRetUVT = (((lnBaseDepuradaUVT - lnUVTMenos) * (ifciValoresCertif.inPorcentajeRet / 100)) + lnUVTMas);
        //    // Se calcula el monto de la retención en pesos
        //    lnMontoRetencion = (lnMontoRetUVT * lnValorUVT);
        //    // Se redondea el monto de la retención
        //    if ((lnMontoRetencion >= 1000))
        //    {
        //        lnMontoRetencion = Math.Round(lnMontoRetencion, -(3));
        //    }
        //    else
        //    {
        //        lnMontoRetencion = Math.Round(lnMontoRetencion, -(2));
        //    }
        //    pnMontoBase = lnBaseDepurada;
        //    return lnMontoRetencion;
        //}

        //FINBORRAR

        /// <summary>
        /// Se llena la información de si la retención permite generar valores en cero
        /// </summary>
        /* Separacion=ND*/
        public void FillPermGenRetCero(bool pbPermiteGenerar)
        {

            if (pbPermiteGenerar)
            {
                isPermiteGenRetCero = "S";
            }
            else
            {
                isPermiteGenRetCero = "N";
            }
        }

        /// <summary>
        /// Función que obtiene el valor de si la retención permite generar valores en cero
        /// </summary>
        /* Separacion=ND*/
        public void GetPermGenRetCero(ref bool pbPermiteGenerar)
        {

            if ((isPermiteGenRetCero == "S"))
            {
                pbPermiteGenerar = true;
            }
            else
            {
                pbPermiteGenerar = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public bool PermiteValoresEnCero()
        {
            bool lbPermite = false;

            GetPermGenRetCero(ref lbPermite);
            return lbPermite;
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void GetTipoApliCancelar(ref string psTipoAplicCancelar)
        {

            psTipoAplicCancelar = isTipoApliCancelar;
        }

        /// <summary>
        /// 
        /// </summary>
        /* Separacion=ND*/
        public void FillTipoApliCancelar(string psTipoApliCancelar)
        {

            isTipoApliCancelar = psTipoApliCancelar;
        }

        #endregion Funciones

    } // fclsRetencion    

   
}