using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class NestedObjectBinding
    {
#pragma warning disable IDE1006 // Naming Styles
        public bool isBinded { get; set; }

        public string bindingId { get; set; }

        public ObjectBinding objectBinding { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
