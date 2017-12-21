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
            return true;
        }

        public bool GetValue(Type interfaceType, string name, out object value)
        {
            value = interfaceType.GetProperty(name).GetMethod.ReturnType.GetDefault();
            return true;
        }

        public bool Invoke(Type interfaceType, string name, object[] args, out object result)
        {
            result = interfaceType.GetMethod(name).ReturnType.GetDefault();
            return true;
        }

        public void OnNotify(string name, object args)
        {
            if (_eventHandlers.ContainsKey(name))
            {
                var handler = _eventHandlers[name];
                handler.Method.Invoke(handler.Target, new object[] { this, args ?? EventArgs.Empty} );
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
