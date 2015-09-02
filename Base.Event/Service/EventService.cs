using Base.DAL;
using Base.Service;

namespace Base.Event.Service
{
    public class EventService : BaseObjectService<Entities.Event>, IEventService
    {
        public EventService(IBaseObjectServiceFacade facade) : base(facade) { }


        protected override IObjectSaver<Entities.Event> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Entities.Event> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.Type);
        }
    }
}
