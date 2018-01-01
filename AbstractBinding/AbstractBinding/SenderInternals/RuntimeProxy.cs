using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AbstractBinding.Messages;

namespace AbstractBinding.SenderInternals
{
    public class RuntimeProxy : IDisposable
    {
        private static AssemblyName _assyName = new AssemblyName($"{typeof(RuntimeProxy).Assembly.GetName().Name}.Runtime");
        private static AssemblyBuilder _assyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
            _assyName,
            AssemblyBuilderAccess.Run,
            new List<CustomAttributeBuilder>()
            {
                new CustomAttributeBuilder(typeof(InternalsVisibleToAttribute).GetConstructor(new[] {typeof(string)}),
                new object[] { typeof(RuntimeProxy).Assembly.GetName().Name })
            });
        private static ModuleBuilder _moduleBuilder = _assyBuilder.DefineDynamicModule(_assyName.FullName);
        private static Dictionary<Type, Type> _runtimeProxyTypes = new Dictionary<Type, Type>();

        private readonly string _objectId;
        private readonly IProxyClient _client;
        private readonly Dictionary<string, EventHandler> _eventHandlers = new Dictionary<string, EventHandler>();

        internal RuntimeProxy(string objectId, IProxyClient client)
        {
            _objectId = objectId ?? throw new ArgumentNullException(nameof(objectId));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public static RuntimeProxy Create(Type type, string objectId, IProxyClient client)
        {
            return (RuntimeProxy)typeof(RuntimeProxy).GetMethod(nameof(Create), new Type[] { typeof(string), typeof(IProxyClient) }).GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, new object[] { objectId, client });
        }

        public static T Create<T>(string objectId, IProxyClient client)
        {;
            if (_runtimeProxyTypes.ContainsKey(typeof(T)))
            {
                return (T)Activator.CreateInstance(_runtimeProxyTypes[typeof(T)], objectId, client);
            }

            // Verify type is interface
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException("Generic argument must be an interface.");
            }

            TypeBuilder typeBuilder = _moduleBuilder.DefineType($"{typeof(T).Name}Proxy", TypeAttributes.Public);

            // Define parent and interface
            typeBuilder.SetParent(typeof(RuntimeProxy));
            typeBuilder.CreatePassThroughConstructors(typeof(RuntimeProxy));
            typeBuilder.AddInterfaceImplementation(typeof(T));

            // Implement events
            foreach (var eventInfo in typeof(T).GetContractEvents())
            {
                Expression<Func<RuntimeTypeHandle, object>> getTypeFromHandle = t => Type.GetTypeFromHandle(t);
                EventBuilder eventBuilder = typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);
                FieldBuilder eventFieldBuilder = typeBuilder.DefineField($"_{eventInfo.Name}", eventInfo.EventHandlerType, FieldAttributes.Private);

                // Implement event handler add method
                // add { ... }
                {
                    MethodInfo addMethodInfo = eventInfo.GetAddMethod();
                    MethodBuilder getMb = typeBuilder.DefineMethod(addMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { eventInfo.EventHandlerType });
                    ILGenerator ilGenerator = getMb.GetILGenerator();

                    string eventName = eventInfo.Name;
                    Type ehType = eventInfo.EventHandlerType;
                    LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                    LocalBuilder objectLb = ilGenerator.DeclareLocal(typeof(object), true);
                    LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);

                    // _event += value;
                    Expression<Func<Delegate, object>> combine = d => Delegate.Combine(d, d);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.EmitCall(OpCodes.Call, combine.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Castclass, ehType);
                    ilGenerator.Emit(OpCodes.Stfld, eventFieldBuilder);

                    // this.Subscribe(eventId, value);
                    Expression<Func<RuntimeProxy, object>> addEventHandler = o => o.Subscribe(null, null);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldstr, eventName);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                    ilGenerator.EmitCall(OpCodes.Callvirt, addEventHandler.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Stloc_2);
                    
                    ilGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(getMb, addMethodInfo);
                }
                // Implement event handler remove method
                // remove { ... }
                {
                    MethodInfo removeMethodInfo = eventInfo.GetRemoveMethod();
                    MethodBuilder getMb = typeBuilder.DefineMethod(removeMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new Type[] { eventInfo.EventHandlerType });
                    ILGenerator ilGenerator = getMb.GetILGenerator();

                    string eventName = eventInfo.Name;
                    Type ehType = eventInfo.EventHandlerType;
                    LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                    LocalBuilder objectLb = ilGenerator.DeclareLocal(typeof(object), true);
                    LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);

                    // _event -= value;
                    Expression<Func<Delegate, object>> remove = d => Delegate.Remove(d, d);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.EmitCall(OpCodes.Call, remove.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Castclass, ehType);
                    ilGenerator.Emit(OpCodes.Stfld, eventFieldBuilder);

                    // this.Unsubscribe(eventId, value);
                    Expression<Func<RuntimeProxy, object>> removeEventHandler = o => o.Unsubscribe(null, null);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldstr, eventName);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, eventFieldBuilder);
                    ilGenerator.EmitCall(OpCodes.Callvirt, removeEventHandler.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Stloc_2);
                    
                    ilGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(getMb, removeMethodInfo);
                }
            }

            // Implement properties
            foreach (var propertyInfo in typeof(T).GetContractProperties())
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

                    // obj = this.GetValue(propertyId, out result);
                    object obj = null;
                    Expression<Func<RuntimeProxy, object>> setValue = o => o.GetValue(null, out obj);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldstr, propertyName);
                    ilGenerator.Emit(OpCodes.Ldloca_S, 1);
                    ilGenerator.EmitCall(OpCodes.Callvirt, setValue.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Stloc_2);

                    // Handle value return types
                    Expression<Func<object, object>> createInstance = a => Activator.CreateInstance(a.GetType());
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    if (propertyInfo.PropertyType.IsValueType)
                    {
                        Label retisnull = ilGenerator.DefineLabel();
                        Label endofif = ilGenerator.DefineLabel();

                        // Create instance if value is null
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

                    // this.SetValue(propertyId, value);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Stloc_1);
                    Expression<Func<RuntimeProxy, object>> setValue = o => o.SetValue(null, null);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldstr, propertyName);
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    if (propertyInfo.PropertyType.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
                    }
                    ilGenerator.EmitCall(OpCodes.Callvirt, setValue.GetMethodInfo(), null);
                    ilGenerator.Emit(OpCodes.Stloc_2);
                    
                    ilGenerator.Emit(OpCodes.Ret);

                    propertyBuilder.SetSetMethod(setMb);
                    typeBuilder.DefineMethodOverride(setMb, setMethodInfo);
                }
            }

            // Implement methods
            foreach (var methodInfo in typeof(T).GetContractMethods())
            {
                object obj = null;
                Expression<Func<RuntimeProxy, object>> invoke = o => o.Invoke(null, null, out obj);
                Expression<Func<object, object>> createInstance = a => Activator.CreateInstance(a.GetType());
                Expression<Func<RuntimeTypeHandle, object>> getTypeFromHandle = t => Type.GetTypeFromHandle(t);
                Expression<Func<List<object>, object>> listToArray = l => l.ToArray();
                Expression<Action<List<object>>> listAdd = l => l.Add(new object());
                var parameterInfoArray = methodInfo.GetParameters();
                var genericArgumentArray = methodInfo.GetGenericArguments();

                MethodBuilder mb = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, parameterInfoArray.Select(pi => pi.ParameterType).ToArray());
                if (genericArgumentArray.Any())
                {
                    mb.DefineGenericParameters(genericArgumentArray.Select(s => s.Name).ToArray());
                }

                ILGenerator ilGenerator = mb.GetILGenerator();
                LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
                LocalBuilder paramsLb = ilGenerator.DeclareLocal(typeof(List<object>), true);
                LocalBuilder resultLb = ilGenerator.DeclareLocal(typeof(object), true);
                LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);
                
                // Create method argument array
                ilGenerator.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(Type.EmptyTypes));
                ilGenerator.Emit(OpCodes.Stloc_1);
                int i = 0;
                foreach (ParameterInfo pi in methodInfo.GetParameters())
                {
                    i++;
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    ilGenerator.Emit(OpCodes.Ldarg, i);
                    if (pi.ParameterType.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Box, pi.ParameterType);
                    }
                    ilGenerator.EmitCall(OpCodes.Callvirt, listAdd.GetMethodInfo(), null);
                }

                // this.Invoke(methodId, args, out result);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldstr, methodInfo.GetFullName());
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.EmitCall(OpCodes.Callvirt, listToArray.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Ldloca_S, 2);
                ilGenerator.EmitCall(OpCodes.Callvirt, invoke.GetMethodInfo(), null);
                ilGenerator.Emit(OpCodes.Stloc_3);

                if (methodInfo.ReturnType != typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Ldloc_2);

                    // Handle value return types
                    if (methodInfo.ReturnType.IsValueType)
                    {
                        Label retisnull = ilGenerator.DefineLabel();
                        Label endofif = ilGenerator.DefineLabel();

                        // Create instance if value is null
                        ilGenerator.Emit(OpCodes.Ldnull);
                        ilGenerator.Emit(OpCodes.Ceq);
                        ilGenerator.Emit(OpCodes.Brtrue_S, retisnull);
                        ilGenerator.Emit(OpCodes.Ldloc_2);
                        ilGenerator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                        ilGenerator.Emit(OpCodes.Br_S, endofif);
                        ilGenerator.MarkLabel(retisnull);
                        ilGenerator.Emit(OpCodes.Ldtoken, methodInfo.ReturnType);
                        ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandle.GetMethodInfo(), null);
                        ilGenerator.EmitCall(OpCodes.Call, createInstance.GetMethodInfo(), null);
                        ilGenerator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                        ilGenerator.MarkLabel(endofif);
                    }
                }
                
                ilGenerator.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(mb, methodInfo);
            }

            // Create and store new type instance
            Type type = typeBuilder.CreateType();
            _runtimeProxyTypes.Add(typeof(T), type);

            return (T)Activator.CreateInstance(type, objectId, client);
        }

        internal bool Subscribe(string eventId, EventHandler handler)
        {
            _eventHandlers.Add(eventId, handler);
            var request = new SubscribeRequest()
            {
                objectId = _objectId,
                eventId = eventId
            };
            var response = _client.Request(request);

            switch (response)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case SubscribeResponse subscribeResp:
                    if (subscribeResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{subscribeResp.objectId}'.");
                    }
                    else if (subscribeResp.eventId != eventId)
                    {
                        throw new InvalidResponseException($"Incorrect event ID. Expected '{eventId}', but received '{subscribeResp.eventId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.subscribe}', but received '{response.responseType}'.");
                    
            }
            return true;
        }

        internal bool Unsubscribe(string eventId, EventHandler handler)
        {
            _eventHandlers.Remove(eventId);
            var request = new UnsubscribeRequest()
            {
                objectId = _objectId,
                eventId = eventId
            };
            var response = _client.Request(request);

            switch (response)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case UnsubscribeResponse unsubscribeResp:
                    if (unsubscribeResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{unsubscribeResp.objectId}'.");
                    }
                    else if (unsubscribeResp.eventId != eventId)
                    {
                        throw new InvalidResponseException($"Incorrect event ID. Expected '{eventId}', but received '{unsubscribeResp.eventId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.unsubscribe}', but received '{response.responseType}'.");
            }
            return true;
        }

        internal bool GetValue(string propertyId, out object value)
        {
            var request = new PropertyGetRequest()
            {
                objectId = _objectId,
                propertyId = propertyId
            };
            var response = _client.Request(request);

            switch (response)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case PropertyGetResponse propertyGetResp:
                    value = propertyGetResp.value;
                    if (propertyGetResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{propertyGetResp.objectId}'.");
                    }
                    else if (propertyGetResp.propertyId != propertyId)
                    {
                        throw new InvalidResponseException($"Incorrect property ID. Expected '{propertyId}', but received '{propertyGetResp.propertyId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.propertyGet}', but received '{response.responseType}'.");
            }
            return true;
        }

        internal bool SetValue(string propertyId, object value)
        {
            var request = new PropertySetRequest()
            {
                objectId = _objectId,
                propertyId = propertyId,
                value = value
            };
            var response = _client.Request(request);

            switch (response)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case PropertySetResponse propertySetResp:
                    if (propertySetResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{propertySetResp.objectId}'.");
                    }
                    else if (propertySetResp.propertyId != propertyId)
                    {
                        throw new InvalidResponseException($"Incorrect property ID. Expected '{propertyId}', but received '{propertySetResp.propertyId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.propertySet}', but received '{response.responseType}'.");
            }
            return true;
        }

        internal bool Invoke(string methodId, object[] args, out object result)
        {
            var request = new InvokeRequest()
            {
                objectId = _objectId,
                methodId = methodId,
                methodArgs = args
            };
            var response = _client.Request(request);

            switch (response)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case InvokeResponse invokeResp:
                    result = invokeResp.result;
                    if (invokeResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{invokeResp.objectId}'.");
                    }
                    else if (invokeResp.methodId != methodId)
                    {
                        throw new InvalidResponseException($"Incorrect method ID. Expected '{methodId}', but received '{invokeResp.methodId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.invoke}', but received '{response.responseType}'.");
            }
            return true;
        }

        internal void OnEventNotification(EventNotification notification)
        {
            if (_eventHandlers.ContainsKey(notification.eventId))
            {
                var handler = _eventHandlers[notification.eventId];
                handler.Method.Invoke(handler.Target, new object[] { this, notification.eventArgs });
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RuntimeObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
