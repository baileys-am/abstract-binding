using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    public class ObjectDescription
    {
        public List<string> Events { get; set; } = new List<string>();

        public List<string> Properties { get; set; } = new List<string>();

        public List<string> Methods { get; set; } = new List<string>();

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
