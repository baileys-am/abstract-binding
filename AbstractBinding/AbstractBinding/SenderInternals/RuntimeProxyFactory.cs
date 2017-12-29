using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace AbstractBinding.SenderInternals
{
    internal class RuntimeProxyFactory
    {
        private readonly ISenderClient _client;
        private readonly ISerializer _serializer;
        private readonly AssemblyName _assName;
        private readonly AssemblyBuilder _assBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly Dictionary<Type, Type> _proxies = new Dictionary<Type, Type>();

        public RuntimeProxyFactory(ISenderClient client, ISerializer serializer)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _assName = new AssemblyName($"{typeof(RuntimeProxyFactory).Assembly.GetName().Name}.Runtime");
            _assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assName, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assBuilder.DefineDynamicModule(_assName.FullName);
        }

        public RuntimeProxy Create(Type type, string objectId)
        {
            return (RuntimeProxy)GetType().GetMethod(nameof(Create), new Type[] { typeof(string) }).GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(this, new object[] { objectId });
        }

        public T Create<T>(string objectId)
        {
            // Verify type is interface
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException("Generic argument must be an interface.");
            }

            // Create proxy if new
            if (!_proxies.TryGetValue(typeof(T), out Type type))
            {
                // Initialize type builder
                TypeBuilder typeBuilder = _moduleBuilder.DefineType($"{typeof(T).Name}Proxy", TypeAttributes.Public);

                // Define parent and interface
                typeBuilder.SetParent(typeof(RuntimeProxy));
                typeBuilder.CreatePassThroughConstructors(typeof(RuntimeProxy));
                typeBuilder.AddInterfaceImplementation(typeof(T));

                // Implement events
                foreach (var eventInfo in typeof(T).GetContractEvents())
                {
                    ImplementEvent(typeBuilder, eventInfo);
                }

                // Implement properties
                foreach (var propertyInfo in typeof(T).GetContractProperties())
                {
                    ImplementProperty(typeBuilder, propertyInfo);
                }

                // Implement methods
                foreach (var methodInfo in typeof(T).GetContractMethods())
                {
                    ImplementMethod(typeBuilder, methodInfo);
                }

                // Create and return new type instance
                type = typeBuilder.CreateType();
                _proxies.Add(typeof(T), type);
            }

            return (T)Activator.CreateInstance(_proxies[typeof(T)], objectId, _client, _serializer);
        }

        private void ImplementEvent(TypeBuilder typeBuilder, EventInfo eventInfo)
        {
            Expression<Func<RuntimeTypeHandle, object>> getTypeFromHandle = t => Type.GetTypeFromHandle(t);
            EventBuilder eventBuilder = typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);
            FieldBuilder eventFieldBuilder = typeBuilder.DefineField(string.Concat("_", eventInfo.Name), eventInfo.EventHandlerType, FieldAttributes.Private);

            //add
            {
                MethodInfo addMethodInfo = eventInfo.GetAddMethod();
                MethodBuilder getMb = typeBuilder.DefineMethod(addMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { eventInfo.EventHandlerType });
                ILGenerator ilGenerator = getMb.GetILGenerator();

                string eventName = eventInfo.Name;
                Type ehType = eventInfo.EventHandlerType;
                LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                LocalBuilder objectLb = ilGenerator.DeclareLocal(typeof(object), true);
                LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);

                //C#: Type.GetTypeFromHandle(interfaceType)
                ilGenerator.Emit(OpCodes.Ldtoken, eventInfo.DeclaringType);
                ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_0);

                //C#: Delegate.Combine(eventHandler, value)
                Expression<Func<Delegate, object>> combine = d => Delegate.Combine(d, d);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.EmitCall(OpCodes.Call, combine.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Castclass, ehType);
                ilGenerator.Emit(OpCodes.Stfld, eventFieldBuilder);

                //C#: DynamicProxy.TrySetMember(interfaceType, propertyName, eventHandler)
                Expression<Func<RuntimeProxy, object>> addEventHandler = o => o.Subscribe(null, null, null);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldstr, eventName);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                ilGenerator.EmitCall(OpCodes.Callvirt, addEventHandler.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_2);

                //C#: return
                ilGenerator.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(getMb, addMethodInfo);
            }
            //remove
            {
                MethodInfo removeMethodInfo = eventInfo.GetRemoveMethod();
                MethodBuilder getMb = typeBuilder.DefineMethod(removeMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { eventInfo.EventHandlerType });
                ILGenerator ilGenerator = getMb.GetILGenerator();

                string eventName = eventInfo.Name;
                Type ehType = eventInfo.EventHandlerType;
                LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                LocalBuilder objectLb = ilGenerator.DeclareLocal(typeof(object), true);
                LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);

                //C#: Type.GetTypeFromHandle(interfaceType)
                ilGenerator.Emit(OpCodes.Ldtoken, eventInfo.DeclaringType);
                ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_0);

                //C#: Delegate.Remove(eventHandler, value)
                Expression<Func<Delegate, object>> remove = d => Delegate.Remove(d, d);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.EmitCall(OpCodes.Call, remove.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Castclass, ehType);
                ilGenerator.Emit(OpCodes.Stfld, eventFieldBuilder);

                //C#: DynamicProxy.TrySetMember(interfaceType, propertyName, eventHandler)
                Expression<Func<RuntimeProxy, object>> removeEventHandler = o => o.Unsubscribe(null, null, null);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldstr, eventName);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                ilGenerator.EmitCall(OpCodes.Callvirt, removeEventHandler.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_2);

                //C#: return
                ilGenerator.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(getMb, removeMethodInfo);
            }
        }

        private void ImplementProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        {
            Expression<Func<RuntimeTypeHandle, object>> getTypeFromHandle = t => Type.GetTypeFromHandle(t);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, null);

            if (propertyInfo.CanRead)
            {
                MethodInfo getMethodInfo = propertyInfo.GetGetMethod();

                MethodBuilder getMb = typeBuilder.DefineMethod(getMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, propertyInfo.PropertyType, Type.EmptyTypes);
                ILGenerator ilGenerator = getMb.GetILGenerator();
                
                string propertyName = propertyInfo.Name;
                LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                LocalBuilder outObjectLb = ilGenerator.DeclareLocal(typeof(object), true);
                LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);

                //C#: Type.GetTypeFromHandle(interfaceType)
                ilGenerator.Emit(OpCodes.Ldtoken, propertyInfo.DeclaringType);
                ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_0);

                //C#: ret = DynamicProxy.TryGetMember(interfaceType, propertyName, out outObject)
                object obj = 0.0;
                Expression<Func<RuntimeProxy, object>> setValue = o => o.GetValue(null, null, out obj);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldstr, propertyName);
                ilGenerator.Emit(OpCodes.Ldloca_S, 1);
                ilGenerator.EmitCall(OpCodes.Callvirt, setValue.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_2);

                //C#: if(ret == ValueType && ret == null){
                //    ret = Activator.CreateInstance(returnType) }
                Expression<Func<object, object>> createInstance = a => Activator.CreateInstance(a.GetType());
                ilGenerator.Emit(OpCodes.Ldloc_1);
                if (propertyInfo.PropertyType.IsValueType)
                {
                    Label retisnull = ilGenerator.DefineLabel();
                    Label endofif = ilGenerator.DefineLabel();

                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ceq);
                    ilGenerator.Emit(OpCodes.Brtrue_S, retisnull);
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    ilGenerator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    ilGenerator.Emit(OpCodes.Br_S, endofif);
                    ilGenerator.MarkLabel(retisnull);
                    ilGenerator.Emit(OpCodes.Ldtoken, propertyInfo.PropertyType);
                    ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                    ilGenerator.EmitCall(OpCodes.Call, createInstance.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    ilGenerator.MarkLabel(endofif);
                }
                //C#: return ret
                ilGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMb);
                typeBuilder.DefineMethodOverride(getMb, getMethodInfo);
            }

            if (propertyInfo.CanWrite)
            {
                MethodInfo setMethodInfo = propertyInfo.GetSetMethod();
                MethodBuilder setMb = typeBuilder.DefineMethod(setMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { propertyInfo.PropertyType });
                ILGenerator ilGenerator = setMb.GetILGenerator();

                string propertyName = propertyInfo.Name;
                LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                LocalBuilder objectLb = ilGenerator.DeclareLocal(propertyInfo.PropertyType, true);
                LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);
                
                //C#: Type.GetTypeFromHandle(interfaceType)
                ilGenerator.Emit(OpCodes.Ldtoken, propertyInfo.DeclaringType);
                ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_0);

                //C#: object = value
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Stloc_1);

                //C#: DynamicProxy.TrySetMember(interfaceType, propertyName, object)
                Expression<Func<RuntimeProxy, object>> setValue = o => o.SetValue(null, null, null);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldstr, propertyName);
                ilGenerator.Emit(OpCodes.Ldloc_1);
                if (propertyInfo.PropertyType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
                }
                ilGenerator.EmitCall(OpCodes.Callvirt, setValue.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_2);

                //C#: return
                ilGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetSetMethod(setMb);
                typeBuilder.DefineMethodOverride(setMb, setMethodInfo);
            }
        }

        private void ImplementMethod(TypeBuilder tb, MethodInfo mi)
        {
            object obj = null;
            Expression<Func<RuntimeProxy, object>> invoke = o => o.Invoke(null, null, null, out obj);
            Expression<Func<object, object>> createInstance = a => Activator.CreateInstance(a.GetType());
            Expression<Func<RuntimeTypeHandle, object>> getTypeFromHandle = t => Type.GetTypeFromHandle(t);
            Expression<Func<List<object>, object>> listToArray = l => l.ToArray();
            Expression<Action<List<object>>> listAdd = l => l.Add(new object());
            var parameterInfoArray = mi.GetParameters();
            var genericArgumentArray = mi.GetGenericArguments();
            
            MethodBuilder mb = tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual, mi.ReturnType, parameterInfoArray.Select(pi => pi.ParameterType).ToArray());
            if (genericArgumentArray.Any())
            {
                mb.DefineGenericParameters(genericArgumentArray.Select(s => s.Name).ToArray());
            }

            ILGenerator ilGenerator = mb.GetILGenerator();
            LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
            LocalBuilder paramsLb = ilGenerator.DeclareLocal(typeof(List<object>), true);
            LocalBuilder resultLb = ilGenerator.DeclareLocal(typeof(object), true);
            LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);
            
            //C#: Type.GetTypeFromHandle(interfaceType)
            ilGenerator.Emit(OpCodes.Ldtoken, mi.DeclaringType);
            ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
            ilGenerator.Emit(OpCodes.Stloc_0);

            //C#: params = new List<object>()
            ilGenerator.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc_1);

            int i = 0;
            foreach (ParameterInfo pi in mi.GetParameters())
            {
                //C#: params.Add(param[i])
                i++;
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ldarg, i);
                if (pi.ParameterType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Box, pi.ParameterType);
                }
                ilGenerator.EmitCall(OpCodes.Callvirt, listAdd.GetMethodInfo(), null);
            }
            //C#: ret = DynamicProxy.TryInvokeMember(interfaceType, propertyName, params, out result)
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldstr, mi.GetFullName());
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.EmitCall(OpCodes.Callvirt, listToArray.GetMethodInfo(), null);
            ilGenerator.Emit(OpCodes.Ldloca_S, 2);
            ilGenerator.EmitCall(OpCodes.Callvirt, invoke.GetMethodInfo(), null);
            ilGenerator.Emit(OpCodes.Stloc_3);

            if (mi.ReturnType != typeof(void))
            {
                ilGenerator.Emit(OpCodes.Ldloc_2);
                //C#: if(ret == ValueType && ret == null){
                //    ret = Activator.CreateInstance(returnType) }
                if (mi.ReturnType.IsValueType)
                {
                    Label retisnull = ilGenerator.DefineLabel();
                    Label endofif = ilGenerator.DefineLabel();

                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ceq);
                    ilGenerator.Emit(OpCodes.Brtrue_S, retisnull);
                    ilGenerator.Emit(OpCodes.Ldloc_2);
                    ilGenerator.Emit(OpCodes.Unbox_Any, mi.ReturnType);
                    ilGenerator.Emit(OpCodes.Br_S, endofif);
                    ilGenerator.MarkLabel(retisnull);
                    ilGenerator.Emit(OpCodes.Ldtoken, mi.ReturnType);
                    ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                    ilGenerator.EmitCall(OpCodes.Call, createInstance.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Unbox_Any, mi.ReturnType);
                    ilGenerator.MarkLabel(endofif);
                }
            }

            //C#: return ret
            ilGenerator.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(mb, mi);
        }
    }
}
