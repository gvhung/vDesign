using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class GotoStepService : BaseObjectService<GotoStep>, IGotoStepService
    {
        public GotoStepService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override GotoStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new GotoStep
            {
                Title = "Переход к шагу",
                Description = "Переход к указаному шагу",
                IsEntryPoint = false,
                ReturnToStep = "",
            };
        }
    }
}