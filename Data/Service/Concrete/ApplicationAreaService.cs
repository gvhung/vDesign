using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class ApplicationAreaService : BaseObjectService<ApplicationArea>, IApplicationAreaService
    {
        public ApplicationAreaService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

    }
}
