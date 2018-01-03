using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using AbstractBinding.Messages;
using AbstractBinding.RecipientInternals;

namespace AbstractBinding
{
    public class Recipient
    {
        private readonly Dictionary<string, RegisteredObject> _registeredObjects = new Dictionary<string, RegisteredObject>();

        public void Register<T>(string objectId, T obj, params Type[] nestedTypes)
        {
            // Create registered object
            var registeredObject = RegisteredObject.Create(objectId, obj, nestedTypes);

            // Register property objects
            foreach (var property in typeof(T).GetContractProperties().Where(p => nestedTypes.Contains(p.PropertyType)))
            {
                object value = property.GetValue(obj);
                if (value != null)
                {
                    var genericRegisterMethod = GetType().GetMethods().First(m => m.Name == nameof(Register) && m.GetParameters().Count() == 2);
                    genericRegisterMethod.MakeGenericMethod(property.PropertyType).Invoke(this, new object[] { value, nestedTypes });
                }
            }

            // Store registered object
            _registeredObjects.Add(objectId, registeredObject);
        }

        public void Register<T>(T obj, params Type[] nestedTypes)
        {
            while (true)
            {
                string id = Guid.NewGuid().ToString();
                if (_registeredObjects.ContainsKey(id))
                {
                    continue;
                }
                else
                {
                    Register(id, obj, nestedTypes);
                    break;
                }
            }
        }

        public IResponse Request(IRequest request)
        {
            return Request(request, null);
        }

        public IResponse Request(IRequest request, IRecipientCallback callback)
        {
            try
            {
                switch (request)
                {
                    case GetBindingDescriptionsRequest getBindingsReq:
                        var getBindingsResponse = new GetBindingDescriptionsResponse();
                        foreach (var obj in _registeredObjects)
                        {
                            getBindingsResponse.bindings.Add(obj.Key, new ObjectBinding()
                            {
                                events = obj.Value.Description.Events,
                                properties = obj.Value.Description.Properties.ToDictionary(p => p, p => new NestedObjectBinding()
                                {
                                    isBinded = obj.Value.Description.NestedObjects.ContainsKey(p),
                                    bindingId = obj.Value.Description.NestedObjects.ContainsKey(p) ? "" : null,
                                    objectBinding = new ObjectBinding()
                                }),
                                methods = obj.Value.Description.Methods
                            });
                        }
                        return getBindingsResponse;
                    case SubscribeRequest subscribeReq:
                        var subscribeObj = _registeredObjects[subscribeReq.objectId];
                        subscribeObj.Subscribe(subscribeReq.eventId, callback);
                        var subscribeResponse = new SubscribeResponse()
                        {
                            objectId = subscribeReq.objectId,
                            eventId = subscribeReq.eventId
                        };
                        return subscribeResponse;
                    case UnsubscribeRequest unsubscribeReq:
                        var unsubscribeObj = _registeredObjects[unsubscribeReq.objectId];
                        unsubscribeObj.Unsubscribe(unsubscribeReq.eventId, callback);
                        var unsubscribeResponse = new UnsubscribeResponse()
                        {
                            objectId = unsubscribeReq.objectId,
                            eventId = unsubscribeReq.eventId
                        };
                        return unsubscribeResponse;
                    case PropertyGetRequest propertyGetReq:
                        var propertyGetObj = _registeredObjects[propertyGetReq.objectId];
                        object propertyGetValue = propertyGetObj.GetValue(propertyGetReq.propertyId);
                        var propertyGetResponse = new PropertyGetResponse()
                        {
                            objectId = propertyGetReq.objectId,
                            propertyId = propertyGetReq.propertyId,
                            value = propertyGetValue
                        };
                        return propertyGetResponse;
                    case PropertySetRequest propertySetReq:
                        var propertySetObj = _registeredObjects[propertySetReq.objectId];
                        propertySetObj.SetValue(propertySetReq.propertyId, propertySetReq.value);
                        var propertySetResponse = new PropertySetResponse()
                        {
                            objectId = propertySetReq.objectId,
                            propertyId = propertySetReq.propertyId
                        };
                        return propertySetResponse;
                    case InvokeRequest invokeReq:
                        var invokeObj = _registeredObjects[invokeReq.objectId];
                        object invokeResult = invokeObj.Invoke(invokeReq.methodId, invokeReq.methodArgs);
                        var invokeResponse = new InvokeResponse()
                        {
                            objectId = invokeReq.objectId,
                            methodId = invokeReq.methodId,
                            result = invokeResult
                        };
                        return invokeResponse;
                    default:
                        if (request != null)
                        {
                            throw new RecipientBindingException($"Unsupported request type: {request.requestType}");
                        }
                        else
                        {
                            throw new RecipientBindingException($"Request failed to deserialize.");
                        }
                }
            }
            catch (RecipientBindingException ex)
            {
                var exceptionResponse = new ExceptionResponse()
                {
                    exception = ex
                };

#pragma warning disable EA003 // Catch block swallows an exception
                return exceptionResponse;
            }
#pragma warning restore EA003 // Catch block swallows an exception
        }
    }
}
