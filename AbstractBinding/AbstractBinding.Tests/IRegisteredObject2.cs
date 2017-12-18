using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Tests
{
    public interface IRegisteredObject2
    {
        event EventHandler NotifyOnNonDataChanged2;

        string StringValueProperty2 { get; set; }

        void VoidReturnMethod2(params object[] args);
    }
}
