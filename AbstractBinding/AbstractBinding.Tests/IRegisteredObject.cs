using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Tests
{
    public class DataChangedEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public object Data { get; private set; }

        public DataChangedEventArgs(string name, object data)
        {
            Name = name;
            Data = data;
        }
    }

    public interface IRegisteredObject
    {
        event EventHandler NotifyOnNonDataChanged;
        event EventHandler<DataChangedEventArgs> NotifyOnDataChanged;
        
        string StringValueProperty { get; set; }
        INestedObject NestedObject { get; set; }

        void VoidReturnMethod(params object[] args);
        void VoidReturnMethodStr(string str);
        void VoidReturnMethodStrVal(string str, double val);
    }
}
