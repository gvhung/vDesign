using Base.Ambient;
using Base.DAL;
using Base.Security;
using Base.Service;
using Base.Task.Entities;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Task.Services
{
    public class TaskService : BaseCategorizedItemService<Entities.Task>, ITaskService
    {
        private readonly ICacheWrapper _cacheWrapper;
        private const string cacheKey = "DAF73082-D9AA-45B4-9BF0-27AFEA32B499";
        private static object cacheLock = new object();

        public TaskService(IBaseObjectServiceFacade facade, ICacheWrapper cacheWrapper)
            : base(facade)
        {
            _cacheWrapper = cacheWrapper;
        }

        private string GetKeyCache(int userID)
        {
            return String.Format("{0}-{1}", cacheKey, userID);
        }

        public object GetUserCache(int userID, string key, Func<object> value)
        {
            string userKey = GetKeyCache(userID);

            if (_cacheWrapper[userKey] == null)
            {
                lock (cacheLock)
                {
                    if (_cacheWrapper[userKey] == null)
                        _cacheWrapper[userKey] = new Dictionary<string, object>();
                }
            }

            var cacheUser = _cacheWrapper[userKey] as Dictionary<string, object>;

            if (cacheUser.ContainsKey(key))
                return cacheUser[key];

            lock (cacheLock)
            {
                if (!cacheUser.ContainsKey(key))
                    if (cacheUser.ContainsKey(key)) return cacheUser[key];

                cacheUser.Add(key, value());
            }

            return cacheUser[key];
        }

        public void RemoveUserCache(int userID)
        {
            lock (cacheLock)
            {
                _cacheWrapper.Remove(GetKeyCache(userID));
            }
        }

        public void AddItemToChangeHistory(Entities.Task task, string comment = null)
        {
            AddItemToChangeHistory(task, task.Status, comment);
        }

        public void AddItemToChangeHistory(Entities.Task task, TaskStatus newStatus, string comment = null)
        {
            if (task.TaskChangeHistory == null)
                task.TaskChangeHistory = new List<TaskChangeHistory>();

            task.TaskChangeHistory.Add(new TaskChangeHistory()
            {
                Date = DateTime.Now,
                UserID = AppContext.SecurityUser.ID,
                Status = newStatus,
                Сomment = comment
            });
        }

        public override IQueryable<Entities.Task> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden)
        {
            var strId = HCategory.IdToString(categoryID);

            return GetAll(unitOfWork, hidden)
                .Where(a => (a.TaskCategory.sys_all_parents != null && a.TaskCategory.sys_all_parents.Contains(strId)) || a.TaskCategory.ID == categoryID);
        }

        public override Entities.Task Get(IUnitOfWork unitOfWork, int id)
        {
            var task = base.Get(unitOfWork, id);

            if (task == null || task.Status != TaskStatus.New || task.AssignedToID != AppContext.SecurityUser.ID)
                return task;

            task.Status = TaskStatus.Viewed;

            AddItemToChangeHistory(task);

            return Update(unitOfWork, task);
        }

        public override Entities.Task Create(IUnitOfWork unitOfWork, Entities.Task obj)
        {
            if (obj.AssignedTo != null)
                RemoveUserCache(obj.AssignedTo.ID);

            return base.Create(unitOfWork, obj);
        }

        public override Entities.Task Update(IUnitOfWork unitOfWork, Entities.Task obj)
        {
            if (obj.AssignedTo != null)
                RemoveUserCache(obj.AssignedTo.ID);

            return base.Update(unitOfWork, obj);
        }

        public override void Delete(IUnitOfWork unitOfWork, Entities.Task obj)
        {
            if (obj.AssignedTo != null)
                RemoveUserCache(obj.AssignedTo.ID);

            base.Delete(unitOfWork, obj);
        }

        protected override IObjectSaver<Entities.Task> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Entities.Task> objectSaver)
        {
            if (objectSaver.Dest.Status != objectSaver.Src.Status && (objectSaver.Src.Status == TaskStatus.Complete || objectSaver.Src.Status == TaskStatus.NotRelevant))
            {
                objectSaver.Dest.CompliteDate = DateTime.Now;
            }

            if (objectSaver.IsNew)
                AddItemToChangeHistory(objectSaver.Dest, objectSaver.Src.Status);

            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.AssignedFrom)
                .SaveOneObject(x => x.AssignedTo)
                .SaveOneToMany(x => x.Files);

            //NOTE: для TaskChangeHistory спец. не вызываем SaveOneToMany
        }

        public override Entities.Task CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            var dtm = DateTime.Now;
            dtm = dtm.AddMilliseconds(-dtm.Millisecond);

            return new Entities.Task
            {
                AssignedFrom = unitOfWork.GetRepository<User>().Find(u => u.ID == AppContext.SecurityUser.ID),
                Period = new Base.Entities.Complex.Period() { Start = dtm, End = dtm.AddMinutes(15) }
            };
        }
    }
}
