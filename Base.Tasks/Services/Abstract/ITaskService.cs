using Base.Service;
using Base.Task.Entities;
using System;

namespace Base.Task.Services
{
    public interface ITaskService : IBaseCategorizedItemService<Entities.Task>
    {
        object GetUserCache(int userID, string key, Func<object> value);
        void RemoveUserCache(int userID);

        void AddItemToChangeHistory(Entities.Task task, string comment = null);

        void AddItemToChangeHistory(Entities.Task task, TaskStatus newStatus, string comment = null);
    }
}
