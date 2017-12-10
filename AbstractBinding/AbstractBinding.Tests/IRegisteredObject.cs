using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Tests
{
    public interface IRegisteredObject
    {
        event EventHandler NotifyOnNonDataChanged;
        
        string StringValueProperty { get; set; }

        void VoidReturnMethod(params object[] args);
    }
}
