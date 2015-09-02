using Base.DAL;
using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class MatrixSubType2Service : BaseObjectService<MatrixSubType2>, IMatrixSubTypeService2
    {
        public MatrixSubType2Service(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        protected override IObjectSaver<MatrixSubType2> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<MatrixSubType2> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.MatrixType);
        }
    }
}
