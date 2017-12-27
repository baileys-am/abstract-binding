﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    internal class UnsubscribeRequest : IRequest
    {
        public RequestType requestType => RequestType.unsubscribe;

#pragma warning disable IDE1006 // Naming Styles
        public string objectId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable IDE1006 // Naming Styles
        public string eventId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
