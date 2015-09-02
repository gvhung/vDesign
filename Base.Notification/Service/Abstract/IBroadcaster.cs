using Base.DAL;

namespace Base.Notification.Service.Abstract
{
    public interface IBroadcaster
    {
        void SendNotification(IUnitOfWork unitOfWork, Entities.Notification notification);
    }
}
