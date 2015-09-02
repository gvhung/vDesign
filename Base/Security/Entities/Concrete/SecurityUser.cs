using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Security
{
    public class SecurityUser : ISecurityUser
    {
        private readonly IEnumerable<string> _roles = null;
        private readonly IEnumerable<int> _roleIds = null;
        private readonly IReadOnlyList<SystemRole> _sysroles = null;
        private readonly Dictionary<string, List<TypePermission>> _dicPermissions;

        public SecurityUser(User user, UserCategory company = null)
        {
            _dicPermissions = new Dictionary<string, List<TypePermission>>();

            if (user == null) return;

            this.ID = user.ID;
            this.Login = user.Login;
            this.Email = user.Email;
            this.IsAdmin = user.Roles.Any(m => m.SystemRole == SystemRole.Admin);
            this.IsUnregistered = user.IsUnregistered;
            this.UserName = user.FullName;
            this.ChangePasswordOnFirstLogon = user.ChangePasswordOnFirstLogon;
            this.ChangePassword = user.ChangePassword;
            this.ImageID = user.Image.WithStruct(x => x.FileID, Guid.Empty);
                
            if (user.UserCategory != null)
            {
                this.Dept = new UserDept()
                {
                    ID = user.CategoryID,
                    Name = user.UserCategory.Name,
                };
            }

            if (company != null)
            {
                this.Company = new UserCompany()
                {
                    ID = company.ID,
                    Key = company.CompanyGuid,
                    Name = company.Name,
                };
            }

            ICollection<Role> roles =  new List<Role>();

            GetRoles(user.Roles, roles);

            _roles = roles.Select(m => m.Name.ToUpper()).ToList();
            _roleIds = roles.Select(m => m.ID).ToList();

            _sysroles = roles.Where(x => x.SystemRole != null).Select(m => m.SystemRole ?? 0).ToList().AsReadOnly();

            var permissions = roles.SelectMany(m => m.Permissions).OrderBy(m => m.FullName).ToList();

            permissions.ForEach(perm =>
            {
                if (!_dicPermissions.ContainsKey(perm.FullName))
                {
                    _dicPermissions.Add(perm.FullName, new List<TypePermission>());
                }

                var listPerm = _dicPermissions[perm.FullName];

                if (perm.AllowCreate && !listPerm.Contains(TypePermission.Create))
                {
                    listPerm.Add(TypePermission.Create);
                }
                if (perm.AllowDelete && !listPerm.Contains(TypePermission.Delete))
                {
                    listPerm.Add(TypePermission.Delete);
                }
                if (perm.AllowNavigate && !listPerm.Contains(TypePermission.Navigate))
                {
                    listPerm.Add(TypePermission.Navigate);
                }
                if (perm.AllowRead && !listPerm.Contains(TypePermission.Read))
                {
                    listPerm.Add(TypePermission.Read);
                }
                if (perm.AllowWrite && !listPerm.Contains(TypePermission.Write))
                {
                    listPerm.Add(TypePermission.Write);
                }
            });
        }
        public int ID { get; private set; }
        public string Login { get; private set; }
        public string Email { get; private set; }
        public string UserName { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsGuest { get; set; }

        public string GetKey()
        {
            return IsGuest ? this.Login : this.ID.ToString();
        }

        public bool IsUnregistered { get; private set; }
        public Guid ImageID { get; private set; }
        public bool ChangePasswordOnFirstLogon { get; private set; }
        public DateTime? ChangePassword { get; private set; }
        public UserDept Dept { get; private set; }
        public UserCompany Company { get; private set; }

        public bool IsPermission(Type type, TypePermission typePermission)
        {
            if (this.IsAdmin) return true;

            return _dicPermissions.ContainsKey(type.FullName) && _dicPermissions[type.FullName].Contains(typePermission);
        }

        public bool IsPermission<T>(TypePermission typePermission)
        {
            return IsPermission(typeof(T), typePermission);
        }


        public bool IsRole(string role)
        {
            if (_roles != null && !String.IsNullOrEmpty(role))
            {
                return _roles.Contains(role.ToUpper());
            }

            return false;
        }

        public bool IsRole(int roleID)
        {
            return _roleIds != null && _roleIds.Contains(roleID);
        }

        public bool IsSysRole(SystemRole sysrole)
        {
            return _roles != null && _sysroles.Any(x => x == sysrole);
        }

        private static void GetRoles(IEnumerable<Role> roles, ICollection<Role> resRoles)
        {
            if (resRoles == null) throw new ArgumentNullException("resRoles");

            foreach (var role in roles.Where(role => resRoles.All(x => x.ID != role.ID)))
            {
                resRoles.Add(role);

                if(role.Roles != null)
                    GetRoles(role.Roles, resRoles);
            }
        }
    }


    public class UserDept
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class UserCompany
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid? Key { get; set; }
    }
}
