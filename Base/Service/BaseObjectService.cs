using Base.Ambient;
using Base.DAL;
using Base.Events;
using Base.Security;
using Base.Security.ObjectAccess;
using Base.Security.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Base.Service
{
    public abstract class BaseObjectService<T> : IBaseObjectService<T>
        where T : BaseObject
    {
        private readonly IBaseObjectServiceFacade _facade;
        
        protected BaseObjectService(IBaseObjectServiceFacade facade)
        {
            _facade = facade;
        }

        protected ISecurityService SecurityService
        {
            get
            {
                return _facade.SecurityService;
            }
        }

        protected IUnitOfWorkFactory UnitOfWorkFactory
        {
            get
            {
                return _facade.UnitOfWorkFactory;
            }
        }

        public virtual IQueryable<T> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            IQueryable<T> q = unitOfWork.GetRepository<T>().All();

            q = SecurityService.FilterByAccess(q, unitOfWork);

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

            return q.OrderBy(m => m.SortOrder);
        }

        public virtual T Get(IUnitOfWork unitOfWork, int id)
        {
            var obj = this.GetAll(unitOfWork).FirstOrDefault(m => m.ID == id);

            var eventHandler = Volatile.Read(ref this.OnGet);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnGet,
                    Object = obj,
                    UnitOfWork = unitOfWork
                });
            }

            return obj;
        }

        protected virtual IObjectSaver<T> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<T> objectSaver)
        {
            return objectSaver;
        }

        protected virtual void InitSortOrder(T obj)
        {   
            if (obj.SortOrder != -1) return;

            obj.SortOrder = GetMaxSortOrder() + 1;
        }

        protected  virtual int GetMaxSortOrder()
        {
            using (var uofw = UnitOfWorkFactory.CreateSystem())
            {
                return uofw.GetRepository<T>().All().Max(m => (int?)m.SortOrder) ?? 0;
            }
        }

        public virtual T Create(IUnitOfWork unitOfWork, T obj)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), TypePermission.Create);

            this.InitSortOrder(obj);

            var objectSaver = this.GetForSave(unitOfWork, unitOfWork.GetObjectSaver(obj, null));

            unitOfWork.GetRepository<T>().Create(objectSaver.Dest);
            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnCreate);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnCreate,
                    Object = objectSaver.Dest,
                    ObjectSrc = objectSaver.Dest,
                    UnitOfWork = unitOfWork
                });
            }

            return objectSaver.Dest;
        }

        public virtual IList<T> CreateCollection(IUnitOfWork unitOfWork, IEnumerable<T> collection)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), TypePermission.Create);

            int sortOrder = GetMaxSortOrder() + 1;

            var res = new List<T>();

            foreach (var obj in collection)
            {
                if (obj.SortOrder == -1)
                    obj.SortOrder = sortOrder++;

                var objectSaver = this.GetForSave(unitOfWork, unitOfWork.GetObjectSaver(obj, null));

                unitOfWork.GetRepository<T>().Create(objectSaver.Dest);

                res.Add(objectSaver.Dest);
            }

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnCreate);

            if (eventHandler != null)
            {
                foreach (var obj in res)
                {
                    eventHandler(this, new BaseObjectEventArgs()
                    {
                        Type = TypeEvent.OnCreate,
                        Object = obj,
                        ObjectSrc = obj,
                        UnitOfWork = unitOfWork
                    });
                }
            }

            return res;
        }

        public virtual T Update(IUnitOfWork unitOfWork, T obj)
        {
            if (obj.ID == 0) return this.Create(unitOfWork, obj);

            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), obj.ID, TypePermission.Write, AccessType.Update);

            var objectSaver = this.GetForSave(unitOfWork, unitOfWork.GetObjectSaver(obj, null));

            unitOfWork.GetRepository<T>().Update(objectSaver.Dest);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnUpdate);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnUpdate,
                    Object = objectSaver.Dest,
                    ObjectSrc = obj,
                    UnitOfWork = unitOfWork
                });
            }

            return objectSaver.Dest;
        }

        public virtual IList<T> UpdateCollection(IUnitOfWork unitOfWork, IEnumerable<T> collection)
        {
            var res = new List<IObjectSaver<T>>();

            int sortOrder = -1;

            foreach (var obj in collection)
            {
                var objectSaver = this.GetForSave(unitOfWork, unitOfWork.GetObjectSaver(obj, null));

                if (objectSaver.IsNew)
                {
                    SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), TypePermission.Create);

                    if (objectSaver.Dest.SortOrder == -1)
                    {
                        if (sortOrder == -1)
                            sortOrder = GetMaxSortOrder() + 1;

                        objectSaver.Dest.SortOrder = sortOrder++;
                    }

                    unitOfWork.GetRepository<T>().Create(objectSaver.Dest);
                }
                else
                {
                    SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), obj.ID, TypePermission.Write, AccessType.Update);

                    unitOfWork.GetRepository<T>().Update(objectSaver.Dest);
                }

                res.Add(objectSaver);
            }

            unitOfWork.SaveChanges();

            var onCreateEventHandler = Volatile.Read(ref this.OnCreate);
            var onUpdateEventHandler = Volatile.Read(ref this.OnUpdate);

            if (onCreateEventHandler != null && onUpdateEventHandler != null)
            {
                foreach (var objectSaver in res)
                {
                    if (objectSaver.IsNew)
                    {
                        onCreateEventHandler(this, new BaseObjectEventArgs()
                        {
                            Type = TypeEvent.OnCreate,
                            Object = objectSaver.Dest,
                            ObjectSrc = objectSaver.Src,
                            UnitOfWork = unitOfWork
                        });
                    }
                    else
                    {
                        onUpdateEventHandler(this, new BaseObjectEventArgs()
                        {
                            Type = TypeEvent.OnUpdate,
                            Object = objectSaver.Dest,
                            ObjectSrc = objectSaver.Src,
                            UnitOfWork = unitOfWork
                        });
                    }
                }
            }

            return res.Select(x => x.Dest).ToList();
        }

        public virtual void Delete(IUnitOfWork unitOfWork, T obj)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), obj.ID, TypePermission.Delete, AccessType.Delete);

            obj.Hidden = true;

            unitOfWork.GetRepository<T>().Update(obj);
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

        public virtual void DeleteCollection(IUnitOfWork unitOfWork, IEnumerable<T> collection)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), TypePermission.Delete);

            foreach (var obj in collection)
            {
                SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), obj.ID, AccessType.Delete);

                obj.Hidden = true;
                unitOfWork.GetRepository<T>().Update(obj);
            }

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnDelete);

            if (eventHandler != null)
            {
                foreach (var obj in collection)
                {
                    eventHandler(this, new BaseObjectEventArgs()
                    {
                        Type = TypeEvent.OnDelete,
                        Object = obj,
                        UnitOfWork = unitOfWork
                    });
                }
            }
        }

        public virtual void ChangeSortOrder(IUnitOfWork unitOfWork, T obj, int newOrder)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(T), TypePermission.Write);

            int oldOrder = obj.SortOrder;

            var sortObj = unitOfWork.GetRepository<T>().All().FirstOrDefault(x => x.ID == obj.ID);

            if (sortObj != null && oldOrder != newOrder)
            {
                int max, min;
                string orderby;

                if (newOrder > oldOrder)
                {
                    min = oldOrder;
                    max = newOrder;
                    orderby = "desc";
                }
                else
                {
                    min = newOrder;
                    max = oldOrder;
                    orderby = "asc";
                }

                var q = unitOfWork.GetRepository<T>().All()
                        .Where(m => m.ID != obj.ID)
                        .Where(m => m.SortOrder >= min && m.SortOrder <= max);

                int sort = 0;

                if (orderby == "asc")
                {
                    q = q.OrderBy(m => m.SortOrder);
                    sort = min;
                }
                else
                {
                    q = q.OrderByDescending(m => m.SortOrder);
                    sort = max;
                }

                foreach (var s in q.ToList())
                {
                    if (orderby == "asc")
                        sort++;
                    else
                        sort--;

                    s.SortOrder = sort;

                    unitOfWork.GetRepository<T>().Update(s);
                }

                sortObj.SortOrder = newOrder;

                unitOfWork.GetRepository<T>().Update(sortObj);
                unitOfWork.SaveChanges();

                var eventHandler = Volatile.Read(ref this.OnChangeSortOrder);

                if (eventHandler != null)
                {
                    eventHandler(this, new BaseObjectEventArgs()
                    {
                        Type = TypeEvent.OnChangeSortOrder,
                        Object = obj,
                        UnitOfWork = unitOfWork
                    });
                }
            }
        }

        public virtual T CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            if (obj != null)
            {
                var destObj = obj.ToObject(typeof(T), BaseObject.GetSystemProperties().Select(p => p.Name).ToArray()) as T;

                this.BeforeCreateOnGroundsOf(unitOfWork, obj, destObj);

                return destObj;
            }
            else
            {
                if (AppContext.SecurityUser.IsPermission<T>(TypePermission.Read))
                {
                    return Activator.CreateInstance<T>();
                }
            }

            throw new Exception("Не удалось создать объект на основании");
        }

        protected virtual void BeforeCreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject srcObj, BaseObject destObj)
        {
            return;
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
            return this.Create(unitOfWork, obj as T);
        }

        BaseObject IBaseObjectCRUDService.Update(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return this.Update(unitOfWork, obj as T);
        }

        void IBaseObjectCRUDService.Delete(IUnitOfWork unitOfWork, BaseObject obj)
        {
            this.Delete(unitOfWork, obj as T);
        }

        void IBaseObjectCRUDService.ChangeSortOrder(IUnitOfWork unitOfWork, BaseObject obj, int newSortOrder)
        {
            this.ChangeSortOrder(unitOfWork, obj as T, newSortOrder);
        }

        BaseObject IBaseObjectCRUDService.CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return this.CreateOnGroundsOf(unitOfWork, obj) as BaseObject;
        }
        #endregion CRUD


        public virtual event EventHandler<BaseObjectEventArgs> OnGetAll;

        public virtual event EventHandler<BaseObjectEventArgs> OnGet;

        public virtual event EventHandler<BaseObjectEventArgs> OnCreate;

        public virtual event EventHandler<BaseObjectEventArgs> OnUpdate;

        public virtual event EventHandler<BaseObjectEventArgs> OnDelete;

        public virtual event EventHandler<BaseObjectEventArgs> OnChangeSortOrder;

        public virtual event EventHandler<BaseObjectEventArgs> OnCreateOnGroundsOf;
    }
}
