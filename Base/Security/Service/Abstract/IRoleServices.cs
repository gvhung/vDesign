using Base.DAL;
using Base.Service;

namespace Base.Security.Service
{
    public interface IRoleService : IBaseObjectService<Role>
    {
        Role GetRoleByUserType(IUnitOfWork unitOfWork, UserType userType);
    }
}
