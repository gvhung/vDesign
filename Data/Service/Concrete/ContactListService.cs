using Base.DAL;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class ContactListService : BaseObjectService<ItemContactList>, IContactListService
    {
        public ContactListService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<ItemContactList> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ItemContactList> objectSaver)
        {
            objectSaver.Dest.UserID = Base.Ambient.AppContext.SecurityUser.ID;
            
            return base.GetForSave(unitOfWork, objectSaver);
        }
    }
}
