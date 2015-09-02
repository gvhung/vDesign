using Base.Notification.Service.Abstract;
using Base.Service;

namespace Base.Notification.Service.Concrete
{
    public class DispatchService : BaseObjectService<Entities.DispatchHistory>, IDispatchService
    {
        public DispatchService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
