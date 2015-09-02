using Base.BusinessProcesses.Entities;
using Base.Security;
using System.Linq;

namespace Base.BusinessProcesses.Strategies
{
    public class WorkflowListStrategy<TEntity> : IWorkflowListStrategy where TEntity : BaseObject
    {
        public virtual IQueryable<Workflow> GetWorkflows(ISecurityUser user, TEntity obj, IQueryable<Workflow> workflows)
        {
            return workflows.Where(x => x.IsTemplate);
        }

        IQueryable<Workflow> IWorkflowListStrategy.GetWorkflows(ISecurityUser user, BaseObject obj, IQueryable<Workflow> workflows)
        {
            return GetWorkflows(user, obj as TEntity, workflows);
        }
    }
}