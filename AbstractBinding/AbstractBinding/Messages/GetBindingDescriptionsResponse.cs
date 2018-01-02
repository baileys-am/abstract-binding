﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class GetBindingDescriptionsResponse : IResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public ResponseType responseType => ResponseType.getBindings;

        public Dictionary<string, ObjectBinding> bindings { get; set; } = new Dictionary<string, ObjectBinding>();
#pragma warning restore IDE1006 // Naming Styles
    }
}
