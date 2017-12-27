using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractBinding.Messages;

namespace AbstractBinding.SenderInternals
{
    internal class RuntimeProxy : IDisposable
    {
        private readonly string _objectId;
        private readonly IAbstractClient _client;
        private readonly ISerializer _serializer;
        private readonly Dictionary<string, EventHandler> _eventHandlers = new Dictionary<string, EventHandler>();

        public RuntimeProxy(string objectId, IAbstractClient client, ISerializer serializer)
        {
            _objectId = objectId ?? throw new ArgumentNullException(nameof(objectId));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public bool Subscribe(Type interfaceType, string name, EventHandler result)
        {
            _eventHandlers.Add(name, result);
            var request = new SubscribeRequest()
            {
                objectId = _objectId,
                eventId = name
            };
            var resp = _client.Request(_serializer.SerializeObject(request));
            var respObj = _serializer.DeserializeObject<Response>(resp);

            switch (respObj.responseType)
            {
                case ResponseType.exception:
                    var respEx = _serializer.DeserializeObject<ExceptionResponse>(resp);
                    throw respEx.exception;
                case ResponseType.subscribe:
                    var subEx = _serializer.DeserializeObject<SubscribeResponse>(resp);
                    if (subEx.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{subEx.objectId}'.");
                    }
                    else if (subEx.eventId != name)
                    {
                        throw new InvalidResponseException($"Incorrect event ID. Expected '{name}', but received '{subEx.eventId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.subscribe}', but received '{respObj.responseType}'.");
            }
            return true;
        }

        public bool Unsubscribe(Type interfaceType, string name, EventHandler result)
        {
            _eventHandlers.Remove(name);
            var request = new UnsubscribeRequest()
            {
                objectId = _objectId,
                eventId = name
            };
            var resp = _client.Request(_serializer.SerializeObject(request));
            var respObj = _serializer.DeserializeObject<Response>(resp);

            switch (respObj.responseType)
            {
                case ResponseType.exception:
                    var respEx = _serializer.DeserializeObject<ExceptionResponse>(resp);
                    throw respEx.exception;
                case ResponseType.unsubscribe:
                    var subEx = _serializer.DeserializeObject<UnsubscribeResponse>(resp);
                    if (subEx.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{subEx.objectId}'.");
                    }
                    else if (subEx.eventId != name)
                    {
                        throw new InvalidResponseException($"Incorrect event ID. Expected '{name}', but received '{subEx.eventId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.unsubscribe}', but received '{respObj.responseType}'.");
            }
            return true;
        }

        public bool SetValue(Type interfaceType, string name, object value)
        {
            var request = new PropertySetRequest()
            {
                objectId = _objectId,
                propertyId = name,
                value = value
            };
            var resp = _client.Request(_serializer.SerializeObject(request));
            var respObj = _serializer.DeserializeObject<Response>(resp);

            switch (respObj.responseType)
            {
                case ResponseType.exception:
                    var exResp = _serializer.DeserializeObject<ExceptionResponse>(resp);
                    throw exResp.exception;
                case ResponseType.propertySet:
                    var getResp = _serializer.DeserializeObject<PropertySetResponse>(resp);
                    if (getResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{getResp.objectId}'.");
                    }
                    else if (getResp.propertyId != name)
                    {
                        throw new InvalidResponseException($"Incorrect property ID. Expected '{name}', but received '{getResp.propertyId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.propertySet}', but received '{respObj.responseType}'.");
            }
            return true;
        }

        public bool GetValue(Type interfaceType, string name, out object value)
        {
            var request = new PropertyGetRequest()
            {
                objectId = _objectId,
                propertyId = name
            };
            var resp = _client.Request(_serializer.SerializeObject(request));
            var respObj = _serializer.DeserializeObject<Response>(resp);

            switch (respObj.responseType)
            {
                case ResponseType.exception:
                    var exResp = _serializer.DeserializeObject<ExceptionResponse>(resp);
                    throw exResp.exception;
                case ResponseType.propertyGet:
                    var getResp = _serializer.DeserializeObject<PropertyGetResponse>(resp);
                    value = getResp.value;
                    if (getResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{getResp.objectId}'.");
                    }
                    else if (getResp.propertyId != name)
                    {
                        throw new InvalidResponseException($"Incorrect property ID. Expected '{name}', but received '{getResp.propertyId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.propertyGet}', but received '{respObj.responseType}'.");
            }
            return true;
        }

        public bool Invoke(Type interfaceType, string name, object[] args, out object result)
        {
            var request = new InvokeRequest()
            {
                objectId = _objectId,
                methodId = name,
                methodArgs = args
            };
            var resp = _client.Request(_serializer.SerializeObject(request));
            var respObj = _serializer.DeserializeObject<Response>(resp);

            switch (respObj.responseType)
            {
                case ResponseType.exception:
                    var exResp = _serializer.DeserializeObject<ExceptionResponse>(resp);
                    throw exResp.exception;
                case ResponseType.invoke:
                    var invokeResp = _serializer.DeserializeObject<InvokeResponse>(resp);
                    result = invokeResp.result;
                    if (invokeResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{invokeResp.objectId}'.");
                    }
                    else if (invokeResp.methodId != name)
                    {
                        throw new InvalidResponseException($"Incorrect method ID. Expected '{name}', but received '{invokeResp.methodId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.invoke}', but received '{respObj.responseType}'.");
            }
            return true;
        }

        public void OnEventNotification(EventNotification notification)
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
