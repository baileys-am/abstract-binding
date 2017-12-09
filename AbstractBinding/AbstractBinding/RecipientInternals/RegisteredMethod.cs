using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AbstractBinding.Messages;

namespace AbstractBinding.RecipientInternals
{
    public class RegisteredMethod
    {
        private readonly IAbstractService _service;
        private readonly ISerializer _serializer;
        private readonly object _obj;
        private readonly MethodInfo _methodInfo;

        public string ObjectId { get; private set; }
        public string MethodId { get; private set; }

        public RegisteredMethod(IAbstractService service, ISerializer serializer, string objectId, object obj, MethodInfo methodInfo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));

            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            MethodId = _methodInfo.Name;
        }
        
        public object Invoke(params object[] objs)
        {
            return _methodInfo.Invoke(_obj, new object[] { objs });
        }
    }
}