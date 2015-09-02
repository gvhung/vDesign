using System;

namespace Base.Security
{
    public interface ISecurityUser
    {
        int ID { get; }
        string Login { get; }
        string Email { get; }
        string UserName { get; }
        bool IsAdmin { get; }
        bool ChangePasswordOnFirstLogon { get; }
        DateTime? ChangePassword { get; }
        Guid ImageID { get; }
        UserDept Dept { get; }
        UserCompany Company { get; }
        bool IsPermission(Type type, TypePermission typePermission);
        bool IsPermission<T>(TypePermission typePermission);
        bool IsRole(string role);
        bool IsRole(int roleID);
        bool IsSysRole(SystemRole sysrole);
        bool IsUnregistered { get; }
        bool IsGuest { get; set; }
        string GetKey();
    }
}
