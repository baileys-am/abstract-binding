using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    public class ObjectDescriptor
    {
        public static string GetEventId<T>(string eventName)
        {
            return typeof(T).GetEvent(eventName).GetFullName();
        }

        public static string GetPropertyId<T>(string propertyName, Type returnType = null, params Type[] types)
        {
            if (returnType == null && types.Length == 0)
            {
                return typeof(T).GetProperty(propertyName).GetFullName();
            }
            else if (returnType == null && types.Length != 0)
            {
                return typeof(T).GetProperty(propertyName, types).GetFullName();
            }
            else if (returnType != null && types.Length == 0)
            {
                return typeof(T).GetProperty(propertyName, returnType).GetFullName();
            }
            else
            {
                return typeof(T).GetProperty(propertyName, returnType, types).GetFullName();
            }
        }

        public static string GetMethodId<T>(string methodName, params Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(T).GetMethod(methodName).GetFullName();
            }
            else
            {
                return typeof(T).GetMethod(methodName, types).GetFullName();
            }
        }

        public static ObjectDescription GetObjectDescription<T>(params Type[] nestedTypes)
        {
            // Create nested object descriptions
            Dictionary<string, ObjectDescription> nestedObjects = new Dictionary<string, ObjectDescription>();
            foreach (var type in GetProperties<T>().Where(kvp => nestedTypes.Contains(kvp.Value.PropertyType)))
            {
                nestedObjects.Add(type.Key, GetObjectDescription(type.Value.PropertyType, nestedTypes));
            }

            // Create event descriptions
            List<string> events = GetEvents<T>().Keys.ToList();

            // Create property descriptions
            List<string> properties = GetProperties<T>().Keys.ToList();

            // Create methods descriptions
            List<string> methods = GetMethods<T>().Keys.ToList();

            return new ObjectDescription() { NestedObjects = nestedObjects, Events = events, Properties = properties, Methods = methods };
        }

        public static ObjectDescription GetObjectDescription(Type type, params Type[] nestedTypes)
        {
            var genericMethod = typeof(ObjectDescriptor).GetMethods().First(m => m.Name == nameof(GetObjectDescription) && m.GetParameters().Count() == 1);
            return (ObjectDescription)genericMethod.MakeGenericMethod(type).Invoke(null, new object[] { nestedTypes });
        }

        internal static IReadOnlyDictionary<string, EventInfo> GetEvents<T>()
        {
            var dict = new Dictionary<string, EventInfo>();
            foreach (var eventInfo in typeof(T).GetContractEvents())
            {
                string eventId = GetEventId<T>(eventInfo.Name);
                dict.Add(eventId, eventInfo);
            }
            return dict;
        }

        internal static IReadOnlyDictionary<string, PropertyInfo> GetProperties<T>()
        {
            var dict = new Dictionary<string, PropertyInfo>();
            foreach (var propertyInfo in typeof(T).GetContractProperties())
            {
                string propertyId = GetPropertyId<T>(propertyInfo.Name);
                dict.Add(propertyId, propertyInfo);
            }
            return dict;
        }

        internal static IReadOnlyDictionary<string, MethodInfo> GetMethods<T>()
        {
            var dict = new Dictionary<string, MethodInfo>();
            foreach (var methodInfo in typeof(T).GetContractMethods())
            {
                string methodId = GetMethodId<T>(methodInfo.Name);
                dict.Add(methodId, methodInfo);
            }
            return dict;
        }
    }
}
