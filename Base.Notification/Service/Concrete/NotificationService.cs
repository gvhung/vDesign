using Base.DAL;
using Base.Notification.Entities;
using Base.Notification.Service.Abstract;
using Base.Service;
using System.Collections.Generic;
using System.Linq;

namespace Base.Notification.Service.Concrete
{
    public class NotificationService : BaseObjectService<Entities.Notification>, INotificationService
    {
        private readonly IBroadcaster _broadcaster;
        public NotificationService(IBaseObjectServiceFacade facade, IBroadcaster broadcaster) : base(facade)
        {
            _broadcaster = broadcaster;
        }

        public override Entities.Notification Get(IUnitOfWork unitOfWork, int id)
        {
            Entities.Notification notice = base.Get(unitOfWork, id);

            if (notice != null && notice.Status == NotificationStatus.New && notice.UserID == Ambient.AppContext.SecurityUser.ID)
            {
                notice.Status = NotificationStatus.Viewed;

                this.Update(unitOfWork, notice);
            }

            return notice;
        }

        public override Entities.Notification Create(IUnitOfWork unitOfWork, Entities.Notification obj)
        {
            var repository = unitOfWork.GetRepository<Entities.Notification>();
            repository.Create(obj);
            unitOfWork.SaveChanges();


            _broadcaster.SendNotification(unitOfWork, obj);

            return obj;
        }

        public override Entities.Notification Update(IUnitOfWork unitOfWork, Entities.Notification obj)
        {
            var repository = unitOfWork.GetRepository<Entities.Notification>();

            if (obj.ID == 0) repository.Create(obj);
            else repository.Update(obj);

            unitOfWork.SaveChanges();

            _broadcaster.SendNotification(unitOfWork, obj);

            return obj;
        }

        public override IList<Entities.Notification> CreateCollection(IUnitOfWork unitOfWork, IEnumerable<Entities.Notification> collection)
        {
            var repository = unitOfWork.GetRepository<Entities.Notification>();

            var notifications = collection as IList<Entities.Notification> ?? collection.ToList();
            
            foreach (var notification in notifications)
            {
                repository.Create(notification);
            }
            
            unitOfWork.SaveChanges();

            foreach (var notification in notifications)
            {
                _broadcaster.SendNotification(unitOfWork, notification);
            }

            return notifications;
        }

        public override IList<Entities.Notification> UpdateCollection(IUnitOfWork unitOfWork, IEnumerable<Entities.Notification> collection)
        {
            var repository = unitOfWork.GetRepository<Entities.Notification>();

            var notifications = collection as IList<Entities.Notification> ?? collection.ToList();
            
            foreach (var notification in notifications)
            {
                if (notification.ID == 0) repository.Create(notification);
                else repository.Update(notification);
            }
            
            unitOfWork.SaveChanges();

            foreach (var notification in notifications)
            {
                _broadcaster.SendNotification(unitOfWork, notification);
            }

            return notifications;
        }

        public override IQueryable<Entities.Notification> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return unitOfWork.GetRepository<Entities.Notification>().All();
        }
    }
}
