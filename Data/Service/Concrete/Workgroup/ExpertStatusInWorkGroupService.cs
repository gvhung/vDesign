using Base.Service;
using Data.Entities.Workgroup;
using Data.Service.Abstract.Workgroup;

namespace Data.Service.Concrete.Workgroup
{
    public class ExpertStatusInWorkGroupService : BaseObjectService<ExpertStatusInWorkGroup>, IExpertStatusInWorkGroupService
    {
        public ExpertStatusInWorkGroupService(BaseObjectServiceFacade facade)
            : base(facade)
        {

        }
    }
}
