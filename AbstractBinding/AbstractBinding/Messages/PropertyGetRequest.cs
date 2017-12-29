using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal class PropertyGetRequest : IRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public RequestType requestType => RequestType.propertyGet;
        
        public string objectId { get; set; }

        public string propertyId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
