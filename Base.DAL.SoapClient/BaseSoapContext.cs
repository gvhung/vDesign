using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{
    public abstract class BaseSoapContext : IBaseContext
    {
        readonly IBaseWebServerQueryContext _context;

        public BaseSoapContext(IBaseWebServerQueryContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void CancelChanges()
        {
            foreach (WebEntityEntry entry in this._context.Entries())
            {
                switch (entry.State)
                {
                    case BaseEntityState.Modified:
                        entry.State = BaseEntityState.Unchanged;
                        break;
                    case BaseEntityState.Added:
                        entry.State = BaseEntityState.Detached;
                        break;
                    default: break;
                }
            }
        }

        public IBaseContextSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return new BaseContextSet<TEntity>(_context);
        }

        public void ChangeState<TEntity>(TEntity entity, BaseEntityState entityState) where TEntity : class
        {
            _context.Entry<TEntity>(entity).State = entityState;
        }

        public Dictionary<TEntity, BaseEntityState> GetModifiedEntities<TEntity>(bool recursive = true) where TEntity : class
        {
            return _context.Entries().Where(m => m.State != BaseEntityState.Detached || m.State != BaseEntityState.Unchanged).ToDictionary(m => m.Entity as TEntity, m => m.State);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class BaseContextSet<TEntity> : QueryableWebServerData<TEntity>, IBaseContextSet<TEntity> where TEntity : class
    {
        private readonly IBaseWebServerQueryContext _context;

        public BaseContextSet(IBaseWebServerQueryContext context)
            : base(context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            _context.Attach<TEntity>(entity);
            _context.Entry<TEntity>(entity).State = BaseEntityState.Added;

            return entity;
        }

        public TEntity Attach(TEntity entity)
        {
            return _context.Attach<TEntity>(entity);
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            throw new NotImplementedException();
        }

        public TEntity Create()
        {
            throw new NotImplementedException();
        }

        public TEntity Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public TEntity Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }


        public Task<TEntity> FindAsync(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public new System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
