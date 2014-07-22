using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SQLiteBase;

using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.ViewModels;
using System.Threading;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
    public class Pedidos : Gestor, IPrintable
    {
        #region Propiedades

        #region Lista de Pedidos

        private List<Pedido> gestionados = new List<Pedido>();
        /// <summary>
        /// Contiene los pedidos por compañía que se estan gestionando.
        /// </summary>
        public List<Pedido> Gestionados
        {
            get { return gestionados; }
            set { gestionados = value; }
        }

        #endregion

        #region Configuracion Globales Config.xml & FRDesktop

        private static bool habilitarDescuento1;
        /// <summary>
        /// Indica si el campo de descuento 1 se puede editar al realizar un pedido.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool HabilitarDescuento1
        {
            get { return Pedidos.habilitarDescuento1; }
            set { Pedidos.habilitarDescuento1 = value; }
        }

        private static bool habilitarDescuento2;
        /// <summary>
        /// Indica si el campo de descuento 2 se puede editar al realizar un pedido.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool HabilitarDescuento2
        {
            get { return Pedidos.habilitarDescuento2; }
            set { Pedidos.habilitarDescuento2 = value; }
        }

        //Caso: 33192 LDA 10/02/2010
        //Agentes pueden hacer Facturas con un 100% de descuento
        private static decimal maximoDescuentoPermitido1;
        /// <summary>
        /// Indica el porcentaje maximo de del descuento numero uno que puede ser aplicado
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static decimal MaximoDescuentoPermitido1
        {
            get { return Pedidos.maximoDescuentoPermitido1; }
            set { Pedidos.maximoDescuentoPermitido1 = value; }
        }

        //Caso: 33192 LDA 10/02/2010
        //Agentes pueden hacer Facturas con un 100% de descuento
        private static decimal maximoDescuentoPermitido2;
        /// <summary>
        /// Indica el porcentaje maximo de del descuento numero dos que puede ser aplicado
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static decimal MaximoDescuentoPermitido2
        {
            get { return Pedidos.maximoDescuentoPermitido2; }
            set { Pedidos.maximoDescuentoPermitido2 = value; }
        }


        private static bool mostrarRefExistencias;
        /// <summary>
        /// Indica si el campo de referencia de existncias se muestra en la ventana de cantidades de pedido.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool MostrarRefExistencias
        {
            get { return Pedidos.mostrarRefExistencias; }
            set { Pedidos.mostrarRefExistencias = value; }
        }

        private static int criterioBusquedaDefault;
        /// <summary>
        /// Indica el indice de búsqueda que se debe utilizar por defecto.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static int CriterioBusquedaDefault
        {
            get 
            {  
                return Pedidos.criterioBusquedaDefault; 
            }
            set { Pedidos.criterioBusquedaDefault = value; }
        }

        public static CriterioArticulo CriterioBusquedaDefaultBD
        {
            get
            {                
                switch (Pedidos.criterioBusquedaDefault)
                {
                    case 0: return CriterioArticulo.Ninguno;
                    case 1: return CriterioArticulo.Codigo;
                    case 2: return CriterioArticulo.Barras;
                    case 3: return CriterioArticulo.Descripcion;
                    case 4: return CriterioArticulo.Familia;
                    case 5: return CriterioArticulo.Clase;
                    case 6: return CriterioArticulo.PedidoActual;
                    case 7: return CriterioArticulo.FacturaActual;
                    case 8: return CriterioArticulo.VentaActual;
                    case 9: return CriterioArticulo.BoletaActual;
                    default: return CriterioArticulo.Codigo;
                }
            }
        }

        private static bool cambiarPrecio;
        //Caso: 28086 LDS 02/05/2007
        /// <summary>
        /// Indica si se puede cambiar el precio o no de un artículo 
        /// mientas se realiza la toma de un pedido o factura.
        /// </summary>
        /// <remarks>
        /// El valor por defecto es NO, que indica que no se puede cambiar el precio. 
        /// SI, permite realizar el cambio de precio del artículo.
        /// </remarks>
        public static bool CambiarPrecio
        {
            get { return Pedidos.cambiarPrecio; }
            set { Pedidos.cambiarPrecio = value; }
        }

        private static bool mostrarUtilidadDefinida;
        //LDS 30/02/2007
        /// <summary>
        /// Indica si se debe mostrar o no la utilidad definida en exactus.
        /// </summary>
        /// <remarks>
        /// El valor por defecto es SI, que indica que se debe mostrar.
        /// NO, por lo contrario indica que no se debe mostrar.
        /// </remarks>
        public static bool MostrarUtilidadDefinida
        {
            get { return Pedidos.mostrarUtilidadDefinida; }
            set { Pedidos.mostrarUtilidadDefinida = value; }
        }

        private static bool mostrarNuevaUtilidad;
        //LDS 30/02/2007
        /// <summary>
        /// Indica si se debe mostrar o no la nueva utilidad, la cual se calcula cuando se cambia el precio.
        /// </summary>
        /// <remarks>
        /// El valor por defecto es SI, que indica que se debe mostrar.
        /// NO, por lo contrario indica que no se debe mostrar.
        /// </remarks>
        public static bool MostrarNuevaUtilidad
        {
            get { return Pedidos.mostrarNuevaUtilidad; }
            set { Pedidos.mostrarNuevaUtilidad = value; }
        }

        private static bool mostrarCostoArticulo;
        //LDS 30/02/2007
        /// <summary>
        /// Indica si se debe mostrar o no el costo del artículo.
        /// </summary>
        /// <remarks>
        /// El valor por defecto es SI, que indica que se debe mostrar.
        /// NO, por lo contrario indica que no se debe mostrar.
        /// </remarks>
        public static bool MostrarCostoArticulo
        {
            get { return Pedidos.mostrarCostoArticulo; }
            set { Pedidos.mostrarCostoArticulo = value; }
        }

        private static int maximoLineasDetalle;
        /// <summary>
        ///Cantidad de líneas máxima permitida por pedido.
        /// </summary>
        public static int MaximoLineasDetalle
        {
            get { return Pedidos.maximoLineasDetalle; }
            set { Pedidos.maximoLineasDetalle = value; }
        }

        private static bool facturarPedido = false;
        /// <summary>
        /// Variable que indica que los pedidos en gestion deben ser facturados.
        /// </summary>
        public static bool FacturarPedido
        {
            get { return Pedidos.facturarPedido; }
            set { Pedidos.facturarPedido = value; }
        }

        private static bool existeBodega = false;

        public static bool ExisteBodega
        {
            get { return Pedidos.existeBodega; }
            set { Pedidos.existeBodega = value; }
        }

        
        private static bool cambiarBonificacion;
        /// <summary>
        /// ABC 35137
        /// </summary>
        public static bool CambiarBonificacion
        {
            get { return Pedidos.cambiarBonificacion; }
            set { Pedidos.cambiarBonificacion = value; }
        }

        private static bool cambiarDescuento;

        public static bool CambiarDescuento
        {
            get { return Pedidos.cambiarDescuento; }
            set { Pedidos.cambiarDescuento = value; }
        }

        //Cesar Iglesias.

        /// <summary>
        /// Verifica si la HandHeld utiliza topes
        /// </summary>
        private static bool manejaTopes;

        public static bool ManejaTopes
        {
            get { return Pedidos.manejaTopes; }
            set { Pedidos.manejaTopes = value; }
        }

        /// <summary>
        /// Contine el valor maximo para el Tope1
        /// </summary>
        private static decimal tope1;

        public static decimal Tope1
        {
            get { return Pedidos.tope1; }
            set { Pedidos.tope1 = value; }
        }
        /// <summary>
        /// Contine el valor maximo para el Tope2
        /// </summary>
        private static decimal tope2;

        public static decimal Tope2
        {
            get { return Pedidos.tope2; }
            set { Pedidos.tope2 = value; }
        }
        
        /// <summary>
        /// Mensaje de advertencia al sobre pasar el valor establecido para el tope 1
        /// </summary>
        private static string msjTope1;

        public static string MsjTope1
        {
            get { return Pedidos.msjTope1; }
            set { Pedidos.msjTope1 = value; }
        }

        /// <summary>
        /// Mensaje de advertencia al sobre pasar el valor establecido para el tope 2
        /// </summary>
        private static string msjTope2;

        public static string MsjTope2
        {
            get { return Pedidos.msjTope2; }
            set { Pedidos.msjTope2 = value; }
        }

        /// <summary>
        /// Adicionar bonificaciones
        /// </summary>
        private static bool bonificacionAdicional;

        /// <summary>
        /// Adicionar Bonificaciones
        /// </summary>
        public static bool BonificacionAdicional
        {
            get { return Pedidos.bonificacionAdicional; }
            set { Pedidos.bonificacionAdicional = value; }
        }

        /// <summary>
        /// Cambio de Teclado criterio de Busqueda Pedido
        /// </summary>
        private static bool cambioTeclado;

        /// <summary>
        /// Cambio teclado
        /// </summary>
        public static bool CambioTeclado
        {
            get { return Pedidos.cambioTeclado; }
            set { Pedidos.cambioTeclado = value; }
        }

        /// <summary>
        /// Teclado defecto
        /// </summary>
        private static EMF.WF.TextBoxInput tecladoDefecto;

        /// <summary>
        /// Cambio teclado
        /// </summary>
        public static EMF.WF.TextBoxInput TecladoDefecto
        {
            get { return Pedidos.tecladoDefecto; }
            set { Pedidos.tecladoDefecto = value; }
        }

        /// <summary>
        /// Toma Automatica
        /// </summary>
        private static bool tomaAutomatica;

        /// <summary>
        /// Cambio teclado
        /// </summary>
        public static bool TomaAutomatica
        {
            get { return Pedidos.tomaAutomatica; }
            set { Pedidos.tomaAutomatica = value; }
        }

        //Cesar Iglesias.

        /// <summary>
        /// SubFiltro busqueda familia
        /// </summary>
        private static bool subfiltrofamilia;

        /// <summary>
        /// SubFiltro busqueda familia
        /// </summary>
        public static bool SubFiltroFamilia
        {
            get { return Pedidos.subfiltrofamilia; }
            set { Pedidos.subfiltrofamilia = value; }
        }

        /// <summary>
        /// SubFiltro busqueda familia
        /// </summary>
        private static bool calcularImpuestos;

        /// <summary>
        /// Calcula impuesto en el total
        /// </summary>
        public static bool CalcularImpuestos
        {
            get { return Pedidos.calcularImpuestos; }
            set { Pedidos.calcularImpuestos = value; }
        }

        private static bool desgloseLotesFactura = false;
        /// <summary>
        /// Variable que indica si debe se debe de hacer el desglose lote de facturas.
        /// </summary>
        public static bool DesgloseLotesFactura
        {
            get { return Pedidos.desgloseLotesFactura; }
            set { Pedidos.desgloseLotesFactura = value; }
        }

        private static bool desgloseLotesFacturaObliga = true;
        /// <summary>
        /// Variable que indica si debe se debe de hacer el desglose lote de facturas.
        /// </summary>
        public static bool DesgloseLotesFacturaObliga
        {
            get { return Pedidos.desgloseLotesFacturaObliga; }
            set { Pedidos.desgloseLotesFacturaObliga = value; }
        }

        private static string modoCambiosPrecios = "A";
        /// <summary>
        /// Variable que indica si permite cambios de precios abierto o hacia arriba.
        /// </summary>
        public static string ModoCambiosPrecios
        {
            get { return Pedidos.modoCambiosPrecios; }
            set { Pedidos.modoCambiosPrecios = value; }
        }

        private static string validarLimiteCredito = "N";
        /// <summary>
        /// Variable que indica si valida limite de credito del cliente
        /// </summary>
        public static string ValidarLimiteCredito
        {
            get { return Pedidos.validarLimiteCredito; }
            set { Pedidos.validarLimiteCredito = value; }
        }

        private static string validarDocVencidos = "P";
        /// <summary>
        /// Variable que indica si valida documentos vencidos o pendientes de cobro
        /// </summary>
        public static string ValidarDocVencidos
        {
            get { return Pedidos.validarDocVencidos; }
            set { Pedidos.validarDocVencidos = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static bool artFueraNivPrecio;
        /// <summary>
        /// Indica si va a ser permitido agregar artículos fuera del nivel de precios seleccionado.      
        /// </summary>
        public static bool ArtFueraNivPrecio
        {
            get { return Pedidos.artFueraNivPrecio; }
            set { Pedidos.artFueraNivPrecio = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static int datoFamiliaMostrar;
        /// <summary>
        /// Indica cual sera el dato de familia(clasificacion) a mostrar en el filtro en toma de pedido.      
        /// </summary>
        public static int DatoFamiliaMostrar
        {
            get { return Pedidos.datoFamiliaMostrar; }
            set { Pedidos.datoFamiliaMostrar = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static string documentoGenerar;
        /// <summary>
        /// Indica cual sera el documento a generar en la toma de pedido (pedido,factura,ambos).      
        /// </summary>
        public static string DocumentoGenerar
        {
            get { return Pedidos.documentoGenerar; }
            set { Pedidos.documentoGenerar = value; }
        }

        #endregion

        #region Constantes Valores

        /// <summary>
        /// Cambio Precio Abierto
        /// </summary>
        public const string CAMBIOPRECIO_ABIERTO = "A";

        /// <summary>
        /// Cambio Precio Hacia Arriba
        /// </summary>
        public const string CAMBIOPRECIO_HACIA_ARRIBA = "U";

        /// <summary>
        /// Limite de Credito Factura
        /// </summary>
        public const string LIMITECREDITO_FACTURA = "F";

        /// <summary>
        /// Limite de Ambos
        /// </summary>
        public const string LIMITECREDITO_AMBOS = "A";

        /// <summary>
        /// Limite No aplica
        /// </summary>
        public const string LIMITECREDITO_NOAPLICA = "N";

        /// <summary>
        /// Documentos vencidos no permite
        /// </summary>
        public const string DOCVENCIDOS_NOPERMITE = "N";

        /// <summary>
        /// Documentos vencidos permite
        /// </summary>
        public const string DOCVENCIDOS_PERMITE = "P";

        /// <summary>
        /// Documentos vencidos advertir
        /// </summary>
        public const string DOCVENCIDOS_ADVERTIR = "A";

        // MejorasGrupoPelon600R6 - KF //
        /// <summary>
        /// Documento a Generar: Pedidos
        /// </summary>
        public const string VALOR_PEDIDO = "P";

        /// <summary>
        /// Documento a Generar: Facturas
        /// </summary>
        public const string VALOR_FACTURA = "F";

        /// <summary>
        /// Documento a Generar: Ambos
        /// </summary>
        public const string VALOR_AMBOS = "A";

        #endregion Constantes Valores

        #endregion

        #region Constructores
        public Pedidos()
        { }
        public Pedidos(List<Pedido> gestionados, Cliente cliente)
        {
            this.gestionados = gestionados;
            this.Cliente = cliente;
        }
        #endregion

        #region Logica Negocios
        
        public bool ExistenArticulosGestionados()
        {
            foreach (Pedido ped in gestionados)
            {
                if (!ped.Detalles.Vacio())
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Elimina una linea de pedido.
        /// </summary>
        /// <param name="articulo"></param>
        public void EliminarDetalle(Articulo articulo)
        {
            this.Gestionar(articulo, string.Empty,new Precio(), 0, 0, false, "");
        }

        /// <summary>
        /// Elimina una linea de pedido.
        /// </summary>
        /// <param name="articulo"></param>
        public void EliminarDetalle(Articulo articulo, bool Adicional)
        {
            if (Adicional)
                this.Gestionar(articulo, string.Empty, new Precio(), -1, -1, 0, 0, false, "");
            else
                this.Gestionar(articulo, string.Empty, new Precio(), 0, 0, false, "");
        }

        /// <summary>
        /// Guarda en la base de datos la informacion de los pedidos.
        /// </summary>
        public void GuardarPedidos()
        {
            foreach (Pedido pedido in gestionados)
                try
                {
                    pedido.Guardar(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando "+(pedido.Tipo == TipoPedido.Factura ? "la factura '" : "el pedido '") + "de la compañía '" + pedido.Compania+ "'. " + ex.Message);
                }
        }

        /// <summary>
        /// Guarda en la base de datos la informacion de los pedidos.
        /// </summary>
        public void GuardarPedidosFactDirerencias()
        {
            foreach (Pedido pedido in gestionados)
                try
                {
                    pedido.GuardarFactDiferencias(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando " + (pedido.Tipo == TipoPedido.Factura ? "la factura '" : "el pedido '") + "de la compañía '" + pedido.Compania + "'. " + ex.Message);
                }
        }

        /// <summary>
        /// Actualiza en la base de datos la informacion de los pedidos.
        /// </summary>
        public void ActualizarPedidos()
        {
            foreach (Pedido pedido in gestionados)
            {
                try
                {
                    pedido.Actualizar(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error actualizando "+(pedido.Tipo == TipoPedido.Factura ? "la factura '" : "el pedido '") + pedido.Numero + "'." + ex.Message);
                }
            }
        }

        /// <summary>
        /// Guarda el descuento general en el dataset y actualiza los montos de descuento.
        /// </summary>
        public decimal DefinirDescuento(EDescuento nombreDescuento, decimal porcDescuento)
        {
            decimal montoPorcDescTot = 0;
            foreach (Pedido pedido in gestionados)
            {
                if (nombreDescuento == EDescuento.DESC1)
                {
                    pedido.PorcentajeDescuento1 = porcDescuento;
                    montoPorcDescTot += pedido.MontoDescuento1;
                }
                else
                {
                    pedido.PorcentajeDescuento2 = porcDescuento;
                    montoPorcDescTot += pedido.MontoDescuento2;
                }
            }
            return montoPorcDescTot;
        }

        /// <summary>
        /// Metodo en el que se define la direccion de entrega para el pedido de la compañía seleccionada
        /// </summary>
        /// <param name="cia"></param>
        /// <param name="dir"></param>
        public void DefineDirEntregaPedido(string compania, DireccionEntrega dir)
        {
            foreach (Pedido pedido in gestionados)
            {
                if (pedido.Compania == compania)
                {
                    pedido.DireccionEntrega = dir.Codigo;
                    break;
                }
            }
        }

        /// <summary>
        /// Obtener las companias de los pedidos en gestion.
        /// </summary>
        /// <returns></returns>
        public List<Compania> ObtenerCompanias()
        {
            List<Compania> companias = new List<Compania>();
            companias.Clear();
            foreach (Pedido pedido in gestionados)
            {
                //if (pedido.Detalles.Count > 0)
                companias.Add(pedido.Configuracion.Compania);
            }
            return companias;
        }

        #region Gestion Pedidos

        /// <summary>
        /// Busca un pedido dentro de los pedidos en gestionados utilizando el código de la compañía.
        /// </summary>
        /// <param name="codigoCompania">Código de compañía del pedido.</param>
        /// <returns>En caso de haber un pedido o factura gestionada para la compañía retorna un objeto Pedido de lo contrario retorna null.</returns>
        public Pedido Buscar(string compania)
        {
            foreach (Pedido pedido in gestionados)
            {
                if (pedido.Compania.Equals(compania))
                    return pedido;
            }
            return null;
        }

        /// <summary>
        /// Borra un pedido o factura en gestión utilizando el código de la compañía.
        /// </summary>
        /// <param name="codigoCompania">Código de compañía del pedido o factura.</param>
        public void Borrar(string compania)
        {
            int pos = -1;
            int cont = 0;

            foreach (Pedido pedido in gestionados)
            {
                if (pedido.Compania.Equals(compania))
                {
                    pos = cont;
                    break;
                }

                cont++;
            }

            if (pos > -1)
                gestionados.RemoveAt(pos);
            //ABC Caso: 32892 12/06/2008
            else 
                throw new Exception("No se pudo eliminar el pedido o factura gestionada en la compañía '" + compania + "'.");
        }

        /// <summary>
        /// Busca una linea dentro de los detalles de los pedidos gestionados.
        /// </summary>
        /// <param name="articulo">Articulo a buscar.</param>
        /// <returns></returns>
        public DetallePedido BuscarDetalle(Articulo articulo)
        {
            foreach (Pedido pedido in gestionados)
            {
                foreach (DetallePedido detalle in pedido.Detalles.Lista)
                {
                    if (detalle.Articulo.Codigo == articulo.Codigo && detalle.Articulo.Compania == articulo.Compania)                                              
                        return detalle;
                }
            }
            return null;
        }

        /// <summary>
        /// Busca el nivel de precio de un articulo dentro de un pedido
        /// //LDA R0-02009-S00S
        /// </summary>
        /// <param name="articulo">Articulo a buscar.</param>
        /// <returns></returns>
        public Pedido BuscarNivelPrecio(Articulo articulo)
        {
            foreach (Pedido pedido in gestionados)
            {
                foreach (DetallePedido detalle in pedido.Detalles.Lista)
                {
                    if (detalle.Articulo.Codigo == articulo.Codigo && detalle.Articulo.Compania == articulo.Compania)
                        return pedido;
                }
            }
            return null;
        }
        /// <summary>
        /// Metodo que carga los consecutivos iniciales de los encabezados de los pedidos
        /// </summary>		
        public void CargarConsecutivos()
        {
            foreach (Pedido pedido in gestionados)
            {
                //se verifica que el encabezado tenga líneas de detalle.
                if (!pedido.Detalles.Vacio())
                {
                    string consecutivo = string.Empty;
                    string ncf = string.Empty;

                    if (Pedidos.FacturarPedido)
                    {
                        //Verifica si tiene resolucion para cambiar el numero del pedido
                        if (ResolucionUtilitario.usaResolucionFactura(pedido.Zona))
                        {
                            consecutivo = pedido.ConsecResolucion.ValorResolucion;
                        }
                        else
                        {
                            consecutivo = ParametroSistema.ObtenerFactura(pedido.Compania, pedido.Zona);
                        }

                        if (pedido.Tipo == TipoPedido.Factura && pedido.Configuracion.Compania.UsaNCF && pedido.Configuracion.ClienteCia.TipoContribuyente != string.Empty)
                            pedido.NCF.cargarNuevoNCF();
                    }
                    else
                    {
                        if (pedido.UsuarioCambioDescuentos())
                            consecutivo = ParametroSistema.ObtenerPedidoDesc(pedido.Compania, pedido.Zona);
                        else
                            consecutivo = ParametroSistema.ObtenerPedido(pedido.Compania, pedido.Zona);
                    }
                    //Caso 29861 LDS 01/10/2007
                    //Se captura el mensaje.
                    if (consecutivo == string.Empty)
                        throw new Exception("Error obteniendo consecutivo");

                    //ABC 09/10/2008 Caso:33376A
                    //Se incluye la compañia como nuevo parametro 
                    //en el procedimiento de validacion de exitencia de consecutivo
                    if (!ResolucionUtilitario.usaResolucionFactura(pedido.Zona))
                    {
                        consecutivo = pedido.ValidarExistenciaConsecutivo(consecutivo);
                    }
                    pedido.Numero = consecutivo;
                }
            }
        }
    
        #endregion

        #region Gestion de los articulos

        /// <summary>
        /// Obtiene los montos total del pedido para los pedidos en una lista.
        /// Verifica si el cliente se excedio en credito en  alguna de las companias.
        /// </summary>
        /// <returns>Indicacion de si el cliente esta excedido en credido para alguna de las companias.</returns>
        public bool SacarMontosTotales()
        {
            LimpiarValores();

            bool creditoExcedido = false;

            foreach (Pedido pedido in gestionados)
            {
                TotalImpuesto1 += pedido.Impuesto.MontoImpuesto1;

                //Utiliza Percepción
                if (pedido.Configuracion.Compania.UtilizaMinimoExento)
                    pedido.RecalcularImpuestos(pedido.MontoSubTotal < pedido.Configuracion.Compania.MontoMinimoExcento);               

                TotalImpuesto2 += pedido.Impuesto.MontoImpuesto2;
                TotalDescuentoLineas += pedido.MontoDescuentoLineas;
                TotalBruto += pedido.MontoBruto;
                TotalRetenciones += pedido.inTotalRetenciones;
                TotalDescuento1 += pedido.MontoDescuento1;
                TotalDescuento2 += pedido.MontoDescuento2;
                PorcDesc1 = pedido.PorcentajeDescuento1;
                PorcDesc2 = pedido.PorcentajeDescuento2;
                /*
                //ABC Caso 34622 20-01-2009
                if (pedido.Configuracion.ClienteCia.LimiteCredito > 0 && pedido.MontoNeto > pedido.Configuracion.ClienteCia.LimiteCredito)
                {
                    //Se excedió el crédito para la compañía específica.
                    creditoExcedido = true;
                }
                */
            }
            return creditoExcedido;
        }
      

        //Caso 28086 LDS 04/05/2007  
        /// <summary>
        /// Metodo encargado de realizar la gestion del pedido y la gestion sus lineas de detalle
        /// </summary>
        /// <param name="articulo">articulo inicial del pedido</param>
        /// <param name="zona">Zona a la que se asocia el pedido</param>
        /// <param name="precio">Precio del articulo a gestionar</param>
        /// <param name="cantidadAlmacen">Cantidad de almacen a ingresar del articulo</param>
        /// <param name="cantidadDetalle">Cantidad de detalle a ingresar del articulo</param>
        /// <param name="validarCantidadLineas"></param>
        public void Gestionar(Articulo articulo, string zona, Precio precio,
            decimal cantidadAlmacen,decimal cantidadDetalle,decimal cantidadAlmacenAdicional, decimal cantidadDetalleAdicional,
            bool validarCantidadLineas, string tope)
        {
            Pedido pedido = Buscar(articulo.Compania);

            if (pedido == null)//se crea un nuevo pedido
            {
                //Si la cantidad es 0 ignore
                if (cantidadDetalle == 0 && cantidadAlmacen == 0)
                    return;
                
                pedido = new Pedido(articulo, zona, ObtenerConfiguracionVenta(articulo.Compania));

                pedido.Configuracion.Cargar();

                if (Pedidos.BonificacionAdicional && !Pedidos.FacturarPedido)
                    pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, 
                                                cantidadAlmacenAdicional, cantidadDetalleAdicional, tope);
                else
                    pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);

                gestionados.Add(pedido);
            }
            else
            {
                //El pedido ya esta creado. Se procede a modificar el detalle y el encabezado
                if (cantidadAlmacen == 0 && cantidadDetalle == 0)
                {
                    pedido.EliminarDetalle(articulo.Codigo);
                    if (pedido.Detalles.Vacio())
                        Borrar(pedido.Compania);
                }
                else
                {
                    if (Pedidos.BonificacionAdicional && !Pedidos.FacturarPedido)
                        pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas,
                                                    cantidadAlmacenAdicional, cantidadDetalleAdicional, tope);
                    else
                        pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);
                }
            }
            SacarMontosTotales();
        }

        //Caso 28086 LDS 04/05/2007  
        /// <summary>
        /// Metodo encargado de realizar la gestion del pedido y la gestion sus lineas de detalle
        /// </summary>
        /// <param name="articulo">articulo inicial del pedido</param>
        /// <param name="zona">Zona a la que se asocia el pedido</param>
        /// <param name="precio">Precio del articulo a gestionar</param>
        /// <param name="cantidadAlmacen">Cantidad de almacen a ingresar del articulo</param>
        /// <param name="cantidadDetalle">Cantidad de detalle a ingresar del articulo</param>
        /// <param name="validarCantidadLineas"></param>
        public void GestionarTomaFisica(Articulo articulo, string zona, Precio precio,
            decimal cantidadAlmacen, decimal cantidadDetalle, decimal cantidadAlmacenAdicional, decimal cantidadDetalleAdicional,
            bool validarCantidadLineas, string tope)
        {
            Pedido pedido = Buscar(articulo.Compania);

            if (pedido == null)//se crea un nuevo pedido
            {
                //Si la cantidad es 0 ignore
                if (cantidadDetalle == 0 && cantidadAlmacen == 0)
                    return;

                pedido = new Pedido(articulo, zona, ObtenerConfiguracionVenta(articulo.Compania));

                pedido.Configuracion.Cargar();

                if (Pedidos.BonificacionAdicional && !Pedidos.FacturarPedido)
                    pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas,
                                                cantidadAlmacenAdicional, cantidadDetalleAdicional, tope);
                else
                    pedido.AgregarLineaDetalleTomaFisica(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);

                gestionados.Add(pedido);
            }
            else
            {
                //El pedido ya esta creado. Se procede a modificar el detalle y el encabezado
                if (cantidadAlmacen == 0 && cantidadDetalle == 0)
                {
                    pedido.EliminarDetalle(articulo.Codigo);
                    if (pedido.Detalles.Vacio())
                        Borrar(pedido.Compania);
                }
                else
                {
                    if (Pedidos.BonificacionAdicional && !Pedidos.FacturarPedido)
                        pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas,
                                                    cantidadAlmacenAdicional, cantidadDetalleAdicional, tope);
                    else
                        pedido.AgregarLineaDetalleTomaFisica(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);
                }
            }
            SacarMontosTotales();
        }

        public void Gestionar(Articulo articulo, string zona, Precio precio,
            decimal cantidadAlmacen, decimal cantidadDetalle, bool validarCantidadLineas, string tope)
        {
            this.Gestionar(articulo, zona, precio, cantidadAlmacen, cantidadDetalle, 0, 0, validarCantidadLineas, tope);
        }

        public void GestionarTomaFisica(Articulo articulo, string zona, Precio precio,
            decimal cantidadAlmacen, decimal cantidadDetalle, bool validarCantidadLineas, string tope)
        {
            this.GestionarTomaFisica(articulo, zona, precio, cantidadAlmacen, cantidadDetalle, 0, 0, validarCantidadLineas, tope);
        }

        /// <summary>
        /// Se realiza la gestión de la factura a generar por el desglose de la boleta de venta en consignación.
        /// </summary>
        /// <param name="consignacion">Numero de la consignacion asociada al pedido</param>
        /// <param name="articulo">Corresponde al artículo que se agregará como detalle de la factura.</param>
        /// <param name="zona">Zona a la que se asocia el pedido</param>
        /// <param name="precio">Precio del articulo a gestionar</param>
        /// <param name="cantidadAlmacen">Cantidad de almacen a ingresar del articulo</param>
        /// <param name="cantidadDetalle">Cantidad de detalle a ingresar del articulo</param>
        /// <param name="descuentoCascada">Indica si se debe aplicar el descuento2 en cascada. Esta información se obtiene de la boleta de venta en consignación.</param>
        /// <param name="porcDesc1">Porcentaje de descuento 1 definido en la venta en consignación.</param>
        /// <param name="porcDesc2">Porcentaje de descuento 2 definido en la venta en consignación.</param>
        /// <param name="config">Corresponde a la nota que se le debe asignar a la factura generada por el desglose de la venta en consignación.</param>
        /// <param name="nota">Indica si debe validar la cantidad máxima de líneas de detalle con la cual se puede generar la factura.</param>
        /// <param name="validarCantidadLineas"></param>
        public void GestionarConsignado(string consignacion, Articulo articulo, string zona, Precio precio,
            decimal cantidadAlmacen, decimal cantidadDetalle,
            bool descuentoCascada,decimal porcDesc1,decimal porcDesc2,
            ConfigDocCia config,string nota,bool validarCantidadLineas, string tope)
        {
            Pedido pedido = Buscar(articulo.Compania);

            if (pedido == null)//se crea un nuevo pedido
            {
                pedido = Pedido.Consignado(consignacion,articulo, zona,config,descuentoCascada,porcDesc1,porcDesc2,nota);

                //Caso 28086 LDS 04/05/2007 AgregarLineaDetalle puede levantar la excepcion de que la cantidad de lineas de detalle excede el maximo permitido
                pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);
                gestionados.Add(pedido);
            }
            else
            {
                //Caso 28086 LDS 04/05/2007
                pedido.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);
            }
            SacarMontosTotales();
        }

        #endregion

        #endregion

        #region Impresion de Pedidos

        #region Impresion de resumen de Pedidos

        /// <summary>
        /// Imprime el resumen de pedidos/facturas realizados a un cliente.
        /// </summary>
        /// <param name="pedidos">Lista de pedidos a imprimir</param>
        /// <param name="cliente">Cliente al que se le realizaron los pedidos.</param>
        public void ImprimeResumen(TipoPedido tipo,BaseViewModel viewModel)
        {

            Report resumenPedidos;
            if(tipo==TipoPedido.Factura)
                resumenPedidos = new Report(ReportHelper.CrearRutaReporte(Rdl.ResumenFacturas), Impresora.ObtenerDriver());
            else
                resumenPedidos = new Report(ReportHelper.CrearRutaReporte(Rdl.ResumenPedidos), Impresora.ObtenerDriver());

            resumenPedidos.AddObject(this);
            ImprimeResumen(tipo, viewModel, resumenPedidos);
        }

        private void ImprimeResumen(TipoPedido tipo, BaseViewModel viewModel, Report resumenPedidos)
        {
            resumenPedidos.Print();
            if (resumenPedidos.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión de "+(tipo==TipoPedido.Factura? "la factura" : "el pedido")+ resumenPedidos.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        ImprimeResumen(tipo, viewModel, resumenPedidos);
                    }
                });
        }     

        #endregion

        #region Impresión de Detalle del Pedido

        //Caso 25452 LDS 24/10/2007
        /// <summary>
        /// Imprime los pedidos/facturas con sus respectivos detalles.
        /// </summary>
        /// <param name="pedidos">
        /// Contiene los pedidos/facturas a imprimir.
        /// </param>
        /// <param name="cantidadCopias">
        /// Indica la cantidad de copias que se requiere imprimir de los pedidos/facturas.
        /// </param>	
        /// <param name="criterio">criterio de ordenamiento de los detalles</param> 
        public void ImprimeDetallePedido( int cantidadCopias, DetalleSort.Ordenador criterio)
        {
            Report reportePedido = new Report(ReportHelper.CrearRutaReporte(Rdl.Pedido), Impresora.ObtenerDriver());
            string imprimirTodo = string.Empty;
            foreach (Pedido pedido in gestionados)
            {

                // LJR 20/02/09 Caso 34848 Ordenar articulos de la factura
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    pedido.Detalles.Lista.Sort(new DetalleSort(criterio));
            }
            reportePedido.AddObject(this);
            int plus = 1;
            if (Gestionados[0].Impreso)
            {
                plus = 0;
            }

            for (int i = 1; i <= (cantidadCopias + plus); i++)
            {
                //reportePedido.Print();                               
                reportePedido.PrintAll(ref imprimirTodo);
                imprimirTodo += "\n\n";
                //if (reportePedido.ErrorLog != string.Empty)
                    //throw new Exception("Ocurrió un error durante la impresión del pedido: " + reportePedido.ErrorLog);
            }

            foreach (Pedido pedido in gestionados)
            {
                if (!pedido.Impreso)
                {
                    pedido.Impreso = true;
                    pedido.DBActualizarImpresion();
                }
            }
            //Imprime todo a la vez
            reportePedido.PrintText(imprimirTodo);
            reportePedido = null;
        }
        //Caso 25452 LDS 22/10/2007
        /// <summary>
        /// Imprime las facturas con sus respectivos detalles.
        /// </summary>
        /// <param name="facturas">
        /// Contiene las facturas a imprimir.
        /// </param>
        /// <param name="cantidadCopias">
        /// Indica la cantidad de copias que se requiere imprimir de las facturas.
        /// <param name="criterio">criterio de ordenamiento de los detalles</param>
        public void ImprimeDetalleFactura( int cantidadCopias, DetalleSort.Ordenador criterio,string textoGarantia)
        {            
            //Imprime el original
            bool imprimirOriginal = false;
            string ImprimirTodo = string.Empty;

            foreach (Pedido pedido in gestionados)
            {
                // LJR 20/02/09 Caso 34848 Ordenar articulos de la factura
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    pedido.Detalles.Lista.Sort(new DetalleSort(criterio));
                if (pedido.LeyendaOriginal && !pedido.Impreso)
                {
                    imprimirOriginal = true;
                    break;
                }
            }
            //Contiene las facturas que ya han sido impresas
            ArrayList facturasImpresas = new ArrayList();

            if (imprimirOriginal)
            {
                Report reporteFactura;

                foreach (Pedido pedido in gestionados)
                    if (pedido.Impreso)
                        facturasImpresas.Add(pedido);                

                reporteFactura = new Report(ReportHelper.CrearRutaReporte(Rdl.Factura), Impresora.ObtenerDriver());

                reporteFactura.AddObject(this);

                //reporteFactura.Print();                
                reporteFactura.PrintAll(ref ImprimirTodo);
                ImprimirTodo += "\n\n";

                if (reporteFactura.ErrorLog != string.Empty)
                    throw new Exception("Ocurrió un error durante la impresión de la factura: " + reporteFactura.ErrorLog);

                reporteFactura = null;

                foreach (Pedido pedido in gestionados)
                    if (!pedido.Impreso && pedido.LeyendaOriginal)
                    {
                        pedido.Impreso = true;
                        pedido.LeyendaOriginal = false;
                        pedido.DBActualizarImpresion();
                    }

                for (int indice = facturasImpresas.Count; indice > 0; indice--)
                    gestionados.Remove((Pedido)facturasImpresas[indice - 1]);

                reporteFactura = null;
            }

            //Imprime las copias con la leyenda de copia
            if (cantidadCopias > 0)
            {
                Report reporteFactura;

                foreach (Pedido pedido in facturasImpresas)
                    gestionados.Add(pedido);

                reporteFactura = new Report(ReportHelper.CrearRutaReporte(Rdl.Factura), Impresora.ObtenerDriver());

                reporteFactura.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteFactura.Print();
                    reporteFactura.PrintAll(ref ImprimirTodo );
                    ImprimirTodo += "\n\n";

                    if (reporteFactura.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de la factura: " + reporteFactura.ErrorLog);
                }

                //Intenta imprimir todos los documentos
                if (!string.IsNullOrEmpty(textoGarantia))
                {
                    ImprimirTodo += textoGarantia;
                }
                reporteFactura.PrintText(ImprimirTodo);
                
                reporteFactura = null;
            }
        }

        /// <summary>
        /// Imprime las facturas con sus respectivos detalles.
        /// </summary>
        /// <param name="facturas">
        /// Contiene las facturas a imprimir.
        /// </param>
        /// <param name="cantidadCopias">
        /// Indica la cantidad de copias que se requiere imprimir de las facturas.
        /// <param name="criterio">criterio de ordenamiento de los detalles</param>
        public void ImprimeDetalleFactura(int cantidadCopias, DetalleSort.Ordenador criterio)
        {
            //Imprime el original
            bool imprimirOriginal = false;
            string ImprimirTodo = string.Empty;

            foreach (Pedido pedido in gestionados)
            {
                // LJR 20/02/09 Caso 34848 Ordenar articulos de la factura
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    pedido.Detalles.Lista.Sort(new DetalleSort(criterio));
                if (pedido.LeyendaOriginal && !pedido.Impreso)
                {
                    imprimirOriginal = true;
                    break;
                }
            }
            //Contiene las facturas que ya han sido impresas
            ArrayList facturasImpresas = new ArrayList();

            if (imprimirOriginal)
            {
                Report reporteFactura;

                foreach (Pedido pedido in gestionados)
                    if (pedido.Impreso)
                        facturasImpresas.Add(pedido);

                reporteFactura = new Report(ReportHelper.CrearRutaReporte(Rdl.Factura), Impresora.ObtenerDriver());

                reporteFactura.AddObject(this);

                //reporteFactura.Print();                
                reporteFactura.PrintAll(ref ImprimirTodo);
                ImprimirTodo += "\n\n";

                if (reporteFactura.ErrorLog != string.Empty)
                    throw new Exception("Ocurrió un error durante la impresión de la factura: " + reporteFactura.ErrorLog);

                reporteFactura = null;

                foreach (Pedido pedido in gestionados)
                    if (!pedido.Impreso && pedido.LeyendaOriginal)
                    {
                        pedido.Impreso = true;
                        pedido.LeyendaOriginal = false;
                        pedido.DBActualizarImpresion();
                    }

                for (int indice = facturasImpresas.Count; indice > 0; indice--)
                    gestionados.Remove((Pedido)facturasImpresas[indice - 1]);

                reporteFactura = null;
            }

            //Imprime las copias con la leyenda de copia
            if (cantidadCopias > 0)
            {
                Report reporteFactura;

                foreach (Pedido pedido in facturasImpresas)
                    gestionados.Add(pedido);

                reporteFactura = new Report(ReportHelper.CrearRutaReporte(Rdl.Factura), Impresora.ObtenerDriver());

                reporteFactura.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteFactura.Print();
                    reporteFactura.PrintAll(ref ImprimirTodo);
                    ImprimirTodo += "\n\n";

                    if (reporteFactura.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de la factura: " + reporteFactura.ErrorLog);
                }

                //Intenta imprimir todos los documentos
                reporteFactura.PrintText(ImprimirTodo);

                reporteFactura = null;
            }
        }

        #endregion

        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "PEDIDOS";
        }

        public override object GetField(string name)
        {
            if (name == "LISTA_PEDIDOS")
            {
                return new ArrayList(gestionados);
            }
            else
                return base.GetField(name);
        }

        #endregion


    }
}
