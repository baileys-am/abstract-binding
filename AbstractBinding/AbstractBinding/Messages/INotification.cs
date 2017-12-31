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
    
    public interface INotification
    {
#pragma warning disable IDE1006 // Naming Styles
        NotificationType notificationType { get; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
