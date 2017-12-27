using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class PropertySetResponse : IResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public ResponseType responseType => ResponseType.propertySet;
        
        public string objectId { get; set; }

        public string propertyId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
