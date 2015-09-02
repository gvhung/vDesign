using Base.Service;

namespace Base.Security.ObjectAccess.Services
{
    public class UserCategoryAccessService : BaseObjectService<UserAccess>, IUserCategoryAccessService
    {
        public UserCategoryAccessService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
