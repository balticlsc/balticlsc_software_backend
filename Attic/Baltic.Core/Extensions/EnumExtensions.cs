using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Baltic.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the string from the 'Description' enum attribute
        /// </summary>
        /// <returns>Enum description</returns>
        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetType().
                       GetMember(enumValue.ToString()).
                       FirstOrDefault()?.
                       GetCustomAttribute<DescriptionAttribute>()?.
                       Description
                   ?? enumValue.ToString();
        }

        /// <summary>
        /// Gets the string from the 'Description' enum attribute
        /// </summary>
        /// <returns>Enum description</returns>
        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            return GetDescription((Enum)(object)enumValue);
        }
    }
}