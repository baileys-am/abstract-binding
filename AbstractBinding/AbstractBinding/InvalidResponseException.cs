using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    [Serializable]
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException() : base()
        {
        }

        public InvalidResponseException(string message) : base(message)
        {
        }

        public InvalidResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
