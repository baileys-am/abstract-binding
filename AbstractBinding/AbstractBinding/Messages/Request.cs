﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    internal enum RequestType
    {
        subscribe,
        unsubscribe,
        invoke,
        propertyGet
    }

    [Serializable]
    internal class Request
    {
#pragma warning disable IDE1006 // Naming Styles
        public RequestType requestType { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public string objectId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
