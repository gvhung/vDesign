using Base.Ambient;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Events;
using Base.BusinessProcesses.Exceptions;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Entities.Complex;
using Base.Events;
using Base.Security;
using Base.Service;
using Base.Task.Entities;
using Base.Task.Services;
using Framework.Extensions;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Base.Security.ObjectAccess;
using Base.Validation;


namespace Base.BusinessProcesses.Services.Concrete
{
    public enum PerformerType { Admin = 0, Curator = 1, Performer = 2, Regular = 3, Denied = 4 }

    // In transient scope for workflow transaction !
    public class WorkflowService : BaseCategorizedItemService<Workflow>, IWorkflowService
    {
        private readonly IWorkflowServiceFacade _serviceFacade;
        private readonly IStageUserService _stageUserService;
        private readonly ITaskService _taskService;
        private readonly IUnitOfWorkFactory _uowFactory;
        private IWFObjectService _objectService;
        private readonly IWorkflowContextService _workflowContextService;

        public WorkflowService(
            ITaskService taskService,
            IStageUserService stageUserService,
            IWorkflowServiceFacade serviceFacade,
            IBaseObjectServiceFacade facade,
            IUnitOfWorkFactory uowFactory,
            IWorkflowContextService contextService)
            : base(facade)
        {
            _taskService = taskService;
            _stageUserService = stageUserService;
            _serviceFacade = serviceFacade;
            _uowFactory = uowFactory;
            _workflowContextService = contextService;
        }


        public override IQueryable<Workflow> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return
                GetAll(unitOfWork, hidden)
                    .Where(
                        a => a.IsTemplate &&
                            (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) ||
                            a.Category_.ID == categoryID);
        }

        public override IQueryable<Workflow> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden)
        {
            return
                GetAll(unitOfWork).Where(a => a.IsTemplate && a.CategoryID == categoryID);
        }

        public void OnBPObjectCreate(object sender, BaseObjectEventArgs e)
        {
            var unitOfWork = e.UnitOfWork;
            var obj = e.Object as IBPObject;
            _objectService = sender as IWFObjectService;

            if (obj != null && _objectService != null)
            {
                var typeStr = obj.GetType().GetBaseObjectType().FullName;
                var systemUnitOfWork = _uowFactory.CreateSystem(unitOfWork);

                if (obj.InitWorkflow == null)
                {
                    obj.InitWorkflow = obj.InitWorkflow ?? (e.ObjectSrc as IBPObject).With(x => x.InitWorkflow);
                    obj.InitWorkflow = obj.InitWorkflow ?? loadWorkflow(obj, unitOfWork, typeStr);

                    if (obj.InitWorkflow != null && typeStr == obj.InitWorkflow.ObjectType)
                    {
                        obj.WorkflowContext = new WorkflowContext { Workflow = obj.InitWorkflow };

                        systemUnitOfWork.GetRepository<WorkflowContext>().Create(obj.WorkflowContext);
                        systemUnitOfWork.SaveChanges();

                        if (e.Object is IAccessibleObject)
                            _serviceFacade.CreateAccessItem(systemUnitOfWork, e.Object);

                        StartWorkflow(systemUnitOfWork, _objectService, obj as BaseObject, obj.WorkflowContext);
                    }
                    else
                    {
                        throw ExceptionHelper.CriticalError("Тип бизнесс-процесса указан не верно");
                    }
                }
                else
                {
                    if (obj.WorkflowContext == null)
                    {
                        obj.WorkflowContext = new WorkflowContext { Workflow = obj.InitWorkflow };
                        systemUnitOfWork.GetRepository<WorkflowContext>().Create(obj.WorkflowContext);
                        systemUnitOfWork.SaveChanges();
                    }
                    StartWorkflow(systemUnitOfWork, _objectService, obj as BaseObject, obj.WorkflowContext);
                }
            }
        }

        private void CreateTemplate(IBPObject obj, ISystemUnitOfWork systemUnitOfWork)
        {
            var workflowTemplate = _serviceFacade.CloneWorkflow(obj.InitWorkflow);
            workflowTemplate.IsTemplate = true;
            systemUnitOfWork.GetRepository<Workflow>().Create(workflowTemplate);
        }

        private Workflow loadWorkflow(IBPObject obj, IUnitOfWork unitOfWork, string typeStr)
        {
            Workflow autoWf;

            var wfID = _objectService.GetWorkflowID(unitOfWork, obj as BaseObject);
            if (wfID == Workflow.Default)
            {
                autoWf =
                    GetAll(unitOfWork)
                        .FirstOrDefault(x => x.IsTemplate && x.ObjectType == typeStr && !x.Hidden);
            }
            else
            {
                autoWf = unitOfWork.GetRepository<Workflow>().Find(x => x.ID == wfID);
            }

            return autoWf;
        }

        private Workflow CloneWF(IBPObject obj, IUnitOfWork unitOfWork, string typeStr)
        {
            Workflow autoWf;

            if (obj.InitWorkflow == null)
            {
                var wfID = _objectService.GetWorkflowID(unitOfWork, obj as BaseObject);
                if (wfID == Workflow.Default)
                {
                    autoWf =
                        GetAll(unitOfWork)
                            .FirstOrDefault(x => x.IsTemplate && x.ObjectType == typeStr && !x.Hidden);
                }
                else
                {
                    autoWf = unitOfWork.GetRepository<Workflow>().Find(x => x.ID == wfID);
                }

                if (autoWf != null)
                {
                    autoWf = _serviceFacade.CloneWorkflow(autoWf);
                    autoWf.IsTemplate = false;
                    obj.InitWorkflow = autoWf;
                }
            }
            else
            {
                autoWf = _serviceFacade.CloneWorkflow(obj.InitWorkflow);
                autoWf.IsTemplate = false;
            }

            return autoWf;
        }


        public void OnBPObjectDeleted(object sender, BaseObjectEventArgs e)
        {
            var obj = (IBPObject)e.Object;

            if (obj.WorkflowContext != null)
            {
                obj.WorkflowContext.Hidden = true;
                foreach (var currentStage in obj.WorkflowContext.CurrentStages)
                {
                    currentStage.Hidden = true;
                }
            }
            CompliteWorkflowTransaction(e.UnitOfWork, new List<Task.Entities.Task>(), new List<Task.Entities.Task>());
        }

        public void OnBPObjectUpdate(object sender, BaseObjectEventArgs baseObjectEventArgs)
        {
            //TODO : че будет при изменении вф?
            // OnBPObjectCreate(sender, baseObjectEventArgs);
        }

        public IEnumerable<ChangeHistory> GetAllChangeHistory(IUnitOfWork unitOfWork, string objectType, int objectID)
        {
            return GetChangeHistories(unitOfWork, objectType, objectID);
        }

        public ChangeHistory GetLastChangeHistory(IUnitOfWork unitOfWork, string objectType, int objectID)
        {
            var historyItem = GetChangeHistories(unitOfWork, objectType, objectID).FirstOrDefault();
            if (historyItem != null && historyItem.Date > AppContext.DateTime.Now.AddSeconds(1)) // ээ ну что за х..ня
                throw ExceptionHelper.CriticalError(
                    "История объекта найдена, но дата ее создания больше текущей даты," +
                    " скорее всего на сервере установлено неверное время." +
                    " Бизнес процесс приостановлен. Обратитесь к администратору!");

            return historyItem;
        }

        public IQueryable<ChangeHistory> GetChangeHistories(IUnitOfWork unitOfWork, string objectType, int objectID, int? count = null)
        {
            IQueryable<ChangeHistory> q = unitOfWork.GetRepository<ChangeHistory>().All()
                .Where(x => x.ObjectType.ToUpper() == objectType.ToUpper() && x.ObjectID == objectID)
                .OrderByDescending(x => x.SortOrder);

            return count.HasValue ? q.Take(count.Value) : q;
        }

        private List<User> GetPermittedUsers(IUnitOfWork unitOfWork, Stage stage, BaseObject baseObject)
        {
            var u = _stageUserService.GetStakeholders(unitOfWork, stage, baseObject);
            return u;
        }



        public void ReleasePerform(IUnitOfWork unitOfWork, IWFObjectService objectService, int stageID, int objectID)
        {
            var tasksToUpdate = new List<Task.Entities.Task>();
            var tasksToCreate = new List<Task.Entities.Task>();

            var obj = objectService.Get(unitOfWork, objectID);

            var perform = ((IBPObject)obj).WorkflowContext.CurrentStages.FirstOrDefault(x => x.StageId == stageID);

            var performerType = _workflowContextService.GetPerformerType(unitOfWork, perform, obj);
            if (performerType == PerformerType.Admin ||
                performerType == PerformerType.Performer ||
                performerType == PerformerType.Curator)
            {
                if (perform != null)
                {
                    var stage = perform.Stage;

                    var allTask = GetTasksForAllStageUsers(unitOfWork, stage, obj, AppContext.DateTime.Now).ToList();

                    if (perform.Tasks != null)
                    {
                        foreach (var task in perform.Tasks)
                        {
                            task.Status = task.ForcedTask ? TaskStatus.NotRelevant : TaskStatus.New;
                            tasksToUpdate.Add(task);
                            _taskService.AddItemToChangeHistory(task);
                            allTask.RemoveAll(x => x.AssignedToID == task.AssignedToID);
                        }

                        foreach (var task in allTask)
                        {
                            perform.Tasks.Add(task);
                            tasksToCreate.Add(task);
                        }
                    }
                }
                if (perform != null)
                {
                    perform.PerformUser = null;
                    perform.PerformUserID = null;
                }
                CompliteWorkflowTransaction(unitOfWork, tasksToCreate, tasksToUpdate);
                objectService.Update(unitOfWork, obj);
            }
            else
            {
                throw ExceptionHelper.ActionInvokeException("Объект уже на исполнеии у другого пользователя");
            }
        }

        private int GetLastSortOrder(IUnitOfWork unitOfWork, int objectID, string objectType)
        {
            try
            {
                return unitOfWork.GetRepository<ChangeHistory>().All()
                    .Where(x => x.ObjectID == objectID && x.ObjectType.ToUpper() == objectType.ToUpper())
                    .Max(x => x.SortOrder);
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }

        public void InvokeStage(IUnitOfWork uofw, IWFObjectService baseObjectService, int objectID, int actionID, string comment, int? userID)
        {
            var obj = baseObjectService.Get(uofw, objectID);
            StageAction action =
                ((IBPObject)obj).WorkflowContext.CurrentStages.SelectMany(x => x.Stage.Outputs)
                    .FirstOrDefault(x => x.ID == actionID && !x.Hidden);
            InvokeStage(uofw, baseObjectService, obj, action, comment, userID);

        }

        private void InvokeStage(IUnitOfWork uofw, IWFObjectService baseObjectService, BaseObject obj, StageAction action, string comment, int? userID)
        {
            using (var unitOfWork = _uowFactory.CreateSystem(uofw))
            {
                #region Requires

                _objectService = baseObjectService;
                var objectType = obj.GetType().GetBaseObjectType();
                var oldObject = _serviceFacade.CloneObject(obj, objectType);

                if (action == null)
                    throw ExceptionHelper.ActionInvokeException("Could not find action in stage");

                StagePerform currentPerformer =
                    ((IBPObject)obj).WorkflowContext.CurrentStages.FirstOrDefault(
                        x => x.StageId == action.Step.ID);

                // ReSharper disable once PossibleNullReferenceException
                if (!currentPerformer.Stage.IsEntryPoint)
                {
                    var performerType = _workflowContextService.GetPerformerType(unitOfWork, currentPerformer, obj);
                    if (performerType == PerformerType.Denied)
                        throw ExceptionHelper.ActionInvokeException("Доступ запрещен");
                }

                #endregion

                var tasksToUpdate = new List<Task.Entities.Task>();
                var taskToCreate = new List<Task.Entities.Task>();

                int sortOrder = GetLastSortOrder(unitOfWork, obj.ID, objectType.GetBaseObjectType().FullName);

                WriteStageToHistory(unitOfWork, obj, currentPerformer, action, comment, ref sortOrder);

                if (currentPerformer != null && currentPerformer.Tasks != null)
                    UpdateTasks(comment, currentPerformer, tasksToUpdate);

                var nextPerformers = MoveToNextStage(unitOfWork, obj, action, userID, ref sortOrder, taskToCreate);

                foreach (var nextPerformer in nextPerformers)
                {
                    SetStagePosition(unitOfWork, nextPerformer);

                    foreach (var task in nextPerformer.Tasks.Where(x => x.AssignedToID != AppContext.SecurityUser.ID))
                    {
                        task.Status = TaskStatus.NotRelevant;
                        _taskService.AddItemToChangeHistory(task);
                        tasksToUpdate.Add(task);
                    }
                    WriteToPermitedUsers(unitOfWork, userID, obj, nextPerformer);
                }

                var performer = nextPerformers.FirstOrDefault();
                if (performer != null && (((IBPObject)obj).WorkflowContext.CurrentStages.Count == 1 && nextPerformers.Count == 1 && performer.Stage.StepType == FlowStepType.EndStep))
                {
                    WriteStageToHistory(unitOfWork, obj, nextPerformers.FirstOrDefault(), null,
                        "Завершение бизнес процесса", ref sortOrder);
                    nextPerformers.Clear();
                }

                var wfcontext = ((IBPObject)obj).With(x => x.WorkflowContext);
                UpdateContext(wfcontext, currentPerformer, nextPerformers);
                CompleteActionExecuting(unitOfWork, baseObjectService, obj, oldObject, action, comment);
                baseObjectService.Update(unitOfWork, obj);
                CompliteWorkflowTransaction(unitOfWork, taskToCreate, tasksToUpdate);
            }
        }

        private void SetStagePosition(IUnitOfWork unitOfWork, StagePerform next)
        {
            next.Position = new WorkflowHierarchyPosition();
            FindPosition(unitOfWork, next.Stage, next.Position);
        }

        private void FindPosition(IUnitOfWork unitOfWork, Step next, WorkflowHierarchyPosition position)
        {
            position.Current = next.Workflow;
            var owner = unitOfWork.GetRepository<WorkflowOwnerStep>().All().FirstOrDefault(x => x.ChildWorkflowId == next.WorkflowID);
            if (owner != null)
            {
                position.CurrentWorkflowContainer = owner;
                position.Parent = new WorkflowHierarchyPosition();
                FindPosition(unitOfWork, owner, position.Parent);
            }
        }

        private void UpdateContext(WorkflowContext context, StagePerform oldPerform, ICollection<StagePerform> newPerforms)
        {
            if (context.CurrentStages == null)
                context.CurrentStages = new List<StagePerform>();

            if (oldPerform != null)
            {
                oldPerform.Hidden = true;
                context.CurrentStages.Remove(oldPerform);
            }

            foreach (var newPerform in newPerforms)
            {

                context.CurrentStages.Add(newPerform);
            }
        }

        private void UpdateTasks(string comment, StagePerform currentPerformer, List<Task.Entities.Task> tasksToUpdate)
        {
            foreach (var task in currentPerformer.Tasks.Where(x => x.AssignedToID != AppContext.SecurityUser.ID))
            {
                task.Status = TaskStatus.NotRelevant;

                _taskService.AddItemToChangeHistory(task);

                tasksToUpdate.Add(task);
            }

            var performerTask = currentPerformer.Tasks.FirstOrDefault(x => x.AssignedToID == AppContext.SecurityUser.ID);

            if (performerTask != null)
            {
                performerTask.Status = TaskStatus.Complete;
                performerTask.CompliteDate = AppContext.DateTime.Now;
                performerTask.PercentComplete = 1;

                _taskService.AddItemToChangeHistory(performerTask, comment);

                tasksToUpdate.Add(performerTask);
            }
        }

        private void CompleteActionExecuting(IUnitOfWork unitOfWork, IWFObjectService baseObjectService, BaseObject baseObject, BaseObject oldObject, StageAction action, string comment)
        {
            if (baseObjectService != null)
            {
                var bpObject = baseObject as IBPObject;
                if (bpObject != null)
                {
                    var workflow = bpObject.WorkflowContext.Workflow;
                    if (workflow != null)
                        baseObjectService.OnActionExecuting(new ActionExecuteArgs
                        {
                            UnitOfWork = unitOfWork,
                            OldObject = oldObject,
                            NewObject = baseObject,
                            CurrentStage = action.Step as Stage,
                            Workflow = workflow,
                            EvaluatedAction = action,
                            GetStageJumper = (output, obj) => FindNextStage(unitOfWork, output, obj),
                            Comment = comment
                        });
                }
            }
        }

        private void WriteToPermitedUsers(IUnitOfWork unitOfWork, int? userID, BaseObject baseObject, StagePerform perform)
        {
            var permittedUsers = GetPermittedUsers(unitOfWork, perform.Stage, baseObject);

            var tasksToCreate = new List<Task.Entities.Task>();
            var tasksToUpdate = new List<Task.Entities.Task>();

            if (userID != null)
            {
                if (permittedUsers.Any(x => x.ID == userID.Value))
                    TakeForPerformImpl(unitOfWork, userID.Value, perform, tasksToUpdate, tasksToCreate, baseObject);
                else
                    throw ExceptionHelper.ActionInvokeException(
                        "Это пользователь не может выполнить данный этап");
            }
            else if (permittedUsers.Count == 1)
            {
                var singlePerformer = permittedUsers.FirstOrDefault();

                if (singlePerformer != null)
                    TakeForPerformImpl(unitOfWork, singlePerformer.ID, perform, tasksToUpdate, tasksToCreate, baseObject);
            }
        }

        public void AutoInvokeStage(IUnitOfWork unitOfWork, int objectID, StagePerform perform, string comment = null)
        {
            _objectService = _serviceFacade.GetService(perform.Stage.ObjectType);

            var obj = _objectService.Get(unitOfWork, objectID);
            if (obj == null)
            {
                perform.Hidden = true;
                unitOfWork.SaveChanges();
                return;
            }
            var action = perform.Stage.Outputs.FirstOrDefault(x => x.IsDefaultAction && !x.Hidden);
            if (action != null)
                InvokeStage(unitOfWork, _objectService, objectID, action.ID, comment, null);
        }

        public IEnumerable<Stage> GetNextStage(IUnitOfWork unitOfWork, BaseObject baseObject, int actionID)
        {
            var action = unitOfWork.GetRepository<StageAction>().Find(x => x.ID == actionID);
            if (action == null) throw ExceptionHelper.ActionNotFoundException("Действие не найдено");

            ExecuteAction(baseObject, action);

            return FindNextStage(unitOfWork, action, baseObject).With(x => x.Stages);
        }

        public IEnumerable<ChangeHistory> GetAllChangeHistory(IUnitOfWork unitOfWork, Workflow wfl)
        {
            return
                unitOfWork.GetRepository<ChangeHistory>()
                    .All()
                    .Where(x => x.ObjectType == wfl.ObjectType && x.Step.WorkflowID == wfl.ID);
        }

        public event EventHandler<WorkflowTaskEventArgs> WorkflowTransactionCompleted;

        public bool TestMacros(IUnitOfWork unitOfWork, IEnumerable<InitItem> items, Type type, Type parentType, out Exception exception)
        {
            exception = default(Exception);

            if (type != null && parentType != null)
            {
                var obj = Activator.CreateInstance(type) as BaseObject;
                var parentObj = Activator.CreateInstance(type) as BaseObject;

                try
                {
                    _serviceFacade.InitializeObject(AppContext.SecurityUser, parentObj, obj, items);
                    return true;
                }
                catch (Exception e)
                {
                    exception = e;

                    return false;
                }
            }

            return false;
        }



        public IQueryable<Workflow> GetWorkflowList(IUnitOfWork unitOfWork, Type type, BaseObject model)
        {
            return _serviceFacade.GetWorkflowList(AppContext.SecurityUser, type, model, GetAll(unitOfWork));
        }

        public ChangeHistory GetChangeHistoryByID(IUnitOfWork unitOfWork, int changeHistoryID)
        {
            return unitOfWork.GetRepository<ChangeHistory>().All().FirstOrDefault(x => x.ID == changeHistoryID);
        }

        private void StartWorkflow(ISystemUnitOfWork unitOfWork, IBaseObjectCRUDService baseObjectService, BaseObject baseObject, WorkflowContext wfctx)
        {
            var action = GetEntryPoint(wfctx.Workflow);
            if (action != null)
            {
                var currentStage = action.Step as Stage;
                if (currentStage == null)
                    throw new Exception("Для действия не задан этап");
                const string comment = "Запуск бизнес процесса";

                var context = (baseObject as IBPObject).With(x => x.WorkflowContext);
                StagePerform perform = new StagePerform
                {
                    Stage = (Stage)action.Step,
                    BeginDate = AppContext.DateTime.Now,
                    ObjectID = baseObject.ID,
                    ObjectType = baseObject.GetType().GetBaseObjectType().FullName,
                };

                UpdateContext(context, null, new List<StagePerform>() { perform });

                CompleteActionExecuting(unitOfWork, baseObjectService as IWFObjectService, baseObject, null, action, comment);
                baseObjectService.Update(unitOfWork, baseObject);
                CompliteWorkflowTransaction(unitOfWork, new List<Task.Entities.Task>(), new List<Task.Entities.Task>());

                InvokeStage(unitOfWork, baseObjectService as IWFObjectService, baseObject, action, comment, null);
            }
            else
            {
                throw new Exception("Не найдена точка входа");
            }
        }

        private void CompliteWorkflowTransaction(IUnitOfWork unitOfWork, List<Task.Entities.Task> tasksToCreate, List<Task.Entities.Task> tasksToUpdate)
        {
            using (var uow = _uowFactory.CreateSystem(unitOfWork))
            {
                OnCompliteWorkflowTransaction(new WorkflowTaskEventArgs(uow, tasksToCreate, tasksToUpdate));
            }
        }

        public void TakeForPerform(IUnitOfWork unitOfWork, IWFObjectService objectService, int? userID, int stageID, int objectID)
        {
            //TODO : Проверить не взял ли кто на исполнение
            if (userID == null)
                userID = AppContext.SecurityUser.ID;

            var obj = objectService.Get(unitOfWork, objectID);

            var perform = ((IBPObject)obj).WorkflowContext.CurrentStages.FirstOrDefault(x => x.StageId == stageID);

            if (_workflowContextService.GetPerformerType(unitOfWork, perform, obj) == PerformerType.Denied)
                throw ExceptionHelper.ActionInvokeException("Доступ запрещен");

            if (perform != null && perform.PerformUser != null)
                throw ExceptionHelper.ActionInvokeException("Объект уже на исполнеии у другого пользователя");

            var tasksToCreate = new List<Task.Entities.Task>();
            var taskToUpdate = new List<Task.Entities.Task>();

            TakeForPerformImpl(unitOfWork, userID.Value, perform, taskToUpdate, tasksToCreate, obj);
            CompliteWorkflowTransaction(unitOfWork, tasksToCreate, taskToUpdate);
            objectService.Update(unitOfWork, obj);
        }

        private void TakeForPerformImpl(IUnitOfWork unitOfWork, int userID, StagePerform perform, List<Task.Entities.Task> _tasksToUpdate, List<Task.Entities.Task> _tasksToCreate, BaseObject obj)
        {
            perform.PerformUserID = userID;
            perform.FromUserID = AppContext.SecurityUser.ID;

            if (perform.Tasks == null)
                perform.Tasks = new List<BPTask>();

            foreach (var task in perform.Tasks.Where(x => x.AssignedToID != userID))
            {
                task.Status = TaskStatus.NotRelevant;

                _taskService.AddItemToChangeHistory(task);

                _tasksToUpdate.Add(task);
            }

            var performerTask = perform.Tasks.FirstOrDefault(x => x.AssignedToID == userID);

            if (performerTask != null)
            {
                performerTask.Status = TaskStatus.InProcess;

                _taskService.AddItemToChangeHistory(performerTask); // этот метод вызывается когда выполняется ст или просто из вне

                _tasksToUpdate.Add(performerTask);
            }
            else if (perform.Stage.CreateTask)
            {
                performerTask = GetTask(AppContext.SecurityUser.ID, userID, perform.Stage, obj, AppContext.DateTime.Now);

                performerTask.ForcedTask = true;

                performerTask.Status = AppContext.SecurityUser.ID == userID ? TaskStatus.InProcess : TaskStatus.New;

                perform.Tasks.Add(performerTask);

                _tasksToCreate.Add(performerTask);
            }
        }

        private IEnumerable<RelatedTask> InitRelatedTaskCollection(IUnitOfWork unitOfWork, ChangeHistory changeHistoryItem, BaseObject baseObject, DateTime dt)
        {
            TaskStep ts = changeHistoryItem.Step as TaskStep;
            if (ts == null)
                throw new Exception("Не удалось инициализировать коллекцию задач для шага");
            return _stageUserService.GetStakeholders(unitOfWork, ts, baseObject).Select(x => new RelatedTask
            {
                TaskStep = ts,
                Title = _serviceFacade.Render(ts.TitleTemplate, baseObject).TruncateAtWord(150),
                CategoryID = ts.CategoryID.GetValueOrDefault(),
                AssignedFromID = AppContext.SecurityUser.ID,
                AssignedToID = x.ID,
                Period = new Period
                {
                    Start = dt,
                    End = dt + TimeSpan.FromMinutes(ts.PerformancePeriod)
                },
                Description = _serviceFacade.Render(ts.DescriptionTemplate, baseObject)
            });

        }

        private ICollection<StagePerform> MoveToNextStage(IUnitOfWork unitOfWork, BaseObject baseObject, StageAction action, int? assignToUserID, ref int sortOrder, List<Task.Entities.Task> _tasksToCreate)
        {
            ExecuteAction(baseObject, action);

            var stageJumper = FindNextStage(unitOfWork, action, baseObject);

            WriteStepsBetweenStages(unitOfWork, baseObject, stageJumper, ref sortOrder, _tasksToCreate);

            var nextStages = stageJumper.With(x => x.Stages);

            List<StagePerform> performs = new List<StagePerform>();

            foreach (var nextStage in nextStages)
            {
                StagePerform perform = new StagePerform
                {
                    Stage = nextStage,
                    BeginDate = AppContext.DateTime.Now,
                    ObjectID = baseObject.ID,
                    ObjectType = baseObject.GetType().GetBaseObjectType().FullName,
                };

                if (assignToUserID == null)
                {
                    perform.Tasks = GetTasksForAllStageUsers(unitOfWork, nextStage, baseObject, AppContext.DateTime.Now).ToList();
                }
                else
                {
                    if (perform.Tasks == null)
                        perform.Tasks = new List<BPTask>();

                    perform.Tasks.Add(GetTask(AppContext.SecurityUser.ID, assignToUserID.Value, nextStage, baseObject, AppContext.DateTime.Now));
                }
                _tasksToCreate.AddRange(perform.Tasks);
                performs.Add(perform);
            }
            return performs;
        }

        public void ExecuteNextStage(IUnitOfWork unitOfWork, BaseObject baseObject, StageAction action, int? assignToUserID, ref int counter)
        {
            var context = ((IBPObject)baseObject).WorkflowContext;
            if (context == null)
                throw new Exception("Не удалось найти контекст выполенения");

            StagePerform currentPerformer = context.CurrentStages.FirstOrDefault(x => action != null && x.StageId == action.Step.ID);

            ExecuteAction(baseObject, action);

            WriteStageToHistory(unitOfWork, baseObject, currentPerformer, action, null, ref counter);

            var nextPerformers = MoveToNextStage(unitOfWork, baseObject, action, assignToUserID, ref counter, new List<Task.Entities.Task>());

            if (context.CurrentStages.Count == 1 && nextPerformers.Count == 1)
            {
                StagePerform stagePerform = nextPerformers.FirstOrDefault();
                if (stagePerform != null)
                {
                    var endStep = stagePerform.Stage;
                    if (endStep.StepType == FlowStepType.EndStep)
                    {
                        var endPerform = stagePerform;
                        WriteStageToHistory(unitOfWork, baseObject, endPerform, null, "Завершение бизнес процесса", ref counter);
                        nextPerformers.Clear();
                    }
                }
            }

            UpdateContext(context, currentPerformer, nextPerformers);

            var objService = _serviceFacade.GetService(baseObject.GetType().GetBaseObjectType().FullName);
            objService.Update(unitOfWork, baseObject);
        }

        private void WriteStepsBetweenStages(IUnitOfWork unitOfWork, BaseObject baseObject, StageJumper stageJumper, ref int sortOrder, List<Task.Entities.Task> _tasksToCreate)
        {
            var dt = AppContext.DateTime.Now;
            var histRep = unitOfWork.GetRepository<ChangeHistory>();
            var objType = baseObject.GetType().GetBaseObjectType().FullName;
            foreach (var step in stageJumper.Steps)
            {
                ChangeHistory ch = new ChangeHistory
                {
                    UserID = AppContext.SecurityUser.ID,
                    Date = AppContext.DateTime.Now,
                    Step = step,
                    ObjectID = baseObject.ID,
                    ObjectType = objType,
                    StepID = step.ID,
                    SortOrder = ++sortOrder,
                    IsAgreed = true,
                };
                if (step is CreateObjectStep)
                {
                    CreateObjectStep cos = step as CreateObjectStep;
                    ch.CreatedObject = StartWorkflows(unitOfWork, baseObject, cos);
                }
                if (step is TaskStep)
                {
                    ch.RelatedTasks = InitRelatedTaskCollection(unitOfWork, ch, baseObject, AppContext.DateTime.Now.AddSeconds(1)).ToList();
                    _tasksToCreate.AddRange(ch.RelatedTasks);
                }
                if (step.IsEntryPoint)
                {
                    var stage = step as Stage;
                    if (stage != null)
                    {
                        StageAction action = stage.Outputs.FirstOrDefault();

                        if (action != null)
                        {
                            ch.AgreementItems = new List<AgreementItem>
                            {
                                new AgreementItem
                                {
                                    ActionID = action.ID,
                                    UserID = AppContext.SecurityUser.ID,
                                    Comment = "Запуск дочернего бизнес процесса",
                                    Date = dt,
                                }
                            };
                            ExecuteAction(baseObject, action);
                        }
                    }
                }

                histRep.Create(ch);
                dt += new TimeSpan(0, 0, 1);
            }
        }

        private void WriteStageToHistory(IUnitOfWork unitOfWork, BaseObject obj, StagePerform perform, StageAction action, string comment, ref int sortOrder)
        {
            string objectType = obj.GetType().GetBaseObjectType().FullName;

            ChangeHistory ch = new ChangeHistory
            {
                IsAgreed = true,
                ObjectID = obj.ID,
                ObjectType = objectType,
                UserID = AppContext.SecurityUser.ID,
                Step = perform.Stage,
                SortOrder = ++sortOrder,
                Date = perform.BeginDate,
                AgreementItems = new List<AgreementItem>(),
            };
            if (action != null)
            {
                var agrItem = new AgreementItem
                {
                    ActionID = action.ID,
                    Comment = comment,
                    Date = AppContext.DateTime.Now,
                    UserID = AppContext.SecurityUser.ID,
                    FromUserID = AppContext.SecurityUser.ID,
                };
                ch.AgreementItems.Add(agrItem);
                ExecuteAction(obj, action);
            }
            if (perform.Tasks != null && perform.Tasks.Count > 0)
            {
                ch.Tasks = perform.Tasks;
            }

            var rep = unitOfWork.GetRepository<ChangeHistory>();
            rep.Create(ch);

        }

        // TODO: admin or user?
        private CreatedObject StartWorkflows(IUnitOfWork unitOfWork, BaseObject src, CreateObjectStep createObjectStep)
        {
            var service = _serviceFacade.GetService(createObjectStep.ObjectType, unitOfWork);
            var dest = service.CreateOnGroundsOf(unitOfWork, null);
            _serviceFacade.InitializeObject(AppContext.SecurityUser, src, dest, createObjectStep.InitItems);

            service.Create(unitOfWork, dest);
            return new CreatedObject { ObjectID = dest.ID, Type = createObjectStep.ObjectType, ObjectStepID = createObjectStep.ID };
        }



        protected virtual void OnCompliteWorkflowTransaction(WorkflowTaskEventArgs e)
        {
            var handler = WorkflowTransactionCompleted;
            if (handler != null) handler(this, e);
        }

        private IEnumerable<BPTask> GetTasksForAllStageUsers(IUnitOfWork unitOfWork, Stage stage, BaseObject baseObject, DateTime dt)
        {
            return _stageUserService.GetStakeholders(unitOfWork, stage, baseObject).Select(user => GetTask(AppContext.SecurityUser.ID, user.ID, stage, baseObject, dt));
        }

        private BPTask GetTask(int userFromID, int userToID, Stage stage, BaseObject baseObject, DateTime dt)
        {
            var task = new BPTask
            {
                Title = _serviceFacade.Render(stage.TitleTemplate, baseObject).TruncateAtWord(150),
                CategoryID = stage.CategoryID.GetValueOrDefault(),
                AssignedFromID = userFromID,
                AssignedToID = userToID,
                Period = new Period
                {
                    Start = dt,
                    End = dt + TimeSpan.FromMinutes(stage.PerformancePeriod)
                },
                Description = _serviceFacade.Render(stage.DescriptionTemplate, baseObject),
            };

            _taskService.AddItemToChangeHistory(task);

            return task;
        }

        private void ExecuteAction(BaseObject baseObject, StageAction action)
        {
            if (action.InitItems != null && action.InitItems.Any())
                _serviceFacade.ModifyObject(AppContext.SecurityUser, baseObject, action.InitItems);
        }

        private StageJumper FindNextStage(IUnitOfWork unitOfWork, Output output, BaseObject obj)
        {
            var objectType = obj.GetType().GetBaseObjectType().FullName;

            var changeHistories = unitOfWork.GetRepository<ChangeHistory>().All().Where(x => x.ObjectID == obj.ID && x.ObjectType == objectType).ToList();

            return FindNextStageImpl(changeHistories, output, obj, new StageJumper(), null);
        }

        private StageJumper FindNextStageImpl(List<ChangeHistory> changeHistories, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            Step additionalStep;

            var stepType = output.NextStep.StepType;

            switch (stepType)
            {
                case FlowStepType.Stage:
                    return ProcessStage(output, stageJumper);

                case FlowStepType.EndStep:
                    return ProcessEndStep(changeHistories, output, obj, stageJumper, visitedBranches);

                case FlowStepType.BranchingStep:
                    return ProcessBranchingStep(changeHistories, output, obj, stageJumper, visitedBranches);

                case FlowStepType.TaskStep:
                    additionalStep = ProcessTaskStep(output, stageJumper);
                    break;

                case FlowStepType.CreateObjectTask:
                    additionalStep = ProcessCreateObjectStep(output, stageJumper);
                    break;

                case FlowStepType.RestoreStep:
                    return ProcessRestoreStep(changeHistories, output, stageJumper);

                case FlowStepType.WorkflowOwnerStep:
                    return ProcessWorkflowOwnerStep(changeHistories, output, obj, stageJumper, visitedBranches);

                case FlowStepType.GotoStep:
                    return ProcessGotoStep(changeHistories, output, obj, stageJumper, visitedBranches);

                case FlowStepType.ParalleizationStep:
                    return ProcessParallelStep(changeHistories, output, obj, stageJumper, visitedBranches);

                case FlowStepType.ParallelEndStep:
                    return ProcessParallelEndStep(changeHistories, output, obj, stageJumper, visitedBranches);


                default: throw new Exception("Не удалось определить тип шага");

            }
            if (additionalStep != null)
                return FindNextStageImpl(changeHistories, additionalStep.BaseOutputs.FirstOrDefault(), obj, stageJumper, visitedBranches);

            return null; // Null when end of workflow reached
        }

        #region steps

        private StageJumper ProcessParallelEndStep(List<ChangeHistory> changeHistories, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            var endParallelStep = (ParallelEndStep)output.NextStep;
            if (endParallelStep.WaitAllThreads)
            {
                if (((IBPObject)obj).WorkflowContext.CurrentStages.Count() == 1)// Тип если в текщих этапах тока один то отдаем след                    
                {
                    stageJumper.Steps.Add(endParallelStep);
                    return FindNextStageImpl(changeHistories, endParallelStep.Outputs.FirstOrDefault(), obj, stageJumper, visitedBranches);
                }
                else
                {
                    stageJumper.Stages.Clear();
                    return stageJumper;
                }
            }
            else
            {
                stageJumper.Steps.Add(endParallelStep);
                return FindNextStageImpl(changeHistories, endParallelStep.Outputs.FirstOrDefault(), obj, stageJumper, visitedBranches);
            }
        }

        private StageJumper ProcessParallelStep(List<ChangeHistory> changeHistories, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            ParallelizationStep step = output.NextStep as ParallelizationStep;
            stageJumper.Steps.Add(step);
            // ReSharper disable once PossibleNullReferenceException
            foreach (var so in step.Outputs)
            {
                FindNextStageImpl(changeHistories, so, obj, stageJumper, visitedBranches);
            }
            return stageJumper;
        }

        private StageJumper ProcessGotoStep(List<ChangeHistory> changeHistory, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            var gotoStep = output.NextStep as GotoStep;
            stageJumper.Steps.Add(gotoStep);
            IBPObject bpObject = (IBPObject)obj;

            var step = FindStepByName(bpObject.WorkflowContext.Workflow, gotoStep.ReturnToStep);
            if (step != null)
            {
                if (step is Stage)
                {
                    stageJumper.Stages.Add(step as Stage);
                    return stageJumper;
                }
                if (step is WorkflowOwnerStep)
                {
                    var wfContainer = (WorkflowOwnerStep)step;
                    var stage = GetEntryPoint(wfContainer.ChildWorkflow);
                    return FindNextStageImpl(changeHistory, stage, obj, stageJumper, visitedBranches);
                }
                return FindNextStageImpl(changeHistory, step.BaseOutputs.FirstOrDefault(), obj, stageJumper, visitedBranches);
            }
            throw new Exception("Не удалось найти шаг с именем " + gotoStep.ReturnToStep);
        }

        private Step FindStepByName(Workflow wf, string stepName)
        {
            var step = wf.Steps.FirstOrDefault(x => x.StepName == stepName);
            if (step != null)
                return step;

            var wfs = wf.Steps.OfType<WorkflowOwnerStep>();
            foreach (var childWF in wfs)
            {
                step = FindStepByName(childWF.ChildWorkflow, stepName);
                if (step != null)
                    return step;
            }
            return null;
        }

        private StageJumper ProcessWorkflowOwnerStep(List<ChangeHistory> changeHistories, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            var owner = (WorkflowOwnerStep)output.NextStep;
            stageJumper.Steps.Add(owner);
            var entryStep = owner.ChildWorkflow.Steps.FirstOrDefault(x => x.IsEntryPoint);
            if (entryStep != null)
            {
                stageJumper.Steps.Add(entryStep);
                return FindNextStageImpl(changeHistories, entryStep.BaseOutputs.FirstOrDefault(), obj, stageJumper,
                    visitedBranches);
            }
            throw new Exception("В контейнере бизнес процесса не указан дочерний бизнес процесс");
        }

        private StageJumper ProcessRestoreStep(List<ChangeHistory> changeHistories, Output output, StageJumper stageJumper)
        {
            var rs = output.NextStep as RestoreStep;

            stageJumper.Steps.Add(rs);

            var histories = changeHistories
                .Where(x => x.Step.StepType == FlowStepType.Stage).OrderByDescending(x => x.SortOrder);

            // ReSharper disable once PossibleNullReferenceException
            var prevHistory = histories.Skip(rs.BackStepCount).FirstOrDefault();

            if (prevHistory != null)
            {
                if (prevHistory.Step != null)
                {
                    if (prevHistory.Step.IsEntryPoint)
                        throw new Exception("Объект не может быть передвинут на точку входа");
                    stageJumper.Stages.Add(prevHistory.Step as Stage);
                    return stageJumper;
                }
            }
            throw new Exception("Предыдущий шаг не найден");
        }

        private Step ProcessCreateObjectStep(Output output, StageJumper stageJumper)
        {
            var createObjectStep = output.NextStep as CreateObjectStep;
            stageJumper.Steps.Add(createObjectStep);
            return createObjectStep;
        }

        private Step ProcessTaskStep(Output output, StageJumper stageJumper)
        {
            var taskStep = output.NextStep as TaskStep;
            stageJumper.Steps.Add(taskStep);
            return taskStep;
        }

        private StageJumper ProcessBranchingStep(List<ChangeHistory> changeHistories, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            var step = output.NextStep as BranchingStep;
            var branchingStep = step;
            stageJumper.Steps.Add(branchingStep);
            // ReSharper disable once PossibleNullReferenceException
            var branch = branchingStep.Outputs.FirstOrDefault(x => ExecutePredicate(x, obj));

            if (branch == null)
                throw ExceptionHelper.ActionInvokeException(
                    "Can't resolve path, branch's predicate returns \"false\" and default branch not founded");

            if (visitedBranches == null)
                visitedBranches = new List<Branch>();

            if (!visitedBranches.Contains(branch))
            {
                visitedBranches.Add(branch);

                return FindNextStageImpl(changeHistories, branch, obj, stageJumper, visitedBranches);
            }

            throw ExceptionHelper.ActionInvokeException("Can't resolve path, infinite loop");
        }

        private StageJumper ProcessStage(Output output, StageJumper stageJumper)
        {
            return stageJumper.IfNotNull(x => x.Stages.Add(output.NextStep as Stage));
        }

        private StageJumper ProcessEndStep(List<ChangeHistory> changeHistories, Output output, BaseObject obj, StageJumper stageJumper, ICollection<Branch> visitedBranches)
        {
            var endStep = output.NextStep as EndStep;
            //TODO : не забыть переделать
            var change = changeHistories.OrderByDescending(x => x.SortOrder)
                .FirstOrDefault(
                // ReSharper disable once PossibleNullReferenceException
                    x => ((x.Step is WorkflowOwnerStep) && ((WorkflowOwnerStep)x.Step).ChildWorkflowId == endStep.WorkflowID));

            if (change != null)
            {
                stageJumper.Steps.Add(endStep);
                return FindNextStageImpl(changeHistories, change.Step.BaseOutputs.FirstOrDefault(), obj, stageJumper,
                    visitedBranches);
            }
            stageJumper.Stages.Add(endStep);
            return stageJumper;
        }

        #endregion steps

        private bool ExecutePredicate(Branch branch, BaseObject obj)
        {
            return GetPropertyEnumPairs(branch.BranchConditions, obj).Any(pair => pair.EqualsFor(obj));
        }

        private IEnumerable<PropertyEnumPair> GetPropertyEnumPairs(IEnumerable<BaseMacroItem> macoItems, BaseObject obj)
        {
            return macoItems.Join(obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance),
                a => a.Member, p => p.Name, (a, p) => new { Property = p, Action = a })
                .Select(x =>
                {
                    var propType = x.Property.PropertyType;

                    if (propType.IsGenericType &&
                        propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propType = propType.GetGenericArguments()[0];
                    }

                    return new
                    {
                        Property = x.Property,
                        PropertyType = propType,
                        Action = x.Action
                    };
                })
                .Where(x => Enum.IsDefined(x.PropertyType, x.Action.Value))
                .Select(x => new PropertyEnumPair(x.Property, Enum.Parse(x.PropertyType, x.Action.Value)));
        }

        public override Workflow CreateOnGroundsOf(IUnitOfWork uow, BaseObject obj)
        {
            var wf = base.CreateOnGroundsOf(uow, obj);
            if (wf != null)
                wf.IsTemplate = true;

            return wf;
        }

        protected override IObjectSaver<Workflow> GetForSave(IUnitOfWork uow, IObjectSaver<Workflow> objectSaver)
        {
            var temp = base.GetForSave(uow, objectSaver)
                .SaveOneObject(x => x.Curator)
                .SaveOneObject(x => x.CuratorsCategory)
                .SaveOneObject(x => x.Creator)
                .SaveOneToMany(x => x.Steps, true, stepSaver =>
                {
                    stepSaver.SaveOneToMany(x => x.BaseOutputs, true, saver => saver.SaveOneObject(x => x.NextStep));

                    if (stepSaver.Src is Stage)
                    {
                        stepSaver.AsObjectSaver<Stage>()
                            .SaveOneToMany(x => x.Outputs, true, saver => saver.SaveOneToMany(x => x.InitItems).SaveOneToMany(v => v.ValidatonRules).SaveOneToMany(x => x.Roles, x => x.SaveOneObject(c => c.Object)))
                            .SaveOneObject(stageSaver => stageSaver.TaskCategory)
                            .SaveOneToMany(stageSaver => stageSaver.AssignedToCategory, x => x.SaveOneObject(z => z.Object))
                            .SaveOneToMany(stageSaver => stageSaver.AssignedToUsers,
                                stageSaver => stageSaver.SaveOneObject(stageUserSaver => stageUserSaver.Performer))
                            .SaveOneObject(stageSaver => stageSaver.DetailViewSetting);

                        var extendedStage = stepSaver.Src as ExtendedStage;
                        if (extendedStage != null)
                        {
                            _serviceFacade.SaveExtendedStage(uow, extendedStage,
                                stepSaver.AsObjectSaver<ExtendedStage>());
                        }
                    }
                    else if (stepSaver.Src is BranchingStep)
                    {
                        stepSaver.AsObjectSaver<BranchingStep>()
                            .SaveOneToMany(x => x.Outputs, true, saver => saver.SaveOneToMany(x => x.BranchConditions));
                    }
                    else if (stepSaver.Src is TaskStep)
                    {
                        stepSaver.AsObjectSaver<TaskStep>()
                            .SaveOneObject(x => x.TaskCategory)
                            .SaveOneToMany(stageSaver => stageSaver.AssignedToCategory)
                            .SaveOneToMany(stageSaver => stageSaver.AssignedToUsers,
                                stageSaver => stageSaver.SaveOneObject(stageUserSaver => stageUserSaver.Performer));
                    }
                    else if (stepSaver.Src is CreateObjectStep)
                    {
                        stepSaver.AsObjectSaver<CreateObjectStep>()
                            .SaveOneToMany(stageSaver => stageSaver.InitItems);
                    }
                    else if (stepSaver.Src is WorkflowOwnerStep)
                    {
                        stepSaver.AsObjectSaver<WorkflowOwnerStep>()
                            .SaveOneObject(stageSaver => stageSaver.ChildWorkflow)
                            .SaveOneObject(stageSaver => stageSaver.DetailViewSetting);
                    }
                });

            foreach (var result in temp.Dest.Steps.Where(x => x.BaseOutputs != null).SelectMany(x => x.BaseOutputs)
                .Join(base.GetForSave(uow, objectSaver).Dest.Steps,
                    o => o.NextStepViewID, i => i.ViewID, (o, i) => new { Output = o, NextStep = i }))
            {
                result.Output.NextStep = result.NextStep;
            }

            if (
                temp.Dest.Steps.Where(x => !x.Hidden)
                    .Where(x => x.BaseOutputs != null)
                    .SelectMany(x => x.BaseOutputs)
                    .Where(x => !x.Hidden)
                    .Any(x => x.NextStep == null))
            {
                throw ExceptionHelper.WorkflowSaveException(
                    "Бизнес-процесс не может содержать выходы не асоциированные с каким либо шагом");
            }

            //TODO : Рарзработать алгоритм проверки бп чтобы все выходы вели к концу
            //if(temp.Dest.Steps.Where(x=> x.StepType == FlowStepType.Stage && x.BaseOutputs.Count == 0 && !x.Hidden).Any())
            //{
            //    throw ExceptionHelper.WorkflowSaveException("Этап не может быть без выходов");
            //}

            return temp;
        }

        private StageAction GetEntryPoint(Workflow wf)
        {
            StageAction entryAction = null;

            if (wf != null)
            {
                var entryPointStage = wf.Steps.OfType<Stage>().FirstOrDefault(x => x.IsEntryPoint);

                if (entryPointStage != null)
                    entryAction = entryPointStage.Outputs.FirstOrDefault();

                if (entryAction != null)
                    return entryAction;

                throw ExceptionHelper.ActionNotFoundException("Action not found");
            }

            return null;
        }




        internal class StageJumper
        {
            public StageJumper()
            {
                Steps = new List<Step>();
                Stages = new List<Stage>();
            }

            public ICollection<Stage> Stages { get; set; }
            public IList<Step> Steps { get; set; }
        }

        private class PropertyEnumPair
        {
            private readonly PropertyInfo _property;
            private readonly object _value;

            public PropertyEnumPair(PropertyInfo property, object value)
            {
                _property = property;
                _value = value;
            }

            public void SetValueFor(BaseObject obj)
            {
                _property.SetValue(obj, _value);
            }

            public bool EqualsFor(BaseObject obj)
            {
                var value = _property.GetValue(obj);
                return value != null && value.ToString() == _value.ToString();
            }
        }

        internal class PerofomanceLogger
        {
            public static void Log(string operationName, double totalSeconds)
            {
                using (var stream = new StreamWriter("D:\\log.txt", true))
                {
                    string message = string.Format("{0}: {1}s \n", operationName, totalSeconds);
                    stream.WriteLine(message);
                }
            }
        }

        public IValidationContext GetValidationContext(StageAction action)
        {
            StageActionValidationContext ctx = new StageActionValidationContext(action);
            return ctx;
        }
    }


}