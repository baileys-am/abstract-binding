using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal static class TypeExtensions
    {
        public static IEnumerable<EventInfo> GetContractEvents(this Type type)
        {
            return type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(e => !e.IsSpecialName);
        }

        public static IEnumerable<PropertyInfo> GetContractProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(p => !p.IsSpecialName);
        }

        public static IEnumerable<MethodInfo> GetContractMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(m => !m.IsSpecialName)
                       .Where(m => m.GetBaseDefinition().DeclaringType != typeof(object));
        }

        public static object GetDefault(this Type type)
        {
            Func<object> f = GetDefault<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
        }

        private static T GetDefault<T>()
        {
            return default(T);
        }
    }
}
