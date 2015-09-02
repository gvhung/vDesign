using System;

namespace Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class SystemPropertyAttribute : Attribute
    {
        public SystemPropertyAttribute() { }
    }
}
