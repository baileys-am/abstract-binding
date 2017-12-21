using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AbstractBinding.Messages;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredEvent
    {
        private readonly IAbstractService _service;
        private readonly ISerializer _serializer;
        private readonly object _obj;
        private readonly EventInfo _eventInfo;
        private readonly Delegate _handler;

        public string ObjectId { get; private set; }
        public string EventId { get; private set; }

        public RegisteredEvent(IAbstractService service, ISerializer serializer, string objectId, object obj, EventInfo eventInfo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _eventInfo = eventInfo ?? throw new ArgumentNullException(nameof(eventInfo));

            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            EventId = _eventInfo.Name;

            // Create delegate event handler
            _handler = Delegate.CreateDelegate(
                eventInfo.EventHandlerType,
                this,
                GetType().GetMethod(nameof(_eventHandler), BindingFlags.NonPublic | BindingFlags.Instance)
                         .GetGenericMethodDefinition()
                         .MakeGenericMethod(eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters()[1].ParameterType),
                true);
        }

        ~RegisteredEvent()
        {
            Unsubscribe();
        }

        public void Subscribe()
        {
            try
            {
                // Add the event handler
                _eventInfo.AddEventHandler(_obj, _handler);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to subscribe to {EventId} on {ObjectId}", ex);
            }
        }

        public void Unsubscribe()
        {
            try
            {
                // Remove the event handler
                _eventInfo.RemoveEventHandler(_obj, _handler);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to unsubscribe from {EventId} on {ObjectId}", ex);
            }
        }

        private void _eventHandler<T>(object sender, T e)
        {
            var notification = new EventNotification()
            {
                eventId = EventId,
                objectId = ObjectId,
                eventArgs = e
            };
            var serializedNotification = _serializer.SerializeObject(notification);
            _service.Callback(serializedNotification);
        }
    }
}
