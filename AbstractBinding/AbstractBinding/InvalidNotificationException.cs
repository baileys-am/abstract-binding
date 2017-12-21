using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    [Serializable]
    public class InvalidNotificationException : Exception
    {
        public InvalidNotificationException()
        {
        }

        public InvalidNotificationException(string message) :
            base(message)
        {
        }

        public InvalidNotificationException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public InvalidNotificationException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
