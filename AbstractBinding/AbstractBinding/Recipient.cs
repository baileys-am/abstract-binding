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
        private readonly ISerializer _serializer;
        private readonly RegisteredObjectFactory _objectFactory;
        private readonly Dictionary<string, RegisteredObject> _registeredObjects = new Dictionary<string, RegisteredObject>();
        
        public Recipient(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            var eventFactory = new RegisteredEventFactory(_serializer);
            var propertyFactory = new RegisteredPropertyFactory();
            var methodFactory = new RegisteredMethodFactory();
            _objectFactory = new RegisteredObjectFactory(eventFactory, propertyFactory, methodFactory);
        }

        public void Register<T>(string objectId, T obj, params Type[] nestedTypes)
        {
            // Create registered object
            var registeredObect = _objectFactory.Create(objectId, obj);

            // Register property objects
            foreach (var property in typeof(T).GetContractProperties().Where(p => nestedTypes.Contains(p.PropertyType))
)            {
                object value = property.GetValue(obj);
                if (value != null)
                {
                    var genericRegisterMethod = GetType().GetMethods().First(m => m.Name == nameof(Register) && m.GetParameters().Count() == 1);
                    genericRegisterMethod.MakeGenericMethod(property.PropertyType).Invoke(this, new object[] { value });
                }
            }

            // Store registered object
            _registeredObjects.Add(objectId, registeredObect);
        }

        public void Register<T>(T obj)
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
                    Register(id, obj);
                    break;
                }
            }
        }

        public string Request(string request)
        {
            return Request(request, null);
        }

        public string Request(string request, IRecipientCallback callback)
        {
            var requestObj = _serializer.DeserializeObject<IRequest>(request) ?? throw new RecipientBindingException("Failed to deserialize request.");

            try
            {
                switch (requestObj)
                {
                    case GetBindingDescriptionsRequest getBindingsReq:
                        var getBindingsResp = new GetBindingDescriptionsResponse();
                        foreach (var obj in _registeredObjects)
                        {
                            getBindingsResp.bindings.Add(obj.Key, obj.Value.Description);
                        }
                        return _serializer.SerializeObject(getBindingsResp);
                    case SubscribeRequest subscribeReq:
                        var subscribeObj = _registeredObjects[subscribeReq.objectId];
                        subscribeObj.Subscribe(subscribeReq.eventId, callback);
                        var subscribeResponse = new SubscribeResponse()
                        {
                            objectId = subscribeReq.objectId,
                            eventId = subscribeReq.eventId
                        };
                        return _serializer.SerializeObject(subscribeResponse);
                    case UnsubscribeRequest unsubscribeReq:
                        var unsubscribeObj = _registeredObjects[unsubscribeReq.objectId];
                        unsubscribeObj.Unsubscribe(unsubscribeReq.eventId, callback);
                        var unsubscribeResponse = new UnsubscribeResponse()
                        {
                            objectId = unsubscribeReq.objectId,
                            eventId = unsubscribeReq.eventId
                        };
                        return _serializer.SerializeObject(unsubscribeResponse);
                    case PropertyGetRequest propertyGetReq:
                        var propertyGetObj = _registeredObjects[propertyGetReq.objectId];
                        object propertyGetValue = propertyGetObj.GetValue(propertyGetReq.propertyId);
                        var propertyGetResponse = new PropertyGetResponse()
                        {
                            objectId = propertyGetReq.objectId,
                            propertyId = propertyGetReq.propertyId,
                            value = propertyGetValue
                        };
                        return _serializer.SerializeObject(propertyGetResponse);
                    case PropertySetRequest propertySetReq:
                        var propertySetObj = _registeredObjects[propertySetReq.objectId];
                        propertySetObj.SetValue(propertySetReq.propertyId, propertySetReq.value);
                        var propertySetResponse = new PropertySetResponse()
                        {
                            objectId = propertySetReq.objectId,
                            propertyId = propertySetReq.propertyId
                        };
                        return _serializer.SerializeObject(propertySetResponse);
                    case InvokeRequest invokeReq:
                        var invokeObj = _registeredObjects[invokeReq.objectId];
                        object invokeResult = invokeObj.Invoke(invokeReq.methodId, invokeReq.methodArgs);
                        var invokeResponse = new InvokeResponse()
                        {
                            objectId = invokeReq.objectId,
                            methodId = invokeReq.methodId,
                            result = invokeResult
                        };
                        return _serializer.SerializeObject(invokeResponse);
                    default:
                        if (requestObj != null)
                        {
                            throw new RecipientBindingException($"Unsupported request type: {requestObj.requestType}");
                        }
                        else
                        {
                            throw new RecipientBindingException($"Request failed to deserialize.");
                        }
                }
            }
            catch (RecipientBindingException ex)
            {
                var exceptionResponseObj = new ExceptionResponse()
                {
                    exception = ex
                };

#pragma warning disable EA003 // Catch block swallows an exception
                return _serializer.SerializeObject(exceptionResponseObj);
            }
#pragma warning restore EA003 // Catch block swallows an exception
        }
    }
}
