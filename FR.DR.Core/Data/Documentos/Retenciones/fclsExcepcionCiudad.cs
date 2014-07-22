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
    public class fclsExcepcionCiudad
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

        private string isCodigoPais;
        public string CodigoPais
        {
            get { return isCodigoPais; }
            set { isCodigoPais = value; }
        }


        private string isCodigoDivGeo1;
        public string CodigoDivGeo1
        {
            get { return isCodigoDivGeo1; }
            set { isCodigoDivGeo1 = value; }
        }


        private string isCodigoDivGeo2;
        public string CodigoDivGeo2
        {
            get { return isCodigoDivGeo2; }
            set { isCodigoDivGeo2 = value; }
        }


        private string isCodigoRetencion;
        public string CodigoRetencion
        {
            get { return isCodigoRetencion; }
            set { isCodigoRetencion = value; }
        }


        private decimal inPorcentaje;
        public decimal Porcentaje
        {
            get { return inPorcentaje; }
            set { inPorcentaje = value; }
        }


        private string isCtrCtoExcepCiu;
        public string CtrCtoExcepCiu
        {
            get { return isCtrCtoExcepCiu; }
            set { isCtrCtoExcepCiu = value; }
        }


        private string isCtaCtbExcepCiu;
        public string CtaCtbExcepCiu
        {
            get { return isCtaCtbExcepCiu; }
            set { isCtaCtbExcepCiu = value; }
        }
		
				
        #endregion Variables


        #region Constructores

        /// <summary>
        /// Constructor de la clase fclsExcepcionCiudad
        /// </summary>
        public fclsExcepcionCiudad()
        {
        }

        /// <summary>
        /// Constructor de la clase fclsExcepcionCiudad
        /// </summary>
        public fclsExcepcionCiudad(string psCodigoRetencion)
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

                consulta = " SELECT PAIS, DIVISION_GEOGRAFICA1, DIVISION_GEOGRAFICA2, CODIGO_RETENCION, PORCENTAJE, CENTRO_COSTO, CUENTA_CONTABLE " +
                           " FROM " + Table.ERPADMIN_EXCEP_CIUDAD +
                           " WHERE CODIGO_RETENCION = '" + isCodigoRetencion + "' ";

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                if (reader.Read())
                {
                    isCodigoPais = reader.GetString(0);
                    isCodigoDivGeo1 = reader.GetString(1);
                    isCodigoDivGeo2 = reader.GetString(2);
                    isCodigoRetencion = reader.GetString(3);
                    inPorcentaje = Convert.ToDecimal(reader.GetValue(4));
                    isCtrCtoExcepCiu = reader.GetString(5);
                    isCtaCtbExcepCiu = reader.GetString(6);

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
        public void SetPrecisionDecimal(decimal pnPrecisionDecimalEnRetenciones)
        {

            inPrecisionDecimalEnRetenciones = pnPrecisionDecimalEnRetenciones;
        }

        /// <summary>
        /// Limpiar la instancia.
        /// </summary>
        public void ClearInstance()
        {

            inPrecisionDecimalEnRetenciones = 0;
            isCodigoPais = String.Empty;
            isCodigoDivGeo1 = String.Empty;
            isCodigoDivGeo2 = String.Empty;
            isCodigoRetencion = String.Empty;
            inPorcentaje = 0;
            // MRL Ajustes Colombia R5 -->
            isCtrCtoExcepCiu = String.Empty;
            isCtaCtbExcepCiu = String.Empty;
            // MRL Ajustes Colombia R5 <--
       
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnKey(string prmsCodigoPais, string prmsCodigoDivGeo1, string prmsCodigoDivGeo2, string prmsCodigoRetencion, string prmsCompania)
        {

            string consulta;
            bool bExiste;
            SQLiteDataReader reader = null;


            try
            {
                bExiste = false;

                consulta = " SELECT PAIS, DIVISION_GEOGRAFICA1, DIVISION_GEOGRAFICA2, CODIGO_RETENCION, PORCENTAJE, CENTRO_COSTO, CUENTA_CONTABLE" +
                               " FROM " + Table.ERPADMIN_EXCEP_CIUDAD +
                               " WHERE PAIS = @CODIGO_PAIS AND DIVISION_GEOGRAFICA1 = @CODIGO_DIV_GEO1 AND DIVISION_GEOGRAFICA2 = @CODIGO_DIV_GEO2 AND CODIGO_RETENCION = @CODIGO_RET AND COD_CIA=@COMPANIA";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@CODIGO_PAIS", SqlDbType.NVarChar, prmsCodigoPais),
                                                    GestorDatos.SQLiteParameter("@CODIGO_DIV_GEO1", SqlDbType.NVarChar, prmsCodigoDivGeo1),
                                                    GestorDatos.SQLiteParameter("@CODIGO_DIV_GEO2", SqlDbType.NVarChar, prmsCodigoDivGeo2),
                                                    GestorDatos.SQLiteParameter("@CODIGO_RET", SqlDbType.NVarChar, prmsCodigoRetencion),
                                                    GestorDatos.SQLiteParameter("@COMPANIA", SqlDbType.NVarChar, prmsCompania)
                                                  });

                reader = GestorDatos.EjecutarConsulta(consulta, parametros);

                if (reader.Read())
                {
                    isCodigoPais = reader.GetString(0);
                    isCodigoDivGeo1 = reader.GetString(1);
                    isCodigoDivGeo2 = reader.GetString(2);
                    isCodigoRetencion = reader.GetString(3);
                    inPorcentaje =  Convert.ToDecimal(reader.GetValue(4));

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
        public void FillInstance(decimal pnPrePrecisionDecimalEnRetenciones, string psCodigoPais, string psCodigoDivGeo1, string psCodigoDivGeo2, string psCodigoRetencion, decimal pnPorcentaje)
        {

            inPrecisionDecimalEnRetenciones = pnPrePrecisionDecimalEnRetenciones;
            isCodigoPais = psCodigoPais;
            isCodigoDivGeo1 = psCodigoDivGeo1;
            isCodigoDivGeo2 = psCodigoDivGeo2;
            isCodigoRetencion = psCodigoRetencion;
            inPorcentaje = pnPorcentaje;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetInstance(decimal pnPrecisionDecimalEnRetenciones, string psCodigoPais, string psCodigoDivGeo1, string psCodigoDivGeo2, string psCodigoRetencion, decimal pnPorcentaje)
        {

            pnPrecisionDecimalEnRetenciones = inPrecisionDecimalEnRetenciones;
            psCodigoPais = isCodigoPais;
            psCodigoDivGeo1 = isCodigoDivGeo1;
            psCodigoDivGeo2 = isCodigoDivGeo2;
            psCodigoRetencion = isCodigoRetencion;
            pnPorcentaje = inPorcentaje;
        }

        /// <summary>
        /// Cargar un registro por su llave.
        /// </summary>
        public bool ExistsKey(string psPais, string psCodigoDivGeo1, string psCodigoDivGeo2, string psCodigoRetencion, ref bool bParamExiste)
        {

            string consulta;
            SQLiteDataReader reader = null;
            try
            {


                consulta = " SELECT PAIS, DIVISION_GEOGRAFICA1, DIVISION_GEOGRAFICA2, CODIGO_RETENCION, PORCENTAJE, CENTRO_COSTO, CUENTA_CONTABLE" +
                               " FROM " + Table.ERPADMIN_EXCEP_CIUDAD +
                               " WHERE PAIS = @CODIGO_PAIS AND DIVISION_GEOGRAFICA1 = @CODIGO_DIV_GEO1 AND DIVISION_GEOGRAFICA2 = @CODIGO_DIV_GEO2 AND CODIGO_RETENCION = @CODIGO_RET";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@CODIGO_PAIS", SqlDbType.NVarChar, psPais),
                                                    GestorDatos.SQLiteParameter("@CODIGO_DIV_GEO1", SqlDbType.NVarChar, psCodigoDivGeo1),
                                                    GestorDatos.SQLiteParameter("@CODIGO_DIV_GEO2", SqlDbType.NVarChar, psCodigoDivGeo2),
                                                    GestorDatos.SQLiteParameter("@CODIGO_RET", SqlDbType.NVarChar, psCodigoRetencion)
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
        public void FillInstanceExt(string psCtrCtoExcepCiu, string psCtaCtbExcepCiu)
        {

            isCtrCtoExcepCiu = psCtrCtoExcepCiu;
            isCtaCtbExcepCiu = psCtaCtbExcepCiu;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetInstanceExt(ref string psCtrCtoExcepCiu, ref string psCtaCtbExcepCiu)
        {

            psCtrCtoExcepCiu = isCtrCtoExcepCiu;
            psCtaCtbExcepCiu = isCtaCtbExcepCiu;
        }

        #endregion Funciones

    }
}