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
    /// funciones para ayudar con los recursos
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// obtiene el valor de una constante por su nombre en el tipo indicado
        /// se hizo para el caso de obtener el valor de un recurso, por ejemplo;
        ///     header.ItemTemplateId = GetResourceLayoutID(typeof(Resource.Layout), "BusquedaArticulosHeaderCodigo")
        /// en este caso para obtener el id del template llamado BusquedaArticulosHeaderCodigo
        /// </summary>
        /// <param name="tipo">tipo de la clase.  Como son constantes con el tipo es suficiente</param>
        /// <param name="nombreConstante">el nombre de la constante</param>
        /// <returns>el id de la constante correspondiente</returns>
        public static int GetID(Type tipo, string nombreConstante)
        {
            List<object> constants = new List<object>();

            // Gets all public and static fields, and FlattenHierarchy tells it to get the fields from all base types as well
            FieldInfo[] constantes = tipo.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            // IsLiteral determines if its value is written at compile time and not changeable
            // IsInitOnly determine if the field can be set in the body of the constructor
            // for C# a field which is readonly keyword would have both true 
            //   una constante solo tiene IsLiteral = true (isInitOnlty = false)
            foreach (FieldInfo fi in constantes)
            {
                if (fi.IsLiteral && !fi.IsInitOnly)
                {
                    if (fi.Name == nombreConstante)
                        return (int)fi.GetValue(fi);
                }
            }

            return -1; // si no existe!
        }

    }
}