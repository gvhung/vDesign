using System.Collections.Generic;

namespace Base.Security
{
    public interface IAccessUser
    {
        string Login { get; set; }
        bool IsActive { get; set; }
        string Password { get; set; }
        bool ChangePasswordOnFirstLogon { get; set; }
        ICollection<Role> Roles { get; set; }
    }
}
