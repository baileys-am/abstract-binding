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
        private readonly PropertyInfo _propertyInfo;

        public string PropertyId { get; private set; }

        public RegisteredProperty(object obj, PropertyInfo propertyInfo)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            PropertyId = _propertyInfo.Name;
        }

        public object GetValue()
        {
            return _propertyInfo.GetValue(_obj);
        }
    }
}