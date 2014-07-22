using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Gestor basico para el manejo de facturas,garantías y consignaciones
    /// </summary>
    public abstract class Gestor: IPrintable
    {

        #region Montos Gestion

        /// <summary>
        /// Total Neto por documentos de un cliente multicompañia
        /// Total Bruto + Total Impuestos + Total Descuentos
        /// </summary>
        public decimal TotalNeto
        {            
            get
            {
                return
                    TotalBruto +
                    TotalImpuesto1 + TotalImpuesto2 - TotalDescuentoLineas -
                    TotalDescuento1 - TotalDescuento2-TotalRetenciones;
            }
        }

        /// <summary>
        /// Total Neto por documentos de un cliente multicompañia sin impuestos
        /// Total Bruto + Total Descuentos
        /// </summary>
        public decimal TotalNetoSinImpuesto
        {
            get
            {
                return
                    TotalBruto - TotalDescuentoLineas - TotalDescuento1 - TotalDescuento2-TotalRetenciones;
            }
        }

        private decimal totalImpuesto1 = 0;
        /// <summary>
        /// Total correspondiente al impuestos de ventas para la gestion de documentos del cliente multi cia.
        /// </summary>
        public decimal TotalImpuesto1
        {
            get { return totalImpuesto1; }
            set { totalImpuesto1 = value; }
        }

        private decimal totalImpuesto2 = 0;
        /// <summary>
        /// Total correspondiente al impuesto de consumo para la gestion de documentos del cliente multi cia.
        /// </summary>
        public decimal TotalImpuesto2
        {
            get { return totalImpuesto2; }
            set { totalImpuesto2 = value; }
        }

        private decimal totalBruto = 0;
        /// <summary>
        /// Total bruto para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal TotalBruto
        {
            get { return totalBruto; }
            set { totalBruto = value; }
        }

        private decimal totalRetenciones = 0;
        /// <summary>
        /// Total bruto para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal TotalRetenciones
        {
            get { return totalRetenciones; }
            set { totalRetenciones = value; }
        }


        private decimal totalDescuento = 0;
        /// <summary>
        /// Total de Descuento de todas las lineas para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal TotalDescuentoLineas
        {
            get { return totalDescuento; }
            set { totalDescuento = value; }
        }

        private decimal totalDescuento1 = 0;
        /// <summary>
        /// Total de Descuento 1 para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal TotalDescuento1
        {
            get { return totalDescuento1; }
            set { totalDescuento1 = value; }
        }

        private decimal totalDescuento2 = 0;
        /// <summary>
        /// Total de Descuento 2 para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal TotalDescuento2
        {
            get { return totalDescuento2; }
            set { totalDescuento2 = value; }
        }

        private decimal porcDesc1;
        /// <summary>
        /// Porcentaje de Descuento 1 aplicado para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal PorcDesc1
        {
            get { return porcDesc1; }
            set { porcDesc1 = value; }
        }

        private decimal porcDesc2;
        /// <summary>
        /// Porcentaje de Descuento 2 aplicado para la gestion de documentos del cliente multi cia
        /// </summary>
        public decimal PorcDesc2
        {
            get { return porcDesc2; }
            set { porcDesc2 = value; }
        }

        #endregion

        #region Configuracion

        /* Configuracion generada por el usuario de la PDA para los pedidos de cada una de las companias en ruta
         * La configuracion contiene PAIS,CONDICION DE PAGO,LISTA DE PRECIO,CLASE DE PEDIDO
         * En caso de que no contenga informacion, se debe utilizar los valores por defecto que tenga el cliente,
         * para la compania correspondiente, al cual se le desea realizar el pedido.
         * Lo hacemos privado para evitar que pueda ser modificado desde otro lado que no sea esta clase
         */
        private Hashtable configDocumentoCia = new Hashtable();
        /// <summary>
        /// Configuracion del Cliente Multi cia por cada compañia 
        /// </summary>
        public Hashtable ConfigDocumentoCia
        {
            get { return configDocumentoCia; }
            set { configDocumentoCia = value; }
        }

        #endregion

        #region cliente en gestion

        private ClienteBase cliente;
        /// <summary>
        /// Cliente en la gestion para los reportes
        /// </summary>
        public ClienteBase Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        #endregion 

        #region Logica

        /// <summary>
        /// Metodo encargado de llenar la configuracion del documento para una compania 
        /// en particular. Metodo principalmente usado cuando se desea agregar mas articulos 
        /// a un documento que ya se habia guardado por lo tanto la configuracion del documento
        /// debe ser la misma que aquella con la que la venta habia sido guardado
        /// </summary>
        /// <param name="compania">compania a la que se asocia la configuracion de la venta</param>
        /// <param name="config">configuracion a asociar</param>
        public void CargarConfiguracionVenta(string compania, ConfigDocCia config)
        {
            ConfigDocCia configuracion = (ConfigDocCia)ConfigDocumentoCia[compania];
            
            if (configuracion != null)
                ConfigDocumentoCia.Remove(compania);

            ConfigDocumentoCia.Add(compania, config);
        }

        /// <summary>
        /// Metodo encargado de obtener la configuracion de un pedido para una compania     
        /// </summary>
        /// <param name="compania">Codigo de la compania para el cual se desea obtener la configuracion de pedido</param>
        /// <returns>Retorna la configuracion de la venta para la compania indicada</returns>          
        public ConfigDocCia ObtenerConfiguracionVenta(string compania)
        {
            ConfigDocCia config = null;
            try
            {
                config = (ConfigDocCia)ConfigDocumentoCia[compania];
                if (FRdConfig.UsaEnvases)
                {
                    Garantias.configPedido = config;
                }
            }
            catch
            {
                config = null;
            }
            return config;
        }

        public void AgregarConfiguracion(Cliente clienteActual, string compania)
        {
            CargarConfiguracionVenta(compania, CargarConfiguracionCliente(clienteActual, compania));
        }
        /// <summary>
        /// Carga la configuracion del pedido a partir de la informacion del cliente.
        /// </summary>
        /// <param name="clienteActual">Cliente asociado a la configuracion a cargar</param>
        /// <param name="compania">Compania del cliente a cargar</param>
        /// <returns>La configuracion generada</returns>
        public ConfigDocCia CargarConfiguracionCliente(Cliente clienteActual, string compania)
        {
            //Cargamos los valores del cliente
            Pais pais = new Pais();
            CondicionPago condpago = new CondicionPago();
            NivelPrecio listaPrecio = new NivelPrecio();


            try
            {
                pais.ObtenerPais(compania, clienteActual.Codigo);
            }
            catch
            {
               // Mensaje.mostrarAlerta("Error cargando país del cliente." + ex.Message);
            }

            try
            {
                condpago.ObtenerCondicionPago(compania, clienteActual.Codigo);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Error cargando condición de pago del cliente." + ex.Message);
            }
            try
            {
                listaPrecio = clienteActual.ObtenerClienteCia(compania).NivelPrecio;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //Mensaje.mostrarAlerta("Error cargando nivel de precio del cliente." + ex.Message);
            }

            ConfigDocCia config = new ConfigDocCia(pais, ClaseDoc.Normal, condpago, listaPrecio, Compania.Obtener(compania), clienteActual.ObtenerClienteCia(compania));


            return config;
        }

        /// <summary>
        /// Limpiar los valores globales utilizados durante la gestion de una venta
        /// </summary>
        public void LimpiarValores()
        {
            TotalBruto = 0;
            TotalRetenciones = 0;
            TotalImpuesto1 = 0;
            TotalImpuesto2 = 0;
            TotalDescuentoLineas = 0;
            TotalDescuento1 = 0;
            TotalDescuento2 = 0;
            PorcDesc1 = 0;
            PorcDesc2 = 0;
        }        
        #endregion

        #region IPrintable Members

        public virtual string GetObjectName()
        {
            return "GESTOR";
        }

        public virtual object GetField(string name)
        {
            //En el resumen de pedidos/Consignacion se desea que salga el nombre o codigo del cliente
            //al que se le realizaron los pedidos impresos.
            return Cliente.GetField(name);
        }

        #endregion
    }
}
