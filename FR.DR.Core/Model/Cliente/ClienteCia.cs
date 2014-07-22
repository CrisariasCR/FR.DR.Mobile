using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using EMF.Printing;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{ 
    /// <summary>
    /// Representa un cliente para una compania determinada
    /// </summary>
    public class ClienteCia : IPrintable
    {
        #region Variables y Propiedades de instancia

        private NivelPrecio nivelPrecio;
        /// <summary>
        /// Nivel de precio asociado al cliente en la compania
        /// </summary>
        public NivelPrecio NivelPrecio
        {
            get { return nivelPrecio; }
            set { nivelPrecio = value; }
        }
        private string codigo = string.Empty;
        /// <summary>
        /// Código del cliente.
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string compania = string.Empty;
        /// <summary>
        /// Compania asociada
        /// </summary>
        public string Compania
        {
          get { return compania.ToUpper(); }
          set { compania = value.ToUpper(); }
        }

        private string regimen = string.Empty;
        /// <summary>
        /// Regimen asociado
        /// </summary>
        public string Regimen
        {
            get { return regimen; }
            set { regimen = value; }
        }

        public override string ToString()
        {
            return Compania;
        }

        public string Descripcion 
        {
            get { return Compania; }
        }
        
        

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Tipo de moneda que utiliza el cliente.
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private string contacto = string.Empty;
        /// <summary>
        /// Contacto para el cliente.
        /// </summary>
        public string Contacto
        {
            get { return contacto; }
            set { contacto = value; }
        }

        private string telefono = string.Empty;
        /// <summary>
        /// Telefono del cliente.
        /// </summary>
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; }
        }

        private string condicionPago = string.Empty;
        /// <summary>
        /// Condicion de pago del cliente.
        /// </summary>
        public string CondicionPago
        {
            get { return condicionPago; }
            set { condicionPago = value; }
        }

        private int diasCredito = 0;
        /// <summary>
        /// Dias de credito que tiene el cliente.
        /// </summary>
        public int DiasCredito
        {
            get { return diasCredito; }
            set { diasCredito = value; }
        }

        private decimal limiteCredito = 0;
        /// <summary>
        /// Limite de credito del cliente.
        /// </summary>
        public decimal LimiteCredito
        {
            get { return limiteCredito; }
            set { limiteCredito = value; }
        }

        private decimal exoneracionImp1 = 0;
        /// <summary>
        /// Porcentaje de exoneracion del impuesto de ventas que tiene el cliente.
        /// </summary>
        public decimal ExoneracionImp1
        {
            get { return exoneracionImp1; }
            set { exoneracionImp1 = value; }
        }

        private decimal exoneracionImp2 = 0;
        /// <summary>
        /// Porcentaje de exoneracion del impuesto de consumo que tiene el cliente.
        /// </summary>
        public decimal ExoneracionImp2
        {
            get { return exoneracionImp2; }
            set { exoneracionImp2 = value; }
        }

        private string pais = string.Empty;
        /// <summary>
        /// Pais al que pertenece el cliente.
        /// </summary>
        public string Pais
        {
            get { return pais; }
            set { pais = value; }
        }

        private string divisionGeografica1 = string.Empty;
        /// <summary>
        /// DivisionGeografica1 al que pertenece el cliente.
        /// </summary>
        public string DivisionGeografica1
        {
            get { return divisionGeografica1; }
            set { divisionGeografica1 = value; }
        }

        private string divisionGeografica2 = string.Empty;
        /// <summary>
        /// DivisionGeografica1 al que pertenece el cliente.
        /// </summary>
        public string DivisionGeografica2
        {
            get { return divisionGeografica2; }
            set { divisionGeografica2 = value; }
        }
       
        private string tipo = string.Empty;
        /// <summary>
        /// Tipo de cliente.
        /// </summary>
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        
        private string direccionEntrega = string.Empty;
        /// <summary>
        /// Direccion de entrega predeterminada del cliente.
        /// </summary>
        public string DireccionEntregaDefault
        {
            get { return direccionEntrega; }
            set { direccionEntrega = value; }
        }

        private decimal descuento = 0;
        /// <summary>
        /// Descuento general asociado al cliente.
        /// </summary>
        public decimal Descuento
        {
            get { return descuento; }
            set { descuento = value; }
        }

        private string bodega;
        /// <summary>
        /// Corresponde a la bodega asignada al cliente para realizar la venta en consginación,
        /// si esta variable posee el valor de ND entonces el cliente no trabaja bajo el esquema de venta en 
        /// consignación.
        /// </summary>
        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }

        private List<DireccionEntrega> direccionesEntrega = new List<DireccionEntrega>();
        /// <summary>
        /// Direcciones de entrega del cliente para una compañía.
        /// </summary>
        public List<DireccionEntrega> DireccionesEntrega
        {
            get { return direccionesEntrega; }
            set { direccionesEntrega = value; }
        }


        private List<NotaCredito> notasCredito = new List<NotaCredito>();
        
        /// <summary>
        /// Notas de crédito asociadas al cliente.
        /// </summary>
        public List<NotaCredito> NotasCredito
        {
            get
            {
                return notasCredito;
            }
        }
        
        private List<Factura> facturasPendientes = new List<Factura>();
        /// <summary>
        /// Facturas pendientes del cliente asociadas a una compania.
        /// </summary>
        public List<Factura> FacturasPendientes
        {
            get
            {
                return facturasPendientes;
            }
        }

        //ABC CR0-01786-L1QT Incluye si un cliente es multimoneda.
        private bool multiMoneda = false;
        /// <summary>
        /// Si el cliente es MultiMoneda.
        /// </summary>
        public bool MultiMoneda
        {
            get { return multiMoneda; }
            set { multiMoneda = value; }
        }

        #region Acepta Fracciones
        private bool aceptaFracciones;

        /// <summary>
        /// Indica si el cliente utiliza multiplos de venta.
        /// </summary>
        public bool AceptaFracciones
        {
            get
            {
                return aceptaFracciones;
            }
            set
            {
                aceptaFracciones = value;
            }
        }
        #endregion

        //ABC Manejo NCF
        private string tipoContribuyente;

        /// <summary>
        /// Indica el tipo de contribuyente para NCF
        /// </summary>
        public string TipoContribuyente
        {
            get
            {
                return tipoContribuyente;
            }
            set
            {
                tipoContribuyente = value;
            }
        }

        private string contribuyente;

        /// <summary>
        /// Numero de COntribuyente o NIT
        /// </summary>
        public string Contribuyente
        {
            get
            {
                return contribuyente;
            }
            set
            {
                contribuyente = value;
            }
        }

        #endregion

        public ClienteCia()
        {
        }

        /// <summary>
        /// Obtener la lista de clientes en companias para un codigo de cliente
        /// </summary>
        /// <param name="codigo">codigo del cliente a obtener</param>
        /// <param name="consignacion">solo obtener clientes en consignacion</param>
        /// <returns>lista de clientes en compania</returns>
        public static List<ClienteCia> ObtenerClientes(string codigo, bool consignacion) 
        {
            SQLiteDataReader reader = null;
            List<ClienteCia> clientes = new List<ClienteCia>();
            clientes.Clear();

            try
            {
                string sentencia =
                    " SELECT COD_CIA, COD_TIP_CL,LST_PRE,NOM_CTO,NUM_TEL," +
                    " LIM_CRE,DIA_CRE,POR_EXC_VT,POR_EXC_CS,IND_MON," +
                    " COD_PAIS,COD_CND,DIR_EMB_DEFAULT,DESCUENTO,BODEGA_CONSIGNA,MULT_MON,ACEPTA_FRACCIONES, TIPO_CONTRIBUYENTE, CONTRIBUYENTE , DIVISION_GEOGRAFICA1,DIVISION_GEOGRAFICA2,REGIMEN_TRIB " +
                    " FROM " + Table.ERPADMIN_CLIENTE_CIA +
                    " WHERE COD_CLT = @CLIENTE ";
                
                if (consignacion)
                    sentencia+= " AND BODEGA_CONSIGNA != '" + FRdConfig.NoDefinido + "'";
                            
                SQLiteParameterList parametros = new SQLiteParameterList();
                parametros.Add("@CLIENTE", codigo);

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    ClienteCia cliente = new ClienteCia();
                    cliente.Codigo = codigo;
                    cliente.Compania = reader.GetString(0);
                    cliente.tipo = reader.GetString(1);
                    cliente.NivelPrecio = new NivelPrecio(cliente.Compania, reader.GetInt32(2));
                    cliente.contacto = reader.GetString(3);
                    cliente.telefono = reader.GetString(4);
                    cliente.limiteCredito = reader.GetDecimal(5);
                    cliente.diasCredito = reader.GetInt32(6);
                    cliente.exoneracionImp1 = reader.GetDecimal(7);
                    cliente.exoneracionImp2 = reader.GetDecimal(8);
                    cliente.moneda = (TipoMoneda)Convert.ToChar(reader.GetString(9));
                    cliente.pais = reader.GetString(10);
                    cliente.condicionPago = reader.GetString(11);
                    cliente.direccionEntrega = reader.GetString(12);
                    cliente.descuento = reader.GetDecimal(13);
                    cliente.bodega = reader.GetString(14);
                    //ABC CR0-01786-L1QT Incluye si un cliente es multimoneda.
                    cliente.multiMoneda = reader.GetString(15) == "S";
                    //LAS. Mejora Multiplos de Venta.
                    cliente.aceptaFracciones = reader.GetString(16) == "S";
                    //ABC Manejo NCF
                    cliente.tipoContribuyente = !string.IsNullOrEmpty(reader.GetValue(17).ToString()) ? reader.GetString(17) : string.Empty;

                    //ABC Manejo NCF
                    cliente.contribuyente = !string.IsNullOrEmpty(reader.GetValue(18).ToString()) ? reader.GetString(18) : string.Empty;

                    //Manejo de Retenciones
                    cliente.DivisionGeografica1 = !string.IsNullOrEmpty(reader.GetValue(19).ToString()) ? reader.GetString(19) : string.Empty;
                    cliente.DivisionGeografica2 = !string.IsNullOrEmpty(reader.GetValue(20).ToString()) ? reader.GetString(20) : string.Empty;
                    cliente.Regimen = !string.IsNullOrEmpty(reader.GetValue(21).ToString()) ? reader.GetString(21) : string.Empty;

                    clientes.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede obtener los datos del cliente. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return clientes;
        }
        
        /// <summary>
        /// Carga La descripcion del Nivel de Precio por demanda una sola vez
        /// </summary>
        public void CargarNivelPrecio()
        {
            if (NivelPrecio.Nivel.Equals(string.Empty))
                NivelPrecio.CargarNivelPrecio(Compania);
        }
        
        /// <summary>
        /// Carga facturas pendientes por demanda una sola vez
        /// </summary>        
        public void CargarFacturasPendientes(string zona)
        {
            //if (this.facturasPendientes.Count == 0)
                this.facturasPendientes = Factura.ObtenerFacturasPendientesCobro(this.Compania, this.codigo, zona);       
        }
        
        /// <summary>
        /// Carga notas de credito pendientes por demanda una sola vez
        /// </summary>                
        public void CargarNotasCredito(string zona)
        {
            //if (this.notasCredito.Count == 0)
                this.notasCredito = NotaCredito.ObtenerNotasCredito(this.Compania, this.codigo, zona);
        }
        
        /// <summary>
        /// Obtener direcciones de entrega del cliente en la compania
        /// </summary>
        public void ObtenerDireccionesEntrega()
        {
            if (DireccionesEntrega.Count == 0)
            {
                DireccionesEntrega = DireccionEntrega.ObtenerDireccionesEntrega(this.Compania, this.Codigo);
            }
        }

        /// <summary>
        /// Obtiene la condición de pago que se desplegará en el reporte de venta en consignación.
        /// La condición de pago se desplegará en el siguiente formato: si la condición de pago 
        /// de la venta en consignación es de contado entonces se obtendrá "Contado", si es a crédito se 
        /// desplegará "Crédito a # día(s)".
        /// </summary>
        /// <returns>
        /// Retorna la condicón de pago de la venta en consignación en el formato especificado para el reporte.
        /// </returns>
        private string ObtenerCondicionPago()
        {
            if (condicionPago == Corporativo.Compania.Obtener(this.Compania).CondicionPagoContado)
            {
                return "Contado";
            }
            else
            {
                try
                {
                    //Corporativo.CondicionPago.ObtenerDiasNeto(condicionPago,compania)
                    return "Crédito a " + diasCredito + " día(s)";
                }
                catch (Exception ex)
                {
                    throw new Exception("Error obteniendo los días de crédito. " + ex.Message);
                }
            }

        }

        #region IPrintable Members

        public string GetObjectName()
        {
            return "CLIENTECIA";
        }

        public object GetField(string name)
        {
            switch (name)
            {

                case "CLIENTE_NOMBRE": return contacto;

                case "DIRECCION" :
                case "CLIENTE_DIRECCION": return DireccionEntregaDefault;

                //Caso: 32380 ABC 14/05/2008 Disponibilizar nuevos datos para reporte 
                case "CLIENTE_TELEFONO": return Telefono;

                //Caso 28269 LDS 14/09/2007
                case "CONDICION": return this.ObtenerCondicionPago();

                case "NIT": return contribuyente;

                default: return string.Empty;
             
            }
        }
        #endregion
    }
}
