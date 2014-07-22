using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    public class DescuentoProntoPago
    {
        #region Atributos
        private string condicionPago;
        private string compania;
        private int consecutivo;
        private int diaInicio;
        private int diaFin;
        private decimal descuento;
        #endregion

        #region Propiedades
        /// <summary>
        /// Codigo de la condicin de pago asociada.
        /// </summary>
        public string CondicionPago
        {
            get
            {
                return condicionPago;
            }
            set
            {
                condicionPago = value;
            }
        }
        /// <summary>
        /// Codigo de la compania asociada
        /// </summary>
        public string Compania
        {
            get
            {
                return compania;
            }
            set
            {
                compania = value;
            }
        }
        /// <summary>
        /// Numero de consecutivo del desucento.
        /// </summary>
        public int Consecutivo
        {
            get
            {
                return consecutivo;
            }
            set
            {
                consecutivo = value;
            }
        }
        /// <summary>
        /// Dia de inicio del descucento.
        /// </summary>
        public int DiaInicio
        {
            get
            {
                return diaInicio;
            }
            set
            {
                diaInicio = value;
            }
        }
        /// <summary>
        /// Dia fin del descuento.
        /// </summary>
        public int DiaFin
        {
            get
            {
                return diaFin;
            }
            set
            {
                diaFin = value;
            }
        }
        /// <summary>
        /// Procentage de descuento.
        /// </summary>
        public decimal Descuento
        {
            get
            {
                return descuento;
            }
            set
            {
                descuento = value;
            }
        }
        #endregion

        #region Constructores
        public DescuentoProntoPago()
        {
        }
        public DescuentoProntoPago(string compania, string condicionPago, int consecutivo, int diaInicio, int diaFin, decimal descuento)
        {
            this.compania = compania;
            this.condicionPago = condicionPago;
            this.consecutivo = consecutivo;
            this.diaInicio = diaInicio;
            this.diaFin = diaFin;
            this.descuento = descuento;
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Obtiene los descuentos por pronto pago asociados a la compañia
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        public static List<DescuentoProntoPago> ObtenerDescuentosProntoPago(string compania, string condicion)
        {
            List<DescuentoProntoPago> descuentos = new List<DescuentoProntoPago>();
            descuentos.Clear();
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT COD_CND, CONSECUTIVO, DIA_INICIO, DIA_FIN, DESCUENTO ");
            sentencia.AppendLine(String.Format(" FROM {0}", Table.ERPADMIN_alFAC_DESC_PRONTO_PAGO));
            sentencia.AppendLine(" WHERE UPPER(COD_CIA) = @COMPANIA");
            if(!String.IsNullOrEmpty(condicion)){
                sentencia.AppendLine(" AND COD_CND = @CONDICION");
                parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper())
                    , new SQLiteParameter("@CONDICION", condicion) });
            }else{
                parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()) });
            }
            sentencia.AppendLine(" ORDER BY DES_CND ");

            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                while (reader.Read())
                {
                    DescuentoProntoPago descuento = new DescuentoProntoPago();

                     descuento.Compania = compania;
                     descuento.CondicionPago = reader.GetString(0);
                     descuento.Consecutivo = reader.GetInt32(1);
                     descuento.DiaInicio = reader.GetInt32(2);
                     descuento.DiaFin = reader.GetInt32(3);
                     descuento.Descuento = reader.GetDecimal(4);

                     descuentos.Add(descuento);
                }

                return descuentos;
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede obtener los descuentos por pronto pago para la compañía '" + compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }
        /// <summary>
        /// Obtiene el descuento por pronto pago asociado a la compania, la condicion de pago y la cantidad
        /// de dias dado entre diaInicio y diaFin.
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="condicion"></param>
        /// <param name="dias"></param>
        /// <returns></returns>
        public static DescuentoProntoPago ObtenerDescuentoProntoPago(string compania, string condicion, int dias)
        {
            DescuentoProntoPago descuento = null;
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT COD_CND, CONSECUTIVO, DIA_INICIO, DIA_FIN, DESCUENTO ");
            sentencia.AppendLine(String.Format(" FROM {0}", Table.ERPADMIN_alFAC_DESC_PRONTO_PAGO));
            sentencia.AppendLine(" WHERE UPPER(COD_CIA) = @COMPANIA");
            sentencia.AppendLine(" AND COD_CND = @CONDICION");
            sentencia.AppendLine(" AND @DIAS BETWEEN DIA_INICIO AND DIA_FIN ");

            SQLiteDataReader reader = null;
            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper())
                    , new SQLiteParameter("@CONDICION", condicion)
                    , new SQLiteParameter("@DIAS", dias)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                if(reader.Read())
                {
                    descuento = new DescuentoProntoPago();
                    descuento.Compania = compania;
                    descuento.CondicionPago = reader.GetString(0);
                    descuento.Consecutivo = reader.GetInt32(1);
                    descuento.DiaInicio = reader.GetInt32(2);
                    descuento.DiaFin = reader.GetInt32(3);
                    descuento.Descuento = reader.GetDecimal(4);
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede obtener el descuento por pronto pago para la compañía '" + compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return descuento;

        }
        #endregion
    }
}
