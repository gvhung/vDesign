using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class EndStepService : BaseObjectService<EndStep>, IEndStepService
    {
        public EndStepService(IBaseObjectServiceFacade facade) : base(facade) { }


        public override EndStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new EndStep
            {
                Title = "Точка выхода",
                IsEntryPoint = false,
                Outputs =  new List<StageAction>(),
            };
        }
    }
}
