using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredObject
    {
        private readonly object _obj;
        private readonly IReadOnlyDictionary<string, RegisteredEvent> _events;
        private readonly IReadOnlyDictionary<string, RegisteredProperty> _properties;
        private readonly IReadOnlyDictionary<string, RegisteredMethod> _methods;

        public string ObjectId { get; private set; }

        public RegisteredObject(string objectId,
                                object obj,
                                IReadOnlyDictionary<string, RegisteredEvent> events,
                                IReadOnlyDictionary<string, RegisteredProperty> properties,
                                IReadOnlyDictionary<string, RegisteredMethod> methods)
        {
            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
            _methods = methods ?? throw new ArgumentNullException(nameof(methods));
        }

        public void Subscribe(string eventId)
        {
            if (_events.ContainsKey(eventId))
            {
                _events[eventId].Subscribe();
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have event named {eventId}");
            }
        }

        public void Unsubscribe(string eventId)
        {
            if (_events.ContainsKey(eventId))
            {
                _events[eventId].Unsubscribe();
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have event named {eventId}");
            }
        }

        public object GetValue(string propertyId)
        {
            if (_properties.ContainsKey(propertyId))
            {
                return _properties[propertyId].GetValue();
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have property named {propertyId}");
            }
        }

        public void SetValue(string propertyId, object value)
        {
            if (_properties.ContainsKey(propertyId))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have property named {propertyId}");
            }
        }

        public object Invoke(string methodId, params object[] args)
        {
            if (_methods.ContainsKey(methodId))
            {
                return _methods[methodId].Invoke(args);
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have method named {methodId}");
            }
        }
    }
}