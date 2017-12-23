using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractBinding.Messages;
using AbstractBinding.SenderInternals;

namespace AbstractBinding
{
    public class Sender
    {
        private readonly IAbstractClient _client;
        private readonly ISerializer _serializer;
        private readonly ObjectDescriptionFactory _objDescFactory;
        private readonly RuntimeProxyFactory _runtimeProxyFactory;
        private readonly Dictionary<string, RuntimeProxy> _runtimeProxies = new Dictionary<string, RuntimeProxy>();
        private readonly Dictionary<Type, ObjectDescription> _registeredTypes = new Dictionary<Type, ObjectDescription>();

        public IEnumerable<Type> RegisteredTypes => _registeredTypes.Keys;

        public Sender(IAbstractClient client, ISerializer serializer)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _objDescFactory = new ObjectDescriptionFactory();
            _runtimeProxyFactory = new RuntimeProxyFactory(_client, _serializer);

            _client.NotificationReceived += _client_NotificationReceived;
        }

        ~Sender()
        {
            _client.NotificationReceived -= _client_NotificationReceived;
        }

        public void Register<T>()
        {
            var objDesc = _objDescFactory.Create<T>();
            if (_registeredTypes.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException("Type is already registered.");
            }
            else if (_registeredTypes.ContainsValue(objDesc))
            {
                throw new InvalidOperationException("Type description is equivalent to a type that is already registered.");
            }
            else
            {
                _registeredTypes.Add(typeof(T), objDesc);
            }
        }

        public void SynchronizeBindings()
        {
            // Get bindings
            var request = _serializer.SerializeObject(new GetBindingDescriptionsRequest());
            var resp = _client.Request(request);

            // Parse response
            var respObj = _serializer.DeserializeObject<Response>(resp);

            switch (respObj.responseType)
            {
                case ResponseType.exception:
                    var exceptionRespObj = _serializer.DeserializeObject<ExceptionResponse>(resp);
                    throw exceptionRespObj.exception;
                case ResponseType.getBindings:
                    var getBindingsRespObj = _serializer.DeserializeObject<GetBindingDescriptionsResponse>(resp);

                    // Dispose and clear current runtime objects
                    foreach (var obj in _runtimeProxies.Values)
                    {
                        obj.Dispose();
                    }
                    _runtimeProxies.Clear();

                    // For reach binding create runtime object
                    var exceptions = new List<Exception>();
                    foreach (var obj in getBindingsRespObj.bindings)
                    {
                        var regType = _registeredTypes.FirstOrDefault(d => d.Value.Equals(obj.Value));

                        if (regType.Key != null && regType.Value != null)
                        {
                            // Create runtime object
                            _runtimeProxies.Add(obj.Key, _runtimeProxyFactory.Create(regType.Key, obj.Key));
                        }
                        else
                        {
                            //TODO: Make custom exception containing the binding object' description.
                            exceptions.Add(new Exception($"Registered type could not be found with object description for {obj.Key}"));
                        }
                    }

                    if (exceptions.Count != 0)
                    {
                        throw new AggregateException(exceptions);
                    }
                    break;
                default:
                    throw new InvalidResponseException($"Incorrect response type. Expected '{ResponseType.getBindings}', but received '{respObj.responseType}'.");
            }
        }

        public IReadOnlyDictionary<string, T> GetBindingsByType<T>()
        {
            return _runtimeProxies.Where(kvp => kvp.Value is T).ToDictionary(kvp => kvp.Key, kvp => (T)(kvp.Value as object));
        }

        private void _client_NotificationReceived(object sender, NotificationEventArgs e)
        {
            // Parse notification
            var notifObj = _serializer.DeserializeObject<Notification>(e.Notification);

            switch (notifObj.notificationType)
            {
                case NotificationType.eventInvoked:
                    var eventNotifObj = _serializer.DeserializeObject<EventNotification>(e.Notification);
                    _runtimeProxies[eventNotifObj.objectId].OnNotify(eventNotifObj.eventId, eventNotifObj.eventArgs);
                    break;
                default:
                    throw new InvalidNotificationException("Invalid notification received. Expected notification type(s): eventInvoked.");
            }
        }
    }
}
