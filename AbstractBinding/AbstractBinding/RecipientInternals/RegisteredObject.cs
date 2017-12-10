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

        public string ObjectId { get; private set; }

        public IReadOnlyDictionary<string, RegisteredMethod> Methods { get; private set; }
        public IReadOnlyDictionary<string, RegisteredProperty> Properties { get; private set; }

        public RegisteredObject(string objectId,
                                object obj,
                                IReadOnlyDictionary<string, RegisteredEvent> events,
                                IReadOnlyDictionary<string, RegisteredProperty> properties,
                                IReadOnlyDictionary<string, RegisteredMethod> methods)
        {
            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Methods = methods ?? throw new ArgumentNullException(nameof(methods));
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
            if (Properties.ContainsKey(propertyId))
            {
                return Properties[propertyId].GetValue();
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have property named {propertyId}");
            }
        }

        public void SetValue(string propertyId, object value)
        {
            if (Properties.ContainsKey(propertyId))
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
            if (Methods.ContainsKey(methodId))
            {
                return Methods[methodId].Invoke(args);
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have method named {methodId}");
            }
        }
    }
}