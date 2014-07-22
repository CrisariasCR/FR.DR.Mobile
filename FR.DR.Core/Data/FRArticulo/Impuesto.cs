using System;
using Softland.ERP.FR.Mobile.Cls.Corporativo;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{
    public class Impuesto
    {

        #region Impuestos

        /*private string _Compania;
        /// <summary>
        /// 
        /// </summary>
        public string Compania
        {
            get { return _Compania; }
            set { _Compania = value; }
        }*/

        private decimal impuesto1;
        /// <summary>
        /// Porcentaje Impuesto de ventas que se debe aplicar al articulo.
        /// </summary>
        public decimal Impuesto1
        {
            get { return impuesto1; }
            set { impuesto1 = value; }
        }

        private decimal impuesto2;
        /// <summary>
        /// Porcentaje Impuesto de consumo que se debe aplicar al articulo.
        /// </summary>
        public decimal Impuesto2
        {
            get { return impuesto2; }
            set { impuesto2 = value; }
        }

        public decimal MontoTotal
        {
            get { return montoImpuesto2 + MontoImpuesto1; }
        }

        private decimal montoImpuesto1;
        /// <summary>
        /// Monto calcula por demanda del Impuesto de ventas que se debe aplicar al articulo.
        /// </summary>
        public decimal MontoImpuesto1
        {
            get { return montoImpuesto1; }
            set { montoImpuesto1 = value; }
        }

        private decimal montoImpuesto2;
        /// <summary>
        /// Monto calcula por demanda del Impuesto de consumo que se debe aplicar al articulo.
        /// </summary>
        public decimal MontoImpuesto2
        {
            get {return montoImpuesto2; }
            set { montoImpuesto2 = value; }
        }

        private FormaCalcImpuesto1 imp1AfectaDescto = FormaCalcImpuesto1.NoDefinido;

        public FormaCalcImpuesto1 Imp1AfectaDescto
        {
            get { return imp1AfectaDescto; }
            set { imp1AfectaDescto = value; }
        }

        #endregion

        #region Constructor
        public Impuesto()
        { }
        public Impuesto(decimal montoImpuesto1, decimal montoImpuesto2)
        {
            impuesto1 = decimal.Zero;
            impuesto2 = decimal.Zero; 
            this.montoImpuesto1 = montoImpuesto1;
            this.montoImpuesto2 = montoImpuesto2;

            imp1AfectaDescto = FormaCalcImpuesto1.NoDefinido;         
        }
        public void CargarDatosImpuesto(string compania)
        {
            if (Imp1AfectaDescto == FormaCalcImpuesto1.NoDefinido)
            {
                Compania cia = Compania.Obtener(compania);
                Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
            }
        }
        #endregion
        
        #region logica

        public static FormaCalcImpuesto1 FormaCalculo(string tipo)
        {
            FormaCalcImpuesto1 calculo;
		    switch(tipo)
            {
        	    case "T":
                    calculo = FormaCalcImpuesto1.Total;
				    break;
			    case "L":
                    calculo = FormaCalcImpuesto1.Lineas;
				    break;
			    case "A":
                    calculo = FormaCalcImpuesto1.Ambas;
				    break;
		        case "N":
                    calculo = FormaCalcImpuesto1.Ninguno;
				    break;
                default:
                    calculo = FormaCalcImpuesto1.NoDefinido;
                    break;
            }
            return calculo;
        }

        public decimal ImpuestoConsumoLinea(decimal montoBruto, decimal montoDescLinea, decimal porcDescGeneral, decimal porcExoImp)
        {
            montoImpuesto2 = ObtenerImpuesto(montoBruto,
                                                montoDescLinea,
                                                porcDescGeneral,
                                                porcExoImp,
                                                impuesto2);
            return montoImpuesto2;

        }        

        public decimal ImpuestoVentaLinea(decimal montoBruto, decimal montoDescLinea, decimal porcDescGeneral, decimal porcExoImp)
        {
            montoImpuesto1 = ObtenerImpuesto(montoBruto,
                                                montoDescLinea,
                                                porcDescGeneral,
                                                porcExoImp,
                                                impuesto1);
            return montoImpuesto1;
        }
        /// <summary>
        /// Obtiene el impuesto de venta para una linea de documento de FA 
        /// según el esquema definido en las globales de FA.
        /// </summary>
        /// <param name="montoBruto">Monto de la linea sin incluir impuestos ni descuentos (cant * precio)</param>
        /// <param name="montoDescLinea">Monto de descuento de la linea</param>
        /// <param name="porcDescGeneral">Porcentaje de descuento general (desc1 + desc2).</param>
        /// <param name="porcExoImp">Porcentaje de exoneración de impuesto de venta.</param>
        /// <returns></returns>
        private decimal ObtenerImpuesto(decimal montoBruto,decimal montoDescLinea,decimal porcDescGeneral,decimal porcExoImp, decimal impuesto)
        {
            if (impuesto == decimal.Zero)
                return 0;

            decimal porcImp = impuesto / 100;
            decimal montoImpuesto = 0;
            decimal montoDescGeneral = 0;

            switch (imp1AfectaDescto)
            {
                case FormaCalcImpuesto1.Total:
                    //Imp1 = suma(imp1 * (cant * precio - (desc1 + desc2)))
                    montoDescGeneral = porcDescGeneral * montoBruto;
                    montoImpuesto = porcImp * (montoBruto - montoDescGeneral);
                    break;

                case FormaCalcImpuesto1.Lineas:
                    //Imp1 = suma(imp1 * (cant * precio - descLinea))
                    montoImpuesto = porcImp * (montoBruto - montoDescLinea);
                    break;

                case FormaCalcImpuesto1.Ambas:
                    //Imp1 = suma(imp1 * (cant * precio - (desc1 + desc2 + descLinea)))
                    montoDescGeneral = porcDescGeneral * (montoBruto - montoDescLinea);
                    montoImpuesto = porcImp * (montoBruto - montoDescLinea - montoDescGeneral);
                    break;

                case FormaCalcImpuesto1.Ninguno:
                    //Imp1 = suma(imp1 * cant * precio)
                    montoImpuesto = porcImp * montoBruto;
                    break;
            }

            if (porcExoImp > 0)
                montoImpuesto -= montoImpuesto * porcExoImp / 100;

            return montoImpuesto;
        }

        #endregion
    }
}
