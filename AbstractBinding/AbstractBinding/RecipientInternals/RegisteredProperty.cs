using System;
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
        private readonly KeyValuePair<string, PropertyInfo> _propertyInfo;

        internal string ObjectId { get; private set; }
        internal string PropertyId => _propertyInfo.Key;

        internal RegisteredProperty(string objectId, object obj, KeyValuePair<string, PropertyInfo> propertyInfo)
        {
            ObjectId = objectId ?? throw new ArgumentNullException(nameof(objectId));
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _propertyInfo = propertyInfo;
        }

        internal static RegisteredProperty Create(string objectId, object obj, KeyValuePair<string, PropertyInfo> propertyInfo)
        {
            return new RegisteredProperty(objectId, obj, propertyInfo);
        }

        internal object GetValue()
        {
            try
            {
                return _propertyInfo.Value.GetValue(_obj);
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
                _propertyInfo.Value.SetValue(_obj, value);
            }
            catch (Exception ex)
            {
                throw new RecipientBindingException($"Failed to get {PropertyId} value on {ObjectId}", ex);
            }
        }
    }
}