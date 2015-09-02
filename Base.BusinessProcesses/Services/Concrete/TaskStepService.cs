using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class TaskStepService : BaseObjectService<TaskStep>, ITaskStepService
    {
        public TaskStepService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override TaskStep CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new TaskStep
            {
                Title = "Создание напоминания",
                Outputs = new List<Output>() {new Output()},
            };
        }
    }
}