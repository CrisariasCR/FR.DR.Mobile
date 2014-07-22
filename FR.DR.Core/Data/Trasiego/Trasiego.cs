using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using System.Data;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FR.DR.Core.Data.Trasiego
{
    public class Trasiego
    {
        #region Constantes
        /// <summary>
        /// Boleta Activa
        /// </summary>
        public const string ACTIVO = "A";
        /// <summary>
        /// Boleta Anulada
        /// </summary>
        public const string PENDIENTE = "P";
        
        #endregion Constantes

        #region Propiedades

        private int consecutivo;
        public int Consecutivo 
        {
            get { return consecutivo; }
            set { consecutivo = value; }
        }
        private string compania;
        public string Compania 
        {
            get { return compania; }
            set { compania = value; }
        }
        private string handheld;
        public string HandHeld 
        {
            get { return handheld; }
            set { handheld = value; }
        }
        private DateTime fecha;
        public DateTime Fecha 
        {
            get { return fecha; }
            set { fecha = value; }
        }
        private string bodega;
        public string Bodega 
        {
            get { return bodega; }
            set { bodega=value;}
        }
        private string localizacion;
        public string Localizacion 
        {
            get { return localizacion; }
            set { localizacion = value; }
        }
        private string articulo;
        public string Articulo 
        {
            get { return articulo; }
            set { articulo = value; }
        }
        private string lote;
        public string Lote
        {
            get { return lote; }
            set { lote = value; }
        }
        private decimal cantAlmacen;
        public decimal CantAlmacen
        {
            get { return cantAlmacen; }
            set { cantAlmacen = value; }
        }
        private decimal cantDetalle;
        public decimal CantDetalle 
        {
            get { return cantDetalle; }
            set { cantDetalle = value; }
        }

        private decimal cantDiferencia;
        public decimal CantDiferencia
        {
            get { return cantDiferencia; }
            set { cantDiferencia = value; }
        }

        private TipoTrasiego tipoTras;
        public TipoTrasiego TipoTras 
        {
            get { return tipoTras; }
            set { tipoTras = value; }
        }
        private ClaseTrasiego clasTras;
        public ClaseTrasiego ClasTras 
        {
            get { return clasTras; }
            set { clasTras = value; }
        }

        #endregion

        public Trasiego() 
        {
        }

        public Trasiego(string pCompania, string pHandHeld, string pBodega, string pLocalizacion, DateTime pFecha) 
        {
            Compania = pCompania;
            HandHeld = pHandHeld;
            Fecha = pFecha;
            Bodega = pBodega;
            Localizacion = pLocalizacion;
        }

        public static bool CantidadExedida(string lsArticulo,string lsBodega,decimal ldCantidad) 
        {
            #region Definición de variables
            bool procesoExitoso = true;
            bool exedida = false;
            string insertBoleta = string.Empty;
            decimal cant = 0;
            #endregion Definición de variables

            try
            {

                if (procesoExitoso)
                {
                    //Se crea la sentencia para insertar los datos
                    insertBoleta = " SELECT  EXISTENCIA  FROM "+ Table.ERPADMIN_ARTICULO_EXISTENCIA;
                    insertBoleta = insertBoleta + " WHERE ARTICULO=@ARTICULO AND BODEGA=@BODEGA ";                    

                    //Se crean los parámetros
                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, lsArticulo),
                                                    GestorDatos.SQLiteParameter("@BODEGA", SqlDbType.NVarChar, lsBodega)
                                                  });

                    //Se ejecuta la sentencia
                    object obj=GestorDatos.EjecutarScalar(insertBoleta, parametros);
                    cant = Convert.ToDecimal(obj);
                    exedida = ldCantidad > cant;
                }
            }
            catch (Exception ex)
            {
                procesoExitoso = false;
                throw ex;
            }


            return exedida;


        }

        #region métodos

        public bool RegistrarNuevaBoleta()
        {
            #region Definición de variables
            bool procesoExitoso = true;
            string insertBoleta = string.Empty;
            #endregion Definición de variables

            try
            {
                //Se obtiene el siguiente consecutivo
                procesoExitoso = ObtenerSigteConsecBoleta();

                if (procesoExitoso)
                {
                    //Se crea la sentencia para insertar los datos
                    insertBoleta = String.Format(" INSERT INTO {0} ( ", Table.ERPADMIN_TRASIEGO);
                    insertBoleta = insertBoleta + " CONSECUTIVO, COMPANIA, HANDHELD, FECHA, ARTICULO, BODEGA, LOCALIZACION, LOTE, TIPO, CLASE, CANT_DIF, ESTADO )";
                    insertBoleta = insertBoleta + " VALUES ( @CONSECUTIVO, @COMPANIA, @HANDHELD, @FECHA, @ARTICULO, @BODEGA, @LOCALIZACION, @LOTE, @TIPO, @CLASE, @CANTDIF,@ESTADO )";

                    //Se crean los parámetros
                    SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[]{
                                                    GestorDatos.SQLiteParameter("@CONSECUTIVO", SqlDbType.Int, Consecutivo),
                                                    GestorDatos.SQLiteParameter("@COMPANIA", SqlDbType.NVarChar, Compania),
                                                    GestorDatos.SQLiteParameter("@HANDHELD", SqlDbType.NVarChar, HandHeld),
                                                    GestorDatos.SQLiteParameter("@FECHA", SqlDbType.DateTime, Fecha.ToString("yyyy-MM-dd")), 
                                                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, Articulo),
                                                    GestorDatos.SQLiteParameter("@BODEGA", SqlDbType.NVarChar, Bodega),
                                                    GestorDatos.SQLiteParameter("@LOCALIZACION", SqlDbType.NVarChar, Localizacion),
                                                    GestorDatos.SQLiteParameter("@TIPO", SqlDbType.NVarChar, ((char)TipoTras).ToString()),
                                                    GestorDatos.SQLiteParameter("@CLASE", SqlDbType.NVarChar,((char)ClasTras).ToString()),
                                                    GestorDatos.SQLiteParameter("@LOTE", SqlDbType.NVarChar, Lote),
                                                    GestorDatos.SQLiteParameter("@CANTDIF", SqlDbType.Decimal, CantDiferencia),
                                                    GestorDatos.SQLiteParameter("@ESTADO", SqlDbType.NVarChar, ACTIVO)
                                                  });

                    //Se ejecuta la sentencia
                    GestorDatos.EjecutarComando(insertBoleta, parametros);
                }
            }
            catch(Exception ex)
            {
                procesoExitoso = false;
                throw ex;
            }


            return procesoExitoso;
        }

        public bool ObtenerSigteConsecBoleta()
        {
            #region Definición de Varaibles
            bool procesoExitoso = true;
            string consulta = string.Empty;
            int count = 0;
            SQLiteDataReader reader = null;
            #endregion Definición de Varaibles

            try
            {
                //Se crea la consulta para obtener los datos de la boleta de inventario fisico
                consulta = " SELECT MAX(consecutivo) " +
                           " FROM " + Table.ERPADMIN_TRASIEGO;

                //Se ejecuta la consulta en la base de datos
                reader = GestorDatos.EjecutarConsulta(consulta);

                //Se obtienen los datos
                if (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetValue(0).ToString()))
                    {
                        count = Convert.ToInt16(reader.GetValue(0));
                    }
                    else
                    {
                        count = 0;
                    }
                }
                else
                {
                    count = 0;
                }

                Consecutivo = count + 1;
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

        public static bool HayPendientesCarga()
        {
            string sentencia = "SELECT COUNT(0) FROM " + Table.ERPADMIN_TRASIEGO;
            return (GestorDatos.NumeroRegistros(sentencia) != 0);
        }


        public bool ActualizarExistencias()
        {
            bool result = true;
            try
            {
                string sentencia = string.Empty;
                string sentenciaLote = string.Empty;
                if (TipoTras.Equals(TipoTrasiego.Entrada))
                {
                    sentencia = "UPDATE " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                    " set EXISTENCIA=(EXISTENCIA+@existencia) where ARTICULO=@articulo";
                }
                else
                {
                    sentencia = "UPDATE " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                    " set EXISTENCIA=(EXISTENCIA-@existencia) where ARTICULO=@articulo";
                }
                if (this.Lote!="ND" && !string.IsNullOrEmpty(Lote))
                {
                    if (TipoTras.Equals(TipoTrasiego.Entrada))
                    {
                        sentenciaLote = "UPDATE " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                    " set EXISTENCIA=(EXISTENCIA+@existencia) where ARTICULO=@articulo";
                    }
                    else
                    {
                        sentenciaLote = "UPDATE " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                    " set EXISTENCIA=(EXISTENCIA-@existencia) where ARTICULO=@articulo";
                    }
                }
                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@existencia",SqlDbType.Decimal,(decimal)CantDiferencia),                
                    GestorDatos.SQLiteParameter("@articulo",SqlDbType.NVarChar, this.Articulo)});

                GestorDatos.EjecutarComando(sentencia, parametros);

                if (this.Lote != "ND" && !string.IsNullOrEmpty(Lote))
                {
                    GestorDatos.EjecutarComando(sentenciaLote, parametros);
                }

            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;
        }

        #endregion

    }

    public enum TipoTrasiego
    {
        Entrada = 'E',
        Salida = 'S'        
    }

    public enum ClaseTrasiego
    {
        Vacios = 'V',
        Llenos = 'L'
    }
}