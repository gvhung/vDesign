using Base.Service;

namespace Base.Security.Service
{
    public class ChildRoleService : BaseObjectService<ChildRole>, IChildRoleService
    {
        public ChildRoleService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
