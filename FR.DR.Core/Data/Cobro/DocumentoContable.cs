using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using EMF.Printing;
using System.Collections;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Representa Documentos por cobrar y la aplicacion de movimientos
    /// </summary>
    public class DocumentoContable : Encabezado, IPrintable
    {
        #region Variables y Propiedades de instancia

        private TipoDocumento tipo;
        /// <summary>
        /// Tipo del documento.
        /// </summary>
        public TipoDocumento Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private decimal saldoDocLocal = 0;
        /// <summary>
        /// monto del saldo
        /// </summary>
        public decimal SaldoDocLocal
        {
            get { return saldoDocLocal; }
            set { saldoDocLocal = value; }
        }

        private decimal saldoDocDolar = 0;
        /// <summary>
        /// monto del saldo
        /// </summary>
        public decimal SaldoDocDolar
        {
            get { return saldoDocDolar; }
            set { saldoDocDolar = value; }
        }

        private decimal montoDocLocal = 0;
        /// <summary>
        /// Monto del documento
        /// </summary>
        public decimal MontoDocLocal
        {
            get { return montoDocLocal; }
            set { montoDocLocal = value; }
        }

        private decimal montoDocDolar = 0;
        /// <summary>
        /// monto del documento
        /// </summary>
        public decimal MontoDocDolar
        {
            get { return montoDocDolar; }
            set { montoDocDolar = value; }
        }

        private DateTime fechaRealizacion;
        /// <summary>
        /// Fecha Realizacion
        /// </summary>
        public DateTime FechaRealizacion
        {
            get { return fechaRealizacion; }
            set { fechaRealizacion = value; }
        }

        private DateTime fechaUltimoProceso;
        /// <summary>
        /// Fecha ultima modificacion
        /// </summary>
        public DateTime FechaUltimoProceso
        {
            get { return fechaUltimoProceso; }
            set { fechaUltimoProceso = value; }
        }

        private DateTime fechaVencimiento;
        /// <summary>
        /// Fecha vencimiento
        /// </summary>
        public DateTime FechaVencimiento
        {
            get { return fechaVencimiento; }
            set { fechaVencimiento = value; }
        }

        private TipoMoneda moneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Moneda del documento.
        /// </summary>
        public TipoMoneda Moneda
        {
            get { return moneda; }
            set { moneda = value; }
        }

        private EstadoDocumento estado = EstadoDocumento.Activo;
        /// <summary>
        /// Indice de anulacion del documento.
        /// </summary>
        public EstadoDocumento Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        private decimal tipoCambio = 0;
        /// <summary>
        /// Monto Tipo Cambio
        /// </summary>
        public decimal TipoCambio
        {
            get { return tipoCambio; }
            set { tipoCambio = value; }
        }
       
        private decimal montoMovimientoLocal = 0;
        /// <summary>
        /// monto del movimiento
        /// </summary>
        public decimal MontoMovimientoLocal
        {
            get { return montoMovimientoLocal; }
            set { montoMovimientoLocal = value; }
        }

        private decimal montoMovimientoDolar = 0;
        /// <summary>
        /// monto del movimiento
        /// </summary>
        public decimal MontoMovimientoDolar
        {
            get { return montoMovimientoDolar; }
            set { montoMovimientoDolar = value; }
        }

        private decimal montoAPagarDocLocal = 0;
        /// <summary>
        /// monto a pagar por el documento
        /// </summary>
        public decimal MontoAPagarDocLocal
        {
            get { return montoAPagarDocLocal; }
            set { montoAPagarDocLocal = value; }
        }

        private decimal montoAPagarDocDolar = 0;
        /// <summary>
        /// Monto a pagar por el documento
        /// </summary>
        public decimal MontoAPagarDocDolar
        {
            get { return montoAPagarDocDolar; }
            set { montoAPagarDocDolar = value; }
        }

        private bool procesado = false;
        /// <summary>
        /// indice de procesamiento
        /// </summary>
        public bool Procesado
        {
            get { return procesado; }
            set { procesado = value; }
        }

        private List<Cheque> chequesAsociados = new List<Cheque>();
        /// <summary>
        /// Cheques asociados al movimiento
        /// </summary>
        public List<Cheque> ChequesAsociados
        {
            get { return chequesAsociados; }
            set { chequesAsociados = value; }
        }

        private string condicionPago = String.Empty;
        /// <summary>
        /// Condicion de Pago
        /// </summary>
        public string CondicionPago
        {
            get { return condicionPago; }
            set { condicionPago = value; }
        }

        #endregion

        #region Logica Negocios

        /// <summary>
        /// Guarda en la base de datos los cheques asociados al documento.
        /// </summary>
        public void GuardarCheques()
        {
            if (Cobros.MontoCheques != 0)
            {
                foreach (Cheque cheque in this.ChequesAsociados)
                {
                    cheque.Recibo = this.Numero;
                    cheque.Compania = this.Compania;
                    cheque.Zona = this.Zona;
                    cheque.Guardar(this.tipo);
                }
            }
        }

        #endregion


        #region IPrintable Members

        public override string GetObjectName()
        {
            return "DOCUMENTO_CONTABLE";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                case "MONEDA": 
                    if(Moneda == TipoMoneda.LOCAL)
                        return "Local";
                    else
                        return "Dólar";

                case "MONTO": 
                    if(Moneda == TipoMoneda.LOCAL)
                        return this.MontoDocLocal;
                    else
                        return this.MontoDocDolar;

                case "CHEQUES": return new ArrayList(chequesAsociados);

                case "MONTO_MOVIMIENTO":
                    if (this.Moneda == TipoMoneda.LOCAL)
                        return this.MontoMovimientoLocal;
                    else
                        return this.MontoMovimientoDolar;

                case "MONTO_SALDO":
                        if (this.Moneda == TipoMoneda.LOCAL)
                            return this.SaldoDocLocal;
                        else
                            return this.SaldoDocDolar;

                case "FECHA": return FechaRealizacion;
                
                default: return base.GetField(name);
            }
        }
        #endregion
    }
}
