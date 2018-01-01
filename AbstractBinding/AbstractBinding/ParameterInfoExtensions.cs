using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal static class ParameterInfoExtensions
    {
        internal static bool IsParams(this ParameterInfo param)
        {
            return param.IsDefined(typeof(ParamArrayAttribute), false);
        }
    }
}
