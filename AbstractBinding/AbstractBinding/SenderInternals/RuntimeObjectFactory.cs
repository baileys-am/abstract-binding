using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace AbstractBinding.SenderInternals
{
    internal class RuntimeObjectFactory
    {
        private readonly AssemblyName _assName;
        private readonly AssemblyBuilder _assBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        public RuntimeObjectFactory()
        {
            _assName = new AssemblyName($"{typeof(RuntimeObjectFactory).Namespace}.Runtime");
            _assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assName, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assBuilder.DefineDynamicModule(_assName.FullName);
        }

        public object Create(Type type)
        {
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(type.Name, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(type);
            var newType = typeBuilder.CreateType();
            return Activator.CreateInstance(newType);
        }
    }
}
