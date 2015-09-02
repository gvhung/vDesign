using Base.Service;

namespace Base.Event.Service
{
    public class EventTypeService : BaseObjectService<Base.Event.Entities.EventType>, IEventTypeService
    {
        public EventTypeService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
