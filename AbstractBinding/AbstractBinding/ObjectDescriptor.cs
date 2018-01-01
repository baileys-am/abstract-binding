using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    public class ObjectDescriptor
    {
        public static string GetEventId<T>(string eventName)
        {
            return typeof(T).GetEvent(eventName).Name;
        }

        public static string GetPropertyId<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName).Name;
        }

        public static string GetMethodId<T>(string methodName)
        {
            return typeof(T).GetMethod(methodName).GetFullName();
        }

        public static ObjectDescription GetObjectDescription<T>()
        {
            // Create event descriptions
            var events = new List<string>();
            foreach (var eventInfo in typeof(T).GetContractEvents())
            {
                // Store registered event
                events.Add(eventInfo.Name);
            }

            // Create property descriptions
            var properties = new List<string>();
            foreach (var propertyInfo in typeof(T).GetContractProperties())
            {
                // Store registered property
                properties.Add(propertyInfo.Name);
            }

            // Create methods descriptions
            var methods = new List<string>();
            foreach (var methodInfo in typeof(T).GetContractMethods())
            {
                methods.Add(methodInfo.GetFullName());
            }

            return new ObjectDescription(events, properties, methods);
        }
    }
}
