using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;

namespace Softland.ERP.FR.Mobile.Cls.Cobro
{
    /// <summary>
    /// Gestor de cobros
    /// </summary>
    public class Cobros
    {

        #region Variables de clase

        public static List<Factura> facturasSeleccionadas = new List<Factura>();

        // KFC - Facturas de contado
        public static List<NotaCredito> notasSeleccionadas = new List<NotaCredito>();

        private static decimal montoFacturasLocal = 0;
        /// <summary>
        ///  Monto que el cliente tiene pendiente de pagar.
        /// </summary>
        public static decimal MontoFacturasLocal
        {
            get { return Cobros.montoFacturasLocal; }
            set { Cobros.montoFacturasLocal = value; }
        }
        
        private static decimal montoFacturasDolar = 0;
        /// <summary>
        /// Monto que el cliente tiene pendiente de pagar en dolares.
        /// </summary>
        public static decimal MontoFacturasDolar
        {
            get { return Cobros.montoFacturasDolar; }
            set { Cobros.montoFacturasDolar = value; }
        }

        private static decimal montoNotasCreditoLocal = 0;
        /// <summary>
        /// Monto que hay disponible para el cliente en notas de crédito;
        /// </summary>
        public static decimal MontoNotasCreditoLocal
        {
            get { return Cobros.montoNotasCreditoLocal; }
            set { Cobros.montoNotasCreditoLocal = value; }
        }
        
        private static decimal montoNotasCreditoDolar = 0;
        /// <summary>
        /// Monto en dolar que hay disponible para el cliente en notas de crédito;
        /// </summary>
        public static decimal MontoNotasCreditoDolar
        {
            get { return Cobros.montoNotasCreditoDolar; }
            set { Cobros.montoNotasCreditoDolar = value; }
        }

        // Facturas de contado y recibos en FR - KFC
        private static decimal montoNotasCreditoSelLocal = 0;
        /// <summary>
        /// Monto en local seleccionado en notas de credito para un cliente;
        /// </summary>
        public static decimal MontoNotasCreditoSelLocal
        {
            get { return Cobros.montoNotasCreditoSelLocal; }
            set { Cobros.montoNotasCreditoSelLocal = value; }
        }

        private static decimal montoNotasCreditoSelDolar = 0;
        /// <summary>
        /// Monto en dolar seleccionado en notas de credito para un cliente;
        /// </summary>
        public static decimal MontoNotasCreditoSelDolar
        {
            get { return Cobros.montoNotasCreditoSelDolar; }
            set { Cobros.montoNotasCreditoSelDolar = value; }
        }        

        private static decimal montoEfectivo = 0;
        /// <summary>
        /// Monto en efectivo que se va a pagar
        /// </summary>
        public static decimal MontoEfectivo
        {
            get { return Cobros.montoEfectivo; }
            set { Cobros.montoEfectivo = value; }
        }
        private static decimal montoCheques = 0;
        /// <summary>
        /// monto en cheques que se paga
        /// </summary>
        public static decimal MontoCheques
        {
            get { return Cobros.montoCheques; }
            set { Cobros.montoCheques = value; }
        }
        private static decimal montoTotalPagar = 0;
        /// <summary>
        /// // monto total que se paga 
        /// </summary>
        public static decimal MontoTotalPagar
        {
            get { return Cobros.montoTotalPagar; }
            set { Cobros.montoTotalPagar = value; }
        }
        private static decimal montoPendiente = 0;
        /// <summary>
        /// monto que queda pendiente por pagar de la cuenta
        /// </summary>
        public static decimal MontoPendiente
        {
            get { return Cobros.montoPendiente; }
            set { Cobros.montoPendiente = value; }
        }
        private static decimal montoNotasCreditoSeleccion = 0;
        /// <summary>
        /// Monto de notas de crédito que se utiliza en el pago de facturas seleccionadas;
        /// </summary>
        public static decimal MontoNotasCreditoSeleccion
        {
            get { return Cobros.montoNotasCreditoSeleccion; }
            set { Cobros.montoNotasCreditoSeleccion = value; }
        }
        private static decimal montoFacturasSeleccion = 0;
        /// <summary>
        /// Monto de las facturas que se seleccionaron para pagar.
        /// </summary>
        public static decimal MontoFacturasSeleccion
        {
          get { return Cobros.montoFacturasSeleccion; }
          set { Cobros.montoFacturasSeleccion = value; }
        }
        private static decimal montoSumaChequeEfectivo = 0;
        /// <summary>
        /// Monto de suma de cheques con el efectivo, exceptuando notas de credito
        /// </summary>
        public static decimal MontoSumaChequeEfectivo
        {
            get { return Cobros.montoSumaChequeEfectivo; }
            set { Cobros.montoSumaChequeEfectivo = value; }
        }

        private static Recibo recibo ;
        /// <summary>
        /// Recibo asociado al cobro
        /// </summary>
        public static Recibo Recibo
        {
            get { return Cobros.recibo; }
            set { Cobros.recibo = value; }
        }

        private static decimal tipoCambio = 0;
        /// <summary>
        /// Indica el tipo de cambio del dolar de la compania a la que se le gestiona un cobro.
        /// </summary>
        public static decimal TipoCambio
        {
            get { return Cobros.tipoCambio; }
            set { Cobros.tipoCambio = value; }
        }

        private static TipoMoneda tipoMoneda = TipoMoneda.LOCAL;
        /// <summary>
        /// Indica el tipo de moneda con que se le gestiona un cobro.
        /// </summary>
        public static TipoMoneda TipoMoneda
        {
            get { return Cobros.tipoMoneda; }
            //set { Cobros.tipoMoneda = value; }
        }
        private static TipoCobro tipoCobro = TipoCobro.MontoTotal;
        /// <summary>
        /// Indica el tipo de cobro a realizar
        /// </summary>
        public static TipoCobro TipoCobro
        {
            get { return Cobros.tipoCobro; }
            set { Cobros.tipoCobro = value; }
        }
        /// <summary>
        /// Monto de descuentos por pronto pago.
        /// </summary>
        private static Decimal montoDescuentoProntoPagoLocal = Decimal.Zero;
        /// <summary>
        /// Obtiene el monto de descuentos por pronto pago.
        /// </summary>
        public static Decimal MontoDescuentoProntoPagoLocal
        {
            get { return Cobros.montoDescuentoProntoPagoLocal; }
            set { Cobros.montoDescuentoProntoPagoLocal = value; }
        }

        /// <summary>
        /// Monto de descuentos por pronto pago.
        /// </summary>
        private static Decimal montoDescuentoProntoPagoDolar = Decimal.Zero;
        /// <summary>
        /// Obtiene el monto de descuentos por pronto pago.
        /// </summary>
        public static Decimal MontoDescuentoProntoPagoDolar
        {
            get { return Cobros.montoDescuentoProntoPagoDolar; }
            set { Cobros.montoDescuentoProntoPagoDolar = value; }
        }

        //Proyecto Descuento Pronto Pago Parciales

        public static decimal montoTotalPagarObtener { get { return montoTotalPagar; } }

        public static decimal montoTotalPagarTemporal = 0;

        public static decimal montoTotalPagarTemporalDolar = 0;

        public static decimal montoEntradaEfectivo = 0;

        public static decimal montoEntradaCheque = 0;

        public static decimal montoEntradaNC = 0;

        #region Config.xml

        
        /*
        private static bool aplicarLasNotasCredito;
        /// <summary>
        /// Indica si se deben aplicar las notas de credito o no cuando se gestiona un cobro.
        /// Variable extraida de Config.xml.
        /// </summary>
        public static bool AplicarLasNotasCredito
        {
            get { return Cobros.aplicarLasNotasCredito; }
            set { Cobros.aplicarLasNotasCredito = value; }
        }*/

        // Facturas de contado y recibos en FR - KFC
        private static string aplicarLasNotasCredito;
        /// <summary>
        /// Indica si se deben aplicar las notas de credito o no cuando se gestiona un cobro.
        /// Variable extraida de Config.xml.
        /// </summary>
        public static string AplicarLasNotasCredito
        {
            get { return Cobros.aplicarLasNotasCredito; }
            set { Cobros.aplicarLasNotasCredito = value; }
        }

        private static bool habilitarMontoTotal;
        /// <summary>
        /// Indica si se permite realizar cobros por monto total.
        /// Variable extraida de Config.xml.
        /// </summary>
        public static bool HabilitarMontoTotal
        {
            get { return Cobros.habilitarMontoTotal; }
            set { Cobros.habilitarMontoTotal = value; }
        }

        private static bool cambiarNumeroRecibo; 
        /// <summary>
        /// ABC 35771
        /// Indica si permite cambiar el numero del consecutivo de recibo
        /// </summary>
        public static bool CambiarNumeroRecibo
        {
            get { return Cobros.cambiarNumeroRecibo; }
            set { Cobros.cambiarNumeroRecibo = value; }
        }

        private static string numeroReciboIndicado;
        /// <summary>
        /// ABC 35771
        /// Numero de Recibo asociado, indicado por el usuario
        /// </summary>
        public static string NumeroReciboIndicado
        {
            get { return Cobros.numeroReciboIndicado; }
            set { Cobros.numeroReciboIndicado = value; }
        }

        /// <summary>
        /// Indica si aplican los descuentos por pronto pago.
        /// </summary>
        private static bool aplicarDescuentosProntoPago = true;
        /// <summary>
        /// Indica si aplican los descuentos por pronto pago.
        /// </summary>
        public static bool AplicarDescuentosProntoPago
        {
            get { return Cobros.aplicarDescuentosProntoPago; }
            set { Cobros.aplicarDescuentosProntoPago = value; }
        }

        #endregion Config.xml

        #endregion

        #region Logica Negocio

        #region Sacar Cuentas

        /// <summary>
        /// Esta función hace las respectivas operaciones para mostrar los datos del cobro al cliente
        /// </summary>
        public static void SacarCuentas()
        {
            //le asigan la suma del monto de los cheques efectivo a una variable global
            Cobros.montoSumaChequeEfectivo = Cobros.montoCheques + Cobros.montoEfectivo;

            switch (Cobros.tipoCobro)
            {
                case TipoCobro.SeleccionFacturas:                    

                    //Sacamos el monto pendiente de acuerdo a la moneda
                    if (tipoMoneda == TipoMoneda.LOCAL)
                    {
                        Cobros.montoPendiente = Cobros.montoFacturasSeleccion - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito=="A")
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoLocal;
                        if (aplicarLasNotasCredito == "S")
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoSelLocal;
                        if (aplicarDescuentosProntoPago)
                            Cobros.montoPendiente -= Cobros.montoDescuentoProntoPagoLocal;

                    }
                    else
                    {
                        Cobros.montoPendiente = Cobros.montoFacturasSeleccion - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                            Cobros.montoPendiente -= montoNotasCreditoDolar;
                        if (aplicarLasNotasCredito == "S")
                            Cobros.montoPendiente -= montoNotasCreditoSelDolar;
                        if (aplicarDescuentosProntoPago)
                            Cobros.montoPendiente -= Cobros.montoDescuentoProntoPagoDolar;
                    }
                    break;
                case TipoCobro.MontoTotal:

                    //Cobros.montoTotalPagar = Cobros.montoSumaChequeEfectivo;//monto total le asignan la suma de cheques y efectivo // kf agrego las notas de credito seleccionadas
                    Cobros.montoTotalPagar = Cobros.montoSumaChequeEfectivo + Cobros.MontoNotasCreditoSelLocal;

                    //Sacamos el monto pendiente de acuerdo al tipo de moneda
                    if (tipoMoneda == TipoMoneda.LOCAL)
                    {
                        //Facturas de contado y recibos en FR - KFC
                        Cobros.montoPendiente = Cobros.montoFacturasLocal - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoLocal;
                        if (aplicarLasNotasCredito == "S")
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoSelLocal;
                        if (aplicarDescuentosProntoPago)
                            Cobros.montoPendiente -= Cobros.montoDescuentoProntoPagoLocal;
                    }
                    else
                    {
                        Cobros.montoPendiente = montoFacturasDolar - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                            Cobros.montoPendiente -= montoNotasCreditoDolar;
                        if (aplicarLasNotasCredito == "S")
                            Cobros.montoPendiente -= montoNotasCreditoSelDolar;
                        if (aplicarDescuentosProntoPago)
                            Cobros.montoPendiente -= Cobros.montoDescuentoProntoPagoDolar;
                    }

                    break;
                case TipoCobro.FacturaActual:

                    //Cobros.montoTotalPagar = Cobros.montoSumaChequeEfectivo;//monto total le asignan la suma de cheques y efectivo // kf agrego las notas de credito seleccionadas
                    Cobros.montoTotalPagar = Cobros.montoSumaChequeEfectivo;
                    

                    //Sacamos el monto pendiente de acuerdo al tipo de moneda
                    if (tipoMoneda == TipoMoneda.LOCAL)
                    {
                        Cobros.montoPendiente = Cobros.montoFacturasLocal - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                        {
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoLocal;
                            //Cobros.montoTotalPagar += Cobros.MontoNotasCreditoSelLocal;
                        }
                        if (aplicarLasNotasCredito == "S")
                        {
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoSelLocal;
                            Cobros.montoTotalPagar += Cobros.MontoNotasCreditoSelLocal;
                        }                        
                    }
                    else
                    {
                        Cobros.montoPendiente = montoFacturasDolar - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                            Cobros.montoPendiente -= montoNotasCreditoDolar;
                        if (aplicarLasNotasCredito == "S")
                        {
                            Cobros.montoTotalPagar += Cobros.MontoNotasCreditoSelDolar;
                            Cobros.montoPendiente -= montoNotasCreditoSelDolar;
                        }
                    }

                    break;
            }

            //Redondeamos a 2 decimales para evitar montos demasiado pequenos
            Cobros.montoPendiente = (decimal)Math.Round(Cobros.montoPendiente, 2);

            if (Cobros.montoPendiente < 0)
                Cobros.montoPendiente = 0;// si monto pendiente menor a 0 se le asigna 0
        }


        /// <summary>
        /// Esta función hace las respectivas operaciones para mostrar los datos del cobro al cliente
        /// </summary>
        public static void SacarCuentasMontoTotales()
        {
            //le asigan la suma del monto de los cheques efectivo a una variable global
            Cobros.montoSumaChequeEfectivo = Cobros.montoCheques + Cobros.montoEfectivo;

            switch (Cobros.tipoCobro)
            {
                case TipoCobro.MontoTotal:

                    //Cobros.montoTotalPagar = Cobros.montoSumaChequeEfectivo;//monto total le asignan la suma de cheques y efectivo // kf agrego las notas de credito seleccionadas
                    Cobros.montoTotalPagarTemporal = (Cobros.montoSumaChequeEfectivo - montoEntradaEfectivo - montoEntradaCheque) + (Cobros.MontoNotasCreditoSelLocal - Cobros.montoEntradaNC);
                    Cobros.montoTotalPagarTemporalDolar = Math.Round(Cobros.montoTotalPagarTemporal, 2);

                    //Sacamos el monto pendiente de acuerdo al tipo de moneda
                    if (tipoMoneda == TipoMoneda.LOCAL)
                    {
                        //Facturas de contado y recibos en FR - KFC
                        Cobros.montoPendiente = Cobros.montoFacturasLocal - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoLocal;
                        if (aplicarLasNotasCredito == "S")
                            Cobros.montoPendiente -= Cobros.montoNotasCreditoSelLocal;
                        if (aplicarDescuentosProntoPago)
                            Cobros.montoPendiente -= Cobros.montoDescuentoProntoPagoLocal;
                    }
                    else
                    {
                        Cobros.montoPendiente = montoFacturasDolar - Cobros.montoSumaChequeEfectivo;

                        //Facturas de contado y recibos en FR - KFC
                        //if (aplicarLasNotasCredito)
                        if (aplicarLasNotasCredito == "A")
                            Cobros.montoPendiente -= montoNotasCreditoDolar;
                        if (aplicarLasNotasCredito == "S")
                            Cobros.montoPendiente -= montoNotasCreditoSelDolar;
                        if (aplicarDescuentosProntoPago)
                            Cobros.montoPendiente -= Cobros.montoDescuentoProntoPagoDolar;
                    }

                    break;

            }
        }


        #endregion


        #region AplicarPago

        /// <summary>
        /// Funcion que aplica el pago del recibo dependiendo del tipo de cobro que sea realiza difertentes 
        /// funcionalidades.
        /// </summary>
        public static Recibo AplicarPago()
        {
            //ABC 35771
            if (!Cobros.CambiarNumeroRecibo)
            {
                //invoca la funcion que me trae el numero de recibo
                recibo.Numero = ParametroSistema.ObtenerRecibo(recibo.Compania, recibo.Zona);

                if (recibo.Numero == string.Empty)
                    throw new Exception("No se encontró siguiente consecutivo disponible.");
            }
            else
                recibo.Numero = NumeroReciboIndicado;
				
            recibo.Moneda = tipoMoneda;
            List<Factura> facturas = new List<Factura>();
            List<NotaCredito> notas = new List<NotaCredito>();

            switch (Cobros.tipoCobro)
            {
                case TipoCobro.SeleccionFacturas:
                    facturas = Cobros.facturasSeleccionadas;
                    break;
                case TipoCobro.MontoTotal:
                    facturas = Factura.ObtenerFacturasPendientesCobro(recibo.Compania,recibo.Cliente,recibo.Zona);
                    break;
            }


            if (Cobros.AplicarDescuentosProntoPago)
            {
                AplicarDescuentos(facturas);
            }

            //Facturas de contado y recibos en FR - KFC
            //if (Cobros.aplicarLasNotasCredito)
            if (aplicarLasNotasCredito == "A")
            {
                notas = NotaCredito.ObtenerNotasCredito(recibo.Compania, recibo.Cliente, recibo.Zona);
                if (notas.Count > 0)
                    AplicaNotasCredito(facturas, notas);
            }
            // Si se aplican notas de credito seleccionadas
            if (aplicarLasNotasCredito == "S")
            {
                notas = notasSeleccionadas;
                if (notas.Count > 0)
                    AplicaNotasCredito(facturas, notas);
                //notasSeleccionadas.Clear();
            }

            /*if (Cobros.AplicarDescuentosProntoPago)
            {
                AplicarDescuentos(facturas);
            }*/


            recibo.NotasCreditoAsociadas = notas;
            recibo.FacturasAsociadas = facturas;

            if (tipoMoneda == TipoMoneda.LOCAL)
            {                
                recibo.MontoChequesLocal = Cobros.montoCheques;
                recibo.MontoChequesDolar = recibo.MontoChequesLocal / Cobros.TipoCambio;
                recibo.MontoEfectivoLocal = Cobros.montoEfectivo;
                recibo.MontoEfectivoDolar = recibo.MontoEfectivoLocal / Cobros.TipoCambio;
                recibo.MontoDocLocal = recibo.MontoEfectivoLocal + recibo.MontoChequesLocal;
                recibo.MontoDocDolar = recibo.MontoChequesDolar + recibo.MontoEfectivoDolar;
            }
            else
            {
                recibo.MontoChequesDolar = Cobros.montoCheques;
                recibo.MontoChequesLocal = recibo.MontoChequesDolar * Cobros.TipoCambio;
                recibo.MontoEfectivoDolar = Cobros.montoEfectivo;
                recibo.MontoEfectivoLocal = recibo.MontoEfectivoDolar * Cobros.TipoCambio;
                recibo.MontoDocLocal = recibo.MontoEfectivoLocal + recibo.MontoChequesLocal;
                recibo.MontoDocDolar = recibo.MontoChequesDolar + recibo.MontoEfectivoDolar;
            }

            foreach (Factura factura in facturas)
            {
                pagaFacturas(factura);
                if (factura.MontoMovimientoLocal != 0 || factura.NotasCreditoAplicadas.Count>0)
                    recibo.AgregarDetalle(factura);
            }

            foreach (NotaCredito nota in notas)
            {
                if (nota.MontoMovimientoLocal != 0)
                    recibo.AgregarDetalle(nota);
            }

            try
            {
                //Se limpia la lista de N/C seleccionadas - KFC
                notasSeleccionadas.Clear();
                recibo.Guardar();
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando recibo. " + ex.Message);
            }

            return recibo;
        }


        /// <summary> 
        /// Funcion que aplica el pago del recibo dependiendo del cobro de contado.
        /// Facturas de contado y recibos en FR - KFC
        /// </summary>
        public static Recibo AplicarPagoContado(Pedido pedido)
        {
            if (!Cobros.CambiarNumeroRecibo)
            {
                //invoca la funcion que me trae el numero de recibo
                recibo.Numero = ParametroSistema.ObtenerRecibo(recibo.Compania, recibo.Zona);

                if (recibo.Numero == string.Empty)
                    throw new Exception("No se encontró siguiente consecutivo disponible.");
            }
            else
                recibo.Numero = NumeroReciboIndicado;

            recibo.Moneda = tipoMoneda;
            List<NotaCredito> notas = new List<NotaCredito>();

            

            if (aplicarLasNotasCredito == "A")
            {
                notas = NotaCredito.ObtenerNotasCredito(recibo.Compania, recibo.Cliente, recibo.Zona);
                // metodo sobrecargado 
                AplicaNotasCredito(pedido, notas); 
            }
            // si se aplican notas de credito seleccionadas
            if (aplicarLasNotasCredito == "S") 
            {
                notas = notasSeleccionadas;
                AplicaNotasCredito(pedido, notas);
                //notasSeleccionadas.Clear();
            }

            //No se contemplan descuentos por pronto pago para la cancelacion de un documento de contado. - KFC
            
            recibo.NotasCreditoAsociadas = notas;

            if (tipoMoneda == TipoMoneda.LOCAL)
            {
                recibo.MontoChequesLocal = Cobros.montoCheques;
                recibo.MontoChequesDolar = recibo.MontoChequesLocal / Cobros.TipoCambio;
                recibo.MontoEfectivoLocal = Cobros.montoEfectivo;
                recibo.MontoEfectivoDolar = recibo.MontoEfectivoLocal / Cobros.TipoCambio;
                recibo.MontoDocLocal = recibo.MontoEfectivoLocal + recibo.MontoChequesLocal;
                recibo.MontoDocDolar = recibo.MontoChequesDolar + recibo.MontoEfectivoDolar;
            }
            else
            {
                recibo.MontoChequesDolar = Cobros.montoCheques;
                recibo.MontoChequesLocal = recibo.MontoChequesDolar * Cobros.TipoCambio;
                recibo.MontoEfectivoDolar = Cobros.montoEfectivo;
                recibo.MontoEfectivoLocal = recibo.MontoEfectivoDolar * Cobros.TipoCambio;
                recibo.MontoDocLocal = recibo.MontoEfectivoLocal + recibo.MontoChequesLocal;
                recibo.MontoDocDolar = recibo.MontoChequesDolar + recibo.MontoEfectivoDolar;
            }

            foreach (NotaCredito nota in notas)
            {
                if (nota.MontoMovimientoLocal != 0)
                    recibo.AgregarDetalle(nota);
            }

            recibo.NumeroPedido = pedido.Numero;
            recibo.Pedido = pedido;
            recibo.Estado = EstadoDocumento.Contado;

            try
            {                
                recibo.Guardar();                
                notasSeleccionadas.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando recibo. " + ex.Message);
            }

            return recibo;
        }

        #endregion
        #region ProyectoGasZ
        /// <summary> 
        /// Funcion que obtiene el recibo de contado para Garantias.CAC
        /// </summary>
        public static Recibo AplicarPagoContadoGarantia(Garantia pedido,string compania,Cliente cliente)
        {
            Cobros.IniciarCobro(cliente, compania);
            if (!Cobros.CambiarNumeroRecibo)
            {
                //invoca la funcion que me trae el numero de recibo
                recibo.Numero = ParametroSistema.ObtenerRecibo(recibo.Compania, recibo.Zona);

                if (recibo.Numero == string.Empty)
                    throw new Exception("No se encontró siguiente consecutivo disponible.");
            }
            else
                recibo.Numero = NumeroReciboIndicado;

			if(pedido.Moneda.Equals("L"))
				recibo.Moneda = TipoMoneda.LOCAL;
			else
				recibo.Moneda = TipoMoneda.DOLAR;

            if (tipoMoneda == TipoMoneda.LOCAL)
            {
                recibo.MontoChequesLocal = 0;
                recibo.MontoChequesDolar = 0;
				recibo.MontoEfectivoLocal = pedido.MontoNeto;
                recibo.MontoEfectivoDolar = recibo.MontoEfectivoLocal / Cobros.TipoCambio;
                recibo.MontoDocLocal = recibo.MontoEfectivoLocal + recibo.MontoChequesLocal;
                recibo.MontoDocDolar = recibo.MontoChequesDolar + recibo.MontoEfectivoDolar;
            }
            else
            {
                recibo.MontoChequesDolar = 0;
                recibo.MontoChequesLocal = 0;
				recibo.MontoEfectivoDolar = pedido.MontoNeto;
                recibo.MontoEfectivoLocal = recibo.MontoEfectivoDolar * Cobros.TipoCambio;
                recibo.MontoDocLocal = recibo.MontoEfectivoLocal + recibo.MontoChequesLocal;
                recibo.MontoDocDolar = recibo.MontoChequesDolar + recibo.MontoEfectivoDolar;
            }

            recibo.NumeroPedido = pedido.Numero;
            //recibo.Pedido = pedido;
            recibo.Estado = EstadoDocumento.Contado;

            return recibo;
        }
        #endregion


        #region PagaFacturas

        /// <summary>
        /// funcion que realiza el pago a las facturas
        /// </summary>
        /// <param name="factura">recibe la factura que se va a pagar</param>
        private static void pagaFacturas(Factura factura)
        {
            switch (Cobros.tipoCobro)
            {
                case TipoCobro.SeleccionFacturas:
                    if (Cobros.montoSumaChequeEfectivo != 0)//verifica que el monto de la suma de cheques y efectivo no sea 0
                    {
                        if (tipoMoneda == TipoMoneda.LOCAL)
                        {
                            #region Caso CR2-11678-F70M JEV
                            if (Cobros.montoSumaChequeEfectivo > factura.MontoAPagarDocLocal)
                            //if (Cobros.montoSumaChequeEfectivo > factura.SaldoDocLocal)
                            #endregion Caso CR2-11678-F70M JEV
                            {

                                Cobros.montoSumaChequeEfectivo -= factura.MontoAPagarDocLocal;//le resta al monto pagado el saldo de la factura
                                factura.MontoMovimientoLocal = factura.MontoAPagarDocLocal;//le asigna al movimiento de la factura el total del saldo de la factura
                                factura.MontoMovimientoDolar = factura.MontoAPagarDocDolar;
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;

                            }
                            else
                            {
                                factura.MontoAPagarDocLocal -= Cobros.montoSumaChequeEfectivo;//le resta al saldo de la factura el monto pagado
                                factura.MontoAPagarDocDolar -= Cobros.montoSumaChequeEfectivo / Cobros.TipoCambio;
                                factura.MontoMovimientoLocal += Cobros.montoSumaChequeEfectivo;// al movimineto de la factura le suma el monto pagado
                                factura.MontoMovimientoDolar += Cobros.montoSumaChequeEfectivo / Cobros.TipoCambio;
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;
                                Cobros.montoSumaChequeEfectivo = 0;// el monto de la suma de cheques y efectivos se iguala a 0
                            }
                        }
                        else
                        {	//Moneda DOLAR

                            #region Caso CR2-11678-F70M JEV
                            if (Cobros.montoSumaChequeEfectivo > factura.MontoAPagarDocDolar)
                            //if (Cobros.montoSumaChequeEfectivo > factura.SaldoDocDolar)
                            #endregion Caso CR2-11678-F70M JEV 
                            {
                                Cobros.montoSumaChequeEfectivo -= factura.MontoAPagarDocDolar;//le resta al monto pagado el saldo de la factura
                                factura.MontoMovimientoDolar = factura.MontoAPagarDocDolar;//le asigna al movimiento de la factura el total del saldo de la factura
                                factura.MontoMovimientoLocal = factura.MontoMovimientoDolar * Cobros.TipoCambio;
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;
                            }
                            else
                            {
                                factura.MontoAPagarDocDolar -= Cobros.montoSumaChequeEfectivo;//le resta al saldo de la factura el monto pagado
                                factura.MontoAPagarDocLocal -= Cobros.montoSumaChequeEfectivo * Cobros.TipoCambio;
                                factura.MontoMovimientoDolar += Cobros.montoSumaChequeEfectivo;// al movimineto de la factura le suma el monto pagado
                                factura.MontoMovimientoLocal += Cobros.montoSumaChequeEfectivo * Cobros.TipoCambio;
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;
                                Cobros.montoSumaChequeEfectivo = 0;// el monto de la suma de cheques y efectivos se iguala a 0
                            }
                        }
                    }
                    break;
                case TipoCobro.MontoTotal:

                    if (Cobros.montoTotalPagar != 0)//verifica que el monto total a pagar sea distinto a 0
                    {
                        if (tipoMoneda == TipoMoneda.LOCAL)
                        {
                            if (Cobros.montoTotalPagar > factura.SaldoDocLocal)
                            {
                                Cobros.montoTotalPagar -= factura.SaldoDocLocal;//el monto total a pagar se le resta el saldo de la factura
                                factura.MontoMovimientoLocal = factura.SaldoDocLocal;//el movimiento de la factura se iguala a el saldo total de la factura.
                                factura.MontoMovimientoDolar = factura.SaldoDocDolar;
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;//el saldo de la factura se iguala a 0
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;
                            }
                            else
                            {
                                factura.SaldoDocLocal -= Cobros.montoTotalPagar;//el saldo de la factuura se le resta el total a pagar
                                factura.SaldoDocDolar -= Cobros.montoTotalPagar / Cobros.TipoCambio;
                                factura.MontoMovimientoLocal += Cobros.montoTotalPagar;//el movimiento de la facutura se le suma el monto total a pagar
                                factura.MontoMovimientoDolar += Cobros.montoTotalPagar / Cobros.TipoCambio;
                                Cobros.montoTotalPagar = 0;//el total a pagar se iguala a 0
                            }
                        }
                        else
                        {	//La moneda es DOLAR
                            if (Cobros.montoTotalPagar > factura.SaldoDocDolar)
                            {

                                Cobros.montoTotalPagar -= factura.SaldoDocDolar;//el monto total a pagar se le resta el saldo de la factura
                                factura.MontoMovimientoDolar = factura.SaldoDocDolar;//el movimiento de la factura se iguala a el saldo total de la factura.
                                factura.MontoMovimientoLocal = factura.SaldoDocDolar * Cobros.TipoCambio;
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;//el saldo de la factura se iguala a 0
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;
                            }
                            else
                            {
                                factura.SaldoDocDolar -= Cobros.montoTotalPagar;//el saldo de la factuura se le resta el total a pagar
                                factura.SaldoDocLocal -= Cobros.montoTotalPagar * Cobros.TipoCambio;
                                factura.MontoMovimientoDolar += Cobros.montoTotalPagar;//el movimiento de la facutura se le suma el monto total a pagar
                                factura.MontoMovimientoLocal += Cobros.montoTotalPagar * Cobros.TipoCambio;
                                Cobros.montoTotalPagar = 0;//el total a pagar se iguala a 0
                            }
                        }
                    }
                    break;
            }

            //Redondeamos para evitar saldos demasiado pequenos
            factura.SaldoDocLocal = (decimal)Math.Round(factura.SaldoDocLocal, 2);
            if (factura.SaldoDocLocal < 0)
                factura.SaldoDocLocal = 0;

            //Redondeamos para evitar saldos demasiado pequenos
            factura.SaldoDocDolar = (decimal)Math.Round(factura.SaldoDocDolar, 2);
            if (factura.SaldoDocDolar < 0)
                factura.SaldoDocDolar = 0;
        }

        /// <summary>
        /// funcion que realiza el pago a las facturas
        /// </summary>
        /// <param name="factura">recibe la factura que se va a pagar</param>
        public static void pagaFacturasMontoTotales(Factura factura)
        {

            switch (Cobros.tipoCobro)
            {
                case TipoCobro.MontoTotal:

                    if (factura.SaldoDocLocal != 0 && factura.SaldoDocDolar != 0 && Cobros.montoTotalPagarTemporal != 0)//verifica que el monto total a pagar sea distinto a 0
                    {
                        if (tipoMoneda == TipoMoneda.LOCAL)
                        {
                            if (Cobros.montoTotalPagarTemporal > factura.SaldoDocLocal)
                            {
                                Cobros.montoTotalPagarTemporal -= factura.SaldoDocLocal;//el monto total a pagar se le resta el saldo de la factura
                                factura.MontoMovimientoLocal = factura.SaldoDocLocal;//el movimiento de la factura se iguala a el saldo total de la factura.
                                factura.MontoMovimientoDolar = factura.SaldoDocDolar;
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;//el saldo de la factura se iguala a 0
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;
                            }
                            else
                            {
                                factura.SaldoDocLocal -= Cobros.montoTotalPagarTemporal;//el saldo de la factuura se le resta el total a pagar
                                factura.SaldoDocDolar -= Math.Round((Cobros.montoTotalPagarTemporal / Cobros.TipoCambio), 2);
                                factura.MontoMovimientoLocal += Cobros.montoTotalPagarTemporal;//el movimiento de la facutura se le suma el monto total a pagar
                                factura.MontoMovimientoDolar += Math.Round((Cobros.montoTotalPagarTemporal / Cobros.TipoCambio), 2);
                                Cobros.montoTotalPagarTemporal = 0;//el total a pagar se iguala a 0
                            }
                        }
                        else
                        {	//La moneda es DOLAR
                            if (Cobros.montoTotalPagarTemporalDolar > factura.SaldoDocDolar)
                            {

                                Cobros.montoTotalPagarTemporalDolar -= factura.SaldoDocDolar;//el monto total a pagar se le resta el saldo de la factura
                                factura.MontoMovimientoDolar = factura.SaldoDocDolar;//el movimiento de la factura se iguala a el saldo total de la factura.
                                factura.MontoMovimientoLocal = factura.SaldoDocDolar * Cobros.TipoCambio;
                                factura.SaldoDocDolar -= factura.MontoMovimientoDolar;//el saldo de la factura se iguala a 0
                                factura.SaldoDocLocal -= factura.MontoMovimientoLocal;
                            }
                            else
                            {
                                factura.SaldoDocDolar -= Cobros.montoTotalPagarTemporalDolar;//el saldo de la factuura se le resta el total a pagar
                                factura.SaldoDocLocal -= Math.Round((Cobros.montoTotalPagarTemporalDolar * Cobros.TipoCambio), 2);
                                factura.MontoMovimientoDolar += Cobros.montoTotalPagarTemporalDolar;//el movimiento de la facutura se le suma el monto total a pagar
                                factura.MontoMovimientoLocal += Math.Round((Cobros.montoTotalPagarTemporalDolar * Cobros.TipoCambio), 2);
                                Cobros.montoTotalPagarTemporalDolar = 0;//el total a pagar se iguala a 0
                            }
                        }
                    }
                    break;
            }

            //Redondeamos para evitar saldos demasiado pequenos
            factura.SaldoDocLocal = (decimal)Math.Round(factura.SaldoDocLocal, 2);
            if (factura.SaldoDocLocal < 0)
                factura.SaldoDocLocal = 0;

            //Redondeamos para evitar saldos demasiado pequenos
            factura.SaldoDocDolar = (decimal)Math.Round(factura.SaldoDocDolar, 2);
            if (factura.SaldoDocDolar < 0)
                factura.SaldoDocDolar = 0;
        }

        #endregion

	
        /// <summary>
        /// Esta funcion es la que permite la aplicacion de notas de credito por medio del tipo de pago
        /// monto total
        /// </summary>
        /// <param name="facturas">Lista de facturas a las que hay que aplicar las notas</param>
        /// <param name="notasCredito">Lista de notas de credito que se desean aplicar</param>
        public static void AplicaNotasCredito(List<Factura> facturas, List<NotaCredito> notasCredito)
        {
            foreach (Factura factura in facturas)
            {
                //Cuando el cobro es por seleccion de facturas el usuario digito cuanto queria cancelar
                //en cada una de las facturas seleccionadas.
                //Cuando el cobro es por monto total los montos a pagar son el propio saldo de la factura
                if (Cobros.tipoCobro == TipoCobro.MontoTotal)
                {
                    //le asigna al monto a pagar por factura el saldo
                    factura.MontoAPagarDocLocal = factura.SaldoDocLocal;
                    factura.MontoAPagarDocDolar = factura.SaldoDocDolar;
                }

                #region Caso CR2-11678-F70M JEV
                // KFC - Se restan los montos en efectivo por si el monto de notas de credito es superior
                //       al monto a cancelar, se apliquen las notas desde la mas antigua dejando el saldo en la ultima nota utilizada
                //factura.MontoAPagarDocLocal -= Cobros.MontoSumaChequeEfectivo;
                //factura.MontoAPagarDocDolar -= Cobros.MontoSumaChequeEfectivo / Cobros.TipoCambio;
                #endregion Caso CR2-11678-F70M JEV

                           
                foreach (NotaCredito nota in notasCredito)
                {
                    if (nota.SaldoDocLocal != 0)//valida que el saldo de la nota sea diferente a 0
                    {
                        //valida que el monto a pagar por documento sea distinto a 0
                        //Si es cero salimos ya que no hay mas que cancelar a la factura
                        if (factura.MontoAPagarDocLocal == 0) break;

                        if (TipoMoneda.LOCAL == tipoMoneda)
                            factura.AplicaNotaLocal(nota);//invoca al metodo que le aplica la nota de credito a la factura
                        else
                            factura.AplicaNotaDolar(nota);//invoca al metodo que le aplica la nota de credito a la factura
                    }
                }
            }
        }


        #region  Facturas de contado y recibos en FR - KFC


        
        /// <summary>
        /// Metodo para aplicar las notas de credito en una factura de contado        
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="notasCredito"></param>
        public static void AplicaNotasCredito(Pedido pedido, List<NotaCredito> notasCredito)
        {
            Factura factura = new Factura();
           
            //le asigna al monto a pagar por factura el saldo
            factura.MontoAPagarDocLocal = pedido.MontoNeto;
            factura.MontoAPagarDocDolar = Math.Round(pedido.MontoNeto / Cobros.TipoCambio,2);

            // KFC - Se restan los montos en efectivo por si el monto de notas de credito es superior
            //       al monto a cancelar, se apliquen las notas desde la mas antigua dejando el saldo en la ultima nota utilizada
            factura.MontoAPagarDocLocal -= Cobros.MontoSumaChequeEfectivo;
            factura.MontoAPagarDocDolar -= Math.Round(Cobros.MontoSumaChequeEfectivo / Cobros.TipoCambio,2);

            foreach (NotaCredito nota in notasCredito)
            {
                if (nota.SaldoDocLocal != 0)//valida que el saldo de la nota sea diferente a 0
                {
                    //valida que el monto a pagar por documento sea distinto a 0
                    //Si es cero salimos ya que no hay mas que cancelar a la factura
                    if (factura.MontoAPagarDocLocal == 0) break;

                    if (TipoMoneda.LOCAL == tipoMoneda)
                        factura.AplicaNotaLocal(nota);//invoca al metodo que le aplica la nota de credito a la factura
                    else
                        factura.AplicaNotaDolar(nota);//invoca al metodo que le aplica la nota de credito a la factura
                }
            }
        }

        #endregion


        private static void AplicarDescuentos(List<Factura> facturas)
        {
            foreach (Factura factura in facturas)
            {
                FacturaDescuento descuento = Cobros.BuscarDescuento(factura);
                if (descuento != null && descuento.AplicaDescuento)
                {
                    if (Cobros.TipoMoneda == TipoMoneda.LOCAL)
                    {
                        factura.SaldoDocLocal -= descuento.Monto;
                        factura.SaldoDocDolar -= descuento.Monto / Cobros.TipoCambio;

                        // Modificaciones en Recibos de Contado - KFC
                        // Se agrega para el manejo de las notas de credito por pronto pago
                        factura.MontoAPagarDocLocal -= descuento.Monto;
                        factura.MontoAPagarDocDolar -= descuento.Monto / Cobros.TipoCambio;
                        
                    }
                    else
                    {
                        factura.SaldoDocLocal -= descuento.Monto * Cobros.TipoCambio;
                        factura.SaldoDocDolar -= descuento.Monto;

                        // Modificaciones en Recibos de Contado - KFC
                        factura.MontoAPagarDocLocal -= descuento.Monto * Cobros.TipoCambio;
                        factura.MontoAPagarDocDolar -= descuento.Monto;
                       
                    }
                }
            }

        }

        /// <summary>
        /// Guarda el cobro como un abono a la cuenta del cliente.
        /// </summary>
        public static void CrearNotaCredito()
        {
            //ABC 35771
            if (!Cobros.CambiarNumeroRecibo)
            {
                //invoca la funcion que me trae el numero de recibo o Nota de credito
                recibo.Numero = ParametroSistema.ObtenerRecibo(recibo.Compania, recibo.Zona);

                if (recibo.Numero == string.Empty)
                    throw new Exception("No se encontró siguiente consecutivo disponible.");
            }
            else
                recibo.Numero = NumeroReciboIndicado;


            NotaCredito notaCredito = new NotaCredito(recibo.Numero, tipoMoneda, recibo.Compania,recibo.Zona,recibo.Cliente);

            if (tipoMoneda == TipoMoneda.LOCAL)
            {
                notaCredito.MontoEfectivoLocal = MontoEfectivo;
                notaCredito.MontoEfectivoDolar = notaCredito.MontoEfectivoLocal / TipoCambio;

                notaCredito.MontoChequesLocal = MontoCheques;
                notaCredito.MontoChequesDolar = notaCredito.MontoChequesLocal / TipoCambio;

                notaCredito.MontoDocLocal = notaCredito.MontoEfectivoLocal+ notaCredito.MontoChequesLocal;
                notaCredito.MontoDocDolar = notaCredito.MontoEfectivoDolar + notaCredito.MontoChequesDolar;
            }
            else
            {
                notaCredito.MontoEfectivoDolar = MontoEfectivo;
                notaCredito.MontoEfectivoLocal = notaCredito.MontoEfectivoDolar * TipoCambio;

                notaCredito.MontoChequesDolar = MontoCheques;
                notaCredito.MontoChequesLocal = notaCredito.MontoChequesDolar * TipoCambio;

                notaCredito.MontoDocLocal = notaCredito.MontoEfectivoLocal + notaCredito.MontoChequesLocal;
                notaCredito.MontoDocDolar = notaCredito.MontoEfectivoDolar + notaCredito.MontoChequesDolar;
            }

            try
            {
                notaCredito.Guardar();
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando el abono. " + ex.Message);
            }
        }

        /// <summary>
        /// Inicializa las variables necesarias para crear un cobro
        /// </summary>
        public static void IniciarCobro(Cliente cliente, string compania)
        {
            Cobros.FinalizarCobro();
            Compania cia = Compania.Obtener(compania);
            Cobros.TipoCambio = cia.TipoCambio;
            recibo = new Recibo();
            recibo.FechaInicio = DateTime.Now;
            recibo.Cliente =cliente.Codigo;
            recibo.Zona =cliente.Zona;
            recibo.Compania = compania;
            recibo.TipoCambio = Cobros.TipoCambio;
        }

        /// <summary>
        /// Limpia las variables relacionadas con la gestion de cobro
        /// </summary>
        public static void FinalizarCobro()
        {
            montoCheques = 0;
            montoEfectivo = 0;
            montoFacturasLocal = 0;
            montoFacturasDolar = 0;
            montoFacturasSeleccion = 0;
            montoNotasCreditoLocal = 0;
            montoNotasCreditoDolar = 0;
            montoNotasCreditoSeleccion = 0;
            montoPendiente = 0;
            montoSumaChequeEfectivo = 0;
            montoTotalPagar = 0;
            tipoCobro = TipoCobro.MontoTotal;
            recibo = null;
            montoDescuentoProntoPagoDolar = Decimal.Zero;
            montoDescuentoProntoPagoLocal = Decimal.Zero;

            //Facturas de Contado y Recibos FR - KFC
            MontoNotasCreditoSelLocal = 0;
            MontoNotasCreditoSelDolar = 0;
        }

        #region Descuento Pronto Pago
        /// <summary>
        /// Busca que el descuento que tiene asociado la factura.
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        private static FacturaDescuento BuscarDescuento(Factura factura)
        {
            FacturaDescuento resultado = null;
            foreach(FacturaDescuento descuento in Cobros.Recibo.Descuentos)
            {
                if (descuento.Factura != null && descuento.Factura.Numero == factura.Numero)
                    resultado = descuento;
            }
            return resultado;
        }
        
        #endregion
        #endregion 
    }
}
