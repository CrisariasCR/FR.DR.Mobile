using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using EMF.Printing.Drivers;
using EMF.Win32;
namespace Softland.ERP.FR.Mobile.Cls.Configuracion
{
    public class HandHeldConfig
    {
        #region Variables y Propiedades de instancia

        #region Atributos
        /*ESTAS VARIABLES SE DEJAN POR SI SE DECIDE CENTRALIZAR TODAS LAS VARIABLES A ESTA CLASE*/
        /*
        /// <summary>
        ///  Nombre de la Base de Datos que se encuentra en la pocket, 
        /// este nombre debe ser el mismo que se especifica en el EMM en el campo 
        /// Nombre de la base de datos del tag de Estructura de datos en PDA. 
        /// Este es un campo requerido.
        /// </summary>
        private string bd_nombre;

        /// <summary>
        /// 
        /// </summary>
        private string bd_compania;

        /// <summary>
        /// 
        /// </summary>
        private string serv_nombre;

        /// <summary>
        /// 
        /// </summary>
        private string serv_dominio;

        /// <summary>
        /// 
        /// </summary>
        private string print_tipo;

        /// <summary>
        /// puerto que se utiliza para la comunicación entre la pocket y la impresora
        /// </summary>
        private string print_puerto;

        /// <summary>
        /// corresponde a la cantidad de copias que se sugerirá al realizar la impresión de documentos de tipo: factura, devolución, recibo.
        /// </summary>
        private string print_cantcopias;

        /// <summary>
        /// corresponde a la cantidad de líneas de detalle que se sugerirá al 
        /// realizar la impresión de documentos de tipo: factura, devolución, 
        /// recibo, venta en consignación.
        /// </summary>
        private string print_cantlineas;

        /// <summary>
        /// mostrar o no el mensaje de imprimir un documento.
        /// </summary>
        private string print_sugeririmprimir;

        /// <summary>
        /// indica si la aplicación va a trabajar realizando 
        ///la selección de clientes por medio del escaneo del código de barras 
        ///o por selección manual de la lista
        /// </summary>
        private string cg_selectclienteautomat;

        /// <summary>
        /// indica si la aplicación va a permitir la selección agil de clientes 
        /// por su código independientemente de la ruta, día de visita o si ya se 
        /// realizo la visita o no.
        ///esto es una alternativa a empresas que no manejan el código de barras, 
        ///para encontrar rápidamente un cliente.
        /// </summary>
        private string cg_buscagilcliente;

        /// <summary>
        /// indica el simbolo monetario que debe utilizar el sistema para desplegar los montos en pantalla y en los reportes
        /// </summary>
        private string cg_simbolomonet;

        /// <summary>
        /// indica cuantos decimales son permitidos cuando se 
        ///ingresan cantidades de almacen y detalle.
        /// los valores permitidos son 
        /// 0, 
        /// 1, 
        /// 2 o 
        /// 3 solamente
        /// </summary>
        private string cg_cantdecimales;

        /// <summary>
        /// 
        /// </summary>
        private string cg_permitinfodescvencida;

        /// <summary>
        /// indica si se permite registrar multiples documentos 
        ///(pedidos, inventario, devolucion) para un cliente.  
        ///los valores permitidos son  
        ///0: no permite. 
        ///1: permite n documentos. 
        ///2: permite con advertencia
        /// </summary>
        private string cg_permitregmultidocumento;

        /// <summary>
        /// guarda un log de eventos
        /// </summary>
        private string cg_archivobitacora;

        /// <summary>
        /// mensaje de credito excedido 
        /// </summary>
        private string cg_msjconf_creditexedido;

        /// <summary>
        /// mensaje de credito excedido 
        /// </summary>
        private string cg_msjconf_facvencidas;

        /// <summary>
        /// tag que indica si se desea aplicar o no las notas de credito 
        ///que el cliente tiene al momento de realizar un cobro 
        ///los valores permitidos son no o si
        /// </summary>
        private string cg_cobro_aplicnotacredito;

        /// <summary>
        /// indica si se permite realizar cobros por monto total
        /// </summary>
        private string cg_cobro_habilmontototal;

        /// <summary>
        /// indica si se permite editar el numero de recibo
        /// </summary>
        private string cg_cobro_cambionumrecibo;

        /// <summary>
        /// indica si se debe mostrar la referencia de existencias al crear pedidos
        /// </summary>
        private string cg_pedido_mostrarrefexisten;

        /// <summary>
        /// maneja el limite de lineas de pedido permitidas
        /// </summary>
        private string cg_pedido_maxlineapedido;

        /// <summary>
        /// indica si se puede modificar el descuento 1
        /// </summary>
        private string cg_pedido_habilitardescuento1;

        /// <summary>
        /// indica la cantidad maxima permitida para el descuento número1
        /// </summary>
        private string cg_pedido_maxdescuentopermit1;

        /// <summary>
        /// indica si se puede modificar el descuento 2
        /// </summary>
        private string cg_pedido_habilitardescuento2;

        /// <summary>
        /// indica la cantidad maxima permitida para el descuento número2
        /// </summary>
        private string cg_pedido_maxdescuentopermit2;

        /// <summary>
        /// indica el criterio de búsqueda de articulos que utiliza inicialmente la pantalla de ingreso de cantidades.
        /// se debe digitar: 
        /// 0 - ninguno
        /// 1 - código
        /// 2 - código barras
        /// 3 - descripción
        /// 4 - familia
        /// 5 - clase
        /// 6 - pedido actual
        /// </summary>
        private string cg_pedido_critbusqueda;

        /// <summary>
        /// indica si se puede modificar el precio de un artículo cuando se realiza
        /// un pedido o factura
        /// </summary>
        private string cg_pedido_cambiarprecio;

        /// <summary>
        /// indica si se debe mostrar el costo del artículo en la ventana cambio de precio
        /// </summary>
        private string cg_pedido_vercostoarticulo;

        /// <summary>
        /// indica si se debe mostrar o no la utilidad definida en exactus en la ventana 
        ///	cambio de precio
        /// </summary>
        private string cg_pedido_verutilidaddefinida;

        /// <summary>
        /// indica si se debe mostrar o no la nueva utilidad, la cual se calcula cuando 
        /// cuando se cambia el precio en la ventana cambio de precio
        /// </summary>
        private string cg_pedido_vernuevautilidad;

        /// <summary>
        /// indica si se permite el modificar la candiad a bonificar de un articulo
        /// </summary>
        private string cg_pedido_cambiobonific;

        /// <summary>
        /// indica si se permite el modificar el porcentaje de descuento de un articulo
        /// </summary>
        private string cg_pedido_cambiodescuento;

        /// <summary>
        /// indica si se habilita la opción de realizar devoluciones,
        /// se utiliza unicamente en la pantalla procesos del sistema
        /// </summary>
        private string cg_dev_habilitardevoluciones;

        /// <summary>
        /// indica si se habilita la opción de realizar devoluciones automaticamente
        /// </summary>
        private string cg_dev_devautomatic;

        /// <summary>
        /// indica si se habilita la opción de tipos de devoluciones
        /// </summary>
        private string cg_dev_tipodev_habilitartipo;

        /// <summary>
        /// tipo1
        /// </summary>
        private string cg_dev_tipodev_tipos;

        /// <summary>
        /// permite habilitar el cambio del monto del deposito
        /// </summary>
        private string cg_deposito_cambiomonto;

        /// <summary>
        /// diferencia maxima de cambio
        /// </summary>
        private string cg_deposito_difmax;

        /// <summary>
        /// diferencia minima de cambio
        /// </summary>
        private string cg_deposito_difmin;
        * */
        //LDA mejora Cesar Iglesias

        /// <summary>
        /// Mantiene los text de el form de Pedido
        /// </summary>
        public static string[] labelsPedido;

        public static string[] ordenDefaultColumnasPedido;

        /// <summary>
        /// Muestra el orden de las columnas que seran visibles en el pedido
        /// </summary>
        public static int[] ordenColumnasPedido;
        /// <summary>
        /// Contiene las columnas que seran visibles.
        /// </summary>
        public static string[] columnasVisiblePedido;

        public static string cg_columna_fija;
        //LDA mejora Cesar Iglesias
        #endregion

        #region ConstantesHandheld

       
        /// <summary>
        ///  Nombre de la Base de Datos que se encuentra en la pocket, 
        /// este nombre debe ser el mismo que se especifica en el EMM en el campo 
        /// Nombre de la base de datos del tag de Estructura de datos en PDA. 
        /// Este es un campo requerido.
        /// </summary>
        private const string BD_NOMBRE = "BD_NOMBRE";

        /// <summary>
        /// 
        /// </summary>
        public const string BD_COMPANIA = "BD_COMPANIA";

        /// <summary>
        /// 
        /// </summary>
        public const string SERV_NOMBRE = "SERV_NOMBRE";

        /// <summary>
        /// 
        /// </summary>
        public const string SERV_DOMINIO = "SERV_DOMINIO";

        /// <summary>
        /// 
        /// </summary>
        public const string PRINT_TIPO = "PRINT_TIPO";

        /// <summary>
        /// Puerto que se utiliza para la comunicación entre la pocket y la impresora
        /// </summary>
        public const string PRINT_PUERTO = "PRINT_PUERTO";

        /// <summary>
        /// Corresponde a la cantidad de copias que se sugerirá al realizar la impresión de documentos de Tipo: Factura, Devolución, Recibo.
        /// </summary>
        public const string PRINT_CANTCOPIAS = "PRINT_CANTCOPIAS";

        /// <summary>
        /// Corresponde a la cantidad de líneas de detalle que se sugerirá al 
        /// realizar la impresión de documentos de Tipo: Factura, Devolución, 
        /// Recibo, Venta en Consignación.
        /// </summary>
        public const string PRINT_CANTLINEAS = "PRINT_CANTLINEAS";

        /// <summary>
        /// Mostrar o no el mensaje de Imprimir un documento.
        /// </summary>
        public const string PRINT_SUGERIRIMPRIMIR = "PRINT_SUGERIRIMPRIMIR";

        /// <summary>
        /// Indica si la aplicación va a trabajar realizando 
        ///la selección de clientes por medio del escaneo del código de barras 
        ///o por selección manual de la lista
        /// </summary>
        public const string CG_SELECTCLIENTEAUTOMAT = "CG_SELECTCLIENTEAUTOMAT";

        /// <summary>
        /// Indica si la aplicación va a permitir la selección agil de clientes 
        /// por su código independientemente de la ruta, día de visita o si ya se 
        /// realizo la visita o no.
        ///Esto es una alternativa a empresas que no manejan el código de barras, 
        ///para encontrar rápidamente un cliente.
        /// </summary>
        public const string CG_BUSCAGILCLIENTE = "CG_BUSCAGILCLIENTE";

        /// <summary>
        /// Indica el simbolo monetario que debe utilizar el sistema para desplegar los montos en pantalla y en los reportes
        /// </summary>
        public const string CG_SIMBOLOMONET = "CG_SIMBOLOMONET";

        /// <summary>
        /// Indica cuantos decimales son permitidos cuando se 
        ///ingresan cantidades de almacen y detalle.
        /// Los valores permitidos son 
        /// 0, 
        /// 1, 
        /// 2 o 
        /// 3 solamente
        /// </summary>
        public const string CG_CANTDECIMALES = "CG_CANTDECIMALES";

        /// <summary>
        /// 
        /// </summary>
        public const string CG_PERMITINFODESCVENCIDA = "CG_PERMITINFODESCVENCIDA";

        /// <summary>
        /// indica si se permite registrar multiples documentos 
        ///(Pedidos, inventario, devolucion) para un cliente.  
        ///Los valores permitidos son  
        ///0: no permite. 
        ///1: permite n documentos. 
        ///2: permite con advertencia
        /// </summary>
        public const string CG_PERMITREGMULTIDOCUMENTO = "CG_PERMITREGMULTIDOCUMENTO";

        /// <summary>
        /// Guarda un log de eventos
        /// </summary>
        public const string CG_ARCHIVOBITACORA = "CG_ARCHIVOBITACORA";

        /// <summary>
        /// Mensaje de credito excedido 
        /// </summary>
        public const string CG_MSJCONF_CREDITEXEDIDO = "CG_MSJCONF_CREDITEXEDIDO";

        /// <summary>
        /// Mensaje de credito excedido 
        /// </summary>
        public const string CG_MSJCONF_FACVENCIDAS = "CG_MSJCONF_FACVENCIDAS";

        /// <summary>
        /// Tag que indica si se desea aplicar o no las notas de credito 
        ///que el cliente tiene al momento de realizar un cobro 
        ///Los valores permitidos son NO o SI
        /// </summary>
        public const string CG_COBRO_APLICNOTACREDITO = "CG_COBRO_APLICNOTACREDITO";

        /// <summary>
        /// Indica si se permite realizar cobros por monto total
        /// </summary>
        public const string CG_COBRO_HABILMONTOTOTAL = "CG_COBRO_HABILMONTOTOTAL";

        /// <summary>
        /// Indica si se permite editar el numero de recibo
        /// </summary>
        public const string CG_COBRO_CAMBIONUMRECIBO = "CG_COBRO_CAMBIONUMRECIBO";

        /// <summary>
        /// indica si se debe mostrar la referencia de existencias al crear pedidos
        /// </summary>
        public const string CG_PEDIDO_MOSTRARREFEXISTEN = "CG_PEDIDO_MOSTRARREFEXISTEN";

        /// <summary>
        /// maneja el limite de lineas de pedido permitidas
        /// </summary>
        public const string CG_PEDIDO_MAXLINEAPEDIDO = "CG_PEDIDO_MAXLINEAPEDIDO";

        /// <summary>
        /// Indica si se puede modificar el descuento 1
        /// </summary>
        public const string CG_PEDIDO_HABILITARDESCUENTO1 = "CG_PEDIDO_HABILITARDESCUENTO1";

        /// <summary>
        /// Indica la cantidad maxima permitida para el descuento número1
        /// </summary>
        public const string CG_PEDIDO_MAXDESCUENTOPERMIT1 = "CG_PEDIDO_MAXDESCUENTOPERMIT1";

        /// <summary>
        /// Indica si se puede modificar el descuento 2
        /// </summary>
        public const string CG_PEDIDO_HABILITARDESCUENTO2 = "CG_PEDIDO_HABILITARDESCUENTO2";

        /// <summary>
        /// Indica la cantidad maxima permitida para el descuento número2
        /// </summary>
        public const string CG_PEDIDO_MAXDESCUENTOPERMIT2 = "CG_PEDIDO_MAXDESCUENTOPERMIT2";

        /// <summary>
        /// Indica el criterio de búsqueda de articulos que utiliza inicialmente la pantalla de ingreso de cantidades.
        /// Se debe digitar: 
        /// 0 - Ninguno
        /// 1 - Código
        /// 2 - Código Barras
        /// 3 - Descripción
        /// 4 - Familia
        /// 5 - Clase
        /// 6 - Pedido Actual
        /// </summary>
        public const string CG_PEDIDO_CRITBUSQUEDA = "CG_PEDIDO_CRITBUSQUEDA";

        /// <summary>
        /// Indica si se puede modificar el precio de un artículo cuando se realiza
        /// un pedido o factura
        /// </summary>
        public const string CG_PEDIDO_CAMBIARPRECIO = "CG_PEDIDO_CAMBIARPRECIO";

        /// <summary>
        /// Indica si se debe mostrar el costo del artículo en la ventana Cambio de Precio
        /// </summary>
        public const string CG_PEDIDO_VERCOSTOARTICULO = "CG_PEDIDO_VERCOSTOARTICULO";

        /// <summary>
        /// Indica si se debe mostrar o no la utilidad definida en exactus en la ventana 
        ///	Cambio de Precio
        /// </summary>
        public const string CG_PEDIDO_VERUTILIDADDEFINIDA = "CG_PEDIDO_VERUTILIDADDEFINIDA";

        /// <summary>
        /// Indica si se debe mostrar o no la nueva utilidad, la cual se calcula cuando 
        /// cuando se cambia el precio en la ventana Cambio de Precio
        /// </summary>
        public const string CG_PEDIDO_VERNUEVAUTILIDAD = "CG_PEDIDO_VERNUEVAUTILIDAD";

        /// <summary>
        /// Indica si se permite el modificar la candiad a bonificar de un articulo
        /// </summary>
        public const string CG_PEDIDO_CAMBIOBONIFIC = "CG_PEDIDO_CAMBIOBONIFIC";

        /// <summary>
        /// Indica si se permite el modificar el porcentaje de descuento de un articulo
        /// </summary>
        public const string CG_PEDIDO_CAMBIODESCUENTO = "CG_PEDIDO_CAMBIODESCUENTO";

        /// <summary>
        /// Indica si se habilita la opción de realizar devoluciones,
        /// se utiliza unicamente en la pantalla Procesos del Sistema
        /// </summary>
        public const string CG_DEV_HABILITARDEVOLUCIONES = "CG_DEV_HABILITARDEVOLUCIONES";

        /// <summary>
        /// Indica si se habilita la opción de realizar devoluciones automaticamente
        /// </summary>
        public const string CG_DEV_DEVAUTOMATIC = "CG_DEV_DEVAUTOMATIC";

        /// <summary>
        /// Indica si se habilita la opción de tipos de devoluciones
        /// </summary>
        public const string CG_DEV_TIPODEV_HABILITARTIPO = "CG_DEV_TIPODEV_HABILITARTIPO";

        /// <summary>
        /// Tipo1
        /// </summary>
        public const string CG_DEV_TIPODEV_TIPOS = "CG_DEV_TIPODEV_TIPO";


        /// <summary>
        /// Permite habilitar el cambio del monto del deposito
        /// </summary>
        public const string CG_DEPOSITO_CAMBIOMONTO = "CG_DEPOSITO_CAMBIOMONTO";

        /// <summary>
        /// Diferencia maxima de cambio
        /// </summary>
        public const string CG_DEPOSITO_DIFMAX = "CG_DEPOSITO_DIFMAX";

        /// <summary>
        /// Diferencia minima de cambio
        /// </summary>
        public const string CG_DEPOSITO_DIFMIN = "CG_DEPOSITO_DIFMIN";

        //LDA mejora Cesar Iglesias

        /// <summary>
        /// Fija el tamano de la letra del Grid de FRM
        /// </summary>
        public const string CG_TAMANO_GRID = "CG_TAMANO_GRID";

        /// <summary>
        /// Verifica si se utilizaran topes en la HandHeld
        /// </summary>
        public const string CG_PEDIDO_USAR_TOPES="CG_PEDIDO_USAR_TOPES";

        /// <summary>
        /// Fija el monto maximo para el tope 1
        /// </summary>
        public const string CG_PEDIDO_TOPE1="CG_PEDIDO_TOPE1";

        /// <summary>
        /// Fija el monton maximo para el tope 2
        /// </summary>
        public const string CG_PEDIDO_TOPE2="CG_PEDIDO_TOPE2";

        /// <summary>
        /// Fija el mensaje a mostrar al sobrepasar el Tope 1
        /// </summary>
        public const string CG_MSJCONF_TOPE1 = "CG_MSJCONF_TOPE1";

        /// <summary>
        ///  Fija el mensaje a mostrar al sobrepasar el Tope 2
        /// </summary>
        public const string CG_MSJCONF_TOPE2 = "CG_MSJCONF_TOPE2";

        /// <summary>
        /// Indica si se permite adicionar o agregar bonificaciones de un articulo
        /// </summary>
        public const string CG_PEDIDO_ADICIONBONIFIC = "CG_PEDIDO_ADICIONBONIFIC";

        /// <summary>
        /// Si le permite al rutero en la pantalla de pedidos el cambio de teclado
        /// </summary>
        public const string CG_PEDIDO_CAMBIO_TECLADO = "CG_PEDIDO_CAMBIO_TECLADO";

        /// <summary>
        /// Indica el teclado por defecto
        /// </summary>
        public const string CG_PEDIDO_TECLADO_DEFAULT = "CG_PEDIDO_TECLADO_DEFAULT";

        /// <summary>
        /// Si permite al pedido la toma rapida de cantidades o informacion.
        /// </summary>
        public const string CG_PEDIDO_TOMA_AUTOMATICA = "CG_PEDIDO_TOMA_AUTOMATICA";

        /// <summary>
        /// Indica si permite actualización por medio del Actualizador.
        /// </summary>
        public const string AC_ACTUALIZADA = "AC_ACTUALIZADA";

        /// <summary>
        /// Si permite al pedido la toma rapida de cantidades o informacion.
        /// </summary>
        public const string CG_PEDIDO_SUBFILTRO_FAMILIA = "CG_PEDIDO_SUBFILTRO_FAMILIA";
        
        /// <summary>
        /// Se calcula los impuesto o no en el total de la pantalla de toma de pedidos.
        /// </summary>
        public const string CG_PEDIDO_CALCULAR_IMPUESTOS = "CG_PEDIDO_CALCULAR_IMPUESTOS";

        /// <summary>
        /// Si se ordena por el criterio de busqueda.
        /// </summary>
        public const string CG_ORDENAR_BUSQUEDA = "CG_ORDENAR_BUSQUEDA";

        /// <summary>
        /// Si ordena por indice de orden del articulo
        /// </summary>
        public const string CG_ORDENAR_INDICE_ART = "CG_ORDENAR_INDICE_ART";

        /// <summary>
        /// Si utiliza esquema de descuentos por pronto pago.
        /// </summary>
        public const string CG_COBRO_DESC_PRONTO_PAGO = "CG_DESC_PRONTO_PAGO";

        /// <summary>
        /// Indica si condiciona el desglose de lotes para facturas
        /// </summary>
        public const string CG_DESGLOSE_LOTES_FACTURAS = "CG_DESGLOSE_LOTES_FACTURAS";

        /// <summary>
        /// Purga Automatica Documentos
        /// </summary>
        public const string CG_PURGA_DOC_AUTOMATICA = "CG_PURGA_DOC_AUTOMATICA";

        /// <summary>
        /// Dias Purga Automatica Documentos
        /// </summary>
        public const string CG_PURGA_DIAS_AUTOMATICA = "CG_PURGA_DIAS_AUTOMATICA";

        /// <summary>
        /// Purga Manual Documentos
        /// </summary>
        public const string CG_PURGA_DOC_MANUAL = "CG_PURGA_DOC_MANUAL";

        /// <summary>
        /// Indica el modo de cambio de precio
        /// </summary>
        public const string CG_PEDIDO_CAMBIOPRECIO_MODO = "CG_PEDIDO_CAMBIOPRECIO_MODO";

        /// <summary>
        /// Indica el modo de validación de limite de credito
        /// </summary>
        public const string CG_PEDIDO_LIMITECREDITO = "CG_PEDIDO_LIMITECREDITO";

        /// <summary>
        /// Indica el modo de validación de documentos pendientes de cobro
        /// </summary>
        public const string CG_PEDIDO_DOC_VENCIDOS = "CG_PEDIDO_DOC_VENCIDOS";

        /// <summary>
        /// Indica si el dispositivo puede obtener la ubicación del cliente
        /// </summary>
        public const string CG_GUARDAR_UBI_CLIENTE = "CG_GUARDAR_UBI_CLIENTE";

        /// <summary>
        /// Indica si el dispositivo puede obtener la ubicación de cada visita.
        /// </summary>
        public const string CG_GUARDAR_UBI_VISITA = "CG_GUARDAR_UBI_VISITA";

        /// <summary>
        /// Indica si el dispositivo puede actualizar la ubicación del cliente
        /// </summary>
        public const string CG_ACTUALIZAR_UBI_CLIENTE = "CG_ACTUALIZAR_UBI_CLIENTE";

        /// <summary>
        /// Indica la cantidad metros permitida entre la ubicación del cliente y la de la visita.
        /// </summary>
        public const string CG_DISTANCIA_UBI_CLIENTE = "CG_DISTANCIA_UBI_CLIENTE";

        /// <summary>
        /// Indica la cantidad metros permitida entre la ubicación del cliente y la de la visita.
        /// </summary>
        public const string CG_UBI_NO_PARAR = "CG_UBI_NO_PARAR";

        /// <summary>
        /// // MejorasGrupoPelon600R6 - KF //
        /// Indica si se permite o no agregar articulos fuera del nivel de precios a un pedido.
        /// </summary>
        public const string CG_ART_NO_NIV_PREC = "CG_ART_NO_NIV_PREC";

        /// <summary>
        /// // MejorasGrupoPelon600R6 - KF //
        /// Indica como se mostrará el deto de familia en el filtro de toma de pedido.
        /// </summary>
        public const string CG_PEDIDO_DATO_FAMILIA = "CG_PEDIDO_DATO_FAMILIA";

        /// <summary>
        /// //Facturas de contado y recibos en FR - KFC
        /// Indica el tipo de devoluciones que se perimitiran realizar al rutero
        /// </summary>
        public const string CG_DEV_PERMITIR_TIPO_PAGO = "CG_DEV_PERMITIR_TIPO_PAGO";

        #endregion

        #region ConstantesGlobalesHandHeld
        /// <summary>
        /// Fija las etiquetas mostradas en el form de Pedido
        /// </summary>
        public const string TEXT_ETIQ_PED = "TEXT_ETIQ_PED";

        public const string ORD_PED_COLS = "ORD_PED_COLS";

        public const string DESGLOSE_LOTES = "DESGLOSE_LOTES";

        // MejorasGrupoPelon600R6 - KF //
        public const string DOC_A_GENERAR = "DOC_A_GENERAR";

        public const string UTILIZA_JORNADA_TRABAJO = "UTILIZA_JORNADA_TRABAJO";

        #region MejorasFRTostadoraElDorado600R6 JEV
        /// <summary>
        /// Indica si se utiliza toma 
        /// física de inventarios
        /// </summary>
        public const string UTILIZA_TOMA_FISICA = "UTILIZA_TOMA_FISICA";

        /// <summary>
        /// Indica si se debe facturar las
        /// diferencias encontradas en el 
        /// proceso de toma física
        /// </summary>
        public const string FACTURAR_DIFERENCIAS_TM = "FACTURAR_DIFERENCIAS";

        /// <summary>
        /// Código del cliente para facturación
        /// de diferencias en toma física
        /// </summary>
        public const string CLIENTE_RUTERO = "CLIENTE_RUTERO";

        // Modificaciones en funcionalidad de recibos de contado - KFC
        /// <summary>
        /// Formas de generar el recibo
        /// </summary>
        public const string FORMAS_GENERACION_RECIBOS = "FORMAS_GENERACION_RECIBOS";

        #endregion MejorasFRTostadoraElDorado600R6 JEV

        #endregion
                
        #endregion

        #region Metodos
        /// <summary>
        /// Muestra la informacion referente a la configuracion de la handHeld
        /// </summary>
        /// <param name="constanteId"></param>
        /// <param name="valor"></param>
        private bool fillInformation(string constanteId, string valor, ref string mensaje)
        {
            bool exito = true;

            switch (constanteId)
            {
                    //los valores comentados estan en el Config.xml, se dejan como validacion 
                    //para verificar se estan en la BD
                #region DataBase
                case BD_NOMBRE:
                    //GestorDatos.BaseDatos = valor;
                    break;

                case BD_COMPANIA:
                    //GestorDatos.Owner = valor;
                    break;
                #endregion

                #region ServersSection
                case SERV_NOMBRE:
                    //GestorDatos.ServidorWS = valor;
                    break;
                case SERV_DOMINIO:
                    //GestorDatos.Dominio = valor;
                    break;
                #endregion

                #region General

                case CG_PERMITREGMULTIDOCUMENTO:
                    GestorDocumentos.PermiteRegistroMultiplesDocs = (GestorDocumentos.OpcionMultiple)byte.Parse(valor);
                    break;

                case CG_CANTDECIMALES:
                    FRmConfig.CantidadDecimales = GestorUtilitario.ParseInt(valor);
                    break;

                case CG_MSJCONF_CREDITEXEDIDO:
                    FRmConfig.MensajeCreditoExcedido = valor;
                    break;

                case CG_MSJCONF_FACVENCIDAS:
                    FRmConfig.MensajeFacturasVencidas = valor;
                    break;

                case CG_SIMBOLOMONET:
                    GestorUtilitario.SimboloMonetario = valor;
                    AndroidDriver.simboloMoneda = valor;
                    break;

                case CG_ARCHIVOBITACORA:
                    Bitacora.NombreArchivo = valor;
                    break;

                case CG_PERMITINFODESCVENCIDA:
                    Descuento.PermitirInfoDescuentosVencida = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_BUSCAGILCLIENTE:
                    FRmConfig.BusquedaAgilCliente = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_COBRO_APLICNOTACREDITO:
                    Cobros.AplicarLasNotasCredito = valor.Substring(0,1); 
                    break;

                case CG_COBRO_HABILMONTOTOTAL:
                    Cobros.HabilitarMontoTotal = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_COBRO_CAMBIONUMRECIBO:
                    Cobros.CambiarNumeroRecibo = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_ORDENAR_BUSQUEDA:
                    FRmConfig.OrdenarCriterio = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_ORDENAR_INDICE_ART:
                    FRmConfig.OrdenarIndiceArticulo = GestorUtilitario.ParseBoolean(valor);
                    break;
                case CG_COBRO_DESC_PRONTO_PAGO:
                    Cobros.AplicarDescuentosProntoPago = GestorUtilitario.ParseBoolean(valor);
                    break;


                #endregion General

                #region Pedido

                case CG_PEDIDO_MOSTRARREFEXISTEN:
                    Pedidos.MostrarRefExistencias = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_MAXLINEAPEDIDO:
                    Pedidos.MaximoLineasDetalle = GestorUtilitario.ParseInt(valor);
                    Garantias.MaximoLineasDetalle = GestorUtilitario.ParseInt(valor);
                    break;

                case CG_PEDIDO_HABILITARDESCUENTO1:
                    Pedidos.HabilitarDescuento1 = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_HABILITARDESCUENTO2:
                    Pedidos.HabilitarDescuento2 = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_MAXDESCUENTOPERMIT1:
                    Pedidos.MaximoDescuentoPermitido1 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_PEDIDO_MAXDESCUENTOPERMIT2:
                    Pedidos.MaximoDescuentoPermitido2 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_PEDIDO_CRITBUSQUEDA:
                    Pedidos.CriterioBusquedaDefault = GestorUtilitario.ParseInt(valor);
                    break;

                case CG_PEDIDO_CAMBIARPRECIO:
                    Pedidos.CambiarPrecio = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_VERCOSTOARTICULO:
                    Pedidos.MostrarCostoArticulo = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_VERUTILIDADDEFINIDA:
                    Pedidos.MostrarUtilidadDefinida = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_VERNUEVAUTILIDAD:
                    Pedidos.MostrarNuevaUtilidad = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CAMBIOBONIFIC:
                    Pedidos.CambiarBonificacion = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CAMBIODESCUENTO:
                    Pedidos.CambiarDescuento = GestorUtilitario.ParseBoolean(valor);
                    break;

                #region MejorasFRTostadoraElDorado600R6 JEV
                    //case 
                #endregion MejorasFRTostadoraElDorado600R6 JEV
                #endregion Pedido

                #region Devolucion

                case CG_DEV_HABILITARDEVOLUCIONES:
                    Devoluciones.HabilitarDevoluciones = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_DEVAUTOMATIC:
                    Devoluciones.DevolucionAutomatica = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_TIPODEV_HABILITARTIPO:
                    Devoluciones.TiposDevoluciones = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_TIPODEV_TIPOS:
                    if(Devoluciones.tiposDevolucion != null)
                        Devoluciones.tiposDevolucion.Clear();
                    cargarTipos(valor);
                    break;

                //Facturas de contado y recibos en FR - KFC
                case CG_DEV_PERMITIR_TIPO_PAGO:
                    Devoluciones.tipoPagoDevolucion = valor;
                    break;

                #endregion Devolucion

                #region Deposito

                case CG_DEPOSITO_CAMBIOMONTO:
                    Deposito.CambioMonto = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEPOSITO_DIFMAX:
                    Deposito.DiferenciaMax = Convert.ToDecimal(valor);
                    break;

                case CG_DEPOSITO_DIFMIN:
                    Deposito.DiferenciaMin = Convert.ToDecimal(valor);
                    break;

                #endregion Deposito

                case CG_TAMANO_GRID:
                    GestorDocumentos.tamanoLetra = float.Parse(valor);
                    break;

                case PRINT_TIPO:
                    Impresora.IndicaImpresora(valor);
                    break;

                case PRINT_PUERTO:
                    Impresora.IndicaPuerto(valor);
                    break;

                case PRINT_CANTCOPIAS:
                    Impresora.CantidadCopias = Convert.ToInt32(valor);
                    break;

                case PRINT_CANTLINEAS:
                    Impresora.CantidadLineas = Convert.ToInt32(valor);
                    break;

                case PRINT_SUGERIRIMPRIMIR:
                    Impresora.SugerirImprimir = GestorUtilitario.ParseBoolean(valor);
                    break;
               
                case CG_SELECTCLIENTEAUTOMAT:
                    FRmConfig.SeleccionXCodBarras = GestorUtilitario.ParseBoolean(valor);
                    break; 

                case CG_PEDIDO_USAR_TOPES:
                    Pedidos.ManejaTopes = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_TOPE1:
                    Pedidos.Tope1 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_PEDIDO_TOPE2:
                     Pedidos.Tope2 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_MSJCONF_TOPE1:
                     Pedidos.MsjTope1 = valor;
                    break;

                case CG_MSJCONF_TOPE2:
                    Pedidos.MsjTope2 = valor;
                    break;

                case CG_PEDIDO_ADICIONBONIFIC:
                    Pedidos.BonificacionAdicional= GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CAMBIO_TECLADO:
                    Pedidos.CambioTeclado = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_TECLADO_DEFAULT:
                    Pedidos.TecladoDefecto = (EMF.WF.TextBoxInput)Convert.ToInt32(valor);
                    break;

                case CG_PEDIDO_TOMA_AUTOMATICA:
                    Pedidos.TomaAutomatica = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_SUBFILTRO_FAMILIA:
                    Pedidos.SubFiltroFamilia = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CALCULAR_IMPUESTOS:
                    Pedidos.CalcularImpuestos = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DESGLOSE_LOTES_FACTURAS:
                    Pedidos.DesgloseLotesFacturaObliga = GestorUtilitario.ParseBoolean(valor);
                    break;

                // MejorasGrupoPelon600R6 - KF //
                case CG_ART_NO_NIV_PREC:
                    //valor = "NO";
                   // mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.ArtFueraNivPrecio = GestorUtilitario.ParseBoolean(valor);
                    break;

                // MejorasGrupoPelon600R6 - KF //
                case CG_PEDIDO_DATO_FAMILIA:
                    //valor = "NO";
                    // mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.DatoFamiliaMostrar = GestorUtilitario.ParseInt(valor);
                    break;
                #region MejorasFRTostadoraElDorado600R6 JEV
                //case UTILIZA_TOMA_FISICA:
                    
                //    break;
                //case FACTURAR_DIFERENCIAS_TM:
                    
                //    break;
                case CLIENTE_RUTERO:
                    FRmConfig.ClienteRutero = valor;
                    break;
                #endregion MejorasFRTostadoraElDorado600R6 JEV

                #region Actualizacion
                case AC_ACTUALIZADA :
                    FRmConfig.PermiteActualizar = valor;
                    break;
                #endregion

                #region Purga de documentos
                case CG_PURGA_DOC_AUTOMATICA:
                    FRmConfig.PurgarDocumentosAutomaticamente = GestorUtilitario.ParseBoolean(valor);
                    break;
                case CG_PURGA_DIAS_AUTOMATICA:
                    FRmConfig.PurgarCantidadDias = GestorUtilitario.ParseInt(valor);
                    break;
                case CG_PURGA_DOC_MANUAL:
                    FRmConfig.PugarDocumentosManualmente = GestorUtilitario.ParseBoolean(valor);
                    break;
                #endregion

                case CG_PEDIDO_CAMBIOPRECIO_MODO:
                    Pedidos.ModoCambiosPrecios = valor;
                    break;

                case CG_PEDIDO_LIMITECREDITO:
                    Pedidos.ValidarLimiteCredito = valor;
                    break;

                case CG_PEDIDO_DOC_VENCIDOS:
                    Pedidos.ValidarDocVencidos = valor;
                    break;

                case CG_GUARDAR_UBI_CLIENTE:
                    FRmConfig.GuardarUbicacionClientes = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_GUARDAR_UBI_VISITA:
                    FRmConfig.GuardarUbicacionVisita = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_ACTUALIZAR_UBI_CLIENTE:
                    FRmConfig.ActualizarUbicacionClientes = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DISTANCIA_UBI_CLIENTE:
                    FRmConfig.DistanciaPermitidaUbicacion = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_UBI_NO_PARAR:
                    FRmConfig.NoDetenerGPS = GestorUtilitario.ParseBoolean(valor);
                    break;               


                default:  
                    mensaje = "Variable de configuracion " + constanteId + " no asignada";
                    exito = fillInformationDefault(constanteId);
                    Bitacora.Escribir(mensaje);
                    break;
            }
            return exito;
        }

        /// <summary>
        /// Asigna un valor por default si este no se encuentra en la Base de Datos
        /// </summary>
        /// <param name="constanteId"></param>
        /// <returns></returns>
        private bool fillInformationDefault(string constanteId)
        {
            string valor = string.Empty;
            string mensaje = string.Empty;
            bool exito = true;

            switch (constanteId)
            {
                case CG_PERMITREGMULTIDOCUMENTO:
                    valor = "1";
                    mensaje = excribirBitacora(constanteId, valor);
                    GestorDocumentos.PermiteRegistroMultiplesDocs = (GestorDocumentos.OpcionMultiple)byte.Parse(valor);
                    break;

                case CG_CANTDECIMALES:
                    valor = "3";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.CantidadDecimales = GestorUtilitario.ParseInt(valor);
                    break;

                case CG_MSJCONF_CREDITEXEDIDO:
                    valor = "Tiene el crédito excedido";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.MensajeCreditoExcedido = valor;
                    break;

                case CG_MSJCONF_FACVENCIDAS:
                    valor = "El cliente tiene facturas vencidas";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.MensajeFacturasVencidas = valor;
                    break;

                case CG_SIMBOLOMONET:
                    valor = "$";
                    mensaje = excribirBitacora(constanteId, valor);
                    GestorUtilitario.SimboloMonetario = valor;
                    AndroidDriver.simboloMoneda = valor;
                    break;

                case CG_ARCHIVOBITACORA:
                    valor = "FRmLog.txt";
                    mensaje = excribirBitacora(constanteId, valor);
                    Bitacora.NombreArchivo = valor;
                    break;

                case CG_PERMITINFODESCVENCIDA:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    Descuento.PermitirInfoDescuentosVencida = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_BUSCAGILCLIENTE:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.BusquedaAgilCliente = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_COBRO_APLICNOTACREDITO:
                    valor = "N"; // Facturas de contado y recibos en FR - KFC - cambio valor default SI -> N
                    mensaje = excribirBitacora(constanteId, valor);
                    Cobros.AplicarLasNotasCredito = valor;
                    break;

                case CG_COBRO_HABILMONTOTOTAL:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    Cobros.HabilitarMontoTotal = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_COBRO_CAMBIONUMRECIBO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Cobros.CambiarNumeroRecibo = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_MOSTRARREFEXISTEN:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MostrarRefExistencias = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_MAXLINEAPEDIDO:
                    valor = "510";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MaximoLineasDetalle = GestorUtilitario.ParseInt(valor);
                    Garantias.MaximoLineasDetalle = GestorUtilitario.ParseInt(valor);
                    break;

                case CG_PEDIDO_HABILITARDESCUENTO1:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.HabilitarDescuento1 = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_HABILITARDESCUENTO2:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.HabilitarDescuento2 = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_MAXDESCUENTOPERMIT1:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MaximoDescuentoPermitido1 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_PEDIDO_MAXDESCUENTOPERMIT2:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MaximoDescuentoPermitido2 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_PEDIDO_CRITBUSQUEDA:
                    valor = "1";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.CriterioBusquedaDefault = GestorUtilitario.ParseInt(valor);
                    break;

                case CG_PEDIDO_CAMBIARPRECIO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.CambiarPrecio = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_VERCOSTOARTICULO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MostrarCostoArticulo = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_VERUTILIDADDEFINIDA:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MostrarUtilidadDefinida = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_VERNUEVAUTILIDAD:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MostrarNuevaUtilidad = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CAMBIOBONIFIC:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.CambiarBonificacion = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CAMBIODESCUENTO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.CambiarDescuento = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_HABILITARDEVOLUCIONES:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    Devoluciones.HabilitarDevoluciones = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_DEVAUTOMATIC:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Devoluciones.DevolucionAutomatica = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_TIPODEV_HABILITARTIPO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Devoluciones.TiposDevoluciones = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEV_TIPODEV_TIPOS:
                    valor = "";
                    mensaje = excribirBitacora(constanteId, valor);
                    if (Devoluciones.tiposDevolucion != null)
                        Devoluciones.tiposDevolucion.Clear();
                    cargarTipos(valor);
                    break;
                case CG_DEPOSITO_CAMBIOMONTO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Deposito.CambioMonto = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DEPOSITO_DIFMAX:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Deposito.DiferenciaMax = Convert.ToDecimal(valor);
                    break;

                case CG_DEPOSITO_DIFMIN:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Deposito.DiferenciaMin = Convert.ToDecimal(valor);
                    break;

                case CG_TAMANO_GRID:
                    valor = "9";
                    mensaje = excribirBitacora(constanteId, valor);
                    GestorDocumentos.tamanoLetra = float.Parse(valor);
                    break;

                case PRINT_TIPO:
                    valor = "GENERIC24_180";
                    mensaje = excribirBitacora(constanteId, valor);
                    Impresora.IndicaImpresora(valor);
                    break;

                case PRINT_PUERTO:
                    valor = "COM7";
                    mensaje = excribirBitacora(constanteId, valor);
                    Impresora.IndicaPuerto(valor);
                    break;

                case PRINT_CANTCOPIAS:
                    valor = "1";
                    mensaje = excribirBitacora(constanteId, valor);
                    Impresora.CantidadCopias = Convert.ToInt32(valor);
                    break;

                case PRINT_CANTLINEAS:
                    valor = "50";
                    mensaje = excribirBitacora(constanteId, valor);
                    Impresora.CantidadLineas = Convert.ToInt32(valor);
                    break;

                case PRINT_SUGERIRIMPRIMIR:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Impresora.SugerirImprimir = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_SELECTCLIENTEAUTOMAT:
                    valor = "SI";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.SeleccionXCodBarras = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_USAR_TOPES:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.ManejaTopes = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_TOPE1:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.Tope1 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_PEDIDO_TOPE2:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.Tope2 = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_MSJCONF_TOPE1:
                    valor = "Usted ha excedido el Tope 1";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MsjTope1 = valor;
                    break;

                case CG_MSJCONF_TOPE2:
                    valor = "Usted ha excedido el Tope 2";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.MsjTope2 = valor;
                    break;

                case CG_PEDIDO_ADICIONBONIFIC:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.BonificacionAdicional = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CAMBIO_TECLADO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.CambioTeclado = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_TECLADO_DEFAULT:
                    valor = "1";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.TecladoDefecto = (EMF.WF.TextBoxInput)Convert.ToInt32(valor);
                    break;

                case CG_PEDIDO_TOMA_AUTOMATICA:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.TomaAutomatica = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_SUBFILTRO_FAMILIA:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.SubFiltroFamilia = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_PEDIDO_CALCULAR_IMPUESTOS:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.CalcularImpuestos = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_DESGLOSE_LOTES_FACTURAS:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.DesgloseLotesFacturaObliga = GestorUtilitario.ParseBoolean(valor);
                    break;

                // MejorasGrupoPelon600R6 - KF //
                case CG_ART_NO_NIV_PREC:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.ArtFueraNivPrecio = GestorUtilitario.ParseBoolean(valor);
                    break;
                case CG_PEDIDO_DATO_FAMILIA:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.DatoFamiliaMostrar = GestorUtilitario.ParseInt(valor);
                    break;

                #region Actualizacion
                case AC_ACTUALIZADA:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.PermiteActualizar = valor;
                    break;

                #endregion

                case CG_COBRO_DESC_PRONTO_PAGO:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    Cobros.AplicarDescuentosProntoPago = GestorUtilitario.ParseBoolean(valor);
                    break;

                #region Purga de documentos
                case CG_PURGA_DOC_AUTOMATICA:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.PurgarDocumentosAutomaticamente = GestorUtilitario.ParseBoolean(valor);
                    break;
                case CG_PURGA_DIAS_AUTOMATICA:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.PurgarCantidadDias = GestorUtilitario.ParseInt(valor);
                    break;
                case CG_PURGA_DOC_MANUAL:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.PugarDocumentosManualmente = GestorUtilitario.ParseBoolean(valor);
                    break;
                #endregion

                case CG_PEDIDO_CAMBIOPRECIO_MODO:
                    valor = "A";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.ModoCambiosPrecios = valor;
                    break;

                case CG_PEDIDO_LIMITECREDITO:
                    valor = "N";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.ValidarLimiteCredito = valor;
                    break;

                case CG_PEDIDO_DOC_VENCIDOS:
                    valor = "P";
                    mensaje = excribirBitacora(constanteId, valor);
                    Pedidos.ValidarDocVencidos = valor;
                    break;

                case CG_GUARDAR_UBI_CLIENTE:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.GuardarUbicacionClientes = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_GUARDAR_UBI_VISITA:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.GuardarUbicacionVisita = GestorUtilitario.ParseBoolean(valor);
                    break;

                case CG_ACTUALIZAR_UBI_CLIENTE:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.ActualizarUbicacionClientes = GestorUtilitario.ParseBoolean(valor);
                    break;
                case CG_DISTANCIA_UBI_CLIENTE:
                    valor = "0";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.DistanciaPermitidaUbicacion = GestorUtilitario.ParseDecimal(valor);
                    break;

                case CG_UBI_NO_PARAR:
                    valor = "NO";
                    mensaje = excribirBitacora(constanteId, valor);
                    FRmConfig.NoDetenerGPS = GestorUtilitario.ParseBoolean(valor);
                    break;

                //Facturas de contado y recibos en FR - KFC
                case CG_DEV_PERMITIR_TIPO_PAGO:
                    valor = "A";
                    mensaje = excribirBitacora(constanteId, valor);
                    Devoluciones.tipoPagoDevolucion = valor;
                    break;


                default:
                    exito = false;
                    mensaje += constanteId;
                    Bitacora.Escribir(mensaje);
                    break;
            }
            Bitacora.Escribir("No se le pudo asignar default a la variable " + mensaje);
            return exito;
        }

        private string excribirBitacora(string constanteId, string valor)
        {
            return constanteId + ". Se asigna el valor: " + valor + " , por default"; 
        }
        /// <summary>
        /// Muestra la informacion referente a la configuracion de la handHeld
        /// </summary>
        /// <param name="constanteId"></param>
        /// <param name="valor"></param>
        private bool fillInformationGlobal(string constanteId, string valor, ref string mensaje)
        {
            bool exito = true;

            switch (constanteId)
            {
                case TEXT_ETIQ_PED:
                    HandHeldConfig.labelsPedido = new string[8];
                    HandHeldConfig.labelsPedido = valor.Split('~');
                    break;
                case ORD_PED_COLS:
                    setColumnOrder(valor);
                    break;
                case DESGLOSE_LOTES:
                    Pedidos.DesgloseLotesFactura = valor == "S" ? true : false;
                    break;
                // MejorasGrupoPelon600R6 - KF //
                case DOC_A_GENERAR:
                    Pedidos.DocumentoGenerar = valor;
                    break;
                case UTILIZA_JORNADA_TRABAJO:
                    FRdConfig.UsaJornadaLaboral = valor == "S" ? true : false;
                    break;
                #region MejorasFRTostadoraElDorado600R6 JEV
                case UTILIZA_TOMA_FISICA:
                    FRdConfig.UtilizaTomaFisica = valor == "S" ? true : false;
                    break;
                case FACTURAR_DIFERENCIAS_TM:
                    FRdConfig.FacturarDiferencias = valor == "S" ? true : false;
                    break;
                case CLIENTE_RUTERO:
                    FRdConfig.ClienteRutero = valor;
                    break;
                #endregion MejorasFRTostadoraElDorado600R6 JEV

                // Modificaciones en funcionalidad de recibos de contado - KFC
                case FORMAS_GENERACION_RECIBOS:
                    FRdConfig.FormaGenerarRecibo = valor;
                    break;

                default:
                    mensaje = "Variable de configuracion " + constanteId + " no asignada";
                    exito = fillInformationGlobalDefault(constanteId);
                    Bitacora.Escribir(mensaje);
                    break;
            }
            return exito;
        }

        private bool fillInformationGlobalDefault(string constanteId)
        {
            string valor = string.Empty;
            string mensaje = string.Empty;
            bool exito = true;

            switch (constanteId)
            {
                case TEXT_ETIQ_PED:
                    valor = "Cant~Desc~Bonf~Adic~PreA~PreD~Subt~Totl";
                    HandHeldConfig.labelsPedido = new string[8];
                    HandHeldConfig.labelsPedido = valor.Split('~');
                    break;
                case ORD_PED_COLS:
                    valor="DESC~EMP~COD~PRED~-CANTD~-PREA~-CANTA";
                    setColumnOrder(valor);
                    break;
                // MejorasGrupoPelon600R6 - KF //
                case DOC_A_GENERAR:
                    valor = "A";
                    Pedidos.DocumentoGenerar = valor;
                    break;
                case UTILIZA_JORNADA_TRABAJO:
                    FRdConfig.UsaJornadaLaboral = false;
                    break;
                #region MejorasFRTostadoraElDorado600R6 JEV
                case UTILIZA_TOMA_FISICA:
                    FRdConfig.UtilizaTomaFisica = false;
                    break;
                case FACTURAR_DIFERENCIAS_TM:
                    FRdConfig.FacturarDiferencias = false;
                    break;
                case CLIENTE_RUTERO:
                    FRdConfig.ClienteRutero = string.Empty;
                    break;
                #endregion MejorasFRTostadoraElDorado600R6 JEV

                case FORMAS_GENERACION_RECIBOS:
                    FRdConfig.FormaGenerarRecibo = string.Empty;
                    break;

                default:
                    exito = false;
                    mensaje = "No se logro asignar Default a " + constanteId;
                    Bitacora.Escribir(mensaje);
                    break;
            }
            return exito;
        }
        
        
        private bool setColumnOrder(string valor)
        {

            //COD~DESC~EMP~PREA~PRED~CANTD~CANTA
            bool exito = true;
            try
            {

            //Muestr el valor default de las columnas en caso de q todas sean visibles
           string[] defaultOrden ={
                                    ColumnasPedido.COD.ToString(), 
                                    ColumnasPedido.DESC.ToString(),
                                    ColumnasPedido.EMP.ToString(),
                                    ColumnasPedido.PREA.ToString(),
                                    ColumnasPedido.PRED.ToString(),
                                    ColumnasPedido.CANTA.ToString(),
                                    ColumnasPedido.CANTD.ToString()
                                   };
            //Retorna las columnas que deben ser visibles
            string[] visibleColumn = getVisibleColumns(valor);
            HandHeldConfig.ordenDefaultColumnasPedido = getNewDefaultColumns(defaultOrden, visibleColumn);
            int[] ordenColumnas = new int[visibleColumn.Length];

            for (int i = 0; i < visibleColumn.Length; i++)
            {
                for (int j = 0; j < HandHeldConfig.ordenDefaultColumnasPedido.Length; j++)
                {
                    if (visibleColumn[i] == HandHeldConfig.ordenDefaultColumnasPedido[j])
                    {
                        ordenColumnas[i] = j;
                    }
                }
            }
                        
            HandHeldConfig.columnasVisiblePedido = visibleColumn;
            HandHeldConfig.ordenColumnasPedido = ordenColumnas;
            }
            catch (Exception)
            {
                exito = false;
            }
            return exito;
        }

        private string[] getNewDefaultColumns(string[] defaulColumns, string[] visibleColumns)
        {
            string[] newDefaulColumns = new string[visibleColumns.Length];
            int z=0;
            for (int i = 0; i < defaulColumns.Length; i++)
            {
                for (int j = 0; j < visibleColumns.Length; j++)
                {
                    if (defaulColumns[i] == visibleColumns[j])
                    {
                        newDefaulColumns[z++] = defaulColumns[i];
                    }
                } 
            }

        return newDefaulColumns;
        }

        private string[] getVisibleColumns(string columns)         
        { 
           string[] columnas = columns.Split('~');
           string[] cantNovisibles = columns.Split('-');
           int visibles = columnas.Length - cantNovisibles.Length + 1;
           string[] columnasVisibles = new string[visibles];
           int z = 0;
           for (int i = 0; i < columnas.Length; i++)
           {
               if (!columnas[i].Contains("-"))
               {
                   columnasVisibles[z++] = columnas[i];
               }
           }
           return columnasVisibles;        
        }

        private void setColumnaFija(string valor)
        {
            switch (valor)  
            {
                case "N":
                    HandHeldConfig.cg_columna_fija= FijarColumna.Ninguno.ToString();
                    break;
                case "C":
                    HandHeldConfig.cg_columna_fija = FijarColumna.Codigo.ToString();
                    break;
                case "D":
                    HandHeldConfig.cg_columna_fija = FijarColumna.Descripcion.ToString();
                    break;
                default:
                    break;
            }
        }

        private void cargarTipos(string valor)
        {            
            string[] tipos = valor.Split('~');
            for (int i = 0; i < tipos.Length; i++)
            {
                Devoluciones.cargarTipos(tipos[i]);
            }
        }

        public bool cargarConfiguracionHandHeld(ref string mensaje)
        {
            bool exito = true;
            List<OneHandHeldConfig> HandHeldConfig = ObtenerConfiguracionHandHeldBd();
            foreach (OneHandHeldConfig oneConfig in HandHeldConfig)
            {
                //En el momento que una variable no sea asignada, envia una advertencia
                //y no permite el ingreso al usuario
                if (!fillInformation(oneConfig.Constante, oneConfig.Valor, ref mensaje))
                {
                    exito = false;
                    break;
                }
            }
            return exito;
        }

        /// <summary>
        /// Carga las variables globales de las HandHeld
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public bool cargarConfiguracionGlobalHandHeld(ref string mensaje)
        {
            bool exito = true;
            List<OneHandHeldConfigGlobal> HandHeldConfig = ObtenerConfiguracionGlobalHandHeldBd();
            foreach (OneHandHeldConfigGlobal oneConfig in HandHeldConfig)
            {
                //En el momento que una variable no sea asignada, envia una advertencia
                //y no permite el ingreso al usuario
                if (!fillInformationGlobal(oneConfig.Nombre, oneConfig.Valor, ref mensaje))
                {
                    exito = false;
                    break;
                }
            }
            return exito;
        }

        #endregion

        #region Acceso Datos
        /// <summary>
        /// Carga los datos de la configuracion de la HandHeld
        /// </summary>
        /// <returns></returns>
        private List<OneHandHeldConfig> ObtenerConfiguracionHandHeldBd()
        {
            List<OneHandHeldConfig> HandHeldConfig = new List<OneHandHeldConfig>();
            HandHeldConfig.Clear();
            
            SQLiteDataReader reader = null;

            try
            {
                string sentencia =
                    " SELECT HANDHELD, CONSTANTE, VALOR " +
                    " FROM " + Table.ERPADMIN_CONFIG_HH;
                GestorDatos.Conectar();
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    OneHandHeldConfig oneHandHeldConfig = new OneHandHeldConfig();
                    oneHandHeldConfig.HandHeld = reader.GetString(0);
                    oneHandHeldConfig.Constante = reader.GetString(1);
                    #region MejorasFRTostadoraElDorado600R6 JEV
                    //oneHandHeldConfig.Valor = reader.GetString(2);
                    oneHandHeldConfig.Valor = !string.IsNullOrEmpty(reader.GetValue(2).ToString()) ? reader.GetString(2) : string.Empty;
                    #endregion MejorasFRTostadoraElDorado600R6 JEV
                    HandHeldConfig.Add(oneHandHeldConfig);
                }
               // GestorDatos.Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando configuracion de HandHeld. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return HandHeldConfig;
        }

        /// <summary>
        /// Carga los datos de la configuracion Global de la HandHeld
        /// </summary>
        /// <returns></returns>
        private List<OneHandHeldConfigGlobal> ObtenerConfiguracionGlobalHandHeldBd()
        {
            List<OneHandHeldConfigGlobal> HandHeldConfig = new List<OneHandHeldConfigGlobal>();
            HandHeldConfig.Clear();

            SQLiteDataReader reader = null;

            try
            {
                string sentencia =
                    " SELECT NOMBRE, VALOR" +
                    " FROM " + Table.ERPADMIN_GLOBALES_MODULO +
                    " WHERE MODULO = 'FR' ";
                GestorDatos.Conectar();
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    OneHandHeldConfigGlobal oneHandHeldConfig = new OneHandHeldConfigGlobal();
                    oneHandHeldConfig.Nombre = reader.GetString(0);
                    oneHandHeldConfig.Valor = reader.GetString(1);

                    HandHeldConfig.Add(oneHandHeldConfig);
                }
                // GestorDatos.Desconectar();
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando configuracion Global de HandHeld. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return HandHeldConfig;
        }
        #endregion
    }
}
