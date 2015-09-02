using Base.BusinessProcesses.Entities;
using Base.Security;
using System.Linq;

namespace Base.BusinessProcesses.Strategies
{
    internal interface IWorkflowListStrategy : IWorkflowStrategy
    {
        IQueryable<Workflow> GetWorkflows(ISecurityUser user, BaseObject obj, IQueryable<Workflow> workflows);
    }
}