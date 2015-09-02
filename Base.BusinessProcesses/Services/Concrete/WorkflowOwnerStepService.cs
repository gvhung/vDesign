using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WorkflowOwnerStepService : BaseObjectService<WorkflowOwnerStep>, IWorkflowOwnerStepService
    {
        public WorkflowOwnerStepService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override WorkflowOwnerStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new WorkflowOwnerStep
            {
                Title = "Контейнер бизнес процесса",
                Description = "Данный этап является контейнером бизнес процесса",
                Outputs = new List<Output>() { new Output()},
            };
        }
    }
}
