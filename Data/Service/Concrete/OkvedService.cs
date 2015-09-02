using Base.DAL;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class OkvedService : BaseObjectService<Okved>, IOkvedService
    {
        public OkvedService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<Okved> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Okved> objectSaver)
        {
            objectSaver.SaveOneObject(x => x.Image);
            return base.GetForSave(unitOfWork, objectSaver);
        }
    }
}
