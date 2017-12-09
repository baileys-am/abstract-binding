using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class InvokeRequest : Request
    {
#pragma warning disable IDE1006 // Naming Styles
        public string methodId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public object[] methodArgs { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public InvokeRequest()
        {
            requestType = RequestType.invoke;
        }
    }
}
