using System;
using System.Reflection;

namespace de.alivedevil
{
    static class Helper
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).Length > 0;
        }
    }
}
