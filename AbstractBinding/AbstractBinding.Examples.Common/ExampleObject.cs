using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Examples
{
    public class ExampleObject : IExampleObject
    {
        private event EventHandler _notifyRequested;
        public event EventHandler NotifyRequested
        {
            add
            {
                Console.WriteLine($"Event add: {nameof(NotifyRequested)}");
                _notifyRequested += value;
            }
            remove
            {
                Console.WriteLine($"Event remove: {nameof(NotifyRequested)}");
                _notifyRequested -= value;
            }
        }

        private string _strProperty;
        public string StrProperty
        {
            get
            {
                Console.WriteLine($"Property get: {nameof(StrProperty)}");
                return _strProperty;
            }
            set
            {
                Console.WriteLine($"Property set: {nameof(StrProperty)}; value: {value}");
                _strProperty = value;
            }
        }

        public void MethodVoidStr(string str)
        {
            Console.WriteLine($"Method invoke: {nameof(MethodVoidStr)}");
        }

        public string MethodStr()
        {
            Console.WriteLine($"Method invoke: {nameof(MethodStr)}");
            return $"{nameof(MethodStr)} string result.";
        }

        public void MethodVoidParamsString(params string[] strs)
        {
            Console.WriteLine($"Method invoke: {nameof(MethodVoidParamsString)}");
        }

        public void MethodVoidExampleClass(ArgsExampleClass exClass)
        {
            Console.WriteLine($"Method invoke: {nameof(MethodVoidExampleClass)}");
        }

        public void OnNotifyRequested()
        {
            _notifyRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
