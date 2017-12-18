using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal class GetBindingDescriptionsResponse : Response
    {
#pragma warning disable IDE1006 // Naming Styles
        public Dictionary<string, ObjectDescription> bindings { get; set; } = new Dictionary<string, ObjectDescription>();
#pragma warning restore IDE1006 // Naming Styles

        public GetBindingDescriptionsResponse()
        {
            responseType = ResponseType.getBindings;
        }
    }
}
