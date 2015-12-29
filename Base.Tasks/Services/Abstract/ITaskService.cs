using Base.Service;
using Base.Task.Entities;
using System;

namespace Base.Task.Services
{
    public interface ITaskService : IBaseCategorizedItemService<Entities.Task>
    {
        bool HasUserCache(int userID, string key);
        object GetUserCache(int userID, string key, Func<object> value);
        object GetUserCache(int userID, string key, object value);
        object GetUserCache(int userID, string key);
        void RemoveUserCache(int userID);

        void AddItemToChangeHistory(Entities.Task task, string comment = null);

        void AddItemToChangeHistory(Entities.Task task, TaskStatus newStatus, string comment = null);
    }
}
