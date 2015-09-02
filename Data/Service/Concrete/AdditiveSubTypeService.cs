using Base.DAL;
using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class AdditiveSubTypeService : BaseObjectService<AdditiveSubType>, IAdditiveSubTypeService
    {
        public AdditiveSubTypeService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<AdditiveSubType> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<AdditiveSubType> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.AdditiveType);
        }
    }
}
