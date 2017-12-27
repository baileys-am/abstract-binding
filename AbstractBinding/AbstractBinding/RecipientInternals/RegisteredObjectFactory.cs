using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredObjectFactory
    {
        private readonly RegisteredEventFactory _eventFactory;
        private readonly RegisteredMethodFactory _methodFactory;
        private readonly RegisteredPropertyFactory _propertyFactory;
        private readonly ObjectDescriptionFactory _objDescFactory = new ObjectDescriptionFactory();

        public RegisteredObjectFactory(RegisteredEventFactory eventFactory, RegisteredPropertyFactory propertyFactory, RegisteredMethodFactory methodFactory)
        {
            _eventFactory = eventFactory ?? throw new ArgumentNullException(nameof(eventFactory));
            _propertyFactory = propertyFactory ?? throw new ArgumentNullException(nameof(propertyFactory));
            _methodFactory = methodFactory ?? throw new ArgumentNullException(nameof(methodFactory));
        }

        public RegisteredObject Create<T>(string objectId, T obj)
        {

            // Create description
            var objDesc = _objDescFactory.Create<T>();

            // Create events
            var events = new Dictionary<string, RegisteredEvent>();
            foreach (var eventInfo in typeof(T).GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                               .Where(e => !e.IsSpecialName))
            {
                // Create registered event
                var registeredEvent = _eventFactory.Create(objectId, obj, eventInfo);

                // Store registered event
                events.Add(registeredEvent.EventId, registeredEvent);
            }

            // Create properties
            var properties = new Dictionary<string, RegisteredProperty>();
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                  .Where(p => !p.IsSpecialName))
            {
                // Create registered property
                var registeredProperty = _propertyFactory.Create(objectId, obj, propertyInfo);

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
                var registeredMethod = _methodFactory.Create(objectId, obj, methodInfo);

                // Store registered method
                methods.Add(registeredMethod.MethodId, registeredMethod);
            }

            return new RegisteredObject(objectId, objDesc, obj, events, properties, methods);
        }
    }
}
