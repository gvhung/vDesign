using Base.Service;

namespace Base.Security.Service
{
    public class PermissionService : BaseObjectService<Permission>, IPermissionService
    {
        public PermissionService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
