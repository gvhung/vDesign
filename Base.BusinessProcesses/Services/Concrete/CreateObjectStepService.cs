using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class CreateObjectStepService :BaseObjectService<CreateObjectStep>,  ICreateObjectStepService
    {
        public CreateObjectStepService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override CreateObjectStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new CreateObjectStep
            {
                Title = "Создание объекта", 
                Description = "В этом шаге создается объект",
                Outputs = new List<Output>() { new Output() },
                InitItems = new List<CreateObjectStepMemberInitItem>(),
            };
        }
    }
}