using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FR.DR.Core.Helper
{
    /// <summary>
    /// Tiene funciones que ayudan a manipular los enumerados, utilizando reflection
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// permite obtener el valor de un enumerado a partir del texto del valor
        /// ej: si el valor del enumerado es CriterioArticulo.Barras, lo logra obtener a partir del texto "Barras"
        /// </summary>
        /// <typeparam name="T">Excluiso para tipos enumerados (struct,IConvertible)</typeparam>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(string stringValue) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), stringValue,true);
        }
    }
}