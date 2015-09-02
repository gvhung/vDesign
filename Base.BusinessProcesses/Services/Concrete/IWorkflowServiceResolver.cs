using Base.BusinessProcesses.Strategies;
using Base.DAL;
using Base.Service;
using System;

namespace Base.BusinessProcesses.Services.Concrete
{
    public interface IWorkflowServiceResolver
    {
        IBaseObjectCRUDService GetObjectService(string objectTypeStr, IUnitOfWork unitOfWork = null);
        IBaseObjectCRUDService GetObjectService(Type type, IUnitOfWork unitOfWork = null);

        IWorkflowStrategy GetStrategy(Type type);
        WorkflowCommonStrategyModule GetStrategyModule(Type type);
    }
}