using Base.DAL;
using Base.Service;
using Data.Entities.Workgroup;
using Data.Service.Abstract.Workgroup;

namespace Data.Service.Concrete.Workgroup
{
    public class ExpertService : BaseObjectService<Expert>, IExpertService
    {
        public ExpertService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<Expert> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Expert> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.User)
                .SaveOneObject(x => x.ExpertStatus);
        }
    }
}
