using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AbstractBinding.Examples
{
    public class Serializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            Converters = new List<JsonConverter>() { new StringEnumConverter() }
        };

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, _settings);
        }

        public T DeserializeObject<T>(string serializedObj)
        {
            return JsonConvert.DeserializeObject<T>(serializedObj, _settings);
        }

        public object DeserializeObject(string serializedObj, Type type)
        {
            return JsonConvert.DeserializeObject(serializedObj, type, _settings);
        }
    }
}
