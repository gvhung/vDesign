using System;
using System.Collections.Generic;

namespace Base.Security.Service.Abstract
{
    public interface IUserStatusService
    {
        Dictionary<int, IUserStatus> UserStatuses { get; set; }

        IUserStatus GetUserStatus(int userId);
        IUserStatus GetUserStatus(string connectionId);
        IUserStatus SetOnline(int userId, string connectionId);
        IUserStatus SetOnline(IUserStatus userStatus);
        IUserStatus SetOffline(int userId);
        IUserStatus SetOffline(IUserStatus userStatus);
        List<IUserStatus> GetUserStatuses(bool online = true);
        List<int> GetOnlineIds();
        List<int> GetOnlineIds(List<int> containtsIds);
    }
}