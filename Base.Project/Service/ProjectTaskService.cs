using Base.DAL;
using Base.Project.Entities;
using Base.Security;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Task.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Base.Events;
using TaskEntities = Base.Task.Entities;

namespace Base.Project.Service
{
    public class ProjectTaskService : BaseCategoryService<ProjectTask>, IProjectTaskService
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        
        public ProjectTaskService(IBaseObjectServiceFacade facade, ITaskService taskService, IProjectService projectService)
            : base(facade)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        public int GetGeneralTaskId(IUnitOfWork unitOfWork, int projectId)
        {
            return _projectService.GetAll(unitOfWork).Where(x => x.ID == projectId)
                .Select(x => x.GeneralTaskID).FirstOrDefault();
        }

        public ProjectTask GetGeneralTask(IUnitOfWork unitOfWork, int projectId)
        {
            return _projectService.GetAll(unitOfWork).Where(x => x.ID == projectId)
                .Select(x => x.GeneralTask).SingleOrDefault();
        }

        public IQueryable<ProjectTask> GetProjectTasks(IUnitOfWork unitOfWork, int projectId, bool? hidden = false)
        {
            return GetAllChildren(unitOfWork, GetGeneralTaskId(unitOfWork, projectId), hidden);
        }

        protected override IObjectSaver<ProjectTask> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ProjectTask> objectSaver)
        {
            if (objectSaver.Src.TaskType == TaskEntities.TaskType.Note)
            {
                objectSaver.Src.Task.Period.End = objectSaver.Src.Task.Period.Start;
                objectSaver.Src.System = true;
            }

            //NOTE: Should be simplified
            if (objectSaver.Src.Status == TaskEntities.TaskStatus.Complete && objectSaver.Src.PercentComplete < 1)
                objectSaver.Src.PercentComplete = 1;
            else if (objectSaver.Src.Status != TaskEntities.TaskStatus.Complete && objectSaver.Src.PercentComplete >= 1)
                objectSaver.Src.Status = TaskEntities.TaskStatus.Complete;

            //Roman's request
            if (objectSaver.Src.Status == TaskEntities.TaskStatus.InProcess && objectSaver.Src.PercentComplete < .05)
                objectSaver.Src.PercentComplete = .05;


            var obj = base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.AssignedTo)
                .SaveOneObject(x => x.AssignedFrom)
                .SaveOneToMany(x => x.Files);

            if (obj.IsNew)
            {
                _taskService.AddItemToChangeHistory(obj.Src.Task);
            }
            else
            {
                var task = _taskService.Get(unitOfWork, obj.Dest.Task.ID);
                obj.Src.Task.ToObject(task, false);
                _taskService.Update(unitOfWork, task);
            }

            return obj;
        }

        public override void Delete(IUnitOfWork unitOfWork, ProjectTask obj)
        {
            var task = _taskService.Get(unitOfWork, obj.Task.ID);

            task.Hidden = true;
            _taskService.Update(unitOfWork, task);

            base.Delete(unitOfWork, obj);
        }

        public List<ProjectTask> CreateOrUpdateCollection(IUnitOfWork unitOfWork, List<ProjectTask> projectTasks)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ProjectTask), TypePermission.Create);
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(ProjectTask), TypePermission.Write);

            var res = new List<ProjectTask>();

            projectTasks.ForEach(task =>
            {
                var objectSaver = GetForSave(unitOfWork, unitOfWork.GetObjectSaver(task, null));

                if (task.ID == 0)
                    unitOfWork.GetRepository<ProjectTask>().Create(objectSaver.Dest);
                else
                    unitOfWork.GetRepository<ProjectTask>().Create(objectSaver.Dest);

                res.Add(objectSaver.Dest);
            });

            unitOfWork.SaveChanges();

            //OnCreateCollection event handler ?

            return res;
        }

        public void CalculateRoot(IUnitOfWork unitOfWork, int projectId)
        {
            var project = _projectService.Get(unitOfWork, projectId);

            var tasks = GetProjectTasks(unitOfWork, projectId).ToList();

            if(!tasks.Any()) return;

            project.Period.Start = tasks.Min(x => x.Period.Start);
            project.Period.End = tasks.Max(x => x.Period.End);
            project.PercentComplete = tasks.Sum(x => x.PercentComplete) / tasks.Count;

            _projectService.Update(unitOfWork, project);

            //NOTE: Check
            //this.UnitOfWork.SaveChanges();
        }

        public void CalculateRoot(IUnitOfWork unitOfWork, Entities.Project project)
        {
            var tasks = GetProjectTasks(unitOfWork, project.ID).ToList();

            if (!tasks.Any()) return;

            project.Period.Start = tasks.Min(x => x.Period.Start);
            project.Period.End = tasks.Max(x => x.Period.End);
            project.PercentComplete = tasks.Sum(x => x.PercentComplete) / tasks.Count;
        }

    }
}
