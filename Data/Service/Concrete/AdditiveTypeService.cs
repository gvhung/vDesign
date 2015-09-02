using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class AdditiveTypeService : BaseObjectService<AdditiveType> , IAdditiveTypeService
    {
        public AdditiveTypeService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}
