using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Encapsula propiedades de la configuracion de un documento que es facturable,asociado a una compania y un cliente en particular
    /// </summary>
    public class ConfigDocCia
    {
        #region Propiedades

        private ClaseDoc clase = ClaseDoc.Normal;
        /// <summary>
        /// la clase de documento
        /// </summary>
        public ClaseDoc Clase
        {
            get { return clase; }
            set { clase = value; }
        }

        private Pais pais = new Pais();
        /// <summary>
        /// Codigo de pais del documento
        /// </summary>
        public Pais Pais
        {
            get { return pais; }
            set { pais = value; }
        }

        private CondicionPago condicionPago = new CondicionPago();
        /// <summary>
        /// Codigo de Condicion de pago del documento
        /// </summary>
        public Softland.ERP.FR.Mobile.Cls.Corporativo.CondicionPago CondicionPago
        {
            get { return condicionPago; }
            set { condicionPago = value; }
        }

        private NivelPrecio nivel = new NivelPrecio();
        /// <summary>
        /// Nivel de precio asociada al cliente.
        /// </summary>
        public NivelPrecio Nivel
        {
            get { return nivel; }
            set { nivel = value; }
        }
        
        private Compania compania = new Compania();
        /// <summary>
        /// Compania asociada a la configuracion
        /// </summary>
        public Compania Compania
        {
              get { return compania; }
              set { compania = value; }
        }

        private ClienteCia cliente = new ClienteCia();
        /// <summary>
        /// Cliente compania asociado a la configuracion
        /// </summary>
        public ClienteCia ClienteCia
        {
            get { return cliente; }
            set { cliente = value; }
        }        

        #endregion 
        
        #region Constructores

        public ConfigDocCia()
        { }

        public ConfigDocCia(Pais pais, ClaseDoc clase, CondicionPago condicionPago, NivelPrecio nivelPrecio, Compania cia, ClienteCia cliente)
        {
            this.clase = clase;
            this.pais= pais;
            this.condicionPago = condicionPago;
            this.Nivel = nivelPrecio;
            this.Compania = cia;
            this.cliente = cliente;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Utilizado para cargar datos completos de la compania, nivel de precio, pais y condicion de pago 
        /// </summary>
        public void Cargar()
        {
            this.Compania = Corporativo.Compania.Obtener(compania.Codigo);
            this.compania.CargarConfiguracionImpuestos();
            pais.Cargar(compania.Codigo);
            condicionPago.Cargar(compania.Codigo);
            nivel.CargarNivelPrecio(compania.Codigo);
        }

        #endregion
    }
}
