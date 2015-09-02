using Base.Service;
using Base.Task.Entities;

namespace Base.Task.Services
{
    public interface IInTaskCategoryService : IBaseCategoryService<TaskCategory>, IReadOnly
    {
    }
}
