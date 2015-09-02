using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class FillerTypeService : BaseObjectService<FillerType>, IFillerTypeService
    {
        public FillerTypeService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }
    }
}
