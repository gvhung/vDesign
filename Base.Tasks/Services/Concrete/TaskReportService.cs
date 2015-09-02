using Base.Ambient;
using Base.DAL;
using Base.Security;
using Base.Service;
using Base.Task.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Task.Services
{
    public class TaskReportService : BaseCategorizedItemService<TaskReport>, ITaskReportService
    {
        private readonly ITaskService _taskService;

        public TaskReportService(IBaseObjectServiceFacade facade, ITaskService taskService) : base(facade)
        {
            _taskService = taskService;
        }

        public override IQueryable<TaskReport> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return GetAllTasks(unitOfWork).GroupBy(x => x.AssignedTo).Select(x => new TaskReport() { ID = x.Key.ID, AssignedTo = x.Key });
        }

        private IQueryable<Entities.Task> GetAllTasks(IUnitOfWork unitOfWork, int categoryId = 0)
        {
            var tasks = _taskService.GetAll(unitOfWork).Where(x => !x.System);

            if (AppContext.SecurityUser.IsSysRole(SystemRole.Ceo) || AppContext.SecurityUser.IsAdmin)
            {
                if (AppContext.SecurityUser.Company != null)
                {
                    var strId = HCategory.IdToString(AppContext.SecurityUser.Company.ID);

                    tasks = tasks.Where(x =>
                        x.AssignedFrom.UserCategory.sys_all_parents.Contains(strId) || x.AssignedFrom.CategoryID == AppContext.SecurityUser.Company.ID
                        || x.AssignedTo.UserCategory.sys_all_parents.Contains(strId) || x.AssignedTo.CategoryID == AppContext.SecurityUser.Company.ID);
                }
            }
            else
            {
                var strId = HCategory.IdToString(AppContext.SecurityUser.Dept.ID);

                tasks = tasks.Where(x => x.AssignedTo.UserCategory.sys_all_parents.Contains(strId) || x.AssignedTo.CategoryID == AppContext.SecurityUser.Dept.ID);
            }

            if (categoryId != 0)
            {
                var strId = HCategory.IdToString(categoryId);

                tasks = tasks.Where(a => (a.TaskCategory.sys_all_parents != null && a.TaskCategory.sys_all_parents.Contains(strId)) || a.TaskCategory.ID == categoryId);
            }

            return tasks.Where(x => x.AssignedToID != null);
        }

        public override IQueryable<TaskReport> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden)
        {
            return GetAllTasks(unitOfWork, categoryID)
                .GroupBy(x => x.AssignedTo)
                .Select(g => new TaskReport()
                {
                    ID = g.OrderBy(x => x.TaskCategory.sys_all_parents).FirstOrDefault().ID,
                    AssignedTo = g.Key,
                    CountNew = g.Count(x => x.Status == TaskStatus.New),
                    CountActive = g.Count(x => x.Status == TaskStatus.Viewed || x.Status == TaskStatus.InProcess || x.Status == TaskStatus.Rework),
                    CountExpired = g.Count(x => (x.Status == TaskStatus.New || x.Status == TaskStatus.Viewed || x.Status == TaskStatus.InProcess || x.Status == TaskStatus.Rework) && (x.Period.End < DateTime.Now)),
                    CountRevise = g.Count(x => x.Status == TaskStatus.Revise),
                    CountComplete = g.Count(x => x.Status == TaskStatus.Complete),
                    SortOrder = 0,
                });
        }

        public override IQueryable<TaskReport> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            return GetAllCategorizedItems(unitOfWork, categoryID, hidden);
        }

        public override TaskReport Get(IUnitOfWork unitOfWork, int id)
        {
            var firstTask = _taskService.Get(unitOfWork, id);

            var startDate = DateTime.Now.AddMonths(-3);

            var tasks = GetAllTasks(unitOfWork, firstTask.CategoryID)
                .Where(x => x.AssignedToID == firstTask.AssignedToID)
                .Where(x => x.Status == TaskStatus.New || x.Status == TaskStatus.Viewed || 
                    x.Status == TaskStatus.InProcess || x.Status == TaskStatus.Rework || 
                    x.Status == TaskStatus.Revise || x.Status == TaskStatus.Complete)
                .Where(x => x.CompliteDate == null || x.CompliteDate >= startDate);
                //.ToList() - Removed    


            var taskRep = tasks
                .GroupBy(x => x.AssignedTo)
                .Select(g => new TaskReport()
                                 {
                                     ID = g.FirstOrDefault().ID,
                                     AssignedTo = g.Key,
                                     NewTasks = g.Where(x => x.Status == TaskStatus.New).ToList(),
                                     ActiveTasks = g.Where(x => x.Status == TaskStatus.Viewed || x.Status == TaskStatus.InProcess || x.Status == TaskStatus.Rework).ToList(),
                                     
                                     ExpiredTasks = g.Where(x => x.Status == TaskStatus.New || x.Status == TaskStatus.Viewed || 
                                         x.Status == TaskStatus.InProcess || x.Status == TaskStatus.Rework)
                                         .Where(x => x.Period.End < DateTime.Now).ToList(),
                                     
                                     ReviseTasks = g.Where(x => x.Status == TaskStatus.Revise).ToList(),
                                     CompleteTasks = g.Where(x => x.Status == TaskStatus.Complete).ToList(),
                                 }).FirstOrDefault();

            return taskRep ?? new TaskReport()
            {
                ID = firstTask.ID,
                AssignedTo = firstTask.AssignedTo,
                NewTasks = new List<Entities.Task>(),
                ActiveTasks = new List<Entities.Task>(),
                ExpiredTasks = new List<Entities.Task>(),
                ReviseTasks = new List<Entities.Task>(),
                CompleteTasks = new List<Entities.Task>(),
            };
        }
    }
}
