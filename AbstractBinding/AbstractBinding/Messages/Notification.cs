using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    public enum NotificationType
    {
        eventInvoked
    }

    [Serializable]
    internal class Notification
    {
#pragma warning disable IDE1006 // Naming Styles
        public NotificationType notificationType { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public string objectId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
