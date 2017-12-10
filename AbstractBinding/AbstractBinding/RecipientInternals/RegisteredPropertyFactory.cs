using System;
using System.Reflection;

namespace AbstractBinding.RecipientInternals
{
    internal class RegisteredPropertyFactory
    {
        public RegisteredProperty Create(string objectId, object obj, PropertyInfo propertyInfo)
        {
            return new RegisteredProperty(objectId, obj, propertyInfo);
        }
    }
}