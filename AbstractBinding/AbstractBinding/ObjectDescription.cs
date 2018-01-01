using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    public class ObjectDescription
    {
        public IEnumerable<string> Events { get; private set; } = new List<string>();

        public IEnumerable<string> Properties { get; private set; } = new List<string>();

        public IEnumerable<string> Methods { get; private set; } = new List<string>();

        internal ObjectDescription(IEnumerable<string> events, IEnumerable<string> properties, IEnumerable<string> methods)
        {
            Events = events;
            Properties = properties;
            Methods = methods;
        }

        internal bool Equals(ObjectDescription desc)
        {
            return Events.SequenceEqual(desc.Events) &&
                   Properties.SequenceEqual(desc.Properties) &&
                   Methods.SequenceEqual(desc.Methods);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ObjectDescription desc:
                    return this.Equals(desc);
                default:
                    return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
