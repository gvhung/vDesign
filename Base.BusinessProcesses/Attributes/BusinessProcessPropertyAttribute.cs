using Base.Security;
using System;

namespace Base.BusinessProcesses.Attributes
{
    [Obsolete("Атрибут не на что не повлияет")]
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class BusinessProcessPropertyAttribute : Attribute
    {
        public string Title { get; set; }

        public TypePermission Permissions { get; private set; }

        public BusinessProcessPropertyAttribute() 
        {
            Permissions = TypePermission.Create | TypePermission.Delete | TypePermission.Navigate | TypePermission.Read | TypePermission.Write;
        }
        public BusinessProcessPropertyAttribute(TypePermission permissions) 
        {
            Permissions = permissions;
        }
        public BusinessProcessPropertyAttribute(string title, TypePermission permissions)
        {
            Title = title;

            Permissions = permissions;
        }
    }
}
