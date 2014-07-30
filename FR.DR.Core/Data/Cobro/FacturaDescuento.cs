using System;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    public class FacturaDescuento
    {
        #region Atributos
        /// <summary>
        /// Factura asocidad al descuento.
        /// </summary>
        private Factura factura;
        /// <summary>
        /// Recibo asociado al descuento.
        /// </summary>
        private Recibo recibo;

        /// <summary>
        /// Porcentage del descuento.
        /// </summary>
        private decimal porcentage = Decimal.Zero;

        /// <summary>
        /// Monto del descuento.
        /// </summary>
        private decimal monto = Decimal.Zero;

        /// <summary>
        /// Indica si se aplica el descuento a la factura.
        /// </summary>
        private bool aplicaDescuento = false;

        private NotaCredito notaCredito;
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene la factura asociada al descuento.
        /// </summary>
        public Factura Factura
        {
            get { return factura; }
        }
        /// <summary>
        /// Obtiene el recibo asociado al descuento.
        /// </summary>
        public Recibo Recibo
        {
            get { return recibo; }
        }
        /// <summary>
        /// Tipo documento al que se aplica el descuento.
        /// </summary>
        public TipoDocumento TipoDocumento
        {
            get { return factura.Tipo; }
        }

        public string TipoDoc
        {
            get { return factura.Tipo.ToString(); }
        }
        /// <summary>
        /// Numero del documento asociado al descuento.
        /// </summary>
        public string Documento
        {
            get
            {
                return factura.Numero;
            }
        }
        /// <summary>
        /// Porcentage de descuento por aplicar al documento.
        /// </summary>
        public decimal Porcentage
        {
            get
            {
                return Decimal.Round(porcentage,2);
            }
        }
        /// <summary>
        /// Monto del descuento por aplicar al documento.
        /// </summary>
        public decimal Monto
        {
            get
            {
                return Decimal.Round(monto,2);
            }
        }
        /// <summary>
        /// Indica si aplica descuento a la factura.
        /// </summary>
        public bool AplicaDescuento
        {
            get { return aplicaDescuento; }
        }

        /// <summary>
        /// Obtiene la nota de crédio asociada al descuento. Se inicializa después de guardar el descuento.
        /// </summary>
        public NotaCredito NotaCredito
        {
            get
            {
                return notaCredito;
            }
        }
        #endregion

        #region Constructores
        public FacturaDescuento(Factura factura, Recibo recibo)
        {
            this.factura = factura;
            this.recibo = recibo;
            CalcularMonto();
        }
        public FacturaDescuento(Factura factura, Recibo recibo, bool MontoTotal)
        {
            this.factura = factura;
            this.recibo = recibo;
            CalcularMontoTotales();
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Calcula el monto del descuento.
        /// </summary>
        private void CalcularMonto()
        {
            string condicion = factura.CondicionPago;
            int dias = ObtenerDiferenciaDias();
            DescuentoProntoPago descuento =
                DescuentoProntoPago.ObtenerDescuentoProntoPago(factura.Compania, condicion, dias);
            aplicaDescuento = false;
            if (descuento != null)
            {
                porcentage = descuento.Descuento;
                decimal montoAPagar = decimal.Zero;
                if (Cobros.TipoCobro == TipoCobro.MontoTotal)
                {
                    aplicaDescuento = true;
                    montoAPagar = Cobros.TipoMoneda == TipoMoneda.LOCAL ? factura.MontoDocLocal : factura.MontoDocDolar;
                }
                else
                {                 
                    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    {
                        aplicaDescuento = (factura.MontoDocLocal == factura.MontoAPagarDocLocal) ||
                           ((factura.MontoDocLocal >= factura.MontoAPagarDocLocal) && !FRdConfig.ProntoPagoTotales);
                        montoAPagar = aplicaDescuento ?
                            factura.MontoAPagarDocLocal : decimal.Zero;
                    }
                    else
                    {
                        aplicaDescuento = (factura.MontoDocDolar == factura.MontoAPagarDocDolar) ||
                            ((factura.MontoDocDolar >= factura.MontoAPagarDocDolar) && !FRdConfig.ProntoPagoTotales);
                        montoAPagar = aplicaDescuento ?
                            factura.MontoAPagarDocDolar : decimal.Zero;
                    }
                }
                
                monto = montoAPagar * porcentage / 100; 
            }
        }

        /// <summary>
        /// Calcula el monto del descuento.
        /// </summary>
        private void CalcularMontoTotales()
        {
            string condicion = factura.CondicionPago;
            int dias = ObtenerDiferenciaDias();
            DescuentoProntoPago descuento =
                DescuentoProntoPago.ObtenerDescuentoProntoPago(factura.Compania, condicion, dias);
            aplicaDescuento = false;
            if (descuento != null)
            {
                porcentage = descuento.Descuento;
                decimal montoAPagar = decimal.Zero;
                if (Cobros.TipoCobro == TipoCobro.MontoTotal)
                {
                    aplicaDescuento = true;
                    montoAPagar = Cobros.TipoMoneda == TipoMoneda.LOCAL ? factura.MontoMovimientoLocal : factura.MontoMovimientoDolar;
                }
                else
                {
                    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    {
                        aplicaDescuento = (factura.MontoDocLocal == factura.MontoAPagarDocLocal) ||
                            ((factura.MontoDocLocal >= factura.MontoAPagarDocLocal) && !FRdConfig.ProntoPagoTotales);
                        montoAPagar = aplicaDescuento ?
                            factura.MontoAPagarDocLocal : decimal.Zero;
                    }
                    else
                    {
                        aplicaDescuento = (factura.MontoDocDolar == factura.MontoAPagarDocDolar) ||
                            ((factura.MontoDocDolar >= factura.MontoAPagarDocDolar) && !FRdConfig.ProntoPagoTotales);
                        montoAPagar = aplicaDescuento ?
                            factura.MontoAPagarDocDolar : decimal.Zero;
                    }
                }

                monto = montoAPagar * porcentage / 100;
            }
        }


        /// <summary>
        /// Obitene la diferencia de dias entre hoy y la fecha de realización de la factura.
        /// </summary>
        /// <returns></returns>
        private int ObtenerDiferenciaDias()
        {
            TimeSpan time = DateTime.Now.Subtract(factura.FechaRealizacion);
            return time.Days;
        }

        /// <summary>
        /// Guarda la nota de crédito asociada a la factura.
        /// </summary>
        public bool Guardar(string numeroRecibo)
        {
            bool resultado = false;
            try
            {
                string consecutivo = GestorUtilitario.proximoCodigo(
                   ParametroSistema.ObtenerNotaCredito(factura.Compania, factura.Zona), 20);
                notaCredito = new NotaCredito(consecutivo, recibo.Moneda, factura.Compania
                    , factura.Zona, recibo.Cliente);

                //Modificaciones en funcionalidad de generacion de recibos de contado - KFC
                factura.MontoAPagarDocLocal = factura.MontoDocLocal;
                factura.MontoAPagarDocLocal -= factura.MontoMovimientoLocal;
                factura.MontoAPagarDocLocal -= factura.TotalNotasCredito;

                factura.MontoAPagarDocDolar = factura.MontoDocDolar;
                factura.MontoAPagarDocDolar -= factura.MontoMovimientoDolar;
                factura.MontoAPagarDocDolar -= factura.TotalNotasCredito / Cobros.TipoCambio;

                if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                {
                    notaCredito.MontoMovimientoLocal = monto;
                    notaCredito.MontoMovimientoDolar = monto / Cobros.TipoCambio;
                    factura.AplicaNotaLocal(notaCredito);
                }
                else
                {
                    notaCredito.MontoMovimientoDolar = monto;
                    notaCredito.MontoMovimientoLocal = monto * Cobros.TipoCambio;
                    factura.AplicaNotaDolar(notaCredito);
                }
                notaCredito.Tipo = factura.Tipo;
                notaCredito.SaldoDocDolar = notaCredito.SaldoDocLocal = 0;
                notaCredito.GuardarPorDescuento(numeroRecibo,TipoDocumento.NotaCreditoCrear,factura.Numero);
                ParametroSistema.IncrementarNotaCredito(factura.Compania, factura.Zona);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return resultado;
        }


        #endregion
    }
}
