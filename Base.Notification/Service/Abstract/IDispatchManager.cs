using Base.Service;

namespace Base.Notification.Service.Abstract
{
    public interface IDispatchManager : IService
    {
        void CreateNotice(BaseObject obj);
    }
}
