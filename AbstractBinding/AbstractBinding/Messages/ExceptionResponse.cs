using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Messages
{
    [Serializable]
    public class ExceptionResponse : IResponse
    {
        public ResponseType responseType => ResponseType.exception;

#pragma warning disable IDE1006 // Naming Styles
        public RecipientBindingException exception { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
