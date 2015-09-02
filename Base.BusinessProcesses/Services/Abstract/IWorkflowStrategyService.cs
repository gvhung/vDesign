using Base.BusinessProcesses.Strategies;
using Base.Service;
using System;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IWorkflowStrategyService : IService
    {
        TStrategy GetStrategyInstance<TStrategy>(string name) where TStrategy : class, IWorkflowStrategy;
        IEnumerable<WorkflowStrategyDescriptor> GetStrategies<TStrategy>() where TStrategy : class, IWorkflowStrategy;
        TStrategy GetCommonStrategyInstance<TStrategy>(Func<WorkflowCommonStrategyDescriptor, bool> selector) where TStrategy : class, IWorkflowStrategy;
    }
}