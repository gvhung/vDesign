using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.BusinessProcesses.Services.Concrete;
using Base.DAL;
using Base.Service;
using Base.Task.Services;
using Base.UI;
using Framework;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Base.Validation;
using WebUI.Authorize;
using WebUI.Models.BusinessProcess;

namespace WebUI.Controllers
{
    public class BusinessProcessController : BaseController
    {
        private readonly IWorkflowService _workflowService;
        private readonly ITaskService _taskService;
        private readonly IWorkflowServiceResolver _serviceProvider;
        private readonly IWorkflowCacheService _cashService;
        private readonly IValidationService _validationService;
        private readonly IWorkflowContextService _contextService;

        public BusinessProcessController(
            IBaseControllerServiceFacade baseServiceFacade,
            IWorkflowService workflowService,
            ITaskService taskService,
            IWorkflowServiceResolver serviceProvider,
            IWorkflowCacheService cashService,
            IValidationService validationService,
            IWorkflowContextService contextService)
            : base(baseServiceFacade)
        {
            _workflowService = workflowService;
            _taskService = taskService;
            _serviceProvider = serviceProvider;
            _cashService = cashService;
            _validationService = validationService;
            _contextService = contextService;
        }

        public ActionResult GetPermittedUsers(int objectID, string objectType, int stageID)
        {
            using (var uow = CreateUnitOfWork())
            {
                var objService = this.GetService<IWFObjectService>(objectType);
                var obj = objService.Get(uow, objectID);
                var bpObject = obj as IBPObject;

                var stage = bpObject.WorkflowContext.CurrentStages.FirstOrDefault(x => x.StageId == stageID).Stage;
                return new JsonNetResult(new
                {
                    Dialog =
                        RenderPartialViewToString("_UserList", _contextService.GetPermittedUsers(uow, stage, obj))
                });
            }
        }

        public ActionResult WorkflowReport(int id)
        {
            using (var uow = CreateUnitOfWork())
            {
                var wfl = _workflowService.Get(uow, id);

                if (wfl != null)
                    return PartialView("_WorkflowReport", new WorkflowReportVm(wfl, _workflowService.GetAllChangeHistory(uow, wfl).Where(x => x.Step is Stage)));

                return Content("Бизнес процесс не найден");
            }
        }

        public ActionResult CheckNextStage(int actionID, string objectType, int objectID)
        {
            using (var uow = CreateUnitOfWork())
            {
                var intError = 0;
                var strMsg = "";

                try
                {
                    var objService = this.GetService<IWFObjectService>(objectType);
                    var obj = objService.Get(uow, objectID);
                    if (obj == null) throw new Exception("Объект не найден");
                    var action = uow.GetRepository<StageAction>().All().FirstOrDefault(x => x.ID == actionID);
                    var validationContext = _workflowService.GetValidationContext(action);
                    var results = _validationService.Validate(validationContext, obj);

                    if (results.Count == 0)
                    {

                        var nextStage = _workflowService.GetNextStage(uow, obj, actionID);

                        if (nextStage != null && nextStage.Count() == 1 && nextStage.FirstOrDefault().IsCustomPerformer)
                        {
                            return new JsonNetResult(new
                            {
                                Dialog =
                                    RenderPartialViewToString("_PerformersSelect",
                                        _contextService.GetPermittedUsers(uow, nextStage.FirstOrDefault(), obj))
                            });
                        }
                        return new JsonNetResult(new { Success = "Ok" });
                    }
                    return new JsonNetResult(new { error = -554, message = "Не пройдены проверки валыдиции. пока затычка такая" });
                }
                catch (Exception e)
                {
                    intError = 1;

                    strMsg = String.Format("Ошибка выполнения этапа бизнес процесса: {0}", e.ToStringWithInner());
                }
                return new JsonNetResult(new { error = intError, message = strMsg });
            }
        }

        public ActionResult ExecuteAction(int actionID, int objectID, string objectType, string comment, int? userID)
        {
            using (var uow = CreateUnitOfWork())
            {
                var action = uow.GetRepository<StageAction>().All().FirstOrDefault(x => x.ID == actionID);
                var objectService = this.GetService<IWFObjectService>(objectType);

                if (action.RequiredComment)
                {
                    if (String.IsNullOrEmpty(comment))
                        return new JsonNetResult(new { error = 1, message = "Введите комментарий" });
                }
                else
                {
                    if (String.IsNullOrEmpty(comment))
                        comment = "";
                }

                var intError = 0;
                var strMsg = "";

                try
                {
                    _workflowService.InvokeStage(uow, objectService, objectID, actionID, comment, userID);
                    ClearCash(objectID, objectType);
                }
                catch (Exception e)
                {
                    intError = 1;

                    strMsg = e.ToStringWithInner();
                }
                return new JsonNetResult(new { error = intError, message = strMsg });
            }
        }

        private ICollection<StageVM> GetCurrentStages(IUnitOfWork uow, string ObjectType, int objectID)
        {
            var objService = this.GetService<IWFObjectService>(ObjectType);

            var bpObject = objService.GetAll(uow).FirstOrDefault(x => x.ID == objectID);

            var o = (IBPObject)bpObject;

            var currentStages = _contextService.GetCurrentStages(uow, o);

            return currentStages.Select(stage => new StageVM(stage)
            {
                PerformerType = _contextService.GetPerformerType(uow, stage, bpObject),
                ObjectID = objectID,
                ObjectType = ObjectType,
                PermittedUsers = _contextService.GetPermittedUsers(uow, stage.Stage, bpObject),
                CurrentUser = SecurityUser,
            }).ToList();
        }

        [AllowGuest]
        public ActionResult GetToolbar(string mnemonic, int objectID)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    if (objectID != 0)
                    {
                        WorkflowToolbarVm toolbar = new WorkflowToolbarVm(this.SecurityUser, objectID, mnemonic)
                        {
                            Stages = GetCurrentStages(uow, mnemonic, objectID)
                        };
                        return PartialView("~/Views/BusinessProcess/_Toolbar.cshtml", toolbar);
                    }
                    return null;
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.ToStringWithInner());
                }

            }
        }

        public ActionResult TakeForPerform(int objectID, string objectType, int stageID, int? userID)
        {
            using (var uow = CreateUnitOfWork())
            {
                var intError = 0;
                var strMsg = "";

                BaseObject result = null;

                try
                {
                    var objService = this.GetService<IWFObjectService>(objectType);
                    _workflowService.TakeForPerform(uow, objService, userID, stageID, objectID);

                    ClearCash(objectID, objectType);
                }
                catch (Exception e)
                {
                    intError = 1;
                    strMsg = String.Format("Ошибка выполнения этапа бизнес процесса: {0}", e.ToStringWithInner());
                }

                return new JsonNetResult(new
                {
                    error = intError,
                    message = strMsg,
                    model = result
                });

            }
        }

        private void ClearCash(int objectID, string objectType)
        {
            string key = _getCashKey(objectType, objectID, WorkflowCacheType.Toolbar);
            _cashService.Clear(key);

            key = _getCashKey(objectType, objectID, WorkflowCacheType.TimeLine);
            _cashService.Clear(key);
        }

        private string _getCashKey(string type, int objectId, WorkflowCacheType cashType)
        {
            string key = String.Format("A5EC7C63F62848DE8CE09CBF49D1B7C4-[{0}|{1}|{2}]", GetViewModelConfig(type).TypeEntity, objectId, cashType);
            return key;
        }

        public ActionResult ReleasePerform(int objectID, string objectType, int stageID)
        {
            using (var uow = CreateUnitOfWork())
            {
                var intError = 0;
                var strMsg = "";

                BaseObject result = null;

                try
                {
                    var objService = this.GetService<IWFObjectService>(objectType);
                    _workflowService.ReleasePerform(uow, objService, stageID, objectID);
                    ClearCash(objectID, objectType);
                }
                catch (Exception e)
                {
                    intError = 1;
                    strMsg = String.Format("Ошибка выполнения этапа бизнес процесса: {0}", e.ToStringWithInner());
                }

                return new JsonNetResult(new
                {
                    error = intError,
                    message = strMsg,
                    model = result
                });

            }
        }

        [AllowGuest]
        public ActionResult TimeLine(string objectType, int objectID, int workflowID, bool showCurrentStages = true)
        {
            using (var uow = CreateUnitOfWork())
            {
                try
                {
                    var workflow = uow.GetRepository<Workflow>().All().FirstOrDefault(x => x.ID == workflowID);

                    TimeLineVm model = new TimeLineVm
                    {
                        WorkflowId = workflowID,
                        Elements = new List<TimeLineElementVm>(),
                    };

                    var history = _workflowService.GetAllChangeHistory(uow, workflow.ObjectType, objectID).Where(x => x.Step.WorkflowID == workflow.ID && (x.Step is Stage || x.Step is WorkflowOwnerStep));
                    foreach (var historyItem in history)
                    {
                        TimeLineElementVm result = null;
                        if (historyItem.Step is Stage)
                        {
                            result = GetStageTimeLineElement(uow, historyItem);
                        }
                        else if (historyItem.Step is WorkflowOwnerStep)
                        {
                            var wfownerStep = (WorkflowOwnerStep)historyItem.Step;
                            var endstep = _workflowService.GetAllChangeHistory(uow, workflow.ObjectType, objectID).FirstOrDefault(x => x.Step.WorkflowID == wfownerStep.ChildWorkflowId && x.Step is EndStep);
                            DateTime? endDate = null;
                            if (endstep != null)
                                endDate = endstep.Date;
                            result = new WorkflowOwnerStepTimeLineLementVm(endDate, historyItem);
                        }
                        model.Elements.Add(result);
                    }
                    var list = model.Elements.ToList();

                    for (var i = 0; i < list.Count; i++)
                        if (i + 1 < list.Count)
                            list[i].Previous = list[i + 1];

                    //TODO : set real mnemonic

                    if (showCurrentStages)
                    {
                        model.ShowCurrentStages = true;
                        model.CurrentStages = new CurrentStagesVM
                        {
                            CurrnetStages = GetCurrentStages(uow, workflow.ObjectType, objectID),
                            ObjectID = objectID,
                            ObjectType = workflow.ObjectType,
                        };
                    }

                    var partialView = RenderPartialViewToString("~/Views/BusinessProcess/_TimeLine.cshtml", model);
                    return Content(partialView);
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.ToStringWithInner());
                }
            }
        }

        private TimeLineElementVm GetStageTimeLineElement(IUnitOfWork uow, ChangeHistory lastHistoryItem)
        {
            StageTimeLineElementVm result;
            Stage stage = lastHistoryItem.Step as Stage;
            if (stage.Outputs != null && stage.Outputs.Any(o => !o.Hidden))
            {
                result = new ClosedTimeLineElementVm(lastHistoryItem);
            }
            else
            {
                result = new TerminatedTimeLineElementVm(lastHistoryItem);
            }
            return result;
        }

        public ActionResult GetActions(string objType)
        {
            var type = this.GetType(objType);

            if (type == null) return null;

            var config = this.DefaultViewModelConfig(type);

            if (config == null) return null;

            return new JsonNetResult(config.TypeEntity.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.PropertyType.IsEnum)
                    .ToDictionary(x => x.Name, x => new BPActionBuilderVm
                    {
                        Member = x.Name,
                        Title = x.GetCustomAttribute<DetailViewAttribute>().With(d => d.Name) ?? x.Name,
                        Values = Enum.GetValues(x.PropertyType).Cast<Enum>().Select(e => new BPActionValueVm
                        {
                            Title = e.GetDescription(),
                            Value = e.ToString()
                        })
                    }));
        }

        public ActionResult CheckTypes(string testType, string baseType)
        {
            var allTypes = ViewModelConfigs.Select(x => x.TypeEntity).ToList();

            var type1 = allTypes.FirstOrDefault(x => x.FullName == testType);
            var type2 = allTypes.FirstOrDefault(x => x.FullName == baseType);

            return new JsonNetResult(new { result = type1 != null && type2 != null && (type2.IsAssignableFrom(type1) || type1 == type2) });
        }

        public ActionResult TaskToolbar(int? taskID)
        {
            using (var uow = CreateUnitOfWork())
            {
                if (taskID.HasValue && taskID.Value != 0)
                {
                    var task = _taskService.GetAll(uow).FirstOrDefault(x => x.ID == taskID.Value);

                    ChangeHistory historyItem = null;

                    var isRelatedTask = false;

                    var bpTask = task as BPTask;
                    //if (bpTask != null)    TODO : понять че за нах
                    //    historyItem = bpTask.ChangeHistory;

                    //var relatedTask = task as RelatedTask;
                    //if (relatedTask != null)
                    //{
                    //    isRelatedTask = true;
                    //    historyItem = relatedTask.ChangeHistory;
                    //}


                    if (historyItem != null)
                    {
                        var type = this.GetType(historyItem.ObjectType);

                        if (type != null)
                        {
                            var mnemonic = type.FullName;

                            var obj = _serviceProvider.GetObjectService(mnemonic).Get(uow, historyItem.ObjectID);

                            if (typeof(ICategorizedItem).IsAssignableFrom(type))
                            {
                                var listViewCategorizedItem = this.DefaultViewModelConfig(type).ListView as ListViewCategorizedItem;
                                if (listViewCategorizedItem != null)
                                {
                                    var mnemonicCategory = listViewCategorizedItem.MnemonicCategory;

                                    var configCategory = this.GetViewModelConfig(mnemonicCategory);

                                    var treeView = configCategory.ListView as TreeView;
                                    if (treeView != null && treeView.ExtendedCategory)
                                    {
                                        var categorizedItem = obj as ICategorizedItem;

                                        if (categorizedItem != null)
                                        {
                                            var extendedCategory = categorizedItem.Category as IExtendedCategory;

                                            if (extendedCategory != null && !String.IsNullOrEmpty(extendedCategory.CategoryItemMnemonic))
                                                mnemonic = extendedCategory.CategoryItemMnemonic;
                                        }
                                    }
                                }
                            }

                            if (obj != null)
                            {
                                return !isRelatedTask
                                    ? PartialView("_TaskToolbar",
                                        new TaskToolbarViewModel(this.GetViewModelConfig(mnemonic), historyItem, obj))
                                    : PartialView("_TaskToolbarWithObjectLink",
                                        new TaskToolbarViewModel(this.GetViewModelConfig(mnemonic), historyItem, obj)
                                        {
                                            TaskID = taskID.Value,
                                            IconOnly = true
                                        });
                            }
                        }
                    }
                }
            }


            return RedirectToAction("Toolbar", "Task", new { taskID = taskID });
        }

        public ActionResult GetHelp(string objectType)
        {
            var type = this.GetType(objectType);

            if (type != null)
            {
                var types = new[] { typeof(String), typeof(Decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid) };

                var items = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.PropertyType.IsValueType || type.IsPrimitive || types.Contains(x.PropertyType))
                    .Select(x => new { Attr = x.GetCustomAttribute<DetailViewAttribute>(), Property = x })
                    .Where(x => x.Attr != null)
                    .Select(x => new { Title = x.Attr.Name, Text = String.Format("[{0}] или [{1}]", x.Property.Name, x.Attr.Name) })
                    .Concat(new[] { new { Title = "Предыдущий комментарий", Text = "[Comment]" } });

                return JsonNet(items);
            }

            return null;
        }

        public ActionResult GetEditorList(string objectType)
        {
            var type = this.GetType(objectType);

            if (type != null)
            {
                var props = this.GetEditors(type, x =>
                {
                    if (x != typeof(String) && x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        return false;
                    }

                    return true;
                }).ToList();

                return PartialView("_GetEditorList", new ObjectInitializerVm
                {
                    Editors = props,
                    Type = type,
                    Mnemonic = this.DefaultViewModelConfig(type).Mnemonic
                });
            }

            return null;
        }

        public ActionResult GetEditor(string mnemonic, string property, string parenttype, string objectType)
        {
            var type = Type.GetType(objectType) ?? this.GetType(objectType);
            var parentType = this.GetType(parenttype);

            if (type != null)
            {
                var model = new WithCustomEditorVm
                {
                    Config = this.GetViewModelConfig(mnemonic),
                    Property = property
                };

                if (type.IsNumericType() && parentType != null)
                {
                    model.Editors = this.GetEditors(parentType, x => x.IsNumericType());

                    return View("Editors/Int", model);
                }

                if (typeof(BaseObject).IsAssignableFrom(type))
                {
                    ViewBag.Config = this.DefaultViewModelConfig(type);

                    model.Editors = this.GetEditors(parentType, x => x == type);

                    return View("Editors/BaseObject", model);
                }

                if (type == typeof(String))
                {
                    model.Editors = this.GetEditors(parentType, x => x == typeof(String));

                    return View("Editors/String", model);
                }

                if (type.IsEnum)
                {
                    model.Editors = Enum.GetValues(type).OfType<Enum>().Select(x => new EditorVm(x.GetDescription(), x.GetValue().ToString()));

                    return View("Editors/Enum", model);
                }

                if (type == typeof(bool) || type == typeof(bool?))
                {
                    model.Editors = model.Editors = this.GetEditors(parentType, x => x == typeof(bool) || x == typeof(bool?));

                    return View("Editors/Bool", model);
                }

                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    model.Editors = model.Editors = this.GetEditors(parentType, x => x == typeof(DateTime) || x == typeof(DateTime?));

                    return View("Editors/DateTime", model);
                }
            }

            return null;
        }


        private IEnumerable<EditorVm> GetEditors(Type type, Func<Type, bool> predicate = null)
        {
            var props = new List<EditorVm>();

            var allProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.SetField).Where(x => x.CanWrite).ToList();

            if (typeof(ICategorizedItem).IsAssignableFrom(type))
            {
                var hCategoryProperty = allProps.FirstOrDefault(x => Attribute.IsDefined(x, typeof(ForeignKeyAttribute)));

                if (hCategoryProperty != null)
                    props.Add(new EditorVm("Категория", hCategoryProperty));
            }

            if (predicate != null)
                allProps = allProps.Where(x => predicate(x.PropertyType)).ToList();
            return allProps.Select(x => new { Prop = x, Attr = x.GetCustomAttribute<DetailViewAttribute>() })
                .Where(x => x.Attr != null).Select(x => new EditorVm(x.Attr.Name, x.Prop)).Union(props);
        }

        private Type GetType(string objectType)
        {
            if (!String.IsNullOrEmpty(objectType))
                return ViewModelConfigs.Select(x => x.TypeEntity)
                   .FirstOrDefault(x => x.FullName == objectType);

            return null;
        }

        public ActionResult GetObject(string type, int? id = null)
        {
            using (var uow = CreateUnitOfWork())
            {
                var viewModelConfig = this.ViewModelConfigs.FirstOrDefault(x => x.TypeEntity.FullName == type);

                if (viewModelConfig != null)
                {
                    BaseObject baseObject = null;

                    if (id != null)
                    {
                        var serv = this.GetService<IBaseObjectCRUDService>(viewModelConfig.Mnemonic);

                        baseObject = serv.Get(uow, id.Value);
                    }

                    return JsonNet(new
                    {
                        Config = new { viewModelConfig.LookupProperty, ObjectType = viewModelConfig.TypeEntity.FullName },
                        Object = baseObject.IfNotNullReturn(x => new { x.ID, Title = x.GetType().GetProperty(viewModelConfig.LookupProperty).GetValue(x) })
                    });
                }

                return null;
            }
        }

        public ActionResult TestMacros(IEnumerable<InitItem> items, string type, string parentType)
        {
            using (var uow = CreateUnitOfWork())
            {
                var objType = this.GetType(type);
                var parentObjType = this.GetType(parentType);

                Exception exception;
                var result = _workflowService.TestMacros(uow, items, objType, parentObjType, out exception);

                return JsonNet(new
                {
                    result,
                    error = !result ? exception.ToStringWithInner() : String.Empty
                });
            }
        }

        public ActionResult WorkflowList(string mnemonic, BaseObject model)
        {
            using (var uow = CreateUnitOfWork())
            {
                return JsonNet(
                    _workflowService.GetWorkflowList(uow, GetViewModelConfig(mnemonic).TypeEntity, model)
                        .Select(x => new
                        {
                            x.Title,
                            x.ID,
                            x.Description,
                            x.CreatedDate,
                            x.Creator
                        }));
            }
        }

        public ActionResult StageExtenders()
        {
            var extenders =
                ViewModelConfigs.Where(x => x.TypeEntity.IsSubclassOf(typeof(StageExtender)));

            return PartialView(extenders);
        }
    }
}
