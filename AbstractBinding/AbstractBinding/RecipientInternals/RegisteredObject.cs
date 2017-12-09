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
        private readonly RegisteredEventFactory _eventFactory;
        private readonly RegisteredMethodFactory _methodFactory;
        private readonly object _obj;
        private readonly Dictionary<string, RegisteredMethod> _methods = new Dictionary<string, RegisteredMethod>();
        private readonly Dictionary<string, RegisteredEvent> _events = new Dictionary<string, RegisteredEvent>();

        public string ObjectId { get; private set; }
        
        public IReadOnlyDictionary<string, RegisteredMethod> Methods => _methods;

        public IReadOnlyDictionary<string, RegisteredEvent> Events => _events;

        public RegisteredObject(RegisteredEventFactory eventFactory, RegisteredMethodFactory methodFactory, string objectId, object obj)
        {
            _eventFactory = eventFactory ?? throw new ArgumentNullException(nameof(eventFactory));
            _methodFactory = methodFactory ?? throw new ArgumentNullException(nameof(methodFactory));
            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            
            // Register events
            foreach (var eventInfo in _obj.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // Create registered event
                var registeredEvent = _eventFactory.Create(ObjectId, _obj, eventInfo);

                // Store registered event
                _events.Add(registeredEvent.EventId, registeredEvent);
            }

            // Register methods
            foreach (var methodInfo in _obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                     .Where(m => m.IsPublic &&
                                                                !m.IsSpecialName &&
                                                                 m.GetBaseDefinition().DeclaringType != typeof(object)))
            {
                // Create registered method
                var registeredMethod = _methodFactory.Create(ObjectId, _obj, methodInfo);
                
                // Store registered method
                _methods.Add(registeredMethod.MethodId, registeredMethod);
            }
        }
    }
}