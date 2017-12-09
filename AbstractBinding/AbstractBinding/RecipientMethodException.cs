using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    [Serializable]
    public class RecipientMethodException : Exception
    {
        public RecipientMethodException()
        {
        }

        public RecipientMethodException(string message) :
            base(message)
        {
        }

        public RecipientMethodException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public RecipientMethodException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
