using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Security.Entities.Concrete;
using Base.Security.Service.Abstract;

namespace Base.Security.Service.Concrete
{
    public class UserStatusService : IUserStatusService
    {
        public UserStatusService()
        {
            UserStatuses = new Dictionary<int, IUserStatus>();
        }

        public Dictionary<int, IUserStatus> UserStatuses { get; set; }

        public IUserStatus GetUserStatus(int userId)
        {
            return UserStatuses.FirstOrDefault(x => x.Key == userId).Value;
        }

        public IUserStatus GetUserStatus(string connectionId)
        {
            return UserStatuses.FirstOrDefault(x => x.Value.ConnectionId == connectionId).Value;
        }

        public IUserStatus SetOnline(int userId, string connectionId)
        {
            var status = GetUserStatus(userId);

            if (status != null)
            {
                status.ConnectionId = connectionId;
                status.Online = true;
                return status;
            }

            var userStatus = new UserStatus()
            {
                UserId = userId,
                ConnectionId = connectionId,
                Online = true
            };

            UserStatuses.Add(userStatus.UserId, userStatus);

            return userStatus;
        }

        public IUserStatus SetOnline(IUserStatus userStatus)
        {
            var status = GetUserStatus(userStatus.UserId);

            if (status != null)
            {
                status.ConnectionId = userStatus.ConnectionId;
                status.Online = true;
                return status;
            }

            UserStatuses.Add(userStatus.UserId, userStatus);
            return userStatus;
        }

        public IUserStatus SetOffline(int userId)
        {
            var status = GetUserStatus(userId);

            if (status == null) return null;

            status.Online = false;
            status.LastDate = DateTime.Now;

            return status;
        }

        public IUserStatus SetOffline(IUserStatus userStatus)
        {
            var status = GetUserStatus(userStatus.UserId);

            if (status == null) return userStatus;

            status.Online = false;
            status.LastDate = DateTime.Now;

            return status;
        }

        public List<IUserStatus> GetUserStatuses(bool online = true)
        {
            return UserStatuses.Where(x => x.Value.Online == online).Select(x => x.Value).ToList();
        }

        public List<int> GetOnlineIds()
        {
            return UserStatuses.Any() ? GetUserStatuses().Select(x => x.UserId).ToList() : new List<int>();
        }

        public List<int> GetOnlineIds(List<int> containtsIds)
        {
            return containtsIds.Where(x => GetOnlineIds().Contains(x)).ToList();
        }
    }
}
