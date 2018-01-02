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
        internal static IEnumerable<EventInfo> GetContractEvents(this Type type)
        {
            return type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(e => !e.IsSpecialName);
        }

        internal static IEnumerable<PropertyInfo> GetContractProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(p => !p.IsSpecialName);
        }

        internal static IEnumerable<MethodInfo> GetContractMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(m => !m.IsSpecialName)
                       .Where(m => m.GetBaseDefinition().DeclaringType != typeof(object));
        }

        internal static object GetDefault(this Type type)
        {
            if (type == typeof(void))
            {
                return null;
            }
            else
            {
                Func<object> f = GetDefault<object>;
                return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
            }
        }

        private static T GetDefault<T>()
        {
            return default(T);
        }
    }
}
