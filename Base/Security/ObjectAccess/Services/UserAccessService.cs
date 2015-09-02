using Base.Service;

namespace Base.Security.ObjectAccess.Services
{
    public class UserAccessService : BaseObjectService<UserAccess>, IUserAccessService
    {
        public UserAccessService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
