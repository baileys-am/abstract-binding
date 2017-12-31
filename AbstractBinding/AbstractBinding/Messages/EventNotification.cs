using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class EventNotification : Notification
    {
#pragma warning disable IDE1006 // Naming Styles
        public string eventId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public object eventArgs { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
