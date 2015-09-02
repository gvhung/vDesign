using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public UnitOfWork(IRepositoryFactory repositoryFactory)
        {
            ID = Guid.NewGuid();
            _repositoryFactory = repositoryFactory;
        }

        public Guid ID { get; private set; }

        protected IBaseContext GetContext<TObject>() where TObject : BaseObject
        {
            return _repositoryFactory.ContextOf<TObject>();
        }

        protected IReadOnlyList<IBaseContext> GetContexts()
        {
            return _repositoryFactory.GetContexts();
        }

        public int SaveChanges()
        {
            int i = 0;

            foreach (var context in this.GetContexts())
            {
                i = i + context.SaveChanges();
            }

            return i;
        }

        public async Task<int> SaveChangesAsync()
        {
            int i = 0;

            foreach (var context in this.GetContexts())
            {
                i = i + await context.SaveChangesAsync();
            }

            return i;
        }

        public virtual IObjectSaver<TObject> GetObjectSaver<TObject>(TObject objSrc, TObject objDest) where TObject : BaseObject
        {
            var repository = GetRepository<TObject>();

            return repository.GetObjectSaver(this, objSrc, objDest);
        }

        public virtual void Dispose()
        {
            _repositoryFactory.Dispose();
        }

        public virtual IRepository<TObject> GetRepository<TObject>() where TObject : BaseObject
        {
            return _repositoryFactory.RepositoryOf<TObject>();
        }


        public Dictionary<TEntity, BaseEntityState> GetModifiedEntities<TEntity>(bool recursive = true) where TEntity : BaseObject
        {
            var contex = this.GetContext<TEntity>();

            return contex != null ? contex.GetModifiedEntities<TEntity>(recursive) : new Dictionary<TEntity, BaseEntityState>();
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }


        public bool IsModifiedEntity<TEntity>(TEntity entity, BaseEntityState modif) where TEntity : BaseObject
        {
            var contex = this.GetContext<TEntity>();

            return contex != null && contex.IsModifiedEntity(entity, modif);
        }
    }
}
