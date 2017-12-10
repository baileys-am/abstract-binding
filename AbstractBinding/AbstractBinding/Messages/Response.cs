﻿using System;
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
        exception,
        propertyGet,
        propertySet
    }

    [Serializable]
    internal class Response
    {
#pragma warning disable IDE1006 // Naming Styles
        public ResponseType responseType { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
