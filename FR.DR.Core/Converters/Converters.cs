using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Globalization;
using Cirrious.MvvmCross.Converters;
using Cirrious.MvvmCross.Plugins.Color;

using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls;
using FR.DR.Core.Helper;

namespace FR.DR.Core.Converters
{

    #region Numéricos

    public class FormatoNumero : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object esDolar, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is decimal)
                {
                    if (esDolar != null && esDolar is bool)
                    {
                        return GestorUtilitario.FormatNumero((decimal)value, (bool)esDolar);
                    }
                    // mvega: no hizo falta
                    //else if (esDolar is string) // es la moneda
                    //{
                    //    return GestorUtilitario.FormatNumero((decimal)value, (string)esDolar == Softland.ERP.FR.Mobile.Cls.TipoMoneda.DOLAR);
                    //}
                    else
                    {
                        return GestorUtilitario.FormatNumero((decimal)value);
                    }
                }
                else
                {
                    return value;
                }
            }
            return null;
        }
    }

    public class FormatoNumeroDecimal : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object esDolar, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is decimal)
                {
                        return GestorUtilitario.FormatNumero((decimal)value);
                    
                }
                else
                {
                    return value;
                }
            }
            return null;
        }
    }

    public class FormatoEntero : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is decimal)
            {
                return GestorUtilitario.FormatEntero((decimal)value);
            }
            return null;
        }
    }

    public class FormatoMonto : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is decimal)
            {
                return GestorUtilitario.Formato((decimal)value);
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return decimal.Parse(value.ToString());
        }
    }

    public class FormatoMontoPorcentaje : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is decimal)
            {
                return GestorUtilitario.FormatPorcentaje((decimal)value);
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return decimal.Parse(value.ToString());
        }
    }

    public class ConvertidorCantidades : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var cantidad = int.Parse(value.ToString());
                return cantidad > 0 ? "Sí Tiene" : "No Tiene";
            }
            return "Null";
        }

    }

    public class ConvertidorImpuestos : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var impuesto = decimal.Parse(value.ToString());
                return impuesto > 0 ? "Sí" : "No";
            }
            return "Null";
        }
    }

    public class FormatoDecimal : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is decimal)
            {
                return ((decimal)value).ToString("#0.00");
            }
            return "Null";
        }
    }

    #endregion

    #region Enumerados

    public class ConvertidorTipoDocumento : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                TipoDocumento tipoDocumento = (TipoDocumento)value;

                switch (tipoDocumento)
                {
                    case TipoDocumento.Factura: return "FAC";
                    case TipoDocumento.NotaDebito: return "N/D";
                    case TipoDocumento.LetraCambio: return "L/C";
                    case TipoDocumento.OtroDebito: return "O/D";
                    case TipoDocumento.Intereses: return "INT";
                    case TipoDocumento.BoletaVenta: return "B/V";
                    case TipoDocumento.InteresCorriente: return "I/C";
                    case TipoDocumento.NotaCredito: return "N/C";
                    case TipoDocumento.NotaCreditoNueva: return "N/C";

                    case TipoDocumento.FacturaContado: return "FAC";
                    case TipoDocumento.NotaCreditoAux: return "N/CA";
                    case TipoDocumento.Recibo: return "REC";
                    case TipoDocumento.NotaCreditoCrear: return "N/CC";
                    case TipoDocumento.TodosDocumentosDebito: return "Debs";
                    default:
                        throw new Exception(string.Format("No se puede convertir el tipo de documento '{0}' ", tipoDocumento));
                }
            }
            return null;
        }
    }

    public class EnumDescription : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var Enumerado = (Enum)value;
                return Enumerado.Descripcion();
            }
            return null;
        }
    }

    #endregion

    //public class ConvertidorPrecios : MvxBaseValueConverter
    //{
    //    public override object Convert(object value, Type targetType, object esDolar, CultureInfo culture)
    //    {
    //        if (value != null && !string.IsNullOrEmpty(value.ToString()))
    //        {
    //            bool Dolar = bool.Parse(esDolar.ToString());
    //            var precio = decimal.Parse(value.ToString());

    //            return GestorUtilitario.FormatNumero(precio, Dolar);
    //        }
    //        return "Null";
    //    }
    //}

    #region Comportamiento de Controles

    public class CountToColor : MvxBaseValueConverter
    {
        private static readonly MvxColor RedColor = new MvxColor(0xff, 0x00, 0x00);
        private static readonly MvxColor GreenColor = new MvxColor(0x00, 0xff, 0x00);

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var cantidad = int.Parse(value.ToString());
                return cantidad > 0 ? RedColor : GreenColor;
            }
            return "Null";
        }
    }

    public class CountToBool : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                return (int)value > 0;
            }
            return "Null";
        }
    }

    public class CountToEnabled : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int)
            {
                return (int)value > 1;
            }
            return "Null";
        }
    }

    /// <summary>
    /// convierte el tipo de moneda a Bool, segun el parametro indicado
    ///     Si parametro es 'False' entonces retorna true si el tipo moneda es Local
    ///     Si parametro es 'True' entonces retorna true si el tipo moneda es Dolar
    /// ejemplo de uso:
    ///     Moneda,Converter=TipoMonedaToBool,ConverterParameter='False' para local
    ///     Moneda,Converter=TipoMonedaToBool,ConverterParameter='True' para dolar
    /// </summary>
    public class TipoMonedaToBool : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object oesDolar, CultureInfo culture)
        {
            if (value != null)
            {
                var moneda = (TipoMoneda)value;
                bool esDolar = bool.Parse(oesDolar.ToString());

                bool result = (moneda == TipoMoneda.DOLAR && esDolar ||
                               moneda == TipoMoneda.LOCAL && !esDolar);

                return result;
            }
            return false;
        }
    }

    /// <summary>
    /// lo convierte a Visible: se basa TipoMonedaToBool, pero lo pasa a Visible o Invisible
    /// </summary>
    public class TipoMonedaToVisibility : TipoMonedaToBool
    {
        public override object Convert(object value, Type targetType, object esDolar, CultureInfo culture)
        {
            bool visible = (bool)base.Convert(value, targetType, esDolar, culture);

            return visible ? ViewStates.Visible : ViewStates.Invisible;
        }
    }

    public class MonedaCobrosToVisibility : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object esDolar, CultureInfo culture)
        {
            bool targetDolar = bool.Parse(esDolar.ToString());
            TipoMoneda moneda = Softland.ERP.FR.Mobile.Cls.Cobro.Cobros.TipoMoneda;
                

            bool visible = (moneda == TipoMoneda.DOLAR && targetDolar) ||
                            (moneda != TipoMoneda.DOLAR && !targetDolar);

            return visible ? ViewStates.Visible : ViewStates.Invisible;
        }
    }

    /// <summary>
    /// asigna el formato de acuerdo a la moneda que indique Cobros.TipoMoneda
    /// </summary>
    public class FormatoMonedaCobros : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object dummy, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is decimal)
                {
                    if (Softland.ERP.FR.Mobile.Cls.Cobro.Cobros.TipoMoneda == TipoMoneda.DOLAR)
                    {
                        return GestorUtilitario.FormatNumero((decimal)value, true);
                    }
                    else
                    {
                        return GestorUtilitario.FormatNumero((decimal)value);
                    }
                }
                else
                {
                    return value;
                }
            }
            return null;
        }
    }
    
    public class BoolToVisibility : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                bool valor = (bool)value;

                if (parameter != null && parameter is bool)
                {
                    valor = !valor;
                }
                return valor ? ViewStates.Visible : ViewStates.Invisible;
            }
            return ViewStates.Invisible;
        }
    }

    public class BoolToCollapsed : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                bool valor = (bool)value;

                if (parameter != null && parameter is bool)
                {
                    valor = !valor;
                }
                return valor ? ViewStates.Visible : ViewStates.Gone;
            }
            return ViewStates.Gone;
        }
    }

    #endregion

    public class ConvertidorFecha : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DateTime)
            {
                return GestorUtilitario.ObtenerFechaString((DateTime)value);
            }
            return "Null";
        }
    }

    public class ConvertidorHora : MvxBaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DateTime)
            {
                return GestorUtilitario.ObtenerHoraString((DateTime)value);
            }
            return "Null";
        }
    }

    public class Converters
    {
        public readonly ConvertidorImpuestos ConvertidorImpuestos = new ConvertidorImpuestos();
        public readonly ConvertidorCantidades ConvertidorCantidades = new ConvertidorCantidades();
        //public readonly ConvertidorPrecios ConvertidorPrecios = new ConvertidorPrecios();
        public readonly ConvertidorFecha ConvertidorFecha = new ConvertidorFecha();
        public readonly ConvertidorHora ConvertidorHora = new ConvertidorHora();
        public readonly CountToColor CountToColor = new CountToColor();
        public readonly CountToBool CountToBool = new CountToBool();
        public readonly CountToEnabled CountToEnabled = new CountToEnabled();
        
        public readonly TipoMonedaToBool TipoMonedaToBool = new TipoMonedaToBool();
        public readonly TipoMonedaToVisibility TipoMonedaToVisibility = new TipoMonedaToVisibility();

        public readonly FormatoNumero FormatoNumero = new FormatoNumero();
        public readonly FormatoNumeroDecimal FormatoNumeroDecimal = new FormatoNumeroDecimal();
        public readonly FormatoEntero FormatoEntero = new FormatoEntero();
        public readonly FormatoMonto FormatoMonto = new FormatoMonto();
        public readonly FormatoMontoPorcentaje FormatoMontoPorcentaje = new FormatoMontoPorcentaje();

        public readonly ConvertidorTipoDocumento ConvertidorTipoDocumento = new ConvertidorTipoDocumento();
        public readonly FormatoDecimal FormatoDecimal = new FormatoDecimal();
        public readonly BoolToVisibility BoolToVisibility = new BoolToVisibility();
        public readonly BoolToCollapsed BoolToCollapsed = new BoolToCollapsed();
        public readonly EnumDescription EnumDescription = new EnumDescription();
    }
}