using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal static class PropertyInfoExtensions
    {
        internal static string GetFullName(this PropertyInfo info)
        {
            return $"{info.ReflectedType.FullName}.{info.Name}";
        }
    }
}
