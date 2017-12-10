using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class PropertySetResponse : Response
    {
#pragma warning disable IDE1006 // Naming Styles
        public string objectId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public string propertyId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public PropertySetResponse()
        {
            responseType = ResponseType.propertySet;
        }
    }
}
