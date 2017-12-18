using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal class ObjectDescriptionFactory
    {
        internal ObjectDescription Create<T>()
        {
            // Create event descriptions
            var events = new Dictionary<string, EventDescription>();
            foreach (var eventInfo in typeof(T).GetContractEvents())
            {
                // Store registered event
                events.Add(eventInfo.Name, new EventDescription());
            }

            // Create property descriptions
            var properties = new Dictionary<string, PropertyDescription>();
            foreach (var propertyInfo in typeof(T).GetContractProperties())
            {
                // Store registered property
                properties.Add(propertyInfo.Name, new PropertyDescription());
            }

            // Create methods descriptions
            var methods = new Dictionary<string, MethodDescription>();
            foreach (var methodInfo in typeof(T).GetContractMethods())
            {
                methods.Add(methodInfo.Name, new MethodDescription());
            }

            return new ObjectDescription() { Events = events, Properties = properties, Methods = methods };
        }
    }
}
