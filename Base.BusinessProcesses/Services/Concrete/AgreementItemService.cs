using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.Service;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class AgreementMapService : BaseObjectService<AgreementItem>, IAgreementItemService
    {
        public AgreementMapService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
