using Base.DAL;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class OrganizationService : BaseObjectService<Organization>, IOrganizationService
    {
        public OrganizationService(BaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<Organization> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Organization> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneToMany(x => x.Manufacturings);
        }
    }
}
