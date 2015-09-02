using Base.DAL;
using Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Service
{
    public interface IBaseObjectService<T> : IBaseObjectCRUDService
        where T : BaseObject
    {
        IQueryable<T> GetAll(IUnitOfWork unitOfWork, bool? hidden = false);
        T Get(IUnitOfWork unitOfWork, int id);
        T Create(IUnitOfWork unitOfWork, T obj);
        IList<T> CreateCollection(IUnitOfWork unitOfWork, IEnumerable<T> collection);
        T Update(IUnitOfWork unitOfWork, T obj);
        IList<T> UpdateCollection(IUnitOfWork unitOfWork, IEnumerable<T> collection);
        void Delete(IUnitOfWork unitOfWork, T obj);
        void DeleteCollection(IUnitOfWork unitOfWork, IEnumerable<T> collection);
        void ChangeSortOrder(IUnitOfWork unitOfWork, T obj, int newSortOrder);
        T CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj);
    }

    public interface IBaseObjectCRUDService : IService
    {
        IQueryable<BaseObject> GetAll(IUnitOfWork unitOfWork, bool? hidden = false);
        
        event EventHandler<BaseObjectEventArgs> OnGetAll;

        BaseObject Get(IUnitOfWork unitOfWork, int id);
        event EventHandler<BaseObjectEventArgs> OnGet;

        BaseObject Create(IUnitOfWork unitOfWork, BaseObject obj);
        event EventHandler<BaseObjectEventArgs> OnCreate;

        BaseObject Update(IUnitOfWork unitOfWork, BaseObject obj);
        event EventHandler<BaseObjectEventArgs> OnUpdate;

        void Delete(IUnitOfWork unitOfWork, BaseObject obj);
        event EventHandler<BaseObjectEventArgs> OnDelete;

        void ChangeSortOrder(IUnitOfWork unitOfWork, BaseObject obj, int newSortOrder);
        event EventHandler<BaseObjectEventArgs> OnChangeSortOrder;

        BaseObject CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj);
        event EventHandler<BaseObjectEventArgs> OnCreateOnGroundsOf;
    }
}
