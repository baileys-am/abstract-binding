using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredObjectFactory
    {
        private readonly RegisteredEventFactory _eventFactory;
        private readonly RegisteredMethodFactory _methodFactory;

        public RegisteredObjectFactory(RegisteredEventFactory eventFactory, RegisteredMethodFactory methodFactory)
        {
            _eventFactory = eventFactory ?? throw new ArgumentNullException(nameof(eventFactory));
            _methodFactory = methodFactory ?? throw new ArgumentNullException(nameof(methodFactory));
        }

        public RegisteredObject Create(string objectId, object obj)
        {
            return new RegisteredObject(_eventFactory, _methodFactory, objectId, obj);
        }
    }
}
