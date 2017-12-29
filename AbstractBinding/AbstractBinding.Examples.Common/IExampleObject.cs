using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Examples
{
    public class ArgsExampleClass
    {
        public string StringProperty { get; set; }
        public double DoubleProperty { get; set; }
    }

    public interface INestedExampleClass
    {
        event EventHandler NestedNotifyRequested; 
    }

    public class NestedExampleClass : INestedExampleClass
    {
        public event EventHandler NestedNotifyRequested;
    }

    public interface IExampleObject
    {
        event EventHandler NotifyRequested;

        string StrProperty { get; set; }

        INestedExampleClass NestedObject { get; set; }

        void MethodVoidStr(string str);

        string MethodStr();

        void MethodVoidParamsString(params string[] strs);

        void MethodVoidExampleClass(ArgsExampleClass exClass);
    }
}
