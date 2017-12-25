using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AbstractBinding.Examples
{
    public class Serializer : ISerializer
    {
        public T DeserializeObject<T>(string serializedObj)
        {
            return JsonConvert.DeserializeObject<T>(serializedObj, new StringEnumConverter());
        }

        public object DeserializeObject(string serializedObj, Type type)
        {
            return JsonConvert.DeserializeObject(serializedObj, type);
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new StringEnumConverter());
        }
    }
}
