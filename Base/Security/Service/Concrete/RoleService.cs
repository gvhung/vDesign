using Base.DAL;
using Base.Service;
using System;
using System.Linq;

namespace Base.Security.Service
{
    public class RoleService : BaseObjectService<Role>, IRoleService
    {
        private readonly ISecurityUserService _securityUserService;

        public RoleService(IBaseObjectServiceFacade facade, ISecurityUserService securityUserService)
            : base(facade)
        {
            _securityUserService = securityUserService;
        }

        protected override IObjectSaver<Role> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Role> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneToMany(x => x.Permissions)
                .SaveOneToMany(x => x.ChildRoles, x => x.SaveOneObject(r => r.Role));
        }

        public Role GetRoleByUserType(IUnitOfWork unitOfWork, UserType userType)
        {
            SystemRole systemRole;

            switch (userType)
            {
                case UserType.Base:
                    systemRole = SystemRole.Base;
                    break;

                case UserType.Expert:
                    systemRole = SystemRole.Expert;
                    break;

                case UserType.Developer:
                    systemRole = SystemRole.Developer;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return this.GetAll(unitOfWork).FirstOrDefault(x => x.SystemRole == systemRole);
        }

        public override Role Update(IUnitOfWork unitOfWork, Role obj)
        {
            _securityUserService.ClearAll();

            return base.Update(unitOfWork, obj);
        }
    }
}
