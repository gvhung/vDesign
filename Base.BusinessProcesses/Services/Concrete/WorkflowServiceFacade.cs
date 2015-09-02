using Base.Ambient;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.BusinessProcesses.Strategies;
using Base.DAL;
using Base.Helpers;
using Base.Security;
using Base.Security.ObjectAccess;
using Base.Security.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WorkflowServiceFacade : IWorkflowServiceFacade
    {
        private readonly ISecurityService _accessItemService;
        private readonly ITemplateRenderer _renderer;
        private readonly IWorkflowServiceResolver _serviceResolver;
        private readonly IWFObjectInitializer _objectInitializer;
        private readonly IWorkflowStrategyService _strategyService;
        private readonly IHelperJsonConverter _helperJsonConverter;

        public WorkflowServiceFacade(
            ISecurityService accessItemService,
            ITemplateRenderer renderer,
            IWorkflowServiceResolver serviceResolver,
            IWFObjectInitializer objectInitializer,
            IWorkflowStrategyService strategyService,
            IHelperJsonConverter helperJsonConverter)
        {
            _accessItemService = accessItemService;
            _renderer = renderer;
            _serviceResolver = serviceResolver;
            _objectInitializer = objectInitializer;
            _strategyService = strategyService;
            _helperJsonConverter = helperJsonConverter;
        }

        public ObjectAccessItem CreateAccessItem(IUnitOfWork uow, BaseObject obj)
        {
            return _accessItemService.CreateAndSaveAccessItem(uow, obj);
        }

        public string Render(string template, BaseObject obj, IDictionary<string, string> additional = null)
        {
            return _renderer.Render(template, obj, additional);
        }

        public IWFObjectService GetService(string objectTypeStr, IUnitOfWork unitOfWork = null)
        {
            return _serviceResolver.GetObjectService(objectTypeStr, unitOfWork) as IWFObjectService;
        }

        public void InitializeObject(ISecurityUser securityUser, BaseObject src, BaseObject dest,
            IEnumerable<InitItem> inits)
        {
            _objectInitializer.InitializeObject(securityUser, src, dest, inits);
        }

        public void ModifyObject(ISecurityUser securityUser, BaseObject src, IEnumerable<InitItem> inits)
        {
            _objectInitializer.InitializeObject(securityUser, src, src, inits);
        }

        public BaseObject CloneObject(BaseObject obj, Type objType)
        {
            var ser = _helperJsonConverter.SerializeObject(obj);
            var deser = _helperJsonConverter.DeserializeObject(ser, objType);
            return deser;
        }

        public Workflow CloneSmallCopy(Workflow wf)
        {
            throw new NotImplementedException("Реализуй создание малой копии");
        }

        public Workflow CloneWorkflow(Workflow wf)
        {
            Workflow clonedWF = new Workflow(wf) {CreatedDate = AppContext.DateTime.Now};
            return clonedWF;
        }
        
        public void CreateChildAccessItem(IUnitOfWork unitOfWork, Workflow wf)
        {
            var wfParentSteps = wf.Steps.OfType<WorkflowOwnerStep>();
            foreach (var parentStep in wfParentSteps)
            {
                CreateAccessItem(unitOfWork, parentStep.ChildWorkflow);
                CreateChildAccessItem(unitOfWork, parentStep.ChildWorkflow);
            }
        }

        public void SaveExtendedStage(IUnitOfWork uow, ExtendedStage extendedStage, IObjectSaver<ExtendedStage> stageSaver)
        {
            if (extendedStage.Extender != null)
            {
                var extenderType = extendedStage.Extender.GetType().GetBaseObjectType();

                var exStageService = _serviceResolver.GetObjectService(extenderType);
                if (exStageService.GetType().GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IStageExtenderService<>)))
                {
                    var createMethod = stageSaver.GetType().GetMethod("Create").MakeGenericMethod(extenderType);
                    var extenderSaver = createMethod.Invoke(stageSaver, new[] { extendedStage.Extender, null });

                    var saveMethod = exStageService.GetType().GetMethod("ExternalSave");
                    if (saveMethod != null)
                        saveMethod.Invoke(exStageService, new[] { uow, extenderSaver });
                }
            }
        }

        public void OnEnterToExtendedStage(ISecurityUser securityUser, ExtendedStage extendedStage, BaseObject baseObject)
        {
            var service = GetExtenderService(extendedStage);

            service.OnStageEnter(securityUser, extendedStage, extendedStage.Extender, baseObject);
        }

        public void OnLeaveFromExtendedStage(ISecurityUser securityUser, ExtendedStage extendedStage, BaseObject baseObject)
        {
            var service = GetExtenderService(extendedStage);

            service.OnStageLeave(securityUser, extendedStage, extendedStage.Extender, baseObject);
        }

        private IStageExtenderService GetExtenderService(ExtendedStage extendedStage)
        {
            if (extendedStage.Extender != null)
            {
                var extenderService =
                    _serviceResolver.GetObjectService(extendedStage.Extender.GetType().GetBaseObjectType()) as IStageExtenderService;

                if (extenderService != null)
                    return extenderService;
            }

            throw new Exception("Extender service not founded");
        }

        public IQueryable<Workflow> GetWorkflowList(ISecurityUser securityUser, Type type, BaseObject model, IQueryable<Workflow> all)
        {
            var listStrategy = _strategyService.GetCommonStrategyInstance<IWorkflowListStrategy>(x => x.EntityType == type) ??
                               new WorkflowListStrategy<BaseObject>();

            return listStrategy.GetWorkflows(securityUser, model, all).Where(x => x.ObjectType == type.FullName);
        }
    }
}