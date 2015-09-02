using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        Guid ID { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IObjectSaver<TEntity> GetObjectSaver<TEntity>(TEntity objSrc, TEntity objDest) where TEntity : BaseObject;
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseObject;
        bool IsModifiedEntity<TEntity>(TEntity entity, BaseEntityState modif) where TEntity : BaseObject;
        Dictionary<TEntity, BaseEntityState> GetModifiedEntities<TEntity>(bool recursive = true) where TEntity : BaseObject;
    }

    public interface ISystemUnitOfWork : IUnitOfWork
    {
        
    }
}
