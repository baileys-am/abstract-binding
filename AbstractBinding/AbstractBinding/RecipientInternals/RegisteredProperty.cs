﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredProperty
    {
        private readonly object _obj;
        private readonly PropertyInfo _propertyInfo;

        internal string ObjectId { get; private set; }
        internal string PropertyId { get; private set; }

        internal RegisteredProperty(string objectId, object obj, PropertyInfo propertyInfo)
        {
            ObjectId = objectId ?? throw new ArgumentNullException(nameof(objectId));
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            PropertyId = _propertyInfo.Name;
        }

        internal static RegisteredProperty Create(string objectId, object obj, PropertyInfo propertyInfo)
        {
            return new RegisteredProperty(objectId, obj, propertyInfo);
        }

        internal object GetValue()
        {
            try
            {
                return _propertyInfo.GetValue(_obj);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to get {PropertyId} value on {ObjectId}", ex);
            }
        }

        internal void SetValue(object value)
        {
            try
            {
                _propertyInfo.SetValue(_obj, value);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to get {PropertyId} value on {ObjectId}", ex);
            }
        }
    }
}