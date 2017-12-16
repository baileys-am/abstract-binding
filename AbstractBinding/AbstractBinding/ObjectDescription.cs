using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal class ObjectDescription
    {
        public Dictionary<string, EventDescription> Events { get; set; } = new Dictionary<string, EventDescription>();

        public Dictionary<string, PropertyDescription> Properties { get; set; } = new Dictionary<string, PropertyDescription>();

        public Dictionary<string, MethodDescription> Methods { get; set; } = new Dictionary<string, MethodDescription>();

        public bool Equals(ObjectDescription objDesc)
        {
            return Events.Keys.SequenceEqual(objDesc.Events.Keys) &&
                   Properties.Keys.SequenceEqual(objDesc.Properties.Keys) &&
                   Methods.Keys.SequenceEqual(objDesc.Methods.Keys);// &&
                   //Events.Values.SequenceEqual(objDesc.Events.Values) &&
                   //Properties.Values.SequenceEqual(objDesc.Properties.Values) &&
                   //Methods.Values.SequenceEqual(objDesc.Methods.Values);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ObjectDescription objDesc:
                    return this.Equals(objDesc);
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
