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
        private readonly IAbstractService _service;
        private readonly ISerializer _serializer;
        private readonly RegisteredObjectFactory _objectFactory;
        private readonly Dictionary<string, RegisteredObject> _registeredObjects = new Dictionary<string, RegisteredObject>();
        
        public Recipient(IAbstractService service, ISerializer serializer)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            var eventFactory = new RegisteredEventFactory(_service, _serializer);
            var propertyFactory = new RegisteredPropertyFactory();
            var methodFactory = new RegisteredMethodFactory(_service, _serializer);
            _objectFactory = new RegisteredObjectFactory(eventFactory, propertyFactory, methodFactory);
        }

        public void Register<T>(string objectId, T obj)
        {
            // Create registered object
            var registeredObect = _objectFactory.Create(objectId, obj);

            // Store registered object
            _registeredObjects.Add(objectId, registeredObect);
        }

        public string Request(string request)
        {
            var requestObj = _serializer.DeserializeObject<Request>(request ?? throw new ArgumentNullException(nameof(request)));

            switch (requestObj.requestType)
            {
                case RequestType.subscribe:
                    var subscribeRequest = _serializer.DeserializeObject<SubscribeRequest>(request);
                    var subscribeObj = _registeredObjects[subscribeRequest.objectId];
                    try
                    {
                        subscribeObj.Events[subscribeRequest.eventId].Subscribe();
                    }
                    catch (Exception ex)
                    {
                        var exResponse = new ExceptionResponse()
                        {
                            objectId = subscribeRequest.objectId,
                            methodId = subscribeRequest.eventId,
                            exception = new RecipientMethodException($"Failed to invoke {subscribeRequest.eventId} on {subscribeRequest.objectId}", ex)
                        };

#pragma warning disable EA003 // Catch block swallows an exception
                        return _serializer.SerializeObject(exResponse);
#pragma warning restore EA003 // Catch block swallows an exception
                    }

                    var subscribeResponse = new SubscribeResponse()
                    {
                        objectId = subscribeRequest.objectId,
                        eventId = subscribeRequest.eventId
                    };
                    return _serializer.SerializeObject(subscribeResponse);
                case RequestType.unsubscribe:
                    var unsubscribeRequest = _serializer.DeserializeObject<UnsubscribeRequest>(request);
                    var unsubscribeObj = _registeredObjects[unsubscribeRequest.objectId];
                    try
                    {
                        unsubscribeObj.Events[unsubscribeRequest.eventId].Unsubscribe();
                    }
                    catch (Exception ex)
                    {
                        var exResponse = new ExceptionResponse()
                        {
                            objectId = unsubscribeRequest.objectId,
                            methodId = unsubscribeRequest.eventId,
                            exception = new RecipientMethodException($"Failed to invoke {unsubscribeRequest.eventId} on {unsubscribeRequest.objectId}", ex)
                        };

#pragma warning disable EA003 // Catch block swallows an exception
                        return _serializer.SerializeObject(exResponse);
#pragma warning restore EA003 // Catch block swallows an exception
                    }

                    var unsubscribeResponse = new UnsubscribeResponse()
                    {
                        objectId = unsubscribeRequest.objectId,
                        eventId = unsubscribeRequest.eventId
                    };
                    return _serializer.SerializeObject(unsubscribeResponse);
                case RequestType.invoke:
                    var invokeRequest = _serializer.DeserializeObject<InvokeRequest>(request);
                    var invokeObj = _registeredObjects[invokeRequest.objectId];
                    object invokeResult = null;
                    try
                    {
                        invokeResult = invokeObj.Methods[invokeRequest.methodId].Invoke(invokeRequest.methodArgs);
                    }
                    catch (Exception ex)
                    {
                        var exResponse = new ExceptionResponse()
                        {
                            objectId = invokeRequest.objectId,
                            methodId = invokeRequest.methodId,
                            exception = new RecipientMethodException($"Failed to invoke {invokeRequest.methodId} on {invokeRequest.objectId}", ex)
                        };

#pragma warning disable EA003 // Catch block swallows an exception
                        return _serializer.SerializeObject(exResponse);
#pragma warning restore EA003 // Catch block swallows an exception
                    }
                   
                    var invokeResponse = new InvokeResponse()
                    {
                        objectId = invokeRequest.objectId,
                        methodId = invokeRequest.methodId,
                        result = invokeResult
                    };
                    return _serializer.SerializeObject(invokeResponse);
                case RequestType.propertyGet:
                    var propertyGetRequest = _serializer.DeserializeObject<PropertyGetRequest>(request);
                    var propertyGetObject = _registeredObjects[propertyGetRequest.objectId];
                    object propertyGetValue = null;
                    try
                    {
                        propertyGetValue = propertyGetObject.Properties[propertyGetRequest.propertyId].GetValue();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    
                    var propertyGetResponse = new PropertyGetResponse()
                    {
                        objectId = propertyGetRequest.objectId,
                        propertyId = propertyGetRequest.propertyId,
                        value = propertyGetValue
                    };
                    return _serializer.SerializeObject(propertyGetResponse);
                default:
                    throw new InvalidOperationException($"Unsupported request type: {requestObj.requestType}");
            }
        }
    }
}
