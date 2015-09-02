using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.Service;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WeekendService : BaseObjectService<Weekend>, IWeekendService
    {
        public WeekendService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}
