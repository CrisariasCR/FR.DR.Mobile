using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Softland.ERP.FR.Mobile.Cls
{
    public class FRmConfig
    {
        public const string EMMCarga = "FRmcarga";
        public const string EMMDescarga = "FRmdescarga";
        public const string EMMVerificar = "FRaverificar";
        public const string EMMPurga = "FRmpurga";
        public const string EMMCrear = "FRmcrear";
        public const string EMMPrueba = "FRmprueba";
        public static bool EnConsulta = false;
        public static string NombreHandHeld = string.Empty;
        public static string Impresora = string.Empty;
        public static double TamañoPapel = 2.5;
        public static bool GuardarenSD = false;

#if DEBUG // mvega: mientras no haya pantalla de login y mientras trabajamos desde el sdcard
        //private static string fullAppPath = "/sdcard";
        private static string fullAppPath = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
#else
        private static string fullAppPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#endif


        public static string FullAppPath
        {
            get { return fullAppPath; }
            set { fullAppPath = value;}
        }
        /// <summary>
        /// Caractér que utiliza FR como sufijo de los códigos de barras leidos con 
        /// un lector.
        /// </summary>
        public const string CaracterDeRetorno = "/";

        #region Variables de Configuracion Config.xml

        /// <summary>
        /// Indica si la seleccion de clientes se puede realizar leyendo el codigo de barras.
        /// </summary>
        public static bool SeleccionXCodBarras = false;
        
        /// <summary>
        ///Caso: 34430 LJR 18/12/2008
        /// Indica si se permite la búsqueda ágil de clientes.
        /// </summary>
        public static bool BusquedaAgilCliente = false;

        /// <summary>
        /// Mensaje personalizado que se debe desplegar cuando el cliente se excede del
        /// limite de credito asignado.
        /// </summary>
        /// <remarks>
        /// Este mensaje se extrae de Config.xml ya que el cliente desea 
        /// que este mensaje sea personalizado.
        /// Este mensaje aparece cuando el monto de un pedido se excede el credito
        /// permitido para el cliente al cual se le esta realizando el pedido.
        /// </remarks>
        public static string MensajeCreditoExcedido = string.Empty;
        /// <summary>
        /// Mensaje personalizado que se debe desplegar cuando el cliente se excede del
        /// limite de credito asignado.
        /// </summary>
        /// <remarks>
        /// Este mensaje se extrae de Config.xml ya que el cliente desea 
        /// que este mensaje sea personalizado.
        /// Este mensaje aparece cuando el cliente tiene facturas vencidas
        /// </remarks>
        public static string MensajeFacturasVencidas = string.Empty;

        /// <summary>
        /// Cantidad de decimales permitido para las cantidades de articulo. 
        /// Variables cargada de Config.xml
        /// </summary>
        public static int CantidadDecimales;

        /// <summary>
        /// Indica si se le permite a la HH actualiza mediante el Actualizador
        /// </summary>
        public static string PermiteActualizar = String.Empty;

        /// <summary>
        /// Ordena por criterio de busqueda
        /// </summary>
        public static bool OrdenarCriterio = false;

        /// <summary>
        /// Ordena por indice de orden del articulo
        /// </summary>
        public static bool OrdenarIndiceArticulo = false;

        /// <summary>
        /// Indica si se debe purgar los documentos automáticamente.
        /// </summary>
        public static bool PurgarDocumentosAutomaticamente = false;

        /// <summary>
        /// Cantidad de dias que se matendran los documentos activos.
        /// </summary>
        public static int PurgarCantidadDias = 1;

        /// <summary>
        /// Indica si se permite purgar manualmente los documentos.
        /// </summary>
        public static bool PugarDocumentosManualmente = false;

        /// <summary>
        /// Indica si el sistema debe guardar la posición de los clientes.
        /// </summary>
        public static bool GuardarUbicacionClientes = false;

        /// <summary>
        /// Indica si debe guardar la posición de cada visita.
        /// </summary>
        public static bool GuardarUbicacionVisita = false;

        /// <summary>
        /// Indica si el sistema debe actualizar la posición de los clientes.
        /// </summary>
        public static bool ActualizarUbicacionClientes = false;

        /// <summary>
        /// Indica la máxima distancia permitida entre la ubiación del cliente y el de la visita
        /// </summary>
        public static decimal DistanciaPermitidaUbicacion = 0;

        /// <summary>
        /// Indica si el proceso de obtención de la posición se debe detener.
        /// </summary>
        public static bool NoDetenerGPS = false;

        #region MejorasFRTostadoraElDorado600R6 JEV
        public static string ClienteRutero = string.Empty;
        #endregion MejorasFRTostadoraElDorado600R6 JEV

        #region  Facturas de contado y recibos en FR - KFC

        //Constantes Recibo Factura Contado
        public const string ObligarDesglose = "O";
        public const string Consultar = "C";
        public const string GenerarEfectivo = "N";

        //Constantes Tipo Pago Devolucion
        public const string TipoPagoCredito = "C";
        public const string TipoPagoEfectivo = "E";
        public const string TipoPagoAmbos = "A";

        //Constantes Aplicar Notas Crédito
        public const string SeleccionNotasCredito = "S";
        public const string AplicacionAutomatica = "A";
        public const string NoAplicaNotasCredito = "N";

        #endregion


        #endregion
    }
}
