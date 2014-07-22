using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Text;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    /// <summary>
    /// Nivel de precio asociado a una venta
    /// </summary>
    public class NivelPrecio
    {

        #region Variables y Propiedades de instancia

        private string compania = string.Empty;
        /// <summary>
        /// Codigo de la compania a la que pertenece la lista de precio.
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); }
        }
        private int codigo = 0;
        /// <summary>
        /// Codigo de la lista de precio .
        /// </summary>
        public int Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string nivel = string.Empty;
        /// <summary>
        /// Es el codigo del nivel de precios que se maneja en Exactus.
        /// </summary>
        public string Nivel
        {
            get { return nivel; }
            set { nivel = value; }
        }

        public override string ToString()
        {
            return Nivel;
        }

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Tipo de moneda del nivel de precios que se maneja en Exactus.
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private Impuesto1Incluido impuesto1Incluido = Impuesto1Incluido.NO;
        /// <summary>
        /// Indica si los precios incluyen el impuesto 1
        /// </summary>
        public Impuesto1Incluido Impuesto1Incluido
        {
            get { return impuesto1Incluido; }
            set { impuesto1Incluido = value; }
        }


        #endregion

        public NivelPrecio(string compania, int codigo)
        {
            this.codigo = codigo;
            this.Compania = compania;
        }
        public NivelPrecio()
        {
        }

        /// <summary>
        /// Obtener los distintos niveles de precio para una compania
        /// </summary>
        /// <param name="compania">compania seleccionada</param>
        /// <returns>lista de los niveles de precio asociados</returns>
        public static List<NivelPrecio> ObtenerNivelesPrecio(string compania)
        {
            return ObtenerNivelesPrecio(compania, null);
        }

        /// <summary>
        /// Obtener los distintos niveles de precio para una compania
        /// </summary>
        /// <param name="compania">compania seleccionada</param>
        /// <param name="moneda">filtro de moneda</param>
        /// <returns>lista de los niveles de precio asociados</returns>
        public static List<NivelPrecio> ObtenerNivelesPrecio(string compania, TipoMoneda? moneda)
        {
            List<NivelPrecio> niveles = new List<NivelPrecio>();
            niveles.Clear();

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@COMPANIA", compania.ToUpper());

            // LAS. CASO # CR1-06885-D3FR
            // Se incluye un filtro de moneda.

            StringBuilder sentencia = new StringBuilder();
            sentencia.AppendLine(String.Format(" SELECT LISTA ,NIVEL_PRECIO,MONEDA,IMPUESTO1_INCLUIDO FROM {0} ", Table.ERPADMIN_NIVEL_LISTA));
            sentencia.AppendLine(" WHERE UPPER(COMPANIA) = @COMPANIA ");
            if (moneda != null)
            {
                sentencia.AppendLine(" AND MONEDA = @MONEDA");
                parametros.Add("@MONEDA", moneda == TipoMoneda.LOCAL ? "L" : "D");
            }

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia.ToString(), parametros);

                while (reader.Read())
                {
                    NivelPrecio nivel = new NivelPrecio();
                    nivel.Compania = compania;
                    nivel.codigo = reader.GetInt32(0);
                    nivel.nivel = reader.GetString(1);
                    nivel.moneda = (TipoMoneda)Convert.ToChar(reader.GetString(2));
                    nivel.impuesto1Incluido = (Impuesto1Incluido)Convert.ToChar(reader.GetString(3));

                    niveles.Add(nivel);
                }
                return niveles;
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo cargar niveles de precios para la compañía '" + compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        /// <summary>
        /// Obtiene nivel precio de un cliente de una compania.
        /// </summary>
        /// <param name="compania">compania seleccionada</param>/// 
        /// <param name="cliente">cliente seleccionado</param>
        public void ObtenerNivelPrecio(string compania, string cliente)
        {
            string sentencia =
                 " SELECT N.LISTA ,N.NIVEL_PRECIO,N.MONEDA " +
                @" FROM " + Table.ERPADMIN_NIVEL_LISTA + " N, " + Table.ERPADMIN_CLIENTE_CIA + " C " +
                @" WHERE UPPER(C.COD_CIA) = @COMPANIA " +
                @" AND C.COD_CLT = @CLIENTE " +
                @" AND C.LST_PRE = N.LISTA " +
                @" AND C.COD_CIA = N.COMPANIA ";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@COMPANIA", compania.ToUpper());
            parametros.Add("@CLIENTE", cliente);

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    this.Compania = compania;
                    codigo = reader.GetInt32(0); ;
                    nivel = reader.GetString(1);
                    moneda = (TipoMoneda)Convert.ToChar(reader.GetString(2));
                }
                else
                    throw new Exception("No se puede obtener nivel de precio del cliente");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Cargar el nivel de precio en la compania
        /// </summary>
        /// <param name="cia">compania a la cual se asocia el nivel de precio</param>
        /// <returns>carga exitosa</returns>
        public bool CargarNivelPrecio(string cia)
        {
            string sentencia =
                " SELECT NIVEL_PRECIO,MONEDA,IMPUESTO1_INCLUIDO FROM " + Table.ERPADMIN_NIVEL_LISTA +
                @" WHERE UPPER(COMPANIA) = @COMPANIA" +
                @" AND LISTA = @NIVEL";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@COMPANIA", cia.ToUpper());
            parametros.Add("@NIVEL", codigo);
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    compania = cia;
                    nivel = reader.GetString(0);
                    moneda = (TipoMoneda)Convert.ToChar(reader.GetString(1));
                    impuesto1Incluido = (Impuesto1Incluido)Convert.ToChar(reader.GetString(2));
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo cargar el nivel de precios '" + this.Codigo + "' para la compañía '" + this.Compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        public static TipoMoneda monedaLista(string compania, int codigo)
        {
            List<NivelPrecio> listas = ObtenerNivelesPrecio(compania);
            foreach (NivelPrecio lista in listas)
            {
                if (lista.Codigo == codigo)
                    return lista.Moneda;
            }
            return TipoMoneda.LOCAL;
        }
    }
}
