using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class EventNotification : Notification
    {
#pragma warning disable IDE1006 // Naming Styles
        public string eventId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public object eventArgs { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public static Type CreateGenericType(Type type)
        {
            return typeof(EventNotification<>).MakeGenericType(type);
        }
    }

    [Serializable]
    internal class EventNotification<T> : EventNotification
    {
#pragma warning disable IDE1006 // Naming Styles
        public new T eventArgs { get => (T)base.eventArgs; set => base.eventArgs = value; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
