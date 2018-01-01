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

        internal string ObjectId { get; private set; }
        internal ObjectDescription Description { get; private set; }

        internal RegisteredObject(string objectId,
                                ObjectDescription objDesc,
                                object obj,
                                IReadOnlyDictionary<string, RegisteredEvent> events,
                                IReadOnlyDictionary<string, RegisteredProperty> properties,
                                IReadOnlyDictionary<string, RegisteredMethod> methods)
        {
            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            Description = objDesc ?? throw new ArgumentNullException(nameof(objDesc));
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
            _methods = methods ?? throw new ArgumentNullException(nameof(methods));
        }

        internal static RegisteredObject Create<T>(string objectId, T obj)
        {
            // Create description
            var objDesc = ObjectDescriptor.GetObjectDescription<T>();

            // Create events
            var events = new Dictionary<string, RegisteredEvent>();
            foreach (var eventInfo in typeof(T).GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                               .Where(e => !e.IsSpecialName))
            {
                // Create registered event
                var registeredEvent = RegisteredEvent.Create(objectId, obj, eventInfo);

                // Store registered event
                events.Add(registeredEvent.EventId, registeredEvent);
            }

            // Create properties
            var properties = new Dictionary<string, RegisteredProperty>();
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                  .Where(p => !p.IsSpecialName))
            {
                // Create registered property
                var registeredProperty = RegisteredProperty.Create(objectId, obj, propertyInfo);

                // Store registered property
                properties.Add(registeredProperty.PropertyId, registeredProperty);
            }

            // Create methods
            var methods = new Dictionary<string, RegisteredMethod>();
            foreach (var methodInfo in typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                .Where(m => !m.IsSpecialName &&
                                                             m.GetBaseDefinition().DeclaringType != typeof(object)))
            {
                // Create registered method
                var registeredMethod = RegisteredMethod.Create(objectId, obj, methodInfo);

                // Store registered method
                methods.Add(registeredMethod.MethodId, registeredMethod);
            }

            return new RegisteredObject(objectId, objDesc, obj, events, properties, methods);
        }

        internal void Subscribe(string eventId, IRecipientCallback callback)
        {
            if (_events.ContainsKey(eventId))
            {
                _events[eventId].Subscribe(callback);
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have event named {eventId}");
            }
        }

        internal void Unsubscribe(string eventId, IRecipientCallback callback)
        {
            if (_events.ContainsKey(eventId))
            {
                _events[eventId].Unsubscribe(callback);
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have event named {eventId}");
            }
        }

        internal object GetValue(string propertyId)
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

        internal void SetValue(string propertyId, object value)
        {
            if (_properties.ContainsKey(propertyId))
            {
                _properties[propertyId].SetValue(value);
            }
            else
            {
                throw new InvalidOperationException($"Object, {ObjectId}, does have property named {propertyId}");
            }
        }

        internal object Invoke(string methodId, object[] args)
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