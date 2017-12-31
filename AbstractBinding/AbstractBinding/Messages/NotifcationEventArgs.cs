using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    public class NotificationEventArgs
    {
        public INotification Notification { get; private set; }

        public NotificationEventArgs(INotification notification)
        {
            Notification = notification;
        }
    }
}
