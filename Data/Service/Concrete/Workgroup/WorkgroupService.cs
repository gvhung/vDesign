using Base;
using Base.DAL;
using Base.Service;
using Data.Entities.Workgroup;
using Data.Service.Abstract.Workgroup;
using System.Collections.Generic;

namespace Data.Service.Concrete.Workgroup
{
    public class WorkgroupService : BaseObjectService<Entities.Workgroup.WorkGroup>, IWorkgroupService
    {
        public WorkgroupService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<WorkGroup> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<WorkGroup> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneToMany(x => x.WorkGroupExperts, x => x.SaveOneObject(z => z.Expert).SaveOneObject(y => y.ExpertStatusInWorkGroup));
        }

        public override WorkGroup CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new WorkGroup
            {
                WorkGroupExperts = new List<WorkGroupExpert>()
            };
        }
    }
}
