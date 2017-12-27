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
        private readonly ISerializer _serializer;

        public RegisteredEventFactory(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public RegisteredEvent Create(string id, object objectId, EventInfo eventInfo)
        {
            return new RegisteredEvent(_serializer, id, objectId, eventInfo);
        }
    }
}
