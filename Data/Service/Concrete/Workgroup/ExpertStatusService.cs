using Base.Service;
using Data.Entities.Workgroup;
using Data.Service.Abstract.Workgroup;

namespace Data.Service.Concrete.Workgroup
{
    public class ExpertStatusService : BaseObjectService<ExpertStatus> , IExpertStatusService
    {
        public ExpertStatusService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}
