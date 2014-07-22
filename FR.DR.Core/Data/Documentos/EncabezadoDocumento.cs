using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Collections.Generic;
using System;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using EMF.Printing;
using FR.DR.Core.Data.Documentos.Retenciones;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Clase que representa un encabezado de documento generico
    /// </summary>
    public abstract class EncabezadoDocumento : Encabezado//, IPrintable
    {
        #region Fechas Registro

        private DateTime horaInicio = DateTime.Now;
        /// <summary>
        /// Variable que indica la hora a la cual inicia el documento
        /// </summary>
        public DateTime HoraInicio
        {
            get { return horaInicio; }
            set { horaInicio = value; }
        }

        private DateTime horaFin = DateTime.Now;
        /// <summary>
        /// Variable que indica la hora a la cual finaliza el documento
        /// </summary>
        public DateTime HoraFin
        {
            get { return horaFin; }
            set { horaFin = value; }
        }

        private DateTime fechaRealizacion = DateTime.Now;
        /// <summary>
        /// Variable que indica la fecha en la cual se realiza el documento
        /// </summary>
        public DateTime FechaRealizacion
        {
            get { return fechaRealizacion; }
            set { fechaRealizacion = value; RaisePropertyChanged("FechaRealizacion"); }
        }
        #endregion

        #region Retenciones 

        private List<fclsRetencion> arregloRetenciones;
        /// <summary>
        /// Arreglo de retenciones.
        /// </summary>
        public List<fclsRetencion> iArregloRetenciones
        {
            get { return arregloRetenciones; }
            set { arregloRetenciones = value; }
        }
       

        private decimal totalRetenciones;
        /// <summary>
        /// Total Retenciones.
        /// </summary>
        public decimal inTotalRetenciones
        {
            get { return totalRetenciones; }
            set { totalRetenciones = value; }
        }



        #endregion

        #region Montos

        private decimal montoBruto = 0;
        /// <summary>
        /// Monto bruto.
        /// </summary>
        public decimal MontoBruto
        {
            get { return montoBruto; }
            set { montoBruto = value; }
        }

        /// <summary>
        /// Indica el monto neto: Monto Bruto - Total de Descuentos + Total de Impuestos
        /// </summary>
        public decimal MontoNeto
        {
            get
            {
                return this.MontoBruto - this.MontoTotalDescuento + this.Impuesto.MontoTotal-this.inTotalRetenciones;
            }
        }

        public decimal MontoSubTotal
        {
            get
            {
                return this.MontoBruto - this.MontoTotalDescuento;
            }
        }        

        #endregion
        
        #region Descuentos

        /// <summary>
        /// Descuento total aplicado al pedido.
        /// Descuentos x linea + descuento 1 + descuento 2.
        /// </summary>
        public decimal MontoTotalDescuento
        {
            get { 
                return montoDescuento + montoDescuento1 + montoDescuento2; }
        }

        private decimal montoDescuento = 0;
        /// <summary>
        /// Monto de descuento por todas las lineas.
        /// </summary>
        public decimal MontoDescuentoLineas
        {
            get { return montoDescuento; }
            set { montoDescuento = value; }
        }

        private decimal montoDescuento1 = 0;
        /// <summary>
        /// Monto de descuento 1.
        /// </summary>
        public decimal MontoDescuento1
        {
            get { return montoDescuento1; }
            set { montoDescuento1 = value; }
        }

        private decimal porcentajeDescuento1 = 0;
        /// <summary>
        /// Porcentaje del descuento 1 expresando como entero (0 - 100)
        /// </summary>
        public virtual decimal PorcentajeDescuento1
        {
            get { return porcentajeDescuento1; }
            set { porcentajeDescuento1 = value; }
        }

        private decimal montoDescuento2 = 0;
        /// <summary>
        /// Monto de descuento 2.
        /// </summary>
        public decimal MontoDescuento2
        {
            get { return montoDescuento2; }
            set { montoDescuento2 = value; }
        }

        private decimal porcentajeDescuento2 = 0;
        /// <summary>
        /// Porcentaje del descuento 2 expresando como entero (0 - 100)
        /// </summary>
        public virtual decimal PorcentajeDescuento2
        {
            get { return porcentajeDescuento2; }
            set { porcentajeDescuento2 = value; }
        }

        #endregion 

        #region Impuesto

        private Impuesto impuesto = new Impuesto();
        /// <summary>
        /// Llevar control de los Montos de los impuestos.
        /// </summary>
        public Impuesto Impuesto
        {
            get { return impuesto; }
            set { impuesto = value; }
        }

        #endregion

        #region Bodega

        private string bodega = string.Empty;
        /// <summary>
        /// Indica la bodega asociada.
        /// </summary>
        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }

        #endregion

        #region NivelPrecios

        private int nivelPrecio = 0;
        /// <summary>
        /// Nivel de precio asociada.
        /// </summary>
        public int NivelPrecio
        {
            get { return nivelPrecio; }
            set { nivelPrecio = value; }
        }

        #endregion

        #region Observaciones

        private string notas = string.Empty;
        /// <summary>
        /// Nota u observacion al documento
        /// </summary>
        public string Notas
        {
            get { return notas; }
            set { notas = value; }
        }

        #endregion

        #region Metodos

        #region Retenciones


        #endregion

        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "ENCABEZADO_DOCUMENTO";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                case "FECHA": return this.FechaRealizacion;
                case "SUB_TOTAL": return this.MontoBruto;
                case "DESCUENTO_UNO": return this.MontoDescuento1;
                case "DESCUENTO_DOS": return this.MontoDescuento2;
                case "DESCUENTO": return this.MontoDescuentoLineas;
                case "IMP_VENTA": return this.Impuesto.MontoImpuesto1;
                case "IMP_CONSUMO": return this.Impuesto.MontoImpuesto2;
                //Caso: 32380 ABC 14/05/2008 Disponibilizar nuevos datos para reporte Venta en Consignación(Monto en letras)
                case "TOTAL_LETRAS": return GestorUtilitario.NumeroALetras(this.MontoNeto);
                case "TOTAL": return this.MontoNeto;
                case "RETENCIONES": return this.inTotalRetenciones;
                case "TOTAL_BRUTO": return this.montoBruto;
                case "SUBTOTAL": return this.MontoSubTotal;
                case "OBSERVACIONES": return this.notas;
                default: return base.GetField(name);
            }
        }

        #endregion

    }
}
