using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class Regla
    {
        #region Attributes
        private String compania;
        private String codigo;
        private String descripcion;

        private String tipo;
        private String tipoDescuento;
        private String tipoValor;
        private String tipoBonificacion;

        private bool activa;
        private bool validarCantidad;
        private bool utilizarArticuloLinea;
        private bool aplicacionGrupos;

        private String filtroCliente;
        private String filtroArticulo;
        private String filtroArticuloBonificacion;


        private decimal cantidadMinima;
        private decimal cantidadMaxima;
        private decimal valor;

        // kfc - motor de precios 7 >
        private int prioridad;
        private bool requiereAutorizacion;
        private decimal montoMinimo;
        private string categoriaMembresia;
        private string codigoFormaPago;
        private string detalleFormaPago;
        private decimal minimoPago;
        private decimal cantidadMinDetalle;
        private decimal cantidadMaxDetalle;
        //< kfc        


        private List<Escala> escalas;
        #endregion

        #region Properties
        public String Compania
        {
            get { return compania; }
            set { compania = value; }
        }
        public String Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public String Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        public String Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        public String TipoDescuento
        {
            get { return tipoDescuento; }
            set { tipoDescuento = value; }
        }
        public String TipoValor
        {
            get { return tipoValor; }
            set { tipoValor = value; }
        }
        public String TipoBonificacion
        {
            get { return tipoBonificacion; }
            set { tipoBonificacion = value; }
        }
        public bool Activa
        {
            get { return activa; }
            set { activa = value; }
        }
        public bool ValidarCantidad
        {
            get { return validarCantidad; }
            set { validarCantidad = value; }
        }

        public bool UtilizarArticuloLinea
        {
            get { return utilizarArticuloLinea; }
            set { utilizarArticuloLinea = value; }
        }
        public bool AplicacionGrupos
        {
            get { return aplicacionGrupos; }
            set { aplicacionGrupos = value; }
        }
        public String FiltroCliente
        {
            get { return filtroCliente; }
            set { filtroCliente = value; }
        }

        public String FiltroArticulo
        {
            get { return filtroArticulo; }
            set { filtroArticulo = value; }
        }
        public String FiltroArticuloBonificacion
        {
            get { return filtroArticuloBonificacion; }
            set { filtroArticuloBonificacion = value; }
        }
        public decimal CantidadMinima
        {
            get { return cantidadMinima; }
            set { cantidadMinima = value; }
        }
        public decimal CantidadMaxima
        {
            get { return cantidadMaxima; }
            set { cantidadMaxima = value; }
        }
        public decimal Valor
        {
            get { return valor; }
            set { valor = value; }
        }
        public List<Escala> Escalas
        {
            get { return escalas; }
            set { escalas = value; }
        }

        #region KFC - MP 7

        public bool RequiereAutorizacion
        {
            get { return requiereAutorizacion; }
            set { requiereAutorizacion = value; }
        }

        public int Prioridad
        {
            get { return prioridad; }
            set { prioridad = value; }
        }

        public decimal MontoMinimo
        {
            get { return montoMinimo; }
            set { montoMinimo = value; }
        }

        public string FormaPago
        {
            get { return codigoFormaPago; }
            set { codigoFormaPago = value; }
        }

        public string DetalleFormaPago
        {
            get { return detalleFormaPago; }
            set { detalleFormaPago = value; }
        }

        public decimal MinimoPago
        {
            get { return minimoPago; }
            set { minimoPago = value; }
        }

        public string CategoriaMembresia
        {
            get { return categoriaMembresia; }
            set { categoriaMembresia = value; }
        }

        public decimal CantidadMinimaDetalle
        {
            get { return cantidadMinDetalle; }
            set { cantidadMinDetalle = value; }
        }

        public decimal CantidadMaximaDetalle
        {
            get { return cantidadMaxDetalle; }
            set { cantidadMaxDetalle = value; }
        }

        #endregion


        #endregion

        #region Constructors

        #endregion

        #region Methods

        public static List<Regla> FindAll(String compania, String paquete)
        {
            List<Regla> resultado = new List<Regla>();
            SQLiteParameterList parametros;
            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(" SELECT reg.COMPANIA,reg.REGLA, DESCRIPCION, TIPO, ACTIVA, FILTRO_CLIENTE, VALIDAR_CANTIDAD, CANTIDAD_MINIMA ");
            sentencia.AppendLine(" , CANTIDAD_MAXIMA, FILTRO_ARTICULO, TIPO_DESCUENTO, TIPO_VALOR, VALOR, TIPO_BONIFICACION, UTILIZAR_ARTICULO_LINEA, FILTRO_ARTICULO_BONIFICACION, APLICACION_GRUPOS");
            // kfc -mp
            sentencia.AppendLine(" , PRIORIDAD, MONTO_MINIMO, FORMA_PAGO, DETALLE_FORMA_PAGO, MINIMO_PAGO, REQUIERE_AUTORIZACION, CANTIDAD_MIN_DETALLE, CANTIDAD_MAX_DETALLE");

            sentencia.AppendLine(String.Format(" FROM {0} reg", Table.ERPADMIN_DES_BON_REGLA));
            sentencia.AppendLine(String.Format(" INNER JOIN {0} pr ON pr.REGLA = reg.REGLA AND pr.COMPANIA = reg.COMPANIA ", Table.ERPADMIN_DES_BON_PAQUETE_REGLA));
            sentencia.AppendLine(" WHERE UPPER(reg.COMPANIA) = @COMPANIA");
            sentencia.AppendLine(" AND pr.PAQUETE = @PAQUETE");
            sentencia.AppendLine(" AND reg.ACTIVA = 'S' ");
            sentencia.AppendLine(" ORDER BY reg.REGLA asc ");
            parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()), new SQLiteParameter("@PAQUETE", paquete) });
            
            SQLiteDataReader reader = null;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                while (reader.Read())
                {
                    Regla regla = new Regla();
                    regla.Compania = reader.GetString(0);
                    regla.Codigo = reader.GetString(1);
                    regla.Descripcion = reader.GetString(2);
                    regla.Tipo = reader.GetString(3);
                    regla.Activa = reader.GetString(4) == "S";
                    regla.FiltroCliente = reader.IsDBNull(5) ? null : reader.GetString(5);
                    regla.ValidarCantidad = reader.GetString(6) == "S";
                    regla.CantidadMinima = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7);
                    regla.CantidadMaxima = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8);
                    regla.FiltroArticulo = reader.IsDBNull(9) ? null : reader.GetString(9);
                    regla.TipoDescuento = reader.GetString(10);
                    regla.TipoValor = reader.GetString(11);
                    regla.Valor = reader.IsDBNull(12) ? 0  : reader.GetDecimal(12);
                    regla.TipoBonificacion = reader.GetString(13);
                    regla.UtilizarArticuloLinea = reader.GetString(14) == "S";
                    regla.FiltroArticuloBonificacion = reader.IsDBNull(15)? null : reader.GetString(15);
                    regla.AplicacionGrupos = reader.GetString(16) == "S";

                    // kfc - mp 7 >
                    regla.prioridad = reader.IsDBNull(17) ? 100000 : reader.GetInt32(17);
                    regla.montoMinimo = reader.GetDecimal(18);
                    regla.codigoFormaPago = reader.IsDBNull(19) ? string.Empty : reader.GetString(19);
                    regla.detalleFormaPago = reader.IsDBNull(20) ? string.Empty : reader.GetString(20);
                    regla.minimoPago = reader.IsDBNull(21) ? 100000 : reader.GetDecimal(21);
                    regla.requiereAutorizacion = reader.GetString(22) == "S";
                    regla.cantidadMinDetalle = reader.IsDBNull(23) ? 0 : reader.GetDecimal(23);
                    regla.cantidadMaxDetalle = reader.IsDBNull(24) ? 0 : reader.GetDecimal(24);

                    //< kfc mp 7                    

                    resultado.Add(regla);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error cargando los paquetes de descuentos-bonificación para la compañía '{0}'. {1}  '", compania, ex.Message));
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return resultado;
        }

        /// <summary>
        /// Carga las escalas asociadas a la regla
        /// </summary>
        public void CargarEscalas()
        {
            this.escalas = new List<Escala>();

            this.escalas = Escala.FindAll(this.compania, this.codigo);
        }

        #endregion
    }
}
