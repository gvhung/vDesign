using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class MatrixTypeService : BaseObjectService<MatrixType>, IMatrixTypeService
    {
        public MatrixTypeService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }
    }
}
