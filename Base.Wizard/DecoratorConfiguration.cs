using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Base.Wizard
{
    public class DecoratorConfiguration<TObject>
    {
        private readonly List<PropertyInfo> _properties = new List<PropertyInfo>();

        public List<PropertyInfo> Properties
        {
            get { return _properties; }
        }

        public DecoratorConfiguration<TObject> Decorate<TProperty>(Expression<Func<TObject, TProperty>> selector)
        {
            var type = typeof(TObject);

            var member = selector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                    selector));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",
                    selector));

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    string.Format("Expression '{0}' refers to a property that is not from type {1}.", selector,
                        type));

            _properties.Add(propInfo);

            return this;
        }
    }
}