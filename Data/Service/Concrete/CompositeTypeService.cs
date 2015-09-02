using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class CompositeTypeService : BaseObjectService<CompositeType> , ICompositeTypeService
    {
        public CompositeTypeService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}
