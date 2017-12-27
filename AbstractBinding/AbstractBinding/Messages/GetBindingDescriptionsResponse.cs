using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal class GetBindingDescriptionsResponse : IResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public ResponseType responseType => ResponseType.getBindings;

        public Dictionary<string, ObjectDescription> bindings { get; set; } = new Dictionary<string, ObjectDescription>();
#pragma warning restore IDE1006 // Naming Styles
    }
}
