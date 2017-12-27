using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal class GetBindingDescriptionsRequest : IRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public RequestType requestType => RequestType.getBindings;
#pragma warning restore IDE1006 // Naming Styles
    }
}
