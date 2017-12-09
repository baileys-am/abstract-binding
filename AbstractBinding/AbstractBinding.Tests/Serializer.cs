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
    class Serializer
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new StringEnumConverter());
        }

        public static T Deserialize<T>(string serializedObj)
        {
            return JsonConvert.DeserializeObject<T>(serializedObj, new StringEnumConverter());
        }

        public static bool JsonCompare(object obj1, object obj2)
        {
            return Serialize(obj1).Equals(Serialize(obj2));
        }
    }
}
