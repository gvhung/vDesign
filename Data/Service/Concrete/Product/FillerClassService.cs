using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class FillerClassService : BaseObjectService<FillerClass>, IFillerClassService
    {
        public FillerClassService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }
    }
}
