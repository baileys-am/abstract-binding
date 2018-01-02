using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class ObjectBinding
    {
#pragma warning disable IDE1006 // Naming Styles
        public List<string> events { get; set; } = new List<string>();

        public Dictionary<string, NestedObjectBinding> properties { get; set; } = new Dictionary<string, NestedObjectBinding>();

        public List<string> methods { get; set; } = new List<string>();
#pragma warning restore IDE1006 // Naming Styles

        public ObjectDescription ToObjectDescription()
        {
            return new ObjectDescription()
            {
                NestedObjects = properties.Where(kvp => kvp.Value.isBinded).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.objectBinding.ToObjectDescription()),
                Events = events.ToList(),
                Properties = properties.Keys.ToList(),
                Methods = methods.ToList()
            };
        }
    }
}
