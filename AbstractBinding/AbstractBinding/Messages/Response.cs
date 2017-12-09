using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal enum ResponseType
    {
        subscribe,
        unsubscribe,
        invoke,
        exception
    }

    [Serializable]
    internal class Response
    {
#pragma warning disable IDE1006 // Naming Styles
        public ResponseType responseType { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public string objectId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
