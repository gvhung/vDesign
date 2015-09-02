using System.Collections.Generic;
using Base.Project.Entities;
using Base.Service;
using System.Linq;
using Base.DAL;

namespace Base.Project.Service
{
    public interface IProjectTaskService : IBaseCategoryService<ProjectTask>
    {
        ProjectTask GetGeneralTask(IUnitOfWork unitOfWork, int projectId);
        int GetGeneralTaskId(IUnitOfWork unitOfWork, int projectId);
        IQueryable<ProjectTask> GetProjectTasks(IUnitOfWork unitOfWork, int projectId, bool? hidden = false);
        void CalculateRoot(IUnitOfWork unitOfWork, int projectId);
        void CalculateRoot(IUnitOfWork unitOfWork, Entities.Project project);
        List<ProjectTask> CreateOrUpdateCollection(IUnitOfWork unitOfWork, List<ProjectTask> projectTasks);
    }
}
