using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractBinding.Messages;

namespace AbstractBinding
{
    internal interface IProxyClient
    {
        event EventHandler<Messages.NotificationEventArgs> NotificationReceived;

        IResponse Request(IRequest request);
    }
}
