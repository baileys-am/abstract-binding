﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class InvokeResponse : IResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public ResponseType responseType => ResponseType.invoke;

        public string objectId { get; set; }

        public string methodId { get; set; }

        public object result { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
