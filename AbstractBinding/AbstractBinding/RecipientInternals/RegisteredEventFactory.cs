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
        public RegisteredEvent Create(string id, object objectId, EventInfo eventInfo)
        {
            return new RegisteredEvent(id, objectId, eventInfo);
        }
    }
}
