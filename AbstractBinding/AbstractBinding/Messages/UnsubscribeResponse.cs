using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class UnsubscribeResponse : Response
    {
#pragma warning disable IDE1006 // Naming Styles
        public string objectId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public string eventId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public UnsubscribeResponse()
        {
            responseType = ResponseType.unsubscribe;
        }
    }
}
