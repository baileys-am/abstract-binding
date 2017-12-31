using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    public enum RequestType
    {
        subscribe,
        unsubscribe,
        invoke,
        propertyGet,
        propertySet,
        getBindings
    }

    public interface IRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        RequestType requestType { get; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
