using Base.Ambient;
using Base.DAL;
using Base.Service;
using Base.Task.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Base.Task.Services
{
    public class InTaskCategoryService : BaseCategoryService<TaskCategory>, IInTaskCategoryService
    {
        private readonly ITaskCategoryService _taskCategoryService;
        private readonly ITaskService _taskService;

        public InTaskCategoryService(IBaseObjectServiceFacade facade, ITaskCategoryService taskCategoryService, ITaskService taskService) : base(facade)
        {
            _taskCategoryService = taskCategoryService;
            _taskService = taskService;
        }

        public override IQueryable<TaskCategory> GetAll(IUnitOfWork unitOfWork, bool? hidden)
        {
            var taskCategories = _taskService.GetAll(unitOfWork, hidden: null)
                .Where(x => x.AssignedToID == AppContext.SecurityUser.ID)
                .Select(x => x.TaskCategory).Distinct().Select(x => new
                {
                    ID = x.ID,
                    ParentID = x.ParentID,
                    Parents = x.sys_all_parents,
                });

            var ids = new List<int>();

            foreach (var category in taskCategories)
            {
                if (category.ParentID != null)
                    ids.AddRange(category.Parents.Split(HCategory.Seperator).Select(HCategory.IdToInt));

                ids.Add(category.ID);
            }

            return _taskCategoryService.GetAll(unitOfWork, hidden).Where(x => ids.Contains(x.ID));

        }
    }
}
