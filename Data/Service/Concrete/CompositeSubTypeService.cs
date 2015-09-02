using Base.DAL;
using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class CompositeSubTypeService : BaseObjectService<CompositeSubType> , ICompositeSubTypeService
    {
        public CompositeSubTypeService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<CompositeSubType> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<CompositeSubType> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x=>x.CompositeType);
        }
    }
}
