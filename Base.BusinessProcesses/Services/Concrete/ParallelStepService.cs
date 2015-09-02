using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class ParallelizationStepService : BaseObjectService<ParallelizationStep>, IParallelizationStepService
    {
        public ParallelizationStepService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        public override ParallelizationStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new ParallelizationStep
            {
                Title = "Параллельное выполнение бизнес процесса ",
                Description = "Данный шаг осуществляет параллельное выполнение бизнес процесса",
                Outputs = new List<Output>(),
                IsEntryPoint = false,
            };
        }
    }
}