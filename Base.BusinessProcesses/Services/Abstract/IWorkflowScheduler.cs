using Base.BusinessProcesses.Entities;
using System;
using System.Linq.Expressions;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IWorkflowScheduler
    {
        void ProcessWorkflowsDefault();
        void ProcessWorkflows(Expression<Func<WorkflowContext, bool>> selectStrategy);
    }
}