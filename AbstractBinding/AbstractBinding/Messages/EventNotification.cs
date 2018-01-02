using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class EventNotification : INotification
    {
#pragma warning disable IDE1006 // Naming Styles
        public NotificationType notificationType => NotificationType.eventInvoked;

        public string objectId { get; set; }

        public string eventId { get; set; }

        public object eventArgs { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
