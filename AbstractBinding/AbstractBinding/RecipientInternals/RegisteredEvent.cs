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
        private readonly object _obj;
        private readonly KeyValuePair<string, EventInfo> _eventInfo;
        private readonly Delegate _handler;
        private readonly object _subscribeLock = new object();
        private readonly List<IRecipientCallback> _callbacks = new List<IRecipientCallback>();

        private bool _subscribed;

        internal string ObjectId { get; private set; }
        internal string EventId => _eventInfo.Key;

        internal RegisteredEvent(string objectId, object obj, KeyValuePair<string, EventInfo> eventInfo)
        {
            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _eventInfo = eventInfo;

            // Create delegate event handler
            _handler = Delegate.CreateDelegate(
                eventInfo.Value.EventHandlerType,
                this,
                GetType().GetMethod(nameof(_eventHandler), BindingFlags.NonPublic | BindingFlags.Instance)
                         .GetGenericMethodDefinition()
                         .MakeGenericMethod(eventInfo.Value.EventHandlerType.GetMethod("Invoke").GetParameters()[1].ParameterType),
                true);
        }

        ~RegisteredEvent()
        {
            lock (_subscribeLock)
            {
                if (_subscribed)
                {
                    // Remove the event handler
                    _eventInfo.Value.RemoveEventHandler(_obj, _handler);
                }

                _callbacks.Clear();
            }
        }

        internal static RegisteredEvent Create(string id, object objectId, KeyValuePair<string, EventInfo> eventInfo)
        {
            return new RegisteredEvent(id, objectId, eventInfo);
        }

        internal void Subscribe(IRecipientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            try
            {
                lock (_subscribeLock)
                {
                    if (!_subscribed)
                    {
                        // Add the event handler
                        _eventInfo.Value.AddEventHandler(_obj, _handler);
                    }

                    if (!_callbacks.Contains(callback))
                    {
                        _callbacks.Add(callback);
                    }

                    _subscribed = true;
                }
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to subscribe to {EventId} on {ObjectId}", ex);
            }
        }

        internal void Unsubscribe(IRecipientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            try
            {
                lock (_subscribeLock)
                {
                    if (_subscribed)
                    {
                        // Remove the event handler
                        _eventInfo.Value.RemoveEventHandler(_obj, _handler);
                    }
                    
                    _subscribed = false;
                }
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
            lock (_subscribeLock)
            {
                foreach (var callback in _callbacks)
                {
                    callback.Callback(notification);
                }
            }
        }
    }
}
