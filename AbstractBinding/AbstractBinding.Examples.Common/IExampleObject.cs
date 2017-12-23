using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Examples
{
    public interface IExampleObject
    {
        event EventHandler NotifyRequested;

        string StrProperty { get; set; }

        void MethodVoidStr(string str);
    }
}
