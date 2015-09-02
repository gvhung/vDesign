using Base.DAL;
using Base.Service;
using Data.Entities.Workgroup;
using Data.Service.Abstract.Workgroup;

namespace Data.Service.Concrete.Workgroup
{
    public class WorkgroupExpertService : BaseObjectService<WorkGroupExpert>, IWorkgroupExpertService
    {
        public WorkgroupExpertService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<WorkGroupExpert> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<WorkGroupExpert> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.ExpertStatusInWorkGroup);
        }
    }
}
