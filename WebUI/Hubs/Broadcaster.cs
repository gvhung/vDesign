using Base.DAL;
using Base.Notification.Entities;
using Base.Notification.Service.Abstract;
using Base.Security;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;

namespace WebUI.Hubs
{
    public class Broadcaster : IBroadcaster
    {
        private readonly IHubContext _hubContext;
        
        public Broadcaster()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }

        public void SendNotification(IUnitOfWork unitOfWork, Notification notification)
        {
            string user = notification.User != null ? notification.User.Login : null;

            if (user == null && notification.UserID != null)
            {
                user =
                    unitOfWork.GetRepository<User>()
                        .All()
                        .Where(x => x.ID == notification.UserID)
                        .Select(x => x.Login)
                        .FirstOrDefault();
            }

            if(String.IsNullOrEmpty(user)) return;

            if (notification.Status == NotificationStatus.New)
                _hubContext.Clients.User(user).create(notification.Title);
            else
                _hubContext.Clients.User(user).update(notification.ID);
        }
    }
}