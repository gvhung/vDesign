using Base.Ambient;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Security;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WorkflowScheduler : IWorkflowScheduler
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IWorkflowServiceFactory _serviceFactory;
        private readonly IWorkflowUserService _workflowUserService;
        private readonly IAppContextBootstrapper _appContextBootstrapper;
        private readonly IProductionCalendarService _productionCalendarService;

        public WorkflowScheduler(
            IUnitOfWorkFactory unitOfWorkFactory,
            IWorkflowServiceFactory serviceFactory, IWorkflowUserService workflowUserService, IAppContextBootstrapper appContextBootstrapper, IProductionCalendarService productionCalendarService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _serviceFactory = serviceFactory;
            _workflowUserService = workflowUserService;
            _appContextBootstrapper = appContextBootstrapper;
            _productionCalendarService = productionCalendarService;
        }

        public void ProcessWorkflows(Expression<Func<WorkflowContext, bool>> selectStrategy)
        {
            const string comment = "Время этапа вышло, менеджер бизнес-процессов перевел объект на следующий этап";          

            using (_appContextBootstrapper.LocalContextSecurity(_workflowUserService.WorkflowManager))
            {
                using (var uof = _unitOfWorkFactory.CreateSystem())
                {
                    //var stagePerformsIDs = uof.GetRepository<WorkflowContext>().All().SelectMany(x => x.CurrentStages).Where(z => z.Stage.AutoProcess && z.ObjectID != null && !z.Hidden).Select(x => x.ID).ToArray();
                    var stagePerforms = 
                        uof.GetRepository<StagePerform>()
                            .All()
                            .Where(x => x.Stage.AutoProcess && x.ObjectID != null && !x.Hidden);

                    var service = _serviceFactory.WorkflowService();

                    foreach (StagePerform perform in stagePerforms)
                    {
                        if (_productionCalendarService.GetEndDate(perform.BeginDate, perform.Stage.PerformancePeriod, perform.Stage.PerfomancePeriodType) <= DateTime.Now)
                        {
                            service.AutoInvokeStage(uof, perform.ObjectID.GetValueOrDefault(), perform, comment);
                        }
                    }
                }
            }
        }


        public void ProcessWorkflowsDefault()
        {
            ProcessWorkflows(x => !x.Hidden);
        }
    }

    [Serializable]
    public class WorkflowAutoProcessException : Exception
    {
        public StagePerform StagePerform { get; set; }

        public WorkflowAutoProcessException()
        {
        }

        public WorkflowAutoProcessException(StagePerform stagePerform, Exception inner)
            : base(inner.Message, inner)
        {
            StagePerform = stagePerform;
        }

        protected WorkflowAutoProcessException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}