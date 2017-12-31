using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    public class NotificationEventArgs
    {
        public Notification Notification { get; private set; }

        public NotificationEventArgs(Notification notification)
        {
            Notification = notification;
        }
    }
}
