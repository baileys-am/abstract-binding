﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class UnsubscribeRequest : IRequest
    {
#pragma warning disable IDE1006 // Naming Styles
        public RequestType requestType => RequestType.unsubscribe;

        public string objectId { get; set; }

        public string eventId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
