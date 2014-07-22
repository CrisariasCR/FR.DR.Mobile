using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    public enum ModoDescuentoMultipleEnum
    {
        PrimeroSegunPrioridad,
        PorcentajeMayor, AcumularTodos, AcumularCascada
    }

    /// <summary>
    /// Informacion detallada de una compania
    /// </summary>
    public class Compania : CompaniaBase
    {
        #region Variables y Propiedades de instancia

        private static List<Compania> gestionadas = new List<Compania>();
        /// <summary>
        /// Companias gestionadas
        /// </summary>
        public static List<Compania> Gestionadas
        {
            get
            {
                if (Compania.Gestionadas.Count == 0)
                {
                    Compania.Gestionadas.Clear();
                    Compania.Gestionadas = Compania.Obtener();
                }
                return Compania.gestionadas; 
            }
            set { Compania.gestionadas = value; }
        }

        private string codigo = string.Empty;
        /// <summary>
        /// Codigo compania
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value.ToUpper(); }
        }

        private string paisInstalado = string.Empty;
        /// <summary>
        /// Codigo compania
        /// </summary>
        public string PaisInstalado
        {
            get { return paisInstalado; }
            set { paisInstalado = value.ToUpper(); }
        }

        private bool retenciones;
        /// <summary>
        /// Usa Retenciones
        /// </summary>
        public bool Retenciones
        {
            get { return retenciones; }
            set { retenciones = value; }
        }


        private FormaCalcImpuesto1 impuesto1AfectaDescto = FormaCalcImpuesto1.Ninguno;
        /// <summary>
        /// Global de FA que indica la manera en que se calcula el impuesto de venta.
        /// </summary>
        public FormaCalcImpuesto1 Impuesto1AfectaDescto
        {
            get { return impuesto1AfectaDescto; }
            set { impuesto1AfectaDescto = value; }
        }

        private string condicionPagoContado = string.Empty;
        /// <summary>
        /// Global de FA que indica la condición de pago de contado utilizada.
        /// </summary>
        public string CondicionPagoContado
        {
            get { return condicionPagoContado; }
            set { condicionPagoContado = value; }
        }

        private decimal tipoCambio = 0;
        /// <summary>
        /// Tipo de cambio del dolar que utiliza la compania.
        /// </summary>
        public decimal TipoCambio
        {
            get { return tipoCambio; }
            set { tipoCambio = value; }
        }

        private bool descuentoCascada = false;
        /// <summary>
        /// Global de FA que indica si los descuentos 1 y 2 son en cascada.
        /// </summary>
        public bool DescuentoCascada
        {
            get { return descuentoCascada; }
            set { descuentoCascada = value; }
        }

        private string impuesto1_desc = string.Empty;
        /// <summary>
        /// Descripción del Impuesto 1.
        /// </summary>
        public string Impuesto1Descripcion
        {
            get { return impuesto1_desc; }
            set { impuesto1_desc = value; }
        }

        private string impuesto2_desc = string.Empty;
        /// <summary>
        /// Descripción del Impuesto 2.
        /// </summary>
        public string Impuesto2Descripcion
        {
            get { return impuesto2_desc; }
            set { impuesto2_desc = value; }
        }

        private bool utilizaMinExcento = false;
        /// <summary>
        /// Si utiliza minimo excento de impuesto.
        /// </summary>
        public bool UtilizaMinimoExento
        {
            get { return utilizaMinExcento; }
            set { utilizaMinExcento = value; }
        }

        private decimal montoMinExcento = decimal.Zero;
        /// <summary>
        /// Monto minimo excento de impuesto.
        /// </summary>
        public decimal MontoMinimoExcento
        {
            get { return montoMinExcento; }
            set { montoMinExcento = value; }
        }

        private bool usaNCF = false;
       
        public bool UsaNCF
        {
            get { return usaNCF; }
            set { usaNCF = value; }
        }


        #region  Facturas de contado y recibos en FR - KFC

        private string condicionPagoDevoluciones = string.Empty;
        /// <summary>
        /// Condición de pago por defecto definida para devoluciones.
        /// </summary>
        public string CondicionPagoDevoluciones
        {
            get { return condicionPagoDevoluciones; }
            set { condicionPagoDevoluciones = value; }
        }

        #endregion

        #region  Cambios Motor de Precios 7.0 - KFC

        /// <summary>
        /// Condición de pago por defecto definida para devoluciones.
        /// </summary>
        public ModoDescuentoMultipleEnum ModoDescuentosMultiples
        {
            get;
            set;
        }

        #endregion


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor Compania
        /// </summary>
        public Compania()
        {
        }

        /// <summary>
        /// Constructor de compania
        /// </summary>
        /// <param name="codigo">codigo de la compania</param>
        public Compania(string codigo)
        {
            this.codigo = codigo;
        }
        #endregion

        /// <summary>
        /// Obtener una compania de las gestionadas
        /// </summary>
        /// <param name="cia">codigo de compania a obtener</param>
        /// <returns>compania buscada</returns>
        public static Compania Obtener(string cia)
        {
            if (gestionadas.Count == 0)
            {
                gestionadas.Clear();
                gestionadas = Obtener();
            }
            foreach (Compania compania in gestionadas)
                if (cia.Equals(compania.Codigo))
                    return compania;
            return null;
        }

        #region Acceso Datos

        /// <summary>
        /// Cargar los datos de la compania
        /// </summary>
        public void Cargar()
        {
            string sentencia =
                @" SELECT NOM_CIA,DI1_CIA,TEL_CIA,SLG_CIA,NIT_CIA, " +
                @" IMP1_AFECTA_DESCTO, COND_PAGO_CONTAD, CAMBIODOLAR, DESCUENTO_CASCADA, USA_NCF, COND_PAGO_DEV, MODO_DESC_MULT,PAIS,RETENCIONES " +
                @" FROM " + Table.ERPADMIN_COMPANIA  +
                @" WHERE UPPER(COD_CIA) = @COMPANIA";
            
            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@COMPANIA", codigo.ToUpper());

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                if (reader.Read())
                {
                    Nombre = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        Direccion = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        Telefono = reader.GetString(2);
                    if (!reader.IsDBNull(3))
                        Slogan = reader.GetString(3);
                    if (!reader.IsDBNull(4))
                        Nit = reader.GetString(4);
                    
                    impuesto1AfectaDescto= Impuesto.FormaCalculo(reader.GetString(5));
                    condicionPagoContado = reader.GetString(6);
                    tipoCambio = reader.GetDecimal(7);
                    descuentoCascada = reader.GetString(8).Equals("S");
                    //ABC Manejo NCF
                    if (!reader.IsDBNull(9))
                        usaNCF = reader.GetString(9).Equals("S");

                    //KFC- Recibos de contado
                    if (!reader.IsDBNull(10))
                        condicionPagoDevoluciones = reader.GetString(10);

                    //KFC- Cambios Motor de precios
                    if (!reader.IsDBNull(11))
                    {
                        switch (reader.GetString(11))
                        {
                            case "M":
                                ModoDescuentosMultiples = ModoDescuentoMultipleEnum.PorcentajeMayor;
                                break;
                            case "P":
                                ModoDescuentosMultiples = ModoDescuentoMultipleEnum.PrimeroSegunPrioridad;
                                break;
                            case "A":
                                ModoDescuentosMultiples = ModoDescuentoMultipleEnum.AcumularTodos;
                                break;
                            case "C":
                                ModoDescuentosMultiples = ModoDescuentoMultipleEnum.AcumularCascada;
                                break;
                        }
                    }

                    //Retenciones
                    if (!reader.IsDBNull(12))
                        PaisInstalado = reader.GetString(12);
                    if (!reader.IsDBNull(13))
                        Retenciones = reader.GetString(13).Equals("S");

                }
                else
                    throw new Exception("No se obtuvo información de la compañía '" + codigo + "'.");

            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo información de la compañía '" + codigo + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        
        }

        /// <summary>
        /// Cargar los datos de la compania
        /// </summary>
        public void CargarConfiguracionImpuestos()
        {
            string sentencia =
                @" SELECT NOMBRE,VALOR " +                  
                @" FROM " + Table.ERPADMIN_GLOBALES_MODULO +
                @" WHERE UPPER(MODULO) = @COMPANIA";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@COMPANIA", codigo.ToUpper());

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                //if (reader.FieldCount > 0)
                //{
                    while (reader.Read())
                    {
                        switch (reader.GetString(0))
                        {
                            case "IMPUESTO1_DESC":
                                this.Impuesto1Descripcion = reader.GetString(1);
                                break;
                            case "IMPUESTO2_DESC":
                                this.Impuesto2Descripcion = reader.GetString(1);
                                break;
                            case "UTILIZA_MIN_EXENC":
                                this.UtilizaMinimoExento = reader.GetString(1).Equals("S");
                                break;
                            case "MONTO_MIN_EXENC":
                                this.MontoMinimoExcento = Convert.ToDecimal(reader.GetString(1));
                                break;
                        }
                        //reader.NextResult();
                    }
                //}
                //else
                //    throw new Exception("No se obtuvo información de impuestos para la compañía '" + codigo + "'.");

            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo información de impuestos para la compañía '" + codigo + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        public static Compania Obtener(string codigo,List<Compania> companias )
        {
            foreach(Compania cia in companias)
                if(cia.codigo.Equals(codigo))
                    return cia;
            return null;
        }

        public static List<Compania> Obtener() 
        {
            SQLiteDataReader reader = null;

            List<Compania> companias = new List<Compania>();
            companias.Clear();

            string sentencia = "SELECT COD_CIA,NOM_CIA, CAMBIODOLAR, IMP1_AFECTA_DESCTO,PAIS,RETENCIONES FROM " + Table.ERPADMIN_COMPANIA;
	
			try
			{
                reader = GestorDatos.EjecutarConsulta(sentencia);

				while(reader.Read())
                {
                    Compania c = new Compania();
                    c.codigo = reader.GetString(0);
                    c.Nombre = reader.GetString(1);
                    c.tipoCambio = reader.GetDecimal(2);
                    c.Impuesto1AfectaDescto = Impuesto.FormaCalculo(reader.GetString(3));
                    if (!reader.IsDBNull(4))
                        c.PaisInstalado = reader.GetString(4);
                    if (!reader.IsDBNull(5))
                        c.Retenciones = reader.GetString(5).Equals("S");
					companias.Add(c);
                }
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}

			return companias;
        }
        
        public static List<Compania> ObtenerEnInventario()
        {
            SQLiteDataReader reader = null;

            List<Compania> companias = new List<Compania>();
            companias.Clear();

            string sentencia = "SELECT DISTINCT COMPANIA FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA;

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    Compania c = new Compania();
                    c.codigo = reader.GetString(0).ToUpper();                    
                    if(!companias.Exists(x=>x.Codigo==c.codigo))
                        companias.Add(c);
                }
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
          
            return companias;
        }

        /*
        public static List<Compania> ObtenerCompanias(string cliente, bool consignacion)
        {
            List<Compania> companias = new List<Compania>();
            companias.Clear();

            string sentencia = 
                 " SELECT DISTINCT COD_CIA FROM " + Softland.ERP.FR.Mobile.Cls.FRCliente.ClienteCia.TABLA +
                @" WHERE COD_CLT = @CLIENTE";

            if (consignacion)
            {
                sentencia+= " AND BODEGA_CONSIGNA != " + Global.NoDefinido;
            }
            
            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = { new SQLiteParameter("@CLIENTE", cliente) };
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Compania c = new Compania();
                    c._Codigo = reader.GetString(0);
                    companias.Add(c);
                }
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

            return companias;
        }
        */
        #endregion


    }
}
