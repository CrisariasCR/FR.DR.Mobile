using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    public class Lotes
    {
#pragma warning disable 414

        #region Constructor

        public Lotes()
        { 
        }

        #endregion Constructor

        #region Variables y Propiedades

        private string lote = string.Empty;
        /// <summary>
        /// Lote
        /// </summary>
        public string Lote
        {
            get { return lote; }
            set { lote = value; }
        }

        private string localizacion = string.Empty;
        /// <summary>
        /// Localización
        /// </summary>
        public string Localizacion
        {
            get { return localizacion; }
            set { localizacion = value; }
        }

        private DateTime fechavence;
        /// <summary>
        /// Fecha Vencimiento
        /// </summary>
        public DateTime FechaVencimiento
        {
            get { return fechavence; }
            set { fechavence = value; }
        }

        private decimal unidadEmpaque = decimal.Zero;
        /// <summary>
        /// Factor de unidad de empaque.
        /// </summary>
        public decimal UnidadEmpaque
        {
            get { return unidadEmpaque; }
            set { unidadEmpaque = value; }
        }

        private decimal existencia = decimal.Zero;
        /// <summary>
        /// Existencia Lote
        /// </summary>
        public decimal Existencia
        {
            get { 
                    return existencia; 
                }
            set {
                    if (UnidadEmpaque != 1)
                        GestorUtilitario.CalcularCantidadBonificada(value, UnidadEmpaque, ref existenciaAlmacen, ref existenciaDetalle);
                    existencia = value;
                }
        }

        private decimal existenciaParcial = decimal.Zero;
        /// <summary>
        /// Existencia parcial del lote
        /// </summary>
        public decimal ExistenciaParcial
        {
            get
            {
                return existencia - cantidad;
            }
            set
            {
                /*if (UnidadEmpaque != 1)
                    GestorUtilitario.CalcularCantidadBonificada(value, UnidadEmpaque, ref existenciaAlmacen, ref existenciaDetalle);*/
                existencia = value;
            }
        }

        private decimal existenciaAlmacen = decimal.Zero;
        /// <summary>
        /// Existencia Unidades de Almacen
        /// </summary>
        public decimal ExistenciaAlmacen
        {
            get { return existenciaAlmacen - cantidad; }
            set { existenciaAlmacen = value; }
        }

        private decimal existenciaDetalle = decimal.Zero;
        /// <summary>
        /// Existencia Unidades de Detalle
        /// </summary>
        public decimal ExistenciaDetalle
        {
            get { return existenciaDetalle - cantidadDetalle; }
            set { existenciaDetalle = value; }
        }

        private decimal cantidad = decimal.Zero;
        /// <summary>
        /// Cantidad en Unidades de Almacen
        /// </summary>
        public decimal Cantidad
        {
            get {
                    cantidad = GestorUtilitario.TotalAlmacen(this.CantidadAlmacen.ToString(), this.CantidadDetalle.ToString(), (int)this.unidadEmpaque);
                    return cantidad;
                }            
        }

        private decimal cantidadAlmacen = decimal.Zero;
        /// <summary>
        /// Cantidad Unidades de Almacen
        /// </summary>
        public decimal CantidadAlmacen
        {
            get { return cantidadAlmacen; }
            set { 
                    cantidadAlmacen = value;
                    cantidad = GestorUtilitario.TotalAlmacen(this.CantidadAlmacen.ToString(), this.CantidadDetalle.ToString(), (int)this.unidadEmpaque);
                }
        }

        private decimal cantidadDetalle = decimal.Zero;
        /// <summary>
        /// Cantidad Unidades de Detalle
        /// </summary>
        public decimal CantidadDetalle
        {
            get { return cantidadDetalle; }
            set { 
                    cantidadDetalle = value;
                    cantidad = GestorUtilitario.TotalAlmacen(this.CantidadAlmacen.ToString(), this.CantidadDetalle.ToString(), (int)this.unidadEmpaque);
                }
        }

        #endregion Variables y Propiedades

        #region Metodos Publicos

        public void GuardarLineaLote(string compania, string numeroPedido, int numLinea, string bodega, string articulo, string bonificada)
        {
            string sentencia =
                "INSERT INTO " + Table.ERPADMIN_LINEA_PED_LOTE_LOC +
                "        ( COMPANIA, BODEGA, NUM_PED, NUM_LN, ARTICULO, LOCALIZACION, LOTE, CNT_MAX, CNT_MIN, ART_BON ) " +
                " VALUES (@COMPANIA, @BODEGA, @NUM_PED, @NUM_LN, @ARTICULO, @LOCALIZACION, @LOTE, @CNT_MAX, @CNT_MIN, @ART_BON)";


            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar, compania),
                    GestorDatos.SQLiteParameter("@BODEGA",SqlDbType.NVarChar, bodega),
                    GestorDatos.SQLiteParameter("@NUM_PED",SqlDbType.NVarChar , numeroPedido),
                    GestorDatos.SQLiteParameter("@NUM_LN", SqlDbType.Int, numLinea),
                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, articulo),
                    GestorDatos.SQLiteParameter("@LOCALIZACION",SqlDbType.NVarChar, Localizacion),
                    GestorDatos.SQLiteParameter("@LOTE",SqlDbType.NVarChar, Lote),
                    GestorDatos.SQLiteParameter("@CNT_MAX",SqlDbType.Decimal, CantidadAlmacen),                                       
                    GestorDatos.SQLiteParameter("@CNT_MIN",SqlDbType.Decimal, CantidadDetalle),   
                    GestorDatos.SQLiteParameter("@ART_BON",SqlDbType.NVarChar, bonificada)});

            int linea = GestorDatos.EjecutarComando(sentencia, parametros);
            if (linea != 1)
                throw new Exception("No se puede guardar el lote: " + Lote + " para el artículo: " + articulo + " del pedido: '" + numeroPedido + "'.");
            else
                actualizarLote(compania, bodega, articulo, false);

        }

        public static List<Lotes> ConsultarLineaLote(string compania, string numeroPedido, string bodega, string articulo, decimal unidadEmpaque)
        {
            SQLiteDataReader reader = null;
            List<Lotes> detalle = new List<Lotes>();

            string sentencia =
                "SELECT LOCALIZACION, LOTE, CNT_MAX, CNT_MIN FROM " + Table.ERPADMIN_LINEA_PED_LOTE_LOC +
                " WHERE COMPANIA = '" + compania + "' AND BODEGA = '" + bodega + "' AND NUM_PED = '" + numeroPedido + "'" +
                " AND ARTICULO = '" + articulo + "' AND ART_BON = 'N'";

            /*SQLiteParameterList parametros = {
                    GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar, compania),
                    GestorDatos.SQLiteParameter("@BODEGA",SqlDbType.NVarChar, bodega),
                    GestorDatos.SQLiteParameter("@NUM_PED",SqlDbType.NVarChar , numeroPedido),
                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, articulo)};*/

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    Lotes lote = new Lotes();

                    lote.Localizacion = reader.GetString(0);
                    lote.Lote = reader.GetString(1);
                    lote.UnidadEmpaque = unidadEmpaque;
                    lote.CantidadAlmacen = reader.GetDecimal(2);
                    lote.CantidadDetalle = reader.GetDecimal(3);
                    detalle.Add(lote);
                }
                return detalle;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo los lotes asociados al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Actualiza las Existencias del Lote
        /// </summary>
        /// <param name="compania">Compañia</param>
        /// <param name="bodega">Bodega</param>
        /// <param name="articulo">Artículo</param>
        public void actualizarLote(string compania, string bodega, string articulo, bool suma)
        {
            string sentencia =
                "UPDATE " + Table.ERPADMIN_ARTICULO_EXISTENCIA_LOTE;

            if (suma)
                sentencia = sentencia + " SET EXISTENCIA = EXISTENCIA + " + Cantidad;
            else
                sentencia = sentencia + " SET EXISTENCIA = " + ExistenciaParcial.ToString();

            sentencia = sentencia + " WHERE COMPANIA = '" + compania.ToUpper() + "'" +
            " AND BODEGA = '" + bodega.ToUpper() + "'" +
            " AND ARTICULO = '" + articulo.ToUpper() + "'" +
            " AND LOTE = '" + Lote.ToUpper() + "'" +
            " AND LOCALIZACION = '" + Localizacion.ToUpper() + "'";


            /*SQLiteParameterList parametros = {
                    GestorDatos.SQLiteParameter("@EXISTENCIA",SqlDbType.Decimal, ExistenciaParcial),
                    GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar, compania),
                    GestorDatos.SQLiteParameter("@BODEGA",SqlDbType.NVarChar, bodega),
                    GestorDatos.SQLiteParameter("@ARTICULO", SqlDbType.NVarChar, articulo),
                    GestorDatos.SQLiteParameter("@LOCALIZACION",SqlDbType.NVarChar, Lote),
                    GestorDatos.SQLiteParameter("@LOTE",SqlDbType.NVarChar, Localizacion)};*/

            int linea = GestorDatos.EjecutarComando(sentencia);
            if (linea != 1)
                throw new Exception("No se puede actualizar existencias para el lote " + Lote + " del artículo " + articulo + ".");
        }

        #endregion Metodos Publicos

    }
}
