using Base.Security.Service.Abstract;
using Base.Service;

namespace Base.Security.Service.Concrete
{
    public class ProfileService : BaseProfileService<Profile>, IProfileService
    {
        public ProfileService(ISecurityUserService securityUserService, IBaseObjectServiceFacade facade)
            : base(securityUserService, facade)
        {
        }
    }
}
