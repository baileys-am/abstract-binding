﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal static partial class EventInfoExtensions
    {
        internal static string GetFullName(this EventInfo info)
        {
            return $"{info.ReflectedType.FullName}.{info.Name}";
        }
    }
}
