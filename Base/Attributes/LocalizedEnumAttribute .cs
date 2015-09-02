using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Base.Attributes
{
    public class LocalizedEnumAttribute : DescriptionAttribute
    {
        private PropertyInfo _nameProperty;
        private Type _resourceType;

        public LocalizedEnumAttribute(string displayNameKey)
            : base(displayNameKey)
        {

        }

        public Type NameResourceType
        {
            get
            {
                return _resourceType;
            }
            set
            {
                _resourceType = value;

                _nameProperty = _resourceType.GetProperty(this.Description, BindingFlags.Static | BindingFlags.Public);
            }
        }

        public override string Description
        {
            get
            {
                if (_nameProperty == null)
                {
                    return base.Description;
                }

                return (string)_nameProperty.GetValue(_nameProperty.DeclaringType, null);
            }
        }
    }

    public static class EnumExtender
    {
        public static string GetLocalizedDescription(this Enum enumeration)
        {
            if (enumeration == null)
                return null;

            string description = enumeration.ToString();

            FieldInfo fieldInfo = enumeration.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Any())
                return attributes[0].Description;

            return description;
        }
    }
}
