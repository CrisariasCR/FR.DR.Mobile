using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using EMF.Printing.Drivers;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{
    /// <summary>
    /// Representa un cliente con todas sus companias
    /// </summary>
    public class Cliente : ClienteBase
    {
        
        #region Variables y Propiedades de instancia

        private List<ClienteCia> clienteCompania = new List<ClienteCia>();
        /// <summary>
        /// Lista de sus caracteristicas especificas en cada compania
        /// </summary>
        public List<ClienteCia> ClienteCompania
        {
            get { return clienteCompania; }
            set { clienteCompania = value; }
        }

        /// <summary>
        /// Verifica si el cliente tiene facturas vencidas. 
        /// </summary>
        /// <returns></returns>
        public bool TieneFacturasVencidas
        {
            get
            {
                return Factura.TieneFacturasPendientes(this.Codigo, TipoDocumento.Factura, true);
            }
        }
        /// <summary>
        /// Verifica si cliente tiene facturas de contado sin cancelar.
        /// </summary>
        /// <returns></returns>
        public bool TieneFacturasContadoPendientes
        {
            get
            {
                return Factura.TieneFacturasPendientes(this.Codigo, TipoDocumento.FacturaContado, false);
            }
        }
        /// <summary>
        /// Verifica si cliente tiene facturas por cobrar.
        /// </summary>
        /// <returns></returns>
        public bool TieneFacturasPendientes
        {
            get
            {
                return Factura.TieneFacturasPendientes(this.Codigo, TipoDocumento.Factura, false);
            }
        }
        /// <summary>
        /// Verifica si cliente tiene documentos sin cancelar.
        /// </summary>
        /// <returns></returns>
        public bool TieneDocumentosVencidos
        {
            get
            {
                return Factura.TieneDocumentosPendientes(this.Codigo, true);
            }
        }
        /// <summary>
        /// Determina si el cliente tiene niveles de precio definido
        /// </summary>
        public bool NivelesPreciosDefinidos
        {
            get
            {
                bool hayNivelesDefinidos = false;
                foreach (ClienteCia clt in clienteCompania)
                {
                    clt.CargarNivelPrecio();
                    if (clt.NivelPrecio.Nivel != string.Empty && clt.NivelPrecio.Codigo!= 99 )
                        hayNivelesDefinidos = true;
                }
                return hayNivelesDefinidos;
            }
        }

        /// <summary>
        /// Obtiene los dias que el cliente debe ser visitado
        /// </summary>
        /// <returns>Una cadena de letras que representan un dia de la semana separadas por coma</returns>
        public List<Dias> ObtenerDiasVisita(string zona)
        {
            return Ruta.DiasVisita(this.Codigo, zona);
        }
        /// <summary>
        /// Carga las facturas pendientes de cobro a un cliente para una compania en particular
        /// </summary>
        /// <param name="compania">compania asociada</param>
        public void CargarFacturasCobro(string compania)
        {
                foreach (ClienteCia clt in clienteCompania)
                {
                    if (clt.Compania== compania)
                        clt.CargarFacturasPendientes(Zona);
                }        
        }
        /// <summary>
        /// Carga las notas de credito pendientes de aplicar a un cliente para una compania en particular
        /// </summary>
        /// <param name="compania">compania asociada</param>       
        public void CargarNotasCredito(string compania)
        {
            foreach (ClienteCia clt in clienteCompania)
            {
                if (clt.Compania == compania)
                    clt.CargarNotasCredito(Zona);
            }
        }
        /// <summary>
        /// Verifica si cliente tiene limite de credito excedido.
        /// </summary>
        /// <returns></returns>
        
        // PA2-01490-78K2 - KFC
        // Se agrega el parametro con el monto de la factura en proceso para que haga la validacion
        public bool LimiteCreditoExcedido(string cia,decimal montoFactActual)
        {
            return Factura.ValidaLimiteCredito(this.Codigo, cia, montoFactActual);
        }
        //public bool LimiteCreditoExcedido(string cia)
        //{
        //    return Factura.ValidaLimiteCredito(this.Codigo, cia);
        //}       

        
        #region Montos del cliente

        private ClienteMontos montos;
        /// <summary>
        /// Montos de los reportes
        /// </summary>
        public ClienteMontos Montos
        {
            get { return montos; }
            set { montos = value; }
        }

        #endregion

        #endregion

        #region Acceso Datos

        /// <summary>
        /// Cargar los datos del cliente
        /// </summary>
        public void Cargar() 
        {
            string sentencia =
                " SELECT NOM_CLT,DIR_CLT FROM " + Table.ERPADMIN_CLIENTE +
                " WHERE COD_CLT = @CLIENTE";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@CLIENTE", Codigo);

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                if (reader.Read())
                {
                    Nombre = reader.GetString(0);
                    Direccion = reader.GetString(1);
                }
                else
                    throw new Exception("No se encontró información del cliente.");
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
        /// Obtener la lista de los clientes que han sido visitados en ruta
        /// </summary>
        /// <returns>lista de clientes visitados</returns>
        public static List<Cliente> ObtenerClientesVisitados()
        {
            SQLiteDataReader reader = null;
            List<Cliente> clientes = new List<Cliente>();
            clientes.Clear();
            try
            {
                string sentencia =
                    " SELECT DISTINCT C.COD_CLT, C.NOM_CLT " +
                    " FROM " + Table.ERPADMIN_VISITA + " V, " + Table.ERPADMIN_CLIENTE + " C " +
                    " WHERE C.COD_CLT = V.CLIENTE " +
                    " AND julianday(date(INICIO)) = julianday(date('now','localtime'))";

                SQLiteParameterList parametros = new  SQLiteParameterList();
                parametros.Add("@HOY", DateTime.Now.ToString("yyyyMMdd"));

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    Cliente cliente = new Cliente();
                    cliente.Codigo = reader.GetString(0);
                    cliente.Nombre = reader.GetString(1);
                    clientes.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando los clientes visitados." + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return clientes;
        }

        /// <summary>
        /// Obtener lista de clientes con los reportes de montos cargados
        /// </summary>
        /// <returns>lista de clientes</returns>
        public static List<Cliente> CargaReporteMontos()
        {
            List<Cliente> clientes = ObtenerClientesVisitados();
            foreach (Cliente cliente in clientes)
            {
                cliente.montos.CargarMontoCobrado(cliente.Codigo);
                cliente.montos.CargarMontoVendido(cliente.Codigo);
            }
            return clientes;
        }
       
        /// <summary>
        /// ABC Caso 35771
        /// Carga los clientes visitados con sus respectivos montos
        /// de los de productos devueltos.
        /// </summary>
        /// <returns>ArrayList con objetos Cliente</returns>
        public static List<Cliente> CargaReporteMontosDevueltos()
        {
            List<Cliente> clientes = ObtenerClientesVisitados();
            foreach (Cliente cliente in clientes)
            {
                try
                {
                    cliente.montos.CargarMontoDevuelto(cliente);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error cargando el monto devuelto por el cliente '" + cliente.Codigo + "'. " + ex.Message);
                }

            }

            return clientes;
        }

        #endregion

        #region Busquedas

        #endregion

        /// <summary>
        /// Cargar los clientes en compania
        /// </summary>
        public void ObtenerClientesCia()
        {
            if (this.clienteCompania.Count == 0)
                clienteCompania = ClienteCia.ObtenerClientes(this.Codigo, false);
        }

        /// <summary>
        /// Determina si un cliente utiliza consignacion
        /// </summary>
        /// <returns></returns>
        public bool UsaConsignacion()
        { 
            ObtenerClientesCia();
            foreach (ClienteCia clt in this.clienteCompania)
            {
                if (clt.Bodega != FRdConfig.NoDefinido)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// obtener un cliente para una compania especifica
        /// </summary>
        /// <param name="cia">compania</param>
        /// <returns>cliente en compania</returns>
        public ClienteCia ObtenerClienteCia(string cia)
        {
            foreach (ClienteCia clt in this.clienteCompania)
            {
                if (clt.Compania.ToUpper().Equals(cia.ToUpper()))
                {
                    clt.CargarNivelPrecio();
                    //Si el cliente es dolar se cambia la variable global a true para que cambien los signos
                    GestorUtilitario.esDolar = clt.Moneda == TipoMoneda.DOLAR;
                    AndroidDriver.esDolar = GestorUtilitario.esDolar;
                    return clt;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Cargar las direcciones de entrega de un cliente en una compania
        /// </summary>
        /// <param name="cia">compania del cliente a cargar</param>
        public void ObtenerDireccionesEntrega(string cia)
        {
            foreach (ClienteCia clt in this.clienteCompania)
            {
                if (clt.Compania.Equals(cia))
                {
                    clt.ObtenerDireccionesEntrega();
                }
            }
        }
        
        /// <summary>
        /// Obtener una lista de clientes por un criterio de busqueda
        /// </summary>
        /// <param name="c">criterio a buscar</param>
        /// <returns>lista de clientes</returns>
        public static List<Cliente> CargarClientes(CriterioBusquedaCliente c)
        {
            List<Cliente> clientes = new List<Cliente>();
            clientes.Clear();
            SQLiteDataReader reader = null;
            
            string sentencia = CrearSentenciaBusquedaCliente(c);
            SQLiteParameterList parametros = CrearParametrosBusquedaCliente(c);
 
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);
                while (reader.Read())
                {
                    if (clientes.Count > 0)
                    {
                        string cod=reader.GetString(0);
                        if (!clientes.Exists(x => x.Codigo == cod))
                        {
                            Cliente cliente = new Cliente();
                            cliente.Codigo = reader.GetString(0);
                            cliente.Nombre = reader.GetString(1);
                            cliente.Direccion = reader.GetString(2);
                            cliente.Zona = reader.GetString(3);
                            clientes.Add(cliente);
                        }
                    }
                    else
                    {
                        Cliente cliente = new Cliente();
                        cliente.Codigo = reader.GetString(0);
                        cliente.Nombre = reader.GetString(1);
                        cliente.Direccion = reader.GetString(2);
                        cliente.Zona = reader.GetString(3);
                        clientes.Add(cliente);
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando los clientes. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return clientes; 
        }
        
        /// <summary>
        /// Crear los parametros de busqueeda para el cliente segun los criterios
        /// </summary>
        /// <param name="c">criterios de busqueda</param>
        /// <returns>parametros</returns>
        private static SQLiteParameterList CrearParametrosBusquedaCliente(CriterioBusquedaCliente c)
        {
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Clear();

            parametros.Add("@VALOR", c.Valor);
            if (c.Criterio == CriterioCliente.Zona)
            {
                //parametros.Add("@HOY", DateTime.Now.ToString("YYYY-MM-DD"));
                if (c.Dias != Dias.T)
                    parametros.Add("@DIA", c.Dias.ToString());
            }
            return parametros;
        }
        
        /// <summary>
        /// Generar la sentencia de busqueda de cliente segun el criterio
        /// </summary>
        /// <param name="c">criterios de busqueda</param>
        /// <returns></returns>
        private static string CrearSentenciaBusquedaCliente(CriterioBusquedaCliente c)
        {
            string sentencia=string.Empty;

            if(c.Criterio == CriterioCliente.Zona)
            {
                if(c.Visita.Equals("Visitados"))
                {
                    sentencia +=
                        " SELECT DISTINCT C.COD_CLT, C.NOM_CLT, C.DIR_CLT, C.COD_ZON, " +
                        " V.INICIO AS FECHA_VISITA,O.DIA,O.ORDEN,C.FEC_PLN " +
                        " FROM " + Table.ERPADMIN_CLIENTE + " C " +
                        " INNER JOIN " + Table.ERPADMIN_alFAC_RUTA_ORDEN + " O " +
                        " ON ( C.COD_ZON = @VALOR " +
                        "      AND O.COD_ZON = C.COD_ZON " +
                        "      AND O.COD_CLT = C.COD_CLT) " +
                        " INNER JOIN " + Table.ERPADMIN_VISITA + " V " +
                        " ON (C.COD_CLT = V.CLIENTE " +
                        " AND julianday(date(V.INICIO)) = julianday(date('now','localtime'))) ";
                    if (c.Dias != Dias.T )
                        sentencia += " WHERE O.DIA = @DIA " ;
                    sentencia += (c.Alfabetico ? " ORDER BY C.NOM_CLT":" ORDER BY FECHA_VISITA,O.DIA,O.ORDEN,C.FEC_PLN");                
                }

                else if(c.Visita.Equals("No Visitados"))
                {
                    sentencia +=
                        " SELECT DISTINCT C.COD_CLT, C.NOM_CLT, C.DIR_CLT, C.COD_ZON, O.DIA,O.ORDEN,C.FEC_PLN" +
                        " FROM " + Table.ERPADMIN_CLIENTE + " C " +
                        " INNER JOIN " + Table.ERPADMIN_alFAC_RUTA_ORDEN + " O " +
                        " ON ( C.COD_ZON = @VALOR "+ 
                        "      AND O.COD_ZON = C.COD_ZON " +
                        "      AND O.COD_CLT = C.COD_CLT) ";
	
			        sentencia += (c.Dias != Dias.T ?" WHERE O.DIA = @DIA AND" : " WHERE " ) +
                        " C.COD_CLT NOT IN (SELECT CLIENTE FROM " + Table.ERPADMIN_VISITA +
                                          " WHERE julianday(date(INICIO)) = julianday(date('now','localtime'))) ";						
			
			        sentencia += (c.Alfabetico ? " ORDER BY C.NOM_CLT":" ORDER BY O.DIA,O.ORDEN,C.FEC_PLN");                
                }
                else if(c.Visita.Equals("Todos"))
                {
                    sentencia +=
                        " SELECT DISTINCT C.COD_CLT, C.NOM_CLT, C.DIR_CLT, C.COD_ZON, " +
                        " V.INICIO AS FECHA_VISITA,O.DIA,O.ORDEN,C.FEC_PLN " +
                        " FROM " + Table.ERPADMIN_CLIENTE + " C " +
                        " INNER JOIN " + Table.ERPADMIN_alFAC_RUTA_ORDEN + " O " +
                        " ON ( C.COD_ZON = @VALOR "+ 
                        "      AND O.COD_ZON = C.COD_ZON " +
                        "      AND O.COD_CLT = C.COD_CLT) " +
                        " LEFT OUTER JOIN " + Table.ERPADMIN_VISITA + " V " +
				        " ON (C.COD_CLT = V.CLIENTE " +
                        " AND julianday(date(V.INICIO)) = julianday(date('now','localtime'))) ";

                    if (c.Dias != Dias.T)
                        sentencia += " WHERE O.DIA = @DIA ";
                    sentencia += (c.Alfabetico ? " ORDER BY C.NOM_CLT" : " ORDER BY FECHA_VISITA,O.DIA,O.ORDEN,C.FEC_PLN");                    
                }
            }
            else
            {
                sentencia+=
                        " SELECT DISTINCT C.COD_CLT, C.NOM_CLT, C.DIR_CLT, C.COD_ZON, C.FEC_PLN " +
                        " FROM " + Table.ERPADMIN_CLIENTE + " C " +
                        " INNER JOIN " + Table.ERPADMIN_alFAC_RUTA_ORDEN + " O " +
                        " ON ( O.COD_CLT = C.COD_CLT) "; 
               
                if(c.Criterio == CriterioCliente.Nombre)
                    sentencia+= " WHERE C.NOM_CLT "+(c.Agil? "=" : "LIKE")+" @VALOR ";
                else if(c.Criterio == CriterioCliente.Codigo)
                    sentencia += " WHERE C.COD_CLT " + (c.Agil ? "=" : "LIKE") + "  @VALOR ";

                sentencia += (c.Alfabetico ? " ORDER BY C.NOM_CLT": " ORDER BY C.COD_ZON, C.FEC_PLN");             
            }
            return sentencia;
        }
        
        #region IPrintable Members

        public override object GetField(string name)
        {
            switch (name)
            {
                case "COBRADO": return montos.MontoPagado;
                case "NOTAS_CREDITO": return montos.MontoNotasAplicadas;
                case "VENDIDO": return montos.MontoComprado;
                //LDS Caso 27409 20/03/2007
                case "PREVENTA": return montos.MontoPreventa;
                case "FACTURADO": return montos.MontoFacturas;
                //ABC Caso 35771 
                case "DEVUELTOSINDOC":return montos.MontoDevueltoSinDoc;
                case "DEVUELTOCONDOC":return montos.MontoDevueltoConDoc;
                case "DEVUELTOTOTAL":return montos.MontoDevuelto;
                default: return base.GetField(name);
            }
        }

        #endregion
    }
}
