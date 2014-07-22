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

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
    public class Garantias : Gestor, IPrintable
    {
        #region Propiedades

        #region Lista de Garantias

        private List<Garantia> gestionados = new List<Garantia>();
        /// <summary>
        /// Contiene los Garantias por compañía que se estan gestionando.
        /// </summary>
        public List<Garantia> Gestionados
        {
            get { return gestionados; }
            set { gestionados = value; }
        }

        #endregion

        #region Configuracion Globales Config.xml & FRDesktop

        public static ConfigDocCia configPedido;

        private static bool habilitarDescuento1;
        /// <summary>
        /// Indica si el campo de descuento 1 se puede editar al realizar un pedido.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool HabilitarDescuento1
        {
            get { return Garantias.habilitarDescuento1; }
            set { Garantias.habilitarDescuento1 = value; }
        }

        private static bool habilitarDescuento2;
        /// <summary>
        /// Indica si el campo de descuento 2 se puede editar al realizar un pedido.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool HabilitarDescuento2
        {
            get { return Garantias.habilitarDescuento2; }
            set { Garantias.habilitarDescuento2 = value; }
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
            get { return Garantias.maximoDescuentoPermitido1; }
            set { Garantias.maximoDescuentoPermitido1 = value; }
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
            get { return Garantias.maximoDescuentoPermitido2; }
            set { Garantias.maximoDescuentoPermitido2 = value; }
        }


        private static bool mostrarRefExistencias;
        /// <summary>
        /// Indica si el campo de referencia de existncias se muestra en la ventana de cantidades de pedido.
        /// Variable cargada desde Config.xml.
        /// </summary>
        public static bool MostrarRefExistencias
        {
            get { return Garantias.mostrarRefExistencias; }
            set { Garantias.mostrarRefExistencias = value; }
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
                return Garantias.criterioBusquedaDefault; 
            }
            set { Garantias.criterioBusquedaDefault = value; }
        }

        public static CriterioArticulo CriterioBusquedaDefaultBD
        {
            get
            {                
                switch (Garantias.criterioBusquedaDefault)
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
            get { return Garantias.cambiarPrecio; }
            set { Garantias.cambiarPrecio = value; }
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
            get { return Garantias.mostrarUtilidadDefinida; }
            set { Garantias.mostrarUtilidadDefinida = value; }
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
            get { return Garantias.mostrarNuevaUtilidad; }
            set { Garantias.mostrarNuevaUtilidad = value; }
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
            get { return Garantias.mostrarCostoArticulo; }
            set { Garantias.mostrarCostoArticulo = value; }
        }

        private static int maximoLineasDetalle;
        /// <summary>
        ///Cantidad de líneas máxima permitida por pedido.
        /// </summary>
        public static int MaximoLineasDetalle
        {
            get { return Garantias.maximoLineasDetalle; }
            set { Garantias.maximoLineasDetalle = value; }
        }

        private static bool facturarPedido = false;
        /// <summary>
        /// Variable que indica que los Garantias en gestion deben ser facturados.
        /// </summary>
        public static bool FacturarGarantia
        {
            get { return Garantias.facturarPedido; }
            set { Garantias.facturarPedido = value; }
        }

        private static bool existeBodega = false;

        public static bool ExisteBodega
        {
            get { return Garantias.existeBodega; }
            set { Garantias.existeBodega = value; }
        }

        
        private static bool cambiarBonificacion;
        /// <summary>
        /// ABC 35137
        /// </summary>
        public static bool CambiarBonificacion
        {
            get { return Garantias.cambiarBonificacion; }
            set { Garantias.cambiarBonificacion = value; }
        }

        private static bool cambiarDescuento;

        public static bool CambiarDescuento
        {
            get { return Garantias.cambiarDescuento; }
            set { Garantias.cambiarDescuento = value; }
        }

        //Cesar Iglesias.

        /// <summary>
        /// Verifica si la HandHeld utiliza topes
        /// </summary>
        private static bool manejaTopes;

        public static bool ManejaTopes
        {
            get { return Garantias.manejaTopes; }
            set { Garantias.manejaTopes = value; }
        }

        /// <summary>
        /// Contine el valor maximo para el Tope1
        /// </summary>
        private static decimal tope1;

        public static decimal Tope1
        {
            get { return Garantias.tope1; }
            set { Garantias.tope1 = value; }
        }
        /// <summary>
        /// Contine el valor maximo para el Tope2
        /// </summary>
        private static decimal tope2;

        public static decimal Tope2
        {
            get { return Garantias.tope2; }
            set { Garantias.tope2 = value; }
        }
        
        /// <summary>
        /// Mensaje de advertencia al sobre pasar el valor establecido para el tope 1
        /// </summary>
        private static string msjTope1;

        public static string MsjTope1
        {
            get { return Garantias.msjTope1; }
            set { Garantias.msjTope1 = value; }
        }

        /// <summary>
        /// Mensaje de advertencia al sobre pasar el valor establecido para el tope 2
        /// </summary>
        private static string msjTope2;

        public static string MsjTope2
        {
            get { return Garantias.msjTope2; }
            set { Garantias.msjTope2 = value; }
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
            get { return Garantias.bonificacionAdicional; }
            set { Garantias.bonificacionAdicional = value; }
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
            get { return Garantias.cambioTeclado; }
            set { Garantias.cambioTeclado = value; }
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
            get { return Garantias.tecladoDefecto; }
            set { Garantias.tecladoDefecto = value; }
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
            get { return Garantias.tomaAutomatica; }
            set { Garantias.tomaAutomatica = value; }
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
            get { return Garantias.subfiltrofamilia; }
            set { Garantias.subfiltrofamilia = value; }
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
            get { return Garantias.calcularImpuestos; }
            set { Garantias.calcularImpuestos = value; }
        }

        private static bool desgloseLotesFactura = false;
        /// <summary>
        /// Variable que indica si debe se debe de hacer el desglose lote de facturas.
        /// </summary>
        public static bool DesgloseLotesFactura
        {
            get { return Garantias.desgloseLotesFactura; }
            set { Garantias.desgloseLotesFactura = value; }
        }

        private static bool desgloseLotesFacturaObliga = true;
        /// <summary>
        /// Variable que indica si debe se debe de hacer el desglose lote de facturas.
        /// </summary>
        public static bool DesgloseLotesFacturaObliga
        {
            get { return Garantias.desgloseLotesFacturaObliga; }
            set { Garantias.desgloseLotesFacturaObliga = value; }
        }

        private static string modoCambiosPrecios = "A";
        /// <summary>
        /// Variable que indica si permite cambios de precios abierto o hacia arriba.
        /// </summary>
        public static string ModoCambiosPrecios
        {
            get { return Garantias.modoCambiosPrecios; }
            set { Garantias.modoCambiosPrecios = value; }
        }

        private static string validarLimiteCredito = "N";
        /// <summary>
        /// Variable que indica si valida limite de credito del cliente
        /// </summary>
        public static string ValidarLimiteCredito
        {
            get { return Garantias.validarLimiteCredito; }
            set { Garantias.validarLimiteCredito = value; }
        }

        private static string validarDocVencidos = "P";
        /// <summary>
        /// Variable que indica si valida documentos vencidos o pendientes de cobro
        /// </summary>
        public static string ValidarDocVencidos
        {
            get { return Garantias.validarDocVencidos; }
            set { Garantias.validarDocVencidos = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static bool artFueraNivPrecio;
        /// <summary>
        /// Indica si va a ser permitido agregar artículos fuera del nivel de precios seleccionado.      
        /// </summary>
        public static bool ArtFueraNivPrecio
        {
            get { return Garantias.artFueraNivPrecio; }
            set { Garantias.artFueraNivPrecio = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static int datoFamiliaMostrar;
        /// <summary>
        /// Indica cual sera el dato de familia(clasificacion) a mostrar en el filtro en toma de pedido.      
        /// </summary>
        public static int DatoFamiliaMostrar
        {
            get { return Garantias.datoFamiliaMostrar; }
            set { Garantias.datoFamiliaMostrar = value; }
        }

        // MejorasGrupoPelon600R6 - KF. Se agrega propiedad
        private static string documentoGenerar;
        /// <summary>
        /// Indica cual sera el documento a generar en la toma de pedido (pedido,factura,ambos).      
        /// </summary>
        public static string DocumentoGenerar
        {
            get { return Garantias.documentoGenerar; }
            set { Garantias.documentoGenerar = value; }
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
        /// Documento a Generar: Garantias
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
        public Garantias()
        { }
        public Garantias(List<Garantia> gestionados, Cliente cliente)
        {
            this.gestionados = gestionados;
            this.Cliente = cliente;
        }
        #endregion

        #region Logica Negocios
        
        public bool ExistenArticulosGestionados()
        {
            foreach (Garantia ped in gestionados)
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
        /// Guarda en la base de datos la informacion de los Garantias.
        /// </summary>
        public void GuardarGarantias(string pedido)
        {
            foreach (Garantia garantia in gestionados)
                try
                {
                    garantia.Guardar(true,pedido);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando "+(garantia.Tipo == TipoPedido.Factura ? "la factura '" : "el pedido '") + "de la compañía '" + garantia.Compania+ "'. " + ex.Message);
                }
        }

        /// <summary>
        /// Actualiza en la base de datos la informacion de los Garantias.
        /// </summary>
        public void ActualizarGarantias(string numPed)
        {
            foreach (Garantia garantia in gestionados)
            {
                try
                {
                    garantia.Actualizar(true,numPed);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error actualizando "+(garantia.Tipo == TipoPedido.Factura ? "la factura '" : "el pedido '") + garantia.Numero + "'." + ex.Message);
                }
            }
        }

        /// <summary>
        /// Guarda el descuento general en el dataset y actualiza los montos de descuento.
        /// </summary>
        public decimal DefinirDescuento(EDescuento nombreDescuento, decimal porcDescuento)
        {
            decimal montoPorcDescTot = 0;
            foreach (Garantia pedido in gestionados)
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
            foreach (Garantia pedido in gestionados)
            {
                if (pedido.Compania == compania)
                {
                    pedido.DireccionEntrega = dir.Codigo;
                    break;
                }
            }
        }

        /// <summary>
        /// Obtener las companias de los Garantias en gestion.
        /// </summary>
        /// <returns></returns>
        public List<Compania> ObtenerCompanias()
        {
            List<Compania> companias = new List<Compania>();
            companias.Clear();
            foreach (Garantia pedido in gestionados)
            {
                //if (pedido.Detalles.Count > 0)
                companias.Add(pedido.Configuracion.Compania);
            }
            return companias;
        }

        #region Gestion Garantias

        /// <summary>
        /// Busca un pedido dentro de los Garantias en gestionados utilizando el código de la compañía.
        /// </summary>
        /// <param name="codigoCompania">Código de compañía del pedido.</param>
        /// <returns>En caso de haber un pedido o factura gestionada para la compañía retorna un objeto Pedido de lo contrario retorna null.</returns>
        public Garantia Buscar(string compania)
        {
            foreach (Garantia pedido in gestionados)
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

            foreach (Garantia pedido in gestionados)
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
        /// Busca una linea dentro de los detalles de los Garantias gestionados.
        /// </summary>
        /// <param name="articulo">Articulo a buscar.</param>
        /// <returns></returns>
        public DetalleGarantia BuscarDetalle(Articulo articulo)
        {
            foreach (Garantia pedido in gestionados)
            {
                foreach (DetalleGarantia detalle in pedido.Detalles.Lista)
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
        public Garantia BuscarNivelPrecio(Articulo articulo)
        {
            foreach (Garantia pedido in gestionados)
            {
                foreach (DetalleGarantia detalle in pedido.Detalles.Lista)
                {
                    if (detalle.Articulo.Codigo == articulo.Codigo && detalle.Articulo.Compania == articulo.Compania)
                        return pedido;
                }
            }
            return null;
        }
        /// <summary>
        /// Metodo que carga los consecutivos iniciales de los encabezados de los Garantias
        /// </summary>		
        public void CargarConsecutivos()
        {
            foreach (Garantia garantia in gestionados)
            {
                //se verifica que el encabezado tenga líneas de detalle.
                if (!garantia.Detalles.Vacio())
                {
                    string consecutivo = string.Empty;
                    string ncf = string.Empty;

                    if (Garantias.FacturarGarantia)
                    {
                        consecutivo = ParametroSistema.ObtenerGarantia(garantia.Compania, garantia.Zona);

                        //Garantias no usan NCF
                        //if (garantia.Tipo == TipoPedido.Factura && garantia.Configuracion.Compania.UsaNCF && garantia.Configuracion.ClienteCia.TipoContribuyente != string.Empty)
                          //  garantia.NCF.cargarNuevoNCF();
                    }
                    else
                    {
                        if (garantia.UsuarioCambioDescuentos())
                            consecutivo = ParametroSistema.ObtenerPedidoDesc(garantia.Compania, garantia.Zona);
                        else
                            consecutivo = ParametroSistema.ObtenerPedido(garantia.Compania, garantia.Zona);
                    }
                    //Caso 29861 LDS 01/10/2007
                    //Se captura el mensaje.
                    if (consecutivo == string.Empty)
                        throw new Exception("Error obteniendo consecutivo");

                    //ABC 09/10/2008 Caso:33376A
                    //Se incluye la compañia como nuevo parametro 
                    //en el procedimiento de validacion de exitencia de consecutivo
                    consecutivo = garantia.ValidarExistenciaConsecutivo(consecutivo);
                    garantia.Numero = consecutivo;
                }
            }
        }
    
        #endregion

        #region Gestion de los articulos

        /// <summary>
        /// Obtiene los montos total del pedido para los Garantias en una lista.
        /// Verifica si el cliente se excedio en credito en  alguna de las companias.
        /// </summary>
        /// <returns>Indicacion de si el cliente esta excedido en credido para alguna de las companias.</returns>
        public bool SacarMontosTotales()
        {
            LimpiarValores();

            bool creditoExcedido = false;

            foreach (Garantia garantia in gestionados)
            {
                //TotalImpuesto1 += garantia.Impuesto.MontoImpuesto1;
                //Utiliza Percepción
                //if (garantia.Configuracion.Compania.UtilizaMinimoExento)
                //    garantia.RecalcularImpuestos(garantia.MontoSubTotal < garantia.Configuracion.Compania.MontoMinimoExcento);               
                //TotalImpuesto2 += garantia.Impuesto.MontoImpuesto2;
                TotalImpuesto1 = 0;
                TotalImpuesto2 = 0;

                TotalDescuentoLineas += garantia.MontoDescuentoLineas;
                TotalBruto += garantia.MontoBruto;
                TotalDescuento1 += garantia.MontoDescuento1;
                TotalDescuento2 += garantia.MontoDescuento2;
                PorcDesc1 = garantia.PorcentajeDescuento1;
                PorcDesc2 = garantia.PorcentajeDescuento2;
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
            decimal cantidadAlmacen,decimal cantidadDetalle, decimal cantidadAlmacenAdicional, decimal cantidadDetalleAdicional,
            bool validarCantidadLineas, string tope)
        {
            Garantia garantia = Buscar(articulo.Compania);

            if (garantia == null)//se crea un nuevo pedido
            {
                //Si la cantidad es 0 ignore
                if (cantidadDetalle == 0 && cantidadAlmacen == 0)
                    return;                
                
                garantia = new Garantia(articulo, zona, configPedido);

                garantia.Configuracion.Cargar();

                if (Garantias.BonificacionAdicional && !Garantias.FacturarGarantia)
                    garantia.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, 
                                                cantidadAlmacenAdicional, cantidadDetalleAdicional, tope);
                else
                    garantia.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);

                gestionados.Add(garantia);
            }
            else
            {
                //El pedido ya esta creado. Se procede a modificar el detalle y el encabezado
                if (cantidadAlmacen == 0 && cantidadDetalle == 0)
                {
                    garantia.EliminarDetalle(articulo.Codigo);
                    if (garantia.Detalles.Vacio())
                        Borrar(garantia.Compania);
                }
                else
                {
                    if (Garantias.BonificacionAdicional && !Garantias.FacturarGarantia)
                        garantia.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas,
                                                    cantidadAlmacenAdicional, cantidadDetalleAdicional, tope);
                    else
                        garantia.AgregarLineaDetalle(articulo, precio, cantidadDetalle, cantidadAlmacen, validarCantidadLineas, tope);
                }
            }
            SacarMontosTotales();
        }

        public void Gestionar(Articulo articulo, string zona, Precio precio,
            decimal cantidadAlmacen, decimal cantidadDetalle, bool validarCantidadLineas, string tope)
        {
            this.Gestionar(articulo, zona, precio, cantidadAlmacen, cantidadDetalle, 0, 0, validarCantidadLineas, tope);
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
            Garantia pedido = Buscar(articulo.Compania);

            if (pedido == null)//se crea un nuevo pedido
            {
                pedido = Garantia.Consignado(consignacion, articulo, zona, config, descuentoCascada, porcDesc1, porcDesc2, nota);

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

        #region Impresion de Garantias

        #region Impresion de resumen de Garantias

        /// <summary>
        /// Imprime el resumen de Garantias/facturas realizados a un cliente.
        /// </summary>
        /// <param name="Garantias">Lista de Garantias a imprimir</param>
        /// <param name="cliente">Cliente al que se le realizaron los Garantias.</param>
        public void ImprimeResumen(TipoPedido tipo,BaseViewModel viewModel)
        {

            Report resumenGarantias;
            if(tipo==TipoPedido.Factura)
                resumenGarantias = new Report(ReportHelper.CrearRutaReporte(Rdl.ResumenFacturas), Impresora.ObtenerDriver());
            else
                resumenGarantias = new Report(ReportHelper.CrearRutaReporte(Rdl.ResumenPedidos), Impresora.ObtenerDriver());

            resumenGarantias.AddObject(this);
            ImprimeResumen(tipo, viewModel, resumenGarantias);
        }

        private void ImprimeResumen(TipoPedido tipo, BaseViewModel viewModel, Report resumenGarantias)
        {
            resumenGarantias.Print();
            if (resumenGarantias.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión de "+(tipo==TipoPedido.Factura? "la factura" : "el pedido")+ resumenGarantias.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        ImprimeResumen(tipo, viewModel, resumenGarantias);
                    }
                });
        }     

        #endregion

        #region Impresión de Detalle del Pedido

        
        /// <summary>
        /// Imprime las facturas con sus respectivos detalles. TODO
        /// </summary>
        /// <param name="facturas">
        /// Contiene las facturas a imprimir.
        /// </param>
        /// <param name="cantidadCopias">
        /// Indica la cantidad de copias que se requiere imprimir de las facturas.
        /// <param name="criterio">criterio de ordenamiento de los detalles</param>
        public void ImprimeDetalleGarantia( int cantidadCopias, DetalleSort.Ordenador criterio)
        {
            //Imprime el original
            bool imprimirOriginal = false;
            string imprimirTodo = string.Empty;

            foreach (Garantia garantia in gestionados)
            {
                // LJR 20/02/09 Caso 34848 Ordenar articulos de la factura
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    garantia.Detalles.Lista.Sort(new DetalleSort(criterio));
                if (garantia.LeyendaOriginal && !garantia.Impreso)
                {
                    imprimirOriginal = true;
                    break;
                }
            }
            //Contiene las facturas que ya han sido impresas
            ArrayList garantiasImpresas = new ArrayList();

            if (imprimirOriginal)
            {
                Report reporteGarantia;

                foreach (Garantia garantia in gestionados)
                    if (garantia.Impreso)
                        garantiasImpresas.Add(garantia);
                

                reporteGarantia = new Report(ReportHelper.CrearRutaReporte(Rdl.Garantia), Impresora.ObtenerDriver());

                reporteGarantia.AddObject(this);

                //reporteGarantia.Print();
                reporteGarantia.PrintAll(ref imprimirTodo);
                imprimirTodo += "\n\n";

                if (reporteGarantia.ErrorLog != string.Empty)
                    throw new Exception("Ocurrió un error durante la impresión de la garantía: " + reporteGarantia.ErrorLog);

                reporteGarantia = null;

                foreach (Garantia garantia in gestionados)
                    if (!garantia.Impreso && garantia.LeyendaOriginal)
                    {
                        garantia.Impreso = true;
                        garantia.LeyendaOriginal = false;
                        garantia.DBActualizarImpresion();
                    }

                for (int indice = garantiasImpresas.Count; indice > 0; indice--)
                    gestionados.Remove((Garantia)garantiasImpresas[indice - 1]);

                reporteGarantia = null;
            }

            //Imprime las copias con la leyenda de copia
            if (cantidadCopias > 0)
            {
                Report reporteGarantia;

                foreach (Garantia garantia in garantiasImpresas)
                    gestionados.Add(garantia);

                reporteGarantia = new Report(ReportHelper.CrearRutaReporte(Rdl.Garantia), Impresora.ObtenerDriver());

                reporteGarantia.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteGarantia.Print();
                    reporteGarantia.PrintAll(ref imprimirTodo);
                    imprimirTodo += "\n\n";

                    if (reporteGarantia.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de la garantía: " + reporteGarantia.ErrorLog);
                }
                //Imprimir todo a la vez
               reporteGarantia.PrintText(imprimirTodo);
                
                reporteGarantia = null;
            }
        }

        /// <summary>
        /// Imprime las facturas con sus respectivos detalles. TODO
        /// </summary>
        /// <param name="facturas">
        /// Contiene las facturas a imprimir.
        /// </param>
        /// <param name="cantidadCopias">
        /// Indica la cantidad de copias que se requiere imprimir de las facturas.
        /// <param name="criterio">criterio de ordenamiento de los detalles</param>
        public void ImprimeDetalleGarantia(int cantidadCopias, DetalleSort.Ordenador criterio,ref string textoGarantia)
        {
            //Imprime el original
            bool imprimirOriginal = false;
            string imprimirTodo = string.Empty;

            foreach (Garantia garantia in gestionados)
            {
                // LJR 20/02/09 Caso 34848 Ordenar articulos de la factura
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    garantia.Detalles.Lista.Sort(new DetalleSort(criterio));
                if (garantia.LeyendaOriginal && !garantia.Impreso)
                {
                    imprimirOriginal = true;
                    break;
                }
            }
            //Contiene las facturas que ya han sido impresas
            ArrayList garantiasImpresas = new ArrayList();

            if (imprimirOriginal)
            {
                Report reporteGarantia;

                foreach (Garantia garantia in gestionados)
                    if (garantia.Impreso)
                        garantiasImpresas.Add(garantia);


                reporteGarantia = new Report(ReportHelper.CrearRutaReporte(Rdl.Garantia), Impresora.ObtenerDriver());

                reporteGarantia.AddObject(this);

                //reporteGarantia.Print();
                reporteGarantia.PrintAll(ref imprimirTodo);
                imprimirTodo += "\n\n";

                if (reporteGarantia.ErrorLog != string.Empty)
                    throw new Exception("Ocurrió un error durante la impresión de la garantía: " + reporteGarantia.ErrorLog);

                reporteGarantia = null;

                foreach (Garantia garantia in gestionados)
                    if (!garantia.Impreso && garantia.LeyendaOriginal)
                    {
                        garantia.Impreso = true;
                        garantia.LeyendaOriginal = false;
                        garantia.DBActualizarImpresion();
                    }

                for (int indice = garantiasImpresas.Count; indice > 0; indice--)
                    gestionados.Remove((Garantia)garantiasImpresas[indice - 1]);

                reporteGarantia = null;
            }

            //Imprime las copias con la leyenda de copia
            if (cantidadCopias > 0)
            {
                Report reporteGarantia;

                foreach (Garantia garantia in garantiasImpresas)
                    gestionados.Add(garantia);

                reporteGarantia = new Report(ReportHelper.CrearRutaReporte(Rdl.Garantia), Impresora.ObtenerDriver());

                reporteGarantia.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteGarantia.Print();
                    reporteGarantia.PrintAll(ref imprimirTodo);
                    imprimirTodo += "\n\n";

                    if (reporteGarantia.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de la garantía: " + reporteGarantia.ErrorLog);
                }
                //Imprimir todo a la vez

                textoGarantia = imprimirTodo;
                
                reporteGarantia = null;
            }
        }



        #endregion

        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "GARANTIAS";
        }

        public override object GetField(string name)
        {
            if (name == "LISTA_GARANTIAS")
            {
                return new ArrayList(gestionados);
            }
            else
                return base.GetField(name);
        }

        #endregion


    }
}
