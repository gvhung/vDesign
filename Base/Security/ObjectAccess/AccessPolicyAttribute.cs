using System;

namespace Base.Security.ObjectAccess
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class AccessPolicyAttribute : Attribute
    {
        public Type AccessPolicy { get; private set; }

        public AccessPolicyAttribute(Type accessPolicy)
        {
            AccessPolicy = accessPolicy;
        }
    }
}