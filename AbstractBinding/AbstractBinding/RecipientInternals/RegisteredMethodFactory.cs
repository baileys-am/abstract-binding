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
        public RegisteredMethod Create(string objectId, object obj, MethodInfo methodInfo)
        {
            return new RegisteredMethod(objectId, obj, methodInfo);
        }
    }
}