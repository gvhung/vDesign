using Base.DAL;
using Base.Service;
using Data.Entities.Product;
using Data.Service.Abstract.Product;

namespace Data.Service.Concrete.Product
{
    public class FillerSubClassService : BaseObjectService<FillerSubClass>, IFillerSubClassService
    {
        public FillerSubClassService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<FillerSubClass> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<FillerSubClass> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.FillerClass);
        }
    }
}
