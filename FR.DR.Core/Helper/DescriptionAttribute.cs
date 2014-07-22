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
    /// Esta clase de atributo permite indicar el valor correspondiente en la base de datos 
    /// para cada uno de los valores(constantes) de un enumerado.
    /// Además expone operaciones que permiten obtener el DBValue a partir del valor de enumerado y viceversa.
    /// </summary>
    /// <precond>
    /// Está pensado para usarlo sólo con enumerados.  Cualquier otro uso no está probado.
    /// Cada uno de los valores del enumerado debe tener definido el atributo DBValue
    /// Los DBValues de los enumerados no deben estar repetidos (esto no se valida!)
    /// </precond>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DescriptionAttribute: Attribute
    {
        //private string description;

        public string Description { get; set; }
        //{
        //    get
        //    {
        //        return description;
        //    }
        //}

        public DescriptionAttribute(string descripcion)
            : base()
        {
            this.Description = descripcion;
        }

        /// <summary>
        /// Obtiene la descripcion correspondiente al valor del enumerado
        /// </summary>
        /// <precond>
        /// La descripcion del enumerado debe tener definido el atributo Description sino levanta una excepción
        /// Las descripciones de los enumerados no deben estar repetidos (esto no se valida!) retorna el primero que coincida
        /// </precond>
        /// <param name="valorEnum">valor del enumerado</param>
        /// <returns>DBValue correspondiente</returns>
        public static string GetDescription(Enum valorEnum)
        {
            string result;
            string description = valorEnum.ToString();
            FieldInfo fieldInfo = valorEnum.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                result = attributes[0].Description;
            }
            else // si no existe, entonces se retorna el valor del enumerado convertido a hilera
            {
#if debug
               // throw new ApplicationException(string.Format("El valor '{0}' del enumerado '{1}' no tiene definido el atributo Description.", valorEnum.ToString(), valorEnum.GetType().ToString()));
               return valorEnum.ToString();
#else
                return valorEnum.ToString();
#endif
            }

            return result;
        }

        public static Type GetNullableType(Type type)
        {
            // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            return type;
        }

        /* no se usa
        public static void GetEnumValue<T>(string descripcion, out T value)
        {

            Type enumType = typeof(T);
            enumType = GetNullableType(enumType);
            // obtiene los campos del tipo (entre ellos los valores del enumerado)
            FieldInfo[] myFields = enumType.GetFields();
            value = default(T);
            // itera por los campos
            for (int i = 0; i < myFields.Length; i++)
            {
                // verifica si está definido el atributo DBValue
                if (myFields[i].IsDefined(typeof(DescriptionAttribute), false))
                {
                    // obtiene el atributo DBValue
                    Object[] myAttributes = myFields[i].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    for (int j = 0; j < myAttributes.Length; j++)
                    {
                        // verifica si coinciden
                        if (((DescriptionAttribute)myAttributes[j]).Description == descripcion)
                        {
                            value = (T)Enum.Parse(enumType, myFields[i].Name);
                            return;
                        }
                    }
                }
            }
            if (descripcion != null)
                throw new ApplicationException("Error obteniendo la descripción del Enumerado " + enumType.Name + " correspondiente a la Descripcion '" + descripcion + "'");
        }
        
        /// <summary>
        /// Obtiene el valor del Enumerado correspondiente a la descripcion indicada
        /// </summary>
        /// <precond>
        /// Alguno de los valores del enumerado debe tener definido el atributo DBValue correspondiente sino levanta una excepción
        /// Los DBValues de los enumerados no deben estar repetidos (esto no se valida!) retorna el primero que coincida
        /// </precond>
        /// <param name="enumType">tipo del enumerado del cual se quiere obtener el valor</param>
        /// <param name="dbValue">valor en la base de datos para el enumerado</param>
        /// <returns>valor del enumerado correspondiente</returns>
        public static Enum GetEnumValue(string descripcion, Type enumType)
        {
            // obtiene los campos del tipo (entre ellos los valores del enumerado)
            FieldInfo[] myFields = enumType.GetFields();
            // itera por los campos
            if (descripcion == null)
            {
                return null;
            }
            for (int i = 0; i < myFields.Length; i++)
            {
                // verifica si está definido el atributo DBValue
                if (myFields[i].IsDefined(typeof(DescriptionAttribute), false))
                {
                    // obtiene el atributo DBValue
                    Object[] myAttributes = myFields[i].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    for (int j = 0; j < myAttributes.Length; j++)
                    {
                        // verifica si coinciden
                        if (((DescriptionAttribute)myAttributes[j]).Description == descripcion)
                            return (Enum)Enum.Parse(enumType, myFields[i].Name);
                    }
                }
            }
            throw new ApplicationException("Error obteniendo el valor del Enumerado " + enumType.Name + " correspondiente a la Descripción'" + descripcion + "'");
        }
        
        /// <summary>
        /// Verifica que la descripción sea válido para el enumerado indicado
        /// </summary>
        /// <param name="dbValue">valor en la base de datos</param>
        /// <param name="enumType">tipo del enumerado del cual se quiere validar un valor</param>
        /// <returns>true si valido, false sino</returns>
        public static bool IsValidDescriptionValue(string descripcion, Type enumType)
        {
            bool result = false;

            // obtiene los campos del tipo (entre ellos los valores del enumerado)
            FieldInfo[] myFields = enumType.GetFields();

            // itera por los campos
            for (int i = 0; i < myFields.Length && !result; i++)
            {
                // verifica si está definido el atributo DBValue
                if (myFields[i].IsDefined(typeof(DescriptionAttribute), false))
                {
                    // obtiene el atributo DBValue
                    Object[] myAttributes = myFields[i].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    for (int j = 0; j < myAttributes.Length; j++)
                    {
                        // verifica si coinciden
                        if (((DescriptionAttribute)myAttributes[j]).Description == descripcion)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        */
    }

    /// <summary>
    /// Define funciones de extension para trabajar con los enumerados de tipo DescriptionAttribute
    /// y para construir los nombres de los templates (item/header)
    /// </summary>
    public static partial class ExtensionHelper
    {
        /// <summary>
        /// le incluye a los tipos Enum, la función/metodo DBValue
        /// con lo cual logramos que a un valor de un enum podamos 
        /// decirle enumValue.Descripcion() ej:
        /// EstadoSincroEnum.Procesado.Descripcion();
        /// </summary>
        /// <param name="value">es el tipo del enumerado</param>
        /// <returns>la Descripcion</returns>
        public static string Descripcion(this Enum value)
        {
            return DescriptionAttribute.GetDescription(value);
        }

        /// <summary>
        /// le incluye en los tipos Enum, la función Descripcion(), la cual permite que a un valor de un enum le podamos 
        /// pedir su descripción, tal y como está definida en el atributo DescripcionAttribute ej:
        /// CriterioFiltro.Barras.Descripcion();
        /// </summary>
        /// <param name="value">es el tipo del enumerado</param>
        /// <returns>el DBValue</returns>
        public static string Description(this Enum value)
        {
            return DescriptionAttribute.GetDescription(value);
        }

        public static string ItemTemplate(this Enum value, string prefijo)
        {
            return prefijo + "Item" + value.ToString();
        }

        public static string HeaderTemplate(this Enum value, string prefijo)
        {
            return prefijo + "Header" + value.ToString();
        }
    }

}