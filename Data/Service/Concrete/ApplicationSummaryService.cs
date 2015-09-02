using Base.DAL;
using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class ApplicationSummaryService : BaseObjectService<ApplicationSummary>, IApplicationSummaryService
    {
        public ApplicationSummaryService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<ApplicationSummary> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ApplicationSummary> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveManyToMany(x => x.ApplicationAreas);
        }
    }
}
