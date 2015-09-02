using Base.DAL;
using Base.Events;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Base.Security.ObjectAccess.Services
{
    public class ObjectAccessItemService : IObjectAccessItemService
    {
        public virtual void OnBeforeSave(IUnitOfWork unitOfWork, ObjectAccessItem src, ObjectAccessItem dest)
        {
            if (src.UserCategories != null)
            {
                src.UserCategories = src.UserCategories.GroupBy(x => x.UserCategoryID).Select(x =>
                {
                    var category = x.FirstOrDefault(c => c.UserCategoryID == x.Key);

                    if (category == null) return null;

                    category.Read = x.Any(c => c.Read);

                    category.Update = x.Any(c => c.Update);

                    category.Delete = x.Any(c => c.Delete);

                    return category;
                }).ToList();
            }

            if (src.Users != null)
            {
                src.Users = src.Users.GroupBy(x => x.UserID).Select(x =>
                {
                    var category = x.FirstOrDefault(c => c.UserID == x.Key);

                    if (category == null) return null;

                    category.Read = x.Any(c => c.Read);

                    category.Update = x.Any(c => c.Update);

                    category.Delete = x.Any(c => c.Delete);

                    return category;
                }).ToList();
            }
        }

        protected virtual IObjectSaver<ObjectAccessItem> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ObjectAccessItem> objectSaver)
        {
            return objectSaver
                .SaveOneObject(x => x.Creator)
                .SaveOneToMany(x => x.UserCategories, x => x.SaveOneObject(a => a.UserCategory))
                .SaveOneToMany(x => x.Users, x => x.SaveOneObject(a => a.User));
        }

        public virtual IQueryable<ObjectAccessItem> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            IQueryable<ObjectAccessItem> q = unitOfWork.GetRepository<ObjectAccessItem>().All();

            if (hidden != null)
            {
                if ((bool)hidden)
                    q = q.Where(a => a.Hidden == true);
                else
                    q = q.Where(a => a.Hidden == false);
            }

            var eventHandler = Volatile.Read(ref this.OnGetAll);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs() { Type = TypeEvent.OnGetAll, UnitOfWork = unitOfWork });
            }

            return q;
        }

        public virtual ObjectAccessItem Get(IUnitOfWork unitOfWork, int id)
        {
            var obj = this.GetAll(unitOfWork).FirstOrDefault(m => m.ID == id);

            var eventHandler = Volatile.Read(ref this.OnGet);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs
                {
                    Type = TypeEvent.OnGet,
                    Object = obj,
                    UnitOfWork = unitOfWork
                });
            }

            return obj;
        }

        protected virtual void InitSortOrder(IUnitOfWork unitOfWork, ObjectAccessItem obj)
        {
            if (obj.SortOrder == -1)
            {
                obj.SortOrder = (this.GetAll(unitOfWork).Max(m => (int?)m.SortOrder) ?? 0) + 1;
            }
        }

        public virtual ObjectAccessItem Create(IUnitOfWork unitOfWork, ObjectAccessItem obj)
        {

            this.InitSortOrder(unitOfWork, obj);

            this.OnBeforeSave(unitOfWork, obj, null);

            var saver = this.GetForSave(unitOfWork, unitOfWork.GetObjectSaver(obj, null));

            unitOfWork.GetRepository<ObjectAccessItem>().Create(saver.Dest);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnCreate);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnCreate,
                    Object = saver.Dest,
                    UnitOfWork = unitOfWork
                });
            }

            return saver.Dest;
        }

        public virtual ObjectAccessItem Update(IUnitOfWork unitOfWork, ObjectAccessItem obj)
        {
            var repository = unitOfWork.GetRepository<ObjectAccessItem>();

            var objDest = repository.Find(obj.ID);

            this.OnBeforeSave(unitOfWork, obj, objDest);

            var saver = this.GetForSave(unitOfWork, unitOfWork.GetObjectSaver(obj, objDest));

            repository.Update(saver.Dest);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnUpdate);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnUpdate,
                    Object = saver.Dest,
                    UnitOfWork = unitOfWork
                });
            }

            return saver.Dest;
        }

        public virtual void Delete(IUnitOfWork unitOfWork, ObjectAccessItem obj)
        {
            var repository = unitOfWork.GetRepository<ObjectAccessItem>();

            obj.Hidden = true;

            repository.Update(obj);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnDelete);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnDelete,
                    Object = obj,
                    UnitOfWork = unitOfWork
                });
            }
        }

        public virtual void ChangeSortOrder(IUnitOfWork unitOfWork, ObjectAccessItem obj, int newOrder)
        {
        }

        public virtual ObjectAccessItem CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return null;
        }

        #region CRUD
        IQueryable<BaseObject> IBaseObjectCRUDService.GetAll(IUnitOfWork unitOfWork, bool? hidden)
        {
            return this.GetAll(unitOfWork, hidden);
        }

        BaseObject IBaseObjectCRUDService.Get(IUnitOfWork unitOfWork, int id)
        {
            return this.Get(unitOfWork, id);
        }

        BaseObject IBaseObjectCRUDService.Create(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return this.Create(unitOfWork, obj as ObjectAccessItem);
        }

        BaseObject IBaseObjectCRUDService.Update(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return this.Update(unitOfWork, obj as ObjectAccessItem);
        }

        void IBaseObjectCRUDService.Delete(IUnitOfWork unitOfWork, BaseObject obj)
        {
            this.Delete(unitOfWork, obj as ObjectAccessItem);
        }

        void IBaseObjectCRUDService.ChangeSortOrder(IUnitOfWork unitOfWork, BaseObject obj, int newSortOrder)
        {
            this.ChangeSortOrder(unitOfWork, obj as ObjectAccessItem, newSortOrder);
        }

        BaseObject IBaseObjectCRUDService.CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return this.CreateOnGroundsOf(unitOfWork, obj);
        }
        #endregion CRUD


        public virtual event EventHandler<BaseObjectEventArgs> OnGetAll;

        public virtual event EventHandler<BaseObjectEventArgs> OnGet;

        public virtual event EventHandler<BaseObjectEventArgs> OnCreate;

        public virtual event EventHandler<BaseObjectEventArgs> OnUpdate;

        public virtual event EventHandler<BaseObjectEventArgs> OnDelete;

        public virtual event EventHandler<BaseObjectEventArgs> OnChangeSortOrder;

        public virtual event EventHandler<BaseObjectEventArgs> OnCreateOnGroundsOf;


        public IList<ObjectAccessItem> CreateCollection(IUnitOfWork unitOfWork, IEnumerable<ObjectAccessItem> collection)
        {
            throw new NotImplementedException();
        }

        public IList<ObjectAccessItem> UpdateCollection(IUnitOfWork unitOfWork, IEnumerable<ObjectAccessItem> collection)
        {
            throw new NotImplementedException();
        }

        public void DeleteCollection(IUnitOfWork unitOfWork, IEnumerable<ObjectAccessItem> collection)
        {
            throw new NotImplementedException();
        }
    }
}
