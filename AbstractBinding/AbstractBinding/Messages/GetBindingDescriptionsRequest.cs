using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal class GetBindingDescriptionsRequest : IRequest
    {
        public RequestType requestType => RequestType.getBindings;
    }
}
