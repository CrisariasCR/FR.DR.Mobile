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

using System.Data;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Data.SQLiteBase;
using FR.Core.Model;
using Android.App;
using Cirrious.MvvmCross.ViewModels;

namespace FR.DR.Core.Data.Documentos.Retenciones
{
    public class fclsExcepcionRegimen
    {
        #region Variables

        private decimal cnPrecisionDecimalEnRetenciones;
        public decimal PrecisionDecimalEnRetenciones
        {
            get { return cnPrecisionDecimalEnRetenciones; }
            set { cnPrecisionDecimalEnRetenciones = value; }
        }


        private decimal inPrecisionDecimalEnRetenciones;
        public decimal iPrecisionDecimalEnRetenciones
        {
            get { return inPrecisionDecimalEnRetenciones; }
            set { inPrecisionDecimalEnRetenciones = value; }
        }						
		
	
        private string isCodigoRetencion;
        public string CodigoRetencion
        {
            get { return isCodigoRetencion; }
            set { isCodigoRetencion = value; }
        }


        private string isCodigoExcepcion;
        public string CodigoExcepcion
        {
            get { return isCodigoExcepcion; }
            set { isCodigoExcepcion = value; }
        }


        private decimal inPorcentaje;
        public Decimal Porcentaje
        {
            get { return inPorcentaje; }
            set { inPorcentaje = value; }
        }

        private string isCtrCtoExcepReg;
        public string CtrCtoExcepReg
        {
            get { return isCtrCtoExcepReg; }
            set { isCtrCtoExcepReg = value; }
        }

        private string isCtaCtbExcepReg;
        public string CtaCtbExcepReg
        {
            get { return isCtaCtbExcepReg; }
            set { isCtaCtbExcepReg = value; }
        }

        #endregion Variables


        #region Constructores

        /// <summary>
        /// Constructor de la clase fclsExcepcionRegimen
        /// </summary>
        public fclsExcepcionRegimen()
        {
        }


        /// <summary>
        /// Constructor de la clase fclsExcepcionRegimen
        /// </summary>
        public fclsExcepcionRegimen(string psCodigoRetencion)
        {
            isCodigoRetencion = psCodigoRetencion;

            InitInstance();
        }


        #endregion Constructores

        #region Funciones


        /// <summary>
        /// Inicialice la instancia.
        /// </summary>
        public bool InitInstance()
        {

            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            SQLiteDataReader reader = null;
            #endregion Definición de Varaibles


            // Valor por defecto de la precisión decimal
            inPrecisionDecimalEnRetenciones = 2;

            try
            {
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico

                consulta = " SELECT CODIGO, CODIGO_RETENCION, PORCENTAJE, CENTRO_COSTO, CUENTA_CONTABLE " +
                           " FROM " + Table.ERPADMIN_EXCEP_REGIMEN +
                           " WHERE CODIGO_RETENCION = '" + isCodigoRetencion + "' ";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                if (reader.Read())
                {
                    isCodigoExcepcion = reader.GetString(0);
                    isCodigoRetencion = reader.GetString(1);
                    inPorcentaje = Convert.ToDecimal(reader.GetValue(2));
                    isCtrCtoExcepReg = reader.GetString(3);
                    isCtaCtbExcepReg = reader.GetString(4);

                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los lotes asociados a la boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return procesoExitoso;

          

      
        }

        /// <summary>
        /// Hace el set de la precision decimal usada par el calculo de las retenciones
        /// </summary>
        public Decimal SetPrecisionDecimal(decimal pnPrecisionDecimalEnRetenciones)
        {

            inPrecisionDecimalEnRetenciones = pnPrecisionDecimalEnRetenciones;
            return inPrecisionDecimalEnRetenciones;
        }

        /// <summary>
        /// Limpiar la instancia.
        /// </summary>
        public void ClearInstance()
        {

            inPrecisionDecimalEnRetenciones = 0;
            isCodigoExcepcion = String.Empty;
            isCodigoRetencion = String.Empty;
            inPorcentaje = 0;
            // MRL Ajustes Colombia R5 -->
            isCtrCtoExcepReg = String.Empty;
            isCtaCtbExcepReg = String.Empty;
            // MRL Ajustes Colombia R5 <--
        
        }
       
        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnKey(string prmsCodigoExcepcion, string prmsCodigoRetencion, string prmsCompania)
        {


            string consulta;
            bool bExiste;
            SQLiteDataReader reader = null;


            try
            {
                bExiste = false;

                consulta = " SELECT CODIGO, CODIGO_RETENCION, PORCENTAJE, CENTRO_COSTO, CUENTA_CONTABLE" +
                               " FROM " + Table.ERPADMIN_EXCEP_REGIMEN +
                               " WHERE CODIGO_RETENCION = @CODIGO_RET AND CODIGO = @CODIGO AND COD_CIA=@COMPANIA";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@CODIGO_RET", SqlDbType.NVarChar, prmsCodigoRetencion),
                                                    GestorDatos.SQLiteParameter("@CODIGO", SqlDbType.NVarChar, prmsCodigoExcepcion),
                                                    GestorDatos.SQLiteParameter("@COMPANIA", SqlDbType.NVarChar, prmsCompania)
                                                  });

                reader = GestorDatos.EjecutarConsulta(consulta, parametros);

                if (reader.Read())
                {
                    isCodigoExcepcion = reader.GetString(0);
                    isCodigoRetencion = reader.GetString(1);
                    inPorcentaje = Convert.ToDecimal(reader.GetValue(2));
                    isCtrCtoExcepReg = reader.GetString(3);
                    isCtaCtbExcepReg = reader.GetString(4);

                    bExiste = true;
                }
                else
                {
                    bExiste = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
        public void FillInstance(decimal pnPrePrecisionDecimalEnRetenciones, string psCodigoExcepcion, string psCodigoRetencion, decimal pnPorcentaje)
        {

            inPrecisionDecimalEnRetenciones = pnPrePrecisionDecimalEnRetenciones;
            isCodigoExcepcion = psCodigoExcepcion;
            isCodigoRetencion = psCodigoRetencion;
            inPorcentaje = pnPorcentaje;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetInstance(decimal pnPrecisionDecimalEnRetenciones, string psCodigoExcepcion, string psCodigoRetencion, decimal pnPorcentaje)
        {

            pnPrecisionDecimalEnRetenciones = inPrecisionDecimalEnRetenciones;
            psCodigoExcepcion = isCodigoExcepcion;
            psCodigoRetencion = isCodigoRetencion;
            pnPorcentaje = inPorcentaje;
        }

        /// <summary>
        /// Cargar un registro por su llave.
        /// </summary>
        public bool ExistsKey(string prmsCodigo, string prmsCodigoRetencion, ref bool bParamExiste)
        {

            string consulta;
            SQLiteDataReader reader=null;
            try
            {


                consulta = " SELECT CODIGO, CODIGO_RETENCION" +
                               " FROM " + Table.ERPADMIN_EXCEP_REGIMEN +
                               " WHERE CODIGO_RETENCION = @CODIGO_RET AND CODIGO = @CODIGO";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@CODIGO_RET", SqlDbType.NVarChar, prmsCodigoRetencion),
                                                    GestorDatos.SQLiteParameter("@CODIGO", SqlDbType.NVarChar, prmsCodigo)
                                                  });

                reader = GestorDatos.EjecutarConsulta(consulta, parametros);

                if (reader.Read())
                {
                    bParamExiste = true;
                }
                else
                {
                    bParamExiste = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
 
            }

            return bParamExiste;

    
        }

        /// <summary>
        /// 
        /// </summary>
        public void FillInstanceExt(string psCtrCtoExcepReg, string psCtaCtbExcepReg)
        {

            isCtrCtoExcepReg = psCtrCtoExcepReg;
            isCtaCtbExcepReg = psCtaCtbExcepReg;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetInstanceExt(ref string psCtrCtoExcepReg, ref string psCtaCtbExcepReg)
        {

            psCtrCtoExcepReg = isCtrCtoExcepReg;
            psCtaCtbExcepReg = isCtaCtbExcepReg;
        }

        /// <summary>
        /// Verifica si la tabla posee excepciones para una determinada retención.
        /// </summary>
        public bool ExistenExcep(string prmsCodigoRetencion, ref bool bParamExiste)
        {
            string consulta;
            bool lbOk;

            consulta = " SELECT CODIGO" + 
                           " FROM " + Table.ERPADMIN_EXCEP_REGIMEN +
                           " WHERE CODIGO_RETENCION = @CODIGO";
            
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add(new SQLiteParameter("CODIGO", prmsCodigoRetencion));


            if (GestorDatos.NumeroRegistros(consulta,parametros) > 0)
            {
                lbOk = true;
            }
            else
            {
                lbOk = false;
            }
           
            return lbOk;
        }

        #endregion Funciones

    }
}