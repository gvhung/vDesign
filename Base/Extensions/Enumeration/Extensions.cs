using Base.Attributes;
using Framework.Maybe;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Base.Extensions.Enumeration
{
    public static class Extensions
    {
        public static string GetAttrColor(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            ColorAttribute attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(ColorAttribute)) as ColorAttribute;

            return attribute.With(x => x.Value);
        }

        public static string GetAttrDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static int GetAttrValue(this Enum value)
        {
            return (int)((object)value);
        }
    }
}
