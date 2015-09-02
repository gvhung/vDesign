using System;

namespace Base.Attributes
{
    [AttributeUsageAttribute(AttributeTargets.All)]
    public class ColorAttribute: Attribute
    {
        public string Value { get; private set; }

        public ColorAttribute(string linkedPropertyName)
        {
            this.Value = linkedPropertyName;
        }
    }
}
