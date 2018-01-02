using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal static class MethodInfoExtensions
    {
        internal static bool HasParamsArg(this MethodInfo info)
        {
            return info.GetParameters().Any(p => p.IsParams());
        }

        internal static string GetFullName(this MethodInfo info)
        {
            var args = string.Join(",", info.GetParameters().Select(o => o.ParameterType));
            return $"{info.ReflectedType.FullName}.{info.Name}({args})";
        }
    }
}
