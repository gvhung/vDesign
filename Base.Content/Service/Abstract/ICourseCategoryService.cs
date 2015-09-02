using Base.Content.Entities;
using Base.DAL;
using Base.Service;
using System.Linq;
namespace Base.Content.Service.Abstract
{
    public interface ICourseCategoryService : IBaseCategoryService<CourseCategory>
    {
        IQueryable<Exercise> GetExercises(IUnitOfWork unitOfWork, int couseID);
    }
}
