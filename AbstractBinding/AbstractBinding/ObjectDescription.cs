using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    public class ObjectDescription
    {
        internal Dictionary<string, EventDescription> Events { get; set; } = new Dictionary<string, EventDescription>();

        internal Dictionary<string, PropertyDescription> Properties { get; set; } = new Dictionary<string, PropertyDescription>();

        internal Dictionary<string, MethodDescription> Methods { get; set; } = new Dictionary<string, MethodDescription>();

        internal bool Equals(ObjectDescription desc)
        {
            return Events.Keys.SequenceEqual(desc.Events.Keys) &&
                   Properties.Keys.SequenceEqual(desc.Properties.Keys) &&
                   Methods.Keys.SequenceEqual(desc.Methods.Keys) &&
                   Events.Values.SequenceEqual(desc.Events.Values) &&
                   Properties.Values.SequenceEqual(desc.Properties.Values) &&
                   Methods.Values.SequenceEqual(desc.Methods.Values);
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
