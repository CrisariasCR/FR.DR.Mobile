﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Softland.ERP.FR.Mobile.Interfaces
{
    /*
    public enum FormaCalcImpuesto1
    {
		/// <summary>
		/// Imp1 = suma(imp1 * (cant * precio - (desc1 + desc2)))
		/// </summary>
		Total,

		/// <summary>
		/// Imp1 = suma(imp1 * (cant * precio - descLinea))
		/// </summary>
		Lineas,

		/// <summary>
		/// Imp1 = suma(imp1 * (cant * precio - (desc1 + desc2 + descLinea)))
		/// </summary>
		Ambas,

		/// <summary>
		/// Imp1 = suma(imp1 * cant * precio)
		/// </summary>
		Ninguno,

        NoDefinido
    }

    #region MejorasFRTostadoraElDorado600R6 JEV
    //public enum EstadoBoletaInvFisico
    //{
    //    /// <summary>
    //    /// Boleta Pendiente de aplicar
    //    /// </summary>
    //    Pendiente = 'P',
    //    /// <summary>
    //    /// Boleta aplicada al inventario
    //    /// </summary>
    //    Aplicada = 'A',
    //    /// <summary>
    //    /// Boleta Facturada
    //    /// </summary>
    //    Facturada = 'F'
    //}
    #endregion MejorasFRTostadoraElDorado600R6 JEV

    public enum TipoMoneda
    {
        /// <summary>
        /// LOCAL = 'L'
        /// </summary>
        LOCAL = 'L',

        /// <summary>
        /// DOLAR = 'D'
        /// </summary>
        DOLAR = 'D'
    }

	public enum TipoDocumento
	{
		/// <summary>
		/// Factura de Contado
		/// </summary>
		FacturaContado = 0,

		/// <summary>
		/// Factura
		/// </summary>
		Factura = 1,

		/// <summary>
		/// Nota de Debito
		/// </summary>
		NotaDebito = 2,

		/// <summary>
		/// Nota de Credito Aux = 3
		/// </summary>
		/// <remarks>
		/// Solamente se usa en FRm para identificar los movimientos de N/C a N/C.
		/// O sea, ver como quedaron los saldos de las N/C luego de ser 
		/// aplicarlas a las facturas en un recibo de cobro.
		/// </remarks>
		NotaCreditoAux = 3,

		/// <summary>
		/// Recibo = 5
		/// </summary>
		Recibo = 5,

		/// <summary>
		/// NotaCredito = 7
		/// </summary>
		NotaCredito = 7,

        /// <summary>
        /// NotaCredito = 9.
        /// Se utiliza cuando se quiere crear una nueva nota de crédito desde la pocket
        /// </summary>
        NotaCreditoCrear = 9,

        /// <summary>
        /// Letra Cambio       
        /// </summary>
        LetraCambio = 10,

        /// <summary>
        /// Otro Debito       
        /// </summary>
        OtroDebito = 11,

        /// <summary>
        /// Intereses      
        /// </summary>
        Intereses = 12,

        /// <summary>
        /// Boleta de Venta       
        /// </summary>
        BoletaVenta = 13,

        /// <summary>
        /// Interes Corriente       
        /// </summary>
        InteresCorriente = 14,

        /// <summary>
        /// Nigun documento        
        /// </summary>
        TodosDocumentosDebito = -1,


        #region  Facturas de contado y recibos en FR - KFC

        /// <summary>
        /// Nota Credito generada en caliente NO APLICADA       
        /// </summary>
        NotaCreditoNueva = 15,       

        #endregion
    }

    public enum Impuesto1Incluido
    {

        /// <summary>
        /// Los precios incluye impuesto 1
        /// </summary>
        SI = 'S',

        /// <summary>
        /// Los precios no incluyen impuesto 1
        /// </summary>
        NO = 'N'
    }

    #region Enumeracion relativa al estado del pedido

    public enum EstadoPedido
    {
        Aprobado = 'A',
        Facturado = 'F',
        Normal = 'N',
        Backorder = 'B',
        Cancelado = 'C'
    }
    
    #region Enumeracion relativa a la clase de pedido

    public enum ClaseDoc
    {
        Normal = 'N',
        CreditoFiscal = 'C'
    }
    #region Enumeracion relativa al tipo del pedido

    public enum TipoPedido
    {
        Prefactura = '1',
        Factura = 'F'
    }
    public enum EDescuento
    {
        DESC1 = 1,
        DESC2 = 2
    }
    #endregion

    #endregion 

    #endregion

    #region Articulo

	/// <summary>
	/// Valor que indentifica que una linea de pedido es bonificada.
	/// </summary>
    public enum LineaBonificada
    { 
        
        BONIFICADA = 'B',
		NOBONIFICADA = '0'
    }


    /// <summary>
    /// Enumaracion para los tipos de descuentos.
    /// </summary>
    public enum TipoDescuento
    {
        /// <summary>
        /// Porcentual = 'P'. Indica que el descuento es por porcentaje.
        /// </summary>
        Porcentual = 'P',

        /// <summary>
        /// Fijo = 'F'. Indica que el descuento es por un monto fijo.
        /// </summary>
        Fijo = 'F'
    }

    /// <summary>
    /// Enumeración para los diferentes esquemas de descuentos y bonificaciones 
    /// soportados por Facturación de Rutero.
    /// </summary>
    public enum EsquemaDescuento
    {
        /// <summary>
        /// NivelesPrecio = 'N'
        /// </summary>
        NivelesPrecio = 'N',

        /// <summary>
        /// ClienteArticulo = 'C'
        /// </summary>
        ClienteArticulo = 'C',

        /// <summary>
        /// PaquetesReglas = 'P'
        /// </summary>
        PaquetesReglas = 'P'
    }


    /// <summary>
    /// Corresponde al estado del artículo.
    /// </summary>
    public enum EstadoArticulo
    {
        /// <summary>
        /// El artículo esta en buen estado. = 'B'
        /// </summary>
        Bueno = 'B',

        /// <summary>
        /// El artículo esta en mal estado. = 'M'
        /// </summary>
        Malo = 'M',

        /// <summary>
        /// El estado del artículo es no definido. = 'N'
        /// </summary>
        NoDefinido = 'N'
    }

    /// <summary>
    /// Posibles criterios de busqueda de un articulo
    /// </summary>
    public enum CriterioArticulo
    {
        Codigo,
        Barras,
        Descripcion,
        Clase,
        Familia,
        Ninguno
    }
    /// <summary>
    /// Filtros de busqueda para articulos
    /// </summary>
    public enum FiltroArticulo
    {
        Existencia,
        NivelPrecio,
        GrupoArticulo
    }

#endregion Articulo

    #region Visita

    public enum TipoVisita
    {
        /// <summary>
        /// Visita real = 'R'
        /// </summary>
        Real = 'R',

        /// <summary>
        /// Visita telefonica = 'T'
        /// </summary>
        Telefonica = 'T'
 
    }
#endregion Visita

    #region Consignacion

    /// <summary>
    /// Corresponde al estado que puede poseer una boleta de venta en consignación.
    /// </summary>
    public enum EstadoConsignacion
    {
        /// <summary>
        /// La boleta de venta en consignación no ha sido sincronizada.
        /// Valor = 'N'.
        /// </summary>
        NoSincronizada = 'N',
        /// <summary>
        /// La boleta de venta en consignación ha sido sincronizada.
        /// Valor = 'S'.
        /// </summary>
        Sincronizada = 'S',
        /// <summary>
        /// La boleta de venta en consignación ha sido procesada por el proceso de carga hacia el ERP.
        /// Valor = 'P'.
        /// </summary>
        Procesada = 'P',
        /// <summary>
        /// El estado no está definido.
        /// Valor = 'X'
        /// </summary>
        NoDefinido = 'X'
    }
#endregion Consignacion

    #region Documentos
 
    public enum DocumentoFR
    { 
        Factura = 'F',
        Consignacion = '9',
        Devolucion = '4',
        Pedido = '1',
        Recibo = '5',
        Inventario = '8',
        Ninguno
    }




#endregion Consignacion

    public enum Dias
    {
        L = 'L', K = 'K', M = 'M', J = 'J', V = 'V', S = 'S', D = 'D', T
    }

    public enum EstadoDocumento
    {
        /// <summary>
        /// Activo = 'A'
        /// </summary>
        Activo = 'A',

        /// <summary>
        /// Anulado = 'N'
        /// </summary>
        Anulado = 'N',

        /// <summary> KFC
        /// Contado = 'C'
        /// </summary>
        Contado = 'C'


    }

    public enum Consecutivo
    { 
        Factura,
        Inventario,
        Devolucion,
        PedidoDescuento,
        Recibo,
        Pedido,
        NotaCredito
    }

    public enum ConsecutivoNCF
    {
        ConsumidorFinal,
        Gubernamental,
        RegimenEspecial,
        Organizacion,
        Devolucion
    }    

    #region Cobro

    //Importante: El orden de este enum no debe ser modificado pues 
    //es utilizado como indice en un combo
    public enum TipoCobro
    {
        SeleccionFacturas,
        MontoTotal,
        FacturaActual, // Agregado KFC  prueba Facturas Contado
        Ninguno
    }

    
    #endregion Cobro

    #region AccesoDatos

    public enum Schema
    {
        FR,
        ALDO
    }
    
    /*public enum Table
    {
        ALDO_alCXC_BCO,
        ALDO_alCXC_CHE,
        ALDO_alCXC_CTA_BCO,
        ALDO_alCXC_DET_DEP,
        ALDO_alCXC_DOC_APL,
        ALDO_alCXC_ENC_DEP,
        ALDO_alCXC_MOV_DIR,
        ALDO_alCXC_PEN_COB,
        ALDO_alFAC_CND_PAG,
        ALDO_alFAC_DET_CONSIG,
        ALDO_alFAC_DET_DEV,
        ALDO_alFAC_DET_HIST_PED,
        ALDO_alFAC_DET_HIST_FAC,
        ALDO_alFAC_DET_INV,
        ALDO_alFAC_DET_PED,
        ALDO_alFAC_DIR_EMB_CLT,
        ALDO_alFAC_ENC_CONSIG,
        ALDO_alFAC_ENC_DEV,
        ALDO_alFAC_ENC_HIST_FAC,
        ALDO_alFAC_ENC_HIST_PED,
        ALDO_alFAC_ENC_INV,
        ALDO_alFAC_ENC_PED,
        ALDO_alFAC_PAIS,
        ALDO_alFAC_RUTA_ORDEN,
        ALDO_alFAC_RZN_VIS,
        //ALDO_alFAC_TIP_CLT,
        ALDO_alSYS_PRM,
        ALDO_ARTICULO_EXISTENCIA,
        ALDO_ARTICULO,
        ALDO_ARTICULO_PRECIO,
        ALDO_BONIFICACION_CLIART,
        ALDO_BONIFICACION_NIVEL,
        ALDO_CLIENTE_CIA,
        ALDO_CLIENTE,
        ALDO_COMPANIA,
        ALDO_DESCUENTO_CLAS, 
        ALDO_DESCUENTO_CLIART,
        ALDO_DESCUENTO_NIVEL,
        ALDO_GLOBALES_FR,
        ALDO_IMPUESTO,
        ALDO_NIVEL_LISTA,
        ALDO_RUTA_CFG,
        ALDO_SUGERIDOVENTA,
        ALDO_SUGERIDO_ASOC,
        ALDO_SUGERIDO_ASOC_LINEA,
        ALDO_VISITA,
        ALDO_VISITA_DOCUMENTO,
        SYSTEM_USUARIO,
        NO_DEF

    }

    public enum Table
    {
        ERPADMIN_alCXC_BCO,
        ERPADMIN_alCXC_CHE,
        ERPADMIN_alCXC_CTA_BCO,
        ERPADMIN_alCXC_DET_DEP,
        ERPADMIN_alCXC_DOC_APL,
        ERPADMIN_alCXC_ENC_DEP,
        ERPADMIN_alCXC_MOV_DIR,
        ERPADMIN_alCXC_PEN_COB,
        ERPADMIN_alFAC_CND_PAG,
        ERPADMIN_alFAC_DET_CONSIG,
        ERPADMIN_alFAC_DET_DEV,
        ERPADMIN_alFAC_DET_HIST_PED,
        ERPADMIN_alFAC_DET_HIST_FAC,
        ERPADMIN_alFAC_DET_INV,
        ERPADMIN_alFAC_DET_PED,
        ERPADMIN_alFAC_DIR_EMB_CLT,
        ERPADMIN_alFAC_ENC_CONSIG,
        ERPADMIN_alFAC_ENC_DEV,
        ERPADMIN_alFAC_ENC_HIST_FAC,
        ERPADMIN_alFAC_ENC_HIST_PED,
        ERPADMIN_alFAC_ENC_INV,
        ERPADMIN_alFAC_ENC_PED,
        ERPADMIN_alFAC_PAIS,
        ERPADMIN_alFAC_RUTA_ORDEN,
        ERPADMIN_alFAC_RZN_VIS,
        //ERPADMIN_alFAC_TIP_CLT,
        ERPADMIN_alSYS_PRM,
        ERPADMIN_ARTICULO_EXISTENCIA,
        ERPADMIN_ARTICULO,
        ERPADMIN_ARTICULO_PRECIO,
        ERPADMIN_BONIFICACION_CLIART,
        ERPADMIN_BONIFICACION_NIVEL,
        ERPADMIN_CLIENTE_CIA,
        ERPADMIN_CLIENTE,
        ERPADMIN_COMPANIA,
        ERPADMIN_DESCUENTO_CLAS,
        ERPADMIN_DESCUENTO_CLIART,
        ERPADMIN_DESCUENTO_NIVEL,
        ERPADMIN_GLOBALES_FR,
        ERPADMIN_IMPUESTO,
        ERPADMIN_NIVEL_LISTA,
        ERPADMIN_RUTA_CFG,
        ERPADMIN_SUGERIDOVENTA,
        ERPADMIN_SUGERIDO_ASOC,
        ERPADMIN_SUGERIDO_ASOC_LINEA,
        ERPADMIN_VISITA,
        ERPADMIN_VISITA_DOCUMENTO,
        SYSTEM_USUARIO,
        NO_DEF,
        ERPADMIN_CONFIG_HH,
        ERPADMIN_GLOBALES_MODULO,
        ERPADMIN_alFAC_DESC_PRONTO_PAGO,
        ERPADMIN_ARTICULO_EXISTENCIA_LOTE,
        ERPADMIN_LINEA_PED_LOTE_LOC,
        ERPADMIN_CLIENTE_UBICACION,
        ERPADMIN_CLIENTE_UBICACION_LOG,
        ERPADMIN_VISITA_UBICACION,
        ERPADMIN_DES_BON_PAQUETE,
        ERPADMIN_DES_BON_REGLA,
        ERPADMIN_DES_BON_PAQUETE_REGLA,
        ERPADMIN_DES_BON_ESCALA_BONIFICACION,
        ERPADMIN_DES_BON_ARTICULO,
        ERPADMIN_DES_BON_CLIENTE,
        ERPADMIN_CLASIFICACION_FR,
        ERPADMIN_JORNADA_RUTAS,
        ERPADMIN_NCF_CONSECUTIVO_RT,
        ERPADMIN_TOMA_FISICA_INV,
        SYSTEM_PRIVILEGIO_EX
    }

    #endregion AccesoDatos

    #region Configuracion

    public enum ColumnasPedido
    {
        COD,
        DESC,
        EMP,
        PREA,
        PRED,
        CANTD,
        CANTA
    }

    public enum FijarColumna
    {
        /// <summary>
        /// No fija ninguna columna. = 'N'
        /// </summary>
        Ninguno = 'N',

        /// <summary>
        /// Fija la columna de Codigo. = 'C'
        /// </summary>
        Codigo = 'C',
        /// <summary>
        /// Fija la columna de Descripcion. = 'D'
        /// </summary>
        Descripcion = 'D'
    }

    public enum PedidosLabels
    {
        Cantidad,
        Descuentos,
        Bonificacion,
        Adicional,
        PrecioAlmacen,
        PrecioDetalle,
        SubTotal,
        Total
    }
    
    #endregion Configuracion

    #region Reporte

    /// <summary>
    /// Reportes Moviles registrados
    /// </summary>
    public enum Rdl
    {
        //Resumenes
        ResumenFacturas,
        ResumenPedidos,
        ResumenDevoluciones,
        DetalleDevolucion,
        ResumenRecibos,

        //Detalles
        Factura,
        Pedido,
        VentaConsignacion,      
        DetalleRecibos,

        ReporteVisita,
        ReporteMontos,
        ReporteCierre,
        ReporteInventario,
        ReporteDevoluciones,
        ReporteJornada,
        ReporteInventarioTomaFisica
    }
    #endregion Reporte

    public enum TipoRegalia
    {
        Bonificacion = 'B',
        DescuentoLinea = 'L',
        DescuentoGeneral = 'G'
    }

    /// <summary>
    /// Campos de busqueda para un cliente
    /// </summary>
    public enum CriterioCliente
    {
        Codigo,
        Nombre,
        Zona,
        Ninguno
    }
    */

}
