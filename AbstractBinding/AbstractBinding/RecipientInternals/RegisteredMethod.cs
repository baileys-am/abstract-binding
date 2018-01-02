using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredMethod
    {
        private readonly object _obj;
        private readonly KeyValuePair<string, MethodInfo> _methodInfo;

        internal string ObjectId { get; private set; }
        internal string MethodId => _methodInfo.Key;

        internal RegisteredMethod(string objectId, object obj, KeyValuePair<string, MethodInfo> methodInfo)
        {
            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _methodInfo = methodInfo;
        }

        internal static RegisteredMethod Create(string objectId, object obj, KeyValuePair<string, MethodInfo> methodInfo)
        {
            return new RegisteredMethod(objectId, obj, methodInfo);
        }

        internal object Invoke(object[] objs)
        {
            try
            {
                return _methodInfo.Value.Invoke(_obj, objs);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to invoke {MethodId} on {ObjectId}", ex);
            }
        }
    }
}