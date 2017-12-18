using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal class GetBindingDescriptionsRequest : Request
    {
        public GetBindingDescriptionsRequest()
        {
            requestType = RequestType.getBindings;
        }
    }
}
