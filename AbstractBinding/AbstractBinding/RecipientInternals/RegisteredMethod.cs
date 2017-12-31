﻿using System;
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
        private readonly MethodInfo _methodInfo;

        public string ObjectId { get; private set; }
        public string MethodId { get; private set; }

        public RegisteredMethod(string objectId, object obj, MethodInfo methodInfo)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));

            ObjectId = String.IsNullOrEmpty(objectId) ? throw new ArgumentNullException(nameof(objectId)) : objectId;
            MethodId = _methodInfo.GetFullName();
        }
        
        public object Invoke(object[] objs)
        {
            try
            {
                return _methodInfo.Invoke(_obj, objs);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to invoke {MethodId} on {ObjectId}", ex);
            }
        }
    }
}