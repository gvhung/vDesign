using Base.Service;

namespace Base.Contractor
{
    public class ContactPersonService : BaseObjectService<ContactPerson>, IContactPersonService
    {
        public ContactPersonService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
