using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace AbstractBinding.Tests
{
    class Serializer : ISerializer
    {
        public static bool JsonCompare(object obj1, object obj2)
        {
            var serializer = new Serializer();
            return serializer.SerializeObject(obj1).Equals(serializer.SerializeObject(obj2));
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new StringEnumConverter());
        }

        public T DeserializeObject<T>(string serializedObj)
        {
            return JsonConvert.DeserializeObject<T>(serializedObj, new StringEnumConverter());
        }

        public object DeserializeObject(string serializedObj, Type type)
        {
            return JsonConvert.DeserializeObject(serializedObj, type, new StringEnumConverter());
        }
    }
}
