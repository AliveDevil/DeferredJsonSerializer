using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace de.alivedevil
{
    internal static class Helper
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).Length > 0;
        }

        public static IEnumerable<MemberInfo> Member(this Type type)
        {
            return Enumerable.Union<MemberInfo>(type.GetProperties(), type.GetFields());
        }
    }
}
