using Base.DAL;
using Base.Entities.Complex;
using Base.Project.Entities;
using Base.Security;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Task.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Ambient;

namespace Base.Project.Service
{
    public class ProjectService : BaseCategorizedItemService<Entities.Project>, IProjectService
    {
        public ProjectService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }


        public override void Delete(IUnitOfWork unitOfWork, Entities.Project obj)
        {
            var generalId = obj.GeneralTask.ID;

            base.Delete(unitOfWork, obj);

            var projectTaskRepo = unitOfWork.GetRepository<ProjectTask>();

            var strId = HCategory.IdToString(generalId);

            var projectTasks = projectTaskRepo.All().Where(node => (node.sys_all_parents != null && node.sys_all_parents.Contains(strId)) || node.ID == generalId).ToList();
            
            projectTasks.ForEach(task => {
                task.Hidden = true;
                task.Task.Hidden = true;
                projectTaskRepo.Update(task);
            });
        }

        public override IQueryable<Entities.Project> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryId, bool? hidden = false)
        {
            var strId = HCategory.IdToString(categoryId);
            return GetAll(unitOfWork, hidden).Where(a => (a.ProjectCategory.sys_all_parents != null && a.ProjectCategory.sys_all_parents.Contains(strId)) || a.ProjectCategory.ID == categoryId);
        }

        protected override IObjectSaver<Entities.Project> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Entities.Project> objectSaver)
        {
            var obj = base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.CreatedBy)
                .SaveOneObject(x => x.ChargeUser)
                .SaveOneToMany(x => x.Files)
                .SaveOneToMany(x => x.InterestedContractors, x => x.SaveOneObject(c => c.Object))
                .SaveOneToMany(x => x.InterestedEmployees, x => x.SaveOneObject(c => c.Object));


            var taskCatRep = unitOfWork.GetRepository<TaskCategory>();
            var projCatRepo = unitOfWork.GetRepository<ProjectCategory>();

            if (obj.IsNew)
            {
                if (obj.Dest.GeneralTask != null)
                {
                    var rootProjectCategory = taskCatRep.Find(x => x.SysName == "ProjectCategory");
                    var currentProjectCategory = projCatRepo.Find(obj.Dest.CategoryID);

                    var folderName = new LinkBaseObject(currentProjectCategory).ToString();

                    var targetCategory = taskCatRep.Find(x => x.SysName == folderName);

                    var cat = new TaskCategory()
                    {
                        Name = obj.Dest.Title,
                        SysName = ""
                    };

                    if(targetCategory != null)
                        cat.SetParent(targetCategory);
                    else if(rootProjectCategory != null)
                        cat.SetParent(rootProjectCategory);


                    obj.Dest.GeneralTask.Task.TaskCategory = cat;
                    obj.Dest.GeneralTask.Task.System = true;
                }

            }
            else
            {
                var repTask = unitOfWork.GetRepository<Task.Entities.Task>();

                var task = repTask.Find(x => x.ID == obj.Dest.GeneralTask.Task.ID);

                obj.Dest.GeneralTask.Task.ToObject(task);

                task.System = true;

                if (task.TaskCategory.SysName == new LinkBaseObject(obj.Dest).ToString())
                    task.TaskCategory.Name = obj.Dest.Title;

                repTask.Update(task);
            }

            return objectSaver;
        }

        public override Entities.Project Create(IUnitOfWork unitOfWork, Entities.Project obj)
        {
            var project = base.Create(unitOfWork, obj);

            if (project.GeneralTask != null)
            {
                project.GeneralTask.Task.TaskCategory.SysName = new LinkBaseObject(project).ToString();
                unitOfWork.GetRepository<Entities.Project>().Update(project);
            }

            return project;
        }

        public override Entities.Project CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            var project = (Entities.Project)obj;

            if (project == null)
            {
                if (AppContext.SecurityUser.IsPermission<Entities.Project>(TypePermission.Read))
                {
                    project = new Entities.Project
                    {
                        GeneralTask = new ProjectTask()
                        {
                            Hidden = true,
                            Task = new Task.Entities.Task()
                            {
                                Hidden = true,
                                Period = new Period() {Start = DateTime.Now.Date, End = DateTime.Now.AddDays(1)}
                            }
                        }
                    };

                    return project;
                }
            }

            return base.CreateOnGroundsOf(unitOfWork, obj);
        }

        public Entities.Project CreateProjectFromTask(IUnitOfWork unitOfWork, int projectTaskId)
        {
            var projectTask = unitOfWork.GetRepository<ProjectTask>().Find(x => x.ID == projectTaskId);

            if (projectTask == null) return null;

            if (GetAll(unitOfWork).Any(x => x.GeneralTaskID == projectTaskId)) return null;

            var project = new Entities.Project()
            {
                GeneralTaskID = projectTaskId
            };

            var generalProjectTaskId = projectTask.Level == 0 ? projectTask.ID : projectTask.GetParentID(0);

            var parentProject = GetAll(unitOfWork)
                .Where(x => x.GeneralTaskID == generalProjectTaskId)
                .Select(x => new
                {
                    x.ID, 
                    x.CategoryID
                }).FirstOrDefault();

            if (parentProject == null) return null;

            project.RootId = parentProject.ID;
            project.CategoryID = parentProject.CategoryID;

            Create(unitOfWork, project);

            return project;
        }
        
    }
}
