using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredEventFactory
    {
        private readonly IAbstractService _service;
        private readonly ISerializer _serializer;

        public RegisteredEventFactory(IAbstractService service, ISerializer serializer)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public RegisteredEvent Create(string id, object objectId, EventInfo eventInfo)
        {
            return new RegisteredEvent(_service, _serializer, id, objectId, eventInfo);
        }
    }
}
