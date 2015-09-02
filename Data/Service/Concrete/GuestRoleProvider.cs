using Base.DAL;
using Base.Security;
using Base.Security.Service;
using Framework.EnumerableExtesions;
using System.Collections.Generic;
using System.Linq;

namespace Data.Service.Concrete
{
    public class GuestRoleProvider : IGuestRoleProvider
    {
        public GuestRoleProvider(IRoleService roleService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            using (var uow = unitOfWorkFactory.CreateSystem())
            {
                var role = roleService.GetAll(uow).FirstOrDefault(x => x.SystemRole == SystemRole.Base);

                if (role != null)
                {
                    List<Role> roles = role.Roles.SelectRecursive(x => x.Roles).Select(x => x.Item).Distinct().ToList();

                    roles.Add(role);

                    _roles = new List<Role>
                    {
                        new Role
                        {
                            ChildRoles = new List<ChildRole>(),
                            Name = "GustRole",
                            SystemRole = SystemRole.Base,
                            Permissions = roles
                                .SelectMany(m => m.Permissions)
                                .OrderBy(m => m.FullName)
                                .Select(x => new Permission(){
                                    FullName = x.FullName,
                                    AllowRead = x.AllowRead,
                                    AllowWrite = x.AllowWrite,
                                    AllowCreate = x.AllowCreate,
                                    AllowDelete = x.AllowDelete,
                                    AllowNavigate = x.AllowNavigate,
                                }).ToList()
                        }
                    };
                }
            }
        }

        private ICollection<Role> _roles = new List<Role>();

        public ICollection<Role> GetRoles()
        {
            return _roles;
        }
    }
}
