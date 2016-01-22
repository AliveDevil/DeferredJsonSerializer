using System;
using System.Reflection;

namespace de.alivedevil
{
    internal static class Helper
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), true).Length > 0;
        }
    }
}
