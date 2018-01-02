using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    public enum ResponseType
    {
        subscribe,
        unsubscribe,
        invoke,
        exception,
        propertyGet,
        propertySet,
        getBindings
    }

    public interface IResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        ResponseType responseType { get; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
