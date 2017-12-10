using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    [Serializable]
    public class RecipientBindingException : Exception
    {
        public RecipientBindingException()
        {
        }

        public RecipientBindingException(string message) :
            base(message)
        {
        }

        public RecipientBindingException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public RecipientBindingException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
