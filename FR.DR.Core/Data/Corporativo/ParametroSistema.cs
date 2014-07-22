using System;
using System.Collections;
using System.Data;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Collections.Generic;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
namespace Softland.ERP.FR.Mobile.Cls
{
	/// <summary>
	/// Contiene cada uno de los consecutivos por ruta.
	/// </summary>
	public class ParametroSistema
    {
        #region Constantes

        private const string  NUM_DEV = "NUM_DEV";
        private const string  NUM_REC = "NUM_REC";
        private const string  NUM_PED = "NUM_PED";
        private const string  NUM_GAR = "NUM_GAR";
        private const string  NUM_PED_DESC = "NUM_PED_DESC";
        private const string  NUM_FAC = "NUM_FAC";
        private const string  NUM_INV = "NUM_INV";
        private const string  NOTACREDITO = "NOTACREDITO";

        #endregion

        #region Variables de Clase

        private static List<ParametroSistema> consecutivos  = new List<ParametroSistema>();
        /// <summary>
        /// Lista de consecutivos de la ruta (uno por cada compania)
        /// </summary>
        public static List<ParametroSistema> Consecutivos
        {
            get { return consecutivos; }
            set { consecutivos = value; }
        }
        
        #endregion

		#region Variables de instancia
        
		private string compania = string.Empty;
        /// <summary>
        /// Compania a la que pertenece el consecutivo
        /// </summary>
        public string Compania
        {
          get { return compania.ToUpper(); }
          set { compania = value.ToUpper(); }
        }
		private string zona = string.Empty;
        /// <summary>
        /// Zona a la que pertenece el consecutivo
        /// </summary>
        public string Zona
        {
          get { return zona; }
          set { zona = value; }
}
		private string numeroPedido = string.Empty;
        /// <summary>
        /// consecutivo para pedidos
        /// </summary>
        public string NumeroPedido
        {
          get { return numeroPedido; }
          set { numeroPedido = value; }
        }
		private string numeroRecibo = string.Empty;
        /// <summary>
        /// consecutivo para recibos de cobro
        /// </summary>
        public string NumeroRecibo
        {
          get { return numeroRecibo; }
          set { numeroRecibo = value; }
        }
		private string numeroDPR = string.Empty ;
        /// <summary>
        /// consecutivo para 
        /// </summary>
        public string NumeroDPR
        {
          get { return numeroDPR; }
          set { numeroDPR = value; }
        }
		private string numeroDevolucion = string.Empty;
        /// <summary>
        /// consecutivo para devoluciones
        /// </summary>
        public string NumeroDevolucion
        {
          get { return numeroDevolucion; }
          set { numeroDevolucion = value; }
        }
		private string numeroFactura = string.Empty;
        /// <summary>
        /// consecutivo para factura
        /// </summary>
        public string NumeroFactura
        {
          get { return numeroFactura; }
          set { numeroFactura = value; }
        }
        private string numeroGarantia = string.Empty;
        /// <summary>
        /// consecutivo para Garantia
        /// </summary>
        public string NumeroGarantia
        {
            get { return numeroGarantia; }
            set { numeroGarantia = value; }
        }
		private string numeroInventario= string.Empty;
        /// <summary>
        /// consecutivo para inventario
        /// </summary>
        public string NumeroInventario
        {
          get { return numeroInventario; }
          set { numeroInventario = value; }
        }
		private string numeroPedidoDescuento = string.Empty;
        /// <summary>
        /// consecutivo para pedido con descuento
        /// </summary>
        public string NumeroPedidoDescuento
        {
          get { return numeroPedidoDescuento; }
          set { numeroPedidoDescuento = value; }
        }

        private string numeroNotaCredito = string.Empty;
        /// <summary>
        /// consecutivo para notas de crédito
        /// </summary>
        public string NumeroNotaCredito
        {
            get { return numeroNotaCredito; }
            set { numeroNotaCredito = value; }
        }

		#endregion

		#region Constantes de la instancia

		private const string SEPARADOR = "#|#";

		#endregion

		#region Metodos de la clase

        #region Incremento Consecutivos

        /// <summary>
        /// Incrementar el consecutivo de devolucion
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarDevolucion(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania,  zona);
            string nuevoConsecutivo= p.IncrementarConsecutivo(Consecutivo.Devolucion, p.NumeroDevolucion);
            p.NumeroDevolucion= nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo de la garantía
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarGarantia(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.Garantia, p.numeroGarantia);
            p.NumeroGarantia = nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo de la factura
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarFactura(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.Factura, p.NumeroFactura);
            p.NumeroFactura = nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo del inventario 
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarInventario(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.Inventario, p.numeroInventario);
            p.numeroInventario = nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo del pedido
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarPedido(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.Pedido, p.NumeroPedido);
            p.numeroPedido = nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo de pedidos con descuentos hechos por el usuario 
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarPedidoDesc(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.PedidoDescuento, p.numeroPedidoDescuento);
            p.numeroPedidoDescuento = nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo de recibos de cobro
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        public static void IncrementarRecibo(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.Recibo, p.numeroRecibo);
            p.numeroRecibo = nuevoConsecutivo;
        }
        /// <summary>
        /// Incrementar el consecutivo de recibos de cobro.
        /// </summary>
        /// <param name="compania"></param>
        /// <param name="zona"></param>
        public static void IncrementarNotaCredito(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            string nuevoConsecutivo = p.IncrementarConsecutivo(Consecutivo.NotaCredito, p.numeroNotaCredito);
            p.numeroNotaCredito = nuevoConsecutivo;
        }
        
        #endregion

        #region Obtencion Consecutivos
        /// <summary>
        /// obtener los parametros de sistema de una compania en su zona
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <param name="zona">codigo de la zona</param>
        private static ParametroSistema ObtenerConsecutivo(string compania, string zona)
        {
            foreach (ParametroSistema p in consecutivos)
            {
                if (p.Compania.Equals(compania) && p.zona.Equals(zona))
                {
                    return p;
                }
            }
            throw new Exception("Error al obtener consecutivos para la compañía '" + compania + "', en la ruta " + zona + ". ");


        }
        /// <summary>
        /// Obtener el campo de la base de datos asociado al consecutivo segun el numero de consecutivo
        /// </summary>
        /// <param name="tipo">tipo de consecutivo</param>
        /// <returns>nombre del campo en la base de datos</returns>
        private string Campo(Consecutivo tipo)
        {
            switch (tipo)
            {
                case Consecutivo.Devolucion: return NUM_DEV;
                case Consecutivo.Factura: return NUM_FAC;
                case Consecutivo.Garantia: return NUM_GAR;
                //LJR 15/01/10 Caso: 37547 Duplicación de Consecutivos: 
                case Consecutivo.Recibo: return NUM_REC;
                case Consecutivo.Pedido: return NUM_PED;
                case Consecutivo.PedidoDescuento: return NUM_PED_DESC;
                case Consecutivo.Inventario: return NUM_INV;
                case Consecutivo.NotaCredito: return NOTACREDITO;
                default: throw new Exception("Tipo de consecutivo invalido");
            }
        }

        /// <summary>
        /// Obtiene el consecutivo disponible de devolución de la ruta actual.
        /// </summary>
        /// <param name="codZon"></param>
        /// <param name="codCia"></param>
        /// <returns></returns>
        public static string ObtenerDevolucion(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroDevolucion;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para recibos de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerRecibo(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroRecibo;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para pedidos de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerPedido(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroPedido;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para pedidos con descuento de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerPedidoDesc(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroPedidoDescuento;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para facturas de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerFactura(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroFactura;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para Garantias de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerGarantia(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroGarantia;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para inventarios de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerInventario(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroInventario;
        }

        /// <summary>
        /// Obtiene el consecutivo disponible para notas de créditos de la ruta actual.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerNotaCredito(string compania, string zona)
        {
            ParametroSistema p = ObtenerConsecutivo(compania, zona);
            return p.numeroNotaCredito;
        }

        #endregion

        #endregion 

        #region Acceso Datos
        /// <summary>
        /// Incrementar un consecutivo
        /// </summary>
        /// <param name="c">Tipo de consecutivo</param>
        /// <param name="consecutivoActual">numero de consecutivo actual</param>
        /// <returns>El siguiente consecutivo</returns>
        private string IncrementarConsecutivo(Consecutivo c, string consecutivoActual)
        {
            string nuevoConsecutivo = GestorUtilitario.proximoCodigo(consecutivoActual,20);
            string sentencia =
						" UPDATE " + Table.ERPADMIN_alSYS_PRM  + " SET "+ Campo(c) + " = @CONSECUTIVO " +
                        " WHERE UPPER(COD_CIA) = @COMPANIA " +
						" AND   COD_ZON = @ZONA ";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar,nuevoConsecutivo),
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,compania.ToUpper()),
                GestorDatos.SQLiteParameter("@ZONA",SqlDbType.NVarChar,zona)});

			int actualizo = GestorDatos.EjecutarComando(sentencia,parametros);
			if(actualizo != 1)
				throw new Exception("No se pudo incrementar el consecutivo.");
            return nuevoConsecutivo;
        }
        
		/// <summary>
		/// Metodo que se encargado de consultar los parametros del sistema que son:
		/// codigo cia, codigo zona y los consecutivos para : pedidos, recibos, DPR,
		/// devolucion y factura
		/// </summary>
		public static void CargarParametros()
		{
            consecutivos.Clear();

			SQLiteDataReader reader = null;

			string sentencia = 
				" SELECT COD_CIA, COD_ZON, NUM_PED, NUM_REC, NUM_DOC, " +
                " NUM_DEV, NUM_INV, NUM_PED_DESC, NUM_FAC, NOTACREDITO,NUM_GAR FROM " + Table.ERPADMIN_alSYS_PRM;

			try
			{
                reader = GestorDatos.EjecutarConsulta(sentencia);
			
				while(reader.Read())
				{
					ParametroSistema parametros = new ParametroSistema(); 
					parametros.Compania = reader.GetString(0);//cod_cia
					parametros.zona = reader.GetString(1);//cod_zon 
					parametros.numeroPedido = reader.GetString(2);//num_ped
					parametros.numeroRecibo = reader.GetString(3);//num_rec
					parametros.numeroDPR = reader.GetString(4);//num_doc
					parametros.numeroDevolucion = reader.GetString(5);//num_dev
					parametros.numeroInventario = reader.GetString(6);//num_inv
					parametros.numeroPedidoDescuento = reader.GetString(7);
					parametros.numeroFactura = reader.GetString(8);//num_fac
                    parametros.numeroNotaCredito = reader.GetString(9);//num_nc
                    parametros.numeroGarantia = reader.GetString(10);//num_gar
					consecutivos.Add(parametros);
				}
			}
			catch(Exception ex)
			{
				throw new Exception("Error al cargar los parámetros del Sistema. " + ex.Message);
			}
			finally
			{
				if (reader != null)
					reader.Close(); 
			}
		}

		/// <summary>
		/// Genera una sentencia SQL de actualizacion de los consecutivos.
		/// </summary>
		/// <returns></returns>
		public static string GenerarSentenciaActualizacion()
		{
			string sentencia = string.Empty;

			foreach(ParametroSistema parametro in consecutivos)
			{
				sentencia += 
					"UPDATE %CIA.alSYS_PRM set " + 
					"NUM_PED = '" + parametro.NumeroPedido + "'," +
					"NUM_DEV = '" + parametro.NumeroDevolucion + "'," + 
					"NUM_REC = '" + parametro.NumeroRecibo + "'," + 
					"NUM_INV = '" + parametro.NumeroInventario + "'," +
					"NUM_FAC = '" + parametro.NumeroFactura + "', " +
					"NUM_DOC = '" + parametro.NumeroDPR + "', " +
					"NUM_PED_DESC = '" + parametro.NumeroPedidoDescuento + "', " +
                    "NOTACREDITO = '" + parametro.numeroNotaCredito + "', " +
                    "NUM_GAR = '" + parametro.NumeroGarantia + "' " +
					"WHERE COD_CIA = '" + parametro.Compania+ "' " +
					"AND   COD_ZON = '" + parametro.zona + "'";

				sentencia += ParametroSistema.SEPARADOR;
			}

			return sentencia;
		}

        #endregion
	}
}
