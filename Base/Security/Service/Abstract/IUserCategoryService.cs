using Base.DAL;
using Base.Service;

namespace Base.Security.Service
{
    public interface IUserCategoryService : IBaseCategoryService<UserCategory>
    {
        UserCategory GetCompany(IUnitOfWork unitOfWork, UserCategory category);
    }
}
