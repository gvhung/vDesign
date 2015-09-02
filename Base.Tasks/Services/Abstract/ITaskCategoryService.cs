using Base.DAL;
using Base.Service;
using Base.Task.Entities;

namespace Base.Task.Services
{
    public interface ITaskCategoryService : IBaseCategoryService<TaskCategory>
    {
        void DuplicateCategory(IUnitOfWork unitOfWork, HCategory cat, string SysName);
    }
}
