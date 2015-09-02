using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class RestoreStepService : BaseObjectService<RestoreStep>, IRestoreStepService
    {
        public RestoreStepService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override RestoreStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {

            return new RestoreStep
            {
                Title = "Возврат по истории",
                Description = "Данный шаг осуществляет возврат на несколько шагов назад по истории",
                BackStepCount = 1
            };

        }

    }
}