using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Concrete;
using Base.BusinessProcesses.Strategies;
using Base.Security;
using Data.Entities;
using System.Linq;

namespace Data.Strategies
{
    public class WorkflowStrategies : WorkflowCommonStrategyModule
    {
        public override void Register()
        {
            //Bind<BaseWorkflowSelectorStrategy>().ToEntity<TestObject>();
            //Bind<BaseWorkflowSelectorStrategy>().ToEntity<NpaWizard>();
        }
    }

    public class BaseWorkflowSelectorStrategy : WorkflowListStrategy<TestObject>
    {
        public override IQueryable<Workflow> GetWorkflows(ISecurityUser user, TestObject obj, IQueryable<Workflow> workflows)
        {
            //return Enumerable.Empty<Workflow>().AsQueryable();

            return base.GetWorkflows(user, obj, workflows);
        }
    }
}