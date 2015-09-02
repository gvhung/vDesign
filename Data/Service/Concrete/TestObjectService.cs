using Base;
using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Security;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class TestObjectService : BaseObjectService<TestObject>, ITestObjectService
    {
        public TestObjectService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<TestObject> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<TestObject> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.User)
                .SaveOneToMany(x => x.TestObjectEntries, x => x.SaveOneObject(c => c.Object));
        }

        public void BeforeInvoke(BaseObject obj)
        {

        }

        public void OnActionExecuting(ActionExecuteArgs args)
        {
            var obj = args.NewObject as TestObject;

            if (obj == null || obj.NextStageDuration == null) return;

            //if(args.CurrentStages!=null && args.CurrentStages.Count() == 1)
            //{
            //    var currentStage = args.CurrentStages.FirstOrDefault();
            //    if (currentStage != null)
            //        currentStage.PerformancePeriod = obj.NextStageDuration.Value;
            //}
            obj.NextStageDuration = null;
        }

        public int GetWorkflowID(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return Workflow.Default;
        }

        public virtual int GetWorkflowID(ISecurityUser securityUser, BaseObject obj)
        {
            return Workflow.Default;
        }
    }

    public class TestObjectEntryService : BaseObjectService<TestObjectEntry>, ITestObjectEntryService
    {
        public TestObjectEntryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }

    public class TestObjectNestedEntryService : BaseObjectService<TestObjectNestedEntry>, ITestObjectNestedEntryService
    {
        public TestObjectNestedEntryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
