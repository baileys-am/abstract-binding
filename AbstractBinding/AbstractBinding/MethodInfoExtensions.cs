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
        public static bool HasParamsProperty(this MethodInfo info)
        {
            return info.GetParameters().Any(p => p.IsParams());
        }
    }
}
