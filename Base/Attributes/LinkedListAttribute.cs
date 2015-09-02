using System;

namespace Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class LinkedListAttribute : Attribute
    {
        public string LinkedPropertyName { get; set; }

        public LinkedListAttribute(string linkedPropertyName)
        {
            this.LinkedPropertyName = linkedPropertyName;
        }
    }
}
