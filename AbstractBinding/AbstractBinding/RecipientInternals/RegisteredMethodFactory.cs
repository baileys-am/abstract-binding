using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredMethodFactory
    {
        private readonly IAbstractService _service;
        private readonly ISerializer _serializer;

        public RegisteredMethodFactory(IAbstractService service, ISerializer serializer)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public RegisteredMethod Create(string objectId, object obj, MethodInfo methodInfo)
        {
            return new RegisteredMethod(_service, _serializer, objectId, obj, methodInfo);
        }
    }
}