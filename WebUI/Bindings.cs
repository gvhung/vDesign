using Base;
using Base.Ambient;
using Base.Audit;
using Base.Audit.Services;
using Base.BusinessProcesses.Security;
using Base.BusinessProcesses.Services.Abstract;
using Base.BusinessProcesses.Services.Concrete;
using Base.Censorship;
using Base.Censorship.Service;
using Base.Conference.Service;
using Base.Content.Service.Abstract;
using Base.Content.Service.Concrete;
using Base.DAL;
using Base.Document.Service;
using Base.Event.Service;
using Base.FileStorage;
using Base.Forum.Service;
using Base.Help.Services;
using Base.Helpers;
using Base.LinkedObjects.Service;
using Base.Notification;
using Base.Notification.Service.Abstract;
using Base.Notification.Service.Concrete;
using Base.OpenID.Service.Abstract;
using Base.OpenID.Service.Concrete;
using Base.Security;
using Base.Security.ObjectAccess.Services;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using Base.Security.Service.Concrete;
using Base.Service;
using Base.Service.Log;
using Base.Settings;
using Base.Task.Services;
using Base.UI;
using Base.UI.Service;
using Base.Validation;
using Base.Wrappers.Web;
using Data.Entities;
using Data.Service.Abstract;
using Data.Service.Abstract.Workgroup;
using Data.Service.Concrete;
using Data.Service.Concrete.Workgroup;
using Data.Strategies;
using Framework.Wrappers;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Ninject.Web.Common;
using System.Web;
using WebUI.Concrete;
using WebUI.Controllers;
using WebUI.Helpers;


namespace WebUI
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IBaseControllerServiceFacade>().To<BaseControllerServiceFacade>();
            Bind<IBaseObjectServiceFacade>().To<BaseObjectServiceFacade>();

            Bind<IUnitOfWorkFactory>().To<UnitOfWorkFactory>().InSingletonScope();
            Bind<IRepositoryFactory>().To<RepositoryFactory>();

            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<ISystemUnitOfWork>().To<SystemUnitOfWork>();
            Bind<ITransactionUnitOfWork>().To<TransactionUnitOfWork>();
            Bind<ISystemTransactionUnitOfWork>().To<SystemTransactionUnitOfWork>();

            Bind<IEntityConfiguration>().To<EntityConfiguration>().InSingletonScope();
            Bind<ISecurityService>().To<SecurityService>();
            Bind<ILogService>().To<LogService>().InSingletonScope();

            //Kernel.BindFilter<AllowGuestAttributeImpl>(FilterScope.Controller, -1)
            //    .WhenActionMethodHas<AllowGuestAttribute>().InRequestScope();

            //Kernel.BindFilter<AllowGuestAttributeImpl>(FilterScope.Controller, -1)
            //    .WhenControllerHas<AllowGuestAttribute>().InRequestScope();

            Bind<IGuestRoleProvider>().To<GuestRoleProvider>().InSingletonScope();
            Bind<IGuestUserService>().To<GuestUserService>().InSingletonScope();
            
            Bind<IAppContextBootstrapper>().To<AppContextBootstrapper>();
            Bind<IDateTimeProvider>().To<DefaultDateTimeProvider>();

            #region Wrappers

            Bind<ISessionWrapper>()
                .ToMethod(x => new SessionWrapper(HttpContext.Current != null ? HttpContext.Current.Session : null))
                .InRequestScope();

            Bind<ICacheWrapper>().To<CacheWrapper>().InSingletonScope();
            Bind<IWebClientAdapter>().To<WebClientAdapter>();
            Bind<IPostedFileWrapper>().To<PostedFileWrapper>();

            Bind<IHelperJsonConverter>().To<HelperJsonConverter>().InSingletonScope();
            #endregion

            Bind<ISecurityUserService>().To<SecurityUserService>().InSingletonScope();

            Bind<IViewModelConfigLoader>().To<ViewModelConfigLoader>().InSingletonScope();
            Bind<IViewModelConfigService>().To<ViewModelConfigService>().InSingletonScope(); 
            Bind<IDetailViewService>().To<DetailViewService>().InSingletonScope();
            Bind<IListViewService>().To<ListViewService>().InSingletonScope();
            Bind<IUiFasade>().To<UiFasade>();

            Bind<IDetailViewSettingService>().To<DetailViewSettingService>().InSingletonScope();
            Bind<IDetailViewSettingManager>().To<DetailViewSettingManager>().InSingletonScope();

            Bind<IMenuLoader>().To<MenuLoader>().InSingletonScope();
            Bind<IMenuService>().To<MenuService>().InScope(context => context.Request);
            Bind<IPathHelper>().To<PathHelper>().InSingletonScope();

            Bind<IRoleService>().To<RoleService>();
            Bind<IChildRoleService>().To<ChildRoleService>();
            Bind<IUserService>().To<UserService>();
            Bind<IAccessUserService>().To<AccessUserService>();
            Bind<IProfileService>().To<ProfileService>();
            Bind<IEmployeeService>().To<EmployeeService>();
            Bind<IPostService>().To<PostService>();
            Bind<IDepartmentService>().To<DepartmentService>();

            Bind<IUserPromotionService>().To<UserPromotionService>();
            Bind<IAccountManager>().To<AccountManager>();

            Bind<IUserCategoryService>().To<UserCategoryService>();

            Bind<IWordService>().To<WordService>();
            Bind<IFileSystemService>().To<FileSystemService>().InSingletonScope();
            Bind<IFileManager>().To<DefaultFileManager>().InSingletonScope();
            
            Bind<IPresetService>().To<PresetService>().InRequestScope();
            Bind<IPresetFactory>().To<PresetFactory>().InSingletonScope();

            Bind<IPermissionService>().To<PermissionService>();
            Bind<ISettingCategoryService>().To<SettingCategoryService>();
            Bind<ISettingItemService>().To<SettingItemService>();

            Bind<IFileStorageCategoryService>().To<FileStorageCategoryService>();
            Bind<IFileStorageItemService>().To<FileStorageItemService>();
            
            #region Workflow
            Bind<IWorkflowCategoryService>().To<WorkflowCategoryService>();
            //Bind<ITemplateActionService>().To<TemplateActionService>();
            //Bind<IActionMacroItemService>().To<ActionMacroItemService>();
            Bind<ITemplateMacroItemService>().To<TemplateMacroItemService>();
            Bind<IStageTemplateService>().To<StageTemplateService>();
            Bind<IStageUserService>().To<StageUserService>();
            Bind<IBPTaskService>().To<BPTaskService>();
            Bind<IStageService>().To<StageService>();
            Bind<IRestoreStepService>().To<RestoreStepService>();
            Bind<IEndStepService>().To<EndStepService>();
            Bind<IWorkflowOwnerStepService>().To<WorkflowOwnerStepService>();
            Bind<IGotoStepService>().To<GotoStepService>();
            Bind<ICreateObjectStepService>().To<CreateObjectStepService>();
            Bind<ITaskStepService>().To<TaskStepService>();
            Bind<IParallelizationStepService>().To<ParallelizationStepService>();
            Bind<IParallelEndStepService>().To<ParallelEndStepService>();
            Bind<IWorkflowContextService>().To<WorkflowContextService>();
            //Bind<IChangeHistoryService>().To<ChangeHistoryService>();

            Bind<IWorkflowServiceFacade>().To<WorkflowServiceFacade>();

            Bind<IWorkflowCacheService>().To<WorkflowCacheService>().InSingletonScope();

            Bind<IWorkflowService>().To<WorkflowService>();

            //Bind<IWorkflowService>().ToMethod(context =>
            //{
            //    var uow = new UnitOfWork(context.Kernel.Get<IRepositoryFactory>());

            //    return new WorkflowService(uow,
            //        context.Kernel.Get<ITaskService>(new TypeMatchingConstructorArgument(typeof(IUnitOfWork),
            //            (_, __) => uow)),
            //        context.Kernel.Get<IStageUserService>(
            //            new TypeMatchingConstructorArgument(typeof(IUnitOfWork), (_, __) => uow)),
            //        context.Kernel.Get<IWorkflowServiceFacade>(),
            //        context.Kernel.Get<ISecurityService>(), context.Kernel.Get<IWorkflowUserService>());
            //});

            Bind<IWorkflowServiceFactory>().ToFactory();
            Bind<IWorkflowStrategyService>()
                .To<WorkflowStrategyService>()
                .InSingletonScope()
                .OnActivation(x => x.RegisterCommonStrategies<WorkflowStrategies>());

            Bind<IWorkflowScheduler>().To<WorkflowScheduler>().InTransientScope();

            Bind<IWorkflowUserService>().To<WorkflowUserService>(); // Ctor arg in transient scope! How?
            Bind<IProductionCalendarService>().To<ProductionCalendarService>();
            Bind<IWeekendService>().To<WeekendService>();

            //Bind<IStageActionService>().To<StageActionService>();
            //Bind<IStepService>().To<StepService>();
            Bind<IBranchingStepService>().To<BranchingStepService>();
            //Bind<IBranchService>().To<BranchService>();
            Bind<IWorkflowServiceResolver>().To<NinjectBaseObjectServiceResolver>();
            Bind<IWFObjectInitializer>().To<WFObjectInitializer>();

            Bind<ITemplateRenderer>().To<TemplateRenderer>();
            //Bind<IScriptProvider>().To<PythonScriptProvider>();

            #endregion

            Bind<ITestObjectService>().To<TestObjectService>();
            Bind<ITestObjectEntryService>().To<TestObjectEntryService>();
            Bind<ITestObjectNestedEntryService>().To<TestObjectNestedEntryService>();

            //Tasks
            Bind<ITaskCategoryService>().To<TaskCategoryService>();
            Bind<IInTaskCategoryService>().To<InTaskCategoryService>();
            Bind<ITaskService>().To<TaskService>();
            Bind<ITaskReportService>().To<TaskReportService>();

            //Document
            Bind<IDocumentTemplateService>().To<DocumentTemplateService>();
            Bind<IDocumentTemplateCategoryService>().To<DocumentTemplateCategoryService>();

            //Notification
            Bind<INotificationService>().To<NotificationService>();

            //Content
            //Bind<IAnswerService>().To<AnswerService>();
            Bind<IContentCategoryService>().To<ContentCategoryService>();
            Bind<IContentItemService>().To<ContentItemService>();
            Bind<IContentSubscriberService>().To<ContentSubscriberService>();
            //Bind<ICourseCategoryService>().To<CourseCategoryService>();
            //Bind<IEducationService>().To<EducationService>();
            //Bind<IExerciseResultService>().To<ExerciseResultService>();
            //Bind<IExerciseService>().To<ExerciseService>();
            //Bind<IJournalEntryService>().To<JournalEntryService>();
            //Bind<IQuestionContentCategoryService>().To<QuestionContentCategoryService>();
            //Bind<IQuestionService>().To<QuestionService>();
            Bind<ITagCategoryService>().To<TagCategoryService>();
            Bind<ITagService>().To<TagService>();
#if IMPORT
            Bind<IEmailService>().To<DummyEmailService>();
#else
            Bind<IEmailService>().To<EmailService>();
#endif
            Bind<IEmailSettingsService>().To<EmailSettingsService>();
            Bind<ISystemUrlHelper>().To<SystemUrlHelper>().InSingletonScope();

            // Icons
            Bind<IIconService>().To<IconService>().InSingletonScope();


            #region audit

            Bind<IAuditItemService>().To<AuditItemService>();

            #endregion

            #region ObjectAccess
            Bind<IObjectAccessItemService>().To<ObjectAccessItemService>();
            Bind<IUserAccessService>().To<UserAccessService>();
            Bind<IUserCategoryAccessService>().To<UserCategoryAccessService>();
            Bind<IAccessPolicyFactory>().To<AccessPolicyFactory>();
            #endregion

            //LinkedObjects
            Bind<ILinkedObjectsService>().To<LinkedObjectsService>();
            Bind<ILinkedDocumentService>().To<LinkedDocumentService>();

            #region EntityType Event

            Bind<IEventService>().To<EventService>();
            Bind<IEventTypeService>().To<EventTypeService>();

            #endregion

            Bind<ITestExtenderService>().To<TestExtenderService>();
            //IModuleInitializer
            Bind<IBaseInitializer>().To<Base.Initializer>();
            Bind<ISecurityInitializer>().To<Base.Security.Initializer>();
            Bind<IAuditInitializer>().To<Base.Audit.Initializer>();
            Bind<INotificationInitializer>().To<Base.Notification.Initializer>();
            Bind<ICensorshipInitializer>().To<Base.Censorship.Initializer>();

            //Data
            Bind<IWorkgroupService>().To<WorkgroupService>();
            Bind<IExpertService>().To<ExpertService>();
            Bind<IWorkgroupExpertService>().To<WorkgroupExpertService>();
            Bind<IExpertStatusService>().To<ExpertStatusService>();
            Bind<IExpertStatusInWorkGroupService>().To<ExpertStatusInWorkGroupService>();


            Bind<IOkvedService>().To<OkvedService>();
  

            Bind<IHelpItemService>().To<HelpItemService>();
            Bind<IHelpItemTagService>().To<HelpItemTagService>();

        
            //OpenID
            Bind<IOpenIdService>().To<OpenIdService>();
            Bind<IOpenIdConfigService>().To<OpenIdConfigService>();
            Bind<IExtAccountService>().To<ExtAccountService>();
            Bind<IExtProfileService>().To<ExtProfileService>();

            Bind<IUnregisteredUserService>().To<UnregisteredUserService>();
            
            Bind<IBroadcaster>().To<Hubs.Broadcaster>();

            

            //Public helpers
            LoadPublic();

            //Forum
            Bind<IForumSectionService>().To<ForumSectionService>();
            Bind<IForumTopicService>().To<ForumTopicService>();
            Bind<IForumPostService>().To<ForumPostService>();
            
            Bind<IManagerNotification>().To<ManagerNotification>();

            //Censorship
            Bind<ICensorshipService>().To<CensorshipService>().InSingletonScope();
            Bind<IRequestLogService>().To<RequestLogService>().InSingletonScope();

        

            Bind<IContractResolverFactory>().To<ContractResolverFactory>().InSingletonScope();

            Bind<IValidationConfigManager>().To<ValidationConfigManager>().InSingletonScope().OnActivation(x=>x.Load());
            Bind<IValidationService>().To<ValidationService>();


            //Conf
            Bind<IConferenceService>().To<ConferenceService>();
            Bind<IConferenceMessageService>().To<ConferenceMessageService>();
            Bind<IPrivateMessageService>().To<PrivateMessageService>();
            Bind<IPublicMessageService>().To<PublicMessageService>();

        }

        #region Activation methods

        #endregion

        private void LoadPublic()
        {
      
        }
    }

    class BindingsWithEvents : Bindings
    {
        public override void Load()
        {
            base.Load();

            #region System Events
#if IMPORT
            var importRegistrator = new ImportEventRegistrator(this);
            importRegistrator.RegisterEvents();
#else
            var registrator = new EventRegistrator(this);

            registrator.RegisterEvents();
#endif
            #endregion
        }
    }
}