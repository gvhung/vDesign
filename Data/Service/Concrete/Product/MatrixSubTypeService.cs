using Base.DAL;
using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class MatrixSubTypeService : BaseObjectService<MatrixSubType>, IMatrixSubTypeService
    {
        public MatrixSubTypeService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        protected override IObjectSaver<MatrixSubType> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<MatrixSubType> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.MatrixType);
        }
    }
}
