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
        private readonly ISenderClient _client;
        private readonly ISerializer _serializer;
        private readonly Dictionary<string, EventHandler> _eventHandlers = new Dictionary<string, EventHandler>();

        public RuntimeProxy(string objectId, ISenderClient client, ISerializer serializer)
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
            var respObj = _serializer.DeserializeObject<IResponse>(resp) ?? throw new InvalidResponseException("Failed to deserialize response.");

            switch (respObj)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case SubscribeResponse subscribeResp:
                    if (subscribeResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{subscribeResp.objectId}'.");
                    }
                    else if (subscribeResp.eventId != name)
                    {
                        throw new InvalidResponseException($"Incorrect event ID. Expected '{name}', but received '{subscribeResp.eventId}'.");
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
            var respObj = _serializer.DeserializeObject<IResponse>(resp) ?? throw new InvalidResponseException("Failed to deserialize response.");

            switch (respObj)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case UnsubscribeResponse unsubscribeResp:
                    if (unsubscribeResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{unsubscribeResp.objectId}'.");
                    }
                    else if (unsubscribeResp.eventId != name)
                    {
                        throw new InvalidResponseException($"Incorrect event ID. Expected '{name}', but received '{unsubscribeResp.eventId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.unsubscribe}', but received '{respObj.responseType}'.");
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
            var respObj = _serializer.DeserializeObject<IResponse>(resp) ?? throw new InvalidResponseException("Failed to deserialize response.");

            switch (respObj)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case PropertyGetResponse propertyGetResp:
                    value = propertyGetResp.value;
                    if (propertyGetResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{propertyGetResp.objectId}'.");
                    }
                    else if (propertyGetResp.propertyId != name)
                    {
                        throw new InvalidResponseException($"Incorrect property ID. Expected '{name}', but received '{propertyGetResp.propertyId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.propertyGet}', but received '{respObj.responseType}'.");
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
            var respObj = _serializer.DeserializeObject<IResponse>(resp) ?? throw new InvalidResponseException("Failed to deserialize response.");

            switch (respObj)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case PropertySetResponse propertySetResp:
                    if (propertySetResp.objectId != _objectId)
                    {
                        throw new InvalidResponseException($"Incorrect object ID. Expected '{_objectId}', but received '{propertySetResp.objectId}'.");
                    }
                    else if (propertySetResp.propertyId != name)
                    {
                        throw new InvalidResponseException($"Incorrect property ID. Expected '{name}', but received '{propertySetResp.propertyId}'.");
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.propertySet}', but received '{respObj.responseType}'.");
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
            var respObj = _serializer.DeserializeObject<IResponse>(resp) ?? throw new InvalidResponseException("Failed to deserialize response.");

            switch (respObj)
            {
                case ExceptionResponse exResp:
                    throw exResp.exception;
                case InvokeResponse invokeResp:
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
