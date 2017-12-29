using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class InvokeRequest : IRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public RequestType requestType => RequestType.invoke;
        
        public string objectId { get; set; }

        public string methodId { get; set; }

        public object[] methodArgs { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
