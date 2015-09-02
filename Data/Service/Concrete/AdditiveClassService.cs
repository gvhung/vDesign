using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class AdditiveClassService : BaseObjectService<AdditiveClass>, IAdditiveClassService
    {
        public AdditiveClassService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}
