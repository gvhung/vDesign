using Base.DAL;
using Base.Events;
using Base.Service;
using System.Collections.Generic;

namespace Base.Notification.Service.Abstract
{
    public interface IManagerNotification : IService
    {
        void CreateNotice(IUnitOfWork unitOfWork, BaseObject obj, BaseEntityState state);
        void CreateNotices(IUnitOfWork unitOfWork, Dictionary<BaseObject, BaseEntityState> objects);
        void CreateNotice(BaseObjectEventArgs e);
    }
}
