using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.DAL
{
    public interface IBaseContext: IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void ChangeState<TEntity>(TEntity entity, BaseEntityState entityState) where TEntity : class;
        bool IsModifiedEntity<TEntity>(TEntity entity, BaseEntityState modif) where TEntity : BaseObject;
        Dictionary<TEntity, BaseEntityState> GetModifiedEntities<TEntity>(bool recursive = true) where TEntity : class;
        bool Transaction { get; }
        IDisposable BeginTransaction();
        void Commit();
        void Rollback();
    }

    public interface IDbTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }

    public enum BaseEntityState
    {
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16
    }
}
